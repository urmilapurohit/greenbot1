
window.onload = function () {
    //InitializeClickEventForJobTypeClickEvent(); 
}
function InitializeClickEventForJobTypeClickEvent() {
    if (userTypeId == 7) {
        //ValidateFieldBasedOnWorkType();
        $('#IsPVDUser,#IsSWHUser').click(function (e) {
            if ($(this).attr('readonly') == 'readonly') {
                e.preventDefault();
                return false;
            }
            var result = ValidateFieldBasedOnWorkType();
            return result;
        });
        if ($("#IsSWHUser").prop("checked") == false && $("#IsPVDUser").prop("checked") == false) {
            $("#IsPVDUser").prop('checked', true);// By Default Value will be set as PVD user
        }
    }
}
function ClearAllJobTypeCheckBoxValue() {
    $("#IsPVDUser").prop('checked', false);
    $("#IsSWHUser").prop('checked', false);
}
function ValidateFieldBasedOnWorkType() {
    var isPVDUser = $("#IsPVDUser").prop('checked');
    var isSWHUser = $("#IsSWHUser").prop('checked');

    if (isPVDUser) {
        $("#CECAccreditationNumber").rules("add", {
            required: true,
            messages: {
                required: "CEC Accreditation Number is required."
            }
        });
        $("#lblCECAccreditationNumber").addClass("required");
        $(".SERole").show();
    }
    else {
        $("#CECAccreditationNumber").rules("add", {
            required: false,
        });
        $(".SERole").hide();
        $("#lblCECAccreditationNumber").removeClass("required");
    }


    if (isSWHUser) {
        $("#ElectricalContractorsLicenseNumber").rules("add", {
            required: true,
            messages: {
                required: "License Number is required."
            }
        });
        $("#lblElectricalContractorsLicenseNumber").addClass("required");
    }
    else {
        $("#ElectricalContractorsLicenseNumber").rules("add", {
            required: false,
        });
        $("#lblElectricalContractorsLicenseNumber").removeClass("required");
    }

    //if ($("#IsVerified").val().toString().toLowerCase() == "false") {
    //    $("#Reason").rules("add", {
    //        required: true,
    //        messages: {
    //            required: "Failure Reason is required."
    //        }
    //    });
    //    $("#Reason").addClass("required");
    //}
    //else {
    //    $("#Reason").rules("add", {
    //        required: false,
    //    });
    //    $("#Reason").removeClass("required");
    //}
}