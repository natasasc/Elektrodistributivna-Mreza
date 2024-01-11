using Microsoft.Win32;
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
    /// Interaction logic for EntityColors.xaml
    /// </summary>
    public partial class EntityColors : Window
    {
        MainWindow mw;

        public EntityColors(MainWindow mw)
        {
            this.mw = mw;

            InitializeComponent();

            foreach (PropertyInfo prop in typeof(System.Drawing.Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    cbSubColor.Items.Add(prop.Name);
                    cbNodeColor.Items.Add(prop.Name);
                    cbSwitchColor.Items.Add(prop.Name);
                }
            }

            mw.colors.SubColor = null;
            mw.colors.NodeColor = null;
            mw.colors.SwitchColor = null;
            mw.colors.SubImg = null;
            mw.colors.NodeImg = null;
            mw.colors.SwitchImg = null;
        }


        private void cbSubColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSubColor.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.colors.SubColor = (SolidColorBrush)converter.ConvertFromString(cbSubColor.SelectedItem.ToString());
            }
        }

        private void subBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                subImg.Source = new BitmapImage(new Uri(op.FileName));
                mw.colors.SubImg = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void cbNodeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbNodeColor.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.colors.NodeColor = (SolidColorBrush)converter.ConvertFromString(cbNodeColor.SelectedItem.ToString());
            }
        }

        private void nodeBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                nodeImg.Source = new BitmapImage(new Uri(op.FileName));
                mw.colors.NodeImg = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void cbSwitchColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSwitchColor.SelectedItem != null)
            {
                System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
                mw.colors.SwitchColor = (SolidColorBrush)converter.ConvertFromString(cbSwitchColor.SelectedItem.ToString());
            }
        }

        private void switchBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                switchImg.Source = new BitmapImage(new Uri(op.FileName));
                mw.colors.SwitchImg = new BitmapImage(new Uri(op.FileName));
            }
        }


        private void submitColors_Click(object sender, RoutedEventArgs e)
        {
            mw.ChangeColors();
            this.Close();
        }

        private void initialColors_Click(object sender, RoutedEventArgs e)
        {
            mw.InitialEntityColors();
            this.Close();
        }
    }
}
