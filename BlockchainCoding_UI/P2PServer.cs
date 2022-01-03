using BlockchainCoding_UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BlockchainCoding
{
    public class P2PServer : WebSocketBehavior
    {
        bool chainSynched = false;
        WebSocketServer wss = null;
        public void Start()
        {
            wss = new WebSocketServer($"ws://127.0.0.1:{Form1.Port}");
            wss.AddWebSocketService<P2PServer>("/Blockchain");
            wss.Start();
            Console.WriteLine($"Server şu adreste başlatıldı ws://127.0.0.1:{Form1.Port}");

        }
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "Merhaba Server")
            {
                Console.WriteLine(e.Data);
                Send("Merhaba Client");
            }
            else
            {
                if (e.Data.Contains("MinerInfo"))
                {
                    List<Miner> newMinerList = JsonConvert.DeserializeObject<List<Miner>>(e.Data);
                    foreach (Miner item in newMinerList)
                    {
                        Form1.MinerList.Add(item);
                    }
                    

                }
                else
                {
                    Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);
                    if (newChain.IsValid() && newChain.Chain.Count > Form1.ourblockchain.Chain.Count)
                    {
                        List<Transaction> newTransactions = new List<Transaction>();
                        newTransactions.AddRange(newChain.PendingTransactions);
                        newTransactions.AddRange(Form1.ourblockchain.PendingTransactions);
                        newChain.PendingTransactions = newTransactions;
                        Form1.ourblockchain = newChain;
                    }
                }
               

            }
            if (!chainSynched)
            {
                Send(JsonConvert.SerializeObject(Form1.ourblockchain));
                chainSynched = true;
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("Bir hata ile karşılaşıldı.!!");
            Console.WriteLine(e.ToString());
        }

    }
}
