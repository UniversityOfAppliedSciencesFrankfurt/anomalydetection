using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetectionRestApi.Controllers
{
    public class TrainingContent
    {
        //string[] csvFilePath, string savePath, string loadimpPath, int numClusters, int numOfAttributes, int kmeansMaxIterations

        public string[] CsvFilePaths { get; set; }
        public string SavePath { get; set; }
        public int NumOfClusters { get; set; }
        public int NumOfAttributes { get; set; }
        public int KmeansMaxIterations { get; set; }

    }
}
