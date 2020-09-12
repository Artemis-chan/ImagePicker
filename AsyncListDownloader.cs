using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace emote_gui_dotnet_win.DL
{
    public class AsyncListDownloader
    {
        private WebClient _web = new WebClient();
        private ListView _list;
        private Queue<string> _dlQ = new Queue<string>();
        private Task _currentDL;

        public AsyncListDownloader(ListView list)
        {
            _list = list;
        }

        ~AsyncListDownloader()
        {
            _web.Dispose();
        }

        public void AddDl(string url)
        {
            if (_currentDL != null)
            {
                _dlQ.Enqueue(url);
                return;
            }
            _currentDL = Task.Run(() => StartDL(url));
        }

        private async Task StartDL(string url)
        {
            //TODO: implement invoke required
            try
            {
                _list.SmallImageList.Images.Add(
                    Image.FromStream(new MemoryStream(await _web.DownloadDataTaskAsync(url))));
            }
            catch (AggregateException)
            {
                //MessageBox.Show(e.Message);
                return;
            }
            catch(WebException)
            {

            }

            _list.Refresh();

            _currentDL = null;
            NextDl();
        }

        private void NextDl()
        {   
            if (_dlQ.Any() && _dlQ.TryDequeue(out string url))
            {
                _currentDL = Task.Run(() => StartDL(url));
            }
        }

        public void Cancel()
        {
            _dlQ.Clear();
            _web.CancelAsync();
            _currentDL?.Wait();
        }
        

    }
}