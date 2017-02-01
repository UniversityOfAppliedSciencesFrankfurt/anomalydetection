
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

            var serieses = new Array();

            var fil = document.getElementById("myFile").value;

            // fileName = fil.name.replace(".json", "");
            var uri = m_SvcUri + "/ADClusteredData/" + fil;

            var pointColour = '#6f00ff';

            $scope.ADC.viewModel.noData = false;


            $.getJSON(uri, function (data) {
                if (data != null) {

                    // Populate series
                    var centroid = new Array();

                    for (m = 0; m < data.length; m++) {

                        var series = new Array();

                        for (n = 0; n < data[m].clusterData.length; n++) {
                            series.push([data[m].clusterData[n][0], data[m].clusterData[n][1]]);

                        }
                        serieses.push(series);

                        centroid.push([data[m].centroid[0], data[m].centroid[1]]);
                    }

                    serieses.push(centroid);

                    //
                    //Creating series data 
                    var nSeries = (function () {
                        var collectionSerieses = Array();
                        for (i = 0; i < serieses.length; i++) {
                            //
                            //Random color for every series
                            var color = ('#' + Math.floor(Math.random() * 16777215).toString(16));
                            if (i != serieses.length - 1) {
                                collectionSerieses.push({ 'name': i + ' Cluster', 'color': color, 'data': serieses[i] })
                            } else {
                                collectionSerieses.push({ 'name': i + ' Centroid', 'color': '#f00', 'data': serieses[i] })
                            }
                        }
                        return collectionSerieses;
                    }());

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
                        series: nSeries
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
            var serieses = new Array();
            var fil = document.getElementById("myFile3D").files[0];
            fileName = fil.name.replace(".json", "");
            var uri = m_SvcUri + "/ADClusteredData/" + fileName;
            var pointColour = '#6f00ff';
            $scope.ADC.viewModel.noData = false;
            $.getJSON(uri, function (data) {
                if (data != null) {
                    //Populate series

                    var centroid = new Array();
                    for (m = 0; m < data.length; m++) {
                        var series = new Array();

                        for (n = 0; n < data[m].clusterData.length; n++) {
                            series.push([data[m].clusterData[n][0], data[m].clusterData[n][1], data[m].clusterData[n][2]]);

                        }
                        serieses.push(series);

                        centroid.push([data[m].centroid[0], data[m].centroid[1], data[m].centroid[2]]);
                    }

                    serieses.push(centroid);

                    var nSeries = (function () {
                        var collectionSerieses = Array();
                        for (i = 0; i < serieses.length; i++) {

                            var color = ('#' + Math.floor(Math.random() * 16777215).toString(16));
                            if (i != serieses.length - 1) {
                                collectionSerieses.push({ 'name': i + ' Cluster', 'color': color, 'data': serieses[i] })
                            } else {
                                collectionSerieses.push({ 'name': i + ' Centroid', 'color': '#f00', 'data': serieses[i] })
                            }
                        }
                        return collectionSerieses;
                    }());

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
                        series: nSeries

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
                        ClusterId: i, centroid: data[i].centroid, ClusterDataDistanceToCentroid: data[i].clusterDataDistanceToCentroid,
                        ClusterDataOriginalIndex: data[i].clusterDataOriginalIndex, ClusterOfNearestForeignSample: data[i].clusterOfNearestForeignSample,
                        DistanceToNearestClusterCentroid: data[i].distanceToNearestClusterCentroid, DistanceToNearestForeignSample: data[i].distanceToNearestForeignSample,
                        DistanceToNearestForeignSampleInNearestCluster: data[i].distanceToNearestForeignSampleInNearestCluster, InClusterFarthestSample: data[i].inClusterFarthestSample,
                        InClusterFarthestSampleIndex: data[i].inClusterFarthestSampleIndex, InClusterMaxDistance: data[i].inClusterMaxDistance, Mean: data[i].mean,
                        NearestCluster: data[i].nearestCluster, NearestForeignSample: data[i].nearestForeignSample, NearestForeignSampleInNearestCluster: data[i].nearestForeignSampleInNearestCluster,
                        StandardDeviation: data[i].standardDeviation
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
            var file = document.getElementById("ImportNewDataForClustering").value;
            //var file = document.getElementById("saveDirectory").value;

            var files = file.split(',');

            var kmeansMaxIterations = $scope.ADC.viewModel.KmeansMaxIterations;
            var numberOfClusters = $scope.ADC.viewModel.NumberOfClusters;
            var numberOfAttributes = $scope.ADC.viewModel.NumberOfAttributes;
            var saveName = $scope.ADC.viewModel.SaveName;
            var replace = true;

            var data = {
                "CsvFilePaths": files,
                "SavePath": saveName,
                "numOfClusters": numberOfClusters,
                "numOfAttributes": numberOfAttributes,
                "kmeansMaxIterations": kmeansMaxIterations
            }


            var config = {
                headers: {
                    'Content-Type': 'application/json;'
                }
            }

            $http.post(m_SvcUri + "/ImportNewDataForClustering/", data, config)
                .success(function (data, status, headers, config) {
                    $scope.resultXY ="Code: "+data.code+", Message: "+data.message;
             })
             .error(function (data, status, header, config) {
                 //Error Code
             });
        };
    });
}());

