using BlockchainCoding_UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;

namespace BlockchainCoding
{
    public class P2PClient
    {
        public static IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();
        public void Connect(string url)
        {
            try
            {
                if (!wsDict.ContainsKey(url))
                {
                    if (CheckUrlStatus(url))
                    {
                        WebSocket ws = new WebSocket(url);
                        ws.WaitTime = TimeSpan.FromSeconds(10);
                        ws.OnMessage += (sender, e) =>
                        {
                            if (e.Data == "Merhaba Client")
                            {
                                Console.WriteLine(e.Data);
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
                        };
                        ws.Connect();
                        if (ws.IsAlive)
                        {
                            ws.Send("Merhaba Server");
                            ws.Send(JsonConvert.SerializeObject(Form1.ourblockchain));
                            wsDict.Add(url, ws);
                        }
                        else
                        {
                            Form1.ConsoleWrite("\n" + url + "-- Baglanti kurulamadi.Lütfen sunucu adresinden emin olunuz.!", LogType.Warning);


                        }
                    }
                    else
                    {
                        Form1.ConsoleWrite("Sunucu adresi geçersiz",LogType.Error);
                    }
                }
                else
                {
                    Form1.ConsoleWrite("Zaten bu sunucuya baglisiniz!", LogType.Warning);

                }
            }
            catch (WebSocketException e)
            {
                Form1.ConsoleWrite("Connection Failed!", LogType.Error);

            }

        }
        public void Send(string url, string data)
        {
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    item.Value.Send(data);
                }
            }
        }
        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
        }
        public IList<string> GetServers()
        {
            IList<string> servers = new List<string>();
            foreach (var item in wsDict)
            {
                servers.Add(item.Key);
            }
            return servers;
        }
        public void Close()
        {
            foreach (var item in wsDict)
            {
                item.Value.Close();
            }
        }

        protected bool CheckUrlStatus(string Website)
        {
            return true;
            //regex yaz.
        }




    }
}
