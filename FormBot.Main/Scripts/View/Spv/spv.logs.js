/*Document Ready function*/
$(document).ready(function () {
    $("#drpReseller").select2();
    //bind data
    FillDropDown('drpReseller', urlGetAllReseller, 0, false, null, "");

    //control init
    $('#txtVpsService').SumoSelect({ csvDispCount: 3, search: true, searchText: 'Search..', selectAll: true, okCancelInMulti: true });
    $('#txtFromDateOrTime').datetimepicker({
        format: 'MM/DD/YYYY HH:mm'
    });
    $("#txtToDateOrTime").datetimepicker({
        format: 'MM/DD/YYYY HH:mm'
    });
    $(".kendo-dropdown-ui").kendoDropDownList();

    //bind data
    BindVspService();
    setTimeout(() => {
        BindSolarCompany($("#drpReseller").val());
        BindSpvLogs();
    }, 200);

    //events
    $("#btnSearch").click(function () {
        $('#dtSpvLogs').kendoGrid('destroy').empty();
        BindSpvLogs();
    });
    $("#drpReseller").change(function () {
        if ($("#drpReseller").val() > 0) {
            BindSolarCompany($("#drpReseller").val());
        }
    });
    $('#collapse').click(function (e) {
        var grid = $("#dtSpvLogs").data("kendoGrid");
        $("#dtSpvLogs .k-master-row").each(function (index) {
            grid.collapseRow(this);
        });
    });
    $("#txtFromDateOrTime").on("dp.change", function (e) {
        $('#txtToDateOrTime').data("DateTimePicker").minDate(e.date);
    });
    $("#txtToDateOrTime").on("dp.change", function (e) {
        $('#txtFromDateOrTime').data("DateTimePicker").maxDate(e.date);
    });
    $("#btnClear").click(function () {
        ClearFilters();
    });
});


$("#btnCsvGenerate").click(function () {
    var fromReqDate = new moment($("#txtFromDateOrTime").val(), 'MM/DD/YYYY HH:mm').toDate();
    var toReqDate = new moment($("#txtToDateOrTime").val(), 'MM/DD/YYYY HH:mm').toDate();
    var diffOfDateInTime = toReqDate.getTime() - fromReqDate.getTime();
    var diffOfDateInDays = diffOfDateInTime / (1000 * 3600 * 24);
    if (diffOfDateInDays > 7 || $("#txtFromDateOrTime").val() == "" || $("#txtToDateOrTime").val()=="") {
        $("#errorMsgRegion").removeClass("alert-success");
        $("#errorMsgRegion").addClass("alert-danger");
        $("#errorMsgRegion").html(closeButton + "Please select date range between 7 days to export csv log.");
        $("#errorMsgRegion").show();
        return false;
    }
    else {
        var ServiceAdministrator = ($('.report-search-box').val() == null ? 'All' : $('.report-search-box').val().join());
        var ResellerId = $("#drpReseller").val();
        var SolarCompanyId = $("#hdnsolarCompanyid").val() == '' || $("#hdnsolarCompanyid").val() == 0 ? -1 : parseInt($("#hdnsolarCompanyid").val());
        var JobReferenceOrId = $("#txtReferenceNumberOrJobId").val();
        var PVDSWHcode = $("#txtPvdOrSwhCode").val();
        var SPVMethod = parseInt($("#drpSpvMethod").val());
        var VerificationStatus = parseInt($("#drpSpvRequestStatus").val());
        var ResponseCode = $("#txtResponseCode").val();
        var Manufacturer = $("#txtManufacturer").val();
        var ModelNumber = $("#txtModelNumber").val();
        var SerialNumer = $("#txtSerialNumber").val();
        var FromRequestDate = $("#txtFromDateOrTime").val() == '' ? null : ($("#txtFromDateOrTime").val());
        var ToRequestDate = $("#txtToDateOrTime").val() == '' ? null : ($("#txtToDateOrTime").val());
        var WithOutSCA = false;
        var solarCompanyName = $("#txtSolarCompany").val();
        var resellerName = $("#drpReseller option:selected").text();
    window.location.href = urlExportCSVForSPVLogs + '?ServiceAdministrator=' + ServiceAdministrator + '&ResellerId=' + ResellerId + '&SolarCompanyId=' + SolarCompanyId
        + '&JobReferenceOrId=' + JobReferenceOrId + '&PVDSWHcode=' + PVDSWHcode + '&SPVMethod=' + SPVMethod + '&VerificationStatus=' + VerificationStatus
        + '&ResponseCode=' + ResponseCode + '&Manufacturer=' + Manufacturer + '&ModelNumber=' + ModelNumber + '&SerialNumer=' + SerialNumer + '&FromRequestDate=' + FromRequestDate
        + '&ToRequestDate=' + ToRequestDate + '&IsWithoutSCA=' + WithOutSCA + '&SolarCompanyName=' + solarCompanyName + '&ResellerName=' + resellerName;

    }
   
});
$("#btnCsvGenerateWithOutSCA").click(function () {
    var fromReqDate = new moment($("#txtFromDateOrTime").val(), 'MM/DD/YYYY HH:mm').toDate();
    var toReqDate = new moment($("#txtToDateOrTime").val(), 'MM/DD/YYYY HH:mm').toDate();
    var diffOfDateInTime = toReqDate.getTime() - fromReqDate.getTime();
    var diffOfDateInDays = diffOfDateInTime / (1000 * 3600 * 24);
    if (diffOfDateInDays > 7 || $("#txtFromDateOrTime").val() == "" || $("#txtToDateOrTime").val()=="") {
        $("#errorMsgRegion").removeClass("alert-success");
        $("#errorMsgRegion").addClass("alert-danger");
        $("#errorMsgRegion").html(closeButton + "Please select date range between 7 days to export csv log.");
        $("#errorMsgRegion").show();
        return false;
    }
    else {
        var ServiceAdministrator = ($('.report-search-box').val() == null ? 'All' : $('.report-search-box').val().join());
        var ResellerId = $("#drpReseller").val();
        var SolarCompanyId = $("#hdnsolarCompanyid").val() == '' || $("#hdnsolarCompanyid").val() == 0 ? -1 : parseInt($("#hdnsolarCompanyid").val());
        var JobReferenceOrId = $("#txtReferenceNumberOrJobId").val();
        var PVDSWHcode = $("#txtPvdOrSwhCode").val();
        var SPVMethod = parseInt($("#drpSpvMethod").val());
        var VerificationStatus = parseInt($("#drpSpvRequestStatus").val());
        var ResponseCode = $("#txtResponseCode").val();
        var Manufacturer = $("#txtManufacturer").val();
        var ModelNumber = $("#txtModelNumber").val();
        var SerialNumer = $("#txtSerialNumber").val();
        var FromRequestDate = $("#txtFromDateOrTime").val() == '' ? null : ($("#txtFromDateOrTime").val());
        var ToRequestDate = $("#txtToDateOrTime").val() == '' ? null : ($("#txtToDateOrTime").val());
        var WithOutSCA = true;
        var solarCompanyName = $("#txtSolarCompany").val();
        var resellerName = $("#drpReseller option:selected").text();
        window.location.href = urlExportCSVForSPVLogs + '?ServiceAdministrator=' + ServiceAdministrator + '&ResellerId=' + ResellerId + '&SolarCompanyId=' + SolarCompanyId
            + '&JobReferenceOrId=' + JobReferenceOrId + '&PVDSWHcode=' + PVDSWHcode + '&SPVMethod=' + SPVMethod + '&VerificationStatus=' + VerificationStatus
            + '&ResponseCode=' + ResponseCode + '&Manufacturer=' + Manufacturer + '&ModelNumber=' + ModelNumber + '&SerialNumer=' + SerialNumer + '&FromRequestDate=' + FromRequestDate
            + '&ToRequestDate=' + ToRequestDate + '&IsWithoutSCA=' + WithOutSCA + '&SolarCompanyName=' + solarCompanyName + '&ResellerName=' + resellerName;

    }
   
});
function GetDates(date) {
    if (date != '') {
        var tickStartDate = ConvertDateToTick(date, dateFormat);
        return moment(tickStartDate).format("YYYY-MM-DD");
    }
    else {
        return '';
    }
}
/*Page specific functions*/
function BindVspService() {
    $.ajax({
        url: urlGetEndPoints,
        type: "POST",
        dataType: "json",
        processData: false,
        cache: false,
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $('#txtVpsService')[0].sumo.add(data[i], data[i]);
                }
            }
            $('.SumoSelect .SelectBox').show();
        },
        error: function (e) {
            console.log(e);
        }
    });
    return false;
}
function BindSolarCompany(resellerID) {
    $("#txtSolarCompany").val("");
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
            });
            $('#txtSolarCompany').autocomplete({
                minLength: 0,
                source: function (request, response) {
                    var data = [];
                    $("#hdnsolarCompanyid").val(0);
                    $.each(solarCompanyList, function (key, value) {
                        if (value.text.toLowerCase().indexOf($("#txtSolarCompany").val().toLowerCase()) > -1) {
                            data.push({ Title: value.text, id: value.value });
                        }
                    });

                    response($.map(data, function (item) {
                        return { label: item.Title, value: item.Title, id: item.id };
                    }));
                },
                select: function (event, ui) {
                    $("#hdnsolarCompanyid").val(ui.item.id); // save selected id to hidden input
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
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}
function BindSpvLogs() {
    var kendoCols = [];
    kendoCols.push({
        title: 'Serial Number',
        field: 'SerialNumber',
        template: function (data) {
            return "<span>" + data.SerialNumber + "</span>";
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    kendoCols.push({
        title: 'Job ID',
        field: 'JobId',
        template: function (data) {
            return "<span><a href='/Job/Index?id=" + data.EncryptedJobId + "'>" + data.JobId + "</a></span>";
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    kendoCols.push({
        title: 'SPV Method',
        field: 'SPVMethod',
        template: function (data) {
            return "<span>" + (data.SPVMethod == 1 ? "Product Verification" : "Installation Verification") + "</span>";
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    kendoCols.push({
        title: 'Verification Status',
        field: 'VerificationStatus',
        template: function (data) {
            return "<span>" + data.VerificationStatus == 0 ? "in progress" : (data.VerificationStatus == 1 ? "<span style='color: #006400;'>Success</span>" : "<span style='color: Red;'>Fail</span>") + "</span>";
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    kendoCols.push({
        title: 'RCode',
        field: 'ResponseCode',
        template: function (data) {
            return "<span>" + (data.ResponseCode == null ? "" : data.ResponseCode) + "</span>";
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    kendoCols.push({
        title: 'Date Time',
        field: 'ResponseTime',
        template: function (data) {
            return (data.ResponseTime != null ? kendo.toString(kendo.parseDate(data.ResponseTime, 'yyyy-MM-dd hh:mm tt'), 'dd/MM/yyyy HH:mm:ss') : '')
        },
        sortable: true,
        width: 50,
        filterable: false,
        media: "(min-width: 100px)"
    });
    $("#dtSpvLogs").kendoGrid({
        columns: kendoCols,
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: urlGetSpvLogs,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.ServiceAdministrator = ($('.report-search-box').val() == null ? 'All' : $('.report-search-box').val().join());
                    options.ResellerId = $("#drpReseller").val();
                    options.SolarCompanyId = $("#hdnsolarCompanyid").val() == '' || $("#hdnsolarCompanyid").val() == 0 ? -1 : parseInt($("#hdnsolarCompanyid").val());
                    options.JobReferenceOrId = $("#txtReferenceNumberOrJobId").val();
                    options.PVDSWHcode = $("#txtPvdOrSwhCode").val();
                    options.SPVMethod = parseInt($("#drpSpvMethod").val());
                    options.VerificationStatus = parseInt($("#drpSpvRequestStatus").val());
                    options.ResponseCode = $("#txtResponseCode").val();
                    options.Manufacturer = $("#txtManufacturer").val();
                    options.ModelNumber = $("#txtModelNumber").val();
                    options.SerialNumer = $("#txtSerialNumber").val();
                    options.FromRequestDate = $("#txtFromDateOrTime").val() == '' ? null : new Date($("#txtFromDateOrTime").val());
                    options.ToRequestDate = $("#txtToDateOrTime").val() == '' ? null : new Date($("#txtToDateOrTime").val());
                    return JSON.stringify(options);
                }
            },
            schema: {
                data: "data",
                total: "total",
            },
            pageSize: "10",
            serverPaging: true,
            serverSorting: true,
            requestStart: function (e) {
                setTimeout(function (e) {
                    $(".k-loading-image").hide();
                });
            }
        },
        dataBound: function () {
            for (var i = 0; i < this.columns.length; i++) {
                this.autoFitColumn(i);
            }
        },
        detailInit: detailInit,
        groupable: false,
        sortable: true,
        filterable: {
            extra: false,
        },
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        resizable: false,
        scrollable: true,
        reorderable: false,
        filterMenuInit: function (e) {
            $(".loading-image").hide();
        },
    });
}
function detailInit(e) {
    $.ajax({
        url: urlGetSpvLogDetails,
        dataType: 'json',
        method: "GET",
        data: { "SPVLogId": parseInt(e.data.SPVLogId) },
        success: function (data) {
            data = "<div><a href='/job/index?id=" + data.EncryptedJobId + "'><strong>Ref Number:</strong> " + data.Refnumber + "</a> <strong>Job ID:</strong> " + data.JobID + " </div>"
                + "<div><strong>Request sent:</strong> " + ConvertToDateWithFormat(data.RequestTime, "dd/mm/yyyy hh:mm:ss") + "</div>"
                + "<div><strong>Job Details:</strong> </div>"
                + "<div>" + data.JobAddress + "</div>"
                + (data.SPVMethod == 2 ? "<div><strong>Installer:</strong> " + data.InstallerName + "</div>" : "")
                + "<div><strong>Manufacturer:</strong> " + data.Manufacturer + "</div>"
                + "<div><strong>Model Number:</strong> " + data.modelNumber + "</div>"
                + (data.SPVMethod == 2 ? "<div><strong>No.ofPanels:</strong> " + data.NoOfPanel + "</div>" : "")
                + "<div><strong>Serial Number:</strong> <span class='text-danger'>" + data.SerialNumber + "</span></div>"
                + "<div><strong>Response Message:</strong></div>"
                + "<div>" + (data.ResponseMessage == null ? "" : data.ResponseMessage) + "</div>"
                + "<div><strong>Reseller:</strong> " + data.ResellerName + " | <strong>Solar Company:</strong> <a href='/User/ViewDetail/" + data.EncryptedSolarCompanyId + "'>" + data.CompanyName + "</a></div>"
                + "<div><strong>Service Administrator:</strong> " + data.ServiceAdministrator + "</div>";
            $(data).appendTo(e.detailCell);
        }
    });
}
function ClearFilters() {
    $('#txtVpsService')[0].sumo.unSelectAll();
    $("#txtSolarCompany").val('');
    $("#hdnsolarCompanyid").val(0);
    $("#txtReferenceNumberOrJobId").val('');
    $("#drpSpvMethod").val(-1);
    $("#drpSpvRequestStatus").val(-1);
    $("#txtResponseCode").val('');
    $("#txtSerialNumber").val('');
    $("#txtManufacturer").val('');
    $("#txtModelNumber").val('');
    $("#txtFromDateOrTime").val('');
    $("#txtToDateOrTime").val('');
    $("#btnSearch").click();
}
