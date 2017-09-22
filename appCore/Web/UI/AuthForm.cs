/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 10-06-2015
 * Time: 02:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace appCore.Web.UI
{
	/// <summary>
	/// Description of AuthForm.
	/// </summary>
	public sealed partial class AuthForm : Form
	{
		public string Username = string.Empty;
		public string Password = string.Empty;

        string application; 
		//private string lastUserLoginAttempt = string.Empty;
		//private int loginFailedAttempts = 0;
		
		public AuthForm(string app)
		{
			InitializeComponent();

            application = app;
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			label3.Text = "Login failed";
			label3.Visible = false;
			if(!(string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox2.Text)))
            {
				Username = textBox1.Text;
				Password = application == "OI" ? textBox2.Text.EncryptText() : textBox2.Text;
				
				Close();
			}
			else
            {
				label3.Text = "No credentials entered";
				label3.Visible = true;
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			textBox1.Text = string.Empty;
			textBox2.Text = string.Empty;
			Close();
		}

		public bool validateLogin(string url)
		{
			string username = textBox1.Text;
			string password = textBox2.Text;
			//string username = Tools.SettingsFileHandler("OIUsername","read",null, MainForm.SettingsFile);
			//string password = Tools.EncryptDecryptText("Dec",Tools.SettingsFileHandler("OIPassword","read",null, MainForm.SettingsFile));

			StringBuilder postData = new StringBuilder();
			postData.Append("/SSO/index.asp?url=http%3A%2F%2F195%2E233%2E194%2E118%2F username=" + username + "&password=" + password);

			ASCIIEncoding ascii = new ASCIIEncoding();
			byte[] postBytes = ascii.GetBytes(postData.ToString());
			
			WebRequest request = WebRequest.Create(url);
			request.Credentials = CredentialCache.DefaultCredentials;
			request.Method = "POST";
			request.ContentLength = postBytes.Length;
			Stream dataStream = request.GetRequestStream ();
			dataStream.Write(postBytes, 0, postBytes.Length);
			try {
				WebResponse response = request.GetResponse();
			}
			catch (Exception) {
				return false;
			}
			
			return true;
		}

        private void AuthForm_Shown(object sender, EventArgs e)
        {
            if (application == "Confluence")
                textBox1.ReadOnly = !string.IsNullOrEmpty(Username);

            textBox1.Text = application == "OI" ? Settings.SettingsFile.OIUsername : Username;
            textBox2.Text = application == "OI" ? Settings.SettingsFile.OIPassword.DecryptText() : string.Empty;
            Text = application == "OI" ? "OI Login" : "Confluence Login";
        }
    }
}
