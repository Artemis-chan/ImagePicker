using System;
using System.Drawing;
using System.Windows.Forms;
namespace emote_gui_dotnet_win
{
    partial class EmoteSearchForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        TextBox queryInput;
        ListView emoteList;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            queryInput = new TextBox();
            emoteList = new ListView();

            //queryInput
            InitQueryInput();
            //list
            InitEmoteList();

            //form
            //AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(524, 225);
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

            ResumeLayout(false);
            PerformLayout();

        }

        private void InitEmoteList()
        {
            emoteList.Name = "Emote List";
            emoteList.Location = new System.Drawing.Point(6, 6);
            emoteList.Size = new System.Drawing.Size(512, 184);
            emoteList.BackColor = Color.FromArgb(60, 60, 68);
            emoteList.ForeColor = Color.WhiteSmoke;
            emoteList.TabIndex = 1;
            emoteList.MultiSelect = false;
            emoteList.UseCompatibleStateImageBehavior = true;
            emoteList.HeaderStyle = ColumnHeaderStyle.None;
            emoteList.View = View.LargeIcon;
            emoteList.HideSelection = false;
        }

        private void InitQueryInput()
        {
            queryInput.Name = "Query Input";
            queryInput.Location = new System.Drawing.Point(6, 198);
            queryInput.Size = new System.Drawing.Size(512, 20);
            queryInput.BackColor = Color.FromArgb(90, 90, 95);
            queryInput.ForeColor = Color.WhiteSmoke;
            queryInput.TextChanged += new System.EventHandler(QueryEmote);
            queryInput.TabIndex = 0;
        }

        #endregion
    }
}

