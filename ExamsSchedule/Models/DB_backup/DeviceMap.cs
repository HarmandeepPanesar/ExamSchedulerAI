using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class DeviceMap
    {
        public int DeviceClientId { get; set; }
        public string DeviceId { get; set; }
        public string AssociateCompany { get; set; }
        public string AssociatedUser { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public string Location { get; set; }
    }
}
