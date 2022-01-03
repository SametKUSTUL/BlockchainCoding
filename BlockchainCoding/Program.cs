using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlockchainCoding
{
    public enum LogType
    {
        Info=15,
        Error=12,
        Warning=14
    }

    class Program
    {
        public static Blockchain ourblockchain = new Blockchain();
        public static int Port = 0;
        public static P2PClient Client = new P2PClient();
        public static P2PServer Server = null;
        public static List<Miner> MinerList = null;
        public static string name = "Unknown";

        public static void ConsoleWrite(string line, LogType type)
        {
            Console.ForegroundColor = (ConsoleColor)type;
            Console.WriteLine(line);
            Console.ForegroundColor = (ConsoleColor)LogType.Info;
        }

        static void Main(string[] args)
        {

            MinerList = new List<Miner>();


            DateTime startTime = DateTime.Now;
            ourblockchain.InitializeChain();
            if (args.Length >= 1)
                Port = int.Parse(args[0]);
            if (args.Length >= 2)
                name = args[1];
            if (Port > 0)
            {
                Server = new P2PServer();
                Server.Start();
            }
            if (name != "Unknown")

                MinerList.Add(new Miner()
                {
                    MinerInfo = name + "--" + Convert.ToString(Port),
                    balance = 0
                });


            Console.WriteLine($"Su anki Kullanıcı:{name}");
            /*Console.WriteLine("=========================");
            Console.WriteLine("1. Server a Baglan");
            Console.WriteLine("2. Transaction Ekle");
            Console.WriteLine("3. Blockchain i Goster");
            Console.WriteLine("4. Kullanici Bakiye Goster");
            Console.WriteLine("5. Zincirdeki baglanan serverları göster");
            Console.WriteLine("6. Zincirdeki tüm minerları göster");
            Console.WriteLine("-1. Cikis");
            Console.WriteLine("=========================");*/

            int selection = 0;

            while (selection != -1)
            {
                switch (selection)
                {
                    case 1:
                        Console.WriteLine("Lütfen Server URL ini Girin:");
                        string serverURL = Console.ReadLine();
                        Client.Connect($"{serverURL}/Blockchain");
                        break;
                    case 2:
                        Console.WriteLine("Lütfen Oy vermek istediğiniz kişinin adini yaziniz");
                        string receiverName = Console.ReadLine();
                        //Console.WriteLine("Miktari girin");
                        //string amount = Console.ReadLine();
                        string amount = "1";
                        ourblockchain.CreateTransaction(new Transaction(name, receiverName, int.Parse(amount)));
                        ourblockchain.ProcessPendingTransactions(name);
                        Client.Broadcast(JsonConvert.SerializeObject(ourblockchain));
                        Client.Broadcast(JsonConvert.SerializeObject(MinerList));
                        break;
                    case 3:
                        Console.WriteLine("Blockchain");
                        Console.WriteLine(JsonConvert.SerializeObject(ourblockchain, Formatting.Indented));
                        break;

                    case 4:
                        ourblockchain.ShowAllUserBalance();
                        break;

                    case 5:

                        foreach (var item in P2PClient.wsDict)
                        {
                            Console.WriteLine("Miner:" + item.Key + " - " + item.Value);
                        }
                        break;

                    case 6:
                        foreach (Miner item in Program.MinerList)
                        {
                            Console.WriteLine(item.MinerInfo + "--" + Convert.ToString(item.balance));
                        }
                        break;
                    default:
                        selection = 0;
                        break;

                }


                Console.WriteLine("< ============= Oy Kullanım Uygulaması ============= >");
                Console.WriteLine("Adaylar: AHMET, MEHMET, AYSE, FATMA, HUSEYIN, ZEYNEP");
                Console.WriteLine("1. Onayla");
                Console.WriteLine("2. Oy Kullan");
                Console.WriteLine("3. Kullanılan Oyları Göster");
                Console.WriteLine("4. Mevcut Durumu Göster");
                Console.WriteLine("5. Zincirdeki baglanan serverları göster");
                Console.WriteLine("6. Zincirdeki tüm minerları göster");
                Console.WriteLine("-1. Cikis");

                string action = Console.ReadLine();
                selection = int.Parse(action);

                Console.Clear();
            }

            Client.Close();




        }
    }
}
