using System;
using System.IO;
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

namespace ImagePicker
{
    static class Program
    {
        private static string _imageFolder;
        public static string ImageFolder
        {
            get { return _imageFolder; }
            set
            {
                if(!Directory.Exists(value))
                    Directory.CreateDirectory(value);
                _imageFolder = value;
            }
        }

        public static EmoteSearchForm instance;

        static CancellationTokenSource source = new CancellationTokenSource();

        [STAThread]
        static void Main()
        {
            Init();

            //PInvoke.AllocConsole();

            Task.Run(RunInputHook);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EmoteSearchForm());
        }

        public static void Init()
        {
            ImageFolder = "Images";
        }

        public static void RunInputHook()
        {
            IKeyboardHook kbHook = new KeyboardHook();
            IMessageLoop loop = new MessageLoop();

            kbHook.OnEvent += (_, e) =>
            {
                if (e.Key.HasFlags(Keys.Alt, Keys.Control, Keys.Q))
                {
                    Exit();
                }
                else if (e.Key.HasFlags(Keys.Alt, Keys.E))
                    StartAppInstance();
            };

            loop.Run(source.Token, kbHook);
        }

        public static void Exit()
        {
            source.Cancel();
            instance.Dispose();
            Application.Exit();
            // Environment.Exit(0);
        }

        private static void StartAppInstance()
        {
            if(instance != null)
            {
                if(!instance.Visible)
                {
                    instance.Show();
                }

                if(PInvoke.GetForegroundWindow() != instance.Handle)
                {
                    //As windows doesn't let background proceses to set foregroundwindow unless one of some specific conditions
                    //are met, restoring the form from minimized is one of the workarounds 
                    //(https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow)

                    // User32.ShowWindow(instance.Handle, 6);
                    // User32.ShowWindow(instance.Handle, 9);
                    instance.WindowState = FormWindowState.Minimized;
                    instance.WindowState = FormWindowState.Normal;
                    // PInvoke.SetForegroundWindow(instance.Handle);
                }
            }
        }
    }
}
