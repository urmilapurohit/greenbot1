$(document).ready(function () {
    $("#VisitHeader").html($("#jobHeader").html())
    ShowHideCompletedDate(1);
    //$("#visitAction li #chkVisitStatus").on('change.bootstrapSwitch', function (event, state) {
    //    event.preventDefault();
    //    var visitStatus = $(this).attr('visitStatus');
    //    if (visitStatus == 1)
    //        visitStatus = 2;
    //    else
    //        visitStatus = 1;
    //    ChangeVisitStatus($(this).attr('jobScedulingId'), visitStatus, $(this));
    //});
    $(".switch").on("click", function (e) {
        debugger;
        e.stopImmediatePropagation();
    });
    $(".schedule-option li #visitDefaultSubmission").click(function () {
        MakeVisitAsDefaultSubmission($(this).attr('jobid'), $(this).attr('jobSchedulingId'), $(this).is(':checked'));
    });

    $("#jobVisitDetail").find('.panel').each(function () {
        ShowHideVisitNotes($(this).find("#jobNotesUL"), false);
        $(this).find("#viewMoreNotes").click(function (e) {
            if ($(this).attr('isShowMore') == '0') {
                ShowHideVisitNotes($(this).parent().find('#jobNotesUL'), true);
                $(this).attr('isShowMore', '1');
                $(this).text('Show Less Notes');
            }
            else {
                ShowHideVisitNotes($(this).parent().find('#jobNotesUL'), false);
                $(this).attr('isShowMore', '0');
                $(this).text('Show More Notes');
            }
            e.preventDefault();
        });
    });


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

    $("#btnClosepopupboxPopulateSignature").click(function () {
        $('#popupboxPopulateSignature').modal('toggle');
    });

    $("#btnClosepopupboxViewSignature").click(function () {
        $('#popupboxViewSignature').modal('toggle');
    });

});

function ShowHideVisitNotes(objUL, isShowMore) {
    if (isShowMore == false) {
        objUL.find('li').each(function () {
            if ($(this).attr('number') == 1 || $(this).attr('number') == 2)
                $(this).show();
            else
                $(this).hide();
        });
    }
    else {
        objUL.find('li').each(function () {
            $(this).show();
            $(".mCustomScrollbar").mCustomScrollbar();
        });
    }
}

function addTemplate() {
    if ($("#addTemplateForm").valid()) {
        CommonAddTemplate(isCheckListView);
    }
}

function OpenaAddTemplate(templateId, itemId, isSaveAsNewTemp = true) {
    if (templateId == '') {
        var itemCount = $("#chkListOfScheduling").find(".checklist").find('li').length;
        if (itemCount == 0) {
            alert('Please add checklist item first.');
            return false;
        }
    }
    CommonOpenaAddTemplate(templateId, itemId, isSaveAsNewTemp);
}

function ResponseSaveScheduleOnDropAndInsertEdit(response, responseID, responseData, isDrop, isAutoScheduleVisit = false) {
    //console.log("3 = " + new Date());

    var ret = false;
    if (responseID > 0) {
        $("#loading-image").show();
        message = "Job schedule has been saved successfully."

        $.ajax(
            {
                url: urlReloadSectionOnVisitSave,
                dataType: 'json',
                data: { id: visitJobId, isCheckListView: true, isReloadGridView: true, solarCompanyId: $("#BasicDetails_SolarCompanyId").val() },
                contentType: 'application/json; charset=utf-8',
                type: 'get',
                async: false,
                success: function (response) {

                    if (response != null) {

                        visitCheckListItemIds = [];

                        HideInstaller();

                        $("#notification").modal('hide');
                        $("#popUpJobDetail").modal('hide');

                        $("#divVisitGridView").html(response.visitPartialView);

                        $(".schedule-option li #visitDefaultSubmission").click(function () {
                            MakeVisitAsDefaultSubmission($(this).attr('jobid'), $(this).attr('jobSchedulingId'), $(this).is(':checked'));

                        });
                        if ($("#btnAddVisit:visible").length == 0) {
                            $(".assign-installer").hide();
                            $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
                            $('#btnInstallerQuickVisit').removeAttr('onclick');
                        }
                        else {
                            $(".assign-installer").show();
                            $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
                            if (InstallerView_SEStatus == 2)
                                $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_InstallerID > 0 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + InstallerView_SEStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + InstallerView_SEStatus + ');');
                            else
                                $('#btnInstallerQuickVisit').removeAttr('onClick')
                        }
                        $("#VisitHeader").html($("#jobHeader").html());

                        $('.reloadCustomJobField').empty();
                        $('.reloadCustomJobField').append(response.customFieldView);

                        $('#loadNewJobPhoto').empty();
                        $('#loadNewJobPhoto').append(response.photoView);

                        $('#checkListItemForTrade').html(response.checkListView).promise().done(function () {
                            callbackCheckList();
                        });

                        $("#loading-image").hide();
                    }
                    else {
                    }
                },
                error: function () {
                }
            });
        showSuccessMessageJobVisit(message);
    }
    else if (parseInt(response) == -1) {
        $("#notification").modal();
        if (isAutoScheduleVisit)
            $("#yesNotification").attr("isAutoScheduleVisit", true);
    }
    else if (parseInt(responseID) == 0) {
        message = responseData;
        showErrorMessageSchedulingPopup(message);
        $("#notification").modal('hide');
    }
    else {
        message = "Job Schedule has not been saved."
        showErrorMessageSchedulingPopup(message);
        $("#notification").modal('hide');
    }
    return ret;
}

function showErrorMessageSchedulingPopup(message) {
    $(".alert").hide();
    $("#errorMsgRegionForPopUp").html(closeButton + message);
    $("#errorMsgRegionForPopUp").show();
}

function showErrorMessageCalendar(message) {
    showErrorMessageJobVisit(message);
}

function showJobSchedulingDetail(jobSchedulingID, e) {
    e.preventDefault();
    $(".visitPopupMsg").hide();
    $("#loading-image").show();
    setTimeout(function () {
        FillJobSchedulingDetail(jobSchedulingID);
        GenerateRandomNumber();
        visitCheckListItemIds = [];
        isVisitCheckListTemplateChange = false;
    }, 500);

}

function DeleteJobScheduling(jobSchedulingID, userId, e) {
    e.preventDefault();
    DeleteJobSchedulingDetail(jobSchedulingID, userId, modelJobId);
}

function ResponseShowJobSchedulingDetail(response) {
    var jobID = modelJobId;
    $("#JobID").val(jobID);
    $("#JobID").attr("disabled", "disabled");
}

function ResponseDelete(responseID, jobSchedulingID) {
    if (responseID && responseID > 0) {
        var strEmailConfigureMsg = '';

        //Email configuration not required
        if (sessionIsUserEmailAccountConfigured == 'False') {
            strEmailConfigureMsg = '(Can not send mail because email account is not configured)';
        }

        message = "Job schedule has been deleted successfully. " + strEmailConfigureMsg;

        $("#jobVisitDetail").find('[data-panelid=' + jobSchedulingID + ']').remove();
        $("#btnAddVisit").show();
        $(".assign-installer").show();
        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
        if (InstallerView_SEStatus == 2)
            $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_InstallerID > 0 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + InstallerView_SEStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + InstallerView_SEStatus + ');');
        else
            $('#btnInstallerQuickVisit').removeAttr('onClick')
        var span = '<span class="deleteInfo" title="Visit item has been deleted">&nbsp;</span>'

        $("#divVisitList").find('[data-jobSchedulingId=' + jobSchedulingID + ']').find('.visitParent').addClass('deleteJobVisit').prepend(span);

        if ($("#divVisitList").find('[data-jobSchedulingId=' + jobSchedulingID + ']').find('.visitParent').find('.submission').length > 0) {
            $("#divVisitList").find('[data-jobSchedulingId=' + jobSchedulingID + ']').find('.visitParent').find('.submission').remove();
            MakeDefaultFolderAsDefaultSubmission();
        }
        var jobID = modelJobId;
        ReloadSTCJobScreen(jobID);

        showSuccessMessageJobVisit(message);
        if ($('#jobVisitDetail').find('.panel').length < 1) {
            $('.installer-text').css('display', '');
            $('.schedule-an-installer').css('display', '');
        }

    }
    else {
        message = "Job schedule has not been deleted.";
        showErrorMessageJobVisit(message);
    }
}

function ReloadJobPhotoSection(jobId) {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlreloadphoto + jobId, function (data) {
            $('#loadNewJobPhoto').empty();
            $('#loadNewJobPhoto').append(data);
        });
    }, 500);


}

function MakeVisitAsDefaultSubmission(jobId, jobSchedulingId, isDefault) {
    $.ajax(
        {
            url: sessionMakeVisitAsDefaultSubmission,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobId: jobId, jobSchedulingId: jobSchedulingId, isDefault: isDefault },
            success: function (response) {
                if (response.status) {

                    if (isDefault == true) {
                        $(".schedule-option li").find('#visitDefaultSubmission').each(function () {
                            if ($(this).attr('jobSchedulingId') != jobSchedulingId) {
                                $(this).prop('checked', false);
                                var removeDefaultSubfromPhoto = $("#divVisitList").find('[data-jobSchedulingId=' + $(this).attr('jobSchedulingId') + ']').find('.visitParent').find('.submission');
                                if (removeDefaultSubfromPhoto) {
                                    removeDefaultSubfromPhoto.remove();
                                }
                            }
                            else {
                                var defaultSpan = "<span class=submission>STC Submission <i class='sprite-img submission-icon'></i></span>";
                                $("#divVisitList").find('[data-jobSchedulingId=' + $(this).attr('jobSchedulingId') + ']').find('.visitParent').prepend(defaultSpan);
                            }
                        });
                        $("#divVisitList").find("#pnlMainDefault").find('.visitParent').find('.submission').remove();
                        showSuccessMessageJobVisit("Visit has been marked as default successfully.");
                    }
                    else {
                        $("#divVisitList").find('[data-jobSchedulingId=' + jobSchedulingId + ']').find('.visitParent').find('.submission').remove();
                        showSuccessMessageJobVisit("Visit has been re-Marked as default successfully.");
                        MakeDefaultFolderAsDefaultSubmission();
                    }

                    //$('#checkListItemForTrade').load(actionCheckListItemForTrade, callbackCheckList);
                    ReloadSTCJobScreen(jobID);

                    //  ReloadJobPhotoSection(jobId);
                }
                else {
                    if (response.error.toLowerCase() == 'sessiontimeout')
                        window.location.href = urlLogout;
                    if (response.error) {
                        showErrorMessageJobVisit(response.error);
                    }
                    else {
                        if (isDefault == true)
                            showErrorMessageJobVisit("Visit has not been marked as default successfully.");
                        else
                            showErrorMessageJobVisit("Visit has not been re-Marked as default successfully.");
                    }
                }
            },
            error: function () {

                if (isDefault == true)
                    showErrorMessageJobVisit("Visit has not been marked as default.");
                else
                    showErrorMessageJobVisit("Visit has not been re-Marked as default.");
            }
        });
}

function ChangeVisitStatus(jobSchedulingId, visitStatus, objSwitch) {
    $.ajax(
        {
            url: changeVisitStatus,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobSchedulingId: jobSchedulingId, visitStatus: visitStatus },
            success: function (response) {
                if (response.status) {
                    debugger;
                    objSwitch.attr('visitStatus', visitStatus);
                    objSwitch.closest('#headingOne').next('div').find('.visit-completed').attr('status', visitStatus);

                    ShowHideCompletedDate(0, response.completedDate, jobSchedulingId);
                    showSuccessMessageJobVisit("Visit status has been changed successfully.");
                    if (visitStatus == "1") {
                        $(".clsVisitStatus").text('Open');
                    }
                    else if (visitStatus == "2") {
                        $(".clsVisitStatus").text('Completed');
                    }
                    SearchHistory();
                }
                else {
                    if (response.error.toLowerCase() == 'sessiontimeout')
                        window.location.href = urlLogout;
                    if (response.error) {
                        showErrorMessageJobVisit(response.error);
                    }
                    else {
                        showErrorMessageJobVisit("Visit status has not been changed.");
                    }
                }
            },
            error: function () {
                showErrorMessageJobVisit("Visit status has not been changed.");
            }
        });
}

function PopulateSignature(jobId, jobScedulingId, e) {
    e.preventDefault();
    $("#jobDocumentPopulate").empty();
    $("#hdnJobSchedulingId").val('');
    $.ajax(
        {
            url: urlGetCheckListDocument,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobId: jobId },
            success: function (response) {
                if (response.length > 0) {
                    for (var i = 0; i < response.length; i++) {

                        if (response[i].FileName.substr((response[i].FileName.lastIndexOf('.') + 1)).toString().toLowerCase() == "pdf") {
                            //var checkbox = '<li><input type=checkbox class="checkbox" jobDocumentId=' + response[i].JobDocumentId + ' jobId=' + response[i].JobId + ' stage=' + response[i].Stage + ' name=' + response[i].Name + ' />' + response[i].Name + '</li>';
                            var checkbox = '<li><input type=checkbox class="checkbox" jobDocumentId=' + response[i].JobDocumentId + ' jobId=' + response[i].JobId + ' DownloadURLPath=' + response[i].DownloadURLPath + ' name=' + response[i].FileName + ' />' + response[i].FileName + '</li>';
                            $("#jobDocumentPopulate").append(checkbox);
                        }
                    }
                    $("#hdnJobSchedulingId").val(jobScedulingId);
                    $("#popupboxPopulateSignature").modal({ backdrop: 'static', keyboard: false });
                }
                else {
                    alert('This job has not any document');
                }
            },
            error: function () {
                showErrorMessageJobVisit("Error while populating signature to documents");
            }
        });
}

function RequestJobData(jobId, jobSchedulingId, e) {
    e.preventDefault();
    $.ajax(
        {
            url: urlRequestJobData,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify({ 'jobId': jobId, 'jobSchedulingId': jobSchedulingId }),
            success: function (response) {
                if (response.status) {
                    showSuccessMessageJobVisit("Request for data created successfully");
                }
                else {
                    showErrorMessageJobVisit("Error while requesting job data");
                }
            },
            error: function () {
                showErrorMessageJobVisit("Error while requesting job data");
            }
        });
}

function SaveSignatureToDocument() {
    var jobDocuments = [];
    $("#jobDocumentPopulate").find('input[type=checkbox]').each(function () {
        if ($(this).is(':checked')) {
            //jobDocuments.push({ JobId: $(this).attr('jobId'), Stage: $(this).attr('stage'), JobDocumentId: $(this).attr('jobDocumentId'), Name: $(this).attr('name') });
            jobDocuments.push({ JobId: $(this).attr('jobId'), JobDocumentId: $(this).attr('jobDocumentId'), DownloadURLPath: $(this).attr('DownloadURLPath') });
        }
    });
    var jobDocumentIds = JSON.stringify(jobDocuments);
    var jobSchedulingId = $("#hdnJobSchedulingId").val();

    $.ajax(
        {
            url: urlSaveSignatureToDocument,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify({ 'documentIds': jobDocumentIds, 'jobSchedulingId': jobSchedulingId }),
            success: function (response) {
                if (response.status) {
                    $('#popupboxPopulateSignature').modal('toggle');
                    showSuccessMessageJobVisit("Visit signature has been populated successfully.");
                }
                else {
                    if (response.error)
                        showErrorMessageJobVisit("Visit signature has not been populated.");
                    else
                        showErrorMessageJobVisit(response.error);
                }
            },
            error: function () {
                showErrorMessageJobVisit("Visit signature has not been populated.");
            }
        });
}

function ClearSignatureToDocument() {
    $("#jobDocumentPopulate").find('input[type=checkbox]').each(function () {
        $(this).prop('checked', false);
    });
}

function ViewVisitSignature(jobSchedulingId, e) {
    e.preventDefault();
    $.ajax(
        {
            url: urlViewVisitSignature,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobSchedulingId: jobSchedulingId },
            success: function (response) {
                if (response.status) {
                    for (var i = 0; i < response.signatures.length; i++) {
                        var path = sessionUploadedDocPath + response.signatures[i].Path;
                        if (response.signatures[i].SignatureTypeId == 4) {
                            $("#imgDesignerSignature").attr('src', path);
                        }
                        if (response.signatures[i].SignatureTypeId == 3) {
                            $("#imgElectricianSignature").attr('src', path);
                        }
                        if (response.signatures[i].SignatureTypeId == 1) {
                            $("#imgOwnerSignature").attr('src', path);
                        }
                        if (response.signatures[i].SignatureTypeId == 2) {
                            $("#imgInstallerSignature").attr('src', path);
                        }
                        if (response.signatures[i].SignatureTypeId == 5) {
                            $("#imgOtherSignature").attr('src', path);
                        }
                    }
                    $("#popupboxViewSignature").modal({ backdrop: 'static', keyboard: false });
                }
                else {
                }
            },
            error: function () {
            }
        });
}

function ShowHideCompletedDate(isFirstTimeLoad, completedDate, jobSchedulingId) {
    $(".visit-completed").each(function () {
        var status = $(this).attr('status');
        if (status == 1) {
            $(this).find('#openStatus').show();
            $(this).find('#completedStatus').hide();
            $(this).find('#displayCompletedDate').hide();
        }
        else {
            $(this).find('#openStatus').hide();
            $(this).find('#completedStatus').show();
            $(this).find('#displayCompletedDate').show();
            if (isFirstTimeLoad == 0 && jobSchedulingId == $(this).attr('id')) {
                $(this).find('#displayCompletedDate').find('span').html(completedDate);
            }
        }
    });
}

function GenerateRandomNumber() {
    $.ajax(
        {
            url: urlGenerateRandomNumber,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            async: true,
            success: function (response) {
                if (response.status) {
                    var number = response.randomNumber;
                    $("#TempJobSchedulingId").val(number);
                }
                else {
                }
            },
            error: function () {
            }
        });
}

function addVisit() {
    $(".visitPopupMsg").hide();
    $("#JobSchedulingID").val(0);
    $("#UserId").val('');

    $("#popUpJobDetail").modal({ backdrop: 'static', keyboard: false });
    $("#popUpJobDetail").find('input[type=text]').each(function () {
        $(this).val('');
        $(this).attr('class', 'form-control valid');
    });
    $("#popUpJobDetail").find('.field-validation-error').attr('class', 'field-validation-valid');
    $("#popUpJobDetail").find('Select').each(function () {

        $(this).find('option:first').attr('selected', 'selected');
    });

    $("#popUpJobDetail").find('textarea').each(function () {
        $(this).val('');
    });
    var jobID = modelJobId;
    $("#jobScheduling").find("select[id='JobID']").html('<option value=" ">Select</option><option value="' + jobID + '">' + $("#BasicDetails_RefNumber").val() + '</option>');
    $("#jobScheduling").find("select[id='JobID']").val(jobID);
    $("#jobScheduling").find("select[id='JobID']").attr("disabled", "disabled");

    $("#popUpJobDetail").find("#RefNumber").val($("#BasicDetails_RefNumber").val());
    $("#popUpJobDetail").find("#Detail").val($("#BasicDetails_Description").val());

    GenerateRandomNumber();
    visitCheckListItemIds = [];
    isVisitCheckListTemplateChange = false;

    TempCheckListTemplateItemAdd(modelDefaultTemplateId, true, $("#BasicDetails_JobID").val());

}

if ($('#btnAssignInstaller').length > 0) {
    $('#btnAssignInstaller').click(function (e) {
        e.preventDefault();
        addVisit();
    });
}

if ($('#btnScheduleInstaller').length > 0) {
    $('#btnScheduleInstaller').click(function (e) {
        e.preventDefault();
        $("#horizontalTab ul li:eq(7)").trigger('click');
        addVisit();
    });
}

HideInstaller();

function HideInstaller() {
    if ($('[name="JobSchedule"]').length > 0) {
        $('#btnAssignInstaller').css('display', 'none');
        $('#btnScheduleInstaller').css('display', 'none');
        $('.installer-text').css('display', 'none');
        $('.schedule-an-installer').css('display', 'none');
    }
}



function showErrorMessageJobVisit(message) {
    $(".alert").hide();
    $("#successMsgRegionJobVisit").hide();
    $("#errorMsgRegionJobVisit").html(closeButton + message);
    $("#errorMsgRegionJobVisit").show();
}
function showSuccessMessageJobVisit(message) {
    $(".alert").hide();
    $("#errorMsgRegionJobVisit").hide();
    $("#successMsgRegionJobVisit").html(closeButton + message);
    $("#successMsgRegionJobVisit").show();
}

function VisitStatusChange(e) {
    debugger;
    //event.preventDefault();
    var visitStatus = $(e).attr('visitStatus');
    if (visitStatus == 1)
        visitStatus = 2;
    else
        visitStatus = 1;
    ChangeVisitStatus($(e).attr('jobScedulingId'), visitStatus, $(e));
}
