using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommonInterfaces;

namespace CommonLibrary
{
    public class Account : IAccount
    {
        public Account(string login, string id, string password, IVendor vendor)
        {
            Login = login;
            Id = id;
            Password = password;
            Vendor = vendor;
        }

        public string Login { get; }
        public string Id { get; }
        public string Password { get; }
        public IVendor Vendor { get; }
        public async Task<IEnumerable<IAsset>> GetAssetsAsync() => await Vendor.GetAssetsAsync(this);
    }
}
