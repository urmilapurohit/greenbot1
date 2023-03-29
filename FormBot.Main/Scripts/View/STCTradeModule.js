function EnableDisableSettlementTerm() {
    CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, $("#OptiPay"), modelIsGSTSetByAdminUser, isGst, null, ownerType, propType);
    CheckRuleofRapidPay($("#RapidPay"), propType, null, ownerType);
    CheckRuleofRapidPay($("#24Hour"), propType, null, ownerType);
    CheckRuleofRapidPay($("#3Days"), propType, null, ownerType);
    CheckRuleofRapidPay($("#7Days"), propType, null, ownerType);
    CheckRuleofCommercial($("#Commercial"), modelIsGSTSetByAdminUser, isGst, gstDocument, null, ownerType, propType);
    CheckRuleofPeakPay(isPeakPayCommercialJob, isPeakPayNonCommercialJob, $("#Peakpay"), modelIsGSTSetByAdminUser, isGst, null, ownerType, propType);

    // if custom settlement term is there
    //rapid - pay, 24hours, 3days, 7days
    if (customSettlementTerm == 7 || customSettlementTerm == 1 || customSettlementTerm == 2 || customSettlementTerm == 3) {
        CheckRuleofRapidPay($("#Custom"), propType, null, ownerType, propType);
    }
        //opti-pay
    else if (customSettlementTerm == 8) {
        CheckRuleofOptiPay(isCustomAllowKW, jobCustomSizeForOptiPay, isCustomCommercialJob, isCustomNonCommercialJob, $("#Custom"), modelIsGSTSetByAdminUser, isGst, null, ownerType, propType);
    }
        //Commercial
    else if (customSettlementTerm == 9) {
        CheckRuleofCommercial($("#Custom"), modelIsGSTSetByAdminUser, isGst, gstDocument, null, ownerType, propType);
    }
        //PeakPay
    else if (customSettlementTerm == 12) {
        CheckRuleofPeakPay(isCustomPeakPayCommercialJob, isCustomPeakPayNonCommercialJob, $("#Custom"), modelIsGSTSetByAdminUser, isGst, null, ownerType, propType);
    }
    else {
        bindClickEventOfSettlementTerm($("#Custom"));
    }
}

function CheckRuleofOptiPay(isAllowKW, jobSizeForOptiPay, isCommercialJob, isNonCommercialJob, objPricingTerm, modelIsGSTSetByAdminUser, isGst, objCheckbox, ownerType, propType) {
    if ((isAllowKW == "true" && parseFloat(sysSize) <= parseFloat(jobSizeForOptiPay)) || isAllowKW == "false") {
        // show/hide jobs for SCA dashboard
        if (objPricingTerm == null) {
            if (isNonCommercialJob == "true" && isCommercialJob == "false") {
                if ((ownerType != "corporate body" && ownerType != "trustee" && propType != "commercial" && propType != "school")) {
                    objCheckbox.closest('tr').show();
                }
                else if ((ownerType == "corporate body" || ownerType == "trustee") && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'false') {
                    objCheckbox.closest('tr').show();
                }
                else {
                    objCheckbox.closest('tr').hide();
                }
            }
            else if (isNonCommercialJob == "false" && isCommercialJob == "true") {
                if (modelIsRegisteredWithGST == 'true') {
                    if (modelIsGSTSetByAdminUser == 2) {
                        objCheckbox.closest('tr').show();
                    }
                    else if ((((ownerType == "corporate body" || ownerType == "trustee")))) {
                        if (modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                            objCheckbox.closest('tr').show();
                        }
                        else {
                            objCheckbox.closest('tr').hide();
                        }
                    }
                    else if (propType == "commercial" || propType == "school") {
                        objCheckbox.closest('tr').show();
                    }
                    else {
                        objCheckbox.closest('tr').hide();
                    }
                }
                else {
                    objCheckbox.closest('tr').hide();
                }
            }
            else if (isNonCommercialJob == "true" && isCommercialJob == "true") {
                objCheckbox.closest('tr').show();
            }
            else {
                objCheckbox.closest('tr').hide();
            }
        }
        else {
            if (isNonCommercialJob == "true" && isCommercialJob == "false") {
                if (ownerType != "corporate body" && ownerType != "trustee" && (propType != "commercial" && propType != "school")) {
                    bindClickEventOfSettlementTerm(objPricingTerm);
                }
                else if ((ownerType == "corporate body" || ownerType == "trustee") && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'false') {
                    bindClickEventOfSettlementTerm(objPricingTerm);
                }
                else {
                    unBindClickEventOfSettlementTerm(objPricingTerm);
                }
            }
            else if (isNonCommercialJob == "false" && isCommercialJob == "true") {
                if (modelIsRegisteredWithGST == 'true') {
                    if (modelIsGSTSetByAdminUser == 2) {
                        bindClickEventOfSettlementTerm(objPricingTerm);
                    }
                    else if ((((ownerType == "corporate body" || ownerType == "trustee")))) {
                        if (modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                            bindClickEventOfSettlementTerm(objPricingTerm);
                        }
                        else {
                            unBindClickEventOfSettlementTerm(objPricingTerm);
                        }
                    }
                    else if (propType == "commercial" || propType == "school") {
                        bindClickEventOfSettlementTerm(objPricingTerm);
                    }
                    else {
                        unBindClickEventOfSettlementTerm(objPricingTerm);
                    }
                }
                else {
                    unBindClickEventOfSettlementTerm(objPricingTerm);
                }
            }
            else if (isNonCommercialJob == "true" && isCommercialJob == "true") {
                bindClickEventOfSettlementTerm(objPricingTerm);
            }
            else {
                unBindClickEventOfSettlementTerm(objPricingTerm);
            }
        }
    }
    else
        unBindClickEventOfSettlementTerm(objPricingTerm);
}
function CheckRuleofRapidPay(objPricingTerm, propType, objCheckbox, ownerType) {
    // show/hide jobs for SCA dashboard
    if (objPricingTerm == null) {
        if ((ownerType == "corporate body" || ownerType == "trustee")) {
            if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                objCheckbox.closest('tr').hide();
            }
            else {
                objCheckbox.closest('tr').show();
            }
        }
        else if (propType == "commercial" || propType == "school") {
            objCheckbox.closest('tr').hide();
        }
        else {
            objCheckbox.closest('tr').show();
        }
    }
    else {
        if ((ownerType == "corporate body" || ownerType == "trustee")) {
            if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                unBindClickEventOfSettlementTerm(objPricingTerm);
            }
            else {
                bindClickEventOfSettlementTerm(objPricingTerm);
            }
        }
        else if (propType == "commercial" || propType == "school") {
            unBindClickEventOfSettlementTerm(objPricingTerm);
        }
        else {
            bindClickEventOfSettlementTerm(objPricingTerm);
        }
    }
}

function CheckRuleofCommercial(objPricingTerm, modelIsGSTSetByAdminUser, isGst, gstDocument, objCheckbox, ownerType, propType) 
{
    // show/hide jobs for SCA dashboard
    if (objPricingTerm == null) {
        if (modelIsRegisteredWithGST == 'true') {
            if ((modelIsGSTSetByAdminUser == 2 || (ownerType == "corporate body" || ownerType == "trustee") || propType == "commercial" || propType == "school")) {
                if ((ownerType == "corporate body" || ownerType == "trustee")) {
                    if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                        objCheckbox.closest('tr').show();
                    }
                    else {
                        objCheckbox.closest('tr').hide();
                    }
                }
                else if (propType == "commercial" || propType == "school") {
                    objCheckbox.closest('tr').show();
                }
                else {
                    objCheckbox.closest('tr').hide();
                }
            }
            else {
                objCheckbox.closest('tr').hide();
            }
        }
        else {
            objCheckbox.closest('tr').hide();
        }
    }
    else {
        //if (modelIsRegisteredWithGST == 'true' && (modelIsGSTSetByAdminUser == 2 || (ownerType == "corporate body" || ownerType == "trustee") || propType == "commercial" || propType == "school")) {
        //    bindClickEventOfSettlementTerm(objPricingTerm);
        if (modelIsRegisteredWithGST == 'true') {
            if ((modelIsGSTSetByAdminUser == 2 || (ownerType == "corporate body" || ownerType == "trustee") || propType == "commercial" || propType == "school")) {
                if ((ownerType == "corporate body" || ownerType == "trustee")) {
                    if (modelIsOwnerRegisteredWithGST != undefined && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                        bindClickEventOfSettlementTerm(objPricingTerm);
                    }
                    else {
                        unBindClickEventOfSettlementTerm(objPricingTerm);
                    }
                }
                else if (propType == "commercial" || propType == "school") {
                    bindClickEventOfSettlementTerm(objPricingTerm);
                }
                else {
                    unBindClickEventOfSettlementTerm(objPricingTerm);
                }
            }
            else {
                unBindClickEventOfSettlementTerm(objPricingTerm);
            }
        }
        else {
            unBindClickEventOfSettlementTerm(objPricingTerm);
        }
    }
}
function CheckRuleofPeakPay(isPeakPayCommercialJob, isPeakPayNonCommercialJob, objPricingTerm, modelIsGSTSetByAdminUser, isGst, objCheckbox, ownerType, propType) {
    // show/hide jobs for SCA dashboard
    if (objPricingTerm == null) {
        if (isPeakPayNonCommercialJob == "true" && isPeakPayCommercialJob == "false") {
            if (ownerType != "corporate body" && ownerType != "trustee" && (propType != "commercial" && propType != "school")) {
                objCheckbox.closest('tr').show();
            }
            else if ((ownerType == "corporate body" || ownerType == "trustee") && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'false') {
                objCheckbox.closest('tr').show();
            }
            else {
                objCheckbox.closest('tr').hide();
            }
        }
        else if (isPeakPayNonCommercialJob == "false" && isPeakPayCommercialJob == "true") {
            if (modelIsRegisteredWithGST == 'true') {
                if (modelIsGSTSetByAdminUser == 2) {
                    objCheckbox.closest('tr').show();
                }
                else if ((((ownerType == "corporate body" || ownerType == "trustee")))) {
                    if (modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                        objCheckbox.closest('tr').show();
                    }
                    else {
                        objCheckbox.closest('tr').hide();
                    }
                }
                else if (propType == "commercial" || propType == "school") {
                    objCheckbox.closest('tr').show();
                }
                else {
                    objCheckbox.closest('tr').hide();
                }
            }
            else {
                objCheckbox.closest('tr').hide();
            }
        }
        else if (isPeakPayNonCommercialJob == "true" && isPeakPayCommercialJob == "true") {
            objCheckbox.closest('tr').show();
        }
        else {
            objCheckbox.closest('tr').hide();
        }
    }
    else {
        if (isPeakPayNonCommercialJob == "true" && isPeakPayCommercialJob == "false") {
            if (ownerType != "corporate body" && ownerType != "trustee" && (propType != "commercial" && propType != "school")) {
                bindClickEventOfSettlementTerm(objPricingTerm);
            }
            else if ((ownerType == "corporate body" || ownerType == "trustee") && modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'false') {
                bindClickEventOfSettlementTerm(objPricingTerm);
            }
            else {
                unBindClickEventOfSettlementTerm(objPricingTerm);
            }
        }
        else if (isPeakPayNonCommercialJob == "false" && isPeakPayCommercialJob == "true") {
            if (modelIsRegisteredWithGST == 'true') {
                if (modelIsGSTSetByAdminUser == 2) {
                    bindClickEventOfSettlementTerm(objPricingTerm);
                }
                else if ((((ownerType == "corporate body" || ownerType == "trustee")))) {
                    if (modelIsOwnerRegisteredWithGST.toString().toLowerCase() == 'true') {
                        bindClickEventOfSettlementTerm(objPricingTerm);
                    }
                    else {
                        unBindClickEventOfSettlementTerm(objPricingTerm);
                    }
                }
                else if (propType == "commercial" || propType == "school") {
                    bindClickEventOfSettlementTerm(objPricingTerm);
                }
                else {
                    unBindClickEventOfSettlementTerm(objPricingTerm);
                }
            }
            else {
                unBindClickEventOfSettlementTerm(objPricingTerm);
            }
        }
        else if (isPeakPayNonCommercialJob == "true" && isPeakPayCommercialJob == "true") {
            bindClickEventOfSettlementTerm(objPricingTerm);
        }
        else {
            unBindClickEventOfSettlementTerm(objPricingTerm);
        }
    }
}

function bindClickEventOfSettlementTerm(objSettlementTerm) {
    if (objSettlementTerm) {
        objSettlementTerm.bind("click", function () { SettlementTermClick(objSettlementTerm) });
        objSettlementTerm.find('span').removeClass("ui-state-disabled");
    }
}
function unBindClickEventOfSettlementTerm(objSettlementTerm) {
    if (objSettlementTerm) {
        objSettlementTerm.unbind("click");
        objSettlementTerm.find('span').addClass("ui-state-disabled");
    }
}
