using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRecruitment.Sita.BackEnd.Models
{
    public class VacancyReAdvertise
    {

        public Guid ReAdvertiseID { get; set; }
        public string BPSNumber { get; set; }
        public int VacancyID { get; set; }
    }
}