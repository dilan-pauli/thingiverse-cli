using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thingiverseCLI.Models
{
    public class ThingFile
    {
        public string name { get; set; }
        public int download_count { get; set; }
        public string download_url { get; set; }
    }
    
    public class ThingInfo
    {
        public string name {  get; set; }
    }
}
