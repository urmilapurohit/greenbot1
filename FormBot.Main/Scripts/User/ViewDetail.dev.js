$(document).ready(function () {
    if (sessionUserTypeId == 1
        || sessionUserTypeId == 3
        || sessionUserTypeId == 2
        || sessionUserTypeId == 5) {
        $('#lblUserName').removeClass("required");
        $("#UserName").rules("add", {
            required: false,
        });

        $('#lblPassword').removeClass("required");
        $("#Password").rules("add", {
            required: false,
        });
    }
    $('.viewPre').show();
    $(".SE").find("[idClass='docVerification']").each(function () {
        $(this).rules("add", {
            required: false,
        });
    });
    //FillDropDownUser('UserTypeId', getUserTypeURL, userTypeId, true, null);
    //FillDropDown('RoleID', getRoleURL, modelRoleId, true, null);

    dropDownData.push({ id: 'UnitTypeId', key: "UnitType", value: unitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', key: "StreetType", value: streetTypeId, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'PostalAddressID', key: "PostalAddress", value: postalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        { id: 'RoleID', key: "Role", value: modelRoleId, hasSelect: true, callback: null, defaultText: null, proc: 'Role_BindDropDown', param: [{ UserType: userTypeId }, { CreatedBy: sessionLoggedInUserId }], bText: 'Name', bValue: 'RoleId' },
        { id: 'UserTypeId', key: "UserType", value: userTypeId, hasSelect: true, callback: null, defaultText: null, proc: 'UserType_BindDropdown', param: [], bText: 'UserTypeName', bValue: 'UserTypeID' }
    );

    if (userTypeId != 0) {
        ChangeUserType(userTypeId);
    }
    if (modelStatus == 2) {
        $('.btnSubmit').hide();
    }
    if (sessionUserTypeId == 1 || sessionUserTypeId == 3) {
        $('.btnSubmit').show();
    }
    if ($('#imgSign').attr('src') != "") {
        var SignName = $('#imgSign').attr('src');
        var guid = modelUserId;
        //var proofDocumentURL = proofDocumentURL;
        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSign = SRC;
        $('#imgSign').attr('class', SignName);

        $('#imgSignature').show();
    }

    if ($('#imgSelfieImage').attr('src') != "") {
        var SignName = $('#imgSelfieImage').attr('src');
        var guid = modelUserId;
        var proofDocumentURL = proofDocumentURL;
        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSelfie = SRC;
        $('#imgSelfieImage').attr('class', SignName);

        $('#imgSelfie').show();
    }

    if (strFromDate == "" || strFromDate == "0001-01-01") {
        $("#fromDate").val("");
    }
    if (strToDate == "0001-01-01" || strToDate == "") {
        $("#toDate").val("");
    }
    if (userTypeId == 4) {
        if (chkSTC == 0) {
            $('.chkisSTC').hide();
            $('.chkSCASignUp').hide();
        }
        else {
            $('.chkisSTC').show();
            $('.chkSCASignUp').show();
        }
    }

    $("#Status").val(parseInt(modelStatus));
    $("#IsVerified").val(isverified);
    $('[name=SEDesigner][value=' + SEDesigner + ']').prop('checked', true)

    $('.viewPre').show();
    $('.ViewBtn').hide();
    $('.viewDetail').hide();
    $('.SCAShowUsername').show();
    $('.SCAcode').show();
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
    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('hide');
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

    $("#btnViewCancel").click(function () {
        if (userTypeId == 4) {
            window.location.href = getSCAURL;
        } else {
            window.location.href = getSEURL;
        }
    });

    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true,   // 100% fit in a container
        closed: 'accordion', // Start closed if in accordion view
        activate: function (event) {
            // Callback function if tab is switched
            //addRules();
            addressValidation();
            var $this = $(this);

            if (userTypeId == 7 && (!$("#IsPVDUser").prop('checked') && !$("#IsSWHUser").prop('checked'))) {// && !$("#IsVEECUser").prop('checked')) {
                $this.removeClass('resp-tab-active');
                $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                $('#' + activeTab).addClass('resp-tab-active');
                $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                alert('At least one work type should be selected.');
                return false;
            }

            if (isValid && CheckShowMessages() && addRules()) {
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
                addRules();
                return false;
            }

            if (activeTab == 't3') {
                if (userTypeId == 4) {
                    autoCompleteRAM(modelRAMId, modelResellerId);
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
    addressID = addressId;
    POAddress(addressID);
    $("#AddressID").val(parseInt(addressId));

    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });

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
    $('.form-box').find('input:first').focus();

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

    dropDownData.bindDropdown();
});
function GetCompnayName() {
    if (customCompanyName == '' || customCompanyName == null) {
        return customCompanyName;
    }
    else {
        return customCompanyName;
    }
}
function validateForm() {
    debugger;
    // Hiding previous error msg
    $('#ValidationSummary').hide();
    $('#msgSection').hide();
    if (($("#IsVerified").val() == "3") && ($("#chkIsSignatureVerified").prop('checked') && $("#chkIsSelfieVerified").prop('checked') && $("#chkIsDriverLicVerified").prop('checked') && $("#chkIsOtherDocVerified").prop('checked'))) {
        showErrorMessage("At least one document need to be unverified for set verification status to rejected.");
        return false;
    }
    if (userTypeId == 7 && !$("#IsPVDUser").prop('checked') && !$("#IsSWHUser").prop('checked')) {// && !$("#IsVEECUser").prop('checked')) {
        alert('At least one work type should be selected.');
        return false;
    }
    else {
        removeSolarElecticianValidation();
        $('#IsAutoRequest').val($('#AutoRequestSwitch').prop('checked'));
        $(".SE").find("[idClass='docVerification']").each(function () {
            $(this).rules("add", {
                required: false,
            });
        });

        $.validator.unobtrusive.parse("#UserDetails");
        //addRules();
        addressValidation();
        if ($("#UserDetails").valid() && CheckShowMessages() && addRules()) {
            return true;
        }
        else {
            addRules();
            return false;
        }
    }
}

$('#ValidationSummaryClose').click(function () {
    $("#ValidationSummary").hide();
});

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

function ChangeUserType(unitTypeID) {
    debugger;
    $('.form-box').find('input:first').focus();
    $('input[type=text]').each(function () {
        // $(this).val('');
        $(this).attr('class', 'form-control valid');
    });

    $(".field-validation-error").attr('class', 'field-validation-valid');

    if (unitTypeID == "") {
        defaultHideField();
    }
    else if (unitTypeID == 1) {
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
        ShowHideWholeSalerInvoice();
        if ($("#UsageType").val() == "3") {
            $('.SAAS').show();
        }
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 3) {
        $('.FSA').hide();
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
    }
    else if (unitTypeID == 4) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SCA').show();
        $('.ProfileSCO').show();
        $('.ProfileRA').hide();
        $('.defaultFormBot').show();
        $('#lblCECAccreditationNumber').addClass("required");
    }
    else if (unitTypeID == 5) {
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.RAM').show();
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 6) {
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
    }
    else if (unitTypeID == 7) {
        ShowHideForSEandSWH();
        $('.SWH').hide();

        if (sessionUserTypeId != 7)
            $(".OnlySE").hide();
        else
            $(".OnlySE").show();

        $('#lblCECAccreditationNumber').addClass("required");
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
    }
    else if (unitTypeID == 10) {
        ShowHideForSEandSWH();
        $('.hideForSWH').hide();
        $('.SWH').show();

        if (sessionUserTypeId != 7)
            $(".OnlySE").hide();
        else
            $(".OnlySE").show();

        $('#lblSWHLicenseNumber').addClass("required");
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
    }
    else if (unitTypeID == 8) {
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
    }
    else {
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
        $('.ProfileRA').hide();
        $('.defaultFormBot').show();
    }
}

function ShowHideForSEandSWH() {
    $('.RA').hide();
    $('.SCA').hide();
    $('.FCO').hide();
    $('.FSA').hide();
    $('.SCO').hide();
    $('.RAM').hide();
    $('.SSC').hide();
    $('.SC').hide();
    $('.ProfileSCO').show();
    $('.ProfileRA').hide();
    $('.defaultFormBot').show();
    $('.SE').show();
}

$("#UserTypeId").change(function () {
    var unitTypeID = $('#UserTypeId option:selected').val();
    ChangeUserType(unitTypeID);

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

$('#imgSignature').click(function () {

    $("#loading-image").css("display", "");
    SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSign').attr('src', SRCSign);
    $('#imgSign').load(function () {
        logoWidth = this.width;
        logoHeight = this.height;
        $('#popupbox').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            //$("#imgSign").height($(window).height() - 205);

            $("#imgSign").closest('div').height($(window).height() - 205);
            $("#imgSign").closest('div').css('overflow-y', 'scroll');
            $("#imgSign").height(logoHeight);
        }
        else {
            $("#imgSign").height(logoHeight);
            $("#imgSign").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            //$("#imgSign").width(screen.width - 10);
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

function SaveStatusAndNote() {
    var status = $('#Status').val();
    var note = $('#Note').val();
    var userId = modelUserId;

    $.ajax(
        {

            url: '@Url.Action("InsertStatusNote", "User")',
            type: "POST",
            async: false,
            data: JSON.stringify({ Id: userId, Status: status, Note: note }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    if (userTypeId == 7) {
                        window.location.href = getSEURL;
                    }
                    else {
                        window.location.href = getSCAURL;
                    }
                }
            }
        });
}

$('.resp-tabs-list li').click(function (e, c) {
    var $this = $(this);
    addRules();
    isValid = $("#UserDetails").valid();
});

function POAddress(addressID) {
    $(".POID").find('input[type=text]').each(function () {
    });
    $(".POID").find('.field-validation-error').attr('class', 'field-validation-valid');

    if (addressID == 1) {
        $('.DPA').show();
        $('.PDA').hide();
        $('#PostalAddressID').attr('class', 'form-control valid');
        $('#PostalDeliveryNumber').attr('class', 'form-control valid');
    }
    else {
        $('.DPA').hide();
        $('.PDA').show();
        $('#UnitTypeId').attr('class', 'form-control valid');
        $('#UnitNumber').attr('class', 'form-control valid');
        $('#StreetNumber').attr('class', 'form-control valid');
        $('#StreetTypeId').attr('class', 'form-control valid');
        $('#StreetName').attr('class', 'form-control valid');
    }
}

$("#AddressID").change(function () {
    var addressID = $('#AddressID option:selected').val();
    POAddress(addressID);
});

$('#chkSCA').change(function () {
    if ($(this).is(":checked")) {
        $('.chkSCASignUp').show();
    }
    else {
        $('.chkSCASignUp').hide();
    }
});

function getDropDownValue() {
    if (validateForm()) {
        return true;
    }
    else {
        return false;
    }
};
function addRules() {

    if (userTypeId == 7) {
        ValidateFieldBasedOnWorkType();
        if ($("#IsVerified").val().toString().toLowerCase() == "2") {
            if ($('#chkIsSignatureVerified').prop('checked') == false || $('#chkIsSelfieVerified').prop('checked') == false || $("#chkIsDriverLicVerified").prop('checked') == false || $("#chkIsOtherDocVerified").prop('checked') == false) {
                showErrorMessage("Please verify all the documents");
                $("html, body").animate({ scrollTop: 0 }, "slow");
                return false;
            }
        }
    }

    if (userTypeId == 4) {
        $("#CECAccreditationNumber").rules("add", {
            required: true,
            messages: {
                required: "CEC Accreditation Number is required."
            }
        });
    }

    if (userTypeId == 4 || userTypeId == 6 || userTypeId == 8) {
        removeValidationOnTabChangeInstallerDesigner();
    }

    if ($("#chkSCA").is(':checked') == true && userTypeId == 4) {
        if ($('#tblDocuments1 tr').length == 0) {
            $(".alert").hide();
            $("#errorMsgRegion").html(closeButton + "Please upload atleast one proof of identity document.");
            $("#errorMsgRegion").show();
            return false;
        }

        else {
            return true;
        }
    }
    else {
        return true;
    }
}

function addressValidation() {
    $("#UnitTypeId").rules("add", {
        required: false,
    });
    $("#UnitNumber").rules("add", {
        required: false,
    });
    if ($("#UnitTypeId").val() == "" && $("#UnitNumber").val() == "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
        $("#UnitNumber").next("span").attr('class', 'field-validation-valid');

        $('#lblStreetNumber').addClass("required");
        $("#StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
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
    if ($("#UnitTypeId").val() > 0 && $("#UnitNumber").val() == "") {
        $("#UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblUnitNumber').addClass("required");
        $('#lblStreetNumber').removeClass("required");
        $("#StreetNumber").rules("add", {
            required: false,
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