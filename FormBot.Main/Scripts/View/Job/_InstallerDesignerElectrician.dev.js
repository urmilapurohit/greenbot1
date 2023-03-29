$(document).ready(function () {    
    var ElectricianspostalAddressID = typeof (JobElectricians_PostalAddressID) != 'undefined' ? JobElectricians_PostalAddressID || 0 : 0,
        ElectriciansUnitTypeID = typeof (JobElectricians_UnitTypeID) != 'undefined' ? JobElectricians_UnitTypeID || 0 : 0,
        ElectriciansStreetTypeID = typeof (JobElectricians_StreetTypeID) != 'undefined' ? JobElectricians_StreetTypeID || 0 : 0;

    dropDownData.push({ id: 'JobElectricians_UnitTypeID', key: "UnitType", value: ElectriciansUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'JobElectricians_StreetTypeID', key: "StreetType", value: ElectriciansStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'JobElectricians_PostalAddressID', key: "PostalAddress", value: ElectricianspostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' }
    );

    var InstallerDesignerPostalAddressID = typeof (InstallerDesignerView_PostalAddressID) != 'undefined' ? InstallerDesignerView_PostalAddressID || 0 : 0,
        InstallerDesignerUnitTypeID = typeof (InstallerDesignerView_UnitTypeID) != 'undefined' ? InstallerDesignerView_UnitTypeID || 0 : 0,
        InstallerDesignerStreetTypeID = typeof (InstallerDesignerView_StreetTypeID) != 'undefined' ? InstallerDesignerView_StreetTypeID || 0 : 0;

    dropDownData.push({ id: 'InstallerDesignerView_UnitTypeID', key: "UnitType", value: InstallerDesignerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'InstallerDesignerView_StreetTypeID', key: "StreetType", value: InstallerDesignerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'InstallerDesignerView_PostalAddressID', key: "PostalAddress", value: InstallerDesignerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' }
    );

    var InstallerpostalAddressID = typeof (JobInstallerDetails_PostalAddressID) != 'undefined' ? JobInstallerDetails_PostalAddressID || 0 : 0,
        InstallerUnitTypeID = typeof (JobInstallerDetails_UnitTypeID) != 'undefined' ? JobInstallerDetails_UnitTypeID || 0 : 0,
        InstallerStreetTypeID = typeof (JobInstallerDetails_StreetTypeID) != 'undefined' ? JobInstallerDetails_StreetTypeID || 0 : 0;

    dropDownData.push({ id: 'JobInstallerDetails_UnitTypeID', key: "UnitType", value: InstallerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'JobInstallerDetails_StreetTypeID', key: "StreetType", value: InstallerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'JobInstallerDetails_PostalAddressID', key: "PostalAddress", value: InstallerpostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' }
    );

    dropDownData.bindDropdown();

    if (!isSCADashboard) {
        $('#InstallerDetailHelp').click(function () {
            if (BasicDetails_JobType == 1)
                $('#popupInstallerDetailsHelp').modal({ backdrop: 'static', keyboard: false });
        });

        if (InstallerView_IsSystemUser.toLowerCase() == 'true') {
            ShowHideSystemUserIcon(true, true);
            InstallerView_IsVisitScheduled.toLowerCase() == 'true' ? ShowHideVisitScheduledIcon(true, true, InstallerView_SEStatus) : ShowHideVisitScheduledIcon(true, false, InstallerView_SEStatus);
        }
        else {
            ShowHideSystemUserIcon(true, false);
            ShowHideVisitScheduledIcon(false, false, InstallerView_SEStatus);
        }

        if (BasicDetails_JobType == 1) {

            DesignerView_IsSystemUser.toLowerCase() == 'true' ? ShowHideSystemUserIcon(false, true) : ShowHideSystemUserIcon(false, false);

            //Fill installer detail
            var insatallerid = BasicDetails_InstallerID || 0;
            var designerid = BasicDetails_DesignerID || 0;
            var electricianId = BasicDetails_JobElectricianID || 0;
            FillInstallerDesignerElectrician(GetInstallerDesignerWithStatus + '?isInstaller=true&existUserId=' + insatallerid + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, insatallerid, $('#txtBasicDetails_InstallerID'), $('#hdBasicDetails_InstallerID'), designerid, $('#txtBasicDetails_DesignerID'), $('#hdBasicDetails_DesignerID'), BasicDetails_JobElectricianID, $('#txtBasicDetails_ElectricianID'), $('#hdBasicDetails_ElectricianID'));
            ShowHideEditIcon('pvdinstaller', insatallerid);
            ShowHideEditIcon('designer', designerid);
            ShowHideEditIcon('electrician', electricianId);
            //ShowHideEditIcon('electrician', $('#hdBasicDetails_ElectricianID').val());

            MakeEnableDisableGetElectricianSignature(BasicDetails_JobElectricianID);
            MakeEnableDisableGetInstallerSignature(insatallerid);
            MakeEnableDisableGetDesignerSignature(designerid);
            ElectricianSignature();

            TownPostcodeAutoComplete($('#InstallerDesignerView_Town'), $('#InstallerDesignerView_State'), $('#InstallerDesignerView_PostCode'));
            TownPostcodeAutoComplete($('#JobElectricians_Town'), $('#JobElectricians_State'), $('#JobElectricians_PostCode'));

            RegisterElectricianSignClickEvent();
            RegisterDesignerSignClickEvent();
            RegisterInstallerSignClickEvent();

            InstDesignElectSignUploadByIcon($("#ownerGetSignByUpload"));

            InstallerDesignerSignatureUpload();
        }
        else {
            var swhInstallerId = $('#JobInstallerDetails_SWHInstallerDesignerId').val() || 0;
            FillDropDownSWHInstaller(GetElectricianURL + '?JobID=' + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&JobType=' + $("#BasicDetails_JobType").val(), swhInstallerId, $('#txtBasicDetails_SWHInstallerID'), $('#hdBasicDetails_SWHInstallerID'));
            ShowHideEditIcon('swhinstaller', swhInstallerId);
            InstDesignElectSignUploadByIcon($("#ownerGetSignByUpload"));
            TownPostcodeAutoComplete($('#JobInstallerDetails_Town'), $('#JobInstallerDetails_State'), $('#JobInstallerDetails_PostCode'));

            MakeEnableDisableGetInstallerSignature(swhInstallerId);
            SWHInstallerSignatureUpload();
            RegisterInstallerSignClickEvent();
        }

        $("#btnSetSignature").click(function (e) {
            var getSign = $('[name=getSign]:checked').val();
            var type = $("#installerDesignerType").attr("instDesignerType");
            if (getSign == 1) {
                if (type == 2) {
                    if ($("#hdBasicDetails_InstallerID").val() > 0) {
                        $("#divInstallerSignDraw").hide();
                        GetSignatureFromVisit(2, true);
                    }
                    else if ($("#JobInstallerDetails_SWHInstallerDesignerId").val() > 0) {
                        $("#divInstallerSignDraw").hide();
                        GetSignatureFromVisit(2, true);
                    }
                }
                else {
                    if ($("#hdBasicDetails_DesignerID").val() > 0) {
                        $("#divDesignerSignDraw").hide();
                        GetSignatureFromVisit(4, true);
                    }
                }
            }
            else {
                if (type == 2) {
                    if ($("#hdBasicDetails_InstallerID").val() > 0) {
                        //$("#divInstallerSignDraw").hide();
                        $("#imgInstallerSign").attr('src', SRCSignInstallerDesigner);
                        UpdateJobSignature($("#imgInstallerSign").attr('src'), 2, true);
                    }
                    else if ($("#JobInstallerDetails_SWHInstallerDesignerId").val() > 0) {
                        $("#divInstallerSignDraw").hide();
                        $("#imgInstallerSign").attr('src', SRCSignSWHInstall);
                        UpdateJobSignature($("#imgInstallerSign").attr('src'), 2, true);
                    }
                }
                else {
                    if ($("#hdBasicDetails_DesignerID").val() > 0) {
                        //$("#divDesignerSignDraw").hide();
                        $("#imgDesignerSign").attr('src', SRCSignInstallerDesigner);
                        UpdateJobSignature($("#imgDesignerSign").attr('src'), 4, true);
                    }
                }
            }
        });

        InstallerDesignerAutocomplete($('#txtBasicDetails_InstallerID'), $('#hdBasicDetails_InstallerID'), true);
        InstallerDesignerAutocomplete($('#txtBasicDetails_DesignerID'), $('#hdBasicDetails_DesignerID'), false);
        ElectricianAutocomplete($('#txtBasicDetails_ElectricianID'), $('#hdBasicDetails_ElectricianID'));
        SWHInstallerAutocomplete($('#txtBasicDetails_SWHInstallerID'), $('#hdBasicDetails_SWHInstallerID'));
    }

    $('#InstallerDesignerView_AddressID').on("change", function () {
        HideShowAddressFieldOverAddressType($("#InstallerDesignerView_AddressID").val(), 'PDA', 'DPA');
    })

    $('#InstallerDesignerView_UnitTypeID').change(function () {
        ChangeUnitTypeId('InstallerDesignerView');
    });

    $('#JobElectricians_AddressID').change(function () {
        HideShowAddressFieldOverAddressType($("#JobElectricians_AddressID").val(), 'PDA', 'DPA');
    })

    $('#JobElectricians_UnitTypeID').change(function () {
        ChangeUnitTypeId('JobElectricians');
    });

    $('#JobInstallerDetails_AddressID').change(function () {
        HideShowAddressFieldOverAddressType($("#JobInstallerDetails_AddressID").val(), 'PDA', 'DPA');
    })

    $('#JobInstallerDetails_UnitTypeID').change(function () {
        ChangeUnitTypeId('JobInstallerDetails');
    });
    $('#btnFindInstaller').click(function (e) {
        if ($('#BasicDetails_JobType').val() == 1 || isSCADashboard) {
            //Hiding error/success msg region 
            $("#errorMsgRegionInstallerDesignerPopup").hide();
            $("#successMsgRegionInstallerDesignerPopup").hide();
            $('.AddEditInstaller').hide();

            $("#FindInstaller_CECAccreditationNumber").val("");
            $("#FindInstaller_FirstName").val("");
            $("#FindInstaller_LastName").val("");
            $('.FindInstaller').show();
            IsFromFindInstaller = true;
            $('#popupInstallerDesigner').modal({ backdrop: 'static', keyboard: false });
        }
        else {

            //Hiding error/success msg region 
            $("#errorMsgRegionSWHInstallerPopup").hide();
            $("#successMsgRegionSWHInstallerPopup").hide();

            $('.SearchGreenbotSWHInstaller').hide();
            $('.AddEditSWHInstaller').hide();
            $('#btnBackFindInstaller').hide();
            ClearPopupSWHInstaller(true);   // clear Add new Installer details
            ClearSearchGreenbotSWHInstaller();  // clear Search Greenbot Installer detail

            //Hiding swh intsaller datatable
            //$('.mulitpleSWHInstallerNote').hide();
            //$('#datatableSWHInstaller').dataTable().fnClearTable();
            //$('#datatableSWHInstaller').hide();
            //$('#swhInstallerList').hide();

            //$('#btnAddSWHInstaller').attr('onclick', ' SaveSWHInstaller(false);');
            //$("#SearchGreenbotSWHInstaller_LicenseNumber").val("");

            $('.modalTitleSWHInstaller').html('Find Installer');
            $('.FindSWHInstaller').show();
            $('.SWHInstallerOptions').show();
            IsFromFindInstaller = true;
            $('#popupSWHInstaller').modal({ backdrop: 'static', keyboard: false });
        }
        e.preventDefault();
    });

    $('#btnResetFindInstaller').click(function (e) {
        $("#FindInstaller_CECAccreditationNumber").val("");
        $("#FindInstaller_FirstName").val("");
        $("#FindInstaller_LastName").val("");
        e.preventDefault();
    });

    $("#btnUpdateInstaller").click(function (e) {
        IsFromFindInstaller = false;
        if ($('#hdBasicDetails_InstallerID').val() > 0) {
            UpdateInstaller($('#hdBasicDetails_InstallerID').val());
            //FillInstallerDesigner($('#hdBasicDetails_InstallerID').val(), true);
            ////Hiding error/success msg region 
            //$("#errorMsgRegionInstallerDesignerPopup").hide();
            //$("#successMsgRegionInstallerDesignerPopup").hide();
            //$('.FindInstaller').hide();

            //$('#popupInstallerDesigner').modal({ backdrop: 'static', keyboard: false });
            //$("#popupInstallerDesigner").find('input[type=text]').each(function () {
            //    $(this).attr('class', 'form-control valid');
            //});
            //$("#popupInstallerDesigner").find('.field-validation-error').attr('class', 'field-validation-valid');
            //$('.AddEditInstaller').show();
        }
        else {
            alert("Please select Installer.");
            return false;
        }
        e.preventDefault();
    });

    $("#btnUpdateDesigner").click(function (e) {
        IsFromFindInstaller = false;
        if ($('#hdBasicDetails_DesignerID').val() > 0) {
            FillInstallerDesigner($('#hdBasicDetails_DesignerID').val(), false);

            //Hiding error/success msg region 
            $("#errorMsgRegionInstallerDesignerPopup").hide();
            $("#successMsgRegionInstallerDesignerPopup").hide();
            $("#InstallerDesignerView_LocationValidation").hide();
            $('.FindInstaller').hide();

            $('#popupInstallerDesigner').modal({ backdrop: 'static', keyboard: false });
            $("#popupInstallerDesigner").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $("#popupInstallerDesigner").find('.field-validation-error').attr('class', 'field-validation-valid');
            $('.AddEditInstaller').show();
        }
        else {
            alert("Please select Designer.");
            return false;
        }
        e.preventDefault();
    });

    $("#addInstallerDesigner").click(function () {
        $("#loading-image").css("display", "");
        SaveInstallerDesigner();
        $("#loading-image").css("display", "none");
    });

    $("#btnClosePopUpBoxInsallerDesigner").click(function () {
        $('#popupboxInsallerDesigner').modal('toggle');
    });

    $("#btnAddElectrician").click(function (e) {
        IsFromFindInstaller = false;
        $(".popupElectrician").find('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $('#JobElectricians_AddressID').val($("#JobElectricians_AddressID option:first").val());
        $("#JobElectricians_UnitTypeID").val($("#JobElectricians_UnitTypeID option:first").val());
        $("#JobElectricians_StreetTypeID").val($("#JobElectricians_StreetTypeID option:first").val());
        $("#JobElectricians_PostalAddressID").val($("#JobElectricians_PostalAddressID option:first").val());
        //ShowHideAddress($('#JobElectricians_AddressID').val());
        HideShowAddressFieldOverAddressType($("#JobElectricians_AddressID").val(), 'PDA', 'DPA');
        $('#btnSaveElectrician').attr('onclick', 'checkExistingCustomElectrician();');

        $('#popupElectrician').modal({ backdrop: 'static', keyboard: false });
        e.preventDefault();
    });

    $('#btnUpdateElectrician').click(function (e) {
        IsFromFindInstaller = false;
        if ($('#txtBasicDetails_ElectricianID').val() == "") {
            alert("Please select Electrician.");
            return false;
        }
        else {
            FillElectrician($('#hdBasicDetails_ElectricianID').val(), $('#txtBasicDetails_ElectricianID').attr('isCustomElectrician'));
            $('#btnSaveElectrician').attr('onclick', 'SaveElectrician($("#txtBasicDetails_ElectricianID").attr("isCustomElectrician"),false);')
            HideShowAddressFieldOverAddressType($('#JobElectricians_AddressID').val(), 'PDA', 'DPA');
            $('#popupElectrician').modal({ backdrop: 'static', keyboard: false });
            $("#popupElectrician").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $("#popupElectrician").find('.field-validation-error').attr('class', 'field-validation-valid');
            $("#JobElectricians_LocationValidation").hide();
        }

        e.preventDefault();
    })

    $("#btnUpdateSWHInstaller").click(function (e) {
        if ($('#hdBasicDetails_SWHInstallerID').val() > 0) {
            FillSWHInstaller($('#hdBasicDetails_SWHInstallerID').val(), $("#BasicDetails_JobType").val());
            //Hiding error/success msg region 
            $("#errorMsgRegionSWHInstallerPopup").hide();
            $("#successMsgRegionSWHInstallerPopup").hide();
            $('.FindSWHInstaller').hide();
            $('.SWHInstallerOptions').hide();
            $('.SearchGreenbotSWHInstaller').hide();

            $('.modalTitleSWHInstaller').html('Installer Detail');
            $('#btnAddUpdateSWHInstaller').attr('onclick', ' SaveSWHInstaller(true);');
            $('#btnClearPopupSWHInstaller').attr('onclick', 'ClearPopupSWHInstaller(false)');
            $('.AddEditSWHInstaller').show();
            $('#popupSWHInstaller').modal({ backdrop: 'static', keyboard: false });
            $("#popupSWHInstaller").find('input[type=text]').each(function () {
                $(this).attr('class', 'form-control valid');
            });
            $("#popupSWHInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
        }
        else {
            alert("Please select SWH Installer.");
            return false;
        }
        
        e.preventDefault();
    });

    $("#btnClosePopUpBoxSWHInsaller").click(function () {
        $('#popupboxSWHInsaller').modal('toggle');
    });

    $('#btnSearchGreenbotSWHInstaller').click(function (e) {
        $('.SWHInstallerOptions').hide();

        $('.modalTitleSWHInstaller').html('Search Greenbot Installer');
        $('.SearchGreenbotSWHInstaller').show();
        $('#btnAddSWHInstaller').attr('onclick', ' SaveSWHInstaller(false,false);');
        $('#btnBackFindInstaller').show();
    })

    $('#btnBackFindInstaller').click(function (e) {
        $('.SearchGreenbotSWHInstaller').hide();
        $('.AddEditSWHInstaller').hide();
        $('#btnBackFindInstaller').hide();
        ClearPopupSWHInstaller(true);   // clear Add new Installer details
        ClearSearchGreenbotSWHInstaller();  // clear Search Greenbot Installer detail

        $('.modalTitleSWHInstaller').html('Find Installer');
        $('.SWHInstallerOptions').show();
    })

    $('#btnAddNewSWHInstaller').click(function (e) {
        $('.SearchGreenbotSWHInstaller').hide();
        $('.SWHInstallerOptions').hide();

        //Hiding error/success msg region 
        $("#errorMsgRegionSWHInstallerPopup").hide();
        $("#successMsgRegionSWHInstallerPopup").hide();
        $('.FindSWHInstaller').hide();

        $('.modalTitleSWHInstaller').html('Add Installer');
        $('.AddEditSWHInstaller').show();
        $("#JobInstallerDetails_LicenseNumber").removeAttr("readonly");
        $("#JobInstallerDetails_Email").removeAttr("readonly");

        $("#JobInstallerDetails_UnitTypeID").val($("#JobInstallerDetails_UnitTypeID option:first").val());
        $("#JobInstallerDetails_StreetTypeID").val($("#JobInstallerDetails_StreetTypeID option:first").val());
        $("#JobInstallerDetails_PostalAddressID").val($("#JobInstallerDetails_PostalAddressID option:first").val());
        $('#btnAddUpdateSWHInstaller').attr('onclick', 'SaveSWHInstaller(false,true);');
        $('#btnClearPopupSWHInstaller').attr('onclick', 'ClearPopupSWHInstaller(true)');
        $('#btnBackFindInstaller').show();
    })
});

$('#eleGetSignFromPopup,#designerGetSignFromPopup,#installerGetSignFromPopup').click(function (e) {
    e.preventDefault();

    var type = $(this).data('type');
    $('#mdlGetSignature').data('type', type)
    if (type == "installer") {
        //installer
        $('#drpSendMail').val('2');
    }
    else if (type == "designer") {
        //designer
        $('#drpSendMail').val('4');
    }
    else if (type == "electrician") {
        //electrician
        $('#drpSendMail').val('3');
    }

    $('#mdlGetSignature').modal('show');

});

function SWHInstallerSignatureUpload() {
    var proofDocumentURL = ProjectSession_UploadedDocumentPath;

    if ($('#imgSignSWHInstall').attr('src') != "") {
        if (JobInstallerDetailsSWH_SESignature != "") {
            var SignName = $('#imgSignSWHInstall').attr('src');
            var guid = ProjectSession_LoggedInUserId;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSignSWHInstall = SRC;
            $('#imgSignSWHInstall').attr('class', SignName);
            $('#imgSWHSignatureInstaller').show();
        }
    }

    if (BasicDetails_InstallerSignature != null && BasicDetails_InstallerSignature != '' && BasicDetails_InstallerSignature != undefined) {
        $("#imgInstallerSign").attr('src', proofDocumentURL + BasicDetails_InstallerSignature_Replace + "?" + new Date().getTime());
    }
    else {
        $("#imgInstallerSign").attr('src', SRCSignSWHInstall);
    }


    $('#imgSWHSignatureInstaller').click(function () {
        $("#loading-image").css("display", "");
        $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall).load(function () {
            logoWidthSignSWHInstall = this.width;
            logoHeightSignSWHInstall = this.height;

            $('#popupboxSWHInsaller').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeightSignSWHInstall) {
                $("#imgSignSWHInstall").closest('div').height($(window).height() - 205);
                $("#imgSignSWHInstall").closest('div').css('overflow-y', 'scroll');
                $("#imgSignSWHInstall").height(logoHeightSignSWHInstall);
            }
            else {
                $("#imgSignSWHInstall").height(logoHeightSignSWHInstall);
                $("#imgSignSWHInstall").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidthSignSWHInstall || logoWidthSignSWHInstall >= screen.width - 250) {

                $('#popupboxSWHInsaller').find(".modal-dialog").width(screen.width - 250);
                $("#imgSignSWHInstall").width(logoWidthSignSWHInstall);
            }
            else {
                $("#imgSignSWHInstall").width(logoWidthSignSWHInstall);
                $('#popupboxSWHInsaller').find(".modal-dialog").width(logoWidthSignSWHInstall);
            }
            $("#loading-image").css("display", "none");
        });

        $("#popupboxSWHInsaller").unbind("error");

        $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });

    $('#uploadBtnSignatureSWHInstaller').fileupload({
        url: Upload_User,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var guid = ProjectSession_LoggedInUserId;

                    var signName = $('#imgSignSWHInstall').attr('class');
                    $("[name='JobInstallerDetails.SESignature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromLogoOnUpload(signName, guid, JobInstallerDetailsSWH_SESignature, ProjectImagePath);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignSWHInstall = imagePath + "/" + data.result[i].FileName;
                    $('#imgSignSWHInstall').attr('src', SRCSignSWHInstall);
                    $('#imgSignSWHInstall').attr('class', data.result[i].FileName);
                    $('#imgSWHSignatureInstaller').show();

                    $('<input type="hidden">').attr({
                        name: 'JobInstallerDetails.SESignature',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);
                }
            }
            if (UploadFailedFiles.length > 0) {
                showErrorMessageForPopup(UploadFailedFiles.length + " " + "File has not been uploaded.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
            }
            if (UploadFailedFilesName.length > 0) {
                showErrorMessageForPopup(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionSWHInstallerPopup"), $("#errorMsgRegionSWHInstallerPopup"));
            }
        },

        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        showErrorMessageForPopup("Please upload small filename.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxLogoSize)) {
                        showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxLogoSize)) {
                    showErrorMessageForPopup("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                    return false;
                }
            }
            if (mimeType != "image") {
                showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            // return true;
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: ProjectSession_LoggedInUserId,
                id: ProjectSession_LoggedInUserId,
            }).appendTo('form');
            return true;
        },
        formData: { userId: ProjectSession_LoggedInUserId },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}

function JobInstallerDetails_ElectricianChange() {
    var id = $('#JobInstallerDetails_ElectricianID').val();
    var jobType = $("#BasicDetails_JobType").val();
    $.ajax(
        {
            url: GetElectricianDetailbySolarcompanyURL,
            data: { Id: id, JobType: jobType },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                if (data != false) {
                    var obj = JSON.parse(data);
                    if (obj[0].SESignature != null && obj[0].SESignature != '') {
                        var SignName = obj[0].SESignature;
                        var guid = ProjectSession_LoggedInUserId;
                        var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                        SRCSignSWHInstall = imagePath + "/" + SignName;
                        $('#imgSignSWHInstall').attr('class', SignName);
                        $('#imgSWHSignatureInstaller').show();
                    }
                    else
                        $('#imgSWHSignatureInstaller').hide();

                    $('#JobInstallerDetails_FirstName').val(obj[0].FirstName);
                    $('#JobInstallerDetails_Surname').val(obj[0].LastName);
                    $('#JobInstallerDetails_Email').val(obj[0].Email);
                    $('#JobInstallerDetails_Phone').val(obj[0].Phone);
                    $('#JobInstallerDetails_Mobile').val(obj[0].Mobile);
                    $('#JobInstallerDetails_LicenseNumber').val(obj[0].ElectricalContractorsLicenseNumber);
                    $("#JobInstallerDetails_LicenseNumber").attr("readonly", "readonly");
                    $('#JobInstallerDetails_LicenseNumber').attr('onblur', '')
                    var ispostal = 'False';
                    if (obj[0].IsPostalAddress == 'False') {
                        ispostal = '1'
                        $('.InstallerPDA').hide();
                        $('.InstallerDPA').show();
                    } else {
                        ispostal = '2'
                        $('.InstallerDPA').hide();
                        $('.InstallerPDA').show();
                    }
                    $('#JobInstallerDetails_AddressID').val(ispostal);
                    $('#JobInstallerDetails_UnitTypeID').val(obj[0].UnitTypeID);
                    $('#JobInstallerDetails_UnitNumber').val(obj[0].UnitNumber);
                    $('#JobInstallerDetails_StreetNumber').val(obj[0].StreetNumber);
                    $('#JobInstallerDetails_StreetName').val(obj[0].StreetName);
                    $('#JobInstallerDetails_StreetTypeID').val(obj[0].StreetTypeID);
                    $('#JobInstallerDetails_Town').val(obj[0].Town);
                    $('#JobInstallerDetails_State').val(obj[0].State);
                    $('#JobInstallerDetails_PostCode').val(obj[0].PostCode);
                    $("#JobInstallerDetails_PostalDeliveryNumber").val(obj[0].PostalDeliveryNumber);
                    $('#JobInstallerDetails_PostalAddressID').val(obj[0].PostalAddressID);

                    ElectrianAddressJson = [];
                    var address;
                    var UnitTypeId = $("#JobInstallerDetails").find("option:selected").text();
                    var UnitNumber = $("#JobInstallerDetails.UnitNumber").val();
                    var StreetNumber = $("#JobInstallerDetails_StreetNumber").val();
                    var StreetName = $("#JobInstallerDetails_StreetName").val();
                    var StreetTypeId = $("#JobInstallerDetails_StreetTypeID").find("option:selected").text();
                    var Town = $("#JobInstallerDetails_Town").val();
                    var State = $("#JobInstallerDetails_State").val();
                    var PostCode = $("#JobInstallerDetails_PostCode").val();
                    var firstName = $('#JobInstallerDetails_FirstName').val();
                    var lastName = $('#JobInstallerDetails_LastName').val();
                    var email = $('#JobInstallerDetails_Email').val();
                    var phone = $('#JobInstallerDetails_Phone').val();
                    var mobile = $('#JobInstallerDetails_Mobile').val();

                    if ($("#JobElectricians_AddressID").val() == 1) {
                        if (UnitNumber != "") {
                            address = UnitTypeId + ' ' + UnitNumber + "/" + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                            address = address.replace("Select", "");
                        } else {
                            address = UnitTypeId + ' ' + StreetNumber + ' ' + StreetName + ' ' + StreetTypeId + ', ' + Town + ' ' + State + ' ' + PostCode;
                            address = address.replace("Select", "");
                        }
                        ElectrianAddressJson.push({
                            PostalAddressType: $("#JobInstallerDetails_AddressID").val(), UnitType: $("#JobInstallerDetails.UnitTypeID").val(), UnitNumber: $("#JobInstallerDetails.UnitNumber").val(),
                            StreetNumber: $("#JobInstallerDetails_StreetNumber").val(), StreetName: $("#JobInstallerDetails_StreetName").val(), StreetType: $("#JobInstallerDetails_StreetTypeID").val(), Town: $("#JobInstallerDetails_Town").val(),
                            State: $("#JobInstallerDetails_State").val(), PostCode: $("#JobInstallerDetails_PostCode").val(), FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile
                        });

                    } else {
                        address = Town + ' ' + State + ' ' + PostCode;
                        ElectrianAddressJson.push({
                            PostalAddressType: $("#JobInstallerDetails_AddressID").val(),
                            Town: $("#JobInstallerDetails_Town").val(),
                            State: $("#JobInstallerDetails_State").val(),
                            PostCode: $("#JobInstallerDetails_PostCode").val(),
                            FirstName: firstName, LastName: lastName, Email: email, Phone: phone, Mobile: mobile
                        })
                    }

                    $("#txtInstallerName").val($('#JobInstallerDetails_FirstName').val() + ' ' + $('#JobInstallerDetails_Surname').val());
                    $('#spantxtInstallerNames').hide();
                }
            }
        });
}

function ElectricianSignature() {
    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
    if (JobElectricians_Signature != null && JobElectricians_Signature != '' && JobElectricians_Signature != undefined) {
        var SRC = JobElectricians_Signature;
        $('#imgSign').attr('src', SRC)
        var SignName = $('#imgSign').attr('src');
        var guid = Model_Guid;
        var imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSign = SRC;
        $('#imgSign').attr('class', SignName);
        $('#imgSignature').show();
    }

    if (BasicDetails_ElectricianSignature) {
        $("#imgElectricianSign").attr('src', proofDocumentURL + BasicDetails_ElectricianSignature_Replace + "?" + new Date().getTime());
    }
    else {
        $("#imgElectricianSign").attr('src', SRCSign);
    }

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

    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });

    ElectricianSignUpload();
}

function ElectricianSignUpload() {
    $('#uploadBtnJobSignature').fileupload({
        url: UploadURL,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {

                    var OldEleSign = JobElectricians_Signature;
                    var guid = Model_Guid;
                    var signName = $('#imgSign').attr('class');
                    $("[name='Signature']").each(function () {
                        $(this).remove();
                    });

                    if (signName != null && signName != "") {
                        DeleteFileFromFolderAndElectrician(signName, guid, OldEleSign);
                    }

                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
                    var SRC = imagePath + "/" + data.result[i].FileName.replace("%", "$");

                    SRCSign = SRC;
                    $('#imgSign').attr('class', data.result[i].FileName.replace("%", "$"));

                    $('#imgSignature').show();

                    $('<input type="hidden">').attr({
                        name: 'Signature',
                        value: data.result[i].FileName.replace("%", "$"),
                        id: data.result[i].FileName.replace("%", "$"),
                    }).appendTo('form');

                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + "File has not been uploaded.");
                $("#errorMsgRegion").show();

            }
            else {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();
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
                    if (data.files[i].size > parseInt(ProjectSession_MaxImageSize)) {
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
                if (data.files[0].size > parseInt(ProjectSession_MaxImageSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
                    $("#errorMsgRegion").show();
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                    return false;
                }
            }
            if (mimeType != "image") {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Please upload a file with .jpg , .jpeg or .png extension.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            if (data.files[0].size > parseInt(1024000)) {
                //showErrorMessageInstallerDetails("Please upload Signature Within 1 MB otherwise it slow down your job performance");
                showErrorMessageForPopup("Please upload Signature Within 1 MB otherwise it slow down your job performance", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                return false;
                //alert("Please upload Signature Within 1 MB otherwise it slow down your job performance");
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();

            $('<input type="hidden">').attr({
                name: 'Guid',
                value: Model_Guid,
                id: Model_Guid,
            }).appendTo('form');
            return true;
        },
        formData: { userId: Model_Guid }
    });
}

function DeleteFileFromFolderAndElectrician(fileNames, guid, OldEleSign) {
    $.ajax({
        url: DeleteFileFromFolderAndElectricianURL,
        data: { fileName: fileNames, FolderName: guid, OldEleSign: OldEleSign },
        contentType: 'application/json',
        method: 'get',
        success: function (data) {

        },
    });
}

function RegisterElectricianSignClickEvent() {
    InstDesignElectSignUploadByIcon($("#eleGetSignByUpload"));

    $("#eleGetSignFromVisit").click(function (e) {
        //if ($("#JobElectricians_Town").val() != null && $("#JobElectricians_Town").val() != "" && $("#JobElectricians_Town").val() != undefined) {
        if ($('#hdBasicDetails_ElectricianID').val() > 0) {
            $("#divElectricianSignDraw").hide();
            GetSignatureFromVisit(3);
        }
        e.preventDefault();
    });

    $("#eleGetSignByDrawing").click(function (e) {
        //if ($("#JobElectricians_Town").val() != null && $("#JobElectricians_Town").val() != "" && $("#JobElectricians_Town").val() != undefined) {
        if ($('#hdBasicDetails_ElectricianID').val() > 0) {
            if (isDrawSignatureOn == 0) {
                isDrawSignatureOn = 1;
                $("#divElectricianSign").hide();
                $("#divElectricianSignDraw").show();
                $('.eleCSign').jSignature('clear');
            }
            else {
                alert('First save installer/designer signature.');
            }
        }
        e.preventDefault();
    });

    //$("#eleGetSignFromPopup").click(function(e){
    //    if($("#JobElectricians_Town").val() != null && $("#JobElectricians_Town").val() != "" && $("#JobElectricians_Town").val() != undefined)
    //    {
    //        $("#divElectricianSignDraw").hide();
    //        $("#imgElectricianSign").attr('src',SRCSign);
    //        UpdateJobSignature($("#imgElectricianSign").attr('src'),3);
    //    }
    //    e.preventDefault();
    //});
}

function RegisterDesignerSignClickEvent() {

    InstDesignElectSignUploadByIcon($("#designerGetSignByUpload"));

    $("#designerGetSignFromVisit").click(function (e) {
        //if($("#BasicDetails_DesignerID").val() > 0)
        //{
        //    $("#divDesignerSignDraw").hide();
        //    GetSignatureFromVisit(4);
        //}
        $('#popupboxSetSignature').modal({ backdrop: 'static', keyboard: false });
        $("#installerDesignerType").attr("instDesignerType", 4);
        $('[name=getSign][value=' + 1 + ']').prop('checked', true);
        e.preventDefault();
    });

    $("#designerGetSignByDrawing").click(function (e) {
        e.preventDefault();
        if ($("#BasicDetails_DesignerID").val() > 0) {
            if (isDrawSignatureOn == 0) {
                isDrawSignatureOn = 1;
                $("#divDesignerSign").hide();
                $("#divDesignerSignDraw").show();
                $('.designerCSign').jSignature('clear');
            }
            else {
                alert('First save installer/electrician signature.');
            }
        }

    });

    //$("#designerGetSignFromPopup").click(function(e){
    //    if($("#BasicDetails_DesignerID").val() > 0)
    //    {
    //        $("#divDesignerSignDraw").hide();
    //        $("#imgDesignerSign").attr('src',SRCSignDesigner);
    //        UpdateJobSignature($("#imgDesignerSign").attr('src'),4);
    //    }
    //    e.preventDefault();
    //});
}

function RegisterInstallerSignClickEvent() {
    InstDesignElectSignUploadByIcon($("#installerGetSignByUpload"));

    $("#installerGetSignFromVisit").click(function (e) {
        //if($("#BasicDetails_InstallerID").val() > 0)
        //{
        //    $("#divInstallerSignDraw").hide();
        //    GetSignatureFromVisit(2);
        //}    
        $('#popupboxSetSignature').modal({ backdrop: 'static', keyboard: false });
        $("#installerDesignerType").attr("instDesignerType", 2);
        $('[name=getSign][value=' + 1 + ']').prop('checked', true);
        e.preventDefault();
    });

    $("#installerGetSignByDrawing").click(function (e) {
        if ($("#BasicDetails_InstallerID").val() > 0) {
            if (isDrawSignatureOn == 0) {
                isDrawSignatureOn = 1;
                $("#divInstallerSign").hide();
                $("#divInstallerSignDraw").show();
                $('.installerCSign').jSignature('clear');
            }
            else {
                alert('First save designer/electrician signature.');
            }
        }
        e.preventDefault();
    });

    //$("#installerGetSignFromPopup").click(function(e){
    //    if($("#BasicDetails_InstallerID").val() > 0)
    //    {
    //
    //        $("#divInstallerSignDraw").hide();
    //        $("#imgInstallerSign").attr('src',SRCSignInstall);
    //        UpdateJobSignature($("#imgInstallerSign").attr('src'),2);
    //    }
    //    e.preventDefault();
    //});
}

function InstDesignElectSignUploadByIcon(objSignUpload) {
    var typeOfSignature = objSignUpload.attr('typeOfSignature');

    objSignUpload.fileupload({
        url: UploadJobSignatureURL,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {

                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    var path = proofDocumentURL + 'JobDocuments/' + Model_JobID + '/' + data.result[i].FileName;

                    if (typeOfSignature == 3) {
                        $("#imgElectricianSign").attr('src', path);
                        $("#divElectricianSignDraw").hide();
                    }
                    else if (typeOfSignature == 4) {
                        $("#imgDesignerSign").attr('src', path);
                        $("#divDesignerSignDraw").hide();
                    }
                    else if (typeOfSignature == 2) {
                        $("#imgInstallerSign").attr('src', path);
                        $("#divInstallerSignDraw").hide();
                    }
                    else if (typeOfSignature == 1) {
                        $("#imgOwnerSign").attr('src', path);
                        //$("#divInstallerSignDraw").hide();
                    }
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                //showErrorMessageInstallerDetails("File has not been uploaded.");
                typeOfSignature == 1 ? showErrorMessageForPopup("File has not been uploaded.", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup("File has not been uploaded.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
            else {
                //showSuccessMessageInstallerDetails("File has been uploaded successfully.");
                typeOfSignature == 1 ? showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionOwnerInstallation"), $("#errorMsgRegionOwnerInstallation")) : showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
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
                    if (data.files[i].size > parseInt(ProjectSession_MaxImageSize)) {
                        //showErrorMessageInstallerDetails(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        typeOfSignature == 1 ? showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        typeOfSignature == 1 ? showErrorMessageForPopup(" Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup(" Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(ProjectSession_MaxImageSize)) {
                    //showErrorMessageInstallerDetails("Maximum file size limit exceeded.Please upload a file smaller than" + " " + maxsize + "MB");
                    typeOfSignature == 1 ? showErrorMessageForPopup("Maximum file size limit exceeded.Please upload a file smaller than" + " " + maxsize + "MB", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup("Maximum file size limit exceeded.Please upload a file smaller than" + " " + maxsize + "MB", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    typeOfSignature == 1 ? showErrorMessageForPopup(" Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup(" Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                    return false;
                }
            }
            if (mimeType != "image") {
                //showErrorMessageInstallerDetails("Please upload a file with .jpg , .jpeg or .png extension.");
                typeOfSignature == 1 ? showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            if (data.files[0].size > parseInt(1024000)) {
                //showErrorMessageInstallerDetails("Please upload Signature Within 1 MB otherwise it slow down your job performance");
                typeOfSignature == 1 ? showErrorMessageForPopup("Please upload Signature Within 1 MB otherwise it slow down your job performance", $("#errorMsgRegionOwnerInstallation"), $("#successMsgRegionOwnerInstallation")) : showErrorMessageForPopup("Please upload Signature Within 1 MB otherwise it slow down your job performance", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                return false;
                //alert("Please upload Signature Within 1 MB otherwise it slow down your job performance");
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            return true;
        },
        formData: { jobId: BasicDetails_JobID, typeOfSignature: typeOfSignature }
    });
}

function UpdateJobSignature(signPath, typeOfSignature, isfromVisitProfile) {

    $.ajax(
        {
            url: UpdateJobSignatureURL,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            data: JSON.stringify({ jobId: BasicDetails_JobID, signPath: signPath, typeOfSignature: typeOfSignature }),
            success: function (response) {
                if (response.status) {
                    //showSuccessMessageInstallerDetails("Signature has been saved successfully.");
                    showSuccessMessageForPopup("Signature has been saved successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                }
                else {
                    //showErrorMessageInstallerDetails(response.error);
                    showErrorMessageForPopup(response.error, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }


                if (isfromVisitProfile) {
                    $('#popupboxSetSignature').modal('toggle');
                }
            },
            error: function () {
                //showErrorMessageInstallerDetails("Signature has not been saved.");
                showErrorMessageForPopup("Signature has not been saved.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        });
}

function GetSignatureFromVisit(typeOfSignature, isfromVisitProfile) {

    if ($("#jobVisitDetail").attr('visitCount') == 0) {
        alert('Please add visit first.');
        return false;
    }

    $.ajax(
        {
            url: GetSignatureFromVisitURL,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            data: JSON.stringify({ jobId: BasicDetails_JobID, typeOfSignature: typeOfSignature }),
            success: function (response) {
                if (response.status) {
                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    if (response.signPath) {
                        if (typeOfSignature == 3)
                            $("#imgElectricianSign").attr('src', proofDocumentURL + response.signPath);
                        else if (typeOfSignature == 2)
                            $("#imgInstallerSign").attr('src', proofDocumentURL + response.signPath);
                        else if (typeOfSignature == 4)
                            $("#imgDesignerSign").attr('src', proofDocumentURL + response.signPath);
                        else if (typeOfSignature == 1)
                            $("#imgOwnerSign").attr('src', proofDocumentURL + response.signPath);

                        //showSuccessMessageInstallerDetails("Signature has been saved successfully.");
                        showSuccessMessageForPopup("Signature has been saved successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                    }
                    else {
                        alert('No signature found from default submission visit.');
                    }
                    if (isfromVisitProfile) {
                        $('#popupboxSetSignature').modal('toggle');
                    }
                }
                else {
                    //showErrorMessageInstallerDetails(response.error);
                    showErrorMessageForPopup(response.error, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
            },
            error: function () {
                //showErrorMessageInstallerDetails("Signature has not been saved.");
                showErrorMessageForPopup("Signature has not been saved.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        });
}

function CDMouseOut() {
    cdIsmouseIn = false;
}

function SaveElectricianDrawSignature(typeOfSignature) {
    var StringOwnerBaseSignature;

    if (typeOfSignature == 3)
        StringOwnerBaseSignature = $('.eleCSign').jSignature('getData');
    if (typeOfSignature == 2)
        StringOwnerBaseSignature = $('.installerCSign').jSignature('getData');
    if (typeOfSignature == 4)
        StringOwnerBaseSignature = $('.designerCSign').jSignature('getData');

    if (StringOwnerBaseSignature != "" && StringOwnerBaseSignature != null && StringOwnerBaseSignature != undefined && StringOwnerBaseSignature != "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANwAAABQCAYAAAByKBsiAAAB3ElEQVR4Xu3TQQ0AAAgDMebfNC7uVQwsabidI0AgE1i2ZIgAgROcJyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQE5wcIhAKCC7FNERCcHyAQCgguxDZFQHB+gEAoILgQ2xQBwfkBAqGA4EJsUwQepAwAUbuAgcwAAAAASUVORK5CYII=") {
        $.ajax(
            {
                url: DrawJobSignatureURL,
                contentType: 'application/json',
                data: JSON.stringify({ jobId: BasicDetails_JobID, StringOwnerBaseSignature: StringOwnerBaseSignature, typeOfSignature: typeOfSignature }),
                method: 'post',
                success: function (response) {
                    if (response.status) {
                        var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                        var path = proofDocumentURL + response.signPath + "?" + new Date().getTime()
                        if (typeOfSignature == 3) {
                            $("#divElectricianSignDraw").hide();
                            $("#imgElectricianSign").attr('src', path);
                            $("#divElectricianSign").show();
                        }
                        if (typeOfSignature == 4) {
                            $("#divDesignerSignDraw").hide();
                            $("#imgDesignerSign").removeAttr('src');
                            $("#imgDesignerSign").attr('src', path);
                            $("#divDesignerSign").show();
                        }
                        if (typeOfSignature == 2) {
                            $("#divInstallerSignDraw").hide();
                            $("#divInstallerSign").show();
                            $("#imgInstallerSign").attr('src', path);
                        }
                        isDrawSignatureOn = 0;
                        //showSuccessMessageInstallerDetails("Signature has been saved successfully.");
                        showSuccessMessageForPopup("Signature has been saved successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                    }
                    else {
                        //showErrorMessageInstallerDetails(response.error);
                        showErrorMessageForPopup(response.error, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                    }
                },
                error: function () {
                    //showErrorMessageInstallerDetails("Signature has not been saved.");
                    showErrorMessageForPopup("Signature has not been saved.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
            });
    }
    else {
        alert('Please draw signature.');
        return false;
    }
}

function deleteImageJob() {
    var FolderName = Model_Guid;
    var fileDelete = $('#imgSign').attr('class');
    var OldEleSign = JobElectricians_Signature;
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
            {
                url: DeleteFileFromFolderAndElectricianURL,
                data: { fileName: fileDelete, FolderName: FolderName, OldEleSign: OldEleSign },
                contentType: 'application/json',
                method: 'get',
                success: function () {
                    var sign = $('#imgSign').attr('class');
                    $("[name='Signature']").each(function () {
                        $(this).remove();
                    });
                    $('#imgSign').removeAttr('src');
                    $('#imgSign').removeAttr('class');
                    $('#popupbox').modal('hide');
                    $("#imgSignature").hide();
                    $(".alert").hide();
                    SRCSign = ''
                    return false;
                }
            });
    }
}

function MakeEnableDisableGetInstallerSignature(installerId) {
    if (installerId > 0) {
        $("#installerGetSignByUpload").prop("disabled", false);
        $("#installerGetSignByUpload").removeClass('disInstallerSignature');
        $("#installerGetSignByUploadIcon").removeClass('disInstallerSignature');
        $("#installerGetSignFromVisit").removeClass('disInstallerSignature');
        $("#installerGetSignByDrawing").removeClass('disInstallerSignature');
        $("#installerGetSignFromPopup").removeClass('disInstallerSignature');
    }
    else {
        $("#installerGetSignByUpload").prop("disabled", true);
        $("#installerGetSignByUpload").addClass('disInstallerSignature');
        $("#installerGetSignByUploadIcon").addClass('disInstallerSignature');
        $("#installerGetSignFromVisit").addClass('disInstallerSignature');
        $("#installerGetSignByDrawing").addClass('disInstallerSignature');
        $("#installerGetSignFromPopup").addClass('disInstallerSignature');
    }
}

function MakeEnableDisableGetDesignerSignature(designerId) {
    if (designerId > 0) {
        $("#designerGetSignByUpload").prop("disabled", false);
        $("#designerGetSignByUpload").removeClass('disDesignerSignature');
        $("#designerGetSignByUploadIcon").removeClass('disInstallerSignature');
        $("#designerGetSignFromVisit").removeClass('disDesignerSignature');
        $("#designerGetSignByDrawing").removeClass('disDesignerSignature');
        $("#designerGetSignFromPopup").removeClass('disDesignerSignature');
    }
    else {
        $("#designerGetSignByUpload").prop("disabled", true);
        $("#designerGetSignByUpload").addClass('disDesignerSignature');
        $("#designerGetSignByUploadIcon").addClass('disInstallerSignature');
        $("#designerGetSignFromVisit").addClass('disDesignerSignature');
        $("#designerGetSignByDrawing").addClass('disDesignerSignature');
        $("#designerGetSignFromPopup").addClass('disDesignerSignature');
    }
}

function MakeEnableDisableGetElectricianSignature(electricianId) {
    //if ($("#JobElectricians_Town").val() != null && $("#JobElectricians_Town").val() != "" && $("#JobElectricians_Town").val() != undefined) {
    if (electricianId > 0) {
        $("#eleGetSignByUpload").prop("disabled", false);
        $("#eleGetSignByUpload").removeClass('disElectricianSignature');
        $("#eleGetSignByUploadIcon").removeClass('disInstallerSignature');
        $("#eleGetSignFromVisit").removeClass('disElectricianSignature');
        $("#eleGetSignByDrawing").removeClass('disElectricianSignature');
        $("#eleGetSignFromPopup").removeClass('disElectricianSignature');
    }
    else {
        $("#eleGetSignByUpload").prop("disabled", true);
        $("#eleGetSignByUpload").addClass('disElectricianSignature');
        $("#eleGetSignByUploadIcon").addClass('disInstallerSignature');
        $("#eleGetSignFromVisit").addClass('disElectricianSignature');
        $("#eleGetSignByDrawing").addClass('disElectricianSignature');
        $("#eleGetSignFromPopup").addClass('disElectricianSignature');
    }
}

function FillInstallerDesigner(installerDesignerId, isInstaller) {
    $.ajax({
        url: FillInstallerDesignerURL,
        type: "GET",
        data: { InstallerDesignerId: installerDesignerId, jobId: $('#BasicDetails_JobID').val() },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (data) {
            if (data && data.InstallerDesignerId) {
                if (data.SESignature != null && data.SESignature != '') {
                    var SignName = data.SESignature;
                    var guid = ProjectSession_LoggedInUserId;
                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignInstallerDesigner = imagePath + "/" + SignName;
                    $('#imgSignInstallerDesigner').attr('class', SignName);
                    $('#imgSignatureInstallerDesigner').show();
                }
                else
                    $('#imgSignatureInstallerDesigner').hide();

                $('#IsInstaller').val(isInstaller);
                $("#InstallerDesignerView_InstallerDesignerId").val(data.InstallerDesignerId);
                if (isInstaller)
                    $("#hdBasicDetails_InstallerID").val(data.InstallerDesignerId);
                else
                    $("#hdBasicDetails_DesignerID").val(data.InstallerDesignerId);

                $("#FindInstaller_CECAccreditationNumber").val(data.CECAccreditationNumber);
                $("#FindInstaller_FirstName").val(data.FirstName);
                $("#FindInstaller_LastName").val(data.LastName);

                $("#InstallerDesignerView_FirstName").val(data.FirstName);
                $("#InstallerDesignerView_LastName").val(data.LastName);
                $("#frmInstallerDesigner").find("[name='SEDesignRoleId']").filter('[value=' + data.SERole + ']').prop('checked', true);
                $("#InstallerDesignerView_CECAccreditationNumber").val(data.CECAccreditationNumber);
                $("#InstallerDesignerView_CECAccreditationNumber").attr("readonly", "readonly");
                $('#InstallerDesignerView_CECAccreditationNumber').attr('onblur', '')
                $("#InstallerDesignerView_Email").val(data.Email);
                $("#InstallerDesignerView_Phone").val(data.Phone);
                $("#InstallerDesignerView_Mobile").val(data.Mobile);
                $("#InstallerDesignerView_ElectricalContractorsLicenseNumber").val(data.ElectricalContractorsLicenseNumber);
                $("#InstallerDesignerView_IsVisitScheduled").val(data.IsVisitScheduled)

                if (data.IsPostalAddress == true) {
                    $('#InstallerDesignerView_AddressID').val(2);
                    HideShowAddressFieldOverAddressType(2, 'PDA', 'DPA');
                    //ShowHideAddress(2);
                }
                else {
                    $('#InstallerDesignerView_AddressID').val(1);
                    HideShowAddressFieldOverAddressType(1, 'PDA', 'DPA');
                    setTimeout(function () {
                        ChangeUnitTypeId('InstallerDesignerView');

                    }, 100);
                    //ShowHideAddress(1);
                }

                $("#InstallerDesignerView_UnitTypeID").val(data.UnitTypeID == 0 ? "" : data.UnitTypeID);
                $("#InstallerDesignerView_UnitNumber").val(data.UnitNumber);
                $("#InstallerDesignerView_StreetNumber").val(data.StreetNumber);
                $("#InstallerDesignerView_StreetName").val(data.StreetName);
                $("#InstallerDesignerView_StreetTypeID").val(data.StreetTypeID);
                $("#InstallerDesignerView_Town").val(data.Town);
                $("#InstallerDesignerView_State").val(data.State);
                $("#InstallerDesignerView_PostCode").val(data.PostCode);
                $("#InstallerDesignerView_PostalAddressID").val(data.PostalAddressID);
                $("#InstallerDesignerView_PostalDeliveryNumber").val(data.PostalDeliveryNumber);

                //RemoveInstallerDesignerValidationMsgAndBorder();
            }
            else {
                message = "Installer/Designer has not been opened."
                //showErrorMessageInstallerDetails(message);
                showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        },
        error: function () {
            message = "Installer/Designer has not been opened."
            //showErrorMessageInstallerDetails(message);
            showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
        }
    });
}

function checkExistInstallerDesigner(obj, title, isFindInstaller) {
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    var sID = 0;
    sID = companyId;//company_Id;

    if (uservalue != "" && uservalue != undefined) {
        $.ajax(
            {
                url: CheckUserExistURL,
                data: { UserValue: uservalue, FieldName: fieldName, userId: null, resellerID: null, solarCompanyId: parseInt(sID) },
                contentType: 'application/json',
                method: 'get',
                success: function (data) {
                    if (data.status == false) {
                        var errorMsg;
                        errorMsg = data.message;
                        chkvar = false;
                        if (isFindInstaller) {
                            showErrorMessageForPopup(errorMsg, $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        }
                        else {
                            showErrorMessageForPopup(errorMsg, $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        }
                    }
                    else {
                        chkvar = true;
                    }
                    $("#imgSignatureInstallerDesigner").hide();
                    $("[name='InstallerDesignerView_SESignature']").val('');

                    if (chkvar == false) {
                        $('#InstallerDesignerView_InstallerDesignerId').val(0);

                        $("#FindInstaller_FirstName").val("");
                        $("#FindInstaller_LastName").val("");
                        $("#frmInstallerDesigner").find("[name='InstallerDesignerView_SEDesignRoleId']").filter('[value=' + 2 + ']').prop('checked', true);

                        $("#InstallerDesignerView_FirstName").val("");
                        $("#InstallerDesignerView_LastName").val("");
                        $("#InstallerDesignerView_Email").val("");
                        $("#InstallerDesignerView_Phone").val("");
                        $("#InstallerDesignerView_Mobile").val("");
                        $("#InstallerDesignerView_ElectricalContractorsLicenseNumber").val("");
                        $("#frmInstallerDesigner").find("[name='SEDesignRoleId']").filter('[value=' + 2 + ']').prop('checked', true);
                        $("#InstallerDesignerView_AddressID").val(1);
                        $("#InstallerDesignerView_UnitTypeID").val("");
                        $("#InstallerDesignerView_UnitNumber").val("");
                        $("#InstallerDesignerView_StreetNumber").val("");
                        $("#InstallerDesignerView_StreetName").val("");
                        $("#InstallerDesignerView_StreetTypeID").val("");
                        $("#InstallerDesignerView_Town").val("");
                        $("#InstallerDesignerView_State").val("");
                        $("#InstallerDesignerView_PostCode").val("");
                        return false;
                    }

                    $("#errorMsgRegionInstallerDesignerPopup").hide();

                    var installerDesignerData = JSON.parse(data.accreditedData);

                    $('#InstallerDesignerView_InstallerDesignerId').val(0);

                    $("#InstallerDesignerView_CECAccreditationNumber").val(installerDesignerData.AccreditationNumber);
                    $("#FindInstaller_CECAccreditationNumber").val(installerDesignerData.AccreditationNumber);

                    $("#FindInstaller_FirstName").val(installerDesignerData.FirstName);
                    $("#FindInstaller_LastName").val(installerDesignerData.LastName);
                    $("#frmInstallerDesigner").find("[name='InstallerDesignerView_SEDesignRoleId']").filter('[value=' + installerDesignerData.RoleId + ']').prop('checked', true);

                    $("#InstallerDesignerView_FirstName").val(installerDesignerData.FirstName);
                    $("#InstallerDesignerView_LastName").val(installerDesignerData.LastName);

                    $("#InstallerDesignerView_Email").val(installerDesignerData.Email);
                    $("#InstallerDesignerView_Phone").val(installerDesignerData.Inst_Phone);
                    $("#InstallerDesignerView_Mobile").val(installerDesignerData.Inst_Mobile);
                    $("#InstallerDesignerView_ElectricalContractorsLicenseNumber").val(installerDesignerData.LicensedElectricianNumber);
                    $("#frmInstallerDesigner").find("[name='SEDesignRoleId']").filter('[value=' + installerDesignerData.RoleId + ']').prop('checked', true);
                    $("#InstallerDesignerView_AddressID").val(1);

                    if (parseInt(installerDesignerData.Inst_UnitTypeID) > 0)
                        $("#InstallerDesignerView_UnitTypeID").val(installerDesignerData.Inst_UnitTypeID);
                    else
                        $("#InstallerDesignerView_UnitTypeID").val(0);

                    $("#InstallerDesignerView_UnitNumber").val(installerDesignerData.MailingAddressUnitNumber);
                    $("#InstallerDesignerView_StreetNumber").val(installerDesignerData.MailingAddressStreetNumber);
                    $("#InstallerDesignerView_StreetName").val(installerDesignerData.MailingAddressStreetName);

                    if (parseInt(installerDesignerData.StreetTypeID) > 0)
                        $("#InstallerDesignerView_StreetTypeID").val(installerDesignerData.StreetTypeID);
                    else
                        $("#InstallerDesignerView_StreetTypeID").val(0);

                    $("#InstallerDesignerView_Town").val(installerDesignerData.MailingCity);
                    $("#InstallerDesignerView_State").val(installerDesignerData.Abbreviation);
                    $("#InstallerDesignerView_PostCode").val(installerDesignerData.PostalCode);
                    $("#InstallerDesignerView_PostalAddressID").val(0);
                    return false;
                    //}
                }
            });
    }
}

function SaveInstallerDesigner() {
    var isValid = addressValidationRules('InstallerDesignerView');

    if (isValid && (IsFromFindInstaller|| $("#frmInstallerDesigner").valid())) {
        var fileName = $("[name='InstallerDesignerView_SESignature']").val();
        var data = JSON.stringify($("#frmInstallerDesigner").serializeToJson());
        var objData = JSON.parse(data);
        var obj = {};
        //obj = objData.InstallerDesignerView;
        obj = objData;
        if (!$('#InstallerDesignerView_SEDesignRoleId').is(':visible')) {
            if ($('#IsInstaller').val() == 'true' && objData.SEDesignRoleId == 2) {
                alert('Please choose installer or designer/installer role.');
                return false;
            }
            if ($('#IsInstaller').val() == 'false' && objData.SEDesignRoleId == 1) {
                alert('Please choose designer or designer/installer role.');
                return false;
            }
        }

        obj.SESignature = $("#imgSignInstallerDesigner").attr('class');
        obj.SEDesignRoleId = $('#InstallerDesignerView_SEDesignRoleId').is(':visible') ? $("input[name='InstallerDesignerView_SEDesignRoleId']:checked").val() : objData.SEDesignRoleId;
        obj.SolarCompanyId = companyId;
        obj.CreatedBy = ProjectSession_LoggedInUserId;

        var dataInstallerDesigner = {};
        dataInstallerDesigner.installerDesignerView = obj;
        //dataInstallerDesigner.jobId = BasicDetails_JobID;

        if (!$('#InstallerDesignerView_SEDesignRoleId').is(':visible') && !$('#SEDesignRoleId').is(':visible')) {
            if ($('#IsInstaller').val() == "true") {
                dataInstallerDesigner.profileType = 2;  //Installer onChange event
            }
            else {
                dataInstallerDesigner.profileType = 4;  //Designer onChange event
            }
        }
        if ($('#SEDesignRoleId').is(':visible')) {
            if (dataInstallerDesigner.installerDesignerView.SEDesignRoleId == 1) {
                dataInstallerDesigner.profileType = 2;  //Installer
            }
            else if (dataInstallerDesigner.installerDesignerView.SEDesignRoleId == 2) {
                dataInstallerDesigner.profileType = 4;  //Designer
            }
            else if (dataInstallerDesigner.installerDesignerView.SEDesignRoleId == 3) {
                dataInstallerDesigner.profileType = 5;  //Both Installer & Designer
            }
        }
        dataInstallerDesigner.signPath = SRCSignInstallerDesigner;
        $.ajax({
            url: AddInstallerDesignerURL,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify(dataInstallerDesigner),
            async: false,
            success: function (response) {

                if (response.status) {
                    showSuccessMessageForPopup(response.message, $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                    if (isSCADashboard) {
                        GetInstallers();
                    }
                    else {
                        var existUserId = $('#IsInstaller').val() == "" ? response.InstallerDesignerId : ($('#IsInstaller').val() == 'true' ? $('#hdBasicDetails_DesignerID').val() : $('#hdBasicDetails_InstallerID').val());
                        FillInstallerDesignerElectrician(GetInstallerDesignerWithStatus + '?isInstaller=true&existUserId=' + existUserId + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, $('#hdBasicDetails_InstallerID').val(), $('#txtBasicDetails_InstallerID'), $('#hdBasicDetails_InstallerID'), $('#hdBasicDetails_DesignerID').val(), $('#txtBasicDetails_DesignerID'), $('#hdBasicDetails_DesignerID'), $('#hdBasicDetails_ElectricianID').val(), $('#txtBasicDetails_ElectricianID'), $('#hdBasicDetails_ElectricianID'));

                        if ($('#IsInstaller').val() == 'true') {
                            MakeEnableDisableGetInstallerSignature(response.InstallerDesignerId);
                            if ($('#SEDesignRoleId').is(':visible')) {
                                $('#InstallerView_Mobile').val($('#InstallerDesignerView_Mobile').val());
                                $('#InstallerView_FirstName').val($('#InstallerDesignerView_FirstName').val());
                                $('#InstallerView_LastName').val($('#InstallerDesignerView_LastName').val());
                            }
                        }

                        if ($('#IsInstaller').val() == 'false') {
                            MakeEnableDisableGetDesignerSignature(response.InstallerDesignerId);
                            if ($('#SEDesignRoleId').is(':visible')) {
                                $('#DesignerView_Mobile').val($('#InstallerDesignerView_Mobile').val());
                                $('#DesignerView_FirstName').val($('#InstallerDesignerView_FirstName').val());
                                $('#DesignerView_LastName').val($('#InstallerDesignerView_LastName').val());
                            }
                        }
                    }
                    $('#popupInstallerDesigner').modal('hide');
                }
                else {
                    $('.FindInstaller').hide();
                    $('.AddEditInstaller').show();
                    showErrorMessageForPopup(response.message, $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerPopup"));
                }
            },
            error: function () {
                showErrorMessageForPopup("Installer/Designer has not been added.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerPopup"));
            }
        });
    }
    else {
        $('.FindInstaller').hide();
        $('.AddEditInstaller').show();
        $("#loading-image").hide();
    }
}

function ResetInstallerDesigner() {
    $("#frmInstallerDesigner").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).removeClass("input-validation-error");
        if ($(this).next().find('span').length > 0) {
            $(this).next().find('span').remove();
        }
    });
    $("#imgSignatureInstallerDesigner").hide();
    $("#InstallerDesignerView_Phone").val("");
    $("#InstallerDesignerView_Mobile").val("");
    if ($('#IsInstaller').val()) {
        $("#frmInstallerDesigner").find("[name='SEDesignRoleId']").filter('[value=1]').prop('checked', true);
        SRCSignInstallerDesignererDesigner = '';
    }
    else {
        $("#frmInstallerDesigner").find("[name='SEDesignRoleId']").filter('[value=2]').prop('checked', true);
        SRCSignInstallerDesignererDesigner = '';
    }
    $("#InstallerDesignerView_AddressID").val(1);
    $("#InstallerDesignerView_UnitTypeID").val("");
    $("#InstallerDesignerView_UnitNumber").val("");
    $("#InstallerDesignerView_StreetNumber").val("");
    $("#InstallerDesignerView_StreetName").val("");
    $("#InstallerDesignerView_StreetTypeID").val("");
    $("#InstallerDesignerView_Town").val("");
    $("#InstallerDesignerView_State").val("");
    $("#InstallerDesignerView_PostCode").val("");
    $("[name='InstallerDesignerView_SESignature']").val('');
    $("#InstallerDesignerView_LocationValidation").hide();
    HideShowAddressFieldOverAddressType($("#InstallerDesignerView_AddressID").val(), 'PDA', 'DPA');
    $("#imgSignDesigner").removeAttr('class');    
}

function TownPostcodeAutoComplete(objTown, objState, objPostCode) {
    objTown.autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: ProcessRequest_User,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: request.term
                },
                success: function (data) {
                    var data1 = JSON.parse(data);
                    console.log(data1);
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
            objState.val(ui.item.state);
            objPostCode.val(ui.item.postcode);
        }
    });

    objPostCode.autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                type: 'GET',
                url: ProcessRequest_User,
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
            objState.val(ui.item.state);
            objTown.val(ui.item.location);
        }
    });
}

function InstallerDesignerSignatureUpload() {
    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
    if ($('#IsInstaller').val() == "") {
        if (BasicDetails_InstallerSignature != null && BasicDetails_InstallerSignature != '' && BasicDetails_InstallerSignature != undefined) {
            $("#imgInstallerSign").attr('src', proofDocumentURL + BasicDetails_InstallerSignature_Replace + "?" + new Date().getTime());
        }

        if (BasicDetails_DesignerSignature != null && BasicDetails_DesignerSignature != '' && BasicDetails_DesignerSignature != undefined) {
            $("#imgDesignerSign").attr('src', proofDocumentURL + BasicDetails_DesignerSignature_Replace + "?" + new Date().getTime());
        }
    }

    if ($('#IsInstaller').val() == 'true') {
        if ($('#imgSignInstallerDesigner').attr('src') != "") {
            if (InstallerView_SESignature != "") {
                var SignName = $('#imgSignInstallerDesigner').attr('src');
                var guid = ProjectSession_LoggedInUserId;
                var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                var SRC = imagePath + "/" + SignName;
                SRCSignInstallerDesigner = SRC;
                $('#imgSignInstallerDesigner').attr('class', SignName);
                $('#imgSignatureInstallerDesigner').show();
            }
        }

        if (BasicDetails_InstallerSignature != null && BasicDetails_InstallerSignature != '' && BasicDetails_InstallerSignature != undefined) {
            $("#imgInstallerSign").attr('src', proofDocumentURL + BasicDetails_InstallerSignature_Replace + "?" + new Date().getTime());
        }
        else {
            $("#imgInstallerSign").attr('src', SRCSignInstallerDesigner);
        }
    }
    if ($('#IsInstaller').val() == 'false') {
        if ($('#imgSignInstallerDesigner').attr('src') != "") {
            if (DesignerView_SESignature != "") {
                var SignName = $('#imgSignInstallerDesigner').attr('src');
                var guid = ProjectSession_LoggedInUserId;
                var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                var SRC = imagePath + "/" + SignName;
                SRCSignInstallerDesigner = SRC;
                $('#imgSignInstallerDesigner').attr('class', SignName);
                $('#imgSignatureInstallerDesigner').show();
            }
        }

        if (BasicDetails_DesignerSignature != null && BasicDetails_DesignerSignature != '' && BasicDetails_DesignerSignature != undefined) {
            $("#imgDesignerSign").attr('src', proofDocumentURL + BasicDetails_DesignerSignature_Replace + "?" + new Date().getTime());
        }
        else {
            $("#imgDesignerSign").attr('src', SRCSignInstallerDesigner);
        }
    }

    $('#imgSignatureInstallerDesigner').click(function () {
        $("#loading-image").css("display", "");
        $('#imgSignInstallerDesigner').attr('src', SRCSignInstallerDesigner).load(function () {
            logoWidthSignInstall = this.width;
            logoHeightSignInstall = this.height;

            $('#popupboxInsallerDesigner').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeightSignInstall) {
                $("#imgSignInstallerDesigner").closest('div').height($(window).height() - 205);
                $("#imgSignInstallerDesigner").closest('div').css('overflow-y', 'scroll');
                $("#imgSignInstallerDesigner").height(logoHeightSignInstall);
            }
            else {
                $("#imgSignInstallerDesigner").height(logoHeightSignInstall);
                $("#imgSignInstallerDesigner").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidthSignInstall || logoWidthSignInstall >= screen.width - 250) {

                $('#popupboxInsallerDesigner').find(".modal-dialog").width(screen.width - 250);
                $("#imgSignInstallerDesigner").width(logoWidthSignInstall);
            }
            else {
                $("#imgSignInstallerDesigner").width(logoWidthSignInstall);
                $('#popupboxInsallerDesigner').find(".modal-dialog").width(logoWidthSignInstall);
            }
            $("#loading-image").css("display", "none");
        });

        $("#popupboxInsallerDesigner").unbind("error");

        $('#imgSignInstallerDesigner').attr('src', SRCSignInstallerDesigner).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });

    $('#uploadBtnSignatureInstallerDesigner').fileupload({
        url: Upload_User,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {
                    var guid = ProjectSession_LoggedInUserId;
                    var signName = $('#imgSignInstallerDesigner').attr('class');
                    $("[name='InstallerDesignerView_SESignature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromLogoOnUpload(signName, guid, InstallerView_SESignature, ProjectImagePath);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignInstallerDesigner = imagePath + "/" + data.result[i].FileName;
                    $('#imgSignInstallerDesigner').attr('src', SRCSignInstallerDesigner);
                    $('#imgSignInstallerDesigner').attr('class', data.result[i].FileName);
                    $('#imgSignatureInstallerDesigner').show();
                    $('<input type="hidden">').attr({
                        name: 'InstallerDesignerView_SESignature',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);
                }
            }
            if (UploadFailedFiles.length > 0) {
                showErrorMessageForPopup(UploadFailedFiles.length + " " + "File has not been uploaded.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
            }
            if (UploadFailedFilesName.length > 0) {
                showErrorMessageForPopup(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                showSuccessMessageForPopup("File has been uploaded successfully.", $("#successMsgRegionInstallerDesignerPopup"), $("#errorMsgRegionInstallerDesignerPopup"));
            }
        },

        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        showErrorMessageForPopup("Please upload small filename.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxLogoSize)) {
                        showErrorMessageForPopup(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessageForPopup("Please upload file that not conatain <> ^ [] .", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxLogoSize)) {
                    showErrorMessageForPopup("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                    return false;
                }
            }
            if (mimeType != "image") {
                showErrorMessageForPopup("Please upload a file with .jpg , .jpeg or .png extension.", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            // return true;
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: ProjectSession_LoggedInUserId,
                id: ProjectSession_LoggedInUserId,
            }).appendTo('form');
            return true;
        },
        formData: { userId: ProjectSession_LoggedInUserId },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}

function deleteSignature(isInstaller) {
    var filePath = '', OldLogo = '';
    if (BasicDetails_JobType == 2) {
        OldLogo = JobInstallerDetailsSWH_SESignature;
        filePath = SRCSignSWHInstall;
    }
    else {
        filePath = SRCSignInstallerDesigner;
        OldLogo = isInstaller.toLowerCase() == 'true' ? InstallerView_SESignature : DesignerView_SESignature;
    }
    if (confirm('Are you sure you want to delete this signature ?')) {
        $.ajax(
            {
                url: DeleteInstallerDesignerSignature,
                data: { filePath: filePath, OldLogo: OldLogo },
                contentType: 'application/json',
                method: 'get',
                success: function (response) {
                    if (response) {
                        if (BasicDetails_JobType == 2) {
                            $('#imgSignSWHInstall').removeAttr('src');
                            $('#imgSignSWHInstall').removeAttr('class');
                            $('#popupboxSWHInsaller').modal('hide');
                            $('#imgSWHSignatureInstaller').hide();
                            SRCSignSWHInstall = '';
                        }
                        else {
                            $('#imgSignInstallerDesigner').removeAttr('src');
                            $('#imgSignInstallerDesigner').removeAttr('class');
                            $('#popupboxInsallerDesigner').modal('hide');
                            $('#imgSignatureInstallerDesigner').hide();
                            SRCSignInstallerDesigner = '';
                        }
                    }
                    return false;
                },
                error: function () {
                    showErrorMessageForPopup("Signature has not been deleted", $("#errorMsgRegionInstallerDesignerPopup"), $("#successMsgRegionInstallerDesignerPopup"));
                }
            });
    }
}

function RemoveInstallerDesignerValidationMsgAndBorder() {
    $("#frmInstallerDesigner").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).removeClass("input-validation-error");
        if ($(this).next().find('span').length > 0) {
            $(this).next().find('span').remove();
        }
    });
}

function FillInstallerDesignerElectrician(url, objInstallerDesignerId, objtxtInstallerDesigner, objhdInstallerDesigner, objDesignerId, objtxtDesigner, objhdDesigner, objElectricianId, objtxtElectrician, objhdElectrician) {
    installerList = [];
    designerList = [];
    electricianList = [];
    objtxtInstallerDesigner.val("");
    objtxtDesigner.val(""); 
    objtxtElectrician.val("");
        $.ajax({
            url: url,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            success: function (success) {
                DesignerList = [];
                InstallerList = [];
                electricianList = [];
                $.each(success.installerList, function (i, installerDesigner) {
                    InstallerList.push({ value: installerDesigner.Value, text: installerDesigner.Text, issystemuser: installerDesigner.IsSystemUser, userid: installerDesigner.UserId });
                });
                $.each(success.designerList, function (i, installerDesigner) {
                    DesignerList.push({ value: installerDesigner.Value, text: installerDesigner.Text, issystemuser: installerDesigner.IsSystemUser, userid: installerDesigner.UserId });
                });
                $.each(success.electricianList, function (i, electrician) {
                    electricianList.push({ value: electrician.Value, text: electrician.Text, issystemuser: electrician.IsSystemUser, iscustomelectrician: electrician.IsCustomElectrician });
                });
                installerList = InstallerList;
                designerList = DesignerList;
                if (objInstallerDesignerId != '') {
                    objhdInstallerDesigner.val(objInstallerDesignerId);
                    $.each(InstallerList, function (key, value) {
                        if (value.value == objInstallerDesignerId) {
                            objtxtInstallerDesigner.val(value.text);
                        }
                    });
                }
                else {
                    objhdInstallerDesigner.val('');
                    objtxtInstallerDesigner.val('');
                }
                if (objDesignerId != '') {
                    objhdDesigner.val(objDesignerId);
                    $.each(DesignerList, function (key, value) {
                        if (value.value == objDesignerId) {
                            objtxtDesigner.val(value.text);
                        }
                    });
                }
                else {
                    objhdDesigner.val('');
                    objtxtDesigner.val('');
                }
                if (objElectricianId != '') {
                    objhdElectrician.val(objElectricianId);
                    $.each(electricianList, function (key, value) {
                        if (value.value == objElectricianId) {
                            objtxtElectrician.val(value.text);
                            objtxtElectrician.attr('isCustomElectrician', value.iscustomelectrician);
                            objtxtElectrician.attr('isSystemUser', value.issystemuser);
                        }
                    });
                }
                else {
                    objhdElectrician.val('');
                    objtxtElectrician.val('');
                }
            }
        });
}

function InstallerDesignerAutocomplete(objtxtInstallerDesigner, objhdInstallerDesigner, isInstaller) {
    objtxtInstallerDesigner.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            var List = [];
            if (isInstaller)
                List = installerList;
            else
                List = designerList;

            objhdInstallerDesigner.val(0);
            $.each(List, function (key, value) {
                if (value.text.toLowerCase().indexOf(objtxtInstallerDesigner.val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value, issystemuser: value.issystemuser, userid: value.userid });
                }
            });
            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id, issystemuser: item.issystemuser, userid: item.userid };
            }))
        },
        select: function (event, ui) {
            objhdInstallerDesigner.val(ui.item.id); // save selected id to hidden input
            objtxtInstallerDesigner.val(ui.item.value);
            objtxtInstallerDesigner.attr('isSystemUser', ui.item.issystemuser);
            objtxtInstallerDesigner.attr('userId', ui.item.userid);
            InstallerDesignerOnChange(objtxtInstallerDesigner, objhdInstallerDesigner, isInstaller, ui.item.issystemuser, ui.item.userid);
        }
    }).bind('focus', function () {
        if (!$(this).val().trim())
            $(this).keydown();
    });
    objtxtInstallerDesigner.autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        var color = '';
        if (item.label.indexOf("(Current)") >= 0) {
            if (item.issystemuser) {
                color = 'green';
            }
            else {
                color = 'grey';
            }
        }
        else if (item.label.indexOf("(") >= 0 && item.label.indexOf("(Current)") < 0)
            color = 'red';
        else
            color = '';

        var atag = "<a style=color:" + color + ">" + t + "</a>";

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            //.append("<a>" + t + "</a>")
            .append(atag)
            .appendTo(ul);
    };
}

function InstallerDesignerOnChange(objtxtInstallerDesigner, objhdInstallerDesigner, isInstaller, isSytemUser, userId) {
    $.ajax({
        url: urlUpdateJobDetailInstallerDesigner,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        async: false,
        data: { installerDesignerId: objhdInstallerDesigner.val(), jobId: BasicDetails_JobID, profileType: isInstaller ? 2 : 4, IsSWHUser: false, userId: userId },
        success: function (data) {
            debugger;
            $('#successMsgRegionInstallerDetails').hide();
            $('#errorMsgRegionInstallerDetails').hide();
            if (data.status) {
                if (isSytemUser) {
                    if (isInstaller) {
                        if (data.SEStatus != 2) {
                            alert("Installer has been added successfully but not a part of solar company, so visits can't be scheduled.");
                        }
                        var isVisitExist = $(".visit-id:visible").length;
                        isVisitExist==0 && data.SEStatus == 2 && (ProjectSession_UserTypeId != 8 || isScheduleAnInstaller.toLowerCase() == 'true') ? addQuickVisit(objhdInstallerDesigner.val(), objtxtInstallerDesigner.val(), data.SEStatus, "Installer has been added successfully.Do you want to schedule visit?") : ShowHideVisitScheduledIcon(true, data.SEStatus == 2 ? true : false, data.SEStatus);
                    }
                    isInstaller ? ShowHideSystemUserIcon(true, true) : ShowHideSystemUserIcon(false, true);
                }
                else {
                    if (isInstaller) {
                        ShowHideSystemUserIcon(true, false);
                        ShowHideVisitScheduledIcon(false, false, 1);    //1 for SolarElectrician Status as Pending
                    }
                    else {
                        ShowHideSystemUserIcon(false, false);
                    }
                }
                InstallerView_SEStatus = data.SEStatus;
                if (objhdInstallerDesigner.val() == 0) { // Removing  installer/designer 
                    if (isInstaller) {
                        ShowHideSystemUserIcon(true, false);
                        ShowHideVisitScheduledIcon(false, false, 1);        //1 for SolarElectrician Status as Pending
                    }
                    else {
                        ShowHideSystemUserIcon(false, false);
                    }
                    showErrorMessageForPopup("Installer/Designer has been removed.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
                else {
                    debugger;
                    showSuccessMessageForPopup("Installer/Designer has been added successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                }
                if (isInstaller) {
                    $('#InstallerView_FirstName').val(data.FirstName);
                    $('#InstallerView_LastName').val(data.LastName);
                    $('#InstallerView_Email').val(data.Email);
                    $('#InstallerView_Mobile').val(data.Mobile);
                    MakeEnableDisableGetInstallerSignature(objhdInstallerDesigner.val());
                    ShowHideEditIcon(objhdInstallerDesigner.attr('name'), objhdInstallerDesigner.val());
                }
                else {
                    //For sending mail/sms for signature
                    $('#DesignerView_FirstName').val(data.FirstName);
                    $('#DesignerView_LastName').val(data.LastName);
                    $('#DesignerView_Email').val(data.Email);
                    $('#DesignerView_Mobile').val(data.Mobile);
                    MakeEnableDisableGetDesignerSignature(objhdInstallerDesigner.val());
                    ShowHideEditIcon(objhdInstallerDesigner.attr('name'), objhdInstallerDesigner.val());
                }
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                LoadCommonJobsWithSameInstallDateAndInstaller();
            }
            ReloadSTCJobScreen(BasicDetails_JobID);
            SearchHistory();
            getJobRetailerSettingDataForJobScreen(BasicDetails_JobID);
        },
        error: function () {
            showErrorMessageForPopup("Installer/Designer has not been added.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
        }
    });
    //}
}

function addQuickVisit(installerDesignerId, installerDesignerName, seStatus, confirmMsg, isYesNotification = false) {
    var msg;
    msg = confirmMsg == "" || typeof (confirmMsg) == "undefined" ? "Are you sure you want to schedule visit" : confirmMsg;
    if (seStatus != 2) {
        alert("Selected installer is a Greenbot user but not a part of solar company, so visits can't be scheduled.");
    }
    else {
        if (isYesNotification || confirm(msg)) {
            $("#loading-image").show();
            setTimeout(function () {
                visitCheckListItemIds = [];
                TempCheckListTemplateItemAdd(defaultTemplateId, true, $("#BasicDetails_JobID").val());

            var obj = {};
            obj.JobSchedulingID = 0; // Intial Default value while scheduling 
            obj.Label = $("#BasicDetails_Title").val();
            obj.Detail = $("#BasicDetails_Description").val();
                var startDate = BasicDetails_InstallationDate.toString().split(' ')[0];
                var tickStartDate = ConvertDateToTick(startDate, getDateFormat);
                obj.strVisitStartDate = moment(tickStartDate).format("YYYY-MM-DD");
                obj.strVisitStartTime = BasicDetails_InstallationDate.toString().split(' ')[1];
                obj.JobID = $("#BasicDetails_JobID").val();
                obj.UserId = installerDesignerId;//$("#txtBasicDetails_InstallerID").attr('userid');
                obj.Status = 0; // Intial Default value while scheduling 
                obj.isNotification = isYesNotification;
                obj.isDrop = false;
                obj.userName = installerDesignerName.split('(')[0];
                var title = $("#JobTitle").val();
                if (title == '' || title == undefined || title == null) {
                    title = $("#JobID option:selected").text();
                }
                obj.JobTitle = title;
                obj.TemplateId = defaultTemplateId;
                obj.IsFromCalendarView = $("#IsFromCalendarView").val();
                obj.SolarCompanyId = companyId;
                obj.VisitCheckListItemIds = visitCheckListItemIds.toString();//null;
                obj.TempJobSchedulingId = $("#TempJobSchedulingId").val();
                obj.IsClassic = $("#BasicDetails_IsClassic").val();

            var dataInstallerDesigner = {};
            dataInstallerDesigner.jobSchedulingPopup = obj;
            dataInstallerDesigner.isQuickAddvisit = true;
                $.ajax({
                    url: url_JobSchedulingDetail,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    type: 'post',
                    data: JSON.stringify(dataInstallerDesigner),
                    async: false,
                    success: function (response) {
                        if (response) {
                            var responseID = response.split('^')[0];
                            var responseData = response.replace(responseID + '^', '');
                            ret = ResponseSaveScheduleOnDropAndInsertEdit(response, responseID, responseData, false, true);
                            ShowHideVisitScheduledIcon(true, responseID > 0 ? true : false, seStatus);
                            
                        }
                    },
                    error: function () {
                        message = "Job Schedule has not been saved."
                        showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                    }
                });
            }, 500);
        }
        else {
            ShowHideVisitScheduledIcon(true, false, seStatus);
        }
    }
}

function FillDropDownElectrician(url, objElectricianId, objtxtElectrician, objhdElectrician) {
    electricianList = [];
    objtxtElectrician.val("");
    $.ajax({
        url: url,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        async: false,
        success: function (success) {
            electricianList = [];
            $.each(success, function (i, electrician) {
                electricianList.push({ value: electrician.Value, text: electrician.Text, issystemuser: electrician.IsSystemUser, iscustomelectrician: electrician.IsCustomElectrician });
            });
                if (objElectricianId != '') {
                    objhdElectrician.val(objElectricianId);
                    $.each(electricianList, function (key, value) {
                        if (value.value == objElectricianId) {
                            objtxtElectrician.val(value.text);
                            objtxtElectrician.attr('isCustomElectrician', value.iscustomelectrician);
                            objtxtElectrician.attr('isSystemUser', value.issystemuser);
                        }
                    });
                }
                else {
                    objhdElectrician.val('');
                    objtxtElectrician.val('');
                }
        }
    });
}

function ElectricianAutocomplete(objtxtElectrician, objhdElectrician) {
    objtxtElectrician.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            List = electricianList;
            objhdElectrician.val(0);
            $.each(List, function (key, value) {
                if (value.text.toLowerCase().indexOf(objtxtElectrician.val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value, issystemuser: value.issystemuser, iscustomelectrician: value.iscustomelectrician });
                }
            });
            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id, issystemuser: item.issystemuser, iscustomelectrician: item.iscustomelectrician };
            }))
        },
        select: function (event, ui) {
            if ($(event.toElement).attr('id') == 'deleteCustomElectrician') {
                return false;
            }
            objhdElectrician.val(ui.item.id); // save selected id to hidden input
            objtxtElectrician.val(ui.item.value);
            objtxtElectrician.attr('isSystemUser', ui.item.issystemuser);
            objtxtElectrician.attr('isCustomElectrician', ui.item.iscustomelectrician);
            ElectricianOnChange(objhdElectrician.val(), objtxtElectrician.attr('isCustomElectrician'))
            return false;
        }
    }).bind('focus', function () {
        if (!$(this).val().trim())
            $(this).keydown();
    });
    objtxtElectrician.autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t //= item.label.replace(re, "<span class='icon-link delete sprite-img' style = 'margin-left: 190px; margin-bottom: -30px;'></span><span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        var color = '';
        if (item.label.indexOf("(Current)") >= 0) {
            if (item.issystemuser) {
                color = 'green';
            }
            else {
                color = 'grey';
            }
        }
        else if (item.label.indexOf("(") >= 0 && item.label.indexOf("(Current)") < 0)
            color = 'red';
        else
            color = '#35B0D1';

        if (item.iscustomelectrician)
            t = item.label.replace(re, "<input type='button' id='deleteCustomElectrician' class='icon-link delete sprite-img' onclick=deleteCustomElectrician(" + item.id + ",$('#BasicDetails_JobID').val()) style = 'margin-left: 180px; margin-bottom: -30px;border: none;'></input><span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");
        else
            t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");

        var atag = "<a style='color:" + color + "'lisolarElectricianId='" + item.id + "'>" + t + "</a>";

        return $("<li class='electrician-delete'></li>")
            .data("item.autocomplete", item)
            .append(atag)
            .appendTo(ul);
    };
}

function FillElectrician(id, isCustomElectrician) {
    var url = isCustomElectrician.toLowerCase() == 'true' ? GetElectricianDetailbySolarcompanyURL : GetElectricianDetailbyInstallerURL;
    $.ajax({
        url: url,
        data: { Id: id },
        contentType: 'application/json',
        method: 'get',
        async: false,
        success: function (data) {
            if (data) {
                var electricianData = JSON.parse(data);
                if (electricianData[0].Signature != null && electricianData[0].Signature != '') {
                    var guid = '0';
                    var imagePath = '';
                    var SignName = electricianData[0].Signature;
                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    if (isCustomElectrician.toLowerCase() == 'false') {
                        guid = electricianData[0].UserID;
                        imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    }
                    else {
                        guid = electricianData[0].JobElectricianID;
                        imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
                    }
                    var SRC = imagePath + "/" + SignName;
                    var SignName = $('#imgSign').attr('src');
                    SRCSign = SRC;
                    $('#imgSign').attr('class', SignName);
                    $('#imgSignature').show();

                    if (isCustomElectrician.toLowerCase() == 'false') {
                        $("input[type='hidden'][name='Signature']").val(electricianData[0].Signature);
                        $('<input type="hidden">').attr({
                            name: 'ProfileSignature',
                            value: electricianData[0].Signature,
                            id: electricianData[0].Signature,
                        }).appendTo('form');
                        $('<input type="hidden">').attr({
                            name: 'ProfileSignatureID',
                            value: electricianData[0].UserID,
                            id: electricianData[0].UserID,
                        }).appendTo('form');
                    }

                } else {
                    $('#imgSign').attr('class', "");
                    $('#imgSign').removeAttr('src');
                    $('#imgSignature').hide();
                }
                if (isCustomElectrician.toLowerCase() == 'true') {
                    $('#JobElectricians_ElectricianID').val(electricianData[0].JobElectricianID);
                    $('#JobElectricians_InstallerID').val(0);
                }
                else {
                    $('#JobElectricians_InstallerID').val(electricianData[0].InstallerID);
                    $('#JobElectricians_ElectricianID').val(0);
                }

                $("#JobElectricians_LicenseNumber").val(electricianData[0].ElectricalContractorsLicenseNumber);
                $("#JobElectricians_CompanyName").val(electricianData[0].CompanyName);
                $("#JobElectricians_FirstName").val(electricianData[0].FirstName);
                $("#JobElectricians_LastName").val(electricianData[0].LastName);
                $("#JobElectricians_Email").val(electricianData[0].Email);
                $("#JobElectricians_Phone").val(electricianData[0].Phone);
                $("#JobElectricians_Mobile").val(electricianData[0].Mobile);
                if (electricianData[0].IsPostalAddress == true) {
                    HideShowAddressFieldOverAddressType(2, 'PDA', 'DPA');
                }
                else {
                    HideShowAddressFieldOverAddressType(1, 'PDA', 'DPA');
                    ChangeUnitTypeId('JobElectricians');
                }
                $('#JobElectricians_UnitNumber').val(electricianData[0].UnitNumber);
                $('#JobElectricians_UnitTypeID').val(electricianData[0].UnitTypeID == 0 ? "" : electricianData[0].UnitTypeID)
                $('#JobElectricians_StreetNumber').val(electricianData[0].StreetNumber);
                $('#JobElectricians_StreetName').val(electricianData[0].StreetName);
                $('#JobElectricians_StreetTypeID').val(electricianData[0].StreetTypeID);
                $('#JobElectricians_Town').val(electricianData[0].Town);
                $('#JobElectricians_State').val(electricianData[0].State);
                $('#JobElectricians_PostCode').val(electricianData[0].PostCode);
                $("#JobElectricians_PostalDeliveryNumber").val(electricianData[0].PostalDeliveryNumber);
                $('#JobElectricians_PostalAddressID').val(electricianData[0].PostalAddressID);

            }
            else {
                message = "Electrician has not been opened."
                //showErrorMessageInstallerDetails(message);
                showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        }
    });
}

function SaveElectrician(isCustomElectrician, isCreateNew) {
    var isValid = addressValidationRules('JobElectricians');
    if (isValid && $('#frmJobElectricians').valid()) {
        var data = JSON.stringify($('#frmJobElectricians').serializeToJson());
        var objData = JSON.parse(data);
        var obj = {};

        obj.jobElectricians = objData.JobElectricians;
        //Adding new electrician
        if (isCustomElectrician == 1) {
            obj.jobElectricians.InstallerID = 0;
            obj.jobElectricians.ElectricianID = 0;
        }
        if ($("#JobElectricians_AddressID").val() == 1)
            obj.jobElectricians.IsPostalAddress = 0;
        else
            obj.jobElectricians.IsPostalAddress = 1;

        obj.jobElectricians.SolarCompanyId = companyId;

        var dataElectrician = {};

        dataElectrician.jobId = BasicDetails_JobID;
        dataElectrician.profileType = 3;
        dataElectrician.jobElectricians = objData.JobElectricians
        dataElectrician.isCreateNew = isCreateNew
        $.ajax({
            url: urlJobElectricians,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            async: false,
            data: JSON.stringify(dataElectrician),
            success: function (response) {
                if (response.status) {
                    FillDropDownElectrician(urlGetElectricianListForJob + '?&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, $('#hdBasicDetails_ElectricianID').val(), $('#txtBasicDetails_ElectricianID'), $('#hdBasicDetails_ElectricianID'));
                    $('#popupElectrician').modal('hide');
                    showSuccessMessageForPopup("Job Electricians has been saved successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                    MakeEnableDisableGetElectricianSignature($('#hdBasicDetails_ElectricianID').val());
                    ShowHideEditIcon($('#hdBasicDetails_ElectricianID').attr('name'), $('#hdBasicDetails_ElectricianID').val());
                }
                else {
                    showErrorMessageForPopup(response.error, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
            },
            error: function () {
                showErrorMessageForPopup("Job Electricians has not been saved.");
            }
        });
    }
}

function ElectricianOnChange(id, isCustomElectrician) {
    $.ajax({
        url: urlUpdateJobDetailJobElectrician,
        data: { jobId: BasicDetails_JobID, solarCompanyId: company_Id, jobElectricianId: id, isCustomElectrician: isCustomElectrician },
        contentType: 'application/json',
        method: 'get',
        async: false,
        success: function (data) {
            if (data.status) {
                showSuccessMessageForPopup("Job Electricians has been saved successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                if ($('#hdBasicDetails_ElectricianID').val() == 0) { //&& isCustomElectrician != 1) { // Removing  electrician                      
                    showErrorMessageForPopup("Electrician has been removed.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
                ReloadSTCJobScreen(BasicDetails_JobID);
                MakeEnableDisableGetElectricianSignature($('#hdBasicDetails_ElectricianID').val());
                ShowHideEditIcon($('#hdBasicDetails_ElectricianID').attr('name'), $('#hdBasicDetails_ElectricianID').val());
            }
        },
        error: function () {
            showErrorMessageInstallerDetails("Job Electricians has not been saved.");
        }
    });
}

function clearPopupElectrician() {
    $(".popupElectrician").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $(".popupElectrician").find('.field-validation-error').attr('class', 'field-validation-valid');
    $('#JobElectricians_InstallerID').val(0);
    $('#JobElectricians_ElectricianID').val(0);
    $("#JobElectricians_UnitTypeID").val($("#JobElectricians_UnitTypeID option:first").val());
    $("#JobElectricians_StreetTypeID").val($("#JobElectricians_StreetTypeID option:first").val());
    $("#JobElectricians_PostalAddressID").val($("#JobElectricians_PostalAddressID option:first").val());
}

function deleteCustomElectrician(jobElectricianId, jobId) {
    if (confirm("Are you sure you want to delete electrician.")) {
        $.ajax({
            url: urlDeleteCustomElectrician,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            data: JSON.stringify({ jobElectricianId: jobElectricianId, jobId: jobId }),
            async: false,
            success: function (response) {
                message = "Electrician has been deleted."
                showSuccessMessageForPopup(message, $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                FillDropDownElectrician(urlGetElectricianListForJob + '?&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&jobId=' + BasicDetails_JobID, null, $('#txtBasicDetails_ElectricianID'), $('#hdBasicDetails_ElectricianID'), true);
                return false;
            },
            error: function () {
                message = "Electrician has been not deleted."
                showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        });
    }
    return false;
}

function checkExistingCustomElectrician() {
    var fullName = $('#JobElectricians_FirstName').val() + " " + $('#JobElectricians_LastName').val();
    jobId = BasicDetails_JobID;
    solarCompanyId = company_Id;
    $.ajax({
        url: urlCheckExistingCustomElectrician,
        data: JSON.stringify({ fullName: fullName, jobId: jobId, solarCompanyID: parseInt(solarCompanyId) }),
        contentType: 'application/json',
        method: 'Post',
        aync: false,
        success: function (response) {
            if (response.status) {
                var isCreate = false;
                if (response.isExist) {
                    if (confirm("Electrician with same First name and Last name already exists, do you want to create a new electrician with same name. ")) {
                        isCreate = true;
                    }
                }
                if (!response.isExist || isCreate) {
                    SaveElectrician(1, true);
                }
            }
            else {
                showErrorMessageForPopup(response.error, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        }
    });
}

function FillDropDownSWHInstaller(url, objSWHInstaller, objtxtSWHInstaller, objhdSWHInstaller) {
    objtxtSWHInstaller.val("");
    $.ajax({
        url: url,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        success: function (success) {
            swhInstallerList = [];
            $.each(success, function (i, SWHInstaller) {
                swhInstallerList.push({ value: SWHInstaller.Value, text: SWHInstaller.Text, issystemuser: SWHInstaller.IsSystemUser, userid: SWHInstaller.UserId });
            });

            if (objSWHInstaller != '') {
                objhdSWHInstaller.val(objSWHInstaller);
                $.each(swhInstallerList, function (key, value) {
                    if (value.value == objSWHInstaller) {
                        objtxtSWHInstaller.val(value.text);
                    }
                });
            }
            else {
                objhdSWHInstaller.val('');
                objtxtSWHInstaller.val('');
            }
        }
    });
}

function SWHInstallerAutocomplete(objtxtSWHInstaller, objhdSWHInstaller) {
    objtxtSWHInstaller.autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];

            objhdSWHInstaller.val(0);
            $.each(swhInstallerList, function (key, value) {
                if (value.text.toLowerCase().indexOf(objtxtSWHInstaller.val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value, issystemuser: value.issystemuser, userid: value.userid });
                }
            });
            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id, issystemuser: item.issystemuser, userid: item.userid };
            }))
        },
        select: function (event, ui) {
            objhdSWHInstaller.val(ui.item.id); // save selected id to hidden input
            objtxtSWHInstaller.val(ui.item.value);
            objtxtSWHInstaller.attr('isSystemUser', ui.item.issystemuser);
            SWHInstallerOnChange(objtxtSWHInstaller, objhdSWHInstaller, ui.item.issystemuser, ui.item.userid);
        }
    }).bind('focus', function () {
        if (!$(this).val().trim())
            $(this).keydown();

    });
    objtxtSWHInstaller.autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        var color = '';
        if (item.issystemuser)
            color = 'green';
        else
            color = 'grey';
        var atag = "<a style=color:" + color + ">" + t + "</a>";

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append(atag)
            .appendTo(ul);
    };
}

function SWHInstallerOnChange(objtxtSWHInstaller, objhdSWHInstaller, isSytemUser, userId) {
    $.ajax({
        url: urlUpdateJobDetailInstallerDesigner,
        contentType: 'application/json',
        dataType: 'json',
        method: 'get',
        cache: false,
        async: false,
        data: { installerDesignerId: objhdSWHInstaller.val(), jobId: BasicDetails_JobID, profileType: 0, IsSWHUser: true, userId: userId },
        success: function (data) {
            //Hiding error/success msg region 
            $('#successMsgRegionInstallerDetails').hide();
            $('#errorMsgRegionInstallerDetails').hide();
            if (data.status) {
                if (isSytemUser) {
                    if (data.SEStatus != 2) {
                        alert("Selected installer is a Greenbot user but not a part of solar company, so visits can't be scheduled.");
                    }
                    var isVisitExist = $(".visit-id:visible").length;
                    isVisitExist==0 && data.SEStatus == 2 && (ProjectSession_UserTypeId != 8 || isScheduleAnInstaller.toLowerCase() == 'true') ? addQuickVisit(objhdSWHInstaller.val(), objtxtSWHInstaller.val(), data.SEStatus, "Installer has been added successfully.Do you want to schedule visit?") : ShowHideVisitScheduledIcon(true, data.SEStatus == 2 ? true : false, data.SEStatus);
                    ShowHideSystemUserIcon(true, true);
                }
                else {
                    ShowHideSystemUserIcon(true, false);
                    ShowHideVisitScheduledIcon(false, false, 1);     //1 for SolarElectrician Status as Pending
                }
                if (!isSytemUser)
                    showSuccessMessageForPopup("SWH Installer has been added successfully.", $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));

                if (objhdSWHInstaller.val() == 0) { // Removing SWH installer 
                    ShowHideSystemUserIcon(true, false);
                    ShowHideVisitScheduledIcon(false, false, 1);     //1 for SolarElectrician Status as Pending
                    showErrorMessageForPopup("SWH Installer has been removed.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
                }
                //For sending mail/sms for signature
                $('#JobInstallerDetails_FirstName').val(data.FirstName);
                $('#JobInstallerDetails_Surname').val(data.LastName);
                $('#JobInstallerDetails_Email').val(data.Email);
                $('#JobInstallerDetails_Mobile').val(data.Mobile);
                MakeEnableDisableGetInstallerSignature(objhdSWHInstaller.val());
                ShowHideEditIcon(objhdSWHInstaller.attr('name'), objhdSWHInstaller.val());
            }
        },
        error: function () {
            //showErrorMessageInstallerDetails("SWH Installer has not been added.");
            showErrorMessageForPopup("SWH Installer has not been added.", $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
        }
    });
}

function checkExistSWHInstaller(obj) {
    var fieldName = "SearchGreenbotSWHInstaller_LicenseNumber";//obj.id;
    var chkvar = '';
    var licenseNumber = obj;//obj.value;
    var sID = 0;
    sID = company_Id;

    $("#errorMsgRegionSWHInstallerPopup").hide();
    $("#successMsgRegionSWHInstallerPopup").hide();

    if (licenseNumber != "" && licenseNumber != undefined) {
        $.ajax({
            url: CheckSWHUserExistURL,
            data: { FieldName: fieldName, userId: null, resellerID: null, solarCompanyId: parseInt(sID), LicenseNumber: licenseNumber },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {
                // clearing data table
                $('.mulitpleSWHInstallerNote').hide();
                $('#datatableSWHInstaller').dataTable().fnClearTable();
                if (data.status == false) {
                    var errorMsg;
                    errorMsg = data.message;
                    $(".alert").hide();
                    $("#errorMsgRegionSWHInstallerPopup").html(closeButton + errorMsg);
                    $("#errorMsgRegionSWHInstallerPopup").show();
                }
                else {
                    var swhData = JSON.parse(data.accreditedData);
                    var swhList = [];

                    if (swhData.length > 1) {
                        $('.mulitpleSWHInstallerNote').show();
                        $('.mulitpleSWHInstallerNote').html("There are mulitple users with same license number.Please add appropriate one.");
                    }
                    else {
                        $('.mulitpleSWHInstallerNote').hide();
                    }
                    $.each(swhData, function (key, value) {
                        swhList.push({
                            AccreditedInstallerId: value.AccreditedInstallerId
                            , FullName: value.FullName
                            , CECAccreditationNumber: value.AccreditationNumber
                            , IsPVDUser: value.IsPVDUser
                            , IsSWHUser: value.IsSWHUser
                            , FullAddress: value.FullAddress
                            , Phone: value.Phone
                            , Email: value.Email
                            , IsSystemUser: value.IsSystemUser
                            , UserId: value.UserId
                            , AddressID: value.IsPostalAddress
                            , UnitTypeID: value.UnitTypeID
                            , UnitNumber: value.UnitNumber
                            , StreetNumber: value.StreetNumber
                            , StreetName: value.StreetName
                            , StreetTypeID: value.StreetTypeID
                            , Town: value.Town
                            , State: value.State
                            , PostCode: value.PostCode
                            , IsPostalAddress: value.IsPostalAddress
                            , FirstName: value.FirstName
                            , LastName: value.Surname
                            ,Mobile:value.Mobile
                        });
                    });

                    var swhTable = $('#datatableSWHInstaller').DataTable({
                        bDestroy: true,
                        iDisplayLength: 10,
                        columns: [
                            {
                                'data': 'Id',
                                "render": function (data, type, full, meta) {
                                    full.StreetNumber = (full.StreetNumber == '' || full.StreetNumber == null) ? 0 : full.StreetNumber;
                                    full.UnitNumber = (full.UnitNumber == '' || full.UnitNumber == null) ? 0 : full.UnitNumber;
                                    full.UnitTypeID = (full.UnitTypeID > 0) ? full.UnitTypeID : 0;
                                    full.StreetTypeID = full.StreetTypeID > 0 ? full.StreetTypeID : 0;
                                    full.StreetName = full.StreetName == '' || full.StreetName == null ? null : full.StreetName;
                                    full.Town = full.Town == '' || full.Town == null ? null : full.Town;
                                    full.State = full.State == '' || full.State == null ? null : full.State;
                                    full.PostCode = full.PostCode == '' || full.PostCode == null ? null : full.PostCode;
                                    full.FirstName = full.FirstName == '' || full.FirstName == null ? null : full.FirstName;
                                    full.LastName = full.LastName == '' || full.LastName == null ? null : full.LastName;
                                    full.Mobile = full.Mobile == '' || full.Mobile == null ? null : full.Mobile;
                                    full.Phone = full.Phone == '' || full.Phone == null ? null : full.Phone;
                                    full.Email = full.Email == '' || full.Email == null ? null : full.Email;
                                    full.PostalDeliveryNumber = full.PostalDeliveryNumber == '' || full.PostalDeliveryNumber == null ? null : full.PostalDeliveryNumber;
                                    full.PostalAddressID = full.PostalAddressID >0 ?  full.PostalAddressID :0;
                                    return '<input type="radio" name="swhInstaller" class="rbSWHInstaller" accreditedInstallerId="' + full.AccreditedInstallerId + '" IsPostalAddress="' + full.IsPostalAddress +
                                        '"  userId="' + full.UserId + '"  email="' + full.Email + '" UnitTypeID="' + full.UnitTypeID + '" UnitNumber="' + full.UnitNumber + '" StreetName="' + full.StreetName + '" StreetTypeID="' + full.StreetTypeID +
                                        '" StreetNumber="' + full.StreetNumber + '" Town="' + full.Town + '" State="' + full.State + '" PostCode="' + full.PostCode + '" isSystemUser="' + full.IsSystemUser + '" FirstName="' +
                                        full.FirstName + '" LastName="' + full.LastName + '" Mobile="' + full.Mobile + '" Phone="' + full.Phone + '" PostalDeliveryNumber="' + full.PostalDeliveryNumber + '" PostalAddressID="' + full.PostalAddressID + '" >';
                                }
                            },
                            { 'data': 'FullName' },
                            { 'data': 'CECAccreditationNumber' },
                            { 'data': 'FullAddress' },
                            { 'data': 'Phone' },
                            { 'data': 'Email' },
                        ],
                        dom: "<<'table-responsive tbfix't>>",
                        createdRow: function (row, data, dataIndex) {
                            if (data.IsSystemUser == true) {
                                $(row.cells).css("color", "green");
                            }
                        }
                    });
                    swhTable.rows.add(swhList).draw();
                    $('#datatableSWHInstaller').show();
                    $('#swhInstallerList').show();
                }
            }
        });
    }
    else {
        alert("Please enter license number");
        return false;
    }
}

function FillSWHInstaller(id, jobType) {
    $.ajax({
        url: GetElectricianDetailbySolarcompanyURL,
        data: { Id: id, JobType: jobType },
        contentType: 'application/json',
        method: 'get',
        success: function (data) {
            if (data != false) {
                var obj = JSON.parse(data);
                if (obj[0].SESignature != null && obj[0].SESignature != '') {
                    var SignName = obj[0].SESignature;
                    var guid = ProjectSession_LoggedInUserId;
                    var proofDocumentURL = ProjectSession_UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignSWHInstall = imagePath + "/" + SignName;
                    $('#imgSignSWHInstall').attr('class', SignName);
                    $('#imgSWHSignatureInstaller').show();
                }
                else
                    $('#imgSWHSignatureInstaller').hide();

                $('#JobInstallerDetails_FirstName').val(obj[0].FirstName);
                $('#JobInstallerDetails_Surname').val(obj[0].LastName);
                $('#JobInstallerDetails_Email').val(obj[0].Email);
                $('#JobInstallerDetails_Phone').val(obj[0].Phone);
                $('#JobInstallerDetails_Mobile').val(obj[0].Mobile);
                $('#JobInstallerDetails_LicenseNumber').val(obj[0].ElectricalContractorsLicenseNumber);
                $("#JobInstallerDetails_LicenseNumber").attr("readonly", "readonly");
                $('#JobInstallerDetails_LicenseNumber').attr('onblur', '')
                if (obj[0].IsPostalAddress.toLowerCase() == 'true') {
                    HideShowAddressFieldOverAddressType(2, 'PDA', 'DPA');
                }
                else {
                    HideShowAddressFieldOverAddressType(1, 'PDA', 'DPA');
                }
                $('#JobInstallerDetails_UnitTypeID').val(obj[0].UnitTypeID == 0 ? "" : obj[0].UnitTypeID);
                $('#JobInstallerDetails_UnitNumber').val(obj[0].UnitNumber);
                $('#JobInstallerDetails_StreetNumber').val(obj[0].StreetNumber);
                $('#JobInstallerDetails_StreetName').val(obj[0].StreetName);
                $('#JobInstallerDetails_StreetTypeID').val(obj[0].StreetTypeID);
                $('#JobInstallerDetails_Town').val(obj[0].Town);
                $('#JobInstallerDetails_State').val(obj[0].State);
                $('#JobInstallerDetails_PostCode').val(obj[0].PostCode);
                $("#JobInstallerDetails_PostalDeliveryNumber").val(obj[0].PostalDeliveryNumber);
                $('#JobInstallerDetails_PostalAddressID').val(obj[0].PostalAddressID);
                $('#JobInstallerDetails_AccreditationNumber').val(obj[0].CECAccreditationNumber);

            }
            else {
                message = "SWH Installer has not been opened."
                showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
            }
        },
        error: function () {
            message = "SWH Installer has not been opened."
            showErrorMessageForPopup(message, $("#errorMsgRegionInstallerDetails"), $("#successMsgRegionInstallerDetails"));
        }
    });
}

function SaveSWHInstaller(isEditSWHInstaller, isAddCustomSWHInstaller) {
    var obj = {};
    var isFormValid = false;
    var isValidLocation = true;
    if (isEditSWHInstaller || isAddCustomSWHInstaller) {
        var isValid = addressValidationRules("JobInstallerDetails");
        if (isValid && $("#frmSWHInstaller").valid()) {
            var data = JSON.stringify($('#frmSWHInstaller').serializeToJson());
            var objData = JSON.parse(data);
            var swhInstallerdata = {};
            swhInstallerdata = objData.JobInstallerDetails;
            swhInstallerdata.InstallerDesignerId = isAddCustomSWHInstaller ? 0 : $('#hdBasicDetails_SWHInstallerID').val();
            swhInstallerdata.SESignature = $("#imgSignSWHInstall").attr('class');
            swhInstallerdata.LastName = $('#JobInstallerDetails_Surname').val();
            swhInstallerdata.ElectricalContractorsLicenseNumber = $('#JobInstallerDetails_LicenseNumber').val();
            swhInstallerdata.CECAccreditationNumber = isAddCustomSWHInstaller ? "" : $('#JobInstallerDetails_AccreditationNumber').val();
            swhInstallerdata.SEDesignRoleId = 0;
            swhInstallerdata.SolarCompanyId = companyId;
            swhInstallerdata.CreatedBy = ProjectSession_LoggedInUserId;
            swhInstallerdata.IsSWHUser = true;
            obj.installerDesignerView = swhInstallerdata;
            obj.signPath = SRCSignSWHInstall;
            obj.accreditedInstallerId = 1;
            obj.jobId = BasicDetails_JobID;
            isFormValid = true;
        }
    }
    else {
        if ($('#SearchGreenbotSWHInstaller_LicenseNumber').is('visible') && $('#SearchGreenbotSWHInstaller_LicenseNumber').val() == "") {
            alert("Please enter license number");
            return false;
        }
        if ($("input[name='swhInstaller']:checked").length == 0 || $("input[name='swhInstaller']:checked").length > 1 ) {
            alert("Please select one record to add installer.");
            return false;
        }
        else if ($("input[name='swhInstaller']:checked").length == 1) {
            obj.installerDesignerView = {};
            obj.installerDesignerView.SolarCompanyId = companyId;
            obj.installerDesignerView.ElectricalContractorsLicenseNumber = $('#SearchGreenbotSWHInstaller_LicenseNumber').val();
            obj.installerDesignerView.Email = $("input[name='swhInstaller']:checked").attr('email');
            obj.accreditedInstallerId = isAddCustomSWHInstaller ? 1 : $("input[name='swhInstaller']:checked").attr('accreditedinstallerid');
            obj.userId = $("input[name='swhInstaller']:checked").attr('userid');
            obj.installerDesignerView.UnitNumber = $("input[name='swhInstaller']:checked").attr('unitnumber') == '0' ? null : $("input[name='swhInstaller']:checked").attr('unitnumber');
            obj.installerDesignerView.StreetNumber = $("input[name='swhInstaller']:checked").attr('streetnumber') == '0' ? null : $("input[name='swhInstaller']:checked").attr('streetnumber');
            obj.installerDesignerView.StreetName = $("input[name='swhInstaller']:checked").attr('streetname') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('streetname') ;
            obj.installerDesignerView.UnitTypeID = $("input[name='swhInstaller']:checked").attr('unittypeid')=='0'?null: $("input[name='swhInstaller']:checked").attr('unittypeid');
            obj.installerDesignerView.StreetTypeID = $("input[name='swhInstaller']:checked").attr('streettypeid')=='0'?null: $("input[name='swhInstaller']:checked").attr('streettypeid');
            obj.installerDesignerView.Town = $("input[name='swhInstaller']:checked").attr('town') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('town');
            obj.installerDesignerView.State = $("input[name='swhInstaller']:checked").attr('state') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('state');
            obj.installerDesignerView.PostCode = $("input[name='swhInstaller']:checked").attr('postcode') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('postcode');
            obj.installerDesignerView.IsPostalAddress = $("input[name='swhInstaller']:checked").attr('ispostaladdress');
            if (obj.installerDesignerView.IsPostalAddress == 'true')
                obj.installerDesignerView.AddressID = 2;
            else
                obj.installerDesignerView.AddressID = 1;
            obj.installerDesignerView.FirstName = $("input[name='swhInstaller']:checked").attr('firstname') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('firstname');
            obj.installerDesignerView.LastName = $("input[name='swhInstaller']:checked").attr('lastname') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('lastname');
            obj.installerDesignerView.Phone = $("input[name='swhInstaller']:checked").attr('phone') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('phone');
            obj.installerDesignerView.Mobile = $("input[name='swhInstaller']:checked").attr('mobile') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('mobile') ;
            obj.installerDesignerView.PostalDeliveryNumber = $("input[name='swhInstaller']:checked").attr('PostalDeliveryNumber') == 'null' ? null : $("input[name='swhInstaller']:checked").attr('PostalDeliveryNumber');
            obj.installerDesignerView.PostalAddressID = ($("input[name='swhInstaller']:checked").attr('PostalAddressID')) == '0'? null :$("input[name='swhInstaller']:checked").attr('PostalAddressID');
            
            $('#JobInstallerDetails_FirstName').val(obj.installerDesignerView.FirstName == 'null' ? '' : obj.installerDesignerView.FirstName);
            $('#JobInstallerDetails_Surname').val(obj.installerDesignerView.LastName == 'null' ? '' : obj.installerDesignerView.LastName);
            $('#JobInstallerDetails_Email').val(obj.installerDesignerView.Email == 'null' ? '' : obj.installerDesignerView.Email);
            $('#JobInstallerDetails_Phone').val(obj.installerDesignerView.Phone == 'null' ? '' : obj.installerDesignerView.Phone);
            $('#JobInstallerDetails_Mobile').val(obj.installerDesignerView.Mobile == 'null' ? '' : obj.installerDesignerView.Mobile);
            $('#JobInstallerDetails_LicenseNumber').val(obj.installerDesignerView.ElectricalContractorsLicenseNumber);
            $("#JobInstallerDetails_LicenseNumber").attr("readonly", "readonly");
            $('#JobInstallerDetails_LicenseNumber').attr('onblur', '')
            if (obj.installerDesignerView.IsPostalAddress == 'true') {
                $('#JobInstallerDetails_AddressID').val(2);
                HideShowAddressFieldOverAddressType(2, 'PDA', 'DPA');
            }
            else {
                $('#JobInstallerDetails_AddressID').val(1);
                HideShowAddressFieldOverAddressType(1, 'PDA', 'DPA');
            }
            $('#JobInstallerDetails_UnitTypeID').val(obj.installerDesignerView.UnitTypeID == null ? '' : obj.installerDesignerView.UnitTypeID);
            $('#JobInstallerDetails_UnitNumber').val(obj.installerDesignerView.UnitNumber == null ? '' : obj.installerDesignerView.UnitNumber);
            $('#JobInstallerDetails_StreetNumber').val(obj.installerDesignerView.StreetNumber == null ? '' : obj.installerDesignerView.StreetNumber);
            $('#JobInstallerDetails_StreetName').val(obj.installerDesignerView.StreetName == null ? '' : obj.installerDesignerView.StreetName);
            $('#JobInstallerDetails_StreetTypeID').val(obj.installerDesignerView.StreetTypeID == null ? '' : obj.installerDesignerView.StreetTypeID);
            $('#JobInstallerDetails_Town').val(obj.installerDesignerView.Town == null ? '' : obj.installerDesignerView.Town);
            $('#JobInstallerDetails_State').val(obj.installerDesignerView.State == null ? '' : obj.installerDesignerView.State);
            $('#JobInstallerDetails_PostCode').val(obj.installerDesignerView.PostCode == null ? '' : obj.installerDesignerView.PostCode);
            $("#JobInstallerDetails_PostalDeliveryNumber").val(obj.installerDesignerView.PostalDeliveryNumber == null ? '' : obj.installerDesignerView.PostalDeliveryNumber);
            $('#JobInstallerDetails_PostalAddressID').val(obj.installerDesignerView.PostalAddressID == null ? '' : obj.installerDesignerView.PostalAddressID);
            addressValidationRules("JobInstallerDetails");
            
            var Town = obj.installerDesignerView.Town;
            var State = obj.installerDesignerView.State;
            var PostCode = obj.installerDesignerView.PostCode;
            if ((Town != null) && (State != null) && (PostCode != null)) {
                $.ajax({
                    type: 'GET',
                    url: actionProcessRequest,
                    dataType: 'json',
                    data: {
                        excludePostBoxFlag: true,
                        q: Town.substring(0, 3)
                    },
                    async: false,
                    success: function (data) {
                        var data1 = JSON.parse(data);
                        if (data1.localities.locality != undefined && data1.localities.locality != null) {
                            if (data1.localities.locality.length > 0) {
                                $.each(data1.localities.locality, function () {
                                    isValidLocation = false;
                                    if (this.location.toLowerCase() == Town.toLowerCase() &&
                                        this.state.toLowerCase() == State.toLowerCase() &&
                                        this.postcode == PostCode
                                    ) {
                                        isValidLocation = true;
                                        return false;
                                    }
                                });
                            }
                            else if (data1.localities.locality.location != undefined && data1.localities.locality.location != null) {
                                var obj = data1.localities.locality;
                                if (obj.location.toLowerCase() == Town.toLowerCase() &&
                                    obj.state.toLowerCase() == State.toLowerCase() &&
                                    obj.postcode == PostCode
                                ) {
                                    isValidLocation = true;
                                    return false;
                                }
                            }
                            else
                                isValidLocation = false;
                        }
                        else
                            isValidLocation = false;

                        if (!isValidLocation) {
                            $('.FindSWHInstaller').hide();
                            $('.SearchGreenbotSWHInstaller').hide();
                            $('.AddEditSWHInstaller').show();
                            $('#btnAddSWHInstaller').attr('onclick', ' SaveSWHInstaller(false,true);');
                            $('#btnAddUpdateSWHInstaller').attr('onclick', ' SaveSWHInstaller(false,true);');
                            showErrorMessageForPopup("Please enter valid state, town and postcode.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                        }
                        else {
                            $("#errorMsgRegionSWHInstallerPopup").hide();
                        }
                    }
                })
            }
           
            isFormValid = true;
        }
        
    }
    obj.profileType = 3;
    obj.jobElectricians = null;
    obj.isSWHInstaller = true;

    if (isFormValid && isValidLocation) {
        $.ajax({
            url: AddInstallerDesignerURL,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            type: 'post',
            data: JSON.stringify(obj),
            success: function (response) {
                if (response.status) {
                    if (response.InstallerDesignerId > 0) {
                        FillDropDownSWHInstaller(GetElectricianURL + '?JobID=' + $("#BasicDetails_JobID").val() + '&solarCompanyId=' + $("#BasicDetails_SolarCompanyId").val() + '&JobType=' + $("#BasicDetails_JobType").val(), $('#hdBasicDetails_SWHInstallerID').val(), $('#txtBasicDetails_SWHInstallerID'), $('#hdBasicDetails_SWHInstallerID'));
                    }
                    $('#popupSWHInstaller').modal('hide');
                    showSuccessMessageForPopup(response.message, $("#successMsgRegionInstallerDetails"), $("#errorMsgRegionInstallerDetails"));
                }
                else {
                    $('.FindSWHInstaller').hide();
                    $('.SearchGreenbotSWHInstaller').hide();
                    $('.AddEditSWHInstaller').show();
                    $('#btnAddSWHInstaller').attr('onclick', ' SaveSWHInstaller(false,true);');
                    $('#btnAddUpdateSWHInstaller').attr('onclick', ' SaveSWHInstaller(false,true);');
                    showErrorMessageForPopup(response.message, $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
                }
            },
            error: function () {
                showErrorMessageForPopup("SWH installer has not been saved.", $("#errorMsgRegionSWHInstallerPopup"), $("#successMsgRegionSWHInstallerPopup"));
            }
        });
    }
}

function ClearSearchGreenbotSWHInstaller() {
    $("#SearchGreenbotSWHInstaller_LicenseNumber").val("");
    $('.mulitpleSWHInstallerNote').hide();
    $('#datatableSWHInstaller').dataTable().fnClearTable();
    $('#swhInstallerList').hide();
    //Hiding error/success msg region 
    $("#errorMsgRegionSWHInstallerPopup").hide();
    $("#successMsgRegionSWHInstallerPopup").hide();
}

function ClearPopupSWHInstaller(isFindInstaller) {
    $(".popupSWHInstaller").find('input[type=text]').each(function () {
        if ($(this).attr('idclass') == "swhInstallerNotRequired") {
            $(this).val('');
        }
        if (isFindInstaller) {
            if ($(this).attr('idclass') == "swhInstallerRequired") {
                $(this).val('');
            }
        }
        $(this).attr('class', 'form-control valid');
    });
    $(".popupSWHInstaller").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#JobInstallerDetails_UnitTypeID").val($("#JobInstallerDetails_UnitTypeID option:first").val());
    $("#JobInstallerDetails_StreetTypeID").val($("#JobInstallerDetails_StreetTypeID option:first").val());
    $("#JobInstallerDetails_PostalAddressID").val($("#JobInstallerDetails_PostalAddressID option:first").val());
    $("#JobInstallerDetails_SESignature").val('');
    $("#imgSWHSignatureInstaller").hide();
    //Hiding error/success msg region 
    $("#errorMsgRegionSWHInstallerPopup").hide();
    $("#successMsgRegionSWHInstallerPopup").hide();
}

function RemoveInstallerDesignerElectrician(objTxt, objHd) {
    if (objHd.val() == 0) {
        if (objHd.attr('name') == "Electrician") {
            clearPopupElectrician();
            ElectricianOnChange(objHd.val(), objTxt.attr('isCustomElectrician'))
        }
        else {
            if (objHd.attr('name') == "pvdInstaller") {
                InstallerDesignerOnChange(objTxt, objHd, true, false);
            }
            else if (objHd.attr('name') == "Designer") {
                InstallerDesignerOnChange(objTxt, objHd, false, false);
            }
            else if (objHd.attr('name') == "swhInstaller") {
                SWHInstallerOnChange(objTxt, objHd, false);
            }
        }
        objTxt.val('');
    }
}

function ShowHideSystemUserIcon(isInstaller, isActive) {    
    if (isInstaller) {
        if (isActive) {
            $('#btnInstaller_SystemUser').find('img').attr('src', ProjectImagePath + 'Images/system-user-a.svg');
            $('#btnInstaller_SystemUser').attr('title', 'Is a Greenbot user, this user will be able to receive visits and view job details if scheduled.');
        }
        else {
            $('#btnInstaller_SystemUser').find('img').attr('src', ProjectImagePath + 'Images/system-user.svg');
            $('#btnInstaller_SystemUser').attr('title', ' Is not a registered Greenbot user, job details are not visible to this person.');
        }
    }
    else {
        if (isActive) {
            $('#btnDesigner_SystemUser').find('img').attr('src', ProjectImagePath + 'Images/system-user-a.svg')
            $('#btnDesigner_SystemUser').attr('title', 'Is a Greenbot user.');
        }
        else {
            $('#btnDesigner_SystemUser').find('img').attr('src', ProjectImagePath + 'Images/system-user.svg');
            $('#btnDesigner_SystemUser').attr('title', ' Is not a registered Greenbot user.');
        }
    }
}

function ShowHideVisitScheduledIcon(isSystemUser, isVisitScheduled, SEStatus) {
    if (isSystemUser) {
        if (isVisitScheduled) {
            $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/scheduled-visit.svg');
            $('#btnInstallerQuickVisit').attr('title', 'This installer has been scheduled a visit and can see this job data');
            $('#btnInstallerQuickVisit').removeAttr('onclick');
        }
        else {
            $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
            $('#btnInstallerQuickVisit').attr('title', ' Job data cannot be seen by this user because no scheduled visit has been created.');
            $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_InstallerID > 0 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + SEStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + SEStatus + ');');
        }
    }
    else {
        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
        $('#btnInstallerQuickVisit').attr('title', ' Job data cannot be seen by this user because no scheduled visit has been created.');
        $('#btnInstallerQuickVisit').removeAttr('onclick');
    }
    if ($("#btnAddVisit:visible").length == 0) {
        $(".assign-installer").hide();
        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
        $('#btnInstallerQuickVisit').removeAttr('onclick');
    }
    else {
        $(".assign-installer").show();
        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
        $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_InstallerID > 0 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + SEStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + SEStatus + ');');
    }
}

function ShowHideEditIcon(attrName, id) {
    if (id > 0) {
        if (attrName.toLowerCase() == 'pvdinstaller') {
            $('#btnUpdateInstaller').show();
        }
        else if (attrName.toLowerCase() == 'designer') {
            $('#btnUpdateDesigner').show();
        }
        else if (attrName.toLowerCase() == 'electrician') {
            $('#btnUpdateElectrician').show();
        }
        else if (attrName.toLowerCase() == 'swhinstaller') {
            $('#btnUpdateSWHInstaller').show();
        }
    }
    else {
        if (attrName.toLowerCase() == 'pvdinstaller') {
            $('#btnUpdateInstaller').hide();
        }
        else if (attrName.toLowerCase() == 'designer') {
            $('#btnUpdateDesigner').hide();
        }
        else if (attrName.toLowerCase() == 'electrician') {
            $('#btnUpdateElectrician').hide();
        }
        else if (attrName.toLowerCase() == 'swhinstaller') {
            $('#btnUpdateSWHInstaller').hide();
        }
    }
}

function UpdateInstaller(installerId) {
    event.preventDefault();
    FillInstallerDesigner(installerId, true);
    //Hiding error/success msg region 
    $("#errorMsgRegionInstallerDesignerPopup").hide();
    $("#successMsgRegionInstallerDesignerPopup").hide();
    $("#InstallerDesignerView_LocationValidation").hide();
    $('.FindInstaller').hide();
    $('#popupInstallerDesigner').modal({ backdrop: 'static', keyboard: false });
    $("#popupInstallerDesigner").find('input[type=text]').each(function () {
        $(this).attr('class', 'form-control valid');
    });
    $("#popupInstallerDesigner").find('.field-validation-error').attr('class', 'field-validation-valid');
    $('.AddEditInstaller').show();
}
function getJobRetailerSettingDataForJobScreen(jobid) {
    debugger;
    $.ajax({
        url: urlGetJobRetailerSettingDataByJobId,
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        data: { JobId: jobid },
        cache: false,
        success: function (result) {
            debugger;
            if (result.success) {
                //var modelRetailerSignURL = "UserDocuments/" + result.data.JobRetailerUserId + "/" + result.data.Signature;
                //if (result.data.Signature != null && result.data.Signature != '' && result.data.Signature != undefined) {
                //    $("#imgJobRetailerSignatureJobDetailScreen").attr('src', $("#imgJobRetailerSignature").attr('src'));
                //}
                //$("#RepresentativeName").val(result.data.RepresentativeName);
                //$("#JobRetailerPositionHeldlbl").val(result.data.PositionHeldlbl);
                $("#statement").html(result.data.Statement);
                //$("#SignedBy").text(result.data.SignedBy);
                //$("#SignedDate").text(result.data.SignedDate);
                //$("#JobRetailer_Latitude").text(result.data.Latitude);
                //$("#JobRetailer_Longitude").text(result.data.Longitude);

                // showSuccessMessageForJobAutoSign("Solar Representative data updated successfully.");


                //ReloadSCAWrittenStatement(JobId, SolarCompanyId, SolarCompanyName);
            }

            else {
                showErrorMessageForJobAutoSign("Something wrong happen");
            }

        }
    });
}

$('#installerGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetinstallerSignatureSelfie').modal('show');
});
$('#designerGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetdesignerSignatureSelfie').modal('show');
});
$('#elecGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetelectricianSignatureSelfie').modal('show');
});