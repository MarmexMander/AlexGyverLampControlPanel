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
using System.Windows.Shapes;

namespace AlexGyver_s_Lamp_Control_Panel
{
    /// <summary>
    /// Логика взаимодействия для AddLampWindow.xaml
    /// </summary>
    public partial class AddLampWindow : Window
    {
        Models.FireLamp lamp;
        public Models.FireLamp ReturnValue { get { return lamp; } }
        public AddLampWindow()
        {
            InitializeComponent();
        }
        public AddLampWindow(string ip, int port, bool isKDnFirmware, string name = "")
        {
            InitializeComponent();
            ipTB.Text = ip;
            portTB.Text = port.ToString();
            checkBox.IsChecked = isKDnFirmware;
            nameTB.Text = name;
        }

        private void OkBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)checkBox.IsChecked)
                    lamp = new Models.KDnLamp(ipTB.Text, int.Parse(portTB.Text), nameTB.Text);
                else
                    lamp = new Models.GyverLamp(ipTB.Text, int.Parse(portTB.Text), nameTB.Text);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Incorrect IP or port");
            }
        }

        private void cancleBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
