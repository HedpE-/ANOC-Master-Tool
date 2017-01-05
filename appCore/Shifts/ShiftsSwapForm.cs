/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04/01/2017
 * Time: 05:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
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
		
		public ShiftsSwapForm() {
			InitializeComponent();
			
			foreach(DataRow row in DB.Databases.shiftsFile.ShiftLeaders) {
				comboBox1.Items.Add(row["Column3"]);
				comboBox2.Items.Add(row["Column3"]);
			}
			foreach(DataRow row in DB.Databases.shiftsFile.Agents) {
				comboBox1.Items.Add(row["Column3"]);
				comboBox2.Items.Add(row["Column3"]);
			}
			
			dateTimePicker1.MinDate = dateTimePicker2.MinDate = dateTimePicker1.Value = dateTimePicker2.Value = DateTime.Now;
			
			// Create the binding.
			service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
			service.UseDefaultCredentials = true;
			// Set the URL.
			service.Url = new Uri("https://outlook-north.vodafone.com/ews/exchange.asmx");
			
			
		}
		
		void ComboBoxesSelectedIndexChanged(object sender, EventArgs e) {
			ComboBox cb = sender as ComboBox;
		}
		
		void Button1Click(object sender, EventArgs e) {
			NameResolutionCollection allContacts = service.ResolveName(comboBox1.Text, ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution requesterContact = null;
			if(allContacts.Count > 0) {
				if(allContacts.Count == 1)
					requesterContact = allContacts[0];
				else {
					foreach(NameResolution nr in allContacts) {
						if(nr.Contact.CompanyName == "Vodafone Portugal" && (nr.Contact.Department.StartsWith("First Line Operations UK") || nr.Contact.Department.EndsWith("UK - RAN"))) {
							requesterContact = nr;
							break;
						}
					}
				}
			}
			allContacts = service.ResolveName(comboBox2.Text, ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution swapContact = null;
			if(allContacts.Count > 0) {
				if(allContacts.Count == 1)
					swapContact = allContacts[0];
				else {
					foreach(NameResolution nr in allContacts) {
						if(nr.Contact.CompanyName == "Vodafone Portugal" && (nr.Contact.Department.StartsWith("First Line Operations UK") || nr.Contact.Department.EndsWith("UK - RAN"))) {
							swapContact = nr;
							break;
						}
					}
				}
			}
			allContacts = service.ResolveName("Rui Gonçalves", ResolveNameSearchLocation.DirectoryOnly, true);
			NameResolution approverContact = null;
			if(allContacts.Count > 0) {
				if(allContacts.Count == 1)
					approverContact = allContacts[0];
				else {
					foreach(NameResolution nr in allContacts) {
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
			                               "Troca com: " + comboBox2.Text + Environment.NewLine +
			                               "Data início: " + dateTimePicker1.Value.ToString(dateTimePicker1.CustomFormat) + Environment.NewLine +
			                               "Data fim: " + dateTimePicker2.Value.ToString(dateTimePicker2.CustomFormat);
			message.Body = new MessageBody(BodyType.Text, body);
			message.ToRecipients.Add(approverContact.Mailbox);
//			message.CcRecipients.Add(swapContact.Mailbox);
			
			message.SendAndSaveCopy();
			
			MainForm.trayIcon.showBalloon("Shift swap request sent", body);
		}
	}
}
