using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlexGyver_s_Lamp_Control_Panel.Models;
using System.Runtime.Serialization;

namespace AlexGyver_s_Lamp_Control_Panel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Lamp> lamps = new List<Lamp>();
        Lamp currentLamp;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            consoleColumn.Width = new GridLength(200,GridUnitType.Pixel);
            Application.Current.MainWindow.Width += 200;
            MinWidth += 200;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            consoleColumn.Width = new GridLength(0, GridUnitType.Star);
            Application.Current.MainWindow.Width -= 200;
            MinWidth -= 200;
        }
    }
}
