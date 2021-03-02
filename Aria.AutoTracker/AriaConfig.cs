using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aria.AutoTracker
{
    [DataContract]
    public class AriaConfig
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public List<TrackerItem> TrackerList { get; set; }
    }
}