using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class BlackList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string UploadPicture { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
