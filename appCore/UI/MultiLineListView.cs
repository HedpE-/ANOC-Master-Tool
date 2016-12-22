/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 21/12/2016
 * Time: 21:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataStripper
{
    public partial class MultiLineListView : System.Windows.Forms.ListBox
    {
        public MultiLineListView()
        {
            //InitializeComponent();
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.ScrollAlwaysVisible = true;
            tbox.Hide();
            tbox.mllb = this;
            Controls.Add(tbox);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            if (Site != null)
                return;
            if (e.Index > -1)
            {
                string s = Items[e.Index].ToString();
                float best = 0;
                foreach (string line in s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    float chk = e.Graphics.MeasureString(line, Font, Width).Width;
                    if (chk > best)
                        best = chk;
                }
                SizeF sf = e.Graphics.MeasureString(s, Font, Width);
                int htex = 1;//(e.Index == 0) ? 15 : 10;
                e.ItemHeight = (int)(sf.Height*Items.Count) + htex;
                e.ItemWidth = (int)best;
                /*NTextBox i = (NTextBox)Items[e.Index];
                e.ItemHeight = i.Height;
                e.ItemWidth = i.Width;*/
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Site != null)
                return;
            if (e.Index > -1)
            {
                string s = Items[e.Index].ToString();

                if ((e.State & DrawItemState.Focus) == 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);
                    e.Graphics.DrawString(s, Font, new SolidBrush(SystemColors.WindowText),
                        e.Bounds);
                    e.Graphics.DrawRectangle(new Pen(SystemColors.Highlight), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                    e.Graphics.DrawString(s, Font, new SolidBrush(SystemColors.HighlightText),
                        e.Bounds);
                }
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            int index = IndexFromPoint(e.X, e.Y);

            if (index != ListBox.NoMatches &&
                index != 65535)
            {

                /*if (e.Button == MouseButtons.Right)
                {
                    SelectedIndex = index;
                    Focus();
                    //tbox.index = index;
                }*/
                /*if (e.Button == MouseButtons.Right)
                {

                    string s = Items[index].ToString();
                    Rectangle rect = GetItemRectangle(index);

                    tbox.Location = new Point(rect.X, rect.Y);
                    tbox.Size = new Size(rect.Width, rect.Height);
                    tbox.Text = s;
                    tbox.index = index;
                    tbox.SelectAll();
                    tbox.Show();
                    tbox.Focus();
                }*/
            }

            base.OnMouseUp(e);
        }

        NTextBox tbox = new NTextBox();

        class NTextBox : TextBox
        {
            public MultiLineListView mllb;
            public int index = -1;

            bool errshown = false;
            bool brementer = false;

            public NTextBox()
            {
                Multiline = true;
                MaxLength = 2147483647;
                MaximumSize = new System.Drawing.Size(0, 0);
                WordWrap = false;
                ScrollBars = ScrollBars.Both;
                AcceptsReturn = true;
                AcceptsTab = true;
            }

            protected override void OnKeyUp(KeyEventArgs e)
            {
                if (brementer)
                {
                    Text = "";
                    brementer = false;
                }
                base.OnKeyUp(e);
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);
                /*if (e.KeyChar == (int)Keys.Return)
                {
                    if (Text.Trim() == "")
                    {
                        errshown = true;
                        brementer = true;

                        MessageBox.Show(
                            "Cannot enter NULL string as item!",
                            "Fatal error!", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        errshown = false;
                        mllb.Items[index] = Text;
                        Hide();
                    }

                }*/

                /*if (e.KeyChar == (int)Keys.Escape)
                {
                    Text = mllb.Items[index].ToString();
                    Hide();
                    mllb.SelectedIndex = index;
                }*/

            }

            protected override void OnLostFocus(System.EventArgs e)
            {

                if (Text.Trim() == "")
                {
                    if (!errshown)
                    {
                        MessageBox.Show(
                            "Cannot enter NULL string as item!",
                            "Fatal error!", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    errshown = false;
                }
                else
                {
                    errshown = false;
                    mllb.Items[index] = Text;
                    Hide();
                }
                base.OnLostFocus(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                int index = SelectedIndex;
                if (index == ListBox.NoMatches ||
                    index == 65535)
                {
                    if (Items.Count > 0)
                        index = 0;
                }
                if (index != ListBox.NoMatches &&
                    index != 65535)
                {

                    string s = Items[index].ToString();
                    Rectangle rect = GetItemRectangle(index);

                    tbox.Location = new Point(rect.X, rect.Y);
                    tbox.Size = new Size(rect.Width, rect.Height);
                    tbox.Text = s;
                    tbox.index = index;
                    tbox.SelectAll();
                    tbox.Show();
                    tbox.Focus();
                }
            }
            base.OnKeyDown(e);
        }
    }
}