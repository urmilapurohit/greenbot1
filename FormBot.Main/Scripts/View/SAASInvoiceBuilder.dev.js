var kendoCols = [];
var kendoColsNew = [];
var ViewPageId = 3;
var lstJobID = [];
var strlstJobID = '';
var GlblInvoiceID = '';
var isAlertSelectOne = false;
var LastProcesssedId = '';
$(document).ready(function () {
    SAASInvoiceBuilderKendoIntialiize();
    setTimeout(function () {
        drawSAASInvoiceBuilderKendoGrid();
    }, 2000);
    BindSAASUser();
    $(document).on("change", "#chkAll", function () {
        $("#datatable").find("[type='checkbox']").prop("checked", this.checked)
        chkCount = this.checked ? $('#datatable tbody').find('tr').length : 0;
    });
    FillDropDownUser(2);
});

function SAASInvoiceBuilderKendoIntialiize() {
    kendoColsNew = [];
    kendoColsNew.push({
        title: 'Chkbox',
        field: 'Id',
        encoded: false,
        template: function (data) {
            return '<input type="checkbox" class="k-checkbox" chkType="' + 'HeaderCheckBox' + '" strJobID = "' + data.strJobID + '" strProcessedJobID = "' + data.strProcessedJobID + '" BillingPeriod = "' + data.BillingPeriod + '" CreatedDate = "' + data.CreatedDate + '" GlobalTermID = "' + data.Id + '" InvoiceAmount = "' + data.InvoiceAmount + '" InvoiceID = "' + data.RowNum + '" InvoicedDate = "' + data.InvoicedDate + '" IsGlobalGST = "' + data.IsGlobalGST + '" QTY = "' + data.QTY + '" Rate = "' + data.Rate + '" SettelmentTerm = "' + data.SettelmentTerm + '" ResellerID="' + data.ResellerID + '" id="chk_' + data.RowNum + '" onchange="CheckChangeParent(\'' + data.strJobID + '\',\'' + data.RowNum + '\')"><label class="k-checkbox-label" for="chk_' + data.RowNum + '">';
        },
        filterable: false,
        sortable: false,
        headerTemplate: '<input type="checkbox" id="chkAll" name="select_all"  class="k-checkbox"><label class="k-checkbox-label" for="chkAll"></label>',
        width: 5,
        stickyPosition: 0,
        reorderable: false,
        resizable: false
    });
    kendoColsNew.push({
        title: "Saas User",
        field: "ResellerName",
        encoded: false,
        headerAttributes: {
            "data-field": "ResellerName",
            "data-columnid": "ResellerName",
            "data-order": 4
        },
        headerTemplate: '<span>Saas User</span>',
        width: 25,
        filterable: {
            operators: {
                string: {
                    "contains": "Contains",
                }
            },
            extra: false
        },
        sortable: true,
    });
    kendoColsNew.push({
        title: "Settelment Term",
        field: "SettelmentTerm",
        encoded: false,
        headerAttributes: {
            "data-field": "SettelmentTerm",
            "data-columnid": "SettelmentTerm",
            "data-order": 4
        },
        headerTemplate: '<span>Settelment Term</span>',
        width: 25,
        template: "#=Keno_SettelmentTerm(data) #",
        filterable: {
            operators: {
                string: {
                    "contains": "Contains",
                }
            },
            extra: false,
            ui: filterStatus
        },
        template: function (data) {
            if (data.SettelmentTerm == 'PerJob-S') {
                return '<span >P/Job</span>';
            }
            else if (data.SettelmentTerm == 'PerSTC-S') {
                return '<span>P/STC</span>';
            }
            else if (data.SettelmentTerm == 'PerUser-S') {
                return '<span>P/User</span>';
            }
        },
        sortable: true,
    });
    kendoColsNew.push({
        title: "Rate",
        field: "Rate",
        encoded: false,
        headerAttributes: {
            "data-field": "Rate",
            "data-columnid": "Rate",
            "data-order": 5
        },
        headerTemplate: '<span>Rate</span>',
        width: 25,
        template: "#=Keno_RateAndGST(data) #",
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to"
                }
            },
            extra: false
        },
        sortable: true,
    });
    kendoColsNew.push({
        title: "QTY",
        field: "QTY",
        encoded: false,
        headerAttributes: {
            "data-field": "QTY",
            "data-columnid": "QTY",
            "data-order": 6
        },
        headerTemplate: '<span>QTY</span>',
        width: 25,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                }
            },
            extra: false
        },
        sortable: true,
    });
    kendoColsNew.push({
        title: "Total Amount",
        field: "InvoiceAmount",
        encoded: false,
        headerAttributes: {
            "data-field": "InvoiceAmount",
            "data-columnid": "InvoiceAmount",
            "data-order": 6
        },
        headerTemplate: '<span>Total Amount</span>',
        width: 25,
        template: "#=Keno_InvoiceAmount(data) #",
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                }
            },
            extra: false
        },
        sortable: true,
    });
    kendoColsNew.push({
        title: "Billing Period",
        field: "BillingPeriod",
        encoded: false,
        headerAttributes: {
            "data-field": "BillingPeriod",
            "data-columnid": "BillingPeriod",
            "data-order": 4
        },
        headerTemplate: '<span>Billing Period</span>',
        width: 25,
        filterable: {
            operators: {
                string: {
                    "contains": "Contains",
                }
            },
            extra: false
        },
        sortable: true,
    });
}


var filterStatus = function (element) {
    var data = [
        { Text: "PerJob-S", Value: 'PerJob-S' },
        { Text: "PerSTC-S", Value: 'PerSTC-S' },
        { Text: "PerUser-S", Value: 'PerUser-S' }
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
};

function drawSAASInvoiceBuilderKendoGrid(filter) {
    if ($('#datatable').hasClass("k-grid")) {
        $('#datatable').kendoGrid('destroy').empty();
    }
    kendo.ui.FilterMenu.fn.options.operators.date = {
        gte: "Begin Date",
        lte: "End Date"
    };
    $("#datatable").kendoGrid({
        columns: kendoColsNew,
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: urlSAASInvoiceBuilderIndexNew,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.UserTypeID = $("#UserTypeID").val();
                    options.SAASUserId = $("#hdnResellerIdForFilter").val();
                    options.UserRole = $('#ddlUserRoles option:selected').val() == 0 ? "" : $('#ddlUserRoles option:selected').val();
                    options.IsIsArchive = $("#IsIsArchive").is(':checked');

                    if (options.filter != undefined) {
                        for (var i = 0; i < options.filter.filters.length; i++) {
                            if (options.filter.filters[i].field == 'IsSAASInvoiced') {
                                if (options.filter.filters[i].value.toLowerCase() == 'yes') {
                                    options.filter.filters[i].value = '1';
                                }
                                else {
                                    options.filter.filters[i].value = '0';
                                }
                            }
                        }
                    }
                    return JSON.stringify(options);
                }
            },
            filter: filter,
            schema: {
                data: "data", // records are returned in the "data" field of the response
                total: "total",
                model: {
                    fields: {
                        CreatedDate: { type: "date" },
                        InvoicedDate: { type: "date" }
                    }
                }
            },
            pageSize: GridConfig[0].PageSize,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            requestStart: function (e) {
                setTimeout(function (e) {
                    $(".k-loading-image").hide();
                })
            },
            requestEnd: function (e) {
            },
            //resizable: true,
            //scrollable: true,
            //reorderable: true,
            filterMenuInit: function (e) {
                /* $(".loading-image").hide();
                 if (e.field.includes("Date")) {
                     this.thead.find("[data-field='" + e.field + "'] .k-grid-filter").click(function (e) {
                         setOperators();
                     });
                     var setOperators = function () {
                         var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                         firstValueDropDown.value("gte");
                         firstValueDropDown.trigger("change");
                         firstValueDropDown.enable(false)
 
                         var logicDropDown = e.container.find("select:eq(1)").data("kendoDropDownList");
                         logicDropDown.wrapper.hide()
 
                         var secondValueDropDown = e.container.find("select:eq(2)").data("kendoDropDownList");
                         secondValueDropDown.enable(false);
                         secondValueDropDown.value("lte");
                         secondValueDropDown.trigger("change");
                     };
                     setOperators();
 
                 } else if (e.container.find('select').val() == "contains") {
                     var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                     firstValueDropDown.enable(false)
                 } else if (e.container.find("select:eq(0)").data("kendoDropDownList") != null && e.container.find("select:eq(0)").data("kendoDropDownList").dataSource.options.data.length == 1) {
                     var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                     firstValueDropDown.enable(false)
                 } else if (e.field == "TotalAmount" || e.field == "AmountPaid" || e.field == "Price") {
                     var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                     firstValueDropDown.value("eq");
                     firstValueDropDown.trigger("change");
                     firstValueDropDown.enable(false)
                     e.container.find('input').attr("onkeypress", "return isNumber(event);")
                 }*/
            },
            dataBound: function (e) {
                $("#chkAll").change();
                $(".k-pager-sizes").find('select').on("change", function () {
                    GridConfig[0].PageSize = $(this).val();
                    GridConfig[0].IsKendoGrid = true;
                    GridConfig[0].ViewPageId = 3;
                })
            }
        },
        groupable: false,
        sortable: true,
        filterable: {
            extra: false,
        },
        //columnResize: function (e) {
        //    SaveUserWiseColumnDetails();
        //},
        columnReorder: onReorder,
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        resizable: true,
        scrollable: true,
        reorderable: true,
        filterMenuInit: function (e) {
            /*$(".loading-image").hide();
            if (e.field.includes("Date")) {
                this.thead.find("[data-field='" + e.field + "'] .k-grid-filter").click(function (e) {
                    setOperators();
                });
                var setOperators = function () {
                    var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                    firstValueDropDown.value("gte");
                    firstValueDropDown.trigger("change");
                    firstValueDropDown.enable(false)

                    var logicDropDown = e.container.find("select:eq(1)").data("kendoDropDownList");
                    logicDropDown.wrapper.hide()

                    var secondValueDropDown = e.container.find("select:eq(2)").data("kendoDropDownList");
                    secondValueDropDown.enable(false)
                    secondValueDropDown.value("lte");
                    secondValueDropDown.trigger("change");
                };
                setOperators();

            } else if (e.container.find('select').val() == "contains") {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.enable(false)
            } else if (e.container.find("select:eq(0)").data("kendoDropDownList") != null && e.container.find("select:eq(0)").data("kendoDropDownList").dataSource.options.data.length == 1) {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.enable(false)
            } else if (e.field == "Price" || e.field == "Quantiy") {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.value("eq");
                firstValueDropDown.trigger("change");
                firstValueDropDown.enable(false)
                e.container.find('input').attr("onkeypress", "return isNumber(event);")
            }*/
        },
        dataBound: function (e) {
            $("#chkAll").change();
            $(".k-pager-sizes").find('select').on("change", function () {
                SetPageSizeForGrid($(this).val(), true, ViewPageId)
                GridConfig[0].PageSize = $(this).val();
                GridConfig[0].IsKendoGrid = true;
                GridConfig[0].ViewPageId = parseInt(ViewPageId);
            });
        },
        detailInit: detailInit
    });

    $("#datatable").data("kendoGrid").thead.kendoTooltip({
        filter: "th:not(':first')",
        content: function (e) {
            var target = e.target;
            return $(target).text();
        }
    });
}

function SaveUserWiseColumnDetails() {
    $("#loading-image").css("display", "");
    var arrColumns = [];
    var orderNo = 0;
    var columnsDetails = $("#datatable").data("kendoGrid").columns;
    $('#datatable th').not(":first").each(function (i, item) {
        orderNo = orderNo + 1;
        var SavePageSize = $("#datatable").data("kendoGrid").dataSource.pageSize();
        arrColumns.push({ Width: columnsDetails[i].width, ColumnID: $(item).attr("data-columnid"), OrderNumber: orderNo, PageSize: SavePageSize });
    });
    $("#loading-image").css("display", "none");
}

function BindSAASUser() {
    $.ajax({
        url: urlSAASUsersList,
        dataType: "json",
        data: {},
        success: function (SAASUsers) {
            $('#SAASUserId').autocomplete({
                minLength: 1,
                source: function (request, response) {
                    var data = [];
                    $.each(SAASUsers, function (key, value) {
                        if (value.ResellerName.toLowerCase().indexOf($("#SAASUserId").val().toLowerCase()) > -1) {
                            data.push({ Title: value.ResellerName, id: value.ResellerID, ResellerIdForFilter: value.ResellerIdForFilter });
                        }
                    });

                    response($.map(data, function (item) {
                        return { label: item.Title, value: item.Title, id: item.id, ResellerIdForFilter: item.ResellerIdForFilter };
                    }))
                },
                select: function (event, ui) {
                    $("#hdnresellerId").val(ui.item.id);
                    $("#hdnResellerIdForFilter").val(ui.item.ResellerIdForFilter);
                }
            }).on('focus', function () { $(this).keydown(); });
        }
    });

    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
}

function onReorder(e) {
    if (typeof e.column.reorderable != "undefined" && !e.column.reorderable) {
        var grid = $('#datatable').data('kendoGrid');
        setTimeout(function () {
            grid.reorderColumn(e.oldIndex, grid.columns[e.newIndex]);
        }, 0);
    }
}

function detailInit(e) {
    GlblInvoiceID = e.data.RowNum;
    $("<div/>").appendTo(e.detailCell).kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: urlSAASInvoiceBuilderDetailNew,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.strJobID = e.data.strJobID;
                    return JSON.stringify(options);
                }
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            schema: {
                data: "data", // records are returned in the "data" field of the response
                total: "total",
                model: {
                    fields: {
                    }
                }
            },
        },
        scrollable: false,
        sortable: true,
        pageable: false,
        dataBound: childGridDataBound,
        columns: [
            {
                template: function (data) {
                    if (data.JobId != null) {
                        return '<input type="checkbox" class="k-checkbox" JobID = "' + data.JobId + '" id="chk_' + data.JobId + "_" + GlblInvoiceID + '" InvoiceId="' + GlblInvoiceID + '"  IsSent="' + data.IsSAASInvoiced + '" HeaderCheckboxId = "' + "chk_" + GlblInvoiceID + '" onchange="CheckChangeChild(\'' + data.JobId + "_" + GlblInvoiceID + '\')"><label class="k-checkbox-label" for="chk_' + data.JobId + "_" + GlblInvoiceID + '" JobID = "' + data.JobId + '" value = "' + data.JobId + "_" + GlblInvoiceID + '">';
                    }
                },
                width: "50px"
            },
            { field: "JobId", title: "Job Id", width: "110px" },
            { field: "RefNumber", title: "Ref. Number", width: "110px" },
            { field: "OwnerName", title: "Owner Name", width: "110px" },
            { field: "OwnerAddress", title: "Owner Address", width: "300px" },
            { field: "STCPrice", title: "STC Amount", width: "100px", template: function (data) { return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.STCPrice).toFixed(2) + '</lable>'; } },
            { field: "Status", title: "CER Status", width: "200px" },
            {
                field: "RECBulkUploadTime",
                title: "REC Creation Date",
                width: "200px",
                template: function (data) {
                    if (data.RECBulkUploadTime != null && data.RECBulkUploadTime != "null") {
                        return ToDate(new Date(moment(data.RECBulkUploadTime).format("MM/DD/YYYY")));
                    }
                    else {
                        return "";
                    }
                }
            },
            { field: "STCPVDCode", title: "PVD Code", width: "100px" },
            {
                title: "Is Sent?",
                template: function (data) {
                    if (data.IsSAASInvoiced) {
                        return '<span style="font-weight: bold;color: Green;">Yes</span>';
                    }
                    else {
                        return '<span style="font-weight: bold;color: Red;">No</span>';
                    }
                },
                width: "100px"
            },
            {
                title: "Action",
                template: function (data) {
                    imgassign = 'text-decoration:none;';
                    var returnHTML = '';
                    if (!data.IsSAASInvoiced) {
                        returnHTML += '<a href="javascript:void(0);" onclick="deleteSAASInvoiceFomBuilder(' + data.JobId + ')" id="" class="btn default icon-btn btn-sm remove-ic" title="Delete invoice"></a>'
                    }
                    else {
                        returnHTML += '<a href="javascript:void(0);" onclick="restoreSAASInvoiceFomBuilder(' + data.JobId + ')" id="" class="btn default btn-sm sprite-img reset_ic" title="Restore invoice"></a>'
                    }
                    return returnHTML;
                },
                width: "100px"
            },
        ]
    });
}

function childGridDataBound(e) {
    var grid = $("#datatable").data("kendoGrid");
    var dataview = e.sender.dataSource._view.length;
    for (var i = 0; i < dataview; i++) {
        var currentUid = e.sender.dataSource._view[i].uid
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        $(currenRow).css('background-color', '#fff1e0');
    }
}

function dirtyField(data, fieldName) {
    var hasClass = $("[data-uid=" + data.JobID + "]").find(".k-dirty-cell").length < 1;
    if (data.dirty && data.dirtyFields[fieldName] && hasClass) {
        return "<span class='k-dirty'></span>"
    }
    else {
        return "";
    }
}

var Kendo_CreatedDate = function (data) {
    return ToDate(data.CreatedDate);
};

function ToDate(data) {
    if (data != null) {
        return data.getFullYear() + '-' + ("0" + (data.getMonth() + 1)).slice(-2) + '-' + ("0" + data.getDate()).slice(-2);
    }
    else {
        return '';
    }
}

var Keno_RateAndGST = function (data) {
    if (data != null) {
        if (data.IsGlobalGST) {
            return '$' + data.Rate + '+GST';
        }
        else {
            return '$' + data.Rate;
        }
    }
};

var Keno_InvoiceAmount = function (data) {
    if (data != null) {
        if (data.InvoiceAmount) {
            return '$' + data.InvoiceAmount;
        }
    }
};

var Keno_SettelmentTerm = function (data) {
    if (data != null) {
        if (data.SettelmentTerm.toLowerCase().includes('user')) {
            return 'P/User';
        }
        else if (data.SettelmentTerm.toLowerCase().includes('job')) {
            return 'P/Job';
        }
        else {
            return 'P/STC'
        }
    }
};

function SendToSAASInvoiceNew() {
    var Temp = 1;
    var Processed = null;
    var isAlert = false;
    //var isAlertSelectOne = false;
    var isSaved = false;
    var result = '';
    selectedRows = [];
    var alreadySentInvoice = true;
    var xeroInvoiceIds = [];
    var objSAASInvoiceBuilder = {};
    var DatatableCheckboxChecked = $('#datatable tbody input[type="checkbox"]:Checked').length;
    $('#datatable tbody input[type="checkbox"]:Checked').each(function () {
        if ($(this).prop('checked')) {
            //isAlertSelectOne = false;
            if (isSaved == false) {  /* To ajax call only onr time bcz if it is header checkbox then all job id is already sent , as there is foreach loop it will also loop through its child checkbox checked so to restrict it */
                if (Processed == 'null') {
                    Processed = null;
                }
                if ($(this).attr('chkType') == 'HeaderCheckBox') {
                    if (Temp == 1) {
                        if ($(this).attr('strprocessedjobid') == 'null') {
                            Processed = null;
                            DatatableCheckboxChecked = Temp;
                        }
                        else {
                            Processed = $(this).attr('strprocessedjobid');
                        }
                        result = confirm("Are you sure you want to send selected SAAS Invoice(s) ?");
                    }
                    else {
                        if ($(this).attr('strprocessedjobid') == 'null') {
                            Processed = null;
                        }
                        else {
                            Processed = Processed;
                        }
                    }
                    debugger;
                    isSaved = true;
                    selectedRows.push($(this).attr('strjobid'));
                    objSAASInvoiceBuilder = {
                        InvoiceID: $(this).attr('invoiceid'),
                        SettelmentTerm: $(this).attr('settelmentterm'),
                        GlobalTermId: $(this).attr('globaltermid'),
                        IsGlobalGST: $(this).attr('isglobalgst'),
                        strJobID: $(this).attr('strjobid'),
                        CreatedDate: $(this).attr('createddate'),
                        Rate: $(this).attr('rate'),
                        QTY: $(this).attr('qty'),
                        InvoiceAmount: $(this).attr('invoiceamount'),
                        BillingPeriod: $(this).attr('billingperiod'),
                        BillingMonth: $(this).attr('billingperiod').split(' ')[0],
                        BillingYear: $(this).attr('billingperiod').split(' ')[1],
                        ResellerID: $(this).attr('resellerid'),
                        strAllJobIds: Processed
                    };
                    if ($('#chkAll').is(":checked") == false && DatatableCheckboxChecked == 1) {
                        Temp = DatatableCheckboxChecked;
                    }
                    else {
                        isSaved = false;
                    }
                }
                else {
                    debugger;
                    if ($(this).attr('issent') == 'true') {
                        alert("Some invoices are alredy sent, Select unsent invoices only");
                    }
                    else {
                        if (Temp == 1) {
                            if ($("#chk_" + $(this).attr('invoiceid')).attr('strprocessedjobid') != 'null') {
                                Processed = $("#chk_" + $(this).attr('invoiceid')).attr('strprocessedjobid');
                            }
                            else {
                                //Processed = Processed;
                            }
                            result = confirm("Are you sure you want to send selected SAAS Invoice(s) ?");
                        }
                        selectedRows.push('1');
                        if ($("#chk_" + LastProcesssedId).attr('invoiceid') != undefined) {
                            if ($("#chk_" + LastProcesssedId).attr('invoiceid') != $(this).attr('invoiceid')) {
                                Processed = null;
                            }
                        }
                        else {
                            Processed = Processed;
                        }
                        debugger;
                        objSAASInvoiceBuilder = {
                            //InvoiceID: $("#chk_" + GlblInvoiceID).attr('invoiceid'),
                            InvoiceID: $(this).attr('invoiceid'),
                            SettelmentTerm: $("#chk_" + $(this).attr('invoiceid')).attr('settelmentterm'),
                            GlobalTermId: $("#chk_" + $(this).attr('invoiceid')).attr('globaltermid'),
                            IsGlobalGST: $("#chk_" + $(this).attr('invoiceid')).attr('isglobalgst'),
                            //strJobID: strlstJobID,
                            strJobID: $(this).attr('jobid'),
                            CreatedDate: $("#chk_" + $(this).attr('invoiceid')).attr('createddate'),
                            Rate: $("#chk_" + $(this).attr('invoiceid')).attr('rate'),
                            //QTY: $("#chk_" + $(this).attr('invoiceid')).attr('qty'),
                            QTY: 1,
                            InvoiceAmount: $("#chk_" + $(this).attr('invoiceid')).attr('invoiceamount'),
                            BillingPeriod: $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod'),
                            BillingMonth: $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod').split(' ')[0],
                            BillingYear: $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod').split(' ')[1],
                            ResellerID: $("#chk_" + $(this).attr('invoiceid')).attr('resellerid'),
                            strAllJobIds: Processed
                        };
                    }
                }
            }
        }
        if (selectedRows != null && selectedRows.length > 0) {
            //if (isAlert == false) {
            //    result = confirm("Are you sure you want to send selected SAAS Invoice(s) ?");
            //    isAlert = true;
            //}
            if (result) {
                selectedRows = [];
                $.ajax({
                    type: 'POST',
                    url: urlSendToSAASInvoices,
                    dataType: 'json',
                    async: false,
                    //contentType: 'application/json; charset=utf-8',
                    //data: JSON.stringify({ invoiceIds: JSON.stringify(selectedRows) }),
                    data: objSAASInvoiceBuilder,
                    success: function (data) {
                        debugger;
                        if (Processed == null) {
                            if ($('#chkAll').is(":checked") == false) {
                                Processed = objSAASInvoiceBuilder.strJobID;
                                LastProcesssedId = objSAASInvoiceBuilder.strJobID;
                            }
                            //Processed = objSAASInvoiceBuilder.strJobID;
                            //LastProcesssedId = objSAASInvoiceBuilder.strJobID;
                        }
                        else {
                            Processed = Processed + ',' + objSAASInvoiceBuilder.strJobID;
                        }
                        //isAlert = false;
                        //showSuccessMessage("Invoice Sent successfully to the SAAS Invoice");
                        if (Temp == DatatableCheckboxChecked) {
                            showSuccessMessage("Invoice Sent successfully to the SAAS Invoice");
                            filter = $('#datatable').data('kendoGrid').dataSource.filter()
                            //$('#datatable').kendoGrid('destroy').empty();
                            SAASInvoiceBuilderKendoIntialiize();
                            drawSAASInvoiceBuilderKendoGrid(filter);
                        }
                        else {
                            Temp++;
                        }
                    },
                    error: function (ex) {
                        Temp++;
                        alert("Error sending invoices");
                    }
                });
            }
        }
        else {
            //if (alreadySentInvoice) {
            //    alert("Some invoices are alredy sent, Select unsent invoices only")
            //}
            //else if (selectedRows == null || selectedRows.length == 0) {
            //if (selectedRows == null || selectedRows.length == 0) {
            //alert('Please select at least one SAAS Invoice to send.');
            //return false;
            //}
        }
    })
    isSaved = true;
    if (DatatableCheckboxChecked == 0) {
        alert('Please select at least one SAAS Invoice to send.');
        return false;
    }
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
}


function FillDropDownUser(id) {
    $.ajax({
        url: '/SAASInvoiceBuilder/GetUserRolesSASS',
        type: "POST",
        data: { id: id },
        dataType: "json",
        success: function (data) {
            $("#ddlUserRoles").empty();
            if (data != '') {
                $("#ddlUserRoles").prepend('<option value="' + 0 + '"> All </option>')
                $.each(data, function (i, res) {
                    $("#ddlUserRoles").append('<option value="' + res.RoleId + '">' + res.Name + '</option>');
                });
            }
        },
        error: function (ex) {
            alert('Failed to Retrieve User Roles.' + ex);
        }
    });
}

function ResetSearchFilters(ShowAll) {
    $("#SAASUserId").val('');
    $("#hdnresellerId").val("");
    $("#UserTypeID").val("");
    $("#ddlUserRoles").val(0);
    $("#hdnResellerIdForFilter").val("");
    drawSAASInvoiceBuilderKendoGrid();
}

function CheckChangeChild(id) {
    debugger;
    //if ($("#chk_" + id).is(':checked')) {
    //    lstJobID.push(id);
    //    strlstJobID = lstJobID.join(", ");
    //}
    //else {
    //    lstJobID = jQuery.grep(lstJobID, function (value) {
    //        return value != id;
    //    });
    //    strlstJobID = lstJobID.join(", ");
    //}

    if ($("#chk_" + id).is(':checked')) {
    }
    else {
        $("#" + $("#chk_" + id).attr("headercheckboxid")).prop("checked", false);
    }
}

function CheckChangeParent(strid, rownum) {
    var ArrId = strid.split(',');

    //for (var i = 0; i < arrid.length; i++) {
    //    if ($("#chk_" + arrid[i]).is(':checked')) {
    //        $("#chk_" + arrid[i]).prop("checked", false);
    //        strlstjobid = '';
    //    }
    //    else {
    //        $("#chk_" + arrid[i]).prop("checked", true);
    //        strlstjobid = strid;
    //    }
    //}

    for (var i = 0; i < ArrId.length; i++) {
        if ($("#chk_" + ArrId[i] + "_" + rownum).is(':checked')) {
            $("#chk_" + ArrId[i] + "_" + rownum).prop("checked", false);
            strlstjobid = '';
        }
        else {
            $("#chk_" + ArrId[i] + "_" + rownum).prop("checked", true);
            strlstjobid = strid;
        }
    }

    //if ($("#chk_" + rownum).is(':checked')) {
    //    for (var i = 0; i < ArrId.length; i++) {
    //        $("#chk_" + ArrId[i] + "_" + rownum).prop("checked", false);
    //        $('.k-grid td .k-checkbox-label[value="' + ArrId[i] + "_" + rownum + '"]').hide();
    //    }
    //}
    //else {
    //    for (var i = 0; i < ArrId.length; i++) {
    //        $('.k-grid td .k-checkbox-label[value="' + ArrId[i] + "_" + rownum + '"]').show();
    //    }
    //}
}

function deleteSAASInvoiceFomBuilder(jobid) {
    var result = confirm("Are you sure you want to remove job with ID: " + jobid + " ?");
    if (result) {
        $.ajax({
            type: "POST",
            url: "/SAASInvoiceBuilder/DeleteSAASInvoiceFomBuilder",
            dataType: "json",
            data: { jobid: jobid },
            success: function (data) {
                filter = $('#datatable').data('kendoGrid').dataSource.filter()
                SAASInvoiceBuilderKendoIntialiize();
                drawSAASInvoiceBuilderKendoGrid(filter);
            },
            error: function (ex) {
                alert("Error deleting data");
            }
        });
    }
}

function restoreSAASInvoiceFomBuilder(invoiceId) {
    $.ajax({
        type: "POST",
        url: "/SAASInvoice/RestoreSAASInvoice",
        dataType: "json",
        data: { InvoiceId: invoiceId },
        success: function (data) {
            filter = $('#datatable').data('kendoGrid').dataSource.filter()
            SAASInvoiceKendoIntialiizeNew();
            drawSAASInvoiceKendoGridNew(filter);
        },
        error: function (ex) {
            alert("Error restoring data");
        }
    });
}

$('#btnCreateNewInvoice').click(function () {
    BindPopSAASUser();
    $('#popupCreateNewInvoice').modal({ backdrop: 'static', keyboard: false });
});

function BindGlobalPricingTerms() {

    $("#lblMonthWarning").text('');
    $("#dvMonthWarning").hide();

    if ($("#ddlPopSaasUsers").val() != '0') {
        $.ajax({
            type: "GET",
            url: "/SAASInvoiceBuilder/GetGlobalBillingTermsList",
            data: { UserID: $("#ddlPopSaasUsers option:selected").attr("userid") },
            success: function (data) {
                var s = '<option value="0" id="popoptn_0">Please Select a Billable Term</option>';
                for (var i = 0; i < data.length; i++) {
                    var gstsection = data[i].IsGST == true ? '+GST' : '';
                    var strBillingTerm = '(' + data[i].BillerCode + ') ' + data[i].TermName + ' ' + '$' + data[i].Price + gstsection + ' ' + data[i].TermCode;
                    s += '<option id="popoptn_' + data[i].Id + '"  value="' + data[i].Id + '" TermName="' + data[i].TermName + '" TermCode="' + data[i].TermCode + '" Price="' + data[i].Price + '" Description="' + data[i].TermDescription + '"  IsGlobalGST="' + data[i].IsGST + '">' + strBillingTerm + '</option>';
                    strBillingTerm = '';
                    gstsection = '';
                }
                $("#ddlPopTermCode").html(s);
            }
        });
    }
    else {
        var s = '<option value="0" id="popoptn_0">Please Select a Billable Term</option>';
        $("#ddlPopTermCode").html(s);
    }
}

function BindPopSAASUser() {

    $.ajax({
        url: "/SAASInvoiceBuilder/GetSAASForPricingManager",
        dataType: "json",
        data: {},
        success: function (SAASUsers) {
            var s = '<option value="0" id="popOptnSaasUser_0">Select</option>';
            for (var i = 0; i < SAASUsers.length; i++) {
                s += '<option id="popOptnSaasUser_' + SAASUsers[i].ResellerIdForFilter + '"  value="' + SAASUsers[i].ResellerIdForFilter + '" ResellerName="' + SAASUsers[i].ResellerName + '" UserId="' + SAASUsers[i].ResellerID + '">' + SAASUsers[i].ResellerName + '</option>';
            }
            $("#ddlPopSaasUsers").html(s);
        }
    });
}

function BindPrice(TermCode) {
    if ($("#ddlPopTermCode").val() != 0) {
        $("#txtpopPrice").val($("#popoptn_" + TermCode.value).attr('price'));
        if ($("#popoptn_" + TermCode.value).attr('isglobalgst')) {
            $('#IspopGlobalGST').prop("checked", true)
        }
        else {
            $('#IspopGlobalGST').prop("checked", false)
        }

        GetMonthAndQTYBasedOnTerms($("#popoptn_" + TermCode.value).attr('termcode'), $("#popoptn_" + TermCode.value).attr('price'), $("#ddlPopSaasUsers option:selected").attr("userid"));
    }
    else {
        $("#txtpopPrice").val('');
        $("#txtpopQuantity").val('');
        $('#IspopGlobalGST').prop("checked", false);
        var s = '<option value=""> Select </option>';
        $("#ddlPopMonth").html(s);
    }
}

function CreateNewInvoice() {
    //var ResellerID, SettlementTermId, Price, UnitQTY, IsGST;
    var UserID, ResellerID, ResellerName, SettelmentTerm, Rate, QTY, BillingPeriod, GlobalTermId, IsGlobalGST, JobID, Counter = 1;
    UserID = $("#ddlPopSaasUsers option:selected").attr("userid");
    ResellerID = $("#ddlPopSaasUsers").val();
    ResellerName = $("#ddlPopSaasUsers option:selected").attr("resellername");
    SettelmentTerm = $("#ddlPopTermCode option:selected").attr("termcode");
    Rate = $("#txtpopPrice").val();
    //QTY = $("#txtpopQuantity").val();
    QTY = 1;
    BillingPeriod = $("#ddlPopMonth option:selected").text();
    GlobalTermId = $("#ddlPopTermCode").val();
    IsGlobalGST = $("#IspopGlobalGST").prop("checked");
    JobID = $("#ddlPopMonth option:selected").attr("strjobid");
    if (ValidateCreateNewInvoice()) {
        for (var i = 0; i < JobID.split(',').length; i++) {
            $.ajax({
                url: urlCreateNewInvoice,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                type: 'post',
                data: JSON.stringify({ UserID: UserID, ResellerID: ResellerID, ResellerName: ResellerName, SettelmentTerm: SettelmentTerm, Rate: Rate, QTY: QTY, BillingPeriod: BillingPeriod, GlobalTermId: GlobalTermId, IsGlobalGST: IsGlobalGST, JobID: JobID.split(',')[i] }),
                async: false,
                success: function (response) {
                    $("#popupCreateNewInvoice .close").click();
                    $("#ddlPopSaasUsers").val('0');
                    $("#ddlPopTermCode").val('0');
                    $("#txtpopPrice").val('');
                    $("#txtpopQuantity").val('');
                    $('#IspopGlobalGST').prop("checked", false);
                    if (Counter == JobID.split(',').length) {
                        drawSAASInvoiceBuilderKendoGrid();
                        showSuccessMessage("Invoice created.");
                    }
                },
                error: function (response) {
                    showErrorMessage("Invoice not created.");
                }
            });
            Counter++;
        }
    }
}

function ValidateCreateNewInvoice() {
    if ($("#ddlPopSaasUsers").val() == '0') {
        alert('Please select saas user.');
        return false;
    }
    else if ($('#ddlPopTermCode').val() == '0') {
        alert('Please select billable term.');
        return false;
    }
    else if ($("#ddlPopMonth").val() == '') {
        alert('Please select month.');
        return false;
    }
    return true;
}

$('#btnCancelCreateInvoicePopup').click(function () {
    $("#popupCreateNewInvoice .close").click();
    $("#ddlPopSaasUsers").val('0');
    $("#ddlPopTermCode").val('0');
    $("#txtpopPrice").val('');
    $("#txtpopQuantity").val('');
    $('#IspopGlobalGST').prop("checked", false);
});

function GetMonthAndQTYBasedOnTerms(SettelmentTerm, Rate, UserID) {
    debugger;
    $.ajax({
        url: urlGetMonthAndQTYBasedOnTerms,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'post',
        data: JSON.stringify({ SettelmentTerm: SettelmentTerm, Rate: Rate, UserID: UserID }),
        success: function (response) {
            //$("#txtpopQuantity").val('');
            $("#ddlPopMonth").empty();
            if (response.data != '') {
                $("#lblMonthWarning").text('');
                $("#dvMonthWarning").hide();
                $("#ddlPopMonth").prepend('<option value=""> Select </option>');
                $.each(response.data, function (i, res) {
                    $("#ddlPopMonth").append('<option value="' + i + '" QTY="' + res.QTY + '" strJobID = "' + res.strJobID + '">' + res.BillingPeriod + '</option>');
                });
            }
            else {
                $("#lblMonthWarning").text('No month data available for selected options.');
                $("#dvMonthWarning").show();
            }

        },
        error: function (response) {
            showErrorMessage("Error fetching data!.");
        }
    });
}

function BindQTY() {
    if ($("#ddlPopMonth").val() != '') {
        $("#txtpopQuantity").val($("#ddlPopMonth option:selected").attr("qty"));
    }
    else {
        $("#txtpopQuantity").val('');
    }
}

$('#btnpopupCreateNewInvoiceClose').click(function () {
    $("#ddlPopSaasUsers").val('0');
    $("#ddlPopTermCode").val('0');
    $("#txtpopPrice").val('');
    $("#ddlPopMonth").val('');
    $("#txtpopQuantity").val('');
    $('#IspopGlobalGST').prop("checked", false);
});

function SendtoSaasInvoicesOptimized() {
    var Dataset = [];
    var Processed = null;
    var objSAASInvoiceBuilder = new Array();
    $('#datatable tbody input[type="checkbox"]:Checked').each(function () {
        var Dataset = {};
        Dataset.InvoiceID = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('invoiceid') : $(this).attr('invoiceid');
        Dataset.SettelmentTerm = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('settelmentterm') : $("#chk_" + $(this).attr('invoiceid')).attr('settelmentterm');
        Dataset.GlobalTermId = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('globaltermid') : $("#chk_" + $(this).attr('invoiceid')).attr('globaltermid');
        Dataset.IsGlobalGST = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('isglobalgst') : $("#chk_" + $(this).attr('invoiceid')).attr('isglobalgst');
        Dataset.strJobID = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('strjobid') : $(this).attr('jobid');
        Dataset.CreatedDate = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('createddate') : $("#chk_" + $(this).attr('invoiceid')).attr('createddate');
        Dataset.Rate = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('rate') : $("#chk_" + $(this).attr('invoiceid')).attr('rate');
        Dataset.QTY = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('qty') : 1;
        //Dataset.InvoiceAmount = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('invoiceamount') : $("#chk_" + $(this).attr('invoiceid')).attr('invoiceamount');
        Dataset.InvoiceAmount = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('invoiceamount') : $("#chk_" + $(this).attr('invoiceid')).attr('rate') * 1;
        Dataset.BillingPeriod = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('billingperiod') : $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod');
        Dataset.BillingMonth = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('billingperiod').split(' ')[0] : $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod').split(' ')[0];
        Dataset.BillingYear = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('billingperiod').split(' ')[1] : $("#chk_" + $(this).attr('invoiceid')).attr('billingperiod').split(' ')[1];
        Dataset.ResellerID = $(this).attr('chkType') == 'HeaderCheckBox' ? $(this).attr('resellerid') : $("#chk_" + $(this).attr('invoiceid')).attr('resellerid');
        Dataset.strAllJobIds = $(this).attr('chkType') == 'HeaderCheckBox' ? Processed : $("#chk_" + $(this).attr('invoiceid')).attr('strprocessedjobid');
        Dataset.isBulkRecord = $(this).attr('chkType') == 'HeaderCheckBox' ? true : false;
        objSAASInvoiceBuilder.push(Dataset);
    });

    if (objSAASInvoiceBuilder.length > 0) {
        var result = result = confirm("Are you sure you want to send selected SAAS Invoice(s) ?");
        if (result) {
            $.ajax({
                type: 'POST',
                url: urlSendToSAASInvoices,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                async: false,
                data: JSON.stringify(objSAASInvoiceBuilder),
                success: function (data) {
                    showSuccessMessage("Invoice Sent successfully to the SAAS Invoice");
                    filter = $('#datatable').data('kendoGrid').dataSource.filter()
                    SAASInvoiceBuilderKendoIntialiize();
                    drawSAASInvoiceBuilderKendoGrid(filter);
                },
                error: function (ex) {
                    alert("Error sending invoices");
                }
            });
        }
    }
    else {
        alert('Please select at least one SAAS Invoice to send.');
        return false;
    }
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');

}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');

}