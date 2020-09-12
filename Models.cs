using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Xml;

namespace AlexGyver_s_Lamp_Control_Panel.Models
{
    [Serializable]
    public class Effect
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public Effect(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Id.ToString() + ' ' + Name;
        }
    }
    public interface FireLamp
    {
        string IP { get; }
        int Port { get; }
        IPEndPoint IPEndPoint { get; }
        string Name { get; set; }
        XmlNode InterfaceData { get; }
        List<Effect> Effects { get; }
        string Logs { get; }
        string LastOutput { get; }
        bool IsEnabled { get; set; }
        bool SendPacket(string _datagram, int attempts = 1);
        bool RefreshData(int attempts = 1);
        bool RefreshInitData(int attempts = 1);
        bool ChangeEffect(int effectId, int attempts = 1);
        bool ChangeEffectArgument(string argument, object value, int attempts = 1);
        bool Turn(int attempts = 1);
        bool TurnOn(int attempts = 1);
        bool TurnOff(int attempts = 1);
    }
    [Serializable]
    public class GyverLamp : FireLamp
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        private IPEndPoint iPEndPoint;
        public IPEndPoint IPEndPoint { get; private set; }
        public string Name { get; set; }
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        string logs;
        public string Logs { get { return logs; } }
        public string LastOutput { get; private set; }
        bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value) TurnOn();
                else TurnOff();
            }
        }
        public XmlNode InterfaceData
        {
            get
            {
                XmlDocument xmlDocument = new XmlDocument();
                return xmlDocument.SelectSingleNode("");
            }
        }
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
                        LastOutput = encodedDatagram;
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
        public bool TurnOn(int attempts = 1)
        {
            isEnabled = true;
            return SendPacket("P_ON", attempts);
        }
        public bool TurnOff(int attempts = 1)
        {
            isEnabled = false;
            return SendPacket("P_OFF", attempts);
        }
        public bool Turn(int attempts = 1)
        {
            if (IsEnabled)
                return TurnOff(attempts);
            else return TurnOn(attempts);
        }
        public bool ChangeEffect(int id, int attempts = 1)
        {
            return SendPacket("EFF" + id.ToString(), attempts);
        }
        public bool ChangeEffectArgument(string argument, object value, int attempts = 1)
        {
            return SendPacket(argument + value.ToString(), attempts);
        }
        public override string ToString()
        {
            return IP + ':' + Port.ToString() + " " + Name;
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
        public List<Effect> Effects { get; private set; }
        [NonSerialized]
        string logs;
        public string Logs { get { return logs; } }
        public string LastOutput { get; private set; }
        bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value) TurnOn();
                else TurnOff();
                isEnabled = value;
            }
        }
        public XmlNode InterfaceData
        {
            get
            {
                if (SendPacket("echo"))
                    return JsonConvert.DeserializeXmlNode("{\"root\":" + LastOutput + "}");
                else return null;
            }
        }
        public KDnLamp(string ip, int port, string name = "")
        {
            IP = ip;
            Port = port;
            Name = name;
            iPEndPoint = new IPEndPoint(long.Parse(IP.Replace(".", "")), Port);
            Effects = new List<Effect>();
        }

        public bool RefreshData(int attempts = 1)
        {
            if (!SendPacket("heap", attempts))
                return false;
            XmlNode interfaceXml = InterfaceData;
            var interfaceData = interfaceXml.ChildNodes[0].ChildNodes;
            var effectsData = interfaceData[6].ChildNodes[1].ChildNodes;
            Effects.Clear();
            foreach (XmlNode effectNode in effectsData)
            {
                if (effectNode.Name == "options")
                {
                    Effects.Add(new Effect(int.Parse(effectNode.ChildNodes[1].InnerText), effectNode.ChildNodes[0].InnerText));
                }
            }
            return true;
        }
        public bool SendPacket(string _datagram, int attempts = 1)
        {
            Uri lampUri = new UriBuilder(IP + ':' + Port.ToString()).Uri;
            //_datagram = "GET " + lampUri + _datagram;
            logs += "-->" + _datagram + Environment.NewLine;
            HttpClient http = new HttpClient();
            http.BaseAddress = lampUri;
            http.Timeout = new TimeSpan(0, 0, 3);
            //HttpRequestMessage httpRequest = new HttpRequestMessage(new HttpMethod(HttpMethod.Get.Method+' '+_datagram),lampUri);
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, lampUri + _datagram);
            var requestTask = http.SendAsync(httpRequest);
            if (requestTask.Wait(3000))
            {
                HttpResponseMessage response = requestTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = "";
                    var readRespToStringTask = response.Content.ReadAsStringAsync();
                    if (readRespToStringTask.Wait(3000))
                    {
                        responseContent = readRespToStringTask.Result;
                        LastOutput = responseContent;
                        logs += "<--" + responseContent + Environment.NewLine;
                        return true;
                    }
                }
            }
            return false;

        }
        public bool TurnOn(int attempts = 1)
        {
            isEnabled = true;
            return SendPacket("cmd?on", attempts);
        }
        public bool TurnOff(int attempts = 1)
        {
            isEnabled = false;
            return SendPacket("cmd?off", attempts);
        }
        public bool Turn(int attempts = 1)
        {
            if (IsEnabled)
                return TurnOff(attempts);
            else return TurnOn(attempts);
        }
        public bool ChangeEffect(int id, int attempts = 1)
        {
            return SendPacket("cmd?effect=" + id.ToString(), attempts);
        }
        public bool ChangeEffectArgument(string argument, object value, int attempts = 1)
        {
            return SendPacket("cmd?" + argument + '=' + value.ToString(), attempts);
        }

        public override string ToString()
        {
            return IP + ':' + Port.ToString() + " " + Name;
        }
    }
}
