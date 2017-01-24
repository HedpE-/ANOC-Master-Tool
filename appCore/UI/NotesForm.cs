/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 11-03-2015
 * Time: 03:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Data;
using FileHelpers;

namespace appCore.UI
{
	/// <summary>
	/// Description of NotesForm.
	/// </summary>
	public partial class NotesForm : Form
	{
		public NotesForm()
		{
			InitializeComponent();
			
			FileHelperEngine<Contact> engine = new FileHelperEngine<Contact>();
			DataTable Contacts = engine.ReadStringAsDT(Resources.Contacts);
			
			Contacts.Columns["ContactNumber"].ColumnName = "Contact Number";
			Contacts.Columns["AlternateContact"].ColumnName = "Alternate Contact";
			
			dataGridView1.DataSource = Contacts;
			
			foreach (DataGridViewColumn column in dataGridView1.Columns)
				column.SortMode = DataGridViewColumnSortMode.NotSortable;
			
			textBox2.Text = Resources.Cells_Prefix;
			textBox3.Rtf = Resources.Useful_Info;
		}
	}
	
	[DelimitedRecord(",")]
	public class Contact {
		public string Name;
		public string ContactNumber;
		[FieldOptional]
		public string AlternateContact;
		[FieldOptional]
		public string Email;
		
		public Contact() {}
	}
}
