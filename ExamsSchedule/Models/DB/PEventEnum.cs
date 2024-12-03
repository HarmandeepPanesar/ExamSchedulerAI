using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PEventEnum
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public int EventType { get; set; }
        public int EventEnum { get; set; }
    }
}
