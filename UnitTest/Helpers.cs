﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest
{
    public static class Helpers
    {
        public static double[][] CreateSampleData(double[][] clusterCentars, int numSkalars, int numDataSamples, double maxDistanceFromClusterCentar)
        {
            List<double[]> samples = new List<double[]>();

            Random rnd = new Random();

            int numClusters = clusterCentars.Length;

            double[] distances = calcMinClusterDistance(clusterCentars, numSkalars);

            double[] allowedDeltas = new double[distances.Length];
            for (int i = 0; i < allowedDeltas.Length; i++)
            {
                allowedDeltas[i] = distances[i] * maxDistanceFromClusterCentar;
            }

            for (int i = 0; i < numDataSamples; i++)
            {
                for (int cluster = 0; cluster < numClusters; cluster++)
                {
                    var clusterSample = new double[numSkalars];

                    for (int skalar = 0; skalar < numSkalars; skalar++)
                    {
                        double sampleVal = 1.0 * rnd.Next((int)(clusterCentars[cluster][skalar] - allowedDeltas[skalar]),
                            (int)(clusterCentars[cluster][skalar] + allowedDeltas[skalar]));
                        clusterSample[skalar] = sampleVal;
                    }

                    samples.Add(clusterSample);
                }
            }

            return samples.ToArray();
        }


        /// <summary>
        /// Calculates minimal distance between cluster centars for each dimension.
        /// </summary>
        /// <param name="clusterCentars">Array of cluster centars.</param>
        /// <param name="numAttributes"></param>
        /// nearest cluster centars. 0.5% would generate samples directly between two cluster boundaries.</param>
        /// <returns>Minimum distance between centars per dimension.</returns>
        private static double[] calcMinClusterDistance(double[][] clusterCentars, int numAttributes)
        {
            double[] distances = new double[numAttributes];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = double.MaxValue;
            }

            for (int i = 0; i < clusterCentars.Length - 1; i++)
            {
                for (int j = i + 1; j < clusterCentars.Length; j++)
                {
                    for (int k = 0; k < distances.Length; k++)
                    {
                        var d = Math.Abs(clusterCentars[i][k] - clusterCentars[j][k]);
                        if (d < distances[k])
                            distances[k] = d;
                    }
                }
            }

            return distances;
        }

        /// <summary>
        /// This is for converting csv file to double array
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double[][] cSVtoDoubleJaggedArray(string FilePath)
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

        public static void WriteToCSVFile(double[][] rawData)
        {
            double[,] data = new double[500,500];
            using (StreamWriter outfile = new StreamWriter(File.Create(@"TestData.csv")))
            {
                for (int x = 0; x < rawData.Length; x++)
                {
                    string content = "";
                    for (int y = 0; y < rawData[x].Length; y++)
                    {
                        content += rawData[x][y]+"," ;
                    }
                    outfile.WriteLine(content);
                }
            }
        }
    }
}
