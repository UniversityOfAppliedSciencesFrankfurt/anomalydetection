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
        public void test()
        {
            AnomalyDetectionAPI AnoDet_Api = new AnomalyDetectionAPI(null, 0);
            ClusteringSettings Settings;
            SaveLoadSettings SaveObject;
            SaveLoadSettings LoadObject;
            AnomalyDetectionResponse ImportData;
            string FilePath = @"C:\Users\mhoshen\Desktop\DataSet\SampleDataSet.csv";
            double[][] RawData = CSVtoDoubleJaggedArray(FilePath);
            string SavePath = @"C:\Users\mhoshen\Desktop\DataSet"+ "json";
            ImportData = SaveLoadSettings.JSON_Settings(SavePath, out SaveObject, true);
            string LoadimpPath = @"C:\Users\mhoshen\Desktop\DataSet" + ".json";
            ImportData = SaveLoadSettings.JSON_Settings(LoadimpPath, out LoadObject, true);
            int kmeansMaxIterations = 5;
            int numClusters = 2;
            int numOfAttributes = 2;
            if (LoadimpPath.Contains("DataSet"))
            {
                Settings = new ClusteringSettings(RawData,kmeansMaxIterations,numClusters,numOfAttributes,SaveObject, Replace: true);
            }
            else
            {
                Settings = new ClusteringSettings(RawData, kmeansMaxIterations, numClusters, numOfAttributes, SaveObject, 1, false, LoadObject, Replace: true);
            }

            ImportData = AnoDet_Api.ImportNewDataForClustering(Settings);
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
    }
}
