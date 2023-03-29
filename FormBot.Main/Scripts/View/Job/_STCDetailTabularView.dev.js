function showhideAdditionalLocationInformation() {
    if ($("#JobSTCDetails_Location").val() != "") {
        if ($("#JobSTCDetails_MultipleSGUAddress").val() != "No" && $("#JobSTCDetails_MultipleSGUAddress").val() != "") {
            $("#STCAdditionalLocation").show();
        }
        else {
            $("#JobSTCDetails_AdditionalLocationInformation").text('');
            $("#STCAdditionalLocation").hide();
        }
    }
    else {
        $("#JobSTCDetails_AdditionalLocationInformation").text('');
        $("#STCAdditionalLocation").hide();
    }
}

function showhideFailedCode(modelSTCCertificateCreated) {
    debugger;
    if (modelSTCCertificateCreated == "" || modelSTCCertificateCreated == "No") {
        $(".DivFailedAccreditationCode").hide();
        $(".failedReasonDiv").hide();
        $("#JobSTCDetails_FailedReason").val('');
        $("#JobSTCDetails_FailedAccreditationCode").val('');
    } else if (modelSTCCertificateCreated == "Yes") {
        $(".DivFailedAccreditationCode").show();
        $(".failedReasonDiv").show();
    }
}

function CompareJobData() {
    return true;
}

function showMessageForSTC(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegionSTCStatus" : "successMsgRegionSTCStatus";
    var inVisible = isError ? "successMsgRegionSTCStatus" : "errorMsgRegionSTCStatus";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}

function GetSTCSettlementDate(settlementterm) {
    $.ajax({
        url: urlGetSTCSettlementDate,
        type: "GET",
        data: { settlementTerm: settlementterm, CustomSettlementTerm: $("#CustomSettlementTerm").val() },
        success: function (Data) {

            if (Data.status) {
                $("#selectedSettlementTerm").html(Data.Days + " Settlement Selected");
                if (Data.SettlementDate != null && Data.SettlementDate != "") {
                    $("#setSettlementDate").html(moment(Data.SettlementDate).format(dateFormat.toUpperCase()));
                }
                else {
                    $("#setSettlementDate").html('');
                }
            }
        }
    });
}

function DisableTradeButton() {
    $('#btnApplyTradeStc').addClass('default');
    $('#btnApplyTradeStc').removeClass('primary');
    $('#btnApplyTradeStc').css('pointer-events', 'none');
    $(".STC-summary").addClass('lock');
}

function EnableTradeButton() {
    $('#btnApplyTradeStc').addClass('primary');
    $('#btnApplyTradeStc').removeClass('default');
    $('#btnApplyTradeStc').css('pointer-events', '');
    $(".STC-summary").addClass('unlock');
}

function calculateTotal(obj) {
    $('#pricingTerm').html(obj.data("price"));
    GetSTCSettlementDate(obj.data('settlementterm'));
}

function deemingPeriodCallBack(deemingPeriod) {
    if (modelSTCDeemingPeriod == 0 || modelSTCDeemingPeriod == null || modelSTCDeemingPeriod == '') {
        $("#JobSTCDetails_DeemingPeriod").val($("#JobSTCDetails_DeemingPeriod option:last").val());
    }
}

function ShowHideGSTSection(propertyType, ownerType) {
    if (tempIsRegisteredWithGST == 2 && modelIsRegisteredWithGST == 'true') {
        $(".isGSTRegistered").show();
        $("#jobGST").show();
        $("#BasicDetails_IsGst").val(true);
    }
    else if (tempIsRegisteredWithGST == 1 && modelIsRegisteredWithGST == 'true') {// && (propertyType == "commercial" || propertyType == "school" || ((ownerType == 'corporate body' || ownerType == 'trustee') && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true'))) {
        if (ownerType == 'corporate body' || ownerType == 'trustee') {
            if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                $(".isGSTRegistered").show();
                $("#jobGST").show();
                $("#BasicDetails_IsGst").val(true);
            }
            else {
                $(".isGSTRegistered").hide();
                $("#BasicDetails_IsGst").val(false);
            }
        }
        else if (propertyType == "commercial" || propertyType == "school") {
            $(".isGSTRegistered").show();
            $("#jobGST").show();
            $("#BasicDetails_IsGst").val(true);
        }
        else {
            $(".isGSTRegistered").hide();
            $("#BasicDetails_IsGst").val(false);
        }

    }
    else {
        $(".isGSTRegistered").hide();
        $("#BasicDetails_IsGst").val(false);
        $("#BasicDetails_IsGst").val(false);
    }
}

$("#JobInstallationDetails_PropertyType").on('focus', function () {
    // Store the current value on focus and on change
    previousPropertyType = this.value;
}).change(function (e) {
    JobInstallationDetails_PropertyType = $("#JobInstallationDetails_PropertyType").val(); /* Set value in master page variable for value required in interdependent tabs*/
    var propertyType = $("#JobInstallationDetails_PropertyType").val();

    if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for commercial jobs.");
        $("#JobInstallationDetails_PropertyType").val(previousPropertyType);
        return false;
    }
    if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for installtion property type school.");
        $("#JobInstallationDetails_PropertyType").val(previousPropertyType);
        return false;
    }
    ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), jobownerdetails_ownertypestc);
    $.ajax({
        url: urlUpdateJobInstallationPropertyType,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'post',
        data: JSON.stringify({ jobId: jobId, propertyType: $('#JobInstallationDetails_PropertyType').val(), isGST: $("#BasicDetails_IsGst").val() }),
        async: false,
        success: function (response) {
            if (response.status) {
                showSuccessMessageForPopup("Installation property type has been saved successfully.", $("#successMsgRegionSTCDetails"), $("#errorMsgRegionSTCDetails"));
                //SavedData = CommonDataForSave();
                ReloadSTCJobScreen(jobId);
                // SearchHistory();
            }
            else
                showErrorMessageForPopup(response.msg, $("#errorMsgRegionSTCDetails"), $("#successMsgRegionSTCDetails"));
        }
    });
});

function showSuccessMessageForPopup(message, objSuccess, objError) {
    debugger;
    $(".alert").hide();
    if (objError)
        objError.hide();
    objSuccess.html(closeButton + message);
    objSuccess.show();
}

function ReloadSTCJobScreen(jobId) {
    ReloadSTCModule();
}

function ReloadSTCModule() {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlGetSTCJobForTabular + BasicDetails_JobID, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                debugger;
                $("#CESDocbtns").hide();
                $("#STCDocBtns").hide();
                $('.isdelete').css("display", "none");
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 4 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 5) {
                $("#CESDocbtns").show();
                $("#STCDocBtns").show();
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                $('.isdelete').css("display", "block");
            }
            debugger;
            if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $('#btnsave').hide();
            }

            $("#loading-image").hide();
        });
    }, 500);
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function CommonDataForSave() {
    debugger;
    if (oldaddress == null || oldaddress == "") {
        oldaddress = $("#JobInstallationDetails_AddressDisplay").val();
        $("#JobInstallationDetails_oldInstallationAddress").val(oldaddress);
    }

    EnableDropDownbyUsertype();
    $("#BasicDetails_JobType").removeAttr("disabled");
    var panelData = '';
    var xmlPanel = '';
    var inverterData = '';
    var xmlInverter = '';

    var batteryManufacturerData = [];
    batteryManufacturerData = JSON.parse(JSON.stringify(batteryXml));

    if (PanelXml.length > 0) {
        var jsonp = JSON.stringify(PanelXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<panel><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
                '<NoOfPanel>' + this['Count'] + '</NoOfPanel><Supplier>' + htmlEncode(this['Supplier']) + '</Supplier></panel>';
        });
        xmlPanel = '<Panels>' + sName + '</Panels>'
    }
    if (InverterXml.length > 0) {

        var jsonp = JSON.stringify(InverterXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<inverter><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
                '<Series>' + htmlEncode(this['Series']) + '</Series><NoOfInverter>' + htmlEncode(this['NoOfInverter']) + '</NoOfInverter></inverter>';
        });
        xmlInverter = '<Inverters>' + sName + '</Inverters>';
    }

    $("#panelXml").val(xmlPanel);
    $("#inverterXml").val(xmlInverter);
    $("#OldPanelDetails").val(JSON.stringify(OldPanelXml));
    $("#NewPanelDetails").val(JSON.stringify(PanelXml));
    $("#OldInverterDetails").val(JSON.stringify(OldInverterXml));
    $("#NewInverterDetails").val(JSON.stringify(InverterXml));

    // Find disabled inputs, and remove the "disabled" attribute
    var disabled = $('form').find(':input:disabled').removeAttr('disabled');

    //serialize form
    var data = JSON.stringify($('form').serializeToJson());

    // re-disabled the set of inputs that you previously enabled
    disabled.attr('disabled', 'disabled');
    var objData = JSON.parse(data);
    if (batteryXml.length == 0 || batterySystemPartOfAnAggregatedControl_Glbl.toLowerCase() == 'select') {
        objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = '';
    } else {
        objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = batterySystemPartOfAnAggregatedControl_Glbl;
    }
    if (batteryXml.length == 0 || changedSettingOfBatteryStorageSystem_Glbl.toLowerCase() == 'select') {
        objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = '';
    } else {
        objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = changedSettingOfBatteryStorageSystem_Glbl;
    }

    var customLength = $("#customDetails").find('.spanCustomFields').length;

    for (var i = 0; i < customLength; i++) {
        delete objData["lstCustomDetails[" + i + "]"];
    }
    if (batteryManufacturerData.length > 0) {

        for (var i = 0; i < batteryManufacturerData.length; i++) {
            delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
        }
        objData.lstJobBatteryManufacturer = batteryManufacturerData;
    }

    // Find disabled inputs, and remove the "disabled" attribute
    var disabledOwnerData = $('#JobOwner').find(':input:disabled').removeAttr('disabled');

    //serialize form
    var ownerData = JSON.stringify($('#JobOwner').serializeToJson());

    // re-disabled the set of inputs that you previously enabled
    disabledOwnerData.attr('disabled', 'disabled');

    objData.JobOwnerDetails = JSON.parse(ownerData).JobOwnerDetails;

    //merge installation detail
    var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
    var objInstallationAddressData = JSON.parse(installationAddressData);
    installationAddressChangeCheck(objInstallationAddressData.JobInstallationDetails);
    var installationData = JSON.stringify($('#JobInstallationDetailInfo').serializeToJson());
    var objInstallationData = JSON.parse(installationData);

    var extraInstallationData = JSON.stringify($('#JobExtraInstallationInfo').serializeToJson());
    var objExtraInstallationData = JSON.parse(extraInstallationData);

    $.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);
    $.extend(objData.JobInstallationDetails, objInstallationData.JobInstallationDetails);
    $.extend(objData.JobInstallationDetails, objExtraInstallationData.JobInstallationDetails);

    objData.JobInstallationDetails.Latitude = $("#lblLatitude").text();
    objData.JobInstallationDetails.Longitude = $("#lblLongitude").text();
    var jobSystemDetails = objData.JobSystemDetails;
    jobSystemDetails.ModifiedCalculatedSTC = $("#BasicDetails_JobType").val() == 1 ? $("#JobSystemDetails_CalculatedSTC").val() : $("#JobSystemDetails_CalculatedSTCForSWH").val();
    jobSystemDetails.NoOfPanel = $("#JobSystemDetails_NoOfPanel").html();
    jobSystemDetails.NoOfInverter = $("#JobSystemDetails_NoOfInverter").html();
    jobSystemDetails.InstallationType = $("#JobSystemDetails_InstallationType").val();
    if (systemXml.length > 0) {
        jobSystemDetails.SystemBrand = systemXml[0].Brand;
        jobSystemDetails.SystemModel = systemXml[0].Model;
    }
    objData.JobSystemDetails = jobSystemDetails;

    var lstCustomDetails = [];
    $("#customDetails").find('.spanCustomFields').each(function () {
        lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).text(), SeparatorId: $(this).attr('data-SeperatorId') });
    });

    objData.lstCustomDetails = lstCustomDetails;
    objData.OldStcValue = $('#OldStcValue').val() == "" ? 0 : $('#OldStcValue').val();
    var stcHistoryMsg = 'Due to change in ';
    var isStcHistoryMsg = false;
    objData.JobSystemDetails.PreviousSystemSize = $('#OldSystemSize').val();
    if ($('#OldSystemSize').val() != $('#JobSystemDetails_SystemSize').val()) {
        stcHistoryMsg = stcHistoryMsg + 'System Size from ' + $('#OldSystemSize').val() + ' to ' + $('#JobSystemDetails_SystemSize').val();
        isStcHistoryMsg = true;
    }
    debugger;
    var OldInstallationDate = moment($('#BasicDetails_strInstallationDateTemp').val()).format('dd/mm/yyyy'.toUpperCase());
    //var OldInstallationDate = $('#BasicDetails_strInstallationDateTemp').val();
    objData.BasicDetails.strInstallationDateTemp = OldInstallationDate;
    if (OldInstallationDate != $('#BasicDetails_strInstallationDate').val()) {
        stcHistoryMsg = stcHistoryMsg + (isStcHistoryMsg ? ', ' : '') + 'Installation Date from ' + OldInstallationDate + ' to ' + $('#BasicDetails_strInstallationDate').val();
        isStcHistoryMsg = true;
    }
    if ($('#OldDeemingPeriod').val() != $('#JobSTCDetails_DeemingPeriod').val()) {
        stcHistoryMsg = stcHistoryMsg + (isStcHistoryMsg ? ', ' : '') + 'Deeming Period from ' + $('#OldDeemingPeriod').val() + ' to ' + $('#JobSTCDetails_DeemingPeriod').val();
    }
    var CheckInstallationDate = moment($('#BasicDetails_strInstallationDate').val(), 'DD/MM/YYYY').add(325, 'd').format('dd/mm/yyyy'.toUpperCase());
    var currentdate = moment().format('DD/MM/YYYY');
    //Check installation date expiry
    if (moment(CheckInstallationDate, 'DD/MM/YYYY') < moment(currentdate, 'DD/MM/YYYY')) {
        objData.IsApproachingExpiryDate = true;
    }
    else {
        objData.IsApproachingExpiryDate = false;
    }
    objData.PropertyType = $('#JobInstallationDetails_PropertyType').val();

    objData.stcHistoryMsg = stcHistoryMsg + '.';
    objData.stcStatusId = $('#STCStatusId').val();
    var PreviousRefNumber = $('#BasicDetails_PreviousRefNumber').val();
    objData.BasicDetails.PreviousRefNumber = PreviousRefNumber;
    var PreviousSSCID = $('#BasicDetails_PreviousSSCID').val();
    objData.BasicDetails.PreviousSSCID = PreviousSSCID;
    var PreviousSCOID = $('#BasicDetails_PreviousSCOID').val();
    objData.BasicDetails.PreviousSCOID = PreviousSCOID;
    var PreviousNMI = $('#JobInstallationDetails_PreviousNMI').val();
    objData.JobInstallationDetails.PreviousNMI = PreviousNMI;
    var PreviousJobStage = $('#BasicDetails_PreviousJobStage').val();
    objData.BasicDetails.PreviousJobStage = PreviousJobStage;
    var PreviousPriority = $('#BasicDetails_PreviousPriority').val();
    objData.BasicDetails.PreviousPriority = PreviousPriority;
    var PreviousInstallingNewPanel = $("#JobInstallationDetails_PreviousInstallingNewPanel").val();
    objData.JobInstallationDetails.PreviousInstallingNewPanel = PreviousInstallingNewPanel;
    debugger;
    var PreviousInstallationLocation = $("#JobInstallationDetails_PreviousLocation").val();
    objData.JobInstallationDetails.PreviousLocation = PreviousInstallationLocation;
    var PreviousSingleMultipleStory = $("#JobInstallationDetails_PreviousSingleMultipleStory").val();
    objData.JobInstallationDetails.PreviousSingleMultipleStory = PreviousSingleMultipleStory;
    var PreviousSystemMountingType = $("#JobSTCDetails_PreviousSystemMountingType").val();
    objData.JobSTCDetails.PreviousSystemMountingType = PreviousSystemMountingType;
    var PreviousConnectionType = $("#JobSTCDetails_PreviousConnectionType").val();
    objData.JobSTCDetails.PreviousConnectionType = PreviousConnectionType;
    var PreviousMultipleSGUAddress = $("#JobSTCDetails_PreviousMultipleSGUAddress").val();
    objData.JobSTCDetails.PreviousMultipleSGUAddress = PreviousMultipleSGUAddress;
    var PreviousDeemingPeriod = $("#JobSTCDetails_PreviousDeemingPeriod").val();
    objData.JobSTCDetails.PreviousDeemingPeriod = PreviousDeemingPeriod;
    var PreviousCertificateCreated = $("#JobSTCDetails_PreviousCertificateCreated").val();
    objData.JobSTCDetails.PreviousCertificateCreated = PreviousCertificateCreated;
    var PreviousFailedAccreditationCode = $("#JobSTCDetails_PreviousFailedAccreditationCode").val();
    objData.JobSTCDetails.PreviousFailedAccreditationCode = PreviousFailedAccreditationCode;
    var PreviousFailedReason = $("#JobSTCDetails_PreviousFailedReason").val();
    objData.JobSTCDetails.PreviousFailedReason = PreviousFailedReason;
    var PreviousSTCLocation = $("#JobSTCDetails_PreviousLocation").val();
    objData.JobSTCDetails.PreviousLocation = PreviousSTCLocation;
    var PreviousAdditionalLocationInformation = $("#JobSTCDetails_PreviousAdditionalLocationInformation").val();
    objData.JobSTCDetails.PreviousAdditionalLocationInformation = PreviousAdditionalLocationInformation;
    var PreviousVolumetricCapacity = $("#JobSTCDetails_PreviousVolumetricCapacity").val();
    objData.JobSTCDetails.PreviousVolumetricCapacity = PreviousVolumetricCapacity;
    var PreviousStatutoryDeclarations = $("#JobSTCDetails_PreviousStatutoryDeclarations").val();
    objData.JobSTCDetails.PreviousStatutoryDeclarations = PreviousStatutoryDeclarations;
    var PreviousSecondhandWaterHeater = $("#JobSTCDetails_PreviousSecondhandWaterHeater").val();
    objData.JobSTCDetails.PreviousSecondhandWaterHeater = PreviousSecondhandWaterHeater;
    var PreviousLatitude = $("#JobSTCDetails_PreviousLatitude").val();
    objData.JobSTCDetails.PreviousLatitude = PreviousLatitude;
    var PreviousLongitude = $("#JobSTCDetails_PreviousLongitude").val();
    objData.JobSTCDetails.PreviousLongitude = PreviousLongitude;
    var PreviousAdditionalSystemInformation = $("#JobSTCDetails_PreviousAdditionalSystemInformation").val();
    objData.JobSTCDetails.PreviousAdditionalSystemInformation = PreviousAdditionalSystemInformation;
    var jobData = JSON.stringify(objData);
    return jobData;
}

function EnableDropDownbyUsertype() {
    if (JOBType == '1') {
        if (USERType == 8 && IsUnderSSC == "True") {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
        if (USERType == 7 || USERType == 9) {

            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
        }
        if (USERType == 2 || USERType == 5) {

            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }

    if (JOBType == '2') {

        if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }

        if (USERType == 2 || USERType == 5) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#Installer").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }

    if (JOBType == 1) {
        if (IsLockedSerialNumber == 'true') {
            debugger;
            $("#aLockedSerialnumber").show();
            $("#aUnLockedSerialnumber").hide();
            $("#JobSystemDetails_SerialNumbers").prop('disabled', true);
        }
        else {
            debugger;
            if (IsLockedSerialNumber == 'false' && (USERType == 1 || USERType == 3 || IsLockUnlockSerial == "True")) {
                $("#aUnLockedSerialnumber").show();
            }
            $("#aLockedSerialnumber").hide();
            $('#JobSystemDetails_SerialNumbers').prop('disabled', false);
        }
    }
    else {
        $("#aLockedSerialnumber").hide();
        $("#aUnLockedSerialnumber").hide();
    }

    if (USERType == 1
        || USERType == 3
        || $('#STCStatusId').val() == 10
        || $('#STCStatusId').val() == 12
        || $('#STCStatusId').val() == 14
        || $('#STCStatusId').val() == 17) {
        $('#JobOwnerDetails_OwnerType').prop('disabled', false);
        $('#JobInstallationDetails_PropertyType').prop('disabled', false);
        $('#JobSystemDetails_SystemSize').prop('disabled', false);
        $('#BasicDetails_strInstallationDate').prop('disabled', false);
    }
    else {
        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
        $('#JobSystemDetails_SystemSize').prop('disabled', true);
        $('#BasicDetails_strInstallationDate').prop('disabled', true);
        $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
    }
}

function callbackCheckList() {
    debugger;
    if ($('#checkListItemForTrade').find('ul').find('li').length > 0)
        $('#checkListItemForTrade').show();
    else
        $('#checkListItemForTrade').hide();
}

function checkSwitchStatus() {
    debugger;
    var isChecked = false;
    $(".ChecklistPoint").each(function () {
        if ($(this).find("i").hasClass('active')) {
            isChecked = true;
        }
        else {
            isChecked = false;
            return false;
        }

    });
    if (isChecked) {
        $("#onoffswitchCheckAllPoints").prop('checked', true);
    }
    else {
        $("#onoffswitchCheckAllPoints").prop('checked', false);
    }
}

function checkListFirTradeOnLoad() {
    $("#onoffswitchCheckAllPoints").change(function (ev) {
        var checked = ev.target.checked;
        if (checked) {
            $(".ChecklistPoint").each(function () {
                if (!$(this).find("i").hasClass('active')) {
                    $(this).click();
                }
            });
        }
        else {
            $(".ChecklistPoint").each(function () {
                if ($(this).find("i").hasClass('active')) {
                    $(this).click();
                }
            });
        }

        checkSwitchStatus();
    });
}

function MarkUnMarkItemForTrade(event, obj, templateId, itemId, isCompleted, visitedCount, totalCount, jobSchedulingId, classTypeId, itemName) {
    var isMark = $(obj).find("i").hasClass('active');
    debugger;
    if (classTypeId == 1 && parseInt(visitedCount) < parseInt(totalCount) && !isMark && !itemName.toLowerCase().includes("inverter")) {
        alert('Please capture all serial numbers.');
        event.preventDefault();
        return false;
    }

    $.ajax(
        {
            url: urlMarkUnMarkCheckListItem,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { templateId: templateId, itemId: itemId, isMark: !isMark, jobSchedulingId: jobSchedulingId },
            success: function (response) {
                if (response.id && response.id > 0) {
                    if (!isMark) {
                        $(obj).find("i").addClass('active');
                    }
                    else {
                        $(obj).find("i").removeClass('active');
                    }
                    showMessageForSTC("CheckList item has been marked/un-marked as completed successfully.");
                }
                else {
                    if (response.error.toLowerCase() == 'sessiontimeout')
                        window.location.href = urlLogout;
                    else {
                        if (response.error)
                            showMessageForSTC(response.error);
                        else
                            showMessageForSTC("CheckList item has not been marked/un-marked as completed.");
                    }
                }
                ReloadSTCJobScreen(jobId);
                checkSwitchStatus();
            },
            error: function () {
                showMessageForSTC("CheckList item has not been marked/un-marked as completed.");
            }
        });

    event.preventDefault();
    return false;
}

/* Added pricingsettelmentterm js code */
function PricingSettlementTermOnLoad() {
    if ((IsShowInDashboard == 'False' || IsTradedFromJobIndex == 'True')
        && (IsSubmissionScreen != 1)
        && (currentJobStatus != 0)
        && ((session_UserTypeId == 1 || session_UserTypeId == 3)
            ||
            (
                (session_UserTypeId == 2 || session_UserTypeId == 4 || session_UserTypeId == 5 || (session_UserTypeId == 8 && showbtn == 'True'))
                &&
                ((currentJobStatus == 12) || (currentJobStatus == 17) || (currentJobStatus == 14))
            )
        )
    ) {
        EnableTradeButton();
    }
    else {
        DisableTradeButton();
    }

    if ((($('#IsWholeSaler').val() && $("#IsWholeSaler").val().toLowerCase() == 'true') || (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "true")) && isWholesalerSCAPricingSettlementTermView == 'false') {
        $(".SettlementTerms").hide();
        $("#calculatedSTCParent").hide();
        $(".total-amonut").hide();
        $(".stc-amount").hide();
        $(".SettlementTermsCERApproved").hide();
    }
    else {
        $(".SettlementTerms").show();
        $("#calculatedSTCParent").show();
        $(".total-amonut").show();
        $(".stc-amount").show();
        $(".SettlementTermsCERApproved").show();
    }
    if (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "true" && IsForDashboardPricingWholesaler.toLowerCase() == "true") {
        $(".SettlementTerms").show();
        $("#calculatedSTCParent").show();
        $(".total-amonut").show();
        $(".stc-amount").show();
        $(".SettlementTermsCERApproved").show();
    }
    if (IsShowInDashboard == 'True') {
        $("#calculatedSTCParent").hide();
    }
    if ((session_UserTypeId != 1 && session_UserTypeId != 3) && (model_STCStatus != 10 && model_STCStatus != 12 && model_STCStatus != 14 && model_STCStatus != 17)) {
        $('#PricingBlock').hide();
        if (typeof (TabularView) != 'undefined') {
            $('.subTitleSTCPricing').hide();
        }
    }

    if (statucId != 12 && statucId != 17) {
        if (session_UserTypeId != 1) {
            $("ul.Processing-time li.LiSettlement").addClass("cursorDefault");
            $("ul.Processing-time li.LiSettlement").unbind("click");
        }
    }
    if (IsSubmissionScreen != 1 && model_IsDashboardPricing == 'false') {
        EnableDisableSettlementTerm();
    }

    $(".LiSettlement").each(function () {
        $(this).click(function () {
            SettlementTermClick($(this));
        });
        if ($(this).data('settlementterm') == settlementTerm)
            $(this).click();

        if (IsShowInDashboard == 'True') {
            if ($(this).attr("data-ispriceup") == 'True') {
                $(this).addClass('up-caret');
            }
            else {
                $(this).addClass('down-caret');
            }
        }
    });

    if (settlementTerm == "0")
        $(".LiSettlement:first").click();
    if (model_IsGridView == 'false') {
        x = document.createElement('div');
        x.innerHTML = Description;
        $("#lblDescription").html($(x).html());
        $("#lblLastUpdatedDate").html(lastUpdatedDate);
    }

    var visibleLiCount = $('.Processing-time').find('li').length;
    var liWidth = 0;
    if (visibleLiCount > 0) {
        liWidth = 100 / visibleLiCount + "%";
    }
    $('.Processing-time').find('li').css("width", liWidth);
}

function SettlementTermClick(obj) {
    var term = obj.attr('data-settlementterm');
    var custTerm = obj.children('#CustomSettlementTerm').val() > 0 ? obj.children('#CustomSettlementTerm').val() : 0;
    $(".LiSettlement").removeClass("active");
    if (obj.children('.ui-state-disabled').length > 0) {
        obj.parent().children('li').children("span:not(.ui-state-disabled)").parent('li:first').addClass('active');
    }
    else {
        obj.addClass("active");
    }

    if (model_STCStatus == 10 || model_STCStatus == 12 || model_STCStatus == 14 || model_STCStatus == 17) {
        calculateTotal(obj);
    }
}

function ApplyTradeStc() {
    debugger;
    var ownersignature = $("#imgOwnerSign").attr('src');
    if (ownersignature == null) {
        alert("Please Upload Owner Signature To Trade STC");
        return;
    }
    var interval = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("interval") : $(".LiSettlement").data("interval");
    var price = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("price") : $(".LiSettlement").data("price");
    var settlementterm = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("settlementterm") : $(".LiSettlement").data("settlementterm");

    if (settlementterm == 12) {
        peakPayTimePeriodValue = peakPayTimePeriod;
        peakPayGstRate = PeakPayGst;
        peakPayFeeValue = PeakPayFee;
    }
    if (settlementterm == 10 && customSettlementTerm == 12) {
        peakPayTimePeriodValue = peakPayCustomTimePeriod;
        peakPayGstRate = CustomPeakPayGst;
        peakPayFeeValue = CustomPeakPayFee;
    }

    if (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "false" && $(".LiSettlement.active").length == 0) {
        alert('Please select atleast one settlement term.');
        return;
    }
    else if ((BasicDetails_IsWholeSaler.toLowerCase() == "true" && $('.Processing-time').find('.LiSettlement').length == 1) || BasicDetails_IsWholeSaler.toLowerCase() == "false") {
        if (model_IsGridView == 'true') {
            var IsSamePrice = true;
            var IsFirst = true;
            var PriceDay1 = 0;
            var PriceDay3 = 0;
            var PriceDay7 = 0;
            var UpFront = 0;
            var PriceOnApproval = 0;
            var PartialPayment = 0;
            var RapidPay = 0;
            var OptiPay = 0;
            var Commercial = 0;
            var Custom = 0;
            var InvoiceStc = 0;
            var PeakPay = 0;
            var isReadyforTrade = true;

            var newselectedRows = [];
            $('#datatable1 tbody input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') == true) {
                    if (IsShowInDashboard == 'True') {
                        if ($(this).attr('IsTraded') != 'true' && $(this).attr('IsCustomPrice') != 'true') {
                            if ($(this).attr('IsReadyToTrade') == 'true') {
                                if (IsFirst) {
                                    PriceDay1 = parseFloat($(this).attr('PriceDay1'));
                                    PriceDay3 = parseFloat($(this).attr('PriceDay3'));
                                    PriceDay7 = parseFloat($(this).attr('PriceDay7'));
                                    PriceOnApproval = parseFloat($(this).attr('PriceOnApproval'));
                                    RapidPay = parseFloat($(this).attr('RapidPay'));
                                    OptiPay = parseFloat($(this).attr('OptiPay'));
                                    Commercial = parseFloat($(this).attr('Commercial'));
                                    Custom = parseFloat($(this).attr('Custom'));
                                    InvoiceStc = parseFloat($(this).attr('InvoiceStc'));
                                    PeakPay = parseFloat($(this).attr('PeakPay'));
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                else if (PriceDay1 != parseFloat($(this).attr('PriceDay1')) || PriceDay3 != parseFloat($(this).attr('PriceDay3')) || PriceDay7 != parseFloat($(this).attr('PriceDay7'))
                                    || PriceOnApproval != parseFloat($(this).attr('PriceOnApproval'))
                                    || RapidPay != parseFloat($(this).attr('RapidPay')) || OptiPay != parseFloat($(this).attr('OptiPay')) || Commercial != parseFloat($(this).attr('Commercial'))
                                    || Custom != parseFloat($(this).attr('Custom')) || InvoiceStc != parseFloat($(this).attr('InvoiceStc')) || PeakPay != parseFloat($(this).attr('PeakPay'))) {
                                    IsSamePrice = false;
                                    isReadyforTrade = false;
                                    alert('Please select jobs which have same price.');
                                    return false;
                                }
                                else {
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                IsFirst = false;
                            }
                            else {
                                IsSamePrice = false;
                                isReadyforTrade = false;
                                alert('Please select jobs which are ready to trade.');
                                return false;
                            }
                        }
                        else {
                            IsSamePrice = false;
                            isReadyforTrade = false;
                            alert('Please select jobs which are neither traded nor have custom price.');
                            return false;
                        }

                    }
                    else {
                        if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $(this).attr('isApproachingExpiryDate') == 'true') {
                            alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
                            isReadyforTrade = false;
                            return false;
                        }
                        else {
                            newselectedRows.push($(this).attr('JobId'));
                        }
                    }
                }
            });

            if (isReadyforTrade == true) {
                if (newselectedRows.length > 0) {
                    if (confirm("Are you sure you want to Trade STC?")) {
                        ApplyTradeActual(newselectedRows.join(','), interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue);//, IsGenerateRecZip);
                    }
                }
                else {
                    alert('Please select atleast one job.');
                }
            }
        }
        else {
            if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $('#IsApproachingExpiryDate').val().toLowerCase() == "true") {
                alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
            }
            else {
                ApplyTradeActual(model_JobId, interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue);//, IsGenerateRecZip);
            }
        }
    }
    else {
        alert('Oops, there is an error in the backend, please contact a Greenbot admin');
    }
}

function ApplyTradeActual(jobids, interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue)//, IsGenerateRecZip)
{
    var isSaved = CompareJobData();
    if (isSaved) {
        $.ajax({
            url: urlApplyTradeSTC,
            type: "POST",
            data: { jobId: jobids, interval: interval, price: price, settlementterm: settlementterm, isGst: model_IsGST, stcAmt: stcAmount, CustomSettlementTerm: customSettlementTerm, peakPayGst: peakPayGstRate, peakPayTimeperiod: peakPayTimePeriodValue, peakPayFee: peakPayFeeValue },
            success: function (Data) {
                if (Data.jobIds != undefined) {
                    showMessageForSTC("There are some illegal characters in the serial number field. Job cannot be traded until these are amended with the correct serials.", true);
                } else {
                    showMessageForSTC("Job has been traded successfully.", false);
                    if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                        $('#btnSaveJob').hide();
                    }
                    if (isSaveJobAfterTrade == "True")
                        $('#btnSaveJob').show();
                    ReloadSTCJobScreen(model_JobId);
                    debugger;
                    SearchHistory();
                    if (typeof (isDynamicJobIndex) != 'undefined' && (isDynamicJobIndex)) {
                        filterKendo = $('#datatable').data('kendoGrid').dataSource.filter();
                        datatableInfo();
                        if (!GridConfig[0].IsKendoGrid) {
                            $('#datatable').DataTable().destroy();
                            drawJobIndex();
                        } else {
                            $('#datatable').kendoGrid('destroy').empty();
                            drawJobIndex(filterKendo);
                        }
                    }
                    else {
                        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
                        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
                        $('#JobSystemDetails_SystemSize').prop('disabled', true);
                        $('#BasicDetails_strInstallationDate').prop('disabled', true);
                        $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
                        $('#PricingBlock').hide();
                    }
                    if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                        $("#CESDocbtns").hide();
                        $("#STCDocBtns").hide();
                        $('.isdelete').css("display", "none");
                    }
                    if (session_UserTypeId == 1 || session_UserTypeId == 3 || session_UserTypeId == 2 || session_UserTypeId == 4 || session_UserTypeId == 5) {
                        $("#CESDocbtns").show();
                        $("#STCDocBtns").show();
                    }
                    if (session_UserTypeId == 1 || session_UserTypeId == 3) {
                        $('.isdelete').css("display", "block");
                    }
                }
            }
        });
    }
    else {
        alert("There are changes in this job that have not been saved. Before trading, please save your changes.");
    }
}

function ShowHideJobs(objTerm, objCustTerm) {
    $('#datatable1').find('tr').show();
    $('#datatable1 tr td').each(function () {
        $(this).find('input[type=checkbox]').each(function () {
            isAllowKW = $(this).attr('isallowkw');
            jobSizeForOptiPay = $(this).attr('jobsizeforoptipay');
            isCommercialJob = $(this).attr('iscommercialjob');
            isNonCommercialJob = $(this).attr('isnoncommercialjob');
            isPeakPayCommercialJob = $(this).attr('isPeakPayCommercialJob');
            isPeakPayNonCommercialJob = $(this).attr('isPeakPayNonCommercialJob');

            isCustomAllowKW = $(this).attr('iscustomallowkw');
            jobCustomSizeForOptiPay = $(this).attr('jobcustomsizeforoptipay');
            isCustomCommercialJob = $(this).attr('iscustomcommercialjob');
            isCustomNonCommercialJob = $(this).attr('iscustomnoncommercialjob');
            isCustomPeakPayCommercialJob = $(this).attr('isCustomPeakPayCommercialJob');
            isCustomPeakPayNonCommercialJob = $(this).attr('isCustomPeakPayNonCommercialJob');

            modelIsGSTSetByAdminUser = $(this).attr('modelisgstsetbyadminuser');
            isGst = $(this).attr('isgst').toLowerCase();
            gstDocument = $(this).attr('gstdocument');
            propType = $(this).attr('proptype').toLowerCase();
            sysSize = $(this).attr('jobsystemsize');
            ownerType = $(this).attr('ownerType').toLowerCase();
        })

        if (objTerm == 7) {
            CheckRuleofRapidPay(null, propType, $(this), ownerType);
        }
        else if (objTerm == 8) {
            CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
        }
        else if (objTerm == 9) {
            CheckRuleofCommercial(null, modelIsGSTSetByAdminUser, isGst, gstDocument, $(this), ownerType, propType);
        }
        else if (objTerm == 12) {
            CheckRuleofPeakPay(isPeakPayCommercialJob, isPeakPayNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
        }
        else if (objTerm == 10) {
            if (objCustTerm == 7) {
                CheckRuleofRapidPay(null, propType, $(this), ownerType);
            }
            else if (objCustTerm == 8) {
                CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
            }
            else if (objCustTerm == 9) {
                CheckRuleofCommercial(null, modelIsGSTSetByAdminUser, isGst, gstDocument, $(this), ownerType, propType);
            }
            else if (objCustTerm == 12) {
                CheckRuleofPeakPay(isCustomPeakPayCommercialJob, isCustomPeakPayNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
            }
        }
    });
    $('#chkAll1').prop('checked', true)
    datatable1Chkbox($('#chkAll1').prop('checked'), false);
}

function SearchHistory() {
    debugger;
    var categoryID = $('#historyCategory').val();
    categoryID = categoryID != null ? categoryID : 0;
    var IsImportantNote = FilterIsImportantNote_Glbl;
    var PostVisibility = $("#filter-postvisibility").val();
    var jobId = $("#BasicDetails_JobID").val();
    var IsDeletedJobNote = $("#IsDeletedJobNote").val();
    var order = "DESC";
    $("#loading-image").show();
    setTimeout(function () {
        $.ajax({
            url: showhistoryUrl,
            type: "GET",
            data: { "jobId": jobId, "categoryID": categoryID, "order": order, "PostVisibility": PostVisibility, "IsImportant": IsImportantNote, "IsDeletedJobNote": IsDeletedJobNote },
            cache: false,
            async: false,
            success: function (Data) {
                window.onbeforeunload = null;
                $("#showfilteredhistory").html($.parseHTML(Data));
                $("divCustom").mCustomScrollbar();
            }
        });
        AddHistoryIcons();
        //tagandsearchfilter();
        hideEditDeleteIcons();
        $("#loading-image").hide();
    }, 10);
}





$("#JobInstallationDetails_InstallingNewPanel").change(function (e) {
    debugger;
    if ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") {
        $("#additionalCapacityNotes").show();
    }
    else {
        $("#JobSTCDetails_AdditionalCapacityNotes").text('');
        $("#additionalCapacityNotes").hide();
    }
    //if ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") {
    //    $("#installationLocation").show();
    //} else {
    //    $("#installationLocation").hide();
    //    $("#JobInstallationDetails_Location").val('');
    //}
    /*showhideAdditionalCapacityNotes();*/
});

//function showhideAdditionalCapacityNotes() {
//    debugger;
//    /*if ($("#JobInstallationDetails_Location").val() != "") {*/
//        if ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") {
//            $("#additionalCapacityNotes").show();
//        }
//        else {
//            $("#JobSTCDetails_AdditionalCapacityNotes").text('');
//            $("#additionalCapacityNotes").hide();
//        }
//    //}
//    //else {
//    //    $("#JobSTCDetails_AdditionalCapacityNotes").text('');
//    //    $("#additionalCapacityNotes").hide();
//    //}
//}
