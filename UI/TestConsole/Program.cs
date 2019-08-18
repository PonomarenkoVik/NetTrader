using CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic;

namespace TestConsole
{
    class Program
    {
        static  void Main(string[] args)
        {
            ITradeLogic tradeLogic = new FacadeTradeLogic();
        
            var instruments = tradeLogic.Instruments;
           
        }

    }
} 
