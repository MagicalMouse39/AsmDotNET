using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

namespace AsmDotNET
{
    public class SearchForm : Form
    {
        private MainWindow win;

        protected Button cancelBtn, goBtn;
        protected RichTextBox searchBox;
        protected CheckBox caseSensitiveBox;

        private int index = 0;

        public SearchForm(MainWindow win)
        {
            this.win = win;

            this.cancelBtn = new Button();
            this.goBtn = new Button();
            this.searchBox = new RichTextBox();
            this.caseSensitiveBox = new CheckBox();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;

            this.BackColor = Color.FromArgb(255, 50, 50, 60);

            this.Text = "Search";

            this.Width = 400;
            this.Height = 200;

            this.caseSensitiveBox.Top = 50;
            this.caseSensitiveBox.Text = "Case sensitive";
            this.caseSensitiveBox.FlatStyle = FlatStyle.Flat;
            this.caseSensitiveBox.ForeColor = Color.White;
            this.caseSensitiveBox.Left = 5;

            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.Left = 5;
            this.cancelBtn.Top = this.Height - this.cancelBtn.Height - 40 - 5;
            this.cancelBtn.FlatStyle = FlatStyle.Flat;
            this.cancelBtn.ForeColor = Color.White;
            this.cancelBtn.Click += (s, e) => this.Close();

            this.goBtn.Left = this.Width - this.goBtn.Width - 15 - 5;
            this.goBtn.Text = "Go";
            this.goBtn.Top = this.Height - this.goBtn.Height - 40 - 5;
            this.goBtn.FlatStyle = FlatStyle.Flat;
            this.goBtn.ForeColor = Color.White;
            this.goBtn.Click += (s, e) =>
            {
                bool cs = this.caseSensitiveBox.Checked;
                string text = cs ? this.win.CodeBox.GetText() : this.win.CodeBox.GetText().ToLower();
                string regex = cs ? this.searchBox.Text : this.searchBox.Text.ToLower();
                if (text.Contains(regex))
                {
                    List<int> indexes = text.AllIndexesOf(regex);
                    this.win.CodeBox.Focus();
                    for (int i = 0; i < indexes.Count; i++)
                    {
                        if (!indexes.Contains(this.index))
                            this.index = indexes[0];
                        else if (indexes[i] == this.index)
                        {
                            if (indexes[i] == indexes[indexes.Count - 1])
                                this.index = indexes[0];
                            else
                                this.index = indexes[i + 1];
                            break;
                        }
                    }
                    this.win.CodeBox.CaretPosition = this.win.CodeBox.Document.ContentStart;
                    this.win.CodeBox.CaretPosition = this.win.CodeBox.CaretPosition.GetPositionAtOffset(index);
                    this.win.CodeBox.ScrollToVerticalOffset(index);
                    var range = this.win.CodeBox.Selection;
                    var start = this.win.CodeBox.Document.ContentStart;
                    var startPos = start.GetPositionAtOffset(this.index + 2);
                    var endPos = start.GetPositionAtOffset(this.index + this.searchBox.Text.Length + 2);
                    range.Select(startPos, endPos);
                }
            };

            this.searchBox.Top = 20;
            this.searchBox.Left = 10;
            this.searchBox.Width = this.Width - 40;
            this.searchBox.Height = 20;
            this.searchBox.Multiline = false;
            this.searchBox.BorderStyle = BorderStyle.None;
            this.searchBox.ForeColor = Color.White;
            this.searchBox.BackColor = Color.FromArgb(255, 30, 30, 40);

            this.AcceptButton = this.goBtn;
            this.CancelButton = this.cancelBtn;

            this.Controls.Add(cancelBtn);
            this.Controls.Add(goBtn);
            this.Controls.Add(searchBox);
            this.Controls.Add(caseSensitiveBox);

            this.Load += (s, e) => this.searchBox.Focus();
        }
    }
}
