using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class DeviceLogs
    {
        public int DeviceLogId { get; set; }
        public string DeviceId { get; set; }
        public string PayLoad { get; set; }
        public DateTime RecordedDtm { get; set; }
    }
}
