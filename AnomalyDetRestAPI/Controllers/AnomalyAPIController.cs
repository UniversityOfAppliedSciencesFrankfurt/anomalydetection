using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AnomalyDetRestAPI.Models;
using System.IO;
using AnomalyDetection.Interfaces;

namespace AnomalyDetRestAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AnomalyAPIController : ApiController
    {
       
        private static double[][] RawData { get; set; }

      
        private static int NumberOfClusters { get; set; }
        IAnomalyDetectionApi AnoDet_Api = new AnomalyDetectionApi.AnomalyDetectionAPI(RawData, NumberOfClusters);
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
        public AnomalyDetectionResponse ImportNewDataForClustering(string FileName, string SavePath, string LoadimpPath, int numClusters, int numOfAttributes, int kmeansMaxIterations)
        {
            ClusteringSettings Settings;
            SaveLoadSettings SaveObject;
            SaveLoadSettings LoadObject;
            AnomalyDetectionResponse ImportData;
            string FilePath = @"C:\Data\" + FileName.TrimEnd() + ".csv";
            double[][] RawData = CSVtoDoubleJaggedArray(FilePath);
            SavePath = @"C:\Data\" + SavePath + "json";
            ImportData = SaveLoadSettings.JSON_Settings(SavePath, out SaveObject, true);
            LoadimpPath = @"C:\Data\Result\" + LoadimpPath.TrimEnd() + ".json";
            ImportData = SaveLoadSettings.JSON_Settings(LoadimpPath, out LoadObject, true);
            if (LoadimpPath.Equals("NewData") )
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, Replace: true);
            }
            else
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject,1,false, LoadObject, Replace: true);
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
        public double[][] GetPreviousSamplesRest(int DataId,string LoadPath)
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
        //public IEnumerable<AnomalyDet01_DataSet_Type> GetAllAnoDD(string type)
        //{
        //    IEnumerable<AnomalyDet01_DataSet_Type> DataTypelData;
        //    AnoDetDataDataContext db = new AnoDetDataDataContext();
        //    if (type == "2D")
        //    {
        //        DataTypelData = from q in db.AnomalyDet01_DataSet_Types
        //                        where (q.DataSet_Scalar_1 != null && q.DataSet_Scalar_2 != null && q.DataSet_Scalar_3 == null)
        //                        select q;
        //    }
        //    else if (type == "3D")
        //    {
        //        DataTypelData = from r in db.AnomalyDet01_DataSet_Types
        //                        where (r.DataSet_Scalar_1 != null && r.DataSet_Scalar_2 != null && r.DataSet_Scalar_3 != null)
        //                        select r;
        //    }
        //    else
        //        DataTypelData = from p in db.AnomalyDet01_DataSet_Types
        //                        select p;
        //    return DataTypelData;
        //}


        static AnomalyAPIController()
        {

        }

        static void Main(string[] args)
        {


        } // Main

        // ============================================================================

       
        #region IAnomalyDetectionApi Members

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

        #endregion
        /// <summary>
        /// This is for converting csv file to double array
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double[][] CSVtoDoubleJaggedArray(string FilePath)
        {
            if (FilePath.EndsWith(".csv"))
            {
                if (File.Exists(FilePath))
                {
                    string CsvFile = "";
                    double[][] CsvData;
                    CsvFile = File.ReadAllText(FilePath);
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
    } // Program
} // ns
