var jsonvarSoldByDate = '';
var jsonvarJobSoldBy = '';
var OwnerAddressJson = [];
var OwnerPostcodeFromjson;
var OwnerEmailFromJson;
var OwnerFirstNameFromJson;
var OwnerLastNameFromJson;
var fillOwnerAddress;
var fillInstallationAddress;
var InstallationJson = [];
var InstallationPostcodeFromjson;
var previousPropertyType;
var source, destination;
var directionsDisplay;
var latitude;
var longitude;
var latitude1;
var longitude1;
var locations = [];
var sourcedetail = [];
var destinationdetail = [];
var isMapsApiLoaded = false;
var MasterSerialArray = [];
var JobSerialArray = [];
var logoWidthGST = 0, logoHeightGST = 0;
var SRCOwnerSign;
$(document).ready(function () {
    debugger;
    $("#BasicDetails_strInstallationDate").val($("#BasicDetails_strInstallationDateTemp").val());
   

    $.fn.serializeToJson = function () {
        var $form = $(this[0]);

        var items = $form.serializeArray();

        var returnObj = {};
        var nestedObjectNames = [];

        $.each(items, function (i, item) {
            if (item.name.indexOf('.') != -1) {
                var nameArray = item.name.split('.');
                if (nestedObjectNames.indexOf(nameArray[0]) < 0) {
                    nestedObjectNames.push(nameArray[0]);
                }
                var tempObj = returnObj[nestedObjectNames[nestedObjectNames.indexOf(nameArray[0])]] || {};
                if (!tempObj[nameArray[1]]) {
                    tempObj[nameArray[1]] = item.value;
                }
                returnObj[nestedObjectNames[nestedObjectNames.indexOf(nameArray[0])]] = tempObj;
            } else if (!returnObj[item.name]) {
                returnObj[item.name] = item.value;
            }
        });
        return returnObj;
    };

    validateOrganisation();     //For tabular view

    if (modelOwnerSign != null && modelOwnerSign != '' && modelOwnerSign != undefined) {
        $("#imgOwnerSign").attr('src', signatureURL + modelOwnerSignURL);
    }

    $("#ownerGetSignFromVisit").click(function (e) {
        GetSignatureFromVisit(1);
        e.preventDefault();
    });

    $('#BasicDetails_IsGst').change(function () {
        $("#tblDocuments").find('tr').each(function () {
            $(this).remove();
        });
        if ($(this).is(":checked")) {
            $("#jobGST").show();
        } else {
            $("#jobGST").hide();
        }
    });

    if ($("#JobOwnerDetails_AddressID").val() == 1)
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", false)
    else
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", true)

    ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());

    GSTFileUpload();

    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('hide');
    });
    fillOwnerAddressDetail();
    debugger;
    var OwnerpostalAddressID = modelOwnerPostalAddressID || 0;
    var OwnerUnitTypeID = modelOwnerUnitTypeID || 0;
    var OwnerStreetTypeID = modelOwnerStreetTypeID || 0;
    var CompanyNameFirstTimeBind = modelOwnerCompanyName;//GetCompnayName();
    if (CompanyNameFirstTimeBind.indexOf('&#39') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&#39;/g, "'");
    }
    else if (CompanyNameFirstTimeBind.indexOf('&amp') != -1) {
        CompanyNameFirstTimeBind = CompanyNameFirstTimeBind.replace(/&amp;/g, '&');
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
    $("#JobOwnerDetails_CompanyName").append($("<option></option>").val(CompanyNameFirstTimeBind).html(CompanyNameFirstTimeBind));
    $("#JobOwnerDetails_CompanyName").val(CompanyNameFirstTimeBind);
    
    dropDownData = [];
    dropDownData.push({ id: 'JobOwnerDetails_UnitTypeID', key: "UnitType", value: OwnerUnitTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'JobOwnerDetails_StreetTypeID', key: "StreetType", value: OwnerStreetTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'JobOwnerDetails_PostalAddressID', key: "PostalAddress", value: OwnerpostalAddressID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });

    //Installtion Detail
    fillInstallationAddressDetail();
    var InstallationpostalAddressID = modelInstallationPostalAddressID || 0;
    var InstallationUnitTypeID = modelInstallationUnitTypeID || 0;
    var InstallationStreetTypeID = modelInstallationStreetTypeID || 0;
    var distributorid = modelInstallationDistributorID || 0;
    var Installationid = modelInstallationElectricityProviderID || 0;

    dropDownData.push({ id: 'BasicDetails_JobStage', key: "JobStage", value: BasicDetails_JobStage, hasSelect: true, callback: null, defaultText: null, proc: 'Job_GetJobSatge', param: [], bText: 'StageName', bValue: 'JobStageId' });
    dropDownData.push({ id: 'BasicDetails_Priority', key: "", value: BasicDetails_Priority, hasSelect: true, callback: null, defaultText: null, proc: 'GetPriority', param: [], bText: 'Text', bValue: 'Value' });
    dropDownData.push({ id: 'JobInstallationDetails_UnitTypeID', key: "UnitType", value: modelInstallationUnitTypeID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'JobInstallationDetails_StreetTypeID', key: "StreetType", value: modelInstallationStreetTypeID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'JobInstallationDetails_PostalAddressID', key: "PostalAddress", value: modelInstallationPostalAddressID, hasSelect: true, callback: fillInstallationAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        { id: 'JobInstallationDetails_DistributorID', key: "Destributors", value: distributorid, hasSelect: true, callback: DisplayInstallationInfo, defaultText: null, proc: 'GetDistributor', param: [], bText: 'DistributorName', bValue: 'DistributorID' },
        { id: 'JobInstallationDetails_ElectricityProviderID', key: "ElectricityProvider", value: Installationid, hasSelect: true, callback: DisplayInstallationInfo, defaultText: null, proc: 'Job_GetElectricityProvider', param: [], bText: 'Provider', bValue: 'ElectricityProviderId' },
        { id: 'BasicDetails_JobSoldBy', key: "SoldBy", value: 0, hasSelect: true, callback: null, defaultText: null, proc: 'GetSoldBy', param: [], bText: 'NAME', bValue: 'NAME' });

    var finalYear = '', deemingperiod;
    var installationDate = $("#BasicDetails_strInstallationDate").val();
    var installationDateEdit = '';
    if (installationDate != null && installationDate != undefined && installationDate != '') {
        finalYear = moment(installationDate).format('yyyy'.toUpperCase());
    }
    deemingperiod = modelSTCDeemingPeriod ? modelSTCDeemingPeriod : 0;

    //BindDeemingPeriodDropdown(finalYear, deemingperiod);
    dropDownData.push({ id: 'JobSTCDetails_DeemingPeriod', key: "DeemingPeriod", value: deemingperiod, hasSelect: true, callback: deemingPeriodCallBack, defaultText: null, proc: '', param: [{ jobYear: finalYear }], bText: 'Text', bValue: 'Value' });

    $("#BasicDetails_strInstallationDate").change(function () {
        var changeFinalYear = '', changeDeemingPeriod;
        var changeinstallationDate = $("#BasicDetails_strInstallationDate").val();
        if (changeinstallationDate != null && changeinstallationDate != undefined && changeinstallationDate != '') {
            changeFinalYear = moment(changeinstallationDate, "DD/MM/YYYY").format('yyyy'.toUpperCase());
        }
        changeDeemingPeriod = $('#JobSTCDetails_DeemingPeriod').val();
        BindDeemingPeriodDropdown(changeFinalYear, 0);
    });

    DisplayInstallationDate();
    DisplaySoldByDate();
    buttonSoldByClick();
    buttonOwnerDetailClick();
    buttonInstallationDetailClick();
    SameAsOwnerAddressMethod();
    OwnerAutoComplete();
    InstallationAutoComplete();

    $("#JobOwnerDetails_OwnerType").change(function (e) {
        validateOrganisation();
    });

    $("#JobOwnerDetails_UnitTypeID").change(function () {
        if ($('#JobOwnerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblOwnerUnitNumber').removeClass("required");
            $('#lblOwnerUnitTypeID').removeClass("required");
            $('#lblOwnerStreetNumber').addClass("required");
        }
        else {
            $('#lblOwnerUnitNumber').addClass("required");
            $('#lblOwnerUnitTypeID').addClass("required");
            $('#lblOwnerStreetNumber').removeClass("required");
        }
    });

    $("#JobInstallationDetails_UnitTypeID").change(function () {
        if ($('#JobInstallationDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallationUnitNumber').removeClass("required");
            $('#lblInstallationUnitTypeID').removeClass("required");
            $('#lblInstallationStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallationUnitNumber').addClass("required");
            $('#lblInstallationUnitTypeID').addClass("required");
        }
    });

    //ShowHideSWHPVDField();

    $('#JobInstallationDetails_ExistingSystem').change(function () {
        OwnerHasExistingSystem($(this).is(":checked"));
    });

    $("#aShowHideOtherInstalltionInfo").click(function (e) {
        ShowHideOtherInstalltionInfo(this);
        e.preventDefault();
    });

    OwnerHasExistingSystem(modelInstallationExistingSystem);

    if (modelBasicJobType == 1) {
        if (modelSTCMultipleSGUAddress == 'Yes') {
            //$("#STCLocation").show();
            $("#STCAdditionalLocation").show();
        } else {
            //$("#STCLocation").hide();
            $("#STCAdditionalLocation").hide();
            $("#JobSTCDetails_AdditionalLocationInformation").val('');
            //$("#JobSTCDetails_Location").val('');
        }
    }
    if (modelBasicJobType == 2) {
        if (modelSTCMultipleSGUAddress == 'Yes' || (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3)) {
            $("#additionalInfo").show();
        } else {
            $("#additionalInfo").hide();
        }
    }
    
    $("#JobSTCDetails_MultipleSGUAddress").change(function (e) {
        if (modelBasicJobType == 1) {
            if ($("#JobSTCDetails_MultipleSGUAddress").val() != "No" && $("#JobSTCDetails_MultipleSGUAddress").val() != "") {
            $("#JobSTCDetails_AdditionalLocationInformation").show();
        }
        else {
            $("#JobSTCDetails_AdditionalLocationInformation").hide();
        }
        showhideAdditionalLocationInformation();
        }
        if (modelBasicJobType == 2) {
            if ($("#JobSTCDetails_MultipleSGUAddress").val() =="Yes" || (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3)) {
                $("#additionalInfo").show();
            } else {
                $("#additionalInfo").hide();
            }
        }

    });

    //$("#JobSTCDetails_Location").change(function (e) {
    //    if ($("#JobSTCDetails_Location").val() != "") {
    //        $("#JobSTCDetails_AdditionalLocationInformation").show();
    //    }
    //    else {
    //        $("#JobSTCDetails_AdditionalLocationInformation").hide();
    //    }
    //    showhideAdditionalLocationInformation();
    //})

    $("#JobInstallationDetails_InstallingNewPanel").change(function (e) {
        if (($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") || ProjectSession_UserTypeId==1) {
            EnableTradeButton();
            $("#JobSTCDetails_AdditionalCapacityNotes").show();
    }
        else {
            if (!isJobWithCommonInstallationAddress) {
                EnableTradeButton();
            }
            else {
                DisableTradeButton();
            }
            $("#JobSTCDetails_AdditionalCapacityNotes").hide();
        }
        showhideAdditionalCapacityNotes();
    });

    $("#JobInstallationDetails_PropertyType").on('focus', function () {
        // Store the current value on focus and on change
        previousPropertyType = this.value;
    }).change(function (e) {

        var propertyType = $("#JobInstallationDetails_PropertyType").val();
        var emailOwner = $('#JobOwnerDetails_Email').val();


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
        ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());
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
                    SavedData = CommonDataForSave();
                    ReloadSTCJobScreen(jobId);
                   // SearchHistory();
                }
                else
                    showErrorMessageForPopup(response.msg, $("#errorMsgRegionSTCDetails"), $("#successMsgRegionSTCDetails"));
            }
        });
    });

    //$("#JobInstallationDetails_Location").change(function (e) {
    //    if ($("#JobInstallationDetails_Location").val() != "") {
    //        $("#JobSTCDetails_AdditionalCapacityNotes").show();
    //    }
    //    else {
    //        $("#JobSTCDetails_AdditionalCapacityNotes").hide();
    //    }
    //    showhideAdditionalCapacityNotes();
    //});

    $("#JobSTCDetails_VolumetricCapacity").change(function () {
        $("#spanStatutoryDeclarations").hide();
        if ($(this).val() == "Yes") {
            $("#divStatutoryDeclarations").show();
        } else {
            $("#divStatutoryDeclarations").val($("#divStatutoryDeclarations option:first").val());
            $("#divStatutoryDeclarations").hide();
        }
    });

    $('#JobSTCDetails_StatutoryDeclarations').change(function () {
        if ($(this).val() != "Select" && $(this).val() != null && $(this).val() != undefined && $(this).val() != "") {
            $("#spanStatutoryDeclarations").hide();
        }
    });

    showhideFailedCode(modelSTCCertificateCreated)
   // $('#JobSTCDetails_FailedReason').html(modelFailedReason);
    $("#JobSTCDetails_CertificateCreated").change(function (e) {
        showhideFailedCode($(this).val());
    });

    if (modelInstalltionInstallingNewPanel == 'Replacing' || modelInstalltionInstallingNewPanel == 'Adding' || modelInstalltionInstallingNewPanel == 'Extension') {
        //$("#installationLocation").show();
        $("#additionalCapacityNotes").show();
    }
    else {
        //$("#installationLocation").hide();
        $("#additionalCapacityNotes").hide();
        $("#JobSTCDetails.AdditionalCapacityNotes").val('');
        //$("#JobInstallationDetails_Location").val('');
    }

    $("#btnCustomJobField").click(function () {
        ReloadCustomField();
    });
    DistinctCustomField();
    buttonMapClick();

    showhideAdditionalCapacityNotes();

    $("#JobOwnerDetails_CompanyABN").change(function () {
        $.ajax({
            type: "GET",
            url: urlGetCompanyABN,
            data: { id: $('#JobOwnerDetails_CompanyABN').val() },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {

                if (data == 0) {
                    $('#JobOwnerDetails_CompanyName').empty();
                    $("#JobOwnerDetails_CompanyName").append($("<option></option>").val("").html("Select"));
                    showErrorMessageForPopup("Invalid Company ABN.", $("#errorMsgRegionOwnerPopup"), $("#successMsgRegionInstallerPopup"));
                    return false;
                }
                else {
                    if ($('#JobOwnerDetails_CompanyName option').length > 1) {
                        $('#JobOwnerDetails_CompanyName').empty();
                        $("#JobOwnerDetails_CompanyName").append($("<option></option>").val("").html("Select"));
                        $.each(data, function (key, value) {
                            $("#JobOwnerDetails_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                        });
                    }
                    else {
                        $.each(data, function (key, value) {
                            $("#JobOwnerDetails_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                        });
                    }
                    initializeOwnerDetails(data);
                    return data;
                }
            }
        });
    });
    if ($("#isotherDetails").length == 0 && $("#isCustomDetails").length == 0) {
        $("#otherInstalltionInfo").hide();
    }

    $("#hdnsolarCompanyid").val($("#BasicDetails_SolarCompanyId").val());
    var priority;
    if (BasicDetails_Priority == '') { priority = 0; } else { priority = BasicDetails_Priority; }

    $("#BasicDetails_Priority").change(function () {
        var jobPriority = $("#BasicDetails_Priority option:selected").text();
        $('#BasicDetails_CurrentPriority').val(jobPriority);
    });

    $('#BasicDetails_JobSoldBy').change(function () {
        var jobsoldBy = $('#BasicDetails_JobSoldBy').val();
        $('#BasicDetails_JobSoldByText').val(jobsoldBy);
    });
    debugger;
    jsonvarSoldByDate = $('#BasicDetails_strSoldByDate').val();
    jsonvarJobSoldBy = $('#BasicDetails_SoldBy').val();
    if (jsonvarSoldByDate != '') {
        $('#BasicDetails_SoldByDate').val(jsonvarSoldByDate);
    }
    $('#BasicDetails_SoldByDate').val(jsonvarSoldByDate);
    $('#BasicDetails_JobSoldByText').val(jsonvarJobSoldBy);

    if ($('#BasicDetails_IsAllowTrade').length && $('#BasicDetails_IsAllowTrade').val().toLowerCase() == "false") {
        $('.term').remove();
    }
    CheckInstallationAddressIsValidOrNot();
    setTimeout(function () {
        CheckOwnerAddressIsValidOrNot();
    }, 3000);


});

window.mapsCallback = function () {
    getLatitudeLongitude(DisplayLatLonOfInstallationAdd, $("#installationAdd").html());
};
function LoadInstallationSignatureLocation(srcAddress, isFromDocumentReady = 0) {
    loadMapScript();
    if (isFromDocumentReady == 0) {
        $('#popupMap').modal({ backdrop: 'static', keyboard: false });
    }

    setTimeout(function () {
        $('#txtSource').focus();
    }, 1000);
    $('#txtSource').val(srcAddress);
    $('#txtDestination').val($('#txtAddress').val());

    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
}

function buttonMapClick() {

    $('#btnMap').click(function () {
        $("#lblSrcMap").html("Source");
        $("#txtSource").attr("placeholder", "Source");
        LoadInstallationSignatureLocation('');
    });

    $('#InstallerMap').click(function () {
        $("#lblSrcMap").html("Geo Location");
        $("#txtSource").attr("placeholder", "Geo Location");
        LoadInstallationSignatureLocation($('#InstallerLatLong').val());
    });

    $('#DesignerMap').click(function () {
        $("#lblSrcMap").html("Geo Location");
        $("#txtSource").attr("placeholder", "Geo Location");
        LoadInstallationSignatureLocation($('#DesignerLatLong').val());
    });

    $('#ElectricianMap').click(function () {
        $("#lblSrcMap").html("Geo Location");
        $("#txtSource").attr("placeholder", "Geo Location");
        LoadInstallationSignatureLocation($('#ElectricianLatLong').val());
    });

    $('#OwnerMap').click(function () {
        $("#lblSrcMap").html("Geo Location");
        $("#txtSource").attr("placeholder", "Geo Location");
        LoadInstallationSignatureLocation($('#OwnerLatLong').val());
    });
}

function showHideInstallationAddress(addressId) {
    if (addressId == 1) {
        $('.InstallationDPA').show();
        $('.InstallationPDA').hide();
    }
    else {
        $('.InstallationPDA').show();
        $('.InstallationDPA').hide();
    }
}

function fillInstallationAddressDetail() {

    showHideInstallationAddress(modelInstalltionAddressID);

    $('#JobInstallationDetails_AddressID').change(function () {
        showHideInstallationAddress($('#JobInstallationDetails_AddressID').val());
    });

    fillInstallationAddress = function () {
        if ($("#BasicDetails_JobID").val() != "0") {
            if ($("#JobInstallationDetails_Town").length > 0 && $("#JobInstallationDetails_Town").val().trim() != "" && $("#JobInstallationDetails_Town").val() != undefined && $("#JobInstallationDetails_Town").val() != null) {

                if ($("#JobInstallationDetails_UnitTypeID").find('option').length > 0 && $("#JobInstallationDetails_StreetTypeID").find('option').length > 0 && $("#JobInstallationDetails_PostalAddressID").find('option').length > 0) {
                    var address;
                    var UnitTypeId = $("#JobInstallationDetails_UnitTypeID").find("option:selected").text();
                    var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();
                    var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
                    var StreetName = $("#JobInstallationDetails_StreetName").val();
                    var StreetTypeId = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
                    var PostalAddressID = $("#JobInstallationDetails_PostalAddressID").find("option:selected").text();
                    var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
                    var Town = $("#JobInstallationDetails_Town").val();
                    var State = $("#JobInstallationDetails_State").val();
                    var PostCode = $("#JobInstallationDetails_PostCode").val();
                    if ($("#JobInstallationDetails_AddressID").val() == 1) {
                        if (UnitNumber != "") {
                            address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                            address = address.replace("Select", "");
                        } else {
                            address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                            address = address.replace("Select", "");
                        }
                        InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), UnitType: $("#JobInstallationDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallationDetails_UnitNumber").val(), StreetNumber: $("#JobInstallationDetails_StreetNumber").val(), StreetName: $("#JobInstallationDetails_StreetName").val(), StreetType: $("#JobInstallationDetails_StreetTypeID").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
                    } else {
                        address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
                        InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), PostalAddressID: $("#JobInstallationDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallationDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
                    }
                    InstallationPostcodeFromjson = $("#JobInstallationDetails_PostCode").val();
                    address = address.replace("Select", "");
                    if (address.trim() != "/ ," && address.trim() != "/Select ,") {
                        $("#JobInstallationDetails_AddressDisplay").val(address);
                        $("#txtAddress").val(address); $('#spantxtAddress').hide();
                    }
                }
            }
        }
        DisplayInstallationAdd();
    };
    DisplayInstallationInfo();
    DisplayInstallationExtraInfo();
}

function SameAsOwnerAddressMethod() {
    
    $('#JobInstallationDetails_IsSameAsOwnerAddress').change(function () {
        if ($(this).is(":checked")) {
            debugger;
            $.each(OwnerAddressJson, function (key, value) {

                $('#JobInstallationDetails_UnitNumber').val(value.UnitNumber);
                $('#JobInstallationDetails_UnitTypeID').val(value.UnitType);
                $('#JobInstallationDetails_StreetNumber').val(value.StreetNumber);
                $('#JobInstallationDetails_StreetName').val(value.StreetName);
                $('#JobInstallationDetails_StreetTypeID').val(value.StreetType);
                $('#JobInstallationDetails_Town').val(value.Town);
                $('#JobInstallationDetails_State').val(value.State);
                $('#JobInstallationDetails_PostCode').val(value.PostCode);

                $("#JobInstallationDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
                $('#JobInstallationDetails_PostalAddressID').val(value.PostalAddressID);

            });

            if ($(".OwnerAddress").val() == 1) {
                $(".InstallationAddress").val(1);
                $('.InstallationDPA').show();
                $('.InstallationPDA').hide();
            }
            else {
                $(".InstallationAddress").val(2);
                $('.InstallationPDA').show();
                $('.InstallationDPA').hide();
            }
            $(".popupAddress").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
            $('#spantxtAddress').hide();
            $('#popupAddress').modal('toggle');
        } else {
            $(".popupAddress").find('input[type=text]').each(function () {
                $(this).val('');
                $(this).attr('class', 'form-control valid');
            });
            $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
            $("#JobInstallationDetails_UnitTypeID").val($("#JobInstallationDetails_UnitTypeID option:first").val());
            $("#JobInstallationDetails_StreetTypeID").val($("#JobInstallationDetails_StreetTypeID option:first").val());
            $("#JobInstallationDetails_PostalAddressID").val($("#JobInstallationDetails_PostalAddressID option:first").val());

        }
    });
}

function buttonInstallationDetailClick() {

    $("#btnInstallationInfo").click(function () {
        $('#popupInstalltionInfo').modal({ backdrop: 'static', keyboard: false });
    });

    $("#btnInstallationExtraInfo").click(function () {
        $("#popupInstalltionExtraInfo").modal({ backdrop: 'static', keyboard: false });
    });

    $('#btnAddressDetail').click(function (e) {
        e.preventDefault();

        $('#popupAddress').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#JobInstallationDetails_AddressID').focus();
        }, 1000);

        $(".popupAddress").find('input[type=text]').each(function () {
            $(this).attr('class', 'form-control valid');
        });
        $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
        $.each(InstallationJson, function (key, value) {
            $('#JobInstallationDetails_AddressID').val(value.PostalAddressType);
            $('#JobInstallationDetails_UnitNumber').val(value.UnitNumber);
            $('#JobInstallationDetails_UnitTypeID').val(value.UnitType);
            $('#JobInstallationDetails_StreetNumber').val(value.StreetNumber);
            $('#JobInstallationDetails_StreetName').val(value.StreetName);
            $('#JobInstallationDetails_StreetTypeID').val(value.StreetType);
            $('#JobInstallationDetails_Town').val(value.Town);
            $('#JobInstallationDetails_State').val(value.State);
            $('#JobInstallationDetails_PostCode').val(value.PostCode);
            $("#JobInstallationDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
            $('#JobInstallationDetails_PostalAddressID').val(value.PostalAddressID);
        });

        if ($('#JobInstallationDetails_UnitTypeID option:selected').val() == "") {
            $('#lblInstallationUnitNumber').removeClass("required");
            $('#lblInstallationUnitTypeID').removeClass("required");
            $('#lblInstallationStreetNumber').addClass("required");
        }
        else {
            $('#lblInstallationUnitNumber').addClass("required");
            $('#lblInstallationUnitTypeID').addClass("required");
        }

        if ($('#JobInstallationDetails_AddressID').val() == 1) {
            $('.InstallationDPA').show();
            $('.InstallationPDA').hide();
        }
        else {
            $('.InstallationPDA').show();
            $('.InstallationDPA').hide();
        }

        $("#JobInstallationDetails_LocationValidation").hide();
    });
}

function InstallationAutoComplete() {

    $("#JobInstallationDetails_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: actionProcessRequest,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    if (data1.localities.locality instanceof Array) {
                        response($.map(data1.localities.locality, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    }
                    else {
                        response($.map(data1.localities, function (item) {
                            return {
                                label: item.location + ', ' + item.state + ', ' + item.postcode,
                                value: item.location,
                                state: item.state,
                                postcode: item.postcode
                            }
                        }));
                    }
                }
            })
        },
        select: function (event, ui) {
            $('#JobInstallationDetails_State').val(ui.item.state);
            $('#JobInstallationDetails_PostCode').val(ui.item.postcode);
        }
    });

    $("#JobInstallationDetails_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: actionProcessRequest,
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
            $('#JobInstallationDetails_State').val(ui.item.state);
            $('#JobInstallationDetails_Town').val(ui.item.location);
        }
    });
}

function OwnerAutoComplete() {
    $("#JobOwnerDetails_Town").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: actionProcessRequest,
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
            $('#JobOwnerDetails_State').val(ui.item.state);
            $('#JobOwnerDetails_PostCode').val(ui.item.postcode);

            $('#JobOwnerDetails_State').valid();
            $('#JobOwnerDetails_PostCode').valid();
        }
    });

    $("#JobOwnerDetails_PostCode").autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: actionProcessRequest,
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
            $('#JobOwnerDetails_State').val(ui.item.state);
            $('#JobOwnerDetails_Town').val(ui.item.location);
            $('#JobOwnerDetails_State').valid();
            $('#JobOwnerDetails_Town').valid();
        }
    });
}

function validateOrganisation() {
    if ($("#JobOwnerDetails_OwnerType").val() == 'Government body' || $("#JobOwnerDetails_OwnerType").val() == 'Corporate body' || $("#JobOwnerDetails_OwnerType").val() == 'Trustee') {

        $('#JobOwnerDetails_CompanyABN').removeAttr("disabled");
        $('#JobOwnerDetails_CompanyName').removeAttr("disabled");

        $("#OwnerCompanyABN").addClass("required");
        $("#OwnerCompanyName").addClass("required");
        $("#OwnerEmail").addClass("required");
        $("#OwnerCompanyABNValidate").show();
        $("#OwnerCompanyNameValidate").show();
        $("#OwnerEmailValidate").show();
        $("#JobOwnerDetails_CompanyName").rules("add", {
            required: true,
            messages: {
                required: "Company Name is required."
            }
        });

        $("#JobOwnerDetails_CompanyABN").rules("add", {
            required: true,
            messages: {
                required: "Company ABN is required."
            }
        });
        $("#JobOwnerDetails_Email").rules("add", {
            required: true,
            messages: {
                required: "Email address is required."
            }
        });
    }
    else {
        //remove company mendatory
        $('#JobOwnerDetails_CompanyABN').val('');
        $('#JobOwnerDetails_CompanyABN').prop("disabled", true);
        $("#JobOwnerDetails_CompanyABN").rules('remove');
        $("#JobOwnerDetails_CompanyABN").removeClass('input-validation-error');
        $("#OwnerCompanyABN").removeClass("required");
        $("#OwnerCompanyABNValidate").hide();

        $("#JobOwnerDetails_CompanyName").prop("selectedIndex", 0);
        $('#JobOwnerDetails_CompanyName').prop("disabled", true);
        $("#JobOwnerDetails_CompanyName").rules('remove');
        $("#JobOwnerDetails_CompanyName").removeClass('input-validation-error');
        $("#OwnerCompanyName").removeClass("required");
        $("#OwnerCompanyNameValidate").hide();
        $("#JobOwnerDetails_Email").rules('remove');
        $("#JobOwnerDetails_Email").removeClass('input-validation-error');
        $("#OwnerEmail").removeClass("required");
        $("#OwnerEmailValidate").hide();
    }
}

function showHideOwnerAddress(addressID) {
    if (addressID == 1) {
        $('.OwnerDPA').show();
        $('.OwnerPDA').hide();
    }
    else {
        $('.OwnerPDA').show();
        $('.OwnerDPA').hide();
    }
}

function fillOwnerAddressDetail() {
    showHideOwnerAddress(modelOwnerAddressID);
    $('#JobOwnerDetails_AddressID').change(function () {
        showHideOwnerAddress($('#JobOwnerDetails_AddressID').val())
    });
    fillOwnerAddress = function () {
        if ($("#BasicDetails_JobID").val() != "0") {
            if ($("#JobOwnerDetails_UnitTypeID").find('option').length > 0 && $("#JobOwnerDetails_PostalAddressID").find('option').length > 0 && $("#JobOwnerDetails_StreetTypeID").find('option').length > 0) {
                if ($("#JobOwnerDetails_AddressID").val() == 1) {
                    OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), UnitType: $("#JobOwnerDetails_UnitTypeID").val(), UnitNumber: $("#JobOwnerDetails_UnitNumber").val(), StreetNumber: $("#JobOwnerDetails_StreetNumber").val(), StreetName: $("#JobOwnerDetails_StreetName").val(), StreetType: $("#JobOwnerDetails_StreetTypeID").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
                }
                else {
                    OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), PostalAddressID: $("#JobOwnerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobOwnerDetails_PostalDeliveryNumber").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
                }
                OwnerPostcodeFromjson = $("#JobOwnerDetails_PostCode").val();
                OwnerEmailFromJson = $('#JobOwnerDetails_Email').val();
                OwnerFirstNameFromJson = $("#JobOwnerDetails_FirstName").val();
                OwnerLastNameFromJson = $("#JobOwnerDetails_LastName").val();
            }
        }
        DisplayOwnerAdd();
    };
}

function buttonOwnerDetailClick() {
    $('#btnOwnerDetails').click(function () {
        $('#errorMsgRegionOwnerPopup').hide();
        $('#successMsgRegionInstallerPopup').hide();
        $('#popupOwner').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#JobOwnerDetails_OwnerType').focus();
            $("#JobOwnerDetails_OwnerType").change();
        }, 1000);

        $(".popupOwner").find('input[type=text]').each(function () {

            $(this).attr('class', 'form-control valid');
        });
        $(".popupOwner").find('.field-validation-error').attr('class', 'field-validation-valid');
        RebindOwnerDetailsPopup();
        if ($('#JobOwnerDetails_UnitTypeID option:selected').val() == "") {
            $('#lblOwnerUnitNumber').removeClass("required");
            $('#lblOwnerUnitTypeID').removeClass("required");
            $('#lblOwnerStreetNumber').addClass("required");
        }
        else {
            $('#lblOwnerUnitNumber').addClass("required");
            $('#lblOwnerUnitTypeID').addClass("required");
            $('#lblOwnerStreetNumber').removeClass("required");
        }
        if ($('#JobOwnerDetails_AddressID').val() == 1) {
            $('.OwnerDPA').show();
            $('.OwnerPDA').hide();
        }
        else {
            $('.OwnerPDA').show();
            $('.OwnerDPA').hide();
        }
        $("#JobOwnerDetails_LocationValidation").hide();
    });
}

function RebindOwnerDetailsPopup() {
    $.each(OwnerAddressJson, function (key, value) {
        if ($("#JobOwnerDetails_CompanyABN").val() != null && $("#JobOwnerDetails_CompanyABN").val().length > 0) {
            GetCompanyABN($("#JobOwnerDetails_CompanyABN").val());
        }
        $('#JobOwnerDetails_OwnerType').val(value.OwnerType);
        if (value.CompanyName != '') {
            $("#JobOwnerDetails_CompanyName").val(value.CompanyName);
            $("#JobOwnerDetails_CompanyABN").val(value.CompanyABN);
        }
        $("#JobOwnerDetails_FirstName").val(value.FirstName);
        $('#JobOwnerDetails_LastName').val(value.LastName);
        $('#JobOwnerDetails_Email').val(value.Email);
        $('#JobOwnerDetails_Phone').val(value.Phone);
        $('#JobOwnerDetails_Mobile').val(value.Mobile);
        $('#JobOwnerDetails_AddressID').val(value.PostalAddressType);
        $('#JobOwnerDetails_UnitNumber').val(value.UnitNumber);
        $('#JobOwnerDetails_UnitTypeID').val(value.UnitType);
        $('#JobOwnerDetails_StreetNumber').val(value.StreetNumber);
        $('#JobOwnerDetails_StreetName').val(value.StreetName);
        $('#JobOwnerDetails_StreetTypeID').val(value.StreetType);
        $('#JobOwnerDetails_Town').val(value.Town);
        $('#JobOwnerDetails_State').val(value.State);
        $('#JobOwnerDetails_PostCode').val(value.PostCode);
        $("#JobOwnerDetails_PostalDeliveryNumber").val(value.PostalDeliveryNumber);
        $('#JobOwnerDetails_PostalAddressID').val(value.PostalAddressID);
    });
}

function initializeOwnerDetails(data) {
    $("#JobOwnerDetails_CompanyName").change(function () {
        $.each(data, function (key, value) {
            var Cname = value.CompanyName;
            var drpVal = $('select#JobOwnerDetails_CompanyName option:selected').val();
        });
    });
}

function buttonSoldByClick() {
    $('#btnSoldBy').click(function () {
        $('#popupSoldBy').modal({ backdrop: 'static', keyboard: false });
        if (jsonvarSoldByDate != '') {
            $('#BasicDetails_strSoldByDate').val(jsonvarSoldByDate);
        }
        $('#BasicDetails_JobSoldByText').val(jsonvarJobSoldBy);
    });
}

function DisplaySoldByDate() {
    var SoldByDate = $('#BasicDetails_strSoldByDate').val();
    if (SoldByDate != null && SoldByDate != undefined && SoldByDate != '') {
        $('#BasicDetails_strSoldByDate').val('').removeAttr('value');
        var SoldByDateEdit = moment(SoldByDate).format(dateFormat.toUpperCase());
        $('#BasicDetails_strSoldByDate').val(SoldByDateEdit);
        // display default date
        $("#BasicDetails_strSoldByDate").attr('data-date-format', 'dd/mm/yyyy');

        $('#BasicDetails_strSoldByDate').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    } else {
        $('#BasicDetails_strSoldByDate').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    }
    debugger;

    jsonvarSoldByDate = $('#BasicDetails_SoldByDate').val();
    jsonvarJobSoldBy = $('#BasicDetails_SoldBy').val();
}

function DisplayInstallationDate() {
    var installationDate = $("#BasicDetails_strInstallationDate").val();
    if (installationDate != null && installationDate != undefined && installationDate != '') {
        $("#BasicDetails_strInstallationDate").val('').removeAttr('value');

        var installationDateEdit = moment(installationDate).format(dateFormat.toUpperCase());
        $("#BasicDetails_strInstallationDate").val(installationDateEdit);

        // display default date
        $("#BasicDetails_strInstallationDate").attr('data-date-format', 'dd/mm/yyyy');

        $('.date-pick, .date-pick1, .date-pick').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });

    } else {
        $('.date-pick, .date-pick1, .date-pick').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    }
}

function validateSoldBy() {
    debugger;
    if ($('#BasicDetails_JobSoldByText').val() != '') {
        var soldBy = $('#BasicDetails_JobSoldByText').val();
        var soldbyDate = $('#BasicDetails_strSoldByDate').val();
        $('#BasicDetails_SoldBy').val(soldBy);
        $('#BasicDetails_SoldByDate').val(soldbyDate);
        jsonvarSoldByDate = soldbyDate;
        jsonvarJobSoldBy = $('#BasicDetails_JobSoldByText').val();
        $('#popupSoldBy').modal('toggle');
    }
    else {
        $('#spanSoldBy').show().fadeOut(5000);
    }
}

function clearSoldBy() {
    $('#popupSoldBy').modal('hide');
}

function DisplayInstallationAdd() {
    var addressLine1, addressLine2, addressLine3, streetAddress, postCodeAddress;
    var PostalDeliveryType = $("#JobInstallationDetails_PostalAddressID").val() > 0 ? $("#JobInstallationDetails_PostalAddressID option:selected").text() : "";
    var UnitTypeName = $("#JobInstallationDetails_UnitTypeID").val() > 0 ? $("#JobInstallationDetails_UnitTypeID option:selected").text() : "";
    var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
    var StreetName = $("#JobInstallationDetails_StreetName").val();
    var StreetTypeName = $("#JobInstallationDetails_StreetTypeID").val() > 0 ? $("#JobInstallationDetails_StreetTypeID option:selected").text() : "";
    var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
    var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();

    PostalDeliveryType = (PostalDeliveryType == "" || PostalDeliveryType == null) ? "" : PostalDeliveryType;
    UnitTypeName = (UnitTypeName == "" || UnitTypeName == null) ? "" : UnitTypeName;
    StreetNumber = (StreetNumber == "" || StreetNumber == null) ? "" : StreetNumber;
    streetAddress = StreetNumber + ((StreetName == "" || StreetName == null) ? "" : ' ' + StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? '' : ' ' + StreetTypeName);
    postCodeAddress = $("#JobInstallationDetails_Town").val() + ' ' + $("#JobInstallationDetails_State").val() + ' ' + $("#JobInstallationDetails_PostCode").val();

    if ($("#JobInstallationDetails_AddressID").val() == 1) {
        // physical address
        if ((UnitTypeName == "" || UnitTypeName == null) && (UnitNumber == "" || UnitNumber == null)) {
            addressLine1 = streetAddress;
            addressLine2 = postCodeAddress;
            addressLine3 = "";
        }
        else {
            addressLine1 = UnitTypeName + ((UnitNumber == "" || UnitNumber == null) ? "" : ' ' + UnitNumber)
            addressLine2 = streetAddress;
            addressLine3 = postCodeAddress;
        }
    }
    else {
        // P.o.box
        addressLine1 = PostalDeliveryType + ((PostalDeliveryNumber == "" || PostalDeliveryNumber == null) ? "" : ' ' + PostalDeliveryNumber);
        addressLine2 = postCodeAddress;
    }
    var fullAddress = addressLine1 + '</br>' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br>' + addressLine3 : '');
    $("#installationAdd").html(fullAddress);

}

function DisplayOwnerAdd() {
    debugger;
    var addressLine1, addressLine2, addressLine3, streetAddress, postCodeAddress, companyName, ownerName;

    var PostalDeliveryType = $("#JobOwnerDetails_PostalAddressID").val() > 0 ? $("#JobOwnerDetails_PostalAddressID option:selected").text() : "";
    var UnitTypeName = $("#JobOwnerDetails_UnitTypeID").val() > 0 ? $("#JobOwnerDetails_UnitTypeID option:selected").text() : "";
    var StreetNumber = $("#JobOwnerDetails_StreetNumber").val();
    var StreetName = $("#JobOwnerDetails_StreetName").val();
    var StreetTypeName = $("#JobOwnerDetails_StreetTypeID").val() > 0 ? $("#JobOwnerDetails_StreetTypeID option:selected").text() : "";
    var PostalDeliveryNumber = $("#JobOwnerDetails_PostalDeliveryNumber").val();
    var UnitNumber = $("#JobOwnerDetails_UnitNumber").val();

    PostalDeliveryType = (PostalDeliveryType == "" || PostalDeliveryType == null) ? "" : PostalDeliveryType;
    UnitTypeName = (UnitTypeName == "" || UnitTypeName == null) ? "" : UnitTypeName;
    StreetNumber = (StreetNumber == "" || StreetNumber == null) ? "" : StreetNumber;
    streetAddress = StreetNumber + ((StreetName == "" || StreetName == null) ? "" : ' ' + StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? '' : ' ' + StreetTypeName);
    postCodeAddress = $("#JobOwnerDetails_Town").val() + ' ' + $("#JobOwnerDetails_State").val() + ' ' + $("#JobOwnerDetails_PostCode").val();

    if ($("#JobOwnerDetails_AddressID").val() == 1) {
        // physical address
        if ((UnitTypeName == "" || UnitTypeName == null) && (UnitNumber == "" || UnitNumber == null)) {
            addressLine1 = streetAddress;
            addressLine2 = postCodeAddress;
            addressLine3 = "";
        }
        else {
            addressLine1 = UnitTypeName + ((UnitNumber == "" || UnitNumber == null) ? "" : ' ' + UnitNumber)
            addressLine2 = streetAddress;
            addressLine3 = postCodeAddress;
        }
    }
    else {
        // P.o.box
        addressLine1 = PostalDeliveryType + ((PostalDeliveryNumber == "" || PostalDeliveryNumber == null) ? "" : ' ' + PostalDeliveryNumber);
        addressLine2 = postCodeAddress;
    }

    companyName = $("#JobOwnerDetails_CompanyName").val();
    ownerName = $("#JobOwnerDetails_FirstName").val() + ' ' + $("#JobOwnerDetails_LastName").val();

    var fullAddress = companyName + '</br>' + ownerName + '</br>' + addressLine1 + '</br>' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br>' + addressLine3 : '');
    var ownerFullAddress = addressLine1+','  + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ?',' + addressLine3 : '');
    $("#ownerFullAddress").html(ownerFullAddress);
    $("#ownerAdd").html(fullAddress);
    $("#ownerTypeName").html($("#JobOwnerDetails_OwnerType").val());
    var email = $("#JobOwnerDetails_Email").val();
    $("#phoneOwner").html($("#JobOwnerDetails_Phone").val());
    $("#mobileOwner").html($("#JobOwnerDetails_Mobile").val());
    $(".emailOwnerA").html($("#JobOwnerDetails_Email").val());
    $(".emailOwnerA").attr('href', "mailto:'" + email + "'");
    $(".emailOwnerA").attr('title', $("#JobOwnerDetails_Email").val());
}

function DisplayInstallationInfo(isJobAdded) {
    if ($("#JobInstallationDetails_DistributorID").val() > 0) {
        $("#lblDistributor").html($("#JobInstallationDetails_DistributorID option:selected").text());
        if (isJobAdded)
            $("#lblDistributor").data('id', $("#JobInstallationDetails_DistributorID").val());
    }
    else
        $("#lblDistributor").html('');

    if ($("#JobInstallationDetails_ElectricityProviderID").val() > 0)
        $("#lblElectricityProvider").html($("#JobInstallationDetails_ElectricityProviderID option:selected").text());
    else
        $("#lblElectricityProvider").html('');

    if ($("#JobInstallationDetails_PhaseProperty").val() > 0)
        $("#lblPhaseProperty").html($("#JobInstallationDetails_PhaseProperty option:selected").text());
    else
        $("#lblPhaseProperty").html('');

    $("#lblNMI").html($("#JobInstallationDetails_NMI").val());
    $("#lblMeterNumber").html($("#JobInstallationDetails_MeterNumber").val());
    $("#lblLatitude").html(modelInstallationLatitude);
    $("#lblLongitude").html(modelInstallationLongitude);

}

function DisplayInstallationExtraInfo() {
    $("#lblOwnerHasExistingSystem").html($('#JobInstallationDetails_ExistingSystem').is(":checked") ? "Yes" : "No");
    if ($('#JobInstallationDetails_ExistingSystem').is(":checked")) {
        $("#pExistingSystemSize").show();
        $("#pSystemLocation").show();
        $("#pNoOfInstallationPanel").show();
        $("#lblExistingSystemSize").html($("#JobInstallationDetails_ExistingSystemSize").val());
        $("#lblSystemLocation").html($("#JobInstallationDetails_SystemLocation").val());
        $("#lblNoOfInstallationPanel").html($("#JobInstallationDetails_NoOfPanels").val());
    }
    else {
        $("#pExistingSystemSize").hide();
        $("#pSystemLocation").hide();
        $("#pNoOfInstallationPanel").hide();
        $("#lblExistingSystemSize").html('');
        $("#lblSystemLocation").html('');
        $("#lblNoOfInstallationPanel").html('');
    }
    $("#lblAdditionalInstallationInformation").html($("#JobInstallationDetails_AdditionalInstallationInformation").val());
    $('#customFields').find('textarea').each(function () {
        $('#customDetails').find('[id=' + this.id + ']').text(this.value);
    });

}

function validateOwner() {
    debugger;
    var isValid = addressValidationRules("JobOwnerDetails");
    if (isValid && $('#JobOwner').valid()) {
        var PreviousFirstName = $('#JobOwnerDetails_PreviousFirstName').val();
        var PreviousLastName = $('#JobOwnerDetails_PreviousLastName').val();
        var PreviousEmail = $('#JobOwnerDetails_PreviousEmail').val();
        var PreviousMobile = $('#JobOwnerDetails_PreviousMobile').val();
        var PreviousPhone = $('#JobOwnerDetails_PreviousPhone').val();
        var PreviousOwnerType = $('#JobOwnerDetails_PreviousOwnerType').val();
        var PreviousCompanyName = $('#JobOwnerDetails_PreviousCompanyName').val();
        var OldABNNumber = $('#JobOwnerDetails_OldABNNumber').val();
        //var PreviousAddressID = $('#JobOwnerDetails_PreviousAddressID').val();
        //var PreviousUnitTypeID = $('#JobOwnerDetails_PreviousUnitTypeID').val();
        //var PreviousUnitNumber = $('#JobOwnerDetails_PreviousUnitNumber').val();
        //var PreviousStreetNumber = $('#JobOwnerDetails_PreviousStreetNumber').val();
        //var PreviousStreetName = $('#JobOwnerDetails_PreviousStreetName').val();
        //var PreviousStreetTypeID = $('#JobOwnerDetails_PreviousStreetTypeID').val();
        //var PreviousTown = $('#JobOwnerDetails_PreviousTown').val();
        //var PreviousState = $('#JobOwnerDetails_PreviousState').val();
        //var PreviousPostCode = $('#JobOwnerDetails_PreviousPostCode').val();
        var oldOwnerAddress = $('#ownerFullAddress').html();

        var addressLine1, addressLine2, addressLine3, streetAddress, postCodeAddress, companyName, ownerName;

        var PostalDeliveryType = $("#JobOwnerDetails_PostalAddressID").val() > 0 ? $("#JobOwnerDetails_PostalAddressID option:selected").text() : "";
        var UnitTypeName = $("#JobOwnerDetails_UnitTypeID").val() > 0 ? $("#JobOwnerDetails_UnitTypeID option:selected").text() : "";
        var StreetNumber = $("#JobOwnerDetails_StreetNumber").val();
        var StreetName = $("#JobOwnerDetails_StreetName").val();
        var StreetTypeName = $("#JobOwnerDetails_StreetTypeID").val() > 0 ? $("#JobOwnerDetails_StreetTypeID option:selected").text() : "";
        var PostalDeliveryNumber = $("#JobOwnerDetails_PostalDeliveryNumber").val();
        var UnitNumber = $("#JobOwnerDetails_UnitNumber").val();

        PostalDeliveryType = (PostalDeliveryType == "" || PostalDeliveryType == null) ? "" : PostalDeliveryType;
        UnitTypeName = (UnitTypeName == "" || UnitTypeName == null) ? "" : UnitTypeName;
        StreetNumber = (StreetNumber == "" || StreetNumber == null) ? "" : StreetNumber;
        streetAddress = StreetNumber + ((StreetName == "" || StreetName == null) ? "" : ' ' + StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? '' : ' ' + StreetTypeName);
        postCodeAddress = $("#JobOwnerDetails_Town").val() + ' ' + $("#JobOwnerDetails_State").val() + ' ' + $("#JobOwnerDetails_PostCode").val();

        if ($("#JobOwnerDetails_AddressID").val() == 1) {
            // physical address
            if ((UnitTypeName == "" || UnitTypeName == null) && (UnitNumber == "" || UnitNumber == null)) {
                addressLine1 = streetAddress;
                addressLine2 = postCodeAddress;
                addressLine3 = "";
            }
            else {
                addressLine1 = UnitTypeName + ((UnitNumber == "" || UnitNumber == null) ? "" : ' ' + UnitNumber)
                addressLine2 = streetAddress;
                addressLine3 = postCodeAddress;
            }
        }
        else {
            // P.o.box
            addressLine1 = PostalDeliveryType + ((PostalDeliveryNumber == "" || PostalDeliveryNumber == null) ? "" : ' ' + PostalDeliveryNumber);
            addressLine2 = postCodeAddress;
        }

        companyName = $("#JobOwnerDetails_CompanyName").val();
        ownerName = $("#JobOwnerDetails_FirstName").val() + ' ' + $("#JobOwnerDetails_LastName").val();

        var ownerFullAddress = addressLine1 + ',' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? ',' + addressLine3 : '');
        
        var data = JSON.stringify($("#JobOwner").serializeToJson());
        var objData = JSON.parse(data);
        var obj = {};
        obj.jobOwnerDetails = objData.JobOwnerDetails;
        obj.jobId = jobId;
        OwnerAddressCheck(objData.JobOwnerDetails);


        obj.jobOwnerDetails.oldOwnerAddress = oldOwnerAddress;
        obj.jobOwnerDetails.NewOwnerAddress = ownerFullAddress;
        obj.isGST = $("#BasicDetails_IsGst").val();
        obj.jobOwnerDetails.PreviousFirstName = PreviousFirstName;
        obj.jobOwnerDetails.PreviousLastName = PreviousLastName;
        obj.jobOwnerDetails.PreviousEmail = PreviousEmail;
        obj.jobOwnerDetails.PreviousMobile = PreviousMobile;
        obj.jobOwnerDetails.PreviousPhone = PreviousPhone;
        obj.jobOwnerDetails.PreviousOwnerType = PreviousOwnerType;
        obj.jobOwnerDetails.PreviousCompanyName = PreviousCompanyName;
        obj.jobOwnerDetails.OldABNNumber = OldABNNumber;
        //obj.jobOwnerDetails.PreviousAddressID = PreviousAddressID;
        //obj.jobOwnerDetails.PreviousUnitTypeID = PreviousUnitTypeID;
        //obj.jobOwnerDetails.PreviousUnitNumber = PreviousUnitNumber;
        //obj.jobOwnerDetails.PreviousStreetNumber = PreviousStreetNumber;
        //obj.jobOwnerDetails.PreviousStreetName = PreviousStreetName;
        //obj.jobOwnerDetails.PreviousStreetTypeID = PreviousStreetTypeID;
        //obj.jobOwnerDetails.PreviousTown = PreviousTown;
        //obj.jobOwnerDetails.PreviousState = PreviousState;
        //obj.jobOwnerDetails.PreviousPostCode = PreviousPostCode;
        //obj.jobOwnerDetails.UniTypeName = $('#JobOwnerDetails_UnitType').find("option:selected").text();
        //obj.jobOwnerDetails.StreetTypeIDName = $('#JobOwnerDetails_StreetTypeID').find("option:selected").text();
        $.ajax({
            url: urlUpdateJobOwnerDetails,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify(obj),
            async: false,
            success: function (response) {
                if (response.status) {
                    debugger;
                    modelIsOwnerRegisteredWithGST = response.isOwnerRegisteredWithGST;
                    OwnerAddressJson = [];
                    var clientname;
                    if ($("#JobOwnerDetails_OwnerType").val() == 'Government body' || $("#JobOwnerDetails_OwnerType").val() == 'Corporate body' || $("#JobOwnerDetails_OwnerType").val() == 'Trustee') {
                        clientname = $("#JobOwnerDetails_CompanyName").val();
                    } else {
                        var FirstName = $("#JobOwnerDetails_FirstName").val();
                        var LastName = $("#JobOwnerDetails_LastName").val();
                        clientname = FirstName + ' ' + LastName;
                    }
                    if ($("#JobOwnerDetails_AddressID").val() == 1)
                        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", false)
                    else
                        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", true)
                    $('#spantxtClientNames').hide();
                    $("#txtClientName").val(clientname);
                    //$('#popupOwner').modal('toggle');
                    if ($("#JobOwnerDetails_AddressID").val() == 1) {
                        OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), UnitType: $("#JobOwnerDetails_UnitTypeID").val(), UnitNumber: $("#JobOwnerDetails_UnitNumber").val(), StreetNumber: $("#JobOwnerDetails_StreetNumber").val(), StreetName: $("#JobOwnerDetails_StreetName").val(), StreetType: $("#JobOwnerDetails_StreetTypeID").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });

                        modelOwnerStreetName = $("#JobOwnerDetails_StreetName").val();
                        modelOwnerStreetNumber = $("#JobOwnerDetails_StreetNumber").val();
                        modelOwnerStreetTypeID = $("#JobOwnerDetails_StreetTypeID").val();
                        modelOwnerTown = $("#JobOwnerDetails_Town").val();
                        modelOwnerState = $("#JobOwnerDetails_State").val();
                        modelOwnerPostCode = $("#JobOwnerDetails_PostCode").val();

                    } else {
                        OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), PostalAddressID: $("#JobOwnerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobOwnerDetails_PostalDeliveryNumber").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
                    }
                    OwnerPostcodeFromjson = $("#JobOwnerDetails_PostCode").val();
                    OwnerEmailFromJson = $('#JobOwnerDetails_Email').val();
                    OwnerFirstNameFromJson = $("#JobOwnerDetails_FirstName").val();
                    OwnerLastNameFromJson = $("#JobOwnerDetails_LastName").val();

                 


                    DisplayOwnerAdd();
                    $('#popupOwner').modal('toggle');
                    //$("#jobGST").show();

                    //DeleteGSTDocument('.GSTDocumentDelete',false);
                    SavedData = CommonDataForSave();

                    var updateoldFirstName = $("#JobOwnerDetails_FirstName").val();
                    var updateoldLastName = $("#JobOwnerDetails_LastName").val();
                    var updateoldEmail = $('#JobOwnerDetails_Email').val();
                    var updateoldMobile = $('#JobOwnerDetails_Mobile').val();
                    var updateoldPhone = $('#JobOwnerDetails_Phone').val();
                    var updateOldOwnerType = $('#JobOwnerDetails_OwnerType').val();
                    var updateOldCompanyName = $('#JobOwnerDetails_CompanyName').val();
                    var updateOldCompanyABN = $('#JobOwnerDetails_CompanyABN').val();
                    //var updateOldOwnerAddressID = $('#JobOwnerDetails_AddressID').val();
                    //var updateOldOwnerUnitTypeID = $('#JobOwnerDetails_UnitTypeID').val();
                    //var updateOldUnitNumber = $('#JobOwnerDetails_UnitNumber').val();
                    //var updateOldStreetNumber = $('#JobOwnerDetails_StreetNumber').val();
                    //var updateOldStreetName = $('#JobOwnerDetails_StreetName').val();
                    //var updateOldStreetType = $('#JobOwnerDetails_StreetTypeID').val();
                    //var updateOldTown = $('#JobOwnerDetails_Town').val();
                    //var updateOldState = $('#JobOwnerDetails_State').val();
                    //var updateOldPostCode = $('#JobOwnerDetails_PostCode').val();
                    $("#JobOwnerDetails_PreviousFirstName").val(updateoldFirstName);
                    $("#JobOwnerDetails_PreviousLastName").val(updateoldLastName);
                    $("#JobOwnerDetails_PreviousEmail").val(updateoldEmail);
                    $("#JobOwnerDetails_PreviousMobile").val(updateoldMobile);
                    $("#JobOwnerDetails_PreviousPhone").val(updateoldPhone);
                    $('#JobOwnerDetails_PreviousOwnerType').val(updateOldOwnerType);
                    $('#JobOwnerDetails_OldABNNumber').val(updateOldCompanyABN);
                    $('#JobOwnerDetails_PreviousCompanyName').val(updateOldCompanyName);
                    //$('#JobOwnerDetails_PreviousAddressID').val(updateOldOwnerAddressID);
                    //$('#JobOwnerDetails_PreviousUnitTypeID').val(updateOldOwnerUnitTypeID);
                    //$('#JobOwnerDetails_PreviousUnitNumber').val(updateOldUnitNumber);
                    //$('#JobOwnerDetails_PreviousStreetNumber').val(updateOldStreetNumber);
                    //$('#JobOwnerDetails_PreviousStreetName').val(updateOldStreetName);
                    //$('#JobOwnerDetails_PreviousStreetTypeID').val(updateOldStreetType);
                    //$('#JobOwnerDetails_PreviousTown').val(updateOldTown);
                    //$('#JobOwnerDetails_PreviousState').val(updateOldState);
                    //$('#JobOwnerDetails_PreviousPostCode').val(updateOldPostCode);
                    ReloadSTCJobScreen(jobId);
                   // SearchHistory();
                }
                else
                    showErrorMessageForPopup(response.msg, $("#errorMsgRegionOwnerPopup"), $("#successMsgRegionOwnerPopup"));
                ShowHideGSTSection($('#JobInstallationDetails_PropertyType').val().toLowerCase(), $("#JobOwnerDetails_OwnerType").val().toLowerCase());
            }
        });
    }
}
function clearPopupOwner() {
    $(".popupOwner").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupOwner").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobOwnerDetails_UnitTypeID").val($("#JobOwnerDetails_UnitTypeID option:first").val());
    $("#JobOwnerDetails_StreetTypeID").val($("#JobOwnerDetails_StreetTypeID option:first").val());
    $("#JobOwnerDetails_PostalAddressID").val($("#JobOwnerDetails_PostalAddressID option:first").val());
    $("#JobOwnerDetails_LocationValidation").hide();
}

function validateInstallation() {
    debugger;
    oldaddress = $("#JobInstallationDetails_AddressDisplay").val();
    $("#JobInstallationDetails_oldInstallationAddress").val(oldaddress);
    var isValid = addressValidationRules("JobInstallationDetails");
    if (isValid && $('#JobInstallationAddress').valid()) {
        InstallationJson = [];
        var address;
        var UnitTypeId = $("#JobInstallationDetails_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();
        var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
        var StreetName = $("#JobInstallationDetails_StreetName").val();
        var StreetTypeId = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
        var PostalAddressID = $("#JobInstallationDetails_PostalAddressID").find("option:selected").text();
        var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
        var Town = $("#JobInstallationDetails_Town").val();
        var State = $("#JobInstallationDetails_State").val();
        var PostCode = $("#JobInstallationDetails_PostCode").val();
        if ($("#JobInstallationDetails_AddressID").val() == 1) {
            if (UnitNumber != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), UnitType: $("#JobInstallationDetails_UnitTypeID").val(), UnitNumber: $("#JobInstallationDetails_UnitNumber").val(), StreetNumber: $("#JobInstallationDetails_StreetNumber").val(), StreetName: $("#JobInstallationDetails_StreetName").val(), StreetType: $("#JobInstallationDetails_StreetTypeID").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });
        } else {
            address = PostalAddressID + ' ' + PostalDeliveryNumber + ', ' + Town + ' ' + State + ' ' + PostCode;
            InstallationJson.push({ PostalAddressType: $("#JobInstallationDetails_AddressID").val(), PostalAddressID: $("#JobInstallationDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobInstallationDetails_PostalDeliveryNumber").val(), Town: $("#JobInstallationDetails_Town").val(), State: $("#JobInstallationDetails_State").val(), PostCode: $("#JobInstallationDetails_PostCode").val() });

        }
        InstallationPostcodeFromjson = $("#JobInstallationDetails_PostCode").val();
        $("#JobInstallationDetails_AddressDisplay").val(address);
        $("#txtAddress").val(address);
        $('#spantxtAddress').hide();
        $('#popupAddress').modal('toggle');

        DisplayInstallationAdd();
        getlatLongFromInstallationAddress(DisplayLatLonOfInstallationAdd, $("#txtAddress").val());
       // SearchHistory();
    }
}
function clearPopupInstallation() {
    $(".popupAddress").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $("#JobInstallationDetails_StreetTypeID").attr("class", "form-control valid")
    $(".popupAddress").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobInstallationDetails_UnitTypeID").val($("#JobInstallationDetails_UnitTypeID option:first").val());
    $("#JobInstallationDetails_StreetTypeID").val($("#JobInstallationDetails_StreetTypeID option:first").val());
    $("#JobInstallationDetails_PostalAddressID").val($("#JobInstallationDetails_PostalAddressID option:first").val());
    $("#JobInstallationDetails_LocationValidation").hide();
}
function loadMapScript(isFromPhotoView = 0) {
    var scriptMap = document.createElement("script");
    scriptMap.type = "text/javascript";
    var src;
    if (isFromPhotoView != 1) {
        src = JobMapKeyUrl + '&callback=mapsCallback';
    }
    else {
        src = JobMapKeyUrl;
    }
    src = src.toString().replace(/&amp;/g, '&');
    scriptMap.src = src;

    var len = $('script[src="' + src + '"]').length;
    if (len <= 0) {
        if (scriptMap.readyState) {  //IE
            scriptMap.onreadystatechange = function () {
                if (scriptMap.readyState == "loaded" ||
                    scriptMap.readyState == "complete") {
                    scriptMap.onreadystatechange = null;
                    loadMap(isFromPhotoView);
                    if (isFromPhotoView != 1) {
                        var a = setTimeout(function () {
                            if (ProjectSession_UserTypeId == 1) {
                                geocodeAddress($('#txtAddress').val());
                            }
                        }, 1000);
                    }

                }
            };
        } else {  //Others
            scriptMap.onload = function () {
                loadMap(isFromPhotoView);
                if (isFromPhotoView != 1) {
                    var a = setTimeout(function () {
                        if (ProjectSession_UserTypeId == 1) {
                            geocodeAddress($('#txtAddress').val());
                        }
                    }, 1000);
                }
            };
        }

        document.body.appendChild(scriptMap);
    }
    else {
        if (isFromPhotoView != 1) {
            var a = setTimeout(function () {
                if (ProjectSession_UserTypeId == 1) {
                    geocodeAddress($('#txtAddress').val());
                }
            }, 1000);
        }
    }
}

function loadMap(isFromPhotoView = 0) {

    //  var map = new google.maps.Map(document.getElementById('dvMap'));
    var map;
    if (isFromPhotoView == 1) {
        map = new google.maps.Map(document.getElementById('dMap'));
    }
    else {
        map = new google.maps.Map(document.getElementById('dvMap'));
    }
    directionsService = new google.maps.DirectionsService();
    new google.maps.places.SearchBox(document.getElementById('txtSource'));
    new google.maps.places.SearchBox(document.getElementById('txtDestination'));

    directionsDisplay = new google.maps.DirectionsRenderer({
        draggable: false
    });

    google.maps.event.addListener(directionsDisplay, 'directions_changed', function () {

        directions = directionsDisplay.getDirections();
        var distance = directions.routes[0].legs[0].distance.text;
        var duration = directions.routes[0].legs[0].duration.text;
        var source = directions.routes[0].legs[0].start_address;
        var dest = directions.routes[0].legs[0].end_address;
        var dvDistance = document.getElementById("dvDistance");
        dvDistance.innerHTML = "";
        dvDistance.innerHTML += "Distance: " + distance + "   ";
        dvDistance.innerHTML += "Duration:" + duration;
        $("#txtSource").val(source);
        $("#txtDestination").val(dest);
    });

}
function GetRoute(isFromPhotoView = 0) {
    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;

    if (source != "" && destination != "") {
        var India = new google.maps.LatLng(51.508742, -0.120850);
        var mapOptions = {
            zoom: 4,
            center: India
        };
        //map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
        if (isFromPhotoView == 1) {
            map = new google.maps.Map(document.getElementById('dMap'), mapOptions);
        }
        else {
            map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
        }
        directionsDisplay.setMap(map);
        //*********DIRECTIONS AND ROUTE**********************//
        var request = {
            origin: source,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
            else {
                createMarker(destination, 25.2744, 133.7751);
                $("#errorMsgRegionMap").html(closeButton + " Both Source and Destination  are not in same country.");
                $("#errorMsgRegionMap").show();
                //$("#errorMsgRegionMap").fadeOut(5000);
            }
        });
    }
    else {
        $("#errorMsgRegionMap").html(closeButton + " Both Source and Destination  address are  required");
        $("#errorMsgRegionMap").show();
        //$("#errorMsgRegionMap").fadeOut(5000);
    }
}
function GetLocation() {
    latitude = '';
    longitude = '';
    latitude1 = '';
    longitude1 = '';

    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    //$("#dvMap").html('');
    locations = [];
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;


    geocoder = new google.maps.Geocoder();

    if (source != "" || destination != "") {

        if (source != "") {
            geocoder.geocode({ 'address': source }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude = results[0].geometry.location.lat();
                    longitude = results[0].geometry.location.lng();
                    sourcedetail = [source, latitude, longitude];
                    locations.push(sourcedetail);
                    GetLocationOnMap(latitude, longitude);
                }
                else {
                    $("#errorMsgRegionMap").html(closeButton + "Invalid source Address.");
                    $("#errorMsgRegionMap").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createMarker('Victoria , Australia', -37.4713, 144.7852);
                }
            });
        }
        if (destination != "") {
            geocoder.geocode({ 'address': destination }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude1 = results[0].geometry.location.lat();
                    longitude1 = results[0].geometry.location.lng();
                    destinationdetail = [destination, latitude1, longitude1];
                    locations.push(destinationdetail);
                    GetLocationOnMap(latitude1, longitude1);
                }
                else {
                    $("#errorMsgRegionMap").html(closeButton + "Invalid Destination Address.");
                    $("#errorMsgRegionMap").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createMarker('Victoria , Australia', -37.4713, 144.7852);
                }
            });
        }
    }
    else {
        $("#errorMsgRegionMap").html(closeButton + "Source or Destination address  are  required");
        $("#errorMsgRegionMap").show();
        //$("#errorMsgRegionMap").fadeOut(5000);
    }
}
function GetLocationOnMap(latitude, longitude) {
    var bounds = new google.maps.LatLngBounds();
    var infowindow = new google.maps.InfoWindow();
    var lat = latitude;
    var lng = longitude;
    var map = new google.maps.Map(document.getElementById('dvMap'), {
        zoom: 10,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });

    var marker, i;
    for (i = 0; i < locations.length; i++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[i][1], locations[i][2]),
            draggable: false,
            map: map
        });

        //extend the bounds to include each marker's position
        bounds.extend(marker.position);

        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infowindow.setContent(locations[i][0]);
                infowindow.open(map, marker);
            }
        })(marker, i));
    }
    //now fit the map to the newly inclusive bounds
    map.fitBounds(bounds);
}
function geocodeAddress(address, isownerAddress = false, isInstallationAdd = false) {
    $("#errorMsgRegionMap").hide();
    var geocoder = new google.maps.Geocoder();
    if (address.trim() == "") {

        createMarker('Victoria , Australia', -37.4713, 144.7852);
    }
    else {
        geocoder.geocode({ address: address }, function (results, status) {
            var addressList = address.split(' ');
            var resultAddressList = [];

            if (results != null) {
                if (results[0].address_components != null && results[0].address_components.length > 0) {
                    for (var i = 0; i < results[0].address_components.length; i++) {
                        resultAddressList.push((results[0].address_components[i].long_name + ",").toLowerCase());
                    }
                }
            }
            if (isInstallationAdd == true && addressList[1] != undefined && addressList[1] != null && resultAddressList != undefined && resultAddressList != null) {
                if (resultAddressList.includes((addressList[1]+" " + addressList[2]).replace(",","").toLowerCase().trim() + ",")) {
                    $("#errorMsgValidInstallationAddress").hide();
                    $('#installationAdd').css('color', '');
                    installationAddressValidationFlag(true);
                }
                else {
                    $("#errorMsgValidInstallationAddress").html(closeButton + "Installation address does not match google street address.");
                    $("#errorMsgValidInstallationAddress").show();
                    $('#installationAdd').css('color', 'Red');
                    installationAddressValidationFlag(false);
                }
            }

            if (isownerAddress == true && addressList[1] != undefined && addressList[1] != null && resultAddressList != undefined && resultAddressList != null) {
                if (resultAddressList.includes((addressList[1] + " " + addressList[2]).replace(",", "").toLowerCase().trim() + ",")) {
                    $("#errorMsgValidOwnerAddress").hide();
                    $('#ownerAdd').css('color', '');
                    OwnerAddressValidationFlag(true);
                }
                else {
                    $("#errorMsgValidOwnerAddress").html(closeButton + "Owner address does not match google street address.");
                    $("#errorMsgValidOwnerAddress").show();
                    $('#ownerAdd').css('color', 'Red');
                    OwnerAddressValidationFlag(false);
                }
            }
            if (status == google.maps.GeocoderStatus.OK) {
                var p = results[0].geometry.location;
                var lat = p.lat();
                var lng = p.lng();
                createMarker(address, lat, lng);
            }
            else {
                $("#errorMsgRegionMap").html(closeButton + "Invalid Destination Address.");
                $("#errorMsgRegionMap").show();
                createMarker('Victoria , Australia', -37.4713, 144.7852);
            }
        }
        );
    }
}

function installationAddressValidationFlag(flag) {
    $.ajax({
        url: installationAddValidFlagChange,
        type: 'post',
        dataType: "json",
        data: { isValid: flag, JobId: jobId },
        success: function (response) {
        },
        error: function (response) {
        }
    });
}
function OwnerAddressValidationFlag(flag) {
    $.ajax({
        url: OwnerAddValidFlagChange,
        type: 'post',
        dataType: "json",
        data: { isValid: flag, JobId: jobId },
        success: function (response) {
        },
        error: function (response) {
        }
    });
}
function createMarker(add, lat, lng) {
    var infowindow = new google.maps.InfoWindow();
    var latlng = new google.maps.LatLng(lat, lng);

    var map = new google.maps.Map(document.getElementById('dvMap'), {
        zoom: 12,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });
    var bounds = new google.maps.LatLngBounds();

    var contentString = add;
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, lng),
        center: latlng,
        map: map,
        draggable: false
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(contentString);
        infowindow.open(map, marker);
    });
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function showErrorMessageForPopup(message, objError, objSuccess) {
    $(".alert").hide();
    if (objSuccess)
        objSuccess.hide();
    objError.html(closeButton + message);
    objError.show();
}

function showSuccessMessageForPopup(message, objSuccess, objError) {
    $(".alert").hide();
    if (objError)
        objError.hide();
    objSuccess.html(closeButton + message);
    objSuccess.show();
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function validateInstallationInfo() {
    if ($('#JobInstallationDetailInfo').valid()) {
        DisplayInstallationInfo();
        $('#popupInstalltionInfo').modal('toggle');
    }
}

function validateInstallationExtraInfo() {
    DisplayInstallationExtraInfo();
    $('#popupInstalltionExtraInfo').modal('toggle');
}

function clearInstallationInfo() {
    $("#installtionInfo").find('input[type=text]').each(function () {
        $(this).val('');
    });
    $("#installtionInfo").find('select ').each(function () {
        $(this).val('');
    });
}

function clearInstallationExtraInfo() {
    $("#installtionExtraInfo").find('input[type=text]').each(function () {
        $(this).val('');
    });
    $("#installtionExtraInfo").find('textarea').each(function () {
        $(this).val('');
    });
    $("#installtionExtraInfo").find('input[type=checkbox]').each(function () {
        $(this).prop('checked', false).trigger('change');
    });
}

function getLatitudeLongitude(callback, address) {
    if ((typeof (google) != 'undefined')) {
        // Initialize the Geocoder
        geocoder = new google.maps.Geocoder();
        if (geocoder) {
            geocoder.geocode({
                'address': address
            }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    callback(results[0]);
                }
                else {
                    callback(null);
                }
            });
        }
    }
}

function getlatLongFromInstallationAddress(callback, address) {
    if ((typeof (google) == 'undefined')) {
        var scriptMap = document.createElement("script");
        scriptMap.type = "text/javascript";
        var src = JobMapKeyUrl;
        src = src.toString().replace(/&amp;/g, '&');
        scriptMap.src = src;
        document.body.appendChild(scriptMap);
    }
    setTimeout(function () { getLatitudeLongitude(callback, address) }, 1000);
}

function DisplayLatLonOfInstallationAdd(result) {
    if (result == null) {
        $("#lblLatitude").html("");
        $("#lblLongitude").html("");
    }
    else {

        $("#lblLatitude").html(result.geometry.location.lat());
        $("#lblLongitude").html(result.geometry.location.lng());
    }
}

function ShowHideOtherInstalltionInfo(obj) {
    if ($(obj).attr('isShow') == 1) {
        $("#otherInstalltionInfo").hide();
        $(obj).attr('isShow', '0');
        $(obj).html('Show More');
    }
    else {
        $("#otherInstalltionInfo").show();
        $(obj).attr('isShow', '1');
        $(obj).html('Show Less');
    }
}

function OwnerHasExistingSystem(value) {
    if (value.toString().toLowerCase() == 'false') {
        $("#ExistingSystem").hide();
    } else {
        $("#ExistingSystem").show();
    }
}

//function ShowHideSWHPVDField() {
//    //PVD job
//    if (modelBasicJobType == '1') {
//        $(".gridPVD").show();
//        $(".gridSWH").hide();
//        FillDropDownOfPanelBrandModel();
//        FillDropDownOfInverterBrandModel();
//        FillDropDownOfBatteryManufacturerModel();
//        $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").show();
//        $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").show();
//        $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem").show();
//        $(".lblPVD").show();
//        $("#ExistingSystem1,#JobInstallationDetails_ExistingSystem,#ExistingSystem").show();
//        $('.addInfo').removeClass('col-md-12');
//        $('.addInfo').addClass('col-md-6');
//        $("#MultipleSGUAddress").html("Is there more than one SGU at this address?:");
//        $("#CertificateCreated").html("Are you creating certificates for a system that has previously been failed by the Clean Energy Regulator?:");
//        $("#divVolumetricCapacity").hide();
//        $("#divStatutoryDeclarations").hide();
//        $("#divSecondhandWaterHeater").hide();
//        var insatallerid = modelBasicInstallerID || 0;
//        var eleParam = [{ JobID: $("#BasicDetails_JobID").val(), solarCompanyId: $("#BasicDetails_SolarCompanyId").val() }];
//        var installerParam = [{ isInstaller: true, existUserId: insatallerid, solarCompanyId: $("#BasicDetails_SolarCompanyId").val(), jobId: jobId }];
//        dropDownData.push({ id: 'JobElectricians_InstallerID', key: "", value: insatallerid, hasSelect: true, callback: null, defaultText: null, proc: 'InstallerDesignerEle', param: installerParam, bText: 'Text', bValue: 'Value' });
//        $("#jobSSCID").show();
//    }
//    //SWH job
//    else if (modelBasicJobType == '2') {
//        $(".gridSWH").show();
//        $(".gridPVD").hide();
//        FillDropDownOfSystemBrandModel();
//        $("#TypeOfConnection,#JobSTCDetails_TypeOfConnection,#SystemMountingType,#JobSTCDetails_SystemMountingType").hide();
//        $("#DeemingPeriod,#JobSTCDetails_DeemingPeriod,#InstallerID,#BasicDetails_InstallerID,#DesignerID,#BasicDetails_DesignerID").hide();
//        $("#NMI,#JobInstallationDetails_NMI,#InstallingNewPanel,#InstallingNewPanel,#InstallingNewPanel,#JobInstallationDetails_InstallingNewPanel,#MeterNumber,#JobInstallationDetails_MeterNumber,#PhaseProperty,#JobInstallationDetails_PhaseProperty,#ElectricityProviderID,#JobInstallationDetails_ElectricityProviderID,#ExistingSystem1,#JobInstallationDetails_ExistingSystem,#ExistingSystem").hide();
//        $(".lblPVD").hide();
//        $("#ExistingSystem1,#JobInstallationDetails_ExistingSystem,#ExistingSystem").hide();
//        $('.addInfo').removeClass('col-md-6');
//        $('.addInfo').addClass('col-md-12');
//        $("#MultipleSGUAddress").html("Is there more than one SWH/ASHP at this address:");
//        $("#CertificateCreated").html("Creating certificates for previously failed SWH:");
//        $("#divVolumetricCapacity").show();
//        if (modelSTCVolumetricCapacity == 'Yes') {
//            $("#divStatutoryDeclarations").show();
//        }
//        $("#divSecondhandWaterHeater").show();
//        $("#jobSSCID").hide();
//    }
//}

function isValidPhone(event, obj) {
    if (!event.ctrlKey) {
        if (event.which == 43 && $("#" + obj.id).val().length < 2) {
            if ($("#" + obj.id).val().indexOf('+') == -1) {
                return true;
            }
        }
        if (event.which > 31 && (event.which < 48 || event.which > 57)) {
            return false;
        }
        return true;
    }
    return true;
}

function GSTFileUpload() {
    var Guid = modelGuid;
    //var jobId = $("#BasicDetails_JobID").val();
    var jobId = Guid;
    var url = actionUploadInvoice;
    $('#uploadGSTFile').fileupload({

        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;

            var UploadFailedFilesType = [];
            UploadFailedFilesType.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    if ($("#BasicDetails_GSTDocument").val() != '' && $("#BasicDetails_GSTDocument").val() != undefined && $("#BasicDetails_GSTDocument").val() != null)
                        $("#BasicDetails_GSTDocument").attr('OldFileName', $("#BasicDetails_GSTDocument").val());
                    else
                        $("#BasicDetails_GSTDocument").attr('OldFileName', '');


                    var Sign = document.getElementsByName("BasicDetails.GSTDocument");
                    var OldFileName = $("#BasicDetails_GSTDocument").attr('OldFileName');
                    if (Sign.length > 0) {
                        var signName = Sign[0].defaultValue;
                        if (signName != null && signName != "" && (OldFileName == '' || OldFileName == null || OldFileName == undefined)) {
                            DeleteGSTFile(signName, jobId);
                        }
                        $("#tblDocuments").find('tr').each(function () {
                            $(this).remove();
                        });
                    }
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var content = "<tr style='margin-top:30px' id= " + data.result[i].FileName.replace("%", "$") + " >"
                    content += '<td class="col-sm-9" style="color:#494949;border-bottom:0px !important">' + data.result[i].FileName.replace("%", "$") + ' </td>';
                    if (mimeType == "image") {
                        content += '<td  class="col-sm-1 action" style="color:blue;border-bottom:0px !important; padding:0px;"><a id="' + data.result[i].FileName.replace("%", "$") + '" style="cursor: pointer" class="' + data.result[i].FileName.replace("%", "$") + ' sprite-img view" title="Preview" onclick="OpenGSTDocument(this)"></a></td>';
                    }
                    else {
                        content += '<td  class="col-sm-1 action" style="color:blue;border-bottom:0px !important; padding:0px;"><a style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadGSTDocument(this)" class="' + data.result[i].FileName.replace("%", "$") + ' sprite-img view"></a></td>';
                    }
                    content += '<td class="col-sm-1 action" style="color:blue;border-bottom:0px !important; padding:0px;"><a style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '" title="Delete" onclick="DeleteGSTDocument(this)" class="sprite-img delete GSTDocumentDelete"></a></td>';
                    content += "</tr>"

                    $('#tblDocuments').append(content);
                    $('#BasicDetails_GSTDocument').val(data.result[i].FileName.replace("%", "$"));
                }
                else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                    UploadFailedFilesType.push(data.result[i].FileName);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));

                }
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesType.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesType.length + " " + "Uploaded file type is not supported.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFilesType.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();
            }
            ReloadSTCJobScreen(jobId);
        },

        progressall: function (e, data) {

        },

        singleFileUploads: false,
        send: function (e, data) {
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 500) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
                        $("#errorMsgRegion").show();
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        $("#errorMsgRegion").show();
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
                    $("#errorMsgRegion").show();

                    return false;
                }
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        //formData: { userId: USERID },
        formData: { guid: Guid, jobId: jobId, isGSTDocument: 1 },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');

}
function DeleteGSTFile(fileNames, guid, OldInvoiceFile) {
    $.ajax(
        {
            url: actionDeleteGSTFile,
            data: { fileName: fileNames, FolderName: guid, OldInvoiceFile: OldInvoiceFile },
            contentType: 'application/json',
            method: 'get',
            async: false,
            success: function (data) {
                return false;
            },
        });
}
function DeleteGSTDocument(obj, isFromDeleteIcon) {
    //var fileName = $(obj).attr('filename');
    var fileName = $(obj).attr('id');
    var FolderName = modelGuid;
    var OldFileName = $(obj).attr('oldFileName');

    var result = false;
    if (!isFromDeleteIcon) {
        result = true;
    }
    else {
        result = confirm('Are you sure you want to delete this file ?');
    }

    if (result) {
        $.ajax(
            {
                url: actionDeleteGSTFile,
                data: { fileName: fileName, FolderName: FolderName, OldInvoiceFile: OldFileName },
                contentType: 'application/json',
                method: 'get',
                success: function () {
                    $("#tblDocuments").find('tr').each(function () {
                        $(this).remove();
                    });

                    $("#BasicDetails_GSTDocument").val('');
                    ReloadSTCJobScreen(modelGuid);
                    return false;
                }
            });
    }
}
//Proof of Open upload
function OpenGSTDocument(obj) {
    var JobId = modelGuid;
    var UploadedDocumentPath = modelUploadedDocumentPath;
    //var fileName = $(obj).attr('filename');
    var fileName = $(obj).attr('id');
    var imagePath = UploadedDocumentPath + "/" + "JobDocuments" + "/" + JobId + "/" + "GST" + "/" + fileName;
    $("#loading-image").css("display", "");
    $('#imgViewImage').attr("src", imagePath).load(function () {
        logoWidthGST = this.width;
        logoHeightGST = this.height;
        $('#popupProof').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeightGST) {
            $("#imgViewImage").closest('div').height($(window).height() - 205);
            $("#imgViewImage").closest('div').css('overflow-y', 'scroll');
            $("#imgViewImage").height(logoHeightGST);
        }
        else {
            $("#imgViewImage").height(logoHeightGST);
            $("#imgViewImage").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthGST || logoWidthGST >= screen.width - 250) {
            $('#popupProof').find(".modal-dialog").width(screen.width - 250);
            $("#imgViewImage").width(logoWidthGST);
        }
        else {
            $("#imgViewImage").width(logoWidthGST);
            $("#popupProof").find(".modal-dialog").width(logoWidthGST);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgViewImage").unbind("error");
    $('#imgViewImage').attr("src", imagePath).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}
function DownloadGSTDocument(obj) {
    var JobId = modelGuid;
    var fileName = $(obj).attr('id');
    window.location.href = actionDownloadGSTDocument + '?FileName=' + escape(fileName) + '&FolderName=' + JobId;
}

$('#aGetOwnerSignature').click(function (e) {
    e.preventDefault();
    $('#mdlGetSignature').modal('show');
});

function SaveCustomField() {
    var customFieldsList = [];
    $.each($("#customFieldForm").find('input[type=text]'), function () {
        customFieldsList.push({ VisitCheckListItemId: $(this).attr('visitchecklistitemid'), FieldValue: $(this).val() });
    });
    if (customFieldsList.length > 0) {
        $.ajax({
            type: 'POST',
            url: actionSaveCustomField,
            dataType: 'json',
            data: { customFields: JSON.stringify(customFieldsList) },
            success: function (result) {
                if (result.status) {
                    $.get(actionReloadCustomFields + '?jobId=' + modelJobId, function (data) {
                        $('#reloadCustomJobField').empty();
                        $('#reloadCustomJobField').append(data);
                        showSuccessMessage("Custom fields have been saved.");
                        $('#popupCustomField').modal('toggle');
                        DistinctCustomField();
                    });
                }
                else {
                    showErrorMessage(result.error);
                }
            },
            error: function (ex) {
                showErrorMessage('Custom fields have not been saved.' + ex);
            }
        });
    }
}

function ClearCustomField() {
    $("#customFieldForm").find('input[type=text]').each(function () {
        $(this).val('');
    });
}

function ReloadCustomField() {
    $.get(actionGetJobCustomFields + '?jobId=' + modelJobId, function (data) {
        $('#customFieldForm').empty();
        $('#customFieldForm').append(data);
        $('#popupCustomField').modal({ backdrop: 'static', keyboard: false });
    });
}

function DistinctCustomField() {

    if ($("#reloadCustomJobField").find('p').length > 0)
        $(".jobCustomField").show();
    else
        $(".jobCustomField").hide();

    var distinctFieldId = [];
    var duplicateId = [];

    $.each($("#reloadCustomJobField").find('p'), function () {
        if ($.inArray($(this).attr('fieldId'), distinctFieldId) !== -1) {
            duplicateId.push({ visitchecklistitemid: $(this).attr('visitchecklistitemid'), fieldId: $(this).attr('fieldId'), fieldValue: $(this).find('span').text() });
        } else {
            distinctFieldId.push($(this).attr('fieldId'));
        }
    });

    if (duplicateId.length > 0) {
        for (var i = 0; i < duplicateId.length; i++) {
            $.each($("#reloadCustomJobField").find('p'), function () {
                if ($(this).attr('visitchecklistitemid') == duplicateId[i].visitchecklistitemid) {
                    var fieldvalue = duplicateId[i].fieldValue;
                    var fieldId = duplicateId[i].fieldId;
                    $.each($("#reloadCustomJobField").find('p'), function () {
                        if ($(this).attr('fieldId') == fieldId) {
                            $(this).find('span').text($(this).find('span').text() + " " + fieldvalue);
                        }
                    });
                    $(this).hide();
                }
            });
        }
    }
}

function SendEmailSignature() {
    $('#errorMsgRegionSignaturePopup').hide();
    $("#successMsgRegionSignaturePopup").hide();

    var type = $('[name=optType]:checked').val();
    var num;
    var value = $('#drpSendMail').val();
    var email;
    var message;
    var JobType = $("#BasicDetails_JobType").val();
    var IsOwner;
    var FirstName = '';
    var LastName = '';

    if (type == "S") {
        if (value == "2") {
            if (JobType == 1) {
                num = $('#InstallerView_Mobile').val().trim();
                FirstName = $("#InstallerView_FirstName").val();
                LastName = $("#InstallerView_LastName").val();
            }
            else {
                num = $('#JobInstallerDetails_Mobile').val().trim();
                FirstName = $("#JobInstallerDetails_FirstName").val();
                LastName = $("#JobInstallerDetails_LastName").val();
            }
            IsOwner = 'No';
            if (JobType == 1 && $('#InstallerView_Mobile').val().trim() == "") {
                $("#errorMsgRegionSignaturePopup").html("Installer Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
            if (JobType == 2 && $('#JobInstallerDetails_Mobile').val().trim() == "") {
                $("#errorMsgRegionSignaturePopup").html("Installer Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
        else if (value == "1") {
            num = $('#mobileOwner').text().trim();
            IsOwner = 'Yes';
            FirstName = $("#JobOwnerDetails_FirstName").val();
            LastName = $("#JobOwnerDetails_LastName").val();
            if ($('#mobileOwner').text().trim() == "") {

                $("#errorMsgRegionSignaturePopup").html("Owner Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
        else if (value == "4") {
            num = $('#DesignerView_Mobile').val().trim();
            FirstName = $("#DesignerView_FirstName").val();
            LastName = $("#DesignerView_LastName").val();
            IsOwner = 'No';
            if ($('#DesignerView_Mobile').val().trim() == "") {
                $("#errorMsgRegionSignaturePopup").html("Designer Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
        else if (value == "3") {
            num = $('#JobElectricians_Mobile').val().trim();
            FirstName = $("#JobElectricians_FirstName").val();
            LastName = $("#JobElectricians_LastName").val();
            IsOwner = 'No';
            if ($('#JobElectricians_Mobile').val().trim() == "") {
                $("#errorMsgRegionSignaturePopup").html("Electrician Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
    }
    else {
        if (value == "2") {
            if (JobType == 1) {
                email = $('#InstallerView_Email').val();
                FirstName = $("#InstallerView_FirstName").val();
                LastName = $("#InstallerView_LastName").val();
            }
            else {
                email = $('#JobInstallerDetails_Email').val();
                FirstName = $("#JobInstallerDetails_FirstName").val();
                LastName = $("#JobInstallerDetails_LastName").val();
            }
            message = "Installer Email is required for it."
            IsOwner = 'No';
        }
        else if (value == "1") {
            email = $('#JobOwnerDetails_Email').val();
            message = "Owner Email is required for it."
            IsOwner = 'Yes';
            FirstName = $("#JobOwnerDetails_FirstName").val();
            LastName = $("#JobOwnerDetails_LastName").val();

        }
        else if (value == "4") {
            email = $('#DesignerView_Email').val();
            message = "Designer Email is required for it."
            IsOwner = 'No';
            FirstName = $("#DesignerView_FirstName").val();
            LastName = $("#DesignerView_LastName").val();

        }
        else if (value == "3") {
            email = $('#JobElectricians_Email').val();
            message = "Electrician Email is required for it."
            IsOwner = 'No';
            FirstName = $("#JobElectricians_FirstName").val();
            LastName = $("#JobElectricians_LastName").val();
        }
    }

    if ((type == "S" && num != "" && num != null && num != undefined) || (type == "M" && email != "" && email != null && email != undefined)) {
        var solarCompany = $("#BasicDetails_CompanyName").val();
        var reseller = $("#resellerName").text();
        var jobId = id;
        var installationAddress = $("#txtAddress").val();
        var maillink;
        if (value == "1") {
            maillink = loginLink + "Job/_OwnerSignature/" + jobId;
        }
        else if (value == "2") {
            //maillink = loginLink + "Job/_InstallerVerification/" + jobId;
            maillink = loginLink + "Job/_InstallerSignature?Id=" + jobId;
        }
        else if (value == "3") {
            maillink = loginLink + "Job/_ElectricianSignature/" + jobId;
        }
        else if (value == "4") {
            maillink = loginLink + "Job/_DesignerSignature/" + jobId;
        }
        var today = new Date();
        var LinkSentedDateTime = (today.getMonth() + 1) + '%' + '2F' + today.getDate() + '%' + '2F' + today.getFullYear() + '%' + '20' + today.getHours() + '%' + '3A' + today.getMinutes() + '%' + '3A' + today.getSeconds();
        if (value == "2") {
            var mailUrl = '';
            if (type == "S") {
                mailUrl = maillink + '&Type=' + JobType + '&SMSOrMail=' + type;
            }
            else {
                mailUrl = '<a href="' + maillink + '&Type=' + JobType + '&SMSOrMail=' + type + '&LinkSentedDateTime=' + LinkSentedDateTime + '">' + maillink + '&Type=' + JobType + '&SMSOrMail=' + type + '&LinkSentedDateTime=' + LinkSentedDateTime + '</a>';
            }
            var mailTo = email;
            $.ajax(
                {
                    url: actionMailForCollectSignature,
                    data: { MailUrl: mailUrl, MailTo: mailTo, JobType: JobType, IsOwner: IsOwner, FirstName: FirstName, LastName: LastName, Type: type, url: mailUrl, mobile: num, IsClassicJob: false, jobId: modelJobId, Reseller: reseller, solarCompany: solarCompany, installationAddress: installationAddress },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        if (data == true) {
                            $(".alert").hide();
                            if ($("#successMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has been sent successfully." : "Mail" + " has been sent successfully.")) {
                                $("#successMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has been sent successfully." : "Mail" + " has been sent successfully.");
                                $("#successMsgRegionSignaturePopup").show();
                                return false;
                            }
                            else {
                                $(".alert").hide();
                                $("#errorMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has not been sent." : "Mail" + " has not been sent.");
                                $("#errorMsgRegionSignaturePopup").show();
                            }

                        }
                        else {

                            if ($("#successMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has been sent successfully." : "Mail" + " has been sent successfully.")) {

                            }
                            else {
                                $(".alert").hide();
                                $("#errorMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has not been sent." : "Mail" + " has not been sent.");
                                $("#errorMsgRegionSignaturePopup").show();
                            }

                        }
                    }
                });
        }
        else {
            var mailUrl = '';
            if (type == "S") {
                mailUrl = maillink + '?Type=' + JobType + '&SMSOrMail=' + type;
            }
            else {
                mailUrl = '<a href="' + maillink + '?Type=' + JobType + '&SMSOrMail=' + type + '&LinkSentedDateTime=' + LinkSentedDateTime + '">' + maillink + '&Type=' + JobType + '&SMSOrMail=' + type + '&LinkSentedDateTime=' + LinkSentedDateTime + '</a>';
            }
            var mailTo = email;
            $.ajax(
                {
                    url: actionMailForCollectSignature,
                    data: { MailUrl: mailUrl, MailTo: mailTo, JobType: JobType, IsOwner: IsOwner, FirstName: FirstName, LastName: LastName, Type: type, url: mailUrl, mobile: num, IsClassicJob: false, jobId: modelJobId, Reseller: reseller, solarCompany: solarCompany, installationAddress: installationAddress },
                    contentType: 'application/json',
                    method: 'get',
                    success: function (data) {
                        if (data == true) {
                            $(".alert").hide();
                            $("#successMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has been sent successfully." : "Mail" + " has been sent successfully.");
                            $("#successMsgRegionSignaturePopup").show();
                            return false;
                        }
                        else {
                            $(".alert").hide();
                            $("#errorMsgRegionSignaturePopup").html(type == "S" ? "SMS" + " has not been sent." : "Mail" + " has not been sent.");
                            $("#errorMsgRegionSignaturePopup").show();
                        }
                    }
                });
        }
    }
    else {
        $("#errorMsgRegionSignaturePopup").html(message);
        $("#errorMsgRegionSignaturePopup").show();
    }
}

function SerialNumbersValidation(serialnumberFieldId, serialnumberSpanId) {
    var isNumbersOverflow = false;
    //var isDuplicateSerialNumber = false;
    var isHaveInvelidChar = false;
    if (serialnumberFieldId.val() != undefined) {
        if (serialnumberFieldId.val().trim() != "") {
            var text = serialnumberFieldId.val().trim();
            var lines = text.split("\n");
            lines = lines.filter(function (n) { return n.length > 0 });
            var count = lines.length;
            for (var i = 0; i < lines.length; i++) {
                if (lines[i].length > 100) {
                    isNumbersOverflow = true;
                    break;
                }
            }
            isDuplicateSerialNumber = hasDuplicates(lines);
        }

        if (isNumbersOverflow) {
            serialnumberSpanId.show().text('Serial numbers\'s maximum length  is of 100 characters.');
        }
        //else if (isDuplicateSerialNumber) {
        //    serialnumberSpanId.show().text('Serial numbers should not be same.');
        //}
        else if (IsHaveInvelidChar(serialnumberFieldId.val().trim())) {
            isHaveInvelidChar = true;
            serialnumberSpanId.show().text('There are some illegal characters in the serial number field.Job cannot be traded until these are amended with the correct serials.');
        }
        else {
            serialnumberSpanId.hide();
        }

        if (isNumbersOverflow || isDuplicateSerialNumber || isHaveInvelidChar) {
            return false;
        }
        else {
            return true;
        }
    }
}
function hasDuplicates(array) {
    var valuesSoFar = [];
    for (var i = 0; i < array.length; ++i) {
        var value = array[i];
        if (valuesSoFar.indexOf(value) !== -1) {
            return true;
        }
        valuesSoFar.push(value);
    }
    return false;
}

function showhideFailedCode(modelSTCCertificateCreated) {
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

function showhideAdditionalCapacityNotes() {
    //if ($("#JobInstallationDetails_Location").val() != "") {
        if ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") {
            $("#additionalCapacityNotes").show();
        }
        else {
            $("#JobSTCDetails_AdditionalCapacityNotes").text('');
            $("#additionalCapacityNotes").hide();
        }
    //}
    //else {
    //    $("#JobSTCDetails_AdditionalCapacityNotes").text('');
    //    $("#additionalCapacityNotes").hide();
    //}
}

function showhideAdditionalLocationInformation() {
    //if ($("#JobSTCDetails_Location").val() != "") {
        if ($("#JobSTCDetails_MultipleSGUAddress").val() != "No" && $("#JobSTCDetails_MultipleSGUAddress").val() != "") {
            $("#STCAdditionalLocation").show();
        }
        else {
            $("#JobSTCDetails_AdditionalLocationInformation").text('');
            $("#STCAdditionalLocation").hide();
        }
    //}
    //else {
    //    $("#JobSTCDetails_AdditionalLocationInformation").text('');
    //    $("#STCAdditionalLocation").hide();
    //}
}

$('#btnSendMail').click(function (e) {
    SendEmailSignature();
    SearchHistory();
});

$("#mdlGetSignature").on('show.bs.modal', function () {
    $('#errorMsgRegionSignaturePopup,#successMsgRegionSignaturePopup').css('display', 'none');
});
$("#mdlGetSignature").on('hide.bs.modal', function () {
    $('#mdlGetSignature').removeData('type');
});


var oldValSerialNumber = "";
var ready = function () {
    // set-number
    $(".text-editor").setNumber({
        activeLine: 1
    });
    $(".text-editor-inverter").setNumber({
        activeLine: 1
    });
    if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
        $(".isSPVRequired").show();
    else
        $(".isSPVRequired").hide();
    $(".text-editor").autosize({
        callback: function (textarea) {
            $(textarea).scroll();
            if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                VerifyUnVerifySerialNumber();
        }
    });
    $(".text-editor-inverter").autosize({
        callback: function (textarea) {
            $(textarea).scroll();
        }
    });
    $(".text-editor").focus();
    $(".text-editor-inverter").focus();
    if ($("#JobSystemDetails_SerialNumbers").prop("disabled"))
        $("div.line-no").removeClass("active");
    $("#JobSystemDetails_SerialNumbers").on("change keyup paste", function () {
        var currentVal = $(this).val();
        if (currentVal == oldValSerialNumber) {
            return; //check to prevent multiple simultaneous triggers
        }
        oldValSerialNumber = currentVal;
        //action to be performed on textarea changed
        if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
            VerifyUnVerifySerialNumber();
    });
};
function GetserialnumberWithSpvStatus() {
    $.ajax(
        {
            url: urlGetserialnumberWithSpvStatus,
            data: {
                jobId: Model_JobID
            },
            dataType: "json",
            success: function (data) {
                if (data.status) {
                    serialNumber = data.serialNumber;
                }
            }
        });
}

function RemoveSPVRelatedClass() {
    $("div.line-no").removeClass("verified")
    $("div.line-no").removeClass("unverified")
    $("div.line-no").removeClass("installationVerified")
    $("div.line-no").removeClass("notverified")
}

function AttachSPVLabelWithSerialNumber() {
    debugger;
    var splitSN = $("#JobSystemDetails_SerialNumbers").val() == undefined ? "" : $("#JobSystemDetails_SerialNumbers").val().split(/\n/g);
    for (var i = 0; i < splitSN.length; i++) {
        var matchSN = serialNumber.find(serialNumber => serialNumber.SerialNumber == splitSN[i]);
        if (matchSN != undefined) {
            if (matchSN.IsSPVInstallationVerified)
                $(".panel-list div.line-no.l-" + (i + 1)).addClass("installationVerified");
            else if (matchSN.IsVerified)
                $(".panel-list div.line-no.l-" + (i + 1)).addClass("verified");
            else if (matchSN.IsVerified == null)
                $(".panel-list div.line-no.l-" + (i + 1)).addClass("unverified");
            else
                $(".panel-list div.line-no.l-" + (i + 1)).addClass("notverified");
        }
        else
            $(".panel-list div.line-no.l-" + (i + 1)).addClass("unverified");
    }
}

function VerifyUnVerifySerialNumber() {
    GetserialnumberWithSpvStatus();
    if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True") {
        ShowSPV();
    }
    else {
        HideSPV();
    }
}
function SPVProductverification(reProductVerification = false) {
    if ($('#JobSystemDetails_SerialNumbers').val().trim() == '') {
        alert("Please enter serial number for do Product Verification.");
        return false;
    }
    else {
        $('#JobSystemDetails_SerialNumbers').removeAttr('disabled');
        var serialnumberArray = $('#JobSystemDetails_SerialNumbers').val($('#JobSystemDetails_SerialNumbers').val().trim()).serializeToJson();;
        $('#JobSystemDetails_SerialNumbers').attr('disabled', 'disabled');
        $.ajax({
            url: urlSPVProductVerification,
            type: "GET",
            data: {
                jobId: Model_JobID, serialNumber: JSON.stringify(serialnumberArray), reProductVerification: reProductVerification//[0].data)
            },
            dataType: "json",
            success: function (data) {
                if (data.status) {
                    $(".alert").hide();
                    $("#successSPVProductVerification").html(closeButton + "Verify serialnumber successfully");
                    $("#successSPVProductVerification").show();
                    serialNumber = data.serialNumber;
                    VerifyUnVerifySerialNumber();
                } else {
                    $(".alert").hide();
                    if (data.IsInstallationVerification) {
                        $("#errorSPVProductVerification").html(closeButton + "There are serial numbers listed which are already SPV Installation Verified.So, SPV verify cannot be done.");
                    }
                    else if (data.IsPhotoUnAvailable) {
                        var failSerialNumber = '';
                        if (data.lstNotExistPhoto != undefined && data.lstNotExistPhoto.length > 0) {
                            failSerialNumber = data.lstNotExistPhoto.join();
                        }
                        $("#errorSPVProductVerification").html(closeButton + "There are serial numbers [" + failSerialNumber + "] listed that do not have matching photos associated. SPV verify cannot be completed.");
                    }

                    else {
                        $("#errorSPVProductVerification").html(closeButton + "Something wrong happen in verifying serial number.");
                    }

                    $("#errorSPVProductVerification").show();
                }
               // SearchHistory();
            }
        });
    }

}

function SPVProductionVerificationErrorLog() {

    $("#spvproduct-failure-reason-div").load(urlGetSPVProductionVerificationErrorLog + '?jobId=' + $("#BasicDetails_JobID").val(), function () {
        $("#spvproduct-verification-failure-reason").modal({ backdrop: 'static', keyboard: false });
        $("#errorMsgRegionSPVProductFailure").hide();
    })
}
function GetARoute() {
    $("#dDistance").html('');
    $("#MapErrorMsgRegion").hide();
    source = document.getElementById("txtSrc").value;
    destination = document.getElementById("txtDest").value;

    if (source != "" && destination != "") {
        var India = new google.maps.LatLng(51.508742, -0.120850);
        var mapOptions = {
            zoom: 4,
            center: India
        };
        map = new google.maps.Map(document.getElementById('dMap'), mapOptions);

        directionsDisplay.setMap(map);
        //*********DIRECTIONS AND ROUTE**********************//
        var request = {
            origin: source,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
            else {
                createAMarker(destination, 25.2744, 133.7751);
                $("#MapErrorMsgRegion").html(closeButton + " Both Source and Destination  are not in same country.");
                $("#MapErrorMsgRegion").show();
            }
        });
    }
    else {
        $("#MapErrorMsgRegion").html(closeButton + " Both Source and Destination  address are  required");
        $("#MapErrorMsgRegion").show();
        //$("#errorMsgRegionMap").fadeOut(5000);
    }
}
function GetALocation() {
    latitude = '';
    longitude = '';
    latitude1 = '';
    longitude1 = '';

    $("#dDistance").html('');
    $("#MapErrorMsgRegion").hide();
    //$("#dvMap").html('');
    locations = [];
    source = document.getElementById("txtSrc").value;
    destination = document.getElementById("txtDest").value;

    geocoder = new google.maps.Geocoder();

    if (source != "" || destination != "") {

        if (source != "") {
            geocoder.geocode({ 'address': source }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude = results[0].geometry.location.lat();
                    longitude = results[0].geometry.location.lng();
                    sourcedetail = [source, latitude, longitude];
                    locations.push(sourcedetail);
                    GetALocationOnMap(latitude, longitude);
                }
                else {
                    $("#MapErrorMsgRegion").html(closeButton + "Invalid source Address.");
                    $("#MapErrorMsgRegion").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createAMarker('Victoria , Australia', -37.4713, 144.7852);
                }
            });
        }
        if (destination != "") {
            geocoder.geocode({ 'address': destination }, function (results, status) {

                if (status == google.maps.GeocoderStatus.OK) {
                    latitude1 = results[0].geometry.location.lat();
                    longitude1 = results[0].geometry.location.lng();
                    destinationdetail = [destination, latitude1, longitude1];
                    locations.push(destinationdetail);
                    GetALocationOnMap(latitude1, longitude1);
                }
                else {
                    $("#MapErrorMsgRegion").html(closeButton + "Invalid Installation Address.");
                    $("#MapErrorMsgRegion").show();
                    //$("#errorMsgRegionMap").fadeOut(5000);
                    createAMarker('Victoria , Australia', -37.4713, 144.7852);
                }
            });
        }
    }
    else {
        $("#MapErrorMsgRegion").html(closeButton + "Source or Destination address  are  required");
        $("#MapErrorMsgRegion").show();
        //$("#errorMsgRegionMap").fadeOut(5000);
    }
}

function GetALocationOnMap(latitude, longitude) {
    var bounds = new google.maps.LatLngBounds();
    var infowindow = new google.maps.InfoWindow();
    var lat = latitude;
    var lng = longitude;
    var map = new google.maps.Map(document.getElementById('dMap'), {
        zoom: 10,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });
    var marker, i;
    for (i = 0; i < locations.length; i++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[i][1], locations[i][2]),
            draggable: false,
            map: map
        });

        //extend the bounds to include each marker's position
        bounds.extend(marker.position);

        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infowindow.setContent(locations[i][0]);
                infowindow.open(map, marker);
            }
        })(marker, i));
    }

    //now fit the map to the newly inclusive bounds
    map.fitBounds(bounds);
}

function createAMarker(add, lat, lng) {
    //var delay = 1;
    var infowindow = new google.maps.InfoWindow();
    var latlng = new google.maps.LatLng(lat, lng);

    var map = new google.maps.Map(document.getElementById('dMap'), {
        zoom: 12,
        center: new google.maps.LatLng(lat, lng),
        mapTypeId: google.maps.MapTypeId.DRIVING
    });
    var bounds = new google.maps.LatLngBounds();

    var contentString = add;
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, lng),
        center: latlng,
        map: map,
        draggable: false
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(contentString);
        infowindow.open(map, marker);
    });
}
function GetCompanyABN(ownerCompanyABN) {
    $.ajax({
        type: "GET",
        url: urlGetCompanyABN,
        data: { id: ownerCompanyABN },
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data == 0) {
                $('#JobOwnerDetails_CompanyName').empty();
                $("#JobOwnerDetails_CompanyName").append($("<option></option>").val("").html("Select"));
            }
            else {
                if ($('#JobOwnerDetails_CompanyName option').length > 1) {
                    $('#JobOwnerDetails_CompanyName').empty();
                    $("#JobOwnerDetails_CompanyName").append($("<option></option>").val("").html("Select"));
                    $.each(data, function (key, value) {
                        $("#JobOwnerDetails_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                else {
                    $.each(data, function (key, value) {
                        $("#JobOwnerDetails_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                initializeOwnerDetails(data);
                var str = modelOwnerCompanyName;
                if (str.indexOf('&#39') != -1) {
                    var str = str.replace(/&#39;/g, "'");
                }
                else if (str.indexOf('&amp') != -1) {
                    var str = str.replace(/&amp;/g, '&');
                }
                else if (str.indexOf('&quot') != -1) {
                    var str = str.replace(/&quot;/g, '"');
                }
                else if (str.indexOf('&lt') != -1) {
                    var str = str.replace(/&lt;/g, '<');
                }
                else if (str.indexOf('&gt') != -1) {
                    var str = str.replace(/&gt;/g, '>');
                }
                $("#JobOwnerDetails_CompanyName").val(str);
                //DisplayOwnerAdd();
                return data;
            }
        }
    });
}

function deemingPeriodCallBack(deemingPeriod) {
    if (modelSTCDeemingPeriod == 0 || modelSTCDeemingPeriod == null || modelSTCDeemingPeriod == '') {
        $("#JobSTCDetails_DeemingPeriod").val($("#JobSTCDetails_DeemingPeriod option:last").val());
    }
}

function BindDeemingPeriodDropdown(year, deemingPeriod) {
    $.ajax({
        type: 'get',
        url: urlGetDeemingPeriod,
        dataType: 'json',
        data: { year: year },
        async: false,
        success: function (data) {
            $("#JobSTCDetails_DeemingPeriod").html('');
            $.each(data, function (i, deemingPeriod) {
                $("#JobSTCDetails_DeemingPeriod").append('<option value="' + deemingPeriod.Value + '">' + deemingPeriod.Text + '</option>');
            });

            if (deemingPeriod != 0) {
                document.getElementById("JobSTCDetails_DeemingPeriod").value = deemingPeriod;
            }
            else {
                $("#JobSTCDetails_DeemingPeriod").val($("#JobSTCDetails_DeemingPeriod option:last").val());
            }
        },
        error: function (ex) {
            alert('Failed to retrieve DeemingPeriod.' + ex);
        }
    });
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
function ShowSPV() {
    AttachSPVLabelWithSerialNumber();
    $(".isSPVRequired").show();
    $("#SPVLabel").show();
    IsSPVRequired = "True";
    $("#btnSPVProductVerification").show();
    $("#btnSPVProductVerificationErrorLog").show();
}
function HideSPV() {
    RemoveSPVRelatedClass();
    $(".isSPVRequired").hide();
    $("#SPVLabel").hide();
    IsSPVRequired = "False";
    $("#btnSPVProductVerification").hide();
    $("#btnSPVProductVerificationErrorLog").hide();
}
function CheckInstallationAddressIsValidOrNot() {
    if (jobInsd_IsInstallationAddressValid == '') {
        var address;
        var UnitTypeId = $("#JobInstallationDetails_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobInstallationDetails_UnitNumber").val();
        var StreetNumber = $("#JobInstallationDetails_StreetNumber").val();
        var StreetName = $("#JobInstallationDetails_StreetName").val();
        var StreetTypeId = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
        var PostalAddressID = $("#JobInstallationDetails_PostalAddressID").find("option:selected").text();
        var PostalDeliveryNumber = $("#JobInstallationDetails_PostalDeliveryNumber").val();
        var Town = $("#JobInstallationDetails_Town").val();
        var State = $("#JobInstallationDetails_State").val();
        var PostCode = $("#JobInstallationDetails_PostCode").val();
        if ($("#JobInstallationDetails_AddressID").val() == 1) {
            if (UnitNumber != "" && UnitTypeId != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            address = address.replace("Select", "");
            if ((typeof (google) == 'undefined')) {
                var scriptMap = document.createElement("script");
                scriptMap.type = "text/javascript";
                scriptMap.async = true;
                var src = JobMapKeyUrl;
                src = src.toString().replace(/&amp;/g, '&');
                scriptMap.src = src;
                document.body.appendChild(scriptMap);
            }
            setTimeout(function () {
                if (ProjectSession_UserTypeId == 1) {
                    geocodeAddress(address, false, true);
                }
            }, 3000);
            $("#errorMsgRegionMap").hide();
        }
    }
    else if (jobInsd_IsInstallationAddressValid == 'False' && ProjectSession_UserTypeId == 1) {
        $("#errorMsgValidInstallationAddress").html(closeButton + "Installation address does not match google street address.");
        $("#errorMsgValidInstallationAddress").show();
        $('#installationAdd').css('color', 'Red');
    }
    else {
        $("#errorMsgValidInstallationAddress").hide();
        $('#installationAdd').css('color', '');
    }
}
function CheckOwnerAddressIsValidOrNot() {
    if (jobOwd_IsOwnerAddressValid == '') {
        var address;
        var UnitTypeId = $("#JobOwnerDetails_UnitTypeID").find("option:selected").text();
        var UnitNumber = $("#JobOwnerDetails_UnitNumber").val();
        var StreetNumber = $("#JobOwnerDetails_StreetNumber").val();
        var StreetName = $("#JobOwnerDetails_StreetName").val();
        var StreetTypeId = $("#JobOwnerDetails_StreetTypeID").find("option:selected").text();
        var Town = $("#JobOwnerDetails_Town").val();
        var State = $("#JobOwnerDetails_State").val();
        var PostCode = $("#JobOwnerDetails_PostCode").val();
        if ($("#JobOwnerDetails_AddressID").val() == 1) {
            if (UnitNumber != "" && UnitTypeId != "") {
                address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            } else {
                address = StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                address = address.replace("Select", "");
            }
            address = address.replace("Select", "");
            if ((typeof (google) == 'undefined')) {
                var scriptMap = document.createElement("script");
                scriptMap.type = "text/javascript";
                scriptMap.async = true;
                var src = JobMapKeyUrl;
                src = src.toString().replace(/&amp;/g, '&');
                scriptMap.src = src;
                document.body.appendChild(scriptMap);
            }
            setTimeout(function () {
                if (ProjectSession_UserTypeId == 1) {
                    geocodeAddress(address, true, false);
                }
            }, 2000);
            $("#errorMsgRegionMap").hide();
        }
    }
    else if (jobOwd_IsOwnerAddressValid == 'False' && ProjectSession_UserTypeId == 1) {
        $("#errorMsgValidOwnerAddress").html(closeButton + "Owner address does not match google street address.");
        $("#errorMsgValidOwnerAddress").show();
        $('#ownerAdd').css('color', 'Red');
    }
    else {
        $("#errorMsgValidOwnerAddress").hide();
        $('#ownerAdd').css('color', '');
    }
}
function installationAddressChangeCheck(JobInstallationDetails) {
    var savedAddress = modelInstallationStreetNumber + ', ' + modelInstallationStreetName + ' ' + modelInstallationStreetTypeID + ', ' + modelInstallationTown + ' ' + modelInstallationState + ' ' + modelInstallationPostCode;
    var StreetType = $("#JobInstallationDetails_StreetTypeID").find("option:selected").text();
    var changedAddress = JobInstallationDetails.StreetNumber + ', ' + JobInstallationDetails.StreetName + ' ' + JobInstallationDetails.StreetTypeID + ', ' + JobInstallationDetails.Town + ' ' + JobInstallationDetails.State + ' ' + JobInstallationDetails.PostCode;
    if (savedAddress.toLowerCase() != changedAddress.toLowerCase()) {
        changedAddress = JobInstallationDetails.StreetNumber.toString().trim() + ', ' + JobInstallationDetails.StreetName.toString().trim() + ' ' + StreetType.toString().trim() + ', ' + JobInstallationDetails.Town.toString().trim() + ' ' + JobInstallationDetails.State.toString().trim() + ' ' + JobInstallationDetails.PostCode.toString().trim();
        if ((typeof (google) == 'undefined')) {
            var scriptMap = document.createElement("script");
            scriptMap.type = "text/javascript";
            var src = JobMapKeyUrl;
            src = src.toString().replace(/&amp;/g, '&');
            scriptMap.src = src;
            document.body.appendChild(scriptMap);
        }
        setTimeout(function () {
            if (ProjectSession_UserTypeId == 1) {
                geocodeAddress(changedAddress, false, true);
            }
        }, 1000);
    }
}
function OwnerAddressCheck(JobOwnerDetails) {
    var savedAddress = modelOwnerStreetNumber + ', ' + modelOwnerStreetName + ' ' + modelOwnerStreetTypeID + ', ' + modelOwnerTown + ' ' + modelOwnerState + ' ' + modelOwnerPostCode;
    var StreetType = $("#JobOwnerDetails_StreetTypeID").find("option:selected").text();
    var changedAddress = JobOwnerDetails.StreetNumber + ', ' + JobOwnerDetails.StreetName + ' ' + JobOwnerDetails.StreetTypeID + ', ' + JobOwnerDetails.Town + ' ' + JobOwnerDetails.State + ' ' + JobOwnerDetails.PostCode;

    if (savedAddress.toLowerCase() != changedAddress.toLowerCase()) {
        changedAddress = JobOwnerDetails.StreetNumber.toString().trim() + ', ' + JobOwnerDetails.StreetName.toString().trim() + ' ' + StreetType.toString().trim() + ', ' + JobOwnerDetails.Town.toString().trim() + ' ' + JobOwnerDetails.State.toString().trim() + ' ' + JobOwnerDetails.PostCode.toString().trim();
        if ((typeof (google) == 'undefined')) {
            var scriptMap = document.createElement("script");
            scriptMap.type = "text/javascript";
            var src = JobMapKeyUrl;
            src = src.toString().replace(/&amp;/g, '&');
            scriptMap.src = src;
            document.body.appendChild(scriptMap);
        }
        setTimeout(function () {
            if (ProjectSession_UserTypeId == 1) {
                geocodeAddress(changedAddress, true, false);
            }
        }, 1000);
    }
}
function callbackCheckList() {
    if ($('#checkListItemForTrade').find('ul').find('li').length > 0)
        $('#checkListItemForTrade').show();
    else
        $('#checkListItemForTrade').hide();
}
$(document).ready(ready);
