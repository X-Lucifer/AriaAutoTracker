using System.Runtime.Serialization;

namespace Aria.AutoTracker
{
    /// <summary>
    /// Tracker子项
    /// </summary>
    [DataContract]
    public class TrackerItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// URL
        /// </summary>
        [DataMember]
        public string Url { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Sort { get; set; }
    }
}