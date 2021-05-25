using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTC_Communication_Utility
{
    public class GraphPoints
    {
        public string SeriesName { get; set; }
        public string Color { get; set; }
        public string NodeAddress { get; set; }
        public Dictionary<double, double> Points { get; set; }
        public string PanelId { get; set; }
        public string SamplingTime { get; set; }

        public GraphPoints()
        {
            Points = new Dictionary<double, double>();
        }
    }
}
