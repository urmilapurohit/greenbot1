var fillOwnerAddress;
var OwnerAddressJson = [];

$(document).ready(function () {
    //$("#frmOwnerDetail").validate(); /* Solution for error Cannot read properties of undefined (reading 'settings') at init.rules */
    if (Model_OwnerSignature != null && Model_OwnerSignature != '') {
        SRCOwnerSign = signatureURL + Model_OwnerSignature_Replace;
        $('#imgOwnerSignatureView').hide();


    }


    validateOrganisation();
    debugger;
    if (modelOwnerSign != null && modelOwnerSign != '' && modelOwnerSign != undefined) {
        $("#imgOwnerSign").attr('src', signatureURL + modelOwnerSignURL);
    }

    $("#ownerGetSignFromVisit").click(function (e) {
        GetSignatureFromVisit(1);
        e.preventDefault();
    });

    if ($("#JobOwnerDetails_AddressID").val() == 1)
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", false)
    else
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop("disabled", true)

    fillOwnerAddressDetail();

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

    debugger;
    dropDownData = [];

    //If Data is already loaded in _source from another tab then no need to fetch it again from Server.
    var lsUnitType = _source.filter(a => a.id == "UnitType");
    if (lsUnitType.length > 0) {
        FillDropDown2("JobOwnerDetails_UnitTypeID", lsUnitType[0].data, OwnerUnitTypeID, true, fillOwnerAddress, null, 'UnitTypeName', 'UnitTypeID');
    }

    var lsStreetType = _source.filter(a => a.id == "StreetType");
    if (lsStreetType.length > 0) {
        FillDropDown2("JobOwnerDetails_StreetTypeID", lsStreetType[0].data, OwnerStreetTypeID, true, fillOwnerAddress, null, 'StreetTypeName', 'StreetTypeID');
    }

    var lsPostalAddress = _source.filter(a => a.id == "PostalAddress");
    if (lsPostalAddress.length > 0) {
        FillDropDown2("JobOwnerDetails_PostalAddressID", lsPostalAddress[0].data, OwnerpostalAddressID, true, fillOwnerAddress, null, 'PostalDeliveryType', 'PostalAddressID');
    }

    dropDownData.push({ id: 'JobOwnerDetails_UnitTypeID', key: "UnitType", value: OwnerUnitTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'JobOwnerDetails_StreetTypeID', key: "StreetType", value: OwnerStreetTypeID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'JobOwnerDetails_PostalAddressID', key: "PostalAddress", value: OwnerpostalAddressID, hasSelect: true, callback: fillOwnerAddress, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
    dropDownData.bindDropdown();

    buttonOwnerDetailClick();
    OwnerAutoComplete();

    $("#JobOwnerDetails_OwnerType").change(function (e) {
        //$("#frmOwnerDetail").validate(); /* Solution for error Cannot read properties of undefined (reading 'settings') at init.rules */
        validateOrganisation();
        localStorage.setItem('SEJobOwnerDetails_OwnerType', $("#JobOwnerDetails_OwnerType").val());
        JobOwnerDetails_OwnerType_Glbl = $("#JobOwnerDetails_OwnerType").val();
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

    setTimeout(function () {
        CheckOwnerAddressIsValidOrNot();
    }, 3000);

    localStorage.setItem('SEJobOwnerDetails_OwnerType', $("#JobOwnerDetails_OwnerType").val());
    JobOwnerDetails_OwnerType_Glbl = $("#JobOwnerDetails_OwnerType").val();

    UploadAllSignatureOfJob($("#uploadJobOwnerSign"));
});

function OwnerSignatureView() {
    $("#imgOwnerSign").attr('src', signatureURL + modelOwnerSignURL);
    $("#imgOwnerSign").show();
    $("#loading-image").css("display", "");
    $('#imgOwnerSign').attr('src', SRCOwnerSign).load(function () {
        logoWidth1 = this.width;
        logoHeight1 = this.height;
        //$('#popupOwnerSign').modal({ backdrop: 'static', keyboard: false });

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
            //$('#popupOwnerSign').find(".modal-dialog").width(screen.width - 250);
            $("#imgOwnerSign").width(logoWidth1);
        }
        else {
            $("#imgOwnerSign").width(logoWidth1);
            //$('#popupOwnerSign').find(".modal-dialog").width(logoWidth1);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgOwnerSign").unbind("error");
    $('#imgOwnerSign').attr('src', SRCOwnerSign).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}

function UploadAllSignatureOfJob(objSignUpload) {
    debugger;

    var typeOfSignature = objSignUpload.attr('typeOfSignature');

    objSignUpload.fileupload({
        url: urlUploadJobSignature,
        dataType: 'json',
        done: function (e, data) {
            debugger;
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {

                    var path = signatureURL + 'JobDocuments/' + Model_JobID + '/' + data.result[i].FileName;

                    if (typeOfSignature == 1) {
                        isOwnerSignUpload = true;
                        SRCOwnerSign = path;
                        $("#imgOwnerSignatureView").hide();
                        OwnerSignatureView();
                    }
                    else {
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
            debugger;
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size / (1024 * 1024) > parseInt(documentSizeLimit)) {
                        showErrorMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size / (1024 * 1024) > parseInt(documentSizeLimit)) {
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

function validateOrganisation() {
    debugger;
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

function initializeOwnerDetails(data) {
    $("#JobOwnerDetails_CompanyName").change(function () {
        $.each(data, function (key, value) {
            var Cname = value.CompanyName;
            var drpVal = $('select#JobOwnerDetails_CompanyName option:selected').val();
        });
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

    var fullAddress = companyName + '</br>' + ownerName + '</br > ' + addressLine1 + '</br > ' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br > ' + addressLine3 : '');
    var ownerFullAddress = addressLine1 + ',' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? ',' + addressLine3 : '');
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

$('#aGetOwnerSignature').click(function (e) {
    e.preventDefault();
    $('#mdlGetSignature').modal('show');
});


$('#btnSendMail').click(function (e) {
    debugger;
    SendEmailSignature();
    SearchHistory();
});

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
                debugger;
                //num = $('#InstallerView_Mobile').val().trim();
                //FirstName = $("#InstallerView_FirstName").val();
                //LastName = $("#InstallerView_LastName").val();
                num = InstallerView_Mobile_Glbl;
                FirstName = InstallerView_FirstName_Glbl;
                LastName = InstallerView_LastName_Glbl;
            }
            else {
                num = $('#JobInstallerDetails_Mobile').val().trim();
                FirstName = $("#JobInstallerDetails_FirstName").val();
                LastName = $("#JobInstallerDetails_LastName").val();
            }
            IsOwner = 'No';
            //if (JobType == 1 && $('#InstallerView_Mobile').val().trim() == "") {
            if (JobType == 1 && InstallerView_Mobile_Glbl == "") {
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
            //num = $('#mobileOwner').text().trim();
            num = $('#JobOwnerDetails_Mobile').val();
            IsOwner = 'Yes';
            FirstName = $("#JobOwnerDetails_FirstName").val();
            LastName = $("#JobOwnerDetails_LastName").val();
            //if ($('#mobileOwner').text().trim() == "") {
            if ($('#JobOwnerDetails_Mobile').val() == "") {

                $("#errorMsgRegionSignaturePopup").html("Owner Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
        else if (value == "4") {
            //num = $('#DesignerView_Mobile').val().trim();
            num = DesignerView_Mobile_Glbl;
            //FirstName = $("#DesignerView_FirstName").val();
            //LastName = $("#DesignerView_LastName").val();
            FirstName = DesignerView_FirstName_Glbl;
            LastName = DesignerView_LastName_Glbl;
            IsOwner = 'No';
            //if ($('#DesignerView_Mobile').val().trim() == "") {
            if (DesignerView_Mobile_Glbl == "") {
                $("#errorMsgRegionSignaturePopup").html("Designer Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
        else if (value == "3") {
            //num = $('#JobElectricians_Mobile').val().trim();
            //FirstName = $("#JobElectricians_FirstName").val();
            //LastName = $("#JobElectricians_LastName").val();
            num = JobElectricians_Mobile_Glbl;
            FirstName = JobElectricians_FirstName_Glbl;
            LastName = JobElectricians_LastName_Glbl;
            IsOwner = 'No';
            //if ($('#JobElectricians_Mobile').val().trim() == "") {
            if (JobElectricians_Mobile_Glbl == "") {
                $("#errorMsgRegionSignaturePopup").html("Electrician Mobile Number is required.");
                $("#errorMsgRegionSignaturePopup").show();
                return false;
            }
        }
    }
    else {
        if (value == "2") {
            if (JobType == 1) {
                //email = $('#InstallerView_Email').val();
                //FirstName = $("#InstallerView_FirstName").val();
                //LastName = $("#InstallerView_LastName").val();
                email = InstallerView_Email_Glbl;
                FirstName = InstallerView_FirstName_Glbl;
                LastName = InstallerView_LastName_Glbl;
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
            //email = $('#DesignerView_Email').val();
            email = DesignerView_Email_Glbl;
            message = "Designer Email is required for it."
            IsOwner = 'No';
            //FirstName = $("#DesignerView_FirstName").val();
            //LastName = $("#DesignerView_LastName").val();
            FirstName = DesignerView_FirstName_Glbl;
            LastName = DesignerView_LastName_Glbl;
        }
        else if (value == "3") {
            //email = $('#JobElectricians_Email').val();
            email = JobElectricians_Email_Glbl;
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
                debugger;
                window.onbeforeunload = null;
                $("#showfilteredhistory").html($.parseHTML(Data));
                $("divCustom").mCustomScrollbar();
            }
        });
        AddHistoryIcons();
        tagandsearchfilter();
        hideEditDeleteIcons();
        $("#loading-image").hide();
    }, 10);
}

function AddHistoryIcons() {
    $('.history-block li').each(function () {
        var historycategoryclassname = $(this).attr('class');
        if (typeof (historycategoryclassname) != 'undefined') {
            if (historycategoryclassname.includes("stage-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-building');
            }
            else if (historycategoryclassname.includes("notes-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-note-sticky');
            }
            else if (historycategoryclassname.includes("message-status")) {
                $(this).find('.border-icon').addClass('message1');
            }
            else if (historycategoryclassname.includes("traded-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-circle-dollar-to-slot');
            }
            else if (historycategoryclassname.includes("stc-status")) {
                $(this).find('.border-icon').addClass('fa-solid fa-pen-to-square');
            }
            else if (historycategoryclassname.includes("scheduling-status")) {
                $(this).find('.border-icon').addClass('fa-solid fa-briefcase');
            }
            else if (historycategoryclassname.includes("documents-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-file-lines');
            }
            else if (historycategoryclassname.includes("signature-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-file-signature');
            }
            else if (historycategoryclassname.includes("spvlogs-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-clipboard-check');
            }
            else if (historycategoryclassname.includes("invoice-status")) {
                $(this).find('.border-icon').addClass('fa-regular fa-file-invoice-dollar');
            }
        }
    });
}

function tagandsearchfilter() {
    debugger;
    var count = 0;
    var tagged = $("#relatedTofilter").val();
    var searchfilter = "@" + tagged;
    var searchboxfilter = $("#JobHistorySearch").val();
    if (tagged == "0") {
        $('#divCustom .history-box').each(function () {

            if ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0) {
                $(this).hide();
            } else {
                $(this).show();
                $("#norecords").empty();
                count++;
            }

        });
    }
    else {
        $('#divCustom .history-box').each(function () {

            if (($(this).text().search(new RegExp(searchfilter, "i")) < 0) || ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0)) {
                $(this).hide();
            } else {

                $(this).show();
                $("#norecords").empty();
                count++;
            }
        });
    }
    if (count > 0) {
        $(".history-box").css(
            "border-top", "1px solid #e3e3e3");
    }
    else {
        $("#norecords").empty();
        var norecords = "<div style='text-align:center;font-size:24px;margin-top:120px'>" + "No Record(s) Found." + "</div>";
        $("#norecords").append(norecords);
    }
}

function hideEditDeleteIcons() {
    debugger;
    $('.IconsEditDelete').each(function () {
        $(this).css("display", "none !important");
    });
}

$('#imgOwnerSignatureView').click(function () {
    debugger;
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

$("#btnClosepopupOwnerSign").click(function () {
    $('#popupOwnerSign').modal('toggle');
});

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