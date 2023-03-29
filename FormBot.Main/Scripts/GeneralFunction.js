var InstallationJsonCommon = [];
var OwnerJsonCommon = [];
var OwnerAddressJsonCommon = [];
var InstallerJsonCommon = [];
var InstallerAddressJsonCommon = [];
function GetUserAddress(popupName, controlPrefix, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, IsElectricianAddress, IsDesignerAddress) {
    var data = [];
    if (IsInstalationAddress) {
        data = InstallationJsonCommon;
    }
    else if (IsOwnerAddress) {
        data = OwnerAddressJsonCommon;
    }
    else if (IsInstallerAddress) {
        data = InstallerAddressJsonCommon;
    }
    $("#" + controlPrefix + "_UnitTypeID").val(data[0].UnitTypeId);
    $("#" + controlPrefix + "_UnitNumber").val(data[0].UnitNumber);
    $("#" + controlPrefix + "_StreetNumber").val(data[0].StreetNumber);
    $("#" + controlPrefix + "_StreetName").val(data[0].StreetName);
    $("#" + controlPrefix + "_StreetTypeID").val(data[0].StreetTypeId);
    $("#" + controlPrefix + "_IsPostalAddress").val(data[0].PostalAddressID);
    $("#" + controlPrefix + "_Town").val(data[0].Town);
    $("#" + controlPrefix + "_State").val(data[0].State);
    $("#" + controlPrefix + "_PostCode").val(data[0].PostCode);
    $("#" + controlPrefix + "_PostalAddressID").val(data[0].PostalDeliveryType);
    $("#" + controlPrefix + "_PostalDeliveryNumber").val(data[0].PostalDeliveryNumber);
    $("#" + popupName).find('input[type=text]').each(function () {
        $(this).removeClass("input-validation-error");
        $(this).next(".field-validation-error").removeClass("field-validation-error").removeClass("field-validation-valid").addClass("field-validation-valid");
    });
    if (data[0].StreetTypeId != '' && data[0].StreetTypeId > 0) {
        $("#" + controlPrefix + "_StreetTypeID").removeClass("input-validation-error");
        $("#" + controlPrefix + "_StreetTypeID").next(".field-validation-error").removeClass("field-validation-error").removeClass("field-validation-valid").addClass("field-validation-valid");
    }
}
function SetUserAddress(popupName, controlPrefix, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress) {
    
    var JsonData = {
        UnitTypeId: $("#" + controlPrefix + "_UnitTypeID").val(),
        UnitNumber: $("#" + controlPrefix + "_UnitNumber").val(),
        StreetNumber: $("#" + controlPrefix + "_StreetNumber").val(),
        StreetName: $("#" + controlPrefix + "_StreetName").val(),
        StreetTypeId: $("#" + controlPrefix + "_StreetTypeID").val(),
        PostalAddressID: $("#" + controlPrefix + "_IsPostalAddress").val(),
        Town: $("#" + controlPrefix + "_Town").val(),
        State: $("#" + controlPrefix + "_State").val(),
        PostCode: $("#" + controlPrefix + "_PostCode").val(),
        PostalDeliveryType: $("#" + controlPrefix + "_PostalAddressID").val(),
        PostalDeliveryNumber: $("#" + controlPrefix + "_PostalDeliveryNumber").val(),
    };
    if (IsInstalationAddress) {
        AddDataInQueue(InstallationJsonCommon, JsonData);
    }
    else if (IsOwnerAddress) {
        AddDataInQueue(OwnerAddressJsonCommon, JsonData);
    }
    else if (IsInstallerAddress) {
        AddDataInQueue(InstallerAddressJsonCommon, JsonData);
    }
}
function validateAddress(popupName, controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress, FormName) {
    addressValidationRules(controlPrefix);
    if ($('#' + FormName).valid()) {
        PopupToggle(popupName);
        SetUserAddress(popupName, controlPrefix, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress);
        DisplayAddress(controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress);
    }
}
function addressValidationRules(controlPrefix) {
    var isValidLocation = true;
    if ($("#" + controlPrefix + "_AddressID").val() == 1) {
        $("#" + controlPrefix + "_UnitTypeID").rules("add", {
            required: false,
        });
        $("#" + controlPrefix + "_UnitNumber").rules("add", {
            required: false,
        });
        if (($("#" + controlPrefix + "_UnitTypeID").val() == "" || $("#" + controlPrefix + "_UnitTypeID").val() == null)&& $("#" + controlPrefix + "_UnitNumber").val() == "") {
            //$('#' + controlPrefix + '_UnitTypeID').parent("label").removeClass("required");
            //$('#' + controlPrefix + '_UnitNumber').parent("label").removeClass("required");
            $("label[for~='" + controlPrefix + "_UnitNumber']").removeClass("required");
            $("label[for~='" + controlPrefix + "_UnitTypeID']").removeClass("required");
            $("#" + controlPrefix + "_UnitNumber").rules("add", {
                required: false
                
            });
            $("#" + controlPrefix + "_UnitTypeID").rules("add", {
                required: false
                
            });

            $("#" + controlPrefix + "_UnitNumber").next("span").attr('class', 'field-validation-valid');
            //$('#' + controlPrefix + '_StreetNumber').parent("label").addClass("required");
            $("label[for~='" + controlPrefix + "_StreetNumber']").addClass("required");
            $("#" + controlPrefix + "_StreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }
        if ($("#" + controlPrefix + "_UnitTypeID").val() > 0 && ($("#" + controlPrefix + "_UnitNumber").val().toString().trim() != "" || $("#" + controlPrefix + "_UnitNumber").val() !=null) ) {
            //$('#' + controlPrefix + '_StreetNumber').parent("label").removeClass("required");                 // Ashish Christain: Only working in veec job
            $("label[for~='" + controlPrefix + "_StreetNumber']").removeClass("required");
            $("#" + controlPrefix + "_StreetNumber").rules("add", {
                required: false
            });
            //$('#' + controlPrefix + '_UnitNumber').parent("label").removeClass("required");
            //$('#' + controlPrefix + '_UnitTypeID').parent("label").removeClass("required");
            $("label[for~='" + controlPrefix + "_UnitNumber']").addClass("required");
            $("label[for~='" + controlPrefix + "_UnitTypeID']").addClass("required");
            $("#" + controlPrefix + "_UnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $("#" + controlPrefix + "_UnitTypeID").rules("add", {
                required: true,
                messages: {
                    required: "Unit Type is required."
                }
            });
        }
        if ($("#" + controlPrefix + "_UnitTypeID").val() > 0 && (($("#" + controlPrefix + "_UnitNumber").val()).toString().trim() == "" || $("#" + controlPrefix + "_UnitNumber").val() ==null)) {
            $("#" + controlPrefix + "_UnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            //$('#' + controlPrefix + '_UnitNumber').parent("label").addClass("required");
            //$('#' + controlPrefix + '_StreetNumber').parent("label").removeClass("required");
            $("label[for~='" + controlPrefix + "_UnitNumber']").addClass("required");
            $("label[for~='" + controlPrefix + "_UnitTypeID']").addClass("required");
            $("label[for~='" + controlPrefix + "_StreetNumber']").removeClass("required");
            $("#" + controlPrefix + "_StreetNumber").rules("add", {
                required: false
            });
        }
        if (($("#" + controlPrefix + "_UnitTypeID").val() == "" || $("#" + controlPrefix + "_UnitTypeID").val() == null) &&( $("#" + controlPrefix + "_UnitNumber").val().toString().trim() != "" )) {

            /*Start 07Feb2020 Ashish Christian: As per the discussion with hus. Street number will not requied when unit type and unit number if any value add in address. So commnetd old code and write new logic blow*/
            ////$('#' + controlPrefix + '_UnitNumber').parent("label").addClass("required");
            ////$('#' + controlPrefix + '_UnitTypeID').parent("label").removeClass("required");
            //$("label[for~='" + controlPrefix + "_UnitNumber']").removeClass("required");
            //$("label[for~='" + controlPrefix + "_UnitTypeID']").removeClass("required");
            //$("#" + controlPrefix + "_UnitNumber").rules("add", {
            //    required: false,
            //});
            //$("#" + controlPrefix + "_UnitTypeID").rules("add", {
            //    required: false,
            //});
            ////$('#' + controlPrefix + '_StreetNumber').parent("label").addClass("required");
            //$("label[for~='" + controlPrefix + "_StreetNumber']").addClass("required");
            //$("#" + controlPrefix + "_StreetNumber").rules("add", {
            //    required: true,
            //    messages: {
            //        required: "Street Number is required."
            //    }
            //});


            $("label[for~='" + controlPrefix + "_UnitNumber']").addClass("required");
            $("label[for~='" + controlPrefix + "_UnitTypeID']").addClass("required");
            $("#" + controlPrefix + "_UnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $("#" + controlPrefix + "_UnitTypeID").rules("add", {
                required: true,
                messages: {
                    required: "Unit Type is required."
                }
            });
            $("label[for~='" + controlPrefix + "_StreetNumber']").removeClass("required");
            $("#" + controlPrefix + "_StreetNumber").rules("add", {
                required: false
               
            });
            /*End 07Feb2020 Ashish Christian: As per the discussion on call with hus. Street number will not requied when unit type and unit number if any value add in address. So commnetd old code and write new logic blow*/
        }
        if ($("#" + controlPrefix + "_StreetTypeID").val() == "" || $("#" + controlPrefix + "_StreetTypeID").val() == null) {
            $("#" + controlPrefix + "_StreetTypeID").rules("add", {
                required: true,
                messages: {
                    required: "Street Type is required."
                }
            });
        }
        if ($("#" + controlPrefix + "_StreetName").val() == "" || $("#" + controlPrefix + "_StreetName").val() == null) {
            $("#" + controlPrefix + "_StreetName").rules("add", {
                required: true,
                messages: {
                    required: "Street Name is required."
                }
            });
        }
    }
    else {
        if ($("#" + controlPrefix + "_PostalAddressID").val() == "" || $("#" + controlPrefix + "_PostalAddressID").val() == null) {
            $("#" + controlPrefix + "_PostalAddressID").rules("add", {
                required: true,
                messages: {
                    required: "Postal Delivery Type is required."
                }
            });
        }
        if ($("#" + controlPrefix + "_PostalDeliveryNumber").val() == "" || $("#" + controlPrefix + "_PostalDeliveryNumber").val() == null) {
            $("#" + controlPrefix + "_PostalDeliveryNumber").rules("add", {
                required: true,
                messages: {
                    required: "Postal Delivery Number is required."
                }
            });
        }
    }
    if ($("#" + controlPrefix + "_UnitTypeID").val() == "" || $("#" + controlPrefix + "_UnitTypeID").val() == null) {
        $("#" + controlPrefix + "_UnitTypeID").val(0);
        //$("#" + controlPrefix + "_UnitTypeID").val($("#" + controlPrefix + "_UnitTypeID option:first").val());
    }
    if ($("#" + controlPrefix + "_StreetTypeID").val() == "" || $("#" + controlPrefix + "_StreetTypeID").val() == null ) {
        $("#" + controlPrefix + "_StreetTypeID").val(0);
    }

    //validate town, state and postcode combination.

    if ($("#" + controlPrefix + "_Town").val().length > 0 && $("#" + controlPrefix + "_State").val().length > 0 && $("#" + controlPrefix + "_PostCode").val().length > 0) {
        $.ajax({
            type: 'GET',
            url: actionProcessRequest,
            dataType: 'json',
            data: {
                excludePostBoxFlag: true,
                q: $("#" + controlPrefix + "_Town").val().substring(0, 3)
            },
            async: false,
            success: function (data) {
                var data1 = JSON.parse(data);
                //var obj = data1.localities.locality;


                if (data1.localities.locality != undefined && data1.localities.locality != null) {
                    if (data1.localities.locality.length > 0) {
                        $.each(data1.localities.locality, function () {
                            isValidLocation = false;
                            if (this.location.toLowerCase() == $("#" + controlPrefix + "_Town").val().toLowerCase() &&
                                this.state.toLowerCase() == $("#" + controlPrefix + "_State").val().toLowerCase() &&
                                this.postcode == $("#" + controlPrefix + "_PostCode").val()
                            ) {
                                isValidLocation = true;
                                return false;
                            }
                        });
                    }
                    else if (data1.localities.locality.location != undefined && data1.localities.locality.location != null) {
                        var obj = data1.localities.locality;
                        if (obj.location.toLowerCase() == $("#" + controlPrefix + "_Town").val().toLowerCase() &&
                            obj.state.toLowerCase() == $("#" + controlPrefix + "_State").val().toLowerCase() &&
                            obj.postcode == $("#" + controlPrefix + "_PostCode").val()
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


                //$.each(obj, function () {
                //    isValidLocation = false;
                //    if (data1.localities.locality.length > 0) {
                //        if (this.location.toLowerCase() == $("#" + controlPrefix + "_Town").val().toLowerCase() &&
                //            this.state.toLowerCase() == $("#" + controlPrefix + "_State").val().toLowerCase() &&
                //            this.postcode == $("#" + controlPrefix + "_PostCode").val()
                //        ) {
                //            isValidLocation = true;
                //            return false;
                //        }
                //    }
                //    else {
                //        if (data1.localities.locality.location.toLowerCase() == $("#" + controlPrefix + "_Town").val().toLowerCase() &&
                //            data1.localities.locality.state.toLowerCase() == $("#" + controlPrefix + "_State").val().toLowerCase() &&
                //            data1.localities.locality.postcode == $("#" + controlPrefix + "_PostCode").val()
                //        ) {
                //            isValidLocation = true;
                //            return false;
                //        }
                //    }
                //})
                if (!isValidLocation) {
                    $("#" + controlPrefix + "_LocationValidation").show();
                }
                else {
                    $("#" + controlPrefix + "_LocationValidation").hide();
                }
            }
        })
    }
    return isValidLocation;
}

function clearAddress(popupName, controlPrefix) {
    $('#' + popupName).find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $('#' + popupName).find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#" + controlPrefix + "_UnitTypeID").val($("#" + controlPrefix + "_UnitTypeID option:first").val());
    $("#" + controlPrefix + "_StreetTypeID").val($("#" + controlPrefix + "_StreetTypeID option:first").val());
    $("#" + controlPrefix + "_PostalAddressID").val($("#" + controlPrefix + "_PostalAddressID option:first").val());
    $("#" + controlPrefix + "_StreetTypeID").removeClass("input-validation-error");
    $("#" + controlPrefix + "_StreetTypeID").next(".field-validation-error").removeClass("field-validation-error").removeClass("field-validation-valid").addClass("field-validation-valid");

}
/**********************Address End*************************/



function PopupToggle(popupName) {
    $('#' + popupName).modal('toggle');
}
function AddDataInQueue(target, data) {
    if (target.length > 0) {
        target[0] = data;
    }
    else {
        target.push(data);
    }
}
function DisplayAddress(controlPrefix, AddressTagId, IsOwnerAddress, IsInstallerAddress, IsInstalationAddress) {
    
    var addressLine1, addressLine2, addressLine3, streetAddress, postCodeAddress, companyName, ownerName, fullAddress;
    var PostalDeliveryType = $("#" + controlPrefix + "_PostalAddressID").val() > 0 ? $("#" + controlPrefix + "_PostalAddressID option:selected").text() : "";
    var UnitTypeName = $("#" + controlPrefix + "_UnitTypeID").val() > 0 ? $("#" + controlPrefix + "_UnitTypeID option:selected").text() : "";
    var StreetNumber = $("#" + controlPrefix + "_StreetNumber").val();
    var StreetName = $("#" + controlPrefix + "_StreetName").val();
    var StreetTypeName = $("#" + controlPrefix + "_StreetTypeID").val() > 0 ? $("#" + controlPrefix + "_StreetTypeID option:selected").text() : "";
    var PostalDeliveryNumber = $("#" + controlPrefix + "_PostalDeliveryNumber").val();
    var UnitNumber = $("#" + controlPrefix + "_UnitNumber").val();
    PostalDeliveryType = (PostalDeliveryType == "" || PostalDeliveryType == null) ? "" : PostalDeliveryType;
    UnitTypeName = (UnitTypeName == "" || UnitTypeName == null) ? "" : UnitTypeName;
    StreetNumber = (StreetNumber == "" || StreetNumber == null) ? "" : StreetNumber;
    streetAddress = StreetNumber + ((StreetName == "" || StreetName == null) ? "" : ' ' + StreetName) + ((StreetTypeName == "" || StreetTypeName == null) ? '' : ' ' + StreetTypeName);
    postCodeAddress = $("#" + controlPrefix + "_Town").val() + ' ' + $("#" + controlPrefix + "_State").val() + ' ' + $("#" + controlPrefix + "_PostCode").val();
    if ($("#" + controlPrefix + "_IsPostalAddress").val() == 1) {
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
        //P.o.box
        addressLine1 = PostalDeliveryType + ((PostalDeliveryNumber == "" || PostalDeliveryNumber == null) ? "" : ' ' + PostalDeliveryNumber);
        addressLine2 = postCodeAddress;
    }
    if (IsOwnerAddress) {
        companyName = $("#" + controlPrefix + "_CompanyName").val();
        ownerName = $("#" + controlPrefix + "_FirstName").val() + ' ' + $("#" + controlPrefix + "_LastName").val();
        fullAddress = companyName + '</br>' + ownerName + '</br>' + addressLine1 + '</br>' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br>' + addressLine3 : '');
    } else if (IsInstallerAddress) {
        companyName = $("#" + controlPrefix + "_CompanyName").val();
        installerName = $("#" + controlPrefix + "_FirstName").val() + ' ' + $("#" + controlPrefix + "_LastName").val();
        fullAddress = installerName + '</br>' + addressLine1 + '</br>' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br>' + addressLine3 : '');
    } else if (IsInstalationAddress) {
        fullAddress = addressLine1 + '</br>' + addressLine2 + (addressLine3 != undefined && addressLine3 != "" && addressLine3 != null ? '</br>' + addressLine3 : '');
        getLatitudeLongitude(DisplayLatLonOfInstallationAdd, fullAddress);
    }
    $("#" + AddressTagId + "Add").html(fullAddress);
    if (!IsInstalationAddress) {
        var email = $("#" + controlPrefix + "_Email").val();
        $("#" + AddressTagId + "Phone").html($("#" + controlPrefix + "_Phone").val());
        $("." + AddressTagId + "Email").html($("#" + controlPrefix + "_Email").val());
        $("." + AddressTagId + "Email").attr('href', "mailto:'" + email + "'");
        $("." + AddressTagId + "Email").attr('title', $("#" + controlPrefix + "_Email").val());
    }
}
function HideShowAddressFieldOverAddressType(AddressType, PDAclass, DPAclass) {
    if (AddressType == 1) {
        $('.' + DPAclass).show();
        $('.' + PDAclass).hide();
    }
    else {
        $('.' + PDAclass).show();
        $('.' + DPAclass).hide();
    }
}
function IsPostalAddressSelected(drpId, IsPostalAddress) {
    if (IsPostalAddress == "true") {
        $('#' + drpId).val("2");
    }
    else {
        $('#' + drpId).val("1");
    }
}

function ChangeUnitTypeId(controlPrefix) {
    if ($('#' + controlPrefix + '_UnitTypeID option:selected').val() == "" && $('#' + controlPrefix + '_UnitNumber').val() == "" ) {
        $("label[for~='" + controlPrefix + "_UnitNumber']").removeClass("required");
        $("label[for~='" + controlPrefix + "_UnitTypeID']").removeClass("required");
        $("label[for~='" + controlPrefix + "_StreetNumber']").addClass("required");
    }
    else {
        $("label[for~='" + controlPrefix + "_UnitNumber']").addClass("required");
        $("label[for~='" + controlPrefix + "_UnitTypeID']").addClass("required");
        $("label[for~='" + controlPrefix + "_StreetNumber']").removeClass("required");
    }
}

function htmlEncode(value) {
    return $('<textarea/>').text(value).html();
}

function fnNiceScroll() {
    $(".niceScroll").niceScroll({
        cursorcolor: "#c1c1c1",
        background: "#f2f2f2",
        cursorwidth: 8,
        cursorborder: "none",
        cursorborderradius: 0,
        autohidemode: true,
        bouncescroll: false,
        horizrailenabled: true,
        railvalign: "bottom"
    });
}

function fnNiceScrollResize() {
    $(".niceScroll").getNiceScroll().resize();
}
function CheckSpecialCharInFileName(filename) {
    if (filename.indexOf('[') != -1 || filename.indexOf('^') != -1 || filename.indexOf(']') != -1) {
        return true;
    } else {
        return false;
    }
}

function ShowErrorMsgForFileName(msg) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + msg);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}
jQuery.fn.onPositionChanged = function (trigger, millis) {
    if (millis == null) millis = 100;
    var o = $(this[0]); // our jquery object
    if (o.length < 1) return o;

    var lastPos = null;
    var lastOff = null;
    setInterval(function () {
        if (o == null || o.length < 1) return o; // abort if element is non existend eny more
        if (lastPos == null) lastPos = o.position();
        if (lastOff == null) lastOff = o.offset();
        var newPos = o.position();
        var newOff = o.offset();
        if (lastPos.top != newPos.top || lastPos.left != newPos.left) {
            $(this).trigger('onPositionChanged', { lastPos: lastPos, newPos: newPos });
            if (typeof (trigger) == "function") trigger(lastPos, newPos);
            lastPos = o.position();
        }
        if (lastOff.top != newOff.top || lastOff.left != newOff.left) {
            $(this).trigger('onOffsetChanged', { lastOff: lastOff, newOff: newOff });
            if (typeof (trigger) == "function") trigger(lastOff, newOff);
            lastOff = o.offset();
        }
    }, millis);

    return o;
};
function fnNiceScroll() {
    $(".niceScroll").niceScroll({
        cursorcolor: "#c1c1c1",
        background: "#f2f2f2",
        cursorwidth: 8,
        cursorborder: "none",
        cursorborderradius: 0,
        autohidemode: true,
        bouncescroll: false,
        horizrailenabled: true,
        railvalign: "bottom"
    });
}

function fnNiceScrollResize() {
    $(".niceScroll").getNiceScroll().resize();
}

function SetPageSizeForGrid(pageSize,isKendoGrid,viewPageId) {
    var obj =
    {
        IsKendoGrid: isKendoGrid,
        PageSize: pageSize,
        ViewPageId: viewPageId
    }
    $.ajax({
        url: "/Job/InsertUpdateUserWiseGridConfiguration",
        type: "POST",
        async: false,
        data: JSON.stringify({ objUserWiseGridConfiguration: obj }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {

        },
        error: function (ex) {

        },
    });
}
function showSnackbar(msg, timeout, title, id) {
    SnackBar({
        message: msg,
        timeout: timeout,
        NotificationTitle: title,
        SnackbarId: id
    });
}
function IsHaveInvelidChar(str) {
    var IsHaveInvelidChar = false; 
    for (var i = 0; i < str.length; i++) {
        
        if (!((str[i].charCodeAt(0) >= 32 && str[i].charCodeAt(0) <= 127) || str[i].charCodeAt(0) == 10)) {
            IsHaveInvelidChar = true;
        }
    }
    return IsHaveInvelidChar;
}