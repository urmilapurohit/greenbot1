/*const { error } = require("jquery");*/
var lineNumberForDuplicateSerial = 0;
var DuplicatePanelSerialNumber;
var DuplicateInverterSerialNumber;
var DuplicateSerialandInverterErrorMsg;
var DuplicateSerialErrorMsgStart = 'Serial numbers cannot be same, ';
var DuplicateSerialErrorMsg = '';
var DuplicateInverterErrorMsg = '';
var inverterSerialValOnPage=false;
var serialValOnPage=false;
$(document).on('mouseenter', '#divHistoryList li', function (event) {
    
    $(".IconsEditDelete", this).css("display", "inline-flex");
}).on('mouseleave', '#divHistoryList li', function () {
    // $("li p a.IconsEditDelete",this).css("background-color", "white");
    $(".IconsEditDelete", this).css("display", "none");
});
if ((typeof $('#IsWholeSaler').val() != 'undefined' && $("#IsWholeSaler").val().toLowerCase() == 'false') || (typeof BasicDetails_IsWholeSaler !== 'undefined' && BasicDetails_IsWholeSaler.toLowerCase() == "false")) {
    $(".SettlementTerms").show();
    $(".total-amonut").show();
}
$(document).on('mouseleave', '#JobSystemDetails_SerialNumbers', function () {
    DuplicateSerialErrorMsg = '';
    var cntSerialNumber = "0";
    var cntNoofPanel = "0";
    serialValOnPage = SerialNumbersValidationOnPage($("#JobSystemDetails_SerialNumbers"), $('#spanSerialNumbers'), true);
    if (serialValOnPage || inverterSerialValOnPage) {
        if (DuplicateSerialErrorMsg == '' && DuplicateInverterErrorMsg == '') {
            DuplicateSerialErrorMsgStart = 'Serial numbers cannot be same, ';
        }
        $('#errorMsgRegionSerialPanel').html(DuplicateSerialErrorMsgStart+DuplicateSerialErrorMsg + DuplicateInverterErrorMsg);
        $('#errorMsgRegionSerialPanel').show();
    }
    else {
        $('#errorMsgRegionSerialPanel').hide();
    }
    cntSerialNumber = ($("#JobSystemDetails_SerialNumbers").val() != '' && $("#JobSystemDetails_SerialNumbers").val() != undefined && $("#JobSystemDetails_SerialNumbers").val() != null) ? $("#JobSystemDetails_SerialNumbers").val().split('\n').length : "0";
    cntNoofPanel = ($("#JobSystemDetails_NoOfPanel").text() != '' && $("#JobSystemDetails_NoOfPanel").text() != undefined && $("#JobSystemDetails_NoOfPanel").text() != null) ? $("#JobSystemDetails_NoOfPanel").text() : "0";
    $("#cntPanelSerialNumber").html(" (" + cntSerialNumber + "/" + cntNoofPanel + ")");
});
$(document).on('mouseleave', '#JobSystemDetails_InverterSerialNumbers', function () {
    DuplicateInverterErrorMsg = '';
    var cntInverterSerialNumber = "0";
    var cntNoOfInverter = "0";
    inverterSerialValOnPage = SerialNumbersValidationOnPage($("#JobSystemDetails_InverterSerialNumbers"), $('#spanInverterSerialNumbers'), false);
    if (inverterSerialValOnPage || serialValOnPage) {
        if (DuplicateSerialErrorMsg == '' && DuplicateInverterErrorMsg == '') {
            DuplicateSerialErrorMsgStart = 'Serial numbers cannot be same, ';
        }
        $('#errorMsgRegionSerialPanel').html(DuplicateSerialErrorMsgStart+DuplicateSerialErrorMsg + DuplicateInverterErrorMsg);
        $('#errorMsgRegionSerialPanel').show();
    }
    else {
        $('#errorMsgRegionSerialPanel').hide();
    }
    if ($("#BasicDetails_JobType").val() == 1) {
        cntInverterSerialNumber = ($("#JobSystemDetails_InverterSerialNumbers").val() != '' && $("#JobSystemDetails_InverterSerialNumbers").val() != undefined && $("#JobSystemDetails_InverterSerialNumbers").val() != null) ? $("#JobSystemDetails_InverterSerialNumbers").val().split('\n').length : "0";
        cntNoOfInverter = ($("#JobSystemDetails_NoOfInverter").text() != '' && $("#JobSystemDetails_NoOfInverter").text() != undefined && $("#JobSystemDetails_NoOfInverter").text() != null) ? $("#JobSystemDetails_NoOfInverter").text() : "0";
        $("#cntInverterSerialNumber").html(" (" + cntInverterSerialNumber + "/" + cntNoOfInverter + ")");
    }
});

var Executed = true;
var SavedData = "";
var showStayMsg = true;

jQuery(function ($) {
    $(document).ajaxStop(function () {
        
        // Executed when all ajax requests are done.
        if (!Executed) {
            SetDataForSSCSCO();
            
            SavedData = CommonDataForSave();
        }
        Executed = true;
    });
});

function CompareJobData() {
    
    SetDataForSSCSCO();
    var currentData = CommonDataForSave();
    
    if (JSON.stringify(currentData) === JSON.stringify(SavedData))
        return true;
    else
        return false;
}

$(document).ready(function () {

    BindNotesType();
    BindUserList();
    SearchHistory();
    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
        LoadWarningNotes();
    }
    
    var cntWarningNotes = $("#cntWarningNotes").text();
    $("#btnWarningNotes").html('Warning Notes (' + cntWarningNotes + ')');

    $("#btnWarningNotes").click(function () {
        $("#warningNotesRegion").toggle();
    });
    $("#historyCategory").change(function () {
        $('#JobHistorySearch').val("");
        SearchHistory();
        $("#width_tmp_option").html($('#historyCategory option:selected').text());
        $(this).width($("#width_tmp_select").width());
    });

    $("#FilterIsImportantNote").change(function () {
        
        $('#JobHistorySearch').val("");
        SearchHistory();
    });

    $("#filter-postvisibility").change(function () {
        
        $('#JobHistorySearch').val("");
        SearchHistory();
        $("#width_tmp_option").html($('#filter-postvisibility option:selected').text());
        $(this).width($("#width_tmp_select").width());
    });

    $('#JobHistorySearch').on('keyup', function () {
        
        tagandsearchfilter();
    });

    $("#relatedTofilter").on("change", function () {
        

        tagandsearchfilter();
        $("#width_tmp_option").html($('#relatedTofilter option:selected').text());
        $(this).width($("#width_tmp_select").width());

    });

    $("#refreshJobHistory").click(function (e) {
        SearchHistory();
    });

    $("#IsDeletedJobNote").on("change", function () {
        SearchHistory();
    });

    //LoadDocuments(documentsJson, false);
    if (JOBType == 1) {
        if (IsLockedSerialNumber == 'true') {

            $("#aLockedSerialnumber").show();
            $("#aUnLockedSerialnumber").hide();
            $("#JobSystemDetails_SerialNumbers").prop('disabled', true);
        }
        else {

            if (IsLockedSerialNumber == 'false' && (USERType == 1 || USERType == 3 || IsLockUnlockSerial == "True")) {
                $("#aUnLockedSerialnumber").show();
            }
            $("#aLockedSerialnumber").hide();
            $('#JobSystemDetails_SerialNumbers').prop('disabled', false);
        }
    }
    else {
        $("#aLockedSerialnumber").hide();
        $("#aUnLockedSerialnumber").hide();
    }
    $("#SerialNumberInverter").hide();
    $("#aInverterSerialNumbers").click(function () {

        $("#SerialNumberPanel").hide();
        $("#SerialNumberInverter").show();
        $(this).closest('ul').find('.active').removeClass('active');
        $(this).closest('li').addClass('active');
    });
    $("#aPanelSerialNumbers").click(function () {
        
        $("#SerialNumberInverter").hide();
        $("#SerialNumberPanel").show();
        $(this).closest('ul').find('.active').removeClass('active');
        $(this).closest('li').addClass('active');
    });
    $("#aLockedSerialnumber").click(function (e) {
        e.preventDefault();
        var result = confirm("Are you sure you want to unlock serialnumbers? if so then job will be not affiliated by SPV.");
        if (result) {
            RemoveSPVOnUnlockSerialnumber();
            if (USERType == 1 || USERType == 3 || IsLockUnlockSerial == "True") {
                $("#aUnLockedSerialnumber").show();
            }
            $("#aLockedSerialnumber").hide();
            $('#JobSystemDetails_SerialNumbers').prop('disabled', false);
            IsLockedSerialNumber = 'false';
            // SearchHistory();
        }
    });

    $("#aUnLockedSerialnumber").click(function (e) {
        e.preventDefault();
        if (USERType == 1 || USERType == 3 || IsLockUnlockSerial == "True") {
            SetSPVOnLockSerialNumber();
            $("#aUnLockedSerialnumber").hide();
            $("#aLockedSerialnumber").show();
            $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
            IsLockedSerialNumber = 'true';
            //  SearchHistory();
        }
    });

    if (BasicDetails_ScoID != 0 && BasicDetails_ScoID > 0) {
        dropDownData.push({ id: 'ScoID', key: "", value: BasicDetails_ScoID, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: jobID }], bText: 'Name', bValue: 'UserId' });
    }
    else {
        dropDownData.push({ id: 'ScoID', key: "", value: null, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: jobID }], bText: 'Name', bValue: 'UserId' });
    }
    $('body').on('click', function (e) {
        $('[data-toggle="popover"]').each(function () {
            if (!$(this).is(e.target) &&
                $(this).has(e.target).length === 0 &&
                $('.popover').has(e.target).length === 0) {
                $(this).popover('hide');
            }
        });
    });
    $('body').on('hidden.bs.popover', function (e) {
        $(e.target).data("bs.popover").inState.click = false;
    });

    $("#SSCID").change(function () {
        var sscname = $("#SSCID option:selected").text();
        $('#BasicDetails_SSCName').val(sscname);
    });
    $("#ScoID").change(function () {
        
        var sconame = $("#ScoID option:selected").text();
        $('#BasicDetails_SCOName').val(sconame);
    });
    dropDownData.push({ id: 'SSCID', key: "", value: BasicDetails_SSCID, hasSelect: true, callback: disableSSCdropdown, defaultText: null, proc: 'User_GetSSCUserByJbID', param: [{ JobID: jobID }], bText: 'Name', bValue: 'UserId' });
    dropDownData.bindDropdown();
    

    $("ul.create-job-menu").on("click", "li", function () {
        if ($(this).closest('ul').attr('id') == "jobAction")
            return false;

        $('ul.create-job-menu').each(function () {
            $(this).find('li').each(function () {
                $(this).removeClass("active");
            });
        });

        $("#invoiceDetail").css("display", "none");


        if ($("#BasicDetails_JobID").val() == "0") {
            $("#ActiveJob").addClass("active");
        } else {
            if ($("#hasJobID").val() == "0") {
                $("#hasJobID").val($("#BasicDetails_JobID").val());
            }
            //prevSelectedTab = $(this).index();
            LoadTabContent($(this).index());
        }

        prevSelectedTab = $(this).index();
        return false;
    });

    $("#btnSPVProductVerification").click(function () {
        SPVProductverification();
    })
    $("#btnSPVProductVerificationErrorLog").click(function () {
        SPVProductionVerificationErrorLog();
    })
    $("#btnReSPVProductVerification").click(function () {
        SPVProductverification(true);
    })

    $("#closePopupSTCJobHistory").click(function () {
        $('#popupSTCJobHistory').modal('toggle');
    });

    $("#aViewSTCJobHistory").click(function () {
        $.get(GetSTCJobHistory + Model_JobID, function (data) {
            $('#STCJobHistoryOfJob').empty();
            $('#STCJobHistoryOfJob').append(data);
            $('#popupSTCJobHistory').modal({ backdrop: 'static', keyboard: false });
        });
    });

    $("#onOffSwitchPreApproval").on('change.bootstrapSwitch', function (event, state) {
        ShowHidePreapprovalBox();
        UpdateAction();
    });

    $("#onOffSwitchConnection").on('change.bootstrapSwitch', function (event, state) {
        ShowHideConnectionBox();
        UpdateAction();
    });
    $("#onOffSwitchNewDocVIewer").on('change.bootstrapSwitch', function (event, state) {
        UpdateActionToSaveIsNewViewerUserWise();
    });
    $(".switch").on("click", function (e) {
        e.stopImmediatePropagation();
    });

    $("#jobNewScreen").closest('body').addClass('grey-bg');

    $("#btnSaveJob").click(function () {
        SaveJobDetail();
    });

    $("#yesWarning").click(function () {
        /*$('#btnSaveJob').hide();*/
        $('#btnSaveJob').attr('disabled', 'disabled');        
        $("#warning").modal('hide');
        SaveJob();
    });

    $("#noWarning").click(function () {
        $("#warning").modal('hide');
        if ($("#BasicDetails_JobID").val() != "0") {
            $("#BasicDetails_JobType").attr('disabled', 'disabled');
            $("#BasicDetails_JobType").change();
        }
    });

    $("#CloseWarning").click(function () {
        $("#STCwarning").modal('hide');
        if ($("#BasicDetails_JobID").val() != "0") {
            $("#BasicDetails_JobType").attr('disabled', 'disabled');
            $("#BasicDetails_JobType").change();
        }
    });
    GetDefaultSettingForJob();

    //checkSerialNumberWithPhotoExistOrNot();
    LoadSerialNumberWithPhotoExistOrNot();

    if (($(".line-no.unverified").length > 0
        || $(".line-no.installationVerified").length > 0
        || $(".line-no.verified").length > 0
        || $(".line-no.notverified").length > 0)
        && $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True") {
        $("#SPVLabel").show();
    }

    else
        $("#SPVLabel").hide();
    if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True") {
        $("#btnSPVProductVerificationErrorLog").show();
        $("#btnSPVProductVerification").show();
    }

    else {

        $("#btnSPVProductVerificationErrorLog").hide();
        $("#btnSPVProductVerification").hide();
    }

    if (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) {
        if ((IsSPVInstallationVerified == 'True' && ($('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17)) || ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17)) {
        //if ((IsSPVInstallationVerified == 'True') || ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17)) {
            $('#btnSaveJob').hide();
        }
    }
    if (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) {
        if ($('#STCStatusId').val() != 14) {
            $('#certificateBlock').hide();
        }
    }
    if ($('#STCStatusId').val() == 14 || isSaveJobAfterTrade == "True")
        $('#btnSaveJob').show();    
    if (basicdetails_IsRegisteredWithGST == 'True') {
        showSnackbar("Company is registered with GST Certificate", false, null, "gstsnackbar");

    }
    else {
        showSnackbar("Company is not registered with GST Certificate", false, null, "gstsnackbar");
    }

    if (basicdetails_IsGenerateRecZip == 'True') {
        $("#onOffSwitchGenerateRecZip").prop('checked', true);
    } else {
        $("#onOffSwitchGenerateRecZip").prop('checked', false);
    }

    $("#onOffSwitchGenerateRecZip").on('change.bootstrapSwitch', function (event, state) {
        var switchStatus = $("#onOffSwitchGenerateRecZip").prop('checked');
        var confirmStatus = true;

        if (switchStatus) {
            if ($('#divDocumentsCES').find('#tbodyCES tr').length == 0 &&
                $('#divDocumentsSTC').find('#tbodySTC tr').length == 0) {
                alert("There are no documents in CES/COC and STC Forms for generating zip file.");
                $("#onOffSwitchGenerateRecZip").prop('checked', false);
                confirmStatus = false;
                return false;
            }

            else {
                if ($('#divDocumentsCES').find('#tbodyCES tr').length > 1 ||
                    $('#divDocumentsSTC').find('#tbodySTC tr').length > 1) {
                    confirmStatus = confirm("There are more than one CES/COC and STC Forms, resulting in larger zip file size.");
                    if (!confirmStatus) {
                        $("#onOffSwitchGenerateRecZip").prop('checked', false);
                        return false;
                    }
                }
            }
        }

        if (confirmStatus) {
            $("#loading-image").show();
            setTimeout(function () {
                $.ajax({
                    url: generateRECZipJobWise,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    type: 'post',
                    data: JSON.stringify({ jobId: jobId, isGenerateRecZip: $("#onOffSwitchGenerateRecZip").prop('checked') }),
                    async: false,
                    success: function (response) {
                        if (response.status) {
                            if ($('#tbodyOther tr[isreczip="true"]').length > 0) {
                                $('#tbodyOther tr[isreczip="true"]').remove();
                            }
                            if ($("#onOffSwitchGenerateRecZip").prop('checked')) {
                                var data = JSON.parse(response.data);
                                var $tbodyOther = $("table #tbodyOther");
                                var path = 'JobDocuments\\' + JobId + '\\OTHER\\' + data[0].FileName;
                                var tr = $('<tr/>').data("data", data[0])
                                    .data('path', data[0].Path)
                                    .data('id', data[0].JobDocumentId);
                                tr.addClass("job-document-" + data[0].JobDocumentId);
                                tr.attr("id", "other_" + data[0].JobDocumentId);
                                tr.attr("isreczip", data[0].IsRecZip);
                                tr.appendTo($tbodyOther);
                                var tdForCheckbox = $('<td/>')
                                    .css("width", "2%")
                                    .appendTo(tr);
                                var td = $('<td/>')
                                    .text(data[0].Path.split('\\').pop())
                                    .appendTo(tr);
                                var td1 = $('<td/>')
                                    .addClass('action')
                                    .appendTo(tr);
                                var td2 = $('<td/>')
                                    .addClass('action')
                                    .appendTo(tr);

                                params = "'OTHER','" + "" + "','" + "" + "','" + data[0].Path + "','" + 0 + "','" + ("other_" + data[0].JobDocumentId) + "','" + data[0].JobDocumentId + "'";
                                var aSendMailDocSignForOther = $('<a/>')
                                    .addClass('icon sprite-img email-ic document-signature-send-request')
                                    .attr('href', 'javascript:void(0);')
                                    .attr('data-filename', data[0].Path.split('\\').pop())
                                    .attr('title', 'Signature Mail')
                                    .data('type', 'OTHER')
                                    .attr('onclick', 'viewDocument(' + params + ')')
                                    .appendTo(td2);

                                if (!(allowdViewTypes.indexOf(data[0].Path.toLowerCase().split('.').pop()) > -1)) {
                                    OpacityIncreaseDecrease(aSendMailDocSignForOther, 0);
                                }

                                var aDelete = $('<a/>')
                                    .addClass('sprite-img delete')
                                    .attr('href', '#')
                                    .attr('title', 'Delete')
                                    .appendTo(td2);

                                var aDownload = $('<a/>')
                                    .addClass('sprite-img download pop')
                                    .attr('href', 'javascript:void(0)')
                                    .attr('data-container', "body")
                                    .attr('data-toggle', "popover")
                                    .attr('data-placement', "bottom")
                                    .attr('onclick', "onclick=DownloadJobDocuments(" + data[0].JobDocumentId + ")")
                                    .appendTo(td2);

                                showSuccessMessage("Zip file generated successfully.", $("#errorMsgRegion"), $("#successMsgRegion"));
                            }
                            else {
                                showSuccessMessage("Zip file removed successfully.", $("#errorMsgRegion"), $("#successMsgRegion"));
                            }
                        }
                    }
                });
            }, 500);
        }
        else {
            $("#onOffSwitchGenerateRecZip").prop('checked', false);
        }

    });

    $("#inGBSCACode").change(function () {
        if (this.value != null && this.value != "") {
            $("#gbScaCodeError").html("");
        }
        else {
            $("#gbScaCodeError").html("GBSCA Code is required.");
        }
    });
    BindAllTabsAjax();
    //showPnlInvoiceElecBillOver60kw();
});


function BindAllTabsAjax() {
    
    $("#loading-image").hide();
    $('#loader-Ajax-notessec').hide();
    //    $.ajax({
    //        url: UrlLoadNotesTabAjax,
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        type: 'get',
    //        data: { jobId: jobId },
    //        async: false,
    //        success: function (response) {
    //            
    //            $("#loading-image").hide();
    //            if (response) {
    //                $('#LoadNotesTabAjax').append(response.notestabView);
    //            }
    //        }
    //    });


        $.ajax({
            url: UrlLoadCustomDetailsAjax,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobId: jobId },
            async: true,
            success: function (response) {
                if (response) {
                    $('#loader-Ajax-customdetail').hide();
                    $('#LoadCustomDetailsAjax').append(response.customdetailView);
                    
                    LoadSystemDetailsAjax();
                }
            }
        });


}

function LoadSystemDetailsAjax() {
    $.ajax({
        url: UrlLoadSystemDetailsAjax,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { jobId: jobId },
        async: true,
        success: function (response) {
            
            if (response) {
                $('#loader-Ajax-system').hide();
                $('#LoadSystemDetailsAjax').append(response.systemdetailView);
                LoadInstallerDesignerElectricianAjax();
                showPnlInvoiceElecBillOver60kw();
            }
        }
    });
}

function LoadInstallerDesignerElectricianAjax() {
    $.ajax({
        url: UrlLoadInstallerDesignerElectricianAjax,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { jobId: jobId },
        async: true,
        success: function (response) {
            
            if (response) {
                $('#loader-Ajax-electrician').hide();
                $('#LoadInstallerDesignerElectricianAjax').append(response.installerdesignerelectricianView);
                LoadJobSchedulingVisitAjax();
            }
        }
    });

}

function LoadJobSchedulingVisitAjax() {
    $.ajax({
            url: UrlLoadJobSchedulingVisitAjax,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobId: jobId },
            async: true,
            success: function (response) {
                
                if (response) {
                    $('#loader-Ajax-Scheduling').hide();
                    $('#LoadJobSchedulingVisitAjax').append(response.JobVisitView);
                    //$('#loader-Ajax-doc').show();
                    if ($("#btnAddVisit:visible").length == 0) {
                        $(".assign-installer").hide();
                        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
                        $('#btnInstallerQuickVisit').removeAttr('onclick');
                    }
                    else {
                        var SEStatus = 2;
                        $(".assign-installer").show();
                        $('#btnInstallerQuickVisit').find('img').attr('src', ProjectImagePath + 'Images/unscheduled-visit.svg');
                        $('#btnInstallerQuickVisit').attr('onClick', BasicDetails_InstallerID > 0 ? 'addQuickVisit($("#hdBasicDetails_InstallerID").val(), $("#txtBasicDetails_InstallerID").val(),' + SEStatus + ');' : 'addQuickVisit($("#hdBasicDetails_SWHInstallerID").val(), $("#txtBasicDetails_SWHInstallerID").val(),' + SEStatus + ');');
                    }
                    LoadDocumentManagerNewViewAjax();
                }
            }
        });

}

function LoadDocumentManagerNewViewAjax() {
    //$('#loader-Ajax-doc').show();
    $.ajax({
        url: UrlLoadDocumentManagerNewViewAjax,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { jobId: jobId },
        async: true,
        success: function (response) {
            
            if (response) {
                //$('#loader-Ajax-doc').hide();
                /*$('#LoadDocumentManagerNewViewAjax').append(response.JobDocumentNewView);*/
                LoadJobDocumentNewAjax();
            }
        }

    });
}

function LoadJobDocumentNewAjax() {
    //$('#loader-Ajax-doc').show();
    $.ajax({
            url: UrlLoadJobDocumentNewAjax,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { jobId: jobId },
            async: true,
        success: function (response) {
                
            if (response) {
                
                $('#loader-Ajax-docstc').hide();
                $('#loader-Ajax-docces').hide();
                $('#loader-Ajax-docSec').hide();
                $('#loader-Ajax-docpanelinvoice').hide();
                $('#loader-Ajax-docelectricity').hide();
                LoadDocuments(documentsJson, false);
                LoadSTCJobNewScreenAjax();
                }
            }

        });
}
function LoadSTCJobNewScreenAjax() {
    //$('#loader-Ajax-doc').show();
    $.ajax({
        url: UrlLoadSTCJobNewScreenAjax,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { jobId: jobId },
        async: true,
        success: function (response) {
            
            if (response) {
                
                $('#loader-Ajax-stcsec').hide();
                $('#loader-Ajax-stcjob').show();
                $('#LoadSTCJobNewScreenAjax').append(response.STCJobNewScreenView);
                //ReloadSTCJobScreen(jobId);
                PricingSettlementTermOnLoad();
                $('#loader-Ajax-stcjob').hide();
                LoadNewJobPhotoAjax();

            }
        }

    });
}

function LoadNewJobPhotoAjax() {
    //$('#loader-Ajax-doc').show();
    
    $.ajax({
        url: UrlLoadNewJobPhotoAjax,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'get',
        data: { jobId: jobId },
        async: true,
        success: function (response) {
            
            if (response) {
                
                $('#LoadNewJobPhotoAjax').append(response.JobPhotosNewView);
                $('#loader-Ajax-photosec').hide();
                Executed = false;
            }
            Executed = false;
        }        
    });
    Executed = false;
}



function disableSSCdropdown() {
    if (BasicDetails_SSCID > 0 && ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3 && ProjectSession_UserTypeId != 4 && ProjectSession_UserTypeId != 5 && ProjectSession_UserTypeId != 2)
        $('#SSCID').prop("disabled", true);
    else
        $('#SSCID').prop("disabled", false);
}

$('#btnBackNew').click(function (e) {
    e.preventDefault();
    if (isParentInvoice) {
        $(document).attr('title', 'GreenBot - ' + 'Invoice');
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();
        $("#history").hide();
        $(".assign").hide();
        IsLast = false;
        pageIndex = 1;
        $('#invoice').load(urlGetJobInvoice);
        $('#invoice').show();
        $("#EmailConversation").hide();
        $("#invoiceDetail").css("display", "none");
        setTimeout(function () { loadInvoice(); }, 1500);
    } else {
        window.location.href = urlIndex;
    }

});
$("#btnJobPrint").click(function () {
    //var jobID = $("#BasicDetails_JobID").val();
    var jobID = queryStringJobID;
    var url = urlPrint + jobID;
    window.open(url);
});

function SaveJobDetail() {
    
    fillOwnerPopup();
    fillInstallationPopup();
    var serialVal;
    var inverterSerialVal;
    var isDuplicatePanelSerialNumber = false;
    var isDuplicateInverterSerialNumber = false;
    var popup = CheckShowErrorMessagesForPopup();
    var validpage = $("#frmCreateJob").valid();    

   
    var stcLatitude = $("#JobSTCDetails_Latitude").val();
    var stcLongitude = $("#JobSTCDetails_Longitude").val();
    var decimal = /^[-+]?[0-9]+\.[0-9]+$/;
    if (stcLatitude != "" && !stcLatitude.match(decimal)) {
        showErrorMessage("Please enter latitude in decimal formate of STC job details.");
        return false;
    }
    if (stcLongitude != "" && !stcLongitude.match(decimal)) {
        showErrorMessage("Please enter longitude in decimal formate of STC job details.");
        return false;
    }
    var propertyType = $("#JobInstallationDetails_PropertyType").val();
    var emailOwner = $('#JobOwnerDetails_Email').val();
    if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for commercial jobs.");
        return false;
    }
    if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for installation property type school.");
        return false;
    }
    //if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyPnlInvoice").children().length == 0) && ($("#tbodyElecBill").children().length == 0)) {
    //    showErrorMessage("Please upload Panel Invoice and Electricity Bill.");
    //    return false;
    //}
    //else if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyPnlInvoice").children().length == 0)) {
    //    showErrorMessage("Please upload Panel Invoice.");
    //    return false;
    //}
    //else if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyElecBill").children().length == 0)) {
    //    showErrorMessage("Please upload Electricity Bill.");
    //    return false;
    //}
    var isRequiredFieldofSTC;
    if ($("#BasicDetails_JobType").val() == 1) //PVD job
        isRequiredFieldofSTC = ($("#BasicDetails_strInstallationDate").val() && $("#JobSTCDetails_DeemingPeriod").val() && $("#JobInstallationDetails_PostCode").val()) ? true : false;
    else //SWH job
        isRequiredFieldofSTC = (systemXml.length > 0 && systemXml[0].Brand && systemXml[0].Model && $("#JobInstallationDetails_PostCode").val()) ? true : false;

    var isSystemSize = $("#JobSystemDetails_SystemSize").val() > 0 ? true : false;
    var isInstallationDate = $("#BasicDetails_strInstallationDate").val() ? true : false;

    var isValidDataForSTC;

    if ($("#BasicDetails_JobType").val() == 1)
        isValidDataForSTC = !isSystemSize || (isSystemSize && isRequiredFieldofSTC);
    else
        isValidDataForSTC = !isInstallationDate || (isInstallationDate && isRequiredFieldofSTC);
    if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
        serialVal = true;
    }
    else {
        DuplicateSerialandInverterErrorMsg = '';
        DuplicateSerialandInverterErrorMsg = 'Serial numbers should not be same, ';
        serialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_SerialNumbers"), $('#spanSerialNumbers'), true);
        inverterSerialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_InverterSerialNumbers"), $('#spanInverterSerialNumbers'), false);
    }
    if (serialVal || isDuplicateInverterSerialNumber) {
        showErrorMessage(DuplicateSerialandInverterErrorMsg);
        $('#errorMsgRegionSerialPanel').show();
        $('#errorMsgRegionSerialInverter').show();
    }
    if (validpage && popup && !serialVal && !inverterSerialVal) {

        if (isValidDataForSTC) {
            $("#loading-image").show();
            checkBusinessRules(false);
        }
        else {
            if ($("#BasicDetails_JobType").val() == 1)
                showErrorMessage("Please fill Installation Date, STC DeemingPeriod, Installation postcode to set STC value.");
            else
                showErrorMessage("Please fill Installation Date, System brand, System Model, Installation postcode to set STC value.");
        }

    }
}

function ReloadJobPhotoSection(jobId) {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlReloadJobPhoto + jobId, function (data) {
            $('#loadNewJobPhoto').empty();
            $('#loadNewJobPhoto').append(data);
        });
    }, 500);

}
function fillInstallationPopup() {
    if (InstallationJson.length > 0) {
        var AddressType = InstallationJson[0].PostalAddressType;
        $("#JobInstallationDetails_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobInstallationDetails_UnitTypeID").val(InstallationJson[0].UnitType);
            $("#JobInstallationDetails_UnitNumber").val(InstallationJson[0].UnitNumber);
            $("#JobInstallationDetails_StreetNumber").val(InstallationJson[0].StreetNumber);
            $("#JobInstallationDetails_StreetName").val(InstallationJson[0].StreetName);
            $("#JobInstallationDetails_StreetTypeID").val(InstallationJson[0].StreetType);
            $("#JobInstallationDetails_PostalAddressID").val('');
            $("#JobInstallationDetails_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobInstallationDetails_PostalAddressID").val(InstallationJson[0].PostalAddressID);
            $("#JobInstallationDetails_PostalDeliveryNumber").val(InstallationJson[0].PostalDeliveryNumber);
            $("#JobInstallationDetails_UnitTypeID").val("");
            $("#JobInstallationDetails_UnitNumber").val("");
            $("#JobInstallationDetails_StreetNumber").val("");
            $("#JobInstallationDetails_StreetName").val("");
            $("#JobInstallationDetails_StreetTypeID").val("");
        }

        $("#JobInstallationDetails_Town").val(InstallationJson[0].Town);
        $("#JobInstallationDetails_State").val(InstallationJson[0].State);
        $("#JobInstallationDetails_PostCode").val(InstallationJson[0].PostCode);
    }

    else {
        $("#JobInstallationDetails_AddressID").val(1);
        $("#JobInstallationDetails_UnitTypeID").val("");
        $("#JobInstallationDetails_UnitNumber").val("");
        $("#JobInstallationDetails_StreetNumber").val("");
        $("#JobInstallationDetails_StreetName").val("");
        $("#JobInstallationDetails_StreetTypeID").val("");
        $("#JobInstallationDetails_Town").val('');
        $("#JobInstallationDetails_State").val('');
        $("#JobInstallationDetails_PostCode").val('');
        $("#JobInstallationDetails_PostalAddressID").val('');
        $("#JobInstallationDetails_PostalDeliveryNumber").val('');
        $("#JobInstallationDetails_IsSameAsOwnerAddress").prop('checked', false);
        $('.InstallationDPA').show();
        $('.InstallationPDA').hide();
    }
}

function fillOwnerPopup() {
    if (OwnerAddressJson.length > 0) {
        var AddressType = OwnerAddressJson[0].PostalAddressType;
        $("#JobOwnerDetails_AddressID").val(AddressType);
        if (AddressType == 1) {
            $("#JobOwnerDetails_UnitTypeID").val(OwnerAddressJson[0].UnitType);
            $("#JobOwnerDetails_UnitNumber").val(OwnerAddressJson[0].UnitNumber);
            $("#JobOwnerDetails_StreetNumber").val(OwnerAddressJson[0].StreetNumber);
            $("#JobOwnerDetails_StreetName").val(OwnerAddressJson[0].StreetName);
            $("#JobOwnerDetails_StreetTypeID").val(OwnerAddressJson[0].StreetType);
            $("#JobOwnerDetails_PostalAddressID").val('');
            $("#JobOwnerDetails_PostalDeliveryNumber").val('');
        }
        else {
            $("#JobOwnerDetails_PostalAddressID").val(OwnerAddressJson[0].PostalAddressID);
            $("#JobOwnerDetails_PostalDeliveryNumber").val(OwnerAddressJson[0].PostalDeliveryNumber);
            $("#JobOwnerDetails_UnitTypeID").val("");
            $("#JobOwnerDetails_UnitNumber").val("");
            $("#JobOwnerDetails_StreetNumber").val("");
            $("#JobOwnerDetails_StreetName").val("");
            $("#JobOwnerDetails_StreetTypeID").val("");
        }
        $('#JobOwnerDetails_CompanyName').val(OwnerAddressJson[0].CompanyName);
        $('#JobOwnerDetails_FirstName').val(OwnerAddressJson[0].FirstName);
        $('#JobOwnerDetails_LastName').val(OwnerAddressJson[0].LastName);
        $('#JobOwnerDetails_Email').val(OwnerAddressJson[0].Email);
        $('#JobOwnerDetails_Mobile').val(OwnerAddressJson[0].Mobile);
        $('#JobOwnerDetails_Phone').val(OwnerAddressJson[0].Phone);
        $("#JobOwnerDetails_Town").val(OwnerAddressJson[0].Town);
        $("#JobOwnerDetails_State").val(OwnerAddressJson[0].State);
        $("#JobOwnerDetails_PostCode").val(OwnerAddressJson[0].PostCode);
    }

    else {
        $("#JobOwnerDetails_AddressID").val(1);
        $("#JobOwnerDetails_UnitTypeID").val("");
        $("#JobOwnerDetails_UnitNumber").val("");
        $("#JobOwnerDetails_StreetNumber").val("");
        $("#JobOwnerDetails_StreetName").val("");
        $("#JobOwnerDetails_StreetTypeID").val("");
        $("#JobOwnerDetails_Town").val('');
        $("#JobOwnerDetails_State").val('');
        $("#JobOwnerDetails_PostCode").val('');
        $("#JobOwnerDetails_PostalAddressID").val('');
        $("#JobOwnerDetails_PostalDeliveryNumber").val('');
        $('#JobOwnerDetails_CompanyName').val('');
        $('#JobOwnerDetails_FirstName').val('');
        $('#JobOwnerDetails_LastName').val('');
        $('#JobOwnerDetails_Email').val('');
        $('#JobOwnerDetails_Mobile').val('');
        $('#JobOwnerDetails_Phone').val('');
        $('.OwnerDPA').show();
        $('.OwnerPDA').hide();
    }
}

function CheckShowErrorMessagesForPopup() {
    var ReturnValue = true;
    var StatutoryDeclarations = $('#JobSTCDetails_StatutoryDeclarations').val();

    if (JOBType == '1') {
        if ($("#JobSTCDetails_MultipleSGUAddress").val() == 'Yes' && $("#JobSTCDetails_Location").val() == "") {
            $("#spanSTCLocation").show();
            ReturnValue = false;
        } else { $("#spanSTCLocation").hide(); }
    }

    if ($("#JobSTCDetails_VolumetricCapacity").val() == "Yes") {
        if (StatutoryDeclarations == "Select" || StatutoryDeclarations == null || StatutoryDeclarations == undefined || StatutoryDeclarations.trim() == "") {
            $("#spanStatutoryDeclarations").show();
            ReturnValue = false;
        }
    }

    if ($("#JobSTCDetails_VolumetricCapacity").val() == "Yes") {
        if (StatutoryDeclarations == "Select" || StatutoryDeclarations == null || StatutoryDeclarations == undefined || StatutoryDeclarations.trim() == "") {
            $("#spanStatutoryDeclarations").show();
            ReturnValue = false;
        }
    }

    return ReturnValue;
}

function checkBusinessRules(isFromSTC) {
    if ($("#SSCID").val() != "" && $("#SSCID").val() > 0) {
        $("#BasicDetails_SSCID").val($("#SSCID").val());
    }
    if ($("#ScoID").val() != "" && $("#ScoID").val() > 0) {
        $("#BasicDetails_ScoID").val($("#ScoID").val());
    }
    var data = CommonDataForSave();

    $.ajax({
        url: urlCheckBusinessRules,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        processData: false, // Not to process data
        data: data,
        cache: false,
        success: function (result) {
            if (result.IsSuccess) {
                if (result.isIsValidSystemSize == 0) {
                    $("#JobSystemDetails_SystemSize").css("border-color", "red");
                    $("#OldSystemSize").css("border-color", "red");
                }
                else {
                    $("#JobSystemDetails_SystemSize").css("border-color", "");
                    $("#OldSystemSize").css("border-color", "");
                }
                if (result.ValidationSummary != null && result.ValidationSummary != undefined && result.ValidationSummary != '') {
                    if (isFromSTC) {
                        if (ProjectSession_UserTypeId != 1) {

                            $(".STCmodelBodyMessage").html('');
                            $(".STCmodelBodyMessage").append(result.ValidationSummary);
                            $('#STCwarning').modal({ backdrop: 'static', keyboard: false });

                            if (result.IsEMailNotification.toString().toLowerCase() == 'true') {
                                //Email configuration not required
                                $.ajax({
                                    url: urlSendMailForDuplicateAddress,
                                    method: 'GET',
                                    data: { jobid: $('#BasicDetails_JobID').val(), mailList: result.EMailList.trim().toString().toLowerCase() },
                                    cache: false,
                                    async: true,
                                    success: function (Data) {
                                    }
                                });
                            }
                        }
                        else {
                            //LoadStc();
                        }

                    } else {
                        $(".modelBodyMessage").html('');
                        $(".modelBodyMessage").append(result.ValidationSummary);
                        $("#warning").modal();
                    }
                }
                else {
                    if (isFromSTC) {
                        //LoadStc();
                    } else {
                        SaveJob();
                    }
                }
            }
            else if (result.IsError) {
                showErrorMessage(result.ValidationSummary);
                DisableDropDownbyUsertype();
            }
        }
    });
}

function EnableDropDownbyUsertype() {
    if (JOBType == '1') {
        if (USERType == 8 && IsUnderSSC == "True") {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
        if (USERType == 7 || USERType == 9) {

            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
        }
        if (USERType == 2 || USERType == 5) {

            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }

    if (JOBType == '2') {

        if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }

        if (USERType == 2 || USERType == 5) {
            $("#JobInstallationDetails_ExistingSystem").removeAttr('disabled');
            $("#JobElectricians").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#Installer").find('Select').each(function () {
                $(this).removeAttr("disabled");
            });
            $("#frmCreateJob").find('Select').each(function () {

                $(this).removeAttr("disabled");
            });
            $("#BasicDetails_strInstallationDate").removeAttr("disabled");
        }
    }

    if (JOBType == 1) {
        if (IsLockedSerialNumber == 'true') {
            
            $("#aLockedSerialnumber").show();
            $("#aUnLockedSerialnumber").hide();
            $("#JobSystemDetails_SerialNumbers").prop('disabled', true);
        }
        else {
            
            if (IsLockedSerialNumber == 'false' && (USERType == 1 || USERType == 3 || IsLockUnlockSerial == "True")) {
                $("#aUnLockedSerialnumber").show();
            }
            $("#aLockedSerialnumber").hide();
            $('#JobSystemDetails_SerialNumbers').prop('disabled', false);
        }
    }
    else {
        $("#aLockedSerialnumber").hide();
        $("#aUnLockedSerialnumber").hide();
    }

    if (USERType == 1
        || USERType == 3
        || $('#STCStatusId').val() == 10
        || $('#STCStatusId').val() == 12
        || $('#STCStatusId').val() == 14
        || $('#STCStatusId').val() == 17) {
        $('#JobOwnerDetails_OwnerType').prop('disabled', false);
        $('#JobInstallationDetails_PropertyType').prop('disabled', false);
        $('#JobSystemDetails_SystemSize').prop('disabled', false);
        $('#BasicDetails_strInstallationDate').prop('disabled', false);
    }
    else {
        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
        $('#JobSystemDetails_SystemSize').prop('disabled', true);
        $('#BasicDetails_strInstallationDate').prop('disabled', true);
        $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
    }
}

function DisableDropDownbyUsertype() {
    if (jobID > 0) {
        $("#BasicDetails_JobType").attr('disabled', 'disabled');
    }

    if (JOBType == '1') {
        if (USERType == 8 && IsUnderSSC == "True") {
            $("#JobOwner").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            //$("#frmCreateJob").find('Select').each(function () {
            //    $(this).attr('disabled', 'disabled');
            //});
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
        if (USERType == 7 || USERType == 9) {
            $("#JobElectricians").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $('#BasicDetails_JobStage').removeAttr("disabled");
        }
        if (USERType == 2 || USERType == 5) {
            $("#JobElectricians").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
    }

    if (JOBType == '2') {



        if (USERType == 7 || USERType == 9 || (USERType == 8 && IsUnderSSC == "True")) {
            $("#JobElectricians").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
            $('#BasicDetails_JobStage').removeAttr("disabled");
        }

        if (USERType == 2 || USERType == 5) {
            $("#JobElectricians").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobOwner").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#JobInstallationAddress").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#Installer").find('Select').each(function () {
                $(this).attr('disabled', 'disabled');
            });
            $("#BasicDetails_strInstallationDate").attr('disabled', 'disabled');
        }
    }

    if (USERType == 1
        || USERType == 3
        || $('#STCStatusId').val() == 10
        || $('#STCStatusId').val() == 12
        || $('#STCStatusId').val() == 14
        || $('#STCStatusId').val() == 17) {
        $('#JobOwnerDetails_OwnerType').prop('disabled', false);
        $('#JobInstallationDetails_PropertyType').prop('disabled', false);
        $('#JobSystemDetails_SystemSize').prop('disabled', false);
        $('#BasicDetails_strInstallationDate').prop('disabled', false);
    }
    else {
        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
        $('#JobSystemDetails_SystemSize').prop('disabled', true);
        $('#BasicDetails_strInstallationDate').prop('disabled', true);
    }
}

function CommonDataForSave() {
    
    if (oldaddress == null || oldaddress == "") {
        oldaddress = $("#JobInstallationDetails_AddressDisplay").val();
        $("#JobInstallationDetails_oldInstallationAddress").val(oldaddress);
    }

    EnableDropDownbyUsertype();
    $("#BasicDetails_JobType").removeAttr("disabled");
    var panelData = '';
    var xmlPanel = '';
    var inverterData = '';
    var xmlInverter = '';
    
    var batteryManufacturerData = [];    
            batteryManufacturerData = JSON.parse(JSON.stringify(batteryXml));
             
    if (PanelXml.length > 0) {
        var jsonp = JSON.stringify(PanelXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<panel><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
                '<NoOfPanel>' + this['Count'] + '</NoOfPanel><Supplier>' + htmlEncode(this['Supplier']) + '</Supplier></panel>';
        });
        xmlPanel = '<Panels>' + sName + '</Panels>'
    }
    if (InverterXml.length > 0) {

        var jsonp = JSON.stringify(InverterXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<inverter><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
                '<Series>' + htmlEncode(this['Series']) + '</Series><NoOfInverter>' + htmlEncode(this['NoOfInverter']) + '</NoOfInverter></inverter>';
        });
        xmlInverter = '<Inverters>' + sName + '</Inverters>';
    }

    $("#panelXml").val(xmlPanel);
    $("#inverterXml").val(xmlInverter);
    $("#OldPanelDetails").val(JSON.stringify(OldPanelXml));
    $("#NewPanelDetails").val(JSON.stringify(PanelXml));
    $("#OldInverterDetails").val(JSON.stringify(OldInverterXml));
    $("#NewInverterDetails").val(JSON.stringify(InverterXml));

    // Find disabled inputs, and remove the "disabled" attribute
    var disabled = $('form').find(':input:disabled').removeAttr('disabled');

    //serialize form
    var data = JSON.stringify($('form').serializeToJson());

    // re-disabled the set of inputs that you previously enabled
    disabled.attr('disabled', 'disabled');
    var objData = JSON.parse(data);
    if (batteryXml.length == 0 || $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val().toLowerCase() == 'select') {
        objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = '';
    } else {
        objData.JobSTCDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
    }
    if (batteryXml.length == 0 || $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val().toLowerCase() == 'select') {
        objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = '';
    } else {
        objData.JobSTCDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();
    }

    var customLength = $("#customDetails").find('.spanCustomFields').length;

    for (var i = 0; i < customLength; i++) {
        delete objData["lstCustomDetails[" + i + "]"];
    }
    if (batteryManufacturerData.length > 0) {

        for (var i = 0; i < batteryManufacturerData.length; i++) {
            delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
        }
        objData.lstJobBatteryManufacturer = batteryManufacturerData;
    }

    // Find disabled inputs, and remove the "disabled" attribute
    var disabledOwnerData = $('#JobOwner').find(':input:disabled').removeAttr('disabled');

    //serialize form
    var ownerData = JSON.stringify($('#JobOwner').serializeToJson());

    // re-disabled the set of inputs that you previously enabled
    disabledOwnerData.attr('disabled', 'disabled');

    objData.JobOwnerDetails = JSON.parse(ownerData).JobOwnerDetails;

    //merge installation detail
    var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
    var objInstallationAddressData = JSON.parse(installationAddressData);
    installationAddressChangeCheck(objInstallationAddressData.JobInstallationDetails);
    var installationData = JSON.stringify($('#JobInstallationDetailInfo').serializeToJson());
    var objInstallationData = JSON.parse(installationData);

    var extraInstallationData = JSON.stringify($('#JobExtraInstallationInfo').serializeToJson());
    var objExtraInstallationData = JSON.parse(extraInstallationData);

    $.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);
    $.extend(objData.JobInstallationDetails, objInstallationData.JobInstallationDetails);
    $.extend(objData.JobInstallationDetails, objExtraInstallationData.JobInstallationDetails);

    objData.JobInstallationDetails.Latitude = $("#lblLatitude").text();
    objData.JobInstallationDetails.Longitude = $("#lblLongitude").text();
    var jobSystemDetails = objData.JobSystemDetails;
    jobSystemDetails.ModifiedCalculatedSTC = $("#BasicDetails_JobType").val() == 1 ? $("#JobSystemDetails_CalculatedSTC").val() : $("#JobSystemDetails_CalculatedSTCForSWH").val();
    jobSystemDetails.NoOfPanel = $("#JobSystemDetails_NoOfPanel").html();
    jobSystemDetails.NoOfInverter = $("#JobSystemDetails_NoOfInverter").html();
    jobSystemDetails.InstallationType = $("#JobSystemDetails_InstallationType").val();
    if (systemXml.length > 0) {
        jobSystemDetails.SystemBrand = systemXml[0].Brand;
        jobSystemDetails.SystemModel = systemXml[0].Model;
    }
    objData.JobSystemDetails = jobSystemDetails;

    var lstCustomDetails = [];
    $("#customDetails").find('.spanCustomFields').each(function () {
        lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).text(), SeparatorId: $(this).attr('data-SeperatorId') });
    });

    objData.lstCustomDetails = lstCustomDetails;
    objData.OldStcValue = $('#OldStcValue').val() == "" ? 0 : $('#OldStcValue').val();
    var stcHistoryMsg = 'Due to change in ';
    var isStcHistoryMsg = false;
    objData.JobSystemDetails.PreviousSystemSize = $('#OldSystemSize').val();
    if ($('#OldSystemSize').val() != $('#JobSystemDetails_SystemSize').val()) {
        stcHistoryMsg = stcHistoryMsg + 'System Size from ' + $('#OldSystemSize').val() + ' to ' + $('#JobSystemDetails_SystemSize').val();
        isStcHistoryMsg = true;
    }
    
    var OldInstallationDate = moment($('#BasicDetails_strInstallationDateTemp').val()).format('dd/mm/yyyy'.toUpperCase());
    //var OldInstallationDate = $('#BasicDetails_strInstallationDateTemp').val();
    objData.BasicDetails.strInstallationDateTemp = OldInstallationDate;
    if (OldInstallationDate != $('#BasicDetails_strInstallationDate').val()) {
        stcHistoryMsg = stcHistoryMsg + (isStcHistoryMsg ? ', ' : '') + 'Installation Date from ' + OldInstallationDate + ' to ' + $('#BasicDetails_strInstallationDate').val();
        isStcHistoryMsg = true;
    }
    if ($('#OldDeemingPeriod').val() != $('#JobSTCDetails_DeemingPeriod').val()) {
        stcHistoryMsg = stcHistoryMsg + (isStcHistoryMsg ? ', ' : '') + 'Deeming Period from ' + $('#OldDeemingPeriod').val() + ' to ' + $('#JobSTCDetails_DeemingPeriod').val();
    }
    var CheckInstallationDate = moment($('#BasicDetails_strInstallationDate').val(), 'DD/MM/YYYY').add(325, 'd').format('dd/mm/yyyy'.toUpperCase());
    var currentdate = moment().format('DD/MM/YYYY');
    //Check installation date expiry
    if (moment(CheckInstallationDate, 'DD/MM/YYYY') < moment(currentdate, 'DD/MM/YYYY')) {
        objData.IsApproachingExpiryDate = true;
    }
    else {
        objData.IsApproachingExpiryDate = false;
    }
    objData.PropertyType = $('#JobInstallationDetails_PropertyType').val();

    objData.stcHistoryMsg = stcHistoryMsg + '.';
    objData.stcStatusId = $('#STCStatusId').val();
    var PreviousRefNumber = $('#BasicDetails_PreviousRefNumber').val();
    objData.BasicDetails.PreviousRefNumber = PreviousRefNumber;
    var PreviousSSCID = $('#BasicDetails_PreviousSSCID').val();
    objData.BasicDetails.PreviousSSCID = PreviousSSCID;
    var PreviousSCOID = $('#BasicDetails_PreviousSCOID').val();
    objData.BasicDetails.PreviousSCOID = PreviousSCOID;
    var PreviousNMI = $('#JobInstallationDetails_PreviousNMI').val();
    objData.JobInstallationDetails.PreviousNMI = PreviousNMI;
    var PreviousJobStage = $('#BasicDetails_PreviousJobStage').val();
    objData.BasicDetails.PreviousJobStage = PreviousJobStage;
    var PreviousPriority = $('#BasicDetails_PreviousPriority').val();
    objData.BasicDetails.PreviousPriority = PreviousPriority;
    var PreviousInstallingNewPanel = $("#JobInstallationDetails_PreviousInstallingNewPanel").val();
    objData.JobInstallationDetails.PreviousInstallingNewPanel = PreviousInstallingNewPanel;
    
    var PreviousInstallationLocation = $("#JobInstallationDetails_PreviousLocation").val();
    objData.JobInstallationDetails.PreviousLocation = PreviousInstallationLocation;
    var PreviousSingleMultipleStory = $("#JobInstallationDetails_PreviousSingleMultipleStory").val();
    objData.JobInstallationDetails.PreviousSingleMultipleStory = PreviousSingleMultipleStory;
    var PreviousSystemMountingType = $("#JobSTCDetails_PreviousSystemMountingType").val();
    objData.JobSTCDetails.PreviousSystemMountingType = PreviousSystemMountingType;
    var PreviousConnectionType = $("#JobSTCDetails_PreviousConnectionType").val();
    objData.JobSTCDetails.PreviousConnectionType = PreviousConnectionType;
    var PreviousMultipleSGUAddress = $("#JobSTCDetails_PreviousMultipleSGUAddress").val();
    objData.JobSTCDetails.PreviousMultipleSGUAddress = PreviousMultipleSGUAddress;
    var PreviousDeemingPeriod = $("#JobSTCDetails_PreviousDeemingPeriod").val();
    objData.JobSTCDetails.PreviousDeemingPeriod = PreviousDeemingPeriod;
    var PreviousCertificateCreated = $("#JobSTCDetails_PreviousCertificateCreated").val();
    objData.JobSTCDetails.PreviousCertificateCreated = PreviousCertificateCreated;
    var PreviousFailedAccreditationCode = $("#JobSTCDetails_PreviousFailedAccreditationCode").val();
    objData.JobSTCDetails.PreviousFailedAccreditationCode = PreviousFailedAccreditationCode;
    var PreviousFailedReason = $("#JobSTCDetails_PreviousFailedReason").val();
    objData.JobSTCDetails.PreviousFailedReason = PreviousFailedReason;
    var PreviousSTCLocation = $("#JobSTCDetails_PreviousLocation").val();
    objData.JobSTCDetails.PreviousLocation = PreviousSTCLocation;
    var PreviousAdditionalLocationInformation = $("#JobSTCDetails_PreviousAdditionalLocationInformation").val();
    objData.JobSTCDetails.PreviousAdditionalLocationInformation = PreviousAdditionalLocationInformation;
    var PreviousVolumetricCapacity = $("#JobSTCDetails_PreviousVolumetricCapacity").val();
    objData.JobSTCDetails.PreviousVolumetricCapacity = PreviousVolumetricCapacity;
    var PreviousStatutoryDeclarations = $("#JobSTCDetails_PreviousStatutoryDeclarations").val();
    objData.JobSTCDetails.PreviousStatutoryDeclarations = PreviousStatutoryDeclarations;
    var PreviousSecondhandWaterHeater = $("#JobSTCDetails_PreviousSecondhandWaterHeater").val();
    objData.JobSTCDetails.PreviousSecondhandWaterHeater = PreviousSecondhandWaterHeater;
    var PreviousLatitude = $("#JobSTCDetails_PreviousLatitude").val();
    objData.JobSTCDetails.PreviousLatitude = PreviousLatitude;
    var PreviousLongitude = $("#JobSTCDetails_PreviousLongitude").val();
    objData.JobSTCDetails.PreviousLongitude = PreviousLongitude;
    var PreviousAdditionalSystemInformation = $("#JobSTCDetails_PreviousAdditionalSystemInformation").val();
    objData.JobSTCDetails.PreviousAdditionalSystemInformation = PreviousAdditionalSystemInformation;
    
    if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyPnlInvoice").children().length == 0) && ($("#tbodyElecBill").children().length == 0)) {
        objData.IsPanelInvoice = 0;
        objData.IsElectricityBill = 0;
    }
    else if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyPnlInvoice").children().length == 0)) {
        objData.IsPanelInvoice = 0;
        objData.IsElectricityBill = 1;
    }
    else if (($("#JobSystemDetails_SystemSize").val() > 60) && ($("#tbodyElecBill").children().length == 0)) {
        objData.IsElectricityBill = 0;
        objData.IsPanelInvoice = 1;
    }
    else {
        objData.IsElectricityBill = 1;
        objData.IsPanelInvoice = 1;
    }
    var jobData = JSON.stringify(objData);
    return jobData;
}

function LoadCommonSerialNumber() {
    $.ajax({
        url: urloadCommonSerialNumber,
        type: "GET",
        data: { jobId: Model_JobID },
        dataType: "json",
        success: function (Data) {
            if (Data.commonSerialNum) {
                if (Data.commonSerialNum.length > 0) {
                    var div = '<div class="warning-notice" id="divWarning"><h5>Panel Warning Notice:</h5>';
                    for (var i = 0; i < Data.commonSerialNum.length; i++) {
                        var companyName = '';
                        var resellerName = '';
                        if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 5) {
                            companyName = "<b class=commonSerialNum>Company : </b>" + Data.commonSerialNum[i].CompanyName;
                        }
                        if (ProjectSession_UserTypeId == 1) {
                            resellerName = " <b class=commonSerialNum>Reseller : </b>" + Data.commonSerialNum[i].ResellerName;
                        }
                        var p = '';
                        if (ProjectSession_UserTypeId == 1 || (ProjectSession_UserTypeId == 4 && Data.commonSerialNum[i].SolarCompanyId == ProjectSession_SolarCompanyId) || (ProjectSession_UserTypeId == 2 && Data.commonSerialNum[i].ResellerId == ProjectSession_ResellerId)) {
                            p = '<p>Serial ' + Data.commonSerialNum[i].SerialNumber + ' has been used in <a target="_blank" href="/Job/Index?id=' + Data.commonSerialNum[i].Id + '"> ' + Data.commonSerialNum[i].RefNumber + '</a> ' + ' (' + Data.commonSerialNum[i].JobId + ')' + companyName + resellerName + '</p>';
                        } else {
                            p = '<p>Serial ' + Data.commonSerialNum[i].SerialNumber + ' has been used by another company</p>';
                        }

                        div = div + p;
                    }
                    div = div + '</div>';

                    $("#loadCommonSerialNumber").html(div);
                }
                else
                    $("#loadCommonSerialNumber").html('');
            }
            // serialNumber = Data.serialnumbers;
            $("div.line-no").removeClass("verified")
            $("div.line-no").removeClass("unverified")
            $("div.line-no").removeClass("installationVerified")
            $("div.line-no").removeClass("notverified")
            $("#GlobalisAllowedSPV").val(Data.GlobalisAllowedSPV)
            if (Data.IsSPVRequired) {
                IsSPVRequired = "True";
                VerifyUnVerifySerialNumber();
                if ($("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                    $(".isSPVRequired").show();
                else
                    $(".isSPVRequired").hide();
            } else {
                $(".isSPVRequired").hide();
                $("#SPVLabel").hide();
                IsSPVRequired = "False"
            }
        }
    });
}

function LoadCommonInverterSerialNumber() {
    $.ajax({
        url: urloadCommonInverterSerialNumber,
        type: "GET",
        data: { jobId: Model_JobID },
        dataType: "json",
        success: function (Data) {
            if (Data.commonSerialNum) {
                if (Data.commonSerialNum.length > 0) {
                    var div = '<div class="warning-notice" id="divWarning"><h5>Inverter Warning Notice:</h5>';
                    for (var i = 0; i < Data.commonSerialNum.length; i++) {
                        var companyName = '';
                        var resellerName = '';
                        if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 5) {
                            companyName = "<b class=commonSerialNum>Company : </b>" + Data.commonSerialNum[i].CompanyName;
                        }
                        if (ProjectSession_UserTypeId == 1) {
                            resellerName = " <b class=commonSerialNum>Reseller : </b>" + Data.commonSerialNum[i].ResellerName;
                        }
                        var p = '';
                        if (ProjectSession_UserTypeId == 1 || (ProjectSession_UserTypeId == 4 && Data.commonSerialNum[i].SolarCompanyId == ProjectSession_SolarCompanyId) || (ProjectSession_UserTypeId == 2 && Data.commonSerialNum[i].ResellerId == ProjectSession_ResellerId)) {
                            p = '<p>Serial ' + Data.commonSerialNum[i].SerialNumber + ' has been used in <a target="_blank" href="/Job/Index?id=' + Data.commonSerialNum[i].Id + '"> ' + Data.commonSerialNum[i].RefNumber + '</a> ' + ' (' + Data.commonSerialNum[i].JobId + ')' + companyName + resellerName + '</p>';
                        } else {
                            p = '<p>Serial ' + Data.commonSerialNum[i].SerialNumber + ' has been used by another company</p>';
                        }

                        div = div + p;
                    }
                    div = div + '</div>';

                    $("#loadCommonInverterSerialNumber").html(div);
                }
                else
                    $("#loadCommonInverterSerialNumber").html('');
            }

        }
    });
}

function ReloadSTCJobScreen(jobId, isRecUp) {
    $('#loader-Ajax-stcjob').show();
    setTimeout(function () {
        $.get(urlGetSTCJobNewScreen + jobID, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            PricingSettlementTermOnLoad();
            //$('#checkListItemForTrade').load(actionCheckListItemForTrade, callbackCheckList);
            if (!isRecUp && (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3)) {
                $('#btnApplyTradeStc').hide();
            }
            if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $("#CESDocbtns").hide();
                $("#STCDocBtns").hide();
                $("#pnlInvoiceDocbtns").hide();
                $("#elecBillDocBtns").hide();
                $('.isdelete').css("display", "none");
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 5 || ProjectSession_UserTypeId == 4) {
                $("#CESDocbtns").show();
                $("#STCDocBtns").show();
                $("#pnlInvoiceDocbtns").show();
                $("#elecBillDocBtns").show();
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                $('.isdelete').css("display", "block");
            }
            if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $('#btnSaveJob').hide();
            }
            if (isSaveJobAfterTrade == "True") {
                $('#btnSaveJob').show();               
            }

            $('#loader-Ajax-stcjob').hide();
            if ($("#JobSystemDetails_SystemSize").val() > 60) {                
                if ((($("#tbodyPnlInvoice").children().length == 0) || ($("#tbodyElecBill").children().length == 0)) && (ProjectSession_UserTypeId != 1)) {
                    //$('#btnApplyTradeStc').hide();
                    DisableTradeButton();
                }
                else {
                    //$('#btnApplyTradeStc').show();
                    EnableTradeButton();
                }
            }
            else {
                //$('#btnApplyTradeStc').show();
                EnableTradeButton();
            }
        });
    }, 500);
}

function SaveJob(saveType) {
    $('#loading-image').show();
    addLogsSerialno();

    SetDataForSSCSCO(true);

    var data = CommonDataForSave();
    var stcStatusId = $('#STCStatusId').val();
    var postUrl = '';
    if (isAddJob == 'True') {
        postUrl = urlCreate;
    } else if (isEditjob == 'True') {
        postUrl = urlEdit;
    }
    setTimeout(function () {
        $('#loading-image').show();
        $.ajax({
            url: postUrl,
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            processData: false, // Not to process data
            data: data,
            cache: false,
            success: function (result) {

                OldPanelXml = [];
                OldPanelXml = JSON.parse(JSON.stringify(PanelXml));
                OldInverterXml = [];
                OldInverterXml = JSON.parse(JSON.stringify(InverterXml));
                if (result.insert || result.update) {
                    if (result.IsSPVRequired == false) {
                        HideSPV();
                    }

                    if ($("#BasicDetails_JobType").val() == 1) {
                        if (result.STCValue > 0)
                            $("#JobSystemDetails_CalculatedSTC").val(result.STCValue);
                        else
                            $("#JobSystemDetails_CalculatedSTC").val("");
                    }
                    else {
                        if (result.STCValue > 0)
                            $("#JobSystemDetails_CalculatedSTCForSWH").val(result.STCValue);
                        else
                            $("#JobSystemDetails_CalculatedSTCForSWH").val("");
                    }

                    $('#OldStcValue').val(result.STCValue);
                    SavedData = CommonDataForSave();
                }

                if (result.update) {


                    if (!result.IsRecUp && $("#BasicDetails_strInstallationDate").val().trim() != "" && $("#JobSystemDetails_SystemSize").val().trim() != "")
                        showSuccessMessage("Job has been updated but STC Value cannot be calculated since REC website is down.");
                    else
                        showSuccessMessage("Job has been updated.");
                    var updateolddate = $('#BasicDetails_strInstallationDate').val();
                    var updateolddateformat = moment(updateolddate, 'DD/MM/YYYY').format('mm/dd/yyyy'.toUpperCase());
                    var updateoldsystemsize = $('#JobSystemDetails_SystemSize').val();
                    var updateoldrefnumber = $("#BasicDetails_RefNumber").val();
                    var updateoldNMI = $("#JobInstallationDetails_NMI").val();
                    var updateoldJobStage = $("#BasicDetails_JobStage").val();
                    var updateoldJobPriority = $("#BasicDetails_CurrentPriority").val();
                    var updateoldInstallingNewPanel = $("#JobInstallationDetails_InstallingNewPanel").val();
                    var updateoldInstallationLocation = $("JobInstallationDetails_Location").val();
                    var updateoldSingleMultipleStory = $("#JobInstallationDetails_SingleMultipleStory").val();
                    var updateoldSystemMountingType = $("#JobSTCDetails_SystemMountingType").val();
                    var updateoldConnectionType = $("#JobSTCDetails_TypeOfConnection").val();
                    var updateoldMultipleSGUAddress = $("#JobSTCDetails_MultipleSGUAddress").val();
                    var updateoldDeemingPeriod = $("#JobSTCDetails_DeemingPeriod").val();
                    var updateoldCertificateCreated = $("#JobSTCDetails_CertificateCreated").val();
                    var updateoldFailedAccreditationCode = $("#JobSTCDetails_FailedAccreditationCode").val();
                    var updateoldFailedReason = $("#JobSTCDetails_FailedReason").val();
                    var updateoldSTCLocation = $("#JobSTCDetails_Location").val();
                    var updateoldAdditionalLocationInformation = $("#JobSTCDetails_AdditionalLocationInformation").val();
                    var updateoldVolumetricCapacity = $("#JobSTCDetails_VolumetricCapacity").val();
                    var updateoldStatutoryDeclarations = $("#JobSTCDetails_StatutoryDeclarations").val();
                    var updateoldSecondhandWaterHeater = $("#JobSTCDetails_SecondhandWaterHeater").val();
                    var updateoldLatitude = $("#JobSTCDetails_Latitude").val();
                    var updateoldLongitude = $("#JobSTCDetails_Longitude").val();
                    var updateoldAdditionalSystemInformation = $("#JobSTCDetails_AdditionalSystemInformation").val();
                    var updateOldSSCID = $('#BasicDetails_SSCID').val();
                    var updateOldSCOID = $('#BasicDetails_ScoID').val();
                    var updateOldsconame = $("#BasicDetails_ScoID option:selected").text();
                    $('#BasicDetails_PreviousSCOName').val(updateOldsconame);
                    var updateOldsscname = $("#BasicDetails_SSCID option:selected").text();
                    $('#BasicDetails_PreviousSSCName').val(updateOldsscname);
                    $("#BasicDetails_PreviousPriority").val(updateoldJobPriority);
                    $('#BasicDetails_PreviousRefNumber').val(updateoldrefnumber);
                    $('#BasicDetails_PreviousSSCID').val(updateOldSSCID);
                    $('#BasicDetails_PreviousSCOID').val(updateOldSCOID);
                    $('#OldSystemSize').val(updateoldsystemsize);
                    $('#BasicDetails_strInstallationDateTemp').val(updateolddateformat);
                    $('#JobInstallationDetails_PreviousNMI').val(updateoldNMI);
                    $('#BasicDetails_PreviousJobStage').val(updateoldJobStage);
                    $("#JobInstallationDetails_PreviousInstallingNewPanel").val(updateoldInstallingNewPanel);
                    $("JobInstallationDetails_PreviousLocation").val(updateoldInstallationLocation);
                    $("#JobInstallationDetails_PreviousSingleMultipleStory").val(updateoldSingleMultipleStory);
                    $("#JobSTCDetails_PreviousSystemMountingType").val(updateoldSystemMountingType);
                    $("#JobSTCDetails_PreviousConnectionType").val(updateoldConnectionType);
                    $("#JobSTCDetails_PreviousMultipleSGUAddress").val(updateoldMultipleSGUAddress);
                    $("#JobSTCDetails_PreviousDeemingPeriod").val(updateoldDeemingPeriod);
                    $("#JobSTCDetails_PreviousCertificateCreated").val(updateoldCertificateCreated);
                    $("#JobSTCDetails_PreviousFailedAccreditationCode").val(updateoldFailedAccreditationCode);
                    $("#JobSTCDetails_PreviousFailedReason").val(updateoldFailedReason);
                    $("#JobSTCDetails_PreviousLocation").val(updateoldSTCLocation);
                    $("#JobSTCDetails_PreviousAdditionalLocationInformation").val(updateoldAdditionalLocationInformation);
                    $("#JobSTCDetails_PreviousVolumetricCapacity").val(updateoldVolumetricCapacity);
                    $("#JobSTCDetails_PreviousStatutoryDeclarations").val(updateoldStatutoryDeclarations);
                    $("#JobSTCDetails_PreviousSecondhandWaterHeater").val(updateoldSecondhandWaterHeater);
                    $("#JobSTCDetails_PreviousLatitude").val(updateoldLatitude);
                    $("#JobSTCDetails_PreviousLongitude").val(updateoldLongitude);
                    $("#JobSTCDetails_PreviousAdditionalSystemInformation").val(updateoldAdditionalSystemInformation);

                    var obj = jQuery.parseJSON(data);
                    modelInstallationStreetName = $("#JobInstallationDetails_StreetName").val();
                    modelInstallationStreetNumber = $("#JobInstallationDetails_StreetNumber").val();
                    modelInstallationStreetTypeID = $("#JobInstallationDetails_StreetTypeID").val();
                    modelInstallationTown = $("#JobInstallationDetails_Town").val();
                    modelInstallationState = $("#JobInstallationDetails_State").val();
                    modelInstallationPostCode = $("#JobInstallationDetails_PostCode").val();
                    oldaddress = $("#JobInstallationDetails_AddressDisplay").val();
                    $("#JobInstallationDetails_oldInstallationAddress").val(oldaddress);
                    var NMI = $("#JobInstallationDetails_NMI").val();
                    if (parseInt(NMI) == 0) {
                        $("#JobInstallationDetails_NMI").val('');
                    }
                    $("#loadPreapprovalConnectionSTC").load(urlLoadPreapprovalConnectionSTC, { createJob: obj });
                    ReloadSTCJobScreen(Model_JobID, result.IsRecUp);
                    ReloadJobPhotoSection($("#BasicDetails_JobID").val());
                    DisableDropDownbyUsertype();
                    GetJobHeader();
                    LoadCommonSerialNumber();
                    LoadCommonInverterSerialNumber();
                    LoadCommonJobsWithSameInstallationAddress();
                    checkSerialNumberWithPhotoExistOrNot();
                    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                        LoadCommonJobsWithSameInstallDateAndInstaller();
                    }
                    SetDataForSSCSCO();
                    SavedData = CommonDataForSave();

                }
                else if (result.error) {
                    showErrorMessage(result.errorMessage);
                    DisableDropDownbyUsertype();
                } else {
                    showErrorMessage("Job has not been saved.");
                    DisableDropDownbyUsertype();
                }
                if (result.insert || result.update) {
                    if ($("#BasicDetails_GSTDocument").val() != '' && $("#BasicDetails_GSTDocument").val() != undefined && $("#BasicDetails_GSTDocument").val() != null)
                        $("#BasicDetails_GSTDocument").attr('OldFileName', $("#BasicDetails_GSTDocument").val());
                    else
                        $("#BasicDetails_GSTDocument").attr('OldFileName', '');
                }
                var cntSerialNumber = "0";
                var cntNoofPanel = "0";
                var cntInverterSerialNumber = "0";
                var cntNoOfInverter = "0";
                cntSerialNumber = ($("#JobSystemDetails_SerialNumbers").val() != '' && $("#JobSystemDetails_SerialNumbers").val() != undefined && $("#JobSystemDetails_SerialNumbers").val() != null) ? $("#JobSystemDetails_SerialNumbers").val().split('\n').length : "0";
                cntNoofPanel = ($("#JobSystemDetails_NoOfPanel").text() != '' && $("#JobSystemDetails_NoOfPanel").text() != undefined && $("#JobSystemDetails_NoOfPanel").text() != null) ? $("#JobSystemDetails_NoOfPanel").text() : "0";
                $("#cntPanelSerialNumber").html(" (" + cntSerialNumber + "/" + cntNoofPanel + ")");
                if ($("#BasicDetails_JobType").val() == 1) {
                    cntInverterSerialNumber = ($("#JobSystemDetails_InverterSerialNumbers").val() != '' && $("#JobSystemDetails_InverterSerialNumbers").val() != undefined && $("#JobSystemDetails_InverterSerialNumbers").val() != null) ? $("#JobSystemDetails_InverterSerialNumbers").val().split('\n').length : "0";
                    cntNoOfInverter = ($("#JobSystemDetails_NoOfInverter").text() != '' && $("#JobSystemDetails_NoOfInverter").text() != undefined && $("#JobSystemDetails_NoOfInverter").text() != null) ? $("#JobSystemDetails_NoOfInverter").text() : "0";
                    $("#cntInverterSerialNumber").html(" (" + cntInverterSerialNumber + "/" + cntNoOfInverter + ")");
                }

                VerifyUnVerifySerialNumber();
                //  window.onbeforeunload = null;
                // SearchHistory();
                /*$('#btnSaveJob').show();*/
                $('#btnSaveJob').removeAttr('disabled');
                $("#loading-image").hide();
            },
            error: function (Error) {
                /*$('#btnSaveJob').show();*/
                $('#btnSaveJob').removeAttr('disabled');
                $("#loading-image").hide();
            }

        });
    },100); 
    $("#BasicDetails_JobType").attr('disabled', 'disabled');
}

function LoadTabContent(index) {
    $('ul.create-job-menu').each(function () {
        $(this).find('li').each(function () {
            $(this).removeClass("active");
        });
    });
    if (index == 0) {
        $("#ActiveJob").addClass("active");
        //  $("#jobTitle").html("Job Details");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        $(document).attr('title', 'GreenBot - ' + 'Job Details');
        $("#invoice").hide();
        $("#history").hide();
        $("#createJobNotes").hide();
        $("#createJobView").show();
        $(".assign").show();
        $("#EmailConversation").hide();
        $("#createJobMessage").hide();
        IsLast = false;
        pageIndex = 1;
        isParentInvoice = false;
    } else if (index == 1) {
        $("#aInvoice").addClass("active");
        //$("#jobTitle").html("Invoice");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        $(document).attr('title', 'GreenBot - ' + 'Invoice');
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();
        $("#history").hide();
        $(".assign").hide();
        IsLast = false;
        pageIndex = 1;
        $('#invoice').load(urlGetJobInvoice, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {
                $('#invoice').show();
                $("#EmailConversation").hide();
                $("#invoiceDetail").css("display", "none");
                loadInvoice();
            }
            if (statusTxt == "error")
                alert("Error: " + xhr.status + ": " + xhr.statusText);
        });
    } else if (index == 2) {
        $("#aNotes").addClass("active");
        // $("#jobTitle").html("Notes");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        if (isNotesView == 'True') {
            // $("#jobTitle").html("Notes");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'Notes');
            $("#createJobView").hide();
            $("#createJobNotes").show();
            $.ajax({
                type: "GET",
                url: urlGetJobNotes + jobIDForScheduling,
                cache: false,
                async: true,
                ifModified: true,
                success: function (Data) {
                    $("#createJobNotes").html(Data);
                },
                error: function (Error) {
                }
            });

            $("#createJobMessage").hide();
            $("#invoice").hide();
            $("#history").hide();
            $("#EmailConversation").hide();
            $(".assign").hide();
            IsLast = false;
            pageIndex = 1;
            setTimeout(function () {
                $("#Notes").focus();
                $(".color:odd").css("background-color", "#fff");
                $(".color1:odd").css("background-color", "#fff");
                $('#hdnJobTitle').val(BasicDetails_Title);
                if ($('#tblCase tr').length == 0) {
                    var trHTML = '';
                    trHTML +=
                        '<tr style="border:none;padding: 10px 20px;height: 34px;text-align:center;">' + '<td colspan="3" style="padding-top:10px;">' + 'No Records Found' + '</td>' + '</tr>';
                    $('#tblCase').append(trHTML);
                }
            }, 2000);
        } else {
            $("#ActiveJob").addClass("active");
        }
        isParentInvoice = false;
    } else if (index == 3) {
        if (isHistoryView == 'True' || isHistoryViewForSCO.toLowerCase == 'true') {
            $("#aHistory").addClass("active");
            // $("#jobTitle").html("History");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'History');
            $("#createJobView").hide();
            $("#createJobNotes").hide();
            $("#createJobMessage").hide();
            $("#EmailConversation").hide();
            $("#invoice").hide();
            $("#history").show();
            $('#history').load(urlShowHistory + jobIDForScheduling);
            $(".assign").hide();
            IsLast = false;
            pageIndex = 1;
        } else { $("#ActiveJob").addClass("active"); }
        isParentInvoice = false;
    } else if (index == 4) {
        if (ProjectSession_IsUserEmailAccountConfigured == 'False') {
            EmailAccountConfigureErrorMessage();
            prevSelectedTab = 0;
            LoadTabContent(prevSelectedTab);
        }
        else {
            if (isMessageView == 'True' || isMessageViewForSCO.toLowerCase == 'true') {
                $("#aMessage").addClass("active");
                //  $("#jobTitle").html("Message");
                $("#jobTitle1").html("<span>" + Model_Header + "</span>");
                $(document).attr('title', 'GreenBot - ' + 'Message');
                $("#createJobView").hide();
                $("#createJobNotes").hide();
                $("#createJobMessage").load(urlLoadMessageTab + $("#BasicDetails_JobID").val());
                $("#createJobMessage").show();
                $("#invoice").hide();
                $("#history").hide();
                $(".assign").hide();
                $("#EmailConversation").hide();
                IsLast = false;
                pageIndex = 1;
            } else { $("#ActiveJob").addClass("active"); }
            isParentInvoice = false;
        }
    }
    else if (index == 5) {
        if (ProjectSession_IsUserEmailAccountConfigured == 'False') {
            prevSelectedTab = 0;
            EmailAccountConfigureErrorMessage();
            LoadTabContent(prevSelectedTab);
            // $("#ActiveJob").addClass("active");
        }
        else {
            $("#aEmail").addClass("active");
            // $("#jobTitle").html("Email");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'Email');
            $("#createJobView").hide();
            $("#createJobNotes").hide();
            $("#createJobMessage").hide();
            $("#EmailConversation").load(urlGetMessageForJobEmailConversationForPreAndConn + $("#BasicDetails_JobID").val());
            $("#EmailConversation").show();
            $("#invoice").hide();
            $("#history").hide();
            $(".assign").hide();
            isParentInvoice = false;
        }
    }
}

function loadInvoice() {
    isParentInvoice = false;

    if ($('#datatable')) {

        var table = $('#datatable').DataTable();
        table.destroy();
    }
    $('#datatable').DataTable({
        iDisplayLength: 10,
        lengthMenu: ProjectConfiguration_GetPageSize,
        order: [[1, "asc"]],
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
            {
                'data': 'JobInvoiceID',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    if (LogInID == full.CreatedBy) {
                        return '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-outgoing.png) no-repeat center;cursor:default; text-decoration:none;margin-left:-30%;"></a>';
                    } else { return '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-incoming.png) no-repeat center;cursor:default; text-decoration:none;margin-left:-30%;"></a>'; }
                },
            },
            { 'data': 'InvoiceNumber', "orderable": true },

            {
                'data': 'CreatedDate',
                "render": function (data) {
                    return ConvertToDateWithFormat(data, ProjectConfiguration_GetDateFormat)
                }
            },
            { 'data': 'Created' },
            { 'data': 'InvoicedTo' },
            {
                'data': 'FileName',
                "render": function (data, type, full, meta) {
                    if (full.Status == 1) {
                        return "<p style='color:green;'>" + full.FileName + '</p>';
                    } else if (full.Status == 2) {
                        return "<p style='color:gray;'>" + full.FileName + '</p>';
                    } else if (full.Status == 3) {
                        return "<p style='color:orange;'>" + full.FileName + '</p>';
                    } else if (full.Status == 4) {
                        return "<p style='color:red;'>" + full.FileName + '</p>';
                    } else { return "<p style='color:red;'></p>"; }

                }, "orderable": false
            },
            {
                'data': 'InvoiceTotal',
                "orderable": true,
                "sClass": "dt-right",
                'render': function (data, type, full, meta) {
                    return PrintDecimal(full.InvoiceTotal);
                },
            },
            {
                'data': 'InvoiceAmountDue',
                "orderable": true,
                "sClass": "dt-right",
                'render': function (data, type, full, meta) {
                    return PrintDecimal(full.InvoiceAmountDue);
                },
            },
            {
                'data': 'JobInvoiceID',
                "render": function (data, type, full, meta) {
                    var pdf = '';
                    if (full.FileExist == '1') {
                        pdf = '<a class="action view" href="javascript:void(0)" style="background:url(../../Images/pdf.png) no-repeat center; text-decoration:none;margin-left:18px;" onclick="DownloadReport(this)" filename="' + full.InvoiceNumber + '.pdf"></a>';
                    }
                    else if (full.FileExist == '0') {
                        pdf = '<a class="action view" style="background:none;text-decoration:none;margin-left:18px;"></a>'
                    }
                    var sent = '';
                    if (full.Sent == 1) {
                        sent = '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-right.png) no-repeat center; cursor:default;text-decoration:none;margin-left:18px;"></a>';
                    } else { sent = '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-wrong.png) no-repeat center; cursor:default;text-decoration:none;margin-left:18px;"></a>'; }

                    var edit = '&nbsp;<a href="javascript:void(0);" class="action view" style="background:url(../../Images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:18px;" onclick="jobInvoiceDetail.loadData(' + full.JobInvoiceID + ')"></a>';
                    var space = '&nbsp;&nbsp;&nbsp;';
                    return pdf + space + sent + edit + '<a href="javascript:void(0)" onclick="deleteInvoice(' + full.JobInvoiceID + ')" class="action view" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;margin-left:18px;"></a>';
                }, "orderable": false
            },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        bServerSide: true,
        sAjaxSource: urlGetInvoiceList + $("#BasicDetails_JobID").val(),
        "fnDrawCallback": function () {
            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length > 1) {
                $("#datatable_length").show();
            } else if ($("#datatable").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            var info = table.page.info();

            if (info.recordsTotal == 0) {
                document.getElementById("invoiceNumRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b>  Invoice(s) found';
            }
            else {
                document.getElementById("invoiceNumRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b>  Invoice(s) found';
            }
            //------------------------------------------------------------------------------------------------------------------------------
        }
    });

    $(".dataTables_empty").css("text-align", "left");

}

var isEmailAlreadyLoaded = false;
var pageIndex = 1;
var pageSize = 10;
var IsLast = false;
var count = 0;
var isParentInvoice = false;

function onclickEvent() {
    var notes = $("#Notes").val();
    var jobID = $("#BasicDetails_JobID").val();
    var title = $("#hdnJobTitle").val();
    var obj = {};
    obj.JobID = jobID;
    obj.Notes = notes;
    obj.JobTitle = title;
    var dataNotes = JSON.stringify(obj);
    var parameters = $('#frmIndex').serialize();
    if (notes == "") {
        $(".alert").hide();
        $("#errorMsgRegion").addClass("alert-danger");
        $("#errorMsgRegion").html(closeButton + "Please enter job notes.");
        $("#errorMsgRegion").show();

    }
    else {
        $.ajax({
            url: urlSaveJobNotes,
            type: "post",
            //data: { notes: notes, jobID: jobID, title: title },
            data: dataNotes,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $(".alert").hide();
                    $("#popuperrorMsgRegion").removeClass("alert-danger");
                    $("#popuperrorMsgRegion").addClass("alert-success");
                    $("#popuperrorMsgRegion").html(closeButton + "Job notes has been saved successfully.");
                    $("#popuperrorMsgRegion").show();

                    $("#Notes").val("");
                    $("#Notes").focus();
                    $.ajax({
                        type: 'POST',
                        url: urlJobList,
                        data: parameters + "&pageIndex=" + 1 + "&jobID=" + jobID,
                        dataType: 'json',
                        async: false,
                        success: function (response) {
                            $("#tblCase tr").remove();
                            var trHTML = '';
                            if (response.PagedList != null) {
                                $.each(response.PagedList, function (i, item) {
                                    if (isDelete == 'false') {
                                        trHTML +=
                                            '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                                            '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                            '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                            '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                                            '</tr>';
                                    }
                                    else {
                                        trHTML +=
                                            '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                                            '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                            '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                            '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                                            '</tr>';
                                    }
                                });
                            }
                            $('#tblCase').append(trHTML);
                            $(".color:odd").css("background-color", "#fff");
                            $(".color1:odd").css("background-color", "#fff");
                        },
                        error: function (req, status, error) {
                            alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
                        }
                    });
                }
                else {
                    $(".alert").hide();
                    $("#popuperrorMsgRegion").removeClass("alert-success");
                    $("#popuperrorMsgRegion").addClass("alert-danger");
                    $("#popuperrorMsgRegion").html(closeButton + "Job note has not been saved.");
                    $("#popuperrorMsgRegion").show();

                }
            },
        });
    }
}

function DeleteJobNotes(jobNotesId) {
    var result = confirm("Are you sure you want to delete this note?");
    var parameters = $('#frmIndex').serialize();
    var jobID = $("#BasicDetails_JobID").val();
    if (result) {
        $.ajax({
            url: urlDeleteJobNotes,
            type: "POST",
            async: false,
            data: JSON.stringify({ jobNotesId: jobNotesId }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $.ajax({
                        type: 'POST',
                        url: urlJobList,
                        data: parameters + "&pageIndex=" + 1 + "&jobID=" + jobID,
                        dataType: 'json',
                        async: false,
                        success: function (response) {
                            $("#tblCase tr").remove();
                            var trHTML = '';
                            if (response.PagedList != null) {
                                $.each(response.PagedList, function (i, item) {
                                    if (isDelete == 'false') {
                                        trHTML +=
                                            '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                                            '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                            '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                            '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                                            '</tr>';
                                    }
                                    else {
                                        trHTML +=
                                            '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                                            '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                            '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                            '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                                            '</tr>';
                                    }
                                });
                            }
                            $('#tblCase').append(trHTML);
                            setTimeout(function () {
                                if ($('#tblCase tr').length == 0) {
                                    var trHTML = '';
                                    trHTML +=
                                        '<tr style="border:none;padding: 10px 20px;height: 34px;text-align:center;">' + '<td colspan="3" style="padding-top:10px;">' + 'No Records Found' + '</td>' + '</tr>';
                                    $('#tblCase').append(trHTML);
                                }
                            }, 200);
                            $(".color:odd").css("background-color", "#fff");
                            $(".color1:odd").css("background-color", "#fff");
                        },
                        error: function (req, status, error) {
                            alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
                        }
                    });
                    $(".alert").hide();
                    $("#successMsgRegion").removeClass("alert-danger");
                    $("#successMsgRegion").addClass("alert-success");
                    $("#successMsgRegion").html(closeButton + "Job notes deleted successfully.");
                    $("#successMsgRegion").show();

                }
            },
        });
    }
}

function scrolldvIndividual(obj) {
    $("#dvIndividual").on("scroll", function (e) {
        $('#tblHeaderScroll').scrollLeft($(tblScroll).scrollLeft());
        var $o = $(e.currentTarget);
        if ($o[0].scrollHeight - $o.scrollTop() <= $o.outerHeight()) {
            GetRecords();
        }
    });
}

function GetRecords() {
    if (!IsLast) {
        pageIndex++;
        var parameters = $('#frmIndex').serialize();
        var jobID = $("#BasicDetails_JobID").val();
        $.ajax({
            type: 'POST',
            url: urlJobList,
            data: parameters + "&pageIndex=" + pageIndex + "&jobID=" + jobID,
            dataType: 'json',
            async: false,
            success: OnSuccess,
            error: function (req, status, error) {
                alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
            }
        });
    }
}

function OnSuccess(response) {
    var trHTML = '';
    if (response.PagedList.length > 0) {
        $.each(response.PagedList, function (i, item) {
            if (isDelete == 'false') {
                trHTML +=
                    '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                    '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                    '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                    '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                    '</tr>';
            }
            else {
                trHTML +=
                    '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                    '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                    '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                    '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' +
                    '</tr>';
            }
        });
    }
    else {
        IsLast = true;
    }
    $('#tblCase').append(trHTML);
    $(".color:odd").css("background-color", "#fff");
    $(".color1:odd").css("background-color", "#fff");
}

var jobInvoiceDetail = jobInvoiceDetail || [];

jobInvoiceDetail = (function () {
    var pub = {};

    pub.loadInvoiceDetailData = function (invoiceID) {
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();

        $("#history").hide();
        $(".assign").hide();
        if ($("#invoice"))
            $("#invoice").css("display", "none");
        if ($("#invoiceDetailList")) {
            $('#invoiceDetailList').html('');
            var jobID = 0;
            if ($("#hasJobID"))
                jobID = $("#hasJobID").val();

            $.ajax({
                url: urlGetJobInvoiceDetail,
                type: "GET",
                dataType: "html",
                data: { invoiceID: invoiceID, jobID: jobID },
                async: true,
                success: function (data) {
                    $('#invoiceDetailList').html(data);
                },
                complete: function (data) {
                    $("#invoiceDetail").css("display", "block");
                    if ($('#invoiceDetailListTable')) {
                        var table = $('#invoiceDetailListTable').DataTable();
                        table.destroy();
                    }
                    pub.loadtable(invoiceID);
                    if ($("#hdnJobInvoiceID")) {
                        $("#hdnJobInvoiceID").val(invoiceID);
                    }
                    $('body').attr('style', 'overflow:scroll;padding-right:0px;');
                },
                error: function (data) {
                    alert(data)
                }
            });
            isParentInvoice = true;
        }
    }

    pub.loadtable = function (invoiceID) {
        var isInvoiced = '', invoiceType = '', queryString = '';
        if ($('#drdInvoiced')) {
            isInvoiced = $('#drdInvoiced').val();
        }
        if ($('#drdInvoiceType')) {
            invoiceType = $('#drdInvoiceType').val();
        }

        if (invoiceID != '')
            queryString += 'invoiceID=' + invoiceID + '&';

        if (isInvoiced != '')
            queryString += 'isInvoiced=' + isInvoiced + '&';

        if (invoiceType != '')
            queryString += 'jobInvoiceType=' + invoiceType;

        if (queryString != '')
            queryString = '?' + queryString;

        var chkCount = 0;

        var jobID = '';
        if ($("#hasJobID"))
            jobID = $("#hasJobID").val();

        $('#invoiceDetailListTable').DataTable({
            paging: false,
            language: {
                sLengthMenu: "Page Size: _MENU_"
            },
            columns: [
                {
                    'data': 'JobInvoiceDetailID',
                    "render": function (data, type, full, meta) {
                        //console.log("JobInvoiceDetailID = "+full.JobInvoiceDetailID);
                        if ($('#chkAll').prop('checked') == true) {
                            return '<input type="checkbox" id="chk_' + full.JobInvoiceDetailID + '" checked>';
                        }
                        else {
                            return '<input type="checkbox" id="chk_' + full.JobInvoiceDetailID + '">';
                        }
                    },
                    "orderable": false
                },
                {
                    'data': 'TimeStart',
                    "render": function (data) {
                        //return this.ConvertToDate(data);
                        return ConvertToDateWithFormat(data, ProjectConfiguration_GetDateFormat)
                    }
                },
                {
                    'data': 'JobInvoiceType',
                    "render": function (data, type, full, meta) {
                        if (full.JobInvoiceType == 1) {
                            return '<img src="/images/invoice-type-part.png" alt="Part" />'
                        }
                        else if (full.JobInvoiceType == 2) {
                            return '<img src="/images/invoice-type-time.png" alt="Time" />'
                        }
                        else if (full.JobInvoiceType == 3) {
                            return '<img src="/images/invoice-type-payment.png" alt="Payment" />'
                        }
                        else {
                            return '';
                        }
                    }, "orderable": false
                },
                { 'data': 'Description' },
                { 'data': 'StaffName', "orderable": false },
                //{ 'data': 'JobScheduleLabel' },
                {
                    'data': 'Quantity', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.Quantity);
                    },
                },
                {
                    'data': 'Sale', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.Sale);
                    },
                },
                {
                    'data': 'SubTotal', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.SubTotal);
                    },
                },
                {
                    'data': 'cost', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.cost);
                    },
                },
                {
                    'data': 'Profit', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.Profit);
                    },
                },
                {
                    'data': 'Margin', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        if (typeof (full.Margin) == 'number') {
                            if (full.Margin === parseInt(full.Margin, 10)) {
                                return full.Margin;
                            } else { return full.Margin.toFixed(0); }
                        }
                        else {
                            return '0';
                        }
                    },
                },
                {
                    'data': 'Tax', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.Tax);
                    },
                },
                {
                    'data': 'Total', "orderable": false, "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.Total);
                    },
                },
                {
                    'data': 'IsBillable',
                    "render": function (data, type, full, meta) {
                        return '<img src="/images/' + full.IsBillableImage + '" alt="" />'
                    }, "orderable": false
                },
                {
                    'data': 'IsInvoiced',
                    "render": function (data, type, full, meta) {

                        if (full.IsInvoiced == true) {
                            return 'Yes'
                        }
                        else if (full.IsInvoiced == false) {
                            return 'No'
                        }
                        else {
                            return '';
                        }
                    }, "orderable": false
                },
                {
                    'data': 'JobInvoiceDetailID',
                    className: 'hide_column',
                },
                {
                    'data': 'JobInvoiceType',
                    className: 'hide_column',
                },
                {
                    'data': 'Payments',
                    className: 'hide_column',
                },
                {
                    'data': 'Remaning',
                    className: 'hide_column',
                },
                {
                    'data': 'TaxAmountConsider',
                    className: 'hide_column',
                }
            ],
            dom: "<<'table-responsive tbfix't><'bottom'l><'clear'>>",
            bServerSide: true,
            sAjaxSource: urlGetInvoiceDetailListing + queryString + "&&jobID=" + jobID,

            "fnDrawCallback": function (row, data, start, end, display) {

                chkCount = 0;
                var api = this.api(), data;
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };

                $("#generateInvoice").attr("data-id", urlCreateReport + invoiceID + '&jobID=' + jobID)
                $("#generateInvoice").attr("style", "cursor:pointer;");
                $("#exportCSV").attr("href", urlCreateCSV + invoiceID)

                $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
                $(".paging a.previous").html("&nbsp");
                $(".paging a.next").html("&nbsp");
                $('.grid-bottom span:first').attr('id', 'spanMain');
                $("#spanMain span").html("");
                $(".ellipsis").html("...");

                if ($(".paging").find("span").length > 1) {
                    $("#invoiceDetailListTable_length").show();
                } else if ($("#invoiceDetailListTable").find("tr").length > 11) { $("#invoiceDetailListTable_length").show(); } else { $("#invoiceDetailListTable_length").hide(); }

                //--------To show display records from total records-------------------
                var table = $('#invoiceDetailListTable').DataTable();
                var info = table.page.info();

                $('#invoiceDetailListTable th:first').removeClass('sorting_asc');

                $('#invoiceDetailListTable tbody').on('click', 'tr td:not(:first-child)', function (e) {

                    var jobInvoiceDetailID = $(e.currentTarget).closest('tr').find('td:eq(15)').html();
                    var jobInvoiceDetailType = $(e.currentTarget).closest('tr').find('td:eq(16)').html();

                    var jobID = '';
                    if ($("#hasJobID"))
                        jobID = $("#hasJobID").val();

                    var xeroApprovedDate = '';
                    if ($("#hdnXeroApprovedDate"))
                        xeroApprovedDate = $("#hdnXeroApprovedDate").val();


                    if (jobInvoiceDetailType == '1' && jobInvoiceDetailID != '' && jobID != ''
                        && xeroApprovedDate == '') {
                        GetJobInvoiceDetailById(jobInvoiceDetailID, jobID);
                    }
                    else if (jobInvoiceDetailType == '2' && jobInvoiceDetailID != '' && xeroApprovedDate == '') {
                        loadAddTime(jobInvoiceDetailID)
                    }
                    else if (jobInvoiceDetailType == '3' && jobInvoiceDetailID != '') {
                        PaymentPopup(jobInvoiceDetailID)
                    }

                });
            },
            "fnServerParams": function (aoData) {
                var table = $('#invoiceDetailListTable').DataTable();

                $('#chkAll').on('click', function () {
                    var rows = table.rows({ 'search': 'applied' }).nodes();
                    $('input[type="checkbox"]', rows).prop('checked', this.checked);
                    chkCount = this.checked ? $('#invoiceDetailListTable >tbody >tr').length : 0;
                });

                $('#invoiceDetailListTable tbody').on('change', 'input[type="checkbox"]', function () {

                    if (this.checked) {
                        chkCount++;
                        if (chkCount == $('#invoiceDetailListTable >tbody >tr').length) {
                            $('#chkAll').prop('checked', this.checked)
                        }
                    }
                    else {
                        chkCount--;
                        $('#chkAll').prop('checked', this.checked)
                    }
                });

                //setTimeout(function(){ console.log("123"); $("btnInvoiceAction").attr("aria-expanded","true");},4000);
            },
            initComplete: function (settings, json) {
            },
        });


    }
    //return pub;
    return {
        loadData: function (invoiceID) { pub.loadInvoiceDetailData(invoiceID); },
        loadInvoiceDetailGrid: function (invoiceID) { pub.loadtable(invoiceID) }
    };

})();

function deleteInvoice(invoiceID) {
    var result = confirm("Are you sure you want to delete this invoice ?");
    if (result) {
        $.ajax({
            url: urlDeleteInvoice,
            type: "POST",
            async: false,
            data: JSON.stringify({ InvoiceID: invoiceID }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.status == "Failed") {
                    $(".alert").hide();
                    $("#errorMsgRegion").removeClass("alert-success");
                    $("#errorMsgRegion").addClass("alert-danger");
                    $("#errorMsgRegion").html(closeButton + data.strErrors);
                    $("#errorMsgRegion").show();


                }
                else {
                    $(".alert").hide();
                    $("#errorMsgRegion").removeClass("alert-danger");
                    $("#errorMsgRegion").addClass("alert-success");
                    $("#errorMsgRegion").html(closeButton + data.strErrors);
                    $("#errorMsgRegion").show();
                    //$("#datatable").dataTable().fnDraw();
                }
                $("#datatable").dataTable().fnDraw();
            },
        });
    }
}

$(document).on("click", "#AddInvoice", function () {
    jobInvoiceDetail.loadData('0');
})

$('#aDeleteJob').click(function (e) {
    e.preventDefault();

    selectedRows = [];
    jobDetail = {
        id: Model_Id,
        jobTitle: $('#BasicDetails_Title').val()
    }
    selectedRows.push(jobDetail);
    if (selectedRows != null && selectedRows.length > 0) {
        var result = confirm("Are you sure you want to delete selected jobs ?");
        if (result) {
            $.ajax({
                url: urlDeleteSelectedJobs,
                type: "POST",
                cache: false,
                data: JSON.stringify({ jobs: selectedRows }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.JsonResponseObj.status == "Completed") {
                        showSuccessMessage("Job has been deleted.");
                        window.location.href = '/Job/Index';
                    }
                    else {
                        showErrorMessage(data.JsonResponseObj.strErrors);
                    }
                },
            });
        }
    }
    else {
        alert('Please select any job for delete.');
    }
});

function GetJobHeader() {
    $.ajax({
        url: urlGetJobHeader,
        type: "GET",
        data: { jobId: Model_JobID },
        dataType: "json",
        success: function (Data) {
            if (Data.status) {
                var refNumber = $("#BasicDetails_RefNumber").val();
                //var companyName = $("#BasicDetails_CompanyName").val();
                var type = '';
                if (JOBType == 1)
                    type = 'PV SOLAR'
                else
                    type = 'SWH'

                var headerHTML = refNumber + '<span style="font-size: 25px;border-left: none;margin-left: 0;">(' + jobId + ')</span>' + '<span id="jobHeader" class="jobHeader"> ' + Data.header + '</span><span class="jobHeader" style="margin-left:0px !important">' + type + '</span>'
                $("#jobTitle").html(headerHTML);
            }
        }
    });
}

$('#btnScrollToTop').click(function (e) {
    e.preventDefault();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
    return false;
});

$("#aSwitch").click(function (e) {
    window.location.href = '/Job/Index?Id=' + eJobId + "&isTabularView=true";;
});

function ReloadSTCModule() {
    ReloadSTCJobScreen(Model_JobID);
}

function GetDefaultSettingForJob() {
    $.ajax(
        {
            url: urlGetDefaultSettingsForJob,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',

            success: function (response) {
                if (response.status) {
                    var data = JSON.parse(response.data);
                    $('#onOffSwitchPreApproval').prop('checked', data.IsPreapproval);
                    ShowHidePreapprovalBox();

                    $('#onOffSwitchConnection').prop('checked', data.IsConnection)
                    ShowHideConnectionBox();
                }
                else {
                    if (response.error) {
                        if (response.error.toLowerCase() == 'sessiontimeout')
                            window.location.href = urlLogout;
                        else
                            showErrorMessage(response.error);
                    }
                    else { }
                    //showErrorMessage("Custom field has not been deleted.");
                }
            },
            error: function () {
                // showErrorMessage("Custom field has not been deleted.");
            }
        });
}
function UpdateActionToSaveIsNewViewerUserWise() {
    var data = {
        IsNewViewer: $('#onOffSwitchNewDocVIewer').prop('checked')
    };

    $.ajax(
        {
            url: urlSaveIsNewViewerUserWise,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify(data),
            success: function (response) {
                if (response == "true") {
                    fnDocViewerOuterPopup();
                    $("#IsNewViewer").val($('#onOffSwitchNewDocVIewer').prop('checked'));
                    // showSuccessMessage("JobAction has been Updated Successfully.");
                }
                else {
                    //showErrorMessage("Custom field has not been deleted.");
                }
            },
            error: function () {
                //showErrorMessage("Custom field has not been deleted.");
            }
        });

}
function UpdateAction() {
    var data = {
        IsPreapproval: $('#onOffSwitchPreApproval').prop('checked'),
        IsConnection: $('#onOffSwitchConnection').prop('checked'), JobId: jobID
    };

    $.ajax(
        {
            url: urlSaveDefaultForJob,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.status) {
                    // showSuccessMessage("JobAction has been Updated Successfully.");
                }
                else {
                    if (response.error) {
                        if (response.error.toLowerCase() == 'sessiontimeout')
                            window.location.href = urlLogout;
                        else
                            showErrorMessage(response.error);
                    }
                    else { }
                    //showErrorMessage("Custom field has not been deleted.");
                }
            },
            error: function () {
                //showErrorMessage("Custom field has not been deleted.");
            }
        });

}

function SetDataForSSCSCO(isFromSaveJob = false) {
    
    if (isAssignSSC_LowerString == 'false') {
        $("#BasicDetails_SSCID").val(BasicDetails_SSCID);
    }
    else {
        if ($("#SSCID").val() != "" && $("#SSCID").val() > 0)
            $("#BasicDetails_SSCID").val($("#SSCID").val());
        else
            $("#BasicDetails_SSCID").val('');
    }
    if (isAssignSCO_LowerString == 'false') {
        $("#BasicDetails_ScoID").val(BasicDetails_ScoID);
    }
    else {
        if ($("#ScoID").val() != "" && $("#ScoID").val() > 0)
            $("#BasicDetails_ScoID").val($("#ScoID").val());
        else
            $("#BasicDetails_ScoID").val('');
    }
    if (isFromSaveJob == false) {
        var Oldsconame = $("#ScoID option:selected").text();
        $('#BasicDetails_PreviousSCOName').val(Oldsconame);
        var Oldsscname = $("#SSCID option:selected").text();
        $('#BasicDetails_PreviousSSCName').val(Oldsscname);
    }

}

function LoadCommonJobsWithSameInstallDateAndInstaller() {
    $.ajax({
        url: urlLoadCommonJobsWithSameInstallDateAndInstaller,
        type: "GET",
        data: { jobId: Model_JobID, installerId: $('#hdBasicDetails_InstallerID').val(), installationDate: $('#BasicDetails_strInstallationDate').val() },
        dataType: "json",
        success: function (Data) {
            //if (Data.commonJobs) {
            if (Data.commonJobs.length > 0) {
                var div = '<div class="warning-notice" id="divWarning"><h5>Warning Notice:</h5> <br/> Installer: <b>' + Data.commonJobs[0].InstallerName + '</b> has been already assigned the following jobs.';
                for (var i = 0; i < Data.commonJobs.length; i++) {
                    var companyName = "<b class=commonJobs>Company : </b>" + Data.commonJobs[i].CompanyName;
                    var resellerName = " <b class=commonJobs>Reseller : </b>" + Data.commonJobs[i].ResellerName;
                    var p = '<p>RefNumber : <a target="_blank" href="/Job/Index?id=' + Data.commonJobs[i].JobID + '"> ' + Data.commonJobs[i].RefNumber + ' </a> ' + companyName + resellerName + '</p>';
                    div = div + p;
                }
                div = div + '</div>';

                $('#loadCommonJobs').show();
                $("#loadCommonJobs").html(div);
            }
            else {
                $("#loadCommonJobs").html('');
                $('#loadCommonJobs').hide();
            }

            if (Data.commonJobsWithFailedSTCStaus.length > 0) {
                var div = '<div class="warning-notice" id="divWarning"><h5>Warning Notice:</h5> <br/> Installer: <b>' + Data.commonJobsWithFailedSTCStaus[0].InstallerName + '</b> has been already assigned the following jobs which have failed stc statuses.';
                for (var i = 0; i < Data.commonJobsWithFailedSTCStaus.length; i++) {
                    var companyName = "<b class=commonJobs>Company : </b>" + Data.commonJobsWithFailedSTCStaus[i].CompanyName;
                    var resellerName = " <b class=commonJobs>Reseller : </b>" + Data.commonJobsWithFailedSTCStaus[i].ResellerName;
                    var p = '<p>RefNumber : <a target="_blank" href="/Job/Index?id=' + Data.commonJobsWithFailedSTCStaus[i].JobID + '"> ' + Data.commonJobsWithFailedSTCStaus[i].RefNumber + ' </a> ' + companyName + resellerName + '</p>';
                    div = div + p;
                }
                div = div + '</div>';

                $('#loadCommonJobsWithFailedStcStatus').show();
                $("#loadCommonJobsWithFailedStcStatus").html(div);
            }
            else {
                $("#loadCommonJobsWithFailedStcStatus").html('');
                $('#loadCommonJobsWithFailedStcStatus').hide();
            }
            //}
        }
    });
}
function RemoveSPVOnUnlockSerialnumber() {
    $.ajax({
        url: urlRemoveSPVOnUnlockSerialnumber,
        type: "GET",
        data: { jobId: Model_JobID },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.status) {
                HideSPV();
                showSuccessMessage("Serialnumber has been unlocked.");
            }
            else {
                showErrorMessage("Serialnumber has not been unlocked.");
            }
        }
    });
}

function SetSPVOnLockSerialNumber() {
    $.ajax({
        url: urlSetSPVOnLockSerialNumber,
        type: "GET",
        data: { jobId: Model_JobID },
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        success: function (result) {
            if (result.status) {
                if (result.isSPVRequired) {
                    $("#GlobalisAllowedSPV").val(result.isSPVRequired);
                    ShowSPV();
                }
                else {
                    HideSPV();
                }
                showSuccessMessage("Serialnumber has been locked.");
            }
            else {
                showErrorMessage("Serialnumber has not been locked.");
            }
        }
    });
}
function LoadCommonJobsWithSameInstallationAddress() {
    $.ajax({
        url: urlLoadCommonJobsWithSameInstallationAddress,
        type: "GET",
        data: { jobId: Model_JobID },
        dataType: "json",
        success: function (Data) {
            //if (Data.commonJobs) {
            if (Data.commonJobs.length > 0) {
                var div = '<div class="warning-notice" id="divWarning"><h5>Warning Notice:</h5> <br/> Same installation address has been already used for the following jobs.';
                for (var i = 0; i < Data.commonJobs.length; i++) {
                    var companyName = "<b class=commonJobs>Company : </b>" + Data.commonJobs[i].CompanyName;
                    var resellerName = " <b class=commonJobs>Reseller : </b>" + Data.commonJobs[i].ResellerName;
                    var stcStatus = " <b class=commonJobs>STC Status : </b>" + Data.commonJobs[i].Status;
                    var p = '<p>RefNumber : <a target="_blank" href="/Job/Index?id=' + Data.commonJobs[i].JobID + '"> ' + Data.commonJobs[i].RefNumber + ' </a> ' + companyName + resellerName + stcStatus + '</p>';
                    div = div + p;
                }
                div = div + '</div>';

                $('#loadCommonJobsWithSameInstallationAddress').show();
                $("#loadCommonJobsWithSameInstallationAddress").html(div);
                isJobWithCommonInstallationAddress = true;
            }
            else {
                $("#loadCommonJobsWithSameInstallationAddress").html('');
                $('#loadCommonJobsWithSameInstallationAddress').hide();
                isJobWithCommonInstallationAddress = false;
            }
        }
    });
}

function LoadSerialNumberWithPhotoExistOrNot() {
    var dataList = JSON.parse(htmlDecode(SerialNumberwithPhotosAvaibilityList).replaceAll("\\", "\\\\"));
    var data = dataList ? dataList.Data : "";
    CommonSerialNumberWithPhotoExistOrNot(data);
}

function CommonSerialNumberWithPhotoExistOrNot(data) {
    if (data.status) {
        if (data.IsPhotoUnAvailable) {
            var failSerialNumber = '';
            if (data.lstNotExistPhoto != undefined && data.lstNotExistPhoto.length > 0) {
                failSerialNumber = data.lstNotExistPhoto.join();
            }
            $("#errorMsgRegion").html(closeButton + "There are serial numbers [" + failSerialNumber + "] listed that do not have matching photos associated.");
            $("#successMsgRegion").hide();
            $("#errorMsgRegion").show();
        }
    }
}

function checkSerialNumberWithPhotoExistOrNot() {
    $.ajax({
        url: urlCheckSerialNumberPhotoAvailability,
        type: "post",
        data: { jobId: Model_JobID },
        dataType: "json",
        success: function (data) {
            CommonSerialNumberWithPhotoExistOrNot(data);
        }
    });
}

function addLogsSerialno() {
    $('#JobSystemDetails_SerialNumbers').removeAttr('disabled');
    var serialnumberArray = $('#JobSystemDetails_SerialNumbers').val($('#JobSystemDetails_SerialNumbers').val().trim()).serializeToJson();
    $('#JobSystemDetails_SerialNumbers').attr('disabled', 'disabled');

    $.ajax({
        url: urlAddlogsSerialNo,
        type: "GET",
        data: {
            jobId: Model_JobID, serialNumber: JSON.stringify(serialnumberArray)
        },
        dataType: "json",
        success: function (data) {
            if (data.status) {
            }
            else {
            }
        }
    });
}

function showMessageForSTC(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegionSTCStatus" : "successMsgRegionSTCStatus";
    var inVisible = isError ? "successMsgRegionSTCStatus" : "errorMsgRegionSTCStatus";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}

function calculateTotal(obj) {
    $("#pricingTerm").html(obj.data("price"));
}
function EnableTradeButton() {
    $("#tradeButton").css("visibility", 'visible');
}
function DisableTradeButton() {
    $("#tradeButton").css("visibility", 'hidden');
}
function STCJobNewScreenOnLoad() {
    var installationDate = new Date($('#BasicDetails_strInstallationDateTemp').val());
    var startDate = new Date('2022/04/30');

    $("#paymentscheduleinfo").hide();
    $("#stcValueForJob").html(pricingManager_STCAmount);
    $("#pricingTerm").html(stc_STCPrice);
    $('#spanSTCStatus').html(stc_Status);
    var STCStatusId = stc_STCStatusId;
    if (STCStatusId != 10 && STCStatusId != 12 && STCStatusId != 14 && STCStatusId != 17) {
        $("#CESDocbtns").hide();
        $("#STCDocBtns").hide();
        $("#pnlInvoiceDocbtns").hide();
        $("#elecBillDocBtns").hide();
        $("#tbodyCES .delete").hide();
        $("#tbodySTC .ganret").hide();
        $("#tbodySTC .delete").hide();
        $("#popupViewerDoc #btnSave").css('display', 'none');
        $('.isdelete').css("display", "none");
    }
    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
        $("#CESDocbtns").show();
        $("#STCDocBtns").show();
        $("#pnlInvoiceDocbtns").show();
        $("#elecBillDocBtns").show();
        $("#tbodyCES .delete").show();
        $("#tbodySTC .ganret").show();
        $("#tbodySTC .delete").show();
        $("#popupViewerDoc #btnSave").show();
        $('.isdelete').css("display", "block");
    }
    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 4 || ProjectSession_UserTypeId == 5) {
        $("#CESDocbtns").show();
        $("#STCDocBtns").show();
        $("#pnlInvoiceDocbtns").show();
        $("#elecBillDocBtns").show();
    }
    if (installationDate > startDate && $('#imgJobRetailerSignatureJobDetailScreen').attr('src') == '') {
        $('#btnApplyTradeStc').hide();
    }

    $("#STCStatusId").val(STCStatusId);
}

window.onbeforeunload = function (e) {
    if (showStayMsg) {        
        if (!CompareJobData()) {            
            return "Changes is not save please save changes.";
        }
    }
    showStayMsg = true;
};
function checkSwitchStatus() {
    var isChecked = false;
    $(".ChecklistPoint").each(function () {
        if ($(this).find("i").hasClass('active')) {
            isChecked = true;
        }
        else {
            isChecked = false;
            return false;
        }

    });
    if (isChecked) {
        $("#onoffswitchCheckAllPoints").prop('checked', true);
    }
    else {
        $("#onoffswitchCheckAllPoints").prop('checked', false);
    }
}

function MarkUnMarkItemForTrade(event, obj, templateId, itemId, isCompleted, visitedCount, totalCount, jobSchedulingId, classTypeId, itemName) {
    var isMark = $(obj).find("i").hasClass('active');
    
    if (classTypeId == 1 && parseInt(visitedCount) < parseInt(totalCount) && !isMark && !itemName.toLowerCase().includes("inverter")) {
        alert('Please capture all serial numbers.');
        event.preventDefault();
        return false;
    }

    $.ajax(
        {
            url: urlMarkUnMarkCheckListItem,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'get',
            data: { templateId: templateId, itemId: itemId, isMark: !isMark, jobSchedulingId: jobSchedulingId },
            success: function (response) {
                if (response.id && response.id > 0) {
                    if (!isMark) {
                        $(obj).find("i").addClass('active');
                    }
                    else {
                        $(obj).find("i").removeClass('active');
                    }
                    showMessageForSTC("CheckList item has been marked/un-marked as completed successfully.");
                }
                else {
                    if (response.error.toLowerCase() == 'sessiontimeout')
                        window.location.href = urlLogout;
                    else {
                        if (response.error)
                            showMessageForSTC(response.error);
                        else
                            showMessageForSTC("CheckList item has not been marked/un-marked as completed.");
                    }
                }
                ReloadSTCJobScreen(jobID);
                checkSwitchStatus();
            },
            error: function () {
                showMessageForSTC("CheckList item has not been marked/un-marked as completed.");
            }
        });

    event.preventDefault();
    return false;
}
function checkListFirTradeOnLoad() {
    $("#onoffswitchCheckAllPoints").change(function (ev) {
        var checked = ev.target.checked;
        if (checked) {
            $(".ChecklistPoint").each(function () {
                if (!$(this).find("i").hasClass('active')) {
                    $(this).click();
                }
            });
        }
        else {
            $(".ChecklistPoint").each(function () {
                if ($(this).find("i").hasClass('active')) {
                    $(this).click();
                }
            });
        }

        checkSwitchStatus();
    });
}

//$(document).ready(function () {
//    BindNotesType();
//    BindUserList();
//    SearchHistory();

//    $("#historyCategory").change(function () {
//        $('#JobHistorySearch').val("");
//        SearchHistory();
//        $("#width_tmp_option").html($('#historyCategory option:selected').text());
//        $(this).width($("#width_tmp_select").width());
//    });

//    $("#FilterIsImportantNote").change(function () {
//        
//        $('#JobHistorySearch').val("");
//        SearchHistory();
//    });

//    $("#filter-postvisibility").change(function () {
//        
//        $('#JobHistorySearch').val("");
//        SearchHistory();
//        $("#width_tmp_option").html($('#filter-postvisibility option:selected').text());
//        $(this).width($("#width_tmp_select").width());
//    });

//    $('#JobHistorySearch').on('keyup', function () {
//        
//        tagandsearchfilter();
//    });

//    $("#relatedTofilter").on("change", function () {
//        

//        tagandsearchfilter();
//        $("#width_tmp_option").html($('#relatedTofilter option:selected').text());
//        $(this).width($("#width_tmp_select").width());

//    });

//    $("#refreshJobHistory").click(function (e) {
//    SearchHistory();
//});

//$("#IsDeletedJobNote").on("change", function () {
//    SearchHistory();
//});

//});



function BindNotesType() {
    
    $.ajax({
        type: 'GET',
        url: getnotestypeurl,
        dataType: 'json',
        async: false,
        data: {},
        success: function (notestype) {


            $.each(notestype, function (i, res) {
                
                if (res.Value != "0") {
                    $("#post-visibility").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                }
                $("#filter-postvisibility").append('<option value="' + res.Value + '">' + res.Text + '</option>');

            });
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                $("#filter-postvisibility").append('<option value="5">Warning</option>');
            }

        },
        error: function (ex) {
            alert('Failed to retrieve User list.' + ex);
        }
    });
}

function SearchHistory() {
    
    var categoryID = $('#historyCategory').val();
    categoryID = categoryID != null ? categoryID : 0;
    var IsImportantNote = document.getElementById("FilterIsImportantNote").checked;
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
function LoadWarningNotes() {
    

    //var PostVisibility = "5";
    //var jobId = $("#BasicDetails_JobID").val();
    //var IsDeletedJobNote = "3";
    //var order = "DESC";

    var IsImportantNote = false;
    var PostVisibility = "5";
    var jobId = $("#BasicDetails_JobID").val();
    var IsDeletedJobNote = "3";
    var order = "DESC";
    $("#loading-image").show();

    $.ajax({
        url: urlLoadWarningNotes,
        type: "GET",
        data: { "jobId": jobId, "categoryID": 0, "order": order, "PostVisibility": PostVisibility, "IsImportant": IsImportantNote, "IsDeletedJobNote": IsDeletedJobNote },
        cache: false,
        async: false,
        success: function (Data) {
            

            $("#warningNotesRegion").html($.parseHTML(Data));

            $("divCustom").mCustomScrollbar();
        }
    });
    $("#loading-image").hide();

}





$("#container").resizable({
    minHeight: 300,
    handles: 's',
    alsoResize: '#showfilteredhistory'
});


function BindUserList() {
    $.ajax({
        type: 'GET',
        url: userlisturl,
        dataType: 'json',
        async: false,
        data: { jobid: jobID },
        success: function (userlist) {

            $("#relatedTofilter").prepend('<option value="' + 0 + '"> All </option>')
            $.each(userlist, function (i, res) {
                if (res.Value != " ") {
                    $("#relatedTofilter").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                }
            });


        },
        error: function (ex) {
            alert('Failed to retrieve User list.' + ex);
        }
    });
}


function tagandsearchfilter() {
    
    var count = 0;
    var tagged = $("#relatedTofilter").val();
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



function DownloadHistoryDocument(e) {
    
    var FolderName = $(e).attr("data-folder");
    var FileName = $(e).attr("data-name");
    window.location.href = BaseURLForJob + 'ViewDownloadFileForStc/Job?FileName=' + FileName + '&FolderName=' + FolderName;
}



function DeleteNote(Notes) {
    var Noteid = $(Notes).attr("data-noteid");
    var JobID = jobID;
    $.ajax({
        type: 'GET',
        url: urlDeleteNote,
        dataType: 'json',
        async: false,
        data: { Noteid: Noteid, JobID: JobID },
        success: function (result) {
            if (result.status) {
                showSuccessMessageJobHistory(result.message);
                SearchHistory();

            }
            else {
                showErrorMessageJobHistory(result.message);
            }

        },
        error: function (ex) {
            showErrorMessageJobHistory("Failed to delete Job Note")
        }
    });
}

function EditNote(Notes) {
    var Noteid = $(Notes).attr("data-noteid");
    var JobID = jobID;
    $.ajax({
        type: 'GET',
        url: urlEditNote,
        dataType: 'json',
        async: false,
        data: { Noteid: Noteid, JobID: JobID },
        success: function (result) {
            if (result.status) {
                
                //CKEDITOR.instances.contenteditor.insertHtml(result.Notes);
                //var Notesdescription = result.Notes;
                //alert(Notesdescription);

                var fooCallback = function () {
                    CKEDITOR.instances.contenteditor.focus();
                    //elem = new CKEDITOR.dom.element("elem");
                    //imgDomElem = CKEDITOR.dom.element.createFromHtml(imgElem);
                    //elem.append(imgDomElem);
                    //CKEDITOR.instances.contenteditor.insertElement(elem);
                    CKEDITOR.instances.contenteditor.insertHtml(result.Notes);
                };
                CKEDITOR.instances.contenteditor.setData("", fooCallback);
                $("#post-visibility").val(result.NotesType);
                $("#IsImportantNote").prop('checked', result.IsImportant);
                $("#hiddenNoteID").val(Noteid);
            }
            else {
                showErrorMessageJobHistory(result.message);
            }
        },
        error: function (ex) {
            
            showErrorMessageJobHistory("Failed to Open Job Note.");
        }
    })
}

function ReplyNote(Notes) {
    
    var Noteid = $(Notes).attr("data-noteid");
    var JobID = jobID;
    $.ajax({
        type: 'GET',
        url: urlReplyNote,
        dataType: 'json',
        async: false,
        data: { Noteid: Noteid, JobID: JobID },
        success: function (result) {
            if (result.status) {
                $('#popupAddReplyNote').modal({ backdrop: 'static', keyboard: false });
                $("#hdnReplyNoteID").val(Noteid);
            }

            else {
                showErrorMessageJobHistory("Job Note has been deleted.")
            }
        }

    })
}

function SaveReplyForNotes() {
    
    var Noteid = $("#hdnReplyNoteID").val();
    var JobbID = jobID;
    //var NotesReplyDescription = $("#notereply").val();
    var NotesReplyDescription = CKEDITOR.instances.notecontenteditor.getData();
    var JobRefNo = jobRefNo;
    if (NotesReplyDescription != "") {
        $.ajax({
            type: 'GET',
            url: urlSaveReplyForNote,
            dataType: 'json',
            async: false,
            data: { Noteid: Noteid, JobID: JobbID, NotesReplyDescription: NotesReplyDescription, JobRefNo: JobRefNo },
            success: function (result) {
                if (result.status) {
                    showSuccessMessageJobHistory("Your reply has been added to Job Note.");
                    $('#popupAddReplyNote').modal('toggle');
                    $("#hdnReplyNoteID").val('');
                    CKEDITOR.instances.notecontenteditor.setData('');
                    SearchHistory();
                }
                else {
                    showErrorMessageJobHistory("Your reply has not been saved to Job Note.");
                    $('#popupAddReplyNote').modal('toggle');
                }
            }
        })
    }
    else {
        $("#errorMsgRegionJobReplyNote").html(closeButton + "Reply is required");
        $("#errorMsgRegionJobReplyNote").show();
    }
}

function ClearReplyForNotes() {
    //$("#notereply").val("");
    CKEDITOR.instances.notecontenteditor.setData('');
}

function showSuccessMessageJobHistory(message) {
    $("#errorMsgRegionJobHistory").hide();
    $("#successMsgRegionJobHistory").html(closeButton + message);
    $("#successMsgRegionJobHistory").show();
}
function showErrorMessageJobHistory(message) {
    $("#successMsgRegionJobHistory").hide();
    $("#errorMsgRegionJobHistory").html(closeButton + message);
    $("#errorMsgRegionJobHistory").show();
}
function showEditDeleteIcons() {
    $(".IconsEditDelete").css("display", "block");
}
function hideEditDeleteIcons() {
    
    $('.IconsEditDelete').each(function () {
        $(this).css("display", "none !important");
    });
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
function changeSCAModal() {
    $('#changeScaModal').modal();
}
function changeSCA() {

    var gbSCACode = $("#inGBSCACode").val();
    var oldgbSCACode = $("#GB_SCACode").val();
    if (gbSCACode != null && gbSCACode != "" && gbSCACode != oldgbSCACode) {
        $.ajax({
            url: urlChangeSCA,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'post',
            data: JSON.stringify({ jobId: jobId, gbSCACode: gbSCACode, resellerID: resellerID, oldSCAName: SCAName, solarCompanyId: SolarCompanyId, jobInstallationYear: JobInstallationYear }),
            success: function (response) {
                if (response.success) {
                    showMessageForChangeSCA(response.message, false);
                    $("#GB_SCACode").val(gbSCACode);
                }
                else {
                    showMessageForChangeSCA(response.message, true);
                }
            }
        });
    }
    else {
        if (gbSCACode == oldgbSCACode) {
            showMessageForChangeSCA("You can not enter same SCA Code.", true);
        }
        else {
            $("#gbScaCodeError").html("GBSCA Code is required.");
        }
    }
}

function showMessageForChangeSCA(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegionChangeSCA" : "successMsgRegionChangeSCA";
    var inVisible = isError ? "successMsgRegionChangeSCA" : "errorMsgRegionChangeSCA";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}

$('#aGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetSignatureSelfie').modal('show');
});

$('#installerGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetinstallerSignatureSelfie').modal('show');
});
$('#designerGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetdesignerSignatureSelfie').modal('show');
});
$('#elecGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetelectricianSignatureSelfie').modal('show');
});

function showPnlInvoiceElecBillOver60kw() {
    if ($('#JobSystemDetails_SystemSize').val() > 60) {
        $("#divDocumentsPnlInvoice").show();
        $("#divDocumentsElecBill").show();
    } else {
        $("#divDocumentsPnlInvoice").hide();
        $("#divDocumentsElecBill").hide();
    }
}

function SerialNumbersValidationOnPage(serialnumberFieldId, serialnumberSpanId, IsPanel) {
    
    var isNumbersOverflow = false;

    //var isDuplicateSerialNumber = false;
    var isHaveInvelidChar = false;
    if (serialnumberFieldId.val() != undefined) {
        if (serialnumberFieldId.val().trim() != "") {
            var text = serialnumberFieldId.val().trim();
            var lines = text.split("\n");
            lines = lines.filter(function (n) { return n.length > 0 });
            var count = lines.length;
            for (var i = 0; i < lines.length; i++) {
                if (lines[i].length > 100) {
                    isNumbersOverflow = true;
                    break;
                }
            }
            var isDuplicateSerialNumberOnPage = hasDuplicatesSerialNumber(lines, IsPanel);
        }

    }
    return isDuplicateSerialNumberOnPage;
}
function hasDuplicatesSerialNumber(array, IsPanel) {
    
    ispanelorinverterduplicate = false;
    for (var i = 0; i < array.length; ++i) {
        for (var j = i + 1; j < array.length; j++) {
            if (array[i] == array[j]) {
                
                lineNumberForDuplicateSerial = j + 1;
                if (IsPanel) {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateSerialErrorMsg += 'Panel Serial number ' + DuplicatePanelSerialNumber + ' is duplicate. ';
                }
                
                else {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateInverterErrorMsg+='Inverter Serial number ' + DuplicatePanelSerialNumber + ' is duplicate.';
                }
                ispanelorinverterduplicate = true;
            }
        }
    }
    if (ispanelorinverterduplicate)
        return true;
    else
        return false;
}
function DuplicateSerialNumbersValidation(serialnumberFieldId, serialnumberSpanId, IsPanel) {
    
    var isNumbersOverflow = false;

    //var isDuplicateSerialNumber = false;
    var isHaveInvelidChar = false;
    if (serialnumberFieldId.val() != undefined) {        
        if (serialnumberFieldId.val().trim() != "") {
            var text = serialnumberFieldId.val().trim();
            var lines = text.split("\n");
            lines = lines.filter(function (n) { return n.length > 0 });
            var count = lines.length;
            for (var i = 0; i < lines.length; i++) {                
                if (lines[i].length > 100) {
                    isNumbersOverflow = true;
                    break;
                }
            }
            var isDuplicateSerialNumberOnPage = hasDuplicatesSerialNumberOnPage(lines, IsPanel);
        }

    }
    return isDuplicateSerialNumberOnPage;
}
function hasDuplicatesSerialNumberOnPage(array, IsPanel) {
    
    ispanelorinverterduplicate = false;
    for (var i = 0; i < array.length; ++i) {
        for (var j = i + 1; j < array.length; j++) {
            if (array[i] == array[j]) {
                lineNumberForDuplicateSerial = j + 1;
                if (IsPanel) {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateSerialandInverterErrorMsg+='Panel Serial number ' + DuplicatePanelSerialNumber + ' is duplicate. ';
                }

                else {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateSerialandInverterErrorMsg+='Inverter Serial number ' + DuplicatePanelSerialNumber + ' is duplicate. ';
                }
                ispanelorinverterduplicate = true;
            }
        }
    }
    if (ispanelorinverterduplicate)
        return true;
    else
        return false;
}
