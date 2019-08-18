using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public interface IInstrument
    {
        string InstrumentId { get; }
        double BankRate { get; set; }
        string InstrumentName { get; }
        string Currency1 { get; }
        string Currency2 { get; }
        IVendor Vendor { get; }
        Task<IEnumerable<Order>> GetLevel2Async(int sourceType);
    }
}
