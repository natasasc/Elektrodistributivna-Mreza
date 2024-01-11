using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for EditTextWindow.xaml
    /// </summary>
    public partial class EditTextWindow : Window
    {
        MainWindow mw;

        public EditTextWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    textColors.Items.Add(prop.Name);
                }
            }
        }

        private void textColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.editText.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(textColors.SelectedItem.ToString());
            }
        }

        private void submitText_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;


            if (!System.Text.RegularExpressions.Regex.IsMatch(textSize.Text, @"-?\d+(?:\.\d+)?")
                    && textSize.Text != null && textSize.Text != "")
            {
                MessageBox.Show("Please enter a number for text size.");
                textSize.Text = textSize.Text.Remove(textSize.Text.Length - 1);
                validated = false;
            }


            if (validated)
            {
                if (textSize.Text != null && textSize.Text != "")
                    mw.editText.FontSize = Double.Parse(textSize.Text);

                this.Close();
            }
        }
    }
}
