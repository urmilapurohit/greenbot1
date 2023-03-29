var kendoCols = [];

function SAASInvoiceKendoIntialiize() {
    kendoCols = [];
    kendoCols.push({
        title: 'Chkbox',
        field: 'Id',
        encoded: false,
        template: function (data) {
            return '<input type="checkbox" class="k-checkbox"  isInvoice = "' + data.IsInvoiceFlag.toLowerCase() + '" SAASInvoiceId="' + data.InvoiceId + '" Price="' + data.TotalAmount + '" Quantity="' + data.Quantity + '" PaymentStatusID="' + data.Status + '" InvoiceNumber="' + data.InvoiceNumber + '" JobID="' + data.JobID + '" XeroInvoiceId="' + data.XeroInvoiceId + '" id="chk_' + data.InvoiceId + '"><label class="k-checkbox-label" for="chk_' + data.InvoiceId + '">';
        },
        filterable: false,
        sortable: false,
        headerTemplate: '<input type="checkbox" id="chkAll" name="select_all"  class="k-checkbox"><label class="k-checkbox-label" for="chkAll"></label>',
        width: 15,
        stickyPosition: 0,
        reorderable: false,
        resizable: false
    });
    kendoCols.push({
        title: "Invoice Id",
        field: "InvoiceNumber",
        encoded: false,
        headerAttributes: {
            "data-field": "Invoice Id",
            "data-columnid": "InvoiceNumber",
            "data-order": 2
        },
        headerTemplate: '<span>Invoice Id</span>',
        width: 32,
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
    kendoCols.push({
        title: "Reseller Name",
        field: "ResellerName",
        encoded: false,
        headerAttributes: {
            "data-field": "ResellerName",
            "data-columnid": "ResellerName",
            "data-order": 2
        },
        headerTemplate: '<span>Reseller Name</span>',
        width: 50,
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
    kendoCols.push({
        title: "Invoice Created",
        field: "CreatedDate",
        encoded: false,
        headerAttributes: {
            "data-field": "Invoice Created",
            "data-columnid": "CreatedDate",
            "data-order": 3
        },
        headerTemplate: '<span>Invoice Created</span>',
        width: 30,
        template: "#=Kendo_CreatedDate(data) #",
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: true,
            ui: eval("filter_CreatedDate")
        },
        sortable: true,
    });
    kendoCols.push({
        title: "Status",
        field: "Status",
        encoded: false,
        headerTemplate: '<span>Status</span>',
        width: 30,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to"
                }
            },
            extra: false,
            ui: filterStatus
        },
        template: function (data) {
            if (data.Status == 1) {
                return '<span >Outstanding</span>';
            }
            else if (data.Status == 2) {
                return '<span>Partial Pay</span>';
            }
            else if (data.Status == 3) {
                return '<span>Paid</span>';
            }
            else if (data.Status == 4) {
                return '<span>Cancelled</span>';
            }
        },
        sortable: true,
        reorderable: false,
        resizable: false,
        stickyPosition: 4,
    });
    kendoCols.push({
        title: "Invoice Due",
        field: "InvoiceDueDate",
        encoded: false,
        headerAttributes: {
            "data-field": "Invoice Due",
            "data-columnid": "InvoiceDueDate",
            "data-order": 5
        },
        headerTemplate: '<span>Invoice Due</span>',
        width: 30,
        template: "#=Kendo_InvoiceDueDate(data) #",
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: true,
            ui: eval("filter_InvoiceDueDate")
        },
        sortable: true,
    });
    kendoCols.push({
        title: "Settlement Term",
        field: "SettlementTerms",
        encoded: false,
        headerAttributes: {
            "data-field": "Settlement Term",
            "data-columnid": "SettlementTerms",
            "data-order": 6
        },
        headerTemplate: '<span>Settlement Term</span>',
        width: 30,
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
    kendoCols.push({
        title: "Rate",
        field: "Price",
        headerAttributes: {
            "data-field": "Rate",
            "data-columnid": "Price",
            "data-order": 7
        },
        template: function (data) {
            if (data.IsGst) {
                return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.Price).toFixed(2) + ' +GST' + '</lable>';
            }
            else {
                return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.Price).toFixed(2) + '</lable>';
            }
        },
        width: 25,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: false
        }
    });
    kendoCols.push({
        title: "Quantity",
        field: "Quantity",
        headerAttributes: {
            "data-field": "Quantity",
            "data-columnid": "Quantity",
            "data-order": 8
        },
        template: function (data) {
            return '<lable class="lblSTcPrice">' + parseInt(data.Quantity) + '</lable>';
        },
        width: 25,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: false
        }
    });
    kendoCols.push({
        title: "Total Amount",
        field: "TotalAmount",
        headerAttributes: {
            "data-field": "Invoice Amount",
            "data-columnid": "TotalAmount",
            "data-order": 7
        },
        template: function (data) {
            return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.TotalAmount).toFixed(2) + '</lable>';
        },
        width: 30,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: false
        }
    });
    kendoCols.push({
        title: "Amount Paid",
        field: "PaidAmount",
        headerAttributes: {
            "data-field": "Amount Paid",
            "data-columnid": "PaidAmount",
            "data-order": 8
        },
        template: function (data) {
            if (data.PaidAmount == null) {
                return '<lable class="lblSTcPrice">' + '$' + parseFloat('0').toFixed(2) + '</lable>';
            }
            else {
                return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.PaidAmount).toFixed(2) + '</lable>';
            }
        },
        width: 30,
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to",
                    "neq": "Is not equal to",
                }
            },
            extra: false
        }
    });
    kendoCols.push({
        title: "Billing Period",
        field: "BillingPeriod",
        encoded: false,
        headerAttributes: {
            "data-field": "Billing Period",
            "data-columnid": "BillingPeriod",
            "data-order": 9
        },
        headerTemplate: '<span>Billing Period</span>',
        width: 40,
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
    kendoCols.push({
        title: "Sent",
        field: "IsSent",
        template: function (data) {
            if (data.IsInvoiceFlag.toLowerCase() == 'true') {
                return '<span style="font-weight: bold;color: Green;">Yes</span>';
            }
            else {
                return '<span style="font-weight: bold;color: Red;">No</span>';
            }
        },
        width: 20,
        attributes: {
            class: "dt-right"
        },
        headerAttributes: {
            "data-field": "IsSent",
            "data-columnid": "IsSent",
            "data-order": 10
        },
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to"
                }
            },
            extra: false
        }
    })
    kendoCols.push({
        title: 'Action',
        field: 'Id',
        template: function (data) {
            imgassign = 'text-decoration:none;';
            if (UserTypeID == 4 || UserTypeID == 6) {
                var assignHref = "javascript:LoadStc('" + data.JobID + "','" + data.STCJobDetailID + "')";
            }
            else {
                var assignHref = "javascript:StcCompliance(this,'" + data.JobID + "','" + data.STCJobDetailID + "')";
            }
            var returnHTML = '';
            if (UserTypeID == 1) {
                returnHTML += '<a href="javascript:void(0);" onclick="CheckInXero(false, false, ' + data.SAASUserId + ');" style="margin-right:0; margin-top:0px;text-decoration:none;background-position: 11px 0px !important;" class="sync-ic" title="Check In Xero"></a>'
                if (!data.IsDeleted) {
                    returnHTML += '<a href="javascript:void(0);" onclick="deleteSAASInvoice(' + data.InvoiceId + ')" id="" class="btn default icon-btn btn-sm remove-ic" title="Delete invoice"></a>'
                }
                else {
                    returnHTML += '<a href="javascript:void(0);" onclick="restoreSAASInvoice(' + data.InvoiceId + ')" id="" class="btn default btn-sm sprite-img reset_ic" title="Restore invoice"></a>'
                }
            }
            returnHTML += '<a href="javascript:void(0);" onclick="DownloadSAASInvoice(\'' + data.InvoiceNumber + '\')" id="" class="btn default btn-sm download_doc" title="Download invoice" style="background-position: center;"></a>'
            return returnHTML;
        },
        sortable: false,
        width: 50,
        filterable: false
    });
}

var filterYesNo = function (element) {
    var data = [
        { Text: "Yes", Value: 1 },
        { Text: "No", Value: 0 },
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
};

var filterStatus = function (element) {
    var data = [
        { Text: "OutStanding", Value: 1 },
        { Text: "PartialPaid", Value: 2 },
        { Text: "Paid", Value: 3 },
        { Text: "Cancelled", Value: 4 }
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
};

var filter_CreatedDate = function (element) {
    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

var filter_InvoiceDueDate = function (element) {
    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

var Kendo_CreatedDate = function (data) {
    return ToDate(data.CreatedDate);
};

var Kendo_InvoiceDueDate = function (data) {
    return ToDate(data.InvoiceDueDate);
};

function drawSAASInvoiceKendoGrid(filter) {
    if ($('#datatable').hasClass("k-grid")) {
        $('#datatable').kendoGrid('destroy').empty();
    }
    kendo.ui.FilterMenu.fn.options.operators.date = {
        gte: "Begin Date",
        lte: "End Date"
    };
    $("#datatable").kendoGrid({
        columns: kendoCols,
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: urlSAASInvoiceIndex,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    if (options.filter != undefined) {
                        for (i = 0; i < options.filter.filters.length; i++) {
                            if (options.filter.filters[i].field == "CreatedDate" || options.filter.filters[i].field == "InvoiceDueDate") {
                                options.filter.filters[i].value = moment(new Date(options.filter.filters[i].value.toString().substr(0, 16))).format("DD-MM-YYYY")
                            }
                        }
                    }
                    if (UserTypeID == 1 || UserTypeID == 3) {
                        options.reseller = document.getElementById("hdnResellerIdForFilter").value;
                    }
                    options.createdFromDate = $("#txtCreatedFromDate").val();
                    options.createdToDate = $("#txtCreatedToDate").val();
                    options.invoiceDueFromDate = $("#txtInvoiceDueFromDate").val();
                    options.invoiceDueToDate = $("#txtInvoiceDueToDate").val();
                    options.InvoiceId = $("#txtInvoiceId").val();
                    options.Owner = $("#searchOwner").val();
                    if ($("#ddlBillingPeriod").val() == '1') {
                        options.BillingPeriod = 'Monthly';
                    }
                    else if ($("#ddlBillingPeriod").val() == '2') {
                        options.BillingPeriod = 'Weekly'
                    }
                    else {
                        options.BillingPeriod = '';
                    }
                    options.JobID = $("#searchJobid").val();
                    if ($("#SettlementTermId").val() == '1') {
                        options.SettlementTermId = 'PerSTC-S';
                    }
                    else if ($("#SettlementTermId").val() == '2') {
                        options.SettlementTermId = 'PerJob-S';
                    }
                    else if ($("#SettlementTermId").val() == '3') {
                        options.SettlementTermId = 'PerUser-S';
                    }
                    else {
                        options.SettlementTermId = '';
                    }
                    options.IsSent = $("#chkSentInvoice").prop("checked");
                    options.reseller = $("#hdnResellerIdForFilter").val();

                    if (options.filter != undefined) {
                        for (var i = 0; i < options.filter.filters.length; i++) {
                            if (options.filter.filters[i].field == 'IsSent') {
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
                        InvoiceDueDate: { type: "date" }
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
            resizable: true,
            scrollable: true,
            reorderable: true,
            filterMenuInit: function (e) {
                $(".loading-image").hide();
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
                }
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
        columnResize: function (e) {
            SaveUserWiseColumnDetails();
        },
        columnReorder: onReorder,
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        resizable: true,
        scrollable: true,
        reorderable: true,
        filterMenuInit: function (e) {
            $(".loading-image").hide();
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
            }
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
        detailInit: detailInitNew
    });

    $("#datatable").data("kendoGrid").thead.kendoTooltip({
        filter: "th:not(':first')",
        content: function (e) {
            var target = e.target;
            return $(target).text();
        }
    });
}

function onReorder(e) {
    if (typeof e.column.reorderable != "undefined" && !e.column.reorderable) {
        var grid = $('#datatable').data('kendoGrid');
        setTimeout(function () {
            grid.reorderColumn(e.oldIndex, grid.columns[e.newIndex]);
        }, 0);
    }
}

function ToDate(data) {
    if (data != null) {
        return data.getFullYear() + '-' + ("0" + (data.getMonth() + 1)).slice(-2) + '-' + ("0" + data.getDate()).slice(-2);
    }
    else {
        return '';
    }
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

function detailInit(e) {
    $("<div/>").appendTo(e.detailCell).kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: urlSAASInvoiceDetail,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.InvoiceId = e.data.InvoiceId;
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
        columns: [
            { field: "InvoiceId", title: "InvoiceId", width: "0px" },
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
            { field: "STCPVDCode", title: "PVD Code", width: "200px" },
        ]
    });
}

function detailInitNew(e) {
    var isBindDetail = e.data.strJobID;
    if (isBindDetail != null) {
        $("<div/>").appendTo(e.detailCell).kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: urlSAASInvoiceDetail,
                        data: {},
                        contentType: 'application/json',
                        dataType: 'json',
                        type: 'POST'
                    },
                    parameterMap: function (options) {
                        options.strJobID = e.data.strJobID;
                        options.IsInvoiced = e.data.IsInvoice;
                        options.InvoiceId = e.data.InvoiceId;
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
                { field: "InvoiceId", title: "InvoiceId", width: "110px" },
                { field: "JobId", title: "Job Id", width: "110px" },
                { field: "RefNumber", title: "Ref. Number", width: "150px" },
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
                { field: "STCPVDCode", title: "PVD Code", width: "200px" },
            ]
        });
    }
}

/* To add different color to child records of detail grid */
function childGridDataBound(e) {
    var grid = $("#datatable").data("kendoGrid");
    var dataview = e.sender.dataSource._view.length;
    for (var i = 0; i < dataview; i++) {
        var currentUid = e.sender.dataSource._view[i].uid
        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        $(currenRow).css('background-color', '#fff1e0');
    }
}

$(document).ready(function () {
    var resellerId;
    var solarCompanyId;
    $('#importCSV').fileupload({

        //url: "/SAASInvoice/ImportExcelForUserTypeMenu",
        url: "/SAASInvoice/ImportCSV",
        dataType: 'json',
        add: function (e, data) {
            resellerId = $("#ResellerId").val();
            //data.formData = { resellerId: resellerId };
            data.submit();
        },
        done: function (e, data) {
            if (data.result.status) {
                //$("#datatable").dataTable().fnDraw();
                filter = $('#datatable').data('kendoGrid').dataSource.filter()
                SAASInvoiceKendoIntialiize();
                drawSAASInvoiceKendoGrid(filter);
                showSuccessMessage("CSV File has been imported successfully.");
            }
            else {
                if (data.result.error)
                    showErrorMessage(data.result.error);
                else
                    showErrorMessage("CSV File has not been imported.");
            }
        },
        progressall: function (e, data) {
        },
        singleFileUploads: false,
        send: function (e, data) {
            var mimeType = data.files[0].type;
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        showErrorMessage("Please upload small filename.");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {
                        showErrorMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    showErrorMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB");
                    return false;
                }
            }
            if (!(mimeType == "text/csv" || mimeType == "application/csv" || mimeType == "csv" || mimeType == "text/x-csv" || mimeType == "application/x-csv" || mimeType == "text/x-comma-separated-values" ||
                mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || mimeType == "application/vnd.ms-excel" || mimeType == "text/comma-separated-values" || mimeType == "")) {
                showErrorMessage("Please upload a file with .csv extension.");
                return false;
            }

            $(".alert").hide();
            $("#errorMsgRegionForPopUp").html("");
            $("#errorMsgRegionForPopUp").hide();
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');

    BindSAASUser();
    BindPopSAASUser();
});

function BindSAASUser() {

    $.ajax({
        url: "/SAASInvoiceBuilder/GetSAASForPricingManager",
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

function ExportCsv() {

    window.location.href = '/SAASInvoice/ExportCSV?&reseller=' + $("#hdnresellerId").val() +
        '&InvoiceId=' + $("#txtInvoiceId").val() +
        '&JobId=' + $("#searchJobid").val() +
        '&Owner=' + $("#searchOwner").val() +
        '&createdFromDate=' + GetDates($("#txtCreatedFromDate").val()) +
        '&createdToDate=' + GetDates($("#txtCreatedToDate").val()) +
        '&invoiceDueFromDate=' + GetDates($("#txtInvoiceDueFromDate").val()) +
        '&invoiceDueToDate=' + GetDates($("#txtInvoiceDueToDate").val()) +
        '&IsSent=' + document.getElementById('chkSentInvoice').checked +
        '&SettlementTermId=' + $("#SettlementTermId").val() +
        '&filter=' + JSON.stringify($('#datatable').data('kendoGrid').dataSource.filter());
}

function ExportAllCsv() {
    window.location.href = '/SAASInvoice/ExportAllCSV?&reseller=' + $("#hdnresellerId").val() +
        '&InvoiceId=' + $("#txtInvoiceId").val() +
        '&JobId=' + $("#searchJobid").val() +
        '&Owner=' + $("#searchOwner").val() +
        '&createdFromDate=' + GetDates($("#txtCreatedFromDate").val()) +
        '&createdToDate=' + GetDates($("#txtCreatedToDate").val()) +
        '&invoiceDueFromDate=' + GetDates($("#txtInvoiceDueFromDate").val()) +
        '&invoiceDueToDate=' + GetDates($("#txtInvoiceDueToDate").val()) +
        '&IsSent=' + document.getElementById('chkSentInvoice').checked +
        '&SettlementTermId=' + $("#SettlementTermId").val() +
        '&filter=' + JSON.stringify($('#datatable').data('kendoGrid').dataSource.filter());
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

function GetDates(date) {
    if (date != '') {
        var tickStartDate = ConvertDateToTick(date, dateFormat);
        return moment(tickStartDate).format("YYYY-MM-DD");
    }
    else {
        return '';
    }
}

function deleteSAASInvoice(invoiceId) {
    var result = confirm("Are you sure you want to remove SAAS Invoice with ID: " + invoiceId + " ?");
    if (result) {
        $.ajax({
            type: "POST",
            url: "/SAASInvoice/DeleteSAASInvoice",
            dataType: "json",
            data: { InvoiceId: invoiceId },
            success: function (data) {
                filter = $('#datatable').data('kendoGrid').dataSource.filter()
                SAASInvoiceKendoIntialiize();
                drawSAASInvoiceKendoGrid(filter);
            },
            error: function (ex) {
                alert("Error deleting data");
            }
        });
    }
}

function RemoveSelectedSAASInvoice() {
    selectedRows = [];
    //var xeroInvoiceIds=[];
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            selectedRows.push($(this).attr('saasinvoiceid'));
            //    if($(this).attr('XeroInvoiceId')!='' && $(this).attr('XeroInvoiceId')!=null && $(this).attr('XeroInvoiceId')!=undefined)
            //        xeroInvoiceIds.push({ STCInvoiceNumber: $(this).attr('STCInvoiceNumber'), XeroInvoiceId: $(this).attr('XeroInvoiceId'), JobID: $(this).attr('jobId'), STCInvoiceID: $(this).attr('id').substring(4), STCJobDetailsID: $(this).attr('stcjobdetailsid') });
        }
    })
    if (selectedRows != null && selectedRows.length > 0) {
        var result = confirm("Are you sure you want to remove selected SAAS Invoice(s) ?");
        if (result) {
            $.ajax({
                type: 'POST',
                url: '/SAASInvoice/RemoveSelectedSAASInvoice',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ invoiceIds: JSON.stringify(selectedRows) }),
                success: function (data) {
                    filter = $('#datatable').data('kendoGrid').dataSource.filter()
                    SAASInvoiceKendoIntialiize();
                    drawSAASInvoiceKendoGrid(filter);
                },
                error: function (ex) {
                    alert("Error deleting data");
                }
            });
        }
    }
    else {
        if (selectedRows == null || selectedRows.length == 0)
            alert('Please select any SAAS Invoice to remove.');
    }
}

function restoreSAASInvoice(invoiceId) {
    $.ajax({
        type: "POST",
        url: "/SAASInvoice/RestoreSAASInvoice",
        dataType: "json",
        data: { InvoiceId: invoiceId },
        success: function (data) {
            filter = $('#datatable').data('kendoGrid').dataSource.filter()
            SAASInvoiceKendoIntialiize();
            drawSAASInvoiceKendoGrid(filter);
        },
        error: function (ex) {
            alert("Error restoring data");
        }
    });
}

function StcCompliance(row, JobId, JobDetailsId) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        type: 'POST',
        url: url_StatusCompliance,
        data: JSON.stringify({ uniq_param: (new Date()).getTime(), JobId: JobId, JobDetailsId: JobDetailsId }),
        cache: false,
        //async: false,
        dataType: "html",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $("#popupStcCompliance").modal({ backdrop: 'static', keyboard: false });
            $("#modalContentStcCompliance").html("");
            $("#modalContentStcCompliance").html(response);
            if ($(row).closest('tr').hasClass("hidden-row"))
                $("#isMultipleRecords").val(1)
            else
                $("#isMultipleRecords").val(0)

        },
        error: function (response) {


        }
    });
}

function DownloadSAASInvoice(InvoiceId) {
    var grid = $("#datatable").data("kendoGrid");
    var currentPage = grid.dataSource.page();
    TempInvoiceId = InvoiceId;
    TempIsSent = $("#chkSentInvoice").prop("checked");
    window.location.href = urlDownloadSaasInvoice + TempInvoiceId + '&IsSent=' + TempIsSent + '&page=' + currentPage;

}

$('#btnCreateNewInvoice').click(function () {
    BindGlobalPricingTerms();
    $('#popupCreateNewInvoice').modal({ backdrop: 'static', keyboard: false });
});

$('#btnCancelCreateInvoicePopup').click(function () {
    $("#popupCreateNewInvoice .close").click();
    $("#ddlPopSaasUsers").val('0');
    $("#ddlPopTermCode").val('0');
    $("#txtpopPrice").val('');
    $("#txtpopQuantity").val('');
    $('#IspopGlobalGST').prop("checked", false);
});

function BindGlobalPricingTerms() {
    $.ajax({
        type: "GET",
        url: "/SAASPricingManager/GetGlobalBillingTermsList",
        data: { IsIsArchive: false },
        success: function (data) {
            var s = '<option value="0" id="popoptn_0">Please Select a Billable Term</option>';
            for (var i = 0; i < data.length; i++) {
                s += '<option id="popoptn_' + data[i].Id + '"  value="' + data[i].Id + '" TermName="' + data[i].TermName + '" TermCode="' + data[i].TermCode + '" Price="' + data[i].GlobalPrice + '" Description="' + data[i].TermDescription + '"  IsGlobalGST="' + data[i].IsGlobalGST + '">' + data[i].strBillableCode + '</option>';
            }
            $("#ddlPopTermCode").html(s);
        }
    });
}

function BindPrice(TermCode) {
    $("#txtpopPrice").val($("#popoptn_" + TermCode.value).attr('price'));
    if ($("#popoptn_" + TermCode.value).attr('isglobalgst')) {
        $('#IspopGlobalGST').prop("checked", true)
    }
    else {
        $('#IspopGlobalGST').prop("checked", false)
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
                s += '<option id="popOptnSaasUser_' + SAASUsers[i].ResellerIdForFilter + '"  value="' + SAASUsers[i].ResellerIdForFilter + '" ResellerName="' + SAASUsers[i].ResellerName + '">' + SAASUsers[i].ResellerName + '</option>';
            }
            $("#ddlPopSaasUsers").html(s);
        }
    });
}


function CreateNewInvoice() {
    var ResellerID, SettlementTermId, Price, UnitQTY, IsGST;
    ResellerID = $("#ddlPopSaasUsers").val();
    SettlementTermId = $("#ddlPopTermCode").val();
    Price = $("#txtpopPrice").val();
    UnitQTY = $("#txtpopQuantity").val();
    IsGST = $("#IspopGlobalGST").prop("checked");

    //if (ValidateCreateNewInvoice()) {
    $.ajax({
        url: urlCreateNewInvoice,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'post',
        data: JSON.stringify({ ResellerID: ResellerID, SettlementTermId: SettlementTermId, Price: Price, UnitQTY: UnitQTY, IsGST: IsGST }),
        success: function (response) {
            $("#popupCreateNewInvoice .close").click();
            $("#ddlPopSaasUsers").val('0');
            $("#ddlPopTermCode").val('0');
            $("#txtpopPrice").val('');
            $("#txtpopQuantity").val('');
            $('#IspopGlobalGST').prop("checked", false);
            drawSAASInvoiceKendoGrid();
            showSuccessMessage("Invoice created.");
        },
        error: function (response) {
            showErrorMessage("Invoice not created.");
        }
    });
    //}
}

function ValidateCreateNewInvoice() {
    if ($("#ddlPopSaasUsers").val() == '0') {
        //$("#ddlPopSaasUsersCheck").show();
        $("#ddlPopSaasUsersCheck").html("Please select saas user.");
        return false;
    }
    else {
        $("#ddlPopSaasUsersCheck").hide();
    }
    if ($("#ddlPopTermCode").val() == '0') {
        //$("#ddlPopTermCodeCheck").show();
        $("#ddlPopTermCodeCheck").html("Please select term code.");
        return false;
    }
    else {
        $("#ddlPopTermCodeCheck").hide();
    }
    if ($("#txtpopPrice").val() == '') {
        //$("#txtpopPriceCheck").show();
        $("#txtpopPriceCheck").html("Please enter price.");
        return false;
    }
    else {
        $("#txtpopPriceCheck").hide();
    }
    if ($("#txtpopQuantity").val() == '') {
        //$("#txtpopQuantityCheck").show();
        $("#txtpopQuantityCheck").html("Please enter quantity.");
        return false;
    }
    else {
        $("#txtpopQuantityCheck").hide();
    }
    return true;
}