using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AnomalyDetectionApi;
using AnomalyDetection.Interfaces;

namespace UnitTest
{
    public class UnitTest
    {
        [Fact]
        public void TestClusterCalcuation()
        {
            //
            // In test we know where are positions of centroids.
            // We will now create data around known centroids and let alorithm
            // find centroids.
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            SaveLoadSettings persistenceProviderSettings;

            var resp = SaveLoadSettings.JSON_Settings("model.json", out persistenceProviderSettings, false);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters);
            ClusteringSettings clusterSettings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, persistenceProviderSettings, KmeansAlgorithm: 1, Replace: true);
            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(clusterSettings);
            Assert.True(response.Code == 0);

            int detectedCluster;
            double[] Sample = new double[] { 26, 28 };
            CheckingSampleSettings SampleSettings = new CheckingSampleSettings(null, Sample, 3);
            response = kmeanApi.CheckSample(SampleSettings, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 0);

            double[] Sample2 = new double[] { 150, 16 };
            CheckingSampleSettings SampleSettings2 = new CheckingSampleSettings(null, Sample2, 3);
            response = kmeanApi.CheckSample(SampleSettings2, out detectedCluster);
            Assert.True(response.Code == 1);
            Assert.True(detectedCluster == -1);// Out of all clusters.

            double[] Sample3 = new double[] { 16, 14 };
            CheckingSampleSettings SampleSettings3 = new CheckingSampleSettings(null, Sample3, 3);
            response = kmeanApi.CheckSample(SampleSettings3, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 1);

            double[] Sample4 = new double[] { 6, 4 };
            CheckingSampleSettings SampleSettings4 = new CheckingSampleSettings(null, Sample4, 3);
            response = kmeanApi.CheckSample(SampleSettings4, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 2);
        }


        [Fact]
        public void TestFixedCentroids()
        {
            //
            // In thes we know where are positions of centroids.
            // We will now create data around known centroids and let alorithm
            // find centroids.
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

            SaveLoadSettings sett;

            var resp = SaveLoadSettings.JSON_Settings("model.json", out sett, false);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters, initialCentroids);
            ClusteringSettings Settings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, sett, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);
            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(Settings);

            int detectedCluster;
            double[] Sample = new double[] { 26, 28 };
            CheckingSampleSettings SampleSettings = new CheckingSampleSettings(null, Sample, 3);
            response = kmeanApi.CheckSample(SampleSettings, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 2);

            double[] Sample2 = new double[] { 150, 16 };
            CheckingSampleSettings SampleSettings2 = new CheckingSampleSettings(null, Sample2, 3);
            response = kmeanApi.CheckSample(SampleSettings2, out detectedCluster);
            Assert.True(response.Code == 1);
            Assert.True(detectedCluster == -1);// Out of all clusters.

            double[] Sample3 = new double[] { 16, 14 };
            CheckingSampleSettings SampleSettings3 = new CheckingSampleSettings(null, Sample3, 3);
            response = kmeanApi.CheckSample(SampleSettings3, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 1);

            double[] Sample4 = new double[] { 6, 4 };
            CheckingSampleSettings SampleSettings4 = new CheckingSampleSettings(null, Sample4, 3);
            response = kmeanApi.CheckSample(SampleSettings4, out detectedCluster);
            Assert.True(response.Code == 0);
            Assert.True(detectedCluster == 0);


        }

        [Fact]
        public void Test()
        {
            AnomalyDetectionAPI AnoDet_Api = new AnomalyDetectionAPI(null, 0);
            ClusteringSettings Settings;
            SaveLoadSettings SaveObject;
            SaveLoadSettings LoadObject;
            AnomalyDetectionResponse ImportData;

            //TODO: Remove user specific paths from application.
            string FilePath = @"C:\Users\mhoshen\Desktop\DataSet\SampleDataSet.csv";
            double[][] RawData = Helpers.cSVtoDoubleJaggedArray(FilePath);
            string SavePath = @"C:\Users\mhoshen\Desktop\DataSet" + "json";
            ImportData = SaveLoadSettings.JSON_Settings(SavePath, out SaveObject, true);
            string LoadimpPath = @"C:\Users\mhoshen\Desktop\DataSet" + ".json";
            ImportData = SaveLoadSettings.JSON_Settings(LoadimpPath, out LoadObject, true);
            int kmeansMaxIterations = 5;
            int numClusters = 2;
            int numOfAttributes = 2;
            if (LoadimpPath.Contains("DataSet"))
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, Replace: true);
            }
            else
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, 1, false, LoadObject, Replace: true);
            }

            ImportData = AnoDet_Api.ImportNewDataForClustering(Settings);
        }


    }
}
