var dropDownData = [];
var isOwnerDetailsFetch = false;
var isInstallationDetails = false;
var isSTCDetails = false;
var isSTCStatus = false;
var isSystemDetails = false;
var isSerialnumberDetails = false;
var isInstallerDetails = false;
var isScheduling = false;
var isDocumentsPhotos = false;
var isNotesHistory = false;
var isWrittenStatementsDeclaration = false;
var isRetailerDetails = false;
var isPreApproval = false;
var isCustomDetails = false;
var isConnections = false;
var UserIDforckeditor = 0;
var isValidTabularData = false;
var isTabularDataUpdated = false;
var DuplicateSerialandInverterErrorMsg;

$(document).ready(function () {
    if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
        LoadWarningNotes();
    }

    var cntWarningNotes = $("#cntWarningNotes").text();
    $("#btnWarningNotes").html('Warning Notes (' + cntWarningNotes + ')');

    $("#btnWarningNotes").click(function () {
        $("#warningNotesRegion").toggle();
    });

    $("#BasicDetails_strInstallationDate").val($("#BasicDetails_strInstallationDateTemp").val());

    $('#pills-tab li:first-child a').tab('show');
    //$("#installation,#stc, #system, #installer, #scheduling, #documents").addClass('disabled');
    //owner
    if ($('#JobOwnerDetails_UnitTypeID option:selected').val() == "") {
        
        $('#lblOwnerUnitNumber').removeClass("required");
        $('#lblOwnerUnitTypeID').removeClass("required");
        $('#lblOwnerStreetNumber').addClass("required");
    }
    else {
        $('#lblOwnerUnitNumber').addClass("required");
        $('#lblOwnerUnitTypeID').addClass("required");
        $('#lblOwnerStreetNumber').removeClass("required");
    }
    BindBasicDetailsValue();
    /*GetDefaultSettingForJob();*/

    $("#BasicDetails_strInstallationDate").change(function () {
        var changeFinalYear = '', changeDeemingPeriod;
        var changeinstallationDate = $("#BasicDetails_strInstallationDate").val();
        if (changeinstallationDate != null && changeinstallationDate != undefined && changeinstallationDate != '') {
            changeFinalYear = moment(changeinstallationDate, "DD/MM/YYYY").format('yyyy'.toUpperCase());
        }
        changeDeemingPeriod = $('#JobSTCDetails_DeemingPeriod').val();
        BindDeemingPeriodDropdown(changeFinalYear, 0);
    });

    $("#hdnsolarCompanyid").val(SolarCompanyId_Glbl);

    $("#inGBSCACode").change(function () {
        if (this.value != null && this.value != "") {
            $("#gbScaCodeError").html("");
        }
        else {
            $("#gbScaCodeError").html("GBSCA Code is required.");
        }
    });

    CheckForChangeSCA();
});
function CheckForChangeSCA() {
    $.ajax({
        type: 'get',
        url: urlGetSTCBasicDetails,
        dataType: 'json',
        data: { jobID: jobID },
        async: false,
        success: function (data) {
            if ((data.stcStatus == 'not yet submitted' || data.stcStatus == 'submit to trade') && isChangeSCA) {
                $("#divIsChangeSCA").html('<input type="button" class="primary" value="Change SCA" onclick="changeSCAModal()" style="margin-left: 5px"/>');
            }
            else {
                $("#divIsChangeSCA").html('');
            }
        },
        error: function (ex) {
            alert("Error occured...");
        }
    });
}
function BindDeemingPeriodDropdown(year, deemingPeriod) {
    $.ajax({
        type: 'get',
        url: urlGetDeemingPeriod,
        dataType: 'json',
        data: { year: year },
        async: false,
        success: function (data) {
            $("#JobSTCDetails_DeemingPeriod").html('');
            $.each(data, function (i, deemingPeriod) {
                $("#JobSTCDetails_DeemingPeriod").append('<option value="' + deemingPeriod.Value + '">' + deemingPeriod.Text + '</option>');
            });

            if (deemingPeriod != 0) {
                document.getElementById("JobSTCDetails_DeemingPeriod").value = deemingPeriod;
            }
            else {
                $("#JobSTCDetails_DeemingPeriod").val($("#JobSTCDetails_DeemingPeriod option:last").val());
            }
        },
        error: function (ex) {
            alert('Failed to retrieve DeemingPeriod.' + ex);
        }
    });
}

$('#STCJobStage_Tabular').change(function () {
    var jobstageid = $('#STCJobStage_Tabular option:selected').val();
    var jobstcdetailsid = $('#STCJobDetailsIdJobStage').val();

    
    var confirmMsg = '';
    var IsValid = true;
    var IsPartialValid = true;
    //var CheckSPVrequireddata = [];

    if (currentStatus != 'CER Approved' && currentStatus != 'New Submission') {
        if (jobstageid == 22 && settlementTerm == 5 && !isPartialValidForSTCInvoice) {
            IsPartialValid = false;
            //return false;
        }
        else if (jobstageid == 14 && isInvoiced == 'false') {
            confirmMsg = 'are you sure you want to change this status as per normal, and status change but no credit note is created because no invoice has been generated.'
        }
        var spvrequireddata = { JobId: jobID, STCJobDetailsID: jobstcdetailsid };
        //CheckSPVrequireddata.push(spvrequireddata);
    }
    else {
        IsValid = false;
        //return false;
    }

    if (IsPartialValid) {
        if (IsValid) {

            if (confirmMsg == '') {
                confirmMsg = jobstageid == 22 ? "Are you sure you want to change stc status as CERApproved as you can not reverse it after change ?" : "Are you sure you want to change stc status ?"
            }
            var result = confirm(confirmMsg);

            //var CheckSPVrequireddata = json.st({ checkSPVrequired: [{ JobId: selectedRowsJobids.join(','), stcjobids: selectedRows.join(',')}] })
            if (result) {
                $.ajax({
                    type: 'POST',
                    url: urlChangeSTCJobStage,
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ stcjobstage: jobstageid, jobID: jobID }), // checkSPVrequired: CheckSPVrequireddata, not passed SPVRequired Param
                    success: function (data) {
                        if (data.success) {
                            showSuccessMessage("Job Stage Changed Successfully");
                            currentStatus = $('#STCJobStage_Tabular option:selected').text();
                        }
                        else {
                            showErrorMessage(data.message);
                        }
                    },
                    error: function (ex) {
                        showErrorMessage('job has not been saved.' + ex);
                    }
                });
            }
        }
        else {
            alert('You can not change stc status of job which have CER Approved or New Submission STC status.');
            return false;
        }
    }
    else {
        alert('You can not change status to CERApproved which has STCStatus Partial and first invoice is not yet generated.');
    }
});

function BindSTCJobStagesTabular() {
    
    $("#STCJobStage_Tabular").empty();
    $.ajax({
        type: 'POST',
        url: urlGetSTCJobStages,
        dataType: 'json',
        async: false,
        //data: { usertypeid: UserTypeID },
        success: function (stages) {
            $("#STCJobStage_Tabular").append('<option value="0">Select STC job stage</option>');
            $.each(stages, function (i, stage) {
                $("#STCJobStage_Tabular").append('<option value="' + stage.Value + '">' +
                    stage.Text + '</option>');
            });
            $("#STCJobStage_Tabular").val($('#STCJobStageId').val());
        },
        error: function (ex) {
            alert('Failed to retrieve Job Stages.' + ex);
        }
    });
    return false;
}

function BindBasicDetailsValue() {
    
    BindSTCJobStagesTabular();
    dropDownData.push({ id: 'BasicDetails_JobStage', key: "JobStage", value: BasicDetails_JobStage, hasSelect: true, callback: null, defaultText: null, proc: 'Job_GetJobSatge', param: [], bText: 'StageName', bValue: 'JobStageId' });
    dropDownData.push({ id: 'BasicDetails_Priority', key: "", value: BasicDetails_Priority, hasSelect: true, callback: null, defaultText: null, proc: 'GetPriority', param: [], bText: 'Text', bValue: 'Value' });
    if (BasicDetails_ScoID != 0 && BasicDetails_ScoID > 0) {
        //FillDropDown('ScoID', urlGetSCOUser, BasicDetails_ScoID, true, null);
        dropDownData.push({ id: 'ScoID', key: "", value: BasicDetails_ScoID, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });
    }
    else {
        //FillDropDown('ScoID', urlGetSCOUser, null, true, null);
        dropDownData.push({ id: 'ScoID', key: "", value: null, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });
    }
    dropDownData.push({ id: 'SSCID', key: "", value: BasicDetails_SSCID, hasSelect: true, callback: disableSSCdropdown, defaultText: null, proc: 'User_GetSSCUserByJbID', param: [{ JobID: Model_JobID }], bText: 'Name', bValue: 'UserId' });

    dropDownData.bindDropdown();

    /* dropDownData.push({ id: 'BasicDetails_JobSoldBy', key: "SoldBy", value: 0, hasSelect: true, callback: null, defaultText: null, proc: 'GetSoldBy', param: [], bText: 'NAME', bValue: 'NAME' });*/
    function disableSSCdropdown() {
        if (BasicDetails_SSCID > 0 && ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3 && ProjectSession_UserTypeId != 4 && ProjectSession_UserTypeId != 5 && ProjectSession_UserTypeId != 2)
            $('#SSCID').prop("disabled", true);
        else
            $('#SSCID').prop("disabled", false);
    }
    dropDownData.bindDropdown();

    DisplayInstallationDate();
    /*DisplaySoldByDate();*/

    //var jsonvarSoldByDate = $('#BasicDetails_strSoldByDate').val();
    //var jsonvarJobSoldBy = $('#BasicDetails_SoldBy').val();
    //if (jsonvarSoldByDate != '') {
    //    $('#BasicDetails_strSoldByDate').val(jsonvarSoldByDate);
    //}
    //$('#BasicDetails_JobSoldByText').val(jsonvarJobSoldBy);
}

function DisplayInstallationDate() {
    var installationDate = $("#BasicDetails_strInstallationDate").val();
    if (installationDate != null && installationDate != undefined && installationDate != '') {
        $("#BasicDetails_strInstallationDate").val('').removeAttr('value');

        var installationDateEdit = moment(installationDate).format(dateFormat.toUpperCase());
        $("#BasicDetails_strInstallationDate").val(installationDateEdit);

        // display default date
        $("#BasicDetails_strInstallationDate").attr('data-date-format', 'dd/mm/yyyy');

        $('.date-pick, .date-pick1, .date-pick').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });

    } else {
        $('.date-pick, .date-pick1, .date-pick').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    }
}

function DisplaySoldByDate() {
    var SoldByDate = $('#BasicDetails_strSoldByDate').val();
    if (SoldByDate != null && SoldByDate != undefined && SoldByDate != '') {
        $('#BasicDetails_strSoldByDate').val('').removeAttr('value');
        var SoldByDateEdit = moment(SoldByDate).format(dateFormat.toUpperCase());
        $('#BasicDetails_strSoldByDate').val(SoldByDateEdit);
        // display default date
        $("#BasicDetails_strSoldByDate").attr('data-date-format', 'dd/mm/yyyy');

        $('#BasicDetails_strSoldByDate').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    } else {
        $('#BasicDetails_strSoldByDate').datepicker({
            format: dateFormat,
            autoclose: true
        }).on('changeDate', function () {
            $(this).datepicker('hide');
        });
    }
    jsonvarSoldByDate = $('#BasicDetails_strSoldByDate').val();
    jsonvarJobSoldBy = $('#BasicDetails_SoldBy').val();
}

function BindDocumentsTab() {
    
    var HDN_JobID = $("#hdnjobId_tblr").val();
    $.ajax({
        url: '/Job/GetDocumentsTab',
        //datatype: "json",
        data: { JobId: HDN_JobID },
        type: "post",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            
            isDocumentsPhotos = true;
            $(".tab8").html(data);
        },
        error: function (xhr) {
            alert('error');
        }
    });

}

function BindPartialView(Flag, DATAOPMODE) {
    
    var isDataAlreadyLoaded = false;
    var HDN_JobID = $("#hdnjobId_tblr").val();
    var HDN_ID = $("#hdnId_tblr").val();

    //if (Flag == 'Owner Details' && isOwnerDetailsFetch == true) {
    //    isDataAlreadyLoaded = true;
    //}
    if (Flag == 'Installation Details' && isInstallationDetails == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'Custom Details' && isCustomDetails == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'STC Details' && isSTCDetails == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'STC Status' && isSTCStatus == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'System Details' && isSystemDetails == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'Installer Details' && isInstallerDetails == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'Scheduling' && isScheduling == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'DocumentsPhotos' && isDocumentsPhotos == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'NotesHistory' && isNotesHistory == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'WrittenStatementsDeclaration' && isWrittenStatementsDeclaration == true) {
        isDataAlreadyLoaded = true;
    }
    if (Flag == 'RetailerDetails' && isRetailerDetails == true) {
        isDataAlreadyLoaded = true;
    }
    //if (Flag == 'PreApproval' && isPreApproval == true) {
    //    isDataAlreadyLoaded = true;
    //}
    //if (Flag == 'Connection' && isConnections == true) {
    //    isDataAlreadyLoaded = true;
    //}
    if (!isDataAlreadyLoaded) {
        
        $.ajax({
            url: '/Job/GetResultByAjax',
            //datatype: "json",
            data: { Flag: Flag, DATAOPMODE: DATAOPMODE, JobId: HDN_JobID, ID: HDN_ID },
            type: "post",
            contenttype: 'application/json; charset=utf-8',
            async: true,
            success: function (data) {
                
                if (Flag == 'Owner Details') {
                    $(".tab1").html(data);
                    isOwnerDetailsFetch = true;
                }
                else if (Flag == 'Installation Details') {
                    $(".tab5").html(data);
                    isInstallationDetails = true;
                }
                else if (Flag == 'Custom Details') {
                    $(".tab15").html(data);
                    isCustomDetails = true;
                }
                else if (Flag == 'STC Details') {
                    $(".tab4").html(data);
                    isSTCDetails = true;
                    if (SessionPanelXml.length > 0) {
                        PanelXml = SessionPanelXml;
                    }
                    if (SessionInvertorXml.length > 0) {
                        InverterXml = SessionInvertorXml;
                    }
                    if (SessionBatteryXml.length > 0) {
                        batteryXml = SessionBatteryXml;
                    }
                }
                else if (Flag == 'STC Status') {
                    $(".tab12").html(data);
                    isSTCStatus = true;
                }
                else if (Flag == 'System Details') {
                    $(".tab2").html(data);
                    SessionPanelXml = PanelXml;
                    SessionInvertorXml = InverterXml;
                    SessionBatteryXml = batteryXml;
                    isSystemDetails = true;
                    if (!isInstallationDetails) {
                        BindPartialView('Installation Details', 3);
                        isInstallationDetails = true;
                    }
                    if (!isSTCDetails) {
                        BindPartialView('STC Details', 4);
                        isSTCDetails = true;
                    }
                }
                else if (Flag == 'Installer Details') {
                    $(".tab3").html(data);
                    isInstallerDetails = true;
                }
                else if (Flag == 'Scheduling') {
                    $(".tab7").html(data);
                    isScheduling = true;
                }
                else if (Flag == 'DocumentsPhotos') {
                    $(".tab8").html(data);
                    isDocumentsPhotos = true;
                    isSerialnumberDetails = true;
                }
                else if (Flag == 'NotesHistory') {
                    $(".tab9").html(data);
                    isNotesHistory = true;
                }
                else if (Flag == 'WrittenStatementsDeclaration') {
                    $(".tab10").html(data);
                    isWrittenStatementsDeclaration = true;
                }
                else if (Flag == 'RetailerDetails') {
                    /*$(".btnsaveandcancel").hide();*/
                    $(".tab11").html(data);
                    isRetailerDetails = true;
                }
                else if (Flag == 'PreApproval') {
                    $(".tab14").html(data);
                    isPreApproval = true;
                }
                else if (Flag == 'Connection') {
                    $(".tab13").html(data);
                    isConnections = true;
                }
            },
            error: function (xhr) {
                alert('error');
            }
        });
    }

}

/* On tab click*/
//$('#pills-tab a').on('click', function (e) {
//    e.preventDefault();
//    $(this).tab('show'); if (this.text == 'Installer Details') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Scheduling') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }

//    else if (this.text == 'Documents & Photos') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Notes & History') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Written Statements Declaration') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Retailer Details') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Trade Job') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else if (this.text == 'Installer') {
//        $("#btnSaveTab").hide();
//        $("#btnCancelSave").hide();
//    }
//    else {
//        $("#btnSaveTab").show();
//        $("#btnCancelSave").show();
//    }
//});

$('a[data-toggle="pill"]').on('shown.bs.tab', function (e) {
    e.target // newly activated tab
    e.relatedTarget // previous active tab
})

$('#btnViewHistory').click(function (e) {
    
    e.preventDefault();
    $('#JobHistoryOfJob').load(urlShowHistory + urlRequestContext, function () {
        $('#JobHistory').modal({ backdrop: 'static', keyboard: false });
    });
});

$('#nxtTab').click(function (e) {
    e.preventDefault();
    $('li.resp-tab-active').next().click();
    $('#successMsgRegionSTCDetails').hide();
    $('#errorMsgRegionSTCDetails').hide();
    $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
});

$('#prevTab').click(function (e) {
    e.preventDefault();
    $('li.resp-tab-active').prev().click();
    $('#successMsgRegionSTCDetails').hide();
    $('#errorMsgRegionSTCDetails').hide();
    $('#spnTabIndex').text($('li.resp-tab-active').index() + 1);
});

$('#btnCancelSave').click(function (e) {
    
    e.preventDefault();
    var tabName = $('#horizontalTab').find('li.active').attr('id');
    var jobId = $("#BasicDetails_JobID").val();

    if (tabName == 'basic') {

        location.reload(true);
        //BindPartialView(tabName,1);
    }
    else if (tabName == "owner") {
        isOwnerDetailsFetch = false;
        $(".tab1").empty();
        BindPartialView("Owner Details", 2);
    }
    else if (tabName == "system") {
        isSystemDetails = false;
        $(".tab2").empty();
        BindPartialView("System Details", 5);
    }
    else if (tabName == "installer") {
        isInstallerDetails = false;
        $(".tab3").empty();
        BindPartialView("Installer Details", 6);

    } else if (tabName == "stc") {
        isSTCDetails = false;
        $(".tab4").empty();
        BindPartialView("STC Details", 4);
    }
    else if (tabName == "stcstatus") {
        isSTCStatus = false;
        $(".tab12").empty();
        BindPartialView("STC Status", 4);
    }
    else if (tabName == "installation") {
        isInstallationDetails = false;
        $(".tab5").empty();
        BindPartialView("Installation Details", 3);
    }
    else if (tabName == "customdetails") {
        isCustomDetails = false;
        $(".tab15").empty();
        BindPartialView("Custom Details", 3);
    }
    else if (tabName == "documents") {
        /*isCustomDetails = false;*/
        isSerialnumberDetails = false;
        isDocumentsPhotos = false;
        $(".tab8").empty();
        BindPartialView("DocumentsPhotos", 8);

    }
    else if (tabName == "notes-history") {
        /*isCustomDetails = false;*/
        $(".tab9").empty();
        BindPartialView("NotesHistory", 9);

    }

});

$('#btnSaveTab').click(function (e) {
    
    /*TabularView.UpdateBasicDetail();*/
    if (!isValidTabularData) {
        isValidTabularData = validTabularPage();
    }
    if (isValidTabularData) {
        if (isOverRideSave == false) {
            e.preventDefault();
            checkBusinessRules(false);
        }
        else {         
            TabularView.UpdateBasicDetail();
            if (isInstallationDetails == true) {
                TabularView.UpdateInstallationDetail();
            }
            if (isCustomDetails == true) {
                TabularView.UpdateCustomDetail();
            }
            if (isSystemDetails == true) {
                TabularView.UpdateSystemDetail();
            }
            if (isSTCDetails == true) {
                TabularView.UpdateStcDetail();
                TabularView.UpdateGstDetail();
            }
            if (isSerialnumberDetails == true || isDocumentsPhotos == true) {
                TabularView.UpdateSerialNumberDetail();
            }            
            if (isTabularDataUpdated) {
                isOverRideSave = false;                
            }
        }
    }   
});

function SavePreapprovalConnectionComment(isStatusChange) {

    
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

function validTabularPage() {
    if ($("#frmBasicDetail").valid()) {
        $("#JobOwnerDetails_JobID").val(BasicDetails_JobID);
        //var isValid = addressValidationRules("JobOwnerDetails");
        var propertyType = JobInstallationDetails_PropertyType;
        var emailOwner = $('#JobOwnerDetails_Email').val();
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            isValidTabularData = false;
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installation property type school.");
            isValidTabularData = false;
            return false;
        }
        if (isSTCDetails) {
            var propertyType = $("#JobInstallationDetails_PropertyType").val();
            var emailOwner = $('#JobOwnerDetails_Email').val();
            var stcLatitude = $("#JobSTCDetails_Latitude").val();
            var stcLongitude = $("#JobSTCDetails_Longitude").val();
            var decimal = /^[-+]?[0-9]+\.[0-9]+$/;
            if (stcLatitude != "" && !stcLatitude.match(decimal)) {
                showErrorMessage("Please enter latitude in decimal format of STC job details.");
                isValidTabularData = false;
                return false;
            }
            if (stcLongitude != "" && !stcLongitude.match(decimal)) {
                showErrorMessage("Please enter longitude in decimal format of STC job details.");
                isValidTabularData = false;
                return false;
            }
            if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
                showErrorMessage("Owner email address is mandatory for commercial jobs.");
                isValidTabularData = false;
                return false;
            }
            if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
                showErrorMessage("Owner email address is mandatory for installtion property type school.");
                isValidTabularData = false;
                return false;
            }
        }

        if (isInstallationDetails) {
            var InstallationAddress = $("#txtAddress1").val();
            if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
                $('#spantxtAddress').show();
                isValidTabularData = false;
                return false;
            } else { $('#spantxtAddress').hide(); }

            var isValid = addressValidationRules("JobInstallationDetails");
            if (!isValid) {
                return false;
            }
        }

        if (isCustomDetails) {
            if (!$("#frmCustomDetails").valid()) {
                return false;
            }
        }

        if (isSystemDetails) {
            // Check to set STC value start
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
            if (!isValidDataForSTC) {
                return false;
            }
        }
        if (isSerialnumberDetails || isDocumentsPhotos) {
            var serialVal = false;
            var inverterSerialVal = false;
            DuplicateSerialandInverterErrorMsg = '';
            if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
                serialVal = true;
            }
            else {
                
                /*serialVal = SerialNumbersValidation();*/
                serialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_SerialNumbers"), $('#spanSerialNumbers'), true);
                inverterSerialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_InverterSerialNumbers"), $('#spanInverterSerialNumbers'), false);
                if (!serialVal || !inverterSerialVal) {                    
                    return false;
                }
            }
        }
        return true;
    }
}

var TabularView = {
    UpdateBasicDetail: function (obj) {
        
        //ty
        //var soldBy = $('#BasicDetails_JobSoldByText').val();
        //$('#BasicDetails_SoldBy').val(soldBy);
        if ($("#frmBasicDetail").valid() && isValidTabularData) {
            
            $('#SSCID').prop("disabled", false);
            var url = urlUpdateBasicDetail,

                data = $('#frmBasicDetail').serializeToJson();
            data.isOverRideSave = isOverRideSave;
            //$('#SSCID').prop("disabled", true);
            
            getDataTabular(url, data, function (result) {
                if (result.status) {
                    
                    //if (isOverRideSave == false) {
                    //    TabularView.UpdateOwnerDetail();                        
                    //var liLength = $(result.Data.ValidationSummary).length;
                    //if (liLength > 0) {
                    //    BusinessValidationPop(result);
                    //}
                    //}
                    //else {
                    isTabularDataUpdated = true;
                    TabularView.UpdateOwnerDetail();
                    /*isOverRideSave = false;*/

                    /*ReloadSTCModule();*/
                    /*showSuccessMessage("Job details has been saved successfully.");*/
                    if ($('#SSCID').val() != "")
                        $('#SSCID').prop("disabled", false);
                    else
                        $('#SSCID').prop("disabled", false);
                    /*   }*/
                    /*isTabularDataUpdated = true;*/
                }
                else {
                    showErrorMessage("Basic details has not been saved.");
                    if ($('#SSCID').val() != "")
                        $('#SSCID').prop("disabled", true);
                    else
                        $('#SSCID').prop("disabled", false);
                    isTabularDataUpdated = false;
                }
            });
        }

    },
    UpdateOwnerDetail: function () {
        
        $("#JobOwnerDetails_JobID").val(BasicDetails_JobID);
        //var isValid = addressValidationRules("JobOwnerDetails");
        var propertyType = JobInstallationDetails_PropertyType;
        var emailOwner = $('#JobOwnerDetails_Email').val();
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            isValidTabularData = false;
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installation property type school.");
            isValidTabularData = false;
            return false;
        }

        if ($("#frmBasicDetail").valid() && isValidTabularData) {
            var url = urlUpdateOwnerDetail,
                isGST = false;
            if ($("#BasicDetails_IsGst").val() != undefined && $("#BasicDetails_IsGst").val() != '') {
                isGST = $('#BasicDetails_IsGst').prop('checked');
            }
            else {
                isGST = modelbasicdetailsIsGST;
            }
            data = $('#frmBasicDetail').serializeToJson().JobOwnerDetails;
            data.isGST = isGST;
            /*isOverRideSave = true;*/
            data.isOverRideSave = isOverRideSave;
            getDataTabular(url, data, function (result) {
                if (result.status) {
                    
                    //if (isOverRideSave == false) {
                    //    var liLength = $(result.Data.ValidationSummary).length;
                    //    if (liLength > 0) {
                    //        BusinessValidationPop(result);
                    //    }
                    //}
                    //else {                        
                    //ShowHideGSTSection(JobInstallationDetails_PropertyType, $("#JobOwnerDetails_OwnerType").val().toLowerCase());
                    OwnerAddressJson = [];
                    if ($("#JobOwnerDetails_AddressID").val() == 1) {
                        OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), UnitType: $("#JobOwnerDetails_UnitTypeID").val(), UnitNumber: $("#JobOwnerDetails_UnitNumber").val(), StreetNumber: $("#JobOwnerDetails_StreetNumber").val(), StreetName: $("#JobOwnerDetails_StreetName").val(), StreetType: $("#JobOwnerDetails_StreetTypeID").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
                    }
                    else {
                        OwnerAddressJson.push({ CompanyABN: $("#JobOwnerDetails_CompanyABN").val(), OwnerType: $("#JobOwnerDetails_OwnerType").val(), CompanyName: $("#JobOwnerDetails_CompanyName").val(), Phone: $('#JobOwnerDetails_Phone').val(), Mobile: $('#JobOwnerDetails_Mobile').val(), FirstName: $('#JobOwnerDetails_FirstName').val(), LastName: $('#JobOwnerDetails_LastName').val(), Email: $('#JobOwnerDetails_Email').val(), PostalAddressType: $("#JobOwnerDetails_AddressID").val(), PostalAddressID: $("#JobOwnerDetails_PostalAddressID").val(), PostalDeliveryNumber: $("#JobOwnerDetails_PostalDeliveryNumber").val(), Town: $("#JobOwnerDetails_Town").val(), State: $("#JobOwnerDetails_State").val(), PostCode: $("#JobOwnerDetails_PostCode").val() });
                    }
                    /*ReloadSTCModule();*/
                    isTabularDataUpdated = true;
                    if (isTabularDataUpdated && !isInstallationDetails && !isCustomDetails && !isSystemDetails && !isSTCDetails && !isDocumentsPhotos) {
                        showSuccessMessage("Job details has been saved successfully");
                    }
                    else if (isTabularDataUpdated && (isInstallationDetails || isCustomDetails || isSystemDetails || isSTCDetails || isDocumentsPhotos)) {
                        showSuccessMessage("Job details has been saved successfully");
                    }
                    
                    else {
                        showErrorMessage("Job details has not been saved.");
                    }
                    
                    /*}*/
                    /*isTabularDataUpdated = true;*/
                    return true;
                }
                else {
                    showErrorMessage("Owner details has not been saved.");
                    return false;
                }
            });
        }
    },
    UpdateStcDetail: function () {

        var propertyType = $("#JobInstallationDetails_PropertyType").val();
        var emailOwner = $('#JobOwnerDetails_Email').val();
        var stcLatitude = $("#JobSTCDetails_Latitude").val();
        var stcLongitude = $("#JobSTCDetails_Longitude").val();
        var decimal = /^[-+]?[0-9]+\.[0-9]+$/;
        if (stcLatitude != "" && !stcLatitude.match(decimal)) {
            showErrorMessage("Please enter latitude in decimal format of STC job details.");
            isValidTabularData = false;
            return false;
        }
        if (stcLongitude != "" && !stcLongitude.match(decimal)) {
            showErrorMessage("Please enter longitude in decimal format of STC job details.");
            isValidTabularData = false;
            return false;
        }
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            isValidTabularData = false;
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installtion property type school.");
            isValidTabularData = false;
            return false;
        }
        
        if (isValidTabularData) {
            var url = urlUpdateStcDetail,
                data = $('#frmStcDetail').serializeToJson();
            data.JobType = $("#BasicDetails_JobType").val();
            data.isOverRideSave = isOverRideSave;
            getDataTabular(url, data, function (result) {
                if (result.status) {
                    //if (isOverRideSave == false) {
                    //    var liLength = $(result.Data.ValidationSummary).length;
                    //    if (liLength > 0) {
                    //        BusinessValidationPop(result);
                    //    }
                    //}
                    //else {
                    /*isOverRideSave = false;*/
                    ReloadSTCModule();
                    /*showSuccessMessage("Job details has been saved successfully.");*/
                    /*  }*/
                    isTabularDataUpdated = true;
                    return true;
                }
                else {
                    showErrorMessage("Stc details has not been saved.");
                    return false;
                }
            });
        }
    },
    UpdateInstallationDetail: function () {
        //if ($('#JobInstallationDetails_IsSameAsOwnerAddress').is(":checked")) {
        //    
        //}
        //else {
        //    var InstallationAddress = $('#txtAddress').val();
        //}
        var InstallationAddress = $("#txtAddress1").val();
        if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
            $('#spantxtAddress').show();
            isValidTabularData = false;
            return false;
        } else { $('#spantxtAddress').hide(); }

        var isValid = addressValidationRules("JobInstallationDetails");
        isValidTabularData = isValid;
        $("#JobInstallationDetails_JobID").val(BasicDetails_JobID);
        if ($("#frmInstallationDetail").valid() && isValid && isValidTabularData) {
            var data = JSON.stringify($('#frmInstallationDetail').serializeToJson());
            var objData = JSON.parse(data);

            var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
            var objInstallationAddressData = JSON.parse(installationAddressData);

            var installationData = JSON.stringify($('#JobInstallationDetailInfo').serializeToJson());
            var objInstallationData = JSON.parse(installationData);

            var extraInstallationData = JSON.stringify($('#JobExtraInstallationInfo').serializeToJson());
            var objExtraInstallationData = JSON.parse(extraInstallationData);

            $.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);
            $.extend(objData.JobInstallationDetails, objInstallationData.JobInstallationDetails);
            $.extend(objData.JobInstallationDetails, objExtraInstallationData.JobInstallationDetails);

            //var lstCustomDetails = [];
            //$("#customDetails").find('.spanCustomFields').each(function () {
            //    lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).val(), SeparatorId: $(this).attr('data-SeperatorId') });
            //});
            //objData.JobInstallationDetails.lstCustomDetails = lstCustomDetails;

            //var customLength = $("#customDetails").find('.spanCustomFields').length;
            //for (var i = 0; i < customLength; i++) {
            //    delete objData["lstCustomDetails[" + i + "]"];
            //}

            var jobData = objData;
            jobData.isOverRideSave = isOverRideSave;

            var url = urlUpdateInstallationDetail;
            getDataTabular(url, jobData, function (result) {
                //ShowHideGSTSection(JobInstallationDetails_PropertyType, JobOwnerDetails_OwnerType_Glbl.toLowerCase());
                //getDataTabular(url, jobData, function (result) {
                if (result.status) {
                    //if (isOverRideSave == false) {
                    //    var liLength = $(result.Data.ValidationSummary).length;
                    //    if (liLength > 0) {
                    //        BusinessValidationPop(result);
                    //    }
                    //}
                    //else {
                    /*isOverRideSave = false;*/
                    /*ReloadSTCModule();*/
                    /*showSuccessMessage("Job details has been saved successfully.");*/
                    /*}*/
                    isTabularDataUpdated = true;
                    return true;
                }
                else {
                    showErrorMessage("Installation details has not been saved.");
                    return false;
                }
            });
        }
    },
    UpdateCustomDetail: function () {
        
        //var InstallationAddress = $('#txtAddress').val();
        //if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
        //    $('#spantxtAddress').show();
        //    return false;
        //} else { $('#spantxtAddress').hide(); }

        //var isValid = addressValidationRules("JobInstallationDetails");

        $("#JobInstallationDetails_JobID").val(BasicDetails_JobID);
        if ($("#frmCustomDetails").valid() && isValidTabularData) {
            var data = JSON.stringify($('#frmCustomDetails').serializeToJson());
            var objData = JSON.parse(data);

            //var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
            //var objInstallationAddressData = JSON.parse(installationAddressData);

            //var installationData = JSON.stringify($('#JobInstallationDetailInfo').serializeToJson());
            //var objInstallationData = JSON.parse(installationData);

            //var extraInstallationData = JSON.stringify($('#JobExtraInstallationInfo').serializeToJson());
            //var objExtraInstallationData = JSON.parse(extraInstallationData);

            //$.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);
            //$.extend(objData.JobInstallationDetails, objInstallationData.JobInstallationDetails);
            //$.extend(objData.JobInstallationDetails, objExtraInstallationData.JobInstallationDetails);

            var lstCustomDetails = [];
            $("#customDetails").find('.spanCustomFields').each(function () {
                lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).val(), SeparatorId: $(this).attr('data-SeperatorId') });
            });
            //$.extend(objData.JobInstallationDetails.lstCustomDetails, lstCustomDetails);
            objData.lstCustomDetails = lstCustomDetails;

            var customLength = $("#customDetails").find('.spanCustomFields').length;
            for (var i = 0; i < customLength; i++) {
                delete objData["lstCustomDetails[" + i + "]"];
            }

            var jobData = objData;
            jobData.isOverRideSave = isOverRideSave;
            jobData.JobId = BasicDetails_JobID;

            var url = urlUpdateCustomDetail;
            getDataTabular(url, jobData, function (result) {
                //ShowHideGSTSection(JobInstallationDetails_PropertyType, JobOwnerDetails_OwnerType_Glbl.toLowerCase());
                //getDataTabular(url, jobData, function (result) {
                if (result.status) {
                    //if (isOverRideSave == false) {
                    //    var liLength = $(result.Data.ValidationSummary).length;
                    //    if (liLength > 0) {
                    //        BusinessValidationPop(result);
                    //    }
                    //}
                    //else {
                    //    isOverRideSave = false;
                    /*ReloadSTCModule();*/
                    /*showSuccessMessage("Custom details has been saved successfully.");*/
                    isTabularDataUpdated = true;
                    return true;
                    /*}*/
                }
                else {
                    showErrorMessage("Custom detail has not been saved.");
                    return false;
                }
            });
        }
    },
    UpdateSystemDetail: function () {
        
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
                    '<Series>' + htmlEncode(this['Series']) + '</Series><NoOfInverter>' + htmlEncode(this['NoOfInverter']) + '</NoOfInverter><Supplier>' + htmlEncode(this['Supplier']) + '</Supplier></inverter>';
            });
            xmlInverter = '<Inverters>' + sName + '</Inverters>';
        }

        $("#panelXml").val(xmlPanel);
        $("#inverterXml").val(xmlInverter);
        $("#JobSystemDetails_jobTypeTab").val(jobType);
        $("#JobSystemDetails_JobID").val(BasicDetails_JobID);
        $("#OldPanelDetails").val(JSON.stringify(OldPanelXml));
        $("#NewPanelDetails").val(JSON.stringify(PanelXml));


        var data = JSON.stringify($('#frmSystemDetail').serializeToJson());

        var objData = {};
        var jobSystemDetails = JSON.parse(data).JobSystemDetails;
        //jobSystemDetails.CalculatedSTC = $("#JobSystemDetails_CalculatedSTC").val();
        //jobSystemDetails.CalculatedSTCForSWH = $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.ModifiedCalculatedSTC = $("#BasicDetails_JobType").val() == 1 ? $("#JobSystemDetails_CalculatedSTC").val() : $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.NoOfPanel = $("#JobSystemDetails_NoOfPanel").html();
        jobSystemDetails.InstallationType = $("#JobSystemDetails_InstallationType").val();
        if (systemXml.length > 0) {
            jobSystemDetails.SystemBrand = systemXml[0].Brand;
            jobSystemDetails.SystemModel = systemXml[0].Model;
        }
        jobSystemDetails.panelXmlTabular = xmlPanel;
        jobSystemDetails.inverterXmlTabular = xmlInverter;
        jobSystemDetails.SerialNumbers = $("#JobSystemDetails_SerialNumbers").val();
        jobSystemDetails.InverterSerialNumbers = $("#JobSystemDetails_InverterSerialNumbers").val();

        if (batteryManufacturerData.length > 0) {

            for (var i = 0; i < batteryManufacturerData.length; i++) {
                delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
            }
            jobSystemDetails.lstJobBatteryManufacturer = batteryManufacturerData;
            if ($("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val().toLowerCase() == 'select') {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = ''
            } else {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
            }
            if ($("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val().toLowerCase() == 'select') {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = '';
            } else {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();
            }
        }

        // Check to set STC value start
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
        isValidTabularData = isValidDataForSTC;
        // Check to set STC value end
        objData.objSystem = jobSystemDetails;
        //objData.OldPanelDetails = JSON.parse(data).OldPanelDetails
        //objData.NewPanelDetails = JSON.parse(data).NewPanelDetails;
        objData.OldPanelDetails = $("#OldPanelDetails").val();
        objData.NewPanelDetails = $("#NewPanelDetails").val();        
        objData.isOverRideSave = isOverRideSave;

        if (isValidTabularData) {
            if (isValidDataForSTC) {
                var url = urlUpdateSystemDetail;
                getDataTabular(url, objData, function (result) {
                    if (result.status) {
                        //if (isOverRideSave == false) {
                        //    var liLength = $(result.Data.ValidationSummary).length;
                        //    if (liLength > 0) {
                        //        BusinessValidationPop(result);
                        //    }
                        //}
                        //else {
                        //    /*isOverRideSave = false;*/
                        OldPanelXml = [];
                        OldPanelXml = JSON.parse(JSON.stringify(PanelXml));
                        if (result.jobType == 1) {
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
                        ReloadSTCModule();
                        serialNumber = result.serialnumbers;
                        $("div.line-no").removeClass("verified")
                        $("div.line-no").removeClass("unverified")
                        $("div.line-no").removeClass("installationVerified")
                        $("div.line-no").removeClass("notverified")
                        $("#GlobalisAllowedSPV").val(result.GlobalisAllowedSPV)
                        if (result.IsSPVRequired) {
                            IsSPVRequired = "True"
                            VerifyUnVerifySerialNumber();
                            var unverifiedSerialNumber = serialNumber.find(serialNumber => serialNumber.IsVerified == null);
                            if (unverifiedSerialNumber != undefined && $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                                $(".isSPVRequired").show();
                            else
                                $(".isSPVRequired").hide();

                        } else {
                            $(".isSPVRequired").hide();
                            $("#SPVLabel").hide();
                            IsSPVRequired = "False"
                        }
                        isTabularDataUpdated = true;
                        //LoadCommonSerialNumber();
                        //LoadCommonInverterSerialNumber();
                        //checkSerialNumberWithPhotoExistOrNot();
                        /*showSuccessMessage(result.isRECUp ? "Job details has been saved successfully." : "Job details has been saved successfully but STC Value cannot be calculated since REC website is down.");*/
                        return true;
                        /* }*/
                    }
                    else {
                        showErrorMessage("System details has not been saved.");
                        return false;
                    }
                });
            }
            else {
                if ($("#BasicDetails_JobType").val() == 1) {
                    showErrorMessage("Please fill Installation Date, STC DeemingPeriod, Installation postcode to set STC value.");
                    return false;
                }
                else {
                    showErrorMessage("Please fill Installation Date, System brand, System Model, Installation postcode to set STC value.");
                }
            }
        }
    },
    UpdateSerialNumberDetail: function () {
        
        var objData = {};
        var isSerialNoloded = true;
        $("#JobSystemDetails_JobID").val(BasicDetails_JobID);
        if ($("#JobSystemDetails_JobID").val() == undefined) {
            isSerialNoloded = false;
        }
        if (isSerialNoloded) {
            var jobSystemDetails = {
                SerialNumbers: $("#JobSystemDetails_SerialNumbers").val(),
                InverterSerialNumbers: $("#JobSystemDetails_InverterSerialNumbers").val(),
                JobID: BasicDetails_JobID
            }
            objData.objSystem = jobSystemDetails;

            var serialVal = false;
            var inverterSerialVal = false;
            if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
                serialVal = true;
            }
            else {
                
                /*serialVal = SerialNumbersValidation();*/
                serialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_SerialNumbers"), $('#spanSerialNumbers'), true);
                inverterSerialVal = DuplicateSerialNumbersValidation($("#JobSystemDetails_InverterSerialNumbers"), $('#spanInverterSerialNumbers'), false);
                if (!serialVal || !inverterSerialVal) {
                    isValidTabularData = false;
                }
            }
            objData.isOverRideSave = isOverRideSave;

            if (serialVal && isValidTabularData) {
                var url = urlUpdateSerialNoDetail;
                getDataTabular(url, objData, function (result) {
                    if (result.status) {
                        
                        //if (isOverRideSave == false) {
                        //    var liLength = $(result.Data.ValidationSummary).length;
                        //    if (liLength > 0) {
                        //        BusinessValidationPop(result);
                        //    }
                        //}
                        //else {
                        //    /*isOverRideSave = false;*/

                        serialNumber = result.serialnumbers;
                        $("div.line-no").removeClass("verified")
                        $("div.line-no").removeClass("unverified")
                        $("div.line-no").removeClass("installationVerified")
                        $("div.line-no").removeClass("notverified")
                        $("#GlobalisAllowedSPV").val(result.GlobalisAllowedSPV)
                        if (result.IsSPVRequired) {
                            IsSPVRequired = "True"
                            VerifyUnVerifySerialNumber();
                            var unverifiedSerialNumber = serialNumber.find(serialNumber => serialNumber.IsVerified == null);
                            if (unverifiedSerialNumber != undefined && $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                                $(".isSPVRequired").show();
                            else
                                $(".isSPVRequired").hide();

                        } else {
                            $(".isSPVRequired").hide();
                            $("#SPVLabel").hide();
                            IsSPVRequired = "False"
                        }
                        LoadCommonSerialNumber();
                        LoadCommonInverterSerialNumber();
                        checkSerialNumberWithPhotoExistOrNot();
                        /*showSuccessMessage("Job details has been saved successfully.")*/
                        // showSuccessMessage(result.isRECUp ? "Job details has been saved successfully." : "Job details has been saved successfully but STC Value cannot be calculated since REC website is down.");
                        /* }*/
                        isTabularDataUpdated = true;
                        return true;
                    }
                    else {
                        showErrorMessage("Serial number details has not been saved.");
                        return false;
                    }
                });
            }
            else {
                showErrorMessage("Serial number should not be same.");
                isValidTabularData = false;
                return false;
            }
        }
        else {
            /*isValidTabularData = true;*/
            return true;
        }
    },

    UpdateGstDetail: function () {
        
        //if ((modelIsGSTSetByAdminUser == 2 && modelIsRegisteredWithGST == 'true') || (($('#JobOwnerDetails_OwnerType').val().toLowerCase() == 'corporatModel BasicDetailsbody' || $('#JobOwnerDetails_OwnerType').val().toLowerCase() == 'trustee') || ($('#JobInstallationDetails_PropertyType').val().toLowerCase() == "commercial" || $('#JobInstallationDetails_PropertyType').val().toLowerCase() == "school")))
        //    $("#BasicDetails_IsGst").prop('checked', true);
        //else
        //    $("#BasicDetails_IsGst").prop('checked', false);
        if (isValidTabularData) {
            var obj = {
                isGST: $('#BasicDetails_IsGst').prop('checked'),
                FileName: $('#BasicDetails_GSTDocument').val(),
                jobId: BasicDetails_JobID
            }

            var url = urlUpdateGstDetail;
            getDataTabular(url, obj, function (result) {
                if (result.status) {
                    //showSuccessMessage("GST detail has been saved successfully.");
                    isTabularDataUpdated = true;
                    return true;
                }
                else {
                    showErrorMessage("GST detail has not been saved.");
                    return false;
                }
            });
        }
    }
};

//tyu
function CommonTabularDataForSave() {
    
    var data = JSON.stringify($('form').serializeToJson());
    var objData = JSON.parse(data);
    objData.isOverRideSave = isOverRideSave;
    $("#JobOwnerDetails_JobID").val(BasicDetails_JobID);
    //var isValid = addressValidationRules("JobOwnerDetails");
    var propertyType = JobInstallationDetails_PropertyType;
    var emailOwner = $('#JobOwnerDetails_Email').val();
    if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for commercial jobs.");
        return false;
    }
    if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
        showErrorMessage("Owner email address is mandatory for installation property type school.");
        return false;
    }

    isGST = false;
    if ($("#BasicDetails_IsGst").val() != undefined && $("#BasicDetails_IsGst").val() != '') {
        isGST = $("#BasicDetails_IsGst").val();
    }

    var ownerData = JSON.stringify($('#frmBasicDetail').serializeToJson().JobOwnerDetails);
    objData.JobOwnerDetails = JSON.parse(ownerData);
    objData.JobOwnerDetails.isGST = isGST;


    if (isSTCDetails == true) {
        var propertyType = $("#JobInstallationDetails_PropertyType").val();
        var emailOwner = $('#JobOwnerDetails_Email').val();
        var stcLatitude = $("#JobSTCDetails_Latitude").val();
        var stcLongitude = $("#JobSTCDetails_Longitude").val();
        var decimal = /^[-+]?[0-9]+\.[0-9]+$/;
        if (stcLatitude != "" && !stcLatitude.match(decimal)) {
            showErrorMessage("Please enter latitude in decimal format of STC job details.");
            return false;
        }
        if (stcLongitude != "" && !stcLongitude.match(decimal)) {
            showErrorMessage("Please enter longitude in decimal format of STC job details.");
            return false;
        }
        if (propertyType.toLowerCase() == "commercial" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for commercial jobs.");
            return false;
        }
        if (propertyType.toLowerCase() == "school" && (emailOwner == null || emailOwner == "")) {
            showErrorMessage("Owner email address is mandatory for installtion property type school.");
            return false;
        }

        var stcData = JSON.stringify($('#frmStcDetail').serializeToJson());
        objData.JobSTCDetails = JSON.parse(stcData).JobSTCDetails;
        objData.JobSTCDetails.JobType = $("#BasicDetails_JobType").val();
    }

    if (isInstallationDetails == true) {
        //var InstallationAddress = $('#txtAddress').val();
        if ($('#JobInstallationDetails_IsSameAsOwnerAddress').is(":checked")) {
            var InstallationAddress = $("#txtAddress1").val();
        }
        else {
            var InstallationAddress = $('#txtAddress').val();
        }
        if (InstallationAddress.trim() == "" || InstallationAddress == null || InstallationAddress == undefined) {
            $('#spantxtAddress').show();
            return false;
        } else { $('#spantxtAddress').hide(); }

        var isValid = addressValidationRules("JobInstallationDetails");

        $("#JobInstallationDetails_JobID").val(BasicDetails_JobID);

        var lstCustomDetails = [];
        $("#customDetails").find('.spanCustomFields').each(function () {
            lstCustomDetails.push({ JobCustomFieldId: $(this).attr('data-jobcustomfieldid'), FieldValue: $(this).val(), SeparatorId: $(this).attr('data-SeperatorId') });
        });

        var installationAddressData = JSON.stringify($('#JobInstallationAddress').serializeToJson());
        var objInstallationAddressData = JSON.parse(installationAddressData);

        var installationData = JSON.stringify($('#frmInstallationDetail').serializeToJson());
        objData.JobInstallationDetails = JSON.parse(installationData).JobInstallationDetails;

        $.extend(objData.JobInstallationDetails, objInstallationAddressData.JobInstallationDetails);

        objData.JobInstallationDetails.lstCustomDetails = lstCustomDetails;

        var customLength = $("#customDetails").find('.spanCustomFields').length;
        for (var i = 0; i < customLength; i++) {
            delete objData["lstCustomDetails[" + i + "]"];
        }
    }

    if (isSystemDetails == true) {
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
                    '<Series>' + htmlEncode(this['Series']) + '</Series><NoOfInverter>' + htmlEncode(this['NoOfInverter']) + '</NoOfInverter><Supplier>' + htmlEncode(this['Supplier']) + '</Supplier></inverter>';
            });
            xmlInverter = '<Inverters>' + sName + '</Inverters>';
        }

        $("#panelXml").val(xmlPanel);
        $("#inverterXml").val(xmlInverter);
        $("#JobSystemDetails_jobTypeTab").val(jobType);
        $("#JobSystemDetails_JobID").val(BasicDetails_JobID);
        $("#OldPanelDetails").val(JSON.stringify(OldPanelXml));
        $("#NewPanelDetails").val(JSON.stringify(PanelXml));

        var systemData = JSON.stringify($('#frmSystemDetail').serializeToJson());
        objData.JobSystemDetails = JSON.parse(systemData).JobSystemDetails;

        var jobSystemDetails = JSON.parse(systemData).JobSystemDetails;
        //jobSystemDetails.CalculatedSTC = $("#JobSystemDetails_CalculatedSTC").val();
        //jobSystemDetails.CalculatedSTCForSWH = $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.ModifiedCalculatedSTC = $("#BasicDetails_JobType").val() == 1 ? $("#JobSystemDetails_CalculatedSTC").val() : $("#JobSystemDetails_CalculatedSTCForSWH").val();
        jobSystemDetails.NoOfPanel = $("#JobSystemDetails_NoOfPanel").html();
        jobSystemDetails.InstallationType = $("#JobSystemDetails_InstallationType").val();
        if (systemXml.length > 0) {
            jobSystemDetails.SystemBrand = systemXml[0].Brand;
            jobSystemDetails.SystemModel = systemXml[0].Model;
        }
        jobSystemDetails.panelXmlTabular = xmlPanel;
        jobSystemDetails.inverterXmlTabular = xmlInverter;
        jobSystemDetails.SerialNumbers = $("#JobSystemDetails_SerialNumbers").val();
        jobSystemDetails.InverterSerialNumbers = $("#JobSystemDetails_InverterSerialNumbers").val();

        if (batteryManufacturerData.length > 0) {

            for (var i = 0; i < batteryManufacturerData.length; i++) {
                delete batteryManufacturerData[i]["JobBatteryManufacturerId"];
            }
            jobSystemDetails.lstJobBatteryManufacturer = batteryManufacturerData;
            if ($("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val().toLowerCase() == 'select') {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = ''
            } else {
                jobSystemDetails.batterySystemPartOfAnAggregatedControl = $("#JobSTCDetails_batterySystemPartOfAnAggregatedControl").val();
            }
            if ($("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val().toLowerCase() == 'select') {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = '';
            } else {
                jobSystemDetails.changedSettingOfBatteryStorageSystem = $("#JobSTCDetails_changedSettingOfBatteryStorageSystem").val();
            }
        }

        // Check to set STC value start
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
        // Check to set STC value end
        objData.objSystem = jobSystemDetails;
        //objData.OldPanelDetails = JSON.parse(data).OldPanelDetails
        //objData.NewPanelDetails = JSON.parse(data).NewPanelDetails;
        objData.OldPanelDetails = $("#OldPanelDetails").val();
        objData.NewPanelDetails = $("#NewPanelDetails").val();
        var serialVal;
        if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
            serialVal = true;
        }
        else {
            serialVal = SerialNumbersValidation();
        }

    }

    if (isDocumentsPhotos == true) {

        $("#JobSystemDetails_JobID").val(BasicDetails_JobID);

        var jobSystemDetails = {
            SerialNumbers: $("#JobSystemDetails_SerialNumbers").val(),
            InverterSerialNumbers: $("#JobSystemDetails_InverterSerialNumbers").val()
        }

        var serialVal;
        if (isPanelSerialNumber.toLowerCase() == 'false' && ProjectSession_UserTypeId == 8) {
            serialVal = true;
        }
        else {
            serialVal = SerialNumbersValidation();
        }

    }
    var jobData = JSON.stringify(objData);
    return jobData;
}
function getDataTabular(url, data, callback) {
    
    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        url: url,
        data: JSON.stringify(data),
        success: function (result) {
            if (result.status) {
                if (callback) {
                    callback(result);
                }                                
            }
            else {
                showErrorMessage(result.error);
            }
        },
        error: function (ex) {
            showErrorMessage('job has not been saved.' + ex);
        }
    });
}

function checkBusinessRules(isFromSTC) {
    
    if ($("#SSCID").val() != "" && $("#SSCID").val() > 0) {
        $("#BasicDetails_SSCID").val($("#SSCID").val());
    }
    if ($("#ScoID").val() != "" && $("#ScoID").val() > 0) {
        $("#BasicDetails_ScoID").val($("#ScoID").val());
    }
    var data = CommonTabularDataForSave();



    $.ajax({
        url: urlCheckBusinessRulesTabular,
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
                        //SaveJob();
                    }
                }
            }
            else if (result.IsError) {
                showErrorMessage(result.ValidationSummary);
                //DisableDropDownbyUsertype();
            }
        }
    });
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function ReloadSTCModule() {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlGetSTCJobForTabular + BasicDetails_JobID, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                
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
            
            if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $('#btnsave').hide();
            }

            $("#loading-image").hide();
        });
    }, 500);
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function ShowHideGSTSection(propertyType, ownerType) {
    
    if (tempIsRegisteredWithGST == 2 && modelIsRegisteredWithGST == 'true') {
        $(".isGSTRegistered").show();
        $("#jobGST").show();
        $("#BasicDetails_IsGst").val(true);
    }
    else if (tempIsRegisteredWithGST == 1 && modelIsRegisteredWithGST == 'true') {// && (propertyType == "commercial" || propertyType == "school" || ((ownerType == 'corporate body' || ownerType == 'trustee') && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true'))) {
        if (ownerType == 'corporate body' || ownerType == 'trustee') {
            if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                $(".isGSTRegistered").show();
                $("#jobGST").show();
                $("#BasicDetails_IsGst").val(true);
            }
            else {
                $(".isGSTRegistered").hide();
                $("#BasicDetails_IsGst").val(false);
            }
        }
        else if (propertyType == "commercial" || propertyType == "school") {
            $(".isGSTRegistered").show();
            $("#jobGST").show();
            $("#BasicDetails_IsGst").val(true);
        }
        else {
            $(".isGSTRegistered").hide();
            $("#BasicDetails_IsGst").val(false);
        }

    }
    else {
        $(".isGSTRegistered").hide();
        $("#BasicDetails_IsGst").val(false);
        $("#BasicDetails_IsGst").val(false);
    }
}

$("#aSwitch").click(function (e) {
    window.location.href = '/Job/Index?Id=' + eJobId + "&isTabularView=false";
});

$("#btnJobPrint").click(function () {
    var url = urlPrint + $(this).attr('jobid');
    window.open(url);
});

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

// To enable next tab after previous tab click
//$(function () {
//    $("#owner").click(function (e) {
//        $("#installation").removeClass('disabled');
//        e.preventDefault();
//    });
//});
//$(function () {
//    $("#installation").click(function (f) {
//        $("#stc").removeClass('disabled');
//        f.preventDefault();
//    });
//});
//$(function () {
//    $("#stc").click(function (g) {
//        $("#system").removeClass('disabled');
//        g.preventDefault();
//    });
//});
//$(function () {
//    $("#system").click(function (g) {
//        $("#installer").removeClass('disabled');
//        g.preventDefault();
//    });
//});
//$(function () {
//    $("#installer").click(function (g) {
//        $("#scheduling").removeClass('disabled');
//        g.preventDefault();
//    });
//});
//$(function () {
//    $("#scheduling").click(function (g) {
//        $("#documents").removeClass('disabled');
//        g.preventDefault();
//    });
//});

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

function ReloadSTCJobScreen(jobId, isRecUp) {
    
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlGetSTCJobNewScreen + jobID, function (data) {
            $('#reloadSTCJobScreen').empty();
            $('#reloadSTCJobScreen').append(data);
            //$('#checkListItemForTrade').load(actionCheckListItemForTrade, callbackCheckList);
            if (!isRecUp && (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3)) {
                $('#btnApplyTradeStc').hide();
            }
            if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                $("#CESDocbtns").hide();
                $("#STCDocBtns").hide();
                $('.isdelete').css("display", "none");
            }
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3 || ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 5 || ProjectSession_UserTypeId == 4) {
                $("#CESDocbtns").show();
                $("#STCDocBtns").show();
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

            $("#loading-image").hide();
        });
    }, 500);
}

function SerialNumbersValidation() {
    var serialnumberFieldId = $("#hdnJobSystemDetails_SerialNumbers").val();
    var isNumbersOverflow = false;
    var isDuplicateSerialNumber = false;

    //if ($("#JobSystemDetails_SerialNumbers").val().trim() != "") {
    if (serialnumberFieldId != "") {
        //var text = $("#JobSystemDetails_SerialNumbers").val().trim();
        if (serialnumberFieldId != undefined) {
            var text = serialnumberFieldId.valueOf().trim();
            var lines = text.split("\n");
            lines = lines.filter(function (n) { return n.length > 0 });
            var count = lines.length;
            for (var i = 0; i < lines.length; i++) {
                if (lines[i].length > 100) {
                    isNumbersOverflow = true;
                    break;

                }

            }
            isDuplicateSerialNumber = hasDuplicates(lines);
        }

    }

    if (isNumbersOverflow) {
        showErrorMessage("Serial numbers\'s maximum length is of 100 characters.")
        $('#spanSerialNumbers').show().text('Serial numbers\'s maximum length is of 100 characters.');
    }
    else if (isDuplicateSerialNumber) {
        showErrorMessage("Serial numbers should not be same.")
        $('#spanSerialNumbers').show().text('Serial numbers should not be same.');
    }
    else {
        $('#spanSerialNumbers').hide();
    }

    if (isNumbersOverflow || isDuplicateSerialNumber) {
        return false;
    }
    else {
        return true;
    }
}

function hasDuplicates(array) {
    var valuesSoFar = [];
    for (var i = 0; i < array.length; ++i) {
        var value = array[i];
        if (valuesSoFar.indexOf(value) !== -1) {
            return true;
        }
        valuesSoFar.push(value);
    }
    return false;
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
            data: JSON.stringify({ jobId: jobID, gbSCACode: gbSCACode, resellerID: resellerID, oldSCAName: SCAName, solarCompanyId: SolarCompanyId, jobInstallationYear: JobInstallationYear }),
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

$("#yesWarning").click(function () {
    $("#warning").modal('hide');
    //SaveJob();
    isOverRideSave = true;
    $('#btnSaveTab').click();
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

function BusinessValidationPop(result) {
    $(".modelBodyMessage").html('');
    $(".modelBodyMessage").append(result.Data.ValidationSummary);
    $("#warning").modal();
    return false;
}

function SearchHistory() {
    
    var categoryID = $('#historyCategory').val();
    categoryID = categoryID != null ? categoryID : 0;
    var IsImportantNote = FilterIsImportantNote_Glbl;
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
        //tagandsearchfilter();
        hideEditDeleteIcons();
        $("#loading-image").hide();
    }, 10);
}

function BindUserList() {
    $.ajax({
        type: 'GET',
        url: userlisturl,
        dataType: 'json',
        async: false,
        data: { jobid: jobid },
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

function CDMouseOut() {
    cdIsmouseIn = false;
}



$("#aViewSTCJobHistory").click(function () {
    $.get(GetSTCJobHistory + Model_JobID, function (data) {
        $('#STCJobHistoryOfJob').empty();
        $('#STCJobHistoryOfJob').append(data);
        $('#popupSTCJobHistory').modal({ backdrop: 'static', keyboard: false });
    });
});

$("#closePopupSTCJobHistory").click(function () {
    $('#popupSTCJobHistory').modal('toggle');
});
/*GetDefaultSettingForJob();*/
$("#onOffSwitchPreApproval").on('change.bootstrapSwitch', function (event, state) {
    ShowHidePreapprovalBox();
    BindPartialView('PreApproval', 12);
    $("#txtPreapprovalComment").val($("#spanPreapprovalComment").html());
    $("#preApprovalConnectionVal").val(1);
    UpdateAction();
});

$("#onOffSwitchConnection").on('change.bootstrapSwitch', function (event, state) {
    ShowHideConnectionBox();
    BindPartialView('Connection', 13);
    $("#txtConnComment").val($("#spanConnectionComment").html());
    $("#preApprovalConnectionVal").val(2);
    UpdateAction();
});
$("#onOffSwitchNewDocVIewer").on('change.bootstrapSwitch', function (event, state) {
    UpdateActionToSaveIsNewViewerUserWise();
});


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

function fnDocViewerOuterPopup() {
    $("#DocViewerOuterPopup").html("");
    $("#DocViewerOuterPopup").load(urlDocViewerOuterPopup);
}

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

$('#aGetSignatureSelfie').click(function (e) {
    e.preventDefault();
    $('#mdlGetSignatureSelfie').modal('show');
});

$("#btnDownloadJobFullPack").click(function (e) {
    //
    //console.log(eJobId);
    //console.log(urlGetFullJobPack);
    window.location.href = urlGetFullJobPack + "?JobID=" + eJobId;
});

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
    if (isNumbersOverflow || isDuplicateSerialNumberOnPage) {
        showErrorMessage(DuplicateSerialandInverterErrorMsg);
        return false;
    }
    else {
        return true;
    }    
}


function hasDuplicatesSerialNumberOnPage(array, IsPanel) {
    
    ispanelorinverterduplicate = false;    
    for (var i = 0; i < array.length; ++i) {
        for (var j = i + 1; j < array.length; j++) {
            if (array[i] == array[j]) {
                lineNumberForDuplicateSerial = j + 1;
                if (IsPanel) {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateSerialandInverterErrorMsg += 'Panel Serial number ' + DuplicatePanelSerialNumber + ' is duplicate. ';
                }

                else {
                    DuplicatePanelSerialNumber = array[j];
                    DuplicateSerialandInverterErrorMsg += 'Inverter Serial number ' + DuplicatePanelSerialNumber + ' is duplicate. ';
                }
                ispanelorinverterduplicate = true;
            }
        }
    }
    if (ispanelorinverterduplicate) {        
        return true;
    }
    else {
        return false;
    }
}
