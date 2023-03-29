$(document).ready(function () {
        // Initialize select2
        $("#ResellerId").select2();
    $('#fileUpload').fileupload({
        url: urlBulkUploadImportRec,
        dataType: 'json',
        done: function (e, data) {
            if (data.result.status) {
                $(".alert").hide();
                $("#errorMsgRegion").removeClass("alert-danger");
                $("#errorMsgRegion").addClass("alert-success");
                $("#errorMsgRegion").html(closeButton + "File imported successfully.");
                $("#errorMsgRegion").show();
            }
            else {
                $(".alert").hide();
                $("#errorMsgRegion").removeClass("alert-success");
                $("#errorMsgRegion").addClass("alert-danger");
                $("#errorMsgRegion").html(closeButton + data.result.message);
                $("#errorMsgRegion").show();
            }
            if (isKendoView == false) {
                setTimeout(function () {
                    SetParamsFromLocalStorage();
                    $("#datatable").dataTable().fnDraw();
                }, 1000);
            }
            else if (isKendoView == true) {
                setTimeout(function () {
                    filter = $('#datatable').data('kendoGrid').dataSource.filter();
                    STCSubmissionKendoIntialiize();
                    drawSTCSubmissionKendoGrid(filter);
                }, 1000);
            }
        }
    });
    $("#ExportCSVRECData").click(function (e) {
        selectedRows = [];
        //var selectedRows_SerialNumbers = [];
        var selectedRows_JobIDs = [];
        var withoutSTC_JobIDs = [];
        var flage = true;
        var withoutStcFlag = false;
        if ($('#datatable tbody input:checked').length == 0) {
            alert('Please select any job to export csv.');
            return false;
        }
        else {
            var JobType = '';
            var gbBatchRecUploadId = $('#datatable tbody input[type="checkbox"]:checked').attr('gbbatchrecuploadid');
            $('#datatable tbody input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') == true) {
                   
                    if (withoutStcFlag==false && JobType != '' && JobType != $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype")) {
                        alert("All selected Jobs should have same job type to export csv.");
                        flage = false;
                        return false;
                    }
                    else
                        JobType = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype");

                    var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();

                    if (withoutStcFlag==false && STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create' && gbBatchRecUploadId == $(this).attr('gbbatchrecuploadid'))//'Approved for REC Creation')
                    {
                        var STCJobDetailsID = $(this).attr('id').substring(4);
                        var STCTerm = $(this).attr('term');
                        selectedRows.push(STCJobDetailsID + '_' + STCTerm);

                        //var serialNumbers = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers");
                        //selectedRows_SerialNumbers.push(serialNumbers);

                        var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                        selectedRows_JobIDs.push(selectedJobID);
                    }
                    else if (withoutStcFlag==false){
                        alert("STC status must be 'Ready To Create' and 'REC BulkUploadId' same for all selected jobs.");
                        selectedRows = null;
                        return false;
                    }
                    var stc = $(this).attr('stc');
                    if (stc == '' || stc == undefined || stc == '0' || stc=='NaN' || stc=='null') {
                        var withoutSTCJobId = $(this).attr("JobID");
                        withoutSTC_JobIDs.push(withoutSTCJobId);
                        selectedRows = null;
                        withoutStcFlag = true;
                    }

                }
            });
        }
        if (withoutSTC_JobIDs != null && withoutSTC_JobIDs.length > 0 && withoutStcFlag==true) {
            $(".alert").hide();
            $("#errorMsgRegion").removeClass("alert-success");
            $("#errorMsgRegion").addClass("alert-danger");
            $("#errorMsgRegion").html(closeButton + "These jobs with job ID: [" + withoutSTC_JobIDs +"]  does not have STC values. Please review and calculate the STCs.");
            $("#errorMsgRegion").show();
        }

        if (selectedRows != null && selectedRows.length > 0 && flage && withoutStcFlag==false) {
            console.log(JobType);
            var rId = 0;
            if (UserTypeID == 1 || UserTypeID == 3) {
                rId = document.getElementById("ResellerId").value;
            }
            console.log(JobType);

            if (JobType == "1") {
                $.ajax({
                    url: checkInstallationDateURL + '?JobID=' + selectedRows_JobIDs.join(','),
                    type: "POST",
                    async: true,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.status == "false") {
                            $(".alert").hide();
                            $("#errorMsgRegion").removeClass("alert-success");
                            $("#errorMsgRegion").addClass("alert-danger");
                            $("#errorMsgRegion").html(closeButton + data.message);
                            $("#errorMsgRegion").show();
                        }
                        else {
                            window.location.href = urlBulkUploadREC_ExportCSV + '?JobID=' + selectedRows_JobIDs.join(',') + '&ResellerId=' + rId + '&JobType=' + JobType;
                            ReloadSectionAfterExportToCSVRECUpload(isKendoView);
                        }
                    },
                });
            }
            else {
                window.location.href = urlBulkUploadREC_ExportCSV + '?JobID=' + selectedRows_JobIDs.join(',') + '&ResellerId=' + rId + '&JobType=' + JobType;
                ReloadSectionAfterExportToCSVRECUpload(isKendoView);
            }
        }
    });
});
function ReloadSectionAfterExportToCSVRECUpload(isKendoView) {
    if (isKendoView == false) {

        setTimeout(function () {
            SetParamsFromLocalStorage();
            $("#datatable").dataTable().fnDraw();
        }, 1000);
    }
    else if (isKendoView == true) {

        setTimeout(function () {
            filter = $('#datatable').data('kendoGrid').dataSource.filter();
            STCSubmissionKendoIntialiize();
            drawSTCSubmissionKendoGrid(filter);
        }, 1000);
    }
}