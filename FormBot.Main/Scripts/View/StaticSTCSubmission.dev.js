var IsDefaultRECCredentials = true;
var rId = 0, selectedRows_JobIDs = [], JobType = '';
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
        //var a = document.getElementById("jobstage_" + id);
        //a.style.backgroundColor = "#f7f7f7";
        $(".filters-row > ul > li > a").css("background", "#f7f7f7");
        var a = document.getElementById("jobstage_" + SelectedStageId);
        a.style.backgroundColor = "#686868";

        if (UserTypeID == 1 || UserTypeID == 3) {
            document.getElementById("RamId").selectedIndex = 0;
        }
        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            localStorage.setItem('STCSubmission_PVDSWHCode', '');
        }
        if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
            document.getElementById("ComplianceOfficerId").selectedIndex = 0;
        }

        localStorage.setItem('StaticSTCSubmission_RefJobId', '');
        localStorage.setItem('STCSubmission_OwnerName', '');
        localStorage.setItem('STCSubmission_InstallationAddress', '');
        $("#txtSubmissionFromDate").val('');
        $("#txtSubmissionToDate").val('');
        $("#txtSettlementFromDate").val('');
        $("#txtSettlementToDate").val('');
        $("#IsSPVRequired").val('All');
        $("#IsSPVInstallationVerified").val('All');
        document.getElementById('chkNotInvoiced').checked = true;
        document.getElementById('chkHasBeenInvoiced').checked = true;
        invoiced = 0;
        GetSTCJobStageCount();
    }

    if (ComplianceIssuesDashboardStatus != null && ComplianceIssuesDashboardStatus != '') {
        //SetStageId(ComplianceIssuesDashboardStatus);

        SelectedStageId = ComplianceIssuesDashboardStatus;
        var id = 0;
        //var a = document.getElementById("jobstage_" + id);
        //a.style.backgroundColor = "#f7f7f7";
        $(".filters-row > ul > li > a").css("background", "#f7f7f7");

        //SelectedStageId = id;

        var a = document.getElementById("jobstage_" + SelectedStageId);
        a.style.backgroundColor = "#686868";

        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
            $("#txtPVDSWHCode").val('');
        }
        if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
            document.getElementById("ComplianceOfficerId").selectedIndex = 0;
        }

        $("#txtRefJobId").val('');
        $("#txtOwnerName").val('');
        $("#txtInstallationAddress").val('');
        $("#txtSubmissionFromDate").val('');
        $("#txtSubmissionToDate").val('');
        $("#txtSettlementFromDate").val('');
        $("#txtSettlementToDate").val('');
        document.getElementById('chkNotInvoiced').checked = true;
        document.getElementById('chkHasBeenInvoiced').checked = true;
        invoiced = 0;
        GetSTCJobStageCount();
    }

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
            pvdswhcode = $("#txtPVDSWHCode").val();
        }
        if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2) {
            RAM = document.getElementById("RamId").value;
        }
        if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
            complianceOfficerId = document.getElementById("ComplianceOfficerId").value;
        }

        var NotInvoiced = "No";
        var HasBeenInvoiced = "No";
        if (document.getElementById('chkNotInvoiced').checked) {
            NotInvoiced = "Yes";
        }
        if (document.getElementById('chkHasBeenInvoiced').checked) {
            HasBeenInvoiced = "Yes";
        }
        window.location.href = urlBulkUploadAll + '?JobID=' + JobID + '&stageid=' + SelectedStageId + '&reseller=' + reseller + '&solarcompanyid=' + solarcompanyid + '&RecCode=' + pvdswhcode + '&RAM=' + RAM + '&complianceOfficerId=' + complianceOfficerId + '&RefJobId=' + $("#txtRefJobId").val() + '&ownername=' + $("#txtOwnerName").val() + '&installationaddress=' + $("#txtInstallationAddress").val() + '&submissionfromdate=' + GetDates($("#txtSubmissionFromDate").val()) + '&submissiontodate=' + GetDates($("#txtSubmissionToDate").val()) + '&settlementfromdate=' + GetDates($("#txtSettlementFromDate").val()) + '&settlementtodate=' + GetDates($("#txtSettlementToDate").val()) + '&IsInvoiced=' + invoiced + '&SettlementTermId=' + $("#SettlementTermId").val() + "&complianceOfficername=" + $("#ComplianceOfficerId option:selected").text() + "&solarcompanyname=" + $("#searchSolarCompany").val() + "&SettlementTermname=" + $("#SettlementTermId option:selected").text() + "&NotInvoiced=" + NotInvoiced + "&HasBeenInvoiced=" + HasBeenInvoiced + "&ResellerName=" + $("#ResellerId option:selected").text() + "&RamName=" + $("#RamId option:selected").text() + '&isAllScaJobView=' + isAllScaJobView + '&isShowOnlyAssignJobsSCO=' + isShowOnlyAssignJobsSCO + '&isSPVRequired=' + $("#IsSPVRequired").val() + '&isSPVInstallationVerified=' + $("#IsSPVInstallationVerified").val();
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
            } else {
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

    //                    //var serialNumbers = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers");
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
    //        console.log(JobType);
    //        var rId = 0;
    //        if (UserTypeID == 1 || UserTypeID == 3) {
    //            rId = document.getElementById("ResellerId").value;
    //        }
    //        console.log(JobType);

    //        window.location.href = urlBulkUploadREC_ExportCSV + '?JobID=' + selectedRows_JobIDs.join(',') + '&ResellerId=' + rId + '&JobType=' + JobType;
    //        setTimeout(function () {
    //            SetParamsFromLocalStorage();
    //            $("#datatable").dataTable().fnDraw();
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
    if (UserTypeID == 8) {
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

        $("#datatable").dataTable().fnDraw();
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
        $("#datatable").dataTable().fnDraw();
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
                        obj.parentNode.innerHTML = $("#txtPopUpSettlementDate").val() + '<a title="edit" href="#" onclick="UpdateSettlementDate(this,' + $("#hdnJobId").val() + ',\'' + $("#txtPopUpSettlementDate").val() + '\');" class="edit sprite-img grid-edit-ic" style="text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
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

    $("#aSwitch").click(function () {
        window.location.href = '/Job/STCSubmission?IsKendoStcSubmissionPage=true';
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
                        obj.parentNode.innerHTML = '<lable class="clsLabelPVDSWH" >' + $("#txtPopUpPvdSwhCode").val() + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + $("#hdncode").val() + ',\'' + $("#txtPopUpPvdSwhCode").val() + '\');" class="edit sprite-img grid-edit-ic" style="text-decoration:none;margin-left:5px">&nbsp;&nbsp;</a>'
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

    $('#datatable').DataTable({
        iDisplayLength: GridConfig[0].PageSize,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },

        columns: [
            {
                'data': 'Id',
                "orderable": false,
                "render": function (data, type, full, meta) {
                    var gbBatchRecId = "";
                    if (full.GBBatchRECUploadId != null && full.GBBatchRECUploadId != "") {
                        gbBatchRecId = (full.GBBatchRECUploadId.split("-").length > 1 && full.GBBatchRECUploadId.split("-")[1] != "") ? full.GBBatchRECUploadId : full.GBBatchRECUploadId.split("-")[0];
                    }
                    if ($('#chkAll').prop('checked') == true) {
                        return '<input type="checkbox"  STCStatusId="' + full.STCStatusId + '" jobId="' + full.JobID + '" id="chk_' + full.STCJobDetailsID + '" term="' + full.STCSettlementTerm + '" custTerm="' + full.CustomSettlementTerm + '" IsInvoiced="' + full.IsInvoiced + '" STCInvoiceCount="' + full.STCInvoiceCount + '" IsCreditNote="' + full.IsCreditNote + '"  IsPayment="' + full.IsPayment + '" STCStatus="' + full.STCStatus + '" PartialValid="' + full.IsPartialValidForSTCInvoice + '" GBBatchRECUploadId="' + gbBatchRecId + '" IsSPVRequired="' + full.IsSPVRequired + '" IsSPVInstallationVerified="' + full.IsSPVInstallationVerified + '" IsRelease="' + full.IsRelease + '" STC="' + full.STC + '" checked >';
                    }
                    else {
                        return '<input type="checkbox"  STCStatusId="' + full.STCStatusId + '" jobId="' + full.JobID + '" id="chk_' + full.STCJobDetailsID + '" term="' + full.STCSettlementTerm + '" custTerm="' + full.CustomSettlementTerm + '" IsInvoiced="' + full.IsInvoiced + '" STCInvoiceCount="' + full.STCInvoiceCount + '" IsCreditNote="' + full.IsCreditNote + '" IsPayment="' + full.IsPayment + '" STCStatus="' + full.STCStatus + '" PartialValid="' + full.IsPartialValidForSTCInvoice + '" GBBatchRECUploadId="' + gbBatchRecId + '" IsSPVRequired="' + full.IsSPVRequired + '" IsSPVInstallationVerified="' + full.IsSPVInstallationVerified + '" IsRelease="' + full.IsRelease + '" STC="' + full.STC + '">';
                    }
                }
            },

            {
                'data': 'HasMultipleRecords',

                "render": function (data, type, full, meta) {
                    if (full.HasMultipleRecords) {
                        return '<i id="ticon_' + full.JobID + '" isKendo = false class="sprite-img grid-toggle-ic" style="cursor:pointer" onclick="GetSubRecords(' + full.JobID + ',this);"></i>';
                    }
                    else {
                        return '';
                    }
                }
            },
            {
                'data': 'IsSPVRequired',
                "render": function (data, type, full, meta) {
                    if (full.IsSPVRequired) {
                        return '<span style="font-weight: bold;color: Green;">Yes</span>';
                    }
                    else {
                        return '<span style="font-weight: bold;color: Red;">No</span>';
                    }
                },
                "orderable": false
            },

            {
                'data': 'IsSPVInstallationVerified',
                visible: (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5 || UserTypeID == 4) ? true : false,
                "render": function (data, type, full, meta) {
                    if (full.IsSPVInstallationVerified == null) {
                        return '<span style="font-weight: bold;">Not Yet Verified</span>';
                    }
                    else if (full.IsSPVInstallationVerified) {
                        return '<span style="font-weight: bold;color: Green;">Verified</span>';
                    }
                    else {
                        return '<span style="font-weight: bold;color: Red;">Verify Failed</span>';
                    }
                },
                "orderable": false
            },

            {
                'data': 'RefNumberOwnerName',
                "render": function (data, type, full, meta) {
                    if (full.RefNumberOwnerName != null) {
                        var url = urlJobIndex + "?id=" + full.Id;
                        return '<a href="' + url + '" style="text-decoration:none;" target="_blank">' + full.RefNumberOwnerName + '</a>'
                    }
                    else {
                        return '';
                    }
                },
            },

            { 'data': 'InstallationAddress' },

            {
                'data': 'STCStatus',
                "render": function (data, type, full, meta) {
                    if (full.STCStatusId == 19 && full.IsSPVInstallationVerified != null && !full.IsSPVInstallationVerified) {
                        //return '<lable class="clsSTCStatusLabel Installation-verified-false" data-JobID=' + full['JobID'] + ' data-JobType=' + full['JobTypeId'] + ' data-SerialNumbers=' + full['SerialNumbers'] + ' style="color:' + full.ColorCode + '">' + full.STCStatus + ' </lable> '
                        return '<lable class="clsSTCStatusLabel Installation-verified-false" data-JobID=' + full['JobID'] + ' data-JobType=' + full['JobType'] + ' style="color:' + full.ColorCode + '">' + full.STCStatus + ' </lable> '

                    }
                    else
                        //return '<lable class="clsSTCStatusLabel" data-JobID=' + full['JobID'] + ' data-JobType=' + full['JobType'] + ' data-SerialNumbers=' + escape(full['SerialNumbers']) + ' style="color:' + full.ColorCode + '">' + full.STCStatus + '</lable>'
                        return '<lable class="clsSTCStatusLabel" data-JobID=' + full['JobID'] + ' data-JobType=' + full['JobType'] + ' style="color:' + full.ColorCode + '">' + full.STCStatus + '</lable>'
                }
            },
            {
                'data': 'GBBatchRECUploadId',
                'visible': (UserTypeID == 1 || UserTypeID == 3) ? true : false,
                "render": function (data, type, full, meta) {
                    if (full.GBBatchRECUploadId != null && full.GBBatchRECUploadId != "") {
                        var imgDelete = 'text-decoration:none;margin-left:5px';
                        return '<lable>' + ((full.GBBatchRECUploadId.split("-").length > 1 && full.GBBatchRECUploadId.split("-")[1] != "") ? full.GBBatchRECUploadId : full.GBBatchRECUploadId.split("-")[0]) + '</lable>' + '<a title="delete" href="#" onclick="RemoveSelectedJobFromBatch(' + full['STCJobDetailsID'] + ');" class="delete sprite-img grid-delete-ic" style="' + imgDelete + '">&nbsp;&nbsp;</a>';
                    }
                    else
                        return '';
                }
            },
            {
                'data': 'strRECBulkUploadTimeDate',
                'visible': (UserTypeID == 1 || UserTypeID == 3) ? true : false,
                "render": function (data, type, full, meta) {
                    //return ConvertToDate(data);
                    //return ConvertToDateWithFormat(data, dateFormat);
                    return ToDate(full.strRECBulkUploadTimeDate);
                }
            },
            { 'data': 'SolarCompany' },
            {
                'data': 'PVDSWHCode',
                visible: (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5) ? true : false,
                "render": function (data, type, full, meta) {

                    //if (full.PVDSWHCode != null) {
                    //    var imgedit = 'background:url(../images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:5px';
                    //    var result = '<lable class="clsLabelPVDSWH">' + full.PVDSWHCode + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + full['STCJobDetailsID'] + ',\'' + full['PVDSWHCode'] + '\');" class="edit" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                    //    return result;
                    //}
                    //else {
                    //    var imgadd = 'background:url(../images/plus.png) no-repeat center; text-decoration:none;margin-left:5px';
                    //    var result = '<a class="img-border" title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + full['STCJobDetailsID'] + ',' + '\'\'' + ');" class="edit" style="' + imgadd + '">&nbsp;&nbsp;</a>';
                    //    return result;
                    //}
                    if (UserTypeID == 1 || UserTypeID == 3) {
                        if (full.PVDSWHCode != null) {
                            var imgedit = 'text-decoration:none;margin-left:5px';
                            var result = '<lable class="clsLabelPVDSWH">' + full.PVDSWHCode + '</lable>' + '<a title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + full['STCJobDetailsID'] + ',\'' + full['PVDSWHCode'] + '\');" class="edit sprite-img grid-edit-ic" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                            return result;
                        }
                        else {
                            var imgadd = 'background-position: -132px -383px;height: 16px;width: 16px!importanttext-decoration:none;margin-left:5px';
                            var result = '<a class="img-border sprite-img add_ic" title="edit" href="#" onclick="AddUpdatePVDSWHCode(this,' + full['STCJobDetailsID'] + ',' + '\'\'' + ');" class="edit" style="' + imgadd + '">&nbsp;&nbsp;</a>';
                            return result;
                        }
                    }
                    else {
                        if (full.PVDSWHCode != null) {
                            //var imgedit = 'background:url(../images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:5px';
                            var result = '<lable class="clsLabelPVDSWH">' + full.PVDSWHCode + '</lable>';
                            return result;
                        }
                        else {
                            var result = "";
                            return result;
                        }
                    }
                }
            },

            {
                'data': 'STC', "sClass": "dt-right",
                "render": function (data, type, full, meta) {
                    //return PrintDecimal(full.STC);
                    return '<lable class="clsLabel">' + parseInt(full.STC) + '</lable>';
                }
            },

            {
                'data': 'STCPrice',
                visible: isWholeSalers == "true" ? false : true,
                "render": function (data, type, full, meta) {

                    if (full.STCSettlementTerm == 4 && full.IsInvoiced == false) {
                        return '';
                    }
                    else if ((full.STCSettlementTerm == 11 || (full.STCSettlementTerm == 10 && full.CustomSettlementTerm == 11)) && full.IsInvoiced == false) {
                        return "";
                    }
                    else {
                        if (full.IsGst) {
                            return '<lable class="lblSTcPrice">' + '$' + parseFloat(data).toFixed(2) + ' +GST' + '</lable>';
                        }
                        else {
                            return '<lable class="lblSTcPrice">' + '$' + parseFloat(data).toFixed(2) + '</lable>';
                        }
                    }

                }
            },

            {
                'data': 'STCSettlementTerm',
                "render": function (data, type, full, meta) {

                    if (full.STCSettlementTerm == 1) {
                        return '24 Hour';
                    }
                    else if (full.STCSettlementTerm == 2) {
                        return '3 Days';
                    }
                    else if (full.STCSettlementTerm == 3) {
                        return '7 Days';
                    }
                    else if (full.STCSettlementTerm == 4) {
                        return 'CER Approved';
                    }
                    else if (full.STCSettlementTerm == 5) {
                        return 'Partial Payment';
                    }
                    else if (full.STCSettlementTerm == 6) {
                        return 'Upfront';
                    }
                    else if (full.STCSettlementTerm == 7) {
                        return 'Rapid-Pay';
                    }
                    else if (full.STCSettlementTerm == 8) {
                        return 'Opti-Pay';
                    }
                    else if (full.STCSettlementTerm == 9) {
                        return 'Commercial';
                    }
                    else if (full.STCSettlementTerm == 10) {
                        return full.CustomTermLabel;
                    }
                    else if (full.STCSettlementTerm == 11) {
                        return "Invoice Stc";
                    }
                    else if (full.STCSettlementTerm == 12) {
                        return "Peak Pay";
                    }
                    else {
                        return '';
                    }
                },
            },

            {
                'data': 'STCSubmissionDate',
                "render": function (data, type, full, meta) {
                    //return ConvertToDate(data);
                    //return ConvertToDateWithFormat(data, dateFormat);
                    return ToDate(full.strSTCSubmissionDate)
                }
            },

            {
                'data': 'STCSettlementDate',
                "render": function (data, type, full, meta) {
                    if (full.STCSettlementTerm != 5) {
                        if (full.strSTCSettlementDate != null) {
                            var sdate = ToDate(full.strSTCSettlementDate);
                            //var sdate = ToDate(data);
                            var imgedit = 'text-decoration:none;margin-left:5px';
                            var result;
                            if (UserTypeID == 1 || UserTypeID == 3)
                                result = '<lable class="clsDateLabel">' + sdate + '</lable>' + '<a title="edit" href="#" onclick="UpdateSettlementDate(this,' + full['STCJobDetailsID'] + ',\'' + sdate + '\');" class="edit sprite-img grid-edit-ic" style="' + imgedit + '">&nbsp;&nbsp;</a>';
                            else
                                result = '<lable class="clsDateLabel">' + sdate + '</lable>';
                            return result;
                        }
                        else {
                            if (full.STCSettlementTerm == 11 || (full.STCSettlementTerm == 10 && full.CustomSettlementTerm == 11))
                                return "";
                            else
                                return '<lable class="clsDateLabel">On Approval</lable>';
                        }
                    }
                    else if (full.STCSettlementTerm == 5) {
                        if (full.strSTCSettlementDate != null) {
                            var sdate = ToDate(full.strSTCSettlementDate);
                            //var sdate = ToDate(data);
                            return '<lable class="clsDateLabel">' + sdate + '(Partial)</lable>';
                        }
                        else {
                            return '<lable class="clsDateLabel">(Partial)</lable>';
                        }
                    }
                }
            },

            { 'data': 'ComplianceOfficer' },

            {
                'data': 'IsInvoiced',
                "render": function (data, type, full, meta) {
                    if (full.IsInvoiced) {
                        return '<span class="sprite-img file-green-ic"></span>';
                    }
                    else if (full.STCSettlementTerm == 5 && full.STCInvoiceCount == 1)
                        return '<span class="sprite-img file-orange-ic"></span>';
                    else {
                        return '<span class="sprite-img file-red-ic"></span>';
                    }
                },
                "orderable": false
            },

            {
                'data': 'Id',
                "render": function (data, type, full, meta) {
                    imgassign = 'text-decoration:none;';
                    if (UserTypeID == 4 || UserTypeID == 6) {
                        var assignHref = "javascript:LoadStc('" + full.JobID + "','" + full.STCJobDetailsID + "')";
                    }
                    else {
                        var assignHref = "javascript:StcCompliance('this','" + full.JobID + "','" + full.STCJobDetailsID + "')";
                    }
                    var returnHTML = '';

                    returnHTML += '<a href="' + assignHref + '" class="action edit sprite-img compliance-ic" style="' + imgassign + '" title="Compliance Check">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';

                    return returnHTML;
                },
                "orderable": false
            },


        ],

        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        initComplete: function (settings, json) {
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

        },

        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).addClass('togglerow');
            if (aData.HasMultipleRecords) {
                $(nRow).addClass('togglerow').find('.toggle-icon').parent().addClass('toggle-btn');
            }
        },

        bServerSide: true,
        sAjaxSource: urlGetJobSTCSubmission,

        "fnDrawCallback": function () {

            $("#btotalTradedSTC").html(0);

            if ($('#chkAll').prop('checked') == true) {

                chkCount = $('#datatable >tbody').find('tr.togglerow').length;
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
                chkCount = 0;
                $("#btotalTradedSTC").html(0);
                $("#btotalAmount").html(0);
                $("#btotalNoofJobs").text(0);
            }
            $(".Installation-verified-false").each(function (k, data) {
                $(this).closest('tr').addClass("installation-notverified");
            })
            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length > 1) {
                $("#datatable_length").show();
            } else if ($("#datatable").find("tr.togglerow").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            var info = table.page.info();
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> Job(s) found';
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody').find('tr.togglerow').length + '</b>  of  <b>' + info.recordsTotal + '</b> Job(s) found';
            }
            //------------------------------------------------------------------------------------------------------------------------------
        },

        "fnServerParams": function (aoData) {
            aoData.push({ "name": "stageid", "value": SelectedStageId });
            if (UserTypeID == 1 || UserTypeID == 3) {
                aoData.push({ "name": "reseller", "value": document.getElementById("ResellerId").value });
            }
            if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
                //aoData.push({ "name": "solarcompanyid", "value": document.getElementById("SolarCompanyId").value });
                aoData.push({ "name": "solarcompanyid", "value": $('#hdnsolarCompanyid').val() });
                aoData.push({ "name": "pvdswhcode", "value": $("#txtPVDSWHCode").val() });
            }
            if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2) {
                aoData.push({ "name": "RAM", "value": document.getElementById("RamId").value });
            }
            if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
                aoData.push({ "name": "complianceOfficerId", "value": document.getElementById("ComplianceOfficerId").value });
            }
            aoData.push({ "name": "RefJobId", "value": $("#txtRefJobId").val() });
            aoData.push({ "name": "ownername", "value": $("#txtOwnerName").val() });
            aoData.push({ "name": "installationaddress", "value": $("#txtInstallationAddress").val() });
            aoData.push({ "name": "submissionfromdate", "value": GetDates($("#txtSubmissionFromDate").val()) });
            aoData.push({ "name": "submissiontodate", "value": GetDates($("#txtSubmissionToDate").val()) });
            aoData.push({ "name": "settlementfromdate", "value": GetDates($("#txtSettlementFromDate").val()) });
            aoData.push({ "name": "settlementtodate", "value": GetDates($("#txtSettlementToDate").val()) });
            aoData.push({ "name": "IsInvoiced", "value": invoiced });
            aoData.push({ "name": "isAllScaJobView", "value": isAllScaJobView });
            aoData.push({ "name": "isShowOnlyAssignJobsSCO", "value": isShowOnlyAssignJobsSCO });
            aoData.push({ "name": "SettlementTermId", "value": $("#SettlementTermId").val() });
            ////aoData.push({ "name": "panelinverterdetails", "value": $("#txtPanelInverterDetails").val() });
            aoData.push({ "name": "isSPVRequired", "value": $("#IsSPVRequired").val() });
            aoData.push({ "name": "isSPVInstallationVerified", "value": $("#IsSPVInstallationVerified").val() });
        }
    });

    var table = $('#datatable').DataTable();
    $('#datatable').on('length.dt', function (e, settings, len) {
        SetPageSizeForGrid(len, false, ViewPageId)
        GridConfig[0].PageSize = len;
        GridConfig[0].IsKendoGrid = false;
        GridConfig[0].ViewPageId = parseInt(ViewPageId);
    });
    $('#chkAll').on('click', function () {
        var rows = table.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
        chkCount = this.checked ? $('#datatable >tbody').find('tr.togglerow').length : 0;

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

    $('#datatable tbody').on('change', 'input[type="checkbox"]', function () {

        if (this.checked) {
            chkCount++;
            if (chkCount == $('#datatable >tbody').find('tr.togglerow').length) {
                $('#chkAll').prop('checked', this.checked);
            }
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            var stcprice = ($(this).parent().parent().find('.lblSTcPrice').text() == '' || $(this).parent().parent().find('.lblSTcPrice').text() == undefined) ? 0 : $(this).parent().parent().find('.lblSTcPrice').text();
            var amount = (parseFloat(parseFloat($("#btotalAmount").html()) + parseFloat(cSTC * parseFloat(stcprice.replace("$", "")))).toFixed(2))
            $("#btotalAmount").html(amount);
            $("#btotalTradedSTC").html(parseFloat(parseFloat(cSTC) + parseFloat(tSTC)).toFixed(2));
            $("#btotalNoofJobs").text(chkCount);
        }
        else {
            chkCount--;
            $('#chkAll').prop('checked', this.checked);
            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            var stcprice = ($(this).parent().parent().find('.lblSTcPrice').text() == '' || $(this).parent().parent().find('.lblSTcPrice').text() == undefined) ? 0 : $(this).parent().parent().find('.lblSTcPrice').text();
            var amount = (parseFloat(parseFloat($("#btotalAmount").html()) - parseFloat(cSTC * parseFloat(stcprice.replace("$", "")))).toFixed(2))
            $("#btotalAmount").html(amount);
            $("#btotalTradedSTC").html(parseFloat(parseFloat(tSTC) - parseFloat(cSTC)).toFixed(2));
            $("#btotalNoofJobs").text(chkCount);
        }
    });

});

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
                if (IsComplianceOfficerExist == false && fco.Value == localStorage.getItem('STCSubmission_ComplianceOfficerId')) {
                    IsComplianceOfficerExist = true;
                }
            });


            if (IsComplianceOfficerExist) {
                document.getElementById("ComplianceOfficerId").value = localStorage.getItem('STCSubmission_ComplianceOfficerId')
            }
            else {
                $("#ComplianceOfficerId").val($("#ComplianceOfficerId option:first").val());
                localStorage.setItem('STCSubmission_ComplianceOfficerId', $("#ComplianceOfficerId option:first").val());
            }

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
        //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
        $('#hdnsolarCompanyid').val('0');
        $("#searchSolarCompany").val('All');
        //localStorage.setItem('STCSubmission_SolarCompanyId',document.getElementById("SolarCompanyId").value);
        localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
        localStorage.setItem('STCSubmission_PVDSWHCode', '');
        //$("#SolarCompanyId").val($("#SolarCompanyId option:first").val());
        //$("#txtPVDSWHCode").val('');
    }
    if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
        document.getElementById("ComplianceOfficerId").selectedIndex = 0;
        localStorage.setItem('STCSubmission_ComplianceOfficerId', document.getElementById("ComplianceOfficerId").value);
    }

    localStorage.setItem('StaticSTCSubmission_RefJobId', '');
    localStorage.setItem('STCSubmission_OwnerName', '');
    localStorage.setItem('STCSubmission_InstallationAddress', '');

    //$("#txtRefJobId").val('');
    //$("#txtOwnerName").val('');
    //$("#txtInstallationAddress").val('');

    $("#txtSubmissionFromDate").val('');
    $("#txtSubmissionToDate").val('');
    $("#txtSettlementFromDate").val('');
    $("#txtSettlementToDate").val('');
    ////$("#txtPanelInverterDetails").val('');
    $("#IsSPVRequired").val('All');
    // $("#IsSPVInstallationVerified").val('All');
    document.getElementById("IsSPVInstallationVerified").selectedIndex = 0;
    document.getElementById("SettlementTermId").selectedIndex = 0;
    localStorage.setItem('STCSubmission_STCSettlementTerm', document.getElementById("SettlementTermId").value);
    document.getElementById('chkNotInvoiced').checked = true;
    document.getElementById('chkHasBeenInvoiced').checked = true;
    invoiced = 0;

    SetParamsFromLocalStorage();
    //GetSTCJobStageCount();
    $("#datatable").dataTable().fnDraw();
    //GetRecFailureReason($("#ResellerId").val());
}

function SearchJobSTCRecords() {
    if (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('STCSubmission_ComplianceOfficerId', document.getElementById("ComplianceOfficerId").value);
    }
    if (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5) {
        //localStorage.setItem('STCSubmission_SolarCompanyId',document.getElementById("SolarCompanyId").value);
        localStorage.setItem('STCSubmission_SolarCompanyId', $('#hdnsolarCompanyid').val());
        localStorage.setItem('STCSubmission_PVDSWHCode', $("#txtPVDSWHCode").val());
    }

    localStorage.setItem('StaticSTCSubmission_RefJobId', $("#txtRefJobId").val());
    localStorage.setItem('STCSubmission_OwnerName', $("#txtOwnerName").val());
    localStorage.setItem('STCSubmission_InstallationAddress', $("#txtInstallationAddress").val());
    localStorage.setItem('STCSubmission_STCSettlementTerm', document.getElementById("SettlementTermId").value);

    //GetSTCJobStageCount();
    $("#datatable").dataTable().fnDraw();
    //GetRecFailureReason($("#ResellerId").val());
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
                $(".table-responsive .hidden-row").remove();
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
        $(".table-responsive .edit-panel").removeClass('edit-panel');
    }
    return false;
}

function SetStageId(id) {
    //var a = document.getElementById("jobstage_" + SelectedStageId);
    //a.style.backgroundColor = "#f7f7f7";
    $(".filters-row > ul > li > a").css("background", "#f7f7f7");

    SelectedStageId = id;

    var a = document.getElementById("jobstage_" + SelectedStageId);
    a.style.backgroundColor = "#686868";

    //if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
    //    localStorage.setItem('STCSubmission_PVDSWHCode', '');
    //}
    //if (UserTypeID == 1 || UserTypeID == 5 || UserTypeID == 2) {
    //    document.getElementById("ComplianceOfficerId").selectedIndex = 0;
    //    localStorage.setItem('STCSubmission_ComplianceOfficerId', document.getElementById("ComplianceOfficerId").value);
    //}

    //localStorage.setItem('StaticSTCSubmission_RefJobId', '');
    //localStorage.setItem('STCSubmission_OwnerName', '');
    //localStorage.setItem('STCSubmission_InstallationAddress', '');

    //$("#txtSubmissionFromDate").val('');
    //$("#txtSubmissionToDate").val('');
    //$("#txtSettlementFromDate").val('');
    //$("#txtSettlementToDate").val('');
    //document.getElementById("SettlementTermId").selectedIndex = 0;
    //localStorage.setItem('STCSubmission_STCSettlementTerm', document.getElementById("SettlementTermId").value);
    //document.getElementById('chkNotInvoiced').checked = true;
    //document.getElementById('chkHasBeenInvoiced').checked = true;
    //invoiced = 0;

    SetParamsFromLocalStorage();

    GetSTCJobStageCount();
    $("#datatable").dataTable().fnDraw();
    //GetRecFailureReason($("#ResellerId").val());
}

function GetSTCJobStageCount() {
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
        async: false,
        data: { reseller: resellerId, ram: ramId, sId: solarcompanyId, isAllScaJobView: isAllScaJobView, isShowOnlyAssignJobsSCO: isShowOnlyAssignJobsSCO },
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
        data: JSON.stringify({ complianceOfficerID: complianceOfficerId, jobs: selectedRows.join(',') }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Jobs assigned to compliance officer successfully.");
                $("#errorMsgRegion").show();
                GetSTCJobStageCount();
                $("#datatable").dataTable().fnDraw();
                //GetRecFailureReason($("#ResellerId").val());
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
            var invoiceCount = $(this).attr('STCInvoiceCount');

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
                        $("#datatable").dataTable().fnDraw();
                        //GetRecFailureReason($("#ResellerId").val());
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
                if (STCStatus != '' && STCStatus != undefined && STCStatus.trim() == 'Ready To Create') {

                    var STCJobDetailsID = $(this).attr('id').substring(4);
                    var STCTerm = $(this).attr('term');
                    var custSTCTerm = $(this).attr('custterm');
                    var IsInvoiced = $(this).attr('IsInvoiced');
                    var jobid = $(this).attr("JobId");
                    var GBBatchRecId = ($(this).attr('gbbatchrecuploadid') == 'undefined' || $(this).attr('gbbatchrecuploadid') == '') ? '' : '_' + $(this).attr('gbbatchrecuploadid');

                    var isSpvRequired = $(this).attr("IsSPVRequired");
                    var isSPVInstallationVerified = $(this).attr("IsSPVInstallationVerified");

                    if ( isSpvRequired.toLowerCase() == "true" && (isSPVInstallationVerified.toLowerCase() == "false" || isSPVInstallationVerified == 'null')) {
                        alert("Installation verification not done yet.");
                        IsValid = false;
                        return false;
                    }
                    // if record should not have STCInvoice and dont have settlement term (On approval) for creating STC invoice.
                    if (IsInvoiced.toLowerCase() == "false" && STCTerm != 4 && STCTerm != 8 && STCTerm != 12 && (STCTerm != 10 && (custSTCTerm != 4 || custSTCTerm != 8 || custSTCTerm != 12))) {
                        selectedRows.push(STCJobDetailsID + '_' + STCTerm + '_' + jobid);
                    }

                    //var serialNumbers = unescape($(this).parent().parent().find('.clsSTCStatusLabel').attr("data-SerialNumbers"));
                    //selectedRows_SerialNumbers.push(serialNumbers);

                    var selectedJobID = $(this).parent().parent().find('.clsSTCStatusLabel').attr("data-JobID");
                    selectedRows_JobIDs.push(selectedJobID + GBBatchRecId);
                    selectedJobIds.push(selectedJobID);

                    //selectedRows_term_isInvoiced.push({STCJobDetailsID: STCJobDetailsID, STCSettlementTerm:STCTerm,IsInvoiced:IsInvoiced});

                }
                else {
                    alert("Can't generate REC Registry for selected STC records.");
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
                    if (data.status.toString() == "false" && data.lstJobIds.length>0) {
                        $(".alert").hide();
                        $("#errorMsgRegion").removeClass("alert-success");
                        $("#errorMsgRegion").addClass("alert-danger");
                        $("#errorMsgRegion").html(closeButton + "[" + data.strlstJobIds+"]" +" Jobs are already In Progress or SuccessFully Uploaded on REC.");
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
                SetParamsFromLocalStorage();
                $("#datatable").dataTable().fnDraw();
                GetSTCJobStageCount();
            }, 1000);
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
    if (data != null && data != '') {
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
    if (UserTypeID == 1 || UserTypeID == 2 || UserTypeID == 3 || UserTypeID == 5) {
        $("#txtPVDSWHCode").val(localStorage.getItem('STCSubmission_PVDSWHCode'));
    }
    $("#txtRefJobId").val(localStorage.getItem('StaticSTCSubmission_RefJobId'));
    $("#txtOwnerName").val(localStorage.getItem('STCSubmission_OwnerName'));
    $("#txtInstallationAddress").val(localStorage.getItem('STCSubmission_InstallationAddress'));
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
        var selectedrowsJobids = [];
        var CheckSPVrequireddata = [];

        var IsValid = true;
        var IsPartialValid = true;
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
                        selectedrowsJobids.push($(this).attr('jobid'));
                    }
                    else {
                        selectedRows.push($(this).attr('id').substring(4));
                        selectedrowsJobids.push($(this).attr('jobid'));
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
                    if (result) {
                        $.ajax({
                            type: 'POST',
                            url: urlBulkChangeSTCJobStage,
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify({ stcjobstage: document.getElementById('STCJobStageID').value, stcjobids: selectedRows.join(','), checkSPVrequired: CheckSPVrequireddata }),

                            success: function (data) {
                                if (data.success) {
                                    $(".alert").hide();
                                    $("#errorMsgRegion").removeClass("alert-danger");
                                    $("#errorMsgRegion").addClass("alert-success");
                                    $("#errorMsgRegion").html(closeButton + "Job Stages updated successfully.");
                                    $("#errorMsgRegion").show();
                                    GetSTCJobStageCount();
                                    $("#datatable").dataTable().fnDraw();
                                    //GetRecFailureReason($("#ResellerId").val());
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
    button.disabled = true;
    selectedRows = [];
    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select atleast one Job for Installation Verify.');
        button.disabled = false;
        return false;
    }
    else {
        var OnlySelectSpvAllowJobs = true;
        var stcstatusreadytocreate = true;
        //$('#datatable tbody input[type="checkbox"]:checked').each(function (text, value) {
        //    if (!IsSpvAllowOrNot(value)) {
        //        OnlySelectSpvAllowJobs = false;
        //    }
        //});
        //if (OnlySelectSpvAllowJobs == false) {
        //    alert("Please select those jobs which allows SPV.");
        //    return false;
        //}


        $('#datatable tbody input[type="checkbox"]:checked').each(function (text, value) {
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
                                    $("#datatable").dataTable().fnDraw();
                                    document.getElementById('STCJobStageID').selectedIndex = 0;
                                }
                            },
                            error: function (ex) {
                                button.disabled = false;
                            }
                        });
                    }
                    if (IsSPVRequired != "true" || IsSPVInstallationVerified == "true") {
                        alert("Selected job must have is spv required true and spv installation verify not true.");
                        flage = false;
                        button.disabled = false;
                        return false;
                    }

                }
                else {
                    alert("STC status must be 'Ready To Create'.");
                    flage = false;
                    button.disabled = false;
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
    //var isUpdateJobStatusFromRec = isSwitch ? false : true;
    //isAutoUpdateCEROn ? $('.btnUpdateJobStatusFromREC').hide() : $('.btnUpdateJobStatusFromREC').show();
    //showHideBtnUpdateJobStatusFromREC(isAutoUpdateCEROn);
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
                    $("#datatable").dataTable().fnDraw();
                    //GetRecFailureReason($("#ResellerId").val());
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
    if (data != null && data != '') {
        var tickStartDate = ConvertDateToTick(data, 'dd/mm/yyyy');
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        var date = new Date(tDate);
        //console.log(moment(date).format('@FormBot.Helper.ProjectConfiguration.GetDateFormat.ToUpper()'));
        //console.log(date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2));
        //return date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2);
        return moment(date).format(dateFormat.toUpperCase());
    }
    else {
        return '';
    }
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

//function RemoveJobFromBatch() {
//    var id = "";
//    $("#rec-failure-reason .collection-item.active:not(.selectAll)").each(function (k, data) {
//        id = id + $(this).data("stcjobdetailsid") + ","
//    })
//    if (id == "") {
//        alert("Please select jobs")
//        return;
//    }
//    RemoveSelectedJobFromBatch(id.replace(/,\s*$/, ""));
//}

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
                else
                    $("#datatable").dataTable().fnDraw();
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
        //var OnlySelectSpvAllowJobs = true;
        //$('#datatable tbody input[type="checkbox"]:checked').each(function (text, value) {
        //    if (!IsSpvAllowOrNot(value)) {
        //        OnlySelectSpvAllowJobs = false;
        //    }
        //});
        //if (OnlySelectSpvAllowJobs == false) {
        //    alert("Please select those jobs which allows SPV.");
        //    return false;
        //}
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
        console.log(selectedRows);
        console.log(selectedRows_JobIDs);
        var result = confirm("Are you sure you want to release spv of selected jobs ? ");
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
                        $("#errorMsgRegion").html(closeButton + "Spv release of selected job done successfully.");
                        $("#errorMsgRegion").show();
                        // GetSTCJobStageCount();
                        $("#datatable").dataTable().fnDraw();
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

    selectedRows = [];
    var selectedRows_JobIDs = [];
    var flage = true;
    if ($('#datatable tbody input:checked').length == 0) {
        alert('Please select atleast one Job for spv reset.');
        return false;
    }
    else {
        ////var OnlySelectSpvAllowJobs = true;
        ////$('#datatable tbody input[type="checkbox"]:checked').each(function (text, value) {
        ////    if (!IsSpvAllowOrNot(value)) {
        ////        OnlySelectSpvAllowJobs = false;
        ////    }
        ////});
        //if (OnlySelectSpvAllowJobs == false) {
        //    alert("Please select those jobs which allows SPV.");
        //    return false;
        //}
        $('#datatable tbody input[type="checkbox"]:checked').each(function () {
            if ($(this).prop('checked') == true) {
                // var STCStatus = $(this).parent().parent().find('.clsSTCStatusLabel').text();
                STCStatus = $(this).attr('STCStatus');
                IsRelease = $(this).attr('IsRelease');
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
        console.log(selectedRows);
        console.log(selectedRows_JobIDs);
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
                        $("#errorMsgRegion").html(closeButton + "Spv reset of selected job done successfully.");
                        $("#errorMsgRegion").show();
                        //  GetSTCJobStageCount();
                        $("#datatable").dataTable().fnDraw();
                    }
                },
                error: function (ex) {

                }
            });
        }
    }
}

function searchSpvFailureReasonByJobIdAndRefNumber() {
    var divSearch = "#accordionExample .panel";
    // Declare variables
    var input, filter, a, i, panelsLength, spvfailerreasonpanel, result = 0;
    input = document.getElementById('txtSpvRefJobId');
    filter = input.value.toUpperCase();
    panelsLength = $(divSearch).length;

    if (panelsLength > 0) {
        // Loop through all list items, and hide those who don't match the search query
        for (i = 0; i < panelsLength; i++) {
            spvfailerreasonpanel = $(divSearch + ":eq(" + i + ")");
            a = $(spvfailerreasonpanel).find(".spv-search:eq(0)");
            spvsearchjobid = a.attr("data-jobid");
            spvsearchrefnumber = a.attr("data-refnumber");

            if (spvsearchjobid.toUpperCase().indexOf(filter) > -1
                || spvsearchrefnumber.toUpperCase().indexOf(filter) > -1) {
                result = 1;
                $(spvfailerreasonpanel).css("display", "");
            } else {
                $(spvfailerreasonpanel).css("display", "none");
            }
        }
        if (result == 0) {
            $("#accordionExample").hide();
            $(".no-record-found").show();
        }
        else {
            $("#accordionExample").show();
            $(".no-record-found").hide();
        }
    }
    else {
        $("#accordionExample").hide();
        $(".no-record-found").show();
    }
}