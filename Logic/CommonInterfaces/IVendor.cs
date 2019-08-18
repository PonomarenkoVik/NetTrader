using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public interface IVendor
    {
        string Name { get; }
        int Id { get; }
        Task<Dictionary<string, IInstrument>> GetInstrumentsAsync();
        Task<IResult> SendCommandAsync(ICommand command);
        Task<List<Order>> GetLevel2Async(IInstrument instrument, int sourceType);
        Task<List<Order>> GetOwnOrdersAsync(IAccount account, IInstrument instrument = null);
        Task<List<IAsset>> GetAssetsAsync(IAccount account);
    }
}
