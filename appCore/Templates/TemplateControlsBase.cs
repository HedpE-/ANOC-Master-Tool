/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31-07-2016
 * Time: 00:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using appCore.Settings;
using appCore.UI;
using appCore.SiteFinder.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of TroubleshootControls.
	/// </summary>
	public class TemplateControlsBase : Panel
	{
        protected ErrorProviderFixed errorProvider = new ErrorProviderFixed();
		
		public AMTMenuStrip MainMenu = new AMTMenuStrip();
        protected ToolStripMenuItem generateTemplateToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem copyToNewTemplateToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem SiteDetailsToolStripMenuItem = new ToolStripMenuItem();

        public static siteDetails SiteDetailsUI;
		
		public Site currentSite;
        protected Template currentTemplate;
        protected Template previousTemplate;

        protected int paddingLeftRight = 1;
		public int PaddingLeftRight
        {
			get
            {
                return paddingLeftRight;
            }
			set
            {
				paddingLeftRight = value;
				DynamicControlsSizesLocations();
			}
		}

        protected int paddingTopBottom = 1;
		public int PaddingTopBottom
        {
			get
            {
                return paddingTopBottom;
            }
			set
            {
				paddingTopBottom = value;
				DynamicControlsSizesLocations();
			}
		}

        protected bool toggled;
		public bool ToggledState
        {
			get
            {
				return toggled;
			}
			set
            {
				if(value != toggled)
					toggled = value;
			}
		}

        protected bool fromLog = false; // flag used to know if constructor was called from LogEditor

        protected UiEnum _uiMode;
        protected virtual UiEnum UiMode
        {
            get
            {
                return _uiMode;
            }
            set
            {
                _uiMode = value;
            }
        }

        public TemplateControlsBase()
		{
			//UiMode = UiEnum.Template;

   //         //fromLog = false;

			//if(GlobalProperties.SiteFinderMainswitch)
   //         {
   //             SiteFinder_Toggle(false, false);
   //             MainMenu.siteFinder_Toggle(false, false);
   //         }
		}

		protected virtual void SiteFinder_Toggle(bool toggle, bool siteFound)
        {
        }

		protected void SiteDetailsButtonClick(object sender, EventArgs e)
		{
			if(SiteDetailsUI != null) {
				SiteDetailsUI.Close();
				SiteDetailsUI.Dispose();
			}
			SiteDetailsUI = new siteDetails(currentSite);
			SiteDetailsUI.Show();
		}

		protected virtual void ClearAllControls(object sender, EventArgs e)
		{
		}
		
		protected virtual void LoadTemplateFromLog(object sender, EventArgs e)
        {
			var form = Application.OpenForms.OfType<MainForm>().First();
			form.Invoke((MethodInvoker)async delegate
            {
                await form.FillTemplateFromLog(currentTemplate);
            });
		}
		
		protected virtual void GenerateTemplate(object sender, EventArgs e)
        {
		}
		
		public virtual void SiteFinderSwitch(string toState)
        {
		}
		
		protected async void LoadDisplayOiDataTable(object sender, EventArgs e)
        {
//			if(e.Button == MouseButtons.Left) {
			string dataToShow = ((ToolStripMenuItem)sender).Name.Replace("Button", string.Empty);
			var fc = Application.OpenForms.OfType<OiSiteTablesForm>().Where(f => f.OwnerControl == this && f.Text.EndsWith(dataToShow)).ToList();
			if(fc.Count > 0)
            {
				fc[0].Close();
				fc[0].Dispose();
			}
			
			if(currentSite.Exists)
            {
				DataTable dt = new DataTable();
				switch(dataToShow)
                {
					case "INCs":
						if(currentSite.Incidents == null)
                        {
                            MainMenu.INCsButton.Text = "Loading data...";
                            MainMenu.INCsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("INC");
							if(currentSite.Incidents != null)
                            {
								if(currentSite.Incidents.Count > 0)
                                {
									MainMenu.INCsButton.Enabled = true;
									MainMenu.INCsButton.ForeColor = Color.DarkGreen;
									MainMenu.INCsButton.Text = "INCs (" + currentSite.Incidents.Count + ")";
								}
								else
                                {
									MainMenu.INCsButton.Enabled = false;
									MainMenu.INCsButton.Text = "No INC history";
								}
							}
							return;
						}
						break;
					case "CRQs":
                        if (currentSite.Changes == null)
                        {
                            MainMenu.CRQsButton.Text = "Loading data...";
                            MainMenu.CRQsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("CRQ");
							if(currentSite.Changes != null)
                            {
								if(currentSite.Changes.Count > 0)
                                {
									MainMenu.CRQsButton.Enabled = true;
									MainMenu.CRQsButton.ForeColor = Color.DarkGreen;
									MainMenu.CRQsButton.Text = "CRQs (" + currentSite.Changes.Count + ")";
								}
								else
                                {
									MainMenu.CRQsButton.Enabled = false;
									MainMenu.CRQsButton.Text = "No CRQ history";
								}
							}
							return;
						}
						break;
					case "BookIns":
						if(currentSite.Visits == null)
                        {
                            MainMenu.BookInsButton.Text = "Loading data...";
                            MainMenu.BookInsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("Bookins");
							if(currentSite.Visits != null)
                            {
								if(currentSite.Visits.Count > 0)
                                {
									MainMenu.BookInsButton.Enabled = true;
									MainMenu.BookInsButton.ForeColor = Color.DarkGreen;
									MainMenu.BookInsButton.Text = "Book Ins List (" + currentSite.Visits.Count + ")";
								}
								else
                                {
									MainMenu.BookInsButton.Enabled = false;
									MainMenu.BookInsButton.Text = "No Book In history";
								}
							}
							return;
						}
						break;
					case "ActiveAlarms":
						if(currentSite.Alarms == null)
                        {
                            MainMenu.ActiveAlarmsButton.Text = "Loading data...";
                            MainMenu.ActiveAlarmsButton.Enabled = false;
                            await currentSite.requestOIDataAsync("Alarms");
							if(currentSite.Alarms != null)
                            {
								if(currentSite.Alarms.Count > 0)
                                {
									MainMenu.ActiveAlarmsButton.Enabled = true;
									MainMenu.ActiveAlarmsButton.ForeColor = Color.DarkGreen;
									MainMenu.ActiveAlarmsButton.Text = "Active alarms (" + currentSite.Alarms.Count + ")";
								}
								else
                                {
									MainMenu.ActiveAlarmsButton.Enabled = false;
									MainMenu.ActiveAlarmsButton.Text = "No alarms to display";
								}
							}
							return;
						}
						break;
				}
				
				OiSiteTablesForm OiTable = null;
				switch(dataToShow)
                {
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
				}
				OiTable.Show();
			}
		}
		
		protected async void RefreshOiData(object sender, EventArgs e)
        {
            await MainMenu.ShowLoading();
            await currentSite.requestOIDataAsync("INCCRQBookinsAlarms");
            await MainMenu.siteFinder_Toggle(true);
		}
		
		protected virtual void InitializeComponent()
		{
		}
		
		protected virtual void DynamicControlsSizesLocations()
        {
		}
	}
}