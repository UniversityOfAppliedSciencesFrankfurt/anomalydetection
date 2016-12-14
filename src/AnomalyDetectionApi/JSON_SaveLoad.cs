using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnomalyDetection.Interfaces;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace AnomalyDetectionApi
{
    /// <summary>
    /// JSON_SaveLoad is class that saves to or loads from JSON files.
    /// </summary>
    public class JSON_SaveLoad : ISaveLoad
    {
        /// <summary>
        /// Save is a function that saves an instance of AnomalyDetectionAPI.
        /// </summary>
        /// <param name="SaveObject">settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="Instance">AnomalyDetectionAPI object to be saved</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Save(SaveLoadSettings SaveObject, AnomalyDetectionAPI Instance)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SaveObject.Path1));
                FileStream fs = new FileStream(SaveObject.Path1, FileMode.Create);
                DataContractJsonSerializer JSONSerializer = new DataContractJsonSerializer(typeof(AnomalyDetectionAPI));
                JSONSerializer.WriteObject(fs, Instance);
                fs.Dispose();
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <Save -JSON- (AnomalyDetecionAPI)>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// Save is a function that saves the clustering results.
        /// </summary>
        /// <param name="SaveObject">settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="Results">the  clustering results object to be saved</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Save(SaveLoadSettings SaveObject, ClusteringResults[] Results)
        {
            try
            {
                //save the clustering results (in a folder called Result in the desired save path)
                string ResultPath = Path.GetDirectoryName(SaveObject.Path1).ToString() + "\\Result\\" + Path.GetFileNameWithoutExtension(SaveObject.Path1).ToString() + ".json";
                Directory.CreateDirectory(Path.GetDirectoryName(ResultPath));
                FileStream fs = new FileStream(ResultPath, FileMode.Create);
                DataContractJsonSerializer JSONSerializer = new DataContractJsonSerializer(typeof(ClusteringResults[]));
                JSONSerializer.WriteObject(fs, Results);
                fs.Dispose();
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <Save -JSON- (ClusteringResults[])>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// SaveChecks is a function that checks the saving settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="SaveObject">settings to save</param>
        /// <param name="CheckedSaveObject">the checked settings to save</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse SaveChecks(SaveLoadSettings SaveObject, out SaveLoadSettings CheckedSaveObject)
        {
            string OutputPath;
            AnomalyDetectionResponse ADResponse = CheckPath(SaveObject.Path1, out OutputPath);
            if (ADResponse.Code == 0)
            {
                if (File.Exists(OutputPath))
                {
                    if ((!SaveObject.Replace))
                    {
                        CheckedSaveObject = null;
                        return new AnomalyDetectionResponse(201, "Function <SaveChecks  -JSON- >: File already exists");
                    }
                    FileAttributes Attributes = File.GetAttributes(OutputPath);
                    if ((Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        CheckedSaveObject = null;
                        return new AnomalyDetectionResponse(204, "Function <SaveChecks  -JSON- >: Unauthorized access to file. File is readonly.");
                    }
                    if ((Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        CheckedSaveObject = null;
                        return new AnomalyDetectionResponse(205, "Function <SaveChecks  -JSON- >: Unauthorized access to file. File is a system file.");
                    }
                }
                ADResponse = SaveLoadSettings.JSON_Settings(OutputPath, out CheckedSaveObject, SaveObject.Replace);
                if (ADResponse.Code != 0)
                {
                    CheckedSaveObject = null;
                    return ADResponse;
                }
                return new AnomalyDetectionResponse(0, "OK");
            }
            else
            {
                CheckedSaveObject = null;
                return ADResponse;
            }

        }

        /// <summary>
        /// LoadJSON_AnomalyDetectionAPI is a function that deserializes and loads an AnomalyDetectionAPI object from a JSON file.
        /// </summary>
        /// <param name="LoadObject">settings to load the AnomalyDetectionAPI instance</param>
        /// <returns>
        /// - Item 1: the loaded AnomalyDetectionAPI object <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        //check for another way to keep centroid private
        public Tuple<AnomalyDetectionAPI, AnomalyDetectionResponse> Load_AnomalyDetectionAPI(SaveLoadSettings LoadObject)
        {
            AnomalyDetectionAPI Obj;
            try
            {
                FileStream fs = new FileStream(LoadObject.Path1, FileMode.Open);
                DataContractJsonSerializer JSONSerializer = new DataContractJsonSerializer(typeof(AnomalyDetectionAPI));
                Obj = (AnomalyDetectionAPI)JSONSerializer.ReadObject(fs);
                fs.Dispose();
                if (Obj.Centroids == null)
                {
                    Obj = null;
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(206, "Function <Load_AnomalyDetectionAPI -JSON- >: Can't deserialize file"));
                }
                return Tuple.Create(Obj, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                Obj = null;
                if (Ex is System.IO.FileNotFoundException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(200, "Function<Load_AnomalyDetectionAPI -JSON- >: File not found"));
                }
                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(204, "Function<Load_AnomalyDetectionAPI -JSON- >: Directory not found"));
                }
                if (Ex is FileLoadException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(202, "Function<Load_AnomalyDetectionAPI -JSON- >: File cannot be loaded"));
                }
                if (Ex is SerializationException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(203, "Function<Load_AnomalyDetectionAPI -JSON- >: File content is corrupted"));
                }
                return Tuple.Create(Obj, new AnomalyDetectionResponse(400, "Function<Load_AnomalyDetectionAPI -JSON- >: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// LoadJSON_ClusteringResults is a function that deserializes and loads a ClusteringResults[] object from a JSON file.
        /// </summary>
        /// <param name="LoadObject">settings to load the ClusteringResults[] object</param>
        /// <returns>
        /// - Item 1: the loaded ClusteringResults[] object <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public Tuple<ClusteringResults[], AnomalyDetectionResponse> Load_ClusteringResults(SaveLoadSettings LoadObject)
        {
            ClusteringResults[] Obj;
            try
            {
                string ResultPath = Path.GetDirectoryName(LoadObject.Path1).ToString() + "\\Result\\" + Path.GetFileNameWithoutExtension(LoadObject.Path1).ToString() + ".json";
                FileStream fs = new FileStream(ResultPath, FileMode.Open);
                DataContractJsonSerializer JSONSerializer = new DataContractJsonSerializer(typeof(ClusteringResults[]));
                Obj = (ClusteringResults[])JSONSerializer.ReadObject(fs);
                fs.Dispose();
                if (Obj.Length == 0)
                {
                    Obj = null;
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(206, "Function <Load_ClusteringResults -JSON- >: Can't deserialize file"));
                }
                return Tuple.Create(Obj, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                Obj = null;
                if (Ex is System.IO.FileNotFoundException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(200, "Function <Load_ClusteringResults -JSON- >: File not found"));
                }
                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(204, "Function<Load_ClusteringResults -JSON- >: Directory not found"));
                }
                if (Ex is FileLoadException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(202, "Function <Load_ClusteringResults -JSON- >: File cannot be loaded"));
                }
                if (Ex is SerializationException)
                {
                    return Tuple.Create(Obj, new AnomalyDetectionResponse(203, "Function <Load_ClusteringResults -JSON- >: File content is corrupted"));
                }
                return Tuple.Create(Obj, new AnomalyDetectionResponse(400, "Function <Load_ClusteringResults -JSON- >: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// LoadChecks is a function that checks the load settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="LoadObject">settings to load</param>
        /// <param name="CheckedLoadObject">the checked settings to load</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse LoadChecks(SaveLoadSettings LoadObject, out SaveLoadSettings CheckedLoadObject)
        {
            string OutputPath;
            AnomalyDetectionResponse ADResponse = CheckPath(LoadObject.Path1, out OutputPath);
            if (ADResponse.Code == 0)
            {
                if (!File.Exists(OutputPath))
                {
                    CheckedLoadObject = null;
                    if (!Directory.Exists(Path.GetDirectoryName(OutputPath)))
                    {
                        return new AnomalyDetectionResponse(121, "Function <LoadChecks -JSON- >: Requested load path does not exist");
                    }
                    return new AnomalyDetectionResponse(200, "Function <LoadChecks -JSON- >: File not found");
                }
                ADResponse = SaveLoadSettings.JSON_Settings(OutputPath, out CheckedLoadObject, LoadObject.Replace);
                if (ADResponse.Code != 0)
                {
                    CheckedLoadObject = null;
                    return ADResponse;
                }
                return new AnomalyDetectionResponse(0, "OK");
            }
            else
            {
                CheckedLoadObject = null;
                return ADResponse;
            }
        }

        /// <summary>
        /// CheckPath is a function that checks the input path for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="InputPath">path to be checked</param>
        /// <param name="OutputPath">the variable through which the error-less path is returned</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse CheckPath(string InputPath, out string OutputPath)
        {
            try
            {
                OutputPath = null;

                if (!Path.IsPathRooted(InputPath))
                {
                    return new AnomalyDetectionResponse(116, "Function<CheckPath>: Path provided : \"" + InputPath + "\" has no root");
                }

                //check directory issues
                char[] InvalidChars;
                string DirectoryPath = Path.GetDirectoryName(InputPath);

                if (DirectoryPath != null)
                {
                    InvalidChars = Path.GetInvalidPathChars();
                    for (int i = 0; i < InvalidChars.Length; i++)
                    {
                        if (DirectoryPath.Contains(InvalidChars[i]))
                        {
                            return new AnomalyDetectionResponse(117, "Function <CheckPath>: Path provided : \"" + InputPath + "\" contains invalid chars. First invalid char encountered is: \'" + InvalidChars[i] + "\'.");
                        }
                    }
                }

                InvalidChars = Path.GetInvalidFileNameChars();
                string SaveName = Path.GetFileNameWithoutExtension(InputPath);
                if (SaveName == "")
                {
                    return new AnomalyDetectionResponse(118, "Function <CheckPath>: Path provided : \"" + InputPath + "\" has no project name specified.");
                }
                for (int i = 0; i < InvalidChars.Length; i++)
                {
                    if (SaveName.Contains(InvalidChars[i]))
                    {
                        return new AnomalyDetectionResponse(119, "Function <CheckPath>: Path provided : \"" + InputPath + "\" has a project name containing invalid chars. First invalid char encountered is: \'" + InvalidChars[i] + "\'.");
                    }
                }

                if (Path.HasExtension(InputPath))
                {
                    if (!Path.GetExtension(InputPath).ToLower().Equals(".json"))
                    {
                        return new AnomalyDetectionResponse(120, "Function <CheckPath>: Path provided : \"" + InputPath + "\" has wrong extension.");
                    }
                }

                string RootPath = Path.GetPathRoot(InputPath);
                if (!RootPath.Contains("\\"))
                {
                    DirectoryPath = DirectoryPath.TrimStart(RootPath.ToCharArray());
                    DirectoryPath = RootPath + "\\" + DirectoryPath;
                }
                OutputPath = DirectoryPath + "\\" + SaveName + ".json";
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                OutputPath = null;
                return new AnomalyDetectionResponse(400, "Function <CheckPath>: Unhandled exception:\t" + Ex.ToString());
            }
        }
    }
}
