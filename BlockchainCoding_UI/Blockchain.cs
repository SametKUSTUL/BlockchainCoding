using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainCoding
{
    public class Blockchain
    {
        public IList<Transaction> PendingTransactions = new List<Transaction>();
        public IList<Block> Chain { get; set; }
        public int difficulty { get; set; } = 2;
        public int Reward { get; set; } = 0;
        public Blockchain()
        {
            
        }
        public void InitializeChain()
        {
            Chain = new List<Block>();
            AddGenesisBlock();
        }
        public Block CreateGenesisBlock()
        {
            Block block= new Block(DateTime.Now, null, PendingTransactions);
            block.Mine(difficulty);
            PendingTransactions = new List<Transaction>();

            return block;
        }
        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }
        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }
        public void AddBlock(Block block)
        {
            Block latestblock = GetLatestBlock();
            block.Index = latestblock.Index + 1;
            block.PreviousHash = latestblock.Hash;
            block.Hash = block.CalculateHash();
            block.Mine(this.difficulty);
            Chain.Add(block);
        }
        public void CreateTransaction( Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }
        public void ProcessPendingTransactions(string minerAddress)
        {
            CreateTransaction(new Transaction(null, minerAddress, Reward));
            Block block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);
            AddBlock(block);
            PendingTransactions = new List<Transaction>();
           

        }
        public bool IsValid()
        {
            for (int i=1; i<Chain.Count;i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];
                if (currentBlock.Hash!=currentBlock.CalculateHash())
                {
                    return false;
                }
                if (currentBlock.PreviousHash!=previousBlock.Hash)
                {
                    return false;
                }
                
            }
            return true;
        }
        public int GetBalance(string address)
        {
            int balance = 0;
            for (int i = 0; i < Chain.Count; i++)
            {
               for (int j=0;j<Chain[i].Transactions.Count;j++)
                {
                    var transaction = Chain[i].Transactions[j];
                    /*if (transaction.FromAddress==address)
                    {
                        balance -= transaction.Amount;
                    }*/
                    if (transaction.ToAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                } 

            }
            return balance;
        }

        public void ShowAllUserBalance()
        {
            IDictionary<string, int> UserBalance = new Dictionary<string,int>();
            foreach (var item in Chain)
            {
                foreach (var item2 in item.Transactions)
                {
                    if (!String.IsNullOrEmpty(item2.FromAddress))
                    {
                        if (!UserBalance.ContainsKey(item2.FromAddress))
                        {
                            UserBalance.Add(item2.FromAddress, GetBalance(item2.FromAddress));
                        }
                    }

                    if (!String.IsNullOrEmpty(item2.ToAddress))
                    {
                        if (!UserBalance.ContainsKey(item2.ToAddress))
                        {
                            UserBalance.Add(item2.ToAddress, GetBalance(item2.ToAddress));
                        }
                    }
                    

                }

            }

            foreach (var item in UserBalance)
            {
                Console.WriteLine(item.Key +":"+item.Value.ToString());
            }

        }


        

    }
}
