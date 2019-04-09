using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoPasteCNet.src.BindKeyboard
{
    class Keyboard
    {
        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const int V = 0x56; //V key code

        public void AutoCopyPasteEvent(String text)
        {
            try
            {
                // Copy to Clipboard
                OpenClipboard(IntPtr.Zero);
                var ptr = Marshal.StringToHGlobalUni(text);
                SetClipboardData(13, ptr); 
                CloseClipboard();
                Marshal.FreeHGlobal(ptr);

                // Hold Control down and press V
                keybd_event(VK_LCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(V, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(V, 0, KEYEVENTF_KEYUP, 0);

                // unbind the control key + v flags
                keybd_event(V, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                keybd_event(VK_LCONTROL, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void ClearClipboard()
        {
            OpenClipboard(IntPtr.Zero);
            var ptr = Marshal.StringToHGlobalUni("");
            SetClipboardData(13, ptr);
            CloseClipboard();
            Marshal.FreeHGlobal(ptr);
        }
    }
}
