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
    /// Interaction logic for EllipseWindow.xaml
    /// </summary>
    public partial class EllipseWindow : Window
    {
        MainWindow mw;

        public EllipseWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    ellipseColors.Items.Add(prop.Name);
                    ellipseStrokeColors.Items.Add(prop.Name);
                    textColors.Items.Add(prop.Name);
                }
            }
        }

        private void ellipseColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ellipseColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.objEllipse.Fill = (System.Windows.Media.Brush)converter.ConvertFromString(ellipseColors.SelectedItem.ToString());
            }
        }

        private void ellipseStrokeColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ellipseStrokeColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.objEllipse.Stroke = (System.Windows.Media.Brush)converter.ConvertFromString(ellipseStrokeColors.SelectedItem.ToString());
            }
        }
        
        private void textColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textColors.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.textEllipse.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(textColors.SelectedItem.ToString());
            }
        }

        private void submitEllipse_Click(object sender, RoutedEventArgs e)
        {
            bool validated = true;

            if (!System.Text.RegularExpressions.Regex.IsMatch(ellipseX.Text, @"\d+(?:\.\d+)?"))
            {
                MessageBox.Show("Please enter a number for X.");
                ellipseX.Text = ellipseX.Text.Remove(0);
                validated = false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(ellipseY.Text, @"\d+(?:\.\d+)?"))
            {
                MessageBox.Show("Please enter a number for Y.");
                ellipseY.Text = ellipseY.Text.Remove(0);
                validated = false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(ellipseStrokeThickness.Text, @"\d+(?:\.\d+)?"))
            {
                MessageBox.Show("Please enter a number for EllipseStrokeThickness.");
                ellipseStrokeThickness.Text = ellipseStrokeThickness.Text.Remove(0);
                validated = false;
            }

            if (ellipseColors.SelectedItem == null)
            {
                MessageBox.Show("Please choose a color for ellipse fill.");
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

            if (ellipseStrokeColors.SelectedItem == null)
            {
                MessageBox.Show("Please choose a color for ellipse stroke.");
                validated = false;
            }

            if (validated)
            {
                mw.objEllipse.Width = Double.Parse(ellipseX.Text);
                mw.objEllipse.Height = Double.Parse(ellipseY.Text);
                mw.objEllipse.StrokeThickness = Double.Parse(ellipseStrokeThickness.Text);
                if (ellipseOpacity.Text != "" || ellipseOpacity.Text != null)
                {
                    mw.objEllipse.Opacity = Double.Parse(ellipseOpacity.Text)/100;
                }

                if (ellipseText.Text != null && ellipseText.Text != "")
                {
                    mw.textEllipse.Text = ellipseText.Text;
                    mw.textEllipse.Height = mw.objEllipse.Height/2;
                    mw.textEllipse.Width = mw.objEllipse.Width/2;
                    if (mw.objEllipse.Height < mw.objEllipse.Width)
                        mw.textEllipse.FontSize = mw.objEllipse.Height / 5;
                    else
                        mw.textEllipse.FontSize = mw.objEllipse.Width / 5;
                    mw.textEllipse.TextWrapping = TextWrapping.Wrap;
                }

                mw.FinishedEllipse();
                this.Close();
            }
        }

       
    }
}
