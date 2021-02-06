using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Externs;
using Gma.System.MouseKeyHook;

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

			RunInputHook();
			RunApp();
		}

        public static void Init()
        {
            ImageFolder = "Images";
        }

        private static void RunApp()
		{

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new EmoteSearchForm());
        }

        private static void RunInputHook()
		{
			var gh = Hook.GlobalEvents();
			gh.OnCombination(new Dictionary<Combination, Action>()
			{
				{Combination.TriggeredBy(Keys.E).Alt(), () => ShowAppInstance()},
                {Combination.TriggeredBy(Keys.Q).Alt(), () => Exit()}
			});
        }

        public static void Exit()
        {
            source.Cancel();
            instance.Dispose();
            Application.Exit();
            // Environment.Exit(0);
        }

        private static void ShowAppInstance()
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
