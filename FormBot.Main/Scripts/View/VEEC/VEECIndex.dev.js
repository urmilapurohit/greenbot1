var jobIndexTable;;
$(document).ready(function () {
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
    $('#txtFromDateCommencement').datepicker({
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
    $("#txtToDateCommencement").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtFromDateActivity').datepicker({
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
    $("#txtToDateActivity").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtToDate').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
    });
    $('#txtToDateCommencement').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
    });
    $('#txtToDateActivity').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDate").val(), dateFormat));
    });

    $('#txtFromDateInstalling').datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#txtToDateInstalling").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#txtFromDateInstalling").val(), dateFormat));
            var toDate = new Date(ConvertDateToTick($("#txtToDateInstalling").val(), dateFormat));
            if (fromDate > toDate) {
                $("#txtToDateInstalling").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#txtFromDateInstalling").val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if ($('#txtToDateInstalling').data('datepicker')) {
            $('#txtToDateInstalling').data('datepicker').setStartDate(new Date(tDate));
        }
    });
    $("#txtToDateInstalling").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtToDateInstalling').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtFromDateInstalling").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtToDateInstalling").val(), dateFormat));
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
                    if (IsResellerExist == false && res.Value == localStorage.getItem('VEECList_ResellerId')) {
                        IsResellerExist = true;
                    }
                });
                
                if (IsResellerExist) {
                    document.getElementById("ResellerId").value = localStorage.getItem('VEECList_ResellerId')
                }
                else {
                    $("#ResellerId").val($("#ResellerId option:first").val());
                    localStorage.setItem('VEEC_ResellerId', $("#ResellerId option:first").val());
                }

                if ($("#ResellerId").val() > 0 && (UserTypeID == 1 || UserTypeID == 3)) {
                    BindSolarCompany(document.getElementById("ResellerId").value);
                    //GetJobStageCount();
                    //checkIsWholeSaler($('#ResellerId').val());
                }
            },
            error: function (ex) {
                alert('Failed to retrieve Resellers.' + ex);
            }
        });
    }
    else if (UserTypeID == 2) {
        BindSolarCompany(ResellerId);
        //GetJobStageCount();
    }
    else if (UserTypeID == 5) {

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

                //GetJobStageCount();
            },
            error: function (ex) {
                alert('Failed to retrieve Solar Companies.' + ex);
            }
        });
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
        //GetJobStageCount();
    }
    //SetVeecParamsFromLocalStorage();
    jobIndexTable = $('#datatable').DataTable({
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_",
        },

        "aaSorting": [],

        columns: [
                {
                    'data': 'RefNumber',
                    "render": function (data, type, full, meta) {
                        if (full.RefNumber != null) {
                            var url = urlIndex + full.Id;

                            return '<a href="' + url + '" style="text-decoration:none;">' + full.RefNumber + '</a>'
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'ClientName',
                    "render": function (data, type, full, meta) {
                        if (full.ClientName != null) {
                            return full.ClientName + '<br/>' + 'Ph:' + full.phone;
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'JobAddress',
                    "render": function (data, type, full, meta) {
                        if (full.JobAddress != null || full.JobAddress != '') {
                            return full.JobAddress
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'strCreatedDate',
                    "render": function (data, type, full, meta) {
                        return ToDate(data);
                    }
                },
                {
                    'data': 'strCommencementDate',
                    "render": function (data, type, full, meta) {
                        return ToDate(data);
                    }
                },
                 {
                     'data': 'strActivityDate',
                     "render": function (data, type, full, meta) {
                         return ToDate(data);
                     }
                 },
               {
                   'data': 'Id',
                   "orderable": false,
                   "render": function (data, type, full, meta) {                     
                       if ($('#chkAll').prop('checked') == true) {
                           return '<input type="checkbox" VEECID="' + full.VEECId + '" id="chk_' + full.Id + '" checked>';
                       }
                       else {
                           return '<input type="checkbox" VEECID="' + full.VEECId + '" id="chk_' + full.Id + '">';
                       }
                   }
               },
        ],

        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        initComplete: function (settings, json) {
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
        },

        bServerSide: true,
        sAjaxSource: urlGetVEECList,
        "fnDrawCallback": function () {

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
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b>  Jobs found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b>  Jobs found';
            }
            //------------------------------------------------------------------------------------------------------------------------------

            //$('#chkAll').prop('checked', false);
            //var table = $('#datatable').DataTable();
            //var rows = table.rows({ 'search': 'applied' }).nodes();
            //$('input[type="checkbox"]', rows).prop('checked', false);
            //chkCount=0;
            $('[data-toggle="tooltip"]').tooltip();
        },

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

            if (aData.UserID == 0) {
                window.location = urlLogout;
            }

            if (aData["Urgent"] == "1") {
                nRow.className = "urgentrow";
            }
        },

        "fnServerParams": function (aoData) {
            aoData.push({ "name": "stageid", "value": SelectedStageId });
            if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
                aoData.push({ "name": "solarcompanyid", "value": $("#hdnsolarCompanyid").val() });
                //aoData.push({ "name": "isarchive", "value": document.getElementById("chkIsArchive").checked });
            }
            if (UserTypeID == 6) {
                aoData.push({ "name": "solarcompanyid", "value": $("#hdnsolarCompanyid").val() });
            } 
            aoData.push({ "name": "FromDate", "value": $("#txtFromDate").val() });
            aoData.push({ "name": "ToDate", "value": $("#txtToDate").val() });
            aoData.push({ "name": "FromDateCommencement", "value": $("#txtFromDateCommencement").val() });
            aoData.push({ "name": "ToDateCommencement", "value": $("#txtToDateCommencement").val() });
            aoData.push({ "name": "FromDateActivity", "value": $("#txtFromDateActivity").val() });
            aoData.push({ "name": "ToDateActivity", "value": $("#txtToDateActivity").val() });
            aoData.push({ "name": "Searchtext", "value": $("#txtSearchfor").val() });
            //aoData.push({ "name": "IsScheduled", "value": $("#JobScheduleTypeId").val() == "Scheduled"? "Yes" : "No" })
        }
    });
    
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
    $("#ResellerId").change(function () {
        if ($("#ResellerId").val() > 0 && (UserTypeID == 1 || UserTypeID == 3)) {
            IsServerCallForSearch = true;
            BindSolarCompany(document.getElementById("ResellerId").value);
            $('#searchSolarCompany').focus();
            //$('#searchSolarCompany').val('');
            //checkIsWholeSaler($('#ResellerId').val());
        }
    });
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
    
});

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

    cols.push({ 'name': 'Action', 'data': 'Id', 'orderable': false, 'render': fn_Action, 'visible': ((UserTypeID == 6) || (isAssignSSC == 'true' && ((UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) ? true : false, });


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

    var dbFieldsWidth = []; // dynamic column viewbag
    dbFieldsWidth = listColumnsWidth.split(',');
    var iCnt = 0;
    colsDefs.push({ "width": '2%', "targets": iCnt });
    $.each(dbFieldsWidth, function (i, e) {
        iCnt = iCnt + 1;
        colsDefs.push({ "width": e + '%', "targets": iCnt });
    });
    iCnt = iCnt + 1;
    colsDefs.push({ "width": '2%', "targets": iCnt });
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

                if (IsCompanyExist == false && company.Value == localStorage.getItem('VEECList_SolarCompanyId')) {
                    IsCompanyExist = true;
                }
            });

            if (IsCompanyExist) {
                //document.getElementById("SolarCompanyId").value = localStorage.getItem('JobList_SolarCompanyId');

                $('#hdnsolarCompanyid').val(localStorage.getItem('VEECList_SolarCompanyId'));
            }
            else {
                //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
                //localStorage.setItem('JobList_SolarCompanyId',$("#SolarCompanyId option:first").val());

                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('VEECList_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            if (localStorage.getItem('VEECList_SolarCompanyId') == '') {

                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('VEECList_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            $.each(solarCompanyList, function (key, value) {

                if (value.value == localStorage.getItem('VEECList_SolarCompanyId')) {

                    $("#searchSolarCompany").val(value.text);
                    $('#hdnsolarCompanyid').val(localStorage.getItem('VEECList_SolarCompanyId'));
                }
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
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

function SearchVEEC() {
    $("#loading-image").css("display", "");
    if (UserTypeID == 1 || UserTypeID == 3) {
        localStorage.setItem('VEECList_ResellerId', document.getElementById("ResellerId").value);
    }
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5 || UserTypeID == 6) {

        localStorage.setItem('JobList_SolarCompanyId', $('#hdnsolarCompanyid').val());
    }


    if (UserTypeID == 1 || UserTypeID == 3) {
        localStorage.setItem('ResellerId', document.getElementById("ResellerId").value);
    }
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5 || UserTypeID == 6) {
        localStorage.setItem('searchSolarCompany', $('#hdnsolarCompanyid').val());
    }


    jobIndexTable.draw();
    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
    //    localStorage.setItem('VEECList_IsArchive', document.getElementById("chkIsArchive").checked);
    //}
    //if (IsServerCallForSearch) {
    //    //datatableInfo();
    //    $('#datatable').DataTable().destroy();
    //    drawJobIndex();
    //    IsServerCallForSearch = false;
    //}

    //$.fn.dataTable.ext.search = [];

    //FilterIsArchiveData();

    //var searchTerm = $("#txtSearchfor").val().toLowerCase();
    //if (searchTerm != '' && searchTerm != null) {
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
    //}
    
    //$.fn.dataTable.ext.search.push(function (settings, searchData, dataIndex) {
    //    if ($('#hdnFilter_3').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("InstallationState:name").index()].toLowerCase()) !== -1
    //        &&
    //        $('#hdnFilter_4').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("Priority:name").index()].toLowerCase()) !== -1
    //         &&
    //        $('#hdnFilter_5').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsTraded:name").index()].toLowerCase()) !== -1
    //         &&
    //        $('#hdnFilter_6').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsPreApprovaApproved:name").index()].toLowerCase()) !== -1
    //         &&
    //        $('#hdnFilter_7').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("IsConnectionCompleted:name").index()].toLowerCase()) !== -1
    //         &&
    //        $('#hdnFilter_8').text().toLowerCase().split(",").indexOf(searchData[jobIndexTable.columns().column("JobTypeId:name").index()].toLowerCase()) !== -1
    //        ) {

    //        return true;
    //    }
    //    return false;
    //})

    //if ($('#txtFromDate').val() != '' && $('#txtToDate').val() != '') {
    //    var chunkscreate = $('#txtFromDate').val().split('/');
    //    mincreatedate = new Date([chunkscreate[2], chunkscreate[1], chunkscreate[0]].join("-"));
    //    var chunkscreate2 = $('#txtToDate').val().split('/');
    //    maxcreatedate = new Date([chunkscreate2[2], chunkscreate2[1], chunkscreate2[0]].join("-"));

    //    $.fn.dataTable.ext.search.push(
    //    function (settings, searchData, dataIndex) {
    //        if (typeof searchData._date == 'undefined') {
    //            var dateindex = jobIndexTable.columns().column('CreatedDate:name').index();
    //            searchData._date = searchData[dateindex];
    //            var chunks = searchData._date.split('/');
    //            searchData._date = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
    //        }
    //        if (searchData._date == "Invalid Date") {
    //            return false;
    //        }
    //        if (searchData._date < mincreatedate) {
    //            return false;
    //        }
    //        if (searchData._date > maxcreatedate) {
    //            return false;
    //        }
    //        return true;
    //    });
    //}

    //if ($('#txtFromDate2').val() != '' && $('#txtToDate2').val() != '') {

    //    var chunks = $('#txtFromDate2').val().split('/');
    //    min = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
    //    var chunks2 = $('#txtToDate2').val().split('/');
    //    max = new Date([chunks2[2], chunks2[1], chunks2[0]].join("-"));

    //    $.fn.dataTable.ext.search.push(
    //    function (settings, searchData, dataIndex) {
    //        if (typeof searchData._date2 == 'undefined') {
    //            var dateindex = jobIndexTable.columns().column('InstallationDate:name').index();
    //            searchData._date2 = searchData[dateindex];
    //            var chunks = searchData._date2.split('/');
    //            searchData._date2 = new Date([chunks[2], chunks[1], chunks[0]].join("-"));
    //        }
    //        if (searchData._date2 == "Invalid Date") {
    //            return false;
    //        }
    //        if (searchData._date2 < min) {
    //            return false;
    //        }

    //        if (searchData._date2 > max) {
    //            return false;
    //        }
    //        return true;
    //    });
    //}

    
    ////$.fn.dataTable.ext.search.pop();

    //if (UserTypeID != 7 || UserTypeID != 9 || UserTypeID != 10) {
    //    GetJobStageCount();
    //}

    //$("#loading-image").css("display", "none");

    //if ($('#btnLock').val() == "Lock") {
    //    DataTableColResize($('#trHeadersDynamic th').length);
    //    DisableAllColReorder();
    //}
}

function drawJobIndex() {

    $('.dropdown-filter-dropdown').remove();

    //$('#datatable tfoot td').each(function () {
    //    var title = $(this).text();
    //    $(this).html('<input class="colWiseSearch" type="text" placeholder="Search ' + title + '" />');
    //});

    jobIndexTable = $('#datatable').DataTable({
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_",
        },

        "aaSorting": [],

        //columnDefs: colsDefs,
        columns: [
                {
                    'data': 'RefNumber',
                    "render": function (data, type, full, meta) {
                        if (full.RefNumber != null) {
                            var url = urlIndex + full.Id;

                            return '<a href="' + url + '" style="text-decoration:none;">' + full.RefNumber + '</a>'
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'ClientName',
                    "render": function (data, type, full, meta) {
                        if (full.ClientName != null) {
                            return full.ClientName + '<br/>' + 'Ph:' + full.phone;
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'JobAddress',
                    "render": function (data, type, full, meta) {
                        if (full.JobAddress != null || full.JobAddress != '') {
                            return full.JobAddress
                        }
                        else {
                            return '';
                        }
                    },
                },
                {
                    'data': 'strCreatedDate',
                    "render": function (data, type, full, meta) {
                        return ToDate(data);
                    }
                },
                {
                    'data': 'strCommencementDate',
                    "render": function (data, type, full, meta) {
                        return ToDate(data);
                    }
                },
                 {
                     'data': 'strActivityDate',
                     "render": function (data, type, full, meta) {
                         return ToDate(data);
                     }
                 },
               {
                   'data': 'Id',
                   "orderable": false,
                   "render": function (data, type, full, meta) {
                       if ($('#chkAll').prop('checked') == true) {
                           return '<input type="checkbox" VEECID="' + full.VEECId + '" id="chk_' + full.Id + '" checked>';
                       }
                       else {
                           return '<input type="checkbox" VEECID="' + full.VEECId + '" id="chk_' + full.Id + '">';
                       }
                   }
               },
        ],
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
            //        jobIndexTable.column('IsOwnerSiganture:name').index() == index ||
            //        jobIndexTable.column('IsInstallerSiganture:name').index() == index ||
            //        jobIndexTable.column('IsGroupSiganture:name').index() == index ||
            //        jobIndexTable.column('IsSerialNumberCheck:name').index() == index ||
            //        jobIndexTable.column('IsSTCSubmissionPhotos:name').index() == index ||
            //        jobIndexTable.column('IsTraded:name').index() == index
            //        ) {
            //        var selectoptions = [];
            //        var selectoptions2 = [];
            //        var selecttrade = [];
            //        var column = this;
            //        var select = $('<input type="text" class=' + column.index() + ' />')
            //            .appendTo($(column.header()).find('div>div').empty());
            //        $('.' + column.index()).attr("Placeholder", "Search" + " " + column.header().innerText);
            //        select.addClass('dropdown-filter-menu-search form-control');
            //        select.addClass('getalldata');
            //        //  $(".caseValuePrecentageRestriction").on("keypress", function (event) {
            //        $('.' + column.index()).keyup(function (event) {
            //            if ($(this).val() == '') {
            //                //jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search('', true, false).draw();
            //                jobIndexTable.column($(this).parent().parent().parent().index() + ':visible').search('').draw();
            //            }
            //        })
            //        var arrTradeStatus = JSON.parse(colTradeStatus);
            //        if (jobIndexTable.column('IsTraded:name').index() == index) {

            //            $.each(arrTradeStatus, function (index, item) {
            //                selectoptions.push(item.Name);
            //            });
            //            $('.' + column.index()).autocomplete({
            //                source: selectoptions,
            //                minLength: 0,
            //                scroll: true,
            //                select: function (event, ui) {

            //                    var val = ui.item.value;
            //                    jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                }
            //            }).focus(function () {
            //                $(this).autocomplete("search", "");
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
            //                            selectoptions.push(item.Name);
            //                        }
            //                    });
            //                    $('.' + column.index()).autocomplete({
            //                        source: selectoptions,
            //                        minLength: 0,
            //                        scroll: true,
            //                        select: function (event, ui) {
            //                            var val = ui.item.value;
            //                            jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                        }
            //                    }).focus(function () {
            //                        $(this).autocomplete("search", "");
            //                    });

            //                }
            //                else {
            //                    if (jobIndexTable.column('IsGst:name').index() == index ||
            //                        jobIndexTable.column('IsSTCForm:name').index() == index ||
            //                        jobIndexTable.column('IsCESForm:name').index() == index ||
            //                        jobIndexTable.column('IsBasicValidation:name').index() == index ||
            //                        jobIndexTable.column('IsOwnerSiganture:name').index() == index ||
            //                        jobIndexTable.column('IsInstallerSiganture:name').index() == index ||
            //                        jobIndexTable.column('IsGroupSiganture:name').index() == index ||
            //                        jobIndexTable.column('IsSerialNumberCheck:name').index() == index ||
            //                        jobIndexTable.column('IsSTCSubmissionPhotos:name').index() == index) {
            //                        $('.' + column.index()).autocomplete({
            //                            source: ['Yes', 'No'],
            //                            minLength: 0,
            //                            scroll: true,
            //                            select: function (event, ui) {
            //                                var val = ui.item.value;
            //                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                            }
            //                        }).focus(function () {
            //                            $(this).autocomplete("search", "");
            //                        });

            //                    }
            //                    else if (jobIndexTable.column('IsInvoiced:name').index() == index) {
            //                        $('.' + column.index()).autocomplete({
            //                            source: ['Invoiced', 'Pending'],
            //                            minLength: 0,
            //                            scroll: true,
            //                            select: function (event, ui) {
            //                                var val = ui.item.value;
            //                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                            }
            //                        }).focus(function () {
            //                            $(this).autocomplete("search", "");
            //                        });

            //                    }
            //                    else if (jobIndexTable.column('IsPreApprovaApproved:name').index() == index) {
            //                        $('.' + column.index()).autocomplete({
            //                            source: ['Approved', 'Pending'],
            //                            minLength: 0,
            //                            scroll: true,
            //                            select: function (event, ui) {
            //                                var val = ui.item.value;
            //                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                            }
            //                        }).focus(function () {
            //                            $(this).autocomplete("search", "");
            //                        });
            //                    }
            //                    else if (jobIndexTable.column('IsConnectionCompleted:name').index() == index) {
            //                        $('.' + column.index()).autocomplete({
            //                            source: ['Completed', 'Pending'],
            //                            minLength: 0,
            //                            scroll: true,
            //                            select: function (event, ui) {
            //                                var val = ui.item.value;
            //                                jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                            }
            //                        }).focus(function () {
            //                            $(this).autocomplete("search", "");
            //                        });
            //                    }
            //                    else {
            //                        if (d != null && d != '' && d != ' ') {
            //                            selectoptions.push(d);
            //                            $('.' + column.index()).autocomplete({
            //                                source: selectoptions,
            //                                minLength: 0,
            //                                scroll: true,
            //                                select: function (event, ui) {
            //                                    var val = ui.item.value;
            //                                    jobIndexTable.column($(this.parentNode.parentNode.parentNode).index() + ':visible').search(val ? '^' + val + '$' : '', true, false).draw();
            //                                }
            //                            }).focus(function () {
            //                                $(this).autocomplete("search", "");
            //                            });

            //                        }
            //                    }
            //                }

            //            });
            //        }
            //    }
            //});

            $("#datatable").width($(".JCLRgrips").width());

            FilterIsArchiveData();
            jobIndexTable.draw();
        },

        //bServerSide: true,
        //sAjaxSource: urlGetJobListUserWiseColumns,
        "ajax": {
            "url": urlGetJobListUserWiseColumns,
            "data": (document.getElementById("ResellerId") != null) ? { solarcompanyid: $("#hdnsolarCompanyid").val(), sResellerId: document.getElementById("ResellerId").value } : { solarcompanyid: $("#hdnsolarCompanyid").val() },
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
                    ((UserTypeID == 6) || (isAssignSSC == 'true' && ((UserTypeID == 4 && IsSSCReseller == 'true') || (UserTypeID == 8 && IsSubContractor == 'false' && IsSSCReseller == 'true')))) ? table.columns(col + ":name").visible(true) : table.columns(col + ":name").visible(false);
                }
                else
                    table.columns(col + ":name").visible(true);
            });

            // Assign Width - In Percentage
            var arrColumnsWidth = [];
            arrColumnsWidth.push(2);
            //arrColumnsWidth = listColumnsWidth.split(',');
            $.each(listColumnsWidth.split(','), function (i, data) {
                arrColumnsWidth.push(data);
            });
            arrColumnsWidth.push(2);
            $('#trHeadersDynamic th').each(function (i, item) {
                $(item).css('width', arrColumnsWidth[i] + '%');
            });

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