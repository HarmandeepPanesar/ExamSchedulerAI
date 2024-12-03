using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class StudentJson
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string department { get; set; }
        public int firstaccess { get; set; }
        public int lastaccess { get; set; }
        public string auth { get; set; }
        public bool suspended { get; set; }
        public bool confirmed { get; set; }
        public string lang { get; set; }
        public string theme { get; set; }
        public string timezone { get; set; }
        public int mailformat { get; set; }
        public string description { get; set; }
        public int descriptionformat { get; set; }
        public string profileimageurlsmall { get; set; }
        public string profileimageurl { get; set; }
    }
}
