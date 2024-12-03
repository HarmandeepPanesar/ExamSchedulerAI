using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class Lmsintegration
    {
        public int Id { get; set; }
        public string WebServiceLink { get; set; }
        public string SecurityToken { get; set; }
    }
}
