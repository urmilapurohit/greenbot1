$(document).ready(function () {
    //$("#btnPanelBrandadd").click(function () {
    //    debugger;
    //    ClearPanelModelBrand();
    //    $('#popupboxPanelBrand').modal({ backdrop: 'static', keyboard: false });
    //    MakePanelModelNumDisabled();
    //    BindPanelModel('', BasicDetails_JobType, 'Model', null, '');
    //    // BindPanelBrand('', BasicDetails_JobType, 'Brand', null, '');
    //});

    //$("#btnInverterBrandAdd").click(function () {
    //    ClearInverterModelBrand();
    //    $('#popupboxInverterBrand').modal({ backdrop: 'static', keyboard: false });
    //    MakeInverterModelSeriesDisabled();
    //    BindInverterModel('');
    //    //BindInverterBrand('', '');
    //});

    //$("#btnBatteryManufactureAdd").click(function () {
    //    ClearBatteryManufacturer();
    //    $('#popupboxBatteryManufacturer').modal({ backdrop: 'static', keyboard: false });
    //    MakeBatteryModelNumberDisabled();
    //    BindBatteryManufacturer();
    //});

    //$("#btnSystemBrandAdd").click(function () {
    //    ClearSystemModelBrand();
    //    $('#popupboxSystemBrand').modal({ backdrop: 'static', keyboard: false });
    //    MakeSystemModelSeriesDisabled();
    //    BindSystemModel('', BasicDetails_JobType, 'Model', null, '');
    //    // BindSystemBrand('', BasicDetails_JobType, 'Brand', null, '');
    //});

    $("#btnClosepopupboxPanelBrand").click(function () {
        $('#popupboxPanelBrand').modal('toggle');
    });

    $("#btnClosepopupboxInverterBrand").click(function () {
        $('#popupboxInverterBrand').modal('toggle');
    });

    $("#btnClosepopupboxBatteryManufacturer").click(function () {
        $('#popupboxBatteryManufacturer').modal('toggle');
    });

    $("#btnClosepopupboxSystemBrand").click(function () {
        $('#popupboxSystemBrand').modal('toggle');
    });

    $('#btnAddPanel').click(function () {
        $("#loading-image").css("display", "");
        var rowId = $("#hdnPanelRowId").val();
        AddPanelBrandSave(rowId);
    });

    $('#btnAddSystem').click(function () {
        $("#loading-image").css("display", "");
        var rowId = $("#hdnSystemRowId").val();
        AddSystemBrandSave(rowId);
    });

    $('#btnAddInverter').click(function () {
        $("#loading-image").css("display", "");
        var rowId = $("#hdnInverterRowId").val();
        AddInverterBrandSave(rowId);
    });

    $('#btnAddBatteryManfacturer').click(function () {
        $("#loading-image").css("display", "");
        var rowId = $("#hdnBatteryManufacturerRowId").val();
        AddBatteryManufacturerModelSave(rowId);
    });

    $("#resetPanelBrand").click(function () {
        var panelRowId = $("#hdnPanelRowId").val();
        ClearPanelModelBrand();
        $("#hdnPanelRowId").val(panelRowId);
    });

    $("#resetSystemBrand").click(function () {
        var systemRowId = $("#hdnSystemRowId").val();
        ClearSystemModelBrand();
        $("#hdnSystemRowId").val(systemRowId);
        $("#JobSystemDetails_InstallationType").val('');
    });

    $("#resetInverterBrand").click(function () {
        var inverterRowId = $("#hdnInverterRowId").val();
        ClearInverterModelBrand();
        $("#hdnInverterRowId").val(inverterRowId);
    });

    $("#resetBatteryManfacturer").click(function () {
        var batteryManufacturerRowId = $("#hdnBatteryManufacturerRowId").val();
        ClearBatteryManufacturer();
        $("#hdnBatteryManufacturerRowId").val(batteryManufacturerRowId);
    });

    $("#searchPanelBrand").change(function () {
        if ($("#searchPanelBrand").val() == "" || $("#searchPanelBrand").val() == null) {
            $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
            $("#JobPanelDetails_NoOfPanel").val('');
        }
        var panelBrand = $("#searchPanelBrand").val();
        var panelModel = $("#searchPanelModel").val();
        FillStartEndDateForPanel(panelBrand, panelModel);
    });

    $("#searchSystemBrand").change(function () {
        //if ($("#searchSystemBrand").val() == "" || $("#searchSystemBrand").val() == null) {
        //    MakeSystemModelSeriesDisabled();
        //}
        //$("#txtSWHpanelStartDate").val('');
        //$("#txtSWHpanelEndDate").val('');
        if ($("#searchSystemBrand").val() == "" || $("#searchSystemBrand").val() == null) {
            $("#JobSystemDetails_NoOfPanel").prop("disabled", true);
            $("#JobSystemDetails_NoOfPanel").val('');
        }
        var SWHpanelBrand = $("#searchSystemBrand").val();
        var SWHpanelModel = $("#searchSystemModel").val();
        FillStartEndDateForSWHBrandModel(SWHpanelBrand, SWHpanelModel);
    });

    $("#searchInverterBrand").change(function () {
        if ($("#searchInverterBrand").val() == "" || $("#searchInverterBrand").val() == null) {
            $("#searchInverterNumber").prop("disabled", true);
            $("#searchInverterNumber").val('');

        }
        var inverterBrand = $("#searchInverterBrand").val();
        var inverterSeries = $("#searchInverterSeries").val();
        var inverterModel = $("#searchInverterModel").val();
        FillStartEndDateForInverter(inverterBrand, inverterModel, inverterSeries);


        //if ($("#searchInverterBrand").val() == "" || $("#searchInverterBrand").val() == null) {
        //    MakeInverterModelSeriesDisabled();
        //}
        //$("#txtInverterStartDate").val('');
        //$("#txtInverterEndDate").val('');
    });
    $("#searchInverterModel").change(function () {
        if ($("#searchInverterModel").val() == "" || $("#searchInverterModel").val() == null) {
            MakeInverterModelSeriesDisabled();
        }
        $("#txtInverterStartDate").val('');
        $("#txtInverterEndDate").val('');
        //if ($("#searchInverterModel").val() == "" || $("#searchInverterModel").val() == null) {
        //    $("#searchInverterNumber").prop("disabled", true);
        //    $("#searchInverterNumber").val('');

        //}
        //var inverterBrand = $("#searchInverterBrand").val();
        //var inverterSeries = $("#searchInverterSeries").val();
        //var inverterModel = $("#searchInverterModel").val();
        //FillStartEndDateForInverter(inverterBrand, inverterModel, inverterSeries);
    });
    $("#searchBatteryManufacturer").change(function () {
        if ($("#searchBatteryManufacturer").val() == "" || $("#searchBatteryManufacturer").val() == null) {
            MakeBatteryModelNumberDisabled();
        }
        EnableDisableBatteryManufacturerDetails();
    });
    $("#searchBatteryModelNumber").change(function () {
        if ($("#searchBatteryModelNumber").val() == "" || $("#searchBatteryModelNumber").val() == null) {
            EnableDisableBatteryManufacturerDetails();
        }
    });

    $("#searchPanelModel").change(function () {
        debugger;
        if ($("#searchPanelModel").val() == "" || $("#searchPanelModel").val() == null) {

            MakePanelModelNumDisabled();

        }
        $("#txtpanelStartDate").val('');
        $("#txtpanelEndDate").val('');

    });
    //$("#searchPanelModel").click(function () {
    //    debugger;
    //    var panelBrand = $("#searchPanelBrand").val();
    //    var panelModel = $("#searchPanelModel").val();
    //    FillStartEndDateForPanel(panelBrand, panelModel);
    //});

    $("#searchSystemModel").change(function () {
        if ($("#searchSystemModel").val() == "" || $("#searchSystemModel").val() == null) {
            MakeSystemModelSeriesDisabled();
        }
        $("#txtSWHpanelStartDate").val('');
        $("#txtSWHpanelEndDate").val('');

    });

    $("#searchInverterSeries").change(function () {
        if ($("#searchInverterSeries").val() == "" || $("#searchInverterSeries").val() == null) {
            $("#searchInverterModel").prop("disabled", true);
            $("#searchInverterModel").val('');
            $("#hdnInverterModel").val('');
        }
    });

    $("#btnPopupSerialNumber").click(function () {
        PopupSerialNumber();
    });

});

function CalculateSTC() {
    $("#JobSystemDetails_CalculatedSTC").val('');
    $("#JobSystemDetails_CalculatedSTCForSWH").val();
    var JobType = $("#BasicDetails_JobType").val();
    var systemsize = $("#JobSystemDetails_SystemSize").val();

    var installationDate = $("#BasicDetails_strInstallationDate").val();
    var expectedInstallDate = "";
    if (installationDate != null && installationDate != undefined && installationDate != '') {
        var installDate = ConvertDateToTick(installationDate, ProjectConfiguration_GetDateFormat);
        expectedInstallDate = moment(installDate).format("YYYY-MM-DD");
    }

    var deemingPeriod = $("#JobSTCDetails_DeemingPeriod").val();
    var postcode = InstallationPostcodeFromjson;
    var systemBrand = $("#hdnSystemBrand").val();
    var systemModel = $("#hdnSystemModel").val();

    if (JobType == 1) {
        if (expectedInstallDate != null && expectedInstallDate != "" && deemingPeriod != null && deemingPeriod != "" && postcode != null && postcode != "" && systemsize != null && systemsize != "") {
            $.ajax(
                {
                    url: urlCalculateSTC,
                    data: { sguType: 'SolarDeemed', expectedInstallDate: expectedInstallDate, deemingPeriod: deemingPeriod, postcode: postcode, systemsize: systemsize },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        if (data.toLowerCase() == 'false') {
                            $("#spanJobSystemDetails_CalculatedSTC").show().text("STC Value cannot be calculated since REC website is down.").fadeOut(6000);
                        }
                        else {
                            var obj = JSON.parse(data);
                            if (obj.status == "Failed") {
                                $("#spanJobSystemDetails_CalculatedSTC").show().text(obj.fieldErrors[0].defaultMessage);
                            }
                            else {
                                var numberOfStcs = obj;
                                $("#JobSystemDetails_CalculatedSTC").val(numberOfStcs);
                            }
                        }
                    }
                });
        }
        else {
            $("#spanJobSystemDetails_CalculatedSTC").show().text("Installation Date, STC DeemingPeriod,Installation postcode and System size are required for it.").fadeOut(6000);
        }
    }

    else if (JobType == 2) {
        if (expectedInstallDate != null && expectedInstallDate != "" && expectedInstallDate != undefined && postcode != null && postcode != "" && postcode != undefined && systemBrand != null && systemBrand != "" && systemModel != null && systemModel != "") {
            $.ajax(
                {
                    url: urlCalculateSWHSTC,
                    data: { expectedInstallDate: expectedInstallDate, postcode: postcode, systemBrand: systemBrand, systemModel: systemModel },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        var obj = JSON.parse(data);
                        if (obj.status == "Failed") {
                            $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text(obj.errors[0]);
                        }
                        else {
                            var numberOfStcs = obj.result.numStc;
                            $("#JobSystemDetails_CalculatedSTCForSWH").val(numberOfStcs);
                        }
                    }
                });
        }
        else {
            $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text('Installation Date,System brand,System Model,Owner postcode are required for it.');
        }
    }
    else {
        $("#spanJobSystemDetails_CalculatedSTC").show().text("Please select Job Type.").fadeOut(6000);
        $("#spanJobSystemDetails_CalculatedSTCForSWH").show().text("Please select Job Type.").fadeOut(6000);
    }
}

function PopupSerialNumber() {
    $('#serialNumberPopup').html('');
    var SerialNumberText = $("#JobSystemDetails_SerialNumbers").val().trim();
    var data = JSON.stringify({ "serialNumbers": $("#JobSystemDetails_SerialNumbers").val(), "jobID": $("#BasicDetails_JobID").val() });
    $.ajax({
        url: urlCheckDuplicateSerialNumbers,
        type: "post",
        dataType: "json",
        data: data,
        cache: false,
        async: true,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.length > 0) {
                MasterSerialArray = [];
                JobSerialArray = [];
                var SetialArray = $.parseJSON(data);
                $.grep(SetialArray, function (e, i) {
                    if (!e.IsExistInMaster) { MasterSerialArray.push(e.ExistsSerialNumber); }
                    else { JobSerialArray.push(e.ExistsSerialNumber); }
                });

                var lines = SerialNumberText.split("\n");
                lines = lines.filter(function (n) { return n.length > 0 });
                for (var lineCount = 1; lineCount < lines.length + 1; lineCount++) {
                    if (JobSerialArray.indexOf(lines[lineCount - 1]) !== -1) {
                        $('#serialNumberPopup').append("<li><span style='background-color: rgba(30, 121, 204, 0.21);'>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                    }
                    else if (MasterSerialArray.indexOf(lines[lineCount - 1]) !== -1) {
                        $('#serialNumberPopup').append("<li><span style='background-color: rgba(220, 82, 109, 0.21);'>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                    }
                    else {
                        $('#serialNumberPopup').append("<li><span>" + lineCount + ' :' + lines[lineCount - 1] + "</span></li>");
                    }
                }

            } else {
                MasterSerialArray = [];
                JobSerialArray = [];
            }
            $('#popupSerialNumber').modal({ backdrop: 'static', keyboard: false });
        },
    });
}

function FillDropDownOfPanelBrandModel() {
    debugger;
    $('#searchPanelBrand').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnPanelBrand").val('');
            $.each(panelBrandList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchPanelBrand").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            debugger;
            $("#hdnPanelBrand").val(ui.item.id);

            $("#JobPanelDetails_NoOfPanel").prop("disabled", false);
            // $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
            $("#JobPanelDetails_NoOfPanel").val('');
            //BindPanelModel('', BasicDetails_JobType, 'cer', $("#hdnPanelBrand").val());
            // FillStartEndDateForPanel(encodeURIComponent(ui.item.value));
            FillDropdownSupplierByManufacturer(encodeURIComponent(ui.item.value));
            var panelModel = $("#searchPanelModel").val();
            FillStartEndDateForPanel(ui.item.value, panelModel);

        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchPanelBrand").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#searchPanelModel').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnPanelModel").val('');
            $.each(panelModelList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchPanelModel").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            debugger;
            $("#JobPanelDetails_NoOfPanel").prop("disabled", false);
            $("#searchPanelBrand").prop("disabled", false);
            $("#hdnPanelModel").val(ui.item.id);
            //var panelModel = $("#searchPanelModel").val();
            BindPanelBrand('', BasicDetails_JobType, 'Brand', $("#hdnPanelModel").val());
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchPanelModel").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
    $('#tblPanel tr').each(function () {
        debugger;
        var PanelBrand = $(this).find("td").eq(0).attr("class");
        var PanelModel = $(this).find("td").eq(1).attr("class");
        var NoOfPanel = $(this).find("td").eq(4).attr("class");
        var trId = $(this).find("td").eq(5).attr("rowid");
        var PanelSupplier = $(this).find("td").eq(5).attr("data-supplier");
        debugger;
        PanelXml.push({ ID: trId, Brand: PanelBrand, Model: PanelModel, Count: NoOfPanel, Supplier: PanelSupplier });
        OldPanelXml.push({ ID: trId, Brand: PanelBrand, Model: PanelModel, Count: NoOfPanel, Supplier: PanelSupplier });
    });
}

function FillDropDownOfSystemBrandModel() {
    $('#searchSystemBrand').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnSystemBrand").val('');
            $.each(systemBrandList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchSystemBrand").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            debugger;
            $("#hdnSystemBrand").val(ui.item.id);
            $("#searchSystemModel").prop("disabled", false);
            $("#JobSystemDetails_NoOfPanel1").prop("disabled", false);
            $("#JobSystemDetails_NoOfPanel1").val('');
            // var SWHpanelBrand = $("#searchSystemBrand").val();
            var SWHpanelModel = $("#searchSystemModel").val();
            FillStartEndDateForSWHBrandModel(ui.item.value, SWHpanelModel);
            //BindSystemModel('', BasicDetails_JobType, 'cer', $("#hdnSystemBrand").val());
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchSystemBrand").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#searchSystemModel').autocomplete({
        minLength: 0,
        source: function (request, response) {
            debugger;
            var data = [];
            $("#hdnSystemModel").val('');
            $.each(systemModelList, function (key, value) {
                debugger;
                if (value.text.toLowerCase().indexOf($("#searchSystemModel").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }

            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#JobSystemDetails_NoOfPanel1").prop("disabled", false);
            $("#searchSystemBrand").prop("disabled", false);
            $("#hdnSystemModel").val(ui.item.id);
            BindSystemBrand('', BasicDetails_JobType, 'Brand', $("#hdnSystemModel").val());

        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchSystemModel").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#tblSystem tr').each(function () {
        var PanelBrand = $(this).find("td").eq(0).attr("class");
        $("#hdnSystemBrand").val(PanelBrand);
        var PanelModel = $(this).find("td").eq(1).attr("class");
        $("#hdnSystemModel").val(PanelModel);
        var NoOfPanel = $(this).find("td").eq(4).attr("class");
        var trId = $(this).find("td").eq(5).attr("rowid");
        systemXml.push({ ID: trId, Brand: PanelBrand, Model: PanelModel, Count: NoOfPanel });
    });

    ShowHideAddSystemButton();
}

function FillDropDownOfInverterBrandModel() {
    $('#searchInverterBrand').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnInverterBrand").val('');
            $.each(inverterBrandList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchInverterBrand").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {

            $("#hdnInverterBrand").val(ui.item.id);

            $("#searchInverterSeries").prop("disabled", false);
            //  $("#searchInverterModel").prop("disabled", true);
            //  $("#searchInverterModel").val('');
            //  $("#hdnInverterModel").val('');


            $("#searchInverterNumber").prop('disabled', false);
            var inverterBrand = ui.item.value;
            var inverterSeries = $("#searchInverterSeries").val();
            var inverterModel = $("#searchInverterModel").val();
            BindInverterSeries('', inverterModel);
            // FillStartEndDateForInverter(inverterBrand, inverterModel, inverterSeries);

        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchInverterBrand").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#searchInverterSeries').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnInverterSeries").val('');
            $.each(inverterSeriesList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchInverterSeries").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnInverterSeries").val(ui.item.id);
            $("#searchInverterNumber").prop('disabled', false);
            var inverterBrand = $("#searchInverterBrand").val();
            var inverterSeries = ui.item.value;
            var inverterModel = $("#searchInverterModel").val();
            FillStartEndDateForInverter(inverterBrand, inverterModel, inverterSeries);
            //  $("#searchInverterModel").prop("disabled", false);
            // BindInverterModel('');

        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchInverterSeries").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#searchInverterModel').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnInverterModel").val('');
            $.each(inverterModelList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchInverterModel").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            debugger;
            $("#hdnInverterModel").val(ui.item.id);
            $("#searchInverterBrand").prop("disabled", false);
            // $("#searchInverterNumber").prop('disabled', false);
            var inverterBrand = $("#searchInverterBrand").val();
            var inverterSeries = $("#searchInverterSeries").val();
            var inverterModel = ui.item.value;
            BindInverterBrand('', '', ui.item.value);
            // FillStartEndDateForInverter(inverterBrand, inverterModel, inverterSeries);
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchInverterModel").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#tblInverter tr').each(function () {
        debugger;
        //$("#gridInverter").show();
        var InverterBrand = $(this).find("td").eq(0).attr("class");
        var series = $(this).find("td").eq(1).attr("class");
        var InverterModel = $(this).find("td").eq(2).attr("class");
        var InverterQuantity = $(this).find("td").eq(5).attr("class");
        var trId = $(this).find("td").eq(6).attr("rowid");
        InverterXml.push({ ID: trId, Brand: InverterBrand, Series: series, Model: InverterModel, NoOfInverter: InverterQuantity });
        OldInverterXml.push({ ID: trId, Brand: InverterBrand, Series: series, Model: InverterModel, NoOfInverter: InverterQuantity });

    });


}

function FillDropDownOfBatteryManufacturerModel() {
    $('#searchBatteryManufacturer').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnBatteryManufacturer").val('');
            $.each(batteryManufacturerList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchBatteryManufacturer").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnBatteryManufacturer").val(ui.item.id);
            $("#searchBatteryModelNumber").prop("disabled", false);
            BindBatteryModelNumber('');
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchBatteryManufacturer").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#searchBatteryModelNumber').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnBatteryModelNumber").val('');
            $.each(batteryModelList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchBatteryModelNumber").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnBatteryModelNumber").val(ui.item.id);
            EnableDisableBatteryManufacturerDetails();
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchBatteryModelNumber").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        ul.addClass("autocompleteChangeUser");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };

    $('#tblBatteryManufacturer tr').each(function () {
        var batteryManufacturer = $(this).find("td").eq(0).attr("class");
        var batteryModel = $(this).find("td").eq(1).attr("class");
        var trId = $(this).find("td").eq(2).attr("rowid");
        //batteryXml.push({ ID: trId, batteryManufacturer: batteryManufacturer, batteryModel: batteryModel });
        batteryXml.push({ JobBatteryManufacturerId: trId, Manufacturer: batteryManufacturer, ModelNumber: batteryModel });
    });
    $('#popupboxPanelBrand').on('hidden.bs.modal', function () {
        $(".job-panel-supplier-popup-drpsupplier").hide();
    });
}

function BindPanelBrand(JobPanelDetailsBrand, jobType, mode, CertificateHolder, supplier = null) {
    debugger;
    $("#searchPanelBrand").val("");
    $.ajax({
        type: 'get',
        url: urlGetPanelBrand + mode + '&CertificateHolder=' + CertificateHolder + '&JobType=' + jobType,
        dataType: 'json',
        success: function (brands) {
            panelBrandList = [];
            $.each(brands, function (i, brand) {
                panelBrandList.push({ value: brand.Value, text: brand.Text });
            });

            if (JobPanelDetailsBrand != '') {
                $('#hdnPanelBrand').val(JobPanelDetailsBrand);
                $.each(panelBrandList, function (key, value) {
                    if (value.value == JobPanelDetailsBrand) {
                        $("#searchPanelBrand").val(value.text);

                        //BindPanelModel(JobPanelDetailsModel, jobType, 'cer', $('#hdnPanelBrand').val());
                    }
                });
            }
            else {
                $('#hdnPanelBrand').val('');
                $("#searchPanelBrand").val('');
            }
            FillDropdownSupplierByManufacturer($("#searchPanelBrand").val(), supplier);

        },
        error: function (ex) {
            alert('Failed to retrieve panel brand.' + ex);
        }
    });
    return false;
}

function BindSystemBrand(JobSystemDetailsSystemBrand, jobType, mode, CertificateHolder, JobSystemDetailsModel) {

    $("#searchSystemBrand").val("");
    $.ajax({
        type: 'get',
        url: urlGetPanelBrand + mode + '&CertificateHolder=' + CertificateHolder + '&JobType=' + jobType,
        dataType: 'json',
        success: function (brands) {


            systemBrandList = [];
            $.each(brands, function (i, brand) {
                systemBrandList.push({ value: brand.Value, text: brand.Text });
            });

            if (JobSystemDetailsSystemBrand != '') {
                $('#hdnSystemBrand').val(JobSystemDetailsSystemBrand);
                $.each(systemBrandList, function (key, value) {
                    if (value.value == JobSystemDetailsSystemBrand) {
                        $("#searchSystemBrand").val(value.text);
                        //BindSystemModel(JobSystemDetailsModel, jobType, 'Model', $('#hdnSystemBrand').val());
                    }
                });
            }
            else {
                $('#hdnSystemBrand').val('');
                $("#searchSystemBrand").val('');
            }

        },
        error: function (ex) {
            alert('Failed to retrieve system brand.' + ex);
        }
    });
    return false;
}

function BindPanelModel(JobPanelDetailsModel, jobType, mode, CertificateHolder, supplier = null) {
    debugger;
    $("#searchPanelModel").val("");
    $.ajax({
        type: 'get',
        url: urlGetPanelModel + mode + '&CertificateHolder=' + escape(CertificateHolder) + '&JobType=' + jobType,
        dataType: 'json',
        success: function (models) {
            panelModelList = [];
            $.each(models, function (i, model) {
                panelModelList.push({ value: model.Value, text: model.Text });
            });
            debugger;
            if (JobPanelDetailsModel != '') {
                $('#hdnPanelModel').val(JobPanelDetailsModel);
                $.each(panelModelList, function (key, value) {
                    if (value.value == JobPanelDetailsModel) {
                        $("#searchPanelModel").val(value.text);
                        BindPanelBrand(CertificateHolder, jobType, 'Brand', $('#hdnPanelModel').val(), supplier);
                    }
                });
            }
            else {
                $('#hdnPanelModel').val('');
                $("#searchPanelModel").val('');
            }
            //$("#txtpanelStartDate").val('');
            //$("#txtpanelEndDate").val('');
        },
        error: function (ex) {
            alert('Failed to retrieve panel model.' + ex);
        }
    });
    return false;
}

function BindSystemModel(JobSystemDetailsModel, jobType, mode, CertificateHolder) {
    $("#searchSystemModel").val("");
    $.ajax({
        type: 'get',
        url: urlGetPanelModel + mode + '&CertificateHolder=' + escape(CertificateHolder) + '&JobType=' + jobType,
        dataType: 'json',
        success: function (models) {
            systemModelList = [];
            $.each(models, function (i, model) {
                systemModelList.push({ value: model.Value, text: model.Text });
            });

            if (JobSystemDetailsModel != '') {
                $('#hdnSystemModel').val(JobSystemDetailsModel);
                $.each(systemModelList, function (key, value) {
                    if (value.value == JobSystemDetailsModel) {
                        $("#searchSystemModel").val(value.text);
                        BindSystemBrand(CertificateHolder, jobType, 'Brand', $('#hdnSystemModel').val());
                    }
                });
            }
            else {
                $('#hdnSystemModel').val('');
                $("#searchSystemModel").val('');
            }
        },
        error: function (ex) {
            alert('Failed to retrieve system model.' + ex);
        }
    });
    return false;
}

function BindInverterSeries(JobInverterDetailsSeries, JobInverterDetailsModel) {

    $("#searchInverterSeries").val("");
    $.ajax({
        type: 'get',
        url: urlGetInverterModel + encodeURIComponent($('#hdnInverterModel').val()) + '&Manufacturer=' + encodeURIComponent($('#hdnInverterBrand').val()),
        dataType: 'json',
        success: function (series) {
            inverterSeriesList = [];
            $.each(series, function (i, sr) {
                inverterSeriesList.push({ value: sr.Value, text: sr.Text });
            });

            if (JobInverterDetailsSeries != '') {
                $('#hdnInverterSeries').val(JobInverterDetailsSeries);
                $.each(inverterSeriesList, function (key, value) {
                    if (value.value == JobInverterDetailsSeries) {
                        $("#searchInverterSeries").val(value.text);
                        //  BindInverterBrand('', JobInverterDetailsSeries, JobInverterDetailsModel);
                        // BindInverterModel(JobInverterDetailsModel);
                    }
                });
            }
            else {
                $('#hdnInverterSeries').val('');
                $("#searchInverterSeries").val('');
            }
        },
        error: function (ex) {
            alert('Failed to retrieve inverter series.' + ex);
        }
    });
    return false;
}

function BindInverterModel(JobInverterDetailsModel, JobInverterBrand, JobInverterSeries, JobInverterCount) {
    debugger;
    $("#searchInverterModel").val("");
    $.ajax({
        type: 'get',
        url: urlGetInverterSeries + encodeURIComponent($("#hdnInverterSeries").val()) + '&Manufacturer=' + encodeURIComponent($("#hdnInverterBrand").val()),
        dataType: 'json',
        success: function (models) {

            inverterModelList = [];
            $.each(models, function (i, model) {
                inverterModelList.push({ value: model.Value, text: model.Text });
            });

            if (JobInverterDetailsModel != '') {
                $('#hdnInverterModel').val(JobInverterDetailsModel);
                $.each(inverterModelList, function (key, value) {
                    if (value.value == JobInverterDetailsModel) {
                        $("#searchInverterModel").val(value.text);
                        BindInverterBrand(JobInverterBrand, JobInverterSeries, JobInverterDetailsModel, JobInverterCount);
                    }
                });
            }
            else {
                $('#hdnInverterModel').val('');
                $("#searchInverterModel").val('');
            }
        },
        error: function (ex) {
            alert('Failed to retrieve inverter model.' + ex);
        }
    });
    return false;
}

function BindBatteryModelNumber(JobBatteryModelNumber) {
    $("#searchBatteryModelNumber").val("");
    $.ajax({
        type: 'get',
        url: urlGetBatteryModel + $("#hdnBatteryManufacturer").val(),
        async: false,
        dataType: 'json',
        success: function (models) {

            batteryModelList = [];
            $.each(models, function (i, model) {
                batteryModelList.push({ value: model.Value, text: model.Text });
            });

            if (JobBatteryModelNumber != '') {
                $('#hdnBatteryModelNumber').val(JobBatteryModelNumber);
                $.each(batteryModelList, function (key, value) {
                    if (value.value == JobBatteryModelNumber) {
                        $("#searchBatteryModelNumber").val(value.text);
                    }
                });
            }
            else {
                $('#hdnBatteryModelNumber').val('');
                $("#searchBatteryModelNumber").val('');
            }
        },
        error: function (ex) {
            alert('Failed to retrieve battery model number.' + ex);
        }
    });
    return false;
}

function BindInverterBrand(JobInverterDetailsBrand, JobInverterDetailsSeries, JobInverterDetailsModel, JobInverterQuantity) {
    debugger;
    $("#searchInverterBrand").val("");
    $.ajax({
        type: 'get',
        url: urlGetInverterBrand + encodeURIComponent($('#hdnInverterModel').val()),
        dataType: 'json',
        success: function (brands) {
            inverterBrandList = [];
            $.each(brands, function (i, brand) {
                inverterBrandList.push({ value: brand.Value, text: brand.Text });
            });
            debugger;
            if (JobInverterDetailsBrand != '') {
                $('#hdnInverterBrand').val(JobInverterDetailsBrand);
                $.each(inverterBrandList, function (key, value) {
                    if (value.value == JobInverterDetailsBrand) {
                        $("#searchInverterNumber").val(JobInverterQuantity);
                        $("#searchInverterBrand").val(value.text);
                        // BindInverterModel(JobInverterDetailsModel);
                        BindInverterSeries(JobInverterDetailsSeries, JobInverterDetailsModel);
                        //  BindInverterSeries(JobInverterDetailsSeries, JobInverterDetailsModel)
                    }
                });
            }
            else {
                $('#hdnInverterBrand').val('');
                $("#searchInverterBrand").val('');
            }
            //$("#txtInverterStartDate").val('');
            //$("#txtInverterEndDate").val('');
        },
        error: function (ex) {
            alert('Failed to retrieve inverter brand.' + ex);
        }
    });
    return false;
}

function BindBatteryManufacturer(JobBatteryManufacturer, JobBatteryModelNumber) {
    $("#searchBatteryManufacturer").val("");
    $.ajax({
        type: 'get',
        url: urlGetBatteryManufacturer,
        async: false,
        dataType: 'json',
        success: function (manufacturers) {
            batteryManufacturerList = [];
            $.each(manufacturers, function (i, manufacturer) {
                batteryManufacturerList.push({ value: manufacturer.Value, text: manufacturer.Text });
            });

            if (JobBatteryManufacturer != '') {
                $('#hdnBatteryManufacturer').val(JobBatteryManufacturer);
                $.each(batteryManufacturerList, function (key, value) {
                    if (value.value == JobBatteryManufacturer) {
                        $("#searchBatteryManufacturer").val(value.text);
                        BindBatteryModelNumber(JobBatteryModelNumber)
                    }
                });
            }
            else {
                $('#hdnBatteryManufacturer').val('');
                $("#searchBatteryManufacturer").val('');
            }

        },
        error: function (ex) {
            alert('Failed to retrieve battery manufacturer.' + ex);
        }
    });
    return false;
}

function AddPanelBrandSave(rowId) {

    var installationdate = $("#BasicDetails_strInstallationDate").val();
    var panelStartDate = $("#txtpanelStartDate").val();
    var panelEndDate = $("#txtpanelEndDate").val();
    var newPanelenddate = new Date(panelEndDate.split('/')[2] + "/" + panelEndDate.split('/')[1] + "/" + panelEndDate.split('/')[0]);
    var newPanelstartdate = new Date(panelStartDate.split('/')[2] + "/" + panelStartDate.split('/')[1] + "/" + panelStartDate.split('/')[0]);
    var newinstallationdate = new Date(installationdate.split('/')[2] + "/" + installationdate.split('/')[1] + "/" + installationdate.split('/')[0]);
    if (newinstallationdate >= newPanelenddate || newinstallationdate <= newPanelstartdate) {
        //alert("The selected Panel brand and model was not accredited at the time of installation(InstallationDate)-" + installationdate);

        if (confirm('The selected Panel brand and model was not accredited at the time of installation - InstallationDate: ' + installationdate + ' \nDo you still want to continue?')) {
            SavePanelBrandModel(rowId);
        }

        else {
            // $('#popupboxPanelBrand').modal('toggle');
            $("#loading-image").css("display", "none");
            return true;
        }
    }
    else {
        SavePanelBrandModel(rowId);
    }

}
function SavePanelBrandModel(rowId) {
    var panelBrand = $("#hdnPanelBrand").val();
    var panelModel = $("#hdnPanelModel").val();
    var panelStartDate = $("#txtpanelStartDate").val();
    var panelEndDate = $("#txtpanelEndDate").val();
    var noOfPanel = $("#JobPanelDetails_NoOfPanel").val();
    var drpSupplier = $("#drpSupplier").val();
    if ($("#drpSupplier option").not("option[value='']").length > 0 && (drpSupplier == "")) {
        $("#spanJobPanelDetails_Supplier").show();
        $("#loading-image").css("display", "none");
        return false;
    }
    else {
        if ((panelBrand != null && panelBrand != "") && (panelModel != null && panelModel != "") &&
            (noOfPanel != null && noOfPanel != "" && noOfPanel != 0)
            && (panelStartDate != null && panelStartDate != "")
            && (panelEndDate != null && panelEndDate != "")
        ) {
            if (rowId == null || rowId == "") {
                var trId = Fuid();
                var trID = trId;
                var count = 1;
                var content = "<tr id='" + trID + "'>"
                content += '<td class="' + panelBrand + '"  width="40%">' + panelBrand + ' </td>';
                content += '<td class="' + panelModel + '"  width="40%">' + panelModel + ' </td>';
                content += '<td class="' + panelStartDate + '"  width="7%">' + panelStartDate + ' </td>';
                content += '<td class="' + panelEndDate + '"  width="7%">' + panelEndDate + ' </td>';
                content += '<td class="' + noOfPanel + '"  width="7%" >' + noOfPanel + ' </td>';
                content += '<td  class=action id="tdAction" data-supplier="' + drpSupplier + '" rowid="' + trID + '"  width="15%"><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editPanel(\'' + trID + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeletePanelFromGrid(\'' + trID + '\')"></a></td>';
                content += "</tr>"
                $('#tblPanel').append(content);
                //$('#gridPanel').show();
                var panel = 0;
                if ($("#JobSystemDetails_NoOfPanel").text() != '') {
                    panel = $("#JobSystemDetails_NoOfPanel").text();
                }
                $("#JobSystemDetails_NoOfPanel").text(parseInt(panel) + parseInt(noOfPanel));
                debugger;
                PanelXml.push({ ID: trID, Brand: panelBrand, Model: panelModel, Count: noOfPanel, Supplier: drpSupplier });
            }
            else {
                $.each(PanelXml, function (i, e) {
                    debugger;
                    if (e.ID == rowId) {
                        e.Brand = panelBrand;
                        e.Model = panelModel;
                        e.Count = noOfPanel;
                        e.Supplier = drpSupplier;
                    }
                });

                $('#tblPanel tr').each(function () {
                    debugger;
                    var numberOfPanel = $(this).find("td").eq(4).attr("class");
                    var trId = $(this).find("td").eq(5).attr("rowid");
                    if (trId == rowId) {
                        $(this).find("td").eq(0).attr("class", panelBrand);
                        $(this).find("td").eq(0).text(panelBrand);
                        $(this).find("td").eq(1).attr("class", panelModel);
                        $(this).find("td").eq(1).text(panelModel);
                        $(this).find("td").eq(2).attr("class", panelStartDate);
                        $(this).find("td").eq(2).text(panelStartDate);
                        $(this).find("td").eq(3).attr("class", panelEndDate);
                        $(this).find("td").eq(3).text(panelEndDate);
                        $(this).find("td").eq(4).attr("class", noOfPanel);
                        $(this).find("td").eq(4).text(noOfPanel);
                        $(this).find("td").eq(5).attr("data-supplier", drpSupplier);

                        $("#JobSystemDetails_NoOfPanel").text(parseInt($("#JobSystemDetails_NoOfPanel").text()) - numberOfPanel);
                        $("#JobSystemDetails_NoOfPanel").text(parseInt($("#JobSystemDetails_NoOfPanel").text()) + parseInt(noOfPanel));
                    }
                });
            }

            $('#popupboxPanelBrand').modal('toggle');
            $("#loading-image").css("display", "none");
        }
        else {
            $("#loading-image").css("display", "none");
            if (panelBrand == null || panelBrand.trim() == "") {
                $("#spanJobPanelDetails_Brand").show();
            }

            if (panelModel == null || panelModel.trim() == "") {
                $("#spanJobPanelDetails_Model").show();
            }

            if (noOfPanel == null || noOfPanel.trim() == "" || noOfPanel == "0") {
                $("#spanNoOfPanelValue").show();
            }

            if (drpSupplier == null || drpSupplier.trim() == "" || drpSupplier == 0) {
                $("#spanJobPanelDetails_Supplier").show();
            }

            if (panelStartDate == null || panelStartDate.trim() == "") {
                $("#spanJobPanelDetails_CECApprovedDate").show();
            }

            if (panelEndDate == null || panelEndDate.trim() == "") {
                $("#spanJobPanelDetails_ExpiryDate").show();
            }
        }
    }

}
function AddSystemBrandSave(rowId) {
    debugger;
    var installationdate = $("#BasicDetails_strInstallationDate").val();
    var swhpanelStartDate = $("#txtSWHpanelStartDate").val();
    var swhpanelEndDate = $("#txtSWHpanelEndDate").val();
    var newPanelenddate = new Date(swhpanelEndDate.split('/')[2] + "/" + swhpanelEndDate.split('/')[1] + "/" + swhpanelEndDate.split('/')[0]);
    var newPanelstartdate = new Date(swhpanelStartDate.split('/')[2] + "/" + swhpanelStartDate.split('/')[1] + "/" + swhpanelStartDate.split('/')[0]);
    var newinstallationdate = new Date(installationdate.split('/')[2] + "/" + installationdate.split('/')[1] + "/" + installationdate.split('/')[0]);
    if (newinstallationdate >= newPanelenddate || newinstallationdate <= newPanelstartdate) {
        // alert("The selected Panel brand and model was not accredited at the time of installation(InstallationDate)-" + installationdate);
        if (confirm('The selected Panel brand and model was not accredited at the time of installation - InstallationDate: ' + installationdate + ' \nDo you still want to continue?')) {
            saveSystemBrandModel(rowId);
        }
        else {
            $("#loading-image").css("display", "none");
            return true;
        }
    }
    else {
        saveSystemBrandModel(rowId);
    }


}
function saveSystemBrandModel(rowId) {
    var systemBrand = $("#hdnSystemBrand").val();
    var systemModel = $("#hdnSystemModel").val();
    var swhpanelStartDate = $("#txtSWHpanelStartDate").val();
    var swhpanelEndDate = $("#txtSWHpanelEndDate").val();
    var noOfPanel = $("#JobSystemDetails_NoOfPanel1").val();
    if ((systemBrand != null && systemBrand != "") && (systemModel != null && systemModel != "") && (noOfPanel != null && noOfPanel != "" && noOfPanel != 0)
        && (swhpanelStartDate != null && swhpanelStartDate != "") && (swhpanelEndDate != null && swhpanelEndDate != "")
    ) {

        if (rowId == null || rowId == "") {
            var trId = Fuid();
            var trID = trId;
            var count = 1;
            var content = "<tr id='" + trID + "'>"
            content += '<td class="' + systemBrand + '"  width="40%">' + systemBrand + ' </td>';
            content += '<td class="' + systemModel + '"  width="35%">' + systemModel + ' </td>';
            content += '<td class="' + swhpanelStartDate + '"  width="10%" >' + swhpanelStartDate + ' </td>';
            content += '<td class="' + swhpanelEndDate + '"  width="10%" >' + swhpanelEndDate + ' </td>';
            content += '<td class="' + noOfPanel + '"  width="10%" >' + noOfPanel + ' </td>';
            content += '<td  class=action rowid="' + trID + '" id="tdAction"  width="15%"><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editSystem(\'' + trID + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeleteSystemFromGrid(\'' + trID + '\')"></a></td>';
            content += "</tr>"
            $('#tblSystem').append(content);
            var panel = 0;
            if ($("#JobSystemDetails_NoOfPanel").text() != '') {
                panel = $("#JobSystemDetails_NoOfPanel").text();
            }
            $("#JobSystemDetails_NoOfPanel").text(parseInt(panel) + parseInt(noOfPanel));
            systemXml.push({ ID: trID, Brand: systemBrand, Model: systemModel, Count: noOfPanel });
        }
        else {
            $.each(systemXml, function (i, e) {
                if (e.ID == rowId) {
                    e.Brand = systemBrand;
                    e.Model = systemModel;
                    e.Count = noOfPanel;
                    e.startDate = swhpanelStartDate;
                    e.endDate = swhpanelEndDate;
                }
            });

            $('#tblSystem tr').each(function () {
                var numberOfPanel = $(this).find("td").eq(4).attr("class");
                var trId = $(this).find("td").eq(5).attr("rowid");
                if (trId == rowId) {
                    $(this).find("td").eq(0).attr("class", systemBrand);
                    $(this).find("td").eq(0).text(systemBrand);
                    $(this).find("td").eq(1).attr("class", systemModel);
                    $(this).find("td").eq(1).text(systemModel);
                    $(this).find("td").eq(2).attr("class", swhpanelStartDate);
                    $(this).find("td").eq(2).text(swhpanelStartDate);
                    $(this).find("td").eq(3).attr("class", swhpanelEndDate);
                    $(this).find("td").eq(3).text(swhpanelEndDate);
                    $(this).find("td").eq(4).attr("class", noOfPanel);
                    $(this).find("td").eq(4).text(noOfPanel);

                    $("#JobSystemDetails_NoOfPanel").text(parseInt($("#JobSystemDetails_NoOfPanel").text()) - numberOfPanel);
                    $("#JobSystemDetails_NoOfPanel").text(parseInt($("#JobSystemDetails_NoOfPanel").text()) + parseInt(noOfPanel));
                }
            });
        }

        $('#popupboxSystemBrand').modal('toggle');
        $("#loading-image").css("display", "none");
    }
    else {
        $("#loading-image").css("display", "none");

        if (systemBrand == null || systemBrand.trim() == "") {
            $("#spanJobSystemDetails_Brand").show();
        }

        if (systemModel == null || systemModel.trim() == "") {
            $("#spanJobSystemDetails_Model").show();
        }

        if (noOfPanel == null || noOfPanel.trim() == "" || noOfPanel == "0") {
            $("#spanJobSystemDetails_NoOfPanel").show();
        }

        if (swhpanelStartDate == null || swhpanelStartDate.trim() == "") {
            $("#spanJobSystemDetails_CECApprovedDate").show();
        }

        if (swhpanelEndDate == null || swhpanelEndDate.trim() == "") {
            $("#spanJobSystemDetails_ExpiryDate").show();
        }
    }
}

function AddInverterBrandSave(rowId) {
    debugger;

    var inverterStartDate = $("#txtInverterStartDate").val();
    var inverterEndDate = $("#txtInverterEndDate").val();

    var installationdate = $("#BasicDetails_strInstallationDate").val();
    var newInverterEnddate = new Date(inverterEndDate.split('/')[2] + "/" + inverterEndDate.split('/')[1] + "/" + inverterEndDate.split('/')[0]);
    var newInverterStartDate = new Date(inverterStartDate.split('/')[2] + "/" + inverterStartDate.split('/')[1] + "/" + inverterStartDate.split('/')[0]);
    var newinstallationdate = new Date(installationdate.split('/')[2] + "/" + installationdate.split('/')[1] + "/" + installationdate.split('/')[0]);
    if (newinstallationdate >= newInverterEnddate || newinstallationdate <= newInverterStartDate) {
        /*alert("The selected Inverter brand and model was not accredited at the time of installation(InstallationDate) - " + installationdate);*/
        if (confirm('The selected Inverter brand and model was not accredited at the time of installation - InstallationDate: ' + installationdate + ' \nDo you still want to continue?')) {
            saveInverterBrandModel(rowId);
        }
        else {
            $("#loading-image").css("display", "none");
            return true;
        }
    }
    else {
        saveInverterBrandModel(rowId);
    }



}
function saveInverterBrandModel(rowId) {
    var inverterBrand = $("#hdnInverterBrand").val();
    var inverterModel = $("#hdnInverterModel").val();
    var inverterStartDate = $("#txtInverterStartDate").val();
    var inverterEndDate = $("#txtInverterEndDate").val();
    var series = $("#hdnInverterSeries").val();
    var noOfInverter = $("#searchInverterNumber").val();
    if ((inverterBrand != null && inverterBrand != "") && (inverterModel != null && inverterModel != "") &&
        (series != null && series != "") && (noOfInverter != null && noOfInverter != "" && noOfInverter != 0)
        && (inverterStartDate != null && series != "") && (inverterEndDate != null && inverterEndDate != "")
    ) {
        if (rowId == null || rowId == "") {
            var trId = Fuid();
            var count = 1;
            var content = "<tr id='+ trId +'>"
            content += '<td class="' + inverterBrand + '"  width="30%">' + inverterBrand + ' </td>';
            content += '<td class="' + series + '"  width="28%">' + series + ' </td>';
            content += '<td class="' + inverterModel + '" width="28%">' + inverterModel + ' </td>';
            content += '<td class="' + inverterStartDate + '" width="5%">' + inverterStartDate + ' </td>';
            content += '<td class="' + inverterEndDate + '" width="5%">' + inverterEndDate + ' </td>';
            content += '<td class="' + noOfInverter + '" width="5%">' + noOfInverter + ' </td>';
            content += '<td  class=action rowid="' + trId + '" id="tdAction"  width="5%" ><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editInverter(\'' + trId + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeleteInverterFromGrid(\'' + trId + '\')"></a></td>';
            content += "</tr>"
            $('#tblInverter').append(content);
            //$('#gridInverter').show();
            var inverter = 0;
            if ($("#JobSystemDetails_NoOfInverter").text() != '') {
                inverter = $("#JobSystemDetails_NoOfInverter").text();
            }
            $("#JobSystemDetails_NoOfInverter").text(parseInt(inverter) + parseInt(noOfInverter));

            InverterXml.push({ ID: trId, Brand: inverterBrand, Model: inverterModel, Series: series, NoOfInverter: noOfInverter });
        }
        else {
            $.each(InverterXml, function (i, e) {
                if (e.ID == rowId) {
                    e.Brand = inverterBrand;
                    e.Model = inverterModel;
                    e.Series = series;
                    e.NoOfInverter = noOfInverter;
                }
            });
            $('#tblInverter tr').each(function () {
                var numberOfInverter = $(this).find("td").eq(5).attr("class");
                var trId = $(this).find("td").eq(6).attr("rowid");
                if (trId == rowId) {
                    $(this).find("td").eq(0).attr("class", inverterBrand);
                    $(this).find("td").eq(0).text(inverterBrand);
                    $(this).find("td").eq(1).attr("class", series);
                    $(this).find("td").eq(1).text(series);
                    $(this).find("td").eq(2).attr("class", inverterModel);
                    $(this).find("td").eq(2).text(inverterModel);
                    $(this).find("td").eq(3).attr("class", inverterStartDate);
                    $(this).find("td").eq(3).text(inverterStartDate);
                    $(this).find("td").eq(4).attr("class", inverterEndDate);
                    $(this).find("td").eq(4).text(inverterEndDate);
                    $(this).find("td").eq(5).attr("class", noOfInverter);
                    $(this).find("td").eq(5).text(noOfInverter);

                    $("#JobSystemDetails_NoOfInverter").text(parseInt($("#JobSystemDetails_NoOfInverter").text()) - numberOfInverter);
                    $("#JobSystemDetails_NoOfInverter").text(parseInt($("#JobSystemDetails_NoOfInverter").text()) + parseInt(noOfInverter));
                }
            });
        }

        $('#popupboxInverterBrand').modal('toggle');
        $("#loading-image").css("display", "none");
    }
    else {
        $("#loading-image").css("display", "none");
        if (inverterBrand == null || inverterBrand.trim() == "")
            $("#spanJobInverterDetails_Brand").show();

        if (inverterModel == null || inverterModel.trim() == "")
            $("#spanJobInverterDetails_Model").show();

        if (series == null || series.trim() == "")
            $("#spanJobInverterDetails_Series").show();

        if (noOfInverter.trim() == 0 || noOfInverter.trim() == "")
            $("#spanNoOfInverterValue").show();

        if (inverterStartDate == null || inverterStartDate.trim() == "")
            $("#spanJobInverterDetails_CECApprovedDate").show();

        if (inverterEndDate == null || inverterEndDate.trim() == "")
            $("#spanJobInverterDetails_ExpiryDate").show();
    }
}

function AddBatteryManufacturerModelSave(rowId) {
    var batteryManufacturer = $("#hdnBatteryManufacturer").val();
    var batteryModel = $("#hdnBatteryModelNumber").val();
    var batterySystemPartOfAnAggregatedControl = $('#JobSTCDetails_batterySystemPartOfAnAggregatedControl').val();
    var changedSettingOfBatteryStorageSystem = $('#JobSTCDetails_changedSettingOfBatteryStorageSystem').val();

    if ((batteryManufacturer != null && batteryManufacturer != "") && (batteryModel != null && batteryModel != "") && batterySystemPartOfAnAggregatedControl != "" && changedSettingOfBatteryStorageSystem != "") {
        if (rowId == null || rowId == "") {
            var trId = Fuid();
            var count = 1;
            var content = "<tr id='+ trId +'>"
            content += '<td class="' + batteryManufacturer + '"  width="35%">' + batteryManufacturer + ' </td>';
            content += '<td class="' + batteryModel + '" width="25%">' + batteryModel + ' </td>';
            content += '<td  class=action rowid="' + trId + '" id="tdAction"  width="15%" ><a class="edit sprite-img" title="Edit" id="signEdit" style="cursor: pointer" href="javascript:void(0)" onclick="editBatteryManufacturer(\'' + trId + '\')"></a>&nbsp;&nbsp;<a class="delete sprite-img" title="Delete" id="signDelete" style="cursor: pointer" onclick="DeleteBatteryManufacturerFromGrid(\'' + trId + '\')"></a></td>';
            content += "</tr>"
            $('#tblBatteryManufacturer').append(content);
            batteryXml.push({ JobBatteryManufacturerId: trId, Manufacturer: batteryManufacturer, ModelNumber: batteryModel });
        }
        else {
            $.each(batteryXml, function (i, e) {
                if (e.JobBatteryManufacturerId == rowId) {
                    e.Manufacturer = batteryManufacturer;
                    e.ModelNumber = batteryModel;
                }
            });
            $('#tblBatteryManufacturer tr').each(function () {
                var trId = $(this).find("td").eq(2).attr("rowid");
                if (trId == rowId) {
                    $(this).find("td").eq(0).attr("class", batteryManufacturer);
                    $(this).find("td").eq(0).text(batteryManufacturer);
                    $(this).find("td").eq(1).attr("class", batteryModel);
                    $(this).find("td").eq(1).text(batteryModel);
                }
            });
        }

        $('#popupboxBatteryManufacturer').modal('toggle');
        $("#loading-image").css("display", "none");
    }
    else {
        $("#loading-image").css("display", "none");
        if (batteryManufacturer == null || batteryManufacturer.trim() == "")
            $("#spanJobBatteryManufacturer").show();

        if (batteryModel == null || batteryModel.trim() == "")
            $("#spanJobBatteryModelNumber").show();

        if (batterySystemPartOfAnAggregatedControl == "")
            $("#spanBatterySystemPartOfAnAggregatedControl").show();

        if (changedSettingOfBatteryStorageSystem == "")
            $("#spanChangedSettingOfBatteryStorageSystem").show();
    }
}

function ClearPanelModelBrand() {
    $('#hdnPanelBrand').val('');
    $("#searchPanelBrand").val('');
    $('#hdnPanelModel').val('');
    $("#searchPanelModel").val('');
    //$("#searchPanelModel").prop('disabled', true);
    $("#searchPanelBrand").prop('disabled', true);
    $("#JobPanelDetails_NoOfPanel").val('');
    $("#JobPanelDetails_NoOfPanel").prop('disabled', true);
    $("#spanJobPanelDetails_Brand").hide();
    $("#spanJobPanelDetails_Model").hide();
    $("#spanJobPanelDetails_Supplier").hide();
    $("#spanNoOfPanelValue").hide();
    $("#hdnPanelRowId").val('');
    $("#txtpanelStartDate").val('');
    $("#txtpanelEndDate").val('');

}

function ClearInverterModelBrand() {
    $('#hdnInverterBrand').val('');
    $("#searchInverterBrand").val('');
    $('#hdnInverterSeries').val('');
    $("#searchInverterSeries").val('');
    $("#searchInverterSeries").prop('disabled', true)
    $('#hdnInverterModel').val('');
    $("#searchInverterModel").val('');
    //$("#searchInverterModel").prop('disabled', true)
    $("#spanJobInverterDetails_Brand").hide();
    $("#spanJobInverterDetails_Series").hide();
    $("#spanJobInverterDetails_Model").hide();
    $("#hdnInverterRowId").val('');
    $("#searchInverterNumber").val('');
    $("#searchInverterNumber").prop('disabled', true)
    $("#spanNoOfInverterValue").hide();
    $("#txtInverterStartDate").val('');
    $("#txtInverterEndDate").val('');

}

function ClearBatteryManufacturer() {
    $('#hdnBatteryManufacturer').val('');
    $("#searchBatteryManufacturer").val('');
    $('#hdnBatteryModelNumber').val('');
    $("#searchBatteryModelNumber").val('');
    $("#searchBatteryModelNumber").prop('disabled', true);
    $("#spanJobBatteryManufacturer").hide();
    $("#spanJobBatteryModelNumber").hide();
    $("#spanBatterySystemPartOfAnAggregatedControl").hide();
    $("#spanChangedSettingOfBatteryStorageSystem").hide();
    $("#hdnBatteryManufacturerRowId").val('');
    EnableDisableBatteryManufacturerDetails();
}

function ClearSystemModelBrand() {
    $('#hdnSystemBrand').val('');
    $("#searchSystemBrand").val('');
    $('#hdnSystemModel').val('');
    $("#searchSystemModel").val('');
    $("#JobSystemDetails_NoOfPanel1").val('');
    $("#spanJobSystemDetails_Brand").hide();
    $("#spanJobSystemDetails_Model").hide();
    $("#spanJobSystemDetails_NoOfPanel").hide();
    $("#hdnSystemRowId").val('');
    $("#txtSWHpanelStartDate").val('');
    $("#txtSWHpanelEndDate").val('');

}

function editPanel(trId) {
    debugger;
    ClearPanelModelBrand();
    $('#popupboxPanelBrand').modal({ backdrop: 'static', keyboard: false });
    $.each(PanelXml, function (i, e) {
        if (e.ID == trId) {
            $("#searchPanelBrand").prop("disabled", false);
            $("#JobPanelDetails_NoOfPanel").prop("disabled", false);
            BindPanelModel(e.Model, BasicDetails_JobType, 'Model', e.Brand, e.Supplier);
            //BindPanelBrand(e.Brand, BasicDetails_JobType, 'Brand', null, e.Model);
            $("#JobPanelDetails_NoOfPanel").val(e.Count);
            $("#hdnPanelRowId").val(trId);
            FillDropdownSupplierByManufacturer(e.Brand, e.Supplier);

            FillStartEndDateForPanel(e.Brand, e.Model);
        }
    });
}

function editSystem(trId) {
    ClearSystemModelBrand();
    $('#popupboxSystemBrand').modal({ backdrop: 'static', keyboard: false });
    $.each(systemXml, function (i, e) {
        debugger;
        if (e.ID == trId) {
            $("#searchSystemBrand").prop("disabled", false);
            $("#JobSystemDetails_NoOfPanel1").prop("disabled", false);
            //BindSystemBrand(e.Brand, BasicDetails_JobType, 'Brand', null, e.Model);
            BindSystemModel(e.Model, BasicDetails_JobType, 'Model', e.Brand, null);
            $("#JobSystemDetails_NoOfPanel1").val(e.Count);
            $("#hdnSystemRowId").val(trId);
            FillStartEndDateForSWHBrandModel(e.Brand, e.Model);
        }
    });
}

function editInverter(trId) {
    ClearInverterModelBrand();
    $('#popupboxInverterBrand').modal({ backdrop: 'static', keyboard: false });
    $.each(InverterXml, function (i, e) {
        if (e.ID == trId) {
            $("#searchInverterSeries").prop("disabled", false);
            $("#searchInverterModel").prop("disabled", false);
            $("#searchInverterNumber").prop("disabled", false);
            // BindInverterBrand(e.Brand, e.Series, e.Model, e.NoOfInverter);
            BindInverterModel(e.Model, e.Brand, e.Series, e.NoOfInverter);
            $("#searchInverterNumber").val(e.NoOfInverter);
            $("#hdnInverterRowId").val(trId);
            FillStartEndDateForInverter(e.Brand, e.Model, e.Series);
        }
    })
}

function editBatteryManufacturer(trId) {
    ClearBatteryManufacturer();
    $('#popupboxBatteryManufacturer').modal({ backdrop: 'static', keyboard: false });
    $.each(batteryXml, function (i, e) {
        if (e.JobBatteryManufacturerId == trId) {
            $("#searchBatteryManufacturer").prop("disabled", false);
            $("#searchBatteryModelNumber").prop("disabled", false);
            BindBatteryManufacturer(e.Manufacturer, e.ModelNumber);
            $("#hdnBatteryManufacturerRowId").val(trId);
        }
    })
    EnableDisableBatteryManufacturerDetails();
}

function DeletePanelFromGrid(trId) {
    if (confirm('Are you sure you want to delete this panel?')) {
        debugger;
        PanelXml = $.grep(PanelXml, function (e, i) {
            if (e.ID == trId) {
                $("#JobSystemDetails_NoOfPanel").text(parseInt($("#JobSystemDetails_NoOfPanel").text()) - parseInt(e.Count));
                document.getElementById("tblPanel").deleteRow(i);
                return false;
            } else { return true; }
        });
    }
}

function DeleteSystemFromGrid(trId) {
    if (confirm('Are you sure you want to delete this system detail?')) {
        systemXml = $.grep(systemXml, function (e, i) {
            if (e.ID == trId) {
                $("#JobSystemDetails_NoOfPanel").text(0);
                document.getElementById("tblSystem").deleteRow(i);
                return false;
            } else { return true; }
        });
        ShowHideAddSystemButton();
    }
}

function DeleteInverterFromGrid(trId) {
    debugger;
    if (confirm('Are you sure you want to delete this Inverter?')) {
        InverterXml = $.grep(InverterXml, function (e, i) {
            if (e.ID == trId) {
                debugger;
                $("#JobSystemDetails_NoOfInverter").text(parseInt($("#JobSystemDetails_NoOfInverter").text()) - parseInt(e.NoOfInverter));
                document.getElementById("tblInverter").deleteRow(i);
                return false;
            }
            else
                return true;
        });

    }
}

function DeleteBatteryManufacturerFromGrid(trId) {
    if (confirm('Are you sure you want to delete this Battery manufacturer?')) {
        batteryXml = $.grep(batteryXml, function (e, i) {
            if (e.JobBatteryManufacturerId == trId) {
                document.getElementById("tblBatteryManufacturer").deleteRow(i);
                return false;
            }
            else
                return true;
        });

    }
}

function MakePanelModelNumDisabled() {
    $("#searchPanelBrand").prop("disabled", true);
    $("#searchPanelBrand").val('');
    $("#JobPanelDetails_NoOfPanel").prop("disabled", true);
    $("#JobPanelDetails_NoOfPanel").val('');
    $('#hdnPanelModel').val('');
    $("#drpSupplier").prop("disabled", true);
    $("#drpSupplier").html('');
}

function MakeInverterModelSeriesDisabled() {
    $("#searchInverterSeries").prop("disabled", true);
    $("#searchInverterSeries").val('');
    $("#hdnInverterSeries").val('');
    //$("#searchInverterModel").prop("disabled", true);
    //$("#searchInverterModel").val('');
    //$("#hdnInverterModel").val('');
    $("#searchInverterBrand").prop("disabled", true);
    $("#searchInverterBrand").val('');
    $("#searchInverterBrand").val('');
    $("#searchInverterNumber").val('');
    $("#searchInverterNumber").prop("disabled", true);
}

function MakeBatteryModelNumberDisabled() {
    $("#searchBatteryModelNumber").prop("disabled", true);
    $("#searchBatteryModelNumber").val('');
    $("#hdnBatteryModelNumber").val('');
}

function MakeSystemModelSeriesDisabled() {
    $("#searchSystemBrand").prop("disabled", true);
    $("#searchSystemBrand").val('');
    $("#JobSystemDetails_NoOfPanel1").prop("disabled", true);
    $("#JobSystemDetails_NoOfPanel1").val('');
    $('#hdnSystemModel').val('');
}

function EnableDisableBatteryManufacturerDetails() {
    if ($('#hdnBatteryManufacturer').val().length > 0 && $('#hdnBatteryModelNumber').val().length > 0) {
        $('.batteryManufacturerDetails').find('select').prop('disabled', false)
    }
    else {
        $('.batteryManufacturerDetails').find('select').prop('disabled', true)
    }
}

function ShowHideAddSystemButton() {
    if (systemXml.length > 0)
        $("#btnSystemBrandAdd").hide();
    else
        $("#btnSystemBrandAdd").show();
}

//Generate New GUID
function Fuid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

/*Supplier bind based on selected manufacturer*/
function FillDropdownSupplierByManufacturer(manufacturerName, supplierName) {
    debugger;
    FillDropDown('drpSupplier', urlGetSupplierByManufacturer + "?ManufacturerName=" + manufacturerName, ((supplierName != undefined && supplierName != '' && supplierName != null && supplierName != '') ? supplierName : 0), true, SupplierDropdownEnable(), "Select Supplier");

}
function FillStartEndDateForPanel(manufacturerName, Model) {
    $.ajax(
        {
            url: urlgetStartEndDateForPanel,
            data: { manufacturer: manufacturerName, model: Model },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                if (data.status == true) {
                    $("#txtpanelStartDate").val(data.startDate);
                    $("#txtpanelEndDate").val(data.endDate);
                }
                else {
                    $("#txtpanelStartDate").val('');
                    $("#txtpanelEndDate").val('');
                }
            }
        });
}
function FillStartEndDateForInverter(manufacturerName, Model, Series) {
    $.ajax(
        {
            url: urlgetStartEndDateForInverter,
            data: { Brand: manufacturerName, Model: Model, series: Series },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                if (data.status == true) {
                    $("#txtInverterStartDate").val(data.startDate);
                    $("#txtInverterEndDate").val(data.endDate);
                }
                else {
                    $("#txtInverterStartDate").val('');
                    $("#txtInverterEndDate").val('');
                }
            }
        });
}
function FillStartEndDateForSWHBrandModel(manufacturerName, Model) {
    $.ajax(
        {
            url: urlgetStartEndDateForSWHBrandModel,
            data: { Brand: manufacturerName, Model: Model },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                if (data.status == true) {
                    $("#txtSWHpanelStartDate").val(data.startDate);
                    $("#txtSWHpanelEndDate").val(data.endDate);
                }
                else {
                    $("#txtSWHpanelStartDate").val('');
                    $("#txtSWHpanelEndDate").val('');
                }
            }
        });
}
function SupplierDropdownEnable() {
    debugger;
    setTimeout(function () {
        debugger;
        if ($("#drpSupplier").val() == '' && $("#drpSupplier option").length > 0 && $("#drpSupplier option").not("option[value='']").length == 1) {
            $(".job-panel-supplier-popup-drpsupplier").show();
            $("#drpSupplier option").not("option[value='']").eq(0).prop("selected", true);
            $("#drpSupplier").attr("disabled", true);
        }
        else if ($("#drpSupplier option").length > 0 && $("#drpSupplier option").not("option[value='']").length > 0) {
            $(".job-panel-supplier-popup-drpsupplier").show();
            if ($("#drpSupplier option").not("option[value='']").length == 1) {
                $("#drpSupplier").attr("disabled", true);
            }
            else {
                $("#drpSupplier").attr("disabled", false);
            }
        }
        else {
            $(".job-panel-supplier-popup-drpsupplier").hide();
            $("#drpSupplier").attr("disabled", true);
        }
    }, 1000);
}
