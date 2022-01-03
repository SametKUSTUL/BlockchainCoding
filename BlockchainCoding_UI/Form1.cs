using BlockchainCoding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockchainCoding_UI
{
    public enum LogType
    {
        Info = 15,
        Error = 12,
        Warning = 14
    }
    public partial class Form1 : Form
    {
        public static Blockchain ourblockchain = new Blockchain();
        public static int Port = 5001;
        public static P2PClient Client = new P2PClient();
        public static P2PServer Server = null;
        public static List<Miner> MinerList = null;
        public static string name = "Unknown";

        public Form1()
        {
            InitializeComponent();

            ourblockchain.InitializeChain();
            Server = new P2PServer();
            Server.Start();

        }
        public static void ConsoleWrite(string line, LogType type)
        {
            Console.ForegroundColor = (ConsoleColor)type;
            Console.WriteLine(line);
            Console.ForegroundColor = (ConsoleColor)LogType.Info;
        }

    }
}
