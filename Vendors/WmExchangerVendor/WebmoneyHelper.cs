using CommonInterfaces;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WmExchangerVendor
{

    internal class WebmoneyHelper
    {
        internal static List<Order> CreateOrdersByPage(IInstrument instr, string page, PageType pageType)
        {
            return pageType == PageType.Web ? CreateOrdersByWebPage(instr, page) : CreateOrdersByXMLPage(instr, page);
        }


        private static List<Order> CreateOrdersByWebPage(IInstrument instr, string page)
        {
            List<Order> orders = new List<Order>();
            string[] orderlines = page.Split(OrderSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in orderlines)
            {
                var order = CreateOrderByWebLine(instr, line);
                if (order == null)
                    continue;

                orders.Add(order);
            }
            return orders;
        }


        private static Order CreateOrderByWebLine(IInstrument instr, string orderLine)
        {
            string orderId = String.Empty;
            string instrumentName = String.Empty;
            double straightCrossRate = double.NaN;
            double reverseCrossRate = double.NaN;
            DateTime applicationDate = DateTime.MinValue;
            double sum1 = double.NaN;
            double sum2 = double.NaN;

            List<string> orderPointlines = GetWebPageOrderPoints(orderLine);
            if (orderPointlines == null || orderPointlines.Count < OrderPointNumber)
                return null;
            orderId = orderPointlines[0];

            instrumentName = orderPointlines[1];
            if (!double.TryParse(orderPointlines[2], out reverseCrossRate))
                return null;

            bool normalFormat = orderPointlines[5].Length == 19;
            string date = orderPointlines[normalFormat ? 5 : 8].Substring(0, 19);
            if (!DateTime.TryParseExact(date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out applicationDate))
                return null;
            if (!double.TryParse(orderPointlines[6], out sum1))
                return null;
            if (!double.TryParse(orderPointlines[7], out sum2))
                return null;
            if (!double.TryParse(orderPointlines[normalFormat ? 8 : 5], out straightCrossRate))
                return null;



            return new Order(orderId, applicationDate, instr, sum1, sum2, straightCrossRate, reverseCrossRate, false, null);
        }


        private static List<string> GetWebPageOrderPoints(string line)
        {
            List<string> orderPointlines = new List<string>();
            if (line.Length < OrderSymbolNumber)
                return null;

            var num = line.Substring(0, OrderSymbolNumber);
            if (!int.TryParse(num, out int number))
                return null;

            orderPointlines.Add(num);

            string orderline = line.Substring(OrderSymbolNumber, line.Length - OrderSymbolNumber);
            var tempStrings = orderline.Split(OrderPointSeparator, StringSplitOptions.RemoveEmptyEntries);

            string[] firstStringSplit =
                tempStrings[0].Split(FirstStringSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var orderIndex in OrderIndexes)
            {
                if (orderIndex < firstStringSplit.Length)
                    orderPointlines.Add(firstStringSplit[orderIndex]);
            }

            foreach (var tempString in tempStrings)
            {
                bool isPointOrder = true;
                foreach (var exceptString in OrderExceptString)
                {
                    if (tempString.Contains(exceptString))
                    {
                        isPointOrder = false;
                        break;
                    }
                }

                if (isPointOrder)
                    orderPointlines.Add(tempString);
            }

            return orderPointlines;
        }

        private static List<Order> CreateOrdersByXMLPage(IInstrument instr, string xmlPage)
        {
            if (instr == null || string.IsNullOrEmpty(xmlPage))
                return null;
            xmlPage = xmlPage.Trim();
            var tempBankRate = xmlPage.Split(BankRateSeparators, StringSplitOptions.RemoveEmptyEntries);
            var stringBankRate = tempBankRate[1].Split(BankRatePointSeparators, StringSplitOptions.RemoveEmptyEntries)[1];
            double.TryParse(stringBankRate, out double bankRate);
            instr.BankRate = bankRate;

            var orderLines = xmlPage.Split(OrderXMLSepatators, StringSplitOptions.RemoveEmptyEntries);

            var orders = new List<Order>();
            for (int i = 1; i < orderLines.Length; i++)
            {
                var order = CreateOrderByXMLLine(instr, orderLines[i]);
                if (order == null)
                    continue;

                orders.Add(order);
            }
            return orders;
        }

        private static Order CreateOrderByXMLLine(IInstrument instr, string orderLine)
        {
            string pointLine;
            string orderId = GetOrderPointFromXMLString(orderLine, idSeparator);
            double.TryParse(GetOrderPointFromXMLString(orderLine, amountinSeparator), out double sum1);
            double.TryParse(GetOrderPointFromXMLString(orderLine, amountoutSeparator), out double sum2);
            double.TryParse(GetOrderPointFromXMLString(orderLine, inoutrateSeparator), out double straightRate);
            double.TryParse(GetOrderPointFromXMLString(orderLine, outinrateSeparator), out double reverseRate);
            var percentBankRate = ("Percent Bank Rate", GetOrderPointFromXMLString(orderLine, percentbankrateSeparator));
            double.TryParse(GetOrderPointFromXMLString(orderLine, allamountinSeparator), out double allSum);
            string date = GetOrderPointFromXMLString(orderLine, querydateSeparator);
            DateTime.TryParseExact(date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out DateTime applicationDate);
            bool isOwn = false;
            return new Order(orderId, applicationDate, instr, sum1, sum2, straightRate, reverseRate, isOwn, new List<(string, string)>() { percentBankRate });
        }

        private static string GetOrderPointFromXMLString(string line, string key)
        {
            var separator = new[] { "\"" };
            var tempString = line.Split(new[] { key }, StringSplitOptions.RemoveEmptyEntries)[1];
            var result = tempString.Split(separator, StringSplitOptions.RemoveEmptyEntries)[0];
            return result;
        }

        public static List<Order> CombineOrders(IInstrument instr, List<Order> ordersFromWeb, List<Order> ordersFromXML)
        {
            return null;
        }


        public static Dictionary<string, IInstrument> CreateInstruments(string page, IVendor vendor)
        {
            var instruments = new Dictionary<string, IInstrument>();
            var instrList = page.Split(instrSeparators, StringSplitOptions.RemoveEmptyEntries);
            instrList = instrList.Where((i) => i.Length > 70).ToArray();
            for (int i = 0; i < instrList.Length; i++)
            {
                var instrument = CreateInstrument(instrList[i], vendor);
                instruments.Add(instrument.InstrumentName, instrument);
            }
            return instruments;
        }

        private static IInstrument CreateInstrument(string instr, IVendor vendor)
        {
            int pos = instr.IndexOf(instrPointSeparator);
            var tempLines = instr.Substring(0, pos).Trim().Split(instrNameSeparator);
            string currency1 = tempLines[0];
            string currency2 = tempLines[1];
            string instrName = currency1 + "/" + currency2;

            string instrId = instr.Split(exchTypeSeparators, StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", String.Empty);
            return new WebmoneyInstrument(instrName, instrId, vendor, currency1, currency2);
        }

        private static readonly byte OrderSymbolNumber = 8;
        private static readonly byte OrderPointNumber = 8;

        //web page separators
        private static readonly string[] OrderSeparator = new string[] { "title=\"#" };
        private static readonly string[] OrderPointSeparator = new string[] { "align='right'>", "</td>", "<span>", "</span>" };
        private static readonly char[] FirstStringSeparator = new[] { ' ', ';', ':' };
        private static readonly string[] OrderExceptString = new string[] { "<td", "<tr", "</tr", "&", "%", "div", "class" };
        private static readonly int[] OrderIndexes = new int[] { 0, 1, 3, 7 };
        //web page separators

        //XML page separators
        private static readonly string[] BankRateSeparators = new string[] { "<BankRate", "</BankRate>" };
        private static readonly string[] BankRatePointSeparators = new string[] { ">" };
        private static readonly string[] OrderXMLSepatators = new[] { "<query", "/>" };

        private static readonly string idSeparator = "id=\"";
        private static readonly string amountinSeparator = "amountin=\"";
        private static readonly string amountoutSeparator = "amountout=\"";
        private static readonly string inoutrateSeparator = "inoutrate=\"";
        private static readonly string outinrateSeparator = "outinrate=\"";
        private static readonly string percentbankrateSeparator = "procentbankrate=\"";
        private static readonly string allamountinSeparator = "allamountin=\"";
        private static readonly string querydateSeparator = "querydate=\"";
        //XML page separators

        //XML instrument separators
        private static readonly string[] instrSeparators = new[] { "Direct=\"", "/>" };
        private static readonly string[] exchTypeSeparators = new[] { "exchtype=\"", "/>" };
        private static readonly char instrPointSeparator = '\"';
        private static readonly char instrNameSeparator = '-';

        //XML instrument separators

    }
}
