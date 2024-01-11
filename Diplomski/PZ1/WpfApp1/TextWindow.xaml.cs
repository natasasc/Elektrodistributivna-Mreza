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
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        MainWindow mw;

        public TextWindow(MainWindow mw)
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
                mw.objText.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(textColors.SelectedItem.ToString());
            }
        }

        private void submitText_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;

            if (text.Text == null || text.Text == "")
            {
                MessageBox.Show("Please enter some text.");
                validated = false;
            }

            if (textColors.SelectedItem == null)
            {
                MessageBox.Show("Please choose a color for text.");
                validated = false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(textSize.Text, @"-?\d+(?:\.\d+)?"))
            {
                MessageBox.Show("Please enter a number for text size.");
                textSize.Text = textSize.Text.Remove(textSize.Text.Length - 1);
                validated = false;
            }


            if (validated)
            {
                mw.objText.Text = text.Text;
                mw.objText.Width = double.NaN;      // kazu da je ovako automatski postavljena sirina
                mw.objText.Height = double.NaN;
                mw.objText.FontSize = Double.Parse(textSize.Text);

                mw.FinishedText();
                this.Close();
            }
        }
    }
}
