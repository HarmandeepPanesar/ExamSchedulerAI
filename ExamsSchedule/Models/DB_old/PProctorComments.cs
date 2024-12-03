using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PProctorComments
    {
        public int Id { get; set; }
        public int ProctorStatusId { get; set; }
        public string Comment { get; set; }
        public DateTime Datetime { get; set; }
        public int UserId { get; set; }
    }
}
