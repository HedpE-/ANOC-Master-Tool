/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 04/01/2017
 * Time: 05:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace appCore.Shifts
{
	partial class ShiftsSwapForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.DateTimePicker dateTimePicker1;
		private System.Windows.Forms.DateTimePicker dateTimePicker2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.DateTimePicker dateTimePicker3;
		private System.Windows.Forms.DateTimePicker dateTimePicker4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShiftsSwapForm));
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
			this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
			this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Sorted = true;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBoxesSelectedIndexChanged);
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			resources.ApplyResources(this.comboBox2, "comboBox2");
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Sorted = true;
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBoxesSelectedIndexChanged);
			// 
			// dateTimePicker1
			// 
			resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
			this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker1.Name = "dateTimePicker1";
			this.dateTimePicker1.ValueChanged += new System.EventHandler(this.DateTimePickersValueChanged);
			// 
			// dateTimePicker2
			// 
			resources.ApplyResources(this.dateTimePicker2, "dateTimePicker2");
			this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker2.Name = "dateTimePicker2";
			this.dateTimePicker2.ValueChanged += new System.EventHandler(this.DateTimePickersValueChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// dateTimePicker3
			// 
			resources.ApplyResources(this.dateTimePicker3, "dateTimePicker3");
			this.dateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker3.Name = "dateTimePicker3";
			this.dateTimePicker3.ValueChanged += new System.EventHandler(this.DateTimePickersValueChanged);
			// 
			// dateTimePicker4
			// 
			resources.ApplyResources(this.dateTimePicker4, "dateTimePicker4");
			this.dateTimePicker4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker4.Name = "dateTimePicker4";
			this.dateTimePicker4.ValueChanged += new System.EventHandler(this.DateTimePickersValueChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// ShiftsSwapForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.dateTimePicker4);
			this.Controls.Add(this.dateTimePicker3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dateTimePicker2);
			this.Controls.Add(this.dateTimePicker1);
			this.Controls.Add(this.comboBox2);
			this.Controls.Add(this.comboBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::appCore.UI.Resources.app_icon;
			this.Name = "ShiftsSwapForm";
			this.ResumeLayout(false);

		}
	}
}