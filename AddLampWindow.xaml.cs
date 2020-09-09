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
        Models.Lamp lamp;
        public Models.Lamp ReturnValue { get { return lamp; } }
        public AddLampWindow()
        {
            InitializeComponent();
        }
        public AddLampWindow(string ip, int port, string name = "")
        {
            InitializeComponent();
            ipTB.Text = ip;
            portTB.Text = port.ToString();
            nameTB.Text = name;
        }

        private void OkBTN_Click(object sender, RoutedEventArgs e)
        {
            lamp = new Models.Lamp(ipTB.Text, int.Parse(portTB.Text), nameTB.Text);
            this.Close();
        }

        private void cancleBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
