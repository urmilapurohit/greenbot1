$(document).ready(function () {
    debugger;
    $("input[name='InstallerUserType'][value='0']").prop('checked', true);
    $('.showSWH').hide();
    $('input[type=radio][name=InstallerUserType]').change(function () {
        $("#UserDetails").find('input[type=text]').each(function () {
            $(this).val('');
        });
        ShowHidePVD();
    });

    if (userTypeId == 7) {
        $('#AutoRequestSwitch').prop('checked', ($('#IsAutoRequest').val().toLowerCase() == "true" ? true : false));
    }
    if (sessionUserTypeId == 1 && userTypeId == 2) {
        $("#RecUsernamePwd").show();
        $("#RecSuperadmin").show();
    }
    else if (sessionUserTypeId == 2) {
        $("#RecUsernamePwd").show();
        $("#RecSuperadmin").hide();
    }

    if ((sessionUserTypeId == 2 || sessionUserTypeId == 1) && userTypeId == 4 && modelWholeSaler == "False") {
        $(".btnCheckInXero").show();
    }
    else {
        $(".btnCheckInXero").hide();
    }
    if (userTypeId == 4) {
        if (SCisAllowedSPV == null || SCisAllowedSPV == '') {
            $("#onOffSwitchSPV").prop('checked', false);
        } else {
            $("#onOffSwitchSPV").prop('checked', (SCisAllowedSPV.toString().toLowerCase() == 'true' ? true : false));
        }
    }
    //if (userTypeId == 2 && sessionUserTypeId == 1) {
    //    if (modelIsAllowedMasking == null || modelIsAllowedMasking == '') {
    //        $("#onOffSwitchMasking").prop('checked', false);
    //    } else {
    //        $("#onOffSwitchMasking").prop('checked', (modelIsAllowedMasking.toString().toLowerCase() == 'true' ? true : false));
    //    }
    //}
    $("#searchRAM").change(function () {
        GetClientNumberofSCAOnRAMChange();
        if ($("#hdnRAMID").val() == '')
            $("#RAMId").val(0);
        else
            $("#RAMId").val($("#hdnRAMID").val());
    });
    $("#CompanyNameRefresh").click(function () {
        ChangeCompanyABN($('#CompanyABN').val(), $('#CompanyName'), false);
        //$("a").addClass("fa fa-refresh fa-spin");
        $("#CompanyNameRefresh i").addClass("fa-spin");

    });

    $("#CompanyName").change(function () {
        $.ajax({
            url: getEntityNameURL,
            type: 'GET',
            data: { companyABN: $("#CompanyABN").val() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                if (response.success) {
                    $("#EntityName").val(response.data)
                }
            },
            error: function (response) {
                alert(response);
            }
        });
    });

    if (userTypeId == 4 && pageType != 'profile' && pageType != 'viewdetail') {
        autoCompleteRAM(modelRAMId, modelResellerId);
    }

    $("#checkInXero").click(function () {
        CheckInXero(false, -1);
    });
    $("#btnCloseRecAcc").click(function () {
        $('#popupboxREC').modal('toggle');
    });
    if ($('#imgSelfieImage').attr('src') != "") {
        debugger;
        var SignName = $('#imgSelfieImage').attr('src');
        var guid = modelUserId;
        var proofDocumentURL = UploadedDocumentPath;
        var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
        var SRC = imagePath + "/" + SignName;
        SRCSelfie = SRC;
        $('#imgSelfieImage').attr('class', SignName);

        $('#imgSelfie').show();
    }

    $("#txtName").focus();

    Array.prototype.pushArray = function () {
        var toPush = this.concat.apply([], arguments);
        for (var i = 0, len = toPush.length; i < len; ++i) {
            this.push(toPush[i]);
        }
    };
    $.fn.serializeToJson = function () {
        var $form = $(this[0]);

        var items = $form.serializeArray();

        var returnObj = {};
        var nestedObjectNames = [];

        $.each(items, function (i, item) {
            //Split nested objects and assign properties
            //You may want to make this recursive - currently only works one step deep, but that's all I need
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

    //FillDropDown('installerDesignerView_UnitTypeID', getUnitTypeURL, installerDesignerUnitTypeId, true, null);
    //FillDropDown('installerDesignerView_StreetTypeID', getStreetTypeURL, installerDesignerStreetTypeId, true, null);
    //    FillDropDown('installerDesignerView_PostalAddressID', getPostalAddressURL, installerDesignerPostalAddressID, true, null);

    dropDownData.push({ id: 'installerDesignerView_UnitTypeID', key: "UnitType", value: installerDesignerUnitTypeId, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'installerDesignerView_StreetTypeID', key: "StreetType", value: installerDesignerStreetTypeId, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'installerDesignerView_PostalAddressID', key: "PostalAddress", value: installerDesignerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });


    $("#btnClosePopUpBoxInsallerDesigner").click(function () {
        $('#popupboxInsallerDesigner').modal('toggle');
    });

    //FillDropDown('WholesalerUnitTypeID', getUnitTypeURL, WholesalerUnitTypeID, true, null);
    //FillDropDown('WholesalerStreetTypeID', getStreetTypeURL, WholesalerStreetTypeID, true, null);
    //    FillDropDown('WholesalerPostalAddressID', getPostalAddressURL, WholesalerPostalAddressID, true, null);

    if (userTypeId == 2) {
        dropDownData.push({ id: 'WholesalerUnitTypeID', key: "UnitType", value: WholesalerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
            { id: 'WholesalerStreetTypeID', key: "StreetType", value: WholesalerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
            { id: 'WholesalerPostalAddressID', key: "PostalAddress", value: WholesalerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
        //{ id: 'InvoicerUnitTypeID', key: "UnitType", value: InvoicerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        //{ id: 'InvoicerStreetTypeID', key: "StreetType", value: InvoicerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        //{ id: 'InvoicerPostalAddressID', key: "PostalAddress", value: InvoicerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
    }
    dropDownData.push({ id: 'InvoicerUnitTypeID', key: "UnitType", value: InvoicerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'InvoicerStreetTypeID', key: "StreetType", value: InvoicerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'InvoicerPostalAddressID', key: "PostalAddress", value: InvoicerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });

    GetInvoicerDetails();
    //logo upload
    $('#uploadBtnSignatureInstallDesign').fileupload({
        url: projectURL + 'Upload',
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var guid = USERID;

                    var signName = $('#imgSignInstallDesign').attr('class');
                    $("[name='installerDesignerView.SESignature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromLogoOnUpload(signName, guid, installerDesignerSESignature);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    SRCSignInstallDesign = imagePath + "/" + data.result[i].FileName;
                    $('#imgSignInstallDesign').attr('src', SRCSignInstallDesign);
                    $('#imgSignInstallDesign').attr('class', data.result[i].FileName);

                    $('#imgSignatureInstallDesign').show();

                    $('<input type="hidden">').attr({
                        name: 'installerDesignerView.SESignature',
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
                showErrorMessage(UploadFailedFiles.length + " " + "File has not been uploaded.");
            }
            if (UploadFailedFilesName.length > 0) {
                showErrorMessage(UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                showSuccessMessage("File has been uploaded successfully.");
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
                        showErrorMessage("Please upload small filename.");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxLogoSize)) {
                        showErrorMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxLogoSize)) {
                    showErrorMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB");
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
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        formData: { userId: USERID },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');


    if ($('#imgSignInstallDesign').attr('src') != "") {
        if (installerDesignerSESignature != "") {
            var SignName = $('#imgSignInstallDesign').attr('src');
            var guid = modelUserId;
            var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
            var SRC = imagePath + "/" + SignName;
            SRCSignInstallDesign = SRC;
            //$('#imgSign').attr('src', SRC).load(function() { logoWidthSign=this.width; logoHeightSign=this.height});
            $('#imgSignInstallDesign').attr('class', SignName);
            $('#imgSignatureInstallDesign').show();
        }
    }

    $('#imgSignatureInstallDesign').click(function () {
        $("#loading-image").css("display", "");
        $('#imgSignInstallDesign').attr('src', SRCSignInstallDesign).load(function () {
            logoWidthSignInstallDesign = this.width;
            logoHeightSignInstallDesign = this.height;

            $('#popupboxInsallerDesigner').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeightSignInstallDesign) {
                $("#imgSignInstallDesign").closest('div').height($(window).height() - 205);
                $("#imgSignInstallDesign").closest('div').css('overflow-y', 'scroll');
                $("#imgSignInstallDesign").height(logoHeightSignInstallDesign);
            }
            else {
                $("#imgSignInstallDesign").height(logoHeightSignInstallDesign);
                $("#imgSignInstallDesign").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidthSignInstallDesign || logoWidthSignInstallDesign >= screen.width - 250) {

                $('#popupboxInsallerDesigner').find(".modal-dialog").width(screen.width - 250);
                $("#imgSignInstallDesign").width(logoWidthSignInstallDesign);
            }
            else {
                $("#imgSignInstallDesign").width(logoWidthSignInstallDesign);
                $('#popupboxInsallerDesigner').find(".modal-dialog").width(logoWidthSignInstallDesign);
            }

            $("#loading-image").css("display", "none");
        });
        $("#imgSignInstallDesign").unbind("error");
        $('#imgSignInstallDesign').attr('src', SRCSignInstallDesign).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });

    $('#imgSelfie').click(function () {
        $("#loading-image").css("display", "");
        if (modelUserId > 0)
            SRCSelfie = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSelfieImage').attr('class');
        $('#imgSelfieImage').attr('src', SRCSelfie);
        $('#imgSelfieImage').load(function () {
            logoWidth = this.width;
            logoHeight = this.height;
            $('#popupboxSelfie').modal({ backdrop: 'static', keyboard: false });

            if ($(window).height() <= logoHeight) {
                //$("#imgSign").height($(window).height() - 205);

                $("#imgSelfieImage").closest('div').height($(window).height() - 205);
                $("#imgSelfieImage").closest('div').css('overflow-y', 'scroll');
                $("#imgSelfieImage").height(logoHeight);
            }
            else {
                $("#imgSelfieImage").height(logoHeight);
                $("#imgSelfieImage").closest('div').removeAttr('style');
            }

            if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
                //$("#imgSign").width(screen.width - 10);
                //$('#popupbox').find(".modal-dialog").width(screen.width - 10);

                $('#popupboxSelfie').find(".modal-dialog").width(screen.width - 250);
                $("#imgSelfieImage").width(logoWidth);
            }
            else {
                $("#imgSelfieImage").width(logoWidth);
                $('#popupboxSelfie').find(".modal-dialog").width(logoWidth);
            }
            $("#loading-image").css("display", "none");
        });
        $("#imgSelfieImage").unbind("error");
        $('#imgSelfieImage').attr('src', SRCSelfie).error(function () {
            alert('Image does not exist.');
            $("#loading-image").css("display", "none");
        });
    });


    $("#installerDesignerView_AddressID").change(function () {
        var addressID = $('#installerDesignerView_AddressID option:selected').val();
        POAddressInstallerDesigner(addressID);
    });


    $("#addSWHInstaller").click(function () {
        $("#addInstallerDesigner").click();
    });

    $("#addInstallerDesigner").click(function () {
        var sID = 0;
        sID = GetSolarCompanyIdforInstallerDesigner();

        if (userTypeId == 4 || userTypeId == 6 || userTypeId == 8) {
            $(".installerDesignerDiv").find("[idClass='installerDesignerRequired']").each(function () {
                $(this).rules("add", {
                    required: true,
                });
            });

            $(".installerDesignerDiv").find("[idClass='installerDesignerNotRequired']").each(function () {
                $(this).rules("add", {
                    required: false,
                });
            });

            if ($("#txtName").length > 0) {
                $("#txtName").rules("add", {
                    required: false,
                });
            }
            if ($("#txtCECAccreditationNumber").length > 0) {
                $("#txtCECAccreditationNumber").rules("add", {
                    required: false,
                });
            }

            if (userTypeId == 1) {
                if (SendBy != 1) {
                    $("#searchSolarCompany").rules("add", {
                        required: false,
                    });
                }
            }
        }

        if (userTypeId == 8) {
            $("#installerDesignerView_UnitTypeID").rules("remove", "required");
            $("#installerDesignerView_StreetTypeID").rules("remove", "required");
        }

        if ($("#UserDetails").valid()) {

            var fileName = $("[name='installerDesignerView.SESignature']").val();

            var data = JSON.stringify($('form').serializeToJson());
            var objData = JSON.parse(data);
            var obj = {};

            obj.SESignature = $("#imgSignInstallDesign").attr('class');
            obj.InstallerDesignerId = objData.installerDesignerView.InstallerDesignerId;
            obj.FirstName = objData.installerDesignerView.FirstName;
            obj.LastName = objData.installerDesignerView.LastName;
            //obj.SESignature =  objData.installerDesignerView.SESignature;
            obj.SolarCompanyId = sID;
            obj.Email = objData.installerDesignerView.Email;
            obj.Phone = objData.installerDesignerView.Phone;
            obj.Mobile = objData.installerDesignerView.Mobile;
            obj.CreatedBy = USERID;

            var radioValue = $("input[name='InstallerUserType']:checked").val();
            if (radioValue == 0) {
                obj.IsSWHUser = false;
                obj.SEDesignRoleId = objData.SEDesignRoleId;
                obj.CECAccreditationNumber = objData.installerDesignerView.CECAccreditationNumber;
                obj.ElectricalContractorsLicenseNumber = objData.installerDesignerView.ElectricalContractorsLicenseNumber;
            }
            else if (radioValue == 1) {
                obj.IsSWHUser = true;
                obj.ElectricalContractorsLicenseNumber = objData.installerDesignerView.SWHLicenseNumber;
            }

            if ($("#installerDesignerView_AddressID").val() == 1) {
                obj.IsPostalAddress = 0;

                if (objData.installerDesignerView.UnitTypeID == "" || objData.installerDesignerView.UnitTypeID == undefined || objData.installerDesignerView.UnitTypeID == null)
                    obj.UnitTypeID = 0;
                else
                    obj.UnitTypeID = objData.installerDesignerView.UnitTypeID;

                obj.UnitNumber = objData.installerDesignerView.UnitNumber;
                obj.StreetNumber = objData.installerDesignerView.StreetNumber;
                obj.StreetName = objData.installerDesignerView.StreetName;

                if ($("#installerDesignerView_StreetTypeID").val() == "" || $("#installerDesignerView_StreetTypeID").val() == undefined || $("#installerDesignerView_StreetTypeID").val() == null)
                    obj.StreetTypeID = 0;
                else
                    obj.StreetTypeID = $("#installerDesignerView_StreetTypeID").val();
            }
            else {
                obj.IsPostalAddress = 1;

                if ($("#installerDesignerView_PostalAddressID").val() == "" || $("#installerDesignerView_PostalAddressID").val() == undefined || $("#installerDesignerView_PostalAddressID").val() == null)
                    obj.PostalAddressID = 0;
                else
                    obj.PostalAddressID = $("#installerDesignerView_PostalAddressID").val();

                obj.PostalDeliveryNumber = $("#installerDesignerView_PostalDeliveryNumber").val();
            }

            obj.Town = objData.installerDesignerView.Town;
            obj.State = objData.installerDesignerView.State;
            obj.PostCode = objData.installerDesignerView.PostCode;

            var dataInstallerDesigner = JSON.stringify(obj);

            //call ajax


            $.ajax({
                type: 'GET',
                url: processRequestURL,
                dataType: 'json',
                data: {
                    excludePostBoxFlag: true,
                    q: obj.Town
                },
                success: function (data) {

                    var data1 = JSON.parse(data);
                    var town = $("#installerDesignerView_Town").val().toUpperCase();
                    var state = $("#installerDesignerView_State").val();
                    var postCode = $("#installerDesignerView_PostCode").val();
                    var isValid = false;

                    if (data1.localities && data1.localities.locality) {
                        if (data1.localities.locality.length) {
                            for (var i = 0; i < data1.localities.locality.length; i++) {
                                if (data1.localities.locality[i].location == town && data1.localities.locality[i].state == state && data1.localities.locality[i].postcode == postCode) {
                                    isValid = true;
                                    break;
                                }
                            }
                        }
                        else {
                            if (data1.localities.locality.location == town && data1.localities.locality.state == state && data1.localities.locality.postcode == postCode) {
                                isValid = true;
                            }
                        }
                    }
                    if (isValid) {
                        $.ajax(
                            {
                                url: installerDesignerAddURL,
                                dataType: 'json',
                                contentType: 'application/json; charset=utf-8', // Not to set any content header
                                type: 'post',
                                data: dataInstallerDesigner,
                                success: function (response) {
                                    if (response.status) {
                                        ResetInstallerDesigner();
                                        $("#datatable").dataTable().fnDraw();
                                        showSuccessMessage(response.message);
                                        $("#btnResetInstallerDesigner").show();
                                        $("#btnCancelInstallerDesigner").hide();
                                    }
                                    else {
                                        showErrorMessage(response.message);
                                    }
                                },
                                error: function () {
                                    showErrorMessage("Installer/Designer has not been added.");
                                }
                            });
                    }
                    else {
                        alert("Please enter valid address.");
                        return false;
                    }
                }
            })
        }
    });

    TownPostcodeAutoComplete($('#txtTown'), $('#txtState'), $('#txtPostCode'));
    TownPostcodeAutoComplete($('#installerDesignerView_Town'), $('#installerDesignerView_State'), $('#installerDesignerView_PostCode'));
    TownPostcodeAutoComplete($('#WholesalerTown'), $('#WholesalerState'), $('#WholesalerPostCode'));
    TownPostcodeAutoComplete($('#InvoicerTown'), $('#InvoicerState'), $('#InvoicerPostCode'))

    //$('#IsWholeSaler').click(function(){
    //    ShowHideWholeSalerInvoice();
    //});

    $("#WholesalerIsPostalAddress").change(function () {
        var addressID = $('#WholesalerIsPostalAddress option:selected').val();
        POWholesalerAddress(addressID);
    });
    $("#InvoicerAddressID").change(function () {
        var addressID = $('#InvoicerAddressID option:selected').val();
        POInvoicerAddress(addressID);
    });
    $("#AddressID").change(function () {
        var addressID = $('#AddressID option:selected').val();
        POAddress(addressID);
    });

    if ((userTypeId == 7 || userTypeId == 9)) {
        $("#datatableUserDevice").kendoGrid({
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: urlGetUserDeviceKendo,
                        data: {},
                        contentType: 'application/json',
                        dataType: 'json',
                        type: 'POST',
                        async: false
                    },
                    parameterMap: function (options) {
                        options.userId = userID;
                        return JSON.stringify(options);
                    }
                },
                schema: {
                    data: "data", // records are returned in the "data" field of the response
                    total: "total"
                },
                pageSize: 10,
            },
            noRecords: true,
            pageable: {
                buttonCount: 5,
                pageSizes: [10, 25, 50, 100]
            },
            columns: [{
                template: "#= ++rowNumber #",
                field: "",
                title: "No.",
                width: "60px"
            }, {
                template: "#=Kendo_DeviceType(data) #",
                field: "Type",
                title: "Platform",
                width: "100px"
            }, {
                field: "DeviceInfo",
                title: "Device Info"
            }, {
                template: function (data) { return '<button type="button" class="btn primary" userDeviceId="' + data.UserDeviceID + '"onclick="DeviceLogout(' + data.UserDeviceID + ',false);">Logout</button>' },
                field: "",
                headerTemplate: function (data) { return '<button type="button" class="btn primary" id="allDeviceLogout" onclick="DeviceLogout(' + userID + ',true);">Logout All Devices</button>' },
                width: "200px",
                attributes: {
                    style: "text-align: -webkit-center;"
                }
            }],
            dataBinding: function () {
                rowNumber = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            }
        });

        if ($("#datatableUserDevice").data("kendoGrid").dataSource.total() == 0) {
            $('#allDeviceLogout').attr("disabled", "disabled");
        }
    }
});

var Kendo_DeviceType = function (data) {
    if (data.Type != null) {
        if (data.Type.toLowerCase() == "iphone") {
            return '<span class="icon"><img src="../../Images/ios.svg" /></span>';
        }
        else {
            return '<span class="icon"><img src="../../Images/android.svg" /></span>';
        }
    }
    else
        return '';
}

function POWholesalerAddress(addressID) {
    if (addressID == 1) {
        $('.DPAWholesaler').show();
        $('.PDAWholesaler').hide();
    }
    else {
        $('.DPAWholesaler').hide();
        $('.PDAWholesaler').show();
    }
}

function POInvoicerAddress(addressID) {
    if (addressID == 1) {
        $('.DPAInvoicer').show();
        $('.PDAInvoicer').hide();
    }
    else {
        $('.DPAInvoicer').hide();
        $('.PDAInvoicer').show();
    }
}

//function ShowHideWholeSalerInvoice() {
//    //if ($('#IsSAASUser').is(":checked")) {
//    //    $("#t10").show(); // SAAS Invoice tab
//    //    $('.tab10btn').hide(); // Account details Save Details Button
//    //    $('.tab10Next').show(); // Next button for SAAS Tab
//    //    $(".SAAS").show();
//    //    $("#prevBtnFromSAASTabToWholeSalerTab").hide();
//    //    $("#prevBtnFromSAASTabToAccountDetailTab").show();
//    //}
//    //else {
//    //    $("#t10").hide();
//    //    $(".SAAS").hide();
//    //    $('.tab10Next').hide(); // Next button for SAAS Tab
//    //    $("#prevBtnFromSAASTabToWholeSalerTab").show();
//    //    $("#prevBtnFromSAASTabToAccountDetailTab").hide();
//    //}

//    if ($('#IsWholeSaler').is(":checked")) {
//        $("#t7").show();
//        $('.tab7Next').show();
//        $("#tab10NextFromWholesaler").hide();
//        $('.tab10btn').hide();
//        $("#wholeSalerTabSavebtn").show();
//    }
//    else {
//        $("#t7").hide();
//        $('.tab7Next').hide(); // Next button for Wholesaler Tab
//        $("#tab10NextFromWholesaler").hide();
//        $('.tab10btn').show(); // Account details Save Details Button
//        $("#wholeSalerTabSavebtn").hide();
//    }

//    //if ($('#IsSAASUser').is(":checked") && $('#IsWholeSaler').is(":checked")) {
//    //if ($('#IsWholeSaler').is(":checked")) {
//    //    //$('.tab7Next').show(); // Next button for Wholesaler Tab
//    //    //$('.tab10Next').hide(); // Next button for SAAS Tab
//    //    //$("#wholeSalerTabSavebtn").hide();
//    //    //$("#tab10NextFromWholesaler").show();
//    //    //$('.tab10btn').hide(); // Account details Save Details Button
//    //    //$("#prevBtnFromSAASTabToWholeSalerTab").show();
//    //    //$("#prevBtnFromSAASTabToAccountDetailTab").hide();

//    //}
//    //else {
//    //    $('.tab7Next').hide(); // Next button for Wholesaler Tab
//    //    //$('.tab10Next').hide(); // Next button for SAAS Tab
//    //    $("#tab10NextFromWholesaler").hide();
//    //    $('.tab10btn').show(); // Account details Save Details Button
//    //    //$("#prevBtnFromSAASTabToWholeSalerTab").hide();
//    //    //$("#prevBtnFromSAASTabToAccountDetailTab").show();
//    //}

//    //$('.WS').hide();
//}

function ShowHideWholeSalerInvoice() {
    debugger;
    if ($('#IsSAASUser').is(":checked")) {
        if (userTypeId == '2') {
            $("#t10").show(); // SAAS Invoice tab
            $('.tab10btn').hide(); // Account details Save Details Button
            $('.tab10Next').show(); // Next button for SAAS Tab
            $(".SAAS").show();
            $('.PersonlDetailsNext').hide();
            $("#prevBtnFromSAASTabToWholeSalerTab").hide();
            $("#prevBtnFromSAASTabToAccountDetailTab").show();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.ParentBtnOfAccountDetail').hide();
        }
        if (userTypeId == '1') {
            $("#t10").show();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').show(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $(".SAAS").show();
        }
        if (userTypeId == '3') {
            $("#t10").show();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').show(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $(".SAAS").show();
        }
        if (userTypeId == '5') {
            $("#t10").show();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').show(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $(".SAAS").show();
        }
        if (userTypeId == '6') {
            $('.NextBtnToInstallerTabFromAccountTab').show();
            $('.BtnFromInstallerTabToSaasTab').show();
            $('.ParentBtnOfAccountDetail').hide();
            $("#t10").show();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $(".SAAS").show();
        }
        if (userTypeId == '9') {
            $("#t10").show();
            $(".NextBtnToInstallerTabFromAccountTab").hide();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.ParentBtnOfAccountDetail').hide(); // Account details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.tab10Next').show(); // Next button for SAAS Tab
            $(".SAAS").show();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
        }
        if (userTypeId == '8') {
            $("#t10").show();
            $(".NextBtnToInstallerTabFromAccountTab").show();
            $('.ParentBtnOfPersonalDetail').hide(); // Personal details Save Button and cancel button
            $('.ParentBtnOfAccountDetail').hide(); // Account details Save Button and cancel button
            $('.PersonlDetailsNextForInstallerAndDesigner').show(); // Next button for SAAS Tab
            $('.PersonlDetailsNext').hide();
            $(".SAAS").show();
        }
        if (userTypeId == '7') {
            $("#t10").show();
            $(".SAAS").show();
            $('.ParentBtnOfAccountDetail').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.PersonlDetailsNext').hide();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.tab10Next').show(); // Next button for SAAS Tab
        }
        if (userTypeId == '12') {
            $("#t10").show();
            $(".SAAS").show();
            $('.ParentBtnOfAccountDetail').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.tab10Next').show(); // Next button for SAAS Tab
        }
        if (userTypeId == '4') {
            $("#t10").show();
            $(".SAAS").show();
            $('.PersonlDetailsNext').hide();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.tab10Next').hide(); // Next button for SAAS Tab
        }
    }
    else {
        if (userTypeId == '2') {
            $("#t10").hide();
            $(".SAAS").hide();
            $('.tab10Next').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNext').hide();
            $("#prevBtnFromSAASTabToWholeSalerTab").show();
            $("#prevBtnFromSAASTabToAccountDetailTab").hide();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.ParentBtnOfAccountDetail').hide();
        }
        if (userTypeId == '1') {
            $('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $("#t10").hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '3') {
            $('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $("#t10").hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '5') {
            $('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $("#t10").hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '6') {
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.BtnFromInstallerTabToSaasTab').hide();
            //$('.ParentBtnOfAccountDetail').show();
            //$('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $("#t10").hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '9') {
            $(".NextBtnToInstallerTabFromAccountTab").hide();
            $('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.ParentBtnOfAccountDetail').hide(); // Account details Save Button and cancel button
            $('.PersonlDetailsNext').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.tab10Next').hide(); // Next button for SAAS Tab
            $("#t10").hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '8') {
            $("#t10").hide();
            $(".NextBtnToInstallerTabFromAccountTab").show();
            $('.ParentBtnOfPersonalDetail').show(); // Personal details Save Button and cancel button
            $('.ParentBtnOfAccountDetail').hide(); // Account details Save Button and cancel button
            $('.PersonlDetailsNextForInstallerAndDesigner').hide(); // Next button for SAAS Tab
            $('.PersonlDetailsNext').hide();
            $(".SAAS").hide();
        }
        if (userTypeId == '7') {
            $("#t10").hide();
            $(".SAAS").hide();
            $('.ParentBtnOfAccountDetail').show();
            $('.NextBtnToInstallerTabFromAccountTab').show();
            $('.PersonlDetailsNext').hide();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.tab10Next').hide(); // Next button for SAAS Tab
        }
        if (userTypeId == '12') {
            $("#t10").hide();
            $(".SAAS").hide();
            $('.ParentBtnOfAccountDetail').show();
            $('.NextBtnToInstallerTabFromAccountTab').show();
            $('.tab10Next').hide(); // Next button for SAAS Tab
        }
        if (userTypeId == '4') {
            $("#t10").show();
            $(".SAAS").show();
            $('.PersonlDetailsNext').hide();
            $('.PersonlDetailsNextForInstallerAndDesigner').hide();
            $('.NextBtnToInstallerTabFromAccountTab').hide();
            $('.tab10Next').hide(); // Next button for SAAS Tab
        }
    }

    if ($('#IsWholeSaler').is(":checked")) {
        $("#t7").show();
        $("#wholeSalerTabSavebtn").show();
        $("#tab10NextFromWholesaler").hide();
        $('.tab10btn').hide();
        $('.tab7Next').show();
    }
    else {
        $("#t7").hide();
        $("#wholeSalerTabSavebtn").hide();
        $('.tab7Next').hide();
    }

    if ($('#IsSAASUser').is(":checked") && $('#IsWholeSaler').is(":checked")) {
        $('.tab7Next').show(); // Next button for Wholesaler Tab
        $('.tab10Next').hide(); // Next button for SAAS Tab
        $("#wholeSalerTabSavebtn").hide();
        $("#tab10NextFromWholesaler").show();
        $('.tab10btn').hide(); // Account details Save Details Button
        $("#prevBtnFromSAASTabToWholeSalerTab").show();
        $("#prevBtnFromSAASTabToAccountDetailTab").hide();

    }
    else if (!$('#IsSAASUser').is(":checked") && !$('#IsWholeSaler').is(":checked")) {
        $('.tab7Next').hide(); // Next button for Wholesaler Tab
        $('.tab10Next').hide(); // Next button for SAAS Tab
        $("#tab10NextFromWholesaler").hide();
        $('.tab10btn').show(); // Account details Save Details Button
        $("#prevBtnFromSAASTabToWholeSalerTab").hide();
        $("#prevBtnFromSAASTabToAccountDetailTab").show();
    }
}

function ShowHidePVD() {
    var radioValue = $("input[name='InstallerUserType']:checked").val();
    if (radioValue == 0) {
        $('label[for="installerDesignerView_Email"]').removeClass("required");
        $('#installerDesignerView_Email').attr('idClass', 'installerDesignerNotRequired');
        $('.showPVD').show();
        $('.showSWH').hide();
        $("#installerDesignerView_FirstName").attr("readonly", "readonly");
        $("#installerDesignerView_LastName").attr("readonly", "readonly");
    }
    else if (radioValue == 1) {
        $('label[for="installerDesignerView_Email"]').addClass("required");
        $('#installerDesignerView_Email').attr('idClass', 'installerDesignerRequired');
        $('.showPVD').hide();
        $('.showSWH').show();
        $("#installerDesignerView_FirstName").removeAttr("readonly");
        $("#installerDesignerView_LastName").removeAttr("readonly");
    }
}

function ResetInstallerDesigner() {

    RemoveValidationMsgAndBorder();

    $(".installerDesignerDiv").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).val('');
    });

    $(".installerDesignerDiv").find("[idClass='installerDesignerNotRequired']").each(function () {
        $(this).val('');
    });

    $("#installerDesignerView_InstallerDesignerId").val(0);

    $("#imgSignatureInstallDesign").hide();
    $("[name='installerDesignerView.SESignature']").val('');
    $(".installerDesignerDiv").find("[name='SEDesignRoleId']").filter('[value=2]').prop('checked', true);

    if (sessionLoggedInUserId == 1) {
        $("#SolarSubContractorID").val(0);
        $("#ResellerId").val($("#ResellerId option:first").val());
        $("#searchSolarCompany").val("");
    }

    $("#installerDesignerView_AddressID").val(1);
    $("#addInstallerDesigner").html('<span class="sprite-img save_ic"></span> Add Installer/Designer');
    $("#installerDesignerView_CECAccreditationNumber").removeAttr("readonly");
    $("#installerDesignerView_ElectricalContractorsLicenseNumber").removeAttr("readonly");
    $("#installerDesignerView_SWHLicenseNumber").removeAttr("readonly");
    $("#installerDesignerView_Email").removeAttr("readonly");
    //$("#installerDesignerView_FirstName").removeAttr("readonly");
    //$("#installerDesignerView_LastName").removeAttr("readonly");

    $(".PDAInstallerDesigner").each(function () {
        $(this).hide();
    });

    $(".DPAInstallerDesigner").each(function () {
        $(this).show();
    });

    $('#installerDesignerView_CECAccreditationNumber').attr('onblur', "checkExistInstallerDesigner(this,'InstallerDesigner AccreditationNumber')");
}

function CancelInstallerDesigner() {
    ResetInstallerDesigner();

    $("#btnResetInstallerDesigner").show();
    $("#btnCancelInstallerDesigner").hide();
}

function GetSolarCompanyIdforInstallerDesigner() {
    var sID = 0;
    if ((userTypeId == 6 || userTypeId == 4) && (sessionUserTypeId == 1 || sessionUserTypeId == 3 || sessionUserTypeId == 2 || sessionUserTypeId == 5)) {
        sID = modelSolarCompanyId;
    }
    else {
        sID = sessionSolarCompanyId;
    }
    //}
    return sID;
}

function POAddressInstallerDesigner(addressID) {
    if (addressID == 1) {
        $('.DPAInstallerDesigner').show();
        $('.PDAInstallerDesigner').hide();
    }
    else {
        $('.DPAInstallerDesigner').hide();
        $('.PDAInstallerDesigner').show();
    }
}

function checkExistInstallerDesigner(obj, title) {
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    var sID = 0;
    sID = GetSolarCompanyIdforInstallerDesigner();

    if (uservalue != "" && uservalue != undefined) {
        $.ajax(
            {
                url: projectURL + 'CheckUserExist',
                data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID, solarCompanyId: parseInt(sID) },
                async: false,
                contentType: 'application/json',
                method: 'get',
                success: function (data) {

                    if (data.status == false) {
                        var errorMsg;
                        errorMsg = data.message;
                        chkvar = false;
                        $(".alert").hide();
                        $("#errorMsgRegion").html(closeButton + errorMsg);
                        $("#errorMsgRegion").show();
                    }
                    else {
                        chkvar = true;
                    }
                    if (chkvar == false) {
                        $("#installerDesignerView_FirstName").val("");
                        $("#installerDesignerView_LastName").val("");

                        $("#installerDesignerView_Email").val("");
                        $("#installerDesignerView_Phone").val("");
                        $("#installerDesignerView_Mobile").val("");
                        $("#installerDesignerView_ElectricalContractorsLicenseNumber").val("");
                        $(".installerDesignerDiv").find("[name='SEDesignRoleId']").filter('[value=' + 2 + ']').prop('checked', true);
                        $("#installerDesignerView_AddressID").val(1);
                        $("#installerDesignerView_UnitTypeID").val("");
                        $("#installerDesignerView_UnitNumber").val("");
                        $("#installerDesignerView_StreetNumber").val("");
                        $("#installerDesignerView_StreetName").val("");
                        $("#installerDesignerView_StreetTypeID").val("");
                        $("#installerDesignerView_Town").val("");
                        $("#installerDesignerView_State").val("");
                        $("#installerDesignerView_PostCode").val("");
                        return false;
                    }

                    $("#errorMsgRegion").hide();
                    $("#successMsgRegion").hide();

                    var installerDesignerData = JSON.parse(data.accreditedData);

                    $("#installerDesignerView_FirstName").val(installerDesignerData.FirstName);
                    $("#installerDesignerView_LastName").val(installerDesignerData.LastName);

                    $("#installerDesignerView_Email").val(installerDesignerData.Email);
                    $("#installerDesignerView_Phone").val(installerDesignerData.Inst_Phone);
                    $("#installerDesignerView_Mobile").val(installerDesignerData.Inst_Mobile);
                    $("#installerDesignerView_ElectricalContractorsLicenseNumber").val(installerDesignerData.LicensedElectricianNumber);
                    $(".installerDesignerDiv").find("[name='SEDesignRoleId']").filter('[value=' + installerDesignerData.RoleId + ']').prop('checked', true);
                    $("#installerDesignerView_AddressID").val(1);

                    if (parseInt(installerDesignerData.Inst_UnitTypeID) > 0)
                        $("#installerDesignerView_UnitTypeID").val(installerDesignerData.Inst_UnitTypeID);
                    else
                        $("#installerDesignerView_UnitTypeID").val('');

                    $("#installerDesignerView_UnitNumber").val(installerDesignerData.MailingAddressUnitNumber);
                    $("#installerDesignerView_StreetNumber").val(installerDesignerData.MailingAddressStreetNumber);
                    $("#installerDesignerView_StreetName").val(installerDesignerData.MailingAddressStreetName);

                    if (parseInt(installerDesignerData.StreetTypeID) > 0)
                        $("#installerDesignerView_StreetTypeID").val(installerDesignerData.StreetTypeID);
                    else
                        $("#installerDesignerView_StreetTypeID").val('');

                    $("#installerDesignerView_Town").val(installerDesignerData.MailingCity);
                    $("#installerDesignerView_State").val(installerDesignerData.Abbreviation);
                    $("#installerDesignerView_PostCode").val(installerDesignerData.PostalCode);
                    return false;
                }
            });
        $('#' + obj.id).closest('form').valid();
    }
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
}

function showErrorMessageForPopup(message) {
    $(".alertForPopup").hide();
    $("#successMsgRegionForPopup").hide();
    $("#errorMsgRegionForPopup").html(closeButton + message);
    $("#errorMsgRegionForPopup").show();

}

function showSuccessMessageForPopup(message) {
    $(".alertForPopup").hide();
    $("#errorMsgRegionForPopup").hide();
    $("#successMsgRegionForPopup").html(closeButton + message);
    $("#successMsgRegionForPopup").show();
}

function removeSolarElecticianValidation() {

    $(".installerDesignerDiv").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).rules("add", {
            required: false,
        });
    });

    $(".installerDesignerDiv").find("[idClass='installerDesignerNotRequired']").each(function () {
        $(this).rules("add", {
            required: false,
        });
    });

}

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

function removeValidationOnTabChangeInstallerDesigner() {
    $(".installerDesignerDiv").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).rules("add", {
            required: false,
        });
    });
    $(".installerDesignerDiv").find("[idClass='installerDesignerNotRequired']").each(function () {
        $(this).rules("add", {
            required: false,
        });
    });
}

function RemoveValidationMsgAndBorder() {
    $(".installerDesignerDiv").find("[idClass='installerDesignerRequired']").each(function () {
        $(this).removeClass("input-validation-error");
        if ($(this).next().find('span').length > 0) {
            $(this).next().find('span').remove();
        }
    });
}

function CheckInXero(isAllowInsert, XeroContactAlreadyExists = -1) {
    $.ajax(
        {
            url: checkInXeroURL,
            data: { userId: $("#UserId").val(), isAllowInsert: isAllowInsert, XeroContactAlreadyExists: XeroContactAlreadyExists },
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            success: function (response) {
                if (response && response.status == false) {

                    /*
                     * XeroAccountReCreateStatus
                     * 0 first time check
                     * 1 Ask user that already have contact the unique client number are you want to remove old one and create new one?
                     * 2 Remove old one and recreate new contact
                    */
                    if (response.XeroContactAlreadyExists == 1) {
                        var result = confirm("This contact is already exists, would you like to open it?");
                        if (result) {
                            CheckInXero(true, 1);
                            return false;
                        }
                        else
                            return false;
                    }

                    if (response.isAllowInsert == false) {
                        var result = confirm("This contact is not found, would you like to create this contact in XERO?");
                        if (result) {
                            CheckInXero(true, 2);
                            return false;
                        }
                        else
                            return false;
                    }

                    if (response.error.toLowerCase() == 'specified method is not supported.' || response.error.toLowerCase() == 'renewtokenexception' || response.error.toLowerCase() == 'unauthorizedexception' || response.error.toLowerCase() == 'invalid_grant') {
                        window.open(xeroConnectURL, "_blank");
                    }
                    else if (response.error.toLowerCase() == 'sessiontimeout')
                        window.location.href = logoutURL;
                    else if (response.error)
                        showErrorMessage(response.error);
                    else
                        showErrorMessage("Contact has not been checked.");
                }
                else {
                    var obj = jQuery.parseJSON(response.data);
                    $('#LoadContactDetail').load(xeroContactURL, { Data: obj });
                    setTimeout(function () {
                        $('#popupboxContact').modal({ backdrop: 'static', keyboard: false });
                        $("#LoadContactDetail").load();
                        if ($("#onOffSwitchAddCustomCompanyName").prop('checked')) {
                            $(".custom-company-name").show();
                            $("#UserView_CustomCompanyName").rules("add", "required");
                            $("#UserView_CustomCompanyName").prev("label").removeClass("required").addClass("required");
                        }
                        else {
                            $(".custom-company-name").hide();
                            $("#UserView_CustomCompanyName").val(''); onclick = "$('#UsageType').val() == '2'?CheckInXero(false):CheckContactInXero();"
                            $("#UserView_CustomCompanyName").rules("remove", "required");
                            $("#UserView_CustomCompanyName").prev("label").addClass("required").removeClass("required");
                        }
                        HeighlightDifference();
                    }, 1000);
                }
            },
            error: function (response) {
                showErrorMessage("Contact has not been checked.");
            }
        });
}

function BindRAMForSolarCompany(ramId, resellerId) {

    $("#searchRAM").val("");
    $.ajax({
        type: 'get',
        url: GetRAMByResellerURL,
        dataType: 'json',
        data: { resellerId: resellerId },
        success: function (RAMS) {
            RAMList = [];
            $.each(RAMS, function (i, user) {
                RAMList.push({ value: user.Value, text: user.Text });
            });

            if (ramId > 0) {
                $('#hdnRAMID').val(ramId);
                $.each(RAMList, function (key, value) {
                    if (value.value == ramId) {
                        $("#searchRAM").val(value.text);
                    }
                });
            }

        },
        error: function (ex) {
            alert('Failed to retrieve greenbot account manager.' + ex);
        }
    });
    return false;
}

function CheckClientCodePrefix(clientCodePrefix, isCreateFSA, resellerId, createUserType) {
    var RAId;
    var userId;
    var userTypeId;
    if (isCreateFSA == 1) {
        RAId = resellerId;
        userTypeId = createUserType;
        userId = 0;
    }
    else {
        RAId = modelResellerId;
        userTypeId = userTypeId;
        userId = modelUserId;
    }
    if (RAId == "" || RAId == null || RAId == undefined)
        RAId = 0;
    if (userTypeId == "" || userTypeId == null || userTypeId == undefined)
        userTypeId = 0;

    var count = 0;
    $.ajax({
        type: 'get',
        url: CheckClientCodePrefixURL,
        dataType: 'json',
        data: { userTypeId: userTypeId, resellerId: RAId, userId: userId, clientCodePrefix: clientCodePrefix },
        async: false,
        success: function (data) {
            if (data) {
                if (data.count > 0) {
                    if (userTypeId == 5)
                        showErrorMessage("Client code prefix is already used by another account manager.");
                    else
                        showErrorMessage("Client code prefix is already used by another reseller or account manager.");

                    count = 0;
                }
                else
                    count = 1;
            }
            return 0;
        },
        error: function (ex) {
            alert('Failed to check client code prefix');
        }
    });
    return count;
}

function GetClientNumberofSCAOnRAMChange() {
    var resellerId;

    if (sessionUserTypeId == 1)
        resellerId = $("#ResellerID").val();
    else
        resellerId = modelResellerId;

    var url = GetClientNumberOnRAMChangeURL;
    var clientNumber = '';
    var unitTypeId = '';
    var clientNumberUserType = [];

    var ramId = 0;
    if ($("#hdnRAMID").val() == "") {
        ramId = 0;
        unitTypeId = 2;
    }
    else {
        ramId = $("#hdnRAMID").val();
        unitTypeId = 5;
    }
    GetClientNumberOnRAMChange(unitTypeId, resellerId, ramId, url);

}

function autoCompleteRAM(ramId, resellerId) {
    $('#searchRAM').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnRAMID").val('');
            $.each(RAMList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchRAM").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnRAMID").val(ui.item.id); // save selected id to hidden input
            GetClientNumberofSCAOnRAMChange();
            $("#RAMId").val($("#hdnRAMID").val());
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchRAM").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);

    };
    BindRAMForSolarCompany(ramId, resellerId);
}

function ShowHideCheckInXero(userTypeId) {
    if (userTypeId == 4)
        $(".btnCheckInXero").show();
    else
        $(".btnCheckInXero").hide();
}

$("#CompanyABN").change(function () {
    ChangeCompanyABN($('#CompanyABN').val(), $('#CompanyName'), false);
});

$("#WholesalerCompanyABN").change(function () {
    if (userTypeId == 2) {
        ChangeCompanyABN($('#WholesalerCompanyABN').val(), $('#WholesalerCompanyName'), true);
    }
});

function GetCompanyNameFromCompanyABN(id, objCompanyName, strCompanyName, isWholesalerDetails) {
    $.ajax({
        type: "GET",
        url: getGetCompanyABNURL,
        data: { id: id },
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            if (data == 0) {
                objCompanyName.empty();
                $('#fromDate').val("");
                $('#toDate').val("");
                //if(!isWholesalerDetails){
                //    $('#fromDate').val(value.strFromDate);
                //    $('#toDate').val(value.strToDate);
                //}
                objCompanyName.append($("<option></option>").val("").html("Select"));
            }
            else {
                if (objCompanyName.find('option').length > 1) {
                    objCompanyName.empty();
                    objCompanyName.append($("<option></option>").val("").html("Select"));
                    $.each(data, function (key, value) {
                        objCompanyName.append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                else {
                    $.each(data, function (key, value) {
                        objCompanyName.append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                initialize(data, objCompanyName.selector.replace('#', ''), isWholesalerDetails);
                var str = strCompanyName;
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
                objCompanyName.val(str);
                return data;
            }
        }
    });
}

function initialize(data, txtCompanyName, isWholesalerDetails) {
    $("#" + txtCompanyName).change(function () {
        $.each(data, function (key, value) {
            var Cname = value.CompanyName;
            var drpVal = $('select#' + txtCompanyName + ' option:selected').val();
            if (!isWholesalerDetails) {
                if (Cname == drpVal) {
                    $('#fromDate').val(value.strFromDate);
                    $('#toDate').val(value.strToDate);
                }
            }
        });
    });
}

function ChangeCompanyABN(id, objCompanyName, isWholesalerDetails) {
    $.ajax({
        type: "GET",
        url: getGetCompanyABNURL,
        data: { id: id },
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            if (data == 0) {
                objCompanyName.empty();
                $('#fromDate').val("");
                $('#toDate').val("");
                //if(!isWholesalerDetails){
                //    $('#fromDate').val(data.strFromDate);
                //    $('#toDate').val(data.strToDate);
                //}
                objCompanyName.append($("<option></option>").val("").html("Select"));
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Invalide Company ABN.");
                $("#errorMsgRegion").show();
                $("#CompanyNameRefresh i").removeClass("fa-spin");
                return false;
            }
            else {
                if (objCompanyName.find('option').length > 1) {
                    objCompanyName.empty();
                    objCompanyName.append($("<option></option>").val("").html("Select"));
                    $.each(data, function (key, value) {
                        objCompanyName.append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                else {
                    $.each(data, function (key, value) {
                        objCompanyName.append($("<option></option>").val(value.CompanyName).html(value.CompanyName));
                    });
                }
                initialize(data, objCompanyName.selector.replace('#', ''), isWholesalerDetails);

                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#CompanyNameRefresh i").removeClass("fa-spin");
                return data;
            }
        }
    });
}

function TownPostcodeAutoComplete(objTown, objState, objPostCode) {
    objTown.autocomplete({
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
            objState.val(ui.item.state);
            objTown.val(ui.item.location);
        }
    });
}

function POAddress(addressID) {

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

function proofUpload(tableId, proofDocumentType) {
    var url = projectURL + 'Upload';
    $('.btnUploadProofDocs').fileupload({
        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {
                    var rowcount = $('#' + tableId + ' tr').length;
                    var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var documentId = "document" + count;
                    var content = "<tr style='margin-top:30px'>"

                    if (proofDocumentType != 6) {
                        content += '<td class="tdCount col-sm-2" >' + count + '.' + ' </td>';
                    }

                    content += '<td class="col-sm-6" style="color:#494949">' + data.result[i].FileName.replace("%", "$") + ' </td>';

                    if (mimeType == "image" && proofDocumentType != 6) {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" class="' + data.result[i].FileName.replace("%", "$") + '" title="Preview" onclick="OpenDocument(this)" proofDocumentType="' + proofDocumentType + '"></td>';
                    }
                    else {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" class="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadDocument(this)" proofDocumentType="' + proofDocumentType + '"></td>';
                    }
                    var fnDelete = "javascript:DeleteFileFromFolder('" + data.result[i].FileName + "','" + proofDocumentType + "')";
                    content += '<td style="color:blue"><img src=' + ProjectImagePath + 'images/delete-icon.png style="cursor: pointer id="signDelete" title="Delete" onclick="' + fnDelete + '"></td>';
                    content += "</tr>"
                    DocCount += 1;

                    if (proofDocumentType == 6) {
                        $('#' + tableId).html(content);
                    }
                    else {
                        $('#' + tableId).append(content);
                    }
                    $('<input type="hidden">').attr({
                        name: 'ProofFileNamesCreate[' + DocCount + '].FileName',
                        value: data.result[i].FileName.replace("%", "$"),
                        proofDocumentType: proofDocumentType,
                        file: data.result[i].FileName.replace("%", "$"),
                        class: 'ProofFileNamesCreate'
                    }).appendTo('form');
                    $('<input type="hidden">').attr({
                        name: 'ProofFileNamesCreate[' + DocCount + '].ProofDocumentType',
                        value: proofDocumentType,
                        proofDocumentType: proofDocumentType,
                        file: data.result[i].FileName.replace("%", "$"),
                        class: 'ProofFileNamesCreate'
                    }).appendTo('form');
                    if (proofDocumentType == 6) {
                        $('<input type="hidden">').attr({
                            name: 'ContractPathFile[' + DocCount + '].FileName',
                            value: data.result[i].FileName.replace("%", "$"),
                            proofDocumentType: proofDocumentType,
                            file: data.result[i].FileName.replace("%", "$"),
                            class: 'ContractPathFile'
                        }).appendTo('form');
                        $('<input type="hidden">').attr({
                            name: 'ContractPathFile[' + DocCount + '].ProofDocumentType',
                            value: proofDocumentType,
                            proofDocumentType: proofDocumentType,
                            file: data.result[i].FileName.replace("%", "$"),
                            class: 'ContractPathFile'
                        }).appendTo('form');
                    }

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
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();

            }
        },
        progressall: function (e, data) { },
        singleFileUploads: false,
        send: function (e, data) {
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
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
            }).appendTo('form');
            return true;
        },
        formData: {
            userId: USERID
            , ProofDocumentType: parseInt(proofDocumentType)
            , IsScaProofDocs: proofDocumentType == 6 ? false : true
        },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}


function proofUploadInstaller(DocLoc) {

    var url = projectURL + 'Upload';
    $('.btnUploadProofDocs').fileupload({
        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {
                    var rowcount = $('#' + tableId + ' tr').length;
                    var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var documentId = "document" + count;
                    var content = "<tr style='margin-top:30px' id='Doc_" + DocLoc + "'>"
                    content += '<td class="tdCount col-sm-2" >' + count + '.' + ' </td>';
                    content += '<td class="col-sm-6" style="color:#494949">' + data.result[i].FileName.replace("%", "$") + ' </td>';
                    content += '<td class="col-sm-6" style="color:#494949">' + DocLoc + ' </td>';

                    if (mimeType == "image") {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" class="' + data.result[i].FileName.replace("%", "$") + '" title="Preview" onclick="OpenDocument(this)" proofDocumentType="5"></td>';
                    }
                    else {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" class="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadDocument(this)" proofDocumentType="5"></td>';
                    }
                    var fnDelete = "javascript:DeleteFileFromFolder('" + data.result[i].FileName + "','5')";
                    content += '<td style="color:blue"><img src=' + ProjectImagePath + 'images/delete-icon.png style="cursor: pointer id="signDelete" title="Delete" onclick="' + fnDelete + '"></td>';
                    content += "</tr>"
                    DocCount += 1;
                    var tableId = 'tblInstallerDriversLicense';
                    var fileName = data.result[i].FileName;

                    if ($("#DriversLic_" + DocLoc).length > 0) {
                        var FolderName = USERID;
                        var fileDelete = "InstallersDriversLicense" + "\\" + $("#DriversLic_" + DocLoc).val();
                        $.ajax(
                            {
                                url: BaseURL + 'DeleteFileFromFolder/User',
                                data: { fileName: fileDelete, FolderName: FolderName },
                                method: 'get',
                                success: function (data1) {
                                    $("#Doc_" + DocLoc).remove();
                                    $('#' + tableId).append(content);
                                    $('<input type="hidden">').attr({
                                        name: 'ProofFileNamesCreate[' + DocCount + '].FileName',
                                        value: fileName.replace("%", "$"),
                                        proofDocumentType: 5,
                                        file: fileName.replace("%", "$"),
                                        class: 'ProofFileNamesCreate',
                                        id: "DriversLic_" + DocLoc,
                                        DocLoc: DocLoc
                                    }).appendTo('#UserDetails');
                                    $('<input type="hidden">').attr({
                                        name: 'ProofFileNamesCreate[' + DocCount + '].ProofDocumentType',
                                        value: 5,
                                        proofDocumentType: 5,
                                        file: fileName.replace("%", "$"),
                                        class: 'ProofFileNamesCreate',
                                    }).appendTo('#UserDetails');

                                    var DocLocInt = 0;
                                    if (DocLoc == "Front")
                                        DocLocInt = 1;
                                    else if (DocLoc == "Back")
                                        DocLocInt = 2;

                                    $('<input type="hidden">').attr({
                                        name: 'ProofFileNamesCreate[' + DocCount + '].DocLoc',
                                        value: DocLocInt,
                                        proofDocumentType: 5,
                                        file: fileName.replace("%", "$"),
                                        class: 'ProofFileNamesCreate',
                                    }).appendTo('#UserDetails');
                                    $("#DriversLic_" + DocLoc).remove();
                                }
                            });
                    }
                    else {
                        $('#' + tableId).append(content);
                        $('<input type="hidden">').attr({
                            name: 'ProofFileNamesCreate[' + DocCount + '].FileName',
                            value: fileName.replace("%", "$"),
                            proofDocumentType: 5,
                            file: fileName.replace("%", "$"),
                            class: 'ProofFileNamesCreate',
                            id: "DriversLic_" + DocLoc
                        }).appendTo('#UserDetails');
                        $('<input type="hidden">').attr({
                            name: 'ProofFileNamesCreate[' + DocCount + '].ProofDocumentType',
                            value: 5,
                            proofDocumentType: 5,
                            file: fileName.replace("%", "$"),
                            class: 'ProofFileNamesCreate',
                        }).appendTo('#UserDetails');

                        var DocLocInt = 0;
                        if (DocLoc == "Front")
                            DocLocInt = 1;
                        else if (DocLoc == "Back")
                            DocLocInt = 2;

                        $('<input type="hidden">').attr({
                            name: 'ProofFileNamesCreate[' + DocCount + '].DocLoc',
                            value: DocLocInt,
                            proofDocumentType: 5,
                            file: fileName.replace("%", "$"),
                            class: 'ProofFileNamesCreate',
                        }).appendTo('#UserDetails');
                    }
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(fileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(fileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {

                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();

            }
        },
        progressall: function (e, data) { },
        singleFileUploads: true,
        send: function (e, data) {
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
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
            }).appendTo('form');
            return true;
        },
        formData: {
            userId: USERID
            , ProofDocumentType: 5
            , IsScaProofDocs: true
            , DocLoc: DocLoc
        },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}
//function UploadContractPath(tableId, proofDocumentType) {
//    var url = projectURL + 'Upload';
//    $('.btnUploadContractDoc').fileupload({
//        url: url,
//        dataType: 'json',
//        done: function (e, data) {
//            var UploadFailedFiles = [];
//            UploadFailedFiles.length = 0;

//            var UploadFailedFilesName = [];
//            UploadFailedFilesName.length = 0;
//            //formbot start
//            for (var i = 0; i < data.result.length; i++) {

//                if (data.result[i].Status == true) {
//                    var rowcount = $('#' + tableId + ' tr').length;
//                    var count = rowcount + 1;
//                    var documentType = data.result[i].MimeType.split("/");

//                    var content = "<tr style='margin-top:30px' id='0'>"
//                    content += '<td class="col-sm-6" style="color:#494949">' + data.result[i].FileName.replace("%", "$") + ' </td>';
//                    content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer"  id="' + USERID + '" class="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadContractDocument(this)"></td>';
//                    var fnDelete = "javascript:DeleteFileFromDatabase(0,'" + USERID + "','" + data.result[i].FileName.replace("%", "$") + "','" + 6 + "')";
//                    content += '<td style="color:blue"><img src=' + ProjectImagePath + 'images/delete-icon.png style="cursor: pointer id="signDelete" title="Delete" style="cursor: pointer" onclick="' + fnDelete + '"></td>';
//                    content += "</tr>"
//                    DocCount += 1;
//                        //$(".ContractPathFile").each(function () {
//                        //        $(this).remove();
//                        //});
//                        //var fnDeleteContractFile = "javascript:DeleteContractFileFromFolder('" + data.result[i].FileName + "','" + proofDocumentType + "')";
//                    $("#SAASContractPath").html("");
//                    $("#SAASContractPath").html('<a href="#" id="' + USERID + '" class="' + data.result[i].FileName.replace("%", "$") + '" onclick="DownloadContractDocument(this)" proofDocumentType="' + proofDocumentType + '" >Download Contract</a>');
//                    //$('#SAASContractPath').html('<a href="#" id="' + $("#Invoicer").val() + '" class="' + data.result[i].FileName.replace("%", "$") + '" onclick="DownloadContractDocument(this)" proofDocumentType="' + proofDocumentType + '" >Download Contract</a><a href="#" style="cursor: pointer" onclick="' + fnDelete + '">Delete Contract</a>');

//                    //$('#' + tableId).append(content);
//                    //$('<input type="hidden">').attr({
//                    //    name: 'ProofFileNamesCreate[' + DocCount + '].FileName',
//                    //    value: data.result[i].FileName.replace("%", "$"),
//                    //    proofDocumentType: proofDocumentType,
//                    //    file: data.result[i].FileName.replace("%", "$"),
//                    //    class: 'ProofFileNamesCreate'
//                    //}).appendTo('form');
//                    //$('<input type="hidden">').attr({
//                    //    name: 'ProofFileNamesCreate[' + DocCount + '].ProofDocumentType',
//                    //    value: proofDocumentType,
//                    //    proofDocumentType: proofDocumentType,
//                    //    file: data.result[i].FileName.replace("%", "$"),
//                    //    class: 'ProofFileNamesCreate'
//                    //}).appendTo('form');

//                        $('<input type="hidden">').attr({
//                            name: 'ContractPathFile[' + DocCount + '].FileName',
//                            value: data.result[i].FileName.replace("%", "$"),
//                            proofDocumentType: proofDocumentType,
//                            file: data.result[i].FileName.replace("%", "$"),
//                            class: 'ContractPathFile'
//                        }).appendTo('form');
//                        $('<input type="hidden">').attr({
//                            name: 'ContractPathFile[' + DocCount + '].ProofDocumentType',
//                            value: proofDocumentType,
//                            proofDocumentType: proofDocumentType,
//                            file: data.result[i].FileName.replace("%", "$"),
//                            class: 'ContractPathFile'
//                        }).appendTo('form');


//                }
//                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
//                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
//                }
//                else {
//                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
//                }
//            }
//            if (UploadFailedFiles.length > 0) {

//                $(".alert").hide();
//                $("#successMsgRegion").hide();
//                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
//                $("#errorMsgRegion").show();

//            }
//            if (UploadFailedFilesName.length > 0) {
//                $(".alert").hide();
//                $("#successMsgRegion").hide();
//                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
//                $("#errorMsgRegion").show();

//            }
//            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
//                $(".alert").hide();
//                $("#errorMsgRegion").hide();
//                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
//                $("#successMsgRegion").show();

//            }
//        },
//        progressall: function (e, data) { },
//        singleFileUploads: false,
//        send: function (e, data) {
//            var documentType = data.files[0].type.split("/");
//            var mimeType = documentType[0];
//            if (data.files.length == 1) {
//                for (var i = 0; i < data.files.length; i++) {
//                    if (data.files[i].name.length > 50) {
//                        $(".alert").hide();
//                        $("#successMsgRegion").hide();
//                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
//                        $("#errorMsgRegion").show();
//                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
//                        $('body').animate({ scrollTop: 0 }, 'slow');

//                        return false;
//                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
//                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
//                        return false;
//                    }
//                }
//            }
//            if (data.files.length > 1) {
//                for (var i = 0; i < data.files.length; i++) {
//                    if (data.files[i].size > parseInt(MaxImageSize)) {

//                        $(".alert").hide();
//                        $("#successMsgRegion").hide();
//                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
//                        $("#errorMsgRegion").show();

//                        return false;
//                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
//                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
//                        return false;
//                    }
//                }
//            }
//            else {
//                if (data.files[0].size > parseInt(MaxImageSize)) {
//                    $(".alert").hide();
//                    $("#successMsgRegion").hide();
//                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
//                    $("#errorMsgRegion").show();

//                    return false;
//                }
//            }
//            if (mimeType == "image") {
//                $(".alert").hide();
//                $("#errorMsgRegion").html(closeButton + "Please upload a file with .doc , .docx or .pdf extension.");
//                $("#errorMsgRegion").show();
//                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
//                $('body').animate({ scrollTop: 0 }, 'slow');

//                return false;


//            }
//            $(".alert").hide();
//            $("#errorMsgRegion").html("");
//            $("#errorMsgRegion").hide();
//            $('<input type="hidden">').attr({
//                name: 'Guid',
//                value: USERID,
//            }).appendTo('form');
//            return true;
//        },
//        formData: {
//            userId: USERID
//            , ProofDocumentType: parseInt(proofDocumentType)
//            , IsScaProofDocs: false
//        },
//        change: function (e, data) {
//            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
//        }
//    }).prop('disabled', !$.support.fileInput)
//        .parent().addClass($.support.fileInput ? undefined : 'disabled');
//}

function DeviceLogout(userDeviceID, isLogoutAll) {
    $.ajax({
        type: 'get',
        url: urlDeviceLogout,
        dataType: 'json',
        data: { id: userDeviceID, isLogoutAll: isLogoutAll },
        success: function (data) {
            if (data.status) {
                showSuccessMessage("Device logged out successfully.");
                $("#datatableUserDevice").data("kendoGrid").dataSource.read();//.refresh();
                if (isLogoutAll)
                    $('#allDeviceLogout').attr("disabled", "disabled");
                //if(!isLogoutAll)
                //    $("[userdeviceid="+userDeviceID+"]").closest("tr").remove();
                //$("[userdeviceid="+userDeviceID+"]").hide();
            }
            else
                showErrorMessage(data.msg);
        },
        error: function (ex) {
            alert('Failed to logout.');
        }
    });
}

function GetRECAccounts(Type) {
    var recUserName = $("#CERLoginId").val();
    var recPassword = $("#RecPassword").val();
    if (Type == "Admin") {
        recUserName = $("#CERSuperAdminLoginId").val();
        recPassword = $("#RecSuperAdminPassword").val();
    }
    if (recUserName != "" && recPassword != "") {
        $.ajax({
            url: getRECAccountsURL,
            dataType: 'json',
            type: 'get',
            data: {
                recUsername: recUserName,
                recPassword: recPassword
            },
            contentType: 'application/json',
            success: function (response) {
                console.log(response);
                var options = "";
                if (response.status) {
                    if (response.Items.length == 1) {
                        var value = response.Items[0];
                        if (response.isAccess == false) {
                            var str = "<ul style='list-style: circle'><li><b>This REC account does not have bulk upload permissions. REC upload cannot continue</b></li><li><b>REC Company Name: </b>" + value.RECCompName + "</li><li><b>REC Account: </b>" + value.RECAccName + "</li><li><b>REC Name: </b>" + value.RECName + "</li></ul>";
                            showErrorMessage(str);
                            $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                            $('body').animate({ scrollTop: 0 }, 'slow');
                            ClearData(Type);
                            return false;
                        }

                        SelectRECAcc(value.RECAccName, value.RECCompName, value.RECName, Type, recUserName, recPassword)
                    }
                    else {
                        $.each(response.Items, function (i, val) {
                            options += '<tr><td><input type="button" onclick="SelectRECAcc(\'' + val.RECAccName + '\',\'' + val.RECCompName + '\',\'' + val.RECName + '\',\'' + Type + '\',\'' + recUserName + '\',\'' + recPassword + '\')" class="btn primary" name="recAcc" data-id="' + val.RECAccName + '" value="Select" /></td><td>' + val.RECAccName + '</td><td>' + val.RECCompName + '</td>';
                        });

                        $("#recAcc").html(options);
                        $('#popupboxREC').modal({ backdrop: 'static', keyboard: false });
                    }
                }
                else {
                    showErrorMessage(response.message);
                }
            },
            error: function () {
                showErrorMessage("UserName/Password is wrong.");
            }
        });
    }
    else {
        alert("Please enter REC Credentials.");
        return false;
    }
}

function SelectRECAcc(RECAccName, RECCompName, RECName, LoginType, RECUserName, RECPassword) {
    if (LoginType == "Reseller") {
        $("#RecCompUserName").val(RECAccName);
        $("#RecCompUserNamelbl").text(RECAccName);
        $("#RECName").val(RECName);
        $("#RECNamelbl").text(RECName);
        $("#RECCompName").val(RECCompName);
        $("#RECCompNamelbl").text(RECCompName);
    }
    else {
        $("#RecSuperAdminUserName").val(RECAccName);
        $("#SuperAdminRECName").val(RECName);
        $("#SuperAdminRECCompName").val(RECCompName);
        $("#RecSuperAdminUserNamelbl").text(RECAccName);
        $("#SuperAdminRECNamelbl").text(RECName);
        $("#SuperAdminRECCompNamelbl").text(RECCompName);
    }
    if (RECAccName != "Normal User") {
        $("#popupboxREC").modal('toggle');
        $.ajax({
            url: getRECNameURL,
            type: 'GET',
            data: {
                recUsername: RECUserName,
                recPassword: RECPassword,
                RECAccName: RECAccName
            },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                if (response.isAccess == false) {
                    var str = "<ul style='list-style: circle'><li><b>This REC account does not have bulk upload permissions. REC upload cannot continue</b></li><li><b>REC Company Name: </b>" + RECCompName + "</li><li><b>REC Account: </b>" + RECAccName + "</li><li><b>REC Name: </b>" + response.RECName + "</li></ul>";
                    showErrorMessage(str);
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');
                    ClearData(LoginType);
                    return false;
                }
                if (LoginType == "Reseller") {
                    $("#RECName").val(response.RECName);
                    $("#RECNamelbl").text(response.RECName);
                }
                else {
                    $("#SuperAdminRECName").val(response.RECName);
                    $("#SuperAdminRECNamelbl").text(response.RECName);
                }
            }
            , error: function (response) {
                alert(response);
            }
        });
    }
}

function CheckChange() {
    $("#UseCredentialFrom").prop("checked", $("#adminResellerSwitch").prop("checked"));
}

function UpdateContactInXeroLog() {

    var UserId = modelUserId;
    $.ajax({
        url: getLogsForUpdateContactURL,
        type: 'GET',
        data: { UserId: UserId },
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            $('#popupUpdateContactXeroLog').modal('toggle');
            $("#listOfLog").html('');
            if (response.UpdateContactXeroLogs.length > 0) {
                for (var i = 0; i < response.UpdateContactXeroLogs.length; i++) {
                    $("#listOfLog").append('<ul style="background-color:#f6f9f5;margin:6px;   padding: 3px;" ><li class="UpdateContactlog"><span>' + response.UpdateContactXeroLogs[i].HistoryMessage + '.</span></li></ul>');
                }
            }
        }
        , error: function (response) {
            alert(response);
        }
    });
}

function ClearData(Type) {
    if (Type == "Reseller") {
        $("#RecCompUserName").val("");
        $("#RecCompUserNamelbl").text("");
        $("#RECName").val("");
        $("#RECNamelbl").text("");
        $("#RECCompName").val("");
        $("#RECCompNamelbl").text("");
        $("#CERLoginId").val("");
        $("#RecPassword").val("");
    }
    else {
        $("#RecSuperAdminUserName").val("");
        $("#SuperAdminRECName").val("");
        $("#SuperAdminRECCompName").val("");
        $("#RecSuperAdminUserNamelbl").text("");
        $("#SuperAdminRECNamelbl").text("");
        $("#SuperAdminRECCompNamelbl").text("");
        $("#CERSuperAdminLoginId").val("");
        $("#RecSuperAdminPassword").val("");
    }
}


$("#Invoicer").change(function () {
    var AccountCode = $(this).find('option:selected').attr('accountcode');
    $("#AccountCode").val(AccountCode);
});

function clearSAASfields() {
    $("#InvoicerFirstName").val("");
    $("#InvoicerLastName").val("");
    $("#InvoicerPhone").val("");
    $("#InvoicerAddressID").val("1");
    POInvoicerAddress(1);
    $("#InvoicerUnitTypeID").val("");
    $("#InvoicerUnitNumber").val("");
    $("#InvoicerStreetNumber").val("");
    $("#InvoicerStreetName").val("");
    $("#InvoicerStreetTypeID").val("");
    $("#InvoicerTown").val("");
    $("#InvoicerState").val("");
    $("#InvoicerPostCode").val("");
    $("#InvoicerPostalAddressID").val("");
    $("#InvoicerPostalDeliveryNumber").val("");
    //$("#SAASContractPath").html("");
}

function GetInvoicerDetails() {
    $.ajax({
        type: "GET",
        url: "/User/GetInvoicerDetailsList",
        data: {},
        success: function (data) {
            var s = '<option value="0">Select</option>';
            for (var i = 0; i < data.length; i++) {
                if (modelInvoicer == data[i].InvoicerId.toString()) {
                    s += '<option value="' + data[i].InvoicerId + '" AccountCode="' + data[i].AccountCode + '" selected>' + data[i].InvoicerName + '</option>';
                    $("#AccountCode").val(data[i].AccountCode);
                }
                else {
                    s += '<option value="' + data[i].InvoicerId + '" AccountCode="' + data[i].AccountCode + '">' + data[i].InvoicerName + '</option>';
                }

            }
            $("#Invoicer").html(s);
        }
    });
}