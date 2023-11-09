using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cosmic_management_system.Models {
    internal class Response {
        public int status { get; set; }
        public string message { get; set; }
        public object body { get; set; }
        public List<Production> data { get; set; }
    }
}
