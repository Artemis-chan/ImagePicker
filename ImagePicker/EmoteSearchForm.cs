﻿using System;
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
using System.Runtime.InteropServices;
using Externs;
using WindowsInput;
using WindowsInput.Native;

namespace ImagePicker
{
    public partial class EmoteSearchForm : Form
    {
        private const int THUMB_SIZE = 38;

        private Task _imageTask = null;
        private bool _cancelLoad = false;

        ImageData[] _imageFiles;
        DateTime _checkTime = DateTime.MinValue;


        struct ImageData
        {
            public string name;
            public string path;
        }

        public EmoteSearchForm()
        {
            InitializeComponent();

            Program.instance = this;
            taskbarIcon.ShowBalloonTip(2, "o(≧∇≦o)", "Started!\nPress alt + e to show!", ToolTipIcon.None);

            WindowState = FormWindowState.Minimized;
        }
        
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
                    // Clipboard.SetImage(emoteList.LargeImageList.Images[imageIndex]);
                    Clipboard.SetImage(Image.FromFile((string)emoteList.LargeImageList.Images.Keys[imageIndex]));
                }
                Hide();
                e.SuppressKeyPress = true;
                
                SendCtrlV();
            }
        }

        private void SendCtrlV()
        {
            Thread.Sleep(300);
            new InputSimulator().Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right || keyData == Keys.Left)
            {
                if (msg.HWnd != emoteList.Handle)
                {
                    emoteList.Focus();
                    PInvoke.PostMessage(emoteList.Handle, msg.Msg, msg.WParam, msg.LParam);
                    return true;
                }
            }
            else if(msg.HWnd != queryInput.Handle)
            {
                queryInput.Focus();
                PInvoke.PostMessage(queryInput.Handle, msg.Msg, msg.WParam, msg.LParam);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Query
        public void QueryEmote(object sender, EventArgs args)
        {
            CancelFill();

            emoteList.Items.Clear();
            emoteList.LargeImageList?.Dispose();

            if(String.IsNullOrWhiteSpace(queryInput.Text))
                return;

            var imgs = new ImageList(){ ImageSize = new Size(THUMB_SIZE, THUMB_SIZE), ColorDepth = ColorDepth.Depth32Bit, TransparentColor = Color.Transparent };

            emoteList.LargeImageList = imgs;

#if DEBUG
            Console.WriteLine(queryInput.Text);
#endif
            _imageTask = Task.Run(FillImageList);
        }

        void UpdateImageDataArray()
        {
            if (_checkTime < Directory.GetLastWriteTime(Program.ImageFolder))
            {
                _imageFiles = GetFileList();
                _checkTime = DateTime.Now;
            }
        }

        ImageData[] GetFileList()
        {
            List<ImageData> files_ = new List<ImageData>();
            var files = Directory.GetFiles(Program.ImageFolder).Where((_) => ImageCheck.IsImage(_));

            foreach (var item in files)
            {
                files_.Add(new ImageData()
                {
                    name = Path.GetFileNameWithoutExtension(item),
                    path = item
                });
            }

            return files_.ToArray();
        }

        private Task FillImageList()
        {
            UpdateImageDataArray();
            var files = _imageFiles.Where((_) => _.name.Contains(queryInput.Text, StringComparison.OrdinalIgnoreCase)).ToArray();
            // emoteList.SetDoubleBuffer(true);
            int i = 0;
            foreach(var item in files)
            {
                if (_cancelLoad)
                {
                    break;
                }
                AddToList(item.path, item.name, i++);
            }
            // emoteList.SetDoubleBuffer(false);
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
            Console.WriteLine("cancelling");
            _cancelLoad = true;
            _imageTask?.Wait();
            _cancelLoad = false;
        }
        
        #endregion

    }
}
