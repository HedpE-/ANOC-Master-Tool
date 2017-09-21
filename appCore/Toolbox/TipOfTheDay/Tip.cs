using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appCore.Toolbox.TipOfTheDay
{
    public class Tip
    {
        public string Text { get; private set; }
        public string PicturePath { get; private set; }

        public Tip(string text, string picturePath)
        {
            Text = text;
            if (!string.IsNullOrEmpty(picturePath))
                PicturePath = Settings.GlobalProperties.ExternalResourceFilesLocation + "\\" + picturePath;
        }
    }
}
