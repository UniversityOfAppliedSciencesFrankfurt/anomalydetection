using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Test
{

    public class UnitTest
    {
        [Fact]
        public void Test_ImportNewDataForClustering2()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes,KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);
            AnomalyDetectionResponse response;
            for (int i = 0; i < 5; i++)
            {
                 response = kmeanApi.Training(rawData, clusterCentars);
                Assert.True(response.Code == 0);
            }

            //Save Cluster and Instance in json 
            response = kmeanApi.Save("interface.json");

            Assert.True(response.Code == 0);
        }
    }
}
