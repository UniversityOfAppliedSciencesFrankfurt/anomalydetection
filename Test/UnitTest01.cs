using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    static class Globals
    {
        public static int PermCount;
    }
    public class UnitTest01
    {
        // Test: reads function, generate similar functions, cluster (for different
        //       number of clusters and several runs per each number of clusters) and
        //       save the results
        [Fact]
        public void GenerateAndTest()
        {
            // Settings to Generate Similar functions
            string FunctionName = "Function0";
            string directory = @"C:\Users\KywAnn\Desktop\Anomaly Detection Functions Normalized\" + FunctionName + "\\";
            // number of similar functions
            int NumSimFunc = 999;
            // added noise level (distortion) between -NL & NL 
            int NL = 1;

            // Settings for the K-Means Alg
            int maxCount = 500;
            int numClusters = 6;
            int numAttributes = 2;
            int KAlg = 2;
            int Runs = 5;
         
            // generate the similar functions
            GenerateSimilarFunctions(directory + FunctionName + ".csv", NumSimFunc, NL);

            // prepare the 2d functions for clustering
            double[][] mFun = Helpers.cSVtoDoubleJaggedArray(directory + "\\NL" + NL + "\\" + FunctionName + " SimilarFunctions NL" + NL + ".csv");
            // normalize the functions
            mFun = NormalizeData(mFun);
            // save the normalized similar functions
            Helpers.Write2CSVFile(mFun, directory + "\\NL" + NL + "\\" + FunctionName + " SimilarFunctions Normalized NL" + NL + ".csv");
            int NumFun = mFun.Length - 1;
            

            ClusteringSettings clusterSettings;
            AnomalyDetectionAPI kmeanApi;
            AnomalyDetectionResponse response;

            double[][] Centroids;
            // original Centroids
            double[][] oCentroids;
            // matched Centroids
            double[][] mCentroids;

            for (int k = 2; k < numClusters + 1; k++)
            {
                oCentroids = new double[k][];
                clusterSettings = new ClusteringSettings(maxCount, k, numAttributes, KmeansAlgorithm: KAlg, Replace: true);
                kmeanApi = new AnomalyDetectionAPI(clusterSettings);
                for (int j = 0; j < Runs; j++)
                {
                    // save directory
                    string SavePath = directory + "NL" + NL + "\\" + FunctionName + " SimilarFunctions Centroids NL" + NL + " KA" + KAlg + " C" + k + " I" + maxCount + " R" + (j + 1) + ".csv";

                    for (int i = 0; i < NumFun; i++)
                    {
                        // cluster each function
                        double[][] rawData = getSimilarFunctionsData(mFun, i + 1);
                        response = kmeanApi.Training(rawData);
                        Centroids = kmeanApi.GetCentroid();
                        // match the centroids centroids
                        if (i == 0)
                        {
                            oCentroids = Centroids;
                            mCentroids = Centroids;
                        }
                        else
                        { 
                            mCentroids = matchCentroids(Centroids, oCentroids);
                        }

                        // save centroids
                        if (i == 0)
                        {
                            // save or overwrite
                            Helpers.Write2CSVFile(mCentroids, SavePath);
                        }
                        else
                        {
                            // append
                            Helpers.Write2CSVFile(mCentroids, SavePath, true);
                        }
                    }
                    // save in a different format to plot results easily in excel
                    Special2DWrite2CSV(SavePath, k);
                }
            }
        }

        // Test: gets the recommended number of clusters based on the balanced clusters
        //       method
        [Fact]
        public void Test_BalancedClusters()
        {
            // prepare the 2d functions for clustering
            double[][] mFun = Helpers.cSVtoDoubleJaggedArray(@"C:\Users\KywAnn\Desktop\Anomaly Detection Functions Normalized\Sine 3 HP\Sine 3 HP.csv");
            // normalize functions
            mFun = NormalizeData(mFun);
            // load the original function
            double[][] rawData = getSimilarFunctionsData(mFun, 1);

            // Settings for the K-Means Alg
            int maxCount = 500;
            int maxNumberOfClusters = 5;
            int minNumberOfClusters = 2;
            int numAttributes = 2;
            int KAlg = 2;
            // recommended number of clusters
            int recNumClusters;
            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, 2, numAttributes, KmeansAlgorithm: KAlg, Replace: true);
            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);
            // get the suggested number of clusters bas on the balanced clusters method
            AnomalyDetectionResponse ar = kmeanApi.RecommendedNumberOfClusters(rawData, maxCount, numAttributes, maxNumberOfClusters, minNumberOfClusters, 3, null, out recNumClusters, kmeansAlgorithm: KAlg);
        }

        // Test: clusters the training functions (similar functions), then clusters the
        //       resulting centroids, then tests for pattern matching by clustering the 
        //       testing functions (similar & different functions) and checking if the
        //       centroids of the tested function falls in the clusters obtained during
        //       training
        [Fact]
        public void Test_PatternReccognition()
        {
            // directory to load
            string myDirectory = @"C:\Users\KywAnn\Desktop\Anomaly Detection Pattern Recognition\Generated Similar Functions\NL1\";
            // directory to save
            string saveDirectory = @"C:\Users\KywAnn\Desktop\Anomaly Detection Pattern Recognition\LastRun\";
            // functions' names
            string[] FunctionPaths = new string[]
            {
                "Sine 5 HP SimilarFunctions NL1.csv",
                "Triangular 4 HP SimilarFunctions NL1.csv",
                "TriangularHWR SimilarFunctions NL1.csv"
            };

            // Settings for the K-Means Alg
            int maxCount = 500;
            int numClusters = 4;
            int numAttributes = 2;
            int KAlg = 2;

            ClusteringSettings clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: KAlg, Replace: true);
            AnomalyDetectionAPI kmeanApi = new AnomalyDetectionAPI(clusterSettings);

            double[][] mFun;
            // Number of functions (training + testing)
            int NumFun;
            // Number of training functions
            int NumTrainFun = 800;
            // limit between training and testing functions
            int NumFunLimit = 0;
            AnomalyDetectionResponse response;
            double[][] rawData;
            double[][] Centroids;

            // load and cluster all desired functions
            for (int s = 0; s < FunctionPaths.Length; s++)
            {
                mFun = Helpers.cSVtoDoubleJaggedArray(myDirectory + FunctionPaths[s]);
                mFun = NormalizeData(mFun);
                NumFun = mFun.Length - 1;
                for (int i = NumFunLimit; i < NumFun; i++)
                {
                    rawData = getSimilarFunctionsData(mFun, i + 1);
                    response = kmeanApi.Training(rawData);
                    Centroids = kmeanApi.GetCentroid();

                    // save centroids
                    if (i == NumFunLimit)
                    {
                        Helpers.Write2CSVFile(Centroids, saveDirectory+ "Centroids Of "+ FunctionPaths[s]);
                    }
                    else
                    {
                        Helpers.Write2CSVFile(Centroids, saveDirectory + "Centroids Of " + FunctionPaths[s], true);
                    }
                }
                // only cluster testing data for the rest of the functions
                NumFunLimit = NumTrainFun;
            }

            // get centroids for pattern recognition
            rawData = Helpers.cSVtoDoubleJaggedArray(saveDirectory + "Centroids Of " + FunctionPaths[0]);
            // get only the training centroids
            double[][] trainData = new double[NumFunLimit * numClusters][];
            for (int i = 0; i < NumFunLimit * numClusters; i++)
            {
                // get training data
                trainData[i] = rawData[i];
            }
            clusterSettings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: KAlg, Replace: true);
            kmeanApi = new AnomalyDetectionAPI(clusterSettings);
            response = kmeanApi.Training(trainData);
            // get & save the centroids of clustered centroids
            Centroids = kmeanApi.GetCentroid();
            Helpers.Write2CSVFile(Centroids, saveDirectory + "Calculated Centroids.csv");
            // get & save max distance per cluster
            double[] maxDistance = kmeanApi.GetInClusterMaxDistance();
            double[][] tempMaxDistance = new double[1][];
            tempMaxDistance[0] = maxDistance;
            Helpers.Write2CSVFile(tempMaxDistance, saveDirectory + "Calculated Max Distance.csv");

            // start testing for pattern recognition
            int tolerance = 0;
            // start testing centroids of similar functions
            double[][] testData = new double[rawData.Length - NumFunLimit * numClusters][];
            for (int i = 0; i < testData.Length; i++)
            {
                // get testing data
                testData[i] = rawData[i+NumFunLimit*numClusters];
            }
            // contains results of pattern testing (1 for matching, 0 otherwise)
            double[][] testFun = new double[FunctionPaths.Length][];
            // check if the pattern match for similar functions (fits in all clusters)
            testFun[0] = PatternTesting(testData, numClusters, kmeanApi, tolerance);
            // start testing centroids different functions
            for (int p = 1; p < FunctionPaths.Length; p++)
            {
                // get testing data
                testData = Helpers.cSVtoDoubleJaggedArray(saveDirectory + "Centroids Of " + FunctionPaths[p]);
                // check if the pattern match for differnt functions (fits in all clusters)
                testFun[p] = PatternTesting(testData, numClusters, kmeanApi, tolerance);
            }
            // save results
            Helpers.Write2CSVFile(testFun, saveDirectory + "Results.csv");
        }

        [Fact]
        public void Test_BitArray()
        {
            string myDirectorty = @"C:\Users\KywAnn\Desktop\Anomaly Detection Pattern Recognition\BitArray\";
            string trainingFileName = "BitArrayTraining.txt";

            string TrainingFile = Helpers.ReadTxtFile(myDirectorty + trainingFileName);
            TrainingFile = TrainingFile.Remove(TrainingFile.Length - 4, 4);
            TrainingFile = TrainingFile.Replace("\r\n", "");
            string[] TrainingStrings = TrainingFile.Split(new[] { " 1" }, StringSplitOptions.None);
            string[] CSVString = new string[TrainingStrings.Length+1];
            for (int i = 0; i < TrainingStrings[0].Length; i++)
            {
                CSVString[0] += i.ToString() + ',';
            }
            for (int j = 0; j < TrainingStrings.Length; j++)
            {
                for (int i = 0; i < TrainingStrings[0].Length; i++)
                {
                    CSVString[j+1] += TrainingStrings[j][i].ToString() + ',';
                }
                CSVString[j+1] = CSVString[j+1].Remove(CSVString[j+1].Length - 1,1);
            }

            Helpers.Txt2CSV(CSVString, myDirectorty + "BitArrayTraining.csv");
        }

        // generates the similar functions (only 2d)
        private static void GenerateSimilarFunctions(string path, int NumFunctions, double RandomNoiseLimit)
        {
            // load original function
            double[][] mFun = Helpers.cSVtoDoubleJaggedArray(path);
            double[][] mFun2 = new double[mFun.Length - 1][];
            for (int i = 0; i < mFun2.Length; i++)
            {
                mFun2[i] = new double[mFun[0].Length];
            }
            string fName = Path.GetDirectoryName(path) + "\\NL" + RandomNoiseLimit + "\\" + Path.GetFileNameWithoutExtension(path) + " SimilarFunctions NL" + RandomNoiseLimit + ".csv";
            // save original function in new file
            Helpers.Write2CSVFile(mFun, fName);
            // seed for random numbers
            int seed = 0;
            for (int i = 0; i < NumFunctions; i++)
            {
                //only for 2 dimensions
                for (int j = 0; j < mFun[0].Length; j++)
                {
                    // add random noise to the y-coordinates between -NL & +NL                 
                    mFun2[0][j] = mFun[1][j] + GetRandomNumber(RandomNoiseLimit * -1, RandomNoiseLimit, seed);
                    seed++;
                }
                // append the newly generated y-coordinates
                Helpers.Write2CSVFile(mFun2, fName, true);
            }
        }

        // generates random number in the specified range using seed and current time
        private static double GetRandomNumber(double minimum, double maximum, int seed)
        {
            Random rnd = new Random(seed * DateTime.Now.Millisecond);
            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }

        // selects the specified function using function number (only 2d) and changes 
        // the format for clustering purposes
        private static double[][] getSimilarFunctionsData(double[][] mFun, int FunctionNumber)
        {
            double[][] Data = new double[2][];
            // load the x-coordinates
            Data[0] = mFun[0];
            // load the y-coordinates
            Data[1] = mFun[FunctionNumber];
            // change format for clustering purposes
            double[][] rawData = new double[Data[0].Length][];
            for (int i = 0; i < Data[0].Length; i++)
            {
                // each row of rawData will have x & y coordinates
                rawData[i] = new double[2];
                rawData[i][0] = Data[0][i];
                rawData[i][1] = Data[1][i];
            }
            return rawData;
        }

        // sorts centroids w.r.t 1st dimention (x-coordinates)
        /*private static double[][] SortCentroids(double[][] Centroids)
        {
            // sorted centroids w.r.t 1st dimention (x-coordinates)
            double[][] SCentroids = new double[Centroids.Length][];
            for (int i = 0; i < Centroids.Length; i++)
            {
                SCentroids[i] = Centroids[i];

            }
            double[] temp = new double[Centroids[0].Length];
            // smallest x-coordinate and its index
            double min;
            int index;
            // if new minimum was found
            bool newMin;
            for (int i = 0; i < Centroids.Length; i++)
            {
                newMin = false;
                min = SCentroids[i][0];
                index = i;
                for (int j = i + 1; j < Centroids.Length; j++)
                {
                    if (SCentroids[j][0] < min)
                    {
                        // update the minimum values
                        min = SCentroids[j][0];
                        index = j;
                        newMin = true;
                    }
                }
                if (newMin)
                {
                    // swap
                    temp = SCentroids[i];
                    SCentroids[i] = SCentroids[index];
                    SCentroids[index] = temp;
                }

            }
            return SCentroids;
        }*/

        // matches centroids based on minimum total distance and returns them in same
        // cluster order as the original
        private static double[][] matchCentroids(double[][] newCentroids, double[][] originalCentroids)
        {
            int n = originalCentroids.Length;
            // get all possible permutations
            int[][] permutations = FindPermutations(n);
            // get the squared distances
            double[][] squaredDistances = squaredDistanceMatrix(originalCentroids, newCentroids);
            // get best permutaion
            int bestPerm = bestPermutaion(squaredDistances, permutations);
            // initialize and sort the centroids according to best permutation
            double[][] matchedCentroids = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matchedCentroids[i] = new double[originalCentroids[0].Length];
                matchedCentroids[i] = newCentroids[permutations[bestPerm][i]];
            }
            return matchedCentroids;
        }

        // calculates all squared distances of any possible pair between original and
        // new centroids
        private static double[][] squaredDistanceMatrix(double[][] originalCentroids, double[][] newCentroids)
        {
            int n = originalCentroids.Length;
            // initialize the squared distance matrix
            double[][] distanceMatrix = new double[n][];
            for(int i = 0; i < n; i++)
            {
                distanceMatrix[i] = new double[n];
            }
            // calculate the squared distances
            for (int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    distanceMatrix[i][j] = squaredDistance(originalCentroids[i], newCentroids[j]);
                }
            }
            return distanceMatrix;
        }

        // calculates the squared distance between 2 double arrays
        private static double squaredDistance(double[] A, double[] B)
        {
            double SquaredDistance = 0;
            for (int i = 0; i < A.Length; i++)
            {
                // for each coordinate
                SquaredDistance += Math.Pow(A[i] - B[i], 2);
            }
            return SquaredDistance;
        }

        // returns index of the permutaion that result in smallest total distance
        private static int bestPermutaion(double[][] squaredDistances, int[][] permutations)
        {
            // index resulting in minimum total distance
            int bestPerm = 0;
            // calculate total distances of all possible pairs
            double[] Sum = new double[permutations.Length];
            for (int i = 0; i < permutations.Length; i++)
            {
                Sum[i] = 0;
                for(int j = 0; j < squaredDistances.Length; j++)
                {
                    Sum[i] += squaredDistances[j][permutations[i][j]];
                }
                // update index
                if (Sum[i] < Sum[bestPerm])
                {
                    bestPerm = i;
                }
            }
            return bestPerm;
        }

        // saves centroids in a special format to be plot easier using excel (only 2d)
        private static void Special2DWrite2CSV(string path, int NumClusters)
        {
            // load centroids
            double[][] Centroids = Helpers.cSVtoDoubleJaggedArray(path);
            // prepare the new centroids as string arrays
            string[][] EditedCentroids = new string[Centroids.Length][];
            for (int i = 0; i < Centroids.Length; i++)
            {
                EditedCentroids[i] = new string[NumClusters + 1];
                // fill x-coordinates normally
                EditedCentroids[i][0] = Centroids[i][0].ToString();
                // fill y-coordinates based on to which cluster the centroid belongs
                EditedCentroids[i][i % NumClusters + 1] = Centroids[i][1].ToString();
            }
            // change name and save
            string SavePath = path.Remove(path.Length - 4, 4) + " edited.csv";
            Helpers.Write2CSVFile(EditedCentroids, SavePath);
        }

        // normalizes the data
        private static double[][] NormalizeData(double[][] Data)
        {
            int RowsNumber = Data.Length;
            int ColumnsNumber = Data[0].Length;
            // initialize the normalized data
            double[][] NormalizedData = new double[RowsNumber][];
            for (int i = 0; i < Data.Length; i++)
            {
                NormalizedData[i] = new double[ColumnsNumber];
            }

            double[] MeanOfProperties = new double[RowsNumber];
            double[] VarianceOfProperties = new double[RowsNumber];
            bool EqualZero = false;

            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    // calculate mean
                    MeanOfProperties[i] += Data[i][j] / ColumnsNumber; ;
                }
            }

            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    // calculate variance
                    VarianceOfProperties[i] += (Math.Pow(Data[i][j] - MeanOfProperties[i], (double)2)) / ColumnsNumber;
                    if (VarianceOfProperties[i] == 0)
                    {
                        EqualZero = true;
                        break;
                    }
                }
            }
            // Normalize
            if (!EqualZero)
            {
                for (int i = 0; i < RowsNumber; i++)
                {
                    for (int j = 0; j < ColumnsNumber; j++)
                    {
                        NormalizedData[i][j] = (Data[i][j] - MeanOfProperties[i]) / Math.Sqrt(VarianceOfProperties[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < RowsNumber; i++)
                {
                    for (int j = 0; j < ColumnsNumber; j++)
                    {
                        NormalizedData[i][j] = Data[i][j] - MeanOfProperties[i];
                    }
                }
            }
            return NormalizedData;
        }

        // checks and returns result of pattern testing (1 for matching, 0 otherwise)
        private static double[] PatternTesting(double[][] rawData, int numClusters, AnomalyDetectionAPI kmeanApi, int tolerance)
        {
            CheckingSampleSettings SampleSettings;
            int clusterIndex;
            bool fitsPattern;
            double[] result = new double[rawData.Length / numClusters];
            for (int i = 0; i < rawData.Length; i = i + numClusters)
            {
                fitsPattern = true;
                // check each centroid of each function
                for (int j = 0; j < numClusters; j++)
                {
                    // check centroids
                    SampleSettings = new CheckingSampleSettings(null, rawData[i + j], tolerance: tolerance);
                    kmeanApi.CheckSample(SampleSettings, out clusterIndex);
                    // if a centroid doesn't belong to any cluster
                    if (clusterIndex == -1)
                    {
                        fitsPattern = false;
                        break;
                    }
                }
                if (fitsPattern)
                {
                    result[i / numClusters] = 1;
                }
                else
                {
                    result[i / numClusters] = 0;
                }
            }
            // contains results of pattern testing (1 for matching, 0 otherwise)
            return result;
        }

        /* 
         * Permutation functions by Ziezi with minor changes
         * https://stackoverflow.com/questions/11208446/generating-permutations-of-a-set-most-efficiently
         */
        /* Method: FindPermutations(n) */
        private static int[][] FindPermutations(int n)
        {
            int[][] PermArray = new int[factorial(n)][];
            for(int i = 0; i < PermArray.Length; i++)
            {
                PermArray[i] = new int[n];
            }
            int[] arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = i;
            }
            int iEnd = arr.Length - 1;
            Globals.PermCount = 0;
            Permute(arr, iEnd, PermArray);
            return PermArray;
        }
        /* Method: Permute(arr) */
        private static void Permute(int[] arr, int iEnd, int[][] PermArray)
        {
            if (iEnd == 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    PermArray[Globals.PermCount][i] = arr[i];
                }
                Globals.PermCount++;
                /*
                //PrintArray(arr);
                double[][] temp = new double[1][];
                temp[0] = new double[arr.Length];
                for(int i = 0; i < arr.Length; i++)
                {
                    temp[0][i] = arr[i];
                }
                Helpers.Write2CSVFile(temp, @"C:\Users\KywAnn\Desktop\temp.csv", true);
                */
                return;
            }
            Permute(arr, iEnd - 1,PermArray);
            for (int i = 0; i < iEnd; i++)
            {
                swap(ref arr[i], ref arr[iEnd]);
                Permute(arr, iEnd - 1, PermArray);
                swap(ref arr[i], ref arr[iEnd]);
            }
        }
        /* Method: PrintArray() */
        private static void PrintArray(int[] arr, string label = "")
        {
            Console.WriteLine(label);
            Console.Write("{");
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i]);
                if (i < arr.Length - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine("}");
        }
        /* Method: swap(ref int a, ref int b) */
        private static void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        // returns factorial
        static int factorial(int n)
        {
            if (n >= 2)
            {
                return n * factorial(n - 1);
            }              
            return 1;
        }
        //-----------------------------------------------------------
    }
}
