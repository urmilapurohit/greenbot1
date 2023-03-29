$(document).ready(function () {

    if (sessionUserTypeId == '7') {
        $('#IsPVDUser').attr("disabled", true);
        $("[name='IsPVDUser'][type='hidden']").val(isPVDUser)
        $('#IsSWHUser').attr("disabled", true);
        $("[name='IsSWHUser'][type='hidden']").val(isSWHUser)
    }

    if (isRECLogin.toString().toLowerCase() == "true") {
        $("#RecUsernamePwd").show();
    }
    else {
        $("#RecUsernamePwd").hide();
    }

    //$("#RecUsernamePwd").hide();
    //$('.viewDetail').show();
    //FillDropDown('UnitTypeId', getUnitTypeURL, unitTypeID, true, null);
    //FillDropDown('StreetTypeId', getStreetTypeURL, streetTypeId, true, null);
    //FillDropDown('PostalAddressID', getPostalAddressURL, postalAddressID, true, null);

    dropDownData.push({ id: 'UnitTypeId', key: "UnitType", value: unitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', key: "StreetType", value: streetTypeId, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'PostalAddressID', key: "PostalAddress", value: postalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });


    $('#t4').show();
    $('.tab4').show();
    $('#t5').show();
    $('.tab5').show();
    $("#ConfigurationPassword").val(ConfigurationPassword);

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
    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });
    $("#btnClosePopUpBoxSE").click(function () {
        $('#popupboxSE').modal('toggle');
    });
    $("#btnCloseConfigure").click(function () {
        $('#myModal1').modal('toggle');
    });
    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('toggle');
    });
    $("#btnClosepopupboxlogo").click(function () {
        $('#popupboxlogo').modal('toggle');
    });
    $("#btnClosePopUpBoxSelfie").click(function () {
        $('#popupboxSelfie').modal('toggle');
    });
    //$("#btnClosePopUpBoxInsallerDesigner").click(function(){
    //    $('#popupboxInsallerDesigner').modal('toggle');
    //});
    if (sessionLogo != null && sessionTheme != null && sessionLogo != '' && sessionTheme != '') {
        $('#imgLogo').attr('src', (sessionSiteUrlBase + "UserDocuments" + "/" + sessionLogo));
        $('body').attr('id', sessionTheme);
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
    //if(UserTypeId == 8){
    //    $('.SCOPro').show();
    //}
    //if(UserTypeId == 7 || UserTypeId == 9 || UserTypeId == 2){
    //    $('.SCOPro1').show();
    //}
    if (strFromDate == "" || strFromDate == "0001-01-01") {
        $("#fromDate").val("");
    }
    if (strToDate == "0001-01-01" || strToDate == "") {
        $("#toDate").val("");
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
                //$('.form-box').find('input:first').focus();
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
                if (viewBagSignature != '') {
                    var data = vSign;
                    CKEDITOR.instances['editor'].setData(data.Data.Signature);
                }
            }
            //Rec email changes
            if (activeTab == 't5') {
                initSample_Rec();
                if (viewBagRECEmailSign != '') {
                    var data = vRecEmailSign;
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
                        $('#instDesignerAddedByProfile').html(result);
                    }
                });

            }
        }
    });

    GetCompanyNameFromCompanyABN(companyABN, $('#CompanyName'), companyName, false);
    if (modelUserTypeId == 2) {
        GetCompanyNameFromCompanyABN(wholesalerCompanyABN, $('#WholesalerCompanyName'), wholesalerCompanyName, true);
    }

    var userTypeId = sessionUserTypeId;
    if (userTypeId != 0) {
        ChangeUserType(modelUserTypeId);
    }

    if (userTypeId == 4) {
        $('#CompanyABN').prop("readonly", true);
    }

    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });

    $('#btnCancelLast').hide();
    $('#btnCancel').hide();
    $('.chkActive').hide();
    $('.viewPre').show();
    addressID = addressId;
    POAddress(addressID);

    $("#AddressID").val(parseInt(addressId));
    if ($('#UsageType').val() == "2") {
    wholesalerAddressId = wholesalerIsPostalAddress;
    POWholesalerAddress(wholesalerAddressId);
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
if ($('#imgSign').attr('src') != "") {
    if (modelSign != "") {
        var SignName = $('#imgSign').attr('src');
        var guid = USERID;
        var proofDocumentURL = sessionUploadedDocumentPath;
        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSign = SRC;
        $('#imgSign').attr('class', SignName);
        $('#imgSignature').show();
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
    if ($('#imgSign').attr('src') != "") {
        if (modelSign != "") {
            var SignName = $('#imgSign').attr('src');
            var guid = USERID;
            var proofDocumentURL = sessionUploadedDocumentPath;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSign = SRC;
            $('#imgSign').attr('class', SignName);
            $('#imgSignature').show();
        }
    }

    if ($('#imgSelfieImage').attr('src') != "") {
        if (oldFileNameSelfie != "") {
            var SignName = $('#imgSelfieImage').attr('src');
            var guid = USERID;
            var proofDocumentURL = sessionUploadedDocumentPath;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSelfie = SRC;
            $('#imgSelfieImage').attr('class', SignName);
            $('#imgSelfie').show();
        }
    }


    if ($('#imgLG').attr('src') != "") {
        if (sessionLogo != 'Images/logo.png') {
            var SignName = $('#imgLG').attr('src');
            var guid = USERID;
            var proofDocumentURL = sessionUploadedDocumentPath;
            var SRC = proofDocumentURL + "UserDocuments" + "/" + SignName;
            SRCLG = SRC;
            $('#imgLG').attr('class', SignName);
            $('#imgViewLogo').show();
        }
    }
    if (userTypeId == 8) {
        $('.Profile').show();
        $('.SCOProfile').hide();
    }
    if (userTypeId == 2) {
        $('.SCOProfile').show();
        $('.ProfileSCO').show();
        $('.WS').hide();
    }
    if (userTypeId == 4) {
        $('.SCOProfile').show();
        $('.ProfileSCO').show();
        $('.SCAPrev').show();
    }
    if (userTypeId == 9) {
        $('.SCProfile').hide();
        $('.SCO').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
    }
    if (userTypeId == 7) {
        $('.SCProfile').hide();
        $('.SCO').hide();
        $('.SC').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
    }
    if (userTypeId == 6) {
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
            $('.chkSCASignUp').hide();
        }
        else {
            $('.chkSCASignUp').show();
        }
    }
    if (sessionUserTypeId == 4) {
        if (chkSTC == 0) {
            $('.chkisSTC').hide();
        }
        else {
            $('.chkisSTC').show();
        }
    }

    if (sessionUserTypeId == 7) {
        $('.hideSESWH').hide();
    }
    if (sessionUserTypeId != 10) {
        $('.SWH').hide();
    }

    var flg = tempDataFlag;
    if (flg != null && flg.toLowerCase() == "docupload") {
        $("#t1").hide();
        $("#t2").hide();
        $("#t4").hide();
        $("#t9").hide();
        $("#cecDoc").hide();
    }
    dropDownData.bindDropdown();

}
});
function validateForm() {
    //addRules();
    if (modelUserTypeId == 2 || modelUserTypeId == 5) {
        var isExistCode = CheckClientCodePrefix($("#ClientCodePrefix").val());
        if (isExistCode == 0)
            return false;
    }

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
    $('.form-box').find('input:first').focus();
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
        $('.showWholeSaler').show();
        ShowHideWholeSalerInvoice();
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
        //$('#lblCECAccreditationNumber').addClass("required");
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
    SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSign').attr('src', SRCSign);
    $('#imgSign').load(function () {
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

function addRules() {

    if (sessionUserTypeId == 4) {
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

function getDropDownValue() {
    removeSolarElecticianValidation();
    if (sessionUserTypeId == 7) {
        $('#IsAutoRequest').val($('#AutoRequestSwitch').prop('checked'));
    }
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

                        if (data.status == false) {
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
                        return true;
                    }
                });
            }
        }
    }
    else {


        var sID = 0;
        if (fieldName.toLowerCase() == "installerdesignerview_cecaccreditationnumber")
            sID = GetSolarCompanyIdforInstallerDesigner();

        if (uservalue != "" && uservalue != undefined) {
            $.ajax(
                {
                    url: BaseURL + 'CheckUserExist/User',
                    data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID, solarCompanyId: parseInt(sID) },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {


                        if (data.status == false) {
                            var errorMsg;
                            if (fieldName.toLowerCase() == "installerdesignerview_cecaccreditationnumber")
                                errorMsg = data.message;
                            else
                                errorMsg = "User with same " + title + " already exists."

                            chkvar = false;
                            $(".alert").hide();
                            $("#errorMsgRegion").html(closeButton + errorMsg);
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

                        return true;
                    }
                });
        }
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
    //if (SignName != null && SignName != "") {
    //    DeleteFileFromUserOnCancel(SignName, guid);
    //}

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
            //$("#imgLG").width(screen.width - 10);
            //$('#popupboxlogo').find(".modal-dialog").width(screen.width - 10);

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

$("#btnSubmit").submit(function () {
    SetSignature();
});


function SetSignature() {
    $("#EmailSignature").val(CKEDITOR.instances['editor'].getData());
}
$('#imgViewConfigre,.helpic').click(function () {
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

    if ($('#UsageType').val() == "2" && sessionUserTypeId == 2) {

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
    if ($('#UsageType').val() == "3" && sessionUserTypeId == 2) {

        $("#InvoicerUnitTypeID").rules("add", {
            required: false,
        });
        $("#InvoicerUnitNumber").rules("add", {
            required: false,
        });
        if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length == 0) {
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
            $("#InvoicerUnitNumber").next("span").attr('class', 'field-validation-valid');
            $('#lblInvoicerStreetNumber').addClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }

        if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length != 0) {
            $('#lblInvoicerStreetNumber').removeClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: false,
            });
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
        }
        if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length == 0) {
            $("#InvoicerUnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $('#lblInvoicerUnitNumber').addClass("required");
            $('#lblInvoicerStreetNumber').removeClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: false,
            });
        }
        if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length != 0) {
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
            $('#lblInvoicerStreetNumber').addClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }
    }
}

$('#UsageType').change(function () {
    ShowHideWholeSalerInvoice();
    $('.WS').hide();
});

function DownloadContractDocument(e) {
    var folderName = e.id;
    folderName = folderName + "\\" + "SAAS";

    window.location.href = BaseURL + 'ViewDownloadFile/User?FileName=' + e.className + '&FolderName=' + folderName;
}

function DeleteContractFileFromFolder(fileDelete) {
    var FolderName = $("#Invoicer").val();
    fileDelete = "SAAS" + "\\" + fileDelete;
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
            {
                url: BaseURL + 'DeleteContractFileFromFolder/User',
                data: { fileName: fileDelete, FolderName: FolderName },
                method: 'get',
                success: function () {

                    $("#SAASContractPath").html("");
                    $(".alert").hide();
                    $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
                    $("#successMsgRegion").show();
                }
            });
    }
}
