using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using LearningFoundation.Normalizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class UnitTestWithML
    {
        #region Test with Centroid
        [Fact]
        public void TestObjestWithCentroid()
        {
            double[][] initialCentroids = new double[4][];
            initialCentroids[0] = new double[] { 0.2, -4.0 };
            initialCentroids[1] = new double[] { 0.2, -6.0 };
            initialCentroids[2] = new double[] { 0.4, -4.0 };
            initialCentroids[3] = new double[] { 0.4, -6.0 };

            string[] attributes = new string[] { "x", "y" };
            
            var data = getRealDataSample(@"C:\Data\Third.csv");

            List<double[]> list = new List<double[]>();

            foreach(var n in data)
            {
                double[] d = new double[n.Length];

                for (int i = 0; i < n.Length; i++)
                {
                    d[i] = double.Parse(n[i].ToString());
                }
                   

                list.Add(d);
            }

            var rawData = list.ToArray();
            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 4;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            SaveLoadSettings sett;

            var resp = SaveLoadSettings.JSON_Settings(@"C:\Data\Third_Centroid_Without_Nor.json", out sett, true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters);
            
            ClusteringSettings Settings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, sett, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);

            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(Settings);

           
        }

        [Fact]
        public void TestWithNormalize_GaussAndCentroid()
        {
            double[][] initialCentroids = new double[4][];
            initialCentroids[0] = new double[] { 0.21875, 44.0 };
            initialCentroids[1] = new double[] { 0.25, 45.0 };
            initialCentroids[2] = new double[] { 0.46875, 44.0 };
            initialCentroids[3] = new double[] { 0.5, 43.0 };

            string[] attributes = new string[] { "x", "y" };
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return getRealDataSample(@"C:\Data\TestData01.csv");
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            api.UseGaussNormalizer();

            var rawData = api.Run() as double[][];

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 4;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            SaveLoadSettings sett;

            var resp = SaveLoadSettings.JSON_Settings(@"C:\Data\TestData02_Centroid_Gauss.json", out sett, true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters);

            ClusteringSettings Settings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, sett, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);

            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(Settings);


        }

        [Fact]
        public void TestWithNormalize_MinMaxAndCentroid()
        {
            double[][] initialCentroids = new double[4][];
            initialCentroids[0] = new double[] { 0.21875, 44.0 };
            initialCentroids[1] = new double[] { 0.25, 45.0 };
            initialCentroids[2] = new double[] { 0.46875, 44.0 };
            initialCentroids[3] = new double[] { 0.5, 43.0 };

            string[] attributes = new string[] { "x", "y" };
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return getRealDataSample(@"C:\Data\TestData02.csv");
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            var rawData = api.Run() as double[][];

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 4;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            SaveLoadSettings sett;

            var resp = SaveLoadSettings.JSON_Settings(@"C:\Data\TestData02_Centroid_MinMax.json", out sett, false);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(rawData, numClusters);

            ClusteringSettings Settings = new ClusteringSettings(rawData, maxCount, numClusters, numAttributes, sett, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);

            AnomalyDetectionResponse response = kmeanApi.ImportNewDataForClustering(Settings);


        }

        #endregion

        #region Test without Centroid

        [Fact]
        public void TestWithNormalize_Gauss()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return getRealDataSample(@"C:\Data\First.csv");
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            api.UseGaussNormalizer();

            //
            var result = api.Run() as double[][];

            Helpers.WriteToCSVFile(result);
        }

        [Fact]
        public void TestWithNormalize_MinMax()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return getRealDataSample(@"C:\Data\First.csv");
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            api.UseMinMaxNormalizer();

            ////use denormalizer on normalized data
            //api.UseMinMaxDeNormalizer();

            //
            var result = api.Run() as double[][];
            Helpers.WriteToCSVFile(result);

        }
        #endregion

        #region Help
        private DataDescriptor loadMetaData1()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };

            return des;
        }

        private object[][] getRealDataSample(string filePath)
        {
            //
            //iris data file
            var isris_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),filePath);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseCsvDataProvider(isris_path, ',', 0);

            return api.Run() as object[][];
        }
        #endregion
    }
}
