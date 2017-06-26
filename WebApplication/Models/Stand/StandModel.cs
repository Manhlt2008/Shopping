using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Stand
{
    public class StandModel
    {
        public int Id { get; set; }
        public int StandId { get; set; }
        public string StandName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Status { get; set; }
        public string Banner { get; set; }
        public string Description { get; set; }
        public string PayMethod { get; set; }
    }
}