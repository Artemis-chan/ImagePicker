using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using emote_gui_dotnet_win.DB;
using System.Runtime.InteropServices;

namespace emote_gui_dotnet_win
{
    public partial class EmoteSearchForm : Form
    {
        private EmoteQueryClient _eqc = new EmoteQueryClient();
        private WebClient _web = new WebClient();
        private Queue<string> _imgQ = new Queue<string>();
        private Task? _dlTask = null;

        public EmoteSearchForm()
        {
            Program.runninginstance = this;
            InitializeComponent();
        }

        ~EmoteSearchForm()
        {
            _web.Dispose();
        }

        public void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: find out a way to pass input to newly focused control
            if(e.KeyCode == Keys.Escape)
                Hide();

            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if(!emoteList.Focused)
                {
                    emoteList.Focus();
                }
            }
            else if(e.KeyCode == Keys.Enter)
            {
                if(emoteList.Items.Count > 0)
                {
                    object url;
                    if(emoteList.SelectedItems.Count < 1)
                    {
                        url = emoteList.Items[0].Tag;
                    }
                    else
                    {
                        url = emoteList.SelectedItems[0].Tag;
                    }
                    //MessageBox.Show((string)url);
                    Clipboard.SetText((string)url);
                }
                Hide();
            }
            else if(!queryInput.Focused)
            {
                queryInput.Focus();
            }
        }

        #region Query
        public void QueryEmote(object sender, EventArgs args)
        {
            _imgQ.Clear();
            _web.CancelAsync();
            _dlTask?.Wait();
            emoteList.Items.Clear();
            emoteList.SmallImageList?.Dispose();

            //var message = "";
            var dir = @"Z:\Amick\Pictures\Microsoft Clip Organizer";
            var files = Directory.GetFiles(dir);
            var imgs = new ImageList(){ ImageSize = new Size(32, 32) };

            emoteList.SmallImageList = imgs;

            // for (int i = 0; i < files.Length; i++)
            // {
            //     emoteList.Items.Add(i.ToString(), i);
            //     //emoteList.Items.Add(item);
            // }
            // for (int i = 0; i < files.Length; i++)
            // {
            //     string item = files[i];
            //     emoteList.SmallImageList.Images.Add(Image.FromFile(item));
            //     //emoteList.Items.Add(item);
            // }

            FillImages();

            //MessageBox.Show(message);

        }

        private void FillImages()
        {
            using (var reader = _eqc.GetEmotesReader(queryInput.Text))
            {
                int i = 0;
                while (reader.Read())
                {
                    var url = reader.GetString(0);
                    _imgQ.Enqueue(url);
                    emoteList.Items.Add(reader.GetString(1), i);
                    emoteList.Items[i++].Tag = url;
                    //message += $"{reader.GetString(0)} - {reader.GetString(1)}\n";
                }
            }
            _dlTask = Task.Run(AddImages);
        }

        private async Task AddImages()
        {
            while(_imgQ.Any())
            {
                var url = _imgQ.Dequeue();
                try
                {
                    emoteList.SmallImageList.Images.Add(await GetImage(url));
                }
                catch(WebException)
                {
                    return;
                }
                emoteList.Refresh();
            }
        }

        private async Task<Image> GetImage(string url)
        {
            //File.WriteAllBytes( new Uri(url).Segments.Last() + ".png", _web.DownloadData(url));
            return Image.FromStream(new MemoryStream(await _web.DownloadDataTaskAsync(url)));
        }
        #endregion

    }
}
