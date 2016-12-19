using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetectionRestApi
{
    public class AnomalyDet01_DataSet_Type
    {
        public int Data_Id { get; set; }

        public string DataSet_Name { get; set; }

        public System.Nullable<int> DataSet_Scalar_1 { get; set; }

        public System.Nullable<int> DataSet_Scalar_2 { get; set; }

        public System.Nullable<int> DataSet_Scalar_3 { get; set; }

        public System.Nullable<decimal> Max_Threshhold_Distance { get; set; }

        public string Description { get; set; }

        public string DataSource { get; set; }

        public System.Nullable<int> Dimension { get; set; }
    }
}
