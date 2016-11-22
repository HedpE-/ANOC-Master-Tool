namespace appCore.UI
{
    partial class ScrollableMessageBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.txtMessage = new System.Windows.Forms.RichTextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.SuspendLayout();
        	// 
        	// txtMessage
        	// 
        	this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
        	this.txtMessage.BackColor = System.Drawing.SystemColors.Control;
        	this.txtMessage.Location = new System.Drawing.Point(0, 53);
        	this.txtMessage.MaximumSize = new System.Drawing.Size(920, 770);
        	this.txtMessage.MinimumSize = new System.Drawing.Size(470, 400);
        	this.txtMessage.Name = "txtMessage";
        	this.txtMessage.ReadOnly = true;
        	this.txtMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
        	this.txtMessage.Size = new System.Drawing.Size(572, 625);
        	this.txtMessage.TabIndex = 0;
        	this.txtMessage.Text = "";
        	this.txtMessage.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace, this.txtMessage.Font.Size);
        	// 
        	// label1
        	// 
        	this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label1.Location = new System.Drawing.Point(0, 4);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(572, 23);
        	this.label1.TabIndex = 1;
        	this.label1.Text = "Template copied to Clipboard";
        	this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	// 
        	// label2
        	// 
        	this.label2.BackColor = System.Drawing.Color.Transparent;
        	this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label2.Location = new System.Drawing.Point(345, 681);
        	this.label2.Name = "label2";
        	this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        	this.label2.Size = new System.Drawing.Size(227, 44);
        	this.label2.TabIndex = 2;
        	this.label2.Text = "label2";
        	this.label2.Visible = false;
        	// 
        	// ScrollableMessageBox
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoSize = true;
        	this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        	this.ClientSize = new System.Drawing.Size(572, 725);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.txtMessage);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        	this.Icon = global::appCore.UI.Resources.MB_0001_vodafone3;
        	this.MaximizeBox = false;
        	this.MaximumSize = new System.Drawing.Size(926, 776);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(476, 486);
        	this.Name = "ScrollableMessageBox";
        	this.Text = "Message";
        	this.TopMost = true;
        	this.Load += new System.EventHandler(this.ScrollableMessageBox_Load);
        	this.Resize += new System.EventHandler(this.ScrollableMessageBox_Resize);
        	this.ResumeLayout(false);

        }

        private System.Windows.Forms.RichTextBox txtMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}