using AnomalyDetection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetectionApi
{
    /// <summary>
    /// ISaveLoad is the defined interface for saving and loading methods.
    /// </summary>
    public interface ISaveLoad
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
        AnomalyDetectionResponse Save(SaveLoadSettings SaveObject, AnomalyDetectionAPI Instance);

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
        AnomalyDetectionResponse Save(SaveLoadSettings SaveObject, ClusteringResults[] Results);

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
        AnomalyDetectionResponse validateSaveConditions(SaveLoadSettings SaveObject, out SaveLoadSettings CheckedSaveObject);

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
        Tuple<AnomalyDetectionAPI, AnomalyDetectionResponse> Load_AnomalyDetectionAPI(SaveLoadSettings LoadObject);

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
        Tuple<ClusteringResults[], AnomalyDetectionResponse> Load_ClusteringResults(SaveLoadSettings LoadObject);

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
        AnomalyDetectionResponse LoadChecks(SaveLoadSettings LoadObject, out SaveLoadSettings CheckedLoadObject);
    }
}