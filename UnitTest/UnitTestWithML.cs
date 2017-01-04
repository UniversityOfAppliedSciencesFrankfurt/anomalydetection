using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using LearningFoundation;
using LearningFoundation.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class UnitTestWithML
    {
        [Fact]
        public void TestObjest()
        {

            double[][] initialCentroids = new double[4][];
            initialCentroids[0] = new double[] { 0.1875, 22.0 };
            initialCentroids[1] = new double[] { 0.28125, 20.0 };
            initialCentroids[2] = new double[] { 1.375, 24.0 };
            initialCentroids[3] = new double[] { 1.40625, 22.0 };

            string[] attributes = new string[] { "x", "y" };

            var rawData = Helpers.cSVtoDoubleJaggedArray("Accelerometer-2011-04-11-13-29-54-brush_teeth-f1.csv");

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 4;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            SaveLoadSettings sett;

            var resp = SaveLoadSettings.JSON_Settings("model.json", out sett, false);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters, initialCentroids);
            ClusteringSettings Settings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, sett, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);
            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(Settings);

            //int detectedCluster;
            //double[] Sample = new double[] {57};
            //CheckingSampleSettings SampleSettings = new CheckingSampleSettings(null, Sample, 3);
            //response = kmeanApi.CheckSample(SampleSettings, out detectedCluster);
            //Assert.True(response.Code == 0);
            //Assert.True(detectedCluster == 2);
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

            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10, 0.5);
            Helpers.WriteToCSVFile(rawData);

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
        }


    }
}
