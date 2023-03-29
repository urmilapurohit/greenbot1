var cols = [];
var colsDefs = [];
var jobIndexTable;
var arrAdvanceSearchFilters = [];
var IsServerCallForSearch = false;
$(document).ready(function () {

    if (UserTypeID == 4) {
        checkIsWholeSaler(ResellerId);
    }

    $('#searchSolarCompany').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $.each(solarCompanyList, function (key, value) {

                if (value.text.toLowerCase().indexOf($("#searchSolarCompany").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnsolarCompanyid").val(ui.item.id); // save selected id to hidden input
            IsServerCallForSearch = true;
        }
    }).on('focus', function () { $(this).keydown(); });

    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };


    if (JobStage) {
        //SetStageId(JobStage);
        SelectedStageId = JobStage;
        var id = 0;
        var a = document.getElementById("jobstage_" + id);
        a.style.backgroundColor = "#bdbdbd";

        //SelectedStageId = id;

        var a = document.getElementById("jobstage_" + SelectedStageId);
        a.style.backgroundColor = "#5F5D5D";

        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            document.getElementById("chkIsArchive").checked = false;
        }

        $("#txtSearchfor").val('');
        document.getElementById("JobScheduleTypeId").selectedIndex = 0;
        document.getElementById("JobTypeId").selectedIndex = 0;
        document.getElementById("JobPriorityId").selectedIndex = 0;
        $("#txtFromDate").val('');
        $("#txtToDate").val('');

        document.getElementById("chkJobRef").checked = true;
        document.getElementById("chkJobDescription").checked = true;
        document.getElementById("chkJobAddress").checked = true;
        document.getElementById("chkClient").checked = true;
        document.getElementById("chkStaff").checked = false;

        document.getElementById("chkInvoiced").checked = true;
        document.getElementById("chkNotInvoiced").checked = true;
        document.getElementById("chkReadyToTrade").checked = true;
        document.getElementById("chkNotReadyToTrade").checked = true;

        document.getElementById("chkTraded").checked = true;
        document.getElementById("chkNotTraded").checked = true;

        document.getElementById("chkPreApprovalNotBeenApproved").checked = true;
        document.getElementById("chkPreApprovalApproved").checked = true;
        document.getElementById("chkConnectionCompleted").checked = true;
        document.getElementById("chkConnectionNotCompleted").checked = true;

        document.getElementById("chkACT").checked = true;
        document.getElementById("chkNSW").checked = true;
        document.getElementById("chkNT").checked = true;
        document.getElementById("chkQLD").checked = true;
        document.getElementById("chkSA").checked = true;
        document.getElementById("chkTAS").checked = true;
        document.getElementById("chkWA").checked = true;
        document.getElementById("chkVIC").checked = true;
    }

    //FillDropDown('JobStageID', urlGetJobStages, null, true, null, 'Select job stage');
    //BindJobStages();

    if (UserTypeID == 1 || UserTypeID == 3) {
        $("#ResellerId").focus();
    }
    else if (UserTypeID == 2 || UserTypeID == 5) {

        $("#searchSolarcompany").focus();
    }
    else {
        $("#JobScheduleTypeId").focus();
    }

    //FillDropDown('SSCID', urlGetSSCUser, null, true, null);

    $('#txtFromDate').datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#txtToDate").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
            var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
            if (fromDate > toDate) {
                $("#txtToDate").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#txtFromDate").val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if ($('#txtToDate').data('datepicker')) {
            $('#txtToDate').data('datepicker').setStartDate(new Date(tDate));
        }
    });
    $("#txtToDate").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtToDate').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
    });

    if (UserTypeID == 1 || UserTypeID == 3) {
        var IsResellerExist = false;
        $.ajax({
            type: 'get',
            url: urlGetReseller,
            dataType: 'json',
            data: '',
            async: false,
            success: function (reseller) {
                $.each(reseller, function (i, res) {
                    $("#ResellerId").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                    if (IsResellerExist == false && res.Value == localStorage.getItem('JobList_ResellerId')) {
                        IsResellerExist = true;
                    }
                });

                if (IsResellerExist) {
                    document.getElementById("ResellerId").value = localStorage.getItem('JobList_ResellerId')
                }
                else {
                    $("#ResellerId").val($("#ResellerId option:first").val());
                    localStorage.setItem('JobList_ResellerId', $("#ResellerId option:first").val());
                }

                if ($("#ResellerId").val() > 0 && (UserTypeID == 1 || UserTypeID == 3)) {
                    BindSolarCompany(document.getElementById("ResellerId").value);
                    GetJobStageCount();
                    checkIsWholeSaler($('#ResellerId').val());
                }
            },
            error: function (ex) {
                alert('Failed to retrieve Resellers.' + ex);
            }
        });
    }
    else if (UserTypeID == 2) {
        BindSolarCompany(ResellerId);
        GetJobStageCount();
    }
    else if (UserTypeID == 5) {
        if (isAllScaJobView == "true") {
            BindSolarCompany(ResellerId);
        }
        else {
            $("#searchSolarCompany").val("");
            var IsCompanyExist = false;
            $.ajax({
                type: 'POST',
                url: urlGetAssignedSolarCompanyToRAM,
                dataType: 'json',
                async: false,
                //data: '',
                data: { isAll: true },
                success: function (solarcompany) {
                    solarCompanyList = [];
                    $.each(solarcompany, function (i, company) {
                        solarCompanyList.push({ value: company.Value, text: company.Text });

                        if (IsCompanyExist == false && company.Value == localStorage.getItem('JobList_SolarCompanyId')) {
                            IsCompanyExist = true;
                        }
                    });

                    if (IsCompanyExist) {
                        //document.getElementById("SolarCompanyId").value = localStorage.getItem('JobList_SolarCompanyId');

                        $('#hdnsolarCompanyid').val(localStorage.getItem('JobList_SolarCompanyId'));
                    }
                    else {
                        //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
                        //localStorage.setItem('JobList_SolarCompanyId',$("#SolarCompanyId option:first").val());

                        $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                        localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
                    }

                    if (localStorage.getItem('JobList_SolarCompanyId') == '') {

                        $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                        localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
                    }

                    $.each(solarCompanyList, function (key, value) {

                        if (value.value == localStorage.getItem('JobList_SolarCompanyId')) {

                            $("#searchSolarCompany").val(value.text);
                            $('#hdnsolarCompanyid').val(localStorage.getItem('JobList_SolarCompanyId'));
                        }
                    });

                    GetJobStageCount();
                },
                error: function (ex) {
                    alert('Failed to retrieve Solar Companies.' + ex);
                }
            });
        }
    }
    else if (UserTypeID == 6) {
        var IsCompanyExist = false;
        $.ajax({
            type: 'POST',
            url: urlGetRequestedSolarCompanyToSSC,
            dataType: 'json',
            data: '',
            async: false,
            success: function (solarcompany) {

                solarCompanyList = [];

                $.each(solarcompany, function (i, company) {
                    solarCompanyList.push({ value: company.Value, text: company.Text });
                });

                if (IsCompanyExist) {
                    //document.getElementById("SolarCompanyId").value = localStorage.getItem('JobList_SolarCompanyId')

                    $('#hdnsolarCompanyid').val(localStorage.getItem('JobList_SolarCompanyId'));
                }
                else {
                    //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
                    //localStorage.setItem('JobList_SolarCompanyId',$("#SolarCompanyId option:first").val());

                    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
                }
            },
            error: function (ex) {
                alert('Failed to retrieve Solar Companies.' + ex);
            }
        });
        GetJobStageCount();
    }

    SetParamsFromLocalStorage();

    if (NotYetInvoicedDashboardStatus) {
        document.getElementById("chkInvoiced").checked = false;
    }

    //SaveFilter();
    datatableInfo();
    drawJobIndex();

    var table = $('#datatable').DataTable();

    DisableAllColReorder();
    DataTableColResize($('#trHeadersDynamic th').length);

    $.fn.dataTable.ColReorder(table);
    table.on('column-reorder', function (e, settings, details) {
        DataTableColResize($('#trHeadersDynamic th').length - 1);
    });

    SetAdvanceSearchFiltersColumns();
    SetChangeFilter();

    $('#datatable tbody').on('change', 'input[type="checkbox"]', function () {
        if (this.checked) {
            chkCount++;
            if (chkCount == $('#datatable >tbody >tr').length) {
                $('#chkAll').prop('checked', this.checked)
            }
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            $("#btotalTradedSTC").html(parseFloat(parseFloat(cSTC) + parseFloat(tSTC)).toFixed(2));
        }
        else {
            chkCount--;
            $('#chkAll').prop('checked', this.checked)
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            $("#btotalTradedSTC").html(parseFloat(parseFloat(tSTC) - parseFloat(cSTC)).toFixed(2));
        }
    });

    $("#ResellerId").change(function () {
        if ($("#ResellerId").val() > 0 && (UserTypeID == 1 || UserTypeID == 3)) {
            IsServerCallForSearch = true;
            BindSolarCompany(document.getElementById("ResellerId").value);
            $('#searchSolarCompany').focus();
            //$('#searchSolarCompany').val('');
            checkIsWholeSaler($('#ResellerId').val());
        }
    });
});
function UnlockChanges() {
    if ($('#btnLock').val() == "Lock") {
        $("#btnLock").prop('value', 'Unlock');
        var table = $('#datatable').DataTable();
        datatableInfo();
        table.destroy();
        drawJobIndex();
        var table = $('#datatable').DataTable();
        $.fn.dataTable.ColReorder(table);

        table.on('column-reorder', function (e, settings, details) {
            DataTableColResizeNotAll($('#trHeadersDynamic th').length - 1);

        });
        DataTableColResizeNotAll($('#trHeadersDynamic th').length);
    }
    else if ($('#btnLock').val() == "Unlock") {
        $("#btnLock").prop('value', 'Lock');
        DataTableColResize($('#trHeadersDynamic th').length);
        DisableAllColReorder();
    }
}

function DisableAllColReorder() {
    $('.dataTable thead th').each(function () {
        var md = $._data($(this)[0]).events.mousedown;
        for (var i = 0, l = md.length; i < l; i++) {
            if (md[i].namespace == 'ColReorder') {
                md[i].handler = function () { }
            }
        }
    })
}

function DataTableColResizeNotAll(len) {
    var disableArr = [];
    //var len = $('#trHeadersDynamic th').length;
    disableArr.push(0);
    if ((UserTypeID == 6) || (isAssignSSC == 'true' && ((UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) {
        disableArr.push(len - 2);
    }

    $('#datatable').colResizable({
        fixed: true,
        liveDrag: true,
        postbackSafe: true,
        partialRefresh: true,
        flush: true,
        resizeMode: 'fit',
        disabledColumns: disableArr,
        onResize: onSampleResized
    });

    var onSampleResized = function (e) {
        var table = $(e.currentTarget); //reference to the resized table
    };
}

function BindSolarCompany(resellerID) {

    //IsServerCallForSearch = true;

    $("#searchSolarCompany").val("");
    var IsCompanyExist = false;
    $.ajax({
        type: 'POST',
        url: urlGetSolarCompanyByResellerId,
        dataType: 'json',
        async: false,
        data: { id: resellerID, isAll: true },
        success: function (solarcompany) {
            solarCompanyList = [];
            $.each(solarcompany, function (i, company) {
                solarCompanyList.push({ value: company.Value, text: company.Text });

                if (IsCompanyExist == false && company.Value == localStorage.getItem('JobList_SolarCompanyId')) {
                    IsCompanyExist = true;
                }
            });

            if (IsCompanyExist) {
                //document.getElementById("SolarCompanyId").value = localStorage.getItem('JobList_SolarCompanyId');

                $('#hdnsolarCompanyid').val(localStorage.getItem('JobList_SolarCompanyId'));
            }
            else {
                //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
                //localStorage.setItem('JobList_SolarCompanyId',$("#SolarCompanyId option:first").val());

                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            if (localStorage.getItem('JobList_SolarCompanyId') == '') {

                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            $.each(solarCompanyList, function (key, value) {

                if (value.value == localStorage.getItem('JobList_SolarCompanyId')) {

                    $("#searchSolarCompany").val(value.text);
                    $('#hdnsolarCompanyid').val(localStorage.getItem('JobList_SolarCompanyId'));
                }
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}

function ResetSearchFilters(ShowAll) {
    //if (UserTypeID == 1 || UserTypeID == 3) {
    //    $("#ResellerId").val($("#ResellerId option:first").val());
    //    localStorage.setItem('JobList_ResellerId', document.getElementById("ResellerId").value);
    //    BindSolarCompany(document.getElementById("ResellerId").value);
    //    document.getElementById("chkIsArchive").checked = false;
    //}
    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5 || UserTypeID == 6) {
    //    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
    //    localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
    //}

    localStorage.setItem('JobList_Searchfor', '');
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('JobList_IsArchive', false);
    }

    var a = $('input.dropdown-filter-menu-search:text').filter(function () { return this.value != ""; });
    a.val('');
    a.keyup();
    $("#txtSearchfor").val('');
    $("#txtFromDate").val('');
    $("#txtToDate").val('');
    $("#txtFromDate2").val('');
    $("#txtToDate2").val('');

    if (ShowAll) {
        if (SelectedStageId != 0) {
            var a = document.getElementById("jobstage_" + SelectedStageId);
            a.style.backgroundColor = "#bdbdbd";
        }
        SelectedStageId = 0;
    }

    SetParamsFromLocalStorage();

    //$("#datatable").dataTable().fnDraw();
    $.fn.dataTable.ext.search = [];
    FilterIsArchiveData();
    jobIndexTable.draw();

    if (UserTypeID != 7 || UserTypeID != 9 || UserTypeID != 10) {
        GetJobStageCount();
    }

    SelectAllAdvanceSearchCategory();
    SaveFilter();
    $("#btnChangeSearchFilter").val("Change Filter");

    //for (var i = 1; i <= 11; i++) {
    //    localStorage.setItem('locFilter_' + i, '');
    //    localStorage.setItem('lochdnFilter_' + i, '');
    //}
}

function SearchJobs() {
    $("#loading-image").css("display", "");
    if (UserTypeID == 1 || UserTypeID == 3) {
        localStorage.setItem('JobList_ResellerId', document.getElementById("ResellerId").value);
    }
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5 || UserTypeID == 6) {

        localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
    }

    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('JobList_IsArchive', document.getElementById("chkIsArchive").checked);
    }

    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        if (document.getElementById("chkIsArchive").checked) {
            $("#btnOpenDeletedJobs").removeAttr("Style");
            $("#btnDeleteJobs").attr("Style", "display:none");
        }
        else {
            $("#btnOpenDeletedJobs").attr("Style", "display:none");
            $("#btnDeleteJobs").removeAttr("Style");
        }
    }

    //$("#datatable").dataTable().fnDraw();

    if (IsServerCallForSearch) {
        datatableInfo();
        $('#datatable').DataTable().destroy();
        drawJobIndex();
        IsServerCallForSearch = false;
    }

    $.fn.dataTable.ext.search = [];

    FilterIsArchiveData();

    var searchTerm = $("#txtSearchfor").val().toLowerCase().trim();
    if (searchTerm != '' && searchTerm != null) {
        var colSearchIndex = [];
        $.each(arrAdvanceSearchFilters, function (index, colname) {
            colSearchIndex.push(jobIndexTable.columns().column(colname + ":name").index());
        });

        $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
            for (var i = 0, ien = colSearchIndex.length; i < ien; i++) {
                if (searchData[colSearchIndex[i]].toLowerCase().indexOf(searchTerm) !== -1) {
                    return true;
                }
            }
            return false;
        })
    }

    //$.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
    //    if ($('#hdnFilter_3').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("InstallationState:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_4').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("Priority:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_5').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsTraded:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_6').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsPreApprovaApproved:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_7').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsConnectionCompleted:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_8').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("JobTypeId:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#Filter_9').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsGst:name").index()].toLowerCase()) !== -1
    //        &&
    //        (searchData[jobIndexTable.columns().column("PropertyType:name").index()] == '' || $('#Filter_10').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("PropertyType:name").index()].toLowerCase()) !== -1)
    //        &&
    //        $('#Filter_11').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsCustPrice:name").index()].toLowerCase()) !== -1
    //    ) {
    //        return true;
    //    }
    //    return false;
    //})

    if ($('#txtFromDate').val() != '' && $('#txtToDate').val() != '') {
        var chunkscreate = $('#txtFromDate').val().split('/');
        mincreatedate = new Date([chunkscreate[2], chunkscreate[1], chunkscreate[0]].join("-"));
        var chunkscreate2 = $('#txtToDate').val().split('/');
        maxcreatedate = new Date([chunkscreate2[2], chunkscreate2[1], chunkscreate2[0]].join("-"));

        $.fn.dataTable.ext.search.push(
            function (settings, searchData, dataIndex) {
                if (typeof searchData._date == 'undefined') {
                    var dateindex = jobIndexTable.columns().column('CreatedDate:name').index();
                    searchData._date = searchData[dateindex];
                    var chunks = searchData._date.split('/');
                    searchData._date = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
                }
                if (searchData._date == "Invalid Date") {
                    return false;
                }
                if (searchData._date < mincreatedate) {
                    return false;
                }
                if (searchData._date > maxcreatedate) {
                    return false;
                }
                return true;
            });
    }

    if ($('#txtFromDate2').val() != '' && $('#txtToDate2').val() != '') {

        var chunks = $('#txtFromDate2').val().split('/');
        min = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
        var chunks2 = $('#txtToDate2').val().split('/');
        max = new Date([chunks2[2], chunks2[1], chunks2[0]].join("-"));

        $.fn.dataTable.ext.search.push(
            function (settings, searchData, dataIndex) {
                if (typeof searchData._date2 == 'undefined') {
                    var dateindex = jobIndexTable.columns().column('InstallationDate:name').index();
                    searchData._date2 = searchData[dateindex];
                    var chunks = searchData._date2.split('/');
                    searchData._date2 = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
                }
                if (searchData._date2 == "Invalid Date") {
                    return false;
                }
                if (searchData._date2 < min) {
                    return false;
                }

                if (searchData._date2 > max) {
                    return false;
                }
                return true;
            });
    }
    jobIndexTable.draw();
    //$.fn.dataTable.ext.search.pop();

    if (UserTypeID != 7 || UserTypeID != 9 || UserTypeID != 10) {
        GetJobStageCount();
    }

    $("#loading-image").css("display", "none");

    if ($('#btnLock').val() == "Lock") {
        DataTableColResize($('#trHeadersDynamic th').length);
        DisableAllColReorder();
    }
}

function FilterIsArchiveData() {
    if (document.getElementById("chkIsArchive") != null) {
        if (document.getElementById("chkIsArchive").checked) {
            $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
                if (searchData[jobIndexTable.columns().column("IsDeleted:name").index()].toLowerCase().indexOf("true") !== -1) {
                    return true;
                }
                return false;
            })
        }
        else {
            $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
                if (searchData[jobIndexTable.columns().column("IsDeleted:name").index()].toLowerCase().indexOf("false") !== -1) {
                    return true;
                }
                return false;
            })
        }
    }
    else {
        $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
            if (searchData[jobIndexTable.columns().column("IsDeleted:name").index()].toLowerCase().indexOf("false") !== -1) {
                return true;
            }
            return false;
        })
    }
}

$('#txtFromDate2').datepicker({
    format: dateFormat,
    autoclose: true
}).on('changeDate', function () {
    if ($("#txtToDate2").val() != '') {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDate2").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDate2").val(), dateFormat));
        if (fromDate > toDate) {
            $("#txtToDate2").val('');
        }
    }
    var tickStartDate = ConvertDateToTick($("#txtFromDate2").val(), dateFormat);
    tDate = moment(tickStartDate).format("MM/DD/YYYY");
    if ($('#txtToDate2').data('datepicker')) {
        $('#txtToDate2').data('datepicker').setStartDate(new Date(tDate));
    }
});
$("#txtToDate2").datepicker({
    format: dateFormat,
    autoclose: true
});
$('#txtToDate2').change(function () {
    var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
    var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
});

function ConvertToDate(data) {
    if (data != null) {
        var date = new Date(parseInt(data.replace('/Date(', '')));
        return ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear();
    }
    else {
        return '';
    }
}

function GetJobStageCount() {
    //var solarcompanyId = 0;
    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
    //    solarcompanyId = $('#hdnsolarCompanyid').val();
    //}
    //$.ajax({
    //    type: 'POST',
    //    url: urlGetJobStageCount,
    //    dataType: 'json',
    //    async: false,
    //    data: { sId: solarcompanyId },
    //    success: function (jobstagescount) {
    //        var sum = 0;
    //        $.each(jobstagescount.lstJobStagesCount, function (i, count) {
    //            document.getElementById("jobstage_" + count.JobStageId).innerHTML = count.StageName + '<strong>(' + count.jobCount + ')</strong>';
    //            sum = sum + count.jobCount;
    //        });
    //        document.getElementById("jobstage_0").innerHTML = 'Show All' + '<strong>(' + sum + ')</strong>';
    //    },
    //    error: function (ex) {
    //        alert('Failed to retrieve count for Job Stages.' + ex);
    //    }
    //});
    //return false;
}

function DeleteSelectedJobs() {
    selectedRows = [];
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            var jobDetail = {
                id: $(this).attr('id').substring(4),
                jobTitle: $(this).attr('jobtitle')
            }
            selectedRows.push(jobDetail);
        }
    })
    if (selectedRows != null && selectedRows.length > 0) {
        var result = confirm("You are about to delete (" + selectedRows.length + ") jobs are you sure you want to continue?");
        if (result) {
            var msgconfirm = confirm("Are you really sure want to continue?");
            if (msgconfirm) {
                var MsgcloseButton = '<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">&times;</button>';
                $.ajax({
                    url: urlDeleteSelectedJobs,
                    type: "POST",
                    async: false,
                    data: JSON.stringify({ jobs: selectedRows }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.JsonResponseObj.status == "Failed") {
                            $(".alert").hide();
                            $("#errorMsgRegion").removeClass("alert-success");
                            $("#errorMsgRegion").addClass("alert-danger");
                            $("#errorMsgRegion").html(MsgcloseButton + data.JsonResponseObj.strErrors);
                            $("#errorMsgRegion").show();
                            //$("#errorMsgRegion").fadeOut(3000);
                        }
                        else {
                            $(".alert").hide();
                            $("#errorMsgRegion").removeClass("alert-danger");
                            $("#errorMsgRegion").addClass("alert-success");
                            $("#errorMsgRegion").html(MsgcloseButton + data.JsonResponseObj.strErrors);
                            $("#errorMsgRegion").show();
                        }
                        //$("#datatable").dataTable().fnDraw();
                        datatableInfo();
                        $('#datatable').DataTable().destroy();
                        drawJobIndex();
                    },
                });
            }
        }
    }
    else {
        alert('Please select any job for delete.');
    }
    HideToolTip();
}

function OpenDeletedJobs() {
    //var openJobs = [];
    //$('#datatable tbody input[type="checkbox"]').each(function () {
    //    if ($(this).prop('checked') == true) {
    //        openJobs.push($(this).attr('id').substring(4));
    //    }
    //})
    selectedRows = [];
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            var jobDetail = {
                id: $(this).attr('id').substring(4)
            }
            selectedRows.push(jobDetail);
        }
    })
    if (selectedRows != null && selectedRows.length > 0) {
        var result = confirm("Are you sure want to open deleted job(s)?");
        if (result) {
            var MsgcloseButton = '<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">&times;</button>';
            $.ajax({
                url: urlOpenDeletedJobs,
                type: "POST",
                async: false,
                data: JSON.stringify({ jobs: selectedRows }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.JsonResponseObj.status == "Failed") {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-success");
                        $("#errorMsgRegion").addClass("alert-danger");
                        $("#errorMsgRegion").html(MsgcloseButton + data.JsonResponseObj.strErrors);
                        $("#errorMsgRegion").show();
                        //$("#errorMsgRegion").fadeOut(3000);
                    }
                    else {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(MsgcloseButton + data.JsonResponseObj.strErrors);
                        $("#errorMsgRegion").show();
                    }
                    //$("#datatable").dataTable().fnDraw();
                    datatableInfo();
                    $('#datatable').DataTable().destroy();
                    drawJobIndex();
                },
            });
        }
    }
    else {
        alert('Please select any job for open again.');
    }
}

function AssignSSC(id) {

    $("#loading-image").show();

    FillDropDown('SSCID', urlGetSSCUser, null, true, null);

    setTimeout(function () {
        $.get(urlAssignSSC, function (data) {
            $('#divAssignJobSSC').empty();
            $('#divAssignJobSSC').append(data);
            $('#popupProof').modal({ backdrop: 'static', keyboard: false });
            $('#SSCJobID').val(id);
            $('#SSCID').focus();

            $.ajax({
                url: urlGetSSCUserByJbID + '/' + id,
                type: "get",
                async: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    AssignSSCClickEvent();

                    if (data.length == 0) {
                        $("#SSCID").empty();
                        $("#SSCID").append('<option value="">' + "Select" + '</option>');
                    }
                    else {
                        $("#SSCID").empty();
                        $("#SSCID").append('<option value="">' + "Select" + '</option>');
                        $.each(data, function (i, role) {
                            $("#SSCID").append('<option value="' + role.Value + '">' + role.Text + '</option>');
                        });
                    }
                },
            });

            $.ajax({
                url: urlGetSSCID + '/' + id,
                type: "get",
                async: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.length > 0) {
                        $('#SSCID').prop("disabled", true);
                        $('#SSCID').val(parseInt(data[0].SSCID));
                        $('#RemoveSSCNote').val(data[0].Notes);
                        $('#btnJobSSCMapping').hide();
                        $('#sscNotes').show();
                        $('#btnSSCRemove').show();
                        if (data[0].IsSSCRemove == true) {
                            $('#btnSSCRemove').hide();
                            $('#btnSSCCancelRemovalRequest').show();
                            $('#divRequestedBy').show();
                            $('#requestBy').html(data[0].RequestedBy);
                            $('#RemoveSSCNote').prop("disabled", true);
                        }
                        else {
                            $('#btnSSCCancelRemovalRequest').hide();
                            $('#RemoveSSCNote').prop("disabled", false);
                            $('#divRequestedBy').hide();
                        }
                    }
                    else {
                        $('#SSCID').prop("disabled", false);
                        $('#btnJobSSCMapping').show();
                        $('#sscNotes').hide();
                        $('#btnSSCRemove').hide();
                        $('#btnSSCCancelRemovalRequest').hide();
                        $('#RemoveSSCNote').prop("disabled", false);
                    }
                },
            });

            $("#loading-image").hide();
        });
    }, 500);

    //$('#SSCJobID').val(id);
    //$('#popupProof').modal({ backdrop: 'static', keyboard: false });
    //setTimeout(function () {
    //    $('#SSCID').focus();
    //}, 1000);

}



$(document).off('click', '#btnJobSSCMapping').on('click', '#btnJobSSCMapping', function (e) {
    e.preventDefault();
    e.stopPropagation();

    if (typeof (validateExtraFields) == "function") {
        if (validateExtraFields() == false) {
            return false;
        }
    }
    if (typeof (validateForm) != "function" || (typeof (validateForm) == "function" && validateForm())) {
        $(this).closest('form').ajaxformSubmit();
    }
});

$(document).off('click', '#btnJobSCOMapping').on('click', '#btnJobSCOMapping', function (e) {
    e.preventDefault();
    e.stopPropagation();

    if (typeof (validateExtraFields) == "function") {
        if (validateExtraFields() == false) {
            return false;
        }
    }
    if (typeof (validateForm) != "function" || (typeof (validateForm) == "function" && validateForm1())) {
        $(this).closest('form').ajaxformSubmit();
    }
});

function validateForm() {
    $.validator.unobtrusive.parse("#UserDetails");
    if ($("#UserDetails").valid()) {
        return true;
    }
    else {
        return false;
    }
}

function validateForm1() {
    $.validator.unobtrusive.parse("#UserDetail");
    if ($("#UserDetail").valid()) {
        return true;
    }
    else {
        return false;
    }
}

$("#btnAssignSSCClose").click(function () {
    $("#SSCID").next("span").attr('class', 'field-validation-valid');
});

$("#btnCancel").click(function () {
    $("#SSCID").next("span").attr('class', 'field-validation-valid');
});

function SetStageId(id) {
    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#bdbdbd";

    SelectedStageId = id;

    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#5F5D5D";

    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('JobList_IsArchive', false);
    }

    localStorage.setItem('JobList_JobScheduleTypeId', 0)
    document.getElementById("JobScheduleTypeId").selectedIndex = 0;
    localStorage.setItem('JobList_JobTypeId', 0)
    document.getElementById("JobTypeId").selectedIndex = 0;
    localStorage.setItem('JobList_JobPriorityId', 0)
    document.getElementById("JobPriorityId").selectedIndex = 0;
    localStorage.setItem('JobList_Searchfor', '');

    localStorage.setItem('JobList_PreApprovalStatusId', 0)
    document.getElementById("PreApprovalStatusId").selectedIndex = 0;
    localStorage.setItem('JobList_ConnectionStatusId', 0)
    document.getElementById("ConnectionStatusId").selectedIndex = 0;

    $("#txtFromDate").val('');
    $("#txtToDate").val('');

    document.getElementById("chkJobRef").checked = true;
    document.getElementById("chkJobDescription").checked = true;
    document.getElementById("chkJobAddress").checked = true;
    document.getElementById("chkClient").checked = true;
    document.getElementById("chkStaff").checked = false;

    document.getElementById("chkInvoiced").checked = true;
    document.getElementById("chkNotInvoiced").checked = true;
    document.getElementById("chkReadyToTrade").checked = true;
    document.getElementById("chkNotReadyToTrade").checked = true;

    document.getElementById("chkTraded").checked = true;
    document.getElementById("chkNotTraded").checked = true;

    document.getElementById("chkPreApprovalNotBeenApproved").checked = true;
    document.getElementById("chkPreApprovalApproved").checked = true;
    document.getElementById("chkConnectionCompleted").checked = true;
    document.getElementById("chkConnectionNotCompleted").checked = true;

    document.getElementById("chkACT").checked = true;
    document.getElementById("chkNSW").checked = true;
    document.getElementById("chkNT").checked = true;
    document.getElementById("chkQLD").checked = true;
    document.getElementById("chkSA").checked = true;
    document.getElementById("chkTAS").checked = true;
    document.getElementById("chkWA").checked = true;
    document.getElementById("chkVIC").checked = true;

    SetParamsFromLocalStorage();
    //$("#datatable").dataTable().fnDraw();
    datatableInfo();
    $('#datatable').DataTable().destroy();
    drawJobIndex();
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

function LoadStc() {
    var IsSamePrice = true;
    var IsFirst = true;
    var PriceDay1 = 0;
    var PriceDay3 = 0;
    var PriceDay7 = 0;
    var UpFront = 0;
    var PriceOnApproval = 0;
    var PartialPayment = 0;
    var RapidPay = 0;
    var OptiPay = 0;
    var Commercial = 0;
    var Custom = 0;
    var InvoiceStc = 0;
    var PropertyType = 0;
    var IsGst = false;
    var GSTDocument = 0;
    var newselectedRows = [];
    selectedRows = [];
    $('#datatable tbody input:checkbox').each(function () {
        if ($(this).prop('checked') == true) {
            var jobDetail = {
                id: $(this).attr('id').substring(4),
                jobTitle: $(this).attr('jobtitle')
            }
            selectedRows.push(jobDetail);
        }
    })


    if (selectedRows != null && selectedRows.length > 0) {
        $.ajax({
            url: urlGetDataForTradeJob,
            type: "POST",
            async: false,
            data: JSON.stringify({ jobs: selectedRows }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                var tradeData = response.data;
                if ($("#searchSolarCompany").val() != "All") {
                    if (tradeData.length > 0) {

                        for (var i = 0; i < tradeData.length; i++) {
                            if (parseFloat(tradeData[i].SystemSize) <= 60) {
                                if (tradeData[i].IsTraded != true && tradeData[i].IsCustomPrice != true) {
                                    if (tradeData[i].IsReadyToTrade == true) {

                                        IsAllowKW = tradeData[i].UnderKW;
                                        JobSizeForOptiPay = parseFloat(tradeData[i].KWValue);
                                        IsCustomAllowKW = (tradeData[i].IsCustomUnderKW);
                                        JobCustomSizeForOptiPay = parseFloat(tradeData[i].CustomKWValue);
                                        CustTerm = (tradeData[i].CustomSettlementTerm);
                                        SystemSize = parseFloat(tradeData[i].SystemSize);

                                        if ((IsAllowKW == true && SystemSize > JobSizeForOptiPay) || (CustTerm == 8 && IsCustomAllowKW == true && SystemSize > JobCustomSizeForOptiPay)) {
                                            IsSamePrice = false;
                                            alert('Please select jobs which have system size less than or equal to OptiPay KW.');
                                            return false;
                                        }
                                        if (IsFirst) {
                                            PriceDay1 = parseFloat(tradeData[i].PriceDay1);
                                            PriceDay3 = parseFloat(tradeData[i].PriceDay3);
                                            PriceDay7 = parseFloat(tradeData[i].PriceDay7);
                                            UpFront = parseFloat(tradeData[i].UpFront);
                                            PriceOnApproval = parseFloat(tradeData[i].PriceOnApproval);
                                            PartialPayment = parseFloat(tradeData[i].PartialPayment);
                                            RapidPay = parseFloat(tradeData[i].RapidPay);
                                            OptiPay = parseFloat(tradeData[i].OptiPay);
                                            Commercial = parseFloat(tradeData[i].Commercial);
                                            Custom = parseFloat(tradeData[i].Custom);
                                            InvoiceStc = parseFloat(tradeData[i].InvoiceStc);
                                            PropertyType = (tradeData[i].PropertyType);
                                            IsGst = (tradeData[i].IsGst);
                                            GSTDocument = (tradeData[i].GSTDocument);
                                            IsSamePrice = true;
                                            newselectedRows.push(tradeData[i].JobID);
                                        }
                                        else if (PriceDay1 != parseFloat(tradeData[i].PriceDay1) || PriceDay3 != parseFloat(tradeData[i].PriceDay3) || PriceDay7 != parseFloat(tradeData[i].PriceDay7)
                                            || UpFront != parseFloat(tradeData[i].UpFront) || PriceOnApproval != parseFloat(tradeData[i].PriceOnApproval) || PartialPayment != parseFloat(tradeData[i].PartialPayment)
                                            || RapidPay != parseFloat(tradeData[i].RapidPay) || OptiPay != parseFloat(tradeData[i].OptiPay) || Commercial != parseFloat(tradeData[i].Commercial)
                                            || Custom != parseFloat(tradeData[i].Custom) || InvoiceStc != parseFloat(tradeData[i].InvoiceStc) || PropertyType != (tradeData[i].PropertyType)
                                            || IsGst != (tradeData[i].IsGst) || (GSTDocument.length > 0) != (tradeData[i].GSTDocument.length > 0)) {
                                            IsSamePrice = false;
                                            alert('Please select jobs which have same price and installation property type.');
                                            return false;
                                        }
                                        else {
                                            IsSamePrice = true;
                                            newselectedRows.push(tradeData[i].JobID);
                                        }
                                        IsFirst = false;
                                    }
                                    else {
                                        IsSamePrice = false;
                                        alert('Please select jobs which are ready to trade.');
                                        return false;
                                    }
                                }
                                else {
                                    IsSamePrice = false;
                                    alert('Please select jobs which are neither traded nor have custom price.');
                                    return false;
                                }
                            }
                            else {
                                IsSamePrice = false;
                                alert('You can not trade for job from here which has system size greater than 60.');
                                return false;
                            }
                        }

                    }
                }
                else {
                    IsSamePrice = false;
                    alert('Please select single solar company to trade jobs.');
                    return false;
                }
            }
        });
    }
    else {
        alert('Please select any job for trade.');
    }




    //$('#datatable tbody input[type="checkbox"]').each(function () {
    //    if ($("#searchSolarCompany").val() != "All") {
    //        if ($(this).prop('checked') == true) {
    //            if (parseFloat($(this).attr('SystemSize')) <= 60) {
    //                if ($(this).attr('IsTraded') != 'true' && $(this).attr('IsCustomPrice') != 'true') {
    //                    if ($(this).attr('IsReadyToTrade') == 'true') {

    //                        IsAllowKW = ($(this).attr('isallowkw'));
    //                        JobSizeForOptiPay = parseFloat($(this).attr('jobsizeforoptipay'));
    //                        IsCustomAllowKW = ($(this).attr('iscustomallowkw'));
    //                        JobCustomSizeForOptiPay = parseFloat($(this).attr('jobcustomsizeforoptipay'));
    //                        CustTerm = ($(this).attr('custterm'));
    //                        SystemSize = parseFloat($(this).attr('systemsize'));

    //                        if ((IsAllowKW == 'true' && SystemSize > JobSizeForOptiPay) || (CustTerm == 8 && IsCustomAllowKW == 'true' && SystemSize > JobCustomSizeForOptiPay)) {                                
    //                            IsSamePrice = false;
    //                            alert('Please select jobs which have system size less than or equal to OptiPay KW.');
    //                            return false;
    //                        }
    //                        if (IsFirst) {
    //                            PriceDay1 = parseFloat($(this).attr('PriceDay1'));
    //                            PriceDay3 = parseFloat($(this).attr('PriceDay3'));
    //                            PriceDay7 = parseFloat($(this).attr('PriceDay7'));
    //                            UpFront = parseFloat($(this).attr('UpFront'));
    //                            PriceOnApproval = parseFloat($(this).attr('PriceOnApproval'));
    //                            PartialPayment = parseFloat($(this).attr('PartialPayment'));
    //                            RapidPay = parseFloat($(this).attr('RapidPay'));
    //                            OptiPay = parseFloat($(this).attr('OptiPay'));
    //                            Commercial = parseFloat($(this).attr('Commercial'));
    //                            Custom = parseFloat($(this).attr('Custom'));
    //                            InvoiceStc = parseFloat($(this).attr('InvoiceStc'));
    //                            PropertyType = ($(this).attr('PropertyType'));
    //                            IsGst = ($(this).attr('IsGst'));
    //                            GSTDocument = ($(this).attr('GSTDocument'));
    //                            IsSamePrice = true;
    //                            newselectedRows.push($(this).attr('jobid'));
    //                        }
    //                        else if (PriceDay1 != parseFloat($(this).attr('PriceDay1')) || PriceDay3 != parseFloat($(this).attr('PriceDay3')) || PriceDay7 != parseFloat($(this).attr('PriceDay7'))
    //                                || UpFront != parseFloat($(this).attr('UpFront')) || PriceOnApproval != parseFloat($(this).attr('PriceOnApproval')) || PartialPayment != parseFloat($(this).attr('PartialPayment'))
    //                                || RapidPay != parseFloat($(this).attr('RapidPay')) || OptiPay != parseFloat($(this).attr('OptiPay')) || Commercial != parseFloat($(this).attr('Commercial'))
    //                                || Custom != parseFloat($(this).attr('Custom')) || InvoiceStc != parseFloat($(this).attr('InvoiceStc')) || PropertyType != ($(this).attr('PropertyType'))
    //                                || IsGst != ($(this).attr('IsGst')) || (GSTDocument.length > 0) != (($(this).attr('GSTDocument')).length > 0)) {
    //                            IsSamePrice = false;
    //                            alert('Please select jobs which have same price and installation property type.');
    //                            return false;
    //                        }
    //                        else {
    //                            IsSamePrice = true;
    //                            newselectedRows.push($(this).attr('jobid'));
    //                        }
    //                        IsFirst = false;
    //                    }
    //                    else {
    //                        IsSamePrice = false;
    //                        alert('Please select jobs which are ready to trade.');
    //                        return false;
    //                    }
    //                }
    //                else {
    //                    IsSamePrice = false;
    //                    alert('Please select jobs which are neither traded nor have custom price.');
    //                    return false;
    //                }
    //            }
    //            else {
    //                IsSamePrice = false;
    //                alert('You can not trade for job from here which has system size greate than 60.');
    //                return false;
    //            }
    //        }
    //    }
    //    else {
    //        IsSamePrice = false;
    //        alert('Please select single solar company to trade jobs.');
    //        return false;
    //    }
    //});

    if (IsSamePrice) {
        if (newselectedRows.length > 0) {
            $.ajax({
                url: urlBindJobSTCPriceView,
                type: "POST",
                data: { jobId: newselectedRows.join(','), IsGridView: true, IsTradedFromJobIndex: true },
                success: function (Data) {
                    if (Data == '') {
                        alert("Select Custom Price");
                    } else {
                        $("#SettleMentBlock").html(Data);
                        $("#errorMsgRegionSTCStatus").hide();
                        $("#errorMsgRegionSTCStatus").html('');
                        $('#StcModal').modal({ backdrop: 'static', keyboard: false });
                        $(".LiSettlement:first").click();
                    }
                }
            });
        }
        else {
            alert("Atleast one job should be selected.");
        }
    }
    else {
        $('#datatable tbody input[type="checkbox"]').each(function () {
            //$(this).prop('checked')==true
            $(this).attr('checked', false);
            $("#chkAll").attr('checked', false);
            chkCount = 0;
            $("#btotalTradedSTC").html(0);
        });
    }
}
function AcceptRejectJobToSSC(jobID, role) {
    $.ajax({
        url: urlAcceptRejectJobToSSC,
        type: "POST",

        async: false,
        data: JSON.stringify({ jobID: jobID, role: role }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Job has been " + data.success + " successfully.");
                $("#errorMsgRegion").show();
                GetJobStageCount();
                //$("#datatable").dataTable().fnDraw();
                datatableInfo();
                $('#datatable').DataTable().destroy();
                drawJobIndex();
            }
        },
    });
}

function DeleteRemoveSSCRequest(jobID) {
    $.ajax({
        url: urlCancelRemovalRequest,
        type: "POST",
        async: false,
        data: JSON.stringify({ jobId: jobID }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                $(".alert").hide();
                $("#popuperrorMsgRegion").removeClass("alert-danger");
                $("#popuperrorMsgRegion").addClass("alert-success");
                $("#popuperrorMsgRegion").html(closeButton + "Cancel removal request removed successfully from the job.");
                $("#popuperrorMsgRegion").show();
                location.reload();
                //$("#datatable").dataTable().fnDraw();
                datatableInfo();
                $('#datatable').DataTable().destroy();
                drawJobIndex();
            }
        },
    });
}

function BulkChangeJobStage() {
    if (document.getElementById('JobStageID').selectedIndex > 0) {
        selectedRows = [];
        $('#datatable tbody input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') == true) {
                selectedRows.push($(this).attr('JobId'));
            }
        })
        if (selectedRows != null && selectedRows.length > 0) {
            $.ajax({
                type: 'POST',
                url: urlBulkChangeJobStage,
                async: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ jobstage: document.getElementById('JobStageID').value, jobids: selectedRows.join(',') }),
                success: function (data) {
                    if (data.success) {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(closeButton + "Job Stages updated successfully.");
                        $("#errorMsgRegion").show();
                        GetJobStageCount();
                        //$("#datatable").dataTable().fnDraw();
                        datatableInfo();
                        $('#datatable').DataTable().destroy();
                        drawJobIndex();
                        document.getElementById('JobStageID').selectedIndex = 0;
                    }
                },
                error: function (ex) {

                }
            });
        }
        else {
            alert('Please select any Job to update Stage.');
        }
    }
    else {
        alert('Please select Job Stage first.');
    }
}

//function SetTradeSearch(chkTrade){
//    if(chkTrade.checked){
//        Ntraded=1;
//    }
//    else{
//        Ntraded=0;
//    }
//}

function HideToolTip() {
    $('.tooltip').hide();
}

function ShowToolTip(id) {
    $('.tooltip').show();
}

function ToDate(data) {
    if (data != null) {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
        //console.log(moment(date).format(dateFormatMoment));
        //console.log(date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2));
        //return date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2);
        return moment(date).format(dateFormatMoment);
    }
    else {
        return '';
    }
}

function BindJobStages() {
    $("#JobStageID").empty();
    $.ajax({
        type: 'POST',
        url: urlGetJobStages,
        dataType: 'json',
        async: false,
        data: { usertypeid: UserTypeID },
        success: function (stages) {
            $("#JobStageID").append('<option value="0">Select job stage</option>');
            $.each(stages, function (i, stage) {
                $("#JobStageID").append('<option value="' + stage.Value + '">' +
                    stage.Text + '</option>');
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Job Stages.' + ex);
        }
    });
    return false;
}

function SetParamsFromLocalStorage() {
    //if (localStorage.getItem('JobList_JobScheduleTypeId') == "0" || localStorage.getItem('JobList_JobScheduleTypeId') == "") {
    //    document.getElementById("JobScheduleTypeId").selectedIndex = 0;
    //}
    //else {
    //    document.getElementById("JobScheduleTypeId").value = localStorage.getItem('JobList_JobScheduleTypeId');
    //}

    //if (localStorage.getItem('JobList_JobTypeId') == "0" || localStorage.getItem('JobList_JobTypeId') == "") {
    //    document.getElementById("JobTypeId").selectedIndex = 0;
    //}
    //else {
    //    document.getElementById("JobTypeId").value = localStorage.getItem('JobList_JobTypeId');
    //}

    //if (localStorage.getItem('JobList_JobPriorityId') == "0" || localStorage.getItem('JobList_JobPriorityId') == "") {
    //    document.getElementById("JobPriorityId").selectedIndex = 0;
    //}
    //else {
    //    document.getElementById("JobPriorityId").value = localStorage.getItem('JobList_JobPriorityId');
    //}

    //$("#txtSearchfor").val(localStorage.getItem('JobList_Searchfor'));

    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
    //    if (localStorage.getItem('JobList_IsArchive') != null) {
    //        document.getElementById("chkIsArchive").checked = localStorage.getItem('JobList_IsArchive') == "false" ? false : true;
    //    }
    //}

    //if (localStorage.getItem('JobList_PreApprovalStatusId') == "0" || localStorage.getItem('JobList_PreApprovalStatusId') == "") {
    //    document.getElementById("PreApprovalStatusId").selectedIndex = 0;
    //}
    //else {
    //    document.getElementById("PreApprovalStatusId").value = localStorage.getItem('JobList_PreApprovalStatusId');
    //}

    //if (localStorage.getItem('JobList_ConnectionStatusId') == "0" || localStorage.getItem('JobList_ConnectionStatusId') == "") {
    //    document.getElementById("ConnectionStatusId").selectedIndex = 0;
    //}
    //else {
    //    document.getElementById("ConnectionStatusId").value = localStorage.getItem('JobList_ConnectionStatusId');
    //}   
}


function SetChangeFilter() {
    var changetext = false;
    for (var i = 1; i <= 11; i++) {
        if (localStorage.getItem('locFilter_' + i) != null) {
            $('#Filter_' + i).text(localStorage.getItem('locFilter_' + i));
            changetext = true;
        }

        if (localStorage.getItem('lochdnFilter_' + i) != null) {
            $('#hdnFilter_' + i).text(localStorage.getItem('lochdnFilter_' + i));
            changetext = true;
        }
    }

    if (changetext) {
        $("#btnChangeSearchFilter").val("Change Filter *");
    }
}

function DataTableColResize(len) {

    //var disableArr = [];
    ////var len = $('#trHeadersDynamic th').length;
    //disableArr.push(0);
    //if ((UserTypeID == 6) || (isAssignSSC == 'true' && ((UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) {
    //    disableArr.push(len - 2);
    //}

    //$('#datatable').colResizable({
    //    fixed: true,
    //    liveDrag: true,
    //    postbackSafe: true,
    //    partialRefresh: true,
    //    flush: true,
    //    resizeMode: 'fit',
    //    disabledColumns: disableArr,
    //    onResize: onSampleResized
    //});

    //var onSampleResized = function (e) {
    //    var table = $(e.currentTarget); //reference to the resized table
    //};

    $("#datatable").colResizable({
        disable: true
    });
}

function drawJobIndex() {
    $('.dropdown-filter-dropdown').remove();
    //$('#datatable tfoot td').each(function () {
    //    var title = $(this).text();
    //    $(this).html('<input class="colWiseSearch" type="text" placeholder="Search ' + title + '" />');
    //});

    jobIndexTable = $('#datatable').DataTable({
        iDisplayLength: parseInt(GridConfig[0].PageSize),
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_",
        },

        "aaSorting": [],

        columnDefs: colsDefs,
        columns: cols,
        "autoWidth": false,
        colReorder: true,

        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>> R",

        "processing": true,
        initComplete: function (settings, json) {

            var table = $('#datatable').DataTable();

            $('#chkAll').on('click', function () {
                var rows = table.rows({ 'search': 'applied' }).nodes();
                $('input[type="checkbox"]', rows).prop('checked', this.checked);
                chkCount = this.checked ? $('#datatable >tbody >tr').length : 0;

                if (this.checked) {
                    var tSTC = 0;
                    $('#datatable tbody input[type="checkbox"]').each(function () {
                        var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
                        tSTC = (parseFloat(parseFloat(tSTC) + parseFloat(cSTC)).toFixed(2));
                    })
                    $("#btotalTradedSTC").html(tSTC);
                }
                else {
                    $("#btotalTradedSTC").html(0);
                }
            });

            var buttons = new $.fn.dataTable.Buttons(table, {
                buttons: [
                    {
                        extend: 'excel',
                        text: 'Export Excel',
                        className: 'primary exporth_ic pull-left',
                        exportOptions: {
                            columns: ':visible',
                        }
                    }
                ]
            }).container().prependTo($('.totalrow'));

            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");

            //DataTableColResize($('#trHeadersDynamic th').length);

            $('#datatable').excelTableFilter({ columnSelector: '[data-order]' });

            //this.api().columns().every(function (index) {                
            //    var columnsName = jobIndexTable.settings().init().columns;
            //    if (jobIndexTable.column('Priority:name').index() == index ||
            //        jobIndexTable.column('JobStage:name').index() == index ||
            //        jobIndexTable.column('JobTypeId:name').index() == index ||
            //        jobIndexTable.column('OwnerType:name').index() == index ||
            //        jobIndexTable.column('Distributor:name').index() == index ||
            //        jobIndexTable.column('ElectricityProvider:name').index() == index ||
            //        jobIndexTable.column('DeemingPeriod:name').index() == index ||
            //        jobIndexTable.column('InstallationType:name').index() == index ||
            //        jobIndexTable.column('TypeOfConnection:name').index() == index ||
            //        jobIndexTable.column('SystemMountingType:name').index() == index ||
            //        jobIndexTable.column('SolarCompany:name').index() == index ||
            //        jobIndexTable.column('StaffName:name').index() == index ||
            //        jobIndexTable.column('SCOName:name').index() == index ||
            //        jobIndexTable.column('IsGst:name').index() == index ||
            //        jobIndexTable.column('IsSTCForm:name').index() == index ||
            //        jobIndexTable.column('IsCESForm:name').index() == index ||
            //        jobIndexTable.column('IsBasicValidation:name').index() == index ||
            //        jobIndexTable.column('IsInvoiced:name').index() == index ||
            //        jobIndexTable.column('IsPreApprovaApproved:name').index() == index ||
            //        jobIndexTable.column('IsConnectionCompleted:name').index() == index ||
            //        jobIndexTable.column('IsTraded:name').index() == index
            //        //jobIndexTable.column('OwnerUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('OwnerStreetTypeID:name').index() == index ||
            //        //jobIndexTable.column('InstallationUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('InstallationStreetTypeID:name').index() == index ||
            //        //jobIndexTable.column('InstallerUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('InstallerStreetTypeID:name').index() == index ||
            //        //jobIndexTable.column('DesignerUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('DesignerStreetTypeID:name').index() == index ||
            //        //jobIndexTable.column('ElectricianUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('ElectricianStreetTypeID:name').index() == index ||
            //        //jobIndexTable.column('SolarCompanyUnitTypeID:name').index() == index ||
            //        //jobIndexTable.column('SolarCompanyStreetTypeID:name').index() == index 
            //        ) {
            //        var column = this;
            //        var select = $('<select class="forautocomplete"><option value="">Select</option></select>')
            //            //.appendTo($(column.footer()).empty())
            //            .appendTo($(column.header()).find('div>div').empty())
            //            .on('change', function () {                            
            //                var val = $.fn.dataTable.util.escapeRegex($(this).val());
            //                //column.search(val ? '^' + val + '$' : '', true, false).draw();
            //                //jobIndexTable.column($(this).parent().index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //            });
            //        var arrTradeStatus = JSON.parse(colTradeStatus);
            //        if (jobIndexTable.column('IsTraded:name').index() == index) {
            //            $.each(arrTradeStatus, function (index, item) {
            //                select.append('<option value="' + item.Name + '">' + item.Name + '</option>')
            //            });
            //        }
            //        else {
            //            var arrDynamic = [];
            //            if (jobIndexTable.column('Priority:name').index() == index)
            //                arrDynamic = JSON.parse(colPriority);
            //            else if (jobIndexTable.column('JobTypeId:name').index() == index)
            //                arrDynamic = JSON.parse(colJobType);
            //            column.data().unique().sort().each(function (d, j) {
            //                if (arrDynamic.length > 0) {
            //                    $.each(arrDynamic, function (index, item) {
            //                        if (d == item.ID) {
            //                            select.append('<option value="' + item.Name + '">' + item.Name + '</option>')
            //                        }
            //                    });
            //                }
            //                else {
            //                    if (jobIndexTable.column('IsGst:name').index() == index ||
            //                        jobIndexTable.column('IsSTCForm:name').index() == index ||
            //                        jobIndexTable.column('IsCESForm:name').index() == index ||
            //                        jobIndexTable.column('IsBasicValidation:name').index() == index) {
            //                        if (d == true)
            //                            select.append('<option value="' + "Yes" + '">' + "Yes" + '</option>')
            //                        else if (d == false)
            //                            select.append('<option value="' + "No" + '">' + "No" + '</option>')
            //                    }
            //                    else if (jobIndexTable.column('IsInvoiced:name').index() == index) {
            //                        if (d == true)
            //                            select.append('<option value="' + "Invoiced" + '">' + "Invoiced" + '</option>')
            //                        else if (d == false)
            //                            select.append('<option value="' + "Pending" + '">' + "Pending" + '</option>')
            //                    }
            //                    else if (jobIndexTable.column('IsPreApprovaApproved:name').index() == index) {
            //                        if (d == true)
            //                            select.append('<option value="' + "Approved" + '">' + "Approved" + '</option>')
            //                        else if (d == false)
            //                            select.append('<option value="' + "Pending" + '">' + "Pending" + '</option>')
            //                    }
            //                    else if (jobIndexTable.column('IsConnectionCompleted:name').index() == index) {
            //                        if (d == true)
            //                            select.append('<option value="' + "Completed" + '">' + "Completed" + '</option>')
            //                        else if (d == false)
            //                            select.append('<option value="' + "Pending" + '">' + "Pending" + '</option>')
            //                    }
            //                    else {
            //                        if (d != null && d != '' && d != ' ') {
            //                            select.append('<option value="' + d + '">' + d + '</option>')
            //                        }
            //                    }
            //                }
            //            });
            //        }
            //    }
            //});

            this.api().columns().every(function (index) {
                var columnsName = jobIndexTable.settings().init().columns;
                if (jobIndexTable.column('Priority:name').index() == index ||
                    jobIndexTable.column('JobStage:name').index() == index ||
                    jobIndexTable.column('JobTypeId:name').index() == index ||
                    jobIndexTable.column('OwnerType:name').index() == index ||
                    jobIndexTable.column('Distributor:name').index() == index ||
                    jobIndexTable.column('ElectricityProvider:name').index() == index ||
                    jobIndexTable.column('DeemingPeriod:name').index() == index ||
                    jobIndexTable.column('InstallationType:name').index() == index ||
                    jobIndexTable.column('TypeOfConnection:name').index() == index ||
                    jobIndexTable.column('SystemMountingType:name').index() == index ||
                    jobIndexTable.column('SolarCompany:name').index() == index ||
                    jobIndexTable.column('StaffName:name').index() == index ||
                    jobIndexTable.column('SCOName:name').index() == index ||
                    jobIndexTable.column('IsGst:name').index() == index ||
                    jobIndexTable.column('PropertyType:name').index() == index ||
                    jobIndexTable.column('IsCustPrice:name').index() == index ||
                    jobIndexTable.column('IsSTCForm:name').index() == index ||
                    jobIndexTable.column('IsCESForm:name').index() == index ||
                    jobIndexTable.column('IsBasicValidation:name').index() == index ||
                    jobIndexTable.column('IsInvoiced:name').index() == index ||
                    jobIndexTable.column('IsPreApprovaApproved:name').index() == index ||
                    jobIndexTable.column('IsConnectionCompleted:name').index() == index ||
                    jobIndexTable.column('IsOwnerSiganture:name').index() == index ||
                    jobIndexTable.column('IsInstallerSiganture:name').index() == index ||
                    jobIndexTable.column('IsGroupSiganture:name').index() == index ||
                    jobIndexTable.column('IsSerialNumberCheck:name').index() == index ||
                    jobIndexTable.column('IsSTCSubmissionPhotos:name').index() == index ||
                    jobIndexTable.column('IsTraded:name').index() == index
                ) {
                    var selectoptions = [];
                    var selectoptions2 = [];
                    var selecttrade = [];
                    var column = this;
                    var select = $('<input type="text" class=' + column.index() + ' />')
                        .appendTo($(column.header()).find('div>div').empty());
                    $('.' + column.index()).attr("Placeholder", "Search" + " " + column.header().innerText);
                    select.addClass('dropdown-filter-menu-search form-control');
                    select.addClass('getalldata');
                    //  $(".caseValuePrecentageRestriction").on("keypress", function (event) {
                    $('.' + column.index()).keyup(function (event) {
                        if ($(this).val() == '') {
                            //jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search('', true, false).draw();
                            jobIndexTable.column($(this).parent().parent().parent().index() + ':visible').search('').draw();
                        }
                    })
                    var arrTradeStatus = JSON.parse(colTradeStatus);
                    if (jobIndexTable.column('IsTraded:name').index() == index) {

                        $.each(arrTradeStatus, function (index, item) {
                            selectoptions.push(item.Name);
                        });
                        $('.' + column.index()).autocomplete({
                            source: selectoptions,
                            minLength: 0,
                            scroll: true,
                            select: function (event, ui) {

                                var val = ui.item.value;
                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                            }
                        }).focus(function () {
                            $(this).autocomplete("search", "");
                        });
                    }
                    else {
                        var arrDynamic = [];
                        if (jobIndexTable.column('Priority:name').index() == index)
                            arrDynamic = JSON.parse(colPriority);
                        else if (jobIndexTable.column('JobTypeId:name').index() == index)
                            arrDynamic = JSON.parse(colJobType);

                        column.data().unique().sort().each(function (d, j) {
                            if (arrDynamic.length > 0) {
                                $.each(arrDynamic, function (index, item) {
                                    if (d == item.ID) {
                                        selectoptions.push(item.Name);
                                    }
                                });
                                $('.' + column.index()).autocomplete({
                                    source: selectoptions,
                                    minLength: 0,
                                    scroll: true,
                                    select: function (event, ui) {
                                        var val = ui.item.value;
                                        jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                    }
                                }).focus(function () {
                                    $(this).autocomplete("search", "");
                                });

                            }
                            else {
                                if (jobIndexTable.column('IsGst:name').index() == index ||
                                    //jobIndexTable.column('PropertyType:name').index() == index ||
                                    jobIndexTable.column('IsCustPrice:name').index() == index ||
                                    jobIndexTable.column('IsSTCForm:name').index() == index ||
                                    jobIndexTable.column('IsCESForm:name').index() == index ||
                                    jobIndexTable.column('IsBasicValidation:name').index() == index ||
                                    jobIndexTable.column('IsOwnerSiganture:name').index() == index ||
                                    jobIndexTable.column('IsInstallerSiganture:name').index() == index ||
                                    jobIndexTable.column('IsGroupSiganture:name').index() == index ||
                                    jobIndexTable.column('IsSerialNumberCheck:name').index() == index ||
                                    jobIndexTable.column('IsSTCSubmissionPhotos:name').index() == index) {
                                    $('.' + column.index()).autocomplete({
                                        source: ['Yes', 'No'],
                                        minLength: 0,
                                        scroll: true,
                                        select: function (event, ui) {
                                            var val = ui.item.value;
                                            jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                        }
                                    }).focus(function () {
                                        $(this).autocomplete("search", "");
                                    });

                                }
                                else if (jobIndexTable.column('IsInvoiced:name').index() == index) {
                                    $('.' + column.index()).autocomplete({
                                        source: ['Invoiced', 'Pending'],
                                        minLength: 0,
                                        scroll: true,
                                        select: function (event, ui) {
                                            var val = ui.item.value;
                                            jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                        }
                                    }).focus(function () {
                                        $(this).autocomplete("search", "");
                                    });

                                }
                                else if (jobIndexTable.column('IsPreApprovaApproved:name').index() == index) {
                                    $('.' + column.index()).autocomplete({
                                        source: ['Approved', 'Pending'],
                                        minLength: 0,
                                        scroll: true,
                                        select: function (event, ui) {
                                            var val = ui.item.value;
                                            jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                        }
                                    }).focus(function () {
                                        $(this).autocomplete("search", "");
                                    });
                                }
                                else if (jobIndexTable.column('IsConnectionCompleted:name').index() == index) {
                                    $('.' + column.index()).autocomplete({
                                        source: ['Completed', 'Pending'],
                                        minLength: 0,
                                        scroll: true,
                                        select: function (event, ui) {
                                            var val = ui.item.value;
                                            jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                        }
                                    }).focus(function () {
                                        $(this).autocomplete("search", "");
                                    });
                                }
                                else {
                                    if (d != null && d != '' && d != ' ') {
                                        selectoptions.push(d);
                                        $('.' + column.index()).autocomplete({
                                            source: selectoptions,
                                            minLength: 0,
                                            scroll: true,
                                            select: function (event, ui) {
                                                var val = ui.item.value;
                                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
                                            }
                                        }).focus(function () {
                                            $(this).autocomplete("search", "");
                                        });

                                    }
                                }
                            }

                        });
                    }
                }
            });
            $("#datatable").width($(".JCLRgrips").width());

            FilterIsArchiveData();
            jobIndexTable.draw();
        },

        //bServerSide: true,
        //sAjaxSource: urlGetJobListUserWiseColumns,
        "ajax": {
            "url": urlGetJobListUserWiseColumns,
            "data": (document.getElementById("ResellerId") != null) ? { solarcompanyid: $("#hdnsolarCompanyid").val(), sResellerId: document.getElementById("ResellerId").value, isAllScaJobView: isAllScaJobView, isShowOnlyAssignJobsSCO: isShowOnlyAssignJobsSCO } : { solarcompanyid: $("#hdnsolarCompanyid").val(), isAllScaJobView: isAllScaJobView, isShowOnlyAssignJobsSCO: isShowOnlyAssignJobsSCO },
            "dataSrc": '',
            "dataType": "json",
            "contentType": "application/json; charset=utf-8"
        },

        "fnDrawCallback": function () {
            if ($('#chkAll').prop('checked') == false) {
                var table = $('#datatable').DataTable();
                var rows = table.rows({ 'search': 'applied' }).nodes();
                $('input[type="checkbox"]', rows).prop('checked', false);
            }

            $("#btotalTradedSTC").html(0);
            if ($('#chkAll').prop('checked') == true) {
                chkCount = $('#datatable >tbody >tr').length;
                var tSTC = 0;
                $('#datatable tbody input[type="checkbox"]').each(function () {
                    var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
                    tSTC = (parseFloat(parseFloat(tSTC) + parseFloat(cSTC)).toFixed(2));
                })
                $("#btotalTradedSTC").html(tSTC);
            }
            else {
                chkCount = 0;
                $("#btotalTradedSTC").html(0);
            }
            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length >= 1) {
                $("#datatable_length").show();
            }
            else if ($("#datatable").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            var info = table.page.info();

            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsDisplay + '</b>  Jobs found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsDisplay + '</b>  Jobs found';
            }
            //------------------------------------------------------------------------------------------------------------------------------

            $('[data-toggle="tooltip"]').tooltip();

            var arrColumns = listColumns.concat(",Action,Chkbox").split(',');
            $.each(arrColumns, function (i, col) {
                if (col == "Action") {
                    ((UserTypeID == 6) || (isAssignSSC == 'true' && (UserTypeID == 1 || (UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) ? table.columns(col + ":name").visible(true) : table.columns(col + ":name").visible(false);
                }
                else
                    table.columns(col + ":name").visible(true);
            });

            // Assign Width - In Percentage
            var arrColumnsWidth = [];
            arrColumnsWidth.push(50);
            //arrColumnsWidth = listColumnsWidth.split(',');
            $.each(listColumnsWidth.split(','), function (i, data) {
                arrColumnsWidth.push(data);
            });
            arrColumnsWidth.push(50);
            var widthCount = 0;
            $('#trHeadersDynamic th').each(function (i, item) {
                widthCount += parseInt(arrColumnsWidth[i]);
                $(item).innerWidth(arrColumnsWidth[i]);
            });
            $("#datatable").innerWidth(widthCount + 100);
            if ($('#btnLock').val() == "Unlock") {
                DataTableColResize($('#trHeadersDynamic th').length);
                DataTableColResizeNotAll($('#trHeadersDynamic th').length);
            }

            // Assign Width - In Pixel convert from Percentage
            //var parentWidth = $('#datatable').width();
            //var arrColumnsWidth = listColumnsWidth.split(',');
            //arrColumnsWidth.push(2);
            //arrColumnsWidth.push(2);            
            //$('#trHeadersDynamic th').each(function (i, item) {
            //    var thWidthPercent = (arrColumnsWidth[i] * parentWidth) / 100;
            //    $(item).css('width', thWidthPercent + 'px');
            //});
        },

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

            if (aData.UserID == 0) {
                window.location = urlLogout;
            }

            if (aData["Urgent"] == "1") {
                nRow.className = "urgentrow";
            }
        },

        //"fnServerParams": function (aoData) {
        //    aoData.push({ "name": "stageid", "value": SelectedStageId });
        //    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        //        aoData.push({ "name": "solarcompanyid", "value": $("#hdnsolarCompanyid").val() });
        //        aoData.push({ "name": "isarchive", "value": document.getElementById("chkIsArchive").checked });
        //    }
        //    if (UserTypeID == 6) {
        //        aoData.push({ "name": "solarcompanyid", "value": $("#hdnsolarCompanyid").val() });
        //    }
        //    aoData.push({ "name": "scheduletype", "value": document.getElementById("JobScheduleTypeId").value });
        //    aoData.push({ "name": "jobtype", "value": document.getElementById("JobTypeId").value });
        //    aoData.push({ "name": "jobpriority", "value": document.getElementById("JobPriorityId").value });
        //    aoData.push({ "name": "searchtext", "value": $("#txtSearchfor").val() });
        //    aoData.push({ "name": "fromdate", "value": GetDates($("#txtFromDate").val()) });
        //    aoData.push({ "name": "todate", "value": GetDates($("#txtToDate").val()) });
        //    aoData.push({ "name": "jobref", "value": document.getElementById("chkJobRef").checked });
        //    aoData.push({ "name": "jobdescription", "value": document.getElementById("chkJobDescription").checked });
        //    aoData.push({ "name": "jobaddress", "value": document.getElementById("chkJobAddress").checked });
        //    aoData.push({ "name": "jobclient", "value": document.getElementById("chkClient").checked });
        //    aoData.push({ "name": "jobstaff", "value": document.getElementById("chkStaff").checked });
        //    aoData.push({ "name": "invoiced", "value": document.getElementById("chkInvoiced").checked });
        //    aoData.push({ "name": "notinvoiced", "value": document.getElementById("chkNotInvoiced").checked });
        //    aoData.push({ "name": "readytotrade", "value": document.getElementById("chkReadyToTrade").checked });
        //    aoData.push({ "name": "notreadytotrade", "value": document.getElementById("chkNotReadyToTrade").checked });
        //    aoData.push({ "name": "traded", "value": document.getElementById("chkTraded").checked });
        //    aoData.push({ "name": "nottraded", "value": document.getElementById("chkNotTraded").checked });
        //    aoData.push({ "name": "preapprovalnotapproved", "value": document.getElementById("chkPreApprovalNotBeenApproved").checked });
        //    aoData.push({ "name": "preapprovalapproved", "value": document.getElementById("chkPreApprovalApproved").checked });
        //    aoData.push({ "name": "connectioncompleted", "value": document.getElementById("chkConnectionCompleted").checked });
        //    aoData.push({ "name": "connectionnotcompleted", "value": document.getElementById("chkConnectionNotCompleted").checked });
        //    aoData.push({ "name": "ACT", "value": document.getElementById("chkACT").checked });
        //    aoData.push({ "name": "NSW", "value": document.getElementById("chkNSW").checked });
        //    aoData.push({ "name": "NT", "value": document.getElementById("chkNT").checked });
        //    aoData.push({ "name": "QLD", "value": document.getElementById("chkQLD").checked });
        //    aoData.push({ "name": "SA", "value": document.getElementById("chkSA").checked });
        //    aoData.push({ "name": "TAS", "value": document.getElementById("chkTAS").checked });
        //    aoData.push({ "name": "WA", "value": document.getElementById("chkWA").checked });
        //    aoData.push({ "name": "VIC", "value": document.getElementById("chkVIC").checked });
        //    aoData.push({ "name": "preapprovalstatusid", "value": document.getElementById("PreApprovalStatusId").value });
        //    aoData.push({ "name": "connectionstatusid", "value": document.getElementById("ConnectionStatusId").value });
        //},        
    });

    jobIndexTable.columns().every(function (index) {
        if (jobIndexTable.columns().column('Action:name').index() == index || jobIndexTable.columns().column('Chkbox:name').index() == index) {
            return;
        }
        var that = this;
        $('.colWiseSearch', this.footer()).on('keyup change', function () {
            //if (that.visible()) {
            //    if (that.search() !== this.value) {
            //        that.search(this.value).draw();
            //    }
            //}
            jobIndexTable.column($(this).parent().index() + ':visible').search(this.value).draw();
        });
    });
    $('#datatable').on('length.dt', function (e, settings, len) {
        SetPageSizeForGrid(len, false, ViewPageId)
        GridConfig[0].PageSize = len;
        GridConfig[0].IsKendoGrid = false;
        GridConfig[0].ViewPageId = parseInt(ViewPageId);
    });
    //$('#txtSearchfor').unbind().on('keyup', function () {
    //    var searchTerm = this.value.toLowerCase();
    //    //var colSearchName = "Priority,IsPreApprovaApproved,IsConnectionCompleted,InstallationState";
    //    //var arrColSearchName = colSearchName.split(',')
    //    var colSearchIndex = [];
    //    $.each(arrAdvanceSearchFilters, function (index, colname) {
    //        colSearchIndex.push(jobIndexTable.columns().column(colname + ":name").index());
    //    });

    //    $.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
    //        for (var i = 0, ien = colSearchIndex.length; i < ien; i++) {
    //            if (searchData[colSearchIndex[i]].toLowerCase().indexOf(searchTerm) !== -1) {
    //                return true;
    //            }
    //        }
    //        return false;
    //    })
    //    jobIndexTable.draw();
    //    $.fn.dataTable.ext.search.pop();
    //});
}

function datatableInfo() {
    cols = [];
    colsDefs = [];
    var dbFields = []; // dynamic column viewbag
    dbFields = listColumns.split(',');

    cols.push({ 'name': 'Chkbox', 'data': 'Id', 'orderable': false, 'render': fn_Chkbox });
    $.each(dbFields, function (i, e) {
        var var_orderable = false;
        if (e == "Priority" || e == "RefNumber" || e == "ClientName" || e == "CreatedDate" || e == "JobStage" || e == "InstallationDate" || e == "STC" || e == "StaffName" || e == "SCOName")
            var_orderable = true;

        if (CheckRender(e)) {
            cols.push({ 'name': e, 'data': e, 'orderable': var_orderable, 'render': eval('fn_' + e) });
        }
        else
            cols.push({ 'name': e, 'data': e, 'orderable': var_orderable });
    });

    cols.push({ 'name': 'Action', 'data': 'Id', 'orderable': false, 'render': fn_Action, 'visible': ((UserTypeID == 6) || (isAssignSSC == 'true' && (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 5 || (UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) ? true : false});


    if (listColumns.indexOf("CreatedDate") == -1)
        cols.push({ 'name': 'CreatedDate', 'data': 'CreatedDate', 'visible': false, 'render': fn_CreatedDate });
    if (listColumns.indexOf("InstallationDate") == -1)
        cols.push({ 'name': 'InstallationDate', 'data': 'InstallationDate', 'visible': false, 'render': fn_InstallationDate });
    if (listColumns.indexOf("JobTypeId") == -1)
        cols.push({ 'name': 'JobTypeId', 'data': 'JobTypeId', 'visible': false, 'render': fn_JobTypeId });
    if (listColumns.indexOf("RefNumber") == -1)
        cols.push({ 'name': 'RefNumber', 'data': 'RefNumber', 'visible': false, 'render': fn_RefNumber });
    if (listColumns.indexOf("JobAddress") == -1)
        cols.push({ 'name': 'JobAddress', 'data': 'JobAddress', 'visible': false, 'render': fn_JobAddress });
    if (listColumns.indexOf("JobTitle") == -1)
        cols.push({ 'name': 'JobTitle', 'data': 'JobTitle', 'visible': false });
    if (listColumns.indexOf("JobDescription") == -1)
        cols.push({ 'name': 'JobDescription', 'data': 'JobDescription', 'visible': false });
    if (listColumns.indexOf("FullOwnerCompanyDetails") == -1)
        cols.push({ 'name': 'FullOwnerCompanyDetails', 'data': 'FullOwnerCompanyDetails', 'visible': false });
    if (listColumns.indexOf("StaffName") == -1)
        cols.push({ 'name': 'StaffName', 'data': 'StaffName', 'visible': false, 'render': fn_StaffName });
    if (listColumns.indexOf("InstallerFullName") == -1)
        cols.push({ 'name': 'InstallerFullName', 'data': 'InstallerFullName', 'visible': false });
    if (listColumns.indexOf("DesignerFullName") == -1)
        cols.push({ 'name': 'DesignerFullName', 'data': 'DesignerFullName', 'visible': false });
    if (listColumns.indexOf("ElectricianFullName") == -1)
        cols.push({ 'name': 'ElectricianFullName', 'data': 'ElectricianFullName', 'visible': false });
    if (listColumns.indexOf("InstallationState") == -1)
        cols.push({ 'name': 'InstallationState', 'data': 'InstallationState', 'visible': false });
    if (listColumns.indexOf("Priority") == -1)
        cols.push({ 'name': 'Priority', 'data': 'Priority', 'visible': false, 'render': fn_Priority });
    if (listColumns.indexOf("IsPreApprovaApproved") == -1)
        cols.push({ 'name': 'IsPreApprovaApproved', 'data': 'IsPreApprovaApproved', 'visible': false, 'render': fn_IsPreApprovaApproved });
    if (listColumns.indexOf("IsConnectionCompleted") == -1)
        cols.push({ 'name': 'IsConnectionCompleted', 'data': 'IsConnectionCompleted', 'visible': false, 'render': fn_IsConnectionCompleted });
    if (listColumns.indexOf("IsTraded") == -1)
        cols.push({ 'name': 'IsTraded', 'data': 'IsTraded', 'visible': false, 'render': fn_IsTraded });
    if (listColumns.indexOf("IsDeleted") == -1)
        cols.push({ 'name': 'IsDeleted', 'data': 'IsDeleted', 'visible': false });
    if (listColumns.indexOf("JobNumber") == -1)
        cols.push({ 'name': 'JobNumber', 'data': 'JobNumber', 'visible': false });
    if (listColumns.indexOf("JobID") == -1)
        cols.push({ 'name': 'JobID', 'data': 'JobID', 'visible': false });
    if (listColumns.indexOf("IsGst") == -1)
        cols.push({ 'name': 'IsGst', 'data': 'IsGst', 'visible': false, 'render': fn_IsGst });
    if (listColumns.indexOf("PropertyType") == -1)
        cols.push({ 'name': 'PropertyType', 'data': 'PropertyType', 'visible': false });
    if (listColumns.indexOf("IsCustPrice") == -1)
        cols.push({ 'name': 'IsCustPrice', 'data': 'IsCustPrice', 'visible': false, 'render': fn_IsCustPrice });

    var dbFieldsWidth = []; // dynamic column viewbag
    dbFieldsWidth = listColumnsWidth.split(',');
    var iCnt = 0;
    colsDefs.push({ "width": '50', "targets": iCnt });
    $.each(dbFieldsWidth, function (i, e) {
        iCnt = iCnt + 1;
        colsDefs.push({ "width": e, "targets": iCnt });
    });
    iCnt = iCnt + 1;
    colsDefs.push({ "width": '50', "targets": iCnt });
    //colsDefs.push({ targets: '-1,-2', visible: true });
    colsDefs.push({ targets: '0,-1', visible: true });
    colsDefs.push({ targets: '_all', visible: false });

    function CheckRender(functionName) {
        var isDefined = eval('(typeof fn_' + functionName + '==\'function\');');
        if (isDefined) {
            return true;
        }
        else {
            return false;
        }
    }
}

function funTrHeadersDynamic(listUserWiseColumns) {

    //var trFooterData = '';
    //$.each(listUserWiseColumns, function (i, col) {
    //    trFooterData += "<td>" + col.DisplayName + "</td>";
    //});

    //if (trFooterData != null) {
    //    trFooterData += "<td style=\"display:none\"></td>";
    //    trFooterData += "<td style=\"display:none\"></td>";

    //    $('#trFooterDynamic').empty();
    //    $('#trFooterDynamic').append(trFooterData);
    //}

    var trHeaderData = '';

    trHeaderData += "<th style='width: 50; background-image: none !important;'><input type='checkbox' id='chkAll' name='select_all'></th>";
    $.each(listUserWiseColumns, function (i, col) {
        //trData += "<th width='" + col.Width + "%" + "' data-columnid='" + col.ColumnID + "' data-order='" + col.OrderNumber + "'><span>" + col.DisplayName + "</span></th>";
        trHeaderData += "<th style='width:" + col.Width + "' data-columnid='" + col.ColumnID + "' data-order='" + col.OrderNumber + "'><span>" + col.DisplayName + "</span></th>";
    });

    if (trHeaderData != null) {
        //trData += "<th width='2%' class='action'>Action</th>";
        //trData += "<th width='2%' style='background-image: none !important;'><input type='checkbox' id='chkAll' name='select_all'></th>";

        trHeaderData += "<th style='width: 50;' class='action'>Action</th>";

        $('#trHeadersDynamic').empty();
        $('#trHeadersDynamic').append(trHeaderData);
        $("#datatable > tbody").html("");
    }
}

$('.panel-heading input[type=checkbox]').change(function () {
    $(this).closest('.panel').find('[type=checkbox]').prop('checked', this.checked);

    if ($(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]').length == $(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]:checked').length) {
        if ($(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]').length != 0)
            $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').prop('checked', 'checked');
    }
    else {
        $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').removeAttr('checked');
    }

    $('.columnmaster li[data-columnid="2"]').find('input[type="checkbox"]').prop("checked", true);
});

function SaveFilter() {

    arrAdvanceSearchFilters = [];
    $('div[id*="popup_"]').each(function () {
        //var item = $(this);
        var divID = $(this).attr('id');
        var hdnIdValue = $('#' + divID + " input[type=checkbox]:checked").map(function () {

            if (this.id != 'DeSel_' + divID.replace("popup_", '') && this.id != 'Sel_' + divID.replace("popup_", '')) {
                return this.id.replace("popup_", '');
            }
        }).get().join(",");

        var IdValue = $('#' + divID + " input[type=checkbox]:checked").map(function () {

            if (this.id != 'DeSel_' + divID.replace("popup_", '') && this.id != 'Sel_' + divID.replace("popup_", '')) {
                return this.value;
            }
        }).get().join(",");

        $('#Filter_' + divID.replace("popup_", '')).text(IdValue);
        $('#hdnFilter_' + divID.replace("popup_", '')).text(hdnIdValue);

        localStorage.setItem('locFilter_' + divID.replace("popup_", ''), IdValue);
        localStorage.setItem('lochdnFilter_' + divID.replace("popup_", ''), hdnIdValue);

        if (divID.replace("popup_", '') == 3 || divID.replace("popup_", '') == 4 || divID.replace("popup_", '') == 5 || divID.replace("popup_", '') == 6 || divID.replace("popup_", '') == 7 || divID.replace("popup_", '') == 8 || divID.replace("popup_", '') == 9 || divID.replace("popup_", '') == 10 || divID.replace("popup_", '') == 11) {
            if ($(this).data('columnname') != '' || $(this).data('columnname') != null) {
                if (hdnIdValue != '' && hdnIdValue != null) {
                    arrAdvanceSearchFilters.push($(this).data('columnname'));
                }
            }
        }
        else {
            hdnIdValue.split(',').map(function (index) {
                if (index != '' && index != null) {
                    arrAdvanceSearchFilters.push(index);
                }
            });
        }

    });
    $("#Advancefilter").modal('hide');

    $("#btnChangeSearchFilter").val("Change Filter *");
}

function SetAdvanceSearchFiltersColumns() {
    arrAdvanceSearchFilters = [];

    $('[id*="hdnFilter_"]').each(function () {
        //var item = $(this);
        var divID = $(this).attr('id');
        if (divID.replace("hdnFilter_", '') == 3 || divID.replace("hdnFilter_", '') == 4 || divID.replace("hdnFilter_", '') == 5 || divID.replace("hdnFilter_", '') == 6 || divID.replace("hdnFilter_", '') == 7 || divID.replace("hdnFilter_", '') == 8 || divID.replace("hdnFilter_", '') == 9 || divID.replace("hdnFilter_", '') == 10 || divID.replace("hdnFilter_", '') == 11) {
            if ($(this).data('columnname') != '' || $(this).data('columnname') != null) {
                arrAdvanceSearchFilters.push($(this).data('columnname'));
            }
        }
        else {
            $(this).text().split(',').map(function (index) {
                if (index != '' && index != null) {
                    arrAdvanceSearchFilters.push(index);
                }
            });
        }
    });
}

function showErrorMessageUserWiseColumns(message) {
    $(".alert").hide();
    $("#successMsgRegionUserWiseColumns").hide();
    $("#errorMsgRegionUserWiseColumns").html(closeButton + message);
    $("#errorMsgRegionUserWiseColumns").show();
}

function showSuccessMessageUserWiseColumns(message) {
    $(".alert").hide();
    $("#errorMsgRegionUserWiseColumns").hide();
    $("#successMsgRegionUserWiseColumns").html(closeButton + message);
    $("#successMsgRegionUserWiseColumns").show();
}

function showErrorMessageColumnMaster(message) {
    $(".alert").hide();
    $("#errorMsgRegionColumnMaster").html(closeButton + message);
    $("#errorMsgRegionColumnMaster").show();
}


var fn_Priority = function (data, type, full, meta) {

    if (full.Priority == 1) {
        if (full.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-high.png" alt="" / ><span style="display:none">High</span></span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-high.png" alt="High" title="High"><span style="display:none">High</span></span></div>'
        }
    }
    else if (full.Priority == 2) {
        if (full.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-mid.png" alt="" /><span style="display:none">Normal</span></span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-mid.png" alt="Normal" title="Normal"><span style="display:none">Normal</span></span></div>';
        }
    }
    else {
        if (full.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-low.png" alt="" /></span><span style="display:none">Low</span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-low.png" alt="Low" title="Low"><span style="display:none">Low</span></span></div>'
        }
    }
};

var fn_RefNumber = function (data, type, full, meta) {
    if (full.RefNumber != null) {
        var url = urlIndex + full.Id;

        return '<a href="' + url + '" style="text-decoration:none;">' + full.RefNumber + '</a>'
    }
    else {
        return '';
    }
};

//var fn_ClientName = function (data, type, full, meta) {
//    if (full.ClientName != null) {
//        return full.ClientName + '<br/>' + 'Ph:' + full.phone;
//    }
//    else {
//        return '';
//    }
//};

var fn_JobAddress = function (data, type, full, meta) {
    if (full.JobAddress != null || full.JobAddress != '') {
        return full.JobAddress
    }
    else {
        return '';
    }
};

var fn_JobStage = function (data, type, full, meta) {
    if (full.JobStage != null || full.JobStage != '') {
        return '<p style="color:' + full.ColorCode + '">' + full.JobStage + '</p>'
    }
    else {
        return '';
    }
};

var fn_CreatedDate = function (data, type, full, meta) {
    return ToDate(full.strCreatedDate);
};

var fn_InstallationDate = function (data, type, full, meta) {
    return ToDate(full.strInstallationDate);
};

var fn_SignatureDate = function (data, type, full, meta) {
    return ToDate(full.strSignatureDate);
};

var fn_InstallerSignatureDate = function (data, type, full, meta) {
    return ToDate(full.strInstallerSignatureDate);
};

var fn_DesignerSignatureDate = function (data, type, full, meta) {
    return ToDate(full.strDesignerSignatureDate);
};

var fn_ElectricianSignatureDate = function (data, type, full, meta) {
    return ToDate(full.strElectricianSignatureDate);
};

var fn_IsPreApprovaApproved = function (data, type, full, meta) {
    if (full.IsPreApprovaApproved) {
        return '<img src="../Images/active.png" alt="Active" title="Active"><span style="display:none">Approved</span>'
    }
    else {
        return '<img src="../Images/inactive.png" alt="In Active" title="In Active"><span style="display:none">Pending</span>'
    }
};

var fn_IsConnectionCompleted = function (data, type, full, meta) {
    if (full.IsConnectionCompleted) {
        return '<img src="../Images/active.png" alt="Active" title="Active"><span style="display:none">Completed</span>'
    }
    else {
        return '<img src="../Images/inactive.png" alt="In Active" title="In Active"><span style="display:none">Pending</span>'
    }
};

var fn_IsInvoiced = function (data, type, full, meta) {
    if (full.IsInvoiced) {
        return '<img src="../Images/ar_green.png"/><span style="display:none">Invoiced</span>';
    }
    else {
        return '<img src="../Images/ic_reject.png"/><span style="display:none">Pending</span>';
    }
};

var fn_DocumentCount = function (data, type, full, meta) {
    if (full.IsLatestDocument) {
        return '<b style="color:red">' + full.DocumentCount + '</b>';
    }
    else {
        return '<p>' + full.DocumentCount + '</p>'
    }
};

var fn_STC = function (data, type, full, meta) {
    return '<lable class="clsLabel">' + PrintDecimal(full.STC) + '</lable>';
};

var fn_StaffName = function (data, type, full, meta) {
    if (full.StaffName != null) {
        return full.StaffName;
    }
    else {
        return '';
    }
};

var fn_SCOName = function (data, type, full, meta) {
    if (full.SCOName != null) {
        return full.SCOName;
    }
    else {
        return '';
    }
};

var fn_JobTypeId = function (data, type, full, meta) {
    if (full.JobTypeId > 0) {
        if (full.JobTypeId == 1) {
            return 'PVD';
        }
        else if (full.JobTypeId == 2) {
            return 'SWH';
        }
    }
    else
        return '';
};

var fn_IsGst = function (data, type, full, meta) {

    if (full.IsGst) {
        return '<span style="font-weight: bold;color: Green;">Yes</span>';
    }
    else {
        return '<span style="font-weight: bold;color: Red;">No</span>';
    }
};

//var fn_IsCommercial = function (data, type, full, meta) {

//    if (full.IsCommercial) {
//        return '<span style="font-weight: bold;color: Green;">Yes</span>';
//    }
//    else {
//        return '<span style="font-weight: bold;color: Red;">No</span>';
//    }
//};

var fn_IsCustPrice = function (data, type, full, meta) {

    if (full.IsCustPrice) {
        return '<span style="font-weight: bold;color: Green;">Yes</span>';
    }
    else {
        return '<span style="font-weight: bold;color: Red;">No</span>';
    }
};

var fn_IsTraded = function (data, type, full, meta) {
    //if (full.IsTraded) {
    //    return '<img src="../Images/ar_green.png"/><span style="display:none">Traded</span>';
    //}
    //else {
    //    if (full.IsReadyToTrade) {
    //        return '<img src="../Images/ic_ReadyToTrade.png"/><span style="display:none">Ready To Trade</span>';
    //    }
    //    else {
    //        return '<img src="../Images/ic_reject.png"/><span style="display:none">Not Traded</span>';
    //    }
    //}
    if (full.TradeStatus == 1) {
        return '<img src="../Images/ar_green.png"/><span style="display:none">Traded</span>';
    }
    else if (full.TradeStatus == 2) {
        return '<img src="../Images/ic_ReadyToTrade.png"/><span style="display:none">ReadyToTrade</span>';
    }
    else if (full.TradeStatus == 3) {
        return '<img src="../Images/ic_reject.png"/><span style="display:none">NotTraded</span>';
    }
};

var fn_Action = function (data, type, full, meta) {
    if (UserTypeID != 6) {
        var assignSSCButton = "";
        if (full.JobTypeId == 1) {
            if (full.SSCID == null || full.SSCID == 0) {
                imgassignSSC = 'background:url(../Images/ic_assign.png) no-repeat center; text-decoration:none;width:100%;';
            }
            else {
                imgassignSSC = 'background:url(../Images/ic_assign_green.png) no-repeat center; text-decoration:none;width:100%;';
            }
            var assignSSCHref = "javascript:AssignSSC('" + full.Id + "')";
            assignSSCButton = '<a href="' + assignSSCHref + '" class="action delete" style="' + imgassignSSC + '" title="Assign SSC">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
        }
        else {
            imgassignSSC = 'background:url(../Images/ic_assign.png) no-repeat center; text-decoration:none;width:100%;opacity:0.5;cursor:default;';
            assignSSCButton = '<a  class="action delete" style="' + imgassignSSC + '" title="Assign SSC">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
        }
        return assignSSCButton;
    }
    else if (UserTypeID == 6) {
        imgaccept = 'background:url(../Images/ic-accept.png) no-repeat center; text-decoration:none;';
        imgreject = 'background:url(../Images/ic-reject.png) no-repeat center; text-decoration:none;';
        if (full.IsAccept == 1) {
            var acceptHref = "javascript:void(0)";
            acceptButton = '&nbsp;&nbsp;' + '<a href="' + acceptHref + '" class="action delete disabled" style="' + imgaccept + '" title="Accept">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
        }
        else {
            var acceptHref = "javascript:AcceptRejectJobToSSC('" + full.Id + "'" + ",'accepted')";
            acceptButton = '&nbsp;&nbsp;' + '<a href="' + acceptHref + '" class="action delete" style="' + imgaccept + '" title="Accept">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
        }
        var rejectHref = "javascript:AcceptRejectJobToSSC('" + full.Id + "'" + ",'rejected')";
        rejectButton = '&nbsp;&nbsp;' + '<a href="' + rejectHref + '" class="action delete" style="' + imgreject + '" title="Reject">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
        return acceptButton + rejectButton;
    }
};

var fn_Chkbox = function (data, type, full, meta) {
    if ($('#chkAll').prop('checked') == true) {
        return '<input type="checkbox" JobId="' + full.JobID + '" id="chk_' + full.Id + '" JobTitle="' + full.Title + '" checked>';
    }
    else {
        return '<input type="checkbox" JobId="' + full.JobID + '" id="chk_' + full.Id + '" JobTitle="' + full.Title + '">';
    }
};

var fn_IsSTCForm = function (data, type, full, meta) {
    if (full.IsSTCForm != null) {
        if (full.IsSTCForm) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsCESForm = function (data, type, full, meta) {
    if (full.IsCESForm != null) {
        if (full.IsCESForm) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsBasicValidation = function (data, type, full, meta) {
    if (full.IsBasicValidation != null) {
        if (full.IsBasicValidation) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsOwnerSiganture = function (data, type, full, meta) {
    if (full.IsOwnerSiganture != null) {
        if (full.IsOwnerSiganture) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsInstallerSiganture = function (data, type, full, meta) {
    if (full.IsInstallerSiganture != null) {
        if (full.IsInstallerSiganture) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsGroupSiganture = function (data, type, full, meta) {
    if (full.IsGroupSiganture != null) {
        if (full.IsGroupSiganture) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsSerialNumberCheck = function (data, type, full, meta) {
    if (full.IsSerialNumberCheck != null) {
        if (full.IsSerialNumberCheck) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

var fn_IsSTCSubmissionPhotos = function (data, type, full, meta) {
    if (full.IsSTCSubmissionPhotos != null) {
        if (full.IsSTCSubmissionPhotos) {
            return '<span style="font-weight: bold;color: Green;">Yes</span>';
        }
        else {
            return '<span style="font-weight: bold;color: Red;">No</span>';
        }
    }
    else {
        return '';
    }
};

function SelectAdvanceSearchCategory(id) {
    $('#DeSel_' + id).prop("checked", false);
    $('#popup_' + id + ' ul input[type=checkbox]').map(function () {
        this.checked = true;
    });
}

function DeSelectAdvanceSearchCategory(id) {
    $('#Sel_' + id).prop("checked", false);
    $('#popup_' + id + ' ul input[type=checkbox]').map(function () {
        this.checked = false;
    });
}
function SelectAllAdvanceSearchCategory() {
    SelectDeselectSectionWise();
    $('#DeSel_All').prop("checked", false);
    $('.filters-block ul input[type=checkbox]').map(function () {
        this.checked = true;
    });
    $('.filters-block ul input[type=checkbox][id="popup_JobDescription"]').prop("checked", false);
    $('.filters-block ul input[type=checkbox][id="popup_StaffName"]').prop("checked", false);
    $('.filters-block ul input[type=checkbox][id="popup_InstallerFullName"]').prop("checked", false);
    $('.filters-block ul input[type=checkbox][id="popup_DesignerFullName"]').prop("checked", false);
    $('.filters-block ul input[type=checkbox][id="popup_ElectricianFullName"]').prop("checked", false);
}

function DeSelectAllAdvanceSearchCategory() {
    SelectDeselectSectionWise();
    $('#Sel_All').prop("checked", false);
    $('.filters-block ul input[type=checkbox]').map(function () {
        this.checked = false;
    });
}

function SelectDeselectSectionWise() {
    $('.filters-title input[type=checkbox]').map(function () {
        this.checked = false;
    });
}


$("#btnChangeSearchFilter").click(function (e) {
    var selectValues = [];
    for (var i = 1; i <= 11; i++) {
        if (localStorage.getItem('lochdnFilter_' + i) != null && localStorage.getItem('lochdnFilter_' + i) != '') {
            $.each(localStorage.getItem('lochdnFilter_' + i).split(','), function (i, data) {
                selectValues[selectValues.length] = data;
            });
        }
    }

    if (selectValues.length == 0) {
        SelectAllAdvanceSearchCategory();
    }
    else {
        DeSelectAllAdvanceSearchCategory();
        $.each(selectValues, function (key, value) {
            $('.filters-block ul input[type=checkbox][id="popup_' + value + '"]').prop("checked", true);
        });
    }

});

$("#aSwitch").click(function (e) {
    window.location.href = '/Job/Index?IsStaticSearch=false';
});

$('#clearJobCreatedDate').click(function (e) {
    $("#txtFromDate").val('');
    $("#txtToDate").val('');
});

$('#clearJobInstallationDate').click(function (e) {
    $("#txtFromDate2").val('');
    $("#txtToDate2").val('');
});

function checkIsWholeSaler(rId) {

    $.ajax({
        url: urlCheckIsWholeSaler_ByResellerId + rId,
        type: "get",
        async: false,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var response = JSON.parse(data)
            if (typeof (response) != "undefined" && response.Table[0] != null) {
                if (response.Table[0].IsWholeSaler) {
                    $('#IsWholeSaler').val(true);
                    $('#UserId_WholeSaler').val(response.Table[0].UserId);
                }
                else {
                    $('#IsWholeSaler').val(false);
                }
            }
        },
    });
}

function AssignSSCClickEvent() {

    $('#btnJobSSCMapping').click(function (e) {
        if ($("#SSCID").val() != '') {
            var ramid = $("#SSCID").val();
            var jobId = $('#SSCJobID').val();
            var SSCName = $("#SSCID option:selected").text();

            var ramid = $("#SSCID").val();
            var jobId = $('#SSCJobID').val();
            $.ajax({
                url: urlSaveJobToSSC,
                type: "GET",
                data: { ramId: ramid, jobId: jobId, SSCName: SSCName },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.success) {
                        $(".alert").hide();
                        $("#popuperrorMsgRegion").removeClass("alert-danger");
                        $("#popuperrorMsgRegion").addClass("alert-success");
                        $("#popuperrorMsgRegion").html(closeButton + "Job assigned to SSC successfully.");
                        $("#popuperrorMsgRegion").show();
                        //$("#datatable").dataTable().fnDraw();
                        datatableInfo();
                        $('#datatable').DataTable().destroy();
                        drawJobIndex();
                        location.reload();
                    }
                },
            });
        }
        else {
            $(".alert").hide();
            $("#popuperrorMsgRegion").removeClass("alert-success");
            $("#popuperrorMsgRegion").addClass("alert-danger");
            $("#popuperrorMsgRegion").html(closeButton + "Please select SSC first.");
            $("#popuperrorMsgRegion").show();
        }
        e.preventDefault();
        return false;
    });

    //Remove SSC request
    $('#btnSSCRemove').click(function () {
        var sscID = $("#SSCID").val();
        var notes = $('#RemoveSSCNote').val();
        var jobId = $('#SSCJobID').val();
        if (notes == "") {
            $("#RemoveSSCNote").next("span").attr('class', 'field-validation-error')
            $("#RemoveSSCNote").next("span").html('Notes is required.');
        }
        else {
            $.ajax({
                url: urlRemoveSSCRequest,
                type: "GET",
                data: { notes: notes, jobId: jobId, sscID: sscID },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.success) {
                        $(".alert").hide();
                        $("#popuperrorMsgRegion").removeClass("alert-danger");
                        $("#popuperrorMsgRegion").addClass("alert-success");
                        $("#popuperrorMsgRegion").html(closeButton + "Remove request send successfully.");
                        $("#popuperrorMsgRegion").show();
                        window.setTimeout(function () { location.reload() }, 100)
                        //$("#datatable").dataTable().fnDraw();
                        datatableInfo();
                        $('#datatable').DataTable().destroy();
                        drawJobIndex();
                    }
                },
            });
        }
    });

}