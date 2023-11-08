using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cosmic_management_system.Models
{
    internal class StageResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public Stage stage { get; set; }
        public List<Stage> stages { get; set; }
    }
}
