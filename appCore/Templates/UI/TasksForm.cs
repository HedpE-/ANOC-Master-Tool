/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-11-2014
 * Time: 3:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of TasksForm.
	/// </summary>
	public partial class TasksForm : Form
	{
		public string siteID;
		public string siteAddress;
		public string cct;
		public string siteTEF;
		public string relatedINC;
		public string powerCompany;
		string _currentTask = "PWR";
		
		public TasksForm()
		{
			InitializeComponent();
		}
		
		public string currentTask {
			get {
				return _currentTask;
			}
			set {
				_currentTask = value;
				switch(_currentTask) {
					case "PWR":
						textBox1.Text = siteID;
						textBox2.Text = siteAddress;
						textBox3.Height = 20;
						textBox3.Multiline = false;
						textBox3.ReadOnly = true;
						textBox3.Text = siteAddress.Substring(siteAddress.LastIndexOf(',') + 1).Trim();
						textBox4.Visible = true;
						textBox4.Text = powerCompany;
						textBox5.Visible = true;
						checkBox1.Visible = true;
						label1.Text = "Site";
						label2.Text = "Address";
						label3.Text = "Post Code";
						label4.Visible = true;
						label4.Text = "Power Company";
						label5.Text = "Fault Reference";
						label5.Visible = true;
                        label6.Visible = false;
                        label7.Visible = false;
                        comboBox1.Visible = false;
						if(!checkBox1.Checked)
							dateTimePicker1.Visible = false;
						dateTimePicker1.Format = DateTimePickerFormat.Custom;
						dateTimePicker1.CustomFormat = "HH:mm";
						dateTimePicker1.Location = new System.Drawing.Point(387, 114);
						dateTimePicker1.Size = new System.Drawing.Size(52, 20);
						numericUpDown1.Visible = false;
						break;
					case "BT": case "VF Fixed": case "MLL":
						label1.Text = "Site";
						label2.Text = "Address";
						label3.Text = _currentTask + " Circuit(s)";
						label4.Visible = false;
						label5.Text = _currentTask + " Reference";
						label5.Visible = true;
                        label6.Visible = false;
                        label7.Visible = false;
                        textBox1.Text = siteID;
						textBox2.Text = siteAddress;
						textBox3.Height = 46;
						textBox3.Multiline = true;
						textBox3.ReadOnly = false;
						textBox3.ScrollBars = ScrollBars.Vertical;
						textBox3.Text = cct;
						textBox4.Visible = false;
						textBox5.Visible = true;
						if(_currentTask == "BT")
							textBox5.Text = relatedINC;
						comboBox1.Visible = false;
						checkBox1.Visible = false;
						dateTimePicker1.Visible = false;
						numericUpDown1.Visible = false;
						break;
					case "TEF":
						textBox1.Text = siteID;
						textBox2.Text = siteAddress;
						textBox3.Height = 20;
						textBox3.Multiline = false;
						textBox3.ReadOnly = true;
						textBox3.Text = siteTEF;
						textBox4.Visible = true;
						textBox4.Text = string.Empty;
						textBox5.Visible = false;
						label1.Text = "Site";
						label2.Text = "Address";
						label3.Text = "TEF Site";
						label4.Visible = true;
						label4.Text = "TEF Reference";
						label5.Visible = false;
						label6.Visible = false;
                        label7.Visible = false;
                        comboBox1.Visible = false;
						checkBox1.Visible = false;
						dateTimePicker1.Visible = false;
						numericUpDown1.Visible = false;
						break;
					case "Monitoring":
						textBox1.Text = siteID;
						textBox2.Text = siteAddress;
						textBox3.Height = 20;
						textBox3.Multiline = false;
						textBox3.ReadOnly = true;
						textBox3.Text = string.Empty;
						textBox4.Visible = true;
						textBox4.Text = string.Empty;
						textBox5.Visible = false;
						label1.Text = "Site";
						label2.Text = "Address";
						label4.Text = "Fault to monitor";
						label4.Visible = true;
						label5.Text = "Faul clear time";
						label5.Visible = true;
						label6.Text = "Monitor duration";
						label6.Visible = true;
                        label7.Text = string.Empty;
                        label7.Visible = true;
						dateTimePicker1.Visible = true;
						dateTimePicker1.Location = textBox5.Location;
						dateTimePicker1.Size = textBox5.Size;
						dateTimePicker1.Format = DateTimePickerFormat.Custom;
						dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm";
                        dateTimePicker1.Value = DateTime.Now;
						numericUpDown1.Visible = true;
						numericUpDown1.Text = "1";
                        comboBox1.Visible = true;
                        comboBox1.SelectedIndex = 0;
                        checkBox1.Visible = false;
                        break;
				}
			}
        }

        public void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
                currentTask = rb.Text;
        }

        void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked) {
				dateTimePicker1.Visible = true;
				dateTimePicker1.Text = DateTime.Now.ToString("HH:mm");
			}
			else {
				dateTimePicker1.Visible = false;
			}
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			string errmsg = "";
			switch (currentTask) {
				case "PWR":
					if (string.IsNullOrEmpty(textBox4.Text)) {
						errmsg = errmsg + "         - Power Company missing\n";
					}
					if (string.IsNullOrEmpty(textBox5.Text)) {
						errmsg = errmsg + "         - Fault Reference missing\n";
					}
					if (checkBox1.Checked && dateTimePicker1.Text == "") {
						errmsg = errmsg + "          - ETR missing\n";
					}
					break;
				case "BT": case "VF Fixed": case "MLL":
					if (string.IsNullOrEmpty(textBox3.Text)) {
						errmsg = errmsg + "         - " + currentTask + " circuit(s) missing\n";
					}
					if (string.IsNullOrEmpty(textBox5.Text)) {
						errmsg = errmsg + "         - " + currentTask + " Reference missing\n";
					}
					break;
				case "TEF":
					if (string.IsNullOrEmpty(textBox3.Text)) {
						errmsg = errmsg + "         - TEF site missing\n           Insert it in the main application window and try again\n";
					}
					if (string.IsNullOrEmpty(textBox4.Text)) {
						errmsg = errmsg + "         - TEF Reference missing\n";
					}
					break;
				case "Monitoring":
					if (string.IsNullOrEmpty(textBox4.Text)) {
						errmsg = errmsg + "         - Fault to monitor missing\n";
					}
					break;
			}
			if (!string.IsNullOrEmpty(errmsg)) {
                LoadingPanel loading = new LoadingPanel();
                loading.Show(false, this);
				FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loading.Close();
				return;
			}
			string taskString = string.Empty;
			switch (currentTask) {
				case "PWR":
					taskString = "#PWR ";
					if (checkBox1.Checked)
						taskString += "ETR: " + dateTimePicker1.Text;
					else
						taskString += "No ETR";
					taskString += " - Site " + textBox1.Text + " - " + textBox3.Text + " - " + textBox4.Text + " - " + textBox5.Text;
					break;
				case "BT": case "VF Fixed": case "MLL":
					taskString = "Site " + textBox1.Text + " - ";
					string[] rng = textBox3.Text.Split('\n');
					if (rng.Length > 0) {
						foreach(string CCT in rng) {
							if (CCT == rng[0])
								taskString += CCT;
							else
								taskString += " / " + CCT;
						}
					}
					else
						taskString += rng[0];
					taskString += " - " + textBox5.Text;
					taskString = taskString.Replace("\n","");
					break;
				case "TEF":
					taskString = "Site " + textBox1.Text + " - TEF site " + textBox3.Text + " - Ref. " + textBox4.Text;
					break;
				case "Monitoring":
//					double monitorHours = comboBox1.Text == "Hours" ? Convert.ToDouble(numericUpDown1.Text) : Convert.ToDouble(numericUpDown1.Text * 24);
					DateTime monitorEndTime = DateTime.ParseExact(dateTimePicker1.Text, "dd/MM/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture)
						.AddHours(comboBox1.Text == "Hours" ? Convert.ToDouble(numericUpDown1.Text) : Convert.ToDouble(Convert.ToInt16(numericUpDown1.Text) * 24));
					taskString = "Monitor until " + monitorEndTime + " - Site " + textBox1.Text;
					if(!string.IsNullOrEmpty(textBox4.Text))
						taskString += " - " + textBox4.Text;
					//taskString += " for " + numericUpDown1.Text + "h";
					break;
			}
			Clipboard.SetText(taskString);
			MainForm.trayIcon.showBalloon("Template successfully copied to Clipboard","\n\n" + taskString);
		}

        private void MonitoringEndTimeChanged(object sender, EventArgs e)
        {
            if(currentTask == "Monitoring")
            {
                if(!string.IsNullOrEmpty(numericUpDown1.Value.ToString()) && !string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrEmpty(dateTimePicker1.Value.ToString()))
                {
                    DateTime monitorEndTime = DateTime.ParseExact(dateTimePicker1.Text, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                        .AddHours(comboBox1.Text == "Hours" ? Convert.ToDouble(numericUpDown1.Text) : Convert.ToDouble(Convert.ToInt16(numericUpDown1.Text) * 24));
                    label7.Text = monitorEndTime.ToString("dd/MM/yyyy HH:mm");
                    //System.Threading.Thread.Sleep(1);
                }
            }
        }
    }
}