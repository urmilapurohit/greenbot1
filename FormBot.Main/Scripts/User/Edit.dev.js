$(document).ready(function () {

    //$("#onOffSwitchMasking").val($("#onOffSwitchMasking").prop('checked'));
    //$("#onOffSwitchMasking").click(function () {
    //    $("#onOffSwitchMasking").val($("#onOffSwitchMasking").prop('checked'));
    //    SaveMaskingValue();
    //});

    dropDownData.push(
        { id: 'UserTypeId', value: userTypeId, key: "UserType", hasSelect: true, callback: null, defaultText: null, proc: 'UserType_BindDropdown', param: [], bText: 'UserTypeName', bValue: 'UserTypeID' },
        { id: 'UnitTypeId', value: unitTypeID, key: "UnitType", hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', value: streetTypeId, key: "StreetType", hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'ResellerID', value: modelResellerId, key: "Reseller", hasSelect: true, callback: null, defaultText: null, proc: 'Reseller_BindDropDown', param: [], bText: 'ResellerName', bValue: 'ResellerID' },
        { id: 'PostalAddressID', value: postalAddressID, key: "PostalAddress", hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        //{ id: 'FCOGroupId', value: modelFCOGroupId, key: "FCOGroup", hasSelect: true, callback: null, defaultText: null, proc: 'FCOGroup_BindDropdown', param: [], bText: 'GroupName', bValue: 'FCOGroupId' },
        { id: 'SolarCompanyId', value: modelSolarCompanyId, key: "SolarCompany", hasSelect: true, callback: null, defaultText: null, proc: 'SolarCompany_BindDropDown', param: [], bText: 'CompanyName', bValue: 'SolarCompanyId' },
        //{ id: 'RoleID', value: modelRoleId, key: "Role", hasSelect: true, callback: null, defaultText: null, proc: 'Role_BindDropDown', param: [], bText: 'Name', bValue: 'RoleId' },
    );
    dropDownData.bindDropdown();

    $("#UserTypeId").prop("disabled", true);

    ChangeUserType(userTypeId, false);

    if (strFromDate == "" || strFromDate == "0001-01-01") {
        $("#fromDate").val("");
    }
    if (strToDate == "0001-01-01" || strToDate == "") {
        $("#toDate").val("");
    }
    $('.ProfileSCO').show();
    $('.ProfileRA').hide();
    $('.SCOPro').hide();
    $('.SCOPro1').hide();
    if (userTypeId == 8) {
        $('.SCOPro').show();
    }
    if (userTypeId == 7 || userTypeId == 9 || userTypeId == 2) {
        $('.SCOPro1').show();
    }
    addressID = addressId;
    POAddress(addressID);

    if ($('#IsWholeSaler').is(":checked")) {
        wholesalerAddressId = wholesalerIsPostalAddress;
        POWholesalerAddress(wholesalerAddressId);
    }

    if ($('#IsSAASUser').is(":checked")) {
        InvoicerAddressId = InvoicerIsPostalAddress;
        POInvoicerAddress(InvoicerAddressId);
    }

    if ($('#imgSign').attr('src') != "") {
        var SignName = $('#imgSign').attr('src');
        var guid = userID;
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
                if ($(this).attr("class").indexOf("selected") != -1) {
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
                        $('#instDesignerAddedByProfile').html(result);
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
            if ($(this).attr("class").indexOf("selected") != -1) {
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
    if (userTypeId == 8) {
        $('.SCACreate').show();
        $('.ProfileSCO').show();
        $('.uploadSCA').hide();
        $('.SCO').show();
        $('.SCProfile').hide();
        $('#t3').hide();
    }
    if (userTypeId == 8 && sessionUserTypeId == 4) {
        $('.SCACreate').hide();
        $('.chkSCASignUp').hide();
        $('.uploadSCA').hide();
    }
    if (userTypeId == 9) {
        $('.SCACreate').show();
    }
    if (userTypeId == 6) {
        $('.chkSCASignUp').show();
        $('.SCOProfile').show();
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

    GetCompanyNameFromCompanyABN(companyABN, $('#CompanyName'), companyName, false);
    GetCompanyNameFromCompanyABN(wholesalerCompanyABN, $('#WholesalerCompanyName'), wholesalerCompanyName, true);

});

function getDropDownValue() {
    removeSolarElecticianValidation();
    if (validateForm()) {
        $('.SlectBox option:selected').each(function (i) {
            $('<input type="hidden">').attr({
                name: 'FCOGroupSelected',
                value: $(this).val(),
                id: $(this).val(),
            }).appendTo('form');
        });
        return true;
    }
    else {
        return false;
    }
};

function validateForm() {

    if (userTypeId == 2 || userTypeId == 5) {
        var isExistCode = CheckClientCodePrefix($("#ClientCodePrefix").val());
        if (isExistCode == 0)
            return false;
    }

    removeSolarElecticianValidation();
    addRules();
    var obj = [];
    $.validator.unobtrusive.parse("#UserDetails");

    $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
        if ($(this).attr("class").indexOf("selected") != -1) {
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

function ChangeUserType(unitTypeID, isUserTypeLogin) {
    debugger;
    $('#horizontalTab').show();
    $('#spanFCOGroup').hide();
    $('.form-box').find('input:first').focus();
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
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FSA').show();
        $('.defaultFormBot').show();
        $('.showWholeSaler').hide();
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
        $('#dvIsWholeSaler').show();
        if ($("#UsageType").val() == "3") {
            $('.SAAS').show();
        }
        if ($("#IsSAASUser").is(":checked")) {
            $('.SAAS').show();
        }
        $('.RAcode').show();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.ProfileRA').hide();
        $('.showWholeSaler').show();
        $('.SCAcode').show();
        ShowHideWholeSalerInvoice();
        FillDropDown('RoleID', getRoleURL2, modelRoleId, true, null);
    }
    else if (unitTypeID == 3) {
        $('.FSA').hide();
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FCO').show();
        $('.defaultFormBot').show();
        $('.showWholeSaler').hide();
        $('.SCAcode').show();
        FillDropDown('RoleID', getRoleURL3, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('#UserTypeId').val('3');
        }
    }
    else if (unitTypeID == 4) {
        $('.FSA').hide();
        $('.FCO').hide();
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('.showWholeSaler').hide();
        $('.SCAcode').show();
        FillDropDown('RoleID', getRoleURL4, modelRoleId, true, null);
    }
    else if (unitTypeID == 5) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('.showWholeSaler').hide();
        $('.SCAcode').show();
        FillDropDown('RoleID', getRoleURL5, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('.RAMdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else if (unitTypeID == 6) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('.showWholeSaler').hide();
        FillDropDown('RoleID', getRoleURL6, modelRoleId, true, null);
    }
    else if (unitTypeID == 7) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('[name=SEDesigner][value=' + SEDesigner + ']').prop('checked', true);
        $('.showWholeSaler').hide();
        FillDropDown('RoleID', getRoleURL7, modelRoleId, true, null);
    }
    else if (unitTypeID == 8) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('.showWholeSaler').hide();
        FillDropDown('RoleID', getRoleURL8, modelRoleId, true, null);
        if (isUserTypeLogin == true) {
            $('.SCdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
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
        $('.showWholeSaler').hide();
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

$('#btnCancelTab7').click(function () {
    window.location.href = getUserIndexURL;
});
$('#btnCancelTab10').click(function () {
    window.location.href = getUserIndexURL;
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

$('#imgSignature').click(function () {
    $("#loading-image").css("display", "");
    if (modelUserId > 0)
        SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSign').attr('src', SRCSign);
    $('#imgSign').load(function () {
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

function SelectFCOGroup(arrFcoGroup) {
    $.each(arrFcoGroup, function (i, e) {
        $.each($('#FCOGroupId')[0], function (index, ele) {
            if (e && ele.value && parseInt(e) == parseInt(ele.value)) {
                $('#FCOGroupId')[0].sumo.selectItem(index);
            }
        });
    });

}

$('#chkSCA').change(function () {
    if ($(this).is(":checked")) {
        $('.chkSCASignUp').show();
    }
    else {
        $('.chkSCASignUp').hide();
    }
});

function addRules() {

    if (userTypeId == 4 || userTypeId == 6 || userTypeId == 8) {
        removeValidationOnTabChangeInstallerDesigner();
    }

    $("#UnitTypeId").rules("add", {
        required: false,
    });
    $("#UnitNumber").rules("add", {
        required: false,
    });
    $("#Password").rules("add", {
        required: false,
    });
    if ($("#UnitTypeId").val() > 0 && $("#UnitNumber").val() == "") {
        $("#UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $("#UnitNumber").next("span").attr('class', 'field-validation-valid');
        $('#lblUnitNumber').addClass("required");
        $('#lblStreetNumber').removeClass("required");
        $("#StreetNumber").rules("add", {
            required: false,
        });
    }
    if ($("#UnitTypeId").val() > 0 && $("#UnitNumber").val() != "") {
        $('#lblStreetNumber').removeClass("required");
        $("#StreetNumber").rules("add", {
            required: false,
        });
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
    }
    if ($("#UnitTypeId").val() == "" && $("#UnitNumber").val() == "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
        $('#lblStreetNumber').addClass("required");
        $("#StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }
    if ($("#UnitTypeId").val() == "" && $("#UnitNumber").val() != "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
        $('#lblStreetNumber').addClass("required");
        $("#StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }
    if ($('#IsWholeSaler').is(":checked") && $('#UserTypeId').val() == 2) {
        $("#WholesalerUnitTypeID").rules("add", {
            required: false,
        });
        $("#WholesalerUnitNumber").rules("add", {
            required: false,
        });
        if ($("#WholesalerUnitTypeID").val() == "" && $("#WholesalerUnitNumber").val().length == 0) {
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
            $("#WholesalerUnitNumber").next("span").attr('class', 'field-validation-valid');
            $('#lblWholesalerStreetNumber').addClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }

        if ($("#WholesalerUnitTypeID").val() > 0 && $("#WholesalerUnitNumber").val().length != 0) {
            $('#lblWholesalerStreetNumber').removeClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: false,
            });
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
        }
        if ($("#WholesalerUnitTypeID").val() > 0 && $("#WholesalerUnitNumber").val().length == 0) {
            $("#WholesalerUnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $('#lblWholesalerUnitNumber').addClass("required");
            $('#lblWholesalerStreetNumber').removeClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: false,
            });
        }
        if ($("#WholesalerUnitTypeID").val() == "" && $("#WholesalerUnitNumber").val().length != 0) {
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
            $('#lblWholesalerStreetNumber').addClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }
    }
    //if ($('#IsSAASUser').is(":checked") && $('#UserTypeId').val() == 2) {

    //    $("#InvoicerUnitTypeID").rules("add", {
    //        required: false,
    //    });
    //    $("#InvoicerUnitNumber").rules("add", {
    //        required: false,
    //    });
    //    if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length == 0) {
    //        $('#lblInvoicerUnitNumber').removeClass("required");
    //        $('#lblInvoicerUnitTypeID').removeClass("required");
    //        $("#InvoicerUnitNumber").rules("add", {
    //            required: false,
    //        });
    //        $("#InvoicerUnitTypeID").rules("add", {
    //            required: false,
    //        });
    //        $("#InvoicerUnitNumber").next("span").attr('class', 'field-validation-valid');
    //        $('#lblInvoicerStreetNumber').addClass("required");
    //        $("#InvoicerStreetNumber").rules("add", {
    //            required: true,
    //            messages: {
    //                required: "Street Number is required."
    //            }
    //        });
    //    }

    //    if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length != 0) {
    //        $('#lblInvoicerStreetNumber').removeClass("required");
    //        $("#InvoicerStreetNumber").rules("add", {
    //            required: false,
    //        });
    //        $('#lblInvoicerUnitNumber').removeClass("required");
    //        $('#lblInvoicerUnitTypeID').removeClass("required");
    //        $("#InvoicerUnitNumber").rules("add", {
    //            required: false,
    //        });
    //        $("#InvoicerUnitTypeID").rules("add", {
    //            required: false,
    //        });
    //    }
    //    if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length == 0) {
    //        $("#InvoicerUnitNumber").rules("add", {
    //            required: true,
    //            messages: {
    //                required: "Unit Number is required."
    //            }
    //        });
    //        $('#lblInvoicerUnitNumber').addClass("required");
    //        $('#lblInvoicerStreetNumber').removeClass("required");
    //        $("#InvoicerStreetNumber").rules("add", {
    //            required: false,
    //        });
    //    }
    //    if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length != 0) {
    //        $('#lblInvoicerUnitNumber').removeClass("required");
    //        $('#lblInvoicerUnitTypeID').removeClass("required");
    //        $("#InvoicerUnitNumber").rules("add", {
    //            required: false,
    //        });
    //        $("#InvoicerUnitTypeID").rules("add", {
    //            required: false,
    //        });
    //        $('#lblInvoicerStreetNumber').addClass("required");
    //        $("#InvoicerStreetNumber").rules("add", {
    //            required: true,
    //            messages: {
    //                required: "Street Number is required."
    //            }
    //        });
    //    }
    //}
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

$('#IsSAASUser').change(function () {
    ShowHideWholeSalerInvoice();
    if ($("#IsSAASUser").is(":checked")) {
        var InvoicerAddressID = $("#InvoicerAddressID").val();
        POInvoicerAddress(InvoicerAddressID);
    }
});
$('#IsWholeSaler').change(function () {
    ShowHideWholeSalerInvoice();
});

//function SaveMaskingValue() {
//    $.ajax(
//        {
//            url: saveMaskingValueURL,
//            data: { resellerId: resellerID, isAllowedMasking: $("#onOffSwitchMasking").val() },
//            dataType: 'json',
//            contentType: 'application/json; charset=utf-8',
//            type: 'get',
//            success: function (response) {
//                if (response && response.status == false) {
//                    showErrorMessage("Masking value has not been saved");
//                }
//                else if (response && response.status == true) {
//                    showSuccessMessage("Masking value has been saved");
//                }
//            },
//            error: function (response) {
//                showErrorMessage("Masking value has not been saved");
//            }
//        });
//}