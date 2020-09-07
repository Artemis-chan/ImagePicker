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

namespace emote_gui_dotnet_win
{
    public partial class EmoteSearchForm : Form
    {
        private EmoteQueryClient _eqc = new EmoteQueryClient();
        private WebClient _web = new WebClient();
        private Queue<string> _imgQ = new Queue<string>();
        private Task _dlTask;

        public EmoteSearchForm()
        {
            InitializeComponent();
        }

        ~EmoteSearchForm()
        {
            _web.Dispose();
        }

        public void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                Application.Exit();

            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if(!emoteList.Focused)
                    emoteList.Focus();
            }
            else if(e.KeyCode == Keys.Enter)
            {

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
                    _imgQ.Enqueue(reader.GetString(0));
                    emoteList.Items.Add(reader.GetString(1), i++);
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
                emoteList.SmallImageList.Images.Add(await GetImage(url));
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
