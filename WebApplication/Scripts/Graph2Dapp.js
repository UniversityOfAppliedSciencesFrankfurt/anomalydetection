(function () {
    var My2DData = [];
    var app = angular.module('anomalydetection2', []);

    var m_Contacts = {};
    var m_AnomalyDetections = {};
    var m_SelectedAnomaly = {};
    var ChartData = [];
   
    app.controller('AnomalyDetectionController', function ($scope, $http) {

        var m_SvcUri = "http://localhost:17742/api/anomalydetection";

        this.viewModel = {};

        this.queryAnomalyDetections = function () {

            var uri = m_SvcUri + "/query/all";
            
            $http.get(uri)
            .success(function (apidata) {
                $scope.AD.viewModel.anomalydetections2 = apidata;
                ChartData += "[";
                for (i = 0; i < apidata.length; i++) {
                    ChartData += "[";
                    ChartData += apidata[i].Scalar_1_Value;
                    ChartData += ",";
                    ChartData += apidata[i].Scalar_2_Value;
                    ChartData += "]";
                }
                ChartData += "]";
                $scope.basicAreaChart = ChartData;
                $scope.Ano2DData = apidata;
            })
            .error(function (data, status, headers, config) {
                alert(err);
            });
        };


        this.queryAnomalyDetections();

    });
}());