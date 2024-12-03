using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class DashboardItems
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public string Color { get; set; }
        public string Role { get; set; }
    }
}
