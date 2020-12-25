using System;
using System.Drawing;
using System.Windows.Forms;
namespace ImagePicker
{
    partial class EmoteSearchForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        TextBox queryInput = new TextBox();
        ListView emoteList = new ListView();

        //task bar icon
        NotifyIcon taskbarIcon;
        ContextMenuStrip taskbarMenu = new ContextMenuStrip();

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            CancelFill();
            if (disposing && components != null)
            {
                components.Dispose();
            }
            taskbarIcon.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //run component init code
            InitQueryInput();
            InitEmoteList();

            //form
            //AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new Size(524, 225);
            Controls.Add(emoteList);
            Controls.Add(queryInput);
            BackColor = Color.FromArgb(32, 32, 40);
            //Opacity = 0.8D;
            ControlBox = false;
            ShowIcon = false;
            KeyPreview = true;
            KeyDown += new KeyEventHandler(Form_KeyDown);
            //ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;

            UpdateImageDataArray();
            InitTaskbarIcon();

            ResumeLayout(false);
            PerformLayout();
        }

        private void InitTaskbarIcon()
        {
            taskbarIcon = new NotifyIcon();

            taskbarMenu.ShowImageMargin = false;
            taskbarMenu.BackColor = Color.FromArgb(32, 32, 40);
            taskbarMenu.ForeColor = Color.WhiteSmoke;

            taskbarMenu.Items.Add(
                new ToolStripMenuItem("Exit", null, new EventHandler((o, e) => Program.Exit()))
            );

            taskbarIcon.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ImagePicker.res.app.ico"));
            taskbarIcon.ContextMenuStrip = taskbarMenu;
            taskbarIcon.Visible = true;
        }

        private void InitEmoteList()
        {
            emoteList.Name = "Emote List";
            emoteList.Location = new Point(6, 6);
            emoteList.Size = new Size(512, 184);
            emoteList.BackColor = Color.FromArgb(60, 60, 68);
            emoteList.ForeColor = Color.WhiteSmoke;
            emoteList.TabIndex = 1;
            emoteList.MultiSelect = false;
            emoteList.UseCompatibleStateImageBehavior = true;
            emoteList.HeaderStyle = ColumnHeaderStyle.None;
            emoteList.View = View.LargeIcon;
            emoteList.HideSelection = false;
            emoteList.SetDoubleBuffer(true);
        }

        private void InitQueryInput()
        {
            queryInput.Name = "Query Input";
            queryInput.Location = new Point(6, 198);
            queryInput.Size = new Size(512, 20);
            queryInput.BackColor = Color.FromArgb(90, 90, 95);
            queryInput.ForeColor = Color.WhiteSmoke;
            queryInput.TextChanged += new EventHandler(QueryEmote);
            queryInput.TabIndex = 0;
        }

        #endregion
    }
}

