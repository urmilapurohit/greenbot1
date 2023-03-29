
window.onload = function () {
    //InitializeClickEventForJobTypeClickEvent(); 
}
function InitializeClickEventForJobTypeClickEvent() {
    if (userTypeId == 7) {
        //ValidateFieldBasedOnWorkType();
        $('#IsPVDUser,#IsSWHUser').click(function (e) {
            if ($(this).attr('readonly') == 'readonly')
            {
                e.preventDefault();
                return false;
            }
            var result = ValidateFieldBasedOnWorkType();
            return result;
        });
        if ($("#IsSWHUser").prop("checked") == false && $("#IsPVDUser").prop("checked") == false) {
            $("#IsPVDUser").prop('checked', true);// By Default Value will be set as PVD user
        }
        //else {
        //    if ($("#IsPVDUser").prop("checked") == false) {
        //        $("#CECAccreditationNumber").rules("add", {
        //            required: false
        //        });
        //        $("#lblCECAccreditationNumber").removeClass("required");
        //    }
        //    else if ($("#IsSWHUser").prop("checked") == false) {
        //        $("#ElectricalContractorsLicenseNumber").rules("add", {
        //            required: false
        //        });
        //        $("#lblElectricalContractorsLicenseNumber").removeClass("required");
        //    }
        //}
    }
}
function ClearAllJobTypeCheckBoxValue() {
    $("#IsPVDUser").prop('checked', false);
    $("#IsSWHUser").prop('checked', false);
    //$("#IsVEECUser").prop('checked', false);
}
function ValidateFieldBasedOnWorkType() {
    var isPVDUser = $("#IsPVDUser").prop('checked');
    var isSWHUser = $("#IsSWHUser").prop('checked');
    //var isVEECUser = $("#IsVEECUser").prop('checked');

    //if (!isPVDUser && !isSWHUser) {// && !isVEECUser) {
    //    alert('At least one work type should be selected.');
    //    return false;
    //}
    //else {
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
        else
        {
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
        else
        {
            $("#ElectricalContractorsLicenseNumber").rules("add", {
                required: false,
            });
            $("#lblElectricalContractorsLicenseNumber").removeClass("required");
        }


        //if (isPVDUser && !isSWHUser && !isVEECUser) {
        //    $("#ElectricalContractorsLicenseNumber").rules("add", {
        //        required: false,
        //    });
        //    $("#lblElectricalContractorsLicenseNumber").removeClass("required");
        //}
        //if (!isPVDUser && isSWHUser && !isVEECUser) {
        //    $("#CECAccreditationNumber").rules("add", {
        //        required: false,
        //    });
        //    $(".SERole").hide();
        //    $("#lblCECAccreditationNumber").removeClass("required");
        //}
       
        //return true;
    //}
}