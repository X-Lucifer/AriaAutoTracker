using System.Runtime.Serialization;

namespace Aria.AutoTracker
{
    [DataContract]
    public class TrackerItem
    {
        [DataMember]
        public string Url { get; set; }
        
        [DataMember]
        public int Sort { get; set; }
    }
}