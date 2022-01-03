using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainCoding
{
    public class Miner
    {
        private decimal _balance;

        public string MinerInfo { get; set; }
        public decimal balance { get { return _balance; }  set { _balance += value; } }

       
    }
}
