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
    /// Interaction logic for EditEllipseWindow.xaml
    /// </summary>
    public partial class EditEllipseWindow : Window
    {
        MainWindow mw;

        public EditEllipseWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    ellipseColors.Items.Add(prop.Name);
                    ellipseStrokeColors.Items.Add(prop.Name);
                }
            }
        }

        private void ellipseColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ellipseColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.editEllipse.Fill = (System.Windows.Media.Brush)converter.ConvertFromString(ellipseColors.SelectedItem.ToString());
            }
        }

        private void ellipseStrokeColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ellipseStrokeColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.editEllipse.Stroke = (System.Windows.Media.Brush)converter.ConvertFromString(ellipseStrokeColors.SelectedItem.ToString());
            }
        }

        private void submitEllipse_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;

            if (!System.Text.RegularExpressions.Regex.IsMatch(ellipseStrokeThickness.Text, @"-?\d+(?:\.\d+)?")
                    && ellipseStrokeThickness.Text != null && ellipseStrokeThickness.Text != "")
            {
                MessageBox.Show("Please enter a number for EllipseStrokeThickness.");
                ellipseStrokeThickness.Text = ellipseStrokeThickness.Text.Remove(0);
                validated = false;
            }

            if (ellipseOpacity.Text != null && ellipseOpacity.Text != "")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(ellipseOpacity.Text, @"^[1-9][0-9]?$|^100$"))
                {
                    MessageBox.Show("Please enter a number between 1 and 100 for ellipse color opacity.");
                    ellipseOpacity.Text = ellipseOpacity.Text.Remove(0);
                    validated = false;
                }
            }


            if (validated)
            {
                if (ellipseStrokeThickness.Text != null && ellipseStrokeThickness.Text != "")
                    mw.editEllipse.StrokeThickness = Double.Parse(ellipseStrokeThickness.Text);

                if (ellipseOpacity.Text != "" || ellipseOpacity.Text != null)
                {
                    mw.objEllipse.Opacity = Double.Parse(ellipseOpacity.Text) / 100;
                }

                this.Close();
            }
        }
    }
}
