var closeButton = '<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">&times;</button>';
var BaseURL = location.origin;
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    if (!evt.ctrlKey) {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
    return true;
}

function isInt(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function isAlphaNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
        return false;
    }
    return true;
}
function checkExist(obj, title, SpvUserID) {
    var UserTypeId = $('#SpvUserTypeId').val();
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    if (uservalue != "" && uservalue != undefined) {
        $.ajaxSetup({ cache: false });
        $.ajax(
        {
            url: BaseURL + '/SpvUser/CheckUserExist',
            data: { UserValue: uservalue, FieldName: fieldName, SpvUserId: SpvUserID, UserTypeId: UserTypeId },
            contentType: 'application/json',
            method: 'get',
            async: false,
            cache: false,
            success: function (data) {
                if (data == false) {
                    chkvar = false;
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + "User with same " + title + " already exists. Please try with different " + title + ".");
                    $("#errorMsgRegion").show();
                }
                else {
                    chkvar = true;
                }
                if (fieldName == "UserName") {
                    chkUserName = chkvar;
                }
                return false;
            }
        });
    }
}

function showErrorMessages(obj, title) {
    if (obj == false) {
        //$(".alert").hide();
        $("#errorMsgRegion").html(closeButton + title);
        $("#errorMsgRegion").show();

        return false;
    }
    else
        return true;
}