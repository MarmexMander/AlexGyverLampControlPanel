using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using AlexGyver_s_Lamp_Control_Panel.Models;
using System.IO;
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
        public List<FireLamp> savedLamps { get; private set;} = new List<FireLamp>();
        public void SaveLamp(FireLamp lamp)
        {
            savedLamps.Add(lamp);
        }
        public bool SaveToFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            try
            {
                stream = new FileStream("savedLamps.dat", FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, savedLamps);
            }
            catch
            {
                return false;
            }
            finally
            {
                stream.Close();
            }
            return true;
        }
         public bool LoadFromFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            List<FireLamp> lamps;
            try
            {
                stream = new FileStream("savedLamps.dat", FileMode.Open, FileAccess.Read);
                lamps = formatter.Deserialize(stream) as List<FireLamp>;
            }
            catch
            {
                return false;
            }
            finally
            {
                stream.Close();
            }
            savedLamps.Clear();
            foreach (FireLamp lamp in lamps)
            {
                savedLamps.Add(lamp);
            }
            return true;
        }
    }
}
