using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cosmic_management_system.Models {
    public class Vendor {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int location { get; set; }

        public static explicit operator Vendor(List<Vendor> v) {
            throw new NotImplementedException();
        }
    }
}
