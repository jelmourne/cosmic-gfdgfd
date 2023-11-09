using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cosmic_management_system.Models {
    public class Response<T> {
        public int status { get; set; }
        public string message { get; set; }
        public object body { get; set; }
        public List<T> data { get; set; }
    }
}
