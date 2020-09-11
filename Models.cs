using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net.Http;
using System.Net;

namespace AlexGyver_s_Lamp_Control_Panel.Models
{
    public class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public interface FireLamp
    {
        string IP { get; }
        int Port { get; }
        IPEndPoint IPEndPoint { get; }
        string Name { get; set; }
        string MAC { get; }
        List<Effect> Effects { get; }
        string Logs { get; }
        string LastOutput { get; }
        bool SendPacket(string _datagram, int attempts = 1);
        bool RefreshData(int attempts = 1);
    }
    [Serializable]
    public class GyverLamp : FireLamp
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        private IPEndPoint iPEndPoint;
        public IPEndPoint IPEndPoint { get; private set; }
        public string Name { get; set; }
        public string MAC { get; private set; }
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        string logs;
        public string Logs { get { return logs; } }
        [NonSerialized]
        string lastOutput;
        public string LastOutput { get { return lastOutput; } }
        public GyverLamp(string ip, int port, string name = "")
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
    [Serializable]
    public class KDnLamp : FireLamp
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        private IPEndPoint iPEndPoint;
        public IPEndPoint IPEndPoint { get; private set; }
        public string Name { get; set; }
        public string MAC { get; private set; }
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        string logs;
        public string Logs { get { return logs; } }
        [NonSerialized]
        string lastOutput;
        public string LastOutput { get { return lastOutput; } }
        public KDnLamp(string ip, int port, string name = "")
        {
            IP = ip;
            Port = port;
            Name = name;
            iPEndPoint = new IPEndPoint(long.Parse(IP.Replace(".", "")), Port);
        }
        
        public bool RefreshData(int attempts = 1)
        {
            return SendPacket("heap", attempts);
        }
        public bool SendPacket(string _datagram, int attempts = 1)
        {
            Uri lampUri = new UriBuilder(IP+':'+Port.ToString()).Uri;
            //_datagram = "GET " + lampUri + _datagram;
            logs += "-->" + _datagram + Environment.NewLine; 
            HttpClient http = new HttpClient();
            http.BaseAddress = lampUri;
            http.Timeout = new TimeSpan(0,0,3);
            //HttpRequestMessage httpRequest = new HttpRequestMessage(new HttpMethod(HttpMethod.Get.Method+' '+_datagram),lampUri);
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, lampUri+_datagram);
            var requestTask = http.SendAsync(httpRequest);
            if (requestTask.Wait(3000))
            {
                HttpResponseMessage response = requestTask.Result;
                if (response.IsSuccessStatusCode) {
                    string responseContent = "";
                    var readRespToStringTask = response.Content.ReadAsStringAsync();
                    if (readRespToStringTask.Wait(3000)) {
                        responseContent = readRespToStringTask.Result;
                        lastOutput = responseContent;
                        logs += "<--" + responseContent + Environment.NewLine;
                        return true;
                    }
                }
            }
            return false;

        }

        public override string ToString()
        {
            return iPEndPoint.ToString() + " " + Name;
        }
    }
}
