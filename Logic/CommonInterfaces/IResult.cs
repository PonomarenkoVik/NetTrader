using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
