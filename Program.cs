using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Externs;
using GlobalHook.Core;
using GlobalHook.Core.Keyboard;
using GlobalHook.Core.MessageLoop;
using GlobalHook.Core.Windows.Keyboard;
using GlobalHook.Core.Windows.MessageLoop;
using Keys = GlobalHook.Core.Keyboard.Keys;
using ImageDB;

namespace emote_gui_dotnet_win
{
    static class Program
    {
        public static EmoteSearchForm runninginstance;

        [STAThread]
        static void Main()
        {
#if !DEBUG
            if(false) //TODO: add parameter check
#endif
                User32.AllocConsole();
            // ImageDBCreator.CreateDB();

            IKeyboardHook kbHook = new KeyboardHook();
            IMessageLoop loop = new MessageLoop();

            CancellationTokenSource source = new CancellationTokenSource();


            kbHook.OnEvent += (_, e) =>
            {
                if (e.Key.HasFlags(Keys.Alt, Keys.Control, Keys.Q))
                {
                    source.Cancel();
                    Application.Exit();
                    //Environment.Exit();
                }
                else if (e.Key.HasFlags(Keys.Alt, Keys.E))
                    StartAppInstance();
            };

#if DEBUG
            Console.WriteLine("hello");
#else
            MessageBox.Show("Emote Box Running!");
#endif            
            loop.Run(source.Token, kbHook);

        }
        private static void StartAppInstance()
        {
            if(runninginstance != null)
            {
                if(!runninginstance.Visible)
                {
                    runninginstance.Show();
                }
                else
                {
                    User32.ShowWindow(runninginstance.Handle, 6);
                    User32.ShowWindow(runninginstance.Handle, 9);
                    User32.SetForegroundWindow(runninginstance.Handle);
                }
                
            }
            else
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EmoteSearchForm());
            }
        }


    }
}
