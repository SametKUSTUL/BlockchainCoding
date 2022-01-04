using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainCoding
{
    public enum LogType
    {
        Info = 15,
        Error = 12,
        Warning = 14
    }

    class Program
    {
        private static List<string> AdayList = new List<string>() { "AHMET", "MEHMET", "AYSE", "FATMA", "HASAN", "ECEM" };
        public static Blockchain ourblockchain = new Blockchain();
        public static int Port = 0;
        public static P2PClient Client = new P2PClient();
        public static P2PServer Server = null;
        public static List<Miner> MinerList = null;
        public static string name = "Unknown";
        public static int hataliGirisSayisi = 0;
        public static  int MaxHataliGirisSayisi = 3;

        public static void ConsoleWrite(string line, LogType type)
        {
            Console.ForegroundColor = (ConsoleColor)type;
            Console.WriteLine(line);
            Console.ForegroundColor = (ConsoleColor)LogType.Info;
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
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
                        {
                            Console.WriteLine("Lütfen Server URL ini Girin:");
                            string serverURL = Console.ReadLine();
                            if (serverURL.Contains(":"))
                            {
                                List<string> lst = serverURL.Split(":").ToList();
                                string ip = lst[1];
                                ip = ip.Replace("//", "");
                                if (lst[0] == "ws" && ValidateIPv4(ip))
                                {
                                    hataliGirisSayisi = 0;
                                    Client.Connect($"{serverURL}/Blockchain");
                                }
                                else
                                {
                                    hataliGirisSayisi++;
                                    if (hataliGirisSayisi < MaxHataliGirisSayisi)
                                    {
                                        serverURL = string.Empty;
                                        Program.ConsoleWrite("HATA:Yanlıs sunucu adresi!", LogType.Error);
                                        System.Threading.Thread.Sleep(2000);
                                        Console.Clear();
                                        selection = 1;
                                        continue;
                                    }
                                    else
                                    {
                                        serverURL = string.Empty;
                                        Program.ConsoleWrite("HATA:Yanlıs sunucu adresi!", LogType.Error);
                                        System.Threading.Thread.Sleep(2000);
                                        Console.Clear();
                                        selection = 0;
                                        hataliGirisSayisi = 0;

                                    }
                                 
                                }

                            }
                            else
                            {
                                hataliGirisSayisi++;
                                if (hataliGirisSayisi < MaxHataliGirisSayisi)
                                {
                                    serverURL = string.Empty;
                                    Program.ConsoleWrite("HATA:Yanlıs sunucu adresi!", LogType.Error);
                                    System.Threading.Thread.Sleep(2000);
                                    Console.Clear();
                                    selection = 1;
                                    continue;
                                }
                                else
                                {
                                    serverURL = string.Empty;
                                    Program.ConsoleWrite("HATA:Yanlıs sunucu adresi!", LogType.Error);
                                    System.Threading.Thread.Sleep(2000);
                                    Console.Clear();
                                    hataliGirisSayisi = 0;
                                    selection = 0;
                                }

                               
                            }
                            break;
                        }




                    case 2:
                        Console.WriteLine("Lütfen Alici adini Girin");
                        string receiverName = Console.ReadLine();
                        receiverName = receiverName.ToUpper();
                        if (!AdayList.Any(s => receiverName.Contains(s)))
                        {
                            hataliGirisSayisi++;
                            if (hataliGirisSayisi < MaxHataliGirisSayisi)
                            {
                                Program.ConsoleWrite("HATA: Sadece belirtilen adaylara oy kullanabilirsiniz.", LogType.Error);
                                receiverName = string.Empty;
                                System.Threading.Thread.Sleep(2000);
                                Console.Clear();
                                selection = 2;
                                continue;
                            }
                            else
                            {
                                receiverName = string.Empty;
                                System.Threading.Thread.Sleep(2000);
                                Console.Clear();
                                hataliGirisSayisi = 0;
                                selection = 0;
                            }
                           
                        }
                       /* Console.WriteLine("Miktari girin");
                        string amount = Console.ReadLine();*/
                        ourblockchain.CreateTransaction(new Transaction(name, receiverName, 1));
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

                    /*case 5:

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
                        break;*/
                    default:
                        selection = 0;
                        break;

                }


                Console.WriteLine("< ============= Açık Oy Kullanma Uygulaması ============= >");
                //  Program.ConsoleWrite("Adaylar: 1-AHMET \t 2-MEHMET \t 3-AYSE \t 4-FATMA \t 5-HASAN \t 6-ECEM\n", LogType.Warning);
                string adyList = string.Empty;
                foreach (string item in AdayList)
                {
                    adyList += item + "\t";
                }
                Program.ConsoleWrite(string.Concat(new string[] {
                "Adaylar:",
                adyList,
                "\n"
                }), LogType.Warning);
                Console.WriteLine("1. Kullanılan Oyları Servera Gönder ");
                Console.WriteLine("2. Oy Kullan");
                Console.WriteLine("3. Mevcut Blok Zincirini Göster");
                Console.WriteLine("4. Mevcut Oy Durumunu Göster");

                /*  Console.WriteLine("5. Zincirdeki baglanan serverları göster");
                  Console.WriteLine("6. Zincirdeki tüm minerları göster");*/
                Console.WriteLine("-1. Cikis");

                string action = Console.ReadLine();
                if (!Int32.TryParse(action, out selection))
                {
                    Program.ConsoleWrite("HATALI GIRIS, Sadece sayısal ifade girileblir.", LogType.Error);
                    System.Threading.Thread.Sleep(2000);
                }

                Console.Clear();
            }

            Client.Close();




        }
    }
}
