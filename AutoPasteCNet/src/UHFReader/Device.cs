using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReaderB;
using AutoPasteCNet.src.Helper;
using AutoPasteCNet.src.BindKeyboard;

namespace AutoPasteCNet.src.UHFReader
{
    class Device
    {
        private byte fComAdr = 0xff;
        private byte fBaud;
        private int baudrate_default = 57600; // bps
        private int frmcomportindex;
        private int fOpenComIndex;
        private bool ComOpen = false;
        private int fCmdRet = 30;
        private bool fIsInventoryScan;
        private string fInventory_EPC_List;
        private bool fAppClosed;

        private TKHelper tk;
        private Keyboard keyboard;

        public Device()
        {
            tk = new TKHelper();
            keyboard = new Keyboard();
            InitDisplay();
        }

        public void InitDisplay()
        {
            Console.WriteLine("=====================");
            Console.WriteLine("Auto Paste UHF Reader");
            Console.WriteLine("=====================");
        }

        // automatically detected COM UHF Reader
        public void OpenPort()
        {
            int port = 0;
            int openresult, i;
            openresult = 30;
            fBaud = 5;
            openresult = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, fBaud, ref frmcomportindex);
            fOpenComIndex = frmcomportindex;
            if (openresult == 0)
            {
                ComOpen = true;
                if ((fCmdRet == 0x35) | (fCmdRet == 0x30))
                {
                    Console.WriteLine("Error : Serial Communication Error or Occupied.");
                    StaticClassReaderB.CloseSpecComPort(frmcomportindex);
                    ComOpen = false;
                    return;
                }
            }
            if ((fOpenComIndex != -1) & (openresult != 0X35) & (openresult != 0X30))
            {
                string openedCOM = "COM" + Convert.ToString(fOpenComIndex);
                Console.WriteLine("\nSuccessfuly Open " + openedCOM);
                Console.WriteLine("Running ...\n");
                ComOpen = true;
            }
            if ((fOpenComIndex == -1) && (openresult == 0x30))
            {
                Console.WriteLine("Error : Serial Communication Error.");
                return;
            }
        }

        public void ClosePort()
        {
            int port = fOpenComIndex;
            string temp;
            fCmdRet = StaticClassReaderB.CloseSpecComPort(port);
            if (fCmdRet == 0)
            {
                fComAdr = 0xFF;
                //StaticClassReaderB.OpenComPort(port, ref fComAdr, fBaud, ref frmcomportindex);
                fOpenComIndex = frmcomportindex;
                fOpenComIndex = -1;

                Console.WriteLine("Successfully Close COM" + port);
            }
            else
            {
                Console.WriteLine("Error : Serial Communication Error. Can't Close COM Port.");
            }
        }

        public void Inventory()
        {
            int i;
            int CardNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[5000];
            int CardIndex;
            string temps;
            string s, sEPC;
            bool isonlistview;
            fIsInventoryScan = true;
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;
            fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))
            {
                byte[] daw = new byte[Totallen];
                //Console.WriteLine(daw);
                Array.Copy(EPC, daw, Totallen);
                temps = ByteArrayToHexString(daw);
                fInventory_EPC_List = temps;
                m = 0;
                //Console.WriteLine("Card Num : " + CardNum);
                if (CardNum == 0)
                {
                    fIsInventoryScan = false;
                    return;
                } else
                {
                    EPClen = daw[m];
                    sEPC = temps.Substring(m * 2 + 2, EPClen * 2);

                    // Show UID Card
                    string EPC_Card = tk.ConvertEPCHexToNumber(sEPC);
                    Console.WriteLine("\n=========================");
                    Console.WriteLine(tk.GetCurrentDatetime());
                    Console.WriteLine("=========================");
                    Console.WriteLine("UID : " + EPC_Card);

                    // auto copy-paste feature
                    keyboard.AutoCopyPasteEvent(EPC_Card);
                }                
            }
            fIsInventoryScan = false;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
    }
}