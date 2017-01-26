using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Test
{

    public class UnitTest
    {
        [Fact]
        public void Test_Training()
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

            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings); //AnomalyDetectionAPI(clusterSettings), Constractor should not be null when run Training() 
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

        [Fact]
        public void Test_CheckSample()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            double[][] initialCentroids = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings Settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(Settings);

            AnomalyDetectionResponse response = kmeanApi.Training(rawData, clusterCentars);
            Assert.True(response.Code == 0);

            //response = kmeanApi.Save("CheckSample.json");
            //Assert.True(response.Code == 0);

            int detectedCluster;
            double[] Sample = new double[] { 26, 28 };
            CheckingSampleSettings SampleSettings = new CheckingSampleSettings(null, Sample, 3); //If path is null you should run Training()
            response = kmeanApi.CheckSample(SampleSettings, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 2);

            AnomalyDetectionAPI kApi = new AnomalyDetectionAPI();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Instance Result\\CheckSample.json";

            double[] Sample2 = new double[] { 150, 16 };
            CheckingSampleSettings SampleSettings2 = new CheckingSampleSettings(filePath, Sample2, 3);
            response = kApi.CheckSample(SampleSettings2, out detectedCluster);
            Assert.True(response.Code == 1);
            Assert.True(detectedCluster == -1);// Out of all clusters.

            double[] Sample3 = new double[] { 16, 14 };
            CheckingSampleSettings SampleSettings3 = new CheckingSampleSettings(filePath, Sample3, 3);
            response = kApi.CheckSample(SampleSettings3, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 1);

            double[] Sample4 = new double[] { 6, 4 };
            CheckingSampleSettings SampleSettings4 = new CheckingSampleSettings(filePath, Sample4, 3);
            response = kApi.CheckSample(SampleSettings4, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 0);
        }

        [Fact]
        public void Test_GetClusters()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            double[][] initialCentroids = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings Settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(Settings);

            AnomalyDetectionResponse response = kmeanApi.Training(rawData, clusterCentars);
            Assert.True(response.Code == 0);


            Cluster[] clusters;
            response = kmeanApi.GetClusters(null, out clusters); // Path can be null if GetClusters() runs after Training()
            Assert.True(response.Code == 0);
            Assert.True(clusters.Length != 0);
            Assert.True(clusters != null);


            AnomalyDetectionAPI kApi = new AnomalyDetectionAPI();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Cluster Result\\CheckSample.json";
            Cluster[] clusters1;
            response = kmeanApi.GetClusters(null, out clusters1);
            Assert.True(response.Code == 0);
            Assert.True(clusters.Length != 0);
            Assert.True(clusters != null);
        }

        [Fact]
        public void Test_GetPreviousSamples()
        {
            AnomalyDetectionAPI kApi = new AnomalyDetectionAPI();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Instance Result\\CheckSample.json";
            double[][] oldData;

            var response = kApi.GetPreviousSamples(filePath, out oldData);

            Assert.True(response.Code == 0);
            Assert.True(oldData != null);
            Assert.True(oldData.Length != 0);
        }
    }
}
