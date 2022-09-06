using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace DnSrtChecker.Data
{
    [XmlRoot("RestituzioneStatoCassa")]
    public class RtDeviceStatus
    {
        public RtDeviceStatus()
        {
            Devices = new List<RtDevice>();
        }

        [XmlElement("StatoCassa")]
        public List<RtDevice> Devices { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
