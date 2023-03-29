$(document).ready(function () {
    $('#UserDetails').find('input').keypress(function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    dropDownData.push(
        { id: 'UserTypeId', value: userTypeId, key: "UserType", hasSelect: true, callback: null, defaultText: null, proc: 'UserType_BindDropdown', param: [], bText: 'UserTypeName', bValue: 'UserTypeID' },
        { id: 'UnitTypeId', value: unitTypeID, key: "UnitType", hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', value: streetTypeId, key: "StreetType", hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'ResellerID', value: modelResellerId, key: "Reseller", hasSelect: true, callback: null, defaultText: null, proc: 'Reseller_BindDropDown', param: [], bText: 'ResellerName', bValue: 'ResellerID' },
        { id: 'PostalAddressID', value: postalAddressID, key: "PostalAddress", hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        { id: 'FCOGroupId', value: modelFCOGroupId, key: "FCOGroup", hasSelect: true, callback: null, defaultText: null, proc: 'FCOGroup_BindDropdown', param: [], bText: 'GroupName', bValue: 'FCOGroupId' },
        { id: 'SolarCompanyId', value: modelSolarCompanyId, key: "SolarCompany", hasSelect: true, callback: null, defaultText: null, proc: 'SolarCompany_BindDropDown', param: [], bText: 'CompanyName', bValue: 'SolarCompanyId' },
        { id: 'RoleID', value: modelRoleId, key: "Role", hasSelect: true, callback: null, defaultText: null, proc: 'Role_BindDropDown', param: [], bText: 'Name', bValue: 'RoleId' },
    );
    dropDownData.bindDropdown();

    $("#UserTypeId").prop("disabled", true);

    var isUserTypeLogin = false;
    if (sessionUserTypeID == '2') {
        userTypeId = '5';
        isUserTypeLogin = true;
    }
    if (sessionUserTypeID == '4') {
        userTypeId = '8';
        isUserTypeLogin = true;
    }
    if (sessionUserTypeID == '6') {
        userTypeId = '8';
        isUserTypeLogin = true;
    }
    if (strFromDate == "") {
        $("#fromDate").val("");
    }
    if (strToDate == "0001-01-01") {
        $("#toDate").val("");
    }
    if (userTypeId != 0) {
        setTimeout(function () {
            ChangeUserType(userTypeId, isUserTypeLogin);
        }, 1000);
    }
    addressID = addressId;
    POAddress(addressID);

    if ($('#imgSign').attr('src') != "") {
        var SignName = $('#imgSign').attr('src');
        var guid = modelUserId;
        var proofDocumentURL = proofDocumentURL;
        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSign = SRC;
        $('#imgSign').attr('class', SignName);

        $('#imgSignature').show();
    }

    if (chkSTC == 0) {
        $('.chkSCASignUp').hide();
    }
    else {
        $('.chkSCASignUp').show();
    }

    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });
    $("#btnClosePopUpBoxSE").click(function () {
        $('#popupboxSE').modal('toggle');
    });
    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('toggle');
    });
    $("#btnClosePopUpBoxSelfie").click(function () {
        $('#popupboxSelfie').modal('toggle');
    });
    document.getElementById("uploadBtnSignature").disabled = true;
    document.getElementById("uploadBtn1").disabled = true;
    $('#signDelete').hide();
    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true,   // 100% fit in a container
        closed: 'accordion', // Start closed if in accordion view
        activate: function (event) {
            // Callback function if tab is switched
            addRules();
            var obj = [];
            $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
                if ($(this).attr("class") == "selected") {
                    obj.push($(this).val());
                }
            });

            var $this = $(this);
            if ($("#FCOGroupId").is(":visible")) {
                if (isValid && obj.length > 0 && CheckShowMessages()) {
                    //return  CheckShowMessages();
                    $('#spanFCOGroup').hide();
                    activeTab = $this.attr('id');
                    //$('.form-box').find('input:first').focus();
                }
                else {
                    // e.preventDefault();
                    $this.removeClass('resp-tab-active');
                    $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                    $('#' + activeTab).addClass('resp-tab-active');
                    $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                    if (obj.length > 0) {
                        $('#spanFCOGroup').hide();
                    }
                    else {
                        $('#spanFCOGroup').show();
                    }
                    CheckShowMessages();
                    return false;
                }
            }
            else {
                if (isValid && CheckShowMessages()) {
                    //return  CheckShowMessages();
                    activeTab = $this.attr('id');
                    //$('.form-box').find('input:first').focus();
                }
                else {
                    // e.preventDefault();
                    $this.removeClass('resp-tab-active');
                    $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                    $('#' + activeTab).addClass('resp-tab-active');
                    $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                    CheckShowMessages();
                    return false;
                }
            }
            if (activeTab == 't6') {
                $.ajax({
                    type: 'GET',
                    url: getInstallerDesignerAddedByProfileURL,
                    data: { usertypeId: userTypeId, solarcompanyId: modelSolarCompanyId },
                    contentType: 'application/json',
                    success: function (result) {
                        $('instDesignerAddedByProfile').html(result);
                    }
                });

            }
        }
    });

    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });

    window.asd = $('.SlectBox').SumoSelect({ csvDispCount: 4 });

    var arrFcoGroup = [];



    arrFcoGroup = model.ArrFcoGroup;
    SelectFCOGroup(arrFcoGroup);

    $('#FCOGroupId').change(function (event) {
        var obj = [];
        if ($("#FCOGroupId")[0][0].value == "")
            $("#FCOGroupId")[0][0].remove();

        $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
            if ($(this).attr("class") == "selected") {
                $('#FCOGroupId')[0].sumo.selectItem(i);
                obj.push($(this).val());
            }
        });
        if (obj.length > 0) {
            $('#spanFCOGroup').hide();
        }
        else {
            $('#spanFCOGroup').show();
        }
    });

    $("#AddressID").val(parseInt(addressId));
    if (userTypeId == 5) {
        $('.RAMCreate').show();
    }
    if (userTypeId == 5 && sessionUserTypeId == 2) {
        $('.RAMCreate').hide();
    }

    if (userTypeId == 8 && sessionUserTypeId == 4) {
        $('.SCACreate').hide();
        $('.chkSCASignUp').hide();
        $('.uploadSCA').hide();
    }
    if (userTypeId == 9) {
        $('.SCACreate').show();
        $('.SCOPro').hide();
    }
    if (userTypeId == 6) {
        $('.chkSCASignUp').show();
        $('.SCOProfile').show();
    }
    if (userTypeId == 8) {
        $('.SCACreate').show();
        $('.ProfileSCO').show();
        $('.uploadSCA').hide();
        $('#t3').hide();
        $('.SCProfile').hide();
    }
    if (userTypeId == 8 && sessionUserTypeId == 6) {
        $('.SCACreate').hide();
        $('.chkSCASignUp').hide();
        $('.uploadSCA').hide();
    }
    if (userTypeId == 2) {
        $('.uploadSCA').hide();
    }
    if ($('#UnitTypeId option:selected').val() == "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $('#lblStreetNumber').addClass("required");
    }
    else {
        $('#lblUnitNumber').addClass("required");
        $('#lblUnitTypeID').addClass("required");
        $('#lblStreetNumber').removeClass("required");
    }
    var CompanyNameFirstTimeBind = companyName;//GetCompnayName();
    if (CompanyNameFirstTimeBind.indexOf('&#39') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&#39;/g, "'");
    }
    else if (CompanyNameFirstTimeBind.indexOf('&amp') != -1) {
        str = CompanyNameFirstTimeBind.replace(/&amp;/g, '&');
    }
    else if (CompanyNameFirstTimeBind.indexOf('&quot') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&quot;/g, '"');
    }
    else if (CompanyNameFirstTimeBind.indexOf('&lt') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&lt;/g, '<');
    }
    else if (CompanyNameFirstTimeBind.indexOf('&gt') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&gt;/g, '>');
    }
    $("#CompanyName").append($("<option></option>").val(CompanyNameFirstTimeBind).html(CompanyNameFirstTimeBind));
    $("#CompanyName").val(CompanyNameFirstTimeBind);
});

function getDropDownValue() {

    $('.SlectBox option:selected').each(function (i) {
        $('<input type="hidden">').attr({
            name: 'FCOGroupSelected',
            value: $(this).val(),
            id: $(this).val(),
        }).appendTo('form');
    });
    return validateForm();
};

function validateForm() {
    addRules();
    var obj = [];
    $.validator.unobtrusive.parse("#UserDetails");

    $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
        if ($(this).attr("class") == "selected") {
            obj.push($(this).val());
        }
    });

    if ($("#FCOGroupId").is(":visible")) {
        if ($("#UserDetails").valid()) {
            if (obj.length > 0) {
                $('#spanFCOGroup').hide();
                return CheckShowMessages()
                // return true;
            }
            else {
                $('#spanFCOGroup').show();
                CheckShowMessages();
                return false;
            }
        }
        else {
            if (obj.length > 0) {
                $('#spanFCOGroup').hide();

            }
            else {
                $('#spanFCOGroup').show();

            }
            CheckShowMessages();
            return false;
        }
    }
    else {
        if ($("#UserDetails").valid() && CheckShowMessages()) {
            return true;
        }
        else {
            return false;
        }
    }
}

function initialize(data) {
    $("#CompanyName").change(function () {
        $.each(data, function (key, value) {
            var Cname = value.CompanyName;
            var drpVal = $('select#CompanyName option:selected').val();
            if (Cname == drpVal) {
                $('#fromDate').val(value.strFromDate);
                $('#toDate').val(value.strToDate);
            }
        });
    });
}

function ChangeUserType(unitTypeID, isUserTypeLogin) {
    $('#horizontalTab').show();
    $('#spanFCOGroup').hide();
    //$("#FCOGroupId")[0].sumo.unSelectAll();
    $('input[type=text]').each(function () {
        $(this).attr('class', 'form-control valid');
    });

    $(".field-validation-error").attr('class', 'field-validation-valid');

    if (unitTypeID == "") {
        $('#horizontalTab').hide();
        defaultHideField();
    }
    else if (unitTypeID == 1) {
        $('.RA').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FSA').show();
        $('.defaultFormBot').show();
        FillDropDown('RoleID', getRoleURL1, modelRoleId, true, null);
    }
    else if (unitTypeID == 2) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SSC').hide();
        $('.SE').hide();
        $('.SC').hide();
        $('.RA').show();
        if ($("#UsageType").val() == "3") {
            $('.SAAS').show();
        }
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.ProfileRA').hide();
        FillDropDown('RoleID', getRoleURL2, modelRoleId, true, null);
    }
    else if (unitTypeID == 3) {
        $('.FSA').hide();
        $('.RA').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FCO').show();
        $('.defaultFormBot').show();
        FillDropDown('RoleID', getRoleURL3, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('#UserTypeId').val('3');
        }
    }
    else if (unitTypeID == 4) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RA').hide();
        $('.RAM').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SCA').show();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        FillDropDown('RoleID', getRoleURL4, modelRoleId, true, null);
    }
    else if (unitTypeID == 5) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('#UserTypeId').val('5');
        $('.SC').hide();
        $('.RAM').show();
        $('.defaultFormBot').show();
        FillDropDown('RoleID', getRoleURL5, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('.RAMdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else if (unitTypeID == 6) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SC').hide();
        $('.SSC').show();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        FillDropDown('RoleID', getRoleURL6, modelRoleId, true, null);
    }
    else if (unitTypeID == 7) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SE').show();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $('[name=SEDesigner][value=' + SEDesigner + ']').prop('checked', true)
        FillDropDown('RoleID', getRoleURL7, modelRoleId, true, null);
    }
    else if (unitTypeID == 8) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SCO').show();
        $('#UserTypeId').val('8');
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').hide();
        $('.ProfileRA').hide();
        FillDropDown('RoleID', getRoleURL8, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('.SCdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC0').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        FillDropDown('RoleID', getRoleURL9, modelRoleId, true, null);
    }
}

$("#UserTypeId").change(function () {
    var unitTypeID = $('#UserTypeId option:selected').val();
    ChangeUserType(unitTypeID);
});

$('#btnCancel').click(function () {
    window.location.href = getUserIndexURL;
});

$('#btnCancelLast').click(function () {
    window.location.href = getUserIndexURL;
});

$("#CompanyABN").change(function () {
    var id = $('#CompanyABN').val();
    $.ajax({
        type: "GET",
        url: getGetCompanyABNURL,
        data: { id: id },
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            if (data == 0) {
                $('#CompanyName').empty();
                $('#fromDate').val("");
                $('#toDate').val("");
                $("#CompanyName").append($("<option></option>").val("").html("Select"));
            }
            else {
                if ($('#CompanyName option').length > 1) {
                    $('#CompanyName').empty();
                    $("#CompanyName").append($("<option></option>").val("").html("Select"));
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + "Invalide Company ABN.");
                    $("#errorMsgRegion").show();

                    return false;
                    $.each(data, function (key, value) {
                        $("#CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                else {
                    $.each(data, function (key, value) {
                        $("#CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                initialize(data);
                return data;
            }
        }
    });
    //}
});

$(document).ready(function () {
    $('#txtTown').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: processRequestURL,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#txtState').val(ui.item.state);
            $('#txtPostCode').val(ui.item.postcode);
        }
    });

    $('#txtPostCode').autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: processRequestURL,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array)
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                    else
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.postcode,
                                state: item.state,
                                location: item.location
                            }
                        }));
                }
            })
        },
        select: function (event, ui) {
            $('#txtState').val(ui.item.state);
            $('#txtTown').val(ui.item.location);
        }
    });
});

$('#imgSignature').click(function () {
    $("#loading-image").css("display", "");
    $('#imgSign').attr('src', SRCSign).load(function () {
        logoWidth = this.width;
        logoHeight = this.height;
        $('#popupbox').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            $("#imgSign").closest('div').height($(window).height() - 205);
            $("#imgSign").closest('div').css('overflow-y', 'scroll');
            $("#imgSign").height(logoHeight);
        }
        else {
            $("#imgSign").height(logoHeight);
            $("#imgSign").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            //$('#popupbox').find(".modal-dialog").width(screen.width - 10);

            $('#popupbox').find(".modal-dialog").width(screen.width - 250);
            $("#imgSign").width(logoWidth);
        }
        else {
            $("#imgSign").width(logoWidth);
            $('#popupbox').find(".modal-dialog").width(logoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgSign").unbind("error");
    $('#imgSign').attr('src', SRCSign).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
});

$('#imgSignatureSE').click(function () {
    $("#loading-image").css("display", "");
    if (modelUserId > 0)
        SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSignSE').attr('src', SRCSign);
    $('#imgSignSE').load(function () {
        logoWidth = this.width;
        logoHeight = this.height;

        $('#popupboxSE').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            //$("#imgSign").height($(window).height() - 205);
            $("#imgSignSE").closest('div').height($(window).height() - 205);
            $("#imgSignSE").closest('div').css('overflow-y', 'scroll');
            $("#imgSignSE").height(logoHeight);
        }
        else {
            $("#imgSignSE").height(logoHeight);
            $("#imgSignSE").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            $('#popupboxSE').find(".modal-dialog").width(screen.width - 250);
            $("#imgSignSE").width(logoWidth);
        }
        else {
            $("#imgSignSE").width(logoWidth);
            $('#popupboxSE').find(".modal-dialog").width(logoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgSignSE").unbind("error");
    $('#imgSignSE').attr("src", SRCSign).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
});

//File browse get FileName
function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}

$('.resp-tabs-list li').click(function (e, c) {
    var $this = $(this);
    addRules();
    isValid = $("#UserDetails").valid();
});

function POAddress(addressID) {
    if (addressID == 1) {
        $('.DPA').show();
        $('.PDA').hide();
        $('#PostalAddressID').val('');
        $('#PostalDeliveryNumber').val('');
    }
    else {
        $('.DPA').hide();
        $('.PDA').show();
        $('#UnitTypeId').val('');
        $('#UnitNumber').val('');
        $('#StreetNumber').val('');
        $('#StreetName').val('');
        $('#StreetTypeId').val('');
    }
}

$("#AddressID").change(function () {
    var addressID = $('#AddressID option:selected').val();
    POAddress(addressID);
});

function SelectFCOGroup(arrFcoGroup) {
    $.each(arrFcoGroup, function (i, e) {
        $.each($('#FCOGroupId')[0], function (index, ele) {
            if (e && ele.value && parseInt(e) == parseInt(ele.value)) {
                $('#FCOGroupId')[0].sumo.selectItem(index);
            }
        });
    });

}

function DeleteDocumentFolderOnCancel() {
    var guid = USERID;
    var Name = [];
    Name = document.getElementsByName("FileNamesCreate");
    var Sign = document.getElementsByName("Signature");
    var SignName = Sign[0].id;
    if (Name.length > 0) {
        for (var i = 0; i < Name.length; i++) {
            var docname = Name[i].id;
            DeleteFileFromUserOnCancel(docname, guid);
        }
    }

}

function addRules() {

    $("#Password").rules("add", {
        required: false,
    });

}