
(function () {

    var app = angular.module('anodetapp', ['ngCsvImport']);

    var m_Contacts = {};
    var m_AnomalyDetections = {};
    var m_SelectedAnomaly = {};
    /*
         The below code defines the controller name on the Rest API
     */

    app.controller('AnomalyAPIController', function ($scope, $http) {

        var m_SvcUri = "http://localhost:5436/api/anomalydetection";
        $scope.anchorClicks = 0;
        this.viewModel = {};
        $scope.onlyNumbers = /^\d+$/;
        $scope.onlyNumbersandDecimals = /^[1-9][0-9]{0,2}(?:,?[0-9]{3}){0,3}(?:\.[0-9]{1,2})?$/;
        //$scope.onlydecimalnumbers = /-?[0-9]+(\.[0-9]+)?/;

        /// <summary>The below code is for displaying the clusters on a 2D High chart graph</summary>
        $scope.ShowOnGraph = function () {
            //DataTypeid = $scope.ADC.viewModel.Seloption2D;
            var processed_json_0 = new Array();
            var processed_json_1 = new Array();
            var processed_json_2 = new Array();
            var processed_json_3 = new Array();
            var processed_json_4 = new Array();
            var processed_json_5 = new Array();
            var fil = document.getElementById("myFile").files[0];;

            fileName = fil.name.replace(".json", "");
            var uri = m_SvcUri + "/ADClusteredData/" + fileName;

            var pointColour = '#6f00ff';

            $scope.ADC.viewModel.noData = false;

            $.getJSON(uri, function (data) {
                if (data != null) {
                    // Populate series
                    for (i = 0; i < data[0].clusterData.length; i++)
                        processed_json_0.push([data[0].clusterData[i][0], data[0].clusterData[i][1]]);
                    processed_json_3.push([data[0].centroid[0], data[0].centroid[1]]);
                    for (j = 0; j < data[1].clusterData.length; j++)
                        processed_json_1.push([data[1].clusterData[j][0], data[1].clusterData[j][1]]);
                    processed_json_3.push([data[1].centroid[0], data[1].centroid[1]]);
                    if (data.length > 2) {
                        for (k = 0; k < data[2].clusterData.length; k++)
                            processed_json_2.push([data[2].clusterData[k][0], data[2].clusterData[k][1]]);
                        processed_json_3.push([data[2].centroid[0], data[2].centroid[1]]);
                        if (data.length > 3) {
                            for (s = 0; s < data[3].clusterData.length; s++)
                                processed_json_5.push([data[3].clusterData[s][0], data[3].clusterData[s][1]]);
                            processed_json_3.push([data[3].centroid[0], data[3].centroid[1]]);
                        }
                    }
                    // draw chart
                    $('#container').highcharts({
                        chart: {
                            type: 'scatter',
                            zoomType: 'xy'
                        },
                        title: {
                            text: 'Height Versus Weight by Frame'
                        },
                        subtitle: {
                            text: 'Source:Random'
                        },
                        xAxis: {
                            title: {
                                enabled: true,
                                text: 'Height (cm)'
                            },
                            startOnTick: true,
                            endOnTick: true,
                            showLastLabel: true
                        },
                        yAxis: {
                            title: {
                                text: 'Weight (kg)'
                            }
                        },
                        legend: {
                            layout: 'vertical',
                            align: 'right',
                            verticalAlign: 'top',
                            floating: true,
                            backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF',
                            borderWidth: 1
                        },
                        plotOptions: {
                            scatter: {
                                marker: {
                                    radius: 5,
                                    states: {
                                        hover: {
                                            enabled: true,
                                            lineColor: 'rgb(100,100,100)'
                                        }
                                    }
                                },
                                states: {
                                    hover: {
                                        marker: {
                                            enabled: false
                                        }
                                    }
                                },
                                tooltip: {
                                    headerFormat: '<b>{series.name}</b><br>',
                                    pointFormat: '{point.x} cm, {point.y} kg'
                                }
                            }
                        },
                        series: [{
                            name: '1stCluster',
                            color: '#81d8d0',
                            data: processed_json_0
                        }, {
                            name: '2ndCluster',
                            color: '#ff5bc7',
                            data: processed_json_1
                        }, {
                            name: '3rdCluster',
                            color: '#b06d40',
                            data: processed_json_2
                        }, {
                            name: '4thCluster',
                            color: '#76EE00',
                            data: processed_json_5
                        },
                        {
                            name: 'Centeroid',
                            color: '#6f00ff',
                            data: processed_json_3
                        }
                        ]
                    });
                }
                else {
                    $('#container').highcharts({
                        chart: {
                            type: 'scatter',
                            zoomType: 'xy'
                        },
                        title: {
                            text: 'No Data To Display'
                        }

                    });

                }
            });
        };

        /// <summary> The below code is for displaying the clusters on a 3D High chart graph</summary>
        $scope.ShowOnGraph3D = function () {

            //DataTypeid = $scope.ADC.viewModel.Seloption3D;
            var processed_json_0 = new Array();
            var processed_json_1 = new Array();
            var processed_json_2 = new Array();
            var processed_json_3 = new Array();
            var processed_json_4 = new Array();
            var fil = document.getElementById("myFile3D").files[0];
            fileName = fil.name.replace(".json", "");
            var uri = m_SvcUri + "/ADClusteredData/"+ fileName;
            var pointColour = '#6f00ff';
            $scope.ADC.viewModel.noData = false;
            $.getJSON(uri, function (data) {
                if (data != null) {
                    // Populate series
                    for (i = 0; i < data[0].clusterData.length; i++)
                        processed_json_0.push([data[0].clusterData[i][0], data[0].clusterData[i][1], data[0].clusterData[i][2]]);
                    processed_json_3.push([data[0].centroid[0], data[0].centroid[1], data[0].centroid[2]]);
                    for (j = 0; j < data[1].clusterData.length; j++)
                        processed_json_1.push([data[1].clusterData[j][0], data[1].clusterData[j][1], data[1].clusterData[j][2]]);
                    processed_json_3.push([data[1].centroid[0], data[1].centroid[1], data[1].centroid[2]]);
                    if (data.length > 2) {
                        for (k = 0; k < data[2].clusterData.length; k++)
                            processed_json_2.push([data[2].clusterData[k][0], data[2].clusterData[k][1], data[2].clusterData[k][2]]);
                        processed_json_3.push([data[2].centroid[0], data[2].centroid[1], data[2].centroid[2]]);
                    }

                    Highcharts.getOptions().colors = $.map(Highcharts.getOptions().colors, function (color) {
                        return {
                            radialGradient: {
                                cx: 0.4,
                                cy: 0.3,
                                r: 0.5
                            },
                            stops: [
                                [0, color],
                                [1, Highcharts.Color(color).brighten(-0.2).get('rgb')]
                            ]
                        };
                    });

                    // Set up the chart
                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'Chartcontainer3D',
                            margin: 100,
                            type: 'scatter',
                            options3d: {
                                enabled: true,
                                alpha: 10,
                                beta: 30,
                                depth: 250,
                                viewDistance: 5,
                                fitToPlot: true,
                                frame: {
                                    bottom: { size: 1, color: 'rgba(0,0,0,0.02)' },
                                    back: { size: 1, color: 'rgba(0,0,0,0.04)' },
                                    side: { size: 1, color: 'rgba(0,0,0,0.06)' }
                                }
                            }
                        },
                        title: {
                            text: 'Growth chart: Fetal length and weight, week by week'
                        },
                        subtitle: {
                            text: 'Click and drag the plot area to rotate in space'
                        },
                        plotOptions: {
                            scatter: {
                                width: 10,
                                height: 10,
                                depth: 10
                            }
                        },
                        yAxis: {

                            title: "Length"
                        },
                        xAxis: {

                            gridLineWidth: 1,
                            title: "Age"
                        },
                        zAxis: {

                            showFirstLabel: false,
                            title: "Weight"
                        },
                        legend: {
                            layout: 'vertical',
                            align: 'right',
                            verticalAlign: 'top',
                            floating: true,
                            backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF',
                            borderWidth: 1
                        },
                        tooltip: {
                            headerFormat: '<b>{series.name}</b><br>',
                            pointFormat: '{point.x} cm, {point.y} kg'
                        },
                        series: [{
                            name: '1stcluster',
                            color: '#81d8d0',
                            data: processed_json_0
                        }, {
                            name: '2ndCluster',
                            color: '#ff5bc7',
                            data: processed_json_1
                        }, {
                            name: '3rdCluster',
                            color: '#b06d40',
                            data: processed_json_2
                        }, {
                            name: 'Centeroid',
                            color: '#6f00ff',
                            data: processed_json_3
                        }]
                    });


                    // Add mouse events for rotation
                    $(chart.container).bind('mousedown.hc touchstart.hc', function (eStart) {
                        eStart = chart.pointer.normalize(eStart);

                        var posX = eStart.pageX,
                            posY = eStart.pageY,
                            alpha = chart.options.chart.options3d.alpha,
                            beta = chart.options.chart.options3d.beta,
                            newAlpha,
                            newBeta,
                            sensitivity = 5; // lower is more sensitive

                        $(document).bind({
                            'mousemove.hc touchdrag.hc': function (e) {
                                // Run beta
                                newBeta = beta + (posX - e.pageX) / sensitivity;
                                chart.options.chart.options3d.beta = newBeta;

                                // Run alpha
                                newAlpha = alpha + (e.pageY - posY) / sensitivity;
                                chart.options.chart.options3d.alpha = newAlpha;

                                chart.redraw(false);
                            },
                            'mouseup touchend': function () {
                                $(document).unbind('.hc');
                            }
                        });
                    });
                }
                else {
                    $('#Chartcontainer3D').highcharts({
                        chart: {

                        },
                        title: {
                            text: 'No Data To Display'
                        }

                    });
                }
            });
        };

        /// <summary> This for displaying the cluster detail in a Tabular format</summary>
        $scope.queryClusterDetail = function () {
            var cluster_detail = new Array();
            var clusters = new Array();
           // DataTypeid = $scope.ADC.viewModel.Seloption;
            var fil = document.getElementById("FileClusterDet").files[0];
            fileName = fil.name.replace(".json", "");
            uri = m_SvcUri + "/ADClusteredData/" + fileName;
            $http.get(uri)
            .success(function (data) {
                for (i = 0; i < data.length; i++) {
                    clusters.push({ ClusterId: i });
                    cluster_detail.push({
                        ClusterId: i, centroid: data[i].Centroid, ClusterDataDistanceToCentroid: data[i].ClusterDataDistanceToCentroid,
                        ClusterDataOriginalIndex: data[i].ClusterDataOriginalIndex, ClusterOfNearestForeignSample: data[i].ClusterOfNearestForeignSample,
                        DistanceToNearestClusterCentroid: data[i].DistanceToNearestClusterCentroid, DistanceToNearestForeignSample: data[i].DistanceToNearestForeignSample,
                        DistanceToNearestForeignSampleInNearestCluster: data[i].DistanceToNearestForeignSampleInNearestCluster, InClusterFarthestSample: data[i].InClusterFarthestSample,
                        InClusterFarthestSampleIndex: data[i].InClusterFarthestSampleIndex, InClusterMaxDistance: data[i].InClusterMaxDistance, Mean: data[i].Mean,
                        NearestCluster: data[i].NearestCluster, NearestForeignSample: data[i].NearestForeignSample, NearestForeignSampleInNearestCluster: data[i].NearestForeignSampleInNearestCluster,
                        StandardDeviation: data[i].StandardDeviation
                    });

                }



                $scope.ADC.viewModel.SelectedCluster = cluster_detail[0];
                $scope.ADC.viewModel.ClusterDetailtable = cluster_detail;
                $scope.ADC.viewModel.MoreDetail = true;
            })
            .error(function (data, status, headers, config) {
                alert(data.Message);
            });
        };

        /// <summary> This for displaying additional cluster detail in a Tabular format</summary>
        this.moreClusterDetail = function (Id) {
            $scope.ADC.viewModel.MoreDetail = true;
            $scope.ADC.viewModel.SelectedCluster = $scope.ADC.viewModel.ClusterDetailtable[Id];
        };

        /// <summary> This for checking the sample position in a cluster or an outlier check</summary>
        $scope.CheckSample = function () {
            DataTypeid = $scope.ADC.viewModel.Seloption4;
            Clusterdimension = $scope.ADC.viewModel.MachineLClusterdimension;
            var Clusterx = $scope.ADC.viewModel.MachineLClusterx;
            var Clustery = $scope.ADC.viewModel.MachineLClustery;
            var Clusterz = $scope.ADC.viewModel.MachineLClusterz;
            var ClusterTol = $scope.ADC.viewModel.ClusterTol;
            if (ClusterTol == undefined)
                ClusterTol = 10;
            var sample_detail;

            var fil = document.getElementById("SampleClusterCheck").files[0];
            fileName = fil.name.replace(".json", "");
            var uri;
            if ((Clustery == undefined || Clustery == "") && (Clusterz == undefined || Clusterz == ""))
                uri = m_SvcUri + "/GetClusId/" + fileName + "/" + Clusterx + "/" + null + "/" + null + "/" + ClusterTol + "/";
            else if ((Clusterz == undefined || Clusterz == "") && Clustery != null && Clusterx != null)
                uri = m_SvcUri + "/GetClusId/" + fileName + "/" + Clusterx + "/" + Clustery + "/" + null + "/" + ClusterTol + "/";
            else
                uri = m_SvcUri + "/GetClusId/" + fileName + "/" + Clusterx + "/" + Clustery + "/" + Clusterz + "/" + ClusterTol + "/";
            $scope.ADC.viewModel.SampleCluster = "Get the the cluster for the Sample";
            $http.get(uri)
            .success(function (data) {
                $scope.ADC.SampleClusterId = data;

                $scope.ADC.viewModel.SampleCluster = data.Message;

            })
            .error(function (data, status, headers, config) {
                alert(data.Message);
            });
        };
        /// <summary> This for importing new data for clustering</summary>
        $scope.ImportNewData = function () {
            var fil1 = document.getElementById("ImportNewDataForClustering").files[0];
            fileName = fil1.name.replace(".csv", "");
            var kmeansMaxIterations = $scope.ADC.viewModel.KmeansMaxIterations;
            var numberOfClusters = $scope.ADC.viewModel.NumberOfClusters;
            var numberOfAttributes = $scope.ADC.viewModel.NumberOfAttributes;
            var saveName = $scope.ADC.viewModel.SaveName;
            var replace = true;
            //if (document.getElementById("ImportexistLoadPath").files.length > 0) {
            //    var fil3 = document.getElementById("ImportexistLoadPath").files[0];
            //    ImploadPath = fil3.name.replace(".json", "");
            //    var uri = m_SvcUri + "/importNewDataForClustering/" + fileName + "/" + saveName + "/" + loadPath + "/" + numberOfClusters + "/" + numberOfAttributes + "/" + kmeansMaxIterations;

            //}
            //else
            {
                var uri = m_SvcUri + "/ImportNewDataForClustering/" + fileName + "/" + saveName + "/NewData/" + numberOfClusters + "/" + numberOfAttributes + "/" + kmeansMaxIterations;
            }

            $http.get(uri)
           .success(function (data) {
               $scope.ADC.viewModel.ImportMsg = data.Message;

           })
           .error(function (data, status, headers, config) {
               alert(data.Message);
           });

        };
    });
}());

