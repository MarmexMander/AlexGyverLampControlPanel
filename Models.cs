using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace AlexGyver_s_Lamp_Control_Panel.Models
{
    class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [Serializable]
    class Lamp
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        private IPEndPoint iPEndPoint;
        public string Name { get; set; }
        public string MAC { get; private set; }
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        public string logs;
        public string Logs { get { return logs; } }
        [NonSerialized]
        public string lastOutput;
        public string LastOutput { get { return logs; } }
        Lamp(string ip, int port)
        {
            IP = ip;
            Port = port;
            iPEndPoint = new IPEndPoint(long.Parse(IP.Replace(".","")), Port);
        }
        bool RefreshData(int attempts = 1)
        {
            UdpClient udp = new UdpClient(IP,Port);
            byte[] datagram = Encoding.ASCII.GetBytes("GET");
            for (int i = 0; i < attempts; i++)
            {
                udp.Send(datagram, datagram.Length);
                byte[] recivedDatagram = udp.Receive(ref iPEndPoint);
                try
                {
                    string encodedDatagram = Encoding.ASCII.GetString(recivedDatagram, 0, recivedDatagram.Length);
                }
                catch
                {
                    continue;
                }
                //TODO: Data parsing
                return true;
            }
            return false;
        }
    }
}
