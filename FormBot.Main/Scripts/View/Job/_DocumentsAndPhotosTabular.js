$(document).ready(function () {
    //LoadDocuments(documentsJson, false);
});

function SearchHistory() {
    debugger;
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
    debugger;
    $('.IconsEditDelete').each(function () {
        $(this).css("display", "none !important");
    });
}

function ShowSPV() {
    AttachSPVLabelWithSerialNumber();
    $(".isSPVRequired").show();
    $("#SPVLabel").show();
    IsSPVRequired = "True";
    $("#btnSPVProductVerification").show();
    $("#btnSPVProductVerificationErrorLog").show();
}
function HideSPV() {
    RemoveSPVRelatedClass();
    $(".isSPVRequired").hide();
    $("#SPVLabel").hide();
    IsSPVRequired = "False";
    $("#btnSPVProductVerification").hide();
    $("#btnSPVProductVerificationErrorLog").hide();
}

function RemoveSPVRelatedClass() {
    $("div.line-no").removeClass("verified")
    $("div.line-no").removeClass("unverified")
    $("div.line-no").removeClass("installationVerified")
    $("div.line-no").removeClass("notverified")
}


function ReloadSTCJobScreen(jobId) {
    ReloadSTCModule();
}

function ReloadSTCModule() {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlGetSTCJobForTabular + BasicDetails_JobID, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                debugger;
                $("#CESDocbtns").hide();
                $("#STCDocBtns").hide();
                $('.isdelete').css("display", "none");
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 4 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 5) {
                $("#CESDocbtns").show();
                $("#STCDocBtns").show();
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                $('.isdelete').css("display", "block");
            }
            debugger;
            if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $('#btnsave').hide();
            }

            $("#loading-image").hide();
        });
    }, 500);
}