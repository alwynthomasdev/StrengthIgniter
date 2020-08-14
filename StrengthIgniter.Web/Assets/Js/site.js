// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var createDelayedEventHandler = function (callback, ms) {
    var timer = 0;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = setTimeout(function () {
            callback.apply(context, args);
        }, ms || 0);
    };
};

var supressEnterKeyPressEvent = function (e) {
    if (e.keyCode === 10 || e.keyCode === 13) {
        e.preventDefault();
    }
};

function Paginator(options) {
    this.$table = options.table;
    this.url = options.url;
    this.pageLength = options.pageLength;
    this.filters = options.filters;
    this.pageNumber = 1;
    this.itemCount = 0;

    this.sendRequest = function (filters) {
        if (filters) {
            $this.filters = filters;
        }

        var data = $this.filters;
        data.pageNumber = $this.pageNumber;
        data.pageLength = $this.pageLength;

        if (showLoadingMessage) {
            var $tbody = $this.$table.find('tbody');
            $tbody.html(showLoadingMessage());
        }

        $.ajax({
            url: $this.url,
            method: 'GET',
            data: data
        })
            .done(handleResponse)
            .fail(failedRequestHandler);
    };

    var $this = this;

    var rowGenerator = options.rowGenerator;
    var showLoadingMessage = options.showLoadingMessage;

    var failedRequestHandler;
    if (options.failedRequestHandler !== undefined) {
        failedRequestHandler = options.failedRequestHandler;
    } else {
        failedRequestHandler = function (args) {
            console.log('Error occurred when sending a request to the server using the paginator.');
            console.log(args);
        };
    }

    if (!$this.filters) {
        $this.filters = {};
    }

    var init = function () {
        $this.sendRequest();
    };

    var handleResponse = function (data) {
        if (data) {
            var $body = $this.$table.find('tbody');

            if (data.items) {
                $body.html('');
                $.each(data.items, function (idx, item) {
                    var $tr = rowGenerator(item);
                    var $body = $this.$table.find('tbody');
                    $body.append($tr);
                });
            }

            var $paginationContainer = $this.$table.find('.pagination-container');
            $paginationContainer.html('');

            if (data.total > 0) {
                $this.itemCount = data.total;
                createPaginationButtons($paginationContainer);
            }

            //TODO: no records found display
        }
    };

    var createPaginationButtons = function ($paginationContainer) {

        var pageCount = 1;

        if ($this.itemCount > 0) {
            pageCount = Math.ceil($this.itemCount / $this.pageLength);
        }

        if (pageCount > 1) {
            var $paginatorHtml = $('<ul class="pagination"></ul>');

            var rangeStart = 0;
            var rangeEnd = pageCount;
            if (pageCount > 11) {
                rangeStart = $this.pageNumber - 6;
                rangeEnd = $this.pageNumber + 5;
                if (rangeStart < 0) {
                    rangeEnd = Math.abs(rangeStart) + rangeEnd;
                    rangeStart = 0;
                }
                if (rangeEnd > pageCount) {
                    rangeStart = rangeStart - (rangeEnd - pageCount);
                    rangeEnd = pageCount;
                }
            }

            if (pageCount > 11) {
                var $start = $('<li class="page-item"></li>');
                if ($this.pageNumber === 1) {
                    $start.addClass('disabled');
                }
                $start.html('<a class="page-link" data-page="1" href="#"><i class="fa fa-angle-double-left"></i></a>');
                $paginatorHtml.append($start);
            }

            var $prev = $('<li class="page-item"></li>');
            if ($this.pageNumber === 1) {
                $prev.addClass('disabled');
            }
            $prev.html('<a class="page-link" data-page="prv" href="#"><i class="fa fa-angle-left"></i></a>');
            $paginatorHtml.append($prev);

            if (pageCount > 11) {
                if (rangeStart > 0) {
                    var $dot = $('<li class="page-item"></li>');
                    $dot.addClass('disabled');
                    $dot.html('<a class="page-link" href="#">..</a>');
                    $paginatorHtml.append($dot);
                }
            }

            for (var i = rangeStart; i < rangeEnd; i++) {
                var pageNo = i + 1;
                var $page = $('<li class="page-item"></li>');
                if ($this.pageNumber === pageNo) {
                    $page.addClass('active');
                }
                $page.append('<a class="page-link" data-page="' + pageNo + '" href="#">' + pageNo + '</a>');
                $paginatorHtml.append($page);
            }

            if (pageCount > 11) {
                if (rangeEnd < pageCount) {
                    $dot = $('<li class="page-item"></li>');
                    $dot.addClass('disabled');
                    $dot.html('<a class="page-link" href="#">..</a>');
                    $paginatorHtml.append($dot);
                }
            }

            var $next = $('<li class="page-item"></li>');
            if ($this.pageNumber === pageCount) {
                $next.addClass('disabled');
            }
            $next.html('<a class="page-link" data-page="nxt" href="#"><i class="fa fa-angle-right"></i></a>');
            $paginatorHtml.append($next);

            if (pageCount > 11) {
                var $end = $('<li class="page-item"></li>');
                if ($this.pageNumber === pageCount) {
                    $end.addClass('disabled');
                }
                $end.html('<a class="page-link" data-page="' + pageCount + '" href="#"><i class="fa fa-angle-double-right"></i></a>');
                $paginatorHtml.append($end);
            }

            $paginationContainer.html($paginatorHtml);
            $paginatorHtml.on('click', '.page-link', pageClickEventHandler);
        }
    };

    var pageClickEventHandler = function (e) {
        e.preventDefault();
        var $btn = $(this);

        var selectedPage = $btn.data('page');

        if (selectedPage === $this.pageNumber) {
            return;
        } else if (selectedPage === 'prv' && $this.pageNumber !== 1) {
            $this.pageNumber = $this.pageNumber - 1;
        } else if (selectedPage === 'nxt' && $this.pageNumber !== $this.pageLength) {
            $this.pageNumber = $this.pageNumber + 1;
        } else {
            $this.pageNumber = selectedPage;
        }

        $this.sendRequest();
    };

    init();
}

function RenderMaxChart(reference, canvasElementId) {

    $.get('/chart/max/' + reference, function (data) {

        var dates = $.map(data, function (i) {
            return moment(i.date);
        });

        var ctx = document.getElementById(canvasElementId).getContext('2d');
        var chart = new Chart(ctx, {
            // The type of chart we want to create
            type: 'line',

            // The data for our dataset
            data: {
                labels: dates,
                datasets: [{
                    label: 'e1RM',
                    fill: false,
                    borderColor: '#900',
                    data: $.map(data, function (i) {
                        return { x: i.date, y: i.e1RM, reps: i.reps, weight: i.weightKg, rpe: i.rpe }
                    })
                },
                {
                    borderColor: '#f66',
                    label: 'RPE Max',
                    borderDash: [5, 5],
                    fill: false,
                    data: $.map(data, function (i) {
                        return { x: i.date, y: i.rpeMax !== null ? i.rpeMax : i.e1RM, reps: i.reps, weight: i.weightKg, rpe: i.rpe }
                    }),
                }]
            },

            // Configuration options go here
            options: {
                legend: {
                    onClick: (e) => e.stopPropagation()
                },
                tooltips: {
                    callbacks: {
                        title: function (item, data) {

                            //get data set label
                            var dsLabel = '';
                            if (item[0].datasetIndex === 0) {
                                dsLabel = 'e1RM';
                            } else {
                                dsLabel = 'RPE Max';
                            }

                            //get properly formated date
                            var date = moment(item[0].xLabel).format('MMM Do YYYY');

                            //create title
                            return date + ', ' + item[0].yLabel + 'kg ' + dsLabel;
                        },
                        label: function (item, data) {
                            //get the record data from chart data

                            var rec = data.datasets[item.datasetIndex].data[item.index];

                            //if the rpe is null return no label
                            if (item.datasetIndex === 1) {
                                if (rec.rpe === null) {
                                    return '';
                                }
                            }

                            //create label
                            return rec.reps + ' reps, ' + rec.weight + 'kg' + (rec.rpe !== null ? ', RPE'+rec.rpe : '');
                        }
                    }
                },
                scales: {
                    xAxes: [{
                        type: 'time',
                        distribution: 'linear',
                        time: {
                            unit: 'week',
                        },
                        ticks: {
                            display: false,
                            min: moment.min(dates),
                            max: moment.max(dates).add(1, 'w'),
                        }
                    }],
                    yAxes: [{
                        ticks: {
                            callback: function (value, index, values) {
                                return value + 'kg';
                            }
                        }
                    }]
                }
            }
        });

    });
}