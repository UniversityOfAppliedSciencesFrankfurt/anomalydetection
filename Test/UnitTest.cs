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
        //[Fact]
        //public void Test_ImportNewDataForClustering()
        //{
        //    double[][] clusterCentars = new double[3][];
        //    clusterCentars[0] = new double[] { 5.0, 5.0 };
        //    clusterCentars[1] = new double[] { 15.0, 15.0 };
        //    clusterCentars[2] = new double[] { 30.0, 30.0 };

        //    string[] attributes = new string[] { "Height", "Weight" };

        //    var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);

        //    int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
        //    int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
        //    int maxCount = 300;  // trial and error

        //    SaveLoadSettings persistenceProviderSettings;

        //    var resp = SaveLoadSettings.JsonSettings("model.json", out persistenceProviderSettings, false);

        //    IAnomalyDetectionApi kmeanApi = new AnomalyDetectionAPI();
        //    ClusteringSettings clusterSettings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, persistenceProviderSettings, KmeansAlgorithm: 1, Replace: true);
        //    AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(clusterSettings);

        //    Assert.True(response.Code == 0); 
        //}

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

           // SaveLoadSettings persistenceProviderSettings;

           // var resp = SaveLoadSettings.JsonSettings("model.json", out persistenceProviderSettings, false);

            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes,KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);

            //for (int i = 0; i < 1; i++)
            //{
                AnomalyDetectionResponse response = kmeanApi.Training(rawData, clusterCentars);
          //  }

            kmeanApi.Save("interface.json");
            //Assert.True(response.Code == 0);
        }

       // [Fact]
        //public void Test_CheckSample()
        //{
        //    SaveLoadSettings persistenceProviderSettings;

        //    //For checking sample, file path should be Instace Result path
        //    var resp = SaveLoadSettings.JsonSettings(@"C:\Users\mhoshen\Documents\Visual Studio 2015\Projects\AnomDetect.KMeans\UnitTest\Instance Result\model.json", out persistenceProviderSettings, false);

        //    IAnomalyDetectionApi kmeanApi = new AnomalyDetectionAPI();

        //    int detectedCluster;

        //    double[] Sample = new double[] { 26, 28 };

        //    CheckingSampleSettings SampleSettings = new CheckingSampleSettings(persistenceProviderSettings, Sample, 3);

        //    var response = kmeanApi.CheckSample(SampleSettings, out detectedCluster);

        //    Assert.True(response.Code == 0);
        //    Assert.True(detectedCluster == 0);

        //}

        //[Fact]
        //public void Test_GetClusters()
        //{
        //    SaveLoadSettings persistenceProviderSettings;

        //    //For getting clusters, file path should be Cluster Result path
        //    var resp = SaveLoadSettings.JsonSettings(@"C:\Users\mhoshen\Documents\Visual Studio 2015\Projects\AnomDetect.KMeans\UnitTest\Cluster Result\model.json", out persistenceProviderSettings, false);

        //    IAnomalyDetectionApi kmeanApi = new AnomalyDetectionAPI();

        //    Cluster[] detectedCluster;

        //    var response = kmeanApi.GetClusters(persistenceProviderSettings, out detectedCluster);

        //    Assert.True(response.Code == 0);
        //}
    }
}
