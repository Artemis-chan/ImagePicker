using System;
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
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(660, 230);
            Controls.Add(emoteList);
            Controls.Add(queryInput);
            Opacity = 0.8D;
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
            emoteList.Location = new System.Drawing.Point(12, 12);
            emoteList.Name = "Emote List";
            emoteList.Size = new System.Drawing.Size(636, 178);
            emoteList.TabIndex = 1;
            emoteList.Columns.Add("Emote", 634);
            emoteList.MultiSelect = false;
            emoteList.UseCompatibleStateImageBehavior = false;
            emoteList.View = View.Details;
        }

        private void InitQueryInput()
        {
            queryInput.Location = new System.Drawing.Point(12, 196);
            queryInput.Name = "Query Input";
            queryInput.Size = new System.Drawing.Size(636, 20);
            queryInput.TabIndex = 0;
            queryInput.TextChanged += new System.EventHandler(QueryEmote);
        }

        #endregion
    }
}

