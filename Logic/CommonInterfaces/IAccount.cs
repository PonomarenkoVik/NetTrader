using CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public interface IAccount
    {
        string Login { get; }
        string Id { get; }
        string Password { get; }
        Task<IEnumerable<IAsset>> GetAssetsAsync();
        IVendor Vendor { get; }
    }
}
