/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 06-08-2016
 * Time: 03:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTRichTextBox.
	/// </summary>
	public class AMTRichTextBox : RichTextBox
	{
		public string TopBottomEmptyLinesRemove {
			get {
				return string.Join(Environment.NewLine, stripTopBottomWhiteLinesOnArray());
			}
			private set { }
		}
//		Stack<string> undoList = new Stack<string>();
//		Stack<string> redoList = new Stack<string>();
				
		string[] stripTopBottomWhiteLinesOnArray() {
			List<string> linesList = Lines.ToList();
			
			int start = 0, end = linesList.Count - 1;

			while (start < end && string.IsNullOrWhiteSpace(linesList[start])) start++;
			while (end >= start && string.IsNullOrWhiteSpace(linesList[end])) end--;

			return linesList.Skip(start).Take(end - start + 1).ToArray();
		}
		
		// TODO: Translate API
//		prtected override void OnTextChanged(EventArgs e)
//		{
//			undoList.Push(Text);
//		}

		void RichTextBox_CtrlVAZYFix(KeyEventArgs e)
		{
			if(e.Control && (e.KeyCode == Keys.A || e.KeyCode == Keys.V || e.KeyCode == Keys.Z || e.KeyCode == Keys.Y || e.KeyCode == Keys.Back)) {
				// suspend layout to avoid blinking
				SuspendLayout();

				switch(e.KeyCode) {
					case Keys.V:
						if(!ReadOnly) {
							if(Name.Contains("Address"))
								MultiLineAddressFix();
							
							// get insertion point
							if (SelectionLength > 1) {
								SelectedText = ""; // clear selected text before paste
							}
							int insPt = SelectionStart;

							// preserve text from after insertion pont to end of RTF content
							string postRTFContent = Text.Substring(insPt);

							// remove the content after the insertion point
							Text = Text.Substring(0, insPt);

							// add the clipboard content and then the preserved postRTF content
							Text += (string)Clipboard.GetData("Text") + postRTFContent;

							// adjust the insertion point to just after the inserted text
							SelectionStart = TextLength - postRTFContent.Length;

							// cancel the paste
							e.Handled = true;
						}
						break;
					case Keys.Back:
						e.SuppressKeyPress = true;
						int selStart = SelectionStart;
						while (selStart > 0 && Text.Substring(selStart - 1, 1) == " ")
						{
							selStart--;
						}
						int prevSpacePos = -1;
						if (selStart != 0)
						{
							prevSpacePos = Text.LastIndexOf(' ', selStart - 1);
						}
						Select(prevSpacePos + 1, SelectionStart - prevSpacePos - 1);
						SelectedText = "";
						e.Handled = true;
						break;
					case Keys.A:
						SelectAll();
						Focus();
						break;
						// TODO: Implement UNDO on AMTRichTextBox
//						case Keys.Z:
//							if(undoList.Count > 0) {
//								redoList.Push(Text);
//								Text = undoList.Pop();
//							}
//							break;
//						case Keys.Y:
//							if(redoList.Count > 0) {
//								undoList.Push(Text);
//								Text = redoList.Pop();
//							}
//							break;
				}
				// restore layout
				ResumeLayout();
			}
		}

		void MultiLineAddressFix() {
			string clpbrd = Clipboard.GetData("Text").ToString();
			if (clpbrd.Contains("\r\n")) {
				string[] strTofind = { "\r\n" };
				string[] temp = clpbrd.Split(strTofind, StringSplitOptions.None);
				string finalAddress = string.Empty;
				for (int c = 0; c < temp.Length; c++) {
					if (c == temp.Length - 1) {
						if (temp[c] != "") finalAddress += temp[c];
						else finalAddress = finalAddress.Substring(0,finalAddress.Length - 2);
					}
					else finalAddress += temp[c] + ", ";
				}
				Clipboard.SetText(finalAddress);
			}
		}
		
		protected override void OnKeyDown(KeyEventArgs e) {
			RichTextBox_CtrlVAZYFix(e);
		}
		
		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{   
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem("Cut");
				menuItem.Click += delegate { Cut(); };
				menuItem.Enabled = SelectionLength > 0;
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Copy");
				menuItem.Click += delegate { Copy(); };
				menuItem.Enabled = SelectionLength > 0;
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Paste");
				menuItem.Click += delegate { Paste(); };
				menuItem.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Delete");
				menuItem.Click += delegate { SelectedText = string.Empty; };
				menuItem.Enabled = SelectionLength > 0;
				contextMenu.MenuItems.Add(menuItem);

				ContextMenu = contextMenu;
			}
		}
	}
}
