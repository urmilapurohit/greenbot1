$(document).ready(function () {
    
    //Timer(Sessiontimeout - 1);   
    $('.disableMenu').removeClass('disableMenu');
    if ($(".menu .active").length > 1) {
        $('.menu').find('li').first().removeClass('active');
    }

    $('.menu li a').click(function () {
        currentMenu = $(this).attr('id');
        document.cookie = "menuname" + "=" + $(this).attr('id') + "" + "; path=/";
    });

    $("#saveChangeUser").click(function () {
        if ($("#hdnUserId").val() > 0)
            RedirectFSAToUser($("#hdnUserId").val());
        else
            alert('Please select user.');
    });

    $("#resetChangeUser").click(function () {
        $("#UserTypeIdFSA").val(1);
        $(".userDropdown").show();
        var URL = ProjectImagePath + 'SpvUser/' + 'GetSpvUserByUserTypeId/SpvUser?userTypeId=' + 1 + '&isTypeChange=' + 1;
        BindUsersByUserType(URL, '');
    });

    $("#btnClosepopupboxChangeUser").click(function () {
        $('#popupboxChangeUser').modal('toggle');
    });

    $('#searchUser').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnUserId").val('');
            $.each(userList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchUser").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnUserId").val(ui.item.id); // save selected id to hidden input
        }
    }).on('focus', function () { $(this).keydown(); });

    $("#searchUser").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");

        ul.addClass("autocompleteChangeUser");

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);

    };

    //$.ajaxSetup({ cache: false });
    $(document).ajaxStart(function () {
        $("#loading-image").show();
    });

    $(document).ajaxComplete(function () {
        $("#loading-image").hide();
    });

    $(document).ajaxError(function (xhr, errorType, settings, exception) {

        var Errortype = xhr.type;
        var status = errorType.status;
        var exceptionMsg = exception;
        var requestedUrl = settings.url;

        try {
            insertErrorLog(Errortype, status, exceptionMsg, requestedUrl)
        }
        catch (e)
        {
        }
    });

    //if (ProjectSession_Logo != null && ProjectSession_Theme != null && ProjectSession_Logo != '' && ProjectSession_Theme != '') {
    //    if (ProjectSession_Logo == 'Images/logo.png') {
    //        $('#imgLogo').attr('src', (ProjectSession_SiteUrlBase + ProjectSession_Logo));
    //    }
    //    else {
    //        $('body').attr('id', ProjectSession_Theme);
    //        var proofDocumentURL = UploadedDocumentPath;
    //        var logo = ProjectSession_Logo;
    //        var SRC = proofDocumentURL + 'UserDocuments' + '/' + logo;
    //        $('#imgLogo').attr('src', SRC);
    //        $('.hd-top .logo img').css('height', '60px');
    //        $('.hd-top .logo img').css('margin-top', '0px');
    //    }
    //    $('body').attr('id', ProjectSession_Theme);
    //}

    //$("body").find("form").submit(function (e) {
    //    if (typeof (validateExtraFields) == "function") {
    //        if (validateExtraFields() == false) {
    //            return false;
    //        }
    //    }

    //});

    $(document).attr('title', 'GreenBot - ' + $('h1:first').text());
    window.history.forward();
    
    setKeepSessionAlive(Sessiontimeout - 1);    

    var UserTypeChange = function () {
        ShowDropDownBasedOnUserType($("#UserTypeIdFSA").val(), 1, '');
    };
    // Bind UserType Dropdown
    if (ProjectSession_UserTypeId == 1 || ProjectSession_IsUserFSA == 'True') {

        FillDropDownForChangeUser('UserTypeIdFSA', ProjectImagePath + 'SpvUser/' + 'GetSpvUserType/SpvUser', 1, true, null);

        ShowDropDownBasedOnUserType($("#UserTypeIdFSA").val(), 0, ProjectSession_LoggedInUserId);

        $("#UserTypeIdFSA").unbind("change", UserTypeChange);
        $("#UserTypeIdFSA").bind("change", UserTypeChange);
    }
});




function noBack() { window.history.forward(); }

function setKeepSessionAlive(time) {
    setTimeout("KeepSessionAlive()", time * 60000); // every 5 min
}

function KeepSessionAlive() {
    var keepSessionAliveUrl = ProjectImagePath + 'Base/KeepSessionAlive';
    $.ajax({
        type: "POST",
        url: keepSessionAliveUrl,
        success: function () { setKeepSessionAlive(Sessiontimeout - 1); }
    });
}

function insertErrorLog(Errortype, status, exceptionMsg, requestedUrl) {

    $.ajax({
        url: ProjectImagePath + 'Account/' + 'InsertErrorintoLogFile/Account',
        type: "POST",
        dataType: "json",
        data: JSON.stringify({ strErrortype: Errortype, strstatus: status, strexception: exceptionMsg, url: requestedUrl }),
        global: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            //alert("Success");
        },
        error: function (e) {
            //alert("!Error");
        }
    });
}

$(document).on("click", "#closeErrorPopup", function () {
    $('#dialog').modal('hide');
});

//open change user popup
function ChangeUserLink() {
    $('#popupboxChangeUser').modal({ backdrop: 'static', keyboard: false });
}

function FillDropDownForChangeUser(id, url, value, hasSelect, callback, defaultText) {

    if (ProjectSession_SystemUserType && ProjectSession_SystemUserType.length != 0) {
        var SystemUserType_Value = JSON.parse(ProjectSession_SystemUserType.replace(/&quot;/g, '"'));
        DropDownForChangeUser_Values(SystemUserType_Value, id, url, value, hasSelect, callback, defaultText);
    }
    else {

        $.ajax(
            {
                url: url,
                contentType: 'application/json',
                dataType: 'json',
                method: 'get',
                async: false,
                cache: false,
                success: function (success) {
                    DropDownForChangeUser_Values(success, id, url, value, hasSelect, callback, defaultText);
                }
            });
    }
}

function DropDownForChangeUser_Values(SystemUserType_Value, id, url, value, hasSelect, callback, defaultText) {

    var options = '';
    $.each(SystemUserType_Value, function (i, val) {
        options += '<option value = "' + val.Value + '" >' + val.Text + '</option>'
    });
    $("#" + id).html(options);
    if (value && value != '' && value != 0) {
        $("#" + id).val(value);
    }
    if ($('#' + id).attr('selval') && $('#' + id).attr('selval') > 0) {
        $("#" + id).val($('#' + id).attr('selval'));
    }
    if ($("#" + id).selectpicker != undefined) {
        $("#" + id).selectpicker('refresh');
    }
    if (callback != undefined) {
        callback();
    }
    if (id == 'UserTypeIdFSA' && ProjectSession_IsUserFSA == 'True') {
        $("#UserTypeIdFSA").val(ProjectSession_UserTypeId);
    }
    //if ($("#UserTypeIdFSA").val() == "1" || $("#UserTypeIdFSA").val() == "0")
    if ($("#UserTypeIdFSA").val() == "0")
        $(".userDropdown").hide();
    else
        $(".userDropdown").show();
}

function ShowDropDownBasedOnUserType(userTypeId, isTypeChange, userValue) {
    if (userTypeId == 1 || userTypeId == 3) {
        var URL = ProjectImagePath + 'SpvUser/' + 'GetSpvUserByUserTypeId/SpvUser?userTypeId=' + userTypeId + '&isTypeChange=' + isTypeChange;
        BindUsersByUserType(URL, userValue);
        $(".userDropdown").show();
    }
    else {
        $(".userDropdown").hide();
    }
}

function BindUsersByUserType_Values(Users, selectValue) {

    userList = [];
    $.each(Users, function (i, user) {
        userList.push({ value: user.Value, text: user.Text });
    });

    if (selectValue > 0) {
        $('#hdnUserId').val(selectValue);
        $.each(userList, function (key, value) {
            if (value.value == selectValue) {
                $("#searchUser").val(value.text);
            }
        });
    }
    else {
        $('#hdnUserId').val(userList.length > 0 ? userList[0].value : 0);
        $("#searchUser").val(userList.length > 0 ? userList[0].text : '');
    }
}

function RedirectFSAToUser(userId) {
    $.ajax({
        url: ProjectImagePath + 'Account/' + 'RedirectFSAToUser/Account?userID=' + userId,
        type: "GET",
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result && result.status) {
                var url = ProjectImagePath + 'Dashboard/' + 'Index';  // images are not displaying

                if (result.area != "")
                    result.controller = result.area + "/" + result.controller;

                url = url.replace("Index", result.action).replace("Dashboard", result.controller);
                setTimeout(function () {
                    setTimeout(function () { $("#loading-image").show(); }, 60);
                    window.location.href = url;
                }, 0);
            }
            else {
                alert(result.error);
            }
        },
        error: function (e) {
        }
    });
}

function BindUsersByUserType(URL, selectValue) {
    $("#searchUser").val("");
    if (ProjectSession_SystemUsersOfUserType && ProjectSession_SystemUsersOfUserType.length != 0 && URL.indexOf("isTypeChange=1") < 0) {
        var SystemUsersOfUserType_Value = JSON.parse(ProjectSession_SystemUsersOfUserType.replace(/&quot;/g, '"'));//JsonConvert.DeserializeObject(SystemUserType1);
        BindUsersByUserType_Values(SystemUsersOfUserType_Value, selectValue);
    }
    else {
        $.ajax({
            type: 'get',
            url: URL,
            dataType: 'json',
            //data: data,
            success: function (Users) {
                BindUsersByUserType_Values(Users, selectValue);
            },
            error: function (ex) {
                alert('Failed to retrieve Users.' + ex);
            }
        });
    }
    return false;
}
