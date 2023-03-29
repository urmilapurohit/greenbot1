Date.prototype.addDays = function (n) {
    var time = this.getTime();
    var changedDate = new Date(time + (n * 24 * 60 * 60 * 1000));
    this.setTime(changedDate.getTime());
    return this;
};
var dDate = new Date();
var changeEventFired = false;

var selectedPhysicianId = 0;
var activeView = 1;
var isPhysicianMobileView = false;
var loggedInPhysicianId = 0;

var isOnlyView;
var SEUserAssignSWHJob = false;
var SWHUserAssignPVDJob = false;

var Data = {
    objStaff: [],
    objShift: []
};

//var isDrag = false;


var thData = [{
    Name: '12A',
    Time: '0'
}, {
    Name: '1A',
    Time: '1'
}, {
    Name: '2A',
    Time: '2'
}, {
    Name: '3A',
    Time: '3'
}, {
    Name: '4A',
    Time: '4'
}, {
    Name: '5A',
    Time: '5'
}, {
    Name: '6A',
    Time: '6'
}, {
    Name: '7A',
    Time: '7'
}, {
    Name: '8A',
    Time: '8'
}, {
    Name: '9A',
    Time: '9'
}, {
    Name: '10A',
    Time: '10'
}, {
    Name: '11A',
    Time: '11'
}, {
    Name: '12P',
    Time: '12'
}, {
    Name: '1P',
    Time: '13'
}, {
    Name: '2P',
    Time: '14'
}, {
    Name: '3P',
    Time: '15'
}, {
    Name: '4P',
    Time: '16'
}, {
    Name: '5P',
    Time: '17'
}, {
    Name: '6P',
    Time: '18'
}, {
    Name: '7P',
    Time: '19'
}, {
    Name: '8P',
    Time: '20'
}, {
    Name: '9P',
    Time: '21'
}, {
    Name: '10P',
    Time: '22'
}, {
    Name: '11P',
    Time: '23'
}
];


function refreshMobileView() {
    if ($("#divSchedularMobile") != undefined && $("#divSchedularMobile").is(":visible")) {
        isPhysicianMobileView = true;
    }
    else {
        isPhysicianMobileView = false;
    }
}

$('#txtDate').datepicker({
    autoclose: true,
    format: 'mm/dd/yyyy',
    orientation: "auto"
});


var objDate = {
    fullDate: dDate,
    days: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    fullDays: ['Sunday', 'Monday', 'Tueday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
},

 getTodaysInfo = {
     currDate: objDate.fullDate.getDate(),
     currDay: objDate.days[objDate.fullDate.getDay()],
     currMonth: objDate.months[objDate.fullDate.getMonth()]
 };


if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {
    $('.modal').on('show.bs.modal', function () {

        // Position modal absolute and bump it down to the scrollPosition
        $(this)
            .css({
                position: 'absolute',
                marginTop: $(window).scrollTop() + 'px',
                bottom: 'auto'
            });
        // $('#divSchedular').addClass('table-responsive');
        // Position backdrop absolute and make it span the entire page
        //
        // Also dirty, but we need to tap into the backdrop after Boostrap 
        // positions it but before transitions finish.
        //
        setTimeout(function () {
            $('.modal-backdrop').css({
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                height: Math.max(
                    document.body.scrollHeight, document.documentElement.scrollHeight,
                    document.body.offsetHeight, document.documentElement.offsetHeight,
                    document.body.clientHeight, document.documentElement.clientHeight
                ) + 'px'
            });
        }, 0);
    });
}
(function ($, w, doc) {
    $.fn.createSchedular = function (options) {
        var $this = this;
        $this.html('');

        isOnlyView = options.isOnlyView;
        if (options.Type == "M") {
            activeView = 3;
            //refreshShifts();
            $.fn.createSchedularForMonth(options, $this);
            //renderMonthCoverageColors();

        }
        else if (options.Type == "W") {
            activeView = 2;
            //refreshShifts();
            $.fn.createSchedularForWeek(options, $this);

            //if (typeof (renderWeekCoverageColors) == "function")
            //    renderWeekCoverageColors();
        } else {
            activeView = 1;
            //refreshShifts();
            $.fn.createSchedularForDay(options, $this);
            //renderDayCoverageColors();
        }

        if ($("#divSchedularMobile").length != 0) {
            $.fn.createSchedularForWeekMobile(options, $("#divSchedularMobile"));
            //renderWeekCoverageColors();
        }
        refreshDates();

        $('.weekJobs').draggable({
            revert: "invalid",
            scroll: false,
            //containment: ".weekgrid",
            start: function (event, ui) {
                if (isOnlyView) {
                    return false;
                }
                else {
                    $(this).css('z-index', '10');
                    $(this).data("origPosition", $(this).position());

                    $("#divSchedular").removeClass('table-responsive');
                    $("#divSchedular").addClass('table');

                    //ui.helper.bind("click.prevent", function (event) { event.preventDefault(); });

                    $('.childWeekTable').each(function (d, element) {
                        $($(element).find('tr')).each(function (j, ele) {
                            $($(ele).find('td')).each(function (k, data) {
                                if ($(data).find('.parentDropDiv').length > 0) {
                                    $($(data).find('.parentDropDiv')).css('z-index', '2');
                                }
                            });
                        });
                    });
                }
                $(this).popover('hide');
                $(event.toElement).one('click', function (e) { e.stopImmediatePropagation(); });
            },
            drag: function () {
                $(this).popover('hide');
            },
            stop: function (event, ui) {
                $(this).css('z-index', '0');
                //alert('isDrag' + isDrag);
                //setTimeout(function () { ui.helper.unbind("click.prevent"); }, 300);

                $('.childWeekTable').each(function (d, element) {
                    $($(element).find('tr')).each(function (j, ele) {
                        $($(ele).find('td')).each(function (k, data) {
                            if ($(data).find('.parentDropDiv').length > 0) {
                                $($(data).find('.parentDropDiv')).css('z-index', '-1');
                            }
                        });
                    });
                });

                $("#divSchedular").removeClass('table');
                $("#divSchedular").addClass('table-responsive');
                //isDrag = false;
            }
        });
        $('.monthJobs').draggable({
            revert: "invalid",
            scroll: false,
            //containment: ".monthgrid",
            start: function (event, ui) {
                $(this).css('z-index', '10');
                $(this).popover('hide');
                var result = monthDraggableStart(event, ui, $(this));
                return result;
            },
            drag: function () {
                $(this).popover('hide');
            },
            stop: function (event, ui) {
                $(this).css('z-index', '0');
                monthDraggableStop(event, ui);

            },
        });

        $(".weekDroppable").droppable({
            tolerance: "pointer",
            drop: function (event, ui) {
                onDropCheckCount(true, event, ui);
            }
        });

        $(".monthDroppable").droppable({
            tolerance: "pointer",
            drop: function (event, ui) {
                monthDroppableDrop(event, ui);
            }
        });





        function droppableTD(event, ui, obj) {

            // remove colspan
            var colspanValue = ui.draggable.parent().attr('colspan');
            if (colspanValue > 1) {

                ui.draggable.parent().removeAttr("colspan");

                var userID = ui.draggable.parent().attr('name').split('_')[1];
                var dateNum = $(ui.draggable.parent().closest('table').find('th').eq(ui.draggable.parent().index()).next()).attr('DateNum');
                var $td = createNewElement('td');
                var $tr = ui.draggable.parent().parent();
                $td.attr('name', 'w_' + userID + '_' + dateNum);
                //$td.addClass('weekDroppable');
                $td.droppable({
                    drop: function (e, u) {
                        droppableTD(e, u, $(this));
                    }
                });
                $tr.find('td').eq(ui.draggable.parent().index()).after($td);
            }

            //add colspan
            if (colspanValue > 1) {
                for (var i = 1; i < colspanValue; i++) {
                    obj.next().remove();
                }
                obj.attr('colspan', colspanValue);
            }

            ui.draggable.addClass("dropped");
            ui.draggable.detach().appendTo(obj);
        }
    };


    $.fn.createSchedularForDay = function (data, ele) {

        var $table = createNewElement('table'),
            $tbody = createNewElement('tbody');

        if (data.StaffData) {
            var $Thtr = createNewElement('tr');
            $Thtr.append('<th><div class="dayStaff">Staff</div></th>');

            var $TCoverage = createNewElement('tr');

            $TCoverage.append('<td class="whiteCoverage">Coverage</td>');

            for (var k = 0; k < thData.length; k++) {
                var $th = createNewElement('th');
                //$th.text(thData[k].Name);
                //$th.data('value', thData[k]);

                $div = createNewElement('div');
                $div.text(thData[k].Name);
                $div.data('value', thData[k]);
                $th.append($div);

                $Thtr.append($th);

                var $td = createNewElement('td');
                $td.attr('name', 'coverage' + '_' + thData[k].Time);


                $td.attr('shift', thData[k].Time);
                //$td.text('test');
                $TCoverage.append($td);
            }

            $tbody.append($Thtr);
            $tbody.append($TCoverage);

            for (var i = 0; i < data.StaffData.length; i++) {
                var $tr = createNewElement('tr'),
                    $tdName = createNewElement('td');
                //$tdName.text(data.StaffData[i].name);

                //$div2 = createNewElement('div');
                //$div2.addClass('dayDPhoto');

                //var $img = createNewElement('img');
                //$img.attr('src', data.StaffData[i].photo);
                //$img.attr('height', '29px');
                //$img.attr('width', '29px');
                //$div2.append($img);

                //$tdName.append($div2);

                $div = createNewElement('div');
                $div.text(data.StaffData[i].name);
                $div.addClass('dayDName');
                $tdName.append($div);

                $tdName.addClass('physicianNames');
                $tr.append($tdName);
                for (var j = 0; j < thData.length; j++) {
                    var $td = createNewElement('td');
                    $td.attr('name', data.StaffData[i].PhysicianId + '_' + thData[j].Time);
                    $td.attr('shift', thData[j].Time);

                    var $div = createNewElement('div');
                    $td.append($div);

                    $tr.append($td);
                }
                $tbody.append($tr);
            }
            $table.append($tbody).addClass(' daygrid');
            ele.html($table);

            var currentDateShiftData = $.grep(data.ShiftData, function (val, i) {
                return val.date == moment(dDate).format("MM/DD/YYYY");
            });

            currentDateShiftData.sort(function (a, b) {
                return a._start - b._start;
            });

            for (var l = 0; l < currentDateShiftData.length; l++) {
                var startTime = currentDateShiftData[l]._start,
                    endTime = currentDateShiftData[l]._endMin > 0 ? (currentDateShiftData[l]._end + 1) : currentDateShiftData[l]._end;

                if (endTime == 0)
                    endTime = 24;

                var difference = endTime < startTime ? startTime - endTime : endTime - startTime;

                var $tdCol = createNewElement('td');
                $tdCol.attr('colspan', (difference));
                $tdCol.addClass('dayShiftBar');
                var $table = createNewElement('table'),
                    $tbody = createNewElement('tbody'),

                    $tr = createNewElement('tr');

                $table.css({
                    'width': '100%'
                });



                var tdWidth = (100 / (difference * 4));

                if (endTime != 0) {

                    for (var a = startTime; a < endTime; a++) {
                        for (var b = 0; b < 4; b++) {
                            var $td = createNewElement('td');
                            $td.attr('name', currentDateShiftData[l].physicianId + "_" + a + ":" + (15 * b));
                            $td.css({
                                'width': tdWidth + '%'
                            });
                            $tr.append($td);
                        }
                    }
                }
                else {
                    for (var a = startTime; a <= 23 ; a++) {
                        for (var b = 0; b < 4; b++) {
                            var $td = createNewElement('td');
                            $td.attr('name', currentDateShiftData[l].physicianId + "_" + a + ":" + (15 * b));
                            $td.css({
                                'width': tdWidth + '%'
                            });
                            $tr.append($td);
                        }
                    }
                }

                $tbody.append($tr);
                $table.append($tbody);


                if (currentDateShiftData[l].iNoOfDays.split('')[data.currDate.getDay()] === "1") {

                    var d1 = new Date();
                    d1.setHours(currentDateShiftData[l]._end, currentDateShiftData[l]._endMin, 00, 000);
                    var d2 = new Date();
                    d2.setHours(currentDateShiftData[l]._start, currentDateShiftData[l]._startMin, 00, 000);

                    if (moment(d1).format("HH:mm") == "00:00")
                        d1.addDays(1);

                    var span = ((parseInt(((d1 - d2) / 1000)) / 60) / 15);

                    if (span < 0)
                        span = span * -1;

                    var $tdS = createNewElement('td');

                    if (currentDateShiftData[l].status == pendingStatus)
                        $tdS.addClass('yellowShifts');
                    else
                        $tdS.addClass('blueShifts');

                    //$tdS.css({
                    //    'background-color': 'aqua',
                    //    'text-align': 'center',
                    //    'cursor': 'pointer'
                    //});

                    $tdS.attr('colspan', span);

                    $tdS.html('<div class="dayShiftTime">' + currentDateShiftData[l].startFull + ' - ' + currentDateShiftData[l].endFull + '</div> ' + currentDateShiftData[l].region);

                    $tdS.data('ShiftData', currentDateShiftData[l]);

                    $tdS.on('click', function (e) {

                        var shiftData = $(this).data('ShiftData');
                        showShift(shiftData);
                        $('#myModal').modal('show');
                    });

                    $tr.find('[name="' + currentDateShiftData[l].physicianId + '_' + currentDateShiftData[l]._start + ':' + currentDateShiftData[l]._startMin + '"]').nextAll("*:lt( " + ((span) - 1) + ")").remove();
                    $tr.find('[name="' + currentDateShiftData[l].physicianId + '_' + currentDateShiftData[l]._start + ':' + currentDateShiftData[l]._startMin + '"]').replaceWith($tdS);

                    $tdCol.css({ 'padding': '0px' });

                    $tdCol.append($table);

                    $('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + ']').nextAll("*:lt( " + (difference - 1) + ")").remove();

                    if ($('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + ']').length != 0)
                        $('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + ']').replaceWith($tdCol);
                    else {
                        var oldTable = $('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + '\\:' + currentDateShiftData[l]._startMin + ']').parent().parent().parent();
                        $table = oldTable;

                        $('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + '\\:' + currentDateShiftData[l]._startMin + ']').nextAll("*:lt( " + ((span) - 1) + ")").remove();
                        $('[name=' + currentDateShiftData[l].physicianId + '_' + startTime + '\\:' + currentDateShiftData[l]._startMin + ']').replaceWith($tdS);
                    }

                }
            }
        }
        return ele;
    };

    $.fn.createSchedularForWeek = function (data, ele) {
        electrician = [];
        eletricianViseJob = [];
        weekStartDate = '';
        weekEndDate = '';
        electricianViseMainTDCount = [];

        var d = dDate;
        var currentDate = moment(dDate);

        var $table = createNewElement('table'),
             $tbody = createNewElement('tbody'),
             day = d.getDay(),

             curr = data.currDate, // get current date
            //first = curr.getDate() - curr.getDay(), // First day is the day of the month - the day of the week
            first = parseInt(moment(dDate).startOf('week').format('D')),
            //last = first + 6,
            last = parseInt(moment(dDate).startOf('week').add(6, "day").format('D')),
            currDate = curr.getDate();// last day is the first day + 6



        if (data.StaffData) {
            var $Thtr = createNewElement('tr');

            $Thtr.append('<th><div class="weekStaff">Staff</div></th>');

            //var $TCoverage = createNewElement('tr');

            //$TCoverage.append('<td class="whiteCoverage">Coverage</td>');

            var da = first;

            for (var i = 0; i < objDate.days.length; i++) {

                var $th = createNewElement('th');
                //$th.text(objDate.days[i] + " " + first);
                //$th.css({
                //    'text-align': 'center'
                //});

                $div = createNewElement('div');
                $div.text(objDate.days[i] + " " + first);
                $div.addClass('weekHeader');
                $th.append($div);

                $Thtr.append($th);
                $th.attr("DateNum", first);

                first = parseInt(moment(dDate).startOf('week').add(i + 1, 'day').format('D'));
                da = first;
            }

            $tbody.append($Thtr);

            //$tbody.append($TCoverage);

            for (var k = 0; k < data.StaffData.length; k++) {

                da = parseInt(moment(dDate).startOf('week').format('D'));

                var $tr = createNewElement('tr'),
                    $tdName = createNewElement('td');

                $div = createNewElement('div');
                //if (data.StaffData[k].usertypeid == "7" || data.StaffData[k].usertypeid == "9")
                //    $div.text(data.StaffData[k].name + " (SE)");
                //else if (data.StaffData[k].usertypeid == "10")
                //    $div.text(data.StaffData[k].name + " (SWH)");

                //debugger;

                if (data.StaffData[k].IsPVDUser && data.StaffData[k].IsSWHUser)
                    $div.text(data.StaffData[k].name + " (SE-SWH)");

                else if (data.StaffData[k].IsPVDUser || data.StaffData[k].usertypeid == "9")
                    $div.text(data.StaffData[k].name + " (SE)");
                else if (data.StaffData[k].IsSWHUser)
                    $div.text(data.StaffData[k].name + " (SWH)");



                $div.addClass('dayDName');
                $tdName.append($div);

                $tdName.addClass('seNames');

                $tr.append($tdName);
                for (var j = 0; j < objDate.days.length; j++) {

                    var $td = createNewElement('td');
                    $td.attr('name', 'w_' + data.StaffData[k].id + '_' + da);
                    //$td.addClass('weekDroppable');
                    $tr.append($td);

                    var $childtable = createNewElement('table'), $childtbody = createNewElement('tbody');

                    var $childtr = createNewElement('tr');
                    var $childtd = createNewElement('td');
                    var newDate = moment(dDate).startOf('week').add(j, 'day').format('YYYY-MM-DD');
                    $childtd.addClass('weekDroppable');
                    $childtd.css('border', 'none');
                    $childtd.html('<div></div>');
                    $childtd.attr('StartDate', newDate);
                    $childtd.attr('EndDate', newDate);
                    $childtd.attr('userID', data.StaffData[k].id);
                    $childtd.attr('userTypeID', data.StaffData[k].usertypeid);
                    $childtd.attr('IsPVDUser', data.StaffData[k].IsPVDUser);
                    $childtd.attr('IsSWHUser', data.StaffData[k].IsSWHUser);

                    $childtd.attr('isData', 0);
                    $childtr.append($childtd);
                    $childtbody.append($childtr);

                    $childtable.attr("class", "childWeekTable");

                    $childtable.append($childtbody);
                    $td.append($childtable);

                    da = parseInt(moment(dDate).startOf('week').add(j + 1, 'day').format('D'));
                }
                $tbody.append($tr);
            }

            weekStartDate = moment(new Date(moment(dDate).startOf('week').year(), moment(dDate).startOf('week').month(), moment(dDate).startOf('week').date()));
            weekEndDate = moment(new Date(moment(dDate).endOf('week').year(), moment(dDate).endOf('week').month(), moment(dDate).endOf('week').date()));

            $.each(data.ShiftData, function (index, e) {
                if (electrician.length > 0) {
                    var isExist = false;
                    $.each(electrician, function (j, ele) {
                        if (ele.userID == e.userID) {
                            isExist = true;
                            return false;
                        }
                        else
                            isExist = false;
                    });
                    if (!isExist)
                        electrician.push({ userID: e.userID, userTypeID: e.usertypeid, IsPVDUser: e.IsPVDUser, IsSWHUser: e.IsSWHUser });
                }
                else
                    electrician.push({ userID: e.userID, userTypeID: e.usertypeid, IsPVDUser: e.IsPVDUser, IsSWHUser: e.IsSWHUser });
            });

            var tempShiftData = data.ShiftData;
            var tempElectricianJob = [];


            $.each(electrician, function (index, e) {
                tempElectricianJob = [];
                var category1 = [], category2 = [], category3 = [], category4 = [], category5 = [], category6 = [], category7 = [];

                $.each(tempShiftData, function (j, ele) {
                    if (ele.userID == e.userID) {
                        var startDate, endDate;
                        var isBeforeStartDate = moment(ele.startDate).isBefore(weekStartDate.format("YYYY-MM-DD"));
                        if (isBeforeStartDate)
                            startDate = weekStartDate.format("YYYY-MM-DD");
                        else
                            startDate = ele.startDate;

                        var isAfterEndDate = moment(ele.endDate).isAfter(weekEndDate.format("YYYY-MM-DD"));
                        if (isAfterEndDate)
                            endDate = weekEndDate.format("YYYY-MM-DD");
                        else
                            endDate = ele.endDate;

                        var responseData = null;
                        if (!(moment(weekStartDate).isAfter(ele.startDate) && moment(weekStartDate).isAfter(ele.endDate)) && !moment(weekEndDate).isBefore(ele.startDate)) {
                            for (var c = 1; c <= 7; c++) {
                                if (c == 1) {
                                    responseData = electricianCategory(category1, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category1 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 2) {
                                    responseData = electricianCategory(category2, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category2 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 3) {
                                    responseData = electricianCategory(category3, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category3 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 4) {
                                    responseData = electricianCategory(category4, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category4 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 5) {
                                    responseData = electricianCategory(category5, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category5 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 6) {
                                    responseData = electricianCategory(category6, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category6 = responseData;
                                        break;
                                    }
                                }
                                else if (c == 7) {
                                    responseData = electricianCategory(category7, startDate, endDate, ele);
                                    if (responseData != null) {
                                        category7 = responseData;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
                eletricianViseJob.push({ userID: e.userID, category1: category1, category2: category2, category3: category3, category4: category4, category5: category5, category6: category6, category7: category7 });
            });

            $.each(data.ShiftData, function (index, e) {
                $.each(eletricianViseJob, function (j, ele) {
                    var isExist;

                    if (ele.userID == e.userID) {
                        for (var c = 1; c <= 7; c++) {
                            if (c == 1) {
                                isExist = findJobScheduleCategoryVise(ele.category1, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 2) {
                                isExist = findJobScheduleCategoryVise(ele.category2, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 3) {
                                isExist = findJobScheduleCategoryVise(ele.category3, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 4) {
                                isExist = findJobScheduleCategoryVise(ele.category4, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 5) {
                                isExist = findJobScheduleCategoryVise(ele.category5, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 6) {
                                isExist = findJobScheduleCategoryVise(ele.category6, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                            else if (c == 7) {
                                isExist = findJobScheduleCategoryVise(ele.category7, e, $tbody, currentDate, 'blueShifts', ele);
                                if (isExist)
                                    break;
                            }
                        }
                    }
                });
            });



            $table.append($tbody).addClass('weekgrid');
            ele.html($table);

            ResetHeightAndMakeDivDroppable();

            //$(".weekJobs").live('click', function () {
            //    showJobSchedulingDetail($(this).attr('jobSchedulingID'));
            //});
            $(".weekJobs").click(function (e) {
                e.stopPropagation();
                e.cancleBubble = true;
                if (!($(e.target).is('a')) && !($(e.target).is('svg')) && !($(e.target).is('g')) && !($(e.target).is('path')))
                    showJobSchedulingDetail($(this).attr('jobSchedulingID'), $(this).attr('jobTitle'));
                //}
            });
            $('.weekJobs').each(function () {
                var $this = $(this);
                $this.popover({
                    trigger: 'hover',
                    placement: 'top auto',
                    html: true,
                    content: $this.attr('title'),
                    container: 'body'
                });
            });
            //});
        }
        return ele;

    };

    var lTemp = 0;
    $.fn.createSchedularForMonth = function (data, ele) {

        monthWeek1 = [], monthWeek2 = [], monthWeek3 = [], monthWeek4 = [], monthWeek5 = [], monthWeek6 = [];
        week1JobCount = [], week2JobCount = [], week3JobCount = [], week4JobCount = [], week5JobCount = [], week6JobCount = [];

        var $table = createNewElement('table'),
            $tbody = createNewElement('tbody');
        if (data.StaffData) {
            var calender = {
                getFirstDay: function (year, month) {
                    var d = new Date(year, month, 01);
                    return d.getDay();
                },
                getLastDay: function (year, month) {
                    var d = new Date(year, month + 1, 0);
                    return d.getDate();
                },
                setTodaysDate: function () {


                    var date = new Date(),
                        month = date.getMonth(),
                        year = date.getFullYear();
                    calender.generateCalender(year, month);
                },
                getMonth: function (month) {
                    return objDate.months[month];
                },

                generateCalender: function (year, month) {
                    var firstDay = calender.getFirstDay(year, month),
                        lastDay = calender.getLastDay(year, month),
                        tr = '', td = '', z = 1,
                        innTable = '';
                    var tdExtra = '';
                    //var firstDate = year + '-' + (month + 1) + '-' + z;

                    var labelmonth = month >= 9 ? (month + 1) : '0' + (month + 1);
                    var day = z >= 10 ? z : '0' + z;
                    var firstDate = year + '-' + labelmonth + '-' + day;

                    var date;
                    var num;

                    for (var i = 0; i <= 41; i++) {
                        if (((i % 7 == 0) || (i == 41)) && i != 0) {

                            tr += "<tr class=datenumrow>" + td + "</tr>";

                            var endDate = date;
                            var endNum = num;
                            var startDate = moment(new Date(moment(endDate).startOf('week').year(), moment(endDate).startOf('week').month(), moment(endDate).startOf('week').date()));

                            if (i == 7)
                                startDate = firstDate;

                            var startNum = moment(startDate, "YYYY-MM-DD").format("D");

                            for (var j = 0; j < 5; j++) {
                                var rowStartEndNum = startNum + '_' + endNum + '_' + j;
                                tr += "<tr rowStartEndNum='" + rowStartEndNum + "'>" + tdExtra + "</tr>";
                            }

                            td = '';
                            tdExtra = '';
                        }
                        if (i >= firstDay && z <= lastDay) {

                            var labelmonth = month >= 9 ? (month + 1) : '0' + (month + 1);
                            var day = z >= 10 ? z : '0' + z;

                            date = year + '-' + labelmonth + '-' + day;
                            var num = moment(date, "YYYY-MM-DD").format("D");
                            var name = "m_" + (month + 1) + '_' + num;
                            td += "<td  StartDate='" + date + "'>" + num + "</td>";

                            //tdExtra += "<td class='monthDroppable' StartDate='" + date + "' name='" + name + "' isData = 0>&nbsp;</td>";
                            tdExtra += "<td class='monthDroppable' StartDate='" + date + "' name='" + name + "' isData = 0><div></div></td>";
                            //tdExtra += "<td  StartDate='" + date + "' name='" + name + "' isData = 0><div></div></td>";

                            z++;
                        }
                        else if (i > 34) {

                        }
                        else {
                            td += "<td></td>";
                            //tdExtra += "<td><div class='redmoreShift'>View More</div></td>";
                            tdExtra += "<td><div></div></td>";
                        }
                    }

                    var $Thtr = createNewElement('tr');

                    for (var k = 0; k < objDate.days.length; k++) {
                        var $th = createNewElement('th');

                        var $div = createNewElement('div');
                        $div.text(objDate.days[k]);
                        $div.addClass('monthTitle');

                        $th.append($div);
                        $th.text(objDate.days[k]);
                        $th.css({
                            'text-align': 'center'
                        });
                        $Thtr.append($th);

                    }
                    $tbody.html('').append($Thtr);
                    $tbody.append(tr);
                    $table.append($tbody);
                    $table.addClass('monthgrid');
                    ele.html($table);
                    BindDataToMonthGrid($table, data, firstDate);

                    $('.monthJobs').each(function () {
                        var $this = $(this);
                        $this.popover({
                            trigger: 'hover',
                            placement: 'top auto',
                            html: true,
                            content: $this.attr('title'),
                            container: 'body'
                        });
                    });
                    //$('.monthgrid').each(function (d, element) {
                    //    $($(element).find('tr')).each(function (j, ele) {
                    //        $($(ele).find('td')).each(function (k, data) {
                    //            if ($(data).find('.monthJobs'))
                    //            {
                    //                //$(data).removeClass('monthDroppable');
                    //            }
                    //        });
                    //    });
                    //});

                    //ResetHeightAndMakeDivDroppableMonth();
                },

                getTableFromData: function (shiftData, date) {
                    var $innTable = createNewElement('table'),
                        $tbody = createNewElement('tbody'),
                        $th = createNewElement('th'),
                        $thTr = createNewElement('tr');

                    $th.text(moment(date, "YYYY-MM-DD").format("D"));

                    $th.attr('name', 'coverage_' + moment(date, "YYYY-MM-DD").format("D"));
                    $th.attr('shift', moment(date, "YYYY-MM-DD").format("D"));

                    lTemp++;

                    $th.addClass('')
                    $thTr.append($th);
                    $tbody.append($thTr);

                    var data = shiftData.filter(function (el) {
                        return moment(el.date).format("YYYY-MM-DD") == moment(date).format("YYYY-MM-DD");
                    });

                    $tbody.data('allData', data);

                    var isExtra = (data.length > 5);

                    for (var l = 0; l < 5; l++) {

                        var $innTr = createNewElement('tr'),
                            $innTd = createNewElement('td');

                        if (l == 4 && isExtra) {
                            var $a = createNewElement('a');
                            $a.text("VIEW ALL (" + (data.length - l) + " MORE)");

                            $a.attr('onclick', 'showPopup("' + data[0].date + '");');
                            $innTd.append($a);

                            $innTd.css({ 'text-align': 'center', 'cursor': 'pointer' });

                        } else {

                            if (data[l]) {
                                $innTd.addClass('monthShifts');
                                $innTd.attr('jobSchedulingID', data[l].id);


                                //$innTd.html('<span style="padding:2px;">' + data[l].startFull + ' - ' + data[l].endFull + ' / ' + data[l].name + '</span>');

                                $innTd.html('<span style="padding:2px;">' + data[l].startTime + ' - ' + data[l].endTime + ' ' + data[l].label + '</span>');

                                $innTd.attr('onclick', 'showJobSchedulingDetail("' + data[l].id + '");');


                                //if (data[l].status == pendingStatus)
                                //    $innTd.addClass('yellowShifts');
                                //else
                                $innTd.addClass('blueShifts');

                            } else {
                                $innTd.text('');
                            }
                        }

                        $innTr.append($innTd);
                        $tbody.append($innTr);
                    }

                    return $innTable.append($tbody).html();
                }
            };
            calender.generateCalender(objDate.fullDate.getFullYear(), objDate.fullDate.getMonth());

        }
    };

    $.fn.createSchedularForWeekMobile = function (data, ele) {
        var d = dDate;
        var currentDate = moment(dDate);
        var startOfWeek = moment(dDate).startOf('week');
        var originalStartOfWeek = moment(dDate).startOf('week');

        var $ul = createNewElement('ul'),
             day = d.getDay(),

             curr = data.currDate, // get current date
            //first = curr.getDate() - curr.getDay(), // First day is the day of the month - the day of the week
            first = parseInt(startOfWeek.format('D')),
            //last = first + 6,
            last = parseInt(startOfWeek.add(6, "day").format('D')),
            currDate = curr.getDate();// last day is the first day + 6

        if (data.StaffData) {


            var da = first;

            for (var i = 0; i < objDate.days.length; i++) {

                var $li = createNewElement('li');
                $li.attr('name', 'coverage_' + da);
                $li.attr('shift', da);
                //$th.text(objDate.days[i] + " " + first);
                //$th.css({
                //    'text-align': 'center'
                //});

                $div = createNewElement('div');
                $div.text(objDate.days[i] + ", " + originalStartOfWeek.format("MMM") + " " + first);
                $div.addClass('psm_date');
                $li.append($div);
                //$th.attr('name', 'w_' + data.StaffData[i].PhysicianId + '_' + da);                

                $ul.append($li);

                startOfWeek = originalStartOfWeek.add(1, 'day');
                //first++;
                first = parseInt(startOfWeek.format('D'));
                da = first;
            }

            var originalDate = moment(currentDate);
            for (var l = 0; l < data.ShiftData.length; l++) {
                var shiftProcessed = false;
                var color = '';
                currentDate = moment(new Date(originalDate.year(), originalDate.month(), originalDate.date() - originalDate.isoWeekday()));
                for (var i = 0; i < 7; i++) {
                    //if (data.ShiftData[l].iNoOfDays.split('')[moment(data.ShiftData[l].date, "MM/DD/YYYY").day() - 1] === "1") {
                    if (data.ShiftData[l].date == currentDate.format("MM/DD/YYYY") && !shiftProcessed) {

                        var oldHtml = $ul.find('[name=coverage_' + currentDate.format("D") + ']').html();

                        if (data.ShiftData[l].status == pendingStatus)
                            color = 'yellowShifts';
                        else
                            color = 'blueShifts';

                        $ul.find('[name=coverage_' + currentDate.format("D") + ']').html(oldHtml + '<div class="psm_time ' + color + ' weekJobs" ShiftDetail = ' + data.ShiftData[l].id + '>' + data.ShiftData[l].startFull + ' - ' + data.ShiftData[l].endFull + ', <span>' + data.ShiftData[l].region + ' </span></div>');

                        shiftProcessed = true;
                        continue;
                    }

                    currentDate.add(1, "day");
                }
            }

            ele.html($ul);

        }
        return ele;
    };

}(jQuery, window, document));

$('#myModal').on('hidden.bs.modal', function (e) {
    clearModal();
});

var electrician = [];
var eletricianViseJob = [];
var weekStartDate;
var weekEndDate;
var electricianViseMainTDCount = [];

var monthWeek1 = [], monthWeek2 = [], monthWeek3 = [], monthWeek4 = [], monthWeek5 = [], monthWeek6 = [];

var week1JobCount = [], week2JobCount = [], week3JobCount = [], week4JobCount = [], week5JobCount = [], week6JobCount = [];

function WeekViseJobCount(monthWeek, weekJobCount) {
    var startDate = monthWeek.startDate;
    var endDate = monthWeek.endDate;

    var startNum = moment(startDate, "YYYY-MM-DD").format("D");
    var endNum = moment(endDate, "YYYY-MM-DD").format("D");

    for (var i = 0; i < (endNum - startNum) + 1; i++) {
        weekJobCount.push({ startDate: startDate, totalJobCount: 0, displayJobCount: 0, jobData: [] })
        startDate = moment(startDate).add(1, 'day').format("YYYY-MM-DD");
    }
}

function BindWeekViseJobCount(monthWeek, weekJobCount) {
    var totalCount = 0;
    var displayCount = 0;
    $.each(weekJobCount, function (index, ele) {
        $.each(monthWeek.jobData, function (i, e) {
            if (e.startDate <= ele.startDate && ele.startDate <= e.endDate) {

                ele.jobData.push(e);
                totalCount = totalCount + 1;
                if (!(e.startDate == ele.startDate && e.endDate == ele.startDate))
                    displayCount = displayCount + 1;
            }
        });
        ele.totalJobCount = totalCount;
        ele.displayJobCount = displayCount;
        totalCount = 0;
        displayCount = 0;
    });

    var maxTotalJobCount = 0;
    var maxdisplayJobCount = 0;

    $.each(weekJobCount, function (i, e) {
        if (maxTotalJobCount == 0) {
            maxTotalJobCount = e.totalJobCount;
            maxdisplayJobCount = e.displayJobCount;
        }

        if (e.totalJobCount > maxTotalJobCount)
            maxTotalJobCount = e.totalJobCount;
        if (e.displayJobCount > maxdisplayJobCount)
            maxdisplayJobCount = e.displayJobCount;
    });

    monthWeek.totalMaxJobCount = maxTotalJobCount;
    monthWeek.displayMaxJobCount = maxdisplayJobCount;
}

function ResetHeightAndMakeDivDroppableMonth() {

    $(".monthgrid").find('tr').each(function (i, e) {
        $(e).find('.monthDroppable').each(function (index, ele) {
            if ($(ele).find('.monthJobs').length > 0) {
                $(ele).css('position', 'relative');
                //alert($(ele).find('.monthJobs').height() + ' ' + $(ele).attr('startDate'));
                $(ele).css('height', $(ele).find('.monthJobs').height() + 19);
            }
        });
    });


    $(".monthgrid").find('tr').each(function (i, e) {
        $(e).find('.monthDroppable').each(function (index, ele) {
            if ($(ele).find('.monthJobs').length > 0) {
                var tdHeight = $(ele).height();
                var tdWidth = $(ele).width();

                var colSpan = $(ele).attr('colspan');

                $(ele).css('position', 'relative');
                var startDate = $(ele).attr('startdate');
                var userID = $(ele).attr('userID');

                if (colSpan == 1 || colSpan == undefined)
                    $(ele).find('.monthJobs').css("width", "96.5%");
                else if (colSpan == 2)
                    $(ele).find('.monthJobs').css("width", "98%");
                else if (colSpan == 3)
                    $(ele).find('.monthJobs').css("width", "98.5%");
                else if (colSpan == 4 || colSpan == 5 || colSpan == 6 || colSpan == 7)
                    $(ele).find('.monthJobs').css("width", "99%");
                else
                    $(ele).find('.monthJobs').css("width", "98.5%");

                if (colSpan > 1) {
                    var childWidth = tdWidth / colSpan;
                    var $parentDiv = createNewElement('div');
                    $parentDiv.addClass('parentDropDiv');
                    $($parentDiv).css('height', tdHeight);
                    $($parentDiv).css('width', tdWidth);
                    $($parentDiv).css('z-index', '-1');

                    for (var l = 0; l < colSpan; l++) {
                        var $childDropDiv = createNewElement('div');
                        $($childDropDiv).addClass('monthDroppable');
                        $($childDropDiv).css('height', tdHeight);
                        $($childDropDiv).css('width', childWidth - 2);
                        $($childDropDiv).css('float', 'left');
                        $($childDropDiv).attr('startdate', startDate);
                        $($childDropDiv).attr('userID', userID);

                        startDate = moment(startDate).add(1, "day").format("YYYY-MM-DD");

                        $parentDiv.append($childDropDiv);
                    }
                    $(ele).append($parentDiv);
                }
            }
        });
    });

    $(".monthgrid").find('tr').each(function (i, e) {
        $(e).find('.monthDroppable').each(function (index, ele) {
            if ($(ele).find('.monthJobs').length > 0) {
                if ($(ele).attr("colspan") > 1) {
                    $(this).removeClass('monthDroppable');
                }
            }
        });
    });

}

function ResetHeightAndMakeDivDroppable() {
    $('.childWeekTable').each(function (d, element) {
        $($(element).find('tr')).each(function (j, ele) {
            $($(ele).find('td')).each(function (k, data) {
                if ($(data).find('.weekJobs').length > 0) {
                    $(data).css('position', 'relative');
                    $(data).css('height', ($(data).find('.weekJobs').height() + 34));
                }
            });
        });
    });


    $('.childWeekTable').each(function (d, element) {
        var height = 0;
        $($(element).find('tr')).each(function (j, ele) {
            $($(ele).find('td')).each(function (k, data) {
                if ($(data).find('.weekJobs').length > 0) {
                    height = height + $(data).find('.weekJobs').height();
                    return false;
                }
            });
        });
        if (height == 0) {
            var heightParent = $(element).parent().height();
            $(element).css("height", heightParent);
        }
        else
            $(element).css("height", height);
    });

    $('.childWeekTable').each(function (d, element) {
        var heightParent = $(element).parent().height();
        $(element).css("height", heightParent);
    });

    $('.childWeekTable').each(function (d, element) {
        $($(element).find('tr')).each(function (j, ele) {
            $($(ele).find('td')).each(function (k, data) {
                if ($(data).find('.weekJobs').length > 0) {

                    var tdHeight = $(data).height();
                    var tdWidth = $(data).width();

                    var colSpan = $(data).attr('colspan');

                    $(data).css('position', 'relative');
                    var startDate = $(data).attr('startdate');
                    var userID = $(data).attr('userID');

                    if (colSpan == 1 || colSpan == undefined)
                        $(data).find('.weekJobs').css("width", "95.5%");
                    else if (colSpan == 2)
                        $(data).find('.weekJobs').css("width", "98%");
                    else if (colSpan == 3)
                        $(data).find('.weekJobs').css("width", "98.5%");
                    else if (colSpan == 4 || colSpan == 5 || colSpan == 6 || colSpan == 7)
                        $(data).find('.weekJobs').css("width", "99%");
                    else
                        $(data).find('.weekJobs').css("width", "98.5%");

                    if (colSpan > 1) {
                        var childWidth = tdWidth / colSpan;
                        var $parentDiv = createNewElement('div');
                        $parentDiv.addClass('parentDropDiv');
                        $($parentDiv).css('height', tdHeight);
                        $($parentDiv).css('width', tdWidth);
                        $($parentDiv).css('z-index', '-1');
                        for (var l = 0; l < colSpan; l++) {
                            var $childDropDiv = createNewElement('div');
                            $($childDropDiv).addClass('weekDroppable');
                            $($childDropDiv).css('height', tdHeight);
                            $($childDropDiv).css('width', childWidth - 2);
                            $($childDropDiv).css('float', 'left');
                            $($childDropDiv).attr('startdate', startDate);
                            $($childDropDiv).attr('userID', userID);
                            $($childDropDiv).attr('userTypeID', $(data).attr('userTypeID'));

                            startDate = moment(startDate).add(1, "day").format("YYYY-MM-DD");

                            $parentDiv.append($childDropDiv);
                        }
                        $(data).append($parentDiv);
                    }
                }
            });
        });
    });

    $('.childWeekTable').each(function (d, element) {
        $($(element).find('tr')).each(function (j, ele) {
            $($(ele).find('td')).each(function (k, data) {
                if ($(data).find('.weekJobs').length > 0) {

                    //alert($(data).find('.weekJobs').height());

                    var colSpan = $(data).attr('colspan');
                    if (colSpan == undefined || colSpan == 1) {
                        $(data).addClass('weekDroppable');
                    }
                }
                else {
                    $(data).css('border-top', '0px');
                    $(data).css('border-bottom', '0px');
                }
            });
        });
    });


}

function BindDataToMonthGrid(table, data, firstDate) {
    BindWeekArray(firstDate);

    $.each(data.ShiftData, function (index, e) {
        insertDataIntoWeekArray(e);
    });


    for (var i = 1; i <= 6; i++) {
        if (i == 1) {
            WeekViseJobCount(monthWeek1[0], week1JobCount);
            BindWeekViseJobCount(monthWeek1[0], week1JobCount);
            GenerateAdditionalRaw(table, monthWeek1[0], week1JobCount);
        }
        if (i == 2) {
            WeekViseJobCount(monthWeek2[0], week2JobCount);
            BindWeekViseJobCount(monthWeek2[0], week2JobCount);
            GenerateAdditionalRaw(table, monthWeek2[0], week2JobCount);
        }
        if (i == 3) {
            WeekViseJobCount(monthWeek3[0], week3JobCount);
            BindWeekViseJobCount(monthWeek3[0], week3JobCount);
            GenerateAdditionalRaw(table, monthWeek3[0], week3JobCount);

        }
        if (i == 4) {
            WeekViseJobCount(monthWeek4[0], week4JobCount);
            BindWeekViseJobCount(monthWeek4[0], week4JobCount);
            GenerateAdditionalRaw(table, monthWeek4[0], week4JobCount);
        }
        if (i == 5) {
            WeekViseJobCount(monthWeek5[0], week5JobCount);
            BindWeekViseJobCount(monthWeek5[0], week5JobCount);
            GenerateAdditionalRaw(table, monthWeek5[0], week5JobCount);
        }
        if (i == 6) {
            WeekViseJobCount(monthWeek6[0], week6JobCount);
            BindWeekViseJobCount(monthWeek6[0], week6JobCount);
            GenerateAdditionalRaw(table, monthWeek6[0], week6JobCount);
        }
    }

    for (var i = 1; i <= 6; i++) {
        if (i == 1) {
            BindWeekViseJobData(monthWeek1, table);
        }
        if (i == 2) {
            BindWeekViseJobData(monthWeek2, table);
        }
        if (i == 3) {
            BindWeekViseJobData(monthWeek3, table);
        }
        if (i == 4) {
            BindWeekViseJobData(monthWeek4, table);
        }
        if (i == 5) {
            BindWeekViseJobData(monthWeek5, table);
        }
        if (i == 6) {
            BindWeekViseJobData(monthWeek6, table);
        }
    }

    //$(".monthJobs").live('click', function () {
    //    showJobSchedulingDetail($(this).attr('jobSchedulingID'));
    //});

    $(".monthJobs").click(function (e) {
        e.stopPropagation();
        e.cancleBubble = true;
        if (!($(e.target).is('a')) && !($(e.target).is('svg')) && !($(e.target).is('g')) && !($(e.target).is('path')))
            showJobSchedulingDetail($(this).attr('jobSchedulingID'), $(this).attr('jobTitle'));
    });

    ResetHeightAndMakeDivDroppableMonth();

    for (var i = 1; i <= 6; i++) {
        if (i == 1) {
            SetIsHideAttribute(table, monthWeek1[0], week1JobCount);
        }
        if (i == 2) {
            SetIsHideAttribute(table, monthWeek2[0], week2JobCount);
        }
        if (i == 3) {
            SetIsHideAttribute(table, monthWeek3[0], week3JobCount);
        }
        if (i == 4) {
            SetIsHideAttribute(table, monthWeek4[0], week4JobCount);
        }
        if (i == 5) {
            SetIsHideAttribute(table, monthWeek5[0], week5JobCount);
        }
        if (i == 6) {
            SetIsHideAttribute(table, monthWeek6[0], week6JobCount);
        }
    }
}

function SetIsHideAttribute(table, monthWeek, weekJobCount) {
    var startDate = monthWeek.startDate;
    var endDate = monthWeek.endDate;
    var startNum = moment(startDate, "YYYY-MM-DD").format("D");
    var endNum = moment(endDate, "YYYY-MM-DD").format("D");

    var totalMaxJobCount = monthWeek.totalMaxJobCount;
    var displayMaxJobCount = monthWeek.displayMaxJobCount;

    var isHide = true;
    ShowHiddenRow(table, startNum, endNum, startDate, displayMaxJobCount, null, isHide);


    //$.each($(table).find('tr'), function (index, ele) {
    //    if (displayMaxJobCount > 4 && $(ele).attr('rowstartendnum') == startNum + '_' + endNum + '_' + displayMaxJobCount && !($(ele).attr('name') && $(ele).attr('name') == "hidden")) {
    //        $(this).hide();
    //        displayMaxJobCount++;

    //        var total = $(ele).attr('rowstartendnum') == startNum + '_' + endNum + '_' + displayMaxJobCount;
    //        if (total == totalMaxJobCount)
    //            return false;
    //    }
    //});
}

function BindViewMore(i, totalMaxJobCount, displayMaxJobCount, trEle, startNum, endNum, index, viewMoreStartDate, table) {
    if (i == displayMaxJobCount - 1) {
        //if (totalMaxJobCount != displayMaxJobCount) {

        //bind view more
        var viewMoreTR = createNewElement('tr');
        viewMoreTR[0].innerHTML = trEle.html();
        viewMoreTR.attr('rowstartendnum', startNum + '_' + endNum + '_' + (i + 1));
        viewMoreTR.attr('name', 'hidden');
        viewMoreTR.insertAfter($('.monthgrid tbody tr:nth(' + (index) + ')'));

        //find td and bind view more text
        BindAllViewMoreForWeek(viewMoreStartDate, viewMoreTR, startNum, endNum, displayMaxJobCount, totalMaxJobCount, table);

        //for (var c = 0; c < viewMoreStartDate.length; c++) {

        //    $.each($(viewMoreTR[0]).find('td'), function (v, el) {
        //        if ($(el).attr("startdate") && moment($(el).attr("startdate")).isSame(viewMoreStartDate[c])) {
        //            var name = 'h_' + moment(viewMoreStartDate[c], "YYYY-MM-DD").format("D");
        //            $(el)[0].innerHTML = '<div class="redmoreShift" isHide="1" startNum=' + startNum + ' endNum=' + endNum + ' startDate=' + viewMoreStartDate[c] + ' displayMaxJobCount=' + displayMaxJobCount + '>View More</div>';
        //            $(el).attr('name', name);
        //        }
        //    });
        //}


        //}
    }
}

function BindAllViewMoreForWeek(viewMoreStartDate, trEle, startNum, endNum, displayMaxJobCount, totalMaxJobCount, table) {

    if (viewMoreStartDate != null && viewMoreStartDate.length > 0) {
        $.each(viewMoreStartDate, function (i, e) {

            $.each($(trEle).find('td'), function (v, el) {
                if ($(el).attr("startdate") && moment($(el).attr("startdate")).isSame(e.startDate)) {
                    var name = 'h_' + moment(e.startDate, "YYYY-MM-DD").format("D");
                    $(el)[0].innerHTML = '<div class="redmoreShift" isHide="1" totalMaxJobCount=' + totalMaxJobCount + '  startNum=' + startNum + ' endNum=' + endNum + ' startDate=' + e.startDate + ' displayMaxJobCount=' + displayMaxJobCount + '>View More</div>';
                    //$(el)[0].innerHTML = '<div class="redmoreShift" isHide="1"  startNum=' + startNum + ' endNum=' + endNum + ' startDate=' + e.startDate + ' displayMaxJobCount=' + displayMaxJobCount + '>View More</div>';
                    $(el).attr('name', name);
                    $(el).attr('isViewMore', 1);
                    $(el).attr('totalMaxJobCount', e.totalMaxJobCount);
                }
            });

        });

        $(".redmoreShift").click(function (e) {
            e.stopPropagation();
            ShowHiddenRow(table, $(this).attr('startNum'), $(this).attr('endNum'), $(this).attr('startDate'), $(this).attr('displayMaxJobCount'), $(this));
        });
    }

    //for (var c = 0; c < viewMoreStartDate.length; c++) {

    //    $.each($(trEle).find('td'), function (v, el) {
    //        if ($(el).attr("startdate") && moment($(el).attr("startdate")).isSame(viewMoreStartDate[c])) {
    //            var name = 'h_' + moment(viewMoreStartDate[c], "YYYY-MM-DD").format("D");
    //            $(el)[0].innerHTML = '<div class="redmoreShift" isHide="1" startNum=' + startNum + ' endNum=' + endNum + ' startDate=' + viewMoreStartDate[c] + ' displayMaxJobCount=' + displayMaxJobCount + '>View More</div>';
    //            $(el).attr('name', name);
    //            $(el).attr('isViewMore', 1);
    //            $(el).attr('totalMaxJobCount', totalMaxJobCount);
    //        }
    //    });
    //}


}

function GenerateAdditionalRaw(table, monthWeek, weekJobCount) {
    var startDate = monthWeek.startDate;
    var endDate = monthWeek.endDate;
    var startNum = moment(startDate, "YYYY-MM-DD").format("D");
    var endNum = moment(endDate, "YYYY-MM-DD").format("D");

    var totalMaxJobCount = monthWeek.totalMaxJobCount;
    var displayMaxJobCount = monthWeek.displayMaxJobCount;

    var viewMoreStartDate = [];

    if (displayMaxJobCount != totalMaxJobCount) {
        $.each(weekJobCount, function (j, ele) {
            //if (displayMaxJobCount >= 4 && ele.totalJobCount > 4 && ele.totalJobCount > displayMaxJobCount)
            if (ele.totalJobCount > 4 && ele.totalJobCount > displayMaxJobCount)
                viewMoreStartDate.push({ startDate: ele.startDate, totalJobCount: ele.totalJobCount });
        });
    }

    if (displayMaxJobCount <= 4 && totalMaxJobCount > displayMaxJobCount) {
        var trEle = $(table).find('tr[rowstartendnum=' + startNum + '_' + endNum + '_' + 4 + ']');
        //not generate row only bind view more on 4th index
        BindAllViewMoreForWeek(viewMoreStartDate, trEle, startNum, endNum, displayMaxJobCount, totalMaxJobCount, table);

    }
        //else if (displayMaxJobCount == 5 && totalMaxJobCount > displayMaxJobCount) {
        //    var trEle = $(table).find('tr[rowstartendnum=' + startNum + '_' + endNum + '_' + 4 + ']');
        //    BindViewMore(4, totalMaxJobCount, displayMaxJobCount, trEle, startNum, endNum, 5, viewMoreStartDate, table);
        //}
    else {
        for (var i = 5; i < displayMaxJobCount; i++) {

            var rowstartendnum = startNum + '_' + endNum + '_' + i;
            var trEle = createNewElement('tr');
            var previousTr;
            var index;
            $.each($(table).find('tr'), function (index, ele) {
                if ($(ele).attr('rowstartendnum') == startNum + '_' + endNum + '_' + (i - 1)) {
                    previousTr = $(ele);
                    index = $(ele).index();
                    trEle[0].innerHTML = previousTr.html();
                    trEle.attr('rowstartendnum', rowstartendnum);
                    trEle.insertAfter($('.monthgrid tbody tr:nth(' + index + ')'));

                    if (displayMaxJobCount != totalMaxJobCount) {
                        BindViewMore(i, totalMaxJobCount, displayMaxJobCount, trEle, startNum, endNum, index + 1, viewMoreStartDate, table);
                    }

                    //if (i == totalMaxJobCount - 1) {
                    //    if (totalMaxJobCount != displayMaxJobCount) {

                    //        //bind view more
                    //        var viewMoreTR = createNewElement('tr');
                    //        viewMoreTR[0].innerHTML = trEle.html();
                    //        viewMoreTR.attr('rowstartendnum', startNum + '_' + endNum + '_' + (i + 1));
                    //        viewMoreTR.attr('name', 'hidden');
                    //        viewMoreTR.insertAfter($('.monthgrid tbody tr:nth(' + (index + 1) + ')'));
                    //        //find td and bind view more text
                    //        for (var c = 0; c < viewMoreStartDate.length; c++) {

                    //            $.each($(viewMoreTR[0]).find('td'), function (v, el) {
                    //                if ($(el).attr("startdate") && moment($(el).attr("startdate")).isSame(viewMoreStartDate[c])) {
                    //                    var name = 'h_' + moment(viewMoreStartDate[c], "YYYY-MM-DD").format("D");
                    //                    $(el)[0].innerHTML = '<div class="redmoreShift" isHide="1" startNum=' + startNum + ' endNum=' + endNum + ' startDate=' + viewMoreStartDate[c] + ' displayMaxJobCount=' + displayMaxJobCount + '>View More</div>';
                    //                    $(el).attr('name', name);
                    //                }
                    //            });
                    //        }

                    //        $(".redmoreShift").live('click', function () {
                    //            ShowHiddenRow(table, $(this).attr('startNum'), $(this).attr('endNum'), $(this).attr('startDate'), $(this).attr('displayMaxJobCount'), $(this));
                    //        });

                    //    }
                    //}
                }
            });
        }
    }

    //}
}


function GetViewMoreData(weekJobCount, startDate) {
    var data = null;

    $.each(weekJobCount, function (index, ele) {
        if (startDate == ele.startDate) {
            data = ele.jobData;
            return true;
        }
    });

    return data;
}

function ShowHiddenRow(table, startNum, endNum, startDate, displayMaxJobCount, obj, isHide) {
    if (obj != null && obj.attr('isHide') == 1) {

        var data = null;

        for (var i = 1; i <= 6; i++) {

            if (i == 1) {
                data = GetViewMoreData(week1JobCount, startDate);
                if (data != null)
                    break;
            }
            if (i == 2) {
                data = GetViewMoreData(week2JobCount, startDate);
                if (data != null)
                    break;
            }
            if (i == 3) {
                data = GetViewMoreData(week3JobCount, startDate);
                if (data != null)
                    break;
            }
            if (i == 4) {
                data = GetViewMoreData(week4JobCount, startDate);
                if (data != null)
                    break;
            }
            if (i == 5) {
                data = GetViewMoreData(week5JobCount, startDate);
                if (data != null)
                    break;
            }
            if (i == 6) {
                data = GetViewMoreData(week6JobCount, startDate);
                if (data != null)
                    break;
            }
        }

        if (data != null) {
            var viewMoreTable = createNewElement('table'), $viewMoretbody = createNewElement('tbody');
            viewMoreTable.attr('class', 'viewMoreTable');
            $.each(data, function (index, ele) {

                var $viewMoreTr = createNewElement('tr');
                var $childtd = AddRequiredTD(ele, 'blueShifts', ele, 1, true, 0, true);
                $viewMoreTr.append($childtd);

                viewMoreTable.append($viewMoreTr);
            });


            $(".pop-viewmore").find('.viewMoreTable').remove();
            $(".pop-viewmore").append(viewMoreTable);

            //$("#mCSB_1_container").append(viewMoreTable);

            $(".monthJobs").click(function () {
                showJobSchedulingDetail($(this).attr('jobSchedulingID'));
            });

            $(".monthDroppable").droppable({
                tolerance: "pointer",
                drop: function (event, ui) {
                    monthDroppableDrop(event, ui);
                }
            });

            $('.monthJobs').draggable({
                revert: "invalid",
                scroll: false,
                start: function (event, ui) {
                    $(this).css('z-index', '10');
                    var result = monthDraggableStart(event, ui, $(this));
                    return result;
                },
                stop: function (event, ui) {
                    $(this).css('z-index', '0');
                    monthDraggableStop(event, ui);
                },
            });

            var top1 = $('.monthgrid tr.datenumrow').find('td[startdate=' + startDate + ']').parent().next().offset().top;
            var left1 = $('.monthgrid tr.datenumrow').find('td[startdate=' + startDate + ']').offset().left - 5;

            //var totalCount = $(obj).parent().attr('totalMaxJobCount');

            var totalCount = $(obj).attr('totalMaxJobCount');
            //var height1 = obj.offset().top - top1 + 35;

            var height1 = totalCount * 36;

            var viewMoreTop = $(obj).parent().offset().top;
            var footerTop = $("#footer").offset().top;
            var findHeightWithViewMore = top1 + height1;
            var newTop;

            if (findHeightWithViewMore > footerTop) {
                newTop = footerTop - height1;
                $('.pop-viewmore').css({ top: newTop, left: left1, width: 220, display: 'block' });
                //$('#myscrl').css({ top: newTop, left: left1, width: 220, display: 'block' });
            }
            else
                $('.pop-viewmore').css({ top: top1, left: left1, width: 220, display: 'block' });
            //$('#myscrl').css({ top: top1, left: left1, width: 220, display: 'block' });


            //if (footerTop - viewMoreTop < 100)
            //{
            //    var newTop = viewMoreTop - height1 + 36;
            //    $('.pop-viewmore').css({ top: newTop, left: left1, height: height1, width: 220, display: 'block' });
            //}
            //else
            //    $('.pop-viewmore').css({ top: top1, left: left1, height: height1, width: 220, display: 'block' });


            //$('.pop-viewmore').css({ top: top1, left: left1, width: 220, display: 'block' });

            $('.monthJobs').each(function () {
                var $this = $(this);
                $this.popover({
                    trigger: 'hover',
                    placement: 'top auto',
                    html: true,
                    content: $this.attr('title'),
                    container: 'body'
                });
            });

        }
        //$("#footer").css("position", "relative");
    }
}

function BindWeekViseJobData(week, table) {
    $.each(week[0].jobData, function (i, e) {
        var startNum = moment(e.startDate, "YYYY-MM-DD").format("D");
        var requireDiff = GetDiffBetweenTwoDate(e.startDate, e.endDate);
        var requiredTD = requireDiff + 1;
        var isRequireTD = false;
        var isContinue = true;

        $.each($(table).find('tr'), function (index, ele) {
            if (isContinue) {
                $.each($(ele).find('td'), function (k, element) {

                    if ($(element).attr('isData') && $(element).attr('isData') == 0 && moment($(element).attr('startdate')).isSame(e.startDate) && !($(element).attr('isViewMore'))) {
                        if (requiredTD > 1) {
                            var nextTD = $(element).next();

                            for (var m = 0; m < requiredTD - 1; m++) {
                                if (nextTD.attr("isData") == 0) {
                                    isRequireTD = true;
                                    nextTD = $(nextTD).next();
                                }
                                else {
                                    isRequireTD = false;
                                    return false;
                                }
                            }
                        }
                        else
                            isRequireTD = true;

                        if (isRequireTD) {
                            var $childtd = AddRequiredTD(e, 'blueShifts', e, requiredTD, true);
                            if (requiredTD > 1) {
                                for (var i = 0; i < requiredTD - 1; i++) {
                                    $(element).next().remove();
                                }
                            }
                            $(element).replaceWith($childtd);



                            isContinue = false;
                            return true;
                        }
                    }

                });
            }
        });


    });


}

function BindWeekArray(firstDate) {
    var weekStartDate = moment(firstDate).format("YYYY-MM-DD");
    var weekEndDate = moment(new Date(moment(firstDate).endOf('week').year(), moment(firstDate).endOf('week').month(), moment(firstDate).endOf('week').date()));
    weekEndDate = weekEndDate.format("YYYY-MM-DD");

    var lastDateOfMonth = moment(new Date(moment(firstDate).endOf('month').year(), moment(firstDate).endOf('month').month(), moment(firstDate).endOf('month').date())).format("YYYY-MM-DD");

    for (var i = 1; i <= 6; i++) {
        if (i == 1)
            monthWeek1.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });
        if (i == 2)
            monthWeek2.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });
        if (i == 3)
            monthWeek3.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });
        if (i == 4)
            monthWeek4.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });
        if (i == 5)
            monthWeek5.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });
        if (i == 6)
            monthWeek6.push({ id: 1, startDate: weekStartDate, endDate: weekEndDate, jobData: [], totalMaxJobCount: 0, displayMaxJobCount: 0 });

        weekStartDate = moment(weekEndDate).add(1, "day").format("YYYY-MM-DD");

        if (moment(lastDateOfMonth).isSame(weekStartDate))
            weekEndDate = weekStartDate;
        else if (moment(weekStartDate).isBefore(lastDateOfMonth)) {
            weekEndDate = moment(new Date(moment(weekStartDate).endOf('week').year(), moment(weekStartDate).endOf('week').month(), moment(weekStartDate).endOf('week').date())).format("YYYY-MM-DD");
            if (moment(lastDateOfMonth).isBefore(weekEndDate))
                weekEndDate = lastDateOfMonth;
        }
        else {
            weekStartDate = null;
            weekEndDate = null;
        }
    }
}

function insertDataIntoWeekArray(data) {
    var weekArray;
    for (var i = 1; i <= 6; i++) {
        if (i == 1) {
            weekArray = insertJobDataIntoWeekArray(monthWeek1, data);
            if (weekArray != null)
                monthWeek1 = weekArray;
        }
        if (i == 2) {
            weekArray = insertJobDataIntoWeekArray(monthWeek2, data);
            if (weekArray != null)
                monthWeek2 = weekArray;
        }
        if (i == 3) {
            weekArray = insertJobDataIntoWeekArray(monthWeek3, data);
            if (weekArray != null)
                monthWee3 = weekArray;
        }
        if (i == 4) {
            weekArray = insertJobDataIntoWeekArray(monthWeek4, data);
            if (weekArray != null)
                monthWeek4 = weekArray;
        }
        if (i == 5) {
            weekArray = insertJobDataIntoWeekArray(monthWeek5, data);
            if (weekArray != null)
                monthWeek5 = weekArray;
        }
        if (i == 6) {
            weekArray = insertJobDataIntoWeekArray(monthWeek6, data);
            if (weekArray != null)
                monthWeek6 = weekArray;
        }
    }
}

function insertJobDataIntoWeekArray(week, data) {
    var newStartDate = null, newEndDate = null;
    $.each(week, function (index, ele) {
        if (ele.startDate != null && ele.endDate != null) {
            if (moment(data.startDate).isBefore(ele.startDate) && moment(ele.endDate).isBefore(data.endDate) ||
            moment(data.startDate).isBefore(ele.startDate) && moment(ele.endDate).isSame(data.endDate) ||
            moment(data.startDate).isSame(ele.startDate) && moment(ele.endDate).isSame(data.endDate) ||
            moment(data.startDate).isSame(ele.startDate) && moment(ele.endDate).isBefore(data.endDate)) {
                newStartDate = ele.startDate;
                newEndDate = ele.endDate;
            }

            else if (moment(data.startDate).isSame(ele.startDate) && moment(data.endDate).isSame(ele.startDate) ||
                moment(data.startDate).isSame(ele.startDate) && moment(data.endDate).isBefore(ele.endDate) ||
                moment(data.startDate).isBefore(ele.startDate) && moment(ele.startDate).isBefore(data.endDate)) {
                newStartDate = ele.startDate;
                newEndDate = data.endDate;
            }

            else if (moment(data.startDate).isAfter(ele.startDate) && moment(data.endDate).isBefore(ele.endDate)) {
                newStartDate = data.startDate;
                newEndDate = data.endDate;
            }
            else if (moment(ele.startDate).isBefore(data.startDate) && moment(ele.endDate).isSame(data.endDate) ||
                moment(data.startDate).isBefore(ele.endDate) && moment(ele.endDate).isBefore(data.endDate)) {
                newStartDate = data.startDate;
                newEndDate = ele.endDate;
            }
            else if (moment(data.endDate).isSame(ele.endDate) && moment(data.startDate).isSame(ele.endDate) ||
                moment(ele.endDate).isBefore(data.endDate) && moment(data.startDate).isSame(ele.endDate)) {
                newStartDate = data.startDate;
                newEndDate = data.startDate;
            }
            else if (moment(data.endDate).isSame(ele.startDate) && moment(data.startDate).isBefore(ele.startDate)) {
                newStartDate = ele.startDate;
                newEndDate = ele.startDate;
            }
            else {
                newStartDate = null;
                newEndDate = null;
            }
            return false;
        }
    });
    if (newStartDate != null && newEndDate != null) {
        week[0].jobData.push({
            startDate: moment(newStartDate).format("YYYY-MM-DD"), actualStartDate: data.startDate, actualEndDate: data.endDate, weekStartDate: week[0].startDate,
            refNum: data.refNum, priority: data.priority, jobLocation: data.jobLocation, client: data.client, jobStage: data.jobStage,
            weekEndDate: week[0].endDate, endDate: moment(newEndDate).format("YYYY-MM-DD"), startTime: data.startTime, endTime: data.endTime, id: data.id, userID: data.userID,
            label: data.label, jobID: data.jobID, jobTitle: data.jobTitle, SESCName: data.SESCName, fullLabel: data.fullLabel, status: data.status, IsPVDUser: data.IsPVDUser, IsSWHUser: data.IsSWHUser, ID: data.ID
        });
        return week;
    }
    else
        return null;
}

function findJobScheduleCategoryVise(category, data, $tbody, currentDate, color, objElectrician) {
    var isExist = false;

    $.each(category, function (k, catData) {
        if (catData.id == data.id) {
            var requireDiff = GetDiffBetweenTwoDate(catData.startDate, catData.endDate);
            var requiredTD = requireDiff + 1;

            var totalTD;
            //if (requiredTD > 1)
            //{
            var totalDiff = GetDiffBetweenTwoDate(catData.minStartDate, catData.maxEndDate);
            totalTD = totalDiff + 1;
            //}

            var findTd = $tbody.find('[name=w_' + data.userID + '_' + parseInt(moment(catData.minStartDate, "YYYY-MM-DD").format("D")) + ']')

            if ($(findTd).attr("colspan") && parseInt($(findTd).attr("colspan")) > 0) {
            }
            else {
                var removeTotalTD = totalTD;
                while (removeTotalTD > 1) {
                    $(findTd).next().remove();
                    removeTotalTD = removeTotalTD - 1;
                }
                $(findTd).attr('colspan', totalTD);
            }

            var findChildTable = $tbody.find('[name=w_' + data.userID + '_' + parseInt(moment(catData.minStartDate, "YYYY-MM-DD").format("D")) + ']').find('table');
            var isRequireTD = false;
            var isContinue = true;

            if (findChildTable.find('tr').length > 0) {
                $.each(findChildTable.find('tr'), function (i, e) {
                    if (isContinue) {
                        if ($(e).attr('id')) {
                            $.each($(e).find('td'), function (index, ele) {
                                if ($(ele).attr("isData") == 0 && moment($(ele).attr("StartDate")).isSame(catData.startDate)) {
                                    if (requiredTD > 1) {
                                        var nextTD = $(ele).next();

                                        for (var i = 0; i < requiredTD - 1; i++) {
                                            if (nextTD.attr("isData") == 0) {
                                                isRequireTD = true;
                                                nextTD = $(nextTD).next();
                                            }
                                            else {
                                                isRequireTD = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                        isRequireTD = true;

                                    return isRequireTD;
                                }
                            });
                            if (isRequireTD) {
                                // add

                                var width = (100 / totalTD);

                                var $childtd = AddRequiredTD(data, color, catData, requiredTD, false, width);
                                $.each($(e).find('td'), function (index, ele) {
                                    if (moment($(ele).attr("StartDate")).isSame(catData.startDate)) {
                                        if (requiredTD > 1) {
                                            for (var i = 0; i < requiredTD - 1; i++) {
                                                $(ele).next().remove();
                                            }
                                        }

                                        $(ele).replaceWith($childtd);
                                        isContinue = false;
                                    }
                                });
                            }
                        }
                        else {
                            $(e).remove();
                        }
                    }
                });
            }
            if (!isRequireTD) {
                // new create
                var $childtr = createNewElement('tr');
                $childtr.attr("Id", 'w_' + data.userID + '_' + parseInt(moment(catData.startDate, "YYYY-MM-DD").format("D")) + '_' + data.id);

                var width = (100 / totalTD);

                var diffBetweenTwoDate = GetDiffBetweenTwoDate(catData.minStartDate, catData.startDate);
                if (diffBetweenTwoDate == 0) {
                    var $childtd = AddRequiredTD(data, color, catData, requiredTD, false, width);
                    $childtr.append($childtd);

                    var currDate = moment(data.endDate).add(1, "day");
                    if (totalTD > requiredTD) {
                        $childtr = AddExtraTdWithCondition($childtr, totalTD - requiredTD, currDate, data, width + '%');
                    }
                }
                else {
                    var currDate = moment(catData.minStartDate);
                    $childtr = AddExtraTdWithCondition($childtr, diffBetweenTwoDate, currDate, data, width + '%');
                    var $childtd = AddRequiredTD(data, color, catData, requiredTD, false, width);

                    $childtr.append($childtd);

                    var currDate = moment(catData.endDate).add(1, "day");
                    if (totalTD > diffBetweenTwoDate + requiredTD) {
                        $childtr = AddExtraTdWithCondition($childtr, totalTD - (diffBetweenTwoDate + requiredTD), currDate, data, width + '%');
                    }
                }
                findChildTable.append($childtr);
            }

            //$childtd = AddDroppableDiv($childtd, requiredTD);

            isExist = true;
            return isExist;
        }
    });
    return isExist;
}

function AddExtraTdWithCondition($childtr, tdLength, currDate, data, width) {
    var date = currDate;
    for (var i = 0; i < tdLength; i++) {
        var newDate = date.format("YYYY-MM-DD")
        var $childtd = AddExtraTd(newDate, data, width);
        $childtr.append($childtd);
        date = date.add(1, "day");
    }
    return $childtr;
}

function AddExtraTd(newDate, data, width) {
    var $childtd = createNewElement('td');

    $childtd.addClass('weekDroppable');
    $childtd.html('<div></div>');
    $childtd.attr('StartDate', newDate);
    $childtd.attr('EndDate', newDate);
    $childtd.css('width', width);
    $childtd.attr('userID', data.userID);
    $childtd.attr('userTypeID', data.userTypeID);
    $childtd.attr('isData', 0);
    $childtd.attr('IsPVDUser', (data.IsPVDUser == undefined ? false : data.IsPVDUser));
    $childtd.attr('IsSWHUser', (data.IsSWHUser == undefined ? false : data.IsSWHUser));
    //$childtd.css('border-top', '0px !important');
    //$childtd.css('border-bottom', '0px !important');

    return $childtd;
}

function AddRequiredTD(data, color, catData, requiredTD, isMonth, width, isViewMore) {
    if (parseInt(data.status) == 1)
        color = "blueShifts";
    else if (parseInt(data.status) == 2)
        color = "greenShifts";
    else if (parseInt(data.status) == 3)
        color = "orangeShifts";

    var $childtd = createNewElement('td');
    var time;
    if (data.endTime == null) {
        time = data.startTime;
    }
    else {
        time = data.startTime + ' - ' + data.endTime;
    }

    //var label = data.label;

    //var label = data.refNum + "-" + data.client;

    var label = data.refNum + ((data.fullLabel != null && data.fullLabel != '') ? " - " + data.fullLabel : "");

    //var label = data.label.replace("\"", "&quot;");

    if (label != null) {
        //if (requiredTD == 1 || requiredTD == undefined) {
        //    label = label.length > 20 ? label.substring(0, 5) + '..' : label;
        //}
        //else if (requiredTD == 2) {
        //    label = label.length > 40 ? label.substring(0, 10) + '..' : label;
        //}
        //else if (requiredTD == 3) {
        //    label = label.length > 60 ? label.substring(0, 15) + '..' : label;
        //}
        //else if (requiredTD == 4) {
        //    label = label.length > 80 ? label.substring(0, 20) + '..' : label;
        //}
        //else if (requiredTD == 5) {
        //    label = label.length > 100 ? label.substring(0, 25) + '..' : label;
        //}
        //else if (requiredTD == 6) {
        //    label = label.length > 120 ? label.substring(0, 30) + '..' : label;
        //}
        //else if (requiredTD == 7) {
        //    label = label.length > 140 ? label.substring(0, 40) + '..' : label;
        //}
    }
    else
        label = '';

    //var jobReference = 'S6598', jobLocation = '55 Edwards Road, Jackass Flat, Victoria, 3556', priority = 'Normal', client = 'DARREN MORESI';


    var jobReference = data.refNum, jobLocation = data.jobLocation, priority = data.priority, client = data.client;

    //var toolTipTile = (data.fullLabel != null && data.fullLabel != '') ? time + '<br />' + data.fullLabel.replace("\"", "&quot;") + '<br />' + 'Assigned to: ' + '<span>' + data.SESCName.replace("\"", "&quot;") + '</span>' : time.replace("\"", "&quot;") + '<br />' + 'Assigned to: ' + '<span>' + data.SESCName.replace("\"", "&quot;") + '</span>';

    var toolTipTile = (data.fullLabel != null && data.fullLabel != '') ? time + '<br />' + data.fullLabel.replace(/"/g, "&quot;") + '<br />' + 'Assigned to: ' + '<span>' + data.SESCName ? data.SESCName.replace(/"/g, "&quot;") : '' + '</span>' : time.replace(/"/g, "&quot;") + '<br />' + 'Assigned to: ' + '<span>' + data.SESCName.replace(/"/g, "&quot;") + '</span>';


    var toolTipBody = 'Status: ' + '<span>' + data.jobStage.replace(/"/g, "&quot;") + '</span>' + '<br />' + 'Job Reference: ' + '<span>' + jobReference.replace(/"/g, "&quot;") + '</span>' + '<br />' + 'Job Location: ' + '<span>' + jobLocation.replace(/"/g, "&quot;") + '</span>' + '<br />' + 'Priority: ' + '<span>' + priority.replace(/"/g, "&quot;") + '</span>' + '<br />' + 'Client: ' + '<span>' + client.replace(/"/g, "&quot;") + '</span>';
    if (isMonth) {
        if (data.endTime == null) {
            time = data.startTime;
        }
        $childtd.html('<div class="monthJobs view-job-detail ' + color + '" jobSchedulingID="' + data.id + '" startTime="' + data.startTime + '" title="' + toolTipTile + '" data-content="' + toolTipBody + '" data-toggle="popover" ActualStartDate="' + data.actualStartDate + '" ActualEndDate="' + data.actualEndDate + '" StartDate="' + data.startDate + '" EndDate="' + data.endDate + '" userID="' + data.userID + '" status="' + data.status + '" jobID="' + data.jobID + '" ID="' + data.Id + '" jobTitle="' + data.refNum.replace(/"/g, "&quot;") + '"><span class="weekShiftTime">' + time + ' ' + label + '</span><a href="' + '/Job/Index?id=' + data.ID + '" target="_blank" id="btnOpenJobDetail" class="pull-right btnOpenJobDetail"><svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Capa_1" x="0px" y="0px" width="512px" height="512px" viewBox="0 0 459 459" style="enable-background:new 0 0 459 459;" xml:space="preserve"><g><g id="share"><path d="M459,216.75L280.5,38.25v102c-178.5,25.5-255,153-280.5,280.5C63.75,331.5,153,290.7,280.5,290.7v104.55L459,216.75z" fill="#14773c"/></g></g></svg></a>' + "</div>");

        $childtd.attr('class', 'monthDroppable');

        if (isViewMore) {
            $childtd.css('font-size', '12px');
            $childtd.addClass('viewMoreClass');
        }

        if (requiredTD == 1 || requiredTD == undefined)
            $childtd.find('.monthJobs').css("width", "97%");
        else if (requiredTD == 2)
            $childtd.find('.monthJobs').css("width", "98.5%");
        else if (requiredTD == 3)
            $childtd.find('.monthJobs').css("width", "98.5%");
        else if (requiredTD == 4 || requiredTD == 5 || requiredTD == 6 || requiredTD == 7)
            $childtd.find('.monthJobs').css("width", "99.5%");
    }
    else {
        if (data.endTime == null) {
            time = data.startTime;
        }

        $childtd.html('<div class="weekJobs view-job-detail ' + color + '" jobSchedulingID="' + data.id + '" startTime="' + data.startTime + '" title="' + toolTipTile + '" data-content="' + toolTipBody + '" data-toggle="popover" StartDate="' + data.startDate + '" EndDate="' + data.endDate + '" userID="' + data.userID + '" status="' + data.status + '" jobID="' + data.jobID + '" ID="' + data.ID + '" jobType="' + data.jobType + '" IsClassic="' + data.IsClassic + '" jobTitle="' + data.refNum.replace(/"/g, "&quot;") + '"><span class="weekShiftTime">' + time + ' ' + label + '</span><a href="' + '/Job/Index?id=' + data.ID + '" target="_blank" id="btnOpenJobDetail" class="pull-right btnOpenJobDetail"><svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Capa_1" x="0px" y="0px" width="512px" height="512px" viewBox="0 0 459 459" style="enable-background:new 0 0 459 459;" xml:space="preserve"><g><g id="share"><path d="M459,216.75L280.5,38.25v102c-178.5,25.5-255,153-280.5,280.5C63.75,331.5,153,290.7,280.5,290.7v104.55L459,216.75z" fill="#14773c"/></g></g></svg></a>' + "</div>");

    }

    if (!isMonth) {
        width = (width * requiredTD) + "%";

        if (width)
            $childtd.css('width', width);


    }

    $childtd.attr('colspan', requiredTD);
    $childtd.attr('StartDate', catData.startDate);
    $childtd.attr('EndDate', catData.endDate);
    $childtd.attr('userID', data.userID);
    $childtd.attr('isData', 1);
    $childtd.attr('userTypeID', data.userTypeID);
    $childtd.attr('IsPVDUser', (data.IsPVDUser == undefined ? false : data.IsPVDUser));
    $childtd.attr('IsSWHUser', (data.IsSWHUser == undefined ? false : data.IsSWHUser));
    return $childtd;
}

function monthDraggableStart(event, ui, obj) {
    //$(this).data("origPosition", $(this).position());


    if (isOnlyView) {
        return false;
    }
    else {
        $("#divSchedular").removeClass('table-responsive');
        $("#divSchedular").addClass('table');

        // if invalid then set it to original position
        obj.data("origPosition", obj.position());

        $(".monthgrid").find('tr').each(function (i, e) {
            $(e).find('.monthDroppable').each(function (index, ele) {
                if ($(ele).find('.parentDropDiv').length > 0) {
                    $($(ele).find('.parentDropDiv')).css('z-index', '2');
                }
            });
        });
    }


    $(event.toElement).one('click', function (e) { e.stopImmediatePropagation(); });
}

function monthDraggableStop(event, ui) {

    $(".monthgrid").find('tr').each(function (i, e) {
        $(e).find('.monthDroppable').each(function (index, ele) {
            if ($(ele).find('.parentDropDiv').length > 0) {
                $($(ele).find('.parentDropDiv')).css('z-index', '-1');
            }
        });
    });

    $("#divSchedular").removeClass('table');
    $("#divSchedular").addClass('table-responsive');
}

function monthDroppableDrop(event, ui) {
    onDropCheckCount(false, event, ui);
}


function onDropCheckCount(isWeek, event, ui) {
    var result = weekMonthDroppable(isWeek, event, ui);
    if (!result) { //failed or if job is assigned more then 3 to particular electrician.        
        if (viewType == "W") {
            if (SEUserAssignSWHJob) {
                SEUserAssignSWHJob = false;
                alert("Can't assign SWH job to SE User.");
                refreshView();
                return;
            }

            if (SWHUserAssignPVDJob) {
                SWHUserAssignPVDJob = false;
                alert("Can't assign PVD job to SWH User.");
                refreshView();
                return;
            }
        }

        $("#yesNotification").unbind('click');
        $("#noNotification").unbind('click');

        $("#yesNotification").click(function () {

            var data = $("#alertAssignTime").data('dragData');
            var response = SaveScheduleOnDropAndInsertEdit(data.startDate, data.startTime, data.endDate, data.endTime, data.label, data.detail, data.userId, data.jobId, data.jobSchedulingID, true, data.status, data.isDrop, '', ui, data.jobTitle);
            //$("#notification").modal('hide');
            //if (!response) {
            //    ui.draggable.animate(ui.draggable.data().origPosition, "slow");
            //    return;
            //}
        });

        $("#noNotification").click(function () {
            $("#notification").modal('hide');
            ui.draggable.animate(ui.draggable.data().origPosition, "slow");
            return;
        });

        //$("#notification").modal();
    }
    else {
        ui.draggable.animate(ui.draggable.data().origPosition, "slow");
        return;
    }

}

function weekMonthDroppable(isWeek, event, ui) {

    var userID, newStartDate, newEndDate, startDate, endDate, jobId, startTime, actualStartDate, actualEndDate, oldUserId, status, jobTitle, jobType, userTypeID, isClassic;

    var jobId = ui.draggable.attr("jobID");
    var startTime = ui.draggable.attr("startTime");

    var schedulingId = ui.draggable.attr("jobSchedulingID");
    var status = ui.draggable.attr("status");

    if (isWeek) {
        startDate = ui.draggable.attr("StartDate");
        endDate = ui.draggable.attr("EndDate");
    }
    else {
        actualStartDate = ui.draggable.attr("ActualStartDate");
        actualEndDate = ui.draggable.attr("ActualEndDate");
        startDate = ui.draggable.attr("StartDate");
        endDate = ui.draggable.attr("EndDate");
    }

    newStartDate = $(event.target).attr("StartDate");

    if (isWeek)
        userID = $(event.target).attr("userID");
    else
        userID = ui.draggable.attr("userID");

    if (isWeek)
        oldUserId = ui.draggable.attr('userID');
    else
        oldUserId = ui.draggable.attr("userID");

    jobTitle = ui.draggable.attr("jobTitle");
    jobType = ui.draggable.attr("jobType");
    isClassic = ui.draggable.attr("isClassic");
    targetIsSWHUser = $(event.target).attr("isswhuser");
    targetIsPVDuser = $(event.target).attr("ispvduser");


    if (isWeek)
        userTypeID = $(event.target).attr("usertypeid");
    else
        userTypeID = ui.draggable.attr("usertypeid");

    if (userTypeID == 9)
        targetIsPVDuser = 'true';

    var ischeck = 0;
    if (isWeek && jobType == 1 && targetIsPVDuser != 'true') {
        SWHUserAssignPVDJob = true;
        return false;
    }
    else
        ischeck += 1;

    if (isWeek && jobType == 2 && targetIsSWHUser != 'true') {
        SEUserAssignSWHJob = true;
        return false;
    }


    //if (userTypeID == 7 || userTypeID == 9) {
    //    if (jobType != 1) {
    //        SEUserAssignSWHJob = true;
    //        return false;
    //    }
    //}
    //else if (userTypeID == 10) {
    //    if (jobType != 2) {
    //        SWHUserAssignPVDJob = true;
    //        return false;
    //    }
    //}




    if (startDate == newStartDate && userID == oldUserId)
        return true;
    else {
        if (!isWeek) {
            startDate = actualStartDate;
            endDate = actualEndDate;
        }
    }
    var diff = GetDiffBetweenTwoDate(startDate, endDate);

    if (diff > 0) {
        newEndDate = moment(newStartDate).add(diff, "day");
        newEndDate = newEndDate.format("YYYY-MM-DD");
    }
    else
        newEndDate = newStartDate;

    $(".pop-viewmore").hide();

    return DroppableJobScheduleChangeTimeElectrician(schedulingId, newStartDate, newEndDate, userID, startTime, jobId, status, jobTitle, jobType, userTypeID, isClassic);
}

function categoryViseTDCount(category) {

    var mainStartDate, mainEndDate, totalTdCount = null;

    $.each(category, function (j, ele) {
        mainStartDate = ele.minStartDate;
        mainEndDate = ele.maxEndDate;
        return true;
    });

    var mainDiff = GetDiffBetweenTwoDate(mainStartDate, mainEndDate);
    totalTdCount = mainDiff + 1;
    var cat = [];
    return cat.push({ mainStartDate: mainStartDate, mainEndDate: mainEndDate, totalTdCount: totalTdCount });
}

function electricianCategory(category, startDate, endDate, data) {


    if (category && category.length > 0) {
        var newMinStartDate, newMaxEndDate = null;

        $.each(category, function (i, e) {

            if (moment(startDate).isSame(e.minStartDate) && moment(endDate).isAfter(e.maxEndDate)) { //21 -- 24
                newMinStartDate = startDate;
                newMaxEndDate = endDate;
            }
            else if (moment(startDate).isSame(e.minStartDate) && moment(e.maxEndDate).isAfter(endDate)) { // 21 --22
                newMinStartDate = startDate;
                newMaxEndDate = e.maxEndDate;
            }
            else if (moment(startDate).isSame(e.minStartDate) && moment(endDate).isSame(e.maxEndDate)) { // 21 --23 
                newMinStartDate = startDate;
                newMaxEndDate = endDate;
            }
            else if (moment(startDate).isSame(e.minStartDate) && moment(endDate).isSame(e.minStartDate)) {// 21-21
                newMinStartDate = startDate;
                newMaxEndDate = e.maxEndDate;
            }
            else if (moment(endDate).isSame(e.maxEndDate) && moment(startDate).isSame(e.maxEndDate)) {// 23-23
                newMinStartDate = e.minStartDate;
                newMaxEndDate = endDate;
            }
            else if (moment(startDate).isBefore(e.minStartDate) && moment(endDate).isSame(e.maxEndDate)) {// 19-23
                newMinStartDate = startDate;
                newMaxEndDate = endDate;
            }
            else if (moment(e.minStartDate).isBefore(startDate) && moment(endDate).isSame(e.maxEndDate)) {// 22-23
                newMinStartDate = e.minStartDate;
                newMaxEndDate = endDate;
            }
            else if (moment(startDate).isBefore(e.minStartDate) && moment(endDate).isAfter(e.maxEndDate)) {// 19-24
                newMinStartDate = startDate;
                newMaxEndDate = endDate;
            }
            else if (moment(startDate).isSame(e.minStartDate) && moment(e.minStartDate).isBefore(endDate)) {// 21-22
                newMinStartDate = startDate;
                newMaxEndDate = e.maxEndDate;
            }
            else if (moment(e.maxEndDate).isSame(startDate) && moment(endDate).isAfter(e.maxEndDate)) {// 23-24
                newMinStartDate = e.minStartDate;
                newMaxEndDate = endDate;
            }
            else if (moment(e.minStartDate).isBefore(startDate) && moment(e.maxEndDate).isAfter(endDate)) {// 22-22
                newMinStartDate = e.minStartDate;
                newMaxEndDate = e.maxEndDate;
            }
            else if (moment(endDate).isSame(e.minStartDate) && moment(startDate).isBefore(e.minStartDate)) {// 19-21
                newMinStartDate = startDate;
                newMaxEndDate = e.maxEndDate;
            }
            else if (moment(e.minStartDate).isBefore(startDate) && moment(e.maxEndDate).isBefore(endDate)) {// 22-24
                newMinStartDate = e.minStartDate;
                newMaxEndDate = endDate;
            }
            return false;
        });

        if (newMinStartDate == null && newMaxEndDate == null)
            return null;
        else {
            category.push({ startDate: startDate, minStartDate: newMinStartDate, maxEndDate: newMaxEndDate, endDate: endDate, startTime: data.startTime, endTime: data.endTime, id: data.id, userID: data.userID, label: data.label, jobID: data.jobID });

            $.each(category, function (i, e) {
                e.minStartDate = newMinStartDate,
                e.maxEndDate = newMaxEndDate
            });

            return category;
        }
    }
    else {
        category.push({ startDate: startDate, minStartDate: startDate, maxEndDate: endDate, endDate: endDate, startTime: data.startTime, endTime: data.endTime, id: data.id, userID: data.userID, label: data.label, jobID: data.jobID });
        return category;
    }
}

function GetDateNum(date) {
    return parseInt(moment(date, "YYYY-MM-DD").format("D"));
}

function GetDiffBetweenTwoDate(startDate, EndDate) {
    startDate = moment(startDate, 'YYYY-MM-DD');
    EndDate = moment(EndDate, 'YYYY-MM-DD');

    /* using duration */
    //var duration = moment.duration(EndDate.diff(startDate)).days(); // you may use duration
    var duration = Math.round((EndDate - startDate) / (1000 * 60 * 60 * 24));

    return duration;
}

function formatdate(strDate) {
    return strDate.split('/')[1] + '/' + strDate.split('/')[0] + '/' + strDate.split('/')[2];
}

function guid() {
    function _p8(s) {
        var p = (Math.random().toString(16) + "000000000").substr(2, 8);
        return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
    }
    return _p8() + _p8(true);
}

function setFixColumn() {

    var $table = $('.table');
    //Make a clone of our table
    var $fixedColumn = $table.clone().insertBefore($table).addClass('fixed-column');

    //Remove everything except for first column
    $fixedColumn.find('th:not(:first-child),td:not(:first-child)').remove();

    //Match the height of the rows to that of the original table's
    $fixedColumn.find('tr').each(function (i, elem) {
        $(this).height($table.find('tr:eq(' + i + ')').height());
    });

}


function clearModal() {
    $('#myModal').data('id', null);
    $('#myModal').removeData('ShiftData');
    $('#PhysicianDropDown').val('');
    $('#txtDate').val('');
    $('#timepicker1').timepicker('setTime', '');
    $('#timepicker2').timepicker('setTime', '');
    $('#StateDropdown').val('');

    $('.selectpicker').selectpicker('refresh');

}

function showPopup(date) {
    var data = Data.objShift.filter(function (el) {
        return el.date == date;
    });

    if (data.length > 0) {

        var $table = createNewElement('table'),
            $tbody = createNewElement('tbody'),
            $thTr = createNewElement('tr'),
            $th = createNewElement('th');
        $th.css({ 'text-align': 'center' });

        $th.text(date);
        $thTr.append($th);
        $tbody.append($thTr);

        for (var i = 0; i < data.length; i++) {
            var $tr = createNewElement('tr'),
                $td = createNewElement('td');

            $td.addClass('monthShifts');
            $td.attr('ShiftDetail', data[i].id);

            $td.html(data[i].startFull + ' - ' + data[i].endFull + '<br/>' + data[i].name + ' at ' + data[i].region);

            if (data[i].status == pendingStatus)
                $td.addClass('yellowShifts');
            else
                $td.addClass('blueShifts');

            $td.css({ 'text-align': 'center' });
            $tr.append($td);
            $tbody.append($tr);

        }

        $table.append($tbody).addClass('table table-bordered');

        $('#mdlAllDayData').find('.modal-body').html($table);

        $('#mdlAllDayData').modal('show');


    }
}

function createNewElement(eleName) {
    return $(document.createElement(eleName));
}

$('.scheduleOption').click(function (e) {
    // $('#divSchedular').createSchedular({ ShiftData: Data.objShift, StaffData: Data.objStaff, Type: $(this).val() });
    var type = "D";
    $(".scheduleOption").removeClass('active');
    $(this).addClass('active');

    if ($(this).html() == "Day") {
        type = "D";
        activeView = 1;
    }
    else if ($(this).html() == "Week") {
        type = "W";
        activeView = 2;
    }
    else {
        type = "M";
        activeView = 3;
    }

    refreshShifts();

    //$('#divSchedular').createSchedular({ ShiftData: Data.objShift, StaffData: Data.objStaff, Type: type, currDate: dDate });
});

$('#btnApprove').click(function (e) {
    e.preventDefault();

    var sGuid = guid();

    var physician = $.grep(jsonObject.PhysicianList, function (val, i) {
        return val.Value == $('#PhysicianDropDown').val();
    });

    var region = $.grep(jsonObject.RegionList, function (val2, j) {
        return val2.Value == $('#StateDropdown').val();
    });

    var obj = {
        id: $('#myModal').data('id') ? $('#myModal').data('id') : sGuid,
        physicianId: $('#PhysicianDropDown').val().trim(),
        name: physician[0].Text,
        date: $('#txtDate').find('input').val().trim(),
        startFull: $('#timepicker1').data('timepicker').getTime(),
        endFull: $('#timepicker2').data('timepicker').getTime(),
        region: region[0].Text,
        regionId: region[0].Value,
        _start: $('#timepicker1').data('timepicker').meridian == "PM" ? ($('#timepicker1').data('timepicker').hour == '12' ? $('#timepicker1').data('timepicker').hour : ($('#timepicker1').data('timepicker').hour + 12)) : $('#timepicker1').data('timepicker').hour,
        _startMin: $('#timepicker1').data('timepicker').minute,
        _end: $('#timepicker2').data('timepicker').meridian == "PM" ? ($('#timepicker2').data('timepicker').hour == '12' ? $('#timepicker2').data('timepicker').hour : ($('#timepicker2').data('timepicker').hour + 12)) : $('#timepicker2').data('timepicker').hour,
        _endMin: $('#timepicker2').data('timepicker').minute,
        _endHour: $('#timepicker2').data('timepicker').minute > 0 ? ($('#timepicker2').data('timepicker').hour + 1) : $('#timepicker2').data('timepicker').hour,
        sMeridian: $('#timepicker1').data('timepicker').meridian,
        eMeridian: $('#timepicker2').data('timepicker').meridian,
        iNoOfDays: "0011111"
    };

    //Data.objShift.each(function () {
    //    var $this = $(this);
    //    
    //    if ($this.id = obj.id) {

    //    }
    //});

    var len = Data.objShift.length;

    if (obj.id != sGuid) {
        for (var i = 0; i < len; i++) {
            if (Data.objShift[i].id == obj.id) {
                Data.objShift[i] = obj;
                var slen = Data.objStaff.length;
                for (var k = 0; k < slen; k++) {
                    if (Data.objStaff[k].id == obj.id) {
                        Data.objStaff[k] = { id: obj.id, name: physician[0].Text };
                    }
                }
            }
        }
    } else {
        Data.objShift.push(obj);
        Data.objStaff.push({ id: obj.id, name: physician[0].Text });
    }



    var type = "D";
    if (activeView == 1) {
        type = "D";
    }
    else if (activeView == 2) {
        type = "W";
    }
    else {
        type = "M";
    }
    $('#divSchedular').createSchedular({ ShiftData: Data.objShift, StaffData: Data.objStaff, Type: type, currDate: dDate });
    // setFixColumn();

});

function refreshDates() {
    if (loggedInPhysicianId == 0) {

        if (activeView == 1) {
            $('#divDateTime').html(objDate.fullDays[dDate.getDay()] + ", " + objDate.months[dDate.getMonth()] + " " + dDate.getDate() + ", " + dDate.getFullYear());
        }
        else if (activeView == 2) {
            $('#divDateTime').html(moment(dDate).startOf('week').format("MMM") + " " + moment(dDate).startOf('week').format("DD") + " - " + moment(dDate).endOf('week').format("MMM") + " " + moment(dDate).endOf('week').format("DD") + ", " + dDate.getFullYear());
        }
        else {
            $('#divDateTime').html(objDate.months[dDate.getMonth()] + " " + dDate.getFullYear());
        }
    }
    else {
        if (!isPhysicianMobileView) {
            $('#divDateTime').html('<strong>Schedule for: </strong>' + objDate.months[dDate.getMonth()] + " " + moment(dDate).startOf('month').format('D') + ' - ' + objDate.months[dDate.getMonth()] + " " + moment(dDate).endOf('month').format('D') + ", " + dDate.getFullYear());
        }
        else {
            $('#divDateTime').html('<strong>Schedule for: </strong>' + moment(dDate).startOf('week').format("MMM") + " " + moment(dDate).startOf('week').format("DD") + ' - ' + moment(dDate).endOf('week').format("MMM") + " " + moment(dDate).endOf('week').format("DD") + ", " + dDate.getFullYear());
        }
    }
}


