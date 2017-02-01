using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AnomalyDetection.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.IO;
using AnomalyDetectionApi;
using System.Runtime.Serialization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AnomalyDetectionRestApi.Controllers
{
    [Route("api/anomalydetection")]
    public class AnomalyAPIController : Controller
    {

        #region Test

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Test", "Function" };
        }

        #endregion

        #region Private Member Variables

        private static double[][] RawData { get; set; }
        private static int NumberOfClusters { get; set; }

        #endregion

        #region Public Methods 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainingContent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ImportNewDataForClustering")]
        public AnomalyDetectionResponse Training([FromBody] TrainingContent trainingContent)
        {
            ClusteringSettings clusterSettings = new ClusteringSettings(trainingContent.KmeansMaxIterations, trainingContent.NumOfClusters, trainingContent.NumOfAttributes, KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);
            AnomalyDetectionResponse response = null;

            try
            {
                double[][] rawData = null;

                foreach (var file in trainingContent.CsvFilePaths)
                {
                    rawData = dataProvider(file);

                    response = kmeanApi.Training(rawData);
                }

                if(response.Code == 0)
                response = kmeanApi.Save(trainingContent.SavePath);

                return response;
            }
            catch (Exception Ex)
            {

                if (Ex is System.IO.FileNotFoundException)
                {
                    response = new AnomalyDetectionResponse(200, "File not found");
                }
                else if (Ex is System.IO.DirectoryNotFoundException)
                {
                    response = new AnomalyDetectionResponse(204, "Directory not found");
                }
                else
                {
                    response = new AnomalyDetectionResponse(202, "File cannot be loaded");
                }
                return response;
            }

        }
        /// <summary>
        /// Training dataset
        /// </summary>
        /// <param name="csvFilePath">CSV fomated dataset</param>
        /// <param name="savePath">Where to save Instance and Cluster results</param>
        /// <param name="loadimpPath"></param>
        /// <param name="numClusters">Number of Clusters</param>
        /// <param name="numOfAttributes">Number of attributes</param>
        /// <param name="kmeansMaxIterations">Number of iterations</param>
        /// <returns></returns>

        [HttpGet]
        [Route("ImportNewDataForClustering/{csvFilePath}/{savePath}/{LoadimpPath}/{numClusters}/{numOfAttributes}/{kmeansMaxIterations}")]
        public AnomalyDetectionResponse Training(string[] csvFilePath, string savePath, string loadimpPath, int numClusters, int numOfAttributes, int kmeansMaxIterations)
        {
            ClusteringSettings clusterSettings = new ClusteringSettings(kmeansMaxIterations, numClusters, numOfAttributes, KmeansAlgorithm: 1, Replace: true);

            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);
            AnomalyDetectionResponse response;

            try
            {
                double[][] rawData = null;

                foreach (var file in csvFilePath)
                {
                     rawData = dataProvider(file);
                }

                response = kmeanApi.Training(rawData);

                response = kmeanApi.Save(savePath);

                return response;
            }
            catch (Exception Ex)
            {

                if (Ex is System.IO.FileNotFoundException)
                {
                     response = new AnomalyDetectionResponse(200, "File not found");
                }
                else if (Ex is System.IO.DirectoryNotFoundException)
                {
                     response = new AnomalyDetectionResponse(204, "Directory not found");
                }
                else
                {
                     response = new AnomalyDetectionResponse(202, "File cannot be loaded");
                }
                return response;
            }
        }

        /// <summary>
        /// Getting cluster result for drawing 
        /// </summary>
        /// <param name="clusterFilePath">Json formated cluster file path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ADClusteredData/{clusterFilePath}")]
        public Cluster[] GetClusteredData(string clusterFilePath)
        {
            Cluster[] cluster;
            AnomalyDetectionResponse AnoResponse;
            AnomalyDetectionAPI api = new AnomalyDetectionAPI();

            AnoResponse = api.GetClusters(clusterFilePath, out cluster);

            return cluster;
        }

        /// <summary>
        /// Check sample in cluster
        /// </summary>
        /// <param name="filePath">Json formated Instance result path</param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetClusId/{filePath}/{xAxis}/{yAxis}/{zAxis}/{tolerance}")]
        public AnomalyDetectionResponse CheckSampleInCluster(string filePath, double xAxis, double? yAxis, double? zAxis, double tolerance)
        {
            int detectedCluster;
            double[] sample;
            
            if (yAxis.HasValue && !zAxis.HasValue)
                sample = new double[] { xAxis, (double)yAxis };
            else if (yAxis.HasValue && zAxis.HasValue)
                sample = new double[] { xAxis, (double)yAxis, (double)zAxis };
            else
                sample = new double[] { xAxis };

            try
            {
                AnomalyDetectionAPI kApi = new AnomalyDetectionAPI();

                CheckingSampleSettings SampleSettings2 = new CheckingSampleSettings(filePath, sample, tolerance);

                var response = kApi.CheckSample(SampleSettings2, out detectedCluster);

                return response;

            }catch(Exception ex)
            {
                return new AnomalyDetectionResponse(200, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPreviousSamplesRest/{dataId}/{path}")]
        public double[][] GetPreviousData(int dataId, string path)
        {

            AnomalyDetectionAPI kApi = new AnomalyDetectionAPI();
            string filePath = $"{Directory.GetCurrentDirectory()}\\Instance Result\\CheckSample.json";
            double[][] oldData;

            var response = kApi.GetPreviousSamples(filePath, out oldData);

            return oldData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Anodropdown/{type}")]
        public IEnumerable<AnomalyDet01_DataSet_Type> GetAllAnoDD(string type)
        {
            List<AnomalyDet01_DataSet_Type> dataSet_Types = new List<AnomalyDet01_DataSet_Type>();

            using (SqlConnection conn = new SqlConnection(buildCfg().GetConnectionString("ProtocolAdapterDb")))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                if (type == "2D")
                {
                    cmd.CommandText = "select * from AnomalyDet01_DataSet_Type where DataSet_Scalar_1 is not null and DataSet_Scalar_2 is not null and DataSet_Scalar_3 is null or Data_Id = 10";
                }
                else if (type == "3D")
                {
                    cmd.CommandText = "select * from AnomalyDet01_DataSet_Type where DataSet_Scalar_1 is not null and DataSet_Scalar_2 is not null and DataSet_Scalar_3 is not null or Data_Id = 20";
                }
                else
                {
                    cmd.CommandText = "select * from AnomalyDet01_DataSet_Type";
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataset = mapToAnomalyDet01_DataSet_Type(reader);
                        if (dataset != null)
                        {
                            dataSet_Types.Add(dataset);
                        }
                        else
                        {
                            throw new Exception("Queury is not currect.");
                        }
                    }
                }
            }

            return dataSet_Types;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private AnomalyDet01_DataSet_Type mapToAnomalyDet01_DataSet_Type(SqlDataReader reader)
        {
            var dataset_type =  new AnomalyDet01_DataSet_Type();
            dataset_type.Data_Id = ConvertFromDBVal<int>(reader["Data_Id"]);
            dataset_type.DataSet_Name = ConvertFromDBVal<string>(reader["DataSet_Name"]);
            dataset_type.DataSet_Scalar_1 = ConvertFromDBVal<int>(reader["DataSet_Scalar_1"]);
            dataset_type.DataSet_Scalar_2 = ConvertFromDBVal<int>(reader["DataSet_Scalar_2"]);
            dataset_type.DataSet_Scalar_3 = ConvertFromDBVal<int>(reader["DataSet_Scalar_3"]);
            dataset_type.Max_Threshhold_Distance = ConvertFromDBVal<decimal>(reader["Max_Threshhold_Distance"]);
            dataset_type.Description = ConvertFromDBVal<string>(reader["Description"]);
            dataset_type.DataSource = ConvertFromDBVal<string>(reader["DataSource"]);
            dataset_type.Dimension = ConvertFromDBVal<int>(reader["Dimension"]);

            return dataset_type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IConfigurationRoot buildCfg()
        {
            IConfigurationRoot Configuration;
            string basePath = System.IO.Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            return Configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        static AnomalyAPIController()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RawData"></param>
        /// <param name="KmeansMaxIterations"></param>
        /// <param name="NumberOfAttributes"></param>
        /// <param name="MaxNumberOfClusters"></param>
        /// <param name="MinNumberOfClusters"></param>
        /// <param name="Method"></param>
        /// <param name="StdDev"></param>
        /// <param name="RecNumberOfClusters"></param>
        /// <returns></returns>
        public AnomalyDetectionResponse RecommendedNumberOfClusters(double[][] RawData, int KmeansMaxIterations, int NumberOfAttributes, int MaxNumberOfClusters, int MinNumberOfClusters, int Method, double[] StdDev, out int RecNumberOfClusters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="skipRows"></param>
        /// <returns></returns>
        private double[][] dataProvider(string fileName, int skipRows = 0)
        {
            List<double[]> rawData = new List<double[]>();

            using (StreamReader reader = System.IO.File.OpenText(fileName))
            {
                int linenum = 0;
                foreach (string line in readLineFromFile(reader))
                {
                    //split line in to column
                    var strCols = line.Split(',');

                    //skip first ... rows
                    if (linenum < skipRows)
                    {
                        linenum++;
                        continue;
                    }

                    //Transform data from row->col in to col->row
                    //var singleRow = new double[strCols.Length];
                    var singleRow = new List<double>();

                    //define columns
                    for (int i = 0; i < strCols.Length; i++)
                    {
                        double data;

                        if (double.TryParse(strCols[i], out data))
                            singleRow.Add(data);
                    }

                    rawData.Add(singleRow.ToArray());
                }

                return rawData.ToArray();
            }
        }

        /// <summary>
        /// Reading stream reader line by line with IEnumerable collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<string> readLineFromFile(StreamReader reader)
        {
            string currentLine;
            while ((currentLine = reader.ReadLine()) != null)
            {
                yield return currentLine;
            }
        }

        #endregion

    }
}
