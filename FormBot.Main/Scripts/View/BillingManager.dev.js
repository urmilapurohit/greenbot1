$(document).ready(function () {
    ActiveTab = 'UserRole'; // Set active tab name initially on load
    localStorage.setItem('SE_GridTermCodeFilter', ''); // set by default to no filter
    localStorage.setItem('SAASPM_UserName', '');
    localStorage.setItem('SAASPM_RoleID', '');
    localStorage.setItem('SAASPM_UserType', '');
    localStorage.setItem('SAASPM_TermCode', '');
    localStorage.setItem('SAASPM_BillableCode', '');
    localStorage.setItem('SAASPM_ResellerId', '');
    localStorage.setItem('SAASPM_SolarCompany', '');

    $('#myTab li:first-child a').tab('show');
    $("#profile-tab").attr('class', 'disabled')
    $("#dialog").dialog({
        autoOpen: false,
        modal: true,
        title: "View Details"
    });
    $("#dvPrive").hide();
    $("#dvPricingType").hide();
    $("#dvBtnSave").hide();
    //$("#dvBtnCancel").hide();
    $("#dvBillingDescription").hide();
    $("#dvGlobalPricingButtons").hide();
    //$("#dvBtnOptions").hide();
    BindSAASUser();
    DrawSAASPricingGrid();
    BindGlobalPricingTerms();

    FillDropDownUser('drpUserType', urlGetUserType, null, true, null);
    FillDropDown('ResellerId', urlGetReseller, localStorage.getItem('User_ResellerId'), true, null);

    $('#txtCompanyName').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnsolarCompanyid").val(0);
            $.each(solarCompanyList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#txtCompanyName").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
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

    getRole($("#drpUserType").val());

    $('#drpUserType').change(function () {
        var userTypeId = $("#drpUserType").val();

        getRole(userTypeId);

    })
});

function BindSAASUser() {
    $.ajax({
        url: "/Reseller/GetSAASForPricingManager",
        dataType: "json",
        data: {},
        success: function (SAASUsers) {

            $.each(SAASUsers, function (i, company) {
                saasUsersList.push({ value: company.Value, text: company.Text });
            });
            $('#hdnresellerId').val(saasUsersList.length > 0 ? saasUsersList[0].value : 0);
            $('#SAASUserId').val(saasUsersList.length > 0 ? saasUsersList[0].text : '');
        }
    });
}

function BindSolarCompany(resellerID) {
    var IsCompanyExist = false;
    $.ajax({
        type: 'GET',
        url: urlGetSolarCompanyByResellerId,
        dataType: 'json',
        data: { id: resellerID },
        async: true,
        success: function (solarcompany) {
            solarCompanyList = [];
            $.each(solarcompany, function (i, company) {
                solarCompanyList.push({ value: company.Value, text: company.Text });
            });

            $.each(solarCompanyList, function (key, value) {
                if (value.value == localStorage.getItem('User_SolarCompanyId')) {
                    $("#searchSolarCompany").val(value.text);
                    $('#hdnsolarCompanyid').val(localStorage.getItem('User_SolarCompanyId'));
                }
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}

function BindGlobalPricingTerms() {
    $.ajax({
        type: "GET",
        url: "/SAASPricingManager/GetGlobalBillingTermsList",
        data: {},
        success: function (data) {
            // Replace already locally stored in new data
            if (PricingLocalDataArr.length > 0) {
                for (var a = 0; a < PricingLocalDataArr.length; a++) {
                    for (var b = 0; b < data.length; b++) {
                        if (parseInt(PricingLocalDataArr[a].GlobalTermId) == data[b].Id) {
                            data[b].strBillableCode = PricingLocalDataArr[a].NewDisplayValue;
                            data[b].IsGlobalGST = PricingLocalDataArr[a].IsGlobalGST;
                        }
                    }
                }
            }

            var s = '<option value="0">Please Select a Billable Term</option>';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].Id + '" TermName="' + data[i].TermName + '" TermCode="' + data[i].TermCode + '" Price="' + data[i].GlobalPrice + '" Description="' + data[i].TermDescription + '"  IsGlobalGST="' + data[i].IsGlobalGST + '">' + data[i].strBillableCode + '</option>';
            }
            $("#ddlTermCode").html(s);


            $('#ddlBillableTerms').empty();
            $('#ddlBillableTerms').multiselect('destroy');
            for (var i = 0; i < data.length; i++) {
                $('#ddlBillableTerms').append('<option id="chk_' + data[i].Id + '" value="' + data[i].Id + '" TermName="' + data[i].TermName + '" TermCode="' + data[i].TermCode + '" Price="' + data[i].GlobalPrice + '" Description="' + data[i].TermDescription + '"  IsGlobalGST="' + data[i].IsGlobalGST + '">' + data[i].strBillableCode + '</option>');
            }
            $('ddlBillableTerms').multiselect('refresh');
            $(function () {
                $('#ddlBillableTerms').multiselect({
                    includeSelectAllOption: true,         /*To enable the Select all Option*/
                    selectAllValue: 'multiselect-all',     /*The value used for the select all option can be configured using the selectAllValue*/
                    enableFiltering: true,   /*A filter input will be added to dynamically filter all the options*/
                    enableCaseInsensitiveFiltering: true,  /*To enable Case Insenstitive Filtering (Upper and Lower Case Letters)*/
                    maxHeight: '250',
                    buttonWidth: '560',
                });
            });
        }
    });
}

function BindGlobalPricingTermDescription(val) {
    debugger;
    //if ($("#ddlBillableTerms").val() == 0) {
    if ($("#ddlBillableTerms").val() == null) {
        $("#spnBillingDescription").val('');
        $("#dvBillingDescription").hide();
        $("#dvGlobalPricingButtons").hide();
    }
    else {
        //$("#spnBillingDescription").text($('#ddlBillableTerms').find('option:selected').attr('description'));
        //$("#dvBillingDescription").show();
        //$("#dvGlobalPricingButtons").show();

        var Arr = $("#ddlBillableTerms").val();
        var FinalText = '';
        for (var i = 0; i < Arr.length; i++) {
            FinalText += '<b>' + $('#chk_' + Arr[i]).attr('termcode') + ': </b>' + $('#chk_' + Arr[i]).attr('description') + '\n';
        }
        var obj = $("#spnBillingDescription").html(FinalText);
        obj.html(obj.html().replace(/\n/g, '<br/>'));
        $("#dvBillingDescription").show();
        $("#dvGlobalPricingButtons").show();
    }

    //var DatatableCheckboxChecked = $('#datatable tbody input[type="checkbox"]:Checked').length;
    //if (DatatableCheckboxChecked == 0) {
    //    DrawSAASPricingGrid();
    //}
}

function BindGlobalPrices(TermCode) {
    var isGetFromDb = true;
    if (PricingLocalDataArr.length > 0) {
        $.each(PricingLocalDataArr, function (key, value) {
            if (value.GlobalTermId == TermCode.value) {
                $("#txtpopGlobalPrice").val(value.Price);
                isGetFromDb = false;
            }
        });
    }
    if (TermCode.value != 0 && isGetFromDb == true) {
        $.ajax({
            type: "POST",
            url: urlGetGlobalPricingList,
            data: { IsIsArchive: false },
            success: function (response) {
                for (var i = 0; i < response.length; i++) {
                    if (response[i].Id == TermCode.value) {
                        $("#txtpopGlobalPrice").val(response[i].GlobalPrice);

                        if (response[i].IsGlobalGST) {
                            $('#IspopGlobalGST').attr('checked', true);
                        }
                        else {
                            $('#IspopGlobalGST').attr('checked', false);
                        }
                    }
                }
            },
            error: function (Error) {
            }
        });
    }
    else {
        if (isGetFromDb) {
            $('#IspopGlobalGST').attr('checked', false);
            $("#txtpopGlobalPrice").val('');
            $("#ddlTermCode").val(0);
        }
    }
    isGetFromDb = true;
}

function SavePrice(id) {
    if (ValidateForm()) {
        if ($("#btnSavePrice").val() == 'Save') {
            $.ajax({
                type: "POST",
                url: savepricingURL,
                data: { SAASPricingId: 0, SAASUserId: $("#hdnresellerId").val(), SettlementTermId: $("#ddlPricingType").val(), IsEnable: true, Price: $("#txtSAASPrice").val(), IsGst: '', BillingPeriod: '0', SettlementPeriod: '0' },
                success: function (response) {
                    ClearForm();
                    $("#btnSavePrice").val('Save');
                    $("#dvPrive").hide();
                    $("#dvPricingType").hide();
                    $("#dvBtnSave").hide();
                    $("#dvBtnCancel").hide();
                    $('#datatable').DataTable().destroy();
                    $("#SAASUser").val($("#hdnresellerName").val());
                    DrawSAASPricingGrid();
                    alert('Price saved sucessfully.');
                }
            });
        }
        else {
            $.ajax({
                type: "POST",
                url: savepricingURL,
                data: { SAASPricingId: SAASId, SAASUserId: $("#hdnresellerId").val(), SettlementTermId: $("#ddlPricingType").val(), IsEnable: false, Price: $("#txtSAASPrice").val(), IsGst: '', BillingPeriod: '0', SettlementPeriod: '0' },
                success: function (response) {
                    ClearForm();
                    $("#btnSavePrice").val('Save');
                    $("#dvPrive").hide();
                    $("#dvPricingType").hide();
                    $("#dvBtnSave").hide();
                    $("#dvBtnCancel").hide();
                    $('#datatable').dataTable().fnClearTable();
                    $('#datatable').dataTable().fnDestroy();
                    $('#datatable').DataTable().destroy();
                    $("#SAASUser").val($("#hdnresellerName").val());
                    DrawSAASPricingGrid();
                    alert('Price updated sucessfully.');
                }
            });
        }

    }
}

function ClearForm() {
    $("#SAASUser").val('');
    $("#ddlPricingType").val(0);
    $("#txtSAASPrice").val('');
    //$("#hdnresellerId").val('');
    if ($("#btnSavePrice").val() == 'Update') {
        $("#btnSavePrice").val('Save');
    }
    $("#drpUserType").val('');
    $("#txtBillableCode").val('');
    $("#txtTermCode").val('');
    $("#RoleID").val('');
    $("#txtCompanyName").val('')
}

function ValidateForm() {
    if ($("#SAASUser").val() == '') {
        alert('Please select saasuser.');
        return false;
    }
    else if ($("#ddlPricingType").val() == 0) {
        alert('Please select pricing type.');
        return false;
    }
    else if ($("#txtSAASPrice").val() == '') {
        alert('Please select price.');
        return false;
    }
    return true;
}

function EditPrice(Id, Type, Price, ResellerName, SAASUserId) {
    SAASId = Id;
    $("#SAASUser").val(ResellerName);
    $("#ddlPricingType").val(Type);
    $("#txtSAASPrice").val(Price);
    $("#hdnresellerId").val(SAASUserId);
    $("#btnSavePrice").val('Update');
    $("#dvPricingType").show();
    $("#dvPrive").show();
    $("#dvBtnSave").show();
    $("#dvBtnCancel").show();
}

$("#ddlPricingType").change(function () {
    if ($("#ddlPricingType").val() == 0) {
        $("#dvPrive").hide();
    }
    else {
        $("#dvPrive").show();
    }
});

$("#txtSAASPrice").on("input", function () {
    if ($("#txtSAASPrice").val().length > 0) {
        $("#dvBtnSave").show();
        $("#dvBtnCancel").show();
    }
    else {
        $("#dvBtnSave").hide();
        $("#dvBtnCancel").hide();
    }
});

function DrawSAASPricingGrid() {
    if ($.fn.DataTable.isDataTable('#datatable')) {
        $('#datatable').DataTable().destroy();
    }
    $('#datatable tbody').empty();
    $('#datatable').DataTable({
        autoWidth: false,
        destory: true,
        retrieve: true,
        processing: true,
        serverSide: true,
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
            {
                'data': 'RoleId',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    if ($('#chkAll').prop('checked') == true) {
                        return '<input type="checkbox" id="chk_' + full.RoleId + '" RoleId="' + full.RoleId + '" onchange="SetId(' + full.RoleId + ')">';
                    }
                    else {
                        return '<input type="checkbox" id="chk_' + full.RoleId + '" RoleId="' + full.RoleId + '" onchange="SetId(' + full.RoleId + ')">';
                    }
                }
            },
            { 'data': 'RoleId', visible: false },
            { 'data': 'RoleName' },
            { 'data': 'UserTypeName' },
            //{ 'data': 'AppliedRolesUserCount' },
            //{ 'data': 'strAppliedRolesUserCount' },
            {
                'data': 'strAppliedRolesUserCount',
                "render": function (data, type, full, meta) {
                    //return '<a href="' + full.RoleId + '">' + data + '</a>';
                    return '<a href="#profile" data-toggle="tab" onclick="ShowUserInUserRoleGrid(' + full.RoleId + ')">' + data + '</a>';
                },
                "orderable": false,
            },
            {
                'data': 'BillableTerm',
                "render": function (data, type, full, meta) {
                    var SpltData = '';
                    var ReturnData = '';
                    var spltGlobalBillableTermId = '';
                    var spltPrice = '';
                    if (data != '' && data != null) {
                        SpltData = data.split(',');
                        spltGlobalBillableTermId = full.GlobalBillableTermId.split(',');
                        spltPrice = full.strPrice.split(',');
                        for (var i = 0; i < spltGlobalBillableTermId.length; i++) {
                            //for (var j = 0; j < spltPrice.length; j++) {
                            if (spltPrice.length == 1) {
                                ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow;margin-bottom: 5px;" class="col-sm-9">' + SpltData[i].replace(']', '<a href="javascript:deleteAppliedTermFromRoleGrid(' + full.RoleId + ',' + spltGlobalBillableTermId[i] + ',' + spltPrice[i] + ',' + `'${full.strBillableSettingsId}'` + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a>' + ']') + '</p></br>'
                            }
                            else {
                                ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow;margin-bottom: 5px;" class="col-sm-9">' + SpltData[i].replace(']', '<a href="javascript:deleteAppliedTermFromRoleGrid(' + full.RoleId + ',' + spltGlobalBillableTermId[i] + ',' + spltPrice[i] + `,'${full.strBillableSettingsId}'` + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp;</a>' + ']') + '</p></br>'
                            }
                            //}
                        }
                        return ReturnData;
                    }
                    else {
                        return "";
                    }
                },
                "orderable": false,
            },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",

        bServerSide: true,
        sAjaxSource: urlGetSAASPricingList,

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
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> User Role Records(s) found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b> User Role Records(s) found';
            }
        },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "RoleID", "value": localStorage.getItem('SAASPM_RoleID') });
            aoData.push({ "name": "TermCode", "value": $("#txtTermCode").val() });
            aoData.push({ "name": "UserType", "value": localStorage.getItem('SAASPM_UserType') });
            aoData.push({ "name": "BillerCode", "value": $("#txtBillableCode").val() });
            aoData.push({ "name": "TermName", "value": $("#txtTermName").val() });
        }
    });
}

$('#chkAll').on('click', function () {
    var table = $('#datatable').DataTable();
    var rows = table.rows({ 'search': 'applied' }).nodes();
    $('input[type="checkbox"]', rows).prop('checked', this.checked);
    chkCount = this.checked ? $('#datatable >tbody >tr').length : 0;

    $("#datatable input:checkbox").each(function () {
        var $this = $(this);
        if ($this.is(":checked")) {
            if ($this.attr("name") != 'select_all') {
                GlblRoleId += $this.attr("roleid") + ',';
                GlblRoleId.replace(/,\s*$/, ""); // remove comma from last
            }
        }
        else {
            GlblRoleId = '';
        }
    });
});

$('#datatable tbody').on('change', 'input[type="checkbox"]', function () {
    debugger;
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

function DrawUsersInUserRoleGrid(RoleId) {
    if ($.fn.DataTable.isDataTable('#datatableUsersInRoles')) {
        $('#datatableUsersInRoles').DataTable().destroy();
    }
    $('#datatableUsersInRoles tbody').empty();
    $('#datatableUsersInRoles').DataTable({
        autoWidth: false,
        destory: true,
        retrieve: true,
        processing: true,
        serverSide: true,
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
            {
                'data': 'RoleId',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    if ($('#chkAll').prop('checked') == true) {
                        return '<input type="checkbox" id="chk_' + full.UserId + '" onclick="SetUserId(' + full.UserId + ')">';
                    }
                    else {
                        return '<input type="checkbox" id="chk_' + full.UserId + '" onclick="SetUserId(' + full.UserId + ')">';
                    }
                }
            },
            { 'data': 'RoleId', visible: false },
            { 'data': 'UserName' },
            { 'data': 'RoleName' },
            { 'data': 'UserTypeName' },
            {
                'data': 'TermCode',
                "render": function (data, type, full, meta) {
                    var SpltData = '';
                    var ReturnData = '';
                    var spltBillableTermId = '';
                    debugger;
                    if (data != '') {
                        SpltData = data.split(',');
                        spltBillableTermId = full.strBillableSettingsId.split(',');
                        for (var i = 0; i < spltBillableTermId.length; i++) {
                            for (var i = 0; i < SpltData.length; i++) {
                                if (SpltData.length == 1) {
                                    ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow; width:auto;" class="col-sm-6">' + SpltData[i] + '<a href="javascript:deleteAppliedTermFromUserGrid(' + full.UserId + ',' + spltBillableTermId[i] + ',' + full.RoleId + ',' + full.IsGlobalGST + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a>' + '</p>'
                                }
                                else {
                                    ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow; width:auto;" class="col-sm-3">' + SpltData[i] + '<a href="javascript:deleteAppliedTermFromUserGrid(' + full.UserId + ',' + spltBillableTermId[i] + ',' + full.RoleId + ',' + full.IsGlobalGST + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a>' + '</p>'
                                }
                            }
                        }
                        return ReturnData;
                    }
                }
            },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        //"createdRow": function (row, data, dataIndex) {
        //    if (data.IsEnable == false) {
        //        $('td', row).css('background-color', '#D2D2D2');
        //    }
        //},

        bServerSide: true,
        sAjaxSource: urlGetRoleWiseUserSAAS,

        "fnDrawCallback": function () {
            if ($('#chkAll').prop('checked') == true) {
                chkCount = $('#datatableUsersInRoles >tbody >tr').length;
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
            } else if ($("#datatableUsersInRoles").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }
        },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "RoleId", "value": RoleId });
            aoData.push({ "name": "UserName", "value": localStorage.getItem('SAASPM_UserName') });
            aoData.push({ "name": "RoleName", "value": localStorage.getItem('SAASPM_RoleID') });
            aoData.push({ "name": "TermCode", "value": localStorage.getItem('SE_GridTermCodeFilter') });
        }
    });
}

function DrawUsersGrid(RoleId) {
    if ($.fn.DataTable.isDataTable('#datatableUsers')) {
        $('#datatableUsers').DataTable().destroy();
    }
    $('#datatableUsers tbody').empty();
    $('#datatableUsers').DataTable({
        autoWidth: false,
        destory: true,
        retrieve: true,
        processing: true,
        serverSide: true,
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
            {
                'data': 'RoleId',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    if ($('#chkAll').prop('checked') == true) {
                        return '<input type="checkbox" id="chk_' + full.UserId + '" name="' + full.UserId + '" onclick="SetUserId(' + full.UserId + ')">';
                    }
                    else {
                        return '<input type="checkbox" id="chk_' + full.UserId + '" name="' + full.UserId + '" onclick="SetUserId(' + full.UserId + ')">';
                    }
                }
            },
            { 'data': 'Name' },
            { 'data': 'UserName' },
            { 'data': 'Email' },
            { 'data': 'Mobile' },
            { 'data': 'RoleName' },
            { 'data': 'UserTypeName' },
            {
                'data': 'TermCode',
                "render": function (data, type, full, meta) {
                    var SpltData = '';
                    var ReturnData = '';
                    var spltBillableTermId = '';
                    debugger;
                    if (data != '' && data != null) {
                        SpltData = data.split(',');
                        spltBillableTermId = full.strBillableSettingsId.split(',');
                        for (var i = 0; i < spltBillableTermId.length; i++) {
                            for (var i = 0; i < SpltData.length; i++) {
                                if (SpltData.length == 1) {
                                    ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow;width:100%;margin-bottom: 5px;" class="col-sm-6">' + SpltData[i] + '<a href="javascript:deleteAppliedTermFromUserGrid(' + full.UserId + ',' + spltBillableTermId[i] + ',' + full.RoleId + ',' + full.IsGlobalGST + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a>' + '</p></br>'
                                }
                                else {
                                    ReturnData += '<p style="border: 1px solid #dddd50; text-align: center;margin-right: 8px; background: lightyellow;width:100%;margin-bottom: 5px;" class="col-sm-3">' + SpltData[i] + '<a href="javascript:deleteAppliedTermFromUserGrid(' + full.UserId + ',' + spltBillableTermId[i] + ',' + full.RoleId + ',' + full.IsGlobalGST + ')" class="ic-edit" style="background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a>' + '</p></br>'
                                }
                            }
                        }
                        return ReturnData;
                    }
                    else {
                        return "";
                    }
                },
                "orderable": false,
            },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",

        bServerSide: true,
        sAjaxSource: urlGetAllRoleUserList,

        "fnDrawCallback": function () {
            if ($('#chkAll').prop('checked') == true) {
                chkCount = $('#datatableUsers >tbody >tr').length;
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
            } else if ($("#datatableUsers").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatableUsers').DataTable();
            var info = table.page.info();
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> Users Records(s) found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatableUsers >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b> Users Records(s) found';
            }
        },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "RoleName", "value": localStorage.getItem('SAASPM_UserRole') });
            aoData.push({ "name": "TermCode", "value": $("#txtTermCode").val() });
            aoData.push({ "name": "UserType", "value": localStorage.getItem('SAASPM_UserType') });
            aoData.push({ "name": "TermCode", "value": localStorage.getItem('SAASPM_TermCode') });
            aoData.push({ "name": "BillableCode", "value": localStorage.getItem('SAASPM_BillableCode') });
            aoData.push({ "name": "ResellerId", "value": localStorage.getItem('SAASPM_ResellerId') });
            aoData.push({ "name": "SolarCompany", "value": localStorage.getItem('SAASPM_SolarCompany') == "0" ? null : localStorage.getItem('SAASPM_SolarCompany') });
            aoData.push({ "name": "RoleID", "value": RoleId == undefined ? localStorage.getItem('SAASPM_RoleID') : RoleId });
        }
    });
}

$('#chkAll3').on('click', function () {
    var table = $('#datatableUsers').DataTable();
    var rows = table.rows({ 'search': 'applied' }).nodes();
    $('input[type="checkbox"]', rows).prop('checked', this.checked);
    chkCount = this.checked ? $('#datatableUsers >tbody >tr').length : 0;

    $("#datatableUsers input:checkbox").each(function () {
        var $this = $(this);
        if ($this.is(":checked")) {
            if ($this.attr("name") != 'select_all') {
                GlblUserId += $this.attr("name") + ',';
                GlblUserId.replace(/,\s*$/, ""); // remove comma from last
            }
        }
        else {
            GlblUserId = '';
        }
    });
});

function ApplyGlobalPricing() {
    if (GlblRoleId != '') { // Executed when apply to all from user role screen
        var GlobalTermId = $('#ddlBillableTerms').val();
        var RoleID = GlblRoleId.replace(/,\s*$/, "").split(',');
        var cntr = 1;
        for (var i = 0; i < GlobalTermId.length; i++) {
            for (var j = 0; j < RoleID.length; j++) {
                $.ajax({
                    type: "POST",
                    url: SaveUserBillableSettingsURL,
                    data: { RoleId: RoleID[j], GlobalTermId: GlobalTermId[i], Price: $("#chk_" + GlobalTermId[i]).attr('price'), IsGST: $("#chk_" + GlobalTermId[i]).attr('isglobalgst'), DATAOPMODE: 1 },
                    async: false,
                    success: function (response) {
                        $("#ddlBillableTerms").val(0);
                        $("#dvBillingDescription").hide();
                        $("#dvGlobalPricingButtons").hide();
                        DrawSAASPricingGrid();
                        if (cntr == GlobalTermId.length) {
                            alert('Global pricing terms applied sucessfully.');
                        }
                    }
                });
            }
            cntr++;
        }
        GlblRoleId = '';
        $("#ddlBillableTerms").multiselect("clearSelection");
    }
    else if (GlblUserId != '') { // Executed when apply to particular users from users in users in user role grid or user grid
        var GlobalTermId = $('#ddlBillableTerms').val();
        var UserID = GlblUserId.replace(/,\s*$/, "").split(',');
        var FinalPriceToSet = '';
        var IsGST = '';
        var Counter = 1;
        for (var i = 0; i < GlobalTermId.length; i++) {
            for (var j = 0; j < UserID.length; j++) {
                if (PricingLocalDataArr.length > 0) {
                    for (var k = 0; k < PricingLocalDataArr.length; k++) {
                        if (PricingLocalDataArr[k].GlobalTermId == GlobalTermId[i]) {
                            FinalPriceToSet = PricingLocalDataArr[k].Price;
                            IsGST = PricingLocalDataArr[k].IsGlobalGST;
                        }
                        else {
                            FinalPriceToSet = $("#chk_" + GlobalTermId[i]).attr('price');
                            IsGST = $("#chk_" + GlobalTermId[i]).attr('isglobalgst') == "1" ? true : false;
                        }
                    }
                }
                else {
                    FinalPriceToSet = $("#chk_" + GlobalTermId[i]).attr('price');
                    IsGST = $("#chk_" + GlobalTermId[i]).attr('isglobalgst') == "1" ? true : false;
                }
                $.ajax({
                    type: "POST",
                    url: SaveUserBillableSettingsURL,
                    data: { RoleId: 0, GlobalTermId: GlobalTermId[i], Price: FinalPriceToSet, IsGST: IsGST, DATAOPMODE: 2, UserId: UserID[j] },
                    async: false,
                    success: function (response) {
                        if (GrdRoleId != '') {
                            localStorage.setItem('SE_GridTermCodeFilter', '')
                            DrawUsersInUserRoleGrid(GrdRoleId);
                        }
                        if (GlblUserId != '') {
                            localStorage.setItem('SE_GridTermCodeFilter', '')
                            DrawUsersGrid();
                        }
                        PricingLocalDataArr = [];
                        $("#ddlBillableTerms").val(0);
                        $("#dvBillingDescription").hide();
                        $("#dvGlobalPricingButtons").hide();
                        if (Counter == UserID.length) {
                            alert('Global pricing terms applied sucessfully.');
                        }
                    }
                });
            }
            Counter++;
        }
        GlblUserId = '';
        $("#ddlBillableTerms").multiselect("clearSelection");
    }
    else {
        alert('Please select any record!.');
    }
}

function CancelGlobalPricing() {
    $("#ddlBillableTerms").val(0);
    $("#dvBillingDescription").hide();
    $("#dvGlobalPricingButtons").hide();
    $("#ddlBillableTerms").multiselect("clearSelection");
}

$('#btnOptions').click(function () {
    $('#hdnPopGlobalTermId').val($('#ddlBillableTerms').val());
    var UserId = 'hdnPopUserId';
    $('#popupmanageprice').modal({ backdrop: 'static', keyboard: false });
});

function PopSaveGlobalPricing() {
    if ($("#txtpopGlobalPrice").val() != '') {
        $.ajax({
            type: "POST",
            url: SaveGlobalBillableTermSAASURL,
            data: {
                Id: $('#ddlBillableTerms').val(),
                TermName: $('#ddlBillableTerms').find('option:selected').attr('TermName'),
                TermCode: $('#ddlBillableTerms').find('option:selected').attr('TermCode'),
                TermDescription: $('#ddlBillableTerms').find('option:selected').attr('Description'),
                GlobalPrice: $("#txtpopGlobalPrice").val(),
                IsGlobalGST: $('#IspopGlobalGST').is(":checked"),
                CreatedBy: LoggedInUserId,
                CreatedDate: Date.now
            },
            success: function (response) {
                $("#ddlBillableTerms").val(0);
                $("#dvBillingDescription").hide();
                $("#dvGlobalPricingButtons").hide();
                $("#popupmanageprice .close").click();
                if (GrdRoleId != '') { /* When changed from 1st tab then no GrdRoleId value is set as it is set on href link is called from user roles grid*/
                    DrawUsersInUserRoleGrid(GrdRoleId);
                }
                GrdRoleId = '';
                GlblUserId = '';
                DrawUsersGrid();
                BindGlobalPricingTerms();
                alert('Global pricing terms saved sucessfully.');
            }
        });
    }
    else {
        alert('Please enter global price.');
    }
}

$('#myTab a').on('click', function (e) {
    e.preventDefault();
    $(this).tab('show')
    if (this.innerHTML.toLowerCase() == 'users in user roles') {
        if (GrdRoleId != '') {
            $("#dvBtnOptions").show();
            DrawUsersInUserRoleGrid(GrdRoleId);
            GlblRoleId = '';
            GlblUserId = '';
        }
    }
    else if (this.innerHTML.toLowerCase() == 'users') {
        ActiveTab = 'Users';
        $(".userfilter").show();
        $(".rolefilter").hide();

        // Initialize select2
        $("#ResellerId").select2();

        $("#dvBtnOptions").show();
        DrawUsersGrid();
        GlblRoleId = '';
        GlblUserId = '';
    }
    else {
        ActiveTab = 'UserRole';
        $(".userfilter").hide();
        $(".rolefilter").show();
        $("#dvApplyGlobalPricing").show();
        //$("#dvBtnOptions").hide();
        DrawSAASPricingGrid();
        GlblUserId = '';
    }
})

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    e.target // newly activated tab
    e.relatedTarget // previous active tab
})

// Called when href link is called from user roles grid
function ShowUserInUserRoleGrid(RoleId) {
    GrdRoleId = RoleId;
    //DrawUsersInUserRoleGrid(RoleId);
    DrawUsersGrid(RoleId);
    //$("#dvApplyGlobalPricing").hide();
    $("#dvBtnOptions").show();
    $('#myTab li:nth-child(2) a').tab('show');
    //GlblRoleId = '';
}

function SetId(RoleId) {
    if ($("#chk_" + RoleId).prop('checked') == true) {
        GlblRoleId += RoleId + ',';
        GlblRoleId.replace(/,\s*$/, ""); // remove comma from last
    }
    else {
        if (GlblRoleId != '') {
            var spltdta = GlblRoleId.replace(/,\s*$/, "").split(',');
            //spltdta.replace(RoleId,'');
            spltdta.forEach(function (ele, idx) { // remove matching element
                if (spltdta[idx] == RoleId) {
                    spltdta[idx] = '';
                }
            })

            spltdta.forEach(function (ele, idx) { //prepare comma seprated string
                GlblRoleId = '';
                if (ele != '') {
                    GlblRoleId += ele + ',';
                }
                GlblRoleId.replace(/,\s*$/, ""); // remove comma from last
            })
        }
        //else {
        //    GlblRoleId = '';
        //}
    }
}

function SetUserId(UserId) {
    if ($("#chk_" + UserId).prop('checked') == true) {
        GlblUserId += UserId + ',';
        GlblUserId.replace(/,\s*$/, ""); // remove comma from last
    }
    else {
        if (GlblUserId != '') {
            var spltdta = GlblUserId.replace(/,\s*$/, "").split(',');
            //spltdta.replace(RoleId,'');
            spltdta.forEach(function (ele, idx) { // remove matching element
                if (spltdta[idx] == UserId) {
                    spltdta[idx] = '';
                }
            })

            GlblUserId = '';
            spltdta.forEach(function (ele, idx) { //prepare comma seprated string
                if (ele != '') {
                    GlblUserId += ele + ',';
                }
                GlblUserId.replace(/,\s*$/, ""); // remove comma from last
            })
        }
    }
}

function SearchUsers() {
    localStorage.setItem('SAASPM_UserName', $("#txtUserName").val());
    localStorage.setItem('SAASPM_RoleID', $("#RoleID").val());
    localStorage.setItem('SAASPM_UserType', $("#drpUserType").val());
    localStorage.setItem('SAASPM_TermCode', $("#txtTermCode").val());
    localStorage.setItem('SAASPM_BillableCode', $("#txtBillableCode").val());
    localStorage.setItem('SAASPM_ResellerId', $("#ResellerId").val());
    localStorage.setItem('SAASPM_SolarCompany', $("#hdnsolarCompanyid").val());

    if (ActiveTab == 'UserRole') {
        DrawSAASPricingGrid();
    }
    else {
        DrawUsersGrid();
    }
}

function ResetSearchFilters() {
    $("#drpUserType").val('');
    $("#txtBillableCode").val('');
    $("#txtTermCode").val('');
    $("#RoleID").val('');
    $("#txtUserName").val('');
    $("#txtCompanyName").val('');
    $("#txtTermName").val('');
    localStorage.setItem('SAASPM_UserName', '');
    localStorage.setItem('SAASPM_RoleID', '');
    localStorage.setItem('SAASPM_UserType', '');
    localStorage.setItem('SAASPM_TermCode', '');
    localStorage.setItem('SAASPM_BillableCode', '');
    localStorage.setItem('SAASPM_ResellerId', '');
    localStorage.setItem('SAASPM_SolarCompany', '');
    DrawSAASPricingGrid();
    DrawUsersGrid();
}

function deleteAppliedTermFromUserGrid(UserId, BillableSettingsId, RoleId, IsGST) {

    if (UserId != '' && BillableSettingsId != '') {
        $.ajax({
            type: "POST",
            url: SaveUserBillableSettingsURL,
            data: { RoleId: 0, GlobalTermId: 0, Price: 0, IsGST: IsGST, DATAOPMODE: 3, UserId, BillableSettingsId },
            success: function (response) {
                DrawUsersGrid();
                alert('Applied term deleted sucessfully.');
            },
            error: function (Error) {
            }
        });
    }
}

function deleteAppliedTermFromRoleGrid(RoleId, GlobalBillableTermId, Price, strBillableSettingsId) {
    if (RoleId != '' && GlobalBillableTermId != '' && Price != '') {
        $.ajax({
            type: "POST",
            url: SaveUserBillableSettingsURL,
            data: { RoleId: RoleId, GlobalTermId: GlobalBillableTermId, Price: Price.toFixed(2), DATAOPMODE: 4, IsGST: false, strBillableSettingsId: strBillableSettingsId },
            success: function (response) {
                DrawSAASPricingGrid();
                DrawUsersGrid();
                alert('Applied term deleted sucessfully.');
            },
            error: function (Error) {
            }
        });
    }
    else {
        alert("Error deleting applied term!");
    }
}

$('#btnpopupmanagepriceclose').click(function () {
    $('#IspopGlobalGST').attr('checked', false);
    $("#txtpopGlobalPrice").val('');
    $("#ddlTermCode").val(0);
});

function SavePricingLocally() {
    var textvalue = '';
    var SelectedValueText = '';
    var SplitValue1 = '';
    var MultiSelectedValue = '';
    var GlobalTermId = $("#ddlTermCode").val();
    var GloblPrice = $("#txtpopGlobalPrice").val();
    var IsGlobalGST = $('#IspopGlobalGST').is(":checked");
    $(".multiselect-container input[type=checkbox]").each(function () {
        if (this.value == GlobalTermId) {
            SelectedValueText = $("#chk_" + GlobalTermId).text();
            SplitValue1 = SelectedValueText.split('+')[1];
            textvalue = '$' + GloblPrice + '+' + SplitValue1;
        }
    });

    if (PricingLocalDataArr.length > 0) {
        $.each(PricingLocalDataArr, function (key, value) {
            if (value.GlobalTermId == GlobalTermId) {
                value.Price = GloblPrice;
                value.IsGlobalGST = IsGlobalGST;
            }
            else {
                PricingLocalDataArr.push({ GlobalTermId: GlobalTermId, Price: GloblPrice, IsGlobalGST: IsGlobalGST, NewDisplayValue: textvalue });
                return false;
            }
        });
    }
    else {
        PricingLocalDataArr.push({ GlobalTermId: GlobalTermId, Price: GloblPrice, IsGlobalGST: IsGlobalGST, NewDisplayValue: textvalue });
    }
    $("#popupmanageprice .close").click();

    BindGlobalPricingTerms(true, GlobalTermId, textvalue);
}

$('#ResellerId').change(function () {
    BindSolarCompany($('#ResellerId').val());
})

function getRole(userTypeId) {
    if (ProjectSessionUserTypeId == '1') {
        userTypeId = userTypeId;
    }
    else if (ProjectSessionUserTypeId == '2') {
        userTypeId = 5;
    }
    else if (ProjectSessionUserTypeId == '4' || ProjectSessionUserTypeId == '6')
        userTypeId = 8;

    $.ajax({
        url: urlGetRole,
        type: "get",
        data: { id: userTypeId },
        async: false,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data == 0) {
                $("#RoleID").empty();
                $("#RoleID").append('<option value="">' + "Select" + '</option>');
            }
            else {
                $("#RoleID").empty();
                $("#RoleID").append('<option value="" selected>' + "Select" + '</option>');

                $.each(data, function (i, role) {
                    $("#RoleID").append('<option value="' + role.Value + '">' +
                        role.Text + '</option>');
                });

            }
        },
    });
}