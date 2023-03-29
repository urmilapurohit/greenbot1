var town, state, postcode;
var selectedVEECAreaId = 0;
var Prevname = "";
var lstVEECInstaller = [];
var lstVEECUpgradeManager = [];
$(document).ready(function () {
    
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
    var VEECOwnerDetailOwnerType = typeof mddelVEECOwnerType != "undefined" ? mddelVEECOwnerType : 0;
    var LightigDesignMethod1680 = typeof model1680LightigDesignMethod != "undefined" ? model1680LightigDesignMethod : 0;
    var ContractualArrangement1680 = typeof modelContractualArrangement1680 != "undefined" ? modelContractualArrangement1680 : 0;
    var LightLevelVerification1680 = typeof model1680LightLevelVerification != "undefined" ? model1680LightLevelVerification : 0;
    var QualificationOfLightLevelVerifier1680 = typeof model1680QualificationOfLightLevelVerifier != "undefined" ? model1680QualificationOfLightLevelVerifier : 0;
    var QualificationsOfLightingDesigner1680 = typeof model1680QualificationsOfLightingDesigner != "undefined" ? model1680QualificationsOfLightingDesigner : 0;

    dropDownData.push({ id: 'VEECDetail_LightingDesignMethodId', key: "", value: LightigDesignMethod1680, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_Get1680LightingDesignMethod', param: [], bText: 'Name', bValue: 'Id' },
        { id: 'VEECDetail_QualificationsOfLightingDesignerId', key: "", value: QualificationsOfLightingDesigner1680, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_Get1680QualificationsOfLightingDesigner', param: [], bText: 'Name', bValue: 'Id' },
        { id: 'VEECDetail_LightLevelVerificationId', key: "", value: LightLevelVerification1680, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_Get1680LightLevelVerification', param: [], bText: 'Name', bValue: 'Id' },
        { id: 'VEECDetail_QualificationOfLightLevelVerifierId', key: "", value: QualificationOfLightLevelVerifier1680, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_Get1680QualificationsOfLightingDesigner', param: [], bText: 'Name', bValue: 'Id' },
        { id: 'VEECDetail_ContractualArrangementId', key: "", value: ContractualArrangement1680, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_Get1680ContractualArrangement', param: [], bText: 'Name', bValue: 'Id' });

    $('#btnVEECOwnerDetails').click(function () {
        $('#popupVEECOwner').modal({ backdrop: 'static', keyboard: false });
        setTimeout(function () {
            $('#VEECOwnerDetail_CompanyName').focus();
            $("#VEECOwnerDetail_CompanyName").change();
        }, 1000);
        validateOrganisationVEEC('');
        
    });
    $("#VEECOwnerDetail_OwnerType").change(function (e) {
        validateOrganisationVEEC('');
    });
    
    InstallationBtnClickEvent();
    $('#btnVEECInstallationAddress').click(function () {
        $('#popupVEECInstallation').modal({ backdrop: 'static', keyboard: false });
        AddressAutoComplete('#VEECInstallationDetail_Town', '#VEECInstallationDetail_State', '#VEECInstallationDetail_PostCode');
        addressValidationRules("VEECInstallationDetail")
    })

    $("#VEECInstallationDetail_UnitTypeID").change(function () {
        addressValidationRules("VEECInstallationDetail")
    })
    $("#createVEEC_VEECInstallationDetail_UnitTypeID").change(function () {
        addressValidationRules("createVEEC_VEECInstallationDetail")
    })
    $('#btnVEECInstallerDetails').click(function () {
        $('#popupVEECInstaller').modal({ backdrop: 'static', keyboard: false });
    })

    $('#VEECInstaller_IsPostalAddress').change(function () {
        showHideAddress($('#VEECInstaller_IsPostalAddress').val())
    });
    $("#yesWarningVEEC").click(function () {
        $("#warning").modal('hide');
        SaveVEECJob();
    });

    $("#noWarningVEEC").click(function () {
        $("#warning").modal('hide');
    });
    $('#btnBackNew').click(function (e) {
        e.preventDefault();
        window.location.href = urlIndex;
    });
    $("#cancelCreateVEECPopup").click(function () {
        $('#popupboxCreateVEECPopup').modal('toggle');
        $('body').css('overflow', 'auto');
        $('#createVEECPopup').empty();
    });
    $("#txtVEECInstallationAdd").val($("#VEECInstallationAdd").text())
    $("#btnMapVEEC").click(function () {
        $("#lblSrcMap").html("Source");
        $("#txtSourceVEEC").attr("placeholder", "Source");
        LoadVEECInstallationSignatureLocation('');
    })
    DisplayVEECInstallationInfo();
    DisplayAddress('VEECInstallationDetail', 'VEECInstallation', false, false, true);
    DisplayAddress('VEECInstaller', 'VEECInstaller', false, true, false);
    DisplayVEECOwnerAddress('popupVEECOwner', 'VEECOwnerDetail', 'VEECOwner', true, false, false, 'VEECOwner');
    $('#btnFindVEECInstaller').click(function (e) {
        $("#errorMsgRegionVEECInstallerPopup").hide();
        $("#successMsgRegionVEECInstallerPopup").hide();
        $('.SearchGreenbotVEECInstaller').hide();
        $('.AddEditVEECInstaller').hide();
        $('#btnBackFindInstaller').hide();
        ClearPopupVEECInstaller(true);   // clear Add new Installer details
        ClearSearchGreenbotVEECInstaller();
        $('.modalTitleVEECInstaller').html('Find Installer');
        $('.FindVEECInstaller').show();
        $('.VEECInstallerOptions').show();
        $('#popupVEECInstaller').modal({ backdrop: 'static', keyboard: false });
        e.preventDefault();
    });
    $('#btnSearchGreenbotVEECInstaller').click(function (e) {
        $('.VEECInstallerOptions').hide();
        $('.modalTitleVEECInstaller').html('Search Greenbot Installer');
        $('.SearchGreenbotVEECInstaller').show();
        $('#btnAddVEECInstaller').attr('onclick', ' SaveVEECInstaller(false);');
        $('#btnBackFindInstaller').show();
    })
    $('#btnAddUpgradeManager').click(function (e) {
        $('#popupVEECUpgradeManager').find('input[type="text"]').val('')
        $('#popupVEECUpgradeManager').modal({ backdrop: 'static', keyboard: false });
        $("#btnAddUpdateVEECUpgradeManager").show();
        $('#btnAddUpdateVEECUpgradeManager').attr('onclick', 'SaveVEECUpgradeManager(false,true);');
        $('#btnClearPopupVEECUpgradeManager').attr('onclick', 'ClearPopupVEECUpgradeManager(true)');
    })
    $('#btnAddNewVEECInstaller').click(function (e) {
        $('.SearchGreenbotVEECInstaller').hide();
        $('.VEECInstallerOptions').hide();
        $("#errorMsgRegionVEECInstallerPopup").hide();
        $("#successMsgRegionVEECInstallerPopup").hide();
        $('.modalTitleVEECInstaller').html('Add Installer');
        $('.AddEditVEECInstaller').show();
        $('#btnAddUpdateVEECInstaller').attr('onclick', 'SaveVEECInstaller(false,true);');
        $('#btnClearPopupVEECInstaller').attr('onclick', 'ClearPopupVEECInstaller(true)');
        $('#btnBackFindInstaller').show();
        $("#VEECInstaller_ElectricalContractorsLicenseNumber").val('');
        $("#VEECInstaller_ElectricalComplienceNumber").val('');
        $("#VEECInstaller_CompanyName").val('');
        $("#VEECInstaller_FirstName").val('');
        $("#VEECInstaller_LastName").val('');
    })

    $('#btnBackFindInstaller').click(function (e) {
        $('.SearchGreenbotVEECInstaller').hide();
        $('.AddEditVEECInstaller').hide();
        $('#btnBackFindInstaller').hide();
        ClearPopupVEECInstaller(true);   // clear Add new Installer details
        ClearSearchGreenbotVEECInstaller();  // clear Search Greenbot Installer detail

        $('.modalTitleVEECInstaller').html('Find Installer');
        $('.VEECInstallerOptions').show();
    })
    $("#btnEditVEECInstaller").click(function (e) {
        if ($('#VEECInstallerDetail_VEECInstallerId').val() > 0) {
            FillVEECInstaller($('#VEECInstallerDetail_VEECInstallerId').val());
            //Hiding error/success msg region 
            $("#errorMsgRegionCVEECInstallerPopup").hide();
            $("#successMsgRegionVEECInstallerPopup").hide();
            //$('.FindVEECInstaller').hide();
            $('.VEECInstallerOptions').hide();

            $('.modalTitleVEECInstaller').html('Installer Detail');
            $('#btnAddUpdateVEECInstaller').attr('onclick', ' SaveVEECInstaller(true);');
            $('#btnClearPopupVEECInstaller').attr('onclick', 'ClearPopupVEECInstaller(false)');
            $('.AddEditVEECInstaller').show();
            $('#popupVEECInstaller').modal({ backdrop: 'static', keyboard: false });
            $("#popupVEECInstaller").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $("#popupVEECInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
        }
        else {
            alert("Please select VEEC Installer.");
            return false;
        }

        e.preventDefault();
    });
    $("#btnEditVEECUpgradeManager").click(function (e) {
       
        if ($('#VEECUpgradeManager_Id').val() > 0) {
            FillUpgradeManager($('#VEECUpgradeManager_Id').val());
            $("#errorMsgRegionCVEECUpgradeManagerPopup").hide();
            $("#successMsgRegionVEECUpgradeManagerPopup").hide();
            $('#btnAddUpdateVEECUpgradeManager').attr('onclick', ' SaveVEECUpgradeManager(true);');
            $('#btnClearPopupVEECUpgradeManager').attr('onclick', 'ClearPopupVEECUpgradeManager(false)');
            $('#popupVEECUpgradeManager').modal({ backdrop: 'static', keyboard: false });
            $("#popupVEECUpgradeManager").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $("#popupVEECUpgradeManager").find('.field-validation-error').attr('class', 'field-validation-valid');
            if ($("#txtVEECUpgradeManager_Id").attr("issystemuser") == "true") {
                $("#btnAddUpdateVEECUpgradeManager").hide();
                $("#btnClearPopupVEECUpgradeManager").hide();
            }else{
                $("#btnAddUpdateVEECUpgradeManager").show();
                $("#btnClearPopupVEECUpgradeManager").show();
            }
            
        }
        else {
            alert("Please select VEEC Installer.");
            return false;
        }
        e.preventDefault();
    });
    $("#VEECUpgradeManagerDetail_CompanyABN").blur(function () {
        var id = $('#VEECUpgradeManagerDetail_CompanyABN').val();
        GETCompanyName(id);
    });

    $("#VEECDetail_ContractualArrangement1680").change(function () {
        if ($(this).val() == 5) {
            $(".otherContractualArrangements").show()
        } else {
            $(".otherContractualArrangements").hide()
        }
    })

    $("#VEECDetail_LightingDesignMethodId").change(function () {
        if ($(this).val() == 1) {
            $("#lblQualificationsOfLightingDesigner").addClass("required");
            $("#lblDesignerQualificationDetails").addClass("required");
            $("#lblQualificationOfLightLevelVerifier").removeClass("required");
            $("#lblVerifierQualificationDetail").removeClass("required");
            $("#VEECDetail_LightLevelVerificationId option[value*='2']").prop('disabled', false);
        }
        else if($(this).val() == 2) {
            $("#lblQualificationsOfLightingDesigner").removeClass("required");
            $("#lblDesignerQualificationDetails").removeClass("required");
            $("#lblQualificationOfLightLevelVerifier").addClass("required");
            $("#lblVerifierQualificationDetail").addClass("required");
            $("#VEECDetail_LightLevelVerificationId option[value*='2']").prop('disabled', true);
        }
    })
    $("#VEECDetail_LightLevelVerificationId").change(function () {
        if ($(this).val() == 2 || $(this).val() == 3) {
                $("#lblQualificationOfLightLevelVerifier").addClass("required");
                $("#lblVerifierQualificationDetail").addClass("required");
                
        } else {
            $("#lblQualificationOfLightLevelVerifier").removeClass("required");
            $("#lblVerifierQualificationDetail").removeClass("required");
        }
        
    })

    FillDropDownVEECInstaller(GetVEECInstallerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), veecinstallerId, $('#txtVEECInstaller_VEECInstallerId'), $('#VEECInstallerDetail_VEECInstallerId'));
    VEECInstallerAutocomplete($('#txtVEECInstaller_VEECInstallerId'), $('#VEECInstallerDetail_VEECInstallerId'));
    FillDropDownVEECUpgradeManager(GetVEECUpgradeManagerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), VEECUpgradeManagerId, $('#txtVEECUpgradeManager_Id'), $('#VEECUpgradeManager_Id'));
    VEECUpgradeManagerAutocomplete($('#txtVEECUpgradeManager_Id'), $('#VEECUpgradeManager_Id'));
});
window.mapsCallback = function () {
    //isMapsApiLoaded = true;
    getLatitudeLongitude(DisplayLatLonOfInstallationAdd, $("#VEECInstallationAdd").html());
    loadMap();
};
window.onload = function () {
    SetUserAddress('popupVEECInstallation', 'VEECInstallationDetail', false, false, true);
    SetOwnerPopupDetails('popupVEECOwner', 'VEECOwnerDetail');
    SetInstallerPopupDetails('popupVEECInstaller', 'VEECInstaller');
}
function GETCompanyName(id) {
    $.ajax({
        type: "GET",
        url: GetCompanyNameFromABNUrl,
        data: { id: id },
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            if (data == 0) {
                $('#VEECUpgradeManagerDetail_CompanyName').empty();
                $('#fromDate').val("");
                $('#toDate').val("");
                $("#VEECUpgradeManagerDetail_CompanyName").append($("<option></option>").val("").html("Select"));
                $(".alert").hide();
                $("#errorMsgRegionVEECUpgradeManagerPopup").html(closeButton + "Invalide Company ABN.");
                $("#errorMsgRegionVEECUpgradeManagerPopup").show();
                return false;
            }
            else {
                if ($('#VEECUpgradeManagerDetail_CompanyName option').length > 1) {
                    $('#VEECUpgradeManagerDetail_CompanyName').empty();
                    $("#VEECUpgradeManagerDetail_CompanyName").append($("<option></option>").val("").html("Select"));
                    $.each(data, function (key, value) {
                        $("#VEECUpgradeManagerDetail_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                else {
                    $.each(data, function (key, value) {
                        $("#VEECUpgradeManagerDetail_CompanyName").append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                //initialize(data);
                
            }
        }
    });
}
function SaveVEECUpgradeManager(isEditVEECUpgradeManager, isAddVEECUpgradeManager) {
    var obj = {};
    if ($("#frmVEECUpgradeManager").valid())
    {

        if (isEditVEECUpgradeManager)
        {
            obj.VEECUpgradeManagerDetail.VEECUpgradeManagerDetailId = VEECUpgradeManagerId;
        }
        else
        {
            var data = JSON.stringify($('#frmVEECUpgradeManager').serializeToJson());
            obj = JSON.parse(data);
            obj.VEECUpgradeManagerDetail.SolarCompanyId = $('#VEECDetail_SolarCompanyId').val();
            obj.VEECUpgradeManagerDetail.VEECUpgradeManagerDetailId = 0;
            
        }
        $.ajax({
            url: AddVEECUpgradeManagerUrl,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            data: JSON.stringify(obj.VEECUpgradeManagerDetail),
            success: function (response) {
                if (response.status) {
                    if (response.veecUpgradeManagerDetailId > 0) {
                        FillDropDownVEECUpgradeManager(GetVEECUpgradeManagerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), VEECUpgradeManagerId, $('#txtVEECUpgradeManager_Id'), $('#VEECUpgradeManager_Id'));
                        $('#popupVEECUpgradeManager').modal('hide');
                        showSuccessMessageForPopup("VEEC Upgrade Manager Added Successfully", $("#successMsgRegionVEECUpgradeManager"), $("#errorMsgRegionVEECUpgradeManager"));
                    }
                }
                else {
                    showErrorMessageForPopup("VEEC Upgrade Manager Not Added", $("#errorMsgRegionVEECUpgradeManager"), $("#successMsgRegionVEECUpgradeManager"));
                }
            },
            error: function () {
                
                showErrorMessageForPopup("VEEC Upgrade Manager has not been saved.", $("#errorMsgRegionVEECInstallerPopup"), $("#successMsgRegionVEECInstallerPopup"));
            }
        });
    }
    else
    {
        return false;
    }
}
function ClearSearchGreenbotVEECInstaller() {
    $("#SearchGreenbotVEECInstaller_LicenseNumber").val("");
    $('.mulitpleVEECInstallerNote').hide();
    //$('#datatableSWHInstaller').dataTable().fnClearTable();
    $('#veecInstallerList').hide();

    //Hiding error/success msg region 
    $("#errorMsgRegionVEECInstallerPopup").hide();
    $("#successMsgRegionVEECInstallerPopup").hide();
}
function FillVEECInstaller(id) {
   
    var veecInstaller = lstVEECInstaller.find(m=>m.value == id && m.IsDeleted == 0)
    if (veecInstaller != null && veecInstaller != undefined) {
        $("#VEECInstaller_ElectricalContractorsLicenseNumber").val(veecInstaller.ElectricianLicenseNumber)
        $("#VEECInstaller_FirstName").val(veecInstaller.FirstName)
        $("#VEECInstaller_LastName").val(veecInstaller.LastName)
        $("#VEECInstaller_CompanyName").val(veecInstaller.CompanyName)
        $("#VEECInstaller_ElectricalComplianceNumber").val(veecInstaller.ElectricalComplianceNumber)
    }
    veecInstaller = lstVEECInstaller.find(m=>m.value == id)
    $("#VEECInstallerCompanyName").text(" " + veecInstaller.CompanyName);
    $("#VEECInstallerName").text(" " + veecInstaller.FirstName + " " + veecInstaller.LastName);
    $("#VEECInstallerElectricianLicenseNumber").text(" " + veecInstaller.ElectricianLicenseNumber);
    $("#VEECInstallerNameElectricalComplianceNumber").text(" " + veecInstaller.ElectricalComplianceNumber);
    
}

function FillUpgradeManager(id) {
    var VEECupgrademanager = lstVEECUpgradeManager.find(m=>m.value == id && m.IsDeleted == 0);
    if (VEECupgrademanager != null && VEECupgrademanager != undefined) {
        $("#VEECUpgradeManagerDetail_FirstName").val(VEECupgrademanager.FirstName)
        $("#VEECUpgradeManagerDetail_LastName").val(VEECupgrademanager.LastName)
        $("#VEECUpgradeManagerDetail_Phone").val(VEECupgrademanager.Phone)
        $("#VEECUpgradeManagerDetail_CompanyABN").val(VEECupgrademanager.CompanyABN)
        GETCompanyName(VEECupgrademanager.CompanyABN);
        $("#VEECUpgradeManagerDetail_CompanyName").val(VEECupgrademanager.CompanyName)
    }
    VEECupgrademanager = lstVEECUpgradeManager.find(m=>m.value == id);
    $("#VEECUpgradeManagerCompanyName").text(" " + VEECupgrademanager.CompanyName);
    $("#VEECUpgradeManagerName").text(" " + VEECupgrademanager.FirstName + " " + VEECupgrademanager.LastName);
    $("#VEECUpgradeManagerPhone").text(" " + VEECupgrademanager.Phone);
}

function SaveVEECInstaller(isEditVEECInstaller, isAddCustomVEECInstaller) {
    
    var obj = {};
    var isFormValid = false;
    var isEdit = true;
    if (isEditVEECInstaller || isAddCustomVEECInstaller) {
        //addressValidationRules("JobInstallerDetails");
        if ($("#frmVEECInstaller").valid()) {

            var data = JSON.stringify($('#frmVEECInstaller').serializeToJson());
            var obj = JSON.parse(data);
            obj.VEECInstaller.UserId = 0;
            obj.VEECInstaller.SolarCompanyId = $('#VEECDetail_SolarCompanyId').val();
            if (isEditVEECInstaller) {
                obj.VEECInstaller.VEECInstallerId = veecinstallerId;
            } else {
                obj.VEECInstaller.VEECInstallerId = 0;
            }

            //var swhInstallerdata = {};
            //swhInstallerdata = objData.VEECInstaller;

            //swhInstallerdata.InstallerDesignerId = isAddCustomSWHInstaller ? 0 : $('#hdBasicDetails_SWHInstallerID').val();
            //swhInstallerdata.SESignature = $("#imgSignSWHInstall").attr('class');
            //swhInstallerdata.LastName = $('#JobInstallerDetails_Surname').val();
            //swhInstallerdata.ElectricalContractorsLicenseNumber = $('#JobInstallerDetails_LicenseNumber').val();
            //swhInstallerdata.CECAccreditationNumber = isAddCustomSWHInstaller ? "" : $('#JobInstallerDetails_AccreditationNumber').val();
            //swhInstallerdata.SEDesignRoleId = 0;
            //swhInstallerdata.SolarCompanyId = $('#VEECDetail_SolarCompanyId').val();
            //swhInstallerdata.CreatedBy = ProjectSession_LoggedInUserId;
            //swhInstallerdata.IsSWHUser = true;

            //obj.installerDesignerView = swhInstallerdata;
            //obj.signPath = SRCSignSWHInstall;
            //obj.accreditedInstallerId = 1;
            //obj.jobId = BasicDetails_JobID;
            isFormValid = true;
        }
    }
    else {
        if ($("#frmVEECInstaller").valid()) {
            obj.VEECInstaller = {};
            obj.VEECInstaller.SolarCompanyId = $('#VEECDetail_SolarCompanyId').val();
            obj.VEECInstaller.ElectricalContractorsLicenseNumber = $('#SearchGreenbotVEECInstaller_LicenseNumber').val();
            obj.VEECInstaller.Email = $("input[name='veecInstaller']:checked").attr('email');
            obj.VEECInstaller.ElectricalComplienceNumber = $("input[name='veecInstaller']:checked").attr('ces');
            //obj.accreditedInstallerId = $("input[name='veecInstaller']:checked").attr('accreditedinstallerid');
            obj.VEECInstaller.UserId = $("input[name='veecInstaller']:checked").attr('userid');
            isFormValid = true;
        }
    }
    //obj.profileType = 3;
    //obj.jobElectricians = null;
    //obj.isEditVEECInstaller = isAddCustomVEECInstaller ? isAddCustomVEECInstaller : isEditVEECInstaller;

    if (isFormValid) {
        $.ajax({
            url: AddVEECInstallerURL,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            data: JSON.stringify(obj.VEECInstaller),
            success: function (response) {
                if (response.status) {
                    if (response.VEECInstallerId > 0) {
                        FillDropDownVEECInstaller(GetVEECInstallerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), veecinstallerId, $('#txtVEECInstaller_VEECInstallerId'), $('#VEECInstallerDetail_VEECInstallerId'));
                        $('#popupVEECInstaller').modal('hide');
                        if (isEditVEECInstaller) {
                            showSuccessMessageForPopup("VEEC Installer Updated Successfully", $("#successMsgRegionVEECInstaller"), $("#errorMsgRegionVEECInstaller"));
                        }
                        else {
                            showSuccessMessageForPopup("VEEC Installer Added Successfully", $("#successMsgRegionVEECInstaller"), $("#errorMsgRegionVEECInstaller"));
                        }
                    }
                    else if (response.VEECInstallerId == 0) {
                        showErrorMessageForPopup("Electrician With Given CES Number Is Already added", $("#errorMsgRegionVEECInstallerPopup"), $("#successMsgRegionVEECInstallerPopup"));
                    }
                }
                else {
                    showErrorMessageForPopup(response.message, $("#errorMsgRegionVEECInstallerPopup"), $("#successMsgRegionVEECInstallerPopup"));
                }

            },
            error: function () {
                //showErrorMessageInstallerDetails("SWH installer has not been saved.");
                showErrorMessageForPopup("VEEC installer has not been saved.", $("#errorMsgRegionVEECInstallerPopup"), $("#successMsgRegionVEECInstallerPopup"));
            }
        });
    }
}
function ClearPopupVEECInstaller(isFindInstaller) {
   
    $(".popupVEECInstaller").find('input[type=text]').each(function () {
        if ($(this).attr('idclass') == "installerNotRequired") {
            $(this).val('');
        }
        if (isFindInstaller) {
            if ($(this).attr('idclass') == "installerRequired") {
                $(this).val('');
            }
        }
        $(this).attr('class', 'form-control valid');
    });
    $(".popupVEECInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
    
    ////Hiding error/success msg region 
    $("#errorMsgRegionVEECInstallerPopup").hide();
    $("#successMsgRegionVEECInstallerPopup").hide();
}

function ClearPopupVEECUpgradeManager(isFindInstaller) {
    
    $("#VEECUpgradeManagerDetail_FirstName").val("")
    $("#VEECUpgradeManagerDetail_LastName").val("")
    $("#VEECUpgradeManagerDetail_Phone").val("")
    $(".popupVEECUpgradeManager").find('.field-validation-error').attr('class', 'field-validation-valid');

    ////Hiding error/success msg region 
    $("#errorMsgRegionVEECInstallerPopup").hide();
    $("#successMsgRegionVEECInstallerPopup").hide();
}
function FillDropDownVEECInstaller(url, objVEECInstaller, objtxtVEECInstaller, objhdVEECInstaller) {
    objtxtVEECInstaller.val("");
    $.ajax({
        url: url,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        success: function (success) {
            lstVEECInstaller = [];
            $.each(success, function (i, veecInstaller) {
                lstVEECInstaller.push({ value: veecInstaller.VEECInstallerId, text: veecInstaller.FirstName + " " + veecInstaller.LastName, issystemuser: veecInstaller.IsSysUser, ElectricianLicenseNumber: veecInstaller.ElectricalContractorsLicenseNumber, ElectricalComplianceNumber: veecInstaller.ElectricalComplienceNumber, CompanyName: veecInstaller.CompanyName, FirstName: veecInstaller.FirstName ,LastName :  veecInstaller.LastName,IsDeleted : veecInstaller.IsDeleted });
            });

            if (objVEECInstaller != '') {
                objhdVEECInstaller.val(objVEECInstaller);
                var lstVEECIntallerWithoutDeleted = lstVEECInstaller.filter(m=>m.IsDeleted == 0)
                $.each(lstVEECIntallerWithoutDeleted, function (key, value) {
                    if (value.value == objVEECInstaller) {
                        objtxtVEECInstaller.val(value.text);
                        
                    }
                });
                FillVEECInstaller(objVEECInstaller)
            }
            else {
                objhdVEECInstaller.val('');
                objtxtVEECInstaller.val('');
            }
        }
    });
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
function LoadVEECInstallationSignatureLocation(srcAddress) {
    loadMapScriptVEEC();
    $('#popupMapVEEC').modal({ backdrop: 'static', keyboard: false });

    setTimeout(function () {
        $('#txtSourceVEEC').focus();
    }, 1000);

    $('#txtSourceVEEC').val(srcAddress);
    $('#txtDestinationVEEC').val($("#txtVEECInstallationAdd").val());

    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
}
function loadMap() {
    var map = new google.maps.Map(document.getElementById('dvMapVEEC'));
    directionsService = new google.maps.DirectionsService();
    new google.maps.places.SearchBox(document.getElementById('txtSourceVEEC'));
    new google.maps.places.SearchBox(document.getElementById('txtDestinationVEEC'));

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
        $("#txtSourceVEEC").val(source);
        $("#txtDestinationVEEC").val(dest);
    });

}
function loadMapScriptVEEC() {

    var scriptMap = document.createElement("script");
    scriptMap.type = "text/javascript";
    var src = VEECMapKeyUrl + '&callback=mapsCallback';
    src = src.toString().replace(/&amp;/g, '&');
    scriptMap.src = src;

    var len = $('script[src="' + src + '"]').length;
    if (len <= 0) {
        if (scriptMap.readyState) {  //IE
            scriptMap.onreadystatechange = function () {
                if (scriptMap.readyState == "loaded" ||
                        scriptMap.readyState == "complete") {
                    scriptMap.onreadystatechange = null;
                    loadMap();
                    var a = setTimeout(function () { geocodeAddress($('#txtVEECInstallationAdd').val()); }, 1000);
                }
            };
        } else {  //Others
            scriptMap.onload = function () {
                loadMap();
                var a = setTimeout(function () { geocodeAddress($('#txtVEECInstallationAdd').val()); }, 1000);
            };
        }

        document.body.appendChild(scriptMap);
    }
    else {
        var a = setTimeout(function () { geocodeAddress($('#txtVEECInstallationAdd').val()); }, 1000);
    }
}
function GetRoute() {
    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    source = document.getElementById("txtSourceVEEC").value;
    destination = document.getElementById("txtDestinationVEEC").value;

    if (source != "" && destination != "") {
        var India = new google.maps.LatLng(51.508742, -0.120850);
        var mapOptions = {
            zoom: 4,
            center: India
        };
        map = new google.maps.Map(document.getElementById('dvMapVEEC'), mapOptions);

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
    source = document.getElementById("txtSourceVEEC").value;
    destination = document.getElementById("txtDestinationVEEC").value;


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
    var map = new google.maps.Map(document.getElementById('dvMapVEEC'), {
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
function geocodeAddress(address) {
    $("#errorMsgRegionMap").hide();
    var geocoder = new google.maps.Geocoder();
    if (address.trim() == "") {

        createMarker('Victoria , Australia', -37.4713, 144.7852);
    }
    else {
        geocoder.geocode({ address: address }, function (results, status) {
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
function createMarker(add, lat, lng) {
    var infowindow = new google.maps.InfoWindow();
    var latlng = new google.maps.LatLng(lat, lng);

    var map = new google.maps.Map(document.getElementById('dvMapVEEC'), {
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
function InstallationBtnClickEvent() {
    $('#btnVEECInstallationDetails').click(function () {
        $('#popupVEECInstalltionInfo').modal({ backdrop: 'static', keyboard: false });
    });
}




function showHideAddress(addressID) {
    if (addressID == 1) {
        $('.DPA').show();
        $('.PDA').hide();
    }
    else {
        $('.PDA').show();
        $('.DPA').hide();
    }
}



function clearPopupVEECOwner() {
    $(".popupVEECOwner").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupVEECOwner").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#VEECOwnerDetail_UnitTypeID").val($("#VEECOwnerDetail_UnitTypeID option:first").val());
    $("#VEECOwnerDetail_StreetTypeID").val($("#VEECOwnerDetail_StreetTypeID option:first").val());
    $("#VEECOwnerDetail_PostalAddressID").val($("#VEECOwnerDetail_PostalAddressID option:first").val());
}
function ReloadSTCVeecScreen(veecId) {
    $("#loading-image").show();
    setTimeout(function () {
        $.get('@Url.Action("GetSTCVeecNewScreen", "VEEC")?veecId='+veecId, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            //$('#checkListItemForTrade').load(actionCheckListItemForTrade, callbackCheckList);
            $("#loading-image").hide();
        });
    }, 500);
}
function clearPopupVEECInstallation() {
    $(".popupVEECInstallation").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupVEECInstallation").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#VEECInstallationDetail_UnitTypeID").val($("#VEECInstallationDetail_UnitTypeID option:first").val());
    $("#VEECInstallationDetail_StreetTypeID").val($("#VEECInstallationDetail_StreetTypeID option:first").val());
    $("#VEECInstallationDetail_PostalAddressID").val($("#VEECInstallationDetail_PostalAddressID option:first").val());
}

function validateVEECInstallationInfo() {
    if ($('#VEECInstalltionInfo').valid()) {
        var JsonData = {
            IndustryBusinessType: $("#VEECInstallationDetail_IndustryBusinessType").val(),
            NumberOfLevels: $("#VEECInstallationDetail_NumberOfLevels").val(),
            FloorSpace: $("#VEECInstallationDetail_FloorSpace").val(),
            FloorSpaceUpgradedArea: $("#VEECInstallationDetail_FloorSpaceUpgradedArea").val(),
            CertificateElectricalComplianceNumber: $("#VEECInstallationDetail_CertificateElectricalComplianceNumber").val()
        };

        AddDataInQueue(VEECInstalltionInfo, JsonData);
        $('#popupVEECInstalltionInfo').modal('toggle');
        DisplayVEECInstallationInfo();
    }
}
function GetVEECInstallationInfo() {

    $("#VEECInstallationDetail_IndustryBusinessType").val(VEECInstalltionInfo[0].IndustryBusinessType);
    $("#VEECInstallationDetail_NumberOfLevels").val(VEECInstalltionInfo[0].NumberOfLevels);
    $("#VEECInstallationDetail_FloorSpace").val(VEECInstalltionInfo[0].FloorSpace);
    $("#VEECInstallationDetail_FloorSpaceUpgradedArea").val(VEECInstalltionInfo[0].FloorSpaceUpgradedArea);
    $("#VEECInstallationDetail_CertificateElectricalComplianceNumber").val(VEECInstalltionInfo[0].CertificateElectricalComplianceNumber);
    $("#VEECInstalltionInfo").find('input[type=text]').each(function () {
        $(this).removeClass("input-validation-error");
        $(this).next(".field-validation-error").removeClass("field-validation-error").removeClass("field-validation-valid").addClass("field-validation-valid");
    });
    //$(".popupVEECInstalltionInfo").find('.field-validation-error').attr('class', 'field-validation-valid');
}

function clearVEECInstallationInfo() {
    $("#VEECInstalltionInfo").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupVEECInstalltionInfo").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#VEECInstallationDetail_IndustryBusinessType").val($("#VEECInstallationDetail_IndustryBusinessType option:first").val());
}

function clearPopupVEECInstaller() {
    $(".popupVEECInstaller").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupVEECInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#VEECInstaller_UnitTypeID").val($("#VEECInstaller_UnitTypeID option:first").val());
    $("#VEECInstaller_StreetTypeID").val($("#VEECInstaller_StreetTypeID option:first").val());
    $("#VEECInstaller_PostalAddressID").val($("#VEECInstaller_PostalAddressID option:first").val());
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

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();

    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

$(document).on("click", "#btnSaveVEEC", function () {
    $(".modelBodyMessage").html('');
    //$(".modelBodyMessage").append(result.ValidationSummary);
    $("#warning").modal();
})
function VEECInstallerAutocomplete(objtxtVEECInstaller, objhdVEECInstaller) {
    objtxtVEECInstaller.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];

            objhdVEECInstaller.val(0);
            var lstVEECIntallerWithoutDeleted = lstVEECInstaller.filter(m=>m.IsDeleted == 0)
            $.each(lstVEECIntallerWithoutDeleted, function (key, value) {
                if (value.text.toLowerCase().indexOf(objtxtVEECInstaller.val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value, issystemuser: value.issystemuser});
                }
            });
            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id, issystemuser: item.issystemuser };
            }))
        },
        select: function (event, ui) {
            if ($(event.toElement).attr('id') == 'deleteCustomVeecInstaller') {
                return false;
            }
            objhdVEECInstaller.val(ui.item.id); // save selected id to hidden input
            objtxtVEECInstaller.val(ui.item.value);
            objtxtVEECInstaller.attr('isSystemUser', ui.item.issystemuser);
            VEECInstallerOnChange(objtxtVEECInstaller, objhdVEECInstaller, ui.item.issystemuser);
        }
    }).bind('focus', function () {
        if (!$(this).val().trim())
            $(this).keydown();

    });
    
    
    objtxtVEECInstaller.autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");

        var color = '';

        if (item.issystemuser) {
            color = 'green';
            t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        }
        else {
            color = 'grey';
            t = item.label.replace(re, "<input type='button' id='deleteCustomVeecInstaller' class='icon-link delete sprite-img' onclick=deleteCustomVeecInstaller(" + item.id + "," + VEECID + ") style = 'margin-left: 180px; margin-bottom: -30px;border: none;'></input><span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        }
            

        var atag = "<a style='margin-right:20px;color:" + color + "'liveecInstallerId='" + item.id + "'>" + t + "</a>";

        return $("<li style='font-size:14px;width:190px;'></li>")
            .data("item.autocomplete", item)
            //.append("<a>" + t + "</a>")
            .append(atag)
            .appendTo(ul);
    };
}

function VEECInstallerOnChange(objtxtVEECInstaller, objhdVEECInstaller, isSytemUser) {
    $.ajax({
        url: urlUpdateVEECInstaller,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        async: false,
        data: { veecInstallerId: objhdVEECInstaller.val(), VeecId: VEECID },
        success: function (data) {
            //Hiding error/success msg region 
            $('#successMsgRegionVEECInstaller').hide();
            $('#errorMsgRegionVEECInstaller').hide();
            if (data.status) {
                if (objhdVEECInstaller.val() != 0) {
                    showSuccessMessageForPopup("VEEC Installer has been updated.", $("#successMsgRegionVEECInstaller"), $("#errorMsgRegionVEECInstaller"))
                    FillVEECInstaller(objhdVEECInstaller.val())
                }
                if (objhdVEECInstaller.val() == 0) { // Removing SWH installer 
                    showErrorMessageForPopup("VEEC Installer has been removed.", $("#errorMsgRegionVEECInstaller"), $("#successMsgRegionVEECInstaller"));
                }
                
            }
        },
        error: function () {
            showErrorMessageForPopup("VEEC Installer has not been added.", $("#errorMsgRegionVEECInstaller"), $("#successMsgRegionVEECInstaller"));
        }
    });
}

function getVeecData() {
    if ($("#ScoIDVEEC").val() != "" && $("#ScoIDVEEC").val() > 0) {
        $("#VEECDetail_ScoIDVEEC").val($("#ScoIDVEEC").val());
    }
    var data = JSON.stringify($('form').serializeToJson());
    var objData = JSON.parse(data);
    var veecDetail = objData.VEECDetail;
    veecDetail.ScoIDVEEC = $("#VEECDetail_ScoIDVEEC").val();
    objData.VEECDetail = veecDetail;
    var ownerData = JSON.stringify($('#VEECOwner').serializeToJson());
    objData.VEECOwnerDetail = JSON.parse(ownerData).VEECOwnerDetail;
    objData.VEECOwnerDetail.IsPostalAddress = (objData.VEECOwnerDetail.IsPostalAddress == 2 ? true : false);
    var installationAddressData = JSON.stringify($('#VEECInstallation').serializeToJson());
    var objInstallationAddressData = JSON.parse(installationAddressData);
    objInstallationAddressData.IsPostalAddress = (objInstallationAddressData.IsPostalAddress == 2 ? true : false);

    var installationInfoData = JSON.stringify($('#VEECInstalltionInfo').serializeToJson());
    var objInstallationInfoData = JSON.parse(installationInfoData);

    objData.VEECInstallationDetail = $.extend({}, objInstallationAddressData.VEECInstallationDetail, objInstallationInfoData.VEECInstallationDetail);
    //objData.VEECInstallationDetail.IndustryBusinessTypeName = $('#VEECInstallationDetail_IndustryBusinessTypeName').val();
    //objData.VEECInstallationDetail.UnitTypeName = $('#VEECInstallationDetail_UnitTypeName').val();
    //objData.VEECInstallationDetail.StreetTypeName = $('#VEECInstallationDetail_StreetTypeName').val();
    objData.VEECInstallationDetail.IsPostalAddress = (objData.VEECInstallationDetail.IsPostalAddress == 2 ? true : false);
    var upgradeManagerData = JSON.stringify($('#VEECUpgradeManager').serializeToJson());
    objData.VEECUpgradeManagerDetail = JSON.parse(upgradeManagerData).VEECUpgradeManagerDetail;
    //objData.VEECInstaller.IsPostalAddress = (objData.VEECInstaller.IsPostalAddress == 2 ? true : false);

    return objData;
}

function SaveVEECJob() {
    var objData = getVeecData();
    objData.VEECDetail.VEECInstallerId = $('#VEECDetail_VEECInstallerId').val();
    var veecData = JSON.stringify(objData)
    $.ajax({
        url: urlCreateVEEC,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: veecData,
        cache: false,
        success: function (result) {
            if (!result.error)
                showSuccessMessage("VEEC has been updated.");
            else {
                //showErrorMessage("VEEC has not been saved. ");
                showErrorMessage(result.errorMessage);
            }
        },
        error: function (ex) {
            showErrorMessage("VEEC has not been saved. ");
        }
    });
}






function ShowCreatedVEECErrorMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegionForCreateVEECPopUp").html(closeButton + message);
    $("#errorMsgRegionForCreateVEECPopUp").show();
}
$(document).on('click', "#btnUploadVEEC", function () {
    //var objData = getVeecData();
    //objData.lstArea = lstArea;
    //objData.lstBaselineEquipment = lstBaselineEquipment;
    //objData.lstUpgradeEquipment = lstUpgradelineEquipment;
        $.ajax({
        url: urlUpload,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({veecId : VEECID}),
        cache: false,
        success: function (result) {
            if (result.status)
                showSuccessMessage("VEEC has been Uploaded.");
            else
                showErrorMessage(result.errorMsg);
        },
        error: function (ex) {
            showErrorMessage("VEEC has not been uploaded. ");
        }
    });
});


function checkExistVEECInstaller(obj) {

    var fieldName = "SearchGreenbotVEECInstaller_LicenseNumber";//obj.id;
    var chkvar = '';
    var licenseNumber = obj;//obj.value;
    var sID = 0;
    sID = $('#VEECDetail_SolarCompanyId').val();

    $("#errorMsgRegionVEECInstallerPopup").hide();
    $("#successMsgRegionVEECInstallerPopup").hide();

    if (licenseNumber != "" && licenseNumber != undefined) {
        $.ajax({
            url: CheckVEECUserExistURL,
            data: { LicenseNumber: licenseNumber },
            contentType: 'application/json',
            method: 'get',
            success: function (data) { 
                // clearing data table
                $('.mulitpleVEECInstallerNote').hide();
                $('#datatableVEECInstaller').dataTable().fnClearTable();

                if (data.status == false) {
                    var errorMsg;
                    errorMsg = data.message;
                    //chkvar = false;
                    $(".alert").hide();
                    $("#errorMsgRegionVEECInstallerPopup").html(closeButton + errorMsg);
                    $("#errorMsgRegionVEECInstallerPopup").show();
                }
                else {
                    var veecInstallerData = JSON.parse(data.veecInstallerData);
                    var veecInstallerList = [];

                    if (veecInstallerData.length > 1) {
                        $('.mulitpleVEECInstallerNote').show();
                        $('.mulitpleVEECInstallerNote').html("There are mulitple users with same license number.Please add appropriate one.");
                    }
                    else {
                        $('.mulitpleVEECInstallerNote').hide();
                    }
                    $.each(veecInstallerData, function (key, value) {
                        veecInstallerList.push({
                            //AccreditedInstallerId: value.AccreditedInstallerId
                            Name: value.Name
                           , LiecenseNumber: value.ElectricalContractorsLicenseNumber
                            , ElectricalComplienceNumber: value.ElectricalComplienceNumber
                            //, InstallerStatus: value.InstallerStatus
                            //, InstallerExpiryDate: value.InstallerExpiryDate
                           //, FullAddress: value.FullAddress
                           , Phone: value.Phone
                           , Email: value.Email
                           //, IsSystemUser: value.IsSystemUser
                           , UserId: value.UserId
                        });
                    });

                    var veecTable = $('#datatableVEECInstaller').DataTable({
                        bDestroy: true,
                        iDisplayLength: 10,
                        columns: [
                                    {
                                        'data': 'Id',
                                        "render": function (data, type, full, meta) {
                                            return '<input type="radio" name="veecInstaller" class="rbSWHInstaller" '+ ' userId=' + full.UserId + ' email=' + full.Email + ' ces ='+full.ElectricalComplienceNumber+ ' >';
                                        }
                                    },
                                    { 'data': 'Name' },
                                    { 'data': 'LiecenseNumber' },
                                    { 'data': 'ElectricalComplienceNumber' },
                                    { 'data': 'Phone' },
                                    { 'data': 'Email' },
                        ],
                        dom: "<<'table-responsive tbfix't>>"
                        //createdRow: function (row, data, dataIndex) {
                        //    if (data.IsSystemUser == true) {
                        //        $(row.cells).css("color", "green");
                        //    }
                        //}
                    });
                    veecTable.rows.add(veecInstallerList).draw();
                    $('#datatableVEECInstaller').show();
                    $('#veecInstallerList').show();
                    //chkvar = true;
                }
            }
        });
    }
    else {
        alert("Please enter license number");
        return false;
    }
}


function showSuccessMessageForPopup(message, objSuccess, objError) {
    $(".alert").hide();
    if (objError)
        objError.hide();
    objSuccess.html(closeButton + message);
    objSuccess.show();
}
function showErrorMessageForPopup(message, objError, objSuccess) {
    $(".alert").hide();
    if (objSuccess)
        objSuccess.hide();
    objError.html(closeButton + message);
    objError.show();
}

//function checkExistInstaller(obj, title) {
//    var fieldName = obj.id;
//    var chkvar = '';
//    var uservalue = obj.value;
//    var sID = 0;
//    sID = $('#VEECDetail_SolarCompanyId').val();
//    if (uservalue != "" && uservalue != undefined) {
//        $.ajax(
//             {
//                 url: CheckUserExistURL,
//                 data: { UserValue: uservalue, FieldName: fieldName, userId: null, resellerID: null, solarCompanyId: parseInt(sID) },
//                 contentType: 'application/json',
//                 method: 'get',
//                 success: function (data) {
//                     if (data.status == false) {
//                         var errorMsg;
//                         errorMsg = data.message;
//                         chkvar = false;
//                         $(".alert").hide();
//                         $("#errorMsgRegionInstallerPopup").html(closeButton + errorMsg);
//                         $("#errorMsgRegionInstallerPopup").show();
//                     }
//                     else {
//                         chkvar = true;
//                     }
//                     $("#imgSignatureInstaller").hide();
//                     $("[name='InstallerView.SESignature']").val('');
//                     if (chkvar == false) {
//                         $("VEECInstaller_FirstName").val("");
//                         $("VEECInstaller_LastName").val("");
//                         $("#VEECInstaller_Email").val("");
//                         $("#VEECInstaller_Phone").val("");
//                         $("#VEECInstaller_Mobile").val("");
//                         $("#VEECInstaller_ElectricalContractorsLicenseNumber").val("");
//                         //$(".installerDiv").find("[name='SEDesignRoleId']").filter('[value=' + 2 + ']').prop('checked', true);
//                         $("#VEECInstaller_IsPostalAddress").val(1);
//                         $("#VEECInstaller_UnitTypeID").val("");
//                         $("#VEECInstaller_UnitNumber").val("");
//                         $("#VEECInstaller_StreetNumber").val("");
//                         $("#VEECInstaller_StreetName").val("");
//                         $("#VEECInstaller_StreetTypeID").val("");
//                         $("#VEECInstaller_Town").val("");
//                         $("#VEECInstaller_State").val("");
//                         $("#VEECInstaller_PostCode").val("");
//                         return false;
//                     }
//                     $("#errorMsgRegionInstallerPopup").hide();
//                     var installerDesignerData = JSON.parse(data.accreditedData);
//                     $("#VEECInstaller_FirstName").val(installerDesignerData.FirstName);
//                     $("#VEECInstaller_LastName").val(installerDesignerData.LastName);
//                     $("#VEECInstaller_Email").val(installerDesignerData.Email);
//                     $("#VEECInstaller_Phone").val(installerDesignerData.Inst_Phone);
//                     $("#VEECInstaller_Mobile").val(installerDesignerData.Inst_Mobile);
//                     $("#VEECInstaller_ElectricalContractorsLicenseNumber").val(installerDesignerData.LicensedElectricianNumber);
//                     //$(".installerDiv").find("[name='SEDesignRoleId']").filter('[value=' + installerDesignerData.RoleId + ']').prop('checked', true);
//                     $("#VEECInstaller_IsPostalAddress").val(1);
//                     if (parseInt(installerDesignerData.Inst_UnitTypeID) > 0)
//                         $("#VEECInstaller_UnitTypeID").val(installerDesignerData.Inst_UnitTypeID);
//                     else
//                         $("#VEECInstaller_UnitTypeID").val('');
//                     $("#VEECInstaller_UnitNumber").val(installerDesignerData.MailingAddressUnitNumber);
//                     $("#VEECInstaller_StreetNumber").val(installerDesignerData.MailingAddressStreetNumber);
//                     $("#VEECInstaller_StreetName").val(installerDesignerData.MailingAddressStreetName);
//                     if (parseInt(installerDesignerData.StreetTypeID) > 0)
//                         $("#VEECInstaller_StreetTypeID").val(installerDesignerData.StreetTypeID);
//                     else
//                         $("#VEECInstaller_StreetTypeID").val('');
//                     $("#VEECInstaller_Town").val(installerDesignerData.MailingCity);
//                     $("#VEECInstaller_State").val(installerDesignerData.Abbreviation);
//                     $("#VEECInstaller_PostCode").val(installerDesignerData.PostalCode);
//                     return false;
//                 }
//             });
//    }
//}
function RemoveVEECInstaller(objTxt, objHd) {
    if (!(objHd.val() == "")) {
        VEECInstallerOnChange("", $('#VEECInstallerDetail_VEECInstallerId'))
        objTxt.val('');
    }
}

function RemoveUpgradeManager(objTxt, objHd) {
    if (!(objHd.val() == "")) {
        VEECUpgradeManagerOnChange("", $('#VEECUpgradeManager_Id'), false)
        objTxt.val('');
    }
    
}


function GetOwnerPopupDetails(popupName, controlPrefix) {
    $("#" + controlPrefix + "_OwnerType").val(OwnerJsonCommon[0].OwnerType);
    $("#" + controlPrefix + "_CompanyName").val(OwnerJsonCommon[0].CompanyName);
    $("#" + controlPrefix + "_FirstName").val(OwnerJsonCommon[0].FirstName);
    $("#" + controlPrefix + "_LastName").val(OwnerJsonCommon[0].LastName);
    $("#" + controlPrefix + "_Email").val(OwnerJsonCommon[0].Email);
    $("#" + controlPrefix + "_Phone").val(OwnerJsonCommon[0].Phone);
    $("#" + controlPrefix + "_Mobile").val(OwnerJsonCommon[0].Mobile);
    //GetUserAddress(popupName, controlPrefix, true, false, false);
    $("#" + controlPrefix + "_OwnerType").removeClass("input-validation-error");
    $("#" + controlPrefix + "_OwnerType").next(".field-validation-error").removeClass("field-validation-error").removeClass("field-validation-valid").addClass("field-validation-valid");
}
function SetOwnerPopupDetails(popupName, controlPrefix) {
    var OwnerDataJson = {
        OwnerType: $("#" + controlPrefix + "_OwnerType").val(),
        CompanyName: $("#" + controlPrefix + "_CompanyName").val(),
        FirstName: $("#" + controlPrefix + "_FirstName").val(),
        LastName: $("#" + controlPrefix + "_LastName").val(),
        Email: $("#" + controlPrefix + "_Email").val(),
        Phone: $("#" + controlPrefix + "_Phone").val(),
        Mobile: $("#" + controlPrefix + "_Mobile").val()
    };
    AddDataInQueue(OwnerJsonCommon, OwnerDataJson);
    //SetUserAddress(popupName, controlPrefix, true, false, false);
}
function validateOwnerPopupDetails(popupName, controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, FormName) {
    if ($("#" + FormName).valid()) {
        PopupToggle(popupName);
        SetOwnerPopupDetails(popupName, controlPrefix);
        DisplayVEECOwnerAddress(popupName, controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, FormName);
    }
}

function DisplayVEECOwnerAddress(popupName, controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, FormName) {
    companyName = $("#" + controlPrefix + "_CompanyName").val();
    ownerName = $("#" + controlPrefix + "_FirstName").val() + ' ' + $("#" + controlPrefix + "_LastName").val();
    fullAddress = companyName + '</br>' + ownerName;
    $("#" + AddressTagId + "Add").html(fullAddress);
    var email = $("#" + controlPrefix + "_Email").val();
    $("#" + AddressTagId + "Phone").html($("#" + controlPrefix + "_Phone").val());
    $("." + AddressTagId + "Email").html($("#" + controlPrefix + "_Email").val());
    $("." + AddressTagId + "Email").attr('href', "mailto:'" + email + "'");
    $("." + AddressTagId + "Email").attr('title', $("#" + controlPrefix + "_Email").val());
}
function clearOwnerPopupDetails(popupName, controlPrefix) {
    clearAddress(popupName, controlPrefix);
    $("#" + controlPrefix + "_OwnerType").prop("selectedIndex", 0);
}

function GetInstallerPopupDetails(popupName, controlPrefix) {
    $("#" + controlPrefix + "_CompanyName").val(InstallerJsonCommon[0].CompanyName);
    $("#" + controlPrefix + "_CECAccreditationNumber").val(InstallerJsonCommon[0].CECAccreditationNumber);
    $("#" + controlPrefix + "_ElectricalContractorsLicenseNumber").val(InstallerJsonCommon[0].ElectricalContractorsLicenseNumber);
    $("#" + controlPrefix + "_FirstName").val(InstallerJsonCommon[0].FirstName);
    $("#" + controlPrefix + "_LastName").val(InstallerJsonCommon[0].LastName);
    $("#" + controlPrefix + "_Email").val(InstallerJsonCommon[0].Email);
    $("#" + controlPrefix + "_Phone").val(InstallerJsonCommon[0].Phone);
    $("#" + controlPrefix + "_Mobile").val(InstallerJsonCommon[0].Mobile);
    GetUserAddress(popupName, controlPrefix, false, true, false);
}
function SetInstallerPopupDetails(popupName, controlPrefix) {
    var InstallerDataJson = {
        CompanyName: $("#" + controlPrefix + "_CompanyName").val(),
        CECAccreditationNumber: $("#" + controlPrefix + "_CECAccreditationNumber").val(),
        ElectricalContractorsLicenseNumber: $("#" + controlPrefix + "_ElectricalContractorsLicenseNumber").val(),
        FirstName: $("#" + controlPrefix + "_FirstName").val(),
        LastName: $("#" + controlPrefix + "_LastName").val(),
        Email: $("#" + controlPrefix + "_Email").val(),
        Phone: $("#" + controlPrefix + "_Phone").val(),
        Mobile: $("#" + controlPrefix + "_Mobile").val()
    };
    AddDataInQueue(InstallerJsonCommon, InstallerDataJson);
    SetUserAddress(popupName, controlPrefix, false, true, false);
}
function validateInstallerPopupDetails(popupName, controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, FormName) {
    if ($("#" + FormName).valid()) {
        PopupToggle(popupName);
        SetInstallerPopupDetails(popupName, controlPrefix);
        DisplayAddress(controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress);
    }
}
function clearInstallerPopupDetails(popupName, controlPrefix) {
    clearAddress(popupName, controlPrefix);
}
//$('#uploadBtnSignatureInstallerDesigner').fileupload({
//    url: Upload_UserSignature,
//    dataType: 'json',
//    done: function (e, data) {
//        var UploadFailedFiles = [];
//        UploadFailedFiles.length = 0;
//        var UploadFailedFilesName = [];
//        UploadFailedFilesName.length = 0;
//        //formbot start
//        for (var i = 0; i < data.result.length; i++) {
//            if (data.result[i].Status == true) {
//                var guid = ProjectSession_LoggedInUserId;
//                var signName = $('#imgSignInstallerDesigner').attr('class');
//                $("[name='InstallerDesignerView.SESignature']").each(function () {
//                    $(this).remove();
//                });
//                if (signName != null && signName != "") {
//                    DeleteFileFromLogoOnUpload(signName, guid, InstallerView_SESignature, ProjectImagePath);
//                }
//                var proofDocumentURL = UploadedDocumentPath;
//                var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
//                SRCSignInstallerDesigner = imagePath + "/" + data.result[i].FileName;
//                $('#imgSignInstallerDesigner').attr('src', SRCSignInstallerDesigner);
//                $('#imgSignInstallerDesigner').attr('class', data.result[i].FileName);
//                $('#imgSignatureInstallerDesigner').show();
//                $('<input type="hidden">').attr({
//                    name: 'InstallerDesignerView.SESignature',
//                    value: data.result[i].FileName,
//                    id: data.result[i].FileName,
//                }).appendTo('form');
//            }
//            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
//                UploadFailedFilesName.push(data.result[i].FileName);
//            }
//            else {
//                UploadFailedFiles.push(data.result[i].FileName);
//            }
//        }
//        if (UploadFailedFiles.length > 0) {
//            showErrorMessageForPopup(UploadFailedFiles.length + " " + "File has not been uploaded.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//        }
//        if (UploadFailedFilesName.length > 0) {
//            showErrorMessageForPopup(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//        }
//        if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
//            showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionInstallerDesignerPopup"), $("#errorMsgRegionInstallerDesignerPopup"));
//        }
//    },
//    progressall: function (e, data) {
//    },
//    singleFileUploads: false,
//    send: function (e, data) {
//        var documentType = data.files[0].type.split("/");
//        var mimeType = documentType[0];
//        if (data.files.length == 1) {
//            for (var i = 0; i < data.files.length; i++) {
//                if (data.files[i].name.length > 50) {
//                    showErrorMessageForPopup("Please upload small filename.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
//                    $('body').animate({ scrollTop: 0 }, 'slow');
//                    return false;
//                }
//            }
//        }
//        if (data.files.length > 1) {
//            for (var i = 0; i < data.files.length; i++) {
//                if (data.files[i].size > parseInt(MaxLogoSize)) {
//                    showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//                    return false;
//                }
//            }
//        }
//        else {
//            if (data.files[0].size > parseInt(MaxLogoSize)) {
//                showErrorMessageForPopup("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//                return false;
//            }
//        }
//        if (mimeType != "image") {
//            showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
//            $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
//            $('body').animate({ scrollTop: 0 }, 'slow');
//            return false;
//        }
//        $(".alert").hide();
//        $("#errorMsgRegion").html("");
//        $("#errorMsgRegion").hide();
//        // return true;
//        $('<input type="hidden">').attr({
//            name: 'Guid',
//            value: ProjectSession_LoggedInUserId,
//            id: ProjectSession_LoggedInUserId,
//        }).appendTo('form');
//        return true;
//    },
//    formData: { userId: ProjectSession_LoggedInUserId },
//    change: function (e, data) {
//        $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
//    }
//}).prop('disabled', !$.support.fileInput)
//.parent().addClass($.support.fileInput ? undefined : 'disabled');

function FillDropDownVEECUpgradeManager(url, objVEECUpgradeManagerInstaller, objtxtVEECUpgradeManagerInstaller, objhdVEECUpgradeManagerInstaller) {
    objtxtVEECUpgradeManagerInstaller.val("");
    $.ajax({
        url: url,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        success: function (success) {
            lstVEECUpgradeManager = [];
            $.each(success, function (i, veecUpgradeManager) {
                lstVEECUpgradeManager.push({ value: veecUpgradeManager.VEECUpgradeManagerDetailId, text: veecUpgradeManager.FirstName + " " + veecUpgradeManager.LastName, issystemuser: veecUpgradeManager.IsSysUser, FirstName: veecUpgradeManager.FirstName, LastName: veecUpgradeManager.LastName, Phone: veecUpgradeManager.Phone, CompanyName: veecUpgradeManager.CompanyName, CompanyABN: veecUpgradeManager.CompanyABN, IsDeleted: veecUpgradeManager.IsDeleted});
            });
            if (objVEECUpgradeManagerInstaller != '') {
                objhdVEECUpgradeManagerInstaller.val(objVEECUpgradeManagerInstaller);
                var lstVEECUpgradeManagerWithoutDeleted = lstVEECUpgradeManager.filter(m=>m.IsDeleted == 0);
                $.each(lstVEECUpgradeManagerWithoutDeleted, function (key, value) {
                    if (value.value == objVEECUpgradeManagerInstaller) {
                        objtxtVEECUpgradeManagerInstaller.val(value.text);
                        objtxtVEECUpgradeManagerInstaller.attr("issystemuser", value.issystemuser);
                        
                    }
                });
                FillUpgradeManager(objVEECUpgradeManagerInstaller)
            }
            else {
                objhdVEECUpgradeManagerInstaller.val('');
                objtxtVEECUpgradeManagerInstaller.val('');
            }
        }
    });
}


function VEECUpgradeManagerAutocomplete(objtxtVEECUpgradeManagerInstaller, objhdVEECUpgradeManagerInstaller) {
    objtxtVEECUpgradeManagerInstaller.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];

            objhdVEECUpgradeManagerInstaller.val(0);
            var lstVEECUpgradeManagerWithoutDeleted = lstVEECUpgradeManager.filter(m=>m.IsDeleted == 0);
            $.each(lstVEECUpgradeManagerWithoutDeleted, function (key, value) {
                if (value.text.toLowerCase().indexOf(objtxtVEECUpgradeManagerInstaller.val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value, issystemuser: value.issystemuser });
                }
            });
            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id, issystemuser: item.issystemuser };
            }))
        },
        select: function (event, ui) {
            if ($(event.toElement).attr('id') == 'deleteCustomVeecUpgradeManager') {
                return false;
            }
            objhdVEECUpgradeManagerInstaller.val(ui.item.id); // save selected id to hidden input
            objtxtVEECUpgradeManagerInstaller.val(ui.item.value);
            objtxtVEECUpgradeManagerInstaller.attr('isSystemUser', ui.item.issystemuser);
            VEECUpgradeManagerOnChange(objtxtVEECUpgradeManagerInstaller, objhdVEECUpgradeManagerInstaller, ui.item.issystemuser);
        }
    }).bind('focus', function () {
        if (!$(this).val().trim())
            $(this).keydown();

    });


    objtxtVEECUpgradeManagerInstaller.autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");

        var color = '';

        if (item.issystemuser) {
            color = 'green';
            t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        }
        else{
            color = 'grey';
            t = item.label.replace(re, "<input type='button' id='deleteCustomVeecUpgradeManager' class='icon-link delete sprite-img' onclick=deleteCustomVeecUpgradeManager(" + item.id + ","+VEECID+") style = 'margin-left: 180px; margin-bottom: -30px;border: none;'></input><span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        }
        var atag = "<a style='margin-right:20px;color:" + color + "'liveecUpgradeManagerId='" + item.id + "'>" + t + "</a>";

        return $("<li style='font-size:14px;width:190px;'></li>")
            .data("item.autocomplete", item)
            //.append("<a>" + t + "</a>")
            .append(atag)
            .appendTo(ul);
    };
}


function VEECUpgradeManagerOnChange(objtxtVEECUpgradeManagerInstaller, objhdVEECUpgradeManagerInstaller, issystemuser) {
    $.ajax({
        url: urlUpdateVEECUpgradeManager,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        async: false,
        data: { veecUpgradeManagerId: objhdVEECUpgradeManagerInstaller.val(), VeecId: VEECID ,IsSysUser : issystemuser},
        success: function (data) {
            //Hiding error/success msg region 
            $('#successMsgRegionVEECUpgradeManager').hide();
            $('#errorMsgRegionVEECUpgradeManager').hide();
            if (data.status) {
                if (objhdVEECUpgradeManagerInstaller.val() != 0) {
                    FillUpgradeManager(objhdVEECUpgradeManagerInstaller.val())
                    showSuccessMessageForPopup("VEEC Upgrade Manager has been updated.", $("#successMsgRegionVEECUpgradeManager"), $("#errorMsgRegionVEECUpgradeManager"))
                    
                }
                if (objhdVEECUpgradeManagerInstaller.val() == 0) { // Removing SWH installer 
                    showErrorMessageForPopup("VEEC Upgrade Manager has been removed.", $("#errorMsgRegionVEECUpgradeManager"), $("#successMsgRegionVEECUpgradeManager"));
                }
                
            }
        },
        error: function () {
            showErrorMessageForPopup("VEEC Upgrade Manager has not been added.", $("#errorMsgRegionVEECUpgradeManager"), $("#successMsgRegionVEECUpgradeManager"));
        }
    });
}


function deleteCustomVeecUpgradeManager(VeecUpgradeManagerId, VEECId) {
    if (confirm("Are you sure you want to delete UpgradeManager.")) {
        $.ajax({
            url: urlDeleteCustomVEECUpgradeManager,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            async: false,
            data: { veecUpgradeManagerId: VeecUpgradeManagerId },
            success: function (data) {
                //Hiding error/success msg region 
                $('#successMsgRegionVEECUpgradeManager').hide();
                $('#errorMsgRegionVEECUpgradeManager').hide();
                if (data.status) {
                        showSuccessMessageForPopup(data.message, $("#successMsgRegionVEECUpgradeManager"), $("#errorMsgRegionVEECUpgradeManager"))
                        FillDropDownVEECUpgradeManager(GetVEECUpgradeManagerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), VEECUpgradeManagerId, $('#txtVEECUpgradeManager_Id'), $('#VEECUpgradeManager_Id'));
                }
            },
            error: function () {
                showErrorMessageForPopup(data.message, $("#errorMsgRegionVEECUpgradeManager"), $("#successMsgRegionVEECUpgradeManager"));
            }
        });
    }
}


function deleteCustomVeecInstaller(VeecUpgradeManagerId, VEECId) {
    if (confirm("Are you sure you want to delete Installer.")) {
        $.ajax({
            url: urlDeleteCustomVEECUpgradeManager,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            async: false,
            data: { veecUpgradeManagerId: VeecUpgradeManagerId },
            success: function (data) {
                //Hiding error/success msg region 
                $('#successMsgRegionVEECUpgradeManager').hide();
                $('#errorMsgRegionVEECUpgradeManager').hide();
                if (data.status) {
                    if (objhdVEECUpgradeManagerInstaller.val() != 0) {
                        showSuccessMessageForPopup(data.message, $("#successMsgRegionVEECUpgradeManager"), $("#errorMsgRegionVEECUpgradeManager"))
                        FillDropDownVEECInstaller(GetVEECInstallerUrl + '?VEECID=' + VEECID + '&solarCompanyId=' + $('#VEECDetail_SolarCompanyId').val(), veecinstallerId, $('#txtVEECInstaller_VEECInstallerId'), $('#VEECInstallerDetail_VEECInstallerId'));
                    }
                }
            },
            error: function () {
                showErrorMessageForPopup(data.message, $("#errorMsgRegionVEECUpgradeManager"), $("#successMsgRegionVEECUpgradeManager"));
            }
        });
    }
}