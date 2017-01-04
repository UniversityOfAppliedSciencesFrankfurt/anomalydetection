using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AnomalyDetection.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

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
        IAnomalyDetectionApi AnoDet_Api = new AnomalyDetectionApi.AnomalyDetectionAPI(RawData, NumberOfClusters);

        #endregion

        #region Public Methods 

        /// <summary>
        /// This is for importing csv file and running Clustering
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="SavePath"></param>
        /// <param name="LoadimpPath"></param>
        /// <param name="numClusters"></param>
        /// <param name="numOfAttributes"></param>
        /// <param name="kmeansMaxIterations"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("ImportNewDataForClustering/{FileName}/{SavePath}/{LoadimpPath}/{numClusters}/{numOfAttributes}/{kmeansMaxIterations}")]
        public AnomalyDetectionResponse ImportNewDataForClustering(string FileName, string SavePath, string LoadimpPath, int numClusters, int numOfAttributes, int kmeansMaxIterations)
        {
            ClusteringSettings Settings;
            SaveLoadSettings SaveObject;
            SaveLoadSettings LoadObject;
            AnomalyDetectionResponse ImportData;
            string FilePath = @"C:\Data\" + FileName.TrimEnd() + ".csv";
            double[][] RawData = CSVtoDoubleJaggedArray(FilePath);
            SavePath = @"C:\Data\" + SavePath + ".json";
            ImportData = SaveLoadSettings.JSON_Settings(SavePath, out SaveObject, true);
            LoadimpPath = @"C:\Data\Result\" + LoadimpPath.TrimEnd() + ".json";
            ImportData = SaveLoadSettings.JSON_Settings(LoadimpPath, out LoadObject, true);
            if (LoadimpPath.Equals("NewData"))
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, Replace: true);
            }
            else
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, 1, false, LoadObject, Replace: true);
            }

            ImportData = AnoDet_Api.ImportNewDataForClustering(Settings);
            return ImportData;
        }

        /// <summary>
        /// Returns clustered data from the API
        /// </summary>
        /// <param name="DataId"></param>
        /// <param name="LoadPath"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ADClusteredData/{LoadPath}")]
        public ClusteringResults[] ClusteredDatadirect(int DataId, string LoadPath)
        {
            ClusteringResults[] Result;
            AnomalyDetectionResponse AnoResponse;
            SaveLoadSettings LoadObject;
            LoadPath = @"C:\Data\" + LoadPath.TrimEnd() + ".json";
            AnoResponse = SaveLoadSettings.JSON_Settings(LoadPath, out LoadObject, true);
            AnoResponse = AnoDet_Api.GetResults(LoadObject, out Result);
            return Result;
        }

        /// <summary>
        /// Checks for the Sample Clusters
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="xaxis"></param>
        /// <param name="yaxis"></param>
        /// <param name="zaxis"></param>
        /// <param name="Tol"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetClusId/{fileName}/{xaxis}/{yaxis}/{zaxis}/{Tol}")]
        public AnomalyDetectionResponse GetSingleSampleClusterId(string fileName, double xaxis, double? yaxis, double? zaxis, double Tol)
        {
            int ClusterIndex;
            AnomalyDetectionResponse AnoResponse;
            SaveLoadSettings LoadObject;
            double[] SampletoCheck;
            string LoadPath = @"C:\data\" + fileName.TrimEnd() + ".json";

            AnoResponse = SaveLoadSettings.JSON_Settings(LoadPath, out LoadObject, true);
            if (yaxis.HasValue && !zaxis.HasValue)
                SampletoCheck = new double[] { xaxis, (double)yaxis };
            else if (yaxis.HasValue && zaxis.HasValue)
                SampletoCheck = new double[] { xaxis, (double)yaxis, (double)zaxis };
            else
                SampletoCheck = new double[] { xaxis };
            CheckingSampleSettings SampleSettings = new CheckingSampleSettings(LoadObject, SampletoCheck, Tol);


            AnoResponse = AnoDet_Api.CheckSample(SampleSettings, out ClusterIndex);
            return AnoResponse;
        }

        /// <summary>
        /// This is for getting the previously saved clustered data
        /// </summary>
        /// <param name="DataId"></param>
        /// <param name="LoadPath"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPreviousSamplesRest/{DataId}/{LoadPath}")]
        public double[][] GetPreviousSamplesRest(int DataId, string LoadPath)
        {
            AnomalyDetectionResponse AnoResponse;
            SaveLoadSettings LoadObject;
            LoadPath = @"C:\Data\" + LoadPath.TrimEnd() + ".json";
            AnoResponse = SaveLoadSettings.JSON_Settings(LoadPath, out LoadObject, true);

            double[][] PreviousSamples;
            AnoResponse = AnoDet_Api.GetPreviousSamples(LoadObject, out PreviousSamples);
            return PreviousSamples;
        }

        ///// <summary>
        ///// Gets Anomaly Type data for the drop down from azure data base
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
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
        /// This is for converting csv file to double array
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double[][] CSVtoDoubleJaggedArray(string FilePath)
        {
            if (FilePath.EndsWith(".csv"))
            {
                if (System.IO.File.Exists(FilePath))
                {
                    string CsvFile = "";
                    double[][] CsvData;
                    CsvFile = System.IO.File.ReadAllText(FilePath);
                    if (CsvFile.EndsWith("\r\n"))
                    {
                        CsvFile = CsvFile.Remove(CsvFile.Length - 2, 2);
                    }
                    string[] RowDelimiter = { "\r\n" };
                    string[] CellDelimiter = { "," };

                    int CsvFileRowsNumber, CsvFileCellsNumber;
                    string[] Rows, Cells;

                    Rows = CsvFile.Split(RowDelimiter, StringSplitOptions.None);
                    CsvFileRowsNumber = Rows.Length;

                    CsvFileCellsNumber = Rows[0].Split(CellDelimiter, StringSplitOptions.None).Length;
                    CsvData = new double[CsvFileRowsNumber][];
                    for (int i = 0; i < CsvFileRowsNumber; i++)
                    {
                        CsvData[i] = new double[CsvFileCellsNumber];
                    }

                    for (int i = 0; i < CsvFileRowsNumber; i++)
                    {
                        Cells = Rows[i].Split(CellDelimiter, StringSplitOptions.None);

                        for (int j = 0; j < CsvFileCellsNumber; j++)
                        {
                            try
                            {
                                CsvData[i][j] = Convert.ToDouble(Cells[j]);
                            }
                            catch (FormatException)
                            {
                                return null;
                            }
                            catch (OverflowException)
                            {
                                return null;
                            }

                        }
                    }

                    return CsvData;
                }
            }
            return null;
        }
        #endregion

    } // Program
} // ns
