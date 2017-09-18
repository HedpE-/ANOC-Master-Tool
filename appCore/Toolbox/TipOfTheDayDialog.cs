using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

//namespace TipOfTheDayDialog
//{
/// <summary>
/// Summary description for TipOfTheDayDialog.
/// </summary>
public class TipOfTheDayDialog : System.Windows.Forms.Form
{
	private System.Windows.Forms.PictureBox pictureBox1;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Panel panel1;
	private System.Windows.Forms.CheckBox chkShowTipsOnStartup;
	private System.Windows.Forms.Button btnNextTip;
	private System.Windows.Forms.Button btnClose;
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.Container components = null;
	private string _tipFile = appCore.Settings.GlobalProperties.ExternalResourceFilesLocation.FullName + "\\tips.txt";
	private static string _tipCountFile = appCore.Settings.GlobalProperties.AppDataRootDir.FullName + "\\tipcount.bin";
    private List<Tip> _tips = new List<Tip>();
	private System.Windows.Forms.Label lblTip;
	private int _tipCount = 0;
	private Tip _currentTip;

	public TipOfTheDayDialog()
	{
		//
		// Required for Windows Form Designer support
		//
		InitializeComponent();

		try
		{
            ReadTipsFile();

			ReadTipIndex();
            
            _tipCount = _tipCount % _tips.Count; // make sure we don't overrun tips file

            UpdateTip();
		}
		catch (Exception ex)
		{
			MessageBox.Show(String.Format("Trouble Creating Tip Dialog {0}", ex.ToString()));
		}
	}

	public static bool IsShowTipsOnStartUp()
	{
        if (File.Exists(_tipCountFile))
        {
            FileStream file = new FileStream(_tipCountFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(file);
			int dontcare = br.ReadInt32();
			bool showTipsOnStartup = br.ReadBoolean();
			br.Close();
			file.Close();
			return showTipsOnStartup;
		}

		return true;
    }

    public static void ToggleShowTipsOnStartUp(bool toggleState)
    {
        if (File.Exists(_tipCountFile))
        {
            FileStream file = new FileStream(_tipCountFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(file);
            int dontcare = br.ReadInt32();
            br.Close();
            file.Close();
            file = new FileStream(_tipCountFile, FileMode.Truncate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(dontcare);
            bw.Write(toggleState);
            bw.Close();
            file.Close();
        }
    }

    public int ReadTipIndex()
	{
        // see if file exists, if not create it
        if (File.Exists(_tipCountFile) == false)
        {
			CreateInitialTipCountFile();
		}
		else
		{
			ReadTipCountFile();
		}

		return _tipCount;
	}
	
	void CreateInitialTipCountFile()
    {
        FileStream file = new FileStream(_tipCountFile, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(file);
		bw.Write(_tipCount);
		bw.Write(true); // show tips on startup
		bw.Close();
		file.Close();
	}


	void WriteCurrentTipCount()
	{
        // Open the stream for writing over the existing tipcount file
        FileStream file = new FileStream(_tipCountFile, FileMode.Truncate, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(file);
		// write the current index
		bw.Write(_tipCount); // increment count
		// write the startup flag
		bw.Write(chkShowTipsOnStartup.Checked); 

		// close the binary writer and the stream
		bw.Close();
		file.Close();
	}

	void ReadTipCountFile()
	{
        // open the stream for reading
        FileStream file = new FileStream(_tipCountFile, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(file);
		_tipCount = br.ReadInt32();
		chkShowTipsOnStartup.Checked = br.ReadBoolean();
		IncrementTipCount(); // increment the count for next tip
		br.Close();
		file.Close();
	}

	void ReadTipsFile()
	{
		try
        {
            StreamReader reader = new StreamReader(_tipFile);
            string tipBlock = reader.ReadToEnd();
			reader.Close();
            string[] tipsArray = tipBlock.Split(new[] { "\r\n" }, StringSplitOptions.None);
            foreach (string tip in tipsArray)
            {
                var temp = tip.Split(';');
                _tips.Add(new Tip(temp[0], temp.Length > 1 ? temp[1] : string.Empty));
            }
		}
		catch (Exception ex)
		{
			MessageBox.Show(String.Format("Could not open tips file - {0}", ex.Message.ToString()));
			btnNextTip.Enabled = false;
			return;
		}
	}

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	protected override void Dispose( bool disposing )
	{
		if( disposing )
		{
			if(components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose( disposing );
	}

	#region Windows Form Designer generated code
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TipOfTheDayDialog));
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.label1 = new System.Windows.Forms.Label();
		this.lblTip = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.chkShowTipsOnStartup = new System.Windows.Forms.CheckBox();
		this.btnNextTip = new System.Windows.Forms.Button();
		this.btnClose = new System.Windows.Forms.Button();
		this.panel1.SuspendLayout();
		this.SuspendLayout();
		// 
		// pictureBox1
		// 
		this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
		this.pictureBox1.Location = new System.Drawing.Point(32, 16);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(40, 64);
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		// 
		// label1
		// 
		this.label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
		this.label1.Location = new System.Drawing.Point(104, 32);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(160, 24);
		this.label1.TabIndex = 1;
		this.label1.Text = "Did You Know...";
		// 
		// lblTip
		// 
		this.lblTip.Location = new System.Drawing.Point(32, 96);
		this.lblTip.Name = "lblTip";
		this.lblTip.Size = new System.Drawing.Size(288, 104);
		this.lblTip.TabIndex = 2;
		this.lblTip.Text = "Could not read tips file...";
		// 
		// panel1
		// 
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.pictureBox1);
		this.panel1.Controls.Add(this.lblTip);
		this.panel1.Location = new System.Drawing.Point(8, 16);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(360, 224);
		this.panel1.TabIndex = 3;
		// 
		// chkShowTipsOnStartup
		// 
		this.chkShowTipsOnStartup.Checked = true;
		this.chkShowTipsOnStartup.CheckState = System.Windows.Forms.CheckState.Checked;
		this.chkShowTipsOnStartup.Location = new System.Drawing.Point(8, 256);
		this.chkShowTipsOnStartup.Name = "chkShowTipsOnStartup";
		this.chkShowTipsOnStartup.Size = new System.Drawing.Size(136, 16);
		this.chkShowTipsOnStartup.TabIndex = 4;
		this.chkShowTipsOnStartup.Text = "Show Tips On StartUp";
		// 
		// btnNextTip
		// 
		this.btnNextTip.BackColor = System.Drawing.SystemColors.Control;
		this.btnNextTip.Image = ((System.Drawing.Image)(resources.GetObject("btnNextTip.Image")));
		this.btnNextTip.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.btnNextTip.Location = new System.Drawing.Point(160, 252);
		this.btnNextTip.Name = "btnNextTip";
		this.btnNextTip.Size = new System.Drawing.Size(88, 24);
		this.btnNextTip.TabIndex = 5;
		this.btnNextTip.Text = "&Next Tip";
		this.btnNextTip.Click += new System.EventHandler(this.btnNextTip_Click);
		// 
		// btnClose
		// 
		this.btnClose.BackColor = System.Drawing.SystemColors.Control;
		this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnClose.Location = new System.Drawing.Point(272, 252);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(88, 24);
		this.btnClose.TabIndex = 6;
		this.btnClose.Text = "&Close";
		// 
		// TipOfTheDayDialog
		// 
		this.AcceptButton = this.btnClose;
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.BackColor = System.Drawing.SystemColors.Control;
		this.ClientSize = new System.Drawing.Size(386, 288);
		this.Controls.Add(this.btnClose);
		this.Controls.Add(this.btnNextTip);
		this.Controls.Add(this.chkShowTipsOnStartup);
		this.Controls.Add(this.panel1);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "TipOfTheDayDialog";
		this.Text = "Tip of the Day";
		this.Closing += new System.ComponentModel.CancelEventHandler(this.TipOfTheDayDialog_Closing);
		this.panel1.ResumeLayout(false);
		this.ResumeLayout(false);

	}
    #endregion

    /// <summary>
    /// Get the tip at the current tip count
    /// </summary>
    private Tip GetTip()
    {
        return _tips[_tipCount];
    }

    private void UpdateTip()
	{
		_currentTip = GetTip();
		lblTip.Text = _currentTip.Text;
        //pictureBox2.Image = _currentTip.PicturePath;
	}

	void IncrementTipCount()
	{
		_tipCount = (_tipCount + 1) % _tips.Count;
	}

	private void btnNextTip_Click(object sender, System.EventArgs e)
	{
		IncrementTipCount();
		UpdateTip();
	}


	private void TipOfTheDayDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		WriteCurrentTipCount();
	}

    private class Tip
    {
        public string Text { get; set; }
        public string PicturePath { get; set; }
        //public Image Picture
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        public Tip(string text, string picturePath)
        {
            Text = text;
            PicturePath = picturePath;
        }
    }
}


//}
