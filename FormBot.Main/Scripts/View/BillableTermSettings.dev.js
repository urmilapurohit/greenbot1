$(document).ready(function () {
    DrawSAASPricingGrid();
});

function SaveGlobalPricingTerm() {
    if (ValidateForm()) {
        if ($("#btnGlobalPricing").val() == 'Save') {
            $.ajax({
                type: "POST",
                url: SaveGlobalBillableTermURL,
                data: {
                    Id: 0,
                    TermName: $("#txtTermName").val().trim(),
                    TermCode: $('#ddlTermCode').val(),
                    TermDescription: $("#txtTermDescription").val().trim(),
                    GlobalPrice: $("#txtGlobalPrice").val(),
                    IsGlobalGST: $("#IsGlobalGST").is(':checked'),
                    CreatedBy: ProjectSessionLoggedInUserId,
                    CreatedDate: Date.now,
                    BillerCode: ' '
                },
                success: function (response) {
                    ClearForm();
                    DrawSAASPricingGrid();
                    if (response.OutPutValue == 1) {
                        alert('Global term settings saved successfully.');
                    }
                    else {
                        alert('Cannot update global term Settings as same combination is already exists!');
                    }
                }
            });
        }
        else {
            $.ajax({
                type: "POST",
                url: SaveGlobalBillableTermURL,
                data: {
                    Id: $("#hdnglobalpriceId").val(),
                    OldGlobalPrice: $("#hdnoldglobalprice").val(),
                    TermName: $("#txtTermName").val().trim(),
                    TermCode: $('#ddlTermCode').val(),
                    TermDescription: $("#txtTermDescription").val().trim(),
                    GlobalPrice: $("#txtGlobalPrice").val(),
                    IsGlobalGST: $("#IsGlobalGST").is(':checked'),
                    CreatedBy: ProjectSessionLoggedInUserId,
                    CreatedDate: Date.now,
                    BillerCode: $("#hdnbillercode").val()
                },
                success: function (response) {
                    ClearForm();
                    DrawSAASPricingGrid();
                    if (response.OutPutValue == 3) {
                        alert('Cannot update global term as same term combination already exists!');
                    }
                    else {
                        alert('Global term settings updated successfully.');
                    }
                }
            });
        }
    }
}

function ClearForm() {
    $("#hdnglobalpriceId").val('');
    $("#txtTermName").val('');
    $('#ddlTermCode').val(0);
    $("#txtTermDescription").val('');
    $("#txtGlobalPrice").val('');
    $('#IsGlobalGST').prop('checked', false);
    
    $("#btnGlobalPricing").val('Save');
    $('#popupcreatenewbilling').modal('hide');
}

function ValidateForm() {
    if ($("#txtTermName").val() == '') {
        alert('Please enter term name.');
        return false;
    }
    else if ($('#ddlTermCode').val() == '0') {
        alert('Please enter term code.');
        return false;
    }
    else if ($("#txtTermDescription").val() == '') {
        alert('Please enter term description.');
        return false;
    }
    else if ($("#txtGlobalPrice").val() == '') {
        alert('Please enter global price.');
        return false;
    }
    return true;
}

function EditGlobalPrice(Id, BillerCode, TermName, TermCode, TermDescription, GlobalPrice, IsGlobalGST) {
    Glbl_TermCode = TermCode, Glbl_Price = GlobalPrice, Glbl_Description = TermDescription;
    $("#hdnbillercode").val(BillerCode);
    $("#hdnoldglobalprice").val(GlobalPrice);
    $('#btnSaveGlobalPricing').css('display', 'inline-block');
    $("#hdnglobalpriceId").val(Id);
    $("#txtTermName").val(TermName);
    $('#ddlTermCode').val(TermCode);
    $("#txtTermDescription").val(TermDescription);
    $("#txtGlobalPrice").val(GlobalPrice);
    if (IsGlobalGST == 'true') {
        $('#IsGlobalGST').prop('checked', true);
    }
    else {
        $('#IsGlobalGST').prop('checked', false);
    }
    $('#popupcreatenewbilling').modal({ backdrop: 'static', keyboard: false });
    $("#btnGlobalPricing").val('Update');
}

function DrawSAASPricingGrid() {
    if ($.fn.DataTable.isDataTable('#datatable')) {
        $('#datatable').DataTable().destroy();
    }
    $('#datatable tbody').empty();
    $('#datatable').DataTable({
        destory: true,
        retrieve: true,
        processing: true,
        serverSide: true,
        autoWidth: false,
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
            { 'data': 'Id', visible: false },
            { 'data': 'BillerCode' },
            { 'data': 'TermName', "width": "16%" },
            { 'data': 'TermCode' },
            { 'data': 'TermDescription' },
            {
                'data': 'GlobalPrice',
                "render": function (data, type, full, meta) {
                    return full.GlobalPrice.toFixed(2);
                }
            },
            {
                'data': 'IsGlobalGST',
                "render": function (data, type, full, meta) {
                    if (data) {
                        return 'Yes'
                    }
                    else {
                        return 'No'
                    }
                }
            },
            { 'data': 'CreatedByName' },
            {
                'data': 'CreatedDate',
                "render": function (data, type, full, meta) {
                    return ConvertToDateWithFormat(data, DateFormat);
                }
            },
            {
                'data': 'Id',
                'width': '8%',
                "render": function (data, type, full, meta) {
                    var deleteButton = "";
                    var title = "";
                    var viewHref = "";
                    var returnHTML = '';
                    if (full.isEnable == true) {

                        imgEdit = 'background:url(../Images/edit-icon.png) no-repeat center; text-decoration:none;';
                        var EditHref = "javascript:EditGlobalPrice('" + full.Id + "','" + full.BillerCode + "', '" + full.TermName + "','" + full.TermCode + "','" + full.TermDescription + "','" + full.GlobalPrice.toFixed(2) + "','" + full.IsGlobalGST + "')";
                        returnHTML += '&nbsp;&nbsp;' + '<a href="' + EditHref + '" class="action edit" style="' + imgEdit + '" title="Edit">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';

                        imgdelete = 'background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none; margin-left: 10px;';
                        var deleteHref = "javascript:DeleteBillableTerm('" + full.Id + "','" + full.BillerCode + "')";
                        returnHTML += '<a href="' + deleteHref + '" class="action delete" style="' + imgdelete + '" title="Delete">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';

                        imghistory = 'background:url(../Images/-icontest.png) no-repeat center; text-decoration:none; margin-left: 10px;';
                        var historyHref = "javascript:ViewBillingTermHistory('" + full.BillerCode + "')";
                        returnHTML += '<a href="' + historyHref + '" class="action delete" style="' + imghistory + '" title="History">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
                    }
                    else {
                        imgRestore = 'background:url(../Images/ic-reset.png) no-repeat center; text-decoration:none; margin-left: 10px;';
                        var restoreHref = "javascript:restoreBillableTerm('" + full.Id + "','" + full.BillerCode + "')";
                        returnHTML += '<a href="' + restoreHref + '" class="action delete" style="' + imgRestore + '" title="Restore">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';

                        imghistory = 'background:url(../Images/-icontest.png) no-repeat center; text-decoration:none; margin-left: 10px;';
                        var historyHref = "javascript:ViewBillingTermHistory('" + full.BillerCode + "')";
                        returnHTML += '<a href="' + historyHref + '" class="action delete" style="' + imghistory + '" title="History">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
                    }
                    return returnHTML;
                },
                "orderable": false,
            },

        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        "createdRow": function (row, data, dataIndex) {
            if (data.IsEnable == false) {
                $('td', row).css('background-color', '#D2D2D2');
            }
        },
        //initComplete: function( settings, json ) {
        //    $('.grid-boGetSolarElectricianListttom span:first').attr('id','spanMain');
        //    $("#spanMain span").html("");
        //},
        bServerSide: true,
        sAjaxSource: urlGetGlobalPricingList,

        "fnDrawCallback": function () {
            if ($('#chkAll').prop('checked') == true) {
                chkCount = $('#datatable >tbody >tr').length;
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
            var info = table.page.info();
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> Billable Term(s) found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b> Billable Term(s) found';
            }
        },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "TermName", "value": $("#txtSearchTermName").val() });
            aoData.push({ "name": "BillerCode", "value": $("#txtSearchBillableCode").val() });
            aoData.push({ "name": "TermDescription", "value": $("#txtSearchTermDescription").val() });
            aoData.push({ "name": "TermCode", "value": $("#ddlSearchTermCode").val() });
        }
    });
}

function SearchBillableTerms() {
    $("#datatable").dataTable().fnDraw();
}
$('#btnCreateNewBilling').click(function () {
    $('#popupcreatenewbilling').modal({ backdrop: 'static', keyboard: false });
});

function ResetSearchFilters() {
    $("#txtSearchTermName").val('');
    $("#txtSearchBillableCode").val('');
    $("#txtSearchTermDescription").val('');
    $("#ddlSearchTermCode").val(0);
    $("#datatable").dataTable().fnDraw();
}

$('#popupcreatenewbillingclose').click(function () {
    ClearForm();
});

function ViewBillingTermHistory(BillerCode) {
    $("#loading-image").show();
    setTimeout(function () {
        $.ajax({
            url: urlShowHistory,
            type: "GET",
            data: { BillerCode: BillerCode },
            cache: false,
            async: false,
            success: function (Data) {
                debugger;
                window.onbeforeunload = null;
                $("#JobHistoryOfJob").html($.parseHTML(Data));
                if ($('#divHistoryList li').length >= 6) {
                    $('#historyList').css("height", 300);
                }
                else if ($('#divHistoryList li').length >= 2) {
                    $('#historyList').css("height", 200);
                }
                $('#JobHistory').modal({ backdrop: 'static', keyboard: !1 });
                //$("divCustom").mCustomScrollbar();
            }
        });
        $("#loading-image").hide();
    }, 10);
}

$('#JobHistorySearch').on('keyup', function () {
    debugger;
    tagandsearchfilter();
});

function tagandsearchfilter() {
    var count = 0;
    //var tagged = $("#relatedTofilter").val();
    var tagged = "0";
    var searchfilter = '@@' + tagged;
    var searchboxfilter = $("#JobHistorySearch").val();
    if (tagged == "0") {
        $('#divCustom .history-box').each(function () {
            if ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0) {
                $(this).hide();
            } else {
                $(this).show();
                $("#norecords").empty();
                count++;
            }
        });
    }
    else {
        $('#divCustom .history-box').each(function () {

            if (($(this).text().search(new RegExp(searchfilter, "i")) < 0) || ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0)) {
                $(this).hide();
            } else {

                $(this).show();
                $("#norecords").empty();
                count++;
            }
        });
    }
    if (count > 0) {
        $(".history-box").css(
            "border-top", "1px solid #e3e3e3");
    }
    else {
        $("#norecords").empty();
        var norecords = "<div style='text-align:center;font-size:24px;margin-top:120px'>" + "No Record(s) Found." + "</div>";
        $("#norecords").append(norecords);
    }
}

function DeleteBillableTerm(ID, BillingTermCode) {
    $.ajax({
        type: "POST",
        url: urlDeleteBillableTermByID,
        data: {
            Id: ID,
            BillerCode: BillingTermCode
        },
        success: function (response) {
            alert("Billing Term Deleted Sucessfully.");
            DrawSAASPricingGrid();
        },
        error: function (ex) {
            alert("Error deleting data");
        }
    });
}

function restoreBillableTerm(ID, BillingTermCode) {
    $.ajax({
        type: "POST",
        url: urlRestoreBillableTerm,
        data: {
            Id: ID,
            BillerCode: BillingTermCode
        },
        success: function (response) {
            DrawSAASPricingGrid();
        },
        error: function (ex) {
            alert("Error restoring data");
        }
    });
}