$(document).ready(function () {
    if (sessionUserTypeId == '2') {
        $('#t5').show();
        $('.tab5').show();
    }
    $("#RecUsernamePwd").hide();
    setTimeout(function () {
        $('.form-box').find('input:first').focus();
    }, 500);

    dropDownData.push(
        { id: 'UnitTypeId', value: unitTypeID, key: "UnitType", hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', value: streetTypeId, key: "StreetType", hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'PostalAddressID', value: postalAddressID, key: "PostalAddress", hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
    );
    dropDownData.bindDropdown();

    $('#t4').show();
    $('#popupbox1').modal({ backdrop: 'static', keyboard: false });
    $("#imgSign").css('height', '300px');
    $("#signMyProfile").css('height', '300px');
    $("#imgFile").css('height', '400px');
    $("#MainDiv").css('height', '400px');
    $("#ConfigurationPassword").val(modelConfigPassword);
    if (sessionUserTypeId == 7) {
        $("#FirstName").prop("readonly", true);
        $("#LastName").prop("readonly", true);
        $("#Email").prop("readonly", true);
        $("#Phone").prop("readonly", true);
        $("#IsActive").prop("readonly", true);
        $("#CompanyABN").prop("readonly", true);
        $("#UnitNumber").prop("readonly", true);
        $("#txtTown").prop("readonly", true);
        $("#PostalDeliveryNumber").prop("readonly", true);
        $("#AddressID").prop("disabled", true);
        $("#PostalAddressID").prop("disabled", true);
        $("#StreetNumber").prop("readonly", true);
        $("#StreetName").prop("readonly", true);
        $("#CompanyWebsite").prop("readonly", true);
        $("#txtPostCode").prop("readonly", true);
        $("#BSB").prop("readonly", true);
        $("#AccountNumber").prop("readonly", true);
        $("#AccountName").prop("readonly", true);
        $("#CECAccreditationNumber").prop("readonly", true);
        $("#ElectricalContractorsLicenseNumber").prop("readonly", true);
        $("#CECDesignerNumber").prop("readonly", true);
        $("#UserTypeId").prop("readonly", true);
        $("#ResellerID").prop("readonly", true);
        $("#SolarCompanyId").prop("readonly", true);
        $("#CompanyName").prop("disabled", true);
        $("#UnitTypeId").prop("disabled", true);
        $("#StreetTypeId").prop("disabled", true);
    }
    $('.MyProfileConfigure').css('max-height', '500px');
    $('.MyProfileConfigure').css('overflow-y', 'auto');
    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });
    $("#btnClosePopUpBoxSE").click(function () {
        $('#popupboxSE').modal('toggle');
    });
    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('toggle');
    });
    $("#btnCloseConfigure").click(function () {
        $('#myModal1').modal('hide');
    });
    $("#btnClosepopupboxlogo").click(function () {
        $('#popupboxlogo').modal('toggle');
    });
    $("#btnClosePopUpBoxSelfie").click(function () {
        $('#popupboxSelfie').modal('toggle');
    });

    if (strFromDate == "") {
        $("#fromDate").val("");
    }
    if (strToDate == "0001-01-01") {
        $("#toDate").val("");
    }

    if (sessionUserTypeId == 4) {
        if (chkSTC == 0) {
            $('.chkisSTC').hide();
            $('.chkSCASignUp').hide();
        }
        else {
            $('.chkSCASignUp').show();
        }
    }
    if (sessionLogo != null && sessionTheme != null && sessionLogo != '' && sessionTheme != '') {
        $('#imgLogo').attr('src', (sessionSiteURLBase + sessionLogo));
        $('body').attr('id', sessionTheme);
    }
    $('[name=SEDesigner][value=' + SEDesigner + ']').prop('checked', true)

    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true,   // 100% fit in a container
        closed: 'accordion', // Start closed if in accordion view
        activate: function (event) {
            //addRules();
            addressValidation();
            $("#Password").rules("add", {
                required: false,
            });
            // Callback function if tab is switched
            var $this = $(this);

            if (isValid && CheckShowMessages() && addRules()) {
                activeTab = $this.attr('id');

            }
            else {
                $this.removeClass('resp-tab-active');
                $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                $('#' + activeTab).addClass('resp-tab-active');
                $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                CheckShowMessages();
                addRules();
                return false;
            }

            if (activeTab == 't4') {
                initSample();
                CKEDITOR.config.height = 150;
                if (viewBagSignature != '') {
                    var data = dataSignature;
                    CKEDITOR.instances['editor'].setData(data.Data.Signature);
                }
            }

            //Rec email changes
            if (activeTab == 't5') {
                initSample_Rec();
                if (viewBagRECEmailSign != '') {
                    var data = dataRECEmailSign;
                    CKEDITOR.instances['editor_Rec'].setData(data.Data.Signature);
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

    GetCompanyNameFromCompanyABN(companyABN, $('#CompanyName'), companyName, false);

    //var userTypeId = sessionUserTypeId;
    if (sessionUserTypeId != 0) {
        ChangeUserType(userTypeId);
    }

    if (sessionUserTypeId == 4) {
        $('#CompanyABN').prop("readonly", true);
    }

    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });

    addressID = addressId;
    POAddress(addressID);
    $("#AddressID").val(parseInt(addressId));

    if ($('#imgSign').attr('src') != "") {
        if (oldFileName != "") {

            var SignName = $('#imgSign').attr('src');
            var guid = modelUserId;
            var proofDocumentURL = proofDocumentURL;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSign = SRC;
            //$('#imgSign').attr('src', SRC).load(function() { logoWidthSign=this.width; logoHeightSign=this.height});
            $('#imgSign').attr('class', SignName);
            $('#imgSignature').show();
        }
    }

    if ($('#imgLG').attr('src') != "") {

        var imglog = "Images/logo.png";
        if (sessionLogo != imglog) {
            var SignName = $('#imgLG').attr('src');
            var guid = modelUserId;
            var proofDocumentURL = proofDocumentURL;
            var SRC = proofDocumentURL + "UserDocuments" + "/" + SignName;

            SRCLG = SRC;
            $('#imgLG').attr('class', SignName);
            $('#imgViewLogo').show();
        }
    }
    $('#btnCancelLast').hide();
    $('#btnCancel').hide();
    $('.chkActive').hide();
    $('.viewPre').show();
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
    if (sessionUserTypeId == 8) {
        $('.Profile').show();
        $('.SCOProfile').hide();
    }
    if (sessionUserTypeId == 2) {
        $('.SCOProfile').show();
        $('.ProfileSCO').show();
    }
    if (sessionUserTypeId == 4) {
        $('.SCOProfile').show();
        $('.ProfileSCO').show();
        $('.SCAPrev').show();
    }
    if (sessionUserTypeId == 9) {
        $('.SCProfile').hide();
        $('.SCO').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
    }
    if (sessionUserTypeId == 7) {
        $('.SCProfile').hide();
        $('.SCO').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
    }
    if (sessionUserTypeId == 6) {
        $('.SCProfile').hide();
        $('.SCO').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
    }
    if (modelTheme == 2) {
        $('.theme-colors').find('button').not("green").removeClass('active');
        $('.theme-colors').find('[id="blue"]').addClass('active');
    }
    if (modelTheme == 3) {
        $('.theme-colors').find('button').not("green").removeClass('active');
        $('.theme-colors').find('[id="pink"]').addClass('active');
    }
    if (modelTheme == 4) {
        $('.theme-colors').find('button').not("green").removeClass('active');
        $('.theme-colors').find('[id="skyblue"]').addClass('active');
    }
    if (modelTheme == 5) {
        $('.theme-colors').find('button').not("green").removeClass('active');
        $('.theme-colors').find('[id="yellow"]').addClass('active');
    }
    if (modelTheme == 6) {
        $('.theme-colors').find('button').not("green").removeClass('active');
        $('.theme-colors').find('[id="black"]').addClass('active');
    }
    if (sessionUserTypeId == 4) {
        if (chkSTC == 0) {
            $('.chkisSTC').hide();
        }
        else {
            $('.chkisSTC').show();
        }
    }
    setTimeout(function () {
        $('.form-box').find('input:first').focus();
    }, 2000);
});

function validateForm() {
    //addRules();
    addressValidation();
    removeSolarElecticianValidation();
    $.validator.unobtrusive.parse("#UserDetails");

    if ($("#UserDetails").valid() && CheckShowMessages() && addRules()) {
        return true;
    }
    else {
        addRules();
        return false;
    }
}

$('#btnSubmit').click(function () {
    validateForm();
});

function ChangeUserType(unitTypeID) {
    $("#RecUsernamePwd").hide();
    $('input[type=text]').each(function () {
        $(this).attr('class', 'form-control valid');
    });

    $(".field-validation-error").attr('class', 'field-validation-valid');

    if (unitTypeID == "") {
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
        $('.FSA').show();
        $('.defaultFormBot').show();
        $('.Profile').show();

    }
    else if (unitTypeID == 2) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SSC').hide();
        $('.SE').hide();
        $('.RA').show();
        if ($("#UsageType").val() == "3") {
            $('.SAAS').show();
        }
        $('.defaultFormBot').show();
        $('.SCOProfile').show();
        $('.ThemeProfileRA').show();
    }
    else if (unitTypeID == 3) {
        $('.FSA').hide();
        $('.RA').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.FCO').show();
        $('.defaultFormBot').show();
        $('.Profile').show();

    }
    else if (unitTypeID == 4) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RA').hide();
        $('.RAM').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SCA').show();
        $('.defaultFormBot').show();
        $('#lblCECAccreditationNumber').addClass("required");
    }
    else if (unitTypeID == 5) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.RAM').show();
        $('.defaultFormBot').show();
        $('.Profile').show();
    }
    else if (unitTypeID == 6) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SSC').show();
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 7) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SSC').hide();
        $('.SE').show();
        $('.defaultFormBot').show();
        $('#lblCECAccreditationNumber').addClass("required");
    }
    else if (unitTypeID == 8) {
        $('.RA').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SSC').hide();
        $('.SCO').show();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').hide();
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
    }
}

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
    $('#imgSign').attr('src', SRCSign).load(function () {
        logoWidthSign = this.width;
        logoHeightSign = this.height;

        $('#popupbox').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeightSign) {
            //$("#imgSign").height($(window).height() - 205);

            $("#imgSign").closest('div').height($(window).height() - 205);
            $("#imgSign").closest('div').css('overflow-y', 'scroll');
            $("#imgSign").height(logoHeightSign);
        }
        else {
            $("#imgSign").height(logoHeightSign);
            $("#imgSign").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthSign || logoWidthSign >= screen.width - 250) {
            //$("#imgSign").width(screen.width - 10);
            //$('#popupbox').find(".modal-dialog").width(screen.width - 10);

            $('#popupbox').find(".modal-dialog").width(screen.width - 250);
            $("#imgSign").width(logoWidthSign);
        }
        else {
            $("#imgSign").width(logoWidthSign);
            $('#popupbox').find(".modal-dialog").width(logoWidthSign);
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

$('.resp-tabs-list li').click(function (e, c) {
    var $this = $(this);
    addRules();
    isValid = $("#UserDetails").valid();
});

$("#AddressID").change(function () {
    var addressID = $('#AddressID option:selected').val();
    POAddress(addressID);
});


function changeTheme(color, val) {
    $('.theme-colors').find('button').not(color).removeClass('active');
    $('.theme-colors').find('[id=' + color + ']').addClass('active');
    $('#hiddenTheme').val(val);
}
$('#chkSCA').change(function () {
    if ($(this).is(":checked")) {
        $('.chkSCASignUp').show();
    }
    else {
        $('.chkSCASignUp').hide();
    }
});


function getDropDownValue() {
    removeSolarElecticianValidation();
    if (validateForm()) {
        SetSignature();
        return true;
    }
    else {
        return false;
    }
};

function checkExist(obj, title) {
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    if (fieldName == "CompanyABN") {
        if (sessionUserTypeId == 2) {
            if (uservalue != "" && uservalue != undefined) {
                $.ajax({
                    url: BaseURL + 'CheckUserExist/User',
                    data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        if (data == false) {
                            chkvar = false;
                            $(".alert").hide();
                            $("#errorMsgRegion").html(closeButton + "User with same " + title + " already exists.");
                            $("#errorMsgRegion").show();

                        }
                        else {
                            chkvar = true;
                        }
                        if (fieldName == "UserName") {
                            chkUserName = chkvar;
                        }
                        else if (fieldName == "CompanyABN") {
                            chkCompanyABN = chkvar;
                        }
                        else if (fieldName == "CECAccreditationNumber") {
                            chkCECAccreditationNumber = chkvar;
                        }
                        else if (fieldName == "LoginCompanyName") {
                            chkLoginCompanyName = chkvar;
                        }
                        return false;
                    }
                });
            }
        }
    }
    else {
        if (uservalue != "" && uservalue != undefined) {
            $.ajax(
                {
                    url: BaseURL + 'CheckUserExist/User',
                    data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        if (data == false) {
                            chkvar = false;
                            $(".alert").hide();
                            $("#errorMsgRegion").html(closeButton + "User with same " + title + " already exists.");
                            $("#errorMsgRegion").show();

                        }
                        else {
                            chkvar = true;
                        }
                        if (fieldName == "UserName") {
                            chkUserName = chkvar;
                        }
                        else if (fieldName == "CompanyABN") {
                            chkCompanyABN = chkvar;
                        }
                        else if (fieldName == "CECAccreditationNumber") {
                            chkCECAccreditationNumber = chkvar;
                        }
                        else if (fieldName == "LoginCompanyName") {
                            chkLoginCompanyName = chkvar;
                        }
                        return false;
                    }
                });
        }
    }
}

function addRules() {
    if (sessionUserTypeId == 7 || sessionUserTypeId == 4) {
        $("#CECAccreditationNumber").rules("add", {
            required: true,
            messages: {
                required: "CEC Accreditation Number is required."
            }
        });
    }

    if (sessionUserTypeId == 4 || sessionUserTypeId == 6 || sessionUserTypeId == 8) {
        removeValidationOnTabChangeInstallerDesigner();
    }

    $("#Password").rules("add", {
        required: false,
    });

    if ($("#chkSCA").is(':checked') == true) {
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
$('#imgViewLogo').click(function () {
    $("#loading-image").css("display", "");
    $('#imgLG').attr('src', SRCLG).load(function () {
        logoWidthLG = this.width;
        logoHeightLG = this.height;

        $('#popupboxlogo').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeightLG) {
            //$("#imgLG").height($(window).height() - 205);

            $("#imgLG").closest('div').height($(window).height() - 205);
            $("#imgLG").closest('div').css('overflow-y', 'scroll');
            $("#imgLG").height(logoHeightLG);
        }
        else {
            $("#imgLG").height(logoHeightLG);
            $("#imgLG").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthLG || logoWidthLG >= screen.width - 250) {
            $('#popupboxlogo').find(".modal-dialog").width(screen.width - 250);
            $("#imgLG").width(logoWidthLG);
        }
        else {
            $("#imgLG").width(logoWidthLG);
            $('#popupboxlogo').find(".modal-dialog").width(logoWidthLG);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgLG").unbind("error");
    $('#imgLG').attr('src', SRCLG).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
});
function SetSignature() {
    $("#EmailSignature").val(CKEDITOR.instances['editor'].getData());
}
$('#imgViewConfigre').click(function () {
    $('#myModal1').modal({ backdrop: 'static', keyboard: false });
});
function addressValidation() {
    $("#Password").rules("add", {
        required: false,
    });
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