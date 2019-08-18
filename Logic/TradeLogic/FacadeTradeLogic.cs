using CommonInterfaces;
using CommonLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WmExchangerVendor;

namespace TradeLogic
{
    public class FacadeTradeLogic : ITradeLogic
    {
        public FacadeTradeLogic()
        {
            _ = Initialize();
        }



        public Dictionary<string, List<string>> Instruments
        {
            get
            {
                if (_instrumentsByVendors == null || _instrumentsByVendors.Count == 0)
                    return null;

                return new Dictionary<string, List<string>>(_instrByVendorStr);
            }
        }


        public IEnumerable<string> GetAccounts() => _accounts.Select((a) => a.Key).ToList();
      

        public async Task<IEnumerable<IAsset>> GetAssets(string accountName)
        {
            if (_accounts == null)           
                return null;
            
            if (_accounts.TryGetValue(accountName, out IAccount account))
                return await account.GetAssetsAsync();
                       
            return null;
        }

        public async Task<IEnumerable<Order>> GetLevel2Async(string vendorName, string instrumentName, int sourceType)
        {
            var instrument = GetInstrument(vendorName, instrumentName);
            return await instrument?.GetLevel2Async(sourceType);
        }

        public async Task<IResult> SendCommandAsync(ICommand command)
        {
            if (command == null)
                return null;

            var vendor = GetVendor(command);
            return await vendor.SendCommandAsync(command);
        }


        private IInstrument GetInstrument(string vendorName, string instrName)
        {
            if (_instrumentsByVendors == null || !_instrumentsByVendors.TryGetValue(vendorName, out Dictionary<string, IInstrument> instruments))
                return null;

            return !instruments.TryGetValue(instrName, out IInstrument instrument) ? null : instrument;
        }


        private IVendor GetVendor(ICommand command)
        {
            if (command == null || !string.IsNullOrEmpty(command.VendorName))
                return null;

            _vendors.TryGetValue(command.VendorName, out IVendor vendor);
            return vendor;
        }

        private async Task Initialize()
        {
            WebmoneyExchangerVendor wmVendor = new WebmoneyExchangerVendor();
            string key = "<RSAKeyValue><Modulus>pV4KSuF3Tb7KrHeB+Mng4tRp14nw1HjuM/pBqa/YikNM7HBtwJaL9hUE5nZrcge8qjVU60jJyzPTPxEaenverjUM</Modulus><D>5bBcUTgYAzswW48F4eV6QpmscTKTBYvxasFem+NM+mlR2de+G5BO387ziYab09BtUypQKVYbJL9bewyqDqNufd8E</D></RSAKeyValue>";
            IAccount account = new Account("Webmoney.Ponomarenko - 320508520783", "320508520783", key, wmVendor);
            _accounts.Add(account.Login, account);

            _vendors = new Dictionary<string, IVendor>()
            {
                {wmVendor.Name, wmVendor }
            };
            _instrumentsByVendors = new Dictionary<string, Dictionary<string, IInstrument>>();
            _instrByVendorStr = new Dictionary<string, List<string>>();

            foreach (var vendor in _vendors)
            {
                
                var instruments = await vendor.Value.GetInstrumentsAsync();
                if (instruments == null || instruments.Count < 1)
                    continue;
                
                _instrumentsByVendors.Add(vendor.Key, instruments);
                _instrByVendorStr.Add(vendor.Key, instruments.Select((i) => i.Key).ToList());
            }

        }

        private Dictionary<string, List<string>> _instrByVendorStr;
        private Dictionary<string, Dictionary<string, IInstrument>> _instrumentsByVendors;
        private Dictionary<string, IVendor> _vendors;
        private Dictionary<string, IAccount> _accounts;
    }
}
