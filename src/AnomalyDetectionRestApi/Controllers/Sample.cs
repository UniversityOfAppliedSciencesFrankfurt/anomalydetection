using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetectionRestApi.Controllers
{
    //double xAxis, double? yAxis, double? zAxis, double tolerance, [FromBody]Dictionary<string, string> filePath
    public class Sample
    {
        public string FilePath { get; set; }
        public double XAxis { get; set; }
        public double? YAxis { get; set; }
        public double? ZAxis { get; set; }
        public double Tolerance { get; set; }
    }
}
