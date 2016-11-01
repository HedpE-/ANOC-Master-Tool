using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace appCore
{
	public class BrowserView : Form
	{
		private bool _lockedCellsPanelEnabled;

		private string OIUsername = string.Empty;

		private string OIPassword = string.Empty;

		private string SettingsFile = string.Empty;

		private List<HtmlElement> cells2GCheckboxes_wb2 = new List<HtmlElement>();

		private List<HtmlElement> cells3GCheckboxes_wb2 = new List<HtmlElement>();

		private List<HtmlElement> cells4GCheckboxes_wb2 = new List<HtmlElement>();

		private List<HtmlElement> cells2GCheckboxes_wb3 = new List<HtmlElement>();

		private List<HtmlElement> cells3GCheckboxes_wb3 = new List<HtmlElement>();

		private List<HtmlElement> cells4GCheckboxes_wb3 = new List<HtmlElement>();

		private List<HtmlElement> lockCellsCheckboxes = new List<HtmlElement>();

		private bool[] wb2LockCellsPanel;

		private bool[] wb3LockCellsPanel;

		private IContainer components;

		private TabControl tabControl1;

		private TabPage tabPage1;

		private TabPage tabPage2;

		private TabPage tabPage3;

		private WebBrowser webBrowser1;

		private WebBrowser webBrowser2;

		private WebBrowser webBrowser3;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private PictureBox pictureBox3;

		private PictureBox pictureBox4;

		private PictureBox pictureBox5;

		private ComboBox comboBox1;

		private CheckBox checkBox1;

		private GroupBox groupBox1;

		private CheckBox checkBox4;

		private CheckBox checkBox3;

		private CheckBox checkBox2;

		public bool lockedCellsPanelEnabled
		{
			get
			{
				return this._lockedCellsPanelEnabled;
			}
			set
			{
				this._lockedCellsPanelEnabled = value;
				if (value)
				{
					switch (this.tabControl1.SelectedIndex)
					{
						case 1:
							this.groupBox1.Visible |= (this.wb2LockCellsPanel != null);
							return;
						case 2:
							this.groupBox1.Visible |= (this.wb3LockCellsPanel != null);
							return;
						default:
							return;
					}
				}
				else
				{
					switch (this.tabControl1.SelectedIndex)
					{
						case 1:
							this.groupBox1.Visible = false;
							return;
						case 2:
							this.groupBox1.Visible = false;
							return;
						default:
							return;
					}
				}
			}
		}

        public IContainer Components
        {
            get
            {
                return components;
            }

            set
            {
                components = value;
            }
        }

        public BrowserView()
		{
			this.InitializeComponent();
			this.OIUsername = Settings.SettingsFile.OIUsername;
			this.OIPassword = Settings.SettingsFile.OIPassword;
			this.tabControl1.SelectTab(1);
			this.webBrowser1.Navigate("https://st.internal.vodafone.co.uk/");
			Web.UI.AuthForm authForm = new Web.UI.AuthForm("OI");
			authForm.StartPosition = FormStartPosition.CenterParent;
			authForm.ShowDialog();
			if (!string.IsNullOrEmpty(authForm.Username))
			{
				if (authForm.Username != this.OIUsername)
				{
					this.OIUsername = authForm.Username;
					DialogResult dialogResult = MessageBox.Show(string.Concat(new string[]
					                                                          {
					                                                          	"Stored OI Credentials: ",
					                                                          	this.OIUsername,
					                                                          	Environment.NewLine,
					                                                          	Environment.NewLine,
					                                                          	"You entered different credentials, do you want to overwrite the stored information?"
					                                                          }), "OI credentials", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (dialogResult == DialogResult.Yes)
					{
						Settings.SettingsFile.OIUsername = this.OIUsername;
					}
					if ((authForm.Password != this.OIPassword) && dialogResult == DialogResult.Yes)
					{
						this.OIPassword = authForm.Password;
						Settings.SettingsFile.OIPassword = this.OIPassword;
					}
				}
			}
			this.webBrowser2.DocumentCompleted += this.WebBrowserDocumentCompleted;
			this.webBrowser2.Navigate("http://operationalintelligence.vf-uk.corp.vodafone.com/SSO/?action=login");
			this.comboBox1.SelectedIndex = 0;
		}

		private void WebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			WebBrowser webBrowser = (WebBrowser)sender;
			if (webBrowser.Name == "webBrowser2" || webBrowser.Name == "webBrowser3")
			{
				if (webBrowser.Name == "webBrowser2")
				{
					this.wb2LockCellsPanel = null;
					this.cells2GCheckboxes_wb2.Clear();
					this.cells3GCheckboxes_wb2.Clear();
					this.cells4GCheckboxes_wb2.Clear();
				}
				else
				{
					this.wb3LockCellsPanel = null;
					this.cells2GCheckboxes_wb3.Clear();
					this.cells3GCheckboxes_wb3.Clear();
					this.cells4GCheckboxes_wb3.Clear();
					this.lockCellsCheckboxes.Clear();
				}
				this.groupBox1.Visible &= !this.lockedCellsPanelEnabled;
				if (webBrowser.Url.ToString().Contains("operationalintelligence.vf-uk.corp.vodafone.com") || webBrowser.Url.ToString().Contains("195.233.194.118"))
				{
					bool flag = this.checkOILogin(webBrowser);
					if (flag)
					{
						return;
					}
					if (webBrowser.Url.ToString().Contains("/site/index.php") || webBrowser.Url.ToString().EndsWith("/site", StringComparison.Ordinal))
					{
						HtmlElement elementById = webBrowser.Document.GetElementById("div_cells");
						if (elementById != null)
						{
							this.groupBox1.Visible |= this.lockedCellsPanelEnabled;
							this.checkBox1.Enabled = false;
							this.checkBox2.Enabled = false;
							this.checkBox3.Enabled = false;
							this.checkBox4.Enabled = false;
							if (elementById.InnerHtml.Contains("<TD>2G</TD>"))
							{
								string[] separator = new string[]
								{
									"<TD>2G</TD>"
								};
								string[] array = elementById.InnerHtml.Substring(elementById.InnerHtml.IndexOf("<TD>2G</TD>", StringComparison.Ordinal)).Split(separator, StringSplitOptions.None);
								array = (from x in array
								         where !string.IsNullOrEmpty(x)
								         select x).ToArray<string>();
								string[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									string text = array2[i];
									string text2 = string.Empty;
									string text3 = text.Substring(text.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9);
									for (int j = 0; j < text3.Length; j++)
									{
										char c = text3[j];
										if (c == ' ')
										{
											break;
										}
										text2 += c;
									}
									HtmlElement elementById2 = webBrowser.Document.GetElementById(text2);
									if (elementById2.GetAttribute("disabled") == "False")
									{
										if (webBrowser.Name == "webBrowser2")
										{
											this.cells2GCheckboxes_wb2.Add(elementById2);
										}
										else
										{
											this.cells2GCheckboxes_wb3.Add(elementById2);
										}
									}
								}
							}
							if (elementById.InnerHtml.Contains("<TD>3G</TD>"))
							{
								string[] separator2 = new string[]
								{
									"<TD>3G</TD>"
								};
								string[] array3 = elementById.InnerHtml.Substring(elementById.InnerHtml.IndexOf("<TD>3G</TD>", StringComparison.Ordinal)).Split(separator2, StringSplitOptions.None);
								array3 = (from x in array3
								          where !string.IsNullOrEmpty(x)
								          select x).ToArray<string>();
								string[] array4 = array3;
								for (int k = 0; k < array4.Length; k++)
								{
									string text4 = array4[k];
									string text5 = string.Empty;
									string text6 = text4.Substring(text4.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9);
									for (int l = 0; l < text6.Length; l++)
									{
										char c2 = text6[l];
										if (c2 == ' ')
										{
											break;
										}
										text5 += c2;
									}
									HtmlElement elementById3 = webBrowser.Document.GetElementById(text5);
									if (elementById3.GetAttribute("disabled") == "False")
									{
										if (webBrowser.Name == "webBrowser2")
										{
											this.cells3GCheckboxes_wb2.Add(elementById3);
										}
										else
										{
											this.cells3GCheckboxes_wb3.Add(elementById3);
										}
									}
								}
							}
							if (elementById.InnerHtml.Contains("<TD>4G</TD>"))
							{
								string[] separator3 = new string[]
								{
									"<TD>4G</TD>"
								};
								string[] array5 = elementById.InnerHtml.Substring(elementById.InnerHtml.IndexOf("<TD>4G</TD>", StringComparison.Ordinal)).Split(separator3, StringSplitOptions.None);
								array5 = (from x in array5
								          where !string.IsNullOrEmpty(x)
								          select x).ToArray<string>();
								string[] array6 = array5;
								for (int m = 0; m < array6.Length; m++)
								{
									string text7 = array6[m];
									string text8 = string.Empty;
									string text9 = text7.Substring(text7.IndexOf("INPUT id=checkbox", StringComparison.Ordinal) + 9);
									for (int n = 0; n < text9.Length; n++)
									{
										char c3 = text9[n];
										if (c3 == ' ')
										{
											break;
										}
										text8 += c3;
									}
									HtmlElement elementById4 = webBrowser.Document.GetElementById(text8);
									if (elementById4.GetAttribute("disabled") == "False")
									{
										if (webBrowser.Name == "webBrowser2")
										{
											this.cells4GCheckboxes_wb2.Add(elementById4);
										}
										else
										{
											this.cells4GCheckboxes_wb3.Add(elementById4);
										}
									}
								}
							}
							if (webBrowser.Name == "webBrowser2")
							{
								this.checkBox1.Enabled |= this.cells2GCheckboxes_wb2.Any<HtmlElement>();
								this.checkBox2.Enabled |= this.cells3GCheckboxes_wb2.Any<HtmlElement>();
								this.checkBox3.Enabled |= this.cells4GCheckboxes_wb2.Any<HtmlElement>();
							}
							else
							{
								this.checkBox1.Enabled |= this.cells2GCheckboxes_wb3.Any<HtmlElement>();
								this.checkBox2.Enabled |= this.cells3GCheckboxes_wb3.Any<HtmlElement>();
								this.checkBox3.Enabled |= this.cells4GCheckboxes_wb3.Any<HtmlElement>();
							}
							int num = 0;
							foreach (CheckBox checkBox in this.groupBox1.Controls)
							{
								if (checkBox.Enabled)
								{
									num++;
								}
							}
							this.checkBox4.Enabled |= (num > 1);
							foreach (CheckBox checkBox2 in this.groupBox1.Controls)
							{
								checkBox2.Checked = false;
							}
							if (webBrowser.Name == "webBrowser2")
							{
								this.wb2LockCellsPanel = new bool[]
								{
									this.checkBox1.Enabled,
									this.checkBox2.Enabled,
									this.checkBox3.Enabled,
									this.checkBox4.Enabled
								};
								return;
							}
							this.wb3LockCellsPanel = new bool[]
							{
								this.checkBox1.Enabled,
								this.checkBox2.Enabled,
								this.checkBox3.Enabled,
								this.checkBox4.Enabled
							};
							return;
						}
					}
					else if (webBrowser.Url.ToString().Contains("/site/cellslocked.php") || webBrowser.Url.ToString().EndsWith("/site", StringComparison.Ordinal))
					{
						HtmlElement elementById5 = webBrowser.Document.GetElementById("SiteNo");
						if (elementById5.GetAttribute("value") != string.Empty)
						{
							this.groupBox1.Visible |= this.lockedCellsPanelEnabled;
							this.checkBox1.Enabled = false;
							this.checkBox2.Enabled = false;
							this.checkBox3.Enabled = false;
							this.checkBox4.Enabled = false;
							HtmlElement htmlElement = null;
							foreach (HtmlElement htmlElement2 in webBrowser.Document.GetElementsByTagName("form"))
							{
								if (htmlElement2.GetAttribute("name") == elementById5.GetAttribute("value") + "input")
								{
									htmlElement = htmlElement2;
									break;
								}
							}
							string[] separator4 = new string[]
							{
								"<TD><INPUT id="
							};
							string[] array7 = htmlElement.InnerHtml.Split(separator4, StringSplitOptions.None);
							array7 = (from x in array7
							          where !string.IsNullOrEmpty(x)
							          select x).ToArray<string>();
							array7 = array7.Where((string source, int index) => index != 0).ToArray<string>();
							string[] array8 = array7;
							for (int num2 = 0; num2 < array8.Length; num2++)
							{
								string text10 = array8[num2];
								string text11 = string.Empty;
								string text12 = text10;
								for (int num3 = 0; num3 < text12.Length; num3++)
								{
									char c4 = text12[num3];
									if (c4 == ' ')
									{
										break;
									}
									text11 += c4;
								}
								HtmlElement elementById6 = webBrowser.Document.GetElementById(text11);
								if (elementById6.GetAttribute("disabled") == "False")
								{
									this.lockCellsCheckboxes.Add(elementById6);
								}
							}
							if (this.lockCellsCheckboxes.Any<HtmlElement>())
							{
								this.checkBox4.Enabled = true;
								this.wb3LockCellsPanel = new bool[]
								{
									true
								};
							}
						}
					}
				}
			}
		}

		private bool checkOILogin(WebBrowser wb)
		{
			HtmlElementCollection elementsByTagName = wb.Document.GetElementsByTagName("form");
			bool flag = false;
			foreach (HtmlElement htmlElement in elementsByTagName)
			{
				if (htmlElement.GetAttribute("name") == "loginform")
				{
					flag = true;
					break;
				}
			}
			if (flag && !string.IsNullOrEmpty(this.OIUsername) && !string.IsNullOrEmpty(this.OIPassword))
			{
				if (wb.Url.ToString().Contains("http://195.233.194.118"))
				{
					elementsByTagName = wb.Document.GetElementsByTagName("input");
					HtmlElement htmlElement2 = null;
					foreach (HtmlElement htmlElement3 in elementsByTagName)
					{
						string attribute;
						if ((attribute = htmlElement3.GetAttribute("type")) != null)
						{
							if (!(attribute == "text"))
							{
								if (!(attribute == "password"))
								{
									if (attribute == "submit")
									{
										htmlElement2 = htmlElement3;
									}
								}
								else
								{
									htmlElement3.SetAttribute("value", Toolbox.Tools.EncryptDecryptText("Dec", this.OIPassword));
								}
							}
							else
							{
								htmlElement3.SetAttribute("value", this.OIUsername);
							}
						}
					}
					htmlElement2.InvokeMember("Click");
				}
				else
				{
					wb.Document.GetElementById("inpt-ntlgn").SetAttribute("value", this.OIUsername);
					IHTMLDocument2 iHTMLDocument = (IHTMLDocument2)this.webBrowser2.Document.DomDocument;
					IHTMLFormElement iHTMLFormElement = null;
					foreach (IHTMLFormElement iHTMLFormElement2 in iHTMLDocument.forms)
					{
						if (iHTMLFormElement2.name == "loginform")
						{
							iHTMLFormElement = iHTMLFormElement2;
							break;
						}
					}
					IHTMLInputElement iHTMLInputElement = null;
					foreach (IHTMLInputElement iHTMLInputElement2 in iHTMLFormElement)
					{
						if (iHTMLInputElement2.name == "password")
						{
							iHTMLInputElement = iHTMLInputElement2;
							break;
						}
					}
					iHTMLInputElement.select();
					iHTMLInputElement.value = Toolbox.Tools.EncryptDecryptText("Dec", this.OIPassword);
					wb.Document.GetElementById("req_submit").InvokeMember("Click");
				}
				if (wb.Name == "webBrowser3" && (this.webBrowser3.Url.ToString().Contains("195.233.194.118") || this.webBrowser3.Url.ToString().Contains("operationalintelligence.vf-uk.corp.vodafone.com")))
				{
					wb.Navigate(this.NavigateHome());
				}
				else
				{
					wb.Navigate("http://operationalintelligence.vf-uk.corp.vodafone.com/site/");
				}
			}
			return flag;
		}

		private string NavigateHome()
		{
			string text;
			switch (text = this.comboBox1.Text)
			{
				case "SITE Lopedia":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/site/index.php";
				case "SITE Lopedia (Old)":
					return "http://195.233.194.118/site_old/index.php";
				case "Locked Cells":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/site/cellslocked.php";
				case "Locked Cells (Old)":
					return "http://195.233.194.118/site/cellslocked.php";
				case "Sites Off Air":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/site/offair.php";
				case "Vendor Override":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/RANOps/admin/beaconit.php";
				case "Bulk Uploader":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/ukoim/bulk_uploader.php";
				case "ANOC Site Management Diary (Book Ins)":
					return "https://smsproxy.vavs.vodafone.com/anoc/";
				case "COOS - No Unavailability":
					return "http://operationalintelligence.vf-uk.corp.vodafone.com/ranops/coos-tickets.php";
			}
			return string.Empty;
		}

		private void PictureBoxesMouseHover(object sender, EventArgs e)
		{
			PictureBox pictureBox = (PictureBox)sender;
			pictureBox.BorderStyle = BorderStyle.Fixed3D;
		}

		private void PictureBoxesMouseLeave(object sender, EventArgs e)
		{
			PictureBox pictureBox = (PictureBox)sender;
			pictureBox.BorderStyle = BorderStyle.None;
		}

		private void PictureBox1Click(object sender, EventArgs e)
		{
			switch (this.tabControl1.SelectedIndex)
			{
				case 0:
					this.webBrowser1.GoBack();
					return;
				case 1:
					this.webBrowser2.GoBack();
					return;
				case 2:
					this.webBrowser3.GoBack();
					return;
				default:
					return;
			}
		}

		private void PictureBox2Click(object sender, EventArgs e)
		{
			switch (this.tabControl1.SelectedIndex)
			{
				case 0:
					this.webBrowser1.GoForward();
					return;
				case 1:
					this.webBrowser2.GoForward();
					return;
				case 2:
					this.webBrowser3.GoForward();
					return;
				default:
					return;
			}
		}

		private void PictureBox3Click(object sender, EventArgs e)
		{
			switch (this.tabControl1.SelectedIndex)
			{
				case 0:
					this.webBrowser1.Refresh();
					return;
				case 1:
					this.webBrowser2.Refresh();
					return;
				case 2:
					this.webBrowser3.Refresh();
					return;
				default:
					return;
			}
		}

		private void PictureBox4Click(object sender, EventArgs e)
		{
			switch (this.tabControl1.SelectedIndex)
			{
				case 0:
					this.webBrowser1.Navigate("https://st.internal.vodafone.co.uk/");
					return;
				case 1:
					this.webBrowser2.Navigate("http://operationalintelligence.vf-uk.corp.vodafone.com/site/");
					return;
				case 2:
					this.webBrowser3.Navigate(this.NavigateHome());
					return;
				default:
					return;
			}
		}

		private void checkBoxesCheckedState(WebBrowser wb)
		{
			List<HtmlElement> list = new List<HtmlElement>();
			List<HtmlElement> list2 = new List<HtmlElement>();
			List<HtmlElement> list3 = new List<HtmlElement>();
			string name;
			if ((name = wb.Name) != null)
			{
				if (!(name == "webBrowser2"))
				{
					if (name == "webBrowser3")
					{
						list = this.cells2GCheckboxes_wb3;
						list2 = this.cells3GCheckboxes_wb3;
						list3 = this.cells4GCheckboxes_wb3;
					}
				}
				else
				{
					list = this.cells2GCheckboxes_wb2;
					list2 = this.cells3GCheckboxes_wb2;
					list3 = this.cells4GCheckboxes_wb2;
				}
			}
			int i = 1;
			while (i < 5)
			{
				switch (i)
				{
					case 1:
						using (List<HtmlElement>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								HtmlElement current = enumerator.Current;
								if (current.GetAttribute("checked") == "True")
								{
									current.InvokeMember("CLICK");
								}
							}
							break;
						}
//					goto IL_DF;
					case 2:
						goto IL_DF;
					case 3:
						goto IL_130;
				}
				IL_181:
					i++;
				continue;
				IL_DF:
					using (List<HtmlElement>.Enumerator enumerator2 = list2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						HtmlElement current2 = enumerator2.Current;
						if (current2.GetAttribute("checked") == "True")
						{
							current2.InvokeMember("CLICK");
						}
					}
					goto IL_181;
				}
				IL_130:
					foreach (HtmlElement current3 in list3)
				{
					if (current3.GetAttribute("checked") == "True")
					{
						current3.InvokeMember("CLICK");
					}
				}
				goto IL_181;
			}
			if (wb.Name == "webBrowser3" && wb.Url.ToString().Contains("/site/cellslocked.php"))
			{
				foreach (HtmlElement current4 in this.lockCellsCheckboxes)
				{
					if (current4.GetAttribute("checked") == "True")
					{
						current4.InvokeMember("CLICK");
					}
				}
			}
			foreach (CheckBox checkBox in this.groupBox1.Controls)
			{
				checkBox.Checked = false;
			}
		}

		private void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{
			this.comboBox1.Visible = (this.tabControl1.SelectedIndex == 2);
			this.Text = "AMT Browser - " + this.tabControl1.SelectedTab.Text;
			this.checkBox1.CheckedChanged -= new EventHandler(this.CheckBox1CheckedChanged);
			this.checkBox2.CheckedChanged -= new EventHandler(this.CheckBox2CheckedChanged);
			this.checkBox3.CheckedChanged -= new EventHandler(this.CheckBox3CheckedChanged);
			this.checkBox4.CheckedChanged -= new EventHandler(this.CheckBox4CheckedChanged);
			if (this.tabControl1.SelectedIndex == 1)
			{
				if (this.wb2LockCellsPanel != null)
				{
					this.groupBox1.Visible |= this.lockedCellsPanelEnabled;
					for (int i = 1; i < 5; i++)
					{
						this.groupBox1.Controls["checkBox" + i].Enabled = this.wb2LockCellsPanel[i - 1];
					}
					this.checkBoxesCheckedState(this.webBrowser2);
				}
				else
				{
					this.groupBox1.Visible &= !this.lockedCellsPanelEnabled;
				}
			}
			else if (this.tabControl1.SelectedIndex == 2)
			{
				if (this.wb3LockCellsPanel != null)
				{
					this.groupBox1.Visible |= this.lockedCellsPanelEnabled;
					if (this.wb3LockCellsPanel.Length > 1)
					{
						for (int j = 1; j < 5; j++)
						{
							this.groupBox1.Controls["checkBox" + j].Enabled = this.wb3LockCellsPanel[j - 1];
						}
						this.checkBoxesCheckedState(this.webBrowser3);
					}
					else
					{
						for (int k = 1; k < 5; k++)
						{
							if (k != 4)
							{
								this.groupBox1.Controls["checkBox" + k].Enabled = false;
							}
							else
							{
								this.checkBox4.Enabled = this.wb3LockCellsPanel[0];
							}
						}
						this.checkBoxesCheckedState(this.webBrowser3);
					}
				}
				else
				{
					this.groupBox1.Visible &= !this.lockedCellsPanelEnabled;
				}
			}
			this.checkBox1.CheckedChanged += new EventHandler(this.CheckBox1CheckedChanged);
			this.checkBox2.CheckedChanged += new EventHandler(this.CheckBox2CheckedChanged);
			this.checkBox3.CheckedChanged += new EventHandler(this.CheckBox3CheckedChanged);
			this.checkBox4.CheckedChanged += new EventHandler(this.CheckBox4CheckedChanged);
		}

		private void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
		}

		private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			this.webBrowser3.Navigate(this.NavigateHome());
			this.tabPage3.Text = this.comboBox1.Text;
		}

		private void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> list = new List<HtmlElement>();
			list = ((this.tabControl1.SelectedIndex == 1) ? this.cells2GCheckboxes_wb2 : this.cells2GCheckboxes_wb3);
			foreach (HtmlElement current in list)
			{
				current.InvokeMember("CLICK");
			}
		}

		private void CheckBox2CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> list = new List<HtmlElement>();
			list = ((this.tabControl1.SelectedIndex == 1) ? this.cells3GCheckboxes_wb2 : this.cells3GCheckboxes_wb3);
			foreach (HtmlElement current in list)
			{
				current.InvokeMember("CLICK");
			}
		}

		private void CheckBox3CheckedChanged(object sender, EventArgs e)
		{
			List<HtmlElement> list = new List<HtmlElement>();
			list = ((this.tabControl1.SelectedIndex == 1) ? this.cells4GCheckboxes_wb2 : this.cells4GCheckboxes_wb3);
			foreach (HtmlElement current in list)
			{
				current.InvokeMember("CLICK");
			}
		}

		private void CheckBox4CheckedChanged(object sender, EventArgs e)
		{
			if (this.tabControl1.SelectedIndex == 1 || (this.tabControl1.SelectedIndex == 2 && this.webBrowser3.Url.ToString().Contains("/site/index.php")))
			{
				this.CheckBox1CheckedChanged(null, null);
				this.CheckBox2CheckedChanged(null, null);
				this.CheckBox3CheckedChanged(null, null);
				return;
			}
			if (this.tabControl1.SelectedIndex == 2 && this.webBrowser3.Url.ToString().Contains("/site/cellslocked.php"))
			{
				foreach (HtmlElement current in this.lockCellsCheckboxes)
				{
					current.InvokeMember("CLICK");
				}
			}
		}

		private void PictureBox5Click(object sender, EventArgs e)
		{
			MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
			if (mouseEventArgs.Button == MouseButtons.Right)
			{
				if (Control.ModifierKeys.ToString().Contains("Shift") && Control.ModifierKeys.ToString().Contains("Control"))
				{
					this.lockedCellsPanelEnabled = !this.lockedCellsPanelEnabled;
					return;
				}
			}
			else
			{
				this.lockedCellsPanelEnabled = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
            if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tabControl1 = new TabControl();
			this.tabPage1 = new TabPage();
			this.webBrowser1 = new WebBrowser();
			this.tabPage2 = new TabPage();
			this.webBrowser2 = new WebBrowser();
			this.tabPage3 = new TabPage();
			this.webBrowser3 = new WebBrowser();
			this.pictureBox1 = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.pictureBox3 = new PictureBox();
			this.pictureBox4 = new PictureBox();
			this.pictureBox5 = new PictureBox();
			this.comboBox1 = new ComboBox();
			this.checkBox1 = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.checkBox4 = new CheckBox();
			this.checkBox3 = new CheckBox();
			this.checkBox2 = new CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox3).BeginInit();
			((ISupportInitialize)this.pictureBox4).BeginInit();
			((ISupportInitialize)this.pictureBox5).BeginInit();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.tabControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new Point(0, 68);
			this.tabControl1.Margin = new Padding(0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(996, 564);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new EventHandler(this.TabControl1SelectedIndexChanged);
			this.tabPage1.Controls.Add(this.webBrowser1);
			this.tabPage1.Location = new Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new Padding(3);
			this.tabPage1.Size = new Size(988, 538);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Citrix";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.webBrowser1.AllowWebBrowserDrop = false;
			this.webBrowser1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.webBrowser1.Location = new Point(3, 3);
			this.webBrowser1.MinimumSize = new Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.ScriptErrorsSuppressed = true;
			this.webBrowser1.Size = new Size(982, 532);
			this.webBrowser1.TabIndex = 0;
			this.tabPage2.Controls.Add(this.webBrowser2);
			this.tabPage2.Location = new Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new Padding(3);
			this.tabPage2.Size = new Size(988, 538);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "SITE Lopedia";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.webBrowser2.AllowWebBrowserDrop = false;
			this.webBrowser2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.webBrowser2.Location = new Point(3, 3);
			this.webBrowser2.MinimumSize = new Size(20, 20);
			this.webBrowser2.Name = "webBrowser2";
			this.webBrowser2.ScriptErrorsSuppressed = true;
			this.webBrowser2.Size = new Size(982, 532);
			this.webBrowser2.TabIndex = 0;
			this.webBrowser2.Navigating += new WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
			this.tabPage3.Controls.Add(this.webBrowser3);
			this.tabPage3.Location = new Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new Padding(3);
			this.tabPage3.Size = new Size(988, 538);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "SITE Lopedia";
			this.tabPage3.UseVisualStyleBackColor = true;
			this.webBrowser3.AllowWebBrowserDrop = false;
			this.webBrowser3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.webBrowser3.Location = new Point(3, 3);
			this.webBrowser3.MinimumSize = new Size(20, 20);
			this.webBrowser3.Name = "webBrowser3";
			this.webBrowser3.ScriptErrorsSuppressed = true;
			this.webBrowser3.Size = new Size(982, 532);
			this.webBrowser3.TabIndex = 1;
			this.webBrowser3.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.WebBrowserDocumentCompleted);
			this.pictureBox1.Image = appCore.UI.Resources.arrow_left;
			this.pictureBox1.Location = new Point(4, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(40, 40);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new EventHandler(this.PictureBox1Click);
			this.pictureBox1.MouseLeave += new EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox1.MouseHover += new EventHandler(this.PictureBoxesMouseHover);
			this.pictureBox2.Image = appCore.UI.Resources.arrow_right;
			this.pictureBox2.Location = new Point(50, 12);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(40, 40);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new EventHandler(this.PictureBox2Click);
			this.pictureBox2.MouseLeave += new EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox2.MouseHover += new EventHandler(this.PictureBoxesMouseHover);
			this.pictureBox3.Image = appCore.UI.Resources.refresh;
			this.pictureBox3.Location = new Point(96, 12);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new Size(40, 40);
			this.pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox3.TabIndex = 3;
			this.pictureBox3.TabStop = false;
			this.pictureBox3.Click += new EventHandler(this.PictureBox3Click);
			this.pictureBox3.MouseLeave += new EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox3.MouseHover += new EventHandler(this.PictureBoxesMouseHover);
			this.pictureBox4.Image = appCore.UI.Resources.home;
			this.pictureBox4.Location = new Point(142, 12);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new Size(40, 40);
			this.pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox4.TabIndex = 4;
			this.pictureBox4.TabStop = false;
			this.pictureBox4.Click += new EventHandler(this.PictureBox4Click);
			this.pictureBox4.MouseLeave += new EventHandler(this.PictureBoxesMouseLeave);
			this.pictureBox4.MouseHover += new EventHandler(this.PictureBoxesMouseHover);
			this.pictureBox5.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.pictureBox5.Image = appCore.UI.Resources.Badass_browser_1;
			this.pictureBox5.Location = new Point(912, 4);
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.Size = new Size(80, 80);
			this.pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox5.TabIndex = 5;
			this.pictureBox5.TabStop = false;
			this.pictureBox5.Click += new EventHandler(this.PictureBox5Click);
			this.comboBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[]
			                              {
			                              	"SITE Lopedia",
			                              	"SITE Lopedia (Old)",
			                              	"Locked Cells (Old)",
			                              	"Locked Cells",
			                              	"Sites Off Air",
			                              	"Vendor Override",
			                              	"Bulk Uploader",
			                              	"ANOC Site Management Diary (Book Ins)",
			                              	"COOS - No Unavailability"
			                              });
			this.comboBox1.Location = new Point(675, 4);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new Size(231, 21);
			this.comboBox1.TabIndex = 6;
			this.comboBox1.Visible = false;
			this.comboBox1.SelectedIndexChanged += new EventHandler(this.ComboBox1SelectedIndexChanged);
			this.checkBox1.Location = new Point(6, 19);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new Size(40, 15);
			this.checkBox1.TabIndex = 7;
			this.checkBox1.Text = "2G";
			this.checkBox1.TextAlign = ContentAlignment.TopLeft;
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new EventHandler(this.CheckBox1CheckedChanged);
			this.groupBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.groupBox1.Controls.Add(this.checkBox4);
			this.groupBox1.Controls.Add(this.checkBox3);
			this.groupBox1.Controls.Add(this.checkBox2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Location = new Point(537, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(132, 79);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select cells";
			this.groupBox1.Visible = false;
			this.checkBox4.Location = new Point(65, 38);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new Size(61, 15);
			this.checkBox4.TabIndex = 10;
			this.checkBox4.Text = "All cells";
			this.checkBox4.TextAlign = ContentAlignment.TopLeft;
			this.checkBox4.UseVisualStyleBackColor = true;
			this.checkBox4.CheckedChanged += new EventHandler(this.CheckBox4CheckedChanged);
			this.checkBox3.Location = new Point(6, 58);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new Size(40, 15);
			this.checkBox3.TabIndex = 9;
			this.checkBox3.Text = "4G";
			this.checkBox3.TextAlign = ContentAlignment.TopLeft;
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new EventHandler(this.CheckBox3CheckedChanged);
			this.checkBox2.Location = new Point(6, 38);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new Size(40, 15);
			this.checkBox2.TabIndex = 8;
			this.checkBox2.Text = "3G";
			this.checkBox2.TextAlign = ContentAlignment.TopLeft;
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new EventHandler(this.CheckBox2CheckedChanged);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(992, 628);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.comboBox1);
			base.Controls.Add(this.pictureBox5);
			base.Controls.Add(this.pictureBox4);
			base.Controls.Add(this.pictureBox3);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.tabControl1);
			base.Icon = appCore.UI.Resources.Badass_browser;
			this.MinimumSize = new Size(1000, 655);
			base.Name = "BrowserView";
			this.Text = "AMT Browser - Citrix";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox3).EndInit();
			((ISupportInitialize)this.pictureBox4).EndInit();
			((ISupportInitialize)this.pictureBox5).EndInit();
			this.groupBox1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
