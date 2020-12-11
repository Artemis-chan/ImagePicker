using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Externs;

namespace emote_gui_dotnet_win
{
    public partial class EmoteSearchForm : Form
    {
        private const int THUMB_SIZE = 38;

        private Task _imageTask = null;
        private bool _cancelLoad = false;

        public EmoteSearchForm()
        {
            Program.runninginstance = this;
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
            {
                Hide();
                e.SuppressKeyPress = true;
            }

            if(e.KeyCode == Keys.Enter)
            {
                if(emoteList.Items.Count > 0)
                {
                    int imageIndex;
                    if(emoteList.SelectedItems.Count < 1)
                    {
                        imageIndex = emoteList.Items[0].ImageIndex;
                    }
                    else
                    {
                        imageIndex = emoteList.SelectedItems[0].ImageIndex;
                    }
                    //MessageBox.Show((string)path);
                    // Clipboard.SetText((string)path);
                    Clipboard.SetImage(emoteList.LargeImageList.Images[imageIndex]);
                    // Clipboard.SetImage(Image.FromFile((string)emoteList.LargeImageList.Images.Keys[imageIndex]));
                }
                Hide();
                e.SuppressKeyPress = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right || keyData == Keys.Left)
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
            //for debugging purposes
            // ResizedImage("images/airfish.png");
            // return;
            CancelFill();

            emoteList.Items.Clear();
            emoteList.LargeImageList?.Dispose();

            if(String.IsNullOrWhiteSpace(queryInput.Text))
                return;

            //var message = "";
            // var dir = @"Z:\Amick\Pictures\Microsoft Clip Organizer";
            // var files = Directory.GetFiles(dir);
            var imgs = new ImageList(){ ImageSize = new Size(THUMB_SIZE, THUMB_SIZE), ColorDepth = ColorDepth.Depth32Bit, TransparentColor = Color.Transparent };

            emoteList.LargeImageList = imgs;

            // for (int i = 0; i < files.Length; i++)
            // {
            //     emoteList.Items.Add(i.ToString(), i);
            //     //emoteList.Items.Add(item);
            // }
            // for (int i = 0; i < files.Length; i++)
            // {
            //     string item = file;
            //     emoteList.LargeImageList.Images.Add(Image.FromFile(item));
            //     //emoteList.Items.Add(item);
            // }

#if DEBUG
            Console.WriteLine(queryInput.Text);
#endif
            // _imageTask = Task.Run(FillImageList);
            _imageTask = Task.Run(FillImageList);

            //MessageBox.Show(message);

        }

        struct Image_
        {
            public string name;
            public string path;
        }

        Image_[] imageFiles = GetFileList();

        static Image_[] GetFileList()
        {
            List<Image_> files_ = new List<Image_>();
            var files = Directory.GetFiles("images").Where((_) => ImageCheck.IsImage(_));
            foreach (var item in files)
            {
                files_.Add(new Image_()
                {
                    name = Path.GetFileNameWithoutExtension(item),
                    path = item
                });
            }
            return files_.ToArray();
        }

        private Task FillImageList()
        {
            var files = imageFiles.Where((_) => _.name.Contains(queryInput.Text, StringComparison.OrdinalIgnoreCase)).ToArray();
            emoteList.SetDoubleBuffer(true);
            int i = 0;
            foreach(var item in files)
            {
                if (_cancelLoad)
                {
                    break;
                }
                AddToList(item.path, item.name, i++);
            }
            emoteList.SetDoubleBuffer(false);
            return Task.CompletedTask;
        }

        private void AddToList(string path, string name, int i)
        {
            emoteList.Items.Add(name, i);
            emoteList.LargeImageList.Images.Add(path, ResizedImage(path));
        }

        //resizes images so that the aspect ratio is preserved on thumbnail
        private Bitmap ResizedImage(string file)
        {
            var image = Image.FromFile(file);
            int iH = image.Height, iW = image.Width;

            var cImg = new Bitmap(THUMB_SIZE, THUMB_SIZE);
            var g = Graphics.FromImage(cImg);
            g.Clear(Color.Transparent);

            if(iH < THUMB_SIZE && iW < THUMB_SIZE)
            {
                // MessageBox.Show(file);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                
                int h = (THUMB_SIZE - iH) / 2;
                int w = (THUMB_SIZE - iW) / 2;

                g.DrawImage(image,
                            new Rectangle(w, h, iW, iH),
                            new Rectangle(0, 0, iW, iH),
                            GraphicsUnit.Pixel);
            }
            else
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                double mod;
                if (iH > iW)
                {
                    mod = (double)iH / THUMB_SIZE;
                }
                else
                {
                    mod = (double)iW / THUMB_SIZE;
                }
                int h = (int)(iH / mod);
                int w = (int)(iW / mod);

                if(file == "images/airfish.png")
                {
                    Console.Write(' ');
                }

                g.DrawImage(image, 
                            new Rectangle(0, 0, w, h), 
                            new Rectangle(0, 0, iW, iH), 
                            GraphicsUnit.Pixel);
            }
            return cImg;
        }

        private void CancelFill()
        {
            _cancelLoad = true;
            _imageTask?.Wait();
            _cancelLoad = false;
        }
        
        #endregion

    }
}
