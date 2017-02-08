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

namespace Test
{
    public class TestWithLearningApi
    {
        [Fact]
        public void Training()
        {
            int cnt = 0;

            double[][] initialCentroids = new double[4][];
            initialCentroids[0] = new double[] { 0.4, 25.0 };
            initialCentroids[1] = new double[] { 0.4, 15.0 };
            initialCentroids[2] = new double[] {0.6, 15.0 };
            initialCentroids[3] = new double[] { 0.6, 25.0};

            string[] attributes = new string[] { "x", "y" };

            int numAttributes = attributes.Length;
            int numClusters = 4;
            int maxCount = 300;

            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings); //AnomalyDetectionAPI(clusterSettings), Constractor should not be null when run Training() 
            AnomalyDetectionResponse response;

            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var rawDataArray = getData(cnt);
                
                return rawDataArray;
            });

            api.UseDefaultDataMapper();
            api.UseGaussNormalizer();

            //
            for (int i = 0; i < 15; i++)
            {
                cnt = i;

                var rawData = api.Run() as double[][];

                response = kmeanApi.Training(rawData, initialCentroids);

                Helpers.WriteToCSVFile(kmeanApi.GetCentroid(), $"Data\\Centroid{i}.csv");

                //response = kmeanApi.Save($"Function{i}.json");
            }
        }


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
            var isris_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filePath);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseCsvDataProvider(isris_path, ',', 0);

            return api.Run() as object[][];
        }

        private object[][] getData(int cnt)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}\\Data\\Function{cnt}.csv";
            var isris_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filePath);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseCsvDataProvider(isris_path, ',', 0);

            return api.Run() as object[][];
        }
        #endregion
    }
}
