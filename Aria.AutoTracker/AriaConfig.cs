using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aria.AutoTracker
{
    /// <summary>
    /// 配置文件
    /// </summary>
    [DataContract]
    public class AriaConfig
    {
        /// <summary>
        /// Aria2配置文件路径
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Tracker源列表
        /// </summary>
        [DataMember]
        public List<TrackerItem> TrackerList { get; set; }
    }
}