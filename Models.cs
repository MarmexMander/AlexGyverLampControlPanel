using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace AlexGyver_s_Lamp_Control_Panel.Models
{
    public class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [Serializable]
    public class Lamp
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        private IPEndPoint iPEndPoint;
        public string Name { get; set; }
        public string MAC { get; private set; }
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        string logs;
        public string Logs { get { return logs; } }
        [NonSerialized]
        string lastOutput;
        public string LastOutput { get { return logs; } }
        public Lamp(string ip, int port, string name="")
        {
            IP = ip;
            Port = port;
            Name = name;
            iPEndPoint = new IPEndPoint(long.Parse(IP.Replace(".", "")), Port);
        }
        public bool RefreshData(int attempts = 1)
        {
            return SendPacket("GET", attempts);
        }
        public bool SendPacket(string _datagram, int attempts = 1)
        {
            UdpClient udp = new UdpClient(IP, Port);
            byte[] datagram = Encoding.ASCII.GetBytes(_datagram);
            logs += "-->" + _datagram + Environment.NewLine;
            for (int attempt = 0; attempt < attempts; attempt++)
            {
                byte[] recivedDatagram = new byte[1];
                string encodedDatagram;
                udp.Send(datagram, datagram.Length);
                bool recCompleted = false;
                udp.BeginReceive((IAsyncResult res) =>
                {
                    try
                    {
                        recivedDatagram = udp.EndReceive(res, ref iPEndPoint);
                    }
                    catch { }
                    recCompleted = true;
                }, null);

                for (int time = 0; time < 20; time++)
                    if (recCompleted)
                    {
                        encodedDatagram = Encoding.ASCII.GetString(recivedDatagram, 0, recivedDatagram.Length);
                        lastOutput = encodedDatagram;
                        logs += "<--" + encodedDatagram + Environment.NewLine;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                if (recCompleted)
                {
                    udp.Close();
                    return true;
                }
                else
                {
                    try
                    {
                        udp.EndReceive(udp.ReceiveAsync(), ref iPEndPoint);
                    }
                    catch { }
                    udp.Close();
                }
            }

            udp.Close();
            return false;
        }

        public override string ToString()
        {
            return iPEndPoint.ToString() + " " + Name;
        }
    }
}
