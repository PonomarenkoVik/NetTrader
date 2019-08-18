using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public interface ICommand
    {
        string VendorName { get; }
        string InstrumentName { get; }
        CommandType Type { get; }
    }
}
