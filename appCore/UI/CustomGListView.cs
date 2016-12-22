/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 22/12/2016
 * Time: 06:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using GlacialComponents.Controls;

namespace appCore.UI
{
	/// <summary>
	/// Description of CustomGListView.
	/// </summary>
	public class CustomGListView : GlacialList
	{
		protected override void OnMouseWheel(MouseEventArgs e)
        {
//            DW("MouseWheel");

            if(vPanelScrollBar.Visible && e.Delta != 0)
            {
                var smallChange = vPanelScrollBar.SmallChange;
                var cValue = vPanelScrollBar.Value;
                if (e.Delta > 0) //Scroll Down
                {
                    var min = vPanelScrollBar.Minimum;
                    if (cValue > min && cValue - smallChange > min) vPanelScrollBar.Value -= smallChange;                    
                }
                else //Scroll Up
                {
                    var max = vPanelScrollBar.Maximum;
                    if (cValue < max && cValue + smallChange < max) vPanelScrollBar.Value += smallChange;
                }
                Invalidate();
            }

            base.OnMouseWheel(e);
        }
	}
}
