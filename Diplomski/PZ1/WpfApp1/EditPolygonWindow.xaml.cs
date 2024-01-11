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
    /// Interaction logic for EditPolygonWindow.xaml
    /// </summary>
    public partial class EditPolygonWindow : Window
    {
        MainWindow mw;

        public EditPolygonWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    polygonColors.Items.Add(prop.Name);
                    polygonStrokeColors.Items.Add(prop.Name);
                }
            }
        }

        private void polygonColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.editPolygon.Fill = (System.Windows.Media.Brush)converter.ConvertFromString(polygonColors.SelectedItem.ToString());
            }
        }

        private void polygonStrokeColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonStrokeColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.editPolygon.Stroke = (System.Windows.Media.Brush)converter.ConvertFromString(polygonStrokeColors.SelectedItem.ToString());
            }
        }

        private void submitPolygon_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;

            if (!System.Text.RegularExpressions.Regex.IsMatch(polygonStrokeThickness.Text, @"-?\d+(?:\.\d+)?")
                    && polygonStrokeThickness.Text != null && polygonStrokeThickness.Text != "")
            {
                MessageBox.Show("Please enter a number for PolygonStrokeThickness.");
                polygonStrokeThickness.Text = polygonStrokeThickness.Text.Remove(0);
                validated = false;
            }

            if (polygonOpacity.Text != null && polygonOpacity.Text != "")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(polygonOpacity.Text, @"^[1-9][0-9]?$|^100$"))
                {
                    MessageBox.Show("Please enter a number between 1 and 100 for polygon color opacity.");
                    polygonOpacity.Text = polygonOpacity.Text.Remove(0);
                    validated = false;
                }
            }

            if (validated)
            {
                if (polygonStrokeThickness.Text != null && polygonStrokeThickness.Text != "")
                    mw.editPolygon.StrokeThickness = Double.Parse(polygonStrokeThickness.Text);

                if (polygonOpacity.Text != "" || polygonOpacity.Text != null)
                {
                    mw.objPolygon.Opacity = Double.Parse(polygonOpacity.Text) / 100;
                }

                this.Close();
            }
        }
    }
}
