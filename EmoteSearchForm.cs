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
using emote_gui_dotnet_win.DL;
using Externs;

namespace emote_gui_dotnet_win
{
    public partial class EmoteSearchForm : Form
    {
        private EmoteQueryClient _eqc = new EmoteQueryClient();
        private AsyncListDownloader _web;
        private Task _listTask = null;
        private CancellationTokenSource _cancelSource = null;
        private TaskScheduler _taskScheduler;

        public EmoteSearchForm()
        {
            Program.runninginstance = this;
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            InitializeComponent();
        }

        // ~EmoteSearchForm()
        // {
        // }

        #region Input
        public void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: find out a way to pass input to newly focused control
            if(e.KeyCode == Keys.Escape)
                Hide();

            if(e.KeyCode == Keys.Enter)
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
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Up || keyData == Keys.Down)
            {
                if (msg.HWnd != emoteList.Handle)
                {
                    User32.PostMessage(emoteList.Handle, msg.Msg, msg.WParam, msg.LParam);
                    return true;
                }
            }
            else if(msg.HWnd != queryInput.Handle)
            {
                queryInput.Focus();
                User32.PostMessage(queryInput.Handle, msg.Msg, msg.WParam, msg.LParam);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Query
        public void QueryEmote(object sender, EventArgs args)
        {
            CancelImageFill();
            emoteList.Items.Clear();
            emoteList.SmallImageList?.Dispose();

            if(String.IsNullOrWhiteSpace(queryInput.Text))
                return;

            //var message = "";
            // var dir = @"Z:\Amick\Pictures\Microsoft Clip Organizer";
            // var files = Directory.GetFiles(dir);
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

            _cancelSource = new CancellationTokenSource();
            _listTask = Task.Factory.StartNew(FillImages, _cancelSource.Token);

            //MessageBox.Show(message);

        }

        private void CancelImageFill()
        {
            _cancelSource?.Cancel();
            try
            {
                _listTask?.Wait();
            }
            catch(AggregateException)
            { }

            _web.Cancel();
        }

        private Task FillImages()
        {
            if(InvokeRequired)
            {
                _listTask = new Task(() => FillImages());
                _listTask.RunSynchronously(_taskScheduler);
                return Task.CompletedTask;
            }

            using (var reader = _eqc.GetEmotesReader(queryInput.Text))
            {
                int i = 0;
                while (reader.Read())
                {
                    var url = reader.GetString(0);
                    _web.AddDl(url);
                    emoteList.Items.Add(reader.GetString(1), i);
                    emoteList.Items[i++].Tag = url;
                    //message += $"{reader.GetString(0)} - {reader.GetString(1)}\n";
                    if (_cancelSource.Token.IsCancellationRequested)
                    {
                        _cancelSource.Token.ThrowIfCancellationRequested();
                    }
                }
            }
            return Task.CompletedTask;
        }
        #endregion

    }
}
