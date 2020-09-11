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

        FireLamp CurrentLamp { get; set; }
        //Lamp CurrentLamp { get { return currentLamp; } set
        //    {
        //        currentLamp = value;
        //    }

        //}
        bool connected = false;
        public MainWindow()
        {
            InitializeComponent();
            Binding binding = new Binding();
            binding.Source = Controller.MainController.GetInstance();
            binding.Path = new PropertyPath("savedLamps");
            savedLamps.SetBinding(ComboBox.ItemsSourceProperty, binding);
            Controller.MainController.GetInstance().LoadFromFile();
            refreshData();
            //RefreshInterfaceData();
            
            //currentLamp = new Lamp("192.168.0.73", 8888);
            //refreshData();
            //ConsoleOut.Text = currentLamp.Logs;
        }

        //public void RefreshInterfaceData()
        //{
        //    savedLamps.Items.Clear();
        //    savedLamps.Items.Add("Select Lamp");
        //    savedLamps.Items.Add("Add Lamp");
        //    foreach (Lamp lamp in Controller.MainController.GetInstance().savedLamps)
        //        savedLamps.Items.Add(lamp);
        //    savedLamps.SelectedIndex = 0;
        //}
        public void refreshData()
        {
            if (CurrentLamp == null)
            {
                ipAndPort.Content = "Select lamp";
                connectionMarker.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                connected = false;
                saveLampBtn.Visibility = Visibility.Hidden;
                return;
            }
            ipAndPort.Content = CurrentLamp.IP + ':' + CurrentLamp.Port.ToString();
            if (CurrentLamp.RefreshData())
            {
                connectionMarker.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                ConsoleOut.Text = CurrentLamp.Logs;
                connected = true;
                if (Controller.MainController.GetInstance().savedLamps.Find(l => l.IP == CurrentLamp.IP) != null)
                {
                    saveLampBtn.Visibility = Visibility.Hidden;
                }
                else
                    saveLampBtn.Visibility = Visibility.Visible;
            }
            else
            {
                connectionMarker.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                ConsoleOut.Text = CurrentLamp.Logs;
                connected = false;
            }
        }
        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            consoleColumn.Width = new GridLength(200, GridUnitType.Pixel);
            Application.Current.MainWindow.Width += 200;
            MinWidth += 200;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            consoleColumn.Width = new GridLength(0, GridUnitType.Star);
            MinWidth -= 200;
            Application.Current.MainWindow.Width -= 200;
        }

        private void savedLamps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = savedLamps.SelectedItem;
            if (selected == null)
                return;
            if (  selected.GetType().Name == "".GetType().Name )
            {
                if(selected.ToString() == "Add Lamp")
                {
                    AddLampWindow dialog = new AddLampWindow();
                    dialog.ShowDialog();
                    Controller.MainController.GetInstance().SaveLamp(dialog.ReturnValue);
                    //RefreshInterfaceData();
                    refreshData();

                }
                CurrentLamp = null;
                refreshData();
                return;
            }
            if (selected as FireLamp != null)
            {
                CurrentLamp = savedLamps.SelectedItem as FireLamp;
                refreshData();
            }
        }

        private void ConsoleEnter_Click(object sender, RoutedEventArgs e)
        {
            if (ConsoleIn.Text != "")
            {
                CurrentLamp.SendPacket(ConsoleIn.Text);
                ConsoleIn.Text = "";
                ConsoleOut.Text = CurrentLamp.Logs;
            }
        }

        private void saveLampBtn_Click(object sender, RoutedEventArgs e)
        {
            AddLampWindow dialog;
            if (CurrentLamp != null)
            {
                if (Controller.MainController.GetInstance().savedLamps.Find(l => l.IP == CurrentLamp.IP) != null)
                    return;
                dialog = new AddLampWindow(CurrentLamp.IP, CurrentLamp.Port, CurrentLamp.Name);
            }
            else
                dialog = new AddLampWindow();
            dialog.ShowDialog();
            Controller.MainController.GetInstance().SaveLamp(dialog.ReturnValue);
            //RefreshInterfaceData();
            refreshData();
        }

        private void ipAndPort_MouseUp(object sender, MouseButtonEventArgs e)
        {

            var dialog = new AddLampWindow();
            dialog.ShowDialog();
            if(dialog.ReturnValue!=null)
            CurrentLamp = dialog.ReturnValue;
            refreshData();
            //RefreshInterfaceData();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Controller.MainController.GetInstance().SaveToFile();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Controller.MainController.GetInstance().LoadFromFile();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Controller.MainController.GetInstance().SaveToFile();
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            var x = CurrentLamp.InterfaceData;
        }
    }
}
