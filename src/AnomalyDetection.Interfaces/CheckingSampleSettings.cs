using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// CheckingSampleSettings is a class that contains the desired settings by the user for checking to which cluster a sample belongs
    /// </summary>
    public class CheckingSampleSettings
    {
        /// <summary>
        /// settings to the clustering intance that contains the clusters data
        /// </summary>
        public SaveLoadSettings LoadProjectSettings { get; internal set; }

        /// <summary>
        /// the sample to be checked
        /// </summary>
        public double[] Sample { get; internal set; }

        /// <summary>
        /// a value in % representing the tolerance to possible outliers
        /// </summary>
        public double tolerance { get; internal set; }

        /// <summary>
        /// Constructor to create the desired settings by the user for checking to which cluster a sample belongs
        /// </summary>
        /// <param name="LoadProjectSettings">settings to the clustering intance that contains the clusters data</param>
        /// <param name="Sample">the sample to be checked</param>
        /// <param name="tolerance">a value in % representing the tolerance to possible outliers</param>
        public CheckingSampleSettings(SaveLoadSettings LoadProjectSettings, double[] Sample, double tolerance = 0)
        {
            this.LoadProjectSettings = LoadProjectSettings;
            this.Sample = Sample;
            this.tolerance = tolerance;
        }
    }
}
