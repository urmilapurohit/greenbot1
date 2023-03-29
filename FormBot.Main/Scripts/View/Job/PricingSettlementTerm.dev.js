var STCRemark = "";
function PricingSettlementTermOnLoad() {
    if ((IsShowInDashboard == 'False' || IsTradedFromJobIndex == 'True')
        && (IsSubmissionScreen != 1)
        && (currentJobStatus != 0)
        && ((session_UserTypeId == 1 || session_UserTypeId == 3)
            ||
            (
                (session_UserTypeId == 2 || session_UserTypeId == 4 || session_UserTypeId == 5 || (session_UserTypeId == 8 && showbtn == 'True'))
                &&
        ((currentJobStatus == 12) || (currentJobStatus == 17) || (currentJobStatus == 14) || ($("#JobInstallationDetails_InstallingNewPanel").val() != "New" && $("#JobInstallationDetails_InstallingNewPanel").val() != "") || ($("#JobInstallationDetails_InstallingNewPanel").val() == "New" && (!isJobWithCommonInstallationAddress)))
            )
        )
    ) {
        EnableTradeButton();
    }
    else {
        DisableTradeButton();
    }

    if ((($('#IsWholeSaler').val() && $("#IsWholeSaler").val().toLowerCase() == 'true') || (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "true")) && isWholesalerSCAPricingSettlementTermView == 'false') {
        $(".SettlementTerms").hide();
        $("#calculatedSTCParent").hide();
        $(".total-amonut").hide();
        $(".stc-amount").hide();
        $(".SettlementTermsCERApproved").hide();
    }
    else {
        $(".SettlementTerms").show();
        $("#calculatedSTCParent").show();
        $(".total-amonut").show();
        $(".stc-amount").show();
        $(".SettlementTermsCERApproved").show();
    }
    if (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "true" && IsForDashboardPricingWholesaler.toLowerCase() == "true") {
        $(".SettlementTerms").show();
        $("#calculatedSTCParent").show();
        $(".total-amonut").show();
        $(".stc-amount").show();
        $(".SettlementTermsCERApproved").show();
    }
    if (IsShowInDashboard == 'True') {
        $("#calculatedSTCParent").hide();
    }
    if ((session_UserTypeId != 1 && session_UserTypeId != 3) && (model_STCStatus != 10 && model_STCStatus != 12 && model_STCStatus != 14 && model_STCStatus != 17)) {
        $('#PricingBlock').hide();
        if (typeof (TabularView) != 'undefined') {
            $('.subTitleSTCPricing').hide();
        }
    }

    if (statucId != 12 && statucId != 17) {
        if (session_UserTypeId != 1) {
            $("ul.Processing-time li.LiSettlement").addClass("cursorDefault");
            $("ul.Processing-time li.LiSettlement").unbind("click");
        }
    }
    if (IsSubmissionScreen != 1 && model_IsDashboardPricing == 'false') {
        EnableDisableSettlementTerm();
    }

    $(".LiSettlement").each(function () {
        $(this).click(function () {
            SettlementTermClick($(this));
        });
        if ($(this).data('settlementterm') == settlementTerm)
            $(this).click();

        if (IsShowInDashboard == 'True') {
            if ($(this).attr("data-ispriceup") == 'True') {
                $(this).addClass('up-caret');
            }
            else {
                $(this).addClass('down-caret');
            }
        }
    });

    if (settlementTerm == "0")
        $(".LiSettlement:first").click();
    if (model_IsGridView == 'false') {
        x = document.createElement('div');
        x.innerHTML = Description;
        $("#lblDescription").html($(x).html());
        $("#lblLastUpdatedDate").html(lastUpdatedDate);
    }

    var visibleLiCount = $('.Processing-time').find('li').length;
    var liWidth = 0;
    if (visibleLiCount > 0) {
        liWidth = 100 / visibleLiCount + "%";
    }
    $('.Processing-time').find('li').css("width", liWidth);
}

function SettlementTermClick(obj) {
    var term = obj.attr('data-settlementterm');
    var custTerm = obj.children('#CustomSettlementTerm').val() > 0 ? obj.children('#CustomSettlementTerm').val() : 0;
    $(".LiSettlement").removeClass("active");
    if (obj.children('.ui-state-disabled').length > 0) {
        obj.parent().children('li').children("span:not(.ui-state-disabled)").parent('li:first').addClass('active');
    }
    else {
        obj.addClass("active");
    }

    if (model_STCStatus == 10 || model_STCStatus == 12 || model_STCStatus == 14 || model_STCStatus == 17) {
        calculateTotal(obj);
    }
}
function TradeSTCCheckRemarkandProceed() {
    debugger;
    STCRemark = $('#txtstcremark').val();
    if (STCRemark == null || STCRemark == "") {
        $('#errormsgtxtstcremark').show();
    }
    else {
        $('#errormsgtxtstcremark').hide();
        $('#popupstcRemark').modal('toggle');
        ApplyTradeStc();
    }
}
function ApplyTradeForStc() {
    debugger;
    if ((($("#JobInstallationDetails_InstallingNewPanel").val() != "New" || $("#JobInstallationDetails_InstallingNewPanel").val() == "") && (isJobWithCommonInstallationAddress))) {
        $('#popupstcRemark').modal({ backdrop: 'static', keyboard: false });

    }
    else {
        ApplyTradeStc();
    }
}
function ApplyTradeStc() {
    debugger;
    var ownersignature = $("#imgOwnerSign").attr('src');
    if (ownersignature == null) {
        alert("Please Upload Owner Signature To Trade STC");
        return;
    }
    var interval = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("interval") : $(".LiSettlement").data("interval");
    var price = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("price") : $(".LiSettlement").data("price");
    var settlementterm = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("settlementterm") : $(".LiSettlement").data("settlementterm");

    if (settlementterm == 12) {
        peakPayTimePeriodValue = peakPayTimePeriod;
        peakPayGstRate = PeakPayGst;
        peakPayFeeValue = PeakPayFee;
    }
    if (settlementterm == 10 && customSettlementTerm == 12) {
        peakPayTimePeriodValue = peakPayCustomTimePeriod;
        peakPayGstRate = CustomPeakPayGst;
        peakPayFeeValue = CustomPeakPayFee;
    }

    if (BasicDetails_IsWholeSaler && BasicDetails_IsWholeSaler.toLowerCase() == "false" && $(".LiSettlement.active").length == 0) {
        alert('Please select atleast one settlement term.');
        return;
    }
    else if ((BasicDetails_IsWholeSaler.toLowerCase() == "true" && $('.Processing-time').find('.LiSettlement').length == 1) || BasicDetails_IsWholeSaler.toLowerCase() == "false") {
        if (model_IsGridView == 'true') {
            var IsSamePrice = true;
            var IsFirst = true;
            var PriceDay1 = 0;
            var PriceDay3 = 0;
            var PriceDay7 = 0;
            var UpFront = 0;
            var PriceOnApproval = 0;
            var PartialPayment = 0;
            var RapidPay = 0;
            var OptiPay = 0;
            var Commercial = 0;
            var Custom = 0;
            var InvoiceStc = 0;
            var PeakPay = 0;
            var isReadyforTrade = true;

            var newselectedRows = [];
            $('#datatable1 tbody input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') == true) {
                    if (IsShowInDashboard == 'True') {
                        if ($(this).attr('IsTraded') != 'true' && $(this).attr('IsCustomPrice') != 'true') {
                            if ($(this).attr('IsReadyToTrade') == 'true') {
                                if (IsFirst) {
                                    PriceDay1 = parseFloat($(this).attr('PriceDay1'));
                                    PriceDay3 = parseFloat($(this).attr('PriceDay3'));
                                    PriceDay7 = parseFloat($(this).attr('PriceDay7'));
                                    PriceOnApproval = parseFloat($(this).attr('PriceOnApproval'));
                                    RapidPay = parseFloat($(this).attr('RapidPay'));
                                    OptiPay = parseFloat($(this).attr('OptiPay'));
                                    Commercial = parseFloat($(this).attr('Commercial'));
                                    Custom = parseFloat($(this).attr('Custom'));
                                    InvoiceStc = parseFloat($(this).attr('InvoiceStc'));
                                    PeakPay = parseFloat($(this).attr('PeakPay'));
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                else if (PriceDay1 != parseFloat($(this).attr('PriceDay1')) || PriceDay3 != parseFloat($(this).attr('PriceDay3')) || PriceDay7 != parseFloat($(this).attr('PriceDay7'))
                                    || PriceOnApproval != parseFloat($(this).attr('PriceOnApproval'))
                                    || RapidPay != parseFloat($(this).attr('RapidPay')) || OptiPay != parseFloat($(this).attr('OptiPay')) || Commercial != parseFloat($(this).attr('Commercial'))
                                    || Custom != parseFloat($(this).attr('Custom')) || InvoiceStc != parseFloat($(this).attr('InvoiceStc')) || PeakPay != parseFloat($(this).attr('PeakPay'))) {
                                    IsSamePrice = false;
                                    isReadyforTrade = false;
                                    alert('Please select jobs which have same price.');
                                    return false;
                                }
                                else {
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                IsFirst = false;
                            }
                            else {
                                IsSamePrice = false;
                                isReadyforTrade = false;
                                alert('Please select jobs which are ready to trade.');
                                return false;
                            }
                        }
                        else {
                            IsSamePrice = false;
                            isReadyforTrade = false;
                            alert('Please select jobs which are neither traded nor have custom price.');
                            return false;
                        }

                    }
                    else {
                        if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $(this).attr('isApproachingExpiryDate') == 'true') {
                            alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
                            isReadyforTrade = false;
                            return false;
                        }
                        else {
                            newselectedRows.push($(this).attr('JobId'));
                        }
                    }
                }
            });

            if (isReadyforTrade == true) {
                if (newselectedRows.length > 0) {
                    if (confirm("Are you sure you want to Trade STC?")) {
                        ApplyTradeActual(newselectedRows.join(','), interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue, 'STC');//, IsGenerateRecZip);
                    }
                }
                else {
                    alert('Please select atleast one job.');
                }
            }
        }
        else {
            if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $('#IsApproachingExpiryDate').val().toLowerCase() == "true") {
                alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
            }
            else {
                ApplyTradeActual(model_JobId, interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue, 'STC');//, IsGenerateRecZip);
            }
        }
    }
    else {
        alert('Oops, there is an error in the backend, please contact a Greenbot admin');
    }
}

function ApplyTradeSAAS() {
    var interval = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("interval") : $(".LiSettlement").data("interval");
    var price = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("price") : $(".LiSettlement").data("price");
    var settlementterm = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("settlementterm") : $(".LiSettlement").data("settlementterm");
    //var settlementterm = BasicDetails_IsWholeSaler.toLowerCase() == "false" ? $(".LiSettlement.active").data("settlementtermid") : $(".LiSettlement").data("settlementtermid");

    if ($(".LiSettlement.active").length == 0) {
        alert('Please select atleast one settlement term.');
        return;
    }
    else if ($('.Processing-time').find('.LiSettlement.active').length == 1) {
        if (model_IsGridView == 'true') {
            var IsSamePrice = true;
            var IsFirst = true;
            var PriceDay1 = 0;
            var PriceDay3 = 0;
            var PriceDay7 = 0;
            var UpFront = 0;
            var PriceOnApproval = 0;
            var PartialPayment = 0;
            var RapidPay = 0;
            var OptiPay = 0;
            var Commercial = 0;
            var Custom = 0;
            var InvoiceStc = 0;
            var PeakPay = 0;
            var isReadyforTrade = true;

            var newselectedRows = [];
            $('#datatable1 tbody input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') == true) {
                    if (IsShowInDashboard == 'True') {
                        if ($(this).attr('IsTraded') != 'true' && $(this).attr('IsCustomPrice') != 'true') {
                            if ($(this).attr('IsReadyToTrade') == 'true') {
                                if (IsFirst) {
                                    PriceDay1 = parseFloat($(this).attr('PriceDay1'));
                                    PriceDay3 = parseFloat($(this).attr('PriceDay3'));
                                    PriceDay7 = parseFloat($(this).attr('PriceDay7'));
                                    PriceOnApproval = parseFloat($(this).attr('PriceOnApproval'));
                                    RapidPay = parseFloat($(this).attr('RapidPay'));
                                    OptiPay = parseFloat($(this).attr('OptiPay'));
                                    Commercial = parseFloat($(this).attr('Commercial'));
                                    Custom = parseFloat($(this).attr('Custom'));
                                    InvoiceStc = parseFloat($(this).attr('InvoiceStc'));
                                    PeakPay = parseFloat($(this).attr('PeakPay'));
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                else if (PriceDay1 != parseFloat($(this).attr('PriceDay1')) || PriceDay3 != parseFloat($(this).attr('PriceDay3')) || PriceDay7 != parseFloat($(this).attr('PriceDay7'))
                                    || PriceOnApproval != parseFloat($(this).attr('PriceOnApproval'))
                                    || RapidPay != parseFloat($(this).attr('RapidPay')) || OptiPay != parseFloat($(this).attr('OptiPay')) || Commercial != parseFloat($(this).attr('Commercial'))
                                    || Custom != parseFloat($(this).attr('Custom')) || InvoiceStc != parseFloat($(this).attr('InvoiceStc')) || PeakPay != parseFloat($(this).attr('PeakPay'))) {
                                    IsSamePrice = false;
                                    isReadyforTrade = false;
                                    alert('Please select jobs which have same price.');
                                    return false;
                                }
                                else {
                                    IsSamePrice = true;
                                    newselectedRows.push($(this).attr('jobid'));
                                }
                                IsFirst = false;
                            }
                            else {
                                IsSamePrice = false;
                                isReadyforTrade = false;
                                alert('Please select jobs which are ready to trade.');
                                return false;
                            }
                        }
                        else {
                            IsSamePrice = false;
                            isReadyforTrade = false;
                            alert('Please select jobs which are neither traded nor have custom price.');
                            return false;
                        }

                    }
                    else {
                        if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $(this).attr('isApproachingExpiryDate') == 'true') {
                            alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
                            isReadyforTrade = false;
                            return false;
                        }
                        else {
                            newselectedRows.push($(this).attr('JobId'));
                        }
                    }
                }
            });

            if (isReadyforTrade == true) {
                if (newselectedRows.length > 0) {
                    if (confirm("Are you sure you want to Trade STC?")) {
                        ApplyTradeActual(newselectedRows.join(','), interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue, 'SAAS');//, IsGenerateRecZip);
                    }
                }
                else {
                    alert('Please select atleast one job.');
                }
            }
        }
        else {
            if ((settlementterm != 4 && settlementterm != 8 && settlementterm != 12 && customSettlementTerm != 4 && customSettlementTerm != 8 && customSettlementTerm != 12) && $('#IsApproachingExpiryDate').val().toLowerCase() == "true") {
                alert('Approaching 12-month expiry date for claiming STCs. This job can only be traded on approval. Please contact your account manager for further details.');
            }
            else {
                ApplyTradeActual(model_JobId, interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue, 'SAAS');//, IsGenerateRecZip);
            }
        }
    }
    else {
        alert('Oops, there is an error in the backend, please contact a Greenbot admin');
    }
}
function ApplyTradeActual(jobids, interval, price, settlementterm, peakPayGstRate, peakPayTimePeriodValue, peakPayFeeValue, TradeType)//, IsGenerateRecZip)
{
    debugger;
    var isSaved = CompareJobData();
    if (isSaved) {
        $.ajax({
            url: urlApplyTradeSTC,
            type: "POST",
            data: { jobId: jobids, interval: interval, price: price, settlementterm: settlementterm, isGst: model_IsGST, stcAmt: stcAmount, CustomSettlementTerm: customSettlementTerm, peakPayGst: peakPayGstRate, peakPayTimeperiod: peakPayTimePeriodValue, peakPayFee: peakPayFeeValue, TradeType: TradeType, STCRemark: STCRemark },
            success: function (Data) {
                if (Data.jobIds != undefined) {
                    showMessageForSTC("There are some illegal characters in the serial number field. Job cannot be traded until these are amended with the correct serials.", true);
                } else {
                    showMessageForSTC("Job has been traded successfully.", false);
                    if ((ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && $('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                        $('#btnSaveJob').hide();
                    }
                    if (isSaveJobAfterTrade == "True")
                        $('#btnSaveJob').show();
                    ReloadSTCJobScreen(model_JobId);
                    SearchHistory();
                    if (typeof (isDynamicJobIndex) != 'undefined' && (isDynamicJobIndex)) {
                        filterKendo = $('#datatable').data('kendoGrid').dataSource.filter();
                        datatableInfo();
                        if (!GridConfig[0].IsKendoGrid) {
                            $('#datatable').DataTable().destroy();
                            drawJobIndex();
                        } else {
                            $('#datatable').kendoGrid('destroy').empty();
                            drawJobIndex(filterKendo);
                        }
                    }
                    else {
                        $('#JobOwnerDetails_OwnerType').prop('disabled', true);
                        $('#JobInstallationDetails_PropertyType').prop('disabled', true);
                        $('#JobSystemDetails_SystemSize').prop('disabled', true);
                        $('#BasicDetails_strInstallationDate').prop('disabled', true);
                        $('#JobSystemDetails_SerialNumbers').prop('disabled', true);
                        $('#PricingBlock').hide();
                    }
                    if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
                        $("#CESDocbtns").hide();
                        $("#STCDocBtns").hide();
                        $('.isdelete').css("display", "none");
                    }
                    if (session_UserTypeId == 1 || session_UserTypeId == 3 || session_UserTypeId == 2 || session_UserTypeId == 4 || session_UserTypeId == 5) {
                        $("#CESDocbtns").show();
                        $("#STCDocBtns").show();
                    }
                    if (session_UserTypeId == 1 || session_UserTypeId == 3) {
                        $('.isdelete').css("display", "block");
                    }
                }
            }
        });
    }
    else {
        alert("There are changes in this job that have not been saved. Before trading, please save your changes.");
    }
}

function ShowHideJobs(objTerm, objCustTerm) {
    $('#datatable1').find('tr').show();
    $('#datatable1 tr td').each(function () {
        $(this).find('input[type=checkbox]').each(function () {
            isAllowKW = $(this).attr('isallowkw');
            jobSizeForOptiPay = $(this).attr('jobsizeforoptipay');
            isCommercialJob = $(this).attr('iscommercialjob');
            isNonCommercialJob = $(this).attr('isnoncommercialjob');
            isPeakPayCommercialJob = $(this).attr('isPeakPayCommercialJob');
            isPeakPayNonCommercialJob = $(this).attr('isPeakPayNonCommercialJob');

            isCustomAllowKW = $(this).attr('iscustomallowkw');
            jobCustomSizeForOptiPay = $(this).attr('jobcustomsizeforoptipay');
            isCustomCommercialJob = $(this).attr('iscustomcommercialjob');
            isCustomNonCommercialJob = $(this).attr('iscustomnoncommercialjob');
            isCustomPeakPayCommercialJob = $(this).attr('isCustomPeakPayCommercialJob');
            isCustomPeakPayNonCommercialJob = $(this).attr('isCustomPeakPayNonCommercialJob');

            modelIsGSTSetByAdminUser = $(this).attr('modelisgstsetbyadminuser');
            isGst = $(this).attr('isgst').toLowerCase();
            gstDocument = $(this).attr('gstdocument');
            propType = $(this).attr('proptype').toLowerCase();
            sysSize = $(this).attr('jobsystemsize');
            ownerType = $(this).attr('ownerType').toLowerCase();
        })

        if (objTerm == 7) {
            CheckRuleofRapidPay(null, propType, $(this), ownerType);
        }
        else if (objTerm == 8) {
            CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
        }
        else if (objTerm == 9) {
            CheckRuleofCommercial(null, modelIsGSTSetByAdminUser, isGst, gstDocument, $(this), ownerType, propType);
        }
        else if (objTerm == 12) {
            CheckRuleofPeakPay(isPeakPayCommercialJob, isPeakPayNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
        }
        else if (objTerm == 10) {
            if (objCustTerm == 7) {
                CheckRuleofRapidPay(null, propType, $(this), ownerType);
            }
            else if (objCustTerm == 8) {
                CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
            }
            else if (objCustTerm == 9) {
                CheckRuleofCommercial(null, modelIsGSTSetByAdminUser, isGst, gstDocument, $(this), ownerType, propType);
            }
            else if (objCustTerm == 12) {
                CheckRuleofPeakPay(isCustomPeakPayCommercialJob, isCustomPeakPayNonCommercialJob, null, modelIsGSTSetByAdminUser, isGst, $(this), ownerType, propType);
            }
        }
    });
    $('#chkAll1').prop('checked', true)
    datatable1Chkbox($('#chkAll1').prop('checked'), false);
}

$(".LiSettlement").click(function () {
    $(".LiSettlement").removeClass("active");
    $(this).addClass('active');
})