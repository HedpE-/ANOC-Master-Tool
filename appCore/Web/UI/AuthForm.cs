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

        appCore.UI.ErrorProviderFixed errorProvider = new appCore.UI.ErrorProviderFixed();
		//private string lastUserLoginAttempt = string.Empty;
		//private int loginFailedAttempts = 0;
		
		public AuthForm(string app)
        {
            application = app;

            InitializeComponent();

            if (application == "Confluence")
            {
                ComboBox userComboBox = new ComboBox()
                {
                    Location = textBox1.Location,
                    Name = "UserNameComboBox",
                    Size = textBox1.Size
                };
                userComboBox.Items.Add(Settings.CurrentUser.PT.Email);
                userComboBox.Items.Add(Settings.CurrentUser.UK.Email);
                Controls.Remove(textBox1);
                Controls.Add(userComboBox);
            }

            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.SetIconPadding(textBox1, -17);
            errorProvider.SetIconPadding(textBox2, -17);
        }
		
		void Button1Click(object sender, EventArgs e)
        {
            string username = string.Empty;
            if (application == "OI")
            {
                errorProvider.SetError(textBox1, string.Empty);
                username = textBox1.Text;
            }
            else
            {
                errorProvider.SetError(Controls["UserNameComboBox"], string.Empty);
                username = Controls["UserNameComboBox"].Text;
            }
            errorProvider.SetError(textBox2, string.Empty);

            label3.Text = "Login failed";
			label3.Visible = false;

			if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(textBox2.Text))
            {
				Username = username;
				Password = application == "OI" ? textBox2.Text.Encrypt() : textBox2.Text;

                DialogResult = DialogResult.OK;
				Close();
			}
			else
            {
				label3.Text = "Invalid credentials";
				label3.Visible = true;
                if (string.IsNullOrEmpty(username))
                    errorProvider.SetError(application == "OI" ? textBox1 : Controls["UserNameComboBox"], "Please enter a valid username");
                if (string.IsNullOrEmpty(textBox2.Text))
                    errorProvider.SetError(textBox2, "Please enter a valid password");
            }
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			textBox1.Text = string.Empty;
            try
            {
                Controls["UserNameComboBox"].Text = string.Empty;
            }
            catch { }
			textBox2.Text = string.Empty;

            DialogResult = DialogResult.Cancel;
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
			try
            {
				WebResponse response = request.GetResponse();
			}
			catch (Exception)
            {
				return false;
			}
			
			return true;
		}

        private void AuthForm_Shown(object sender, EventArgs e)
        {
            if (application == "Confluence")
            {
                textBox1.ReadOnly = !string.IsNullOrEmpty(Username);
                textBox2.Focus();
            }

            textBox1.Text = application == "OI" ? Settings.SettingsFile.OIUsername : Username;
            textBox2.Text = application == "OI" ? Settings.SettingsFile.OIPassword.Decrypt() : string.Empty;
            Text = application == "OI" ? "OI Login" : "Confluence Login";
        }
    }
}
