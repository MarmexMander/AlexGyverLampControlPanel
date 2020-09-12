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
using System.Xml;

namespace AlexGyver_s_Lamp_Control_Panel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        FireLamp CurrentLamp { get; set; }
        bool connected = false;
        public MainWindow()
        {
            InitializeComponent();
            Controller.MainController.GetInstance().LoadFromFile();
            refreshData();
        }

        //public void RefreshInterfaceData()
        //{

        //}
        public void refreshData()
        {
            Binding bindingSavedLamps = new Binding();
            bindingSavedLamps.Source = Controller.MainController.GetInstance();
            bindingSavedLamps.Path = new PropertyPath("SavedLamps");
            bindingSavedLamps.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            savedLamps.SetBinding(ComboBox.ItemsSourceProperty, bindingSavedLamps);
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
                effectNumberTB.Text = CurrentLamp.CurrentEffect.Id.ToString();
                EffectPicker.SelectedItem = CurrentLamp.Effects.Find(f => f.Id == CurrentLamp.CurrentEffect.Id);
                ConsoleOut.Text = CurrentLamp.Logs;
                connected = true;
                Binding bindingEffects = new Binding();
                bindingEffects.Source = CurrentLamp;
                bindingEffects.Path = new PropertyPath("Effects");
                bindingEffects.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                EffectPicker.SetBinding(ComboBox.ItemsSourceProperty, bindingEffects);
                if (Controller.MainController.GetInstance().SavedLamps.Count > 0)
                    if (Controller.MainController.GetInstance().SavedLamps.Find(l => l.IP == CurrentLamp.IP) != null)
                    {
                        saveLampBtn.Visibility = Visibility.Hidden;
                    }
                    else
                        saveLampBtn.Visibility = Visibility.Visible;
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
            if (selected.GetType().Name == "".GetType().Name)
            {
                if (selected.ToString() == "Add Lamp")
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
                CurrentLamp.RefreshInitData();
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
                if (Controller.MainController.GetInstance().SavedLamps.Find(l => l.IP == CurrentLamp.IP) != null)
                    return;
                dialog = new AddLampWindow(CurrentLamp.IP, CurrentLamp.Port,(CurrentLamp.GetType().FullName == "AlexGyver_s_Lamp_Control_Panel.Models.KDnLamp"), CurrentLamp.Name);
            }
            else
                dialog = new AddLampWindow();
            dialog.ShowDialog();
            Controller.MainController.GetInstance().SaveLamp(dialog.ReturnValue);
            refreshData();
        }

        private void ipAndPort_MouseUp(object sender, MouseButtonEventArgs e)
        {

            var dialog = new AddLampWindow();
            dialog.ShowDialog();
            if (dialog.ReturnValue != null)
                CurrentLamp = dialog.ReturnValue;
            refreshData();
            CurrentLamp.RefreshInitData();
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
            refreshData();
        }

        private void EffectPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentLamp != null)
            {
                CurrentLamp.ChangeEffect((EffectPicker.SelectedItem as Effect).Id);
                effectNumberTB.Text = (EffectPicker.SelectedItem as Effect).Id.ToString();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentLamp!=null)
            CurrentLamp.Turn();
        }

        private void EffectPlusBTN_Click(object sender, RoutedEventArgs e)
        {
            int effectId = int.Parse(effectNumberTB.Text)+1;
            EffectPicker.SelectedItem = CurrentLamp.Effects.Find(f => f.Id == effectId);
        }

        private void EffectMinusBTN_Click(object sender, RoutedEventArgs e)
        {
            int effectId = int.Parse(effectNumberTB.Text) - 1;
            EffectPicker.SelectedItem = CurrentLamp.Effects.Find(f => f.Id == effectId);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new AddLampWindow();
            dialog.ShowDialog();
            if (dialog.ReturnValue != null)
                CurrentLamp = dialog.ReturnValue;
            refreshData();
            CurrentLamp.RefreshInitData();
        }
    }
}
