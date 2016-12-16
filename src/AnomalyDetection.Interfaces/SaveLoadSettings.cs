﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// SaveLoadSettings is a class that contains the desired settings by the user for saving or loading.
    /// </summary>
    public class SaveLoadSettings
    {
        /// <summary>
        /// the desired method for saving or loading
        /// <ul style="list-style-type:none">
        /// <li> - JSON: Used for saving to or loading from JSON files. </li>
        /// </ul>
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// a needed path for saving to or loading from a destination
        /// </summary>
        public string Path1 { get; set; }

        /// <summary>
        /// bool if true, overwriting an existing clustering instance is allowed
        /// </summary>
        public bool Replace { get; internal set; }

        /// <summary>
        /// JSON_Settings is a function that creates saving or loading settings for JSON files.
        /// </summary>
        /// <param name="Path1">path for saving to or loading from a destination</param>
        /// <param name="Replace">bool if true, overwriting an existing clustering instance is allowed</param>
        /// <param name="SaveLoadObject">the returned saved ot load JSON settings</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public static AnomalyDetectionResponse JSON_Settings(string Path1, out SaveLoadSettings SaveLoadObject, bool Replace = false)
        {
            try
            {
                SaveLoadObject = new SaveLoadSettings();
                SaveLoadObject.Method = "JSON";
                SaveLoadObject.Path1 = Path1;
                SaveLoadObject.Replace = Replace;
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                SaveLoadObject = null;
                return new AnomalyDetectionResponse(400, "Function <JSON_Settings>: Unhandled exception:\t" + Ex.ToString());
            }
        }
    }
}