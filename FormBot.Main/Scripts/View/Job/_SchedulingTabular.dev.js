$(document).ready(function () {
    $("#btnAddVisit").click(function (e) {        
        addVisit();
    });

    $('#visitStartDate').datepicker({
        format: FormBot_HelperDateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#visitEndDate").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#visitStartDate").val(), FormBot_HelperDateFormat));
            var toDate = new Date(ConvertDateToTick($("#visitEndDate").val(), FormBot_HelperDateFormat));
            if (fromDate > toDate) {
                $("#visitEndDate").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#visitStartDate").val(), FormBot_HelperDateFormat);
        tDate = moment(tickStartDate).format("MM-DD-YYYY");
        if ($('#visitEndDate').data('datepicker')) {
            $('#visitEndDate').data('datepicker').setStartDate(new Date(tDate));
        }
    }).on('change', function () {
        $(this).valid();
    });

    $('#visitEndDate').datepicker({
        format: FormBot_HelperDateFormat,
        autoclose: true
        //startDate: new Date(ConvertDateToTick($('#visitStartDate').val(), DateFormat)) 
    }).on('change', function () {
        $(this).valid();
    });



    // $('#date-pick, #date-pick1, .date-pick').datepicker({
    //    //format: "dd/mm/yyyy",
    //    format: DateFormat,
    //    autoclose: true
    //}).on('change', function () {
    //    $(this).valid();
    //});

    $('#visitStartTime, #visitEndTime').datetimepicker({
        format: "HH:mm"
    });

    $("#visitStartDate").keydown(function (e) {
        if (e.which == 9) {
            return true;
        }
        return false;
    });
    $("#visitEndDate").keydown(function (e) {
        if (e.which == 9) {
            return true;
        }
        return false;
    });


    $("#visitStartTime").keydown(function (e) {
        if (e.which == 9) {
            return true;
        }
        return false;
    });
    $("#visitEndTime").keydown(function (e) {
        if (e.which == 9) {
            return true;
        }
        return false;
    });
    $("#noNotification").click(function () {
        $("#notification").modal('hide');
        $("#popUpJobDetail").modal('hide');

    });

    $("#yesNotification").click(function () {        
        if ($("#alertAssignTime").data('dragData')) {
            var data = $("#alertAssignTime").data('dragData');
            SaveScheduleOnDropAndInsertEdit(data.startDate, data.startTime, data.endDate, data.endTime, data.label, data.detail, data.userId, data.jobId, data.jobSchedulingID, true, data.status, data.isDrop, "", "", data.jobTitle, data.IsClassic.toString().toLowerCase());
            //saveJobSchedule(true);
        }

        if ($("#yesNotification").attr("isAutoScheduleVisit") == "true")
            addQuickVisit($('#BasicDetails_JobType').val() == 1 ? $('#hdBasicDetails_InstallerID').val() : $('#hdBasicDetails_SWHInstallerID').val(), $('#BasicDetails_JobType').val() == 1 ? $('#txtBasicDetails_InstallerID').val() : $('#txtBasicDetails_SWHInstallerID').val(), 2, '', true);
    });
    $("#saveJobSchedule").click(function () {
        if ($("#jobScheduling").valid()) {
            saveJobSchedule(false);
        }
    });
});

function saveJobSchedule(isNotification) {

    var startDate, startTime, endDate, endTime, label, detail, jobId, userId, jobSchedulingID, url, status, userName, checkListTemplateId = "", tickEndDate = null;
    startDate = $("#visitStartDate").val();
    startTime = $("#visitStartTime").val();
    endDate = $("#visitEndDate").val();
    endTime = $("#visitEndTime").val();
    label = $("#RefNumber").val();
    detail = $("#Detail").val();
    jobId = $("#JobID").val();
    userId = $("#UserId").val();
    jobSchedulingID = $("#JobSchedulingID").val();
    status = $("#Status").val();
    userName = $("#UserId option:selected").text();
    var title = $("#RefNumber").val();
    if (title == '' || title == undefined || title == null) {
        title = $("#JobID option:selected").text();
    }

    jobTitle = title;

    //TODO
    var tickStartDate = ConvertDateToTick(startDate, DateFormat);
    startDate = moment(tickStartDate).format("YYYY-MM-DD");

    if (endDate != null && endDate != "" && endDate != undefined) {
        tickEndDate = ConvertDateToTick(endDate, DateFormat);
        endDate = moment(tickEndDate).format("YYYY-MM-DD");
    }

    if (!isNotification) {
        var isValid = CompareDate(startDate, endDate, startTime, endTime);

        if (!isValid && message != '' && message != undefined && message != null) {
            $(".alert").hide();
            $("#errorMsgRegionForPopUp").html(closeButton + message);
            $("#errorMsgRegionForPopUp").show();

            return false;
        }
    }



    if ($("#IsClassic").val().toString().toLowerCase() == 'false') {
        //if ('@Model.IsCheckListView.ToString().ToLower()' == 'true') {
        if ($("#chkListOfScheduling").find(".checklist").find('li').length < 1) {
            alert('Please add at least one checklist item.');
            return false;
        }
    }

    $("#loading-image").css("display", "");


    //setTimeout(function () {
    //SaveScheduleOnDropAndInsertEdit(startDate, startTime, endDate, endTime, label, detail, userId, jobId, jobSchedulingID, isNotification, status, false, userName, "", jobTitle, '@Model.IsCheckListView.ToString().ToLower()') 
    SaveScheduleOnDropAndInsertEdit(startDate, startTime, endDate, endTime, label, detail, userId, jobId, jobSchedulingID, isNotification, status, false, userName, "", jobTitle, $("#IsClassic").val().toString().toLowerCase())
    //}, 500);
    SearchHistory();

}

function SaveScheduleOnDropAndInsertEdit(startDate, startTime, endDate, endTime, label, detail, userId, jobId, jobSchedulingID, isNotification, status, isDrop, userName, ui, jobTitle, IsClassic) {    
    var checkListTemplateId = '';
    if (IsClassic == "false") {
        checkListTemplateId = $("#CheckListTemplateId").val();
    }

    $.ajaxSetup({ cache: false });
    var dragData = {
        startDate: startDate,
        startTime: startTime,
        endDate: endDate,
        endTime: endTime,
        label: label,
        detail: detail,
        userId: userId,
        jobId: jobId,
        jobSchedulingID: jobSchedulingID,
        isNotification: isNotification,
        status: status,
        isDrop: isDrop,
        userName: userName,
        jobTitle: jobTitle,
        IsClassic: IsClassic,
        TemplateId: checkListTemplateId,
    };
    $("#alertAssignTime").data('dragData', dragData);

    var obj = {};
    obj.JobSchedulingID = jobSchedulingID;
    obj.Label = label;
    obj.Detail = detail;
    obj.strVisitStartDate = startDate;
    obj.strVisitEndDate = endDate;
    obj.strVisitStartTime = startTime;
    obj.strVisitEndTime = endTime;
    obj.JobID = jobId;
    obj.UserId = userId;
    obj.Status = status;
    obj.isNotification = isNotification;
    obj.isDrop = isDrop;
    obj.userName = userName;
    obj.JobTitle = jobTitle;
    obj.TemplateId = checkListTemplateId;
    obj.IsFromCalendarView = $("#IsFromCalendarView").val();

    obj.SolarCompanyId = $("#hdnsolarCompanyid").val();

    //if ($("#IsFromCalendarView").val().toString().toLowerCase() == 'true')
    //{
    //    obj.SolarCompanyId = $("#hdnsolarCompanyid").val();
    //}
    //else
    //{

    //}

    if (IsClassic == "false" || IsClassic == undefined) {

        var visitChecklistItems = []

        $.each($("#chkListOfScheduling").find(".checklist").find('li'), function () {
            visitChecklistItems.push($(this).data('visitchecklistitemid'));
        });

        if (visitChecklistItems.length > 0) {
            obj.VisitCheckListItemIds = visitChecklistItems.toString();
        }
    }

    obj.TempJobSchedulingId = $("#TempJobSchedulingId").val();

    //var data = JSON.stringify(obj);
    var scheduledata = {};
    scheduledata.jobSchedulingPopup = obj;
    scheduledata.isQuickAddvisit = false;

    var ret = false;
    //var data = "{'startDate': '"+startDate+"', 'startTime': '"+startTime+"','endDate':'"+endDate +"','endTime':'"+endTime+"','label' :'"+label +"','detail' :'"+detail +"','userId' :'"+userId +"','jobId' :'"+jobId+"' ,'jobSchedulingID' :'"+jobSchedulingID +"','isNotification' :'"+isNotification +"','status' :'"+status+"', 'isDrop':  '"+isDrop+"' }";
    $.ajax(
        {
            url: urlJobSchedulingDetail, //?startDate=' + startDate + '&startTime=' + startTime + '&endDate=' + endDate + '&endTime=' + endTime + '&label=' + label + '&detail=' + detail + '&userId=' + userId + '&jobId=' + jobId + '&jobSchedulingID=' + jobSchedulingID + '&isNotification=' + isNotification + '&status=' + status + '&isDrop=' + isDrop,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify(scheduledata),
            //async: false,

            //beforeSend: function () {
            //    $("#loading-image1").show();
            //},
            success: function (response) {
                //console.log("0 " + new Date());

                if (response) {

                    //console.log("1 " + new Date());

                    var responseID = response.split('^')[0];
                    var responseData = response.replace(responseID + '^', '');

                    //console.log("2 " + new Date());

                    ret = ResponseSaveScheduleOnDropAndInsertEdit(response, responseID, responseData, isDrop);

                    if (isDrop) {
                        if (!ret) {
                            $("#notification").modal();
                        }
                    }
                }

                if (ui != "" && ui != undefined && ui != null) {
                    $("#notification").modal('hide');
                    if (!ret) {
                        ui.draggable.animate(ui.draggable.data().origPosition, "slow");
                        return;
                    }
                }
                //ReloadJobPhotoSection(jobId);

                //if (IsClassic == "false") {
                //    ReloadJobPhotoSection(jobId);
                //    HideInstaller();
                //}
            },
            error: function () {
                message = "Job Schedule has not been saved."
                showErrorMessage(message);
                $("#notification").modal('hide');
                $("#loading-image1").hide();
            }
        });
    return ret;
}

function FillJobSchedulingDetail(jobSchedulingID, jobTitle) {    
    $.ajaxSetup({ cache: false });

    if (jobSchedulingID > 0) {

        $("#btnJobDelete").css("display", "");

        $.ajax(
            {
                url: urlJobSchedulingDetailById + jobSchedulingID,
                contentType: 'application/json',
                method: 'get',
                cache: false,
                success: function (response) {


                    if (response && response.JobSchedulingID) {

                        if (IsFromCalendarView_Glbl == 'true') {
                            var GetSolarElectricianForJobTypeURL = urlGetSolarElectricianForJobType + response.JobType + "&SolarCompanyId=" + $("#hdnsolarCompanyid").val();
                            FillDropDown('UserId', GetSolarElectricianForJobTypeURL, response.UserId, true, null);
                            var GetJobsForJobTypeURL = urlGetJobsForJobType + response.JobType + "&SolarCompanyId=" + $("#hdnsolarCompanyid").val();
                            FillDropDown('JobID', GetJobsForJobTypeURL, response.JobID, true, null);
                        }

                        var endTime = null, endDate = null;

                        var startTime = response.strVisitStartTime.split(':', 2).join(':');
                        var startDate = moment(response.strVisitStartDate).format(DateFormat.toUpperCase());

                        if (response.strVisitEndTime != null && response.strVisitEndTime != '') {
                            endTime = response.strVisitEndTime.split(':', 2).join(':');
                        }
                        if (response.strVisitEndDate != null && response.strVisitEndDate != '') {
                            endDate = moment(response.strVisitEndDate).format(DateFormat.toUpperCase());
                        }
                        $("#visitStartDate").val(startDate);
                        $("#visitStartTime").val(startTime);
                        $("#visitEndDate").val(endDate);
                        $("#visitEndTime").val(endTime);
                        $("#RefNumber").val(response.Label);
                        $("#Detail").val(response.Detail);
                        $("#UserId").val(response.UserId);
                        $("#jobScheduling").find("select[id='JobID']").val(response.JobID);
                        $("#JobSchedulingID").val(response.JobSchedulingID);
                        $("#Status").val(response.Status);
                        $("#visitStartDate").datepicker("update", startDate);
                        $("#visitEndDate").datepicker("update", endDate);
                        $("#IsClassic").val(response.IsClassic.toString().toLowerCase());
                        $("#JobType").val(response.JobType);
                        var tickStartDateSelect = ConvertDateToTick($('#visitStartDate').val(), DateFormat);
                        $('#visitEndDate').data('datepicker').setStartDate(new Date(tickStartDateSelect));
                        ResponseShowJobSchedulingDetail(response);
                        $("#popUpJobDetail").modal();
                        $("#popUpJobDetail").find('input[type=text]').each(function () {
                            $(this).attr('class', 'form-control valid');
                        });
                        $("#popUpJobDetail").find('Select').each(function () {
                            $(this).attr('class', 'form-control valid');
                        });

                        if (IsFromCalendarView_Glbl == 'false') {
                            $("#jobScheduling").find("select[id='JobID']").attr("disabled", "disabled");
                        }


                        if (response.IsClassic.toString().toLowerCase() == 'false') {
                            //if ('@Model.IsCheckListView' == 'True') {

                            $("#loading-image").show();

                            if (response.CheckListTemplateId > 0)
                                OpenCheckListItemPopup(response.CheckListTemplateEncodedId);
                            else
                                OpenCheckListItemPopup(response.DefaultTemplateId);

                        }
                        else {

                            $("#checkListTemplateForm").empty();
                        }

                        $("#popUpJobDetail").find('.field-validation-error').attr('class', 'field-validation-valid');
                    }
                    else {
                        message = "Job Schedule has not been opened."
                        showErrorMessageCalendar(message);
                    }
                },
                error: function () {
                    message = "Job Schedule has not been opened."
                    showErrorMessageCalendar(message);
                }
            });
    }
}
function CompareDate(startDate, endDate, startTime, endTime) {

    var minutesOfDay = function (m) {
        return moment(m).minutes() + moment(m).hours() * 60;
    }

    if (Date.parse(startDate) > Date.parse(endDate)) {
        message = "Start date should not be greater than end date";
        return false;
    }
    else {
        if (startDate == endDate) {
            if (minutesOfDay(startDate + ' ' + startTime) > minutesOfDay(endDate + ' ' + endTime)) {
                message = "Start time must be less than end Time";
                return false;
            }
            else if (minutesOfDay(startDate + ' ' + startTime) == minutesOfDay(endDate + ' ' + endTime)) {
                message = "Start time and end time should not be same time";
                return false;
            }
            else
                return true;
        }
        else
            return true;
    }
}

function DeleteJobSchedulingDetail(jobSchedulingID, userId) {

    if (confirm('Are you sure you want to delete this job schedule?')) {
        $("#loading-image").css("display", "");
        setTimeout(function () {
            DeleteJSD(jobSchedulingID, userId);
            SearchHistory();
        }, 500);
    }
}

function DeleteJSD(jobSchedulingID, userId) {

    if (jobSchedulingID == null)
        jobSchedulingID = $("#JobSchedulingID").val();

    if (userId == null)
        userId = $("#UserId").val();

    //var userId = $("#UserId").val();

    $.ajax({
        url: urlDeleteJobSchedule + jobSchedulingID + '&userId=' + userId + '&jobTitle=' + $('#JobTitle').val(),
        contentType: 'application/json',
        method: 'post',
        success: function (response) {
            ResponseDelete(response.jobSchedulingId, jobSchedulingID);
            //if (response.installerDesignerId == (JOBType == 1 ? parseInt($('#hdBasicDetails_InstallerID').val()) : parseInt($('#hdBasicDetails_SWHInstallerID').val()))) {
            if (response.installerDesignerId == (BasicDetails_JobType == 1 ? parseInt($('#hdBasicDetails_InstallerID').val()) : parseInt($('#hdBasicDetails_SWHInstallerID').val()))) {
                ShowHideVisitScheduledIcon(true, false, response.seStatus);
                $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_JobType = 1 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + response.seStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + response.seStatus + ');');
            }
        },
        error: function () {
            message = "Job schedule has not been deleted."
            showErrorMessage(message);
        }
    });
}

//function OpenCheckListItemPopup(templateId) {
//    debugger;
//    $(".alert").hide();
//    $("#successMsgRegionAddEditItem").hide();
//    $("#errorMsgRegionItem").hide();

//    $.get(urlGetCheckListItemByTemplateId + templateId + '&isSetFromSetting=' + true, function (data) {
//        $('#checkListTemplateForm').empty();
//        $('#checkListTemplateForm').append(data);
//        $("#popupCheckListTemplate").modal({ backdrop: 'static', keyboard: false });
//    });
//}

function OpenCheckListItemPopup(templateId, isTemplateChange) {
    CommonOpenCheckListItemPopup(templateId, false, $("#JobSchedulingID").val(), isTemplateChange);
}

function CommonOpenCheckListItemPopup(templateId, isSetFromSetting, jobSchedulingId, isTemplateChange, TempJobSchedulingId, jobId, isAddVisit, jobType, isFromIsDeletedChecklistItem) {
    if (isTemplateChange)
        isTemplateChange = true;
    else
        isTemplateChange = false;

    var visitCheckListIdsString = '';

    if (isSetFromSetting == "false" && visitCheckListItemIds.length > 0) {
        visitCheckListIdsString = visitCheckListItemIds.join(',');
    }    
    var url = '/CheckListItem/GetCheckListItemByTemplateId?id=' + templateId + '&isSetFromSetting=' + isSetFromSetting + '&jobSchedulingId=' + jobSchedulingId + '&isTemplateChange=' + isVisitCheckListTemplateChange + '&tempJobSchedulingId=' + TempJobSchedulingId + '&jobId=' + $("#JobID").val() + '&visitCheckListIdsString=' + visitCheckListIdsString + '&isAddVisit=' + isAddVisit + '&JobType=' + jobType + "&SolarCompanyId=" + $("#hdnsolarCompanyid").val() + "&isFromIsDeletedChecklistItem=" + isFromIsDeletedChecklistItem;
    $.get(url, function (data) {

        $('#checkListTemplateForm').empty();
        $('#checkListTemplateForm').append(data);

        if (isSetFromSetting == true) {
            $("#popupCheckListTemplate").modal({ backdrop: 'static', keyboard: false });
        }

        if (templateId == '') {
            $(".liAddCheckListItem").hide();
        }

        if (isSetFromSetting.toString() == "false") {
            visitCheckListItemIds = [];
            $.each($("#chkListOfScheduling").find(".checklist").find('li'), function () {
                visitCheckListItemIds.push($(this).data('visitchecklistitemid'));
            });
        }

        $("#loading-image").hide();

    });
}

function SearchHistory() {    
    var categoryID = $('#historyCategory').val();
    categoryID = categoryID != null ? categoryID : 0;
    //var IsImportantNote = document.getElementById("FilterIsImportantNote").checked; /* Commented temporary as functionality not implemented*/
    var IsImportantNote = false;
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
    var count = 0;
    var tagged = $("#relatedTofilter").val();
    //var searchfilter = U + 00040 + tagged;
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
    $('.IconsEditDelete').each(function () {
        $(this).css("display", "none !important");
    });
}

function callbackCheckList() {
    if ($('#checkListItemForTrade').find('ul').find('li').length > 0)
        $('#checkListItemForTrade').show();
    else
        $('#checkListItemForTrade').hide();
}