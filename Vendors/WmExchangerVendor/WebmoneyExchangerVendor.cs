using CommonInterfaces;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WmExchangerVendor
{
    public class WebmoneyExchangerVendor : IVendor
    {
        public string Name => "wm.exchanger.ru";

        public int Id => 1;

        public Task<List<IAsset>> GetAssetsAsync(IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, IInstrument>> GetInstrumentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetLevel2Async(IInstrument instrument, int sourceType)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOwnOrdersAsync(IAccount account, IInstrument instrument = null)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SendCommandAsync(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}
