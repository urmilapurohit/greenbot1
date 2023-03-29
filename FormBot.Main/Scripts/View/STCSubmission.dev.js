var filter = {};
var IsDefaultRECCredentials = true;
var rId = 0, selectedRows_JobIDs = [], JobType = '';
var isArchiveScreenVar = (typeof isArchiveScreen === 'undefined' ? 0 : (isArchiveScreen ? 1 : 0));
$(document).ready(function () {
    $('#searchSolarCompany').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnsolarCompanyid").val(0);
            $.each(solarCompanyList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchSolarCompany").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnsolarCompanyid").val(ui.item.id); // save selected id to hidden input
        }
    }).on('focus', function () { $(this).keydown(); });

    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };


    if (STCSubmission_Status != null && STCSubmission_Status != '') {
        SelectedStageId = STCSubmission_Status;
        var id = 0;
        var a = document.getElementById("jobstage_" + id);
        a.style.backgroundColor = "#f7f7f7";
        var a = document.getElementById("jobstage_" + SelectedStageId);
        a.style.backgroundColor = "#686868";

        if (UserTypeID == 1 || UserTypeID == 3) {
            document.getElementById("RamId").selectedIndex = 0;
        }
        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            localStorage.setItem('STCSubmission_PVDSWHCode', '');
        }
        if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
            //document.getElementById("ComplianceOfficerId").selectedIndex = 0;
        }
        localStorage.setItem('STCSubmission_RefJobId', '');
        localStorage.setItem('STCSubmission_OwnerName', '');
        localStorage.setItem('STCSubmission_InstallationAddress', '');
        localStorage.setItem('STCSubmission_IsSPVRequired', '-1');
        localStorage.setItem('STCSubmission_IsSPVInstallationVerified', '-1');
        $("#txtSubmissionFromDate").val('');
        $("#txtSubmissionToDate").val('');
        $("#txtSettlementFromDate").val('');
        $("#txtSettlementToDate").val('');
        $("#IsSPVRequired").val('-1');
        $("#IsSPVInstallationVerified").val('-1');
        invoiced = 0;
        GetSTCJobStageCount();
    }

    if (ComplianceIssuesDashboardStatus != null && ComplianceIssuesDashboardStatus != '') {
        //SetStageId(ComplianceIssuesDashboardStatus);

        SelectedStageId = ComplianceIssuesDashboardStatus;
        var id = 0;
        var a = document.getElementById("jobstage_" + id);
        a.style.backgroundColor = "#f7f7f7";

        //SelectedStageId = id;

        var a = document.getElementById("jobstage_" + SelectedStageId);
        a.style.backgroundColor = "#686868";

        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            $("#txtPVDSWHCode").val('');
        }
        if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
            //document.getElementById("ComplianceOfficerId").selectedIndex = 0;
        }

        $("#txtRefJobId").val('');
        $("#txtOwnerName").val('');
        $("#txtRecCode").val('');
        document.getElementById('chkNotInvoiced').checked = true;
        document.getElementById('chkHasBeenInvoiced').checked = true;
        invoiced = 0;
        GetSTCJobStageCount();
    }
    $("#aSwitch").click(function (e) {
        window.location.href = '/Job/StaticSTCSubmission?IsKendoStcSubmissionPage=false';
    });
    BindSTCJobStages();

    $('#txtSubmissionFromDate').datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#txtSubmissionToDate").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#txtSubmissionFromDate").val(), dateFormat));
            var toDate = new Date(ConvertDateToTick($("#txtSubmissionToDate").val(), dateFormat));
            if (fromDate > toDate) {
                $("#txtSubmissionToDate").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#txtSubmissionFromDate").val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if ($('#txtSubmissionToDate').data('datepicker')) {
            $('#txtSubmissionToDate').data('datepicker').setStartDate(new Date(tDate));
        }
    });
    $("#txtSubmissionToDate").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtSubmissionToDate').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtSubmissionFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtSubmissionToDate").val(), dateFormat));
    });

    $("#ExportCSVAll").click(function (e) {
        var JobID = '';
        if ($('#datatable tbody input:checked').length == 0) {
            alert('Please select any job to export csv.');
            return false;
        }
        else {

            if ($('#chkAll').prop('checked') == true) {
                JobID = '';
            }
            else {
                var selectedRows_JobIDs = [];
                $('#datatable tbody input[type="checkbox"]').each(function () {
                    if ($(this).prop('checked') == true) {
                        var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                        selectedRows_JobIDs.push(selectedJobID);
                    }
                })
                if (selectedRows_JobIDs != null && selectedRows_JobIDs.length > 0) {
                    JobID = selectedRows_JobIDs.join(',');
                }
            }
        }

        var reseller = '', solarcompanyid = '', pvdswhcode = '', RAM = '', complianceOfficerId = '';
        if (UserTypeID == 1 || UserTypeID == 3) {
            reseller = document.getElementById("ResellerId").value;
        }
        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            solarcompanyid = $('#hdnsolarCompanyid').val() == 0 ? -1 : $('#hdnsolarCompanyid').val();
            //pvdswhcode = $("#txtPVDSWHCode").val();
        }
        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2) {
            RAM = document.getElementById("RamId").value;
        }
        var filter = $("#datatable").data("kendoGrid").dataSource.filter();
        window.location.href = urlBulkUploadAll + '?JobID=' + JobID + '&stageid=' + SelectedStageId + '&reseller=' + reseller + '&solarcompanyid=' + solarcompanyid + (isArchiveScreenVar ? '&year=' + $("#ddlYear").val() : "") + '&RAM=' + RAM + '&RecCode=' + $("#txtRecCode").val() + '&RefJobId=' + $("#txtRefJobId").val() + '&ownername=' + $("#txtOwnerName").val() + "&complianceOfficername=" + $("#ComplianceOfficerId option:selected").text() + "&solarcompanyname=" + $("#searchSolarCompany").val() + "&ResellerName=" + $("#ResellerId option:selected").text() + "&RamName=" + $("#RamId option:selected").text() + '&isAllScaJobView=' + isAllScaJobView + '&filter= ' + JSON.stringify(filter) + '&isShowOnlyAssignJobsSCO=' + isShowOnlyAssignJobsSCO + '&isSPVRequired=' + $("#IsSPVRequired").val() + '&isSPVInstallationVerified=' + $("#IsSPVInstallationVerified").val();
    });
    $("#ExportCSV").click(function (e) {
        selectedRows = [];
        //var selectedRows_SerialNumbers = [];
        var selectedRows_JobIDs = [];
        var flage = true;
        if ($('#datatable tbody input:checked').length == 0) {
            alert('Please select any job to export csv.');
            return false;
        }
        else {
            var JobType = '';
            $('#datatable tbody input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') == true) {

                    if (JobType != '' && JobType != $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype")) {
                        alert("All selected Jobs should have same job type to export csv.");
                        flage = false;
                        return false;
                    }
                    else
                        JobType = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype");


                    var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();
                    if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create')//'Approved for REC Creation')
                    {
                        var STCJobDetailsID = $(this).attr('id').substring(4);
                        var STCTerm = $(this).attr('term');
                        selectedRows.push(STCJobDetailsID + '_' + STCTerm);

                        //var serialNumbers = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers");
                        //selectedRows_SerialNumbers.push(serialNumbers);

                        var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                        selectedRows_JobIDs.push(selectedJobID);
                    }
                    else {
                        alert("STC status must be 'Ready To Create'.")
                        return false;
                    }
                }
            })
        }
        if (selectedRows != null && selectedRows.length > 0 && flage) {
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
                            window.location.href = urlBulkUpload + '?JobID=' + selectedRows_JobIDs.join(',');
                        }
                    },
                });
            }
            else {
                window.location.href = urlBulkUploadSWHCSV + '?JobID=' + selectedRows_JobIDs.join(',');
            }
        }
    });

    //$("#ExportCSVRECData").click(function (e) {
    //    selectedRows = [];
    //    //var selectedRows_SerialNumbers = [];
    //    var selectedRows_JobIDs = [];
    //    var flage = true;
    //    if ($('#datatable tbody input:checked').length == 0) {
    //        alert('Please select any job to export csv.');
    //        return false;
    //    }
    //    else {
    //        var JobType = '';
    //        $('#datatable tbody input[type="checkbox"]').each(function () {
    //            if ($(this).prop('checked') == true) {

    //                if (JobType != '' && JobType != $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype")) {
    //                    alert("All selected Jobs should have same job type to export csv.");
    //                    flage = false;
    //                    return false;
    //                }
    //                else
    //                    JobType = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype");


    //                var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();
    //                var gbBatchRecUploadId = $(this).attr('gbbatchrecuploadid');

    //                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create' && (gbBatchRecUploadId == '' || gbBatchRecUploadId == undefined))//'Approved for REC Creation')
    //                {
    //                    var STCJobDetailsID = $(this).attr('id').substring(4);
    //                    var STCTerm = $(this).attr('term');
    //                    selectedRows.push(STCJobDetailsID + '_' + STCTerm);

    //                   //var serialNumbers = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers");
    //                    //selectedRows_SerialNumbers.push(serialNumbers);

    //                    var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
    //                    selectedRows_JobIDs.push(selectedJobID);
    //                }
    //                else {
    //                    alert("STC status must be 'Ready To Create' and 'REC BulkUploadId' must be null.");
    //                    selectedRows = null;
    //                    return false;
    //                }

    //            }
    //        })
    //    }

    //    if (selectedRows != null && selectedRows.length > 0 && flage) {
    //        var rId = 0;
    //        if (UserTypeID == 1 || UserTypeID == 3) {
    //            rId = document.getElementById("ResellerId").value;
    //        }
    //        console.log(JobType);
    //        window.location.href = urlBulkUploadREC_ExportCSV + '?JobID=' + selectedRows_JobIDs.join(',') + '&ResellerId=' + rId + '&JobType=' + JobType;
    //        setTimeout(function () {
    //            filter = $('#datatable').data('kendoGrid').dataSource.filter();
    //            STCSubmissionKendoIntialiize();
    //            drawSTCSubmissionKendoGrid(filter);
    //        }, 1000);
    //    }



    //});

    $('#txtSettlementFromDate').datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if ($("#txtSettlementToDate").val() != '') {
            var fromDate = new Date(ConvertDateToTick($("#txtSettlementFromDate").val(), dateFormat));
            var toDate = new Date(ConvertDateToTick($("#txtSettlementToDate").val(), dateFormat));
            if (fromDate > toDate) {
                $("#txtSettlementToDate").val('');
            }
        }
        var tickStartDate = ConvertDateToTick($("#txtSettlementFromDate").val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if ($('#txtSettlementToDate').data('datepicker')) {
            $('#txtSettlementToDate').data('datepicker').setStartDate(new Date(tDate));
        }
    });
    $("#txtSettlementToDate").datepicker({
        format: dateFormat,
        autoclose: true
    });
    $('#txtSettlementToDate').change(function () {
        var fromDate = new Date(ConvertDateToTick($("#txtSettlementFromDate").val(), dateFormat));
        var toDate = new Date(ConvertDateToTick($("#txtSettlementToDate").val(), dateFormat));
    });

    $("#txtPopUpSettlementDate").datepicker({
        format: dateFormat,
        autoclose: true
    });

    if (UserTypeID == 1 || UserTypeID == 3) {
        var IsResellerExist = false;
        $.ajax({
            type: 'get',
            url: urlGetReseller,
            dataType: 'json',
            data: '',
            async: false,
            success: function (reseller) {
                $.each(reseller, function (i, res) {
                    $("#ResellerId").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                    if (IsResellerExist == false && res.Value == localStorage.getItem('STCSubmission_ResellerId')) {
                        IsResellerExist = true;
                    }
                });

                var resellerID = STCSubmission_ResellerId;

                if (resellerID == '' || resellerID == '0') {
                    if (IsResellerExist) {
                        document.getElementById("ResellerId").value = localStorage.getItem('STCSubmission_ResellerId')
                    }
                    else {
                        $("#ResellerId").val($("#ResellerId option:first").val());
                        localStorage.setItem('STCSubmission_ResellerId', $("#ResellerId option:first").val());
                    }
                }
                else {

                    $("#ResellerId").val(resellerID);
                }

                GetRAMByResellerId($("#ResellerId").val());
                BindSolarCompany($("#ResellerId").val(), document.getElementById("RamId").value);
                GetSTCJobStageCount();
            },
            error: function (ex) {
                alert('Failed to retrieve Resellers.' + ex);
            }
        });
    }

    if (UserTypeID == 1) {
        if ($("#ResellerId").val() > 0) {
            GetFCOByResellerId($("#ResellerId").val());
            GetUpdateJobStatusFromRecByResellerId($("#ResellerId").val());
        }
    }

    if (UserTypeID == 2) {
        GetFCOByResellerId(ResellerId);
        GetRAMByResellerId(ResellerId);
        BindSolarCompany(ResellerId, document.getElementById("RamId").value);
        GetSTCJobStageCount();
    }

    if (UserTypeID == 5) {
        GetFCOByResellerId(ResellerId);
        if (isAllScaJobView == "true")
            BindSolarCompany(ResellerId, 0);
        else
            BindSolarCompany(0, LoggedInUserId);
        GetSTCJobStageCount();
    }

    if (UserTypeID == 8)
        GetSTCJobStageCount();

    if (UserTypeID == 1 || UserTypeID == 3) {
        $("#ResellerId").focus();
        //GetRecFailureReason($("#ResellerId").val());
    }
    else if (UserTypeID == 2) {
        //$("#SolarCompanyId").focus();
        $("#searchSolarCompany").focus();
    }
    else if (UserTypeID == 5) {
        $("#RamId").focus();
    }
    else {
        $("#txtRefJobId").focus();
    }

    SetParamsFromLocalStorage();

    $("#ResellerId").change(function () {
        if ($("#ResellerId").val() > 0) {
            GetFCOByResellerId($("#ResellerId").val());
            GetRAMByResellerId($("#ResellerId").val());
            BindSolarCompany($("#ResellerId").val(), 0);
            GetSTCJobStageCount();
            GetUpdateJobStatusFromRecByResellerId($("#ResellerId").val());
            localStorage.setItem('STCSubmission_ResellerId', document.getElementById("ResellerId").value);
            //GetRecFailureReason($("#ResellerId").val());
        }
        else {
            $("#RamId").empty();
            $("#RamId").append('<option value="0">Select</option>');
        }
        filter = $('#datatable').data('kendoGrid').dataSource.filter()
        $('#datatable').kendoGrid('destroy').empty();
        STCSubmissionKendoIntialiize();
        drawSTCSubmissionKendoGrid(filter);
        //$('#datatable').data('kendoGrid').dataSource.filter(filterApply)
    })

    $("#RamId").change(function () {
        if ($("#RamId").val() > 0) {
            BindSolarCompany(0, $("#RamId").val());
        }
        else {
            BindSolarCompany($("#ResellerId").val(), 0);
        }
        GetSTCJobStageCount();
        localStorage.setItem('STCSubmission_RamId', document.getElementById("RamId").value);
        filter = $('#datatable').data('kendoGrid').dataSource.filter()
        $('#datatable').kendoGrid('destroy').empty();
        STCSubmissionKendoIntialiize();
        drawSTCSubmissionKendoGrid(filter);
        //$('#datatable').data('kendoGrid').dataSource.filter(filterApply)
        //$('#datatable').data('kendoGrid').dataSource.read();
    })

    $("#btnSaveSettlementDate").click(function (e) {
        if ($("#txtPopUpSettlementDate ").val() != '') {
            $("#popupbox").modal('hide');

            $.ajax({
                type: 'get',
                url: urlUpdateSettlementDate,
                dataType: 'json',
                async: false,
                data: { jobId: $("#hdnJobId").val(), sDate: GetDates($("#txtPopUpSettlementDate").val()), isMultipleRecords: $("#isMultipleRecords").val() },
                success: function (data) {
                    if (data.success) {
                        obj.parentNode.innerHTML = $("#txtPopUpSettlementDate").val() + '<a title="edit" href="#" onclick="UpdateSettlementDate(this,' + $("#hdnJobId").val() + ',\'' + $("#txtPopUpSettlementDate").val() + '\');" class="edit sprite-img grid-edit-ic" style=" text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(closeButton + "Settlement date updated successfully.");
                        $("#errorMsgRegion").show();
                    }
                },
                error: function (ex) {
                    alert('Failed to update settlement date.' + ex);
                }
            });
            $(".error-message").hide();
        }
        else {
            $(".error-message").show();
        }
    });
    $("#btnSaveComplainceNotes").click(function (e) {

        $("#popupboxaddupdateComplianceNote").modal('hide');
        $.ajax({
            type: 'get',
            url: urlUpdateComplianceNote,
            dataType: 'json',
            async: false,
            data: { STCJobdetailsId: $("#hdnStcJobDetailsId").val(), Complaincenote: $("#txtareaComplianceNote").val(), isMultipleRecords: $("#isMultipleRecords").val() },
            success: function (data) {
                if (data.success) {
                    if ($("#txtareaComplianceNote").val() != "")
                        obj.parentNode.innerHTML = '<lable class="clsLabelComplianceNotes" >' + $("#txtareaComplianceNote").val() + '</lable>' + '<a title="edit" href="#" onclick="AddUpdateComplianceNotes(this,' + $("#hdnStcJobDetailsId").val() + ',\'' + $("#txtareaComplianceNote").val() + '\');" class="edit sprite-img grid-edit-ic" style="text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
                    else
                        obj.parentNode.innerHTML = '<a class="sprite-img add_ic" title="add" href="#" onclick="AddUpdateComplianceNotes(this,' + $("#hdnStcJobDetailsId").val() + ',\'' + $("#txtareaComplianceNote").val() + '\');" style="background-position: -132px -383px;height: 16px;width: 16px!important;text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
                    $(".alert").hide();
                    $("#errorMsgRegion").removeClass("alert-danger");
                    $("#errorMsgRegion").addClass("alert-success");
                    $("#errorMsgRegion").html(closeButton + "Compliance Note updated successfully.");
                    $("#errorMsgRegion").show();
                }
            },
            error: function (ex) {
                alert('Failed to update Compliance Note.' + ex);
            }
        });
    })
    $("#btnSavePvdSwhCode").click(function (e) {
        if ($("#txtPopUpPvdSwhCode").val() != '') {
            $("#popupbox1").modal('hide');

            $.ajax({
                type: 'get',
                url: urlUpdatePVDSWHCode,
                dataType: 'json',
                async: false,
                data: { jobId: $("#hdncode").val(), pvdswhCode: $("#txtPopUpPvdSwhCode").val(), isMultipleRecords: $("#isMultipleRecords").val() },
                success: function (data) {
                    if (data.success) {

                        obj.parentNode.innerHTML = '<lable class="clsLabelPVDSWH" >' + $("#txtPopUpPvdSwhCode").val() + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + $("#hdncode").val() + ',\'' + $("#txtPopUpPvdSwhCode").val() + '\');" class="edit sprite-img grid-edit-ic" style=" text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(closeButton + "PVD/SWH code updated successfully.");
                        $("#errorMsgRegion").show();

                    }
                },
                error: function (ex) {
                    alert('Failed to update settlement date.' + ex);
                }
            });
            $(".error-message").hide();
        }
        else {
            $(".error-message").show();
        }
    });

    $("#btnAssignJobs").click(function (e) {
        if (document.getElementById("AssignComplianceOfficerId").value != 0) {
            $("#Assignjobpopup").modal('hide');
            AssignJobToFCO(document.getElementById("AssignComplianceOfficerId").value);
            $(".error-message").hide();
        }
        else {
            $(".error-message").show();
        }
    });
    STCSubmissionKendoIntialiize();
    drawSTCSubmissionKendoGrid();
    {    //kendo




    }
    $(document).on("change", "#chkAll", function () {
        //$('#chkAll').on('click', function () {
        //var rows = table.rows({ 'search': 'applied' }).nodes();
        //$('input[type="checkbox"]', rows).prop('checked', this.checked);
        $("#datatable").find("[type='checkbox']").prop("checked", this.checked)
        chkCount = this.checked ? $('#datatable tbody').find('tr .k-checkbox-label').length : 0;

        if (this.checked) {
            var tSTC = 0;
            var amount = 0;
            $('#datatable tbody input[type="checkbox"]').each(function () {
                var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
                tSTC = (parseFloat(parseFloat(tSTC) + parseFloat(cSTC)).toFixed(2));
                var stcprice = ($(this).parent().parent().find('.lblSTcPrice').text() == '' || $(this).parent().parent().find('.lblSTcPrice').text() == undefined) ? "0" : $(this).parent().parent().find('.lblSTcPrice').text();
                amount = (parseFloat(parseFloat(amount) + (parseFloat(cSTC) * parseFloat(stcprice.replace("$", "")))).toFixed(2))
            })
            $("#btotalTradedSTC").html(tSTC);
            $("#btotalAmount").html(amount);
            $("#btotalNoofJobs").text(chkCount);
        }
        else {
            $("#btotalTradedSTC").html(0);
            $("#btotalAmount").html(0);
            $("#btotalNoofJobs").text(0);
        }

    });
    $(document).on("change", "#datatable tbody input[type='checkbox']", function () {
        if (this.checked) {
            chkCount++;
            if (chkCount == $("#datatable").data("kendoGrid").dataSource.view().length) {
                $('#chkAll').prop('checked', this.checked);
            }
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            var stcprice = ($(this).parent().parent().find('.lblSTcPrice').text() == '' || $(this).parent().parent().find('.lblSTcPrice').text() == undefined) ? "0" : $(this).parent().parent().find('.lblSTcPrice').text();
            var amount = (parseFloat(parseFloat($("#btotalAmount").html()) + parseFloat(cSTC * parseFloat(stcprice.replace("$", ""))))).toFixed(2)
            $("#btotalAmount").html(amount);
            $("#btotalTradedSTC").html(parseFloat(parseFloat(cSTC) + parseFloat(tSTC)).toFixed(2));
            $("#btotalNoofJobs").text(chkCount);
        }
        else {
            chkCount--;
            $('#chkAll').prop('checked', this.checked);
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            var stcprice = ($(this).parent().parent().find('.lblSTcPrice').text() == '' || $(this).parent().parent().find('.lblSTcPrice').text() == undefined) ? "0" : $(this).parent().parent().find('.lblSTcPrice').text();
            var amount = (parseFloat(parseFloat($("#btotalAmount").html()) - parseFloat(cSTC * parseFloat(stcprice.replace("$", ""))))).toFixed(2)
            $("#btotalAmount").html(amount);
            $("#btotalTradedSTC").html(parseFloat(parseFloat(tSTC) - parseFloat(cSTC)).toFixed(2));
            $("#btotalNoofJobs").text(chkCount);
        }
    });
});


function drawSTCSubmissionKendoGrid(filter) {

    kendo.ui.FilterMenu.fn.options.operators.date = {
        gte: "Begin Date",
        lte: "End Date"
    };
    $("#datatable").kendoGrid({
        columns: kendoCols,
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: urlGetJobSTCSubmissionKendo,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    if (options.filter != undefined) {
                        for (i = 0; i < options.filter.filters.length; i++) {
                            if (options.filter.filters[i].field == "STCSettlementDate" || options.filter.filters[i].field == "STCSubmissionDate" || options.filter.filters[i].field == "InstallationDate" || options.filter.filters[i].field == "CERAuditedDate") {
                                options.filter.filters[i].value = moment(new Date(options.filter.filters[i].value.toString().substr(0, 16))).format("DD-MM-YYYY")
                            }
                        }
                    }
                    options.stageid = SelectedStageId;
                    if (UserTypeID == 1 || UserTypeID == 3) {
                        options.reseller = document.getElementById("ResellerId").value;
                    }
                    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
                        options.solarcompanyid = $('#hdnsolarCompanyid').val();
                    }
                    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2) {
                        options.RAM = document.getElementById("RamId").value;
                    }
                    //if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
                    //    options.complianceOfficerId = document.getElementById("ComplianceOfficerId").value;
                    //}
                    options.RecCode = $("#txtRecCode").val();
                    options.isAllScaJobView = isAllScaJobView;
                    options.isShowOnlyAssignJobsSCO = isShowOnlyAssignJobsSCO;
                    options.RefJobId = $("#txtRefJobId").val();
                    options.ownername = $("#txtOwnerName").val();
                    options.defaultGrid = isDefaultGrid;
                    ////options.PanelInverterDetails = $("#txtPanelInverterDetails").val();
                    options.isSPVRequired = $("#IsSPVRequired").val();
                    options.isSPVInstallationVerified = $("#IsSPVInstallationVerified").val();
                    options.defaultGrid = isDefaultGrid;
                    if (isArchiveScreenVar)
                        options.year = $("#ddlYear").val();
                    return JSON.stringify(options);
                }
            },
            filter: filter,
            schema: {
                data: "data", // records are returned in the "data" field of the response
                total: "total",
                model: {
                    fields: {
                        STCSubmissionDate: { type: "date" },
                        STCSettlementDate: { type: "date" },
                        RECBulkUploadTimeDate: { type: "date" },
                        InstallationDate: { type: "date" },
                        CERAuditedDate: { type: "date" }
                    }
                }
            },
            pageSize: GridConfig[0].PageSize,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            requestStart: function (e) {
                $("#loading-image").addClass("display-block-important");
                setTimeout(function (e) {
                    $(".k-loading-image").hide();
                })
            },
            requestEnd: function (e) {
                $("#loading-image").removeClass("display-block-important");
            },
        },
        groupable: false,
        sortable: true,
        filterable: {
            extra: false,
        },
        columnResize: function (e) {
            SaveUserWiseColumnDetails();
        },
        columnReorder: onReorder,
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        resizable: true,
        scrollable: true,
        reorderable: true,
        filterMenuInit: function (e) {
            $(".loading-image").hide();
            if (e.field.includes("Date")) {
                this.thead.find("[data-field='" + e.field + "'] .k-grid-filter").click(function (e) {
                    setOperators();
                });
                var setOperators = function () {
                    var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                    firstValueDropDown.value("gte");
                    firstValueDropDown.trigger("change");
                    firstValueDropDown.enable(false)

                    var logicDropDown = e.container.find("select:eq(1)").data("kendoDropDownList");
                    logicDropDown.wrapper.hide()

                    var secondValueDropDown = e.container.find("select:eq(2)").data("kendoDropDownList");
                    secondValueDropDown.enable(false)
                    secondValueDropDown.value("lte");
                    secondValueDropDown.trigger("change");
                };
                setOperators();

            } else if (e.container.find('select').val() == "contains") {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.enable(false)
            } else if (e.container.find("select:eq(0)").data("kendoDropDownList") != null && e.container.find("select:eq(0)").data("kendoDropDownList").dataSource.options.data.length == 1) {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.enable(false)
            } else if (e.field == "JobID" || e.field == "STC" || e.field == "STCPrice") {
                var firstValueDropDown = e.container.find("select:eq(0)").data("kendoDropDownList");
                firstValueDropDown.value("eq");
                firstValueDropDown.trigger("change");
                firstValueDropDown.enable(false)
                e.container.find('input').attr("onkeypress", (e.field == "STC" || e.field == "STCPrice") ? "return isDecimal(event);" : "return isNumber(event);")
            }
        },
        dataBound: function (e) {
            $("#chkAll").change();
            $(".k-pager-sizes").find('select').on("change", function () {
                SetPageSizeForGrid($(this).val(), true, ViewPageId)
                GridConfig[0].PageSize = $(this).val();
                GridConfig[0].IsKendoGrid = true;
                GridConfig[0].ViewPageId = parseInt(ViewPageId);
            })
            $(".Installation-verified-false").each(function (k, data) {
                $(this).closest('tr').addClass("installation-notverified");
            })
        },
    });

    $("#datatable").data("kendoGrid").thead.kendoTooltip({
        filter: "th:not(':first')",
        content: function (e) {
            var target = e.target;
            return $(target).text();
        }
    });

}


var Kendo_ComplianceNotes = function (data) {
    if (UserTypeID == 1 || UserTypeID == 3) {
        if (((data.STCJobComplianceID > 0 || (data.ComplianceBy != parseInt(loggedInUserId))) && data.STCStatus == "Under Review")) {
            if (data.ComplianceNotes != null && data.ComplianceNotes != "") {
                var imgedit = ' text-decoration:none;margin-left:5px';
                var result = '<lable class="clsLabelComplianceNotes">' + data.ComplianceNotes + '</lable>' + '<a title="edit" href="#" onclick="AddUpdateComplianceNotes(this,' + data['STCJobDetailsId'] + ',\'' + data['ComplianceNotes'] + '\');" class="edit sprite-img grid-edit-ic" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                return result;
            }
            else {
                var imgadd = 'background-position: -132px -383px;height: 16px;width: 16px!important;text-decoration:none;margin-left:5px';
                var result = '<a class="img-border sprite-img add_ic" title="edit" href="#" onclick="AddUpdateComplianceNotes(this,' + data['STCJobDetailsId'] + ',' + '\'\'' + ');" class="edit" style="' + imgadd + '">&nbsp;&nbsp;</a>';
                return result;
            }
        }
    }
    var result = '<lable class="clsLabelComplianceNotes">' + data.ComplianceNotes + '</lable>';
    return result;
}
var Kendo_GBBatchRECUploadId = function (data) {
    if (data.GBBatchRECUploadId != null && data.GBBatchRECUploadId != "") {
        var imgDelete = 'text-decoration:none;margin-left:5px';
        return '<lable>' + ((data.GBBatchRECUploadId.split("-").length > 1 && data.GBBatchRECUploadId.split("-")[1] != "") ? data.GBBatchRECUploadId : data.GBBatchRECUploadId.split("-")[0]) + '</lable>' + '<a title="delete" href="#" onclick="RemoveSelectedJobFromBatch(' + data['STCJobDetailsId'] + ');" class="delete sprite-img grid-delete-ic " style="' + imgDelete + '">&nbsp;&nbsp;</a>';
    }
    else
        return '';
}
var Kendo_IsInvoiced = function (data) {
    if (data.IsInvoiced) {
        return '<span class="sprite-img file-green-ic "></span>';
    }
    else if (data.STCSettlementTerm == 5 && data.STCInvoiceCount == 1)
        return '<span class="sprite-img file-orange-ic"></span>';
    else {
        return '<span class="sprite-img file-red-ic"></span>';
    }
}

var Kendo_IsRelease = function (data) {
    if (data.IsRelease) {
        return '<span class="sprite-img file-green-ic "></span>';
    }
    else {
        return '<span class="sprite-img file-red-ic"></span>';
    }
}

var Kendo_IsCreditNote = function (data) {
    if (data.IsCreditNote) {
        return '<span class="sprite-img file-green-ic "></span>';
    }
    else {
        return '<span class="sprite-img file-red-ic"></span>';
    }
}
var Kendo_IsSPVRequired = function (data) {
    if (data.IsSPVRequired) {
        return '<span style="font-weight: bold;color: Green;">Yes</span>';
    }
    else {
        return '<span style="font-weight: bold;color: Red;">No</span>';
    }
}

var Kendo_IsSPVInstallationVerified = function (data) {
    if (data.IsSPVInstallationVerified == null) {
        return '<span style="font-weight: bold;">Not Yet Verified</span>';
    }
    else if (data.IsSPVInstallationVerified) {
        return '<span style="font-weight: bold;color: Green;">Verified</span>';
    }
    else {
        return '<span style="font-weight: bold;color: Red;">Verify Failed</span>';
    }
}

var Kendo_RefNumberOwnerName = function (data) {
    if (data.RefNumberOwnerName != null) {
        var url = urlJobIndex + "?id=" + data.Id;
        return '<a href="' + url + '" style="text-decoration:none;">' + data.RefNumberOwnerName + '</a>'
    }
    else {
        return '';
    }
}
var Kendo_JobID = function (data) {
    return data.JobID;
}

var Kendo_SolarCompany = function (data) {
    return "<span style='font-weight:bold;'>" + data.SolarCompany + "</span>";
}

var Kendo_InstallationDate = function (data) {
    return ToDate(data.strInstallationDate)
}

var Kendo_RECBulkUploadTimeDate = function (data) {
    if (data.strRECBulkUploadTimeDate != "")
        return ToDate(data.strRECBulkUploadTimeDate)
    else
        return "";
}

var Kendo_InstallationState = function (data) {
    return data.InstallationState;
}

var Kendo_InstallationTown = function (data) {
    return data.InstallationTown;
}
var Kendo_SystemSize = function (data) {
    return data.SystemSize;
}
var Kendo_InstallationAddress = function (data) {
    return data.InstallationAddress;
}

var Kendo_STCStatus = function (data) {
    if (data.STCStatusId == 19 && data.IsSPVInstallationVerified != null && !data.IsSPVInstallationVerified)
        // return '<lable class="clsSTCStatusLabel Installation-verified-false" data-JobID=' + data['JobID'] + ' data-JobType=' + data['JobTypeId'] + ' data-SerialNumbers=' + data['SerialNumbers'] + ' style="color:' + data.ColorCode + '">' + data.STCStatus + ' </lable>'
        return '<lable class="clsSTCStatusLabel Installation-verified-false" data-JobID=' + data['JobID'] + ' data-JobType=' + data['JobTypeId'] + ' style="color:' + data.ColorCode + '">' + data.STCStatus + ' </lable>'

    else
        //return '<lable class="clsSTCStatusLabel" data-JobID=' + data['JobID'] + ' data-JobType=' + data['JobTypeId'] + ' data-SerialNumbers=' + data['SerialNumbers'] + ' style="color:' + data.ColorCode + '">' + data.STCStatus + '</lable>'
        return '<lable class="clsSTCStatusLabel" data-JobID=' + data['JobID'] + ' data-JobType=' + data['JobTypeId'] + ' style="color:' + data.ColorCode + '">' + data.STCStatus + '</lable>'

}
var Kendo_STCSettlementTerm = function (data) {
    if (data.STCSettlementTerm == 1) {
        return '24 Hour';
    }
    else if (data.STCSettlementTerm == 2) {
        return '3 Days';
    }
    else if (data.STCSettlementTerm == 3) {
        return '7 Days';
    }
    else if (data.STCSettlementTerm == 4) {
        return 'CER Approved';
    }
    else if (data.STCSettlementTerm == 5) {
        return 'Partial Payment';
    }
    else if (data.STCSettlementTerm == 6) {
        return 'Upfront';
    }
    else if (data.STCSettlementTerm == 7) {
        return 'Rapid-Pay';
    }
    else if (data.STCSettlementTerm == 8) {
        return 'Opti-Pay';
    }
    else if (data.STCSettlementTerm == 9) {
        return 'Commercial';
    }
    else if (data.STCSettlementTerm == 10) {
        return data.CustomTermLabel;
    }
    else if (data.STCSettlementTerm == 11) {
        return "Invoice Stc";
    }
    else if (data.STCSettlementTerm == 12) {
        return "Peak Pay";
    }
    else {
        return '';
    }
}


var Kendo_Priority = function (data) {

    if (data.Priority == 1) {
        if (data.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-high.png" alt="" / ><span style="display:none">High</span></span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-high.png" alt="High" title="High"><span style="display:none">High</span></span></div>'
        }
    }
    else if (data.Priority == 2) {
        if (data.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-mid.png" alt="" /><span style="display:none">Normal</span></span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-mid.png" alt="Normal" title="Normal"><span style="display:none">Normal</span></span></div>';
        }
    }
    else {
        if (data.Urgent == 1) {
            return '<div class="ic_cover"><span class="urgent" data-toggle="tooltip" data-placement="right" title="Urgent"><img src="../Images/ic-urgent.png" alt="" /></span><span class="priority"><img src="../Images/prio-low.png" alt="" /></span><span style="display:none">Low</span></div>';
        }
        else {
            return '<div class="ic_cover"><span class="priority"><img src="../Images/prio-low.png" alt="Low" title="Low"><span style="display:none">Low</span></span></div>'
        }
    }
};

var Kendo_STCSubmissionDate = function (data) {
    return ToDate(data.strSTCSubmissionDate)
}
var Kendo_STCSettlementDate = function (data) {
    if (data.STCSettlementTerm != 5) {
        if (data.strSTCSettlementDate != "") {
            var sdate = ToDate(data.strSTCSettlementDate);
            var imgedit = ' text-decoration:none;margin-left:5px';
            var result;
            if (UserTypeID == 1 || UserTypeID == 3) {
                result = '<lable class="clsDateLabel">' + sdate + '</lable>' + '<a title="edit" href="#" onclick="UpdateSettlementDate(this,' + data['STCJobDetailsId'] + ',\'' + sdate + '\');" class="edit sprite-img grid-edit-ic" style="' + imgedit + '">&nbsp;&nbsp;</a>';
            }
            else {
                result = '<lable class="clsDateLabel">' + sdate + '</lable>';
            }
            return result;
        }
        else {
            if (data.STCSettlementTerm == 11 || (data.STCSettlementTerm == 10 && data.CustomSettlementTerm == 11))
                return "";
            else
                return '<lable class="clsDateLabel">On Approval</lable>';
        }
    }
    else if (data.STCSettlementTerm == 5) {
        if (data.STCSettlementDate != null) {
            var sdate = ToDate(data.strSTCSettlementDate);
            return '<lable class="clsDateLabel">' + sdate + '(Partial)</lable>';
        }
        else {
            return '<lable class="clsDateLabel">(Partial)</lable>';
        }
    }
}

var Kendo_JobTypeId = function (data) {
    if (data.JobTypeId > 0) {
        if (data.JobTypeId == 1) {
            return 'PVD';
        }
        else if (data.JobTypeId == 2) {
            return 'SWH';
        }
    }
    else
        return '';
}

var Kendo_CERAuditedDate = function (data) {
    if (data.strCERAuditedDate == null || data.strCERAuditedDate == '')
        return '';

    return ToDate(data.strCERAuditedDate);
}

var filterYesNo = function (element) {
    var data = [
        { Text: "Yes", Value: 1 },
        { Text: "No", Value: 0 },
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
};
var filter_JobTypeId = function (element) {
    var JobType = JSON.parse(colJobType)
    $.each(JobType, function (k, data) {
        data.field = "JobType"
    })
    element.kendoDropDownList({
        dataTextField: "Name",
        dataValueField: "ID",
        dataSource: JobType,
        optionLabel: "--Select Value--"
    });
};

var filter_JobID = function (element) {
    element.addClass("k-textbox");
    element.attr("onkeypress", "return isNumber(event);")
}

var filter_SystemSize = function (element) {
    element.addClass("k-textbox");
    element.attr("onkeypress", "return isDecimal(event);")
}

var filter_STCSubmissionDate = function (element) {

    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

var filter_InstallationDate = function (element) {

    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

var filter_RECBulkUploadTimeDate = function (element) {

    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

var filter_STCSettlementDate = function (element) {

    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};


var filter_STCSettlementTerm = function (element) {
    var data = [
        { Text: "24 Hour", Value: "1" },
        { Text: "3 Days", Value: "2" },
        { Text: "7 Days", Value: "3" },
        { Text: "CER Approved", Value: "4" },
        { Text: "Partial Payment", Value: "5" },
        { Text: "Upfront", Value: "6" },
        { Text: "Rapid-Pay", Value: "7" },
        { Text: "Opti-Pay", Value: "8" },
        { Text: "Commercial", Value: "9" },
        { Text: "Custom", Value: "10" },
        { Text: "Invoice Stc", Value: "11" },
        { Text: "Peak Pay", Value: "12" },
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
}

var filter_IsSPVInstallationVerified = function (element) {
    var data = [
        { Text: "Verified", Value: "1" },
        { Text: "Verify Failed", Value: "0" },
        { Text: "Not Yet Verified", Value: " " },
    ];
    element.kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: data,
        optionLabel: "--Select Value--"
    });
}

var filter_Priority = function (element) {
    var priority = JSON.parse(colPriority);
    element.kendoDropDownList({
        dataTextField: "Name",
        dataValueField: "ID",
        dataSource: priority,
        optionLabel: "--Select Value--"
    });

};

var filter_CERAuditedDate = function (element) {
    element.kendoDatePicker({ format: "dd/MM/yyyy" })
};

function STCSubmissionKendoIntialiize() {
    cols = [];
    kendoCols = [];
    colsDefs = [];
    var dbFields = []; // dynamic column viewbag
    dbFields = listColumns.split(',');
    var dbFieldsWidth = []; // dynamic column viewbag
    dbFieldsWidth = listColumnsWidth.split(',');
    kendoCols.push({
        title: 'Chkbox',
        field: 'Id',
        encoded: false,
        template: function (data) {
            var gbBatchRecId = "";
            if (data.GBBatchRECUploadId != null && data.GBBatchRECUploadId != "") {
                gbBatchRecId = (data.GBBatchRECUploadId.split("-").length > 1 && data.GBBatchRECUploadId.split("-")[1] != "") ? data.GBBatchRECUploadId : data.GBBatchRECUploadId.split("-")[0];
            }
            if ($('#chkAll').prop('checked') == true) {
                return '<input type="checkbox" class="k-checkbox" STCStatusId="' + data.STCStatusId + '" jobId="' + data.JobID + '" id="chk_' + data.STCJobDetailsId + '" term="' + data.STCSettlementTerm + '" custTerm="' + data.CustomSettlementTerm + '" IsInvoiced="' + data.IsInvoiced + '" STCInvoiceCount="' + data.STCInvoiceCount + '" IsCreditNote="' + data.IsCreditNote + '"  IsPayment="' + data.IsPayment + '" STCStatus="' + data.STCStatus + '" PartialValid="' + data.IsPartialValidForSTCInvoice + '" GBBatchRECUploadId="' + gbBatchRecId + '" IsSPVRequired="' + data.IsSPVRequired + '" IsSPVInstallationVerified="' + data.IsSPVInstallationVerified + '"IsRelease="' + data.IsRelease + '" STC="' + data.STC + '"  checked ><label class="k-checkbox-label" for="chk_' + data.STCJobDetailsId + '">';
            }
            else {
                return '<input type="checkbox" class="k-checkbox"  STCStatusId="' + data.STCStatusId + '" jobId="' + data.JobID + '" id="chk_' + data.STCJobDetailsId + '" term="' + data.STCSettlementTerm + '" custTerm="' + data.CustomSettlementTerm + '" IsInvoiced="' + data.IsInvoiced + '" STCInvoiceCount="' + data.STCInvoiceCount + '" IsCreditNote="' + data.IsCreditNote + '" IsPayment="' + data.IsPayment + '" STCStatus="' + data.STCStatus + '" PartialValid="' + data.IsPartialValidForSTCInvoice + '" GBBatchRECUploadId="' + gbBatchRecId + '" IsSPVRequired="' + data.IsSPVRequired + '" IsSPVInstallationVerified="' + data.IsSPVInstallationVerified + '"IsRelease="' + data.IsRelease + '" STC="' + data.STC + '"><label class="k-checkbox-label" for="chk_' + data.STCJobDetailsId + '">';
            }
        },
        filterable: false,
        sortable: false,
        headerTemplate: '<input type="checkbox" id="chkAll" name="select_all"  class="k-checkbox"><label class="k-checkbox-label" for="chkAll"></label>',
        width: 50,
        stickyPosition: 0,
        reorderable: false,
        resizable: false
    });
    kendoCols.push({
        title: "Has Multiple Records",
        field: "HasMultipleRecords",
        encoded: false,
        headerTemplate: '<span> Has Multiple Records </span>',
        width: 50,
        template: function (data) {
            if (data.HasMultipleRecords) {
                return '<i id="ticon_' + data.JobID + '" isKendo = true class="sprite-img grid-toggle-ic" style="cursor:pointer" onclick="GetSubRecords(' + data.JobID + ',this);"></i>';
            }
            else {
                return '';
            }
        },
        filterable: {
            operators: {
                string: {
                    "eq": "Is equal to"
                }
            },
            extra: false,
            ui: filterYesNo
        },
        sortable: true,
        reorderable: false,
        resizable: false,
        stickyPosition: 1,
    });
    $.each(dbFields, function (i, e) {
        var isfilterYesNo = false;
        var isfilterInt = false;
        var isFilterEq = false;
        var isHidden = false;
        if (e == 'HasMultipleRecords' || e == "IsInvoiced" || e == "IsCreditNote" || e == "IsSPVRequired" || e == "IsRelease")
            isfilterYesNo = true;
        if (e == "STCSettlementTerm" || e == "Priority")
            isfilterInt = true
        if (e == "JobID" || e == "JobTypeId" || e == "SystemSize" || e == "IsSPVInstallationVerified")
            isFilterEq = true
        if ((UserTypeID == 4 || UserTypeID == 8) && (e == "JobTypeId" || e == "AccountManager")) {
            isHidden = true;
        }
        if (e == "GBBatchRECUploadId" && !(UserTypeID == 1 || UserTypeID == 3))
            isHidden = true;
        if (e == "IsSPVInstallationVerified" && !(UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5 || UserTypeID == 4))
            isHidden = true;
        var columnData = $.grep(UserWiseColumnsData, function (n, i) {
            return n.Name == e;
        });
        if (e == "STCStatus") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    multi: true,
                    search: true,
                    dataSource: StcStatus,
                    itemTemplate: function (e) {
                        if (e.field == "all") {
                            return "<li><input type='checkbox' id='allsc' name='StcStatus' value='#= all#' class='k-checkbox'/><label class='k-checkbox-label' for='allsc'>#= all#</li>";
                        } else {
                            return "<li><input type='checkbox' id='#=JobStageId#' name='StcStatus' value='#=JobStageId#' class='k-checkbox'/><label class='k-checkbox-label' for='#=JobStageId#'>#= StageName #</li>"
                        }
                    }
                },
                template: "#=Kendo_" + e + "(data) #",
                sortable: true,
                hidden: isHidden
            });
        }
        else if (e == "SolarCompany") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    multi: true,
                    search: true,
                    dataSource: {
                        transport: {
                            read: {
                                url: urlGetSolarCompanyForFilter,
                                dataType: "json",
                                data: {
                                    id: $("#ResellerId").val(),
                                    solarCompanyId: $("#hdnsolarCompanyid").val() == 0 ? -1 : $("#hdnsolarCompanyid").val(),
                                    ramId: $("#RamId").val()
                                }
                            }
                        }
                    },
                    itemTemplate: function (e) {
                        if (e.field == "all") {
                            return "<li><input type='checkbox' id='allsc' name='SolarComanyId' value='#= all#' class='k-checkbox'/><label class='k-checkbox-label' for='allsc'>#= all#</li>";
                        } else {
                            return "<li><input type='checkbox' id='#=Value#' name='SolarComanyId' value='#=Value#' class='k-checkbox'/><label class='k-checkbox-label' for='#=Value#'>#= Text #</li>"
                        }
                    }
                },
                sortable: true,
                hidden: isHidden,
                template: "#=Kendo_" + e + "(data) #"
            });
        }
        else if (e == "ComplianceOfficer") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: 'ComplianceOfficer',
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                encoded: false,
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    multi: true,
                    search: true,
                    dataSource: {
                        transport: {
                            read: {
                                url: urlGetFCOByResellerId,
                                dataType: "json",
                                data: {
                                    rId: (UserTypeID == 2 || UserTypeID == 5) ? ProjectSession_ResellerId : $("#ResellerId").val(),
                                }
                            }
                        }
                    },
                    itemTemplate: function (e) {
                        if (e.field == "all") {
                            return "<li><input type='checkbox' id='allfco' name='ComplianceOfficerId' value='#= all#' class='k-checkbox'/><label class='k-checkbox-label' for='allfco'>#= all#</li>";
                        } else {
                            return "<li><input type='checkbox' id='#=Value#' name='ComplianceOfficerId' value='#=Value#' class='k-checkbox'/><label class='k-checkbox-label' for='#=Value#'>#= Text #</li>"
                        }
                    }
                },
                hidden: isHidden
            })
        }
        else if (e == "STCSettlementTerm") {
            var data = [
                { Text: "24 Hour", Value: "1" },
                { Text: "3 Days", Value: "2" },
                { Text: "7 Days", Value: "3" },
                { Text: "CER Approved", Value: "4" },
                { Text: "Partial Payment", Value: "5" },
                { Text: "Upfront", Value: "6" },
                { Text: "Rapid-Pay", Value: "7" },
                { Text: "Opti-Pay", Value: "8" },
                { Text: "Commercial", Value: "9" },
                { Text: "Custom", Value: "10" },
                { Text: "Invoice Stc", Value: "11" },
                { Text: "Peak Pay", Value: "12" },
            ];
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                encoded: false,
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    multi: true,
                    search: true,
                    dataSource: data,
                    itemTemplate: function (e) {
                        if (e.field == "all") {
                            return "<li><input type='checkbox' id='allfco' name='ComplianceOfficerId' value='#= all#' class='k-checkbox'/><label class='k-checkbox-label' for='allfco'>#= all#</li>";
                        } else {
                            return "<li><input type='checkbox' id='#=Value#' name='ComplianceOfficerId' value='#=Value#' class='k-checkbox'/><label class='k-checkbox-label' for='#=Value#'>#= Text #</li>"
                        }
                    }
                },
                template: "#=Kendo_" + e + "(data) #",
                hidden: isHidden
            })
        }
        else if (e == "AccountManager") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: 'AccountManager',
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                encoded: false,
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    multi: true,
                    search: true,
                    dataSource: {
                        transport: {
                            read: {
                                url: urlGetRAMForReseller,
                                dataType: "json",
                                data: {
                                    rId: $("#ResellerId").val(),
                                }
                            }
                        }
                    },
                    itemTemplate: function (e) {
                        if (e.field == "all") {
                            return "<li><input type='checkbox' id='allfco' name='AccountManagerId' value='#= all#' class='k-checkbox'/><label class='k-checkbox-label' for='allfco'>#= all#</li>";
                        } else {
                            return "<li><input type='checkbox' id='#=Value#' name='AccountManagerId' value='#=Value#' class='k-checkbox'/><label class='k-checkbox-label' for='#=Value#'>#= Text #</li>"
                        }
                    }
                },
                hidden: isHidden
            })
        }
        else if (isfilterYesNo) {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                template: "#=Kendo_" + e + "(data) #",
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to"
                        }
                    },
                    extra: false,
                    ui: filterYesNo
                },
                sortable: true,
                hidden: isHidden
            });
        } else if (isfilterInt) {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                template: "#=Kendo_" + e + "(data) #",
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to",
                            "neq": "Is not equal to",
                        }
                    },
                    extra: false,
                    ui: eval("filter_" + e)
                },
                sortable: true,
                hidden: isHidden
            });
        } else if (isFilterEq) {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                template: "#=Kendo_" + e + "(data) #",
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to"
                        }
                    },
                    extra: false,
                    ui: eval("filter_" + e)
                },
                sortable: true,
                hidden: isHidden
            });
        }
        else if (e.includes("Date")) {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                template: "#=Kendo_" + e + "(data) #",
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to",
                            "neq": "Is not equal to",
                        }
                    },
                    extra: true,
                    ui: eval("filter_" + e)
                },
                sortable: true,
                hidden: isHidden
            });
        } else if (e == "PVDSWHCode") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                template: function (data) {
                    //if (data.PVDSWHCode != null) {
                    //    var imgedit = 'background:url(../images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:5px';
                    //    var result = '<lable class="clsLabelPVDSWH">' + data.PVDSWHCode + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + data['STCJobDetailsId'] + ',\'' + data['PVDSWHCode'] + '\');" class="edit" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                    //    return result;
                    //}
                    //else {
                    //    var imgadd = 'background:url(../images/plus.png) no-repeat center; text-decoration:none;margin-left:5px';
                    //    var result = '<a class="img-border" title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + data['STCJobDetailsId'] + ',' + '\'\'' + ');" class="edit" style="' + imgadd + '">&nbsp;&nbsp;</a>';
                    //    return result;
                    //}
                    if (UserTypeID == 1 || UserTypeID == 3) {
                        if (data.PVDSWHCode != null) {
                            var imgedit = 'text-decoration:none;margin-left:5px';
                            var result = '<lable class="clsLabelPVDSWH">' + data.PVDSWHCode + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + data['STCJobDetailsId'] + ',\'' + data['PVDSWHCode'] + '\');" class="edit sprite-img grid-edit-ic" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                            return result;
                        }
                        else {
                            var imgadd = 'background-position: -132px -383px;height: 16px;width: 16px!important;text-decoration:none;margin-left:5px';
                            var result = '<a class="img-border sprite-img add_ic " title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + data['STCJobDetailsId'] + ',' + '\'\'' + ');" class="edit " style="' + imgadd + '">&nbsp;&nbsp;</a>';
                            return result;
                        }
                    }
                    else {
                        if (data.PVDSWHCode != null) {
                            //var imgedit = 'background:url(../images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:5px';
                            var result = '<lable class="clsLabelPVDSWH">' + data.PVDSWHCode + '</lable>';
                            return result;
                        }
                        else {
                            var result = "";
                            return result;
                        }
                    }

                },
                hidden: (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5) ? false : true,
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    operators: {
                        string: {
                            "contains": "Contains",
                        }
                    },
                    extra: false
                },
                hidden: isHidden
            })
        } else if (e == "STC") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                template: function (data) {
                    return '<lable class="clsLabel">' + parseInt(data.STC) + '</lable>';
                },
                width: parseInt(dbFieldsWidth[i]),
                attributes: {
                    class: "dt-right"
                },
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to",
                            "neq": "Is not equal to",
                        }
                    },
                    extra: false
                },
                hidden: isHidden
            })
        } else if (e == "STCPrice") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                template: function (data) {
                    if (data.STCSettlementTerm == 4 && data.IsInvoiced == false) {
                        return '';
                    }
                    else if ((data.STCSettlementTerm == 11 || (data.STCSettlementTerm == 10 && data.CustomSettlementTerm == 11)) && data.IsInvoiced == false) {
                        return "";
                    }
                    else {
                        if (data.IsGst) {
                            return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.STCPrice).toFixed(2) + ' +GST' + '</lable>';
                        }
                        else {
                            return '<lable class="lblSTcPrice">' + '$' + parseFloat(data.STCPrice).toFixed(2) + '</lable>';
                        }
                    }
                },
                hidden: isWholeSalers == "true" ? true : false,
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to",
                            "neq": "Is not equal to",
                        }
                    },
                    extra: false
                },
                hidden: isHidden
            })
        }
        else if (e == "IsSPVRequired") {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                template: function (data) {
                    if (data.IsSPVRequired) {
                        return '<span style="font-weight: bold;color: Green;">Yes</span>';
                    }
                    else {
                        return '<span style="font-weight: bold;color: Red;">No</span>';
                    }
                },
                width: parseInt(dbFieldsWidth[i]),
                attributes: {
                    class: "dt-right"
                },
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                filterable: {
                    operators: {
                        string: {
                            "eq": "Is equal to"
                        }
                    },
                    extra: false
                },
                hidden: isHidden
            })
        }
        else if (CheckRender(e)) {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                template: "#=Kendo_" + e + "(data) #",
                filterable: {
                    operators: {
                        string: {
                            "contains": "Contains",
                        }
                    },
                    extra: false
                },
                sortable: true,
                hidden: isHidden
            });
        }
        else {
            kendoCols.push({
                title: columnData[0].DisplayName,
                field: e,
                encoded: false,
                headerAttributes: {
                    "data-field": columnData[0].Name,
                    "data-columnid": columnData[0].ColumnID,
                    "data-order": columnData[0].OrderNumber
                },
                headerTemplate: '<span>' + columnData[0].DisplayName + '</span>',
                width: parseInt(dbFieldsWidth[i]),
                filterable: {
                    operators: {
                        string: {
                            "contains": "Contains",
                        }
                    },
                    extra: false
                },
                sortable: true,
                hidden: isHidden
            });
        }
    });

    kendoCols.push({
        title: 'Action',
        field: 'Id',
        template: function (data) {
            imgassign = 'text-decoration:none;';
            if (UserTypeID == 4 || UserTypeID == 6) {
                var assignHref = "javascript:LoadStc('" + data.JobID + "','" + data.STCJobDetailsId + "')";
            }
            else {
                var assignHref = "javascript:StcCompliance(this,'" + data.JobID + "','" + data.STCJobDetailsId + "')";
            }
            var returnHTML = '';
            returnHTML += '<a href="' + assignHref + '" class="action edit sprite-img compliance-ic" style="' + imgassign + '" title="Compliance Check">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
            return returnHTML;
        },
        sortable: false,
        width: 50,
        filterable: false
    });
    //kendoCols.push({
    //    title: 'Is SPV',
    //    field: 'IsSPVRequired',
    //    template: function (data) {
    //        if (data.IsSPVRequired) {
    //            return '<span style="font-weight: bold;color: Green;">Yes</span>';
    //        }
    //        else {
    //            return '<span style="font-weight: bold;color: Red;">No</span>';
    //        }
    //    },
    //    filterable: {
    //        operators: {
    //            string: {
    //                "eq": "Is equal to"
    //            }
    //        },
    //        extra: false,
    //        ui: filterYesNo
    //    },
    //    sortable: false,
    //    width: 65

    //});

    function CheckRender(functionName) {
        var isDefined = eval('(typeof Kendo_' + functionName + '==\'function\');');
        if (isDefined) {
            return true;
        }
        else {
            return false;
        }
    }
}

function GetFCOByResellerId(resellerId) {
    var IsComplianceOfficerExist = false;
    $("#ComplianceOfficerId").empty();
    $("#AssignComplianceOfficerId").empty();
    $.ajax({
        type: 'get',
        url: urlGetFCOByResellerId,
        dataType: 'json',
        async: false,
        data: { rId: resellerId },
        success: function (fcouser) {
            $("#ComplianceOfficerId").append('<option value="0">Select</option>');
            $("#AssignComplianceOfficerId").append('<option value="0">Select</option>');
            $.each(fcouser, function (i, fco) {
                $("#ComplianceOfficerId").append('<option value="' + fco.Value + '">' + fco.Text + '</option>');
                $("#AssignComplianceOfficerId").append('<option value="' + fco.Value + '">' + fco.Text + '</option>');
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Compliance Officers.' + ex);
        }
    });
    return false;
}

function GetRAMByResellerId(resellerId) {
    var IsRamExist = false;
    $("#RamId").empty();
    $.ajax({
        type: 'get',
        url: urlGetRAMForReseller,
        dataType: 'json',
        async: false,
        data: { rId: resellerId },
        success: function (ramuser) {
            $("#RamId").append('<option value="0">Select</option>');
            $.each(ramuser, function (i, ram) {
                $("#RamId").append('<option value="' + ram.Value + '">' + ram.Text + '</option>');
                if (IsRamExist == false && ram.Value == localStorage.getItem('STCSubmission_RamId')) {
                    IsRamExist = true;
                }
            });

            if (IsRamExist) {
                document.getElementById("RamId").value = localStorage.getItem('STCSubmission_RamId')
            }
            else {
                $("#RamId").val($("#RamId option:first").val());
                localStorage.setItem('STCSubmission_RamId', $("#RamId option:first").val());
            }
        },
        error: function (ex) {
            alert('Failed to retrieve Account Managers.' + ex);
        }
    });
    return false;
}

function BindSolarCompany(resellerID, ramID) {
    var IsCompanyExist = false;
    //$("#SolarCompanyId").empty();
    $("#searchSolarCompany").val("");
    var scurl = '';
    var searchid = '';
    if (ramID == 0) {
        scurl = urlGetSolarCompanyByResellerId;
        searchid = resellerID
    }
    else {
        scurl = urlGetSolarCompanyByRAMID;
        searchid = ramID
    }

    $.ajax({
        type: 'POST',
        url: scurl,
        dataType: 'json',
        async: false,
        data: { id: searchid },
        success: function (solarcompany) {
            solarCompanyList = [];
            //$("#SolarCompanyId").append('<option value="' + 0 + '">' + "All" + '</option>');
            solarCompanyList.push({ value: '0', text: 'All' });
            $.each(solarcompany, function (i, company) {
                //$("#SolarCompanyId").append('<option value="' + company.Value + '">' + company.Text + '</option>');
                solarCompanyList.push({ value: company.Value, text: company.Text });
                if (IsCompanyExist == false && company.Value == localStorage.getItem('STCSubmission_SolarCompanyId')) {
                    IsCompanyExist = true;
                }
            });
            var solarCompanyID = STCSubmission_SolarCompanyId;
            if (solarCompanyID == '' || solarCompanyID == null) {
                if (IsCompanyExist) {
                    //document.getElementById("SolarCompanyId").value = localStorage.getItem('STCSubmission_SolarCompanyId');
                    $('#hdnsolarCompanyid').val(localStorage.getItem('STCSubmission_SolarCompanyId'));
                }
                else {
                    //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
                    //localStorage.setItem('STCSubmission_SolarCompanyId',$("#SolarCompanyId option:first").val());
                    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
                }

                if (localStorage.getItem('STCSubmission_SolarCompanyId') == '') {
                    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
                }

                $.each(solarCompanyList, function (key, value) {
                    if (value.value == localStorage.getItem('STCSubmission_SolarCompanyId')) {
                        $("#searchSolarCompany").val(value.text);
                        $('#hdnsolarCompanyid').val(localStorage.getItem('STCSubmission_SolarCompanyId'));
                    }
                });
            }
            else {
                //$("#SolarCompanyId").val(solarCompanyID);
                $('#hdnsolarCompanyid').val(solarCompanyID);
            }
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}

function ResetSearchFilters(ShowAll) {
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        $('#hdnsolarCompanyid').val('0');
        $("#searchSolarCompany").val('All');
        localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
        localStorage.setItem('STCSubmission_PVDSWHCode', '');
    }
    localStorage.setItem('STCSubmission_RecCode', '');
    localStorage.setItem('STCSubmission_RefJobId', '');
    localStorage.setItem('STCSubmission_OwnerName', '');
    localStorage.setItem('STCSubmission_InstallationAddress', '');
    localStorage.setItem('STCSubmission_IsSPVRequired', '-1');
    localStorage.setItem('STCSubmission_IsSPVInstallationVerified', '-1');
    SetParamsFromLocalStorage();
    GetSTCJobStageCount();
    ////$("#txtPanelInverterDetails").val('');
    $("#IsSPVRequired").val('-1');
    $("#IsSPVInstallationVerified").val('-1');
    $("#datatable").data("kendoGrid").dataSource.filter({});
    $('#datatable').data('kendoGrid').dataSource.page(1);
}

function SearchJobSTCRecords() {
    if (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 5) {
        //localStorage.setItem('STCSubmission_ComplianceOfficerId', document.getElementById("ComplianceOfficerId").value);
    }
    if (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5) {
        localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
    }
    localStorage.setItem('STCSubmission_RecCode', $("#txtRecCode").val());
    localStorage.setItem('STCSubmission_RefJobId', $("#txtRefJobId").val());
    localStorage.setItem('STCSubmission_OwnerName', $("#txtOwnerName").val());
    localStorage.setItem('STCSubmission_IsSPVRequired', $("#IsSPVRequired").val());
    localStorage.setItem('STCSubmission_IsSPVInstallationVerified', $("#IsSPVInstallationVerified").val());
    // GetSTCJobStageCount();
    filter = $('#datatable').data('kendoGrid').dataSource.filter()
    $('#datatable').kendoGrid('destroy').empty();
    STCSubmissionKendoIntialiize();
    drawSTCSubmissionKendoGrid(filter);
    GetSTCJobStageCount();
}

function GetDates(date) {
    if (date != '') {
        var tickStartDate = ConvertDateToTick(date, dateFormat);
        return moment(tickStartDate).format("YYYY-MM-DD");
    }
    else {
        return '';
    }
}

function UpdateSettlementDate(t, jobid, settlementdate) {
    obj = t;
    window.event.preventDefault();
    $(".error-message").hide();
    $("#txtPopUpSettlementDate").val(settlementdate);
    $("#hdnJobId").val(jobid);
    $("#popupbox").modal();
    if ($(t).closest('tr').hasClass("hidden-row"))
        $("#isMultipleRecords").val(1)
    else
        $("#isMultipleRecords").val(0)
}

function AddUpdatePVDSWHCode(t, jobid, pvdswhCode) {
    obj = t;
    window.event.preventDefault();
    $(".error-message").hide();
    $("#txtPopUpPvdSwhCode").val(pvdswhCode);
    $("#hdncode").val(jobid);
    $("#popupbox1").modal();
    if ($(t).closest('tr').hasClass("hidden-row"))
        $("#isMultipleRecords").val(1)
    else
        $("#isMultipleRecords").val(0)
    setTimeout(function () {
        $('#txtPopUpPvdSwhCode').focus();
    }, 1000);
}


function AddUpdateComplianceNotes(t, StcJobDetailsId, complianceNote) {
    obj = t;
    window.event.preventDefault();
    $(".error-message").hide();
    $("#txtareaComplianceNote").val(complianceNote);
    $("#popupboxaddupdateComplianceNote").modal();
    $("#hdnStcJobDetailsId").val(StcJobDetailsId);
    if ($(t).closest('tr').hasClass("hidden-row"))
        $("#isMultipleRecords").val(1)
    else
        $("#isMultipleRecords").val(0)
    setTimeout(function () {
        $('#txtareaComplianceNote').focus();
    }, 1000);
}

function StcCompliance(row, JobId, JobDetailsId) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        type: 'POST',
        url: url_StatusCompliance,
        data: JSON.stringify({ uniq_param: (new Date()).getTime(), JobId: JobId, JobDetailsId: JobDetailsId }),
        cache: false,
        //async: false,
        dataType: "html",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $("#popupStcCompliance").modal({ backdrop: 'static', keyboard: false });
            $("#modalContentStcCompliance").html("");
            $("#modalContentStcCompliance").html(response);
            if ($(row).closest('tr').hasClass("hidden-row"))
                $("#isMultipleRecords").val(1)
            else
                $("#isMultipleRecords").val(0)

        },
        error: function (response) {


        }
    });
}

function LoadStc(jobId, JobDetailsId) {
    $.ajax({
        url: url_STCJobPopup,
        type: "POST",
        data: { jobId: jobId, IsSubmissionScreen: 1 },
        async: false,
        success: function (Data) {
            $("#STcBasicDetails").html(Data);
            $('#StcModal').modal({ backdrop: 'static', keyboard: false });

        }
    });

}

function LoadComplienceCheckList() {
    $("#collapsetow").hide();
    var currentStatus = $("#collapseOne").css("display");
    if (currentStatus == "none") {
        $("#collapseOne").show();
    } else {
        $("#collapseOne").hide();
    }
}

function LoadStcHistory() {
    $("#collapseOne").hide();
    var currentStatus = $("#collapsetow").css("display");
    if (currentStatus == "none") {
        $("#collapsetow").show();
    } else {
        $("#collapsetow").hide();
    }
}

function LoadComplienceCheckList() {
    $("#collapsetow").hide();
    var currentStatus = $("#collapseOne").css("display");
    if (currentStatus == "none") {
        $("#collapseOne").show();
    } else {
        $("#collapseOne").hide();
    }
}

function GetSubRecords(jobid, btn) {
    var $togglerowOpen = false;
    var $this = $("#ticon_" + jobid).closest('tr');
    if (!$this.hasClass('open')) {
        $.ajax({
            type: 'POST',
            url: urlGetSubRecordsForJob,
            dataType: 'html',
            async: false,
            data: { id: jobid, isKendo: $(btn).attr("isKendo") },
            success: function (jobrecords) {
                $(".open").removeClass('open');
                $(".kendo-grid .hidden-row").remove();
                $this.addClass('open');
                $("#ticon_" + jobid).closest('tr').after(jobrecords);

                $this.closest('table').find("tr").each(function () {
                    var $tr = $(this);
                    if ($tr.hasClass('togglerow'))
                        $togglerowOpen = false;

                    if ($togglerowOpen)
                        $tr.addClass('edit-panel');

                    if ($tr.hasClass('open'))
                        $togglerowOpen = true;
                });
            },
            error: function (ex) {
                alert('Failed to retrieve job records.' + ex);
            }
        });
    }
    else {
        $(".open").removeClass('open');
        $(".kendo-grid .edit-panel").removeClass('edit-panel');
    }
    return false;
}

function SetStageId(id) {
    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#f7f7f7";

    SelectedStageId = id;

    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#686868";

    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
    //    localStorage.setItem('STCSubmission_PVDSWHCode', '');
    //}
    //if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
    //    //document.getElementById("ComplianceOfficerId").selectedIndex = 0;
    //    //localStorage.setItem('STCSubmission_ComplianceOfficerId', document.getElementById("ComplianceOfficerId").value);
    //}

    //localStorage.setItem('STCSubmission_RefJobId', '');
    //localStorage.setItem('STCSubmission_OwnerName', '');
    //localStorage.setItem('STCSubmission_InstallationAddress', '');
    //localStorage.setItem('STCSubmission_IsSPVRequired', '-1');
    //localStorage.setItem('STCSubmission_IsSPVInstallationVerified', '-1');

    //$("#txtSubmissionFromDate").val('');
    //$("#txtSubmissionToDate").val('');
    //$("#txtSettlementFromDate").val('');
    //$("#txtSettlementToDate").val('');
    //$("#IsSPVRequired").val('-1');
    //$("#IsSPVInstallationVerified").val('-1');

    ////document.getElementById("SettlementTermId").selectedIndex = 0;
    ////localStorage.setItem('STCSubmission_STCSettlementTerm', document.getElementById("SettlementTermId").value);
    ////document.getElementById('chkNotInvoiced').checked = true;
    ////document.getElementById('chkHasBeenInvoiced').checked = true;
    //invoiced = 0;

    SetParamsFromLocalStorage();

    GetSTCJobStageCount(true);
    $('#datatable').data('kendoGrid').dataSource.page(1);
    //$('#datatable').data('kendoGrid').dataSource.read();
}

function GetSTCJobStageCount(asyncVar = false) {
    var resellerId = 0;
    var ramId = 0;
    var solarcompanyId = 0;

    if (UserTypeID == 1 || UserTypeID == 3) {
        resellerId = document.getElementById("ResellerId").value;
        ramId = document.getElementById("RamId").value;
        //solarcompanyId = document.getElementById("SolarCompanyId").value;
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else if (UserTypeID == 2) {
        resellerId = ResellerId;
        ramId = document.getElementById("RamId").value;
        //solarcompanyId = document.getElementById("SolarCompanyId").value;
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else if (UserTypeID == 5) {
        resellerId = ResellerId;
        ramId = LoggedInUserId;
        //solarcompanyId = document.getElementById("SolarCompanyId").value;
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else {
        solarcompanyId = SolarCompanyId;
    }
    $.ajax({
        type: 'POST',
        url: urlGetSTCJobStageCount,
        dataType: 'json',
        async: asyncVar,
        data: { reseller: resellerId, ram: ramId, sId: solarcompanyId, isAllScaJobView: isAllScaJobView, isShowOnlyAssignJobsSCO: isShowOnlyAssignJobsSCO, year: (isArchiveScreenVar ? $("#ddlYear").val() : "") },
        success: function (jobstagescount) {
            var sum = 0;
            $.each(jobstagescount.lstSTCJobStagesCount, function (i, count) {
                document.getElementById("jobstage_" + count.JobStageId).innerHTML = count.StageName + '<span>(' + count.jobCount + ')</span>';
                if (count.JobStageId != -1) {
                    sum = sum + count.jobCount;
                }
            });
            document.getElementById("jobstage_0").innerHTML = 'Show All' + '<span>(' + sum + ')</span>';
        },
        error: function (ex) {
            alert('Failed to retrieve count for Job Stages.' + ex);
        }
    });
    return false;
}

function AssignSelectedJobsToComplianceOfficer() {
    selectedRows = [];
    document.getElementById("AssignComplianceOfficerId").selectedIndex = 0;
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            selectedRows.push($(this).attr('id').substring(4));
        }
    })
    if (selectedRows != null && selectedRows.length > 0) {
        if (UserTypeID == 1) {
            $(".error-message").hide();
            $("#Assignjobpopup").modal();
        }
        else if (UserTypeID == 3) {
            AssignJobToFCO(LoggedInUserId);
        }
    }
    else {
        alert('Please select any job to assign.');
    }
}

function AssignJobToFCO(complianceOfficerId) {
    $.ajax({
        url: urlAssignJobToFCO,
        type: "POST",
        async: false,
        data: JSON.stringify({ complianceOfficerID: complianceOfficerId, jobs: selectedRows.join(','), isArchiveScreen: isArchiveScreenVar }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Jobs assigned to compliance officer successfully.");
                $("#errorMsgRegion").show();
                GetSTCJobStageCount();
                $('#datatable').data('kendoGrid').dataSource.read();

            }
        },
    });
}

function SendSTCInvoice(IsSTCInvoice) {
    selectedRows = [];
    var IsSuccess = true;
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {

            var PWDSWHCode = $(this).parent().parent().find('.clsLabelPVDSWH').text();
            var onApproval = $(this).parent().parent().find('.clsDateLabel').text();
            var IsInvoiced = $(this).attr('IsInvoiced');
            var IsCreditNote = $(this).attr('IsCreditNote');
            var IsPayment = $(this).attr('IsPayment');

            var status = $(this).attr('STCStatus');

            var settlementTerm = $(this).attr('term');
            var custTerm = $(this).attr('custterm');

            if (settlementTerm == 12 || (settlementTerm == 10 && custTerm == 12)) {
                alert("Can't generate STCInvoice for STC record which has PeakPay as settlement term.")
                IsSuccess = false;
                return false;
            }
            if (PWDSWHCode == '' || PWDSWHCode == undefined) {
                alert("Can't generate STCInvoice for STC record which has not PVD code.")
                IsSuccess = false;
            }
            else if (onApproval == 'On Approval' && status != 'CER Approved') {
                alert("Can't generate STCInvoice On Approval STC record")
                IsSuccess = false;
            }
            //else if (onApproval == 'Opti-Pay') {
            //    alert("Can't generate STCInvoice On Opti-Pay")
            //    IsSuccess = false;
            //}
            else if (IsInvoiced == "true" && IsSTCInvoice == '1') {
                alert("STCInvoice has been already generated for this job.")
                IsSuccess = false;
            }
            else if (IsCreditNote == "true" && IsSTCInvoice == '0') {
                alert("CreditNote has been already generated for this job.")
                IsSuccess = false;
            }
            //else if((IsCreditNote=='false' && IsPayment=="false") && IsSTCInvoice=='0'){
            //    alert("CreditNote can not be generated untill Payment is done.")
            //    IsSuccess = false;
            //}
            else if (IsInvoiced == 'false' && IsSTCInvoice == '0') {
                alert("First generate STC Invoice then after credit note will be generated.")
                IsSuccess = false;
            }
            else {
                var STCJobDetailsID = $(this).attr('id').substring(4);
                var STCTerm = $(this).attr('term');
                var jobid = $(this).attr("JobId");
                selectedRows.push(STCJobDetailsID + '_' + STCTerm + '_' + jobid);
            }
        }
    })

    if (IsSuccess) {
        if (selectedRows != null && selectedRows.length > 0) {
            var rId = 0;
            if (UserTypeID == 1 || UserTypeID == 3) {
                rId = document.getElementById("ResellerId").value;
            }
            $.ajax({
                url: urlGenerateSTCInvoiceForSelectedJobs,
                type: "POST",
                //async: false,
                data: JSON.stringify({ resellerId: rId, jobs: selectedRows.join(','), isstcinvoice: IsSTCInvoice, solarCompanyId: $('#hdnsolarCompanyid').val() }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {

                    if (data && data.status == false) {
                        if (data.error.toLowerCase() == 'specified method is not supported.' || data.error.toLowerCase() == 'renewtokenexception') {
                            window.open(urlConnect, "_blank");
                        }
                        else if (data.error.toLowerCase() == 'sessiontimeout')
                            window.location.href = urlLogout;
                    }
                    if (data.success) {
                        $(".alert").hide();
                        $("#errorMsgRegion").html(closeButton + "Invoice is generated successfully for selected jobs.");
                        $("#errorMsgRegion").show();
                        $('#datatable').data('kendoGrid').dataSource.read();
                        GetSTCJobStageCount();
                    }
                },
                error: function (data) {
                    alert('error');
                }
            });
        }
        else {
            alert('Please select any record to generate STC Invoice.');
        }
    }
}

function CreateRECEntry() {
    selectedRows = [];
    //var selectedRows_SerialNumbers = [];
    selectedRows_JobIDs = [];
    var IsValid = true;
    //var IsInvoiced ;
    //var STCSettlementTerm;
    //var selectedRows_term_isInvoiced = [];
    var selectedJobIds = [];

    if ($('#datatable tbody input:checked').length > 100) {
        alert('Maximum 100 jobs can be created in rec-registry at a time.');
        return false;
    }

    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select any job to create entry in REC');
        return false;
    }
    else {
        JobType = '';
        $('#datatable tbody input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') == true) {

                IsInvoiced = $(this).attr('IsInvoiced');
                STCSettlementTerm = $(this).attr('term');

                if (JobType != '' && JobType != $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype")) {
                    alert("All selected Jobs should have same Job Type to generate REC Registry entries");
                    IsValid = false;
                    return false;
                }
                else
                    JobType = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-jobtype");

                var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();
                IsSPVRequired = $(this).attr('IsSPVRequired');
                IsSPVInstallationVerified = $(this).attr('IsSPVInstallationVerified');

                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create') {

                    var STCJobDetailsID = $(this).attr('id').substring(4);
                    var STCTerm = $(this).attr('term');
                    var custSTCTerm = $(this).attr('custterm');
                    var IsInvoiced = $(this).attr('IsInvoiced');
                    var jobid = $(this).attr("JobId");
                    var GBBatchRecId = ($(this).attr('gbbatchrecuploadid') == 'undefined' || $(this).attr('gbbatchrecuploadid') == '') ? '' : '_' + $(this).attr('gbbatchrecuploadid');

                    if (IsSPVRequired == "true" && (IsSPVInstallationVerified == "false" || IsSPVInstallationVerified == "null")) {
                        alert("Installation verification not done yet.");
                        IsValid = false;
                        return false;
                    }
                    // if record should not have STCInvoice and dont have settlement term (On approval) for creating STC invoice.
                    if (IsInvoiced.toLowerCase() == "false" && STCTerm != 4 && STCTerm != 8 && STCTerm != 12 && (STCTerm != 10 && (custSTCTerm != 4 || custSTCTerm != 8 || custSTCTerm != 12))) {
                        selectedRows.push(STCJobDetailsID + '_' + STCTerm + '_' + jobid);
                    }

                    //var serialNumbers = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers");
                    //selectedRows_SerialNumbers.push(serialNumbers);

                    var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                    selectedRows_JobIDs.push(selectedJobID + GBBatchRecId);
                    selectedJobIds.push(selectedJobID);
                    //selectedRows_term_isInvoiced.push({STCJobDetailsID: STCJobDetailsID, STCSettlementTerm:STCTerm,IsInvoiced:IsInvoiced});

                }
                else {
                    alert("Can't generate REC Registry for selected STC records.")
                    IsValid = false;
                    return false;
                }



            }
        })

        if (selectedRows_JobIDs != null && selectedRows_JobIDs.length > 0 && IsValid) {
            //if(selectedRows!=null && selectedRows.length>0 && IsValid){
            //var rId = 0;
            if (UserTypeID == 1 || UserTypeID == 3) {
                rId = document.getElementById("ResellerId").value;
            }

            //var termInvoicedRecord = JSON.stringify(selectedRows_term_isInvoiced);
            $.ajax({
                url: urlCheckResellerRECCredentials,
                type: "GET",
                //async: false,
                data: { resellerID: rId.toString(), JobIds: selectedJobIds.join(',') },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.status.toString() == "true") {
                        userid = data.ResellerDetails[0].Id;

                        for (var i = 0; i < data.ResellerDetails.length; i++) {
                            if (data.ResellerDetails[i].RECLoginType != null && data.ResellerDetails[i].RECLoginType != "" && data.ResellerDetails[i].RECLoginType == "Admin") {
                                $("#defaultCompName").text(data.ResellerDetails[i].RECCompName);
                                $("#defaultAccName").text(data.ResellerDetails[i].RECAccName);
                                $("#hdnDefaultCERLoginId").val(data.ResellerDetails[i].CERLoginId);
                                $("#hdnDefaultCERPassword").val(data.ResellerDetails[i].CERPassword);
                                $("#hdnDefaultLoginType").val(data.ResellerDetails[i].RECLoginType);
                            }
                            else {
                                $("#otherCompName").text(data.ResellerDetails[i].RECCompName);
                                $("#otherAccName").text(data.ResellerDetails[i].RECAccName);
                                $("#hdnOtherCERLoginId").val(data.ResellerDetails[i].CERLoginId);
                                $("#hdnOtherCERPassword").val(data.ResellerDetails[i].CERPassword);
                                $("#hdnOtherLoginType").val(data.ResellerDetails[i].RECLoginType);
                            }
                        }
                        if (UserTypeID == 1 || UserTypeID == 3) {
                            $("#rec-Login").modal({ backdrop: 'static', keyboard: false });
                        }
                        else {

                            if ($("#hdnDefaultCERLoginId").val() != "" && $("#hdnDefaultCERPassword").val() != "" && $("#defaultAccName").text() != "") {
                                UseDefault();
                            }
                            else if ($("#hdnOtherCERLoginId").val() != "" && $("#hdnOtherCERPassword").val() != "" && $("#otherAccName").text() != "") {
                                UseOther();
                            }
                            else {
                                alert("REC Credentials not found. Please Update it from Profile");
                            }
                        }
                    }
                    if (data.status.toString() == "false" && data.lstJobIds.length > 0) {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-success");
                        $("#errorMsgRegion").addClass("alert-danger");
                        $("#errorMsgRegion").html(closeButton + "[" + data.strlstJobIds + "]" + " Jobs are already In Progress or SuccessFully Uploaded on REC.");
                        $("#errorMsgRegion").show();
                    }
                }
            });
        }
    }
}

function CreateInRECEntry() {
    $.ajax({
        url: urlCreateEntryInRECForSelectedJobs,
        type: "POST",
        //async: false,
        data: JSON.stringify({
            resellerId: rId,
            jobs: selectedRows.join(','),
            JobIDs: selectedRows_JobIDs.join(','),
            JobType: JobType,
            TotalSTC: $("#btotalTradedSTC").text(),
            CERLoginId: $("#hdnCERLoginId").val(),
            CERPassword: $("#hdnCERPassword").val(),
            RECAccName: $("#hdnRECAccName").val(),
            RECCompanyName: $("#hdnRECCompanyName").val(),
            LoginType: $("#hdnLoginType").val()
        }),
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
            }
            $(window).scrollTop(0);
            setTimeout(function () {
                filter = $('#datatable').data('kendoGrid').dataSource.filter();
                STCSubmissionKendoIntialiize();
                drawSTCSubmissionKendoGrid(filter);
                GetSTCJobStageCount();
            }, 1000);
            //$('#datatable').data('kendoGrid').dataSource.read();
            //GetSTCJobStageCount();
        },
    });
}

function UseDefault() {
    if ($("#hdnDefaultCERLoginId").val() != "" && $("#hdnDefaultCERPassword").val() != "" && $("#defaultAccName").text() != "") {
        IsDefaultRECCredentials = true;
        $("#hdnCERLoginId").val($("#hdnDefaultCERLoginId").val());
        $("#hdnCERPassword").val($("#hdnDefaultCERPassword").val());
        $("#hdnRECAccName").val($("#defaultAccName").text());
        $("#hdnRECCompanyName").val($("#defaultCompName").text());
        $("#hdnLoginType").val($("#hdnDefaultLoginType").val());
        CreateInRECEntry();
        $('#rec-Login').modal('toggle');
    }
    else {
        alert("Default Credentials are blank");
    }
}

function UseOther() {
    if ($("#hdnOtherCERLoginId").val() != "" && $("#hdnOtherCERPassword").val() != "" && $("#otherAccName").text() != "") {
        IsDefaultRECCredentials = false;
        $("#hdnCERLoginId").val($("#hdnOtherCERLoginId").val());
        $("#hdnCERPassword").val($("#hdnOtherCERPassword").val());
        $("#hdnRECAccName").val($("#otherAccName").text());
        $("#hdnRECCompanyName").val($("#otherCompName").text());
        $("#hdnLoginType").val($("#hdnOtherLoginType").val());
        CreateInRECEntry();
        $('#rec-Login').modal('toggle');
    }
    else {
        alert("Credentials are blank");
    }
}

function UpdateUser() {
    $('#rec-Login').modal('toggle');
    window.open(urlEditUser + "/" + userid + "#horizontalTab3", '_blank');
}

function SetSTCInvoiceStatus() {
    if (document.getElementById('chkNotInvoiced').checked) {
        invoiced = 2;
    }
    if (document.getElementById('chkHasBeenInvoiced').checked) {
        invoiced = 1;
    }
    if (document.getElementById('chkNotInvoiced').checked && document.getElementById('chkHasBeenInvoiced').checked) {
        invoiced = 0;
    }
    if (document.getElementById('chkNotInvoiced').checked == false && document.getElementById('chkHasBeenInvoiced').checked == false) {
        invoiced = 3;
    }
}

function ValidateLettersWithSpaceOnly(evt) {
    evt = (evt) ? evt : event;
    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
        ((evt.which) ? evt.which : 0));

    if ((charCode > 32 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) && (charCode <= 47 || charCode > 58)) {
        return false;
    }

    return true;
}

function CheckTextContent() {
    var totalCharacterCount = $("#txtPopUpPvdSwhCode").val();
    var strValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
    var strChar;
    var FilteredChars = "";
    for (i = 0; i < totalCharacterCount.length; i++) {
        strChar = totalCharacterCount.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            alert('Special characters are not allowed for PVD code.');
            $("#txtPopUpPvdSwhCode").val('');
            return false;
        }
    }
    return false;
}

function ToDate(data) {
    if (data != null) {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
        return moment(date).format(dateFormatMoment);
    }
    else {
        return '';
    }
}

function SetParamsFromLocalStorage() {
    $("#txtRefJobId").val(localStorage.getItem('STCSubmission_RefJobId'));
    $("#txtOwnerName").val(localStorage.getItem('STCSubmission_OwnerName'));
    $("#txtRecCode").val(localStorage.getItem('STCSubmission_RecCode'))
    //$("#txtInstallationAddress").val(localStorage.getItem('STCSubmission_InstallationAddress'));
}

function BindSTCJobStages() {
    $("#STCJobStageID").empty();
    $.ajax({
        type: 'POST',
        url: urlGetSTCJobStages,
        dataType: 'json',
        async: false,
        //data: { usertypeid: UserTypeID },
        success: function (stages) {
            $("#STCJobStageID").append('<option value="0">Select STC job stage</option>');
            $.each(stages, function (i, stage) {
                $("#STCJobStageID").append('<option value="' + stage.Value + '">' +
                    stage.Text + '</option>');
            });
        },
        error: function (ex) {
            alert('Failed to retrieve Job Stages.' + ex);
        }
    });
    return false;
}

function BulkChangeSTCJobStage() {
    if (document.getElementById('STCJobStageID').selectedIndex > 0) {
        var confirmMsg = '';
        selectedRows = [];
        var selectedRowsJobids = [];
        var IsValid = true;
        var IsPartialValid = true;
        var CheckSPVrequireddata = [];
        $('#datatable tbody input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') == true) {
                if ($(this).attr('STCStatus') != 'CER Approved' && $(this).attr('STCStatus') != 'New Submission') {
                    if (document.getElementById('STCJobStageID').value == 22 && $(this).attr('term') == 5 && (!$(this).attr('PartialValid'))) {
                        IsPartialValid = false;
                        return false;
                    }
                    else if (document.getElementById('STCJobStageID').value == 14 && $(this).attr('IsInvoiced') == 'false') {
                        confirmMsg = 'are you sure you want to change this status as per normal, and status change but no credit note is created because no invoice has been generated.'
                        selectedRows.push($(this).attr('id').substring(4));
                        selectedRowsJobids.push($(this).attr('jobid'));
                    }
                    else {
                        selectedRows.push($(this).attr('id').substring(4));
                        selectedRowsJobids.push($(this).attr('jobid'));
                    }
                    var spvrequireddata = { JobId: $(this).attr('jobid'), STCJobDetailsID: $(this).attr('id').substring(4) };
                    CheckSPVrequireddata.push(spvrequireddata);


                }
                else {
                    IsValid = false;
                    return false;
                }
            }
        })
        if (IsPartialValid) {
            if (IsValid) {
                if (selectedRows != null && selectedRows.length > 0) {
                    if (confirmMsg == '') {
                        confirmMsg = document.getElementById('STCJobStageID').value == 22 ? "Are you sure you want to change stc status  as CERApproved of selected jobs as you can not reverse it after change ?" : "Are you sure you want to change stc status of selected jobs ?"
                    }
                    var result = confirm(confirmMsg);



                    //var CheckSPVrequireddata = json.st({ checkSPVrequired: [{ JobId: selectedRowsJobids.join(','), stcjobids: selectedRows.join(',')}] })
                    if (result) {
                        $.ajax({
                            type: 'POST',
                            url: urlBulkChangeSTCJobStage,
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ stcjobstage: document.getElementById('STCJobStageID').value, stcjobids: selectedRows.join(','), checkSPVrequired: CheckSPVrequireddata }),
                            success: function (data) {
                                debugger;
                                if (data.success) {
                                    $(".alert").hide();
                                    $("#errorMsgRegion").removeClass("alert-danger");
                                    $("#errorMsgRegion").addClass("alert-success");
                                    $("#errorMsgRegion").html(closeButton + "Job Stages updated successfully.");
                                    $("#errorMsgRegion").show();
                                    GetSTCJobStageCount();
                                    $('#datatable').data('kendoGrid').dataSource.read();
                                    document.getElementById('STCJobStageID').selectedIndex = 0;
                                }
                                else {
                                    $(".alert").hide();
                                    $("#errorMsgRegion").addClass("alert-danger");
                                    $("#errorMsgRegion").removeClass("alert-success");
                                    $("#errorMsgRegion").html(closeButton + data.message);
                                    $("#errorMsgRegion").show();
                                    GetSTCJobStageCount();
                                    $('#datatable').data('kendoGrid').dataSource.read();
                                    document.getElementById('STCJobStageID').selectedIndex = 0;
                                }
                            },
                            error: function (ex) {

                            }
                        });
                    }
                }
                else {
                    alert('Please select atleast one Job to update STC Stage.');
                    return false;
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
    }
    else {
        alert('Please select STC Job Stage first.');
        return false;
    }
}
function InstallationVerifyAgain(button) {
    var OnlySelectSpvAllowJobs = true;
    var stcstatusreadytocreate = true;
    button.disabled = true;
    selectedRows = [];

    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select atleast one Job for Installation Verify.');
        button.disabled = false;
        return false;
    }

    else {
        $('#datatable tbody input:checked').each(function (text, value) {
            STCStatus = $(this).attr('STCStatus');
            IsSPVRequired = $(this).attr('IsSPVRequired');
            IsSPVInstallationVerified = $(this).attr('isspvinstallationverified');
            if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create') {
                if (IsSPVRequired != "true" || IsSPVInstallationVerified == "true") {

                    OnlySelectSpvAllowJobs = false;
                    button.disabled = false;
                    return false;
                }
            }
            else {
                stcstatusreadytocreate = false;

                return false;
            }


        });
        if (OnlySelectSpvAllowJobs == false) {
            alert("Selected job must have is spv required true and spv installation verify not true.");
            button.disabled = false;
            return false;
        }
        if (stcstatusreadytocreate == false) {
            alert("Selected Job STC status must be 'Ready To Create'.");
            button.disabled = false;
            return false;
        }
        $('#datatable tbody input[type="checkbox"]:checked').each(function () {
            if ($(this).prop('checked') == true) {
                STCStatus = $(this).attr('STCStatus');
                IsSPVRequired = $(this).attr('IsSPVRequired');
                IsSPVInstallationVerified = $(this).attr('isspvinstallationverified');
                var message;
                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create') {

                    if (IsSPVRequired == "true" && (IsSPVInstallationVerified == "false" || IsSPVInstallationVerified == "null")) {
                        selectedRows.push($(this).attr('id').substring(4));
                        $.ajax({
                            type: 'POST',
                            url: urlInstallationVerifyAgain,
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ stcjobids: selectedRows.join(',') }),
                            success: function (data) {
                                if (data.success) {
                                    button.disabled = false;
                                    $(".alert").hide();
                                    //$("#errorMsgRegion").removeClass("alert-success");
                                    //$("#errorMsgRegion").addClass("alert-danger");
                                    if (data.IsError) {
                                        $("#errorMsgRegion").removeClass("alert-success");
                                        $("#errorMsgRegion").addClass("alert-danger");
                                        message = "Something went wrong,Please check SPV failure reason.";
                                    }
                                    else {
                                        $("#errorMsgRegion").removeClass("alert-danger");
                                        $("#errorMsgRegion").addClass("alert-success");
                                        message = "SPV Installation verification done successfully.";
                                    }
                                    $("#errorMsgRegion").html(closeButton + message);
                                    $("#errorMsgRegion").show();
                                    $('#datatable').data('kendoGrid').dataSource.read();
                                    document.getElementById('STCJobStageID').selectedIndex = 0;
                                }
                            },
                            error: function (ex) {
                                button.disabled = false;
                            }
                        });
                    }
                    if (IsSPVRequired != "true" || IsSPVInstallationVerified == "true") {
                        alert("Selected job must have is spv required true and SPV installation verified not true.");
                        flage = false;
                        return false;
                    }

                }
                else {
                    alert("STC status must be 'Ready To Create'.");
                    flage = false;
                    return false;
                }

            }
        });
    }
}

function UpdateJobStatusFromREC(isSwitch) {
    var resellerId = $('#ResellerId').val();
    var isAutoUpdateCEROn = $('#UpdateJobStatusFromRECSwitch').prop('checked');
    var result = true;
    if (isAutoUpdateCEROn && isSwitch) {
        result = confirm("Are you sure you want to switch on Auto CER Update On? On switching yes all CER data will get updated.");
    }
    var msg = isSwitch ? isAutoUpdateCEROn ? "Update job status from rec has been switched ON" : "Update job status from rec has been switched OFF" : "Updating job status from rec";
    if (result) {
        $.ajax({
            url: urlUpdateJobStatusFromREC,
            type: 'post',
            dataType: 'json',
            async: false,
            data: { resellerId: resellerId, isSwitch: isSwitch, isAutoUpdateCEROn: isAutoUpdateCEROn },
            success: function (data) {
                if (data.status) {
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + msg);
                    $("#errorMsgRegion").show();
                    showHideBtnUpdateJobStatusFromREC(isAutoUpdateCEROn)
                    $('#datatable').data('kendoGrid').dataSource.read();
                }
            },
            error: function (ex) {
                alert('Failed to update.' + ex);
            }
        });
    }
    else {
        $('#UpdateJobStatusFromRECSwitch').prop('checked', false);
    }
}

function GetUpdateJobStatusFromRecByResellerId(resellerId) {
    $.ajax({
        url: urlGetUpdateJobStatusFromRecByResellerId,
        type: 'get',
        dataType: 'json',
        async: false,
        data: { resellerId: resellerId },
        success: function (data) {
            $("#errorMsgRegion").hide();
            $('#UpdateJobStatusFromRECSwitch').prop('checked', data.isAutoUpdateCEROn);
            showHideBtnUpdateJobStatusFromREC(data.isAutoUpdateCEROn);
        },
        error: function (ex) {
            alert('Failed to retrive.' + ex);
        }
    })
}

function showHideBtnUpdateJobStatusFromREC(isAutoUpdateCEROn) {
    isAutoUpdateCEROn ? $('.btnUpdateJobStatusFromREC').hide() : $('.btnUpdateJobStatusFromREC').show();
}

function ToDate(data) {
    if (data != null) {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
        return moment(date).format(dateFormat.toUpperCase());
    }
    else {
        return '';
    }
}

function SaveUserWiseColumnDetails() {
    $("#loading-image").css("display", "");
    var arrColumns = [];
    var orderNo = 0;
    var columnsDetails = $("#datatable").data("kendoGrid").columns;
    $('#datatable th').each(function (i, item) {
        orderNo = orderNo + 1;
        var SavePageSize = $("#datatable").data("kendoGrid").dataSource.pageSize();
        arrColumns.push({ Width: columnsDetails[i].width, ColumnID: $(item).attr("data-columnid"), OrderNumber: orderNo, PageSize: SavePageSize });
    });

    if (arrColumns.length > 0) {
        $.ajax({
            url: "/Job/SaveUserWiseColumnsDetails",
            type: "POST",
            async: false,
            data: JSON.stringify({ columns: arrColumns, MenuId: STCSubmissionView }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.status) {
                    listColumns = result.ListColumnName;
                    listColumnsWidth = result.ListColumnWidth;
                    UserWiseColumnsData = result.JSUserColumnList;
                    DefaultPageSize = result.PageSize;
                }
                else {
                    alert("Try again.");
                }
            },
            error: function (ex) {
                alert("Try again. " + ex);
            },
        });
    }
    else {
        alert("Add atleast one column.");
    }
    $("#loading-image").css("display", "none");
}

function onReorder(e) {
    if (typeof e.column.reorderable != "undefined" && !e.column.reorderable) {
        var grid = $('#datatable').data('kendoGrid');
        setTimeout(function () {
            grid.reorderColumn(e.oldIndex, grid.columns[e.newIndex]);
        }, 0);
    }
    setTimeout(function () {
        SaveUserWiseColumnDetails()
    }, 150);

}
function GetRecFailureReason() {
    //$("#rec-failure-reason-div").load(urlGetRecFailureReason + '?resellerId=' + $("#ResellerId").val(), function () {
    //    $("#rec-failure-reason").modal({ backdrop: 'static', keyboard: false });
    //    $("#errorMsgRegionRECFailure").hide();
    //})
    window.open(urlRecFailureReason, '_blank');
    //$.ajax({
    //    url: urlGetRecFailureReason,
    //    type: 'get',
    //    dataType: 'json',
    //    async: false,
    //    data: { resellerId: $("#ResellerId").val()},
    //    success: function (data) {
    //        
    //        if (data.length > 0) {
    //            $(".rec-failure-reason-div").html(data);
    //        }
    //    },
    //    error: function (ex) {
    //        alert('Failed to retrive.' + ex);
    //    }
    //})
}
function GetSPVFailureReason() {
    //$("#spv-failure-reason-div").load(urlGetSpvFailureReason + '?refJobId=' + $("#txtSpvRefJobId").val(), function () {
    //    $("#spv-failure-reason").modal({ backdrop: 'static', keyboard: false });
    //    $("#errorMsgRegionSPVFailure").hide();
    //})
    $("#spv-failure-reason").modal({ backdrop: 'static', keyboard: false });
    $("#errorMsgRegionSPVFailure").hide();
}
$('#btnSearchSpvFailureReason').click(function () {
    var jobid = $("#txtSpvRefJobId").val();
    if (jobid != "") {
        if (isNaN(jobid)) {
            $("#errorMsgRegionSPVFailure").html(closeButton + "Please enter valid JobId for search SPVfailure reasons.");
            $("#errorMsgRegionSPVFailure").show();
        }
        else {
            $("#spv-failure-reason-div").load(urlGetSpvFailureReason + '?refJobId=' + $("#txtSpvRefJobId").val(), function () {
                $("#spv-failure-reason").modal({ backdrop: 'static', keyboard: false });
                $("#errorMsgRegionSPVFailure").hide();
            })
        }

    }
    else {

        $("#errorMsgRegionSPVFailure").html(closeButton + "Please enter valid JobId for search SPVfailure reasons.");
        $("#errorMsgRegionSPVFailure").show();
        return false;
    }

});
function RemoveJobFromBatch() {
    var id = "";
    $("#rec-failure-reason .collection-item.active:not(.selectAll)").each(function (k, data) {
        id = id + $(this).data("stcjobdetailsid") + ","
    })
    if (id == "") {
        alert("Please select jobs")
        return;
    }
    RemoveSelectedJobFromBatch(id.replace(/,\s*$/, ""));
}

function RemoveSelectedJobFromBatch(id) {
    event.preventDefault();
    var openPanelId = $("#accordionExample").find(".collapse.in").attr("id");
    $.ajax({
        url: urlRemoveJobFromBatch,
        type: 'POST',
        dataType: 'json',
        async: false,
        data: { StcJobdetailsId: id },
        success: function (data) {
            if (data.status) {
                $("#errorMsgRegionRECFailure").removeClass("alert-danger");
                $("#errorMsgRegionRECFailure").addClass("alert-success");
                $("#errorMsgRegionRECFailure").html(closeButton + "Remove Job From Batch Successfully");
                $("#errorMsgRegionRECFailure").show();
                if (GridConfig[0].IsKendoGrid)
                    $('#datatable').data('kendoGrid').dataSource.read();
                //else
                //    $("#datatable").dataTable().fnDraw();
                //$("#rec-failure-reason-div").load(urlGetRecFailureReason + '?resellerId=' + $("#ResellerId").val(), function () {
                //    $("#accordionExample").find("#" + openPanelId).addClass("in");
                //})
            } else {
                $("#errorMsgRegionRECFailure").html(closeButton + "Something happend not remove job from batch");
                $("#errorMsgRegionRECFailure").show();
            }
        },
        error: function (ex) {
            alert('Failed to retrive.' + ex);
        }
    })
}

function ResetSPVInstaller() {
    selectedRows = [];
    var selectedRows_JobIDs = [];
    var flage = true;
    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select atleast one Job for spv release.');
        return false;
    }
    else {
        $('#datatable tbody input[type="checkbox"]:checked').each(function () {
            if ($(this).prop('checked') == true) {
                var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();
                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create')//'Approved for REC Creation')
                {
                    IsSPVRequired = $(this).attr('IsSPVRequired');
                    if (IsSPVRequired == "true") {
                        var STCJobDetailsID = $(this).attr('id').substring(4);
                        selectedRows.push(STCJobDetailsID);


                        var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                        selectedRows_JobIDs.push(selectedJobID);
                    }
                    else {
                        alert("Selected job must have is spv required true.");
                        flage = false;
                        return false;
                    }


                }
                else {
                    alert("STC status must be 'Ready To Create'.");
                    flage = false;
                    return false;
                }
            }
        })
    }
    if (selectedRows != null && selectedRows.length > 0 && selectedRows_JobIDs != null && selectedRows_JobIDs.length > 0 && flage) {
        var result = confirm("Are you sure you want to spv release of selected jobs ? ");
        if (result) {
            $.ajax({
                type: 'POST',
                url: urlResetSPVInstaller,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ stcJobDetailIds: selectedRows.join(','), stcjobids: selectedRows_JobIDs.join(',') }),
                success: function (data) {
                    if (data.success) {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(closeButton + "Spv release for selected job done successfully.");
                        $("#errorMsgRegion").show();
                        // GetSTCJobStageCount();
                        $('#datatable').data('kendoGrid').dataSource.read();
                    }
                },
                error: function (ex) {

                }
            });
        }
    }
}
function SearchSPVFailureReason() {
    $("#spv-failure-reason-div").load(urlGetSpvFailureReason + '?resellerId=' + $("#ResellerId").val() + "&RefJobId=" + $("#txtSpvRefJobId").val().trim(), function () {
    })
}

function ResetSPVSearchFilters(ShowAll) {
    $("#txtSpvRefJobId").val('');
    SearchSPVFailureReason();
}

function SPVReset() {
    var selectedRows_JobIDs = [];
    var flage = true;
    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select atleast one Job for spv reset.');
        return false;
    }
    else {
        $('#datatable tbody input[type="checkbox"]:checked').each(function () {
            if ($(this).prop('checked') == true) {
                IsRelease = $(this).attr('IsRelease');
                STCStatus = $(this).attr('STCStatus');
                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create') {
                    if (IsRelease == "true") {
                        var STCJobDetailsID = $(this).attr('id').substring(4);
                        selectedRows.push(STCJobDetailsID);


                        var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                        selectedRows_JobIDs.push(selectedJobID);
                    }
                    else {
                        alert("Selected job must have is spv release true.");
                        flage = false;
                        return false;
                    }
                }
                else {
                    alert("STC status must be 'Ready To Create'.");
                    flage = false;
                    return false;
                }

            }
        })
    }
    if (selectedRows != null && selectedRows.length > 0 && selectedRows_JobIDs != null && selectedRows_JobIDs.length > 0 && flage) {
        var result = confirm("Are you sure you want to reset spv of selected jobs ? ");
        if (result) {
            $.ajax({
                type: 'POST',
                url: urlSPVReset,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ stcJobDetailIds: selectedRows.join(','), jobids: selectedRows_JobIDs.join(',') }),
                success: function (data) {
                    if (data.success) {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-danger");
                        $("#errorMsgRegion").addClass("alert-success");
                        $("#errorMsgRegion").html(closeButton + "Spv reset for selected job done successfully.");
                        $("#errorMsgRegion").show();
                        // GetSTCJobStageCount();
                        $('#datatable').data('kendoGrid').dataSource.read();
                    }
                },
                error: function (ex) {

                }
            });
        }
    }
}
