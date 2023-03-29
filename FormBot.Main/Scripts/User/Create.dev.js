
$(document).ready(function () {

    if (accreditedInstallerId != 0) {
        $("#Password").val(modelPassword);
        $("input[name=SEDesigner][value='" + modelSEDesignRoleId + "']").prop('checked', true);
    }

    FillDropDownUser('UserTypeId', getUserTypeURL, userTypeId ? userTypeId : 0, true, ChangeUserType(userTypeId));

    dropDownData.push(
        { id: 'UnitTypeId', value: unitTypeID, key: "UnitType", hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'StreetTypeId', value: streetTypeId, key: "StreetType", hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'ResellerID', value: modelResellerId, key: "Reseller", hasSelect: true, callback: null, defaultText: null, proc: 'Reseller_BindDropDown', param: [], bText: 'ResellerName', bValue: 'ResellerID' },
        { id: 'PostalAddressID', value: postalAddressID, key: "PostalAddress", hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        { id: 'WholesalerUnitTypeID', key: "UnitType", value: WholesalerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'WholesalerStreetTypeID', key: "StreetType", value: WholesalerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'WholesalerPostalAddressID', key: "PostalAddress", value: WholesalerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' },
        { id: 'InvoicerUnitTypeID', key: "UnitType", value: InvoicerUnitTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'UnitType_BindDropdown', param: [], bText: 'UnitTypeName', bValue: 'UnitTypeID' },
        { id: 'InvoicerStreetTypeID', key: "StreetType", value: InvoicerStreetTypeID, hasSelect: true, callback: null, defaultText: null, proc: 'StreetType_BindDropdown', param: [], bText: 'StreetTypeName', bValue: 'StreetTypeID' },
        { id: 'InvoicerPostalAddressID', key: "PostalAddress", value: InvoicerPostalAddressID, hasSelect: true, callback: null, defaultText: null, proc: 'PostalAddress_BindDropdown', param: [], bText: 'PostalDeliveryType', bValue: 'PostalAddressID' });
    dropDownData.bindDropdown();

    $("#btnClosePopUpBox").click(function () {
        $('#popupbox').modal('toggle');
    });
    $("#btnClosePopUpBoxSE").click(function () {
        $('#popupboxSE').modal('toggle');
    });
    $("#btnClosepopupProof").click(function () {
        $('#popupProof').modal('toggle');
    });
    $("#btnClosePopUpBoxSelfie").click(function () {
        $('#popupboxSelfie').modal('toggle');
    });
    $("#fromDate").val("");
    $("#toDate").val("");
    $('#horizontalTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true,   // 100% fit in a container
        closed: 'accordion', // Start closed if in accordion view
        activate: function (event) {
            // Callback function if tab is switched
            //addRules();
            addressValidation();
            var obj = [];
            $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
                if ($(this).attr("class").indexOf("selected") != -1) {
                    obj.push($(this).val());
                }
            });

            var $this = $(this);

            if (userTypeId == 7 && (!$("#IsPVDUser").prop('checked') && !$("#IsSWHUser").prop('checked'))) {// && !$("#IsVEECUser").prop('checked')) {
                $this.removeClass('resp-tab-active');
                $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                $('#' + activeTab).addClass('resp-tab-active');
                $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                alert('At least one work type should be selected.');
                return false;
            }

            if ($("#FCOGroupId").is(":visible")) {
                if (isValid && obj.length > 0 && CheckShowMessages()) {
                    //return  CheckShowMessages();
                    $('#spanFCOGroup').hide();
                    activeTab = $this.attr('id');
                    $('.form-box').find('input:first').focus();
                }
                else {
                    // e.preventDefault();
                    $this.removeClass('resp-tab-active');
                    $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                    $('#' + activeTab).addClass('resp-tab-active');
                    $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                    if (obj.length > 0) {
                        $('#spanFCOGroup').hide();
                    }
                    else {
                        $('#spanFCOGroup').show();
                    }
                    CheckShowMessages();
                    return false;
                }
            }
            else {
                if (isValid && CheckShowMessages() && addRules()) {
                    //return  CheckShowMessages();
                    activeTab = $this.attr('id');
                    $('.form-box').find('input:first').focus();
                }
                else {
                    // e.preventDefault();
                    $this.removeClass('resp-tab-active');
                    $('.resp-tab-content-active').css('display', 'none').removeClass('resp-tab-content-active');
                    $('#' + activeTab).addClass('resp-tab-active');
                    $('.tab' + activeTab.replace('t', '')).addClass('resp-tab-content-active').css('display', 'block');
                    CheckShowMessages();
                    addRules();
                    return false;
                }
            }

            if (activeTab == 't6') {
                $.ajax({
                    type: 'GET',
                    url: getInstallerDesignerAddedByProfileURL,
                    data: { usertypeId: userTypeId, solarcompanyId: modelSolarCompanyId },
                    contentType: 'application/json',
                    success: function (result) {
                        $('#instDesignerAddedByProfile').html(result);
                    }
                });
            }
        }
    });

    window.asd = $('.SlectBox').SumoSelect({ csvDispCount: 4 });

    $('#verticalTab').easyResponsiveTabs({
        type: 'vertical',
        width: 'auto',
        fit: true
    });

    // defaultHideField();
    $('#fromDate').val('');
    $('#toDate').val('');
    // $('#horizontalTab').hide();
    BindRoles(userTypeId);
    //var userTypeId = userTypeId;
    //var isUserTypeLogin = false;

    if (sessionUserTypeId == '2') {
        $("#UserTypeId").prop("disabled", true);
    }
    if (sessionUserTypeId == '4') {
        $("#UserTypeId").prop("disabled", true);
    }
    if (sessionUserTypeId == '6') {
        $("#UserTypeId").prop("disabled", true);
    }
    if (sessionUserTypeId == '8') {
        $("#UserTypeId").prop("disabled", true);
    }
    if (userTypeId == '2') {
        $('#RecUsernamePwd').show();
        if (sessionUserTypeId == 1) {
            $(".RECSuperAdmin").show();
        }
        else {
            $(".RECSuperAdmin").hide();
        }
    }

    $("#IsActive").prop("checked", true);
    if (userTypeId == 5) {
        $('.RAMCreate').hide();
    }
    if (userTypeId == 8) {
        $('.SCACreate').hide();
        $('.ProfileSCO').show();
        $('.ProfileRA').hide();
        $('.SCOProfile').hide();
    }
    if (userTypeId == 8 && sessionUserTypeId == 4) {
        $('.chkSCASignUp').hide();
        $('.uploadSCA').hide();
        $('.SCO').show();
        $('.SCProfile').hide();
        $('.SCACreate').hide();
        $('.SCACreateSCO').hide();
        $('#t3').hide();
    }
    if (userTypeId == 8 && sessionUserTypeId == 6) {
        $('.chkSCASignUp').hide();
        $('.uploadSCA').hide();
        $('.SCO').show();
        $('.SCProfile').hide();
        $('.SCACreate').hide();
        $('.SCACreateSCO').hide();
        $('#t3').hide();
    }
    //ChangeUserType($("#UserTypeId").val());
});

function getDropDownValue() {
    if (userTypeId == 7) {
        ValidateFieldBasedOnWorkType();
        //if (!ValidateFieldBasedOnWorkType()) {
        //    return false;
        //}
    }
    if ($('#UserTypeId').val() == 7) {
        $('#IsAutoRequest').val($('#AutoRequestSwitch').prop('checked'));
    }
    if (validateForm()) {
        $('.SlectBox option:selected').each(function (i) {
            $('<input type="hidden">').attr({
                name: 'FCOGroupSelected',
                value: $(this).val(),
                id: $(this).val(),
            }).appendTo('form');
        });
        return true;
    }
    else {
        return false;
    }
};

$('#FCOGroupId').change(function (event) {
    var obj = [];
    if ($("#FCOGroupId")[0][0].value == "")
        $("#FCOGroupId")[0][0].remove();

    $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
        if ($(this).attr("class").indexOf("selected") != -1) {
            $('#FCOGroupId')[0].sumo.selectItem(i);
            obj.push($(this).val());
        }
    });
    if (obj.length > 0) {
        $('#spanFCOGroup').hide();
    }
    else {
        $('#spanFCOGroup').show();
    }
});

function validateForm() {
    if ($("#UserTypeId").val() == 7 && !$("#IsPVDUser").prop('checked') && !$("#IsSWHUser").prop('checked')) {// && !$("#IsVEECUser").prop('checked')) {
        alert('At least one work type should be selected.');
        return false;
    }

    //if ($("#UserTypeId").val() == 8 || $("#UserTypeId").val() == 9) {
    if (sessionUserTypeId != 4 && ($("#UserTypeId").val() == 8 || $("#UserTypeId").val() == 9)) {
        if (($("#ResellerID").val() == null || $("#SolarCompanyId").val() == null) && ($("#SolarSubContractorID").val() == null || $("#SolarSubContractorID").val() == "")) {
            alert('Please select either "ResellerName and SolarCompany" or "SolarSubContractor"');
            return false;
        }

        if ($("#ResellerID").val() > 0 && $("#SolarSubContractorID").val() > 0) {
            alert('Please select either SolarCompany or SolarSubContractor');
            return false;
        }

        if ($("#SolarSubContractorID").val() > 0) {
            $("#ResellerID").rules("remove", "required");
            $("#SolarCompanyId").rules("remove", "required")
        }
    }
    //addRules();
    if ($("#UserDetails").valid()) {
        if (userTypeId == 2 || userTypeId == 5 || $("#UserTypeId").val() == 2 || $("#UserTypeId").val() == 5) {
            var isExistCode = CheckClientCodePrefix($("#ClientCodePrefix").val(), 1, $("#ResellerID").val(), $("#UserTypeId").val());
            if (isExistCode == 0)
                return false;
        }
    }

    if ($("#UserTypeId").val() == 4) {
        $("#RAMId").val($("#hdnRAMID").val());
    }

    addressValidation();
    var obj = [];
    $.validator.unobtrusive.parse("#UserDetails");

    $.each($("#FCOGroupId").parent().find('ul').find('li'), function (i, e) {
        if ($(this).attr("class").indexOf("selected") != -1) {
            obj.push($(this).val());
        }
    });

    if ($("#FCOGroupId").is(":visible")) {
        if ($("#UserDetails").valid()) {
            if (obj.length > 0) {
                $('#spanFCOGroup').hide();
                return CheckShowMessages()
                // return true;
            }
            else {
                $('#spanFCOGroup').show();
                CheckShowMessages();
                return false;
            }
        }
        else {
            if (obj.length > 0) {
                $('#spanFCOGroup').hide();

            }
            else {
                $('#spanFCOGroup').show();

            }
            CheckShowMessages();
            return false;
        }
    }
    else {
        if ($("#UserDetails").valid() && CheckShowMessages() && addRules()) {
            return true;
        }
        else {
            addRules();
            return false;
        }
    }
}

function focusToSearch() {
    document.getElementById("btnSubmit").focus();
}

function defaultHideField() {
    $('.RA').hide();
    $('.RAM').hide();
    $('.SCA').hide();
    $('.FCO').hide();
    $('.SCO').hide();
    $('.SE').hide();
    $('.SWH').hide();
    $('.FSA').hide();
    $('.SSC').hide();
    $('.showWholeSaler').hide();
    $('.defaultFormBot').hide();
    $('.drpjobtype').hide();
}

function ChangeUserType(unitTypeID, isUserTypeLogin) {
    ClearAllJobTypeCheckBoxValue();
    chkUserName = true;
    chkCompanyABN = true;
    chkCECAccreditationNumber = true;
    $('#horizontalTab').show();
    $('.form-box').find('input:first').focus();
    $('#spanFCOGroup').hide();

    if ($("#FCOGroupId")[0].sumo) {
        $("#FCOGroupId")[0].sumo.unSelectAll();
    }

    if (accreditedInstallerId == 0) {
        $('#tblDocuments').html('');
        $('input[type=text]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });

        $('input[type=PassWord]').each(function () {
            $(this).val('');
            $(this).attr('class', 'form-control valid');
        });
    }

    //GetSolarCompanyBasedOnRA();

    $(".field-validation-error").attr('class', 'field-validation-valid');
    $('#lblCECAccreditationNumber').removeClass("required");
    $("#chkSCA").prop("checked", false);
    userTypeId = unitTypeID;  // Set usertype id value before calling ShowHideWholeSalerInvoice() method bcz user type id used in this method is set from model but no modal value id changed based on usertypr change so to update usertypeid value.																																																												 
    if (unitTypeID == "") {
        $('#horizontalTab').hide();
        defaultHideField();
        $('.Role').hide();
        $("#RecUsernamePwd").hide();
    }
    else if (unitTypeID == 1) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FSA').show();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.drpjobtype').hide();
        $("#RecUsernamePwd").hide();
        $.ajax({
            url: getRoleURL1,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 2) {
        $('.FSA').hide();
        $('.FCO').hide();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SSC').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SC').hide();
        $('.RA').show();
        $('.PDA').hide();
        $('.Role').show();
        $('.ProfileSCO').show();
        $('.ProfileRA').hide();
        $('.showWholeSaler').show();
        $("#RecUsernamePwd").show();
        ShowHideWholeSalerInvoice();
        $.ajax({
            url: getRoleURL2,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        $('.defaultFormBot').show();
        $('#RecUsernamePwd').show();
    }
    else if (unitTypeID == 3) {
        $('.FSA').hide();
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.FCO').show();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.defaultFormBot').show();
        $('.drpjobtype').hide();

        $.ajax({
            url: getRoleURL3,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });

        if (isUserTypeLogin == true) {
            $('#UserTypeId').val('3');
        }
    }
    else if (unitTypeID == 4) {
        $('.FSA').hide();
        $('.FCO').hide();
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.RAM').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SCA').show();
        $('.PDA').hide();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $('.chkSCASignUp').hide();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.drpjobtype').hide();
        $("#RecUsernamePwd").hide();
        $('#lblCECAccreditationNumber').addClass("required");
        $.ajax({
            url: getRoleURL4,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        checkIsWholeSaler($("#ResellerID").val());
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 5) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('#UserTypeId').val('5');
        $('.SC').hide();
        $('.RAM').show();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.defaultFormBot').show();
        $('.drpjobtype').hide();
        $("#RecUsernamePwd").hide();
        $.ajax({
            url: getRoleURL5,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        if (isUserTypeLogin == true) {
            $('.RAMdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else if (unitTypeID == 6) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SC').hide();
        $('.SSC').show();
        $('.PDA').hide();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.drpjobtype').hide();
        $("#RecUsernamePwd").hide();
        //$('.Showreseller').show();
        $.ajax({
            url: getRoleURL6,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        $('.defaultFormBot').show();
    }
    else if (unitTypeID == 7) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SE').show();
        $('.SWH').hide();
        $('.hideSESWH').hide();
        $('.PDA').hide();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $("#RecUsernamePwd").hide();
        $('.drpjobtype').show();
        userTypeId = unitTypeID;
        InitializeClickEventForJobTypeClickEvent();
        $('#lblCECAccreditationNumber').addClass("required");
        $.ajax({
            url: getRoleURL7,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
    }
    else if (unitTypeID == 10) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.SCO').hide();
        $('.RAM').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SE').show();
        $('.hideForSWH').hide();
        $('.SWH').show();
        $('.hideSESWH').hide();
        $('.PDA').hide();
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $("#RecUsernamePwd").hide();
        $('.Role').show();
        $('.showWholeSaler').hide();
        $('.drpjobtype').hide();
        //$('#lblCECAccreditationNumber').addClass("required");
        $.ajax({
            url: getRoleURL10,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
    }
    else if (unitTypeID == 8) {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('.SC').hide();
        $('.SCO').show();
        $('#UserTypeId').val('8');
        $('.defaultFormBot').show();
        $('.ProfileSCO').show();
        $('.SCOProfile').hide();
        $('.ProfileRA').hide();
        $("#RecUsernamePwd").hide();
        $('.Role').show();
        $('#t3').hide();
        $('.SCProfile').hide();
        $('.showWholeSaler').hide();
        $("#lblResellerID").removeClass("required");
        $("#lblSolarCompanyId").removeClass("required");
        $('.drpjobtype').hide();
        FillDropDown('SolarSubContractorID', getSCAIsSSCByRAIdURL + '?id=' + $('#ResellerID').val() + '&IsSSC=true', modelSSCId, true, null);
        $.ajax({
            url: getRoleURL8,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
        if (isUserTypeLogin == true) {
            $('.SCdrp').hide();
            $("#UserTypeId").prop("disabled", true);
        }
    }
    else {
        //$('.RA').hide();
        $('.RA').show();
        $('#dvIsWholeSaler').hide();
        ShowHideWholeSalerInvoice();
        $('.SCA').hide();
        $('.FCO').hide();
        $('.FSA').hide();
        $('.RAM').hide();
        $('.SE').hide();
        $('.SWH').hide();
        $('.SSC').hide();
        $('.SC0').hide();
        $('.SC').show();
        $('.PDA').hide();
        $('.ProfileSCO').show();
        $('.SCOProfile').show();
        $('.ProfileRA').hide();
        $('.SCNext').hide();
        $('.Role').show();
        $('.SCOPro').hide();
        $('.showWholeSaler').hide();
        $('.drpjobtype').hide();
        $("#lblResellerID").removeClass("required");
        $("#lblSolarCompanyId").removeClass("required");
        $("#RecUsernamePwd").hide();
        FillDropDown('SolarSubContractorID', getSCAIsSSCByRAIdURL + '?id=' + $('#ResellerID').val() + '&IsSSC=true', modelSSCId, true, null);
        //$('.Showreseller').show();
        $.ajax({
            url: getRoleURL9,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data == 0) {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                }
                else {
                    $("#RoleID").empty();
                    $("#RoleID").append('<option value="">' + "Select" + '</option>');
                    var name = '';
                    $.each(data, function (i, role) {
                        if (role.Selected == true) {
                            name = role.Value;
                            CheckRoleHasSaasUser(role.Value);
                        }
                        $("#RoleID").append('<option value="' + role.Value + '">' +
                            role.Text + '</option>');
                    });
                    $("#RoleID").val(parseInt(name));
                }
            },
        });
    }
}

$("#ResellerID").change(function () {

    if ($("#UserTypeId").val() == 4 || $("#UserTypeId").val() == 5) {
        GetClientNumberOnRAMChange(2, $("#ResellerID").val() > 0 ? $("#ResellerID").val() : 0, 0, GetClientNumberOnRAMChangeURL);
    }
    BindRAMForSolarCompany(0, $("#ResellerID").val());
    FillDropDown('SolarCompanyId', getSCAByRAIdURL + '?id=' + $('#ResellerID').val(), modelSolarCompanyId, true, null);
    GetSolarCompanyBasedOnRA();
    checkIsWholeSaler($("#ResellerID").val());
});

$(document).ready(function () {

    TownPostcodeAutoComplete($('#txtTown'), $('#txtState'), $('#txtPostCode'));

    $('#btnCancel').click(function () {
        window.location.href = getUserIndexURL;
    });

    $('#btnCancelLast').click(function () {
        window.location.href = getUserIndexURL;
    });

    $('#btnCancelTab7').click(function () {
        window.location.href = getUserIndexURL;
    });
    $('#btnCancelTab10').click(function () {
        window.location.href = getUserIndexURL;
    });
});


$images = $('#MainDiv').find('div.divFiles');
$(".imageUpload").change(function (event) {
    readURL(this);
});

//Add
function readURL(input) {
    if (input.files && input.files[0]) {
        $.each(input.files, function () {
            var file = getNameFromPath($("#fileToUploadPassport").val());
            var filename = file.substring(0, file.length - 4);
            var fileExtention = file.split('.').pop();
            //var fileExtention = file.substring(file.length - 4, file.length);
            var fileSize = input.files[0].size;
            var reader = new FileReader();
            reader.onload = function (e) {
                //var id = guid();
                //FileArray.push(id);
                if ($("img.imgFiles").length > 0) {
                    $images.append('<img src="' + e.target.result + '" style="display:none;" id="imgFile' + $('img.imgFiles').length + '"  file-id="' + id + '" Status="Add" class="imgFiles" />')
                    selector.find('#Next').addClass("ic_disable");

                    if ($('img.imgFiles').length > 1 && CurrImage < $('img.imgFiles').length - 1)
                        selector.find('#Next').removeClass("ic_disable");
                    $("#msgSection").html('');
                    $("#msgSection").append('<div class="alert alert-success" onclick="$(this).fadeOut(3000)">Image has been added successfully at the end.<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">×</button></div>');
                    //setTimeout(function () {
                    //    $('.alert').fadeOut(4000);
                    //}, 4000);
                }
                else {
                    $images.append('<img src="' + e.target.result + '" id="imgFile' + $('img.imgFiles').length + '"  file-id="' + id + '" Status="Add" class="imgFiles" />')
                    $("#msgSection").html('');
                    $("#msgSection").append('<div class="alert alert-success" onclick="$(this).fadeOut(3000)">Image has been added successfully.<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">×</button></div>');
                    //setTimeout(function () {
                    //    $('.alert').fadeOut(4000);
                    //}, 4000);
                }
                selector.find("#imgFile" + CurrImage).attr("style", "width:100%;height:100%");
                //JSONFiles(id, e.target.result.replace('data:image/jpeg;base64,', '').replace('data:image/png;base64,', ''), 'Add', filename, fileExtention, fileSize);
            }
            reader.readAsDataURL(this);
        });
        setTimeout(function () {
            ShowHideAddDeleteIcon();
        }, 100);
    }
}

//File browse get FileName
function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}

//For Add Button Photo Preview
function ShowAddPreview(input, id, fileID) {
    var file = getNameFromPath($("#" + fileID).val());
    var filename = file.substring(0, file.length - 4);
    var fileExtention = file.split('.').pop();
    //var fileExtention = file.substring(file.length - 4, file.length);
    var size = input.files[0].size;
    var flag = false;

    if (file != null) {
        var extension = file.substr((file.lastIndexOf('.')));
        var finalExtension = '*' + extension.toLowerCase();

        //$.each(allowImageTypes.split(","), function (i, item) {
        //    if (finalExtension.toLowerCase() == item.toLowerCase()) {
        //        flag = true;
        //        return;
        //    }
        //});
    }
    if (flag == false) {
        $(".alert").hide();
        $("#errorMsgRegion").show();
        setTimeout(function () {
            $('.alert').fadeOut(4000);
        }, 4000);
        $('#' + fileID).val('');
        //validateForm();
        return false;
    }
    else {
        if (parseFloat((input.files[0].size) / 1048576) > errorFileSize) {
            $(".alert").hide();
            $("#errorMsgRegion").html(closeButton + "File size exceeds it's maximum limit of " + errorFileSize + " MB.");
            $("#errorMsgRegion").show();
            //setTimeout(function () {
            //    $('.alert').fadeOut(4000);
            //}, 4000);
            $('#' + fileID).val('');
            //validateForm();
            return false;
        }
    }
}

$('.resp-tabs-list li').click(function (e, c) {
    var $this = $(this);
    addRules();
    isValid = $("#UserDetails").valid();
});

$('#imgSignature').click(function () {
    $("#loading-image").css("display", "");
    if (modelUserId > 0)
        SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSign').attr('src', SRCSign);
    $('#imgSign').load(function () {
        logoWidth = this.width;
        logoHeight = this.height;

        $('#popupbox').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            //$("#imgSign").height($(window).height() - 205);
            $("#imgSign").closest('div').height($(window).height() - 205);
            $("#imgSign").closest('div').css('overflow-y', 'scroll');
            $("#imgSign").height(logoHeight);
        }
        else {
            $("#imgSign").height(logoHeight);
            $("#imgSign").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            //$("#imgSign").width(screen.width - 10);
            //$('#popupbox').find(".modal-dialog").width(screen.width - 10);

            $('#popupbox').find(".modal-dialog").width(screen.width - 250);
            $("#imgSign").width(logoWidth);
        }
        else {
            $("#imgSign").width(logoWidth);
            $('#popupbox').find(".modal-dialog").width(logoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgSign").unbind("error");
    $('#imgSign').attr("src", SRCSign).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
});

$('#imgSignatureSE').click(function () {
    $("#loading-image").css("display", "");
    if (modelUserId > 0)
        SRCSign = proofDocumentURL + "UserDocuments" + "/" + modelUserId + "/" + $('#imgSign').attr('class');
    $('#imgSignSE').attr('src', SRCSign);
    $('#imgSignSE').load(function () {
        logoWidth = this.width;
        logoHeight = this.height;

        $('#popupboxSE').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            //$("#imgSign").height($(window).height() - 205);
            $("#imgSignSE").closest('div').height($(window).height() - 205);
            $("#imgSignSE").closest('div').css('overflow-y', 'scroll');
            $("#imgSignSE").height(logoHeight);
        }
        else {
            $("#imgSignSE").height(logoHeight);
            $("#imgSignSE").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            $('#popupboxSE').find(".modal-dialog").width(screen.width - 250);
            $("#imgSignSE").width(logoWidth);
        }
        else {
            $("#imgSignSE").width(logoWidth);
            $('#popupboxSE').find(".modal-dialog").width(logoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgSignSE").unbind("error");
    $('#imgSignSE').attr("src", SRCSign).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
});

$('#chkSCA').change(function () {
    if ($(this).is(":checked")) {
        $('.chkSCASignUp').show();
    }
    else {
        $('.chkSCASignUp').hide();
    }
});

function addRules() {

    if (userTypeId == 4 || userTypeId == 6 || userTypeId == 8) {
        removeValidationOnTabChangeInstallerDesigner();
    }
    var unitTypeID = $('#UserTypeId').val();
    if ((unitTypeID == 4) || (unitTypeID == 7 && $("#IsPVDUser").prop('checked'))) {
        $("#CECAccreditationNumber").rules("add", {
            required: true,
            messages: {
                required: "CEC Accreditation Number is required."
            }
        });
    }

    if ($("#chkSCA").is(':checked') == true) {
        if ($('#tblDocuments1 tr').length == 0) {
            $(".alert").hide();
            $("#errorMsgRegion").html(closeButton + "Please upload atleast one proof of identity document.");
            $("#errorMsgRegion").show();

            return false;
        }

        else {
            return true;
        }
    }
    else {
        return true;
    }
}

function showErrorMessages(obj, title) {
    if (obj == false) {
        //$(".alert").hide();
        $("#errorMsgRegion").html(closeButton + title);
        $("#errorMsgRegion").show();

        return false;
    }
    else
        return true;
}

function DeleteDocumentFolderOnCancel() {
    var guid = USERID;
}
function BindRoles(userTypeId) {
    $.ajax({
        url: getRoleURL,
        type: "get",
        async: false,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data == 0) {
                $("#RoleID").empty();
                $("#RoleID").append('<option value="">' + "Select" + '</option>');
            }
            else {
                $("#RoleID").empty();
                $("#RoleID").append('<option value="">' + "Select" + '</option>');
                var name = '';
                $.each(data, function (i, role) {
                    if (role.Selected == true) {
                        name = role.Value;
                    }
                    $("#RoleID").append('<option value="' + role.Value + '">' +
                        role.Text + '</option>');
                });
                $("#RoleID").val(parseInt(name));
            }
        },
    });

}

function addressValidation() {
    $("#UnitTypeId").rules("add", {
        required: false,
    });
    $("#UnitNumber").rules("add", {
        required: false,
    });
    if ($("#UnitTypeId").val() == "" && $("#UnitNumber").val() == "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
        $("#UnitNumber").next("span").attr('class', 'field-validation-valid');
        $('#lblStreetNumber').addClass("required");
        $("#StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }

    if ($("#UnitTypeId").val() > 0 && $("#UnitNumber").val() != "") {
        $('#lblStreetNumber').removeClass("required");
        $("#StreetNumber").rules("add", {
            required: false,
        });
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
    }
    if ($("#UnitTypeId").val() > 0 && $("#UnitNumber").val() == "") {
        $("#UnitNumber").rules("add", {
            required: true,
            messages: {
                required: "Unit Number is required."
            }
        });
        $('#lblUnitNumber').addClass("required");
        $('#lblStreetNumber').removeClass("required");
        $("#StreetNumber").rules("add", {
            required: false,
        });
    }
    if ($("#UnitTypeId").val() == "" && $("#UnitNumber").val() != "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $("#UnitNumber").rules("add", {
            required: false,
        });
        $("#UnitTypeId").rules("add", {
            required: false,
        });
        $('#lblStreetNumber').addClass("required");
        $("#StreetNumber").rules("add", {
            required: true,
            messages: {
                required: "Street Number is required."
            }
        });
    }

    if ($('#UsageType').val() == "2" && $('#UserTypeId').val() == 2) {

        $("#WholesalerUnitTypeID").rules("add", {
            required: false,
        });
        $("#WholesalerUnitNumber").rules("add", {
            required: false,
        });
        if ($("#WholesalerUnitTypeID").val() == "" && $("#WholesalerUnitNumber").val().length == 0) {
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
            $("#WholesalerUnitNumber").next("span").attr('class', 'field-validation-valid');
            $('#lblWholesalerStreetNumber').addClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }

        if ($("#WholesalerUnitTypeID").val() > 0 && $("#WholesalerUnitNumber").val().length != 0) {
            $('#lblWholesalerStreetNumber').removeClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: false,
            });
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
        }
        if ($("#WholesalerUnitTypeID").val() > 0 && $("#WholesalerUnitNumber").val().length == 0) {
            $("#WholesalerUnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $('#lblWholesalerUnitNumber').addClass("required");
            $('#lblWholesalerStreetNumber').removeClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: false,
            });
        }
        if ($("#WholesalerUnitTypeID").val() == "" && $("#WholesalerUnitNumber").val().length != 0) {
            $('#lblWholesalerUnitNumber').removeClass("required");
            $('#lblWholesalerUnitTypeID').removeClass("required");
            $("#WholesalerUnitNumber").rules("add", {
                required: false,
            });
            $("#WholesalerUnitTypeID").rules("add", {
                required: false,
            });
            $('#lblWholesalerStreetNumber').addClass("required");
            $("#WholesalerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }
    }
    if ($('#UsageType').val() == "3" && $('#UserTypeId').val() == 2) {

        $("#InvoicerUnitTypeID").rules("add", {
            required: false,
        });
        $("#InvoicerUnitNumber").rules("add", {
            required: false,
        });
        if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length == 0) {
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
            $("#InvoicerUnitNumber").next("span").attr('class', 'field-validation-valid');
            $('#lblInvoicerStreetNumber').addClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }

        if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length != 0) {
            $('#lblInvoicerStreetNumber').removeClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: false,
            });
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
        }
        if ($("#InvoicerUnitTypeID").val() > 0 && $("#InvoicerUnitNumber").val().length == 0) {
            $("#InvoicerUnitNumber").rules("add", {
                required: true,
                messages: {
                    required: "Unit Number is required."
                }
            });
            $('#lblInvoicerUnitNumber').addClass("required");
            $('#lblInvoicerStreetNumber').removeClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: false,
            });
        }
        if ($("#InvoicerUnitTypeID").val() == "" && $("#InvoicerUnitNumber").val().length != 0) {
            $('#lblInvoicerUnitNumber').removeClass("required");
            $('#lblInvoicerUnitTypeID').removeClass("required");
            $("#InvoicerUnitNumber").rules("add", {
                required: false,
            });
            $("#InvoicerUnitTypeID").rules("add", {
                required: false,
            });
            $('#lblInvoicerStreetNumber').addClass("required");
            $("#InvoicerStreetNumber").rules("add", {
                required: true,
                messages: {
                    required: "Street Number is required."
                }
            });
        }
    }
}

function SaveUser() {
    debugger;
    if ($('#UserTypeId').val() == 10) {
        if ($('#tblDocuments tr').length <= 0) {
            alert('Please upload document(s).');
            return false;
        }
    }

    var lstFileName = [];
    for (var i = 0; i < $("[name='FileNamesCreate']").length; i++) {
        lstFileName.push({ Name: $("[name='FileNamesCreate']")[i].id });
    }

    $("#FileNamesCreate").val(JSON.stringify(lstFileName));

    var data = JSON.stringify($('#UserDetails').serializeToJson());
    data = JSON.parse(data);

    data.FileNamesCreate = JSON.stringify(lstFileName);
    isValid = $("#UserDetails").valid();
    if (isValid) {
        if ($("#chkSCA").is(':checked') == true) {
            if ($('#tblDocuments1 tr').length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Please upload atleast one proof of identity document.");
                $("#errorMsgRegion").show();

                return false;
            }
        }
        $.ajax({
            url: getUserCreateURL,
            type: "POST",
            dataType: "json",
            data: JSON.stringify(data),
            async: true,
            processData: false,
            cache: false,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result == "true") {
                    window.location.href = getUserIndexURL;
                }
                else {
                    $("#errorMsgRegion").removeClass("alert-success");
                    $("#errorMsgRegion").addClass("alert-danger");
                    $("#errorMsgRegion").html(closeButton + result);
                    $("#errorMsgRegion").show();
                }
            }
        });
        return false;
    } else {
        return false;
    }
}

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

function GetSolarCompanyBasedOnRA() {

    var url = GetClientNumberOnRAMChangeURL;
    if ($("#UserTypeId").val() == 4 || $("#UserTypeId").val() == 5) {
        GetClientNumberOnRAMChange(2, $("#ResellerID").val() > 0 ? $("#ResellerID").val() : 0, 0, url);
    }
    BindRAMForSolarCompany(0, $("#ResellerID").val());
    FillDropDown('SolarCompanyId', getSCAIsSSCByRAIdURL + '?id=' + $('#ResellerID').val() + '&IsSSC=false', modelSolarCompanyId, true, null);
}

function checkIsWholeSaler(rId) {
    if ($("#UserTypeId").val() == 4) {
        $.ajax({
            url: getCheckWholesalerByRAIdURL + '?id=' + rId,
            type: "get",
            async: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var response = JSON.parse(data)
                if (typeof (response) != "undefined" && response.Table[0] != null) {
                    RoleDropdown_forSolarCompany(response.Table[0].IsWholeSaler);
                }
            },
        });
    }
}

function RoleDropdown_forSolarCompany(IsWholeSaler) {
    if (IsWholeSaler)
        //$("#RoleID").val(1078); //ForLocal
        //$("#RoleID").val(73);    //ForStaging
        $("#RoleID").val(76);    //ForLive
    else
        $("#RoleID").val(38);

}

$('#UsageType').change(function () {
    ShowHideWholeSalerInvoice();
});

function DownloadContractDocument(e) {
    var folderName = e.id;
    folderName = folderName + "\\" + "SAAS";

    window.location.href = BaseURL + 'ViewDownloadFile/User?FileName=' + e.className + '&FolderName=' + folderName;
}

function DeleteContractFileFromFolder(fileDelete) {
    var FolderName = $("#Invoicer").val();
    fileDelete = "SAAS" + "\\" + fileDelete;
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
            {
                url: BaseURL + 'DeleteContractFileFromFolder/User',
                data: { fileName: fileDelete, FolderName: FolderName },
                method: 'get',
                success: function () {

                    $("#SAASContractPath").html("");
                    $(".alert").hide();
                    $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
                    $("#successMsgRegion").show();
                }
            });
    }
}
$('#IsSAASUser').change(function () {
    debugger;
    ShowHideWholeSalerInvoice();
    if ($("#IsSAASUser").is(":checked")) {
        var InvoicerAddressID = $("#InvoicerAddressID").val();
        POInvoicerAddress(InvoicerAddressID);
    }
});
$('#IsWholeSaler').change(function () {
    ShowHideWholeSalerInvoice();
});


$('#RoleID').on('change', function () {
    CheckRoleHasSaasUser($('#RoleID').val());
});

function CheckRoleHasSaasUser(RoleId) {
    $.ajax(
        {
            url: urlRoleHasSaasUser,
            data: { RoleId: RoleId },
            method: 'GET',
            success: function (result) {
                if (result.toLowerCase() == 'true') {
                    $("#IsSAASUser").prop('checked', true);
                }
                else {
                    $("#IsSAASUser").prop('checked', false);
                }
            }
        });
}