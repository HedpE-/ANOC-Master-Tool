/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04/01/2017
 * Time: 05:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;

namespace appCore.Shifts
{
	/// <summary>
	/// Description of ShiftsSwapForm.
	/// </summary>
	public partial class ShiftsSwapForm : Form
	{
		ExchangeService service;
		
		ShiftsCalendar requesterCalendar;
		ShiftsCalendar swappedCalendar;
		
		public ShiftsSwapForm() {
			InitializeComponent();
			
			if(Settings.CurrentUser.Role == "Shift Leader") {
				foreach(string str in DB.Databases.shiftsFile.ShiftLeaders) {
					comboBox1.Items.Add(str);
					comboBox2.Items.Add(str);
				}
			}
			else {
				foreach(string str in DB.Databases.shiftsFile.Agents) {
					comboBox1.Items.Add(str);
					comboBox2.Items.Add(str);
				}
			}
			
			
			dateTimePicker1.MinDate =
				dateTimePicker2.MinDate =
				dateTimePicker3.MinDate =
				dateTimePicker4.MinDate =
				dateTimePicker1.Value =
				dateTimePicker2.Value =
				dateTimePicker3.Value =
				dateTimePicker4.Value =
				DateTime.Now;
			
			// Create the binding.
			service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
			service.UseDefaultCredentials = true;
			// Set the URL.
			service.Url = new Uri("https://outlook-north.vodafone.com/ews/exchange.asmx");
			
			requesterCalendar = new ShiftsCalendar(comboBox1.Text, dateTimePicker1.Value, dateTimePicker2.Value);
			Controls.Add(requesterCalendar);
			
			requesterCalendar.BackColor = SystemColors.Control;
			requesterCalendar.BordersToDraw = appCore.UI.AMTRoundCornersPanel.Borders.None;
			requesterCalendar.CornersToRound = appCore.UI.AMTRoundCornersPanel.Corners.None;
			requesterCalendar.DoubleBufferActive = false;
			requesterCalendar.Location = new Point(3, 144);
			requesterCalendar.Name = "requesterCalendar";
			
			swappedCalendar = new ShiftsCalendar(comboBox2.Text, dateTimePicker3.Value, dateTimePicker4.Value);
			Controls.Add(swappedCalendar);
			
			swappedCalendar.BackColor = SystemColors.Control;
			swappedCalendar.BordersToDraw = appCore.UI.AMTRoundCornersPanel.Borders.None;
			swappedCalendar.CornersToRound = appCore.UI.AMTRoundCornersPanel.Corners.None;
			swappedCalendar.DoubleBufferActive = false;
			swappedCalendar.Location = new Point(237, 144);
			swappedCalendar.Name = "swappedCalendar";
		}
		
		void ComboBoxesSelectedIndexChanged(object sender, EventArgs e) {
			ComboBox cb = sender as ComboBox;
			if(!string.IsNullOrEmpty(cb.Text)) {
				if(cb.Name == "comboBox1")
					requesterCalendar.RedrawCalendar(cb.Text, dateTimePicker1.Value, dateTimePicker2.Value);
				else
					swappedCalendar.RedrawCalendar(cb.Text, dateTimePicker3.Value, dateTimePicker4.Value);
			}
		}
		
		void DateTimePickersValueChanged(object sender, EventArgs e) {
			DateTimePicker dtp = sender as DateTimePicker;
			switch(dtp.Name) {
				case "dateTimePicker1": case "dateTimePicker2":
					if(requesterCalendar != null)
						requesterCalendar.RedrawCalendar(comboBox1.Text, dateTimePicker1.Value, dateTimePicker2.Value);
					break;
				case "dateTimePicker3": case "dateTimePicker4":
					if(swappedCalendar != null)
						swappedCalendar.RedrawCalendar(comboBox2.Text, dateTimePicker3.Value, dateTimePicker4.Value);
					break;
			}
		}
		
		void Button1Click(object sender, EventArgs e) {
			NameResolutionCollection foundContacts = service.ResolveName(comboBox1.Text, ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution requesterContact = null;
			if(foundContacts.Count > 0) {
				if(foundContacts.Count == 1)
					requesterContact = foundContacts[0];
				else {
					foreach(NameResolution nr in foundContacts) {
						if(nr.Contact.CompanyName == "Vodafone Portugal" && (nr.Contact.Department.StartsWith("First Line Operations UK") || nr.Contact.Department.EndsWith("UK - RAN"))) {
							requesterContact = nr;
							break;
						}
					}
				}
			}
			foundContacts = service.ResolveName(comboBox2.Text, ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution swapContact = null;
			if(foundContacts.Count > 0) {
				if(foundContacts.Count == 1)
					swapContact = foundContacts[0];
				else {
					foreach(NameResolution nr in foundContacts) {
						if(nr.Contact.CompanyName == "Vodafone Portugal" && (nr.Contact.Department.StartsWith("First Line Operations UK") || nr.Contact.Department.EndsWith("UK - RAN"))) {
							swapContact = nr;
							break;
						}
					}
				}
			}
			foundContacts = service.ResolveName("Rui Gonçalves", ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution approverContact = null;
			if(foundContacts.Count > 0) {
				if(foundContacts.Count == 1)
					approverContact = foundContacts[0];
				else {
					foreach(NameResolution nr in foundContacts) {
						if(nr.Contact.CompanyName == "Vodafone Portugal" && (nr.Contact.Department.StartsWith("First Line Operations UK") || nr.Contact.Department.EndsWith("UK - RAN"))) {
							approverContact = nr;
							break;
						}
					}
				}
			}
			
			EmailMessage message = new EmailMessage(service);
			
			message.Subject = "Troca de turno";
			string body = "Interessado: " + comboBox1.Text + Environment.NewLine +
				"Data início: " + dateTimePicker1.Value.ToString(dateTimePicker1.CustomFormat) + Environment.NewLine +
				"Data fim: " + dateTimePicker2.Value.ToString(dateTimePicker2.CustomFormat) + Environment.NewLine +
				"Troca com: " + comboBox2.Text + Environment.NewLine +
				"Data início: " + dateTimePicker3.Value.ToString(dateTimePicker3.CustomFormat) + Environment.NewLine +
				"Data fim: " + dateTimePicker4.Value.ToString(dateTimePicker4.CustomFormat);
			
			if(((int)(dateTimePicker2.Value - dateTimePicker1.Value).TotalDays) != ((int)(dateTimePicker4.Value - dateTimePicker3.Value).TotalDays)) {
				
				return;
			}
			
			message.Body = new MessageBody(BodyType.Text, body);
			message.ToRecipients.Add(approverContact.Mailbox);
//			message.CcRecipients.Add(swapContact.Mailbox);
			
			message.SendAndSaveCopy();
			
			MainForm.trayIcon.showBalloon("Shift swap request sent", body);
		}
	}
}
