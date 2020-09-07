using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlobalHook.Core;
using GlobalHook.Core.Keyboard;
using GlobalHook.Core.MessageLoop;
using GlobalHook.Core.Windows.Keyboard;
using GlobalHook.Core.Windows.MessageLoop;
using Keys = GlobalHook.Core.Keyboard.Keys;

namespace emote_gui_dotnet_win
{
    static class Program
    {
        public static bool instanceRunning = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IKeyboardHook kbHook = new KeyboardHook();
            IMessageLoop loop = new MessageLoop();

            CancellationTokenSource source = new CancellationTokenSource();

            kbHook.OnEvent += (_, e) =>
            {
                if (e.Key.HasFlags(Keys.Control, Keys.Shift, Keys.S))
                    source.Cancel();
                else if(e.Key.HasFlags(Keys.Control, Keys.Shift, Keys.E))
                    StartAppInstance();
            };

            MessageBox.Show("Emote Box Running!");
            loop.Run(source.Token, kbHook);

        }
        private static void StartAppInstance()
        {
            if(instanceRunning)
                return;
            instanceRunning = true;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EmoteSearchForm());
        }
    }
}
