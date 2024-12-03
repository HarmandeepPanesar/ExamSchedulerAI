using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class Cm
    {
        public string data { get; set; }
        public int id { get; set; }
        public int course { get; set; }
        public int module { get; set; }
        public string name { get; set; }
        public string modname { get; set; }
        public int instance { get; set; }
        public int section { get; set; }
        public int sectionnum { get; set; }
        public int groupmode { get; set; }
        public int groupingid { get; set; }
        public int completion { get; set; }
        public string idnumber { get; set; }
        public int added { get; set; }
        public int score { get; set; }
        public int indent { get; set; }
        public int visible { get; set; }
        public int visibleoncoursepage { get; set; }
        public int visibleold { get; set; }
        public object completiongradeitemnumber { get; set; }
        public int completionview { get; set; }
        public int completionexpected { get; set; }
        public int showdescription { get; set; }
        public object availability { get; set; }
        public int grade { get; set; }
        public string gradepass { get; set; }
        public int gradecat { get; set; }
    }

    public class ExamUploadModel
    {
        public Cm cm { get; set; }
        public List<object> warnings { get; set; }
    }
}

