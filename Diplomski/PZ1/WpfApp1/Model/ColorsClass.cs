using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp1.Model
{
    public class ColorsClass
    {
        System.Windows.Media.Brush subColor;
        System.Windows.Media.Brush nodeColor;
        System.Windows.Media.Brush switchColor;
        BitmapImage subImg;
        BitmapImage nodeImg;
        BitmapImage switchImg;

        public ColorsClass() 
        {
            subColor = null;
            nodeColor = null;
            switchColor = null;
            subImg = null;
            nodeImg = null;
            switchImg = null;
        }

        public ColorsClass(System.Windows.Media.Brush subColor, System.Windows.Media.Brush nodeColor, System.Windows.Media.Brush switchColor,
            BitmapImage subImg, BitmapImage nodeImg, BitmapImage switchImg)
        {
            this.subColor = subColor;
            this.nodeColor = nodeColor;
            this.switchColor = switchColor;
            this.subImg = subImg;
            this.nodeImg = nodeImg;
            this.switchImg = switchImg;
        }

        public System.Windows.Media.Brush SubColor { get { return subColor; } set { subColor = value; } }
        public System.Windows.Media.Brush NodeColor { get { return nodeColor; } set { nodeColor = value; } }
        public System.Windows.Media.Brush SwitchColor { get { return switchColor; } set { switchColor = value; } }
        public BitmapImage SubImg { get { return subImg; } set { subImg = value; } }
        public BitmapImage NodeImg { get { return nodeImg; } set { nodeImg = value; } }
        public BitmapImage SwitchImg { get { return switchImg; } set { switchImg = value; } }
    }
}
