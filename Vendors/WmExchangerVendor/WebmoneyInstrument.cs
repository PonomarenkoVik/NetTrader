using CommonInterfaces;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WmExchangerVendor
{
    class WebmoneyInstrument : IInstrument
    {
        public WebmoneyInstrument(string instrumentName, string instrumentId, IVendor vendor, string currency1, string currency2)
        {
            InstrumentId = instrumentId;
            Vendor = vendor;
            Currency1 = currency1;
            Currency2 = currency2;
            InstrumentName = instrumentName;
        }

        public string InstrumentId { get; }
        public double BankRate { get; set; } = double.NaN;
        public string Currency1 { get; }
        public string Currency2 { get; }
        public string InstrumentName { get; }
        public IVendor Vendor { get; }
        public async Task<IEnumerable<Order>> GetLevel2Async(int sourceType) => await Vendor?.GetLevel2Async(this, sourceType);

    }
}
