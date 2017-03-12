/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 03/12/2016
 * Time: 17:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.OI;
using appCore.OI.JSON;
using appCore.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using FileHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of LockUnlockCellsForm.
	/// </summary>
	public partial class LockUnlockCellsForm : Form
	{
		AMTMenuStrip MainMenu;
		
		public Site currentSite;
		List<Site> cellsLockedSites;
		DataTable Table;
		
		int checkedCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells[0].Value != null) {
						if(Convert.ToBoolean(row.Cells[0].Value))
							c++;
					}
				}
				return c;
			}
		}
		
		int gsmCellsCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells["Tech"].Value != null) {
							if(row.Cells["Tech"].Value.ToString() == "2G" && !rowInactive(row))
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		int gsmCheckedCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells[0].Value != null) {
							if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "2G")
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		int umtsCellsCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells["Tech"].Value != null) {
							if(row.Cells["Tech"].Value.ToString() == "3G" && !rowInactive(row))
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		int umtsCheckedCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells[0].Value != null) {
							if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "3G")
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		int lteCellsCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells["Tech"].Value != null) {
							if(row.Cells["Tech"].Value.ToString() == "4G" && !rowInactive(row))
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		int lteCheckedCount {
			get {
				int c = 0;
				try {
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells[0].Value != null) {
							if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "4G")
								c++;
						}
					}
				}
				catch {}
				return c;
			}
		}
		
		string uiMode;
		string UiMode {
			get { return uiMode; }
			set {
				uiMode = value;
				switch(uiMode) {
					case "Lock Cells":
						button1.Text = "Lock\nCells";
						checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => !s.Locked).Count() > 0;
						checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => !s.Locked).Count() > 0;
						checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => !s.Locked).Count() > 0;
						
						comboBox1.Items.Clear();
						
						if(currentSite.Incidents.Count > 0) {
							List<Incident> filteredINCs = currentSite.Incidents.FindAll(s => s.Status != "Closed" && s.Status != "Resolved");
							foreach(Incident inc in filteredINCs)
								comboBox1.Items.Add(inc.Incident_Ref);
						}
						
						if(currentSite.Changes.Count > 0) {
							List<Change> filteredCRQs = currentSite.Changes.FindAll(s => s.Status == "Scheduled" || s.Status != "Implementation in Progress");
							if(filteredCRQs.Count > 0) {
								for(int c = 0;c < filteredCRQs.Count;c++) {
									Change crq = filteredCRQs[c];
									if(!string.IsNullOrEmpty(crq.Scheduled_Start) && !string.IsNullOrEmpty(crq.Scheduled_End)) {
										if (!(Convert.ToDateTime(crq.Scheduled_Start) <=  DateTime.Now && Convert.ToDateTime(crq.Scheduled_End) >= DateTime.Now)) {
											filteredCRQs.RemoveAt(c);
											c--;
										}
									}
								}
							}
							foreach(Change crq in filteredCRQs)
								comboBox1.Items.Add(crq.Change_Ref);
						}
						
						dataGridView1.Width = checkBox1.Left - 12;
						dataGridView1.Columns["Locked"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						checkBookIn();
						label4.Visible = true;
						if(label4.Text.StartsWith("CAUTION")) {
							if(Controls["panel"] != null) {
								Panel curPanel = Controls["panel"] as Panel;
								Controls.Remove(curPanel);
								curPanel.Dispose();
								
								amtRichTextBox1.Height += 87;
							}
							
							amtRichTextBox1.Height -= 87;
							
							Panel panel = new Panel();
							panel.Anchor = ((AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Right));
							panel.BackColor = Color.Transparent;
							panel.Size = new Size(amtRichTextBox1.Width, 87);
							panel.Location = new Point(amtRichTextBox1.Left, amtRichTextBox1.Bottom);
							panel.Name = "panel";
							RadioButton rb1 = new RadioButton();
							rb1.Text = "FE";
							rb1.Name = "rb1";
							rb1.Enabled = false;
							rb1.Width = 40;
							rb1.Location = new Point(3, 5);
							rb1.Checked = true;
							RadioButton rb2 = new RadioButton();
							rb2.Text = "Requested by";
							rb2.Name = "rb2";
							rb2.Enabled = false;
							rb2.Width = 100;
							rb2.Location = new Point(rb1.Right + 5, rb1.Top);
							
							Label nameLb = new Label();
							nameLb.Text = "Name";
							nameLb.Name = "nameLb";
							nameLb.Location = new Point(0, rb1.Bottom + 10);
							nameLb.Width = 50;
							AMTTextBox nameTb = new AMTTextBox();
							nameTb.Name = "nameTb";
							nameTb.Enabled = false;
							nameTb.Location = new Point(nameLb.Right + 3, nameLb.Top);
							nameTb.Width = 209;
							Label contactLb = new Label();
							contactLb.Text = "Contact";
							contactLb.Name = "contactLb";
							contactLb.Location = new Point(0, nameLb.Bottom + 5);
							contactLb.Width = 50;
							AMTTextBox contactTb = new AMTTextBox();
							contactTb.Name = "contactTb";
							contactTb.Enabled = false;
							contactTb.Location = new Point(contactLb.Right + 3, contactLb.Top);
							contactTb.Width = 209;
							rb1.CheckedChanged += delegate {
								contactTb.Enabled = true;
								nameTb.Text =
									contactTb.Text = string.Empty;
							};
							rb2.CheckedChanged += delegate {
								contactTb.Enabled = false;
								nameTb.Text =
									contactTb.Text = string.Empty;
							};
							
							panel.Controls.AddRange(new Control[]{
							                        	rb1,
							                        	rb2,
							                        	nameLb,
							                        	nameTb,
							                        	contactLb,
							                        	contactTb
							                        });
							Controls.Add(panel);
						}
						break;
					case "Unlock Cells":
						button1.Text = "Unlock\nCells";
						checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => s.Locked).Count() > 0;
						checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => s.Locked).Count() > 0;
						checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => s.Locked).Count() > 0;
						
						dataGridView1.Width = checkBox1.Left - 12;
						dataGridView1.Columns["Lock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Lock Comments"].Width = 300;
						dataGridView1.Columns["Lock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						dataGridView1.Columns["Locked"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						break;
					case "History":
						dataGridView1.Width = dataGridView1.Right + (amtRichTextBox1.Right - dataGridView1.Right) - 5;
						dataGridView1.Columns["Lock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Lock Comments"].Width = 300;
						dataGridView1.Columns["Lock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						dataGridView1.Columns["Unlock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Unlock Comments"].Width = 300;
						dataGridView1.Columns["Unlock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						if((DateTime.Now - currentSite.LockedCellsDetailsTimestamp) >= new TimeSpan(0, 10, 0))
							currentSite.requestOIData("LKULK");
						break;
					case "Cells Locked":
						Text = "Cells Locked";
						Name = uiMode;
						
						MainMenu = new AMTMenuStrip();
						ToolStripMenuItem updateAvailabilityToolStripMenuItem = new ToolStripMenuItem();
						updateAvailabilityToolStripMenuItem.Text = "Update all sites Availability";
						updateAvailabilityToolStripMenuItem.Click += delegate {
							DialogResult ans = DialogResult.No;
							Action action = new Action(delegate {
							                           	ans = FlexibleMessageBox.Show("This option will get all sites Availability stats from OI\nand, depending on the number of sites,\nit might take a few minutes to conclude.\n\nContinue anyway?", "Update availability", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							                           });
							LoadingPanel loading =  new LoadingPanel();
							loading.Show(action, this);
							if(ans == DialogResult.Yes) {
								action = new Action(delegate {
								                    	FetchAvailability(((ListBox)Controls["ListBox"]).Items.Cast<string>());
								                    });
								loading =  new LoadingPanel();
								loading.ShowAsync(action, null, true, this);
							}
						};
						ToolStripMenuItem refreshCellsPageToolStripMenuItem = new ToolStripMenuItem();
						refreshCellsPageToolStripMenuItem.Text = "Refresh data";
						refreshCellsPageToolStripMenuItem.Click += delegate {
							Action action = new Action(delegate {
							                           	populateCellsLocked();
							                           });
							LoadingPanel load = new LoadingPanel();
							load.Show(action, this);
						};
						
						Controls.Add(MainMenu);
//						MainMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						MainMenu.InitializeTroubleshootMenu(true);
						MainMenu.OiButtonsOnClickDelegate += LoadDisplayOiDataTable;
						MainMenu.RefreshButtonOnClickDelegate += refreshOiData;
						MainMenu.MainMenu.DropDownItems.Add(updateAvailabilityToolStripMenuItem);
						MainMenu.MainMenu.DropDownItems.Add("-");
						MainMenu.MainMenu.DropDownItems.Add(refreshCellsPageToolStripMenuItem);
						
						dataGridView1.Height -= MainMenu.Height;
						dataGridView1.Location = new Point(dataGridView1.Left, dataGridView1.Top + MainMenu.Height);
						
						label5.Visible =
							comboBox1.Visible =
							checkBox2.Visible =
							checkBox3.Visible =
							radioButton1.Visible =
							radioButton2.Visible =
							radioButton3.Visible = false;
						
						ListBox sitesListBox = new ListBox();
						sitesListBox.Name = "ListBox";
						sitesListBox.Location = dataGridView1.Location;
						sitesListBox.Size = new Size(70, dataGridView1.Height);
						sitesListBox.SelectionMode = SelectionMode.One;
						sitesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
						sitesListBox.DrawMode = DrawMode.OwnerDrawFixed;
						sitesListBox.DrawItem += ListBoxDrawItem;
						sitesListBox.SelectedIndexChanged += ListBoxSelectedIndexChanged;
						sitesListBox.MouseDoubleClick += (sender, e) => {
							if(e.Button == MouseButtons.Left) {
								int index = cellsLockedSites.FindIndex(s => s.Id == sitesListBox.Text);
								if(index > -1) {
									List<siteDetails> openForms = Application.OpenForms.OfType<siteDetails>().Where(f => f.parentControl == (Control)this).ToList();
									if(openForms.Any()) {
										for(int c = 0;c < openForms.Count();c++) {
											openForms[c].Close();
											openForms[c--].Dispose();
										}
									}
									var SiteDetailsUI = new siteDetails(cellsLockedSites[index], this);
									SiteDetailsUI.Show();
								}
							}
						};
						
						dataGridView1.Width -= sitesListBox.Width + 5;
						dataGridView1.Location = new Point(sitesListBox.Right + 5, dataGridView1.Top);
						dataGridView1.RowsAdded += delegate { checkBox1.Enabled = dataGridView1.RowCount > 0; };
						dataGridView1.RowsRemoved += delegate { checkBox1.Enabled = dataGridView1.RowCount > 0; };
//						Button refreshButton = new Button();
//						refreshButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
//						refreshButton.Size = new Size(67, 24);
//						refreshButton.Location = new Point(dataGridView1.Right + 6, dataGridView1.Bottom - refreshButton.Height);
//						refreshButton.Text = "Refresh";
//						refreshButton.Click += delegate {
//							Action action = new Action(delegate {
//							                           	populateCellsLocked();
//							                           });
//							LoadingPanel load = new LoadingPanel();
//							load.Show(action, this);
//						};
						
						checkBox1.Text = "Select All";
						checkBox1.Height = checkBox1.Height * 2;
						checkBox1.Top = dataGridView1.Top;
						button1.Top = checkBox1.Bottom + 5;
						
						amtRichTextBox1.Height = dataGridView1.Height;
						label3.Top = sitesListBox.Top - label3.Height - 3;
						amtRichTextBox1.Top = dataGridView1.Top;
						label1.Location = new Point(sitesListBox.Left, label3.Top);
						label1.Text = "Sites";
						label1.Anchor = label2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
						label2.Location = new Point(dataGridView1.Left, label1.Top);
						label2.Text = "Locked Cells";
						
						Button legendButton = new Button();
						legendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
						legendButton.Text = "Colors Legend";
						legendButton.Size = new Size(90, 20);
						legendButton.Location = new Point(dataGridView1.Right - legendButton.Width, dataGridView1.Top - legendButton.Height - 3);
						
						legendButton.Click += delegate {
							Panel legendPanel = new Panel();
							legendPanel.BackColor = Color.Black;
							legendPanel.Size = new Size(230, 213);
							Label sitesLabel = new Label();
							sitesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							sitesLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
							sitesLabel.ForeColor = Color.LightGray;
							sitesLabel.Text = "SITES COLOR:";
							sitesLabel.TextAlign = ContentAlignment.MiddleCenter;
							sitesLabel.Size = new Size(legendPanel.Width, 18);
							sitesLabel.Location = new Point(0, 6);
							PictureBox pb1 = new PictureBox();
							pb1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb1.Size = new Size(20, 20);
							pb1.BackColor = Color.Red;
							pb1.BorderStyle = BorderStyle.FixedSingle;
							pb1.Location = new Point(6, sitesLabel.Bottom + 3);
							Label pb1Label = new Label();
							pb1Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb1Label.Text = "Reference Expired";
							pb1Label.ForeColor = Color.White;
							pb1Label.Size = new Size(100, 18);
							pb1Label.Location = new Point(pb1.Right + 3, pb1.Top + 3);
							PictureBox pb2 = new PictureBox();
							pb2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb2.Size = new Size(20, 20);
							pb2.BackColor = Color.LightGreen;
							pb2.BorderStyle = BorderStyle.FixedSingle;
							pb2.Location = new Point(6, pb1.Bottom + 2);
							Label pb2Label = new Label();
							pb2Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb2Label.Text = "Valid reference with non COOS cells";
							pb2Label.ForeColor = Color.White;
							pb2Label.Size = new Size(200, 18);
							pb2Label.Location = new Point(pb2.Right + 3, pb2.Top + 3);
							PictureBox pb3 = new PictureBox();
							pb3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb3.Size = new Size(20, 20);
							pb3.BackColor = Color.Yellow;
							pb3.BorderStyle = BorderStyle.FixedSingle;
							pb3.Location = new Point(6, pb2.Bottom + 2);
							Label pb3Label = new Label();
							pb3Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb3Label.Text = "Valid reference with cells off air";
							pb3Label.ForeColor = Color.White;
							pb3Label.Size = new Size(200, 18);
							pb3Label.Location = new Point(pb3.Right + 3, pb3.Top + 3);
							PictureBox pb4 = new PictureBox();
							pb4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb4.Size = new Size(20, 20);
							pb4.BackColor = Color.Gray;
							pb4.BorderStyle = BorderStyle.FixedSingle;
							pb4.Location = new Point(6, pb3.Bottom + 2);
							Label pb4Label = new Label();
							pb4Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb4Label.Text = "Site off air";
							pb4Label.ForeColor = Color.White;
							pb4Label.Size = new Size(100, 18);
							pb4Label.Location = new Point(pb4.Right + 3, pb4.Top + 3);
							Label cellsLabel = new Label();
							cellsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							cellsLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
							cellsLabel.ForeColor = Color.LightGray;
							cellsLabel.Text = "CELLS COLOR:";
							cellsLabel.TextAlign = ContentAlignment.MiddleCenter;
							cellsLabel.Size = new Size(legendPanel.Width, 18);
							cellsLabel.Location = new Point(0, pb4.Bottom + 8);
							PictureBox pb5 = new PictureBox();
							pb5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb5.Size = new Size(20, 20);
							pb5.BackColor = Color.Red;
							pb5.BorderStyle = BorderStyle.FixedSingle;
							pb5.Location = new Point(6, cellsLabel.Bottom + 3);
							Label pb5Label = new Label();
							pb5Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb5Label.Text = "COOS";
							pb5Label.ForeColor = Color.White;
							pb5Label.Size = new Size(100, 18);
							pb5Label.Location = new Point(pb5.Right + 3, pb5.Top + 3);
							PictureBox pb6 = new PictureBox();
							pb6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb6.Size = new Size(20, 20);
							pb6.BackColor = Color.LightGreen;
							pb6.BorderStyle = BorderStyle.FixedSingle;
							pb6.Location = new Point(6, pb5.Bottom + 2);
							Label pb6Label = new Label();
							pb6Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb6Label.Text = "Non COOS";
							pb6Label.ForeColor = Color.White;
							pb6Label.Size = new Size(100, 18);
							pb6Label.Location = new Point(pb6.Right + 3, pb6.Top + 3);
							PictureBox pb7 = new PictureBox();
							pb7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb7.Size = new Size(20, 20);
							pb7.BackColor = Color.White;
							pb7.BorderStyle = BorderStyle.FixedSingle;
							pb7.Location = new Point(6, pb6.Bottom + 2);
							Label pb7Label = new Label();
							pb7Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
							pb7Label.Text = "Cell off air";
							pb7Label.ForeColor = Color.White;
							pb7Label.Size = new Size(100, 18);
							pb7Label.Location = new Point(pb7.Right + 3, pb7.Top + 3);
							legendPanel.Controls.AddRange(new Control[] {
							                              	sitesLabel,
							                              	pb1, pb1Label,
							                              	pb2, pb2Label,
							                              	pb3, pb3Label,
							                              	pb4, pb4Label,
							                              	cellsLabel,
							                              	pb5, pb5Label,
							                              	pb6, pb6Label,
							                              	pb7, pb7Label
							                              });
							PopupHelper popup = new PopupHelper(legendPanel);
							popup.Show(this);
						};
						Label offAirLabel = new Label();
						offAirLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
						offAirLabel.Size = new Size(90, 18);
						offAirLabel.TextAlign = ContentAlignment.TopCenter;
						offAirLabel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold,GraphicsUnit.Point, ((byte)(0)));
						offAirLabel.ForeColor = Color.Red;
						offAirLabel.Name = "offAirLabel";
						offAirLabel.Text = "Site Off Air";
						offAirLabel.Location = new Point(dataGridView1.Left + ((dataGridView1.Width - offAirLabel.Width) / 2), label2.Top - 2);
						Controls.AddRange(new Control[] {
//						                  	refreshButton,
						                  	sitesListBox,
						                  	legendButton,
						                  	offAirLabel
						                  });
						
						Resize += delegate {
							MainMenu.Width = Width;
							offAirLabel.Left = dataGridView1.Left + ((dataGridView1.Width - offAirLabel.Width) / 2);
						};
						FormClosing += delegate {
							if(cellsLockedSites != null) {
								if(currentSite != null) {
									int siteIndex = cellsLockedSites.FindIndex(s => s.Id == currentSite.Id);
									if(siteIndex > -1)
										cellsLockedSites[siteIndex] = currentSite;
								}
								foreach(Site site in cellsLockedSites)
									site.Dispose();
							}
						};
						
						break;
				}
				
				label4.Visible = uiMode.StartsWith("Lock");
				
				if(!uiMode.Contains("Lock")) { // Will enter only on Unlock Cells and History
					try {
						Panel panel = Controls["panel"] as Panel;
						Controls.Remove(panel);
						panel.Dispose();
						
						amtRichTextBox1.Height += 87;
					}
					catch { }
				}
				
				if(uiMode != "Cells Locked") {
					this.Text = "Lock/Unlock Cells - Site " + currentSite.Id;
					checkBox1.CheckedChanged -= CheckBoxesCheckedChanged;
					checkBox2.CheckedChanged -= CheckBoxesCheckedChanged;
					checkBox3.CheckedChanged -= CheckBoxesCheckedChanged;
					checkBox1.Checked =
						checkBox2.Checked =
						checkBox3.Checked = false;
					checkBox1.CheckedChanged += CheckBoxesCheckedChanged;
					checkBox2.CheckedChanged += CheckBoxesCheckedChanged;
					checkBox3.CheckedChanged += CheckBoxesCheckedChanged;
					amtRichTextBox1.Text =
						comboBox1.Text = string.Empty;
					amtRichTextBox1.Enabled = !string.IsNullOrEmpty(comboBox1.Text);
					comboBox1.Enabled = false;
					
					if(uiMode.Contains("ock Cells"))
						label5.Text = "Selected:\n\n2G: " + gsmCheckedCount + "\n3G: " + umtsCheckedCount + "\n4G: " + lteCheckedCount + "\n\nTotal: " + (gsmCheckedCount + umtsCheckedCount + lteCheckedCount);
					foreach(Control ctrl in Controls) {
						switch(ctrl.GetType().ToString()) {
							case "System.Windows.Forms.CheckBox":
							case "System.Windows.Forms.Label":
							case "System.Windows.Forms.ComboBox":
							case "System.Windows.Forms.Button":
							case "appCore.UI.AMTRichTextBox":
								if(ctrl.Name != "label4")
									ctrl.Visible = uiMode.Contains("ock Cells");
								break;
						}
					}
				}
			}
		}
		
		Form OwnerForm;
		string LockedCellsCSV;
		
		public LockUnlockCellsForm() {
			SplashForm.ShowSplashScreen();
			SplashForm.UpdateLabelText("Loading Cells Locked");
			InitializeComponent();
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			UiMode = "Cells Locked";
			SplashForm.UpdateLabelText("This might take a few minutes");
			populateCellsLocked();
			SplashForm.CloseForm();
		}
		
		public LockUnlockCellsForm(Form parent) {
			InitializeComponent();
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			OwnerForm = parent;
			if(OwnerForm is siteDetails)
				currentSite = ((siteDetails)OwnerForm).currentSite;
			
			Text = "Site " + currentSite.Id + " Lock/Unlock cells";
			
			currentSite.requestOIData("LKULK");
			
			radioButton1.Select();
		}
		
		void populateCellsLocked() {
			string response = OiConnection.requestPhpOutput("cellslocked",string.Empty,null,string.Empty);
			
			if(UiMode == "Cells Locked")
				SplashForm.UpdateLabelText("Getting data from OI");
			
			List<string> sitesList = new List<string>();
			
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.Load(new System.IO.StringReader(response.Substring(response.IndexOf("<body>"))));
			LockedCellsCSV = string.Empty;
			
			var titles = doc.DocumentNode.SelectSingleNode("//body[1]//div[1]").ChildNodes.Where(s => s.Name == "b").ToList();
			var tables = doc.DocumentNode.SelectSingleNode("//body[1]//div[1]").ChildNodes.Where(s => s.InnerHtml.Contains("Locked Time")).ToList();
			
			for(int c = 0;c < titles.Count;c++) {
				string siteCsv = string.Empty;
				sitesList.Add(titles[c].InnerText.Substring("Site ".Length));
				
				// Build CSV
				foreach(var tr in tables[c].ChildNodes) {
					if(tr.Name != "#text" && tr.Name != "th") {
						siteCsv += titles[c].InnerText.Substring("Site ".Length) + ",";
						var childNodes = tr.ChildNodes.Where(s => s.Name == "td" && !s.InnerHtml.Contains("checkbox"));
						foreach(var childNode in childNodes) {
							
							siteCsv += childNode.InnerText.Replace(',',';').Replace("\n","<<lb>>").Replace("\r","");
							if(childNode != childNodes.Last())
								siteCsv += ',';
						}
						if(tr != tables[c].ChildNodes.Last())
							siteCsv += Environment.NewLine;
					}
				}
				LockedCellsCSV += siteCsv;
				if(c != titles.Count - 1)
					LockedCellsCSV += Environment.NewLine;
			}
			
			List<string> expiredSitesList = new List<string>();
			List<string> notExpiredSitesList = new List<string>();
			
			for(int c = 0;c < sitesList.Count;c++) {
				if(GetSiteLockedCells(sitesList[c]).LifeTime == "Expired")
					expiredSitesList.Add(sitesList[c]);
				else
					notExpiredSitesList.Add(sitesList[c]);
			}
			expiredSitesList.Sort(new NumericListComparer<string>());
			notExpiredSitesList.Sort(new NumericListComparer<string>());
			
			ListBox lb = Controls["ListBox"] as ListBox;
			lb.Items.Clear();
			lb.Items.AddRange(expiredSitesList.ToArray());
			lb.Items.AddRange(notExpiredSitesList.ToArray());
			
			if(lb.Items.Count > 0) {
				label1.Text = "Sites (" + lb.Items.Count + ")";
				List<string> noCellsStateSites = new List<string>();
				if(cellsLockedSites == null) {
					if(UiMode == "Cells Locked")
						SplashForm.UpdateLabelText("Collecting sites data");
					
					cellsLockedSites = DB.SitesDB.getSites(lb.Items.Cast<string>().ToList());
					
					foreach(Site site in cellsLockedSites) {
						if(DateTime.Now - site.AvailabilityTimestamp > new TimeSpan(0, 30, 0))
							noCellsStateSites.Add(site.Id);
					}
				}
				else {
					List<int> sitesToRemove = new List<int>();
					List<string> sitesToAdd = lb.Items.Cast<string>().ToList();
					foreach(Site site in cellsLockedSites) {
						if(!lb.Items.Contains(site.Id)) {
							sitesToRemove.Add(cellsLockedSites.IndexOf(site));
						}
						else {
							if(DateTime.Now - site.AvailabilityTimestamp > new TimeSpan(0, 30, 0))
								noCellsStateSites.Add(site.Id);
							sitesToAdd.Remove(site.Id);
						}
					}
					
					if(sitesToAdd.Any()) {
						cellsLockedSites.AddRange(DB.SitesDB.getSites(sitesToAdd));
						foreach(string str in sitesToAdd) {
							Site site = cellsLockedSites.Find(s => s.Id == str);
							if(site != null) {
								if(DateTime.Now - site.AvailabilityTimestamp > new TimeSpan(0, 30, 0))
									noCellsStateSites.Add(site.Id);
							}
						}
					}
					
//					List<Site> sitesToUpdate = new List<Site>();
					foreach(int siteToRemove in sitesToRemove) {
//						sitesToUpdate.Add(cellsLockedSites[siteToRemove]);
						cellsLockedSites.RemoveAt(siteToRemove);
					}
//					if(sitesToUpdate.Count > 0)
//						DB.SitesDB.UpdateSitesOnCache(sitesToUpdate);
				}
				
				if(noCellsStateSites.Count > 0) {
					FetchAvailability(noCellsStateSites);
//					FetchCellsState(noCellsStateSites);
				}
				
				lb.SetSelected(0, true);
			}
		}
		
		void FetchCellsState(IEnumerable<string> sites) {
			List<OiCell> list = new List<OiCell>();
			
			List<Thread> threads = new List<Thread>();
			int finishedThreadsCount = 0;
			Thread thread;
			thread = new Thread(() => {
			                    	string resp = OiConnection.requestApiOutput("cells", sites, 2);
			                    	var jSon = JsonConvert.DeserializeObject<RootObject>(resp);
			                    	foreach(JObject jObj in jSon.data) {
			                    		OiCell oc = jObj.ToObject<OiCell>();
			                    		list.Add(oc);
			                    	}
			                    	
			                    	finishedThreadsCount++;
			                    });
			thread.Name = "getOiCellsState_2G";
			threads.Add(thread);
			
			thread = new Thread(() => {
			                    	string resp = OiConnection.requestApiOutput("cells", sites, 3);
			                    	var jSon = JsonConvert.DeserializeObject<RootObject>(resp);
			                    	foreach(JObject jObj in jSon.data) {
			                    		OiCell oc = jObj.ToObject<OiCell>();
			                    		list.Add(oc);
			                    	}
			                    	
			                    	finishedThreadsCount++;
			                    });
			thread.Name = "getOiCellsState_2G";
			threads.Add(thread);
			
			thread = new Thread(() => {
			                    	string resp = OiConnection.requestApiOutput("cells", sites, 4);
			                    	var jSon = JsonConvert.DeserializeObject<RootObject>(resp);
			                    	foreach(JObject jObj in jSon.data) {
			                    		OiCell oc = jObj.ToObject<OiCell>();
			                    		list.Add(oc);
			                    	}
			                    	
			                    	finishedThreadsCount++;
			                    });
			thread.Name = "getOiCellsState_2G";
			threads.Add(thread);
			
			foreach(Thread th in threads) {
				th.SetApartmentState(ApartmentState.STA);
				th.Start();
			}
			
			while(finishedThreadsCount < threads.Count) { }
			
			foreach(string siteStr in sites) {
				int siteIndex = cellsLockedSites.FindIndex(s => s.Id == siteStr);
				if(siteIndex > -1) {
					foreach (Cell cell in cellsLockedSites[siteIndex].Cells) {
						OiCell oiCell = list.Find(s => s.CELL_NAME == cell.Name);
						if(oiCell != null) {
							cell.Locked = !string.IsNullOrEmpty(oiCell.LOCKED);
							cell.LockedFlagTimestamp = DateTime.Now;
							cell.COOS = !string.IsNullOrEmpty(oiCell.COOS);
							cell.CoosFlagTimestamp = DateTime.Now;
						}
					}
					cellsLockedSites[siteIndex].CellsStateTimestamp = DateTime.Now;
				}
			}
		}
		
		void FetchAvailability(IEnumerable<string> sites) {
//			string resp = OiConnection.requestApiOutput("availability", sites);
			string resp = OiConnection.requestPhpOutput("ca", sites);
			
			try {
//				Availability jSon = JsonConvert.DeserializeObject<Availability>(resp);
//				DataTable dt = jSon.ToDataTable();
				DataTable dt = Toolbox.Tools.ConvertHtmlTabletoDataTable(resp, "table_ca");
				
				foreach(var site in cellsLockedSites) {
					var drs = dt.Rows.Cast<DataRow>().Where(s => s["Site"].ToString() == site.Id);
					if(drs.Any()) {
						site.Availability = dt.Clone();
						foreach(var row in drs)
							site.Availability.Rows.Add(row.ItemArray);
						site.AvailabilityTimestamp = DateTime.Now;
						site.updateCOOS();
					}
				}
			}
			catch { }
		}
		
		bool rowInactive(DataGridViewRow row) {
			switch(UiMode) {
				case "Lock Cells":
					return row.Cells["Locked"].Value.ToString() == "YES";
				case "Unlock Cells":
					return row.Cells["Locked"].Value.ToString() == "No";
			}
			return false;
		}
		
		void ListBoxDrawItem(object sender, DrawItemEventArgs e) {
			ListBox lb = sender as ListBox;
			e.DrawBackground();
			using(Graphics g = e.Graphics) {
				if(((e.State & DrawItemState.Focus) != DrawItemState.Focus) && ((e.State & DrawItemState.Selected) != DrawItemState.Selected)) {
					CellsLockedSite cls = GetSiteLockedCells(lb.Items[e.Index].ToString());
					if(cls.LifeTime == "Expired")
						g.FillRectangle(new SolidBrush(Color.Red), e.Bounds); // Ref expired
					else {
						Site site = cellsLockedSites.Find(s => s.Id == cls.Site);
						if(site != null) {
							foreach(CellsLockedItem cli in cls.CellsLockedItems) {
								Cell cell = site.Cells.Find(c => c.Name == cli.Cell);
								if(cell != null) {
									if(!cell.COOS) {
										g.FillRectangle(new SolidBrush(Color.LightGreen), e.Bounds); // Ref not expired but locked cells non COOS
										break;
									}
								}
								else
									g.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds); // Ref not expired but cells Offair
							}
						}
						else
							g.FillRectangle(new SolidBrush(Color.Gray), e.Bounds); // Site Offair
					}
				}
				else
					g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
				
				if(e.Index != -1)
					g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), lb.GetItemRectangle(e.Index).Location);
			}
			
			e.DrawFocusRectangle();
		}
		
		void ListBoxSelectedIndexChanged(object sender, EventArgs e) {
			ListBox lb = sender as ListBox;
			if(currentSite != null) {
				if(currentSite.Id == lb.Text)
					return;
			}
			
			Action actionNonThreaded = new Action(delegate {
			                                      	if(currentSite != null) {
			                                      		if(currentSite.Id == lb.Text)
			                                      			return;
			                                      		int siteIndex = cellsLockedSites.FindIndex(s => s.Id == currentSite.Id);
			                                      		if(siteIndex > -1)
			                                      			cellsLockedSites[siteIndex] = currentSite;
			                                      	}
			                                      	
			                                      	dataGridView1.DataSource = null;
			                                      	dataGridView1.Columns.Clear();
			                                      	
			                                      	int selectedSiteIndex = cellsLockedSites.FindIndex(s => s.Id == lb.Text);
			                                      	if(selectedSiteIndex > -1) {
			                                      		Controls["offAirLabel"].Visible = false;
			                                      		currentSite = cellsLockedSites[selectedSiteIndex];
			                                      		if(DateTime.Now - currentSite.AvailabilityTimestamp > new TimeSpan(0, 30, 0)) {
//			                                      		if(DateTime.Now - currentSite.CellsStateTimestamp >= new TimeSpan(0, 30, 0)) {
			                                      			Thread thread = new Thread(() => currentSite.requestOIData("Availability"));
			                                      			thread.Name = "ListBoxSelectedIndexChanged";
			                                      			thread.SetApartmentState(ApartmentState.STA);
			                                      			thread.Start();
			                                      		}
			                                      		while((DateTime.Now - currentSite.AvailabilityTimestamp > new TimeSpan(0, 30, 0))) { }
//			                                      		while((DateTime.Now - currentSite.CellsStateTimestamp > new TimeSpan(0, 30, 0))) { }
			                                      	}
			                                      	else {
			                                      		Controls["offAirLabel"].Visible = true;
			                                      		currentSite = null;
			                                      	}
			                                      	
			                                      	dataGridView1.DataSource = GetSiteLockedCellsDT(lb.SelectedItem.ToString());
			                                      	
			                                      	dataGridView1.Columns["Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			                                      	dataGridView1.Columns["Comments"].Width = 300;
			                                      	dataGridView1.Columns["Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			                                      	addCheckBoxColumn();
			                                      	checkBox1.Checked = false;
			                                      	MainMenu.siteFinder_Toggle(true);
			                                      });
			LoadingPanel loading = new LoadingPanel();
			loading.Show(actionNonThreaded, dataGridView1);
		}
		
		DataTable GetSiteLockedCellsDT(string site) {
			DataTable dt = null;
			try {
				var engine = new FileHelperEngine<CellsLockedItem>();
				engine.AfterReadRecord +=  (eng, a) => {
					if(a.Record.Site != site)
						a.SkipThisRecord = true;
					else
						a.Record.Comments = a.Record.Comments.Replace("<<lb>>",Environment.NewLine);
				};
				dt = engine.ReadStringAsDT(LockedCellsCSV);
			}
			catch(FileHelpersException ex) {
				string f = ex.Message;
			}
			
			if(dt != null) {
				dt.Columns["lockedTime"].ColumnName = "Locked Time";
				dt.Columns["ReferenceStatus"].ColumnName = "Status";
				dt.Columns["crqStart"].ColumnName = "CRQ Start Time";
				dt.Columns["crqEnd"].ColumnName = "CRQ End Time";
				dt.Columns["LockedBy"].ColumnName = "Locked By";
			}
			
			return dt;
		}
		
		CellsLockedSite GetSiteLockedCells(string site) {
			List<CellsLockedItem> cellsLockedList = new List<CellsLockedItem>();
			try {
				var engine = new FileHelperEngine<CellsLockedItem>();
				engine.AfterReadRecord +=  (eng, a) => {
					if(a.Record.Site != site)
						a.SkipThisRecord = true;
				};
				cellsLockedList = engine.ReadStringAsList(LockedCellsCSV);
			}
			catch(FileHelpersException ex) {
				string f = ex.Message;
			}
			
			return new CellsLockedSite(cellsLockedList);
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			Action actionNonThreaded = new Action(delegate {
			                                      	RadioButton rb = sender as RadioButton;
			                                      	
			                                      	if(rb.Checked) {
			                                      		SuspendLayout();
			                                      		
			                                      		dataGridView1.DataSource = null;
			                                      		dataGridView1.Columns.Clear();
			                                      		
			                                      		InitializeDataTable(rb.Text);
			                                      		
			                                      		uiMode = rb.Text;
			                                      		
			                                      		dataGridView1.DataSource = Table;
			                                      		
			                                      		UiMode = rb.Text;
			                                      		
			                                      		ResumeLayout();
			                                      	}
			                                      });
			if(!Controls.OfType<LoadingPanel>().Any()) {
				LoadingPanel loading = new LoadingPanel();
				loading.Show(actionNonThreaded, this);
			}
			else
				actionNonThreaded();
		}
		
		void checkBookIn() {
			if(currentSite.Visits == null)
				currentSite.requestOIData("Bookins");
			
			var foundBookIns = currentSite.Visits.FindAll(s => string.IsNullOrEmpty(s.Departed_Site));
			if(foundBookIns.Count > 0) {
				BookIn bookIn = null;
				foreach(BookIn bi in foundBookIns) {
					DateTime arrivedTime = Convert.ToDateTime(bi.Arrived);
					if(arrivedTime.Year == DateTime.Now.Year && arrivedTime.Month == DateTime.Now.Month && arrivedTime.Day == DateTime.Now.Day) {
						bookIn = bi;
						break;
					}
				}
				if(bookIn != null) {
					label4.Text = "Valid Book In found: " + bookIn.Engineer + " - " + bookIn.Mobile + " - " + bookIn.Reference + " - " + bookIn.Arrived;
					label4.ForeColor = Color.DarkGreen;
					label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
				}
				else {
					label4.Text = "CAUTION!! No valid Book In found";
					label4.ForeColor = Color.Red;
					label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
				}
			}
			else {
				label4.Text = "CAUTION!! No valid Book In found";
				label4.ForeColor = Color.Red;
				label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			}
		}
		
		void CheckBoxesCheckedChanged(object sender, EventArgs e) {
			if(UiMode != "Cells Locked") {
				if(UiMode != "History") {
					CheckBox cb = sender as CheckBox;
					var filtered = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => s.Cells["Tech"].Value.ToString() == cb.Text);
					
					foreach(DataGridViewRow dgvr in filtered) {
						if(dgvr.Cells[0].Style.ForeColor != SystemColors.GrayText) {
							DataGridViewCheckBoxCell cell = dgvr.Cells[0] as DataGridViewCheckBoxCell;
							dataGridView1.CellValueChanged -= DataGridView1CellValueChanged;
							
							if(filtered.Last() == dgvr) {
								dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
								cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
							}
							else {
								cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
								dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
							}
						}
					}
				}
			}
			else {
				foreach(DataGridViewRow dgvr in dataGridView1.Rows) {
					DataGridViewCheckBoxCell cell = dgvr.Cells[0] as DataGridViewCheckBoxCell;
					dataGridView1.CellValueChanged -= DataGridView1CellValueChanged;
					
					if(dataGridView1.Rows.Cast<DataGridViewRow>().Last() == dgvr) {
						dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
						cell.Value = checkBox1.Checked ? cell.TrueValue : cell.FalseValue;
					}
					else {
						cell.Value = checkBox1.Checked ? cell.TrueValue : cell.FalseValue;
						dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
					}
				}
				
			}
		}
		
		void ComboBox1TextUpdate(object sender, EventArgs e) {
			amtRichTextBox1.Enabled = !string.IsNullOrEmpty(comboBox1.Text);
		}
		
		void ComboBox1EnabledChanged(object sender, EventArgs e) {
			amtRichTextBox1.Enabled = !string.IsNullOrEmpty(comboBox1.Text) && comboBox1.Enabled;
		}
		
		void AmtRichTextBox1TextChanged(object sender, EventArgs e) {
			button1.Enabled = !string.IsNullOrEmpty(amtRichTextBox1.Text);
		}
		
		void AmtRichTextBox1EnabledChanged(object sender, EventArgs e) {
			button1.Enabled = !string.IsNullOrEmpty(amtRichTextBox1.Text) && amtRichTextBox1.Enabled;
			try {
				Panel panel = Controls["panel"] as Panel;
				panel.Controls["rb1"].Enabled =
					panel.Controls["rb2"].Enabled =
					panel.Controls["nameTb"].Enabled =
					panel.Controls["contactTb"].Enabled = amtRichTextBox1.Enabled;
			}
			catch {}
		}
		
		void Button1Click(object sender, EventArgs e) {
			Action actionNonThreaded = new Action(delegate {
			                                      	string name = string.Empty;
			                                      	string contact = string.Empty;
			                                      	string rbText = string.Empty;
			                                      	if(label4.Visible && label4.Text.StartsWith("CAUTION")) {
			                                      		Panel panel = Controls["panel"] as Panel;
			                                      		rbText = ((RadioButton)panel.Controls["rb1"]).Checked ? panel.Controls["rb1"].Text : panel.Controls["rb2"].Text;
			                                      		if(string.IsNullOrEmpty(panel.Controls["nameTb"].Text) || (rbText.StartsWith("FE") && string.IsNullOrEmpty(panel.Controls["contactTb"].Text))) {
			                                      			Action action = new Action(delegate {
			                                      			                           	FlexibleMessageBox.Show("No valid book in found for this site.\n\n" +
			                                      			                           	                        "It's OK to lock cells without a book in, as long\n" +
			                                      			                           	                        "as you provide the FE or lockdown approver\n" +
			                                      			                           	                        "contact details on the comments.\n\n" +
			                                      			                           	                        "Please fill in the required details.", "Data missing",
			                                      			                           	                        MessageBoxButtons.OK, MessageBoxIcon.Error);
			                                      			                           });
			                                      			LoadingPanel loading = new LoadingPanel();
			                                      			loading.Show(action, this);
			                                      			return;
			                                      		}
			                                      		name = panel.Controls["nameTb"].Text;
			                                      		contact = panel.Controls["contactTb"].Text;
			                                      	}
			                                      	
			                                      	string comment = amtRichTextBox1.Text;
			                                      	if(!string.IsNullOrEmpty(rbText)) {
			                                      		comment += Environment.NewLine;
			                                      		comment += rbText == "FE" ? "FE on site - " + name + ", " + contact : "Requested by " + name;
			                                      	}
			                                      	
			                                      	var filtered = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => (bool?)s.Cells[0].Value == true);
			                                      	List<string> cellsList = new List<string>();
			                                      	foreach(DataGridViewRow row in filtered) {
			                                      		if(UiMode != "Cells Locked")
			                                      			cellsList.Add(row.Cells["Cell Name"].Value.ToString());
			                                      		else
			                                      			cellsList.Add(row.Cells["Cell"].Value.ToString());
			                                      	}
			                                      	if(UiMode.StartsWith("Lock"))
			                                      		sendLockCellsRequest(cellsList, comboBox1.Text, comment);
			                                      	else
			                                      		sendUnlockCellsRequest(cellsList, comment);
			                                      });
			LoadingPanel load = new LoadingPanel();
			load.Show(actionNonThreaded, this);
		}
		
		void sendLockCellsRequest(List<string> cellsList, string reference, string comments) {
			bool manRef = !comboBox1.Items.Contains(reference);
			OiConnection.requestPhpOutput("enterlock", currentSite.Id, cellsList, reference, comments, manRef);
			currentSite.requestOIData("CellsStateLKULK");
			RadioButtonsCheckedChanged(radioButton1, null);
		}
		
		void sendUnlockCellsRequest(List<string> cellsList, string comments) {
			if(UiMode != "Cells Locked") {
				OiConnection.requestPhpOutput("cellslocked", currentSite.Id, cellsList, comments);
				currentSite.requestOIData("CellsStateLKULK");
				RadioButtonsCheckedChanged(radioButton2, null);
			}
			else {
				OiConnection.requestPhpOutput("cellslocked", ((ListBox)Controls["ListBox"]).SelectedItem.ToString(), cellsList, comments);
				populateCellsLocked();
			}
		}
		
		void InitializeDataTable(string radioButtonText) {
			Table = new DataTable();
			
			DataColumn Tech = new DataColumn("Tech");
			DataColumn CellName = new DataColumn("Cell Name");
			DataColumn Switch = new DataColumn("Switch");
			DataColumn OssId = new DataColumn("OSS ID");
			DataColumn Vendor = new DataColumn("Vendor");
			DataColumn NOC = new DataColumn("NOC");
			DataColumn Locked = new DataColumn("Locked");
			Table.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC });
			if(radioButtonText != "History")
				Table.Columns.Add(Locked);
			
			if(radioButtonText == "Lock Cells") {
				foreach (Cell cell in currentSite.Cells) {
					string ossID;
					if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
						ossID = cell.ENodeB_Id;
					else
						ossID = cell.WBTS_BCF;
					
					DataRow row = Table.NewRow();
					row["Tech"] = cell.Bearer;
					row["Cell Name"] = cell.Name;
					row["Switch"] = cell.BscRnc_Id;
					row["OSS ID"] = ossID;
					row["Vendor"] = cell.Vendor.ToString();
					row["NOC"] = cell.Noc;
					row["Locked"] = cell.Locked ? "YES" : "No";
					
					Table.Rows.Add(row);
				}
				addCheckBoxColumn();
			}
			else {
				DataColumn Reference = new DataColumn("Reference");
				DataColumn CaseStatus = new DataColumn("Status");
				DataColumn CrqScheduledStart = new DataColumn("Scheduled Start");
				DataColumn CrqScheduledEnd = new DataColumn("Scheduled End");
				DataColumn LockedTime = new DataColumn("Locked Time");
				DataColumn LockedBy = new DataColumn("Locked By");
				DataColumn LockComments = new DataColumn("Lock Comments");
				Table.Columns.AddRange(new [] { Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments });
				if(radioButtonText == "Unlock Cells") {
					foreach (Cell cell in currentSite.Cells) {
						string ossID;
						if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
							ossID = cell.ENodeB_Id;
						else
							ossID = cell.WBTS_BCF;
						
						DataRow row = Table.NewRow();
						row["Tech"] = cell.Bearer;
						row["Cell Name"] = cell.Name;
						row["Switch"] = cell.BscRnc_Id;
						row["OSS ID"] = ossID;
						row["Vendor"] = cell.Vendor.ToString();
						row["NOC"] = cell.Noc;
						row["Locked"] = cell.Locked ? "YES" : "No";
						
						if(currentSite.LockedCellsDetails == null || (DateTime.Now - currentSite.LockedCellsDetailsTimestamp) >= new TimeSpan(0, 30, 0))
							currentSite.requestOIData("LKULK");
						
						List<DataRow> filtered = new List<DataRow>();
						foreach(DataRow dr in currentSite.LockedCellsDetails.Rows) {
							if(!string.IsNullOrEmpty(dr[6].ToString()) && string.IsNullOrEmpty(dr[9].ToString()))
								filtered.Add(dr);
						}
						
						DataRow tempRow = null;
						if(filtered.Count > 0) {
							foreach(DataRow dr in filtered) {
								if(dr[0].ToString() == cell.Name) {
									tempRow = dr;
									break;
								}
							}
							if(tempRow != null) {
								row["Reference"] = tempRow[2].ToString();
								row["Status"] = tempRow[3].ToString();
								row["Scheduled Start"] = tempRow[4].ToString();
								row["Scheduled End"] = tempRow[5].ToString();
								row["Locked Time"] = tempRow[1].ToString();
								row["Locked By"] = tempRow[6].ToString();
								row["Lock Comments"] = tempRow[7].ToString();
							}
						}
						
						Table.Rows.Add(row);
					}
					addCheckBoxColumn();
				}
				else {
					DataColumn UnlockedTime = new DataColumn("Unlocked Time");
					DataColumn UnlockedBy = new DataColumn("Unlocked By");
					DataColumn UnlockComments = new DataColumn("Unlock Comments");
					Table.Columns.AddRange(new [] { UnlockedTime, UnlockedBy, UnlockComments });
					
					if(currentSite.LockedCellsDetails == null)
						currentSite.requestOIData("LKULK");
					foreach (DataRow dr in currentSite.LockedCellsDetails.Rows) {
						DataRow row = Table.NewRow();
						Cell cell = currentSite.Cells.Find(s => s.Name == dr[0].ToString());
						string ossID = string.Empty;
						if(cell != null) {
							if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
								ossID = cell.ENodeB_Id;
							else
								ossID = cell.WBTS_BCF;
						}
						
						row["Tech"] = cell != null ? cell.Bearer : string.Empty;
						row["Cell Name"] = dr[0].ToString();
						row["Switch"] = cell != null ? cell.BscRnc_Id : string.Empty;
						row["OSS ID"] = ossID;
						row["Vendor"] = cell != null ? cell.Vendor.ToString() : string.Empty;
						row["NOC"] = cell != null ? cell.Noc : string.Empty;
						
						row["Reference"] = dr[2].ToString();
						row["Status"] = dr[3].ToString();
						row["Scheduled Start"] = dr[4].ToString();
						row["Scheduled End"] = dr[5].ToString();
						row["Locked Time"] = dr[1].ToString();
						row["Locked By"] = dr[6].ToString();
						row["Lock Comments"] = dr[7].ToString();
						
						row["Unlocked Time"] = dr[8].ToString();
						row["Unlocked By"] = dr[9].ToString();
						row["Unlock Comments"] = dr[10].ToString();
						
						Table.Rows.Add(row);
					}
				}
			}
		}
		
		void LoadDisplayOiDataTable(object sender, EventArgs e) {
//			if(e.Button == MouseButtons.Left) {
			string dataToShow = ((ToolStripMenuItem)sender).Name.Replace("Button", string.Empty);
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == (Control)this && f.Text.EndsWith(dataToShow)).ToList();
			if(fc.Count > 0) {
				fc[0].Close();
				fc[0].Dispose();
			}
			
			if(currentSite != null) {
				if(currentSite.Exists) {
					DataTable dt = new DataTable();
					switch(dataToShow) {
						case "INCs":
							if(currentSite.Incidents == null) {
								currentSite.requestOIData("INC");
								if(currentSite.Incidents.Count > 0) {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.Incidents.Count + ")";
								}
								else {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
								return;
							}
							break;
						case "CRQs":
							if(currentSite.Changes == null) {
								currentSite.requestOIData("CRQ");
								if(currentSite.Changes.Count > 0) {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.Changes.Count + ")";
								}
								else {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
								return;
							}
							break;
						case "BookIns":
							if(currentSite.Visits == null) {
								currentSite.requestOIData("Bookins");
								if(currentSite.Visits.Count > 0) {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.Visits.Count + ")";
								}
								else {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
								return;
							}
							break;
						case "ActiveAlarms":
							if(currentSite.Alarms == null) {
								currentSite.requestOIData("Alarms");
								if(currentSite.Alarms.Count > 0) {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.Alarms.Count + ")";
								}
								else {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
								return;
							}
							break;
						case "Availability":
							if(currentSite.Availability == null) {
								currentSite.requestOIData("Availability");
								if(currentSite.Availability.Rows.Count > 0) {
									MainMenu.AvailabilityButton.Enabled = true;
									MainMenu.AvailabilityButton.ForeColor = Color.DarkGreen;
									MainMenu.AvailabilityButton.Text = "Availability chart";
								}
								else {
									MainMenu.AvailabilityButton.Enabled = false;
									MainMenu.AvailabilityButton.Text = "No availability chart to display";
								}
								return;
							}
							break;
					}
					
					OiSiteTablesForm OiTable = null;
					switch(dataToShow) {
						case "INCs":
							OiTable = new OiSiteTablesForm(currentSite.Incidents, currentSite.Id, this);
							break;
						case "CRQs":
							OiTable = new OiSiteTablesForm(currentSite.Changes, currentSite.Id, this);
							break;
						case "BookIns":
							OiTable = new OiSiteTablesForm(currentSite.Visits, currentSite.Id, this);
							break;
						case "ActiveAlarms":
							OiTable = new OiSiteTablesForm(currentSite.Alarms, currentSite.Id, this);
							break;
						case "Availability":
							OiTable = new OiSiteTablesForm(currentSite.Availability, dataToShow, currentSite.Id, this);
							break;
					}
					OiTable.Show();
				}
			}
		}
		
		void refreshOiData(object sender, EventArgs e) {
			currentSite.requestOIData("INCCRQBookinsAlarmsAvailability");
			MainMenu.siteFinder_Toggle(true);
		}
		
		DataGridViewCheckBoxColumn addCheckBoxColumn() {
			DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
			chkColumn.HeaderText = "";
			chkColumn.ThreeState = false;
			chkColumn.FalseValue = false;
			chkColumn.TrueValue = true;
			chkColumn.Width = 19;
			dataGridView1.Columns.Insert(0, chkColumn);
			return chkColumn;
		}
		
		void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e) {
			if(e.ColumnIndex == 0) {
				bool canCheck = true;
				if(UiMode != "Cells Locked") {
					switch(dataGridView1.Rows[e.RowIndex].Cells["Tech"].Value.ToString()) {
						case "2G":
							canCheck = umtsCheckedCount == 0 && lteCheckedCount == 0;
							if(canCheck)
								checkBox2.Enabled =
									checkBox3.Enabled = false;
							break;
						case "3G":
							canCheck = gsmCheckedCount == 0 && lteCheckedCount == 0;
							if(canCheck)
								checkBox1.Enabled =
									checkBox3.Enabled = false;
							break;
						case "4G":
							canCheck = gsmCheckedCount == 0 && umtsCheckedCount == 0;
							if(canCheck)
								checkBox1.Enabled =
									checkBox2.Enabled = false;
							break;
					}
				}
				if(!rowInactive(dataGridView1.Rows[e.RowIndex]) && canCheck) {
					DataGridViewCheckBoxCell cell = dataGridView1.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
					cell.Value = cell.Value != null ? !Convert.ToBoolean(cell.Value) : cell.TrueValue;
				}
			}
		}
		
		void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			if(e.ColumnIndex == 0) {
				CheckBox cb = null;
				int checkCount = 0;
				int maxCount = 0;
				if(UiMode != "Cells Locked") {
					comboBox1.Enabled = radioButton1.Checked && checkedCount > 0;
					amtRichTextBox1.Enabled = radioButton2.Checked && checkedCount > 0;
					
					switch(dataGridView1.Rows[e.RowIndex].Cells["Tech"].Value.ToString()) {
						case "2G":
							cb = checkBox1;
							checkCount = gsmCheckedCount;
							maxCount = gsmCellsCount;
							checkBox2.Enabled =
								checkBox3.Enabled = gsmCheckedCount == 0;
							break;
						case "3G":
							cb = checkBox2;
							checkCount = umtsCheckedCount;
							maxCount = umtsCellsCount;
							checkBox1.Enabled =
								checkBox3.Enabled = umtsCheckedCount == 0;
							break;
						case "4G":
							cb = checkBox3;
							checkCount = lteCheckedCount;
							maxCount = lteCellsCount;
							checkBox1.Enabled =
								checkBox2.Enabled = lteCheckedCount == 0;
							break;
					}
					label5.Text = "Selected:\n\n2G: " + gsmCheckedCount + "\n3G: " + umtsCheckedCount + "\n4G: " + lteCheckedCount + "\n\nTotal: " + checkedCount;
				}
				else {
					cb = checkBox1;
					amtRichTextBox1.Enabled = checkedCount > 0;
					checkCount = checkedCount;
					maxCount = dataGridView1.RowCount;
				}
				if(cb != null) {
					cb.CheckedChanged -= CheckBoxesCheckedChanged;
					cb.Checked = maxCount > 0 && checkCount == maxCount;
					cb.CheckedChanged += CheckBoxesCheckedChanged;
				}
			}
		}
		
		void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			switch(UiMode) {
				case "Lock Cells":
					if(!dataGridView1.Rows[e.RowIndex].Cells["NOC"].Value.ToString().Contains("ANOC")) {
						e.CellStyle.ForeColor = SystemColors.GrayText;
						if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
							e.CellStyle.BackColor = SystemColors.InactiveBorder;
//						dataGridView1.Rows[e.RowIndex].Frozen = true;
					}
					else {
						if(dataGridView1.Rows[e.RowIndex].Cells["Locked"].Value.ToString() == "YES") {
							e.CellStyle.ForeColor = SystemColors.GrayText;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = SystemColors.InactiveBorder;
//							dataGridView1.Rows[e.RowIndex].Frozen = true;
						}
						else {
							e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
//							dataGridView1.Rows[e.RowIndex].Frozen = false;
						}
					}
					break;
				case "Unlock Cells":
					if(!dataGridView1.Rows[e.RowIndex].Cells["NOC"].Value.ToString().Contains("ANOC")) {
						e.CellStyle.ForeColor = SystemColors.GrayText;
						if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
							e.CellStyle.BackColor = SystemColors.InactiveBorder;
//						dataGridView1.Rows[e.RowIndex].Frozen = true;
					}
					else {
						if(dataGridView1.Rows[e.RowIndex].Cells["Locked"].Value.ToString() == "No") {
							e.CellStyle.ForeColor = SystemColors.GrayText;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = SystemColors.InactiveBorder;
//							dataGridView1.Rows[e.RowIndex].Frozen = true;
						}
						else {
							e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
//							dataGridView1.Rows[e.RowIndex].Frozen = false;
						}
					}
					break;
				case "History":
					e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
					e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
//					dataGridView1.Rows[e.RowIndex].Frozen = false;
					break;
				case "Cells Locked":
					if(dataGridView1.Columns[e.ColumnIndex].Name == "Cell") {
						if(currentSite != null) {
							Cell cell = currentSite.Cells.Find(c => c.Name == dataGridView1.Rows[e.RowIndex].Cells["Cell"].Value.ToString());
							if(cell != null)
								e.CellStyle.BackColor = cell.COOS ? Color.OrangeRed :  Color.LightGreen;
						}
					}
					break;
			}
			
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Cell") {
				if(!string.IsNullOrEmpty(e.Value.ToString())) {
					string cellPrefix = e.Value.ToString();
					if(cellPrefix.EndsWith("W") || cellPrefix.EndsWith("X") || cellPrefix.EndsWith("Y"))
						cellPrefix = cellPrefix.Substring(0, cellPrefix.Length - 2);
					cellPrefix = cellPrefix.RemoveDigits();
					if(cellPrefix.StartsWith("T"))
						cellPrefix = cellPrefix.Substring(1);
					string prefixDescription = string.Empty;
					List<string> temp = Resources.Cells_Prefix.Split('\n').ToList();
					string tempStr = temp.Find(s => s.StartsWith(cellPrefix));
					if(!string.IsNullOrEmpty(tempStr)) {
						string[] strTofind = { " - " };
						prefixDescription = tempStr.Split(strTofind, StringSplitOptions.None)[1];
					}
					
					dataGridView1.Rows[e.RowIndex].Cells["Cell"].ToolTipText = cellPrefix;
					if(!string.IsNullOrEmpty(prefixDescription))
						dataGridView1.Rows[e.RowIndex].Cells["Cell"].ToolTipText += Environment.NewLine + prefixDescription;
				}
			}
			
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Locked")
				e.CellStyle.BackColor = e.Value.ToString() == "YES" ? Color.OrangeRed : Color.LightGreen;
		}
		
		void LockUnlockCellsFormFormClosing(object sender, FormClosingEventArgs e) {
			if(OwnerForm is siteDetails)
				((siteDetails)OwnerForm).currentSite = currentSite;
		}
	}

	[DelimitedRecord(",")]
	public class CellsLockedItem {
		[FieldOrder(1)]
		public string Site;
		[FieldOrder(2)]
		public string Cell;
		[FieldOrder(3)]
		string lockedTime;
		public DateTime LockedTime {
			get {
				return Convert.ToDateTime(lockedTime);
			}
			private set { }
		}
		[FieldOrder(4)]
		public string Reference;
		[FieldOrder(5)]
		public string ReferenceStatus;
		[FieldOrder(6)]
		string crqStart;
		public DateTime CrqStart {
			get {
				return Convert.ToDateTime(crqStart);
			}
			private set { }
		}
		[FieldOrder(7)]
		string crqEnd;
		public DateTime CrqEnd {
			get {
				return Convert.ToDateTime(crqEnd);
			}
			private set { }
		}
		[FieldOrder(8)]
		public string LockedBy;
		[FieldOrder(9)]
		public string Comments;
		
		public CellsLockedItem() { }
	}

	public class CellsLockedSite {
		public string Site;
		public List<CellsLockedItem> CellsLockedItems;
		string lifeTime;
		public string LifeTime {
			get {
				if(string.IsNullOrEmpty(lifeTime)) {
					if(CellsLockedItems[0].Reference.StartsWith("INC") && CellsLockedItems[0].Reference.Length == 15) {
						if(CellsLockedItems[0].ReferenceStatus == "Resolved" || CellsLockedItems[0].ReferenceStatus == "Closed")
							lifeTime = "Expired";
						else
							lifeTime = "NotExpired";
					}
					else {
						if(CellsLockedItems[0].Reference.StartsWith("CRQ") && CellsLockedItems[0].Reference.Length == 15)
							lifeTime = CellsLockedItems[0].CrqEnd <= DateTime.Now ? "Expired" : "NotExpired";
						else
							lifeTime = "NotExpired";
					}
				}
				return lifeTime;
			}
			private set { lifeTime = value; }
		}
		
		public CellsLockedSite(List<CellsLockedItem> cellsLockedItems) {
			CellsLockedItems = cellsLockedItems;
			Site = CellsLockedItems[0].Site;
		}
	}
}