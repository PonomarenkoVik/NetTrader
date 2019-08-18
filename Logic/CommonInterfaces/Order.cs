using CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public class Order
    {
        public Order(string orderId, DateTime applicationDate, IInstrument instrument, double sum1, double sum2, double straitCrossRate, double reveseCrossRate, bool isOwn, List<ValueTuple<string, string>> additionalColumn)
        {
            OrderId = orderId;
            ApplicationDate = applicationDate;
            Instrument = instrument;
            Sum1 = sum1;
            Sum2 = sum2;
            StraitCrossRate = straitCrossRate;
            ReveseCrossRate = reveseCrossRate;
            IsOwn = isOwn;
            AdditionalColumn = additionalColumn;
        }
        public string OrderId { get; }
        public DateTime ApplicationDate { get; }
        public IInstrument Instrument { get; }
        public double Sum1 { get; set; }
        public double Sum2 { get; }
        public double StraitCrossRate { get; }
        public double ReveseCrossRate { get; }
        public bool IsOwn { get; }
        public List<(string LabelName, string Value)> AdditionalColumn { get; }
    }
}
