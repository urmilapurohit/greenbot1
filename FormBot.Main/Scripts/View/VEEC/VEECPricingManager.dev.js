$(document).ready(function () {

    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
        var IsResellerExist = false;
        $.ajax({
            type: 'get',
            url: urlGetResellerForPricingManager,
            dataType: 'json',
            data: '',
            async: false,
            success: function (reseller) {
                $.each(reseller, function (i, res) {
                    $("#ResellerId").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                    if (IsResellerExist == false && res.Value == localStorage.getItem('VEECPricingManager_ResellerId')) {
                        IsResellerExist = true;
                    }
                });

                if (IsResellerExist) {
                    //document.getElementById("ResellerId").value = localStorage.getItem('VEECPricingManager_ResellerId')
                    $("#ResellerId").val(localStorage.getItem('VEECPricingManager_ResellerId'));
                }
                else {
                    $("#ResellerId").val($("#ResellerId option:first").val());
                    localStorage.setItem('VEECPricingManager_ResellerId', $("#ResellerId option:first").val());
                }
                GetVEECGlobalPriceForReseller($("#ResellerId").val());
                GetRAMByResellerId($("#ResellerId").val());
                BindSolarCompany($("#ResellerId").val(), $("#RamId").val());
            },
            error: function (ex) {
                alert('Failed to retrieve Resellers.' + ex);
            }
        });
    }


    $('#searchSolarCompany').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnsolarCompanyid").val(0);
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

    if (searchTypeMode == 1) {
        $("#searchSolarCompany").val('');
        $("#hdnsolarCompanyid").val('');
        $('.searchTypeMode2').hide();
    }
    else {
        $("#hdnsolarCompanyid").val(localStorage.getItem('VEECPricingManager_SolarCompanyId'));
        $("#searchSolarCompany").val(localStorage.getItem('VEECPricingManager_SolarCompanyName'));
        $('.searchTypeMode2').show();
    }

    BindDataTable();

    $("#ResellerId").change(function () {
        if ($("#ResellerId").val() > 0) {
            GetVEECGlobalPriceForReseller($("#ResellerId").val());
            GetRAMByResellerId($("#ResellerId").val());
            BindSolarCompany($("#ResellerId").val(), 0);
        }
        else {
            $("#RamId").empty();
            $("#RamId").append('<option value="0">Select</option>');
        }
        BindDataTable();
        localStorage.setItem('VEECPricingManager_ResellerId', $("#ResellerId").val());
    })

    $("#RamId").change(function () {
        if ($("#RamId").val() > 0) {
            BindSolarCompany(0, $("#RamId").val());
        }
        else {
            BindSolarCompany($("#ResellerId").val(), 0);
        }
        BindDataTable();
        localStorage.setItem('VEECPricingManager_RamId', $("#RamId").val());
    })

    $("[name=optionsRadios]").click(function () {
        var $this = $(this);
        if (!$this.hasClass("checked")) {
            $("[name=optionsRadios]").removeClass("checked");
            $this.addClass("checked");
            searchTypeMode = $(this).val();
            if (searchTypeMode == 1) {
                $("#searchSolarCompany").val('');
                $("#hdnsolarCompanyid").val('');
                $('.searchTypeMode2').hide();
                if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 3) {
                    $("#dvVEECglobalprice").show();
                }
            }
            else {
                $("#hdnsolarCompanyid").val(localStorage.getItem('VEECPricingManager_SolarCompanyId'));
                $("#searchSolarCompany").val(localStorage.getItem('VEECPricingManager_SolarCompanyName'));
                $('.searchTypeMode2').show();
                if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 3) {
                    $("#dvVEECglobalprice").hide();
                }
            }
            ajaxurl = searchTypeMode == 1 ? urlGetSolarCompanyForPricingManager : urlGetVEECsForPricingManager;
            BindDataTable();
        }
    });

});

function GetVEECGlobalPriceForReseller(resellerId) {
    $.ajax({
        type: 'get',
        url: urlGetVEECGlobalPriceForReseller,
        dataType: 'json',
        async: false,
        data: { rId: resellerId },
        success: function (data) {
            $('#VEECoptiPaySpan').text('$' + parseFloat(data.price.OptiPay));
        },
        error: function (ex) {
            alert('Failed to retrieve Account Managers.' + ex);
        }
    });
    return false;
}

function GetRAMByResellerId(resellerId) {
    var IsRamExist = false;
    $("#RamId").empty();
    $.ajax({
        type: 'get',
        url: urlGetRAMForReseller,
        dataType: 'json',
        async: false,
        cache: false,
        data: { rId: resellerId },
        success: function (ramuser) {
            $("#RamId").append('<option value="0">Select</option>');
            $.each(ramuser, function (i, ram) {
                $("#RamId").append('<option value="' + ram.Value + '">' + ram.Text + '</option>');
                if (IsRamExist == false && ram.Value == localStorage.getItem('VEECPricingManager_RamId')) {
                    IsRamExist = true;
                }
            });

            if (IsRamExist) {
                //document.getElementById("RamId").value = localStorage.getItem('VEECPricingManager_RamId')
                $("#RamId").val(localStorage.getItem('VEECPricingManager_RamId'));
            }
            else {
                $("#RamId").val($("#RamId option:first").val());
                localStorage.setItem('VEECPricingManager_RamId', $("#RamId option:first").val());
            }
        },
        error: function (ex) {
            alert('Failed to retrieve Account Managers.' + ex);
        }
    });
    return false;
}

function BindSolarCompany(resellerID, ramID) {
    var IsCompanyExist = false;
    $("#searchSolarCompany").val("");
    var scurl = '';
    var searchid = '';
    if (ramID == 0) {
        scurl = urlGetSolarCompanyByResellerId;
        searchid = resellerID;
    }
    else {
        scurl = urlGetSolarCompanyByRAMID;
        searchid = ramID;
    }
    $.ajax({
        type: 'POST',
        url: scurl,
        dataType: 'json',
        async: false,
        data: { id: searchid },
        success: function (solarcompany) {
            solarCompanyList = [];
            $.each(solarcompany, function (i, company) {
                solarCompanyList.push({ value: company.Value, text: company.Text });
                if (IsCompanyExist == false && company.Value == localStorage.getItem('VEECPricingManager_SolarCompanyId')) {
                    IsCompanyExist = true;
                }
            });

            if (IsCompanyExist) {
                $('#hdnsolarCompanyid').val(localStorage.getItem('VEECPricingManager_SolarCompanyId'));
            }
            else {
                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('VEECPricingManager_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            if (localStorage.getItem('VEECPricingManager_SolarCompanyId') == '') {
                $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                localStorage.setItem('VEECPricingManager_SolarCompanyId', $('#hdnsolarCompanyid').val());
            }

            $.each(solarCompanyList, function (key, value) {
                if (value.value == localStorage.getItem('VEECPricingManager_SolarCompanyId')) {
                    $("#searchSolarCompany").val(value.text);
                    $('#hdnsolarCompanyid').val(localStorage.getItem('VEECPricingManager_SolarCompanyId'));
                    localStorage.setItem('VEECPricingManager_SolarCompanyName', $('#searchSolarCompany').val());
                }
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}

function ManageVEECGlobalPrice() {
    pricingType = 1

    $.ajax({
        type: 'get',
        url: url_ManagePrice,
        dataType: 'html',
        async: false,
        data: { PricingMode: searchTypeMode, PricingType: pricingType, ResellerId: $("#ResellerId").val(), solarcompany: $("#txtSolarCompany").val(), ram: $("#RamId").val(), name: $("#txtName").val() },
        success: function (response) {
            $("#dvVeecPriceManager").html(response);
            setTimeout(function () {
                $('#txtOptiPay').focus();
            }, 1000);
            $('#popupManageVEECPrice').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (ex) {
            alert('Failed to retrieve data.' + ex);
        }
    });

    //var getUrl = '@Url.Action("_ManagePrice", "PricingManager")' + '?PricingMode='+searchTypeMode+'&PricingType='+1+'&ResellerId='+Rid+'&solarcompany='+encodeURI($("#txtSolarCompany").val())+'&ram='+document.getElementById("RamId").value+'&name='+$("#txtName").val()+'';
    //$("#dvPriceManager").load(getUrl);
}

function ManageVEECCustomPrice() {

    pricingType = 2;

    $.ajax({
        type: 'get',
        url: url_ManagePrice,
        dataType: 'html',
        async: false,
        data: {
            PricingMode: searchTypeMode, PricingType: pricingType, ResellerId: $("#ResellerId").val(), solarcompany: $("#txtSolarCompany").val(), ram: $("#RamId").val(), name: $("#txtName").val(),
            OwnerName: $("#txtName").val(), SolarCompanyId: $('#hdnsolarCompanyid').val(), InstallationAddress: $("#txtInstallationAddress").val(), RefNumber: $("#txtVeecRef").val()
        },
        success: function (response) {
            $("#dvVeecPriceManager").html(response);
            setTimeout(function () {
                $('#txtOptiPay').focus();
            }, 1000);
            $('#popupManageVEECPrice').modal({ backdrop: 'static', keyboard: false });
        },
        error: function (ex) {
            alert('Failed to retrieve data.' + ex);
        }
    });
}

function MoveLtoR() {
    $('#lstLeftList option:selected').each(function () {
        $(this).remove().appendTo("#lstRightList");
    });
}

function MoveRtoL() {
    $('#lstRightList option:selected').each(function () {
        $(this).remove().appendTo("#lstLeftList");
    });
}

function BindDataTable() {
    //var sSize = document.getElementById("drpSystemSize").value;
    if ($('#datatable')) {
        var table = $('#datatable').DataTable();
        table.destroy();
    }
    $('#datatable').DataTable({
        iDisplayLength: 10,
        lengthMenu: ProjectConfiguration_GetPageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },

        columns: [
                {
                    'data': 'SolarCompany',
                    visible: searchTypeMode == 1 ? true : false
                },

                {
                    'data': 'Name',
                    visible: searchTypeMode == 1 ? true : false
                },

                {
                    'data': 'HomeOwnerName',
                    visible: searchTypeMode == 2 ? true : false,
                    "render": function (data, type, full, meta) {
                        return full.RefNumber + '-' + full.HomeOwnerName;
                    }
                },

                {
                    'data': 'HomeOwnerAddress',
                    visible: searchTypeMode == 2 ? true : false
                },

                { 'data': 'AccountManager' },
                {
                    'data': 'CompanyName',
                    visible: searchTypeMode == 2 ? true : false
                },

                 {
                     'data': 'LastTradedPrice', "sClass": "dt-right",
                     visible: searchTypeMode == 1 ? true : false,
                     "render": function (data, type, full, meta) {
                         return PrintDecimal(full.LastTradedPrice);
                     }
                 },

                {
                    'data': 'OptiPay', "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.OptiPay);
                    }
                },

                {
                    'data': 'strOfferExpires',
                    "render": function (data, type, full, meta) {
                        return ToDate(data);
                    }
                },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        initComplete: function (settings, json) {
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");
        },
        bServerSide: true,
        sAjaxSource: ajaxurl,

        "fnDrawCallback": function () {
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
            var recordType = searchTypeMode == 1 ? "Solar Company(s)" : "VEEC(s)";
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> ' + recordType + ' found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b> ' + recordType + ' found';
            }
            //------------------------------------------------------------------------------------------------------------------------------
        },

        "fnServerParams": function (aoData) {
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                aoData.push({ "name": "reseller", "value": $("#ResellerId").val() });
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 2) {
                aoData.push({ "name": "RAM", "value": $("#RamId").val() });
            }
            if (searchTypeMode == 1) {
                aoData.push({ "name": "solarcompany", "value": $("#searchSolarCompany").val() });
                aoData.push({ "name": "name", "value": $("#txtName").val() });
            }
            else {
                //aoData.push({ "name": "systemsize", "value": document.getElementById("drpSystemSize").value });
                aoData.push({ "name": "solarcompanyid", "value": $('#hdnsolarCompanyid').val() });
                aoData.push({ "name": "homeownername", "value": $("#txtName").val() });
                aoData.push({ "name": "veecRef", "value": $("#txtVeecRef").val() });
                aoData.push({ "name": "veecInstallationAddress", "value": $("#txtInstallationAddress").val() });
            }
        }
    });
}

function ToDate(data) {
    if (data != null) {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
       return moment(date).format(ProjectConfiguration_GetDateFormatToUpper);
    }
    else {
        return '';
    }
}

function ResetSearchFilters() {
    //$('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
    //$('#searchSolarCompany').val(solarCompanyList.length > 0 ? solarCompanyList[0].text : '');
    $('#hdnsolarCompanyid').val('');
    $('#searchSolarCompany').val('');
    $("#txtName").val('');
    localStorage.setItem('VEECPricingManager_SolarCompanyId', $('#hdnsolarCompanyid').val());
    localStorage.setItem('VEECPricingManager_SolarCompanyName', $('#searchSolarCompany').val());
    localStorage.setItem('VEECPricingManager_Name', '');

    if (searchTypeMode == 2) {
        $("#txtInstallationAddress").val('');
        $("#txtVeecRef").val('');
        localStorage.setItem('VEECPricingManager_InstallationAddress', '');
        localStorage.setItem('VEECPricingManager_VeecRef', '');
    }
    var table = $('#datatable').DataTable();
    table.destroy();
    BindDataTable();
}

function SearchRecords() {
    localStorage.setItem('VEECPricingManager_SolarCompanyId', $('#hdnsolarCompanyid').val());
    localStorage.setItem('VEECPricingManager_SolarCompanyName', $('#searchSolarCompany').val());
    localStorage.setItem('VEECPricingManager_Name', $("#txtName").val());

    if (searchTypeMode == 2) {
        localStorage.setItem('VEECPricingManager_InstallationAddress', $("#txtInstallationAddress").val());
        localStorage.setItem('VEECPricingManager_VeecRef', $("#txtVeecRef").val());
    }
    var table = $('#datatable').DataTable();
    table.destroy();
    BindDataTable();
}

function VEECOptiPayOptions() {
    $('#popupVEECOptipayOptions').modal({ backdrop: 'static', keyboard: false });
    //$('#popupVEECOptipayOptions').find('form').css('display', "block");
}

function CloseVeecOptiPayParameters() {
    $('#popupVEECOptipayOptions').modal('hide');
}

function SaveVEECPrice() {
    $.validator.unobtrusive.parse("#VEECPricingManagerDetails");

    var selectedItemArray = [];
    var lstSelecteditems = '';
    var expirydate = '';
    var saveUrl = '';
    var scID = '', rsId = '';

    if ($("#VEECPricingManagerDetails").valid()) {
        if (pricingType == 2) {
            $("#lstRightList option").each(function (i) {
                selectedItemArray.push($(this).val().trim());
            });
            lstSelecteditems = selectedItemArray.join(',');
            expirydate = GetDates($("#txtVEECExpiryDate").val());
        }

        if (searchTypeMode == 1) {
            saveUrl = urlSaveVEECPriceForSolarCompany;
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                rsId = $("#ResellerId").val();
            }
        }
        else {
            saveUrl = urlSaveVEECPriceForVEEC;  
        }

        var OptiPay = $('#txtVEECOptiPay').val();

        $.ajax({
            url: saveUrl,
            type: "GET",
            data: { PricingType: pricingType, items: lstSelecteditems, expiryDate: expirydate, ResellerId: rsId, solarCompanyId: scID = $("#hdnsolarCompanyid").val(), optiPay: $("#txtVEECOptiPay").val() },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $('#popupManageVEECPrice').modal('toggle');

                    $('#VEECoptiPaySpan').text((OptiPay == null || OptiPay == '') ? '$' + '0' : '$' + parseFloat(OptiPay));

                    $(".alert").hide();
                    $("#errorMsgRegion").removeClass("alert-danger");
                    $("#errorMsgRegion").addClass("alert-success");
                    $("#errorMsgRegion").html(closeButton + "Price saved successfully.");
                    $("#errorMsgRegion").show();

                    $("#datatable").dataTable().fnDraw();
                }
            },
        });
    }
    else {
        return false;
    }
}

function GetDates(date) {
    if (date != '') {
        var tickStartDate = ConvertDateToTick(date, ProjectConfiguration_GetDateFormat);
        return moment(tickStartDate).format("YYYY-MM-DD");
    }
    else {
        return '';
    }
}



