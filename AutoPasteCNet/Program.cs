using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoPasteCNet.src.BindKeyboard;
using AutoPasteCNet.src.UHFReader;

namespace AutoPasteCNet
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Keyboard keyboard = new Keyboard();
            //keyboard.AutoCopyPasteEvent("Takeru Adelbert");
            Device device = new Device();
            device.OpenPort();
            while(true)
            {
                device.Inventory();
                System.Threading.Thread.Sleep(3000); // 3 seconds
            }
            //device.ClosePort();
        }
    }
}
