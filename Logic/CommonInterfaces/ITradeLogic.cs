using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public interface ITradeLogic
    {    
        /// <summary>
        /// <vendor, List<InstrumentName>>
        /// </summary>
        Dictionary<string, List<string>> Instruments { get; }
        Task<IResult> SendCommandAsync(ICommand command);
        Task<IEnumerable<Order>> GetLevel2Async(string vendorName, string instrumentName, int sourceType);
        Task<IEnumerable<IAsset>> GetAssets(string accountName);
        IEnumerable<string> GetAccounts();
    }
}
