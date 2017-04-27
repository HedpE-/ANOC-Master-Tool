/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 21/11/2016
 * Time: 16:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// PopupHelper
	/// </summary>
	public sealed class PopupHelper : IDisposable
	{
		readonly Control m_control;
		readonly ToolStripDropDown m_tsdd;
		readonly Panel m_hostPanel; // workarround - some controls don't display correctly if they are hosted directly in ToolStripControlHost

		public bool AutoClose {
			get {
				return m_tsdd.AutoClose;
			}
			set {
				m_tsdd.AutoClose = value;
			}
		}

		double opacity = 0.9;
		public double Opacity {
			get {
				return opacity;
			}
			set {
				opacity = value;
				m_tsdd.Opacity = opacity;
			}
		}
		
		public PopupHelper(Control pControl)
		{
			m_hostPanel = new Panel();
			m_hostPanel.Padding = Padding.Empty;
			m_hostPanel.Margin = Padding.Empty;
			m_hostPanel.TabStop = false;
			m_hostPanel.BorderStyle = BorderStyle.None;
			m_hostPanel.BackColor = Color.Transparent;

			m_tsdd = new ToolStripDropDown();
			m_tsdd.CausesValidation = false;

			m_tsdd.Padding = Padding.Empty;
			m_tsdd.Margin = Padding.Empty;
			m_tsdd.Opacity = Opacity;

			m_control = pControl;
			m_control.CausesValidation = false;
			m_control.Resize += MControlResize;

			m_hostPanel.Controls.Add(m_control);

			m_tsdd.Padding = Padding.Empty;
			m_tsdd.Margin = Padding.Empty;

			m_tsdd.MinimumSize = m_tsdd.MaximumSize = m_tsdd.Size = pControl.Size;

			m_tsdd.Items.Add(new ToolStripControlHost(m_hostPanel));
		}

		void ResizeWindow()
		{
			m_tsdd.MinimumSize = m_tsdd.MaximumSize = m_tsdd.Size = m_control.Size;
			m_hostPanel.MinimumSize = m_hostPanel.MaximumSize = m_hostPanel.Size = m_control.Size;
		}

		void MControlResize(object sender, EventArgs e)
		{
			ResizeWindow();
		}

		/// <summary>
		/// Display the popup and keep the focus
		/// </summary>
		/// <param name="pParentControl"></param>
		/// <param name="location"></param>
		public void Show(Control pParentControl, Point location)
		{
			if(pParentControl == null)
				return;

			// position the popup window
			var loc = location != Point.Empty ? location : Cursor.Position;
			m_tsdd.Show(location);
			m_control.Focus();
		}

		public void Close()
		{
			m_tsdd.Close();
		}

		public void Dispose()
		{
			m_control.Resize -= MControlResize;

			m_tsdd.Dispose();
			m_hostPanel.Dispose();
		}
	}
}
