using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexGyver_s_Lamp_Control_Panel.Models
{
    class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class Lamp
    {
        public string IP { get; private set; }
        public string MAC { get; private set; }
        public List<Effect> Effects { get; private set; }
        public string Logs { get; private set; }
        public string LastOutput { get; private set; }

    }
}
