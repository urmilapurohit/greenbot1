ShowHidePreapprovalBox();
ShowHideConnectionBox();
var distributorID = modelInstallationDistributorID;
$(document).ready(function () {    
    $("#spanPreapprovalComment").html(viewBagPreApprovalComment);
    $("#spanConnectionComment").html(viewBagConnectionComment);
    $("#txtPreapprovalComment").val($("#spanPreapprovalComment").html());
    $("#preApprovalConnectionVal").val(1);
    $("#txtConnComment").val($("#spanConnectionComment").html());
    $("#preApprovalConnectionVal").val(2);
    var previousPreapproval;
    $("#JobStatusForPreApproval").on('focus', function () {
        previousPreapproval = this.value;
    }).change(function () {
        if (confirm('Are you sure you want to change preApproval status?')) {
            $("#preApprovalConnectionVal").val(1);
            SavePreapprovalConnectionComment(1);
            previousPreapproval = this.value;
        }
        else {
            $("#JobStatusForPreApproval").val(previousPreapproval);
        }
    });

    var previousConnection;
    $("#JobStatusForConnection").on('focus', function () {
        previousConnection = this.value;
    }).change(function () {
        if (confirm('Are you sure you want to change connection status?')) {
            $("#preApprovalConnectionVal").val(2);
            SavePreapprovalConnectionComment(1);
            previousConnection = this.value;
        }
        else {
            $("#JobStatusForConnection").val(previousConnection);
        }
    });

    $("#btnPopupPreApproval").click(function (e) {
        $("#txtPreOrConnComment").val($("#spanPreapprovalComment").html());
        $("#preApprovalConnectionVal").val(1);
        $("#popupboxPreapprovalConnectionComment").modal({ backdrop: 'static', keyboard: false });
        e.preventDefault();
    });
   

    $("#btnPopupConnection").click(function (e) {
        $("#txtPreOrConnComment").val($("#spanConnectionComment").html());
        $("#preApprovalConnectionVal").val(2);
        $("#popupboxPreapprovalConnectionComment").modal({ backdrop: 'static', keyboard: false });
        e.preventDefault();
    });

    if (viewBagPreApprovalStatus != '') {
        $("#JobStatusForPreApproval option:contains(" + viewBagPreApprovalStatus + ")").attr('selected', true);
    }
    else
        $("#JobStatusForPreApproval").val(1);

    if (viewBagConnectionStatus != '') {
        $("#JobStatusForConnection option:contains(" + viewBagConnectionStatus + ")").attr('selected', true);
    }
    else
        $("#JobStatusForConnection").val(6);

    $("#preapprovalUL").find('li').each(function () {
        if ($(this).find('#hdnIsApplied').attr('isApplied') && $(this).find('#hdnIsApplied').attr('isApplied').toString().toLowerCase() == "isapplied") {
            $(this).find('.appliedApprovalButton').show();
            $(this).find('.applyApprovalButton').hide();

        }
        else {
            $(this).find('.applyApprovalButton').show();
            $(this).find('.appliedApprovalButton').hide();
        }
    });

    $("#connectionUL").find('li').each(function () {
        if ($(this).find('#hdnIsApplied').attr('isApplied') && $(this).find('#hdnIsApplied').attr('isApplied').toString().toLowerCase() == "isapplied") {
            $(this).find('.appliedConnectionButton').show();
            $(this).find('.applyConnectionButton').hide();

        }
        else {
            $(this).find('.applyConnectionButton').show();
            $(this).find('.appliedConnectionButton').hide();
        }
    });

});

function ApplyPreapprovalConnection(obj, PreApprovalConnOrStcClicked) {

    debugger;
    var toemail = $(obj).attr("email");
    var onlineLink = $(obj).attr("link");
    var DocumentStepId = $(obj).attr("documentstepid");
    var type = $(obj).attr("documentStepType");
    $("#PreApprovalConnOrStcClicked").val(PreApprovalConnOrStcClicked);

    $.ajax({
        url: GetEmailSendViewForPreApprovalAndConnectionURL,
        type: "GET",
        data: { "JobId": BasicDetails_JobID, Type: type, isClassic: false },
        cache: false,
        success: function (Data) {
            debugger;
            $("#loadPreapprovalConnectionStepApply").empty();
            $("#loadPreapprovalConnectionStepApply").html(Data);

            if (type == 1) {
                $("#preApproTo").val(toemail.replace(/,/g, ";"));
                $(".CloseEmailPreApproval").hide();
            }

            if (type == 2) {
                $("#onlineLink").attr('href', onlineLink);
                $("#txtCommentOnReference").val($("#hdnComment" + DocumentStepId).val());
            }

            if (type == 3) {
                $("#DrpAlreadyApplied").val($("#hdnComment" + DocumentStepId).val());
            }

            $('.topbord').hide();
            $("#popupDocumentStepId").val(DocumentStepId);
            $("#popupboxPreapprovalConnectionApply").modal({ backdrop: 'static', keyboard: false });
        }
    });
}

function SavePreapprovalConnectionComment(isStatusChange) {

    debugger;
    var jobStatusId = 0;
    var Status = '';
    var comment = '';
    if ($("#preApprovalConnectionVal").val() == 1) {
        jobStatusId = $("#JobStatusForPreApproval").val()
        Status = $("#JobStatusForPreApproval option:selected").text()
    }
    else {
        jobStatusId = $("#JobStatusForConnection").val()
        Status = $("#JobStatusForConnection option:selected").text()
    }

    if (isStatusChange == 1) {
        if ($("#preApprovalConnectionVal").val() == 1)
            comment = Status;
        else
            comment = Status;
    }
    else {
        if ($("#preApprovalConnectionVal").val() == 1)
            comment = $("#txtPreapprovalComment").val();
        else
            comment = $("#txtConnComment").html();
    }

    var jsonData = JSON.stringify({
        JobId: $("#BasicDetails_JobID").val(),
        JobStatusId: jobStatusId,
        PreApprovalAndConnectionId: $("#preApprovalConnectionVal").val(),
        Comment: comment,
        ModifiedBy: ProjectSession_LoggedInUserId,
        Status: Status
    });

    $.ajax({
        url: AddUpdateJobCommentForPreApprAndConnURL,
        type: "POST",
        dataType: "json",
        data: jsonData,
        contentType: 'application/json; charset=utf-8',
        success: function (Data) {
            debugger;
            if (Data) {
                if ($("#preApprovalConnectionVal").val() == 1) {
                    if (isStatusChange == 1) {
                        showSuccessMessage("Preapproval status has been changed successfully.");
                        //$("#txtPreapprovalComment").val(Status);
                        $("#spanPreapprovalComment").text(Status);
                        $("#txtPreapprovalComment").val(Status);
                    }
                        
                    else {
                        $("#spanPreapprovalComment").text($("#txtPreapprovalComment").val());
                        showSuccessMessage("Preapproval comment has been saved successfully.");
                        $("#popupboxPreapprovalConnectionComment").modal("hide");
                    }
                } else {
                    if (isStatusChange == 1) {
                        showSuccessMessage("Connection status has been changed successfully.");
                        $("#spanConnectionComment").text(Status);
                        $("#txtConnComment").val(Status);
                    }
                        
                    else {
                        $("#spanConnectionComment").text($("#txtConnComment").val());
                        showSuccessMessage("Connection comment has been saved successfully.");
                        $("#popupboxPreapprovalConnectionComment").modal("hide");
                    }
                }
            }
            else {
                if ($("#preApprovalConnectionVal").val() == 1) {
                    if (isStatusChange == 1)
                        showErrorMessage("Preapproval status has not been changed.");
                    else
                        showErrorMessage("Preapproval comment has not been saved.");
                }
                else {
                    if (isStatusChange == 1)
                        showErrorMessage("Connection status has not been changed.");
                    else
                        showErrorMessage("Connection comment has not been saved.");
                }
            }
        }
    });
}

function ClearComment() {
    $("#txtPreapprovalComment").val('');
    $("#txtConnComment").val('');
}

function ShowHidePreapprovalBox() {
    if (!$("#onOffSwitchPreApproval").prop('checked')) {
        $("#preApprovalBox").hide();
        $("#onOffSwitchPreApproval").attr('ison', 1);
    }
    else {
        $("#preApprovalBox").show();
        $("#onOffSwitchPreApproval").attr('ison', 0);
    }
}
function ShowHideConnectionBox() {
    if (!$("#onOffSwitchConnection").prop('checked')) {
        $("#connectionBox").hide();
        $("#onOffSwitchConnection").attr('ison', 1);
    }
    else {
        $("#connectionBox").show();
        $("#onOffSwitchConnection").attr('ison', 0);
    }
}
