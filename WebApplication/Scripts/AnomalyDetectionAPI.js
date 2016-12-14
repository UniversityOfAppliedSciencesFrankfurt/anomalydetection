    (function () {

        var app = angular.module('anomalydet', ['ngCsvImport']);

        var m_Contacts = {};
        var m_AnomalyDetections = {};
        var m_SelectedAnomaly = {};
        var sampledataFile = {};

        app.controller('AnomalyAPIController', function ($scope, $http) {

            var m_SvcUri = "http://localhost:17742/anomalydetection";
            $scope.anchorClicks = 0;
            this.viewModel = {};
            //$scope.DataType =
            //        { MLType: "preexist" }
            $scope.onlyNumbers = /^\d+$/;

            $scope.FillinAnomalyDD = function () {
                var uri = m_SvcUri + "/Anodropdown/All";
                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.options = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };
            $scope.FillinAnomalyDD2D = function () {
                var uri = m_SvcUri + "/Anodropdown/2D";
                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.options2D = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };
            $scope.FillinAnomalyDD3D = function () {
                var uri = m_SvcUri + "/Anodropdown/3D";
                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.options3D = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };

            $scope.ShowOnGraph = function (DataTypeid) {
            
                DataTypeid = $scope.ADC.viewModel.Seloption2D;
                if (DataTypeid > 0)
                {
                    var processed_json_0 = new Array();
                    var processed_json_1 = new Array();
                    var processed_json_2 = new Array();
                    var processed_json_3 = new Array();
                    var processed_json_4 = new Array();
                    var uri = m_SvcUri + "/ADclusteredData/" + DataTypeid + "/a";
                    var pointColour = '#6f00ff';

                    $.getJSON(uri, function (data) {
                        // Populate series
                       
                            
                            for (i = 0; i < data[0].ClusterData.length; i++)
                                processed_json_0.push([data[0].ClusterData[i][0], data[0].ClusterData[i][1]]);
                            for (j = 0; j < data[1].ClusterData.length; j++)
                                processed_json_1.push([data[1].ClusterData[j][0], data[1].ClusterData[j][1]]);
                            for (k = 0; k < data[2].ClusterData.length; k++)
                                processed_json_2.push([data[2].ClusterData[k][0], data[2].ClusterData[k][1]]);

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
                                        name: 'Small',
                                        color: '#81d8d0',
                                        data: processed_json_0
                                    }, {
                                        name: 'Medium',
                                        color: '#ffff66',
                                        data: processed_json_1
                                    }, {
                                        name: 'Large',
                                        color: '#b06d40',
                                        data: processed_json_2
                                    }]
                                });
                            })
                       
                    
                }
            };
            $scope.ShowOnGraph3D = function (DataTypeid) {

                DataTypeid = $scope.ADC.viewModel.Seloption3D;
                var processed_json_0 = new Array();
                var processed_json_1 = new Array();
                var processed_json_2 = new Array();
                var processed_json_3 = new Array();
                var processed_json_4 = new Array();
                var uri = m_SvcUri + "/query/" + DataTypeid;
                var pointColour = '#6f00ff';

                $.getJSON(uri, function (data) {
                    // Populate series
                    for (i = 0; i < data.length; i++) {
                        if (data[i].Cluster_Id == 0) {
                            processed_json_0.push([data[i].Scalar_1_Value, data[i].Scalar_2_Value, data[i].Scalar_3_Value]);
                        }
                        else if 
                            (data[i].Cluster_Id == 1) {
                            processed_json_1.push([data[i].Scalar_1_Value, data[i].Scalar_2_Value, data[i].Scalar_3_Value]);
                        }
                        else if 
                            (data[i].Cluster_Id == 2) {
                            processed_json_2.push([data[i].Scalar_1_Value, data[i].Scalar_2_Value, data[i].Scalar_3_Value]);
                        }
                    }
                    var centroiduri = m_SvcUri + "/ClusterDetail/5/3";
                    $.getJSON(centroiduri, function (centroiddata) {
                        for (i = 0; i < centroiddata.length; i++) {

                            processed_json_3.push([centroiddata[i].Centeroid_Scalar_1_Value, centroiddata[i].Centeroid_Scalar_2_Value, centroiddata[i].Centeroid_Scalar_3_Value]);

                        }
                        var outlieruri = m_SvcUri + "/OutliersDetail/5/3";
                        $.getJSON(outlieruri, function (outlierdata) {
                            for (i = 0; i < outlierdata.length; i++) {

                                processed_json_4.push([outlierdata[i].Scalar_1_Value, outlierdata[i].Scalar_2_Value, outlierdata[i].Scalar_3_Value]);

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
                                        fitToPlot: false,
                                        frame: {
                                            bottom: { size: 1, color: 'rgba(0,0,0,0.02)' },
                                            back: { size: 1, color: 'rgba(0,0,0,0.04)' },
                                            side: { size: 1, color: 'rgba(0,0,0,0.06)' }
                                        }
                                    }
                                },
                                title: {
                                    text: 'Growth chart: Child till 1 year'
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
                                    min: 0,
                                    max:15,
                                    title: "Length"
                                },
                                xAxis: {
                                    min: 0,
                                    max: 15,
                                    gridLineWidth: 1,
                                    title: "Age"
                                },
                                zAxis: {
                                    min: 0,
                                    max: 100,
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
                                    name: 'young',
                                    color: '#81d8d0',
                                    data: processed_json_0
                                }, {
                                    name: 'medium',
                                    color: '#ffff66',
                                    data: processed_json_1
                                }, {
                                    name: 'New Born',
                                    color: '#b06d40',
                                    data: processed_json_2
                                }, {
                                    name: 'Centeroid',
                                    color: '#6f00ff',
                                    data: processed_json_3
                                }, {
                                    name: 'Outlier',
                                    color: '#a81d08',
                                    data: processed_json_4
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
                        })
                    })
                });
            };

            $scope.uploadData = function () {
                $scope.anchorClicks++;
            };
            this.createOrUpdate = function () {

                var uri = m_SvcUri + "/post";

                $http.post(uri, $scope.ADC.viewModel.selectedAnomaly)
                .success(function (data) {
                    $scope.ADC.selectedAnomaly = data[0];
                    $scope.ADC.viewModel.anomalydetections = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };

            $scope.queryClusterDetail = function () {
                DataTypeid = $scope.ADC.viewModel.Seloption;
                var uri = m_SvcUri + "/ClusterDetail/" + DataTypeid + "/3";

                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.ClusterDetailtable = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };
            $scope.queryOutlierDetail = function () {
                DataTypeid = $scope.ADC.viewModel.Seloption;
                var uri = m_SvcUri + "/OutliersDetail/" + DataTypeid + "/3";

                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.OutlierDetailtable = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };
            $scope.PositionInExistingClustering = function (csvresult) {
                DataTypeid = $scope.ADC.viewModel.Seloption4;
                var uri = m_SvcUri + "/checkSample/" + DataTypeid + "/3/" + csvresult

                $http.get(uri)
                .success(function (data) {
                    $scope.ADC.viewModel.CheckSampletable = data;
                })
                .error(function (data, status, headers, config) {
                    alert(err);
                });
            };

      
            $scope.DimensionSelection = function () {
                $scope.Scalar1 = "";
                $scope.Scalar2 = "";
                $scope.Scalar3 = "";
                if (($scope.ADC.viewModel.Seloption3 != null) && ($scope.ADC.viewModel.Seloption3 > 0)) {
                    DataTypeid = $scope.ADC.viewModel.Seloption3;
                    var datypeuri = m_SvcUri + "/getDataTypeDetail/" + DataTypeid;

                    $http.get(datypeuri)
                    .success(function (data) {
                        $scope.ADC.viewModel.Dimension = data[0].Dimension;
                    })
                    .error(function (data, status, headers, config) {
                        alert(err);
                    });
                }

            };
            $scope.FillinAnomalyDD();
            $scope.FillinAnomalyDD2D();
            $scope.FillinAnomalyDD3D();

        });
    }());