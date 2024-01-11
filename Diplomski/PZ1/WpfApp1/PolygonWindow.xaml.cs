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
    /// Interaction logic for PolygonWindow.xaml
    /// </summary>
    public partial class PolygonWindow : Window
    {
        MainWindow mw;

        public PolygonWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    polygonColors.Items.Add(prop.Name);
                    polygonStrokeColors.Items.Add(prop.Name);
                    polygonTextColors.Items.Add(prop.Name);
                }
            }
        }

        private void polygonColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.objPolygon.Fill = (System.Windows.Media.Brush)converter.ConvertFromString(polygonColors.SelectedItem.ToString());
            }
        }

        private void polygonStrokeColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonStrokeColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.objPolygon.Stroke = (System.Windows.Media.Brush)converter.ConvertFromString(polygonStrokeColors.SelectedItem.ToString());
            }
        }

        private void polygonTextColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonTextColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.textPolygon.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(polygonTextColors.SelectedItem.ToString());
            }
        }

        private void submitPolygon_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;

            if (!System.Text.RegularExpressions.Regex.IsMatch(polygonStrokeThickness.Text, @"-?\d+(?:\.\d+)?"))
            {
                MessageBox.Show("Please enter a number for PolygonStrokeThickness.");
                polygonStrokeThickness.Text = polygonStrokeThickness.Text.Remove(0);
                validated = false;
            }

            if (polygonColors.SelectedItem == null)
            {
                MessageBox.Show("Please choose a color for polygon fill.");
                validated = false;
            }

            if (polygonStrokeColors.SelectedItem == null)
            {
                MessageBox.Show("Please choose a color for polygon stroke.");
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
                mw.objPolygon.StrokeThickness = Double.Parse(polygonStrokeThickness.Text);

                if (polygonOpacity.Text != "" || polygonOpacity.Text != null)
                {
                    mw.objPolygon.Opacity = Double.Parse(polygonOpacity.Text) / 100;
                }

                if (polygonText.Text != null && polygonText.Text != "")
                {
                    bool first = true;
                    double minX = 0;
                    double maxX = 0;
                    double minY = 0;
                    double maxY = 0;
                    foreach (Point p in mw.objPolygon.Points)
                    {
                        if (first)
                        {
                            minX = p.X;
                            maxX = p.X;
                            minY = p.Y;
                            maxY = p.Y;
                            first = false;
                        }
                        else
                        {
                            if (p.X < minX)
                                minX = p.X;
                            if (p.X > maxX)
                                maxX = p.X;
                            if (p.Y < minY)
                                minY = p.Y;
                            if (p.Y > maxY)
                                maxY = p.Y;
                        }
                    }

                    double width = maxX - minX;
                    double height = maxY - minY;

                    mw.textPolygon.Text = polygonText.Text;
                    mw.textPolygon.Height = height / 2;
                    mw.textPolygon.Width = width / 2;
                    if (height < width)
                        mw.textPolygon.FontSize = height / 5;
                    else
                        mw.textPolygon.FontSize = width / 5;
                    mw.textPolygon.TextWrapping = TextWrapping.Wrap;
                }

                mw.FinishedPolygon();
                this.Close();
            }
        }
    }
}
