using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class SupportParameter
    {
        public int ID { get; set; }
        public string ParameterName { get; set; }
        public int ParameterValue { get; set; }
    }
}