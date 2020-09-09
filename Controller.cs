using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexGyver_s_Lamp_Control_Panel.Models;

namespace AlexGyver_s_Lamp_Control_Panel.Controller
{
    public class MainController
    {
        private MainController() { }
        private static MainController _instance;
        private static readonly object _lock = new object();
        public static MainController GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MainController();
                    }
                }
            }
            return _instance;
        }
        public string Value { get; set; }
        public List<Lamp> savedLamps { get; set; } = new List<Lamp>();
        public void SaveLamp(Lamp lamp)
        {
            savedLamps.Add(lamp);
        }
    }
}
