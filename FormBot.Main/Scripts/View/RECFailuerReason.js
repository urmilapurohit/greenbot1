var SelectedStageId = 1;
var selectedid = 1;
$(document).ready(function () {
    $("#ResellerId").select2();
    $('#datatable').DataTable({
        iDisplayLength: GridConfig[0].PageSize,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },

        columns: [
            {
                'data': 'Id',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    if ($('#chkAll').prop('checked') == true) {
                        return '<input type="checkbox" id="chk_' + full.JobId + '" checked ">';
                    }
                    else {
                        return '<input type="checkbox" id="chk_' + full.JobId + '">';
                    }
                }
            },
            {
                'data': 'Id',

                "render": function (data, type, full, meta) {
                    if (SelectedStageId == 3) {
                        var statusHref = 'javascript:;';
                        var returnHTML = '';

                        returnHTML += '<button class="btn primary" style="margin:10px 0px;"><a href="' + statusHref + '" class="action" style="color:white" title="See Issues" onclick=CheckStatus("' + full.GBBatchRECUploadId + '",' + full.TotalSTC + ',' + full.IsIssue + ')>' + 'See Issues' + '</a></button>';
                        return returnHTML;
                    }
                    if (SelectedStageId == 4) {
                        var statusHrefForUnknown = 'javascript:;';
                        var returnHTMLForUnknown = '';

                        returnHTMLForUnknown += '<button class="btn primary" style="margin:10px 0px;"><a href="' + statusHrefForUnknown + '" class="action" style="color:white" title="See Issues" onclick=CheckUnknownStatus("' + full.GBBatchRECUploadId+ '"'+')>' + 'See Issues' + '</a></button>';
                        return returnHTMLForUnknown;
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'Id',

                "render": function (data, type, full, meta) {
                    var statusHrefForJobid = 'javascript:;';
                    var returnHTMLForJobId = '';
                    returnHTMLForJobId += '<button class="btn primary" style="margin:10px 0px;"><a href="' + statusHrefForJobid + '" class="action" style="color:white" title="See Issues" onclick=GetJobs("' + full.GBBatchRECUploadId + '"' + ')>' + 'See Jobs' + '</a></button>';
                    return returnHTMLForJobId;
                }
            },
            {
                'data': 'GBBatchRECUploadId',

                "render": function (data, type, full, meta) {
                    if (full.GBBatchRECUploadId) {
                        return '<label>' + full.GBBatchRECUploadId + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'TotalSTC',

                "render": function (data, type, full, meta) {
                    if (full.TotalSTC) {
                        return '<label>' + full.TotalSTC + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'RECUserName',

                "render": function (data, type, full, meta) {
                    if (full.RECUserName) {
                        return '<label>' + full.RECUserName + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'RECName',

                "render": function (data, type, full, meta) {
                    if (full.RECName) {
                        return '<label>' + full.RECName + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'ResellerName',

                "render": function (data, type, full, meta) {
                    if (full.ResellerName) {
                        return '<label>' + full.ResellerName + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'RECCompanyName',

                "render": function (data, type, full, meta) {
                    if (full.RECCompanyName) {
                        return '<label>' + full.RECCompanyName + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'CreatedDate',

                "render": function (data, type, full, meta) {
                    if (full.CreatedDate) {
                        //return '<label>' + moment(full.CreatedDate).format("YYYY-MM-DD") + '</label>';
                        return '<label>' + formatDate(full.CreatedDate) + '</label>';

                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'InitiatedBy',

                "render": function (data, type, full, meta) {
                    if (full.InitiatedBy) {

                        return '<label>' + full.InitiatedBy + '</label>';
                    }
                    else {
                        return '';
                    }
                }
            },

        ],

        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        initComplete: function (settings, json) {
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

        },

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).addClass('togglerow');
            if (aData.HasMultipleRecords) {
                $(nRow).addClass('togglerow').find('.toggle-icon').parent().addClass('toggle-btn');
            }
            if (SelectedStageId == 1) {
                var value = 0;
                if (aData.CurrentStatus == "Login")
                    value = 20;
                else if (aData.CurrentStatus == "Search")
                    value = 40;
                else if (aData.CurrentStatus == "Send to REC")
                    value = 60;
                else if (aData.CurrentStatus == "Uploading Files")
                    value = 80;
                else if (aData.CurrentStatus == "Completed")
                    value = 100;
                var $progress = $("<div id= 'progressbar'/>").css("font-size", "0.6em").appendTo($(nRow).find("td:nth-child(2)"));
                $progress.progressbar({
                    value: value,
                    max: 100
                });
            }
        },

        bServerSide: true,
        sAjaxSource: urlGetRecFailureReason,

        "fnDrawCallback": function () {
            if ($('#chkAll').prop('checked') == true) {
                //chkCount = $('#datatable >tbody >tr').length;
            }
            else {
                chkCount = 0;
            }
            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length >= 1) {
                $("#datatable_length").show();
            } else if ($("#datatable").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            $('#datatable').on('length.dt', function (e, settings, len) {
                SetPageSizeForGrid(len, false, ViewPageId)
                GridConfig[0].PageSize = len;
                GridConfig[0].IsKendoGrid = false;
                GridConfig[0].ViewPageId = parseInt(ViewPageId);
            });
            var info = table.page.info();
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> Job(s) found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody').find('tr.togglerow').length + '</b>  of  <b>' + info.recordsTotal + '</b> Job(s) found';
            }
            //------------------------------------------------------------------------------------------------------------------------------
            //var bgColor = "Red";
            //var processVal = $("#progressbar").progressbar("value");
            //if (processVal > 30 && processVal < 60)
            //    bgColor = "Yellow";
            //else if(processVal >= 60)
            //bgColor = "Green";
            if (SelectedStageId == 1) {
                $(".ui-progressbar-value").css({
                    "background": "Green"
                });
            }
        },

        "fnServerParams": function (aoData) {
            aoData.push({ "name": "StageId", "value": SelectedStageId });
            aoData.push({ "name": "sortBy", "value": "GBRecBulkUploadId" });
            aoData.push({ "name": "ResellerId", "value": $("#ResellerId").val() });
            aoData.push({ "name": "bulkUploadId", "value": $("#txtBulkUploadId").val() });
            aoData.push({ "name": "FromDate", "value": GetDates($("#txtCreationFromDate").val()) });
            aoData.push({ "name": "ToDate", "value": GetDates($("#txtCreationToDate").val()) });
            aoData.push({ "name": "RECUserName", "value": $("#txtRECUserName").val() });
            aoData.push({ "name": "RecName", "value": $("#txtRECName").val() });
            aoData.push({ "name": "InitiatedBy", "value": $("#txtInitiatedBy").val()});
        }
    });

    // Look at each table row.


    var table = $('#datatable').DataTable();
    $('#chkAll').on('click', function () {
        var rows = table.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
        chkCount = this.checked ? $('#datatable >tbody >tr').length : 0;
    });

    $('#datatable tbody').on('change', 'input[type="checkbox"]', function () {
        if (this.checked) {
            chkCount++;
            if (chkCount == $('#datatable >tbody >tr').length) {
                $('#chkAll').prop('checked', this.checked)
            }
        }
        else {
            chkCount--;
            $('#chkAll').prop('checked', this.checked)
        }
    });

    $('#rec-status').on('hidden.bs.modal', function () {
        SetStageId(SelectedStageId);
    })

    $.ajax({
        type: 'get',
        url: urlGetReseller,
        dataType: 'json',
        data: '',
        async: false,
        success: function (reseller) {
            $("#ResellerId").append('<option value="">All</option>');
            $.each(reseller, function (i, res) {
                $("#ResellerId").append('<option value="' + res.Value + '">' + res.Text + '</option>');
            });

            $("#ResellerId").val($("#ResellerId option:first").val());
        },
        error: function (ex) {
            alert('Failed to retrieve Resellers.' + ex);
        }
    });

    $('#txtCreationFromDate').datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#txtCreationToDate").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#txtCreationFromDate").val(), dateFormat));
            var toDate = new Date(ConvertDateToTick($("#txtCreationToDate").val(), dateFormat));
            if (fromDate > toDate) {
                $("#txtCreationToDate").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#txtCreationFromDate").val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if ($('#txtCreationToDate').data('datepicker')) {
            $('#txtCreationToDate').data('datepicker').setStartDate(new Date(tDate));
        }
    });

    $("#txtCreationToDate").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtCreationToDate').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtCreationFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtCreationToDate").val(), dateFormat));
    });
});

function formatDate(dt) {
    var date = new Date(moment(dt));
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ':' + seconds;
    return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate() + " " + strTime;
}

function SetStageId(id) {
    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#f7f7f7";

    SelectedStageId = id;

    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#686868";
    $("#datatable").dataTable().fnDraw();
    //GetRecFailureReason($("#ResellerId").val());
}

function ToDate(data) {
    if (data != null) {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
        return moment(date).format(dateFormatMoment);
    }
    else {
        return '';
    }
}

function ToTime(data) {
    if (data != null) {
        var TDate = new Date(data);
        return TDate.toLocaleTimeString('en-US');
    }
    else {
        return '';
    }
}

function GetDates(date) {
    if (date != '') {
        var tickStartDate = ConvertDateToTick(date, dateFormat);
        return moment(tickStartDate).format("YYYY-MM-DD");
    }
    else {
        return '';
    }
}

function CheckStatus(bulkUploadId, TotalSTC, IsIssue, IsFailedJobs = true, isUnLockBtns=false) {
    if (IsIssue == false)
        IsFailedJobs = false;
    $("#rec-status").load(urlGetBatchDetails + '?batchId=' + bulkUploadId + '&totalSTC=' + TotalSTC + '&isIssue=' + IsIssue + '&isFailedJob=' + IsFailedJobs + '&isUnLockBtns=' + isUnLockBtns, function () {
        $("#rec-status").modal({ backdrop: 'static', keyboard: false });
    });
}

function CheckUnknownStatus(bulkUploadId) {
    $.ajax({
        url: urlGetUnknownIssues,
        type: 'GET',
        data: { BulkUploadId: bulkUploadId },
        contentType: 'application/json',
        dataType: 'json',
        success: function (data) {
            if (data.lstIssue.length > 0) {
                $('#UnknownIssue').html('');
                for (var i = 0; i < data.lstIssue.length; i++) {
                    var html = ' <span style="margin-left: 5px !important;">&#8226;</span><span style="margin-left: 10px !important">' + data.lstIssue[i] + '</span>';
                    $("#UnknownIssue").append('<li>' +html +'</li>');
                }
                $('#popUpGetUnknownIssue').modal("show");
            }
            else {
                $('#UnknownIssue').html('');
                $('#popUpGetUnknownIssue').modal("show");
            }
        }
    });
   
}

function GetJobs(bulkUploadId) {
    $("#Job-status").load(urlGetJobDetailsBatchWise + '?batchId=' + bulkUploadId, function () {
        $("#Job-status").modal({ backdrop: 'static', keyboard: true });
    });
}

document.body.addEventListener("click", function (e) {
    //if (e.target.parentElement.className =="closeModal") {
    //    e.preventDefault();
    //} else {
        var element = document.getElementById("Job-status");
        if (element) {
            $("#Job-status").modal('hide');
            $(".modal-backdrop.fade.in").remove();
            // $("div").removeClass("modal-backdrop");
        }
});

function SearchJobSTCRecords() {
    $("#datatable").dataTable().fnDraw();
}

function ResetSearchFilters(ShowAll) {
    $("#txtCreationToDate").val('');
    $("#txtCreationFromDate").val('');
    $("#txtBulkUploadId").val('');
    $("#txtRECUserName").val('');
    $("#txtRECName").val('');
    $("#txtInitiatedBy").val('');
    $("#ResellerId").val('');
    $("#datatable").dataTable().fnDraw();
}