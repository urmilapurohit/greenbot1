$(document).ready(function () {

    date = new Date().toISOString();
    window.dataLayer = window.dataLayer || [];
    gtag('js', new Date());
    gtag('config', 'UA-114902803-1');

    $.ajax({
        url: getNotificationURL + date,
        method: 'POST',
        cache: false,
        async: false,
        success: function (Data) {
            if (Data) {
                Data = Data.replace(/(?:\\r\\n|\\r|\\n)/g, '<br />');
                Data = JSON.parse(Data);
                if (Data != null && Data != undefined) {
                    if (Data.length) {
                        for (var i = 0; i < Data.length; i++) {
                            var data = Data[i];
                            if (data.IsShowToAll) {
                                showSnackbar(data.NotificationContent, false, data.NotificationTitle, data.NotificationId);

                            }
                        }
                    }
                }
            }
        }


    });

    //If session variable null or empty then it will get data from db and set into session.
    if (sessionIsResetPwd == '' || sessionIsResetPwd == 'null') {

        $.ajax({
            type: 'post',
            url: urlGetResetFlag,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.status == true) {
                    if (data.resetPwdflag == 1) {
                        PassswordResetSnackBar();
                    }
                }
            }
        });
    }
    else if (sessionIsResetPwd == 'True') {
        PassswordResetSnackBar();
    }

    $("#createAnotherJob").click(function () {
        $('#popupboxSuccessJobCreatePopup').modal('toggle');
        $.get(urlCreateJobPopup, function (data) {
            $('#createJobNewPopup').empty();
            $('#createJobNewPopup').append(data);
            OpenJobCreatePopup();
        });
    });

    $("#viewCreatedJob").click(function () {

        var url = urlIndexJob + "/" + $(this).attr('jobId');
        window.location.href = url;
    });

    $(".CreateNewJob").click(function () {
        $.get(urlCreateJobPopup, function (data) {
            $('#createJobNewPopup').empty();
            $('#createJobNewPopup').append(data);
            
            OpenJobCreatePopup();
        });
        //$('#popupboxCreateJobTemplate').modal({ backdrop: 'static', keyboard: false });

        //if ($('#imgJobNew').attr('src') == "//:0") {
        //    //$('#imgJobNew').attr('src', '../Images/new_job.png');
        //    //$('#imgJobOld').attr('src', '../Images/old_job.png');
        //    $('#imgJobNew').attr('src', ProjectImagePath + '/Images/new_job.png');
        //    $('#imgJobOld').attr('src', ProjectImagePath + '/Images/old_job.png');
        //}
        //   <img src="~/Images/new_job.png" id="imgJobNew" class="newJob img-responsive" />
        //                        <img src="~/Images/old_job.png" id="imgJobOld" class="oldJob img-responsive" style="display:none" />
    });

    $("#btnClosepopupboxChangeUser").click(function () {
        $('#popupboxChangeUser').modal('toggle');
    });

    //$("#btnClosepopupboxCreateJobTemplate").click(function () {
    //    $('#popupboxCreateJobTemplate').modal('toggle');
    //});
    $(".createJobFromSolarJobScreen").click(function () {
        event.preventDefault();
        $.get(urlCreateJobPopup, function (data) {
            $('#createJobNewPopup').empty();
            $('#createJobNewPopup').append(data);
            OpenJobCreatePopup();
        });
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

    $('#searchReseller').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnResellerId").val('');
            $.each(resellerList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchReseller").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {

            $("#hdnResellerId").val(ui.item.id); // save selected id to hidden input
            if ($("#UserTypeIdFSA").val() == 5 || $("#UserTypeIdFSA").val() == 4) {
                var URL = ProjectImagePath + 'User/' + 'GetUserByResellerId/User?userTypeId=' + $("#UserTypeIdFSA").val() + '&resellerId=' + $('#hdnResellerId').val() + '&isTypeChange=' + 1;
                $(".userDropdown").show();
                BindUsersByUserType(URL, '');
            }
            else if ($("#UserTypeIdFSA").val() == 8) {
                $(".solarCompanyDropdown").show();
                BindSolarCompanyByResellers($('#hdnResellerId').val(), 1, '')
            }

        }
    }).on('focus', function () { $(this).keydown(); });

    $('#searchSolarCompanyFSA').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnSolarCompanyIdFSA").val('');
            $.each(solarCompanyList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchSolarCompanyFSA").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnSolarCompanyIdFSA").val(ui.item.id); // save selected id to hidden input
            var URL = ProjectImagePath + 'User/' + 'GetUserBySolarCompanyId/User?userTypeId=' + $("#UserTypeIdFSA").val() + '&solarCompanyId=' + $('#hdnSolarCompanyIdFSA').val() + '&isTypeChange=' + 1;
            $(".userDropdown").show();
            BindUsersByUserType(URL, '');
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
    $("#searchReseller").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");

        ul.addClass("autocompleteChangeUser");

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);

    };
    $("#searchSolarCompanyFSA").autocomplete('instance')._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) + "</span>");

        ul.addClass("autocompleteChangeUser");

        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);

    };

    //Timer(Sessiontimeout - 1);   

    $('.disableMenu').removeClass('disableMenu');
    if ($(".menu .active").length > 1) {
        $('.menu').find('li').first().removeClass('active');
    }

    $('.menu li a').click(function () {
        currentMenu = $(this).attr('id');
        document.cookie = "menuname" + "=" + $(this).attr('id') + "" + "; path=/";
    });

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

    if (ProjectSession_Logo != null && ProjectSession_Theme != null && ProjectSession_Logo != '' && ProjectSession_Theme != '') {
        if (ProjectSession_Logo == 'Images/logo.png') {
            $('#imgLogo').attr('src', (ProjectSession_SiteUrlBase + ProjectSession_Logo));
        }
        else {
            $('body').attr('id', ProjectSession_Theme);
            var proofDocumentURL = UploadedDocumentPath;
            var logo = ProjectSession_Logo;
            var SRC = proofDocumentURL + 'UserDocuments' + '/' + logo;
            $('#imgLogo').attr('src', SRC);
            $('.hd-top .logo img').css('height', '60px');
            $('.hd-top .logo img').css('margin-top', '0px');
        }
        $('body').attr('id', ProjectSession_Theme);
    }

    $("body").find("form").submit(function (e) {
        if (typeof (validateExtraFields) == "function") {
            if (validateExtraFields() == false) {
                return false;
            }
        }

    });

    $(document).attr('title', 'GreenBot - ' + $('h1:first').text());
    window.history.forward();

    var UserTypeChange = function () {
        ShowDropDownBasedOnUserType($("#UserTypeIdFSA").val(), 1, '', '', '');
    };

    if (ProjectSession_UserTypeId == 1 || ProjectSession_IsUserFSA == 'True') {

        FillDropDownForChangeUser('UserTypeIdFSA', ProjectImagePath + 'UserType/' + 'GetUserType/UserType', 1, true, null);

        ShowDropDownBasedOnUserType($("#UserTypeIdFSA").val(), 0, ProjectSession_ResellerId, ProjectSession_SolarCompanyId, ProjectSession_LoggedInUserId);

        $("#UserTypeIdFSA").unbind("change", UserTypeChange);
        $("#UserTypeIdFSA").bind("change", UserTypeChange);
    }

    $("#saveChangeUser").click(function () {
        if ($("#hdnUserId").val() > 0)
            RedirectFSAToUser($("#hdnUserId").val());
        else
            alert('Please select user.');
    });

    $("#resetChangeUser").click(function () {
        $(".resellerDropdown").hide();
        $(".solarCompanyDropdown").hide();
        $("#UserTypeIdFSA").val(1);

        var URL = ProjectImagePath + 'User/' + 'GetUserByUserTypeId/User?userTypeId=' + 1 + '&isTypeChange=' + 1;
        BindUsersByUserType(URL, '');
    });

    setKeepSessionAlive(Sessiontimeout - 1);
    $("#CreateVEECs").click(function () {
        OpenVEECCreatePopup();
    });

    $("#createAnotherVeec").click(function () {
        $('#popupboxSuccessVeecCreatePopup').modal('toggle');
        OpenVEECCreatePopup();

    });

    $("#viewCreatedVeec").click(function () {

        var url = urlIndexVEEC + "/" + $(this).attr('veecID');
        window.location.href = url;
    });

    $(document).on("click", "#closeErrorPopup", function () {
        $('#dialog').modal('hide');
    });

    $('#jobTemplateId').change(function () {
        if ($(this).val() == "2") {
            $('#popupboxCreateJobTemplate').find('.newJob').css('display', 'none');
            $('#popupboxCreateJobTemplate').find('.oldJob').css('display', '');
        }
        else {
            $('#popupboxCreateJobTemplate').find('.newJob').css('display', '');
            $('#popupboxCreateJobTemplate').find('.oldJob').css('display', 'none');
        }
    });

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

function ChangeUserLink() {
    $('#popupboxChangeUser').modal({ backdrop: 'static', keyboard: false });
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

function BindResellersByUserType(userTypeId, isTypeChange, resellerValue, solarCompanyValue, userValue) {

    var URL = ProjectImagePath + 'Reseller/' + 'GetResellerFSAChangeUser/Reseller';
    $("#searchReseller").val("");
    if (ProjectSession_SystemReseller && ProjectSession_SystemReseller.length != 0 && isTypeChange == 0) {
        var SystemReseller_Value = JSON.parse(ProjectSession_SystemReseller.replace(/&quot;/g, '"'));//JsonConvert.DeserializeObject(SystemUserType1);
        BindSystemReseller_Values(SystemReseller_Value, isTypeChange, resellerValue, solarCompanyValue, userValue);
    }
    else {
        $.ajax({
            type: 'get',
            url: ProjectImagePath + 'Reseller/' + 'GetResellerFSAChangeUser/Reseller',
            dataType: 'json',
            data: { isTypeChange: isTypeChange },
            success: function (Users) {
                BindSystemReseller_Values(Users, isTypeChange, resellerValue, solarCompanyValue, userValue);
            },
            error: function (ex) {
                alert('Failed to retrieve reseller.' + ex);
            }
        });
    }
    return false;
}

function BindSolarCompanyByResellers(resellerId, isTypeChange, solarCompanyValue, userValue) {
    $("#searchSolarCompanyFSA").val("");

    if (ProjectSession_SystemSolarCompanyByReseller && ProjectSession_SystemSolarCompanyByReseller.length != 0) {
        var SystemSolarCompanyByReseller_Value = JSON.parse(ProjectSession_SystemSolarCompanyByReseller.replace(/&quot;/g, '"'));//JsonConvert.DeserializeObject(SystemUserType1);
        BindSystemSolarCompanyByReseller_Value(SystemSolarCompanyByReseller_Value, resellerId, isTypeChange, solarCompanyValue, userValue);
    }
    else {
        $.ajax({
            type: 'get',
            url: ProjectImagePath + 'SolarCompany/' + 'GetSolarCompanyByResellerIdFSAUserChange/SolarCompany',
            dataType: 'json',
            data: { resellerId: resellerId, isTypeChange: isTypeChange },
            success: function (Users) {
                solarCompanyList = [];
                $.each(Users, function (i, user) {
                    solarCompanyList.push({ value: user.Value, text: user.Text });
                });

                if (solarCompanyValue > 0) {
                    $('#hdnSolarCompanyIdFSA').val(solarCompanyValue);
                    $.each(solarCompanyList, function (key, value) {
                        if (value.value == solarCompanyValue) {
                            $("#searchSolarCompanyFSA").val(value.text);
                        }
                    });
                }
                else {
                    $('#hdnSolarCompanyIdFSA').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    $("#searchSolarCompanyFSA").val(solarCompanyList.length > 0 ? solarCompanyList[0].text : '');
                }

                var URL = ProjectImagePath + 'User/' + 'GetUserBySolarCompanyId/User?userTypeId=' + $("#UserTypeIdFSA").val() + '&solarCompanyId=' + $('#hdnSolarCompanyIdFSA').val() + '&isTypeChange=' + isTypeChange;
                $(".userDropdown").show();
                BindUsersByUserType(URL, userValue);

            },
            error: function (ex) {
                alert('Failed to retrieve solarCompany.' + ex);
            }
        });
    }
    return false;
}

function ShowDropDownBasedOnUserType(userTypeId, isTypeChange, resellerValue, solarCompanyValue, userValue) {
    if (userTypeId == 1 || userTypeId == 3 || userTypeId == 2 || userTypeId == 6 || userTypeId == 7 || userTypeId == 9) {
        var URL = ProjectImagePath + 'User/' + 'GetUserByUserTypeId/User?userTypeId=' + userTypeId + '&isTypeChange=' + isTypeChange;
        BindUsersByUserType(URL, userValue);
        $(".userDropdown").show();
        $(".resellerDropdown").hide();
    }
    else {
        $(".userDropdown").hide();
        $(".resellerDropdown").show();
        BindResellersByUserType(userTypeId, isTypeChange, resellerValue, solarCompanyValue, userValue);
    }
    $(".solarCompanyDropdown").hide();
}

function ClearCreatePopup() {
    $("#createJobNewPopup").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $("#createJobNewPopup").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#createJobNewPopup").find('select').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $("#createJobNewPopup").find('textarea').val('');
    $("#JobInstallationDetails_IsSameAsOwnerAddress").attr('checked', false);
}

function OpenJobCreatePopup() {
    ClearCreatePopup();
    $('#popupboxCreateJobPopup').modal({ backdrop: 'static', keyboard: false });
    $("#createJobNewPopup").find('.modal-body').height($(window).height() - 300);
    $("#createJobNewPopup").find('.modal-body').css('overflow-y', 'scroll');
    $("#createJobNewPopup").find('.modal-body').animate({ scrollTop: 0 }, "fast");
    $('body').css('overflow', 'hidden');
    $("#JobOwnerDetails_AddressID_Popup").val(1)
    $("#JobInstallationDetails_AddressID_Popup").val(1)
    $("#JobOwnerDetails_OwnerType_Popup").val("Individual")
    $("#BasicDetails_JobType_Popup").val(1)
}



function DropDownForChangeUser_Values(SystemUserType_Value, id, url, value, hasSelect, callback, defaultText) {

    var options = '';
    //if (hasSelect == true) {
    //    if (defaultText == undefined || defaultText == null)
    //        defaultText = 'Select';
    //    options = "<option value='0'>" + defaultText + "</option>";
    //}
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
    if (id == 'UserFSA' && ProjectSession_IsUserFSA == 'True') {
        $("#UserFSA").val(ProjectSession_LoggedInUserId);
    }
    //if ($("#UserTypeIdFSA").val() == "1" || $("#UserTypeIdFSA").val() == "0")
    if ($("#UserTypeIdFSA").val() == "0")
        $(".userDropdown").hide();
    else
        $(".userDropdown").show();
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

function BindSystemReseller_Values(Users, isTypeChange, resellerValue, solarCompanyValue, userValue) {

    resellerList = [];
    $.each(Users, function (i, user) {
        resellerList.push({ value: user.Value, text: user.Text });
    });

    if (resellerValue > 0) {
        $('#hdnResellerId').val(resellerValue);
        $.each(resellerList, function (key, value) {
            if (value.value == resellerValue) {
                $("#searchReseller").val(value.text);
            }
        });
    }
    else {
        $('#hdnResellerId').val(resellerList.length > 0 ? resellerList[0].value : 0);
        $("#searchReseller").val(resellerList.length > 0 ? resellerList[0].text : '');
    }

    if ($("#UserTypeIdFSA").val() == 5 || $("#UserTypeIdFSA").val() == 4) {
        var URL = ProjectImagePath + 'User/' + 'GetUserByResellerId/User?userTypeId=' + $("#UserTypeIdFSA").val() + '&resellerId=' + $('#hdnResellerId').val() + '&isTypeChange=' + isTypeChange;
        $(".userDropdown").show();
        BindUsersByUserType(URL, userValue);
    }

    if ($("#UserTypeIdFSA").val() == 8) {
        $(".solarCompanyDropdown").show();
        BindSolarCompanyByResellers($('#hdnResellerId').val(), isTypeChange, solarCompanyValue, userValue);
    }
}

function BindSystemSolarCompanyByReseller_Value(Users, resellerId, isTypeChange, solarCompanyValue, userValue) {

    solarCompanyList = [];
    $.each(Users, function (i, user) {
        solarCompanyList.push({ value: user.Value, text: user.Text });
    });

    if (solarCompanyValue > 0) {
        $('#hdnSolarCompanyIdFSA').val(solarCompanyValue);
        $.each(solarCompanyList, function (key, value) {
            if (value.value == solarCompanyValue) {
                $("#searchSolarCompanyFSA").val(value.text);
            }
        });
    }
    else {
        $('#hdnSolarCompanyIdFSA').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
        $("#searchSolarCompanyFSA").val(solarCompanyList.length > 0 ? solarCompanyList[0].text : '');
    }

    var URL = ProjectImagePath + 'User/' + 'GetUserBySolarCompanyId/User?userTypeId=' + $("#UserTypeIdFSA").val() + '&solarCompanyId=' + $('#hdnSolarCompanyIdFSA').val() + '&isTypeChange=' + isTypeChange;
    $(".userDropdown").show();
    BindUsersByUserType(URL, userValue);
}
function OpenVEECCreatePopup() {
    //ClearCreatePopup();
    $('#popupboxCreateVEECPopup').modal({ backdrop: 'static', keyboard: false });
    $("#createVEECPopup").find('.modal-body').height($(window).height() - 300);
    $("#createVEECPopup").find('.modal-body').css('overflow-y', 'scroll');
    $("#createVEECPopup").find('.modal-body').animate({ scrollTop: 0 }, "fast");
    $('body').css('overflow', 'hidden');
}

function gtag() { dataLayer.push(arguments); }

function PassswordResetSnackBar() {
    SnackBar({
        message: null,
        timeout: false,
        NotificationTitle: null,
        SnackbarId: "abc"
    });
    $("div [snackbarid='abc'] .js-snackbar__message").html('Please reset your password,It has been expired.<a href="javascript:void(0)" id="logoutid" style="color:greenyellow">Click Here.</a>');
    $("div [snackbarid='abc'] .js-snackbar__message #logoutid").click(function () {
        var urllogout = logOutURL;
        $.ajax({
            type: 'get',
            url: urllogout,
            success: function (data) {
                window.location.href = forgotPasswordURL;
            }
        });
    });
}
function SetsnackbarSession(SnackbarId) {
    date = new Date().toISOString();
    $.ajax({
        url: setSnackbarSessionURL + SnackbarId,
        method: 'POST',
        cache: false,
        async: false,
        success: function () {
            return true;

        }
    });
}

function ShowHideHeaderSearchBar() {
    $(".searchbar").toggle();
    $("#SearchIcon").toggleClass("active");
}



function SearchMultipleFilter() {
    debugger
    var search = $('#searchresult').val();
    //var JobIDencoded = "id=" + encodeURIComponent(13204);
    $('#searchresult').autocomplete(
        {
            scroll: false,
            selectFirst: false,
            autoFocus: false,
            source: function (request, response) {

                $.ajax(
                    {
                        type: 'GET',
                        url: urllivesearch,
                        data: { search: search },
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        async: false,
                        success: function (data) {
                            
                            response($.map(data.result, function (item) {
                                
                                return {
                                    //label: item.RefNumber + ' ' + item.JobID + ' ' + item.CompanyName ,
                                    label: item.RefNumber + ' ' ,
                                    label2: '(' + item.JobID + ') ',
                                    label3: item.CompanyName,
                                    val: item.Id
                                }
                            }));
                            var totalResults = 10

                            $('.ui-autocomplete').prepend('<span ' +
                                    'class=ui-menu-item style="height:40px;"><span style="color:#1b94da; font-size:15px;">View All results ('+data.TotalRecords+')</span></span>');
                        },
                        error: function (result) { }
                    });
            },
            minLength: 2,
            select: function (event, ui) {
                var vll = ui.item.val;
               var url= window.open('/Job/Index?Id=' + vll, '_blank');
               // var url = window.location.href = '/Job/Index?Id=' + vll // ur own conditions
               // $(location).attr('href', url);
              //  $(location).attr('target', '_blank');
            }
        }).autocomplete("widget").addClass("SearchDropdownAutoComplete");
    //if (event.keyCode == 13) { // this event fired when enter is pressed  
    //    //url = 'Productlist.aspx?prefix=' + ptxt; // ur own conditions  
    //    //$(location).attr('href', url);
    //    //return false;
    //}

    $["ui"]["autocomplete"].prototype["_renderItem"] = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append($("<a></a>").html('<span style="color:#1b94da">' + item.label + '</span>' + item.label2 + '</span><br>from <span style="color:#1b94da">' + item.label3 + '</span>'))
            .appendTo(ul);
    };
}

function RedirectToFormBotHelpPage() {
    window.open(urlFormBotHeplPage, '_blank');
}