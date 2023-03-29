$(document).ready(function () {
    
    if (($(".line-no.unverified").length > 0
        || $(".line-no.installationVerified").length > 0
        || $(".line-no.verified").length > 0
        || $(".line-no.notverified").length > 0)
        && $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
        $("#SPVLabel").show();
    else
        $("#SPVLabel").hide();
    if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True") {
        $("#btnSPVProductVerificationErrorLog").show();
        $("#btnSPVProductVerification").show();
       // $(".isSPVRequired").show();
    }

    else {
        
        $("#btnSPVProductVerificationErrorLog").hide();
        $("#btnSPVProductVerification").hide();
        //$(".isSPVRequired").hide();
    }
    if (BasicDetails_ScoID != 0 && BasicDetails_ScoID > 0) {
        //FillDropDown('ScoID', urlGetSCOUser, BasicDetails_ScoID, true, null);
        dropDownData.push({ id: 'ScoID', key: "", value: BasicDetails_ScoID, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });
    }
    else {
        //FillDropDown('ScoID', urlGetSCOUser, null, true, null);
        dropDownData.push({ id: 'ScoID', key: "", value: null, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });
    }

    $("#SSCID").change(function () {
        var jobPriority = $("#SSCID option:selected").text();
        $('#BasicDetails_SSCName').val(jobPriority);
    });

    dropDownData.push({ id: 'SSCID', key: "", value: BasicDetails_SSCID, hasSelect: true, callback: disableSSCdropdown, defaultText: null, proc: 'User_GetSSCUserByJbID', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });

    dropDownData.bindDropdown();
    function disableSSCdropdown() {
        if (BasicDetails_SSCID > 0 && ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3 && ProjectSession_UserTypeId != 4 && ProjectSession_UserTypeId != 5 && ProjectSession_UserTypeId != 2)
            $('#SSCID').prop("disabled", true);
        else
            $('#SSCID').prop("disabled", false);
    }

    $("#btnJobPrint").click(function () {
        var url = urlPrint + $(this).attr('jobid');
        window.open(url);
    });
    console.log(urlIndex);
    $("#aSwitch").click(function (e) {
        window.location.href = '/Job/Index?Id=' + eJobId+"&isTabularView=false";
    });
    $("#btnSPVProductVerification").click(function () {
        SPVProductverification();
    });
    $("#btnSPVProductVerificationErrorLog").click(function () {
        SPVProductionVerificationErrorLog();
    });
    $("#btnReSPVProductVerification").click(function () {
        SPVProductverification(true);
    })

    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true,   // 100% fit in a container
        closed: 'accordion', // Start closed if in accordion view
        activate: function (event) { // Callback function if tab is switched
            var $tab = $(this);
            var $info = $('#tabInfo');
            var $name = $('span', $info);

            $name.text($tab.text());

            $info.show();
            $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
            $('#successMsgRegionSTCDetails').hide();
            $('#errorMsgRegionSTCDetails').hide();
        }
    });
    $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });
    // show-bcc
    $('.show-bcc').click(function () {
        $('.cc-input-box').slideToggle("fast");
    });
    $(".mCustomScrollbar").mCustomScrollbar();

    UploadAllSignatureOfJob($("#uploadJobOwnerSign"));


    if (Model_liLength != null) {
        var liLength = Model_liLength;
        UpdatePercentage(liLength);
    }


    if (Model_OwnerSignature != null && Model_OwnerSignature != '') {
        SRCOwnerSign = signatureURL + Model_OwnerSignature_Replace;
        $('#imgOwnerSignatureView').show();
    }
    $("#btnClosepopupOwnerSign").click(function () {
        $('#popupOwnerSign').modal('toggle');
    });
    $('#imgOwnerSignatureView').click(function () {
        $("#loading-image").css("display", "");
        $('#imgOwnerSign').attr('src', SRCOwnerSign).load(function () {
            logoWidth1 = this.width;
            logoHeight1 = this.height;
            $('#popupOwnerSign').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeight1) {
                $("#imgOwnerSign").closest('div').height($(window).height() - 150);
                $("#imgOwnerSign").closest('div').css('overflow-y', 'scroll');
                $("#imgOwnerSign").height(logoHeight1);
            }
            else {
                $("#imgOwnerSign").height(logoHeight1);
                $("#imgOwnerSign").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidth1 || logoWidth1 >= screen.width - 250) {
                $('#popupOwnerSign').find(".modal-dialog").width(screen.width - 250);
                $("#imgOwnerSign").width(logoWidth1);
            }
            else {
                $("#imgOwnerSign").width(logoWidth1);
                $('#popupOwnerSign').find(".modal-dialog").width(logoWidth1);
            }
            $("#loading-image").css("display", "none");
        });
        $("#imgOwnerSign").unbind("error");
        $('#imgOwnerSign').attr('src', SRCOwnerSign).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });

    if (ProjectSession_UserTypeId == 1
        || ProjectSession_UserTypeId == 3
        || $('#STCStatusId').val() == 10
        || $('#STCStatusId').val() == 12
        || $('#STCStatusId').val() == 14
        || $('#STCStatusId').val() == 17) {
        $('#JobOwnerDetails_OwnerType').prop('disabled', false);
        $('#JobInstallationDetails_PropertyType').prop('disabled', false);
        $('#JobSystemDetails_SystemSize').prop('disabled', false);
        $('#BasicDetails_strInstallationDate').prop('disabled', false);
        $('#JobSystemDetails_SerialNumbers').prop('disabled', false);
    }
    else {
        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
        $('#JobSystemDetails_SystemSize').prop('disabled', true);
        $('#BasicDetails_strInstallationDate').prop('disabled', true);
        $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
    }

    ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());
});

$('#btnSaveTab').click(function (e) {
    e.preventDefault();

    var Type = $('#horizontalTab').find('.resp-tab-active').data('type');

    if (Type == 'basic') {
        TabularView.UpdateBasicDetail();
    }
    else if (Type == "owner") {
        TabularView.UpdateOwnerDetail();
    }
    else if (Type == "system") {
        TabularView.UpdateSystemDetail();
    }
    else if (Type == "installer") {

    } else if (Type == "stc") {
        TabularView.UpdateStcDetail();
        TabularView.UpdateGstDetail();
    }
    else if (Type == "installation") {
        TabularView.UpdateInstallationDetail();
    }
    else if (Type == "documents") {

    }
    else if (Type == "scheduling") {

    }
    else if (Type == "payment") {

    }
});

var TabularView = {
    UpdateBasicDetail: function (obj) {
        var soldBy = $('#BasicDetails_JobSoldByText').val();
        $('#BasicDetails_SoldBy').val(soldBy);
        if ($("#frmBasicDetail").valid()) {
            $('#SSCID').prop("disabled", false);
            var url = urlUpdateBasicDetail,
                
            data = $('#frmBasicDetail').serializeToJson();
            //$('#SSCID').prop("disabled", true);
            getDataTabular(url, data.BasicDetails, function (result) {
                if (result.status) {
                    //var liLength = $(result.Data.ValidationSummary).length;
                    //UpdatePercentage(liLength);
                    ReloadSTCModule();
                    showSuccessMessage("Basic details has been saved successfully.");
                    if ($('#SSCID').val() !="")
                        $('#SSCID').prop("disabled", true);
                    else
                        $('#SSCID').prop("disabled", false);
                    
                }
                else {
                    showErrorMessage("Basic details has not been saved.");
                    if ($('#SSCID').val() != "")
                        $('#SSCID').prop("disabled", true);
                    else
                        $('#SSCID').prop("disabled", false);
                    
                }
            });
        }
    },
    UpdateOwnerDetail: function () {
        $("#JobOwnerDetails_JobID").val(BasicDetails_JobID);
        var isValid = addressValidationRules("JobOwnerDetails");
        var propertyType = $("#JobInstallationDetails_PropertyType").val();
        var emailOwner = $('#JobOwnerDetails_Email').val();
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installation property type school.");
            return false;
        }
        
        if ($("#frmOwnerDetail").valid() && isValid) {
            var url = urlUpdateOwnerDetail,
            data = $('#frmOwnerDetail').serializeToJson();
            data.jobId = BasicDetails_JobID;
            data.isGST = $("#BasicDetails_IsGst").val();
            getDataTabular(url, data, function (result) {
                if (result.status) {
                    //var liLength = $(result.Data.ValidationSummary).length;
                    //UpdatePercentage(liLength);
                    ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());
                    ReloadSTCModule();
                    showSuccessMessage("Owner details has been saved successfully.");
                }
                else {
                    showErrorMessage("Owner details has not been saved.");
                }
            });
        }
    },
    UpdateStcDetail: function () {
        var url = urlUpdateStcDetail,
        data = $('#frmStcDetail').serializeToJson();
        data.JobType = $("#BasicDetails_JobType").val();
        var propertyType = $("#JobInstallationDetails_PropertyType").val();
        var emailOwner = $('#JobOwnerDetails_Email').val();
        var stcLatitude = $("#JobSTCDetails_Latitude").val();
        var stcLongitude = $("#JobSTCDetails_Longitude").val();
        var decimal = /^[-+]?[0-9]+\.[0-9]+$/;
        if (stcLatitude != "" && !stcLatitude.match(decimal)) {
            showErrorMessage("Please enter latitude in decimal format of STC job details.");
            return false;
        }
        if (stcLongitude != "" && !stcLongitude.match(decimal)) {
            showErrorMessage("Please enter longitude in decimal format of STC job details.");
            return false;
        }
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installtion property type school.");
            return false;
        }
        debugger
        getDataTabular(url, data, function (result) {
            if (result.status) {
                //var liLength = $(result.Data.ValidationSummary).length;
                //UpdatePercentage(liLength);
                ReloadSTCModule();
                showSuccessMessage("Stc detail has been saved successfully.");
            }
            else {
                showErrorMessage("Stc detail has not been saved.");
            }
        });
    },
    UpdateInstallationDetail: function () {

        var InstallationAddress = $('#txtAddress').val();
        if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
            $('#spantxtAddress').show();
            return false;
        } else { $('#spantxtAddress').hide(); }

        $("#JobInstallationDetails_JobID").val(BasicDetails_JobID);
        if ($("#frmInstallationDetail").valid()) {
            var data = JSON.stringify($('#frmInstallationDetail').serializeToJson());
            var objData = JSON.parse(data);

            var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
            var objInstallationAddressData = JSON.parse(installationAddressData);

            var installationData = JSON.stringify($('#JobInstallationDetailInfo').serializeToJson());
            var objInstallationData = JSON.parse(installationData);

            var extraInstallationData = JSON.stringify($('#JobExtraInstallationInfo').serializeToJson());
            var objExtraInstallationData = JSON.parse(extraInstallationData);

            $.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);
            $.extend(objData.JobInstallationDetails, objInstallationData.JobInstallationDetails);
            $.extend(objData.JobInstallationDetails, objExtraInstallationData.JobInstallationDetails);

            var lstCustomDetails = [];
            $("#customDetails").find('.spanCustomFields').each(function () {
                lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).val(), SeparatorId: $(this).attr('data-SeperatorId') });
            });
            objData.JobInstallationDetails.lstCustomDetails = lstCustomDetails;

            var customLength = $("#customDetails").find('.spanCustomFields').length;
            for (var i = 0; i < customLength; i++) {
                delete objData["lstCustomDetails[" + i + "]"];
            }

            var jobData = objData;

            var url = urlUpdateInstallationDetail;
            getDataTabular(url, jobData, function (result) {
                ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());
                //getDataTabular(url, jobData, function (result) {
                if (result.status) {
                    //var liLength = $(result.Data.ValidationSummary).length;
                    //UpdatePercentage(liLength);
                    ReloadSTCModule();
                    showSuccessMessage("Installation detail has been saved successfully.");
                }
                else {
                    showErrorMessage("Installation detail has not been saved.");
                }
            });
        }
    },
    UpdateSystemDetail: function () {
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
                    '<Series>' + htmlEncode(this['Series']) + '</Series><NoOfInverter>' + htmlEncode(this['NoOfInverter']) + '</NoOfInverter><Supplier>' + htmlEncode(this['Supplier']) + '</Supplier></inverter>';
            });
            xmlInverter = '<Inverters>' + sName + '</Inverters>';
        }

        $("#panelXml").val(xmlPanel);
        $("#inverterXml").val(xmlInverter);
        $("#JobSystemDetails_jobTypeTab").val(jobType);
        $("#JobSystemDetails_JobID").val(BasicDetails_JobID);
        $("#OldPanelDetails").val(JSON.stringify(OldPanelXml));
        $("#NewPanelDetails").val(JSON.stringify(PanelXml));


        var data = JSON.stringify($('#frmSystemDetail').serializeToJson());

        var objData = {};
        var jobSystemDetails = JSON.parse(data).JobSystemDetails;
        //jobSystemDetails.CalculatedSTC = $("#JobSystemDetails_CalculatedSTC").val();
        //jobSystemDetails.CalculatedSTCForSWH = $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.ModifiedCalculatedSTC = $("#BasicDetails_JobType").val() == 1 ? $("#JobSystemDetails_CalculatedSTC").val() : $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.NoOfPanel = $("#JobSystemDetails_NoOfPanel").html();
        jobSystemDetails.InstallationType = $("#JobSystemDetails_InstallationType").val();
        if (systemXml.length > 0) {
            jobSystemDetails.SystemBrand = systemXml[0].Brand;
            jobSystemDetails.SystemModel = systemXml[0].Model;
        }
        jobSystemDetails.panelXmlTabular = xmlPanel;
        jobSystemDetails.inverterXmlTabular = xmlInverter;

        if (batteryManufacturerData.length > 0) {

            for (var i = 0; i < batteryManufacturerData.length; i++) {
                delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
            }
            jobSystemDetails.lstJobBatteryManufacturer = batteryManufacturerData;
            if ($("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val().toLowerCase() == 'select') {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = ''
            } else {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
            }
            if ($("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val().toLowerCase() == 'select') {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = '';
            } else {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();
            }
        }
       



        // Check to set STC value start
        var isRequiredFieldofSTC;
        if ($("#BasicDetails_JobType").val() == 1) //PVD job
            isRequiredFieldofSTC = ($("#BasicDetails_strInstallationDate").val() && $("#JobSTCDetails_DeemingPeriod").val() && $("#JobInstallationDetails_PostCode").val()) ? true : false;
        else //SWH job
            isRequiredFieldofSTC = (systemXml.length > 0 && systemXml[0].Brand && systemXml[0].Model && $("#JobInstallationDetails_PostCode").val()) ? true : false;

        var isSystemSize = $("#JobSystemDetails_SystemSize").val() > 0 ? true : false;
        var isInstallationDate = $("#BasicDetails_strInstallationDate").val() ? true : false;

        var isValidDataForSTC;

        if ($("#BasicDetails_JobType").val() == 1)
            isValidDataForSTC = !isSystemSize || (isSystemSize && isRequiredFieldofSTC);
        else
            isValidDataForSTC = !isInstallationDate || (isInstallationDate && isRequiredFieldofSTC);
        // Check to set STC value end
        objData.objSystem = jobSystemDetails;
        objData.OldPanelDetails= JSON.parse(data).OldPanelDetails
        objData.NewPanelDetails = JSON.parse(data).NewPanelDetails;
        var serialVal;
        if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
            serialVal = true;
        }
        else {
            serialVal = SerialNumbersValidation();
        }
        
        if (serialVal) {
            if (isValidDataForSTC) {
                var url = urlUpdateSystemDetail;
                getDataTabular(url, objData, function (result) {
                    if (result.status) {
                        OldPanelXml = [];
                        OldPanelXml = JSON.parse(JSON.stringify(PanelXml));
                        if (result.jobType == 1) {
                            if (result.STCValue > 0)
                                $("#JobSystemDetails_CalculatedSTC").val(result.STCValue);
                            else
                                $("#JobSystemDetails_CalculatedSTC").val("");
                        }
                        else {
                            if (result.STCValue > 0)
                                $("#JobSystemDetails_CalculatedSTCForSWH").val(result.STCValue);
                            else
                                $("#JobSystemDetails_CalculatedSTCForSWH").val("");
                        }
                        ReloadSTCModule();
                        serialNumber = result.serialnumbers;
                        $("div.line-no").removeClass("verified")
                        $("div.line-no").removeClass("unverified")
                        $("div.line-no").removeClass("installationVerified")
                        $("div.line-no").removeClass("notverified")
                        $("#GlobalisAllowedSPV").val(result.GlobalisAllowedSPV)
                        if (result.IsSPVRequired) {
                            IsSPVRequired = "True"
                            VerifyUnVerifySerialNumber();
                            var unverifiedSerialNumber = serialNumber.find(serialNumber => serialNumber.IsVerified == null);
                            if (unverifiedSerialNumber != undefined && $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                                $(".isSPVRequired").show();
                            else
                                $(".isSPVRequired").hide();
                            
                        } else {
                            $(".isSPVRequired").hide();
                            $("#SPVLabel").hide();
                            IsSPVRequired = "False"
                        }
                        showSuccessMessage(result.isRECUp ? "System detail has been saved successfully." : "System detail has been saved successfully but STC Value cannot be calculated since REC website is down.");
                    }
                    else {
                        showErrorMessage("System detail has not been saved.");
                    }
                });
            }
            else {
                if ($("#BasicDetails_JobType").val() == 1)
                    showErrorMessage("Please fill Installation Date, STC DeemingPeriod, Installation postcode to set STC value.");
                else
                    showErrorMessage("Please fill Installation Date, System brand, System Model, Installation postcode to set STC value.");
            }
        }
    },
    UpdateGstDetail: function () {
        if ((modelIsGSTSetByAdminUser == 2 && modelIsRegisteredWithGST == 'true') || (($('#JobOwnerDetails_OwnerType').val().toLowerCase() == 'corporate body' || $('#JobOwnerDetails_OwnerType').val().toLowerCase() == 'trustee') || ($('#JobInstallationDetails_PropertyType').val().toLowerCase() == "commercial" || $('#JobInstallationDetails_PropertyType').val().toLowerCase() == "school")))
            $("#BasicDetails_IsGst").prop('checked', true);
        else
            $("#BasicDetails_IsGst").prop('checked', false);

        var obj = {
            isGST: $('#BasicDetails_IsGst').prop('checked'),
            FileName: $('#BasicDetails_GSTDocument').val(),
            jobId: BasicDetails_JobID
        }

        var url = urlUpdateGstDetail;
        getDataTabular(url, obj, function (result) {
            if (result.status) {
                //showSuccessMessage("GST detail has been saved successfully.");
            }
            else {
                showErrorMessage("GST detail has not been saved.");
            }
        });

    }
};


function getDataTabular(url, data, callback) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        url: url,
        data: JSON.stringify(data),
        success: function (result) {
            if (result.status) {
                if (callback) {
                    callback(result);
                }
            }
            else {
                showErrorMessage(result.error);
            }
        },
        error: function (ex) {
            showErrorMessage('job has not been saved.' + ex);
        }
    });
}

function UploadAllSignatureOfJob(objSignUpload) {
    var typeOfSignature = objSignUpload.attr('typeOfSignature');

    objSignUpload.fileupload({
        url: urlUploadJobSignature,
        dataType: 'json',
        done: function (e, data) {

            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {

                    var path = signatureURL + 'JobDocuments/' + Model_JobID + '/' + data.result[i].FileName;

                    if (typeOfSignature == 1) {
                        isOwnerSignUpload = true;
                        SRCOwnerSign = path;
                        $("#imgOwnerSignatureView").show();
                    }
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                showErrorMessage("File has not been uploaded.");
            }
            else {
                showSuccessMessage("File has been uploaded successfully.");
            }
        },
        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(documentSizeLimit)) {
                        showErrorMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(documentSizeLimit)) {
                    showErrorMessage("Maximum file size limit exceeded.Please upload a file smaller than" + " " + maxsize + "MB");
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    showErrorMessage("Please upload file that not conatain <> ^ [] .");
                    return false;
                }
            }
            if (mimeType != "image") {
                showErrorMessage("Please upload a file with .jpg , .jpeg or .png extension.");
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            return true;
        },
        formData: { jobId: BasicDetails_JobID, typeOfSignature: typeOfSignature }
    });
}

function clearSignature(typeOfSignature) {
    if (confirm('Are you sure you want to delete uploaded signature?')) {
        if (typeOfSignature == 1) {
            ClearAllSignature(typeOfSignature, SRCOwnerSign, isOwnerSignUpload);
            isOwnerSignUpload = false;
            $("#imgOwnerSignatureView").hide();
        }
    }
}

function ClearAllSignature(typeOfSignature, signPath, isUpload) {
    $.ajax(
    {
        url: urlClearJobSignture,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { typeOfSignature: typeOfSignature, signaturePath: signPath, jobId: BasicDetails_JobID, isUpload: isUpload.toString().toLowerCase() },
        success: function (response) {
            if (response.status) {
                showSuccessMessage("Signature has been deleted successfully.");
            }
            else
                showErrorMessage(response.error);
        },
        error: function () {
            showErrorMessage("Signature has not been deleted.");
        }
    });
}
var TypeOneLi = 20;
var TypeTwoLi = 11;

function UpdatePercentage(liLength) {

    var mainLi = jobType == "1" ? TypeOneLi : TypeTwoLi;
    var a = mainLi - liLength,
        per = Math.ceil((a * 100) / mainLi);
    setProgressbarValue(per);
}
function setProgressbarValue(value) {
    $('.progress-bar').css('width', value + '%').attr('aria-valuenow', value).html(value + "%");
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
            if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $('#btnsave').hide();
            }

            $("#loading-image").hide();
        });
    }, 500);
}

$('#btnViewHistory').click(function (e) {
    e.preventDefault();
    $('#JobHistoryOfJob').load(urlShowHistory + urlRequestContext, function () {
        $('#JobHistory').modal({ backdrop: 'static', keyboard: false });
    });
});


$('#nxtTab').click(function (e) {
    e.preventDefault();
    $('li.resp-tab-active').next().click();
    $('#successMsgRegionSTCDetails').hide();
    $('#errorMsgRegionSTCDetails').hide();
    $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
});
$('#prevTab').click(function (e) {
    e.preventDefault();
    $('li.resp-tab-active').prev().click();
    $('#successMsgRegionSTCDetails').hide();
    $('#errorMsgRegionSTCDetails').hide();
    $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
});
$("#JobInstallationDetails_PropertyType").change();

function LoadCommonJobsWithSameInstallDateAndInstaller() {
    $.ajax({
        url: urlLoadCommonJobsWithSameInstallDateAndInstaller,
        type: "GET",
        data: { jobId: Model_JobID, installerId: $('#hdBasicDetails_InstallerID').val(), installationDate: $('#BasicDetails_strInstallationDate').val() },
        dataType: "json",
        success: function (Data) {
            //if (Data.commonJobs) {
                if (Data.commonJobs.length > 2) {
                    var div = '<div class="warning-notice" id="divWarning"><h5>Warning Notice:</h5> <br/> Installer: <b>' + Data.commonJobs[0].InstallerName + '</b> has been already assigned the following jobs.';
                    for (var i = 0; i < Data.commonJobs.length; i++) {
                        var companyName = "<b class=commonJobs>Company : </b>" + Data.commonJobs[i].CompanyName;
                        var resellerName = " <b class=commonJobs>Reseller : </b>" + Data.commonJobs[i].ResellerName;
                        var p = '<p>RefNumber : <a target="_blank" href="/Job/Index?id=' + Data.commonJobs[i].JobID + '"> ' + Data.commonJobs[i].RefNumber + ' </a> ' + companyName + resellerName + '</p>';
                        div = div + p;
                    }
                    div = div + '</div>';

                    $('#loadCommonJobs').show();
                    $("#loadCommonJobs").html(div);
                }
                else {
                    $("#loadCommonJobs").html('');
                    $('#loadCommonJobs').hide();
            }

                if (Data.commonJobsWithFailedSTCStaus.length > 0) {
                    var div = '<div class="warning-notice" id="divWarning"><h5>Warning Notice:</h5> <br/> Installer: <b>' + Data.commonJobsWithFailedSTCStaus[0].InstallerName + '</b> has been already assigned the following jobs which have failed stc statuses.';
                    for (var i = 0; i < Data.commonJobsWithFailedSTCStaus.length; i++) {
                        var companyName = "<b class=commonJobs>Company : </b>" + Data.commonJobsWithFailedSTCStaus[i].CompanyName;
                        var resellerName = " <b class=commonJobs>Reseller : </b>" + Data.commonJobsWithFailedSTCStaus[i].ResellerName;
                        var p = '<p>RefNumber : <a target="_blank" href="/Job/Index?id=' + Data.commonJobsWithFailedSTCStaus[i].JobID + '"> ' + Data.commonJobsWithFailedSTCStaus[i].RefNumber + ' </a> ' + companyName + resellerName + '</p>';
                        div = div + p;
                    }
                    div = div + '</div>';

                    $('#loadCommonJobsWithFailedStcStatus').show();
                    $("#loadCommonJobsWithFailedStcStatus").html(div);
                }
                else {
                    $("#loadCommonJobsWithFailedStcStatus").html('');
                    $('#loadCommonJobsWithFailedStcStatus').hide();
                }
            serialNumber = Data.serialnumbers;
                $("div.line-no").removeClass("verified")
                $("div.line-no").removeClass("unverified")
                $("div.line-no").removeClass("installationVerified")
                $("div.line-no").removeClass("notverified")
            if (Data.IsSPVRequired) {
                IsSPVRequired = "True";
                    VerifyUnVerifySerialNumber();
                    var unverifiedSerialNumber = serialNumber.find(serialNumber => serialNumber.IsVerified == null);
                if (unverifiedSerialNumber != undefined)
                    $(".isSPVRequired").show();
                    else
                    $(".isSPVRequired").hide();
                    
            } else {
                    $(".isSPVRequired").hide();
                    IsSPVRequired = "False"
                }
            //}
        }
    });
}