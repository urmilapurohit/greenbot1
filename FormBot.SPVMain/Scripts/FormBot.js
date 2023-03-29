var BaseURL = '';
var USERID;
var UploadedDocumentPath;
var MaxImageSize;
var ProjectImagePath;
var MaxLogoSize;
var oldFileName;
var oldLogo;
var logoWidth = 0, logoHeight = 0;
var fileLogoWidth = 0, fileLogoHeight = 0;
var SRCFile, SRCSign;

function FillDropDown(id, url, value, hasSelect, callback, defaultText) {
    $.ajax(
        {
            url: url,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            success: function (success) {
                var options = '';
                if (hasSelect == true) {
                    if (defaultText == undefined || defaultText == null)
                        defaultText = 'Select';

                    options = "<option value=''>" + defaultText + "</option>";
                }

                $.each(success, function (i, val) {
                    options += '<option value = "' + val.Value + '" >' + val.Text + '</option>'
                });

                $("#" + id).html(options);

                if (value && value != '' && value != 0) {
                    $("#" + id).val(value);
                }

                if ($('#' + id).attr('selval') && $('#' + id).attr('selval') > 0) {
                    $("#" + id).val($('#' + id).attr('selval'));
                }

                if ($("#" + id).selectpicker != undefined) {
                    $("#" + id).selectpicker('refresh');
                }

                if (callback != undefined) {
                    callback();
                    //setDropDownWidth(id);
                }
            }
        });
}



function LoadTab(obj) {
    var query = top.location.href.match(/([^\/]+)$/)[1];
    window.location.href = $(obj).attr('data') + '/' + query;
}

//function setDropDownWidth(id) {
//    $('#' + id).next('div').find('ul').addClass("selectpicker123");
//}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    if (!evt.ctrlKey) {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
    return true;
}

function isInt(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function isAlphaNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
        return false;
    }
    return true;
}

function ToggleCheckBox(obj) {
    var checked = false;
    $(obj).closest('div').find('[cat = "chkView"]').each(function () {
        if ($(this).prop('checked'))
            checked = true;
    });
    if (!checked) {
        $(obj).closest('div').find('label').not($('[type=checkbox][cat="chkView"]').parent('label')).addClass("disableCheckbox").find('[type=checkbox]').prop('checked', false);
        $(obj).closest('div').find('input[type=checkbox]').not('[cat="chkView"]').attr('disabled', !checked)
    }
    else
        $(obj).closest('div').find('label').not($('[type=checkbox][cat="chkView"]').parent('label')).removeClass("disableCheckbox");
    $(obj).closest('div').find('input[type=checkbox]').not('[cat="chkView"]').attr('disabled', !checked);
}

function ToggleItems(appType, isVisible) {
    var divs = $('div[apptype="' + appType + '"]');
    var a = isVisible ? $(divs).show() : $(divs).hide();
}

var closeButton = '<button type="button" class="close" onclick="$(this).parent().hide();" aria-hidden="true">&times;</button>';

$(document).off('click', '#btnSave').on('click', '#btnSave', function (e) {
    e.preventDefault();
    e.stopPropagation();

    if (typeof (validateExtraFields) == "function") {
        if (validateExtraFields() == false) {
            return false;
        }
    }

    if (typeof (validateForm) != "function" || (typeof (validateForm) == "function" && validateForm())) {
        $(this).closest('form').ajaxformSubmit();
    }
});

//var submitCallback;
jQuery.fn.ajaxformSubmit = function () {

    var formId;
    return this.each(function () {

        if ($(this).is('form')) {

            formId = $(this).find('.form').attr('id');

            $(".alert-danger").hide();
            $(".alert-success").hide();

            var url = $(this).attr('action');
            $.ajax({
                url: url,
                type: 'POST',
                cache: false,
                data: $(this).serialize()
            });
            //.done(function (response) {
            //    submitCallback(response, $(this)[0].url, formId);
            //});
        }
    });
};

//for DatePicker   
function DateValidation(obj, e) {
    if (e.keyCode == 46 || e.keyCode == 8) {
        $(obj).val("");
        return true;
    }
    else if (e.keyCode == 9) {
        return true;
    }
    else {
        return false;
    }
}

//for readonly
function makeFormReadonly(id) {

    $("#" + id).find('input,textarea').not('.noreadonly').prop('readonly', 'readonly');
    //$("#" + id).find('input[type="button"], button').not(".noreadonly").prop('disabled', 'disabled');

    $("#" + id).find(".dropdown-menu").siblings('button').addClass('disabled');
    $('.dropdown-menu').removeAttr('data-toggle');
    $('.selectpicker ').off('keydown').on('keydown', function (e) {
        e.preventDefault();
        return false;
    });
    $('.fileUpload').hide();
    $('.DeleteFile').hide();
    $('.selectpicker123').prop('disabled', 'disabled');
    //$("#" + id).find("#btnSubmit").hide();
    $("#" + id).find("#IsPrivateStr").addClass('disabled');
    $("#IsPrivateStr").attr("disabled", true);

    $("input[type='checkbox']").click(function (e) {
        var editable = $(e.target).parents(".toppad").find("input[type='text'][readonly]").length == 0;

        if (editable)
            return true;
        else
            return false;
    });
}

// Start : Document Slider
var CurrImage = 0;
selector = $("#MainDiv");
var ChangeCallback;
$(document).off('click', '#Next').on('click', '#Next', function (e) {
    var url = $(this).data('request-url');

    if (selector.find("#Next").hasClass("ic_disable")) {
        e.preventDefault();
    }
    else {
        CurrImage = CurrImage + 1;
        //var file = GetFiles();
        selector.find("#imgFile" + (CurrImage - 1)).hide();
        selector.find("#imgFile" + CurrImage).show();

        //GetFileSrc(file, url);
    }
});

$(document).off('click', '#Previous').on('click', '#Previous', function (e) {

    var url = $(this).data('request-url');

    if (selector.find("#Previous").hasClass("ic_disable")) {
        e.preventDefault();
    }

    else {
        CurrImage = CurrImage - 1;
        // var file = GetFiles();
        selector.find("#imgFile" + CurrImage).show();
        selector.find("#imgFile" + (CurrImage + 1)).hide();

        //GetFileSrc(file, url);
    }
});

function GetFileSrc(file, url) {

    if (file && file.id && file.length) {
        if (selector.find('#imgFile' + CurrImage).attr('src') == "") {
            selector.find("#loading-image") && selector.find("#loading-image").show();
            selector.find("div.divFiles") && selector.find("div.divFiles").hide();

            $.ajax({
                type: "GET",
                url: url,
                data: { id: file.id },
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {
                    selector.find("#imgFile" + CurrImage).attr("file-id", file.id);
                    selector.find("#imgFile" + CurrImage).attr("src", data);
                    hideLoader(selector);
                },
                error: function (data) {
                    hideLoader(selector);
                },
                stop: function (data) {
                    hideLoader(selector);
                }

            });
        }

        selector.find("#imgFile" + CurrImage).attr("style", "width:100%;height:100%");
        if (ChangeCallback && typeof (ChangeCallback) == 'function')
            ChangeCallback();
        selector.find('#Next').addClass("ic_disable");

        selector.find('#Previous').removeClass("ic_disable");
        if (file.length > 1 && CurrImage < file.length - 1) {
            selector.find('#Next').removeClass("ic_disable");
        }
        if (CurrImage == 0) {
            selector.find('#Previous').addClass("ic_disable");
        }
    }
}

function hideLoader(selector) {
    selector.find("#loading-image") && selector.find("#loading-image").hide();
    selector.find("div.divFiles") && selector.find("div.divFiles").show();
}

function ToggleCheckBox(obj) {
    var checked = false;
    $(obj).closest('div').find('[cat = "chkView"]').each(function () {
        if ($(this).prop('checked'))
            checked = true;
    });
    if (!checked) {
        $(obj).closest('div').find('label').not($('[type=checkbox][cat="chkView"]').parent('label')).addClass("disableCheckbox");
        $(obj).closest('div').not($('[type=checkbox][cat="chkView"]')).find('[type=checkbox]').prop('checked', false);
        $(obj).closest('div').find('input[type=checkbox]').not('[cat="chkView"]').attr('disabled', !checked)
    }
    else
        $(obj).closest('div').find('label').not($('[type=checkbox][cat="chkView"]').parent('label')).removeClass("disableCheckbox");
    $(obj).closest('div').find('input[type=checkbox]').not('[cat="chkView"]').attr('disabled', !checked);
}
//End : Document Slider

//Common function for user module
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

function CheckShowMessages() {
    var isResponse = true;
    isResponse = showErrorMessages(chkUserName, "User with same user name already exists. Please try with different user name.");
    if (!isResponse)
        return isResponse;
    isResponse = showErrorMessages(chkCompanyABN, "User with same company ABN already exists. Please try with different company ABN.");
    if (!isResponse)
        return isResponse;
    isResponse = showErrorMessages(chkCECAccreditationNumber, "User with same cec accreditation number already exists. Please try with different cec accreditation number.");
    if (!isResponse)
        return isResponse;
    isResponse = showErrorMessages(chkLoginCompanyName, "User with same login company name already exists. Please try with different login company name.");
    if (!isResponse)
        return isResponse;
    return isResponse;
}

function checkExist(obj, title, contactUserId, contactUserTypeId) {
    var UserTypeId = $('#UserTypeId').val();
    var fieldName = obj.id;
    var chkvar = '';
    var uservalue = obj.value;
    if ((fieldName == "CompanyABN" && (UserTypeId == 2 || UserTypeId == 6)) || fieldName == "UserView_CompanyABN") {
        if (fieldName == "UserView_CompanyABN") {
            UserTypeId = contactUserTypeId;
            userID = contactUserId;
        }
        if (uservalue != "" && uservalue != undefined) {
            $.ajaxSetup({ cache: false });
            $.ajax({
                url: BaseURL + 'CheckUserExist/User',
                data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID, UserTypeId: UserTypeId },
                contentType: 'application/json',
                method: 'get',
                cache: false,
                async: false,
                success: function (data) {
                    if (data == false) {
                        chkvar = false;
                        $(".alert").hide();
                        $("#errorMsgRegion").html(closeButton + "User with same " + title + " already exists. Please try with different " + title + ".");
                        $("#errorMsgRegion").show();

                    }
                    else {
                        chkvar = true;
                    }
                    if (fieldName == "UserName") {
                        chkUserName = chkvar;
                    }
                    else if (fieldName == "CompanyABN" || fieldName == "UserView_CompanyABN") {
                        chkCompanyABN = chkvar;
                    }
                    else if (fieldName == "CECAccreditationNumber") {
                        chkCECAccreditationNumber = chkvar;
                    }
                    else if (fieldName == "LoginCompanyName") {
                        chkLoginCompanyName = chkvar;
                    }
                    return false;
                }
            });
        }
        //}
    }
    else {
        if (uservalue != "" && uservalue != undefined) {
            $.ajaxSetup({ cache: false });
            $.ajax(
                 {
                     url: BaseURL + 'CheckUserExist/User',
                     data: { UserValue: uservalue, FieldName: fieldName, userId: userID, resellerID: resellerID, UserTypeId: UserTypeId },
                     contentType: 'application/json',
                     method: 'get',
                     async: false,
                     cache: false,
                     success: function (data) {
                         if (data == false) {
                             chkvar = false;
                             $(".alert").hide();
                             $("#errorMsgRegion").html(closeButton + "User with same " + title + " already exists. Please try with different " + title + ".");
                             $("#errorMsgRegion").show();

                         }
                         else {
                             chkvar = true;
                         }
                         if (fieldName == "UserName") {
                             chkUserName = chkvar;
                         }
                         else if (fieldName == "CompanyABN") {
                             chkCompanyABN = chkvar;
                         }
                         else if (fieldName == "CECAccreditationNumber") {
                             chkCECAccreditationNumber = chkvar;
                         }
                         else if (fieldName == "LoginCompanyName") {
                             chkLoginCompanyName = chkvar;
                         }
                         return false;
                     }
                 });
        }
    }
}

// file upload
$(function () {
    'use strict';
    var url = BaseURL + 'Upload/User';

    //signature upload
    $('#uploadBtnSignature').fileupload({
        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var guid = USERID;
                    var signName = $('#imgSign').attr('class');
                    $("[name='Signature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromSignatureOnUpload(signName, guid, oldFileName);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    var SRC = imagePath + "/" + data.result[i].FileName.replace("%", "$");

                    SRCSign = SRC;

                    //$('#imgSign').attr('src', SRCSign).load(function () { logoWidth = this.width; logoHeight = this.height });
                    $('#imgSign').attr('class', data.result[i].FileName.replace("%", "$"));

                    $('#imgSignature').show();


                    $('<input type="hidden">').attr({
                        name: 'Signature',
                        value: data.result[i].FileName.replace("%", "$"),
                        id: data.result[i].FileName.replace("%", "$"),
                    }).appendTo('form');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();

            }
        },

        progressall: function (e, data) {

        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
                        $("#errorMsgRegion").show();
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        $("#errorMsgRegion").show();

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
                    $("#errorMsgRegion").show();

                    return false;
                }
            }
            if (mimeType != "image") {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Please upload a file with .jpg , .jpeg or .png extension.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;


            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();

            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;

        },
        formData: { userId: USERID },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
.parent().addClass($.support.fileInput ? undefined : 'disabled');

    //logo upload
    $('#uploadBtnLogo').fileupload({
        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {

                    var guid = USERID;
                    var signName = $('#imgLG').attr('class');
                    $("[name='Signature']").each(function () {
                        $(this).remove();
                    });
                    if (signName != null && signName != "") {
                        DeleteFileFromLogoOnUpload(signName, guid, oldLogo);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "UserDocuments" + "/" + guid;
                    var SRC = imagePath + "/" + data.result[i].FileName.replace("%", "$");
                    $('#imgLG').attr('src', SRC);
                    $('#imgLG').attr('class', data.result[i].FileName.replace("%", "$"));

                    $('#imgViewLogo').show();

                    $('<input type="hidden">').attr({
                        name: 'Signature',
                        value: data.result[i].FileName.replace("%", "$"),
                        id: data.result[i].FileName.replace("%", "$"),
                    }).appendTo('form');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();

            }
        },

        progressall: function (e, data) {

        },

        singleFileUploads: false,
        send: function (e, data) {

            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
                        $("#errorMsgRegion").show();
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxLogoSize)) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB");
                        $("#errorMsgRegion").show();

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxLogoSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB");
                    $("#errorMsgRegion").show();

                    return false;
                }
            }
            if (mimeType != "image") {
                $(".alert").hide();
                $("#errorMsgRegion").html(closeButton + "Please upload a file with .jpg , .jpeg or .png extension.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;


            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            return true;
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        formData: { userId: USERID },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
.parent().addClass($.support.fileInput ? undefined : 'disabled');

    //mulitiple proof upload
    $('#uploadBtn1').fileupload({

        url: url,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {
                    var rowcount = $('#tblDocuments tr').length;
                    var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var documentId = "document" + count;
                    var content = "<tr style='margin-top:30px' id= " + data.result[i].FileName.replace("%", "$") + " >"
                    content += '<td class="tdCount col-sm-2" >' + count + '.' + ' </td>';
                    content += '<td class="col-sm-6" style="color:#494949">' + data.result[i].FileName.replace("%", "$") + ' </td>';

                    if (mimeType == "image") {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '"  class="' + data.result[i].FileName.replace("%", "$") + '" title="Preview" onclick="OpenDocument(this)"></td>';
                    }
                    else {
                        content += '<td  class="col-sm-2" style="color:blue"><img src=' + ProjectImagePath + 'images/view-icon.png style="cursor: pointer" id="' + data.result[i].FileName.replace("%", "$") + '" class="' + data.result[i].FileName.replace("%", "$") + '" title="Download" onclick="DownloadDocument(this)"></td>';
                    }
                    content += '<td style="color:blue"><img src=' + ProjectImagePath + 'images/delete-icon.png style="cursor: pointer id="signDelete" title="Delete" onclick="DeleteFileFromFolder(\'' + data.result[i].FileName + '\')"></td>';
                    content += "</tr>"

                    $('#tblDocuments').append(content);
                    $('<input type="hidden">').attr({
                        name: 'FileNamesCreate',
                        value: data.result[i].FileName.replace("%", "$"),
                        id: data.result[i].FileName.replace("%", "$"),
                    }).appendTo('form');

                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName.replace("%", "$"));
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));

                }
            }
            if (UploadFailedFiles.length > 0) {

                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();

            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();

            }
        },

        progressall: function (e, data) {

        },

        singleFileUploads: false,
        send: function (e, data) {
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + "Please upload small filename.");
                        $("#errorMsgRegion").show();
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {

                        $(".alert").hide();
                        $("#successMsgRegion").hide();
                        $("#errorMsgRegion").html(closeButton + " " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB");
                        $("#errorMsgRegion").show();

                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    $(".alert").hide();
                    $("#successMsgRegion").hide();
                    $("#errorMsgRegion").html(closeButton + "Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB");
                    $("#errorMsgRegion").show();

                    return false;
                }
            }
            $(".alert").hide();
            $("#errorMsgRegion").html("");
            $("#errorMsgRegion").hide();
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        formData: { userId: USERID },
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput)
.parent().addClass($.support.fileInput ? undefined : 'disabled');
});

//Proof of Open upload
function OpenDocument(e) {
    var guid = USERID;
    var proofDocumentURL = UploadedDocumentPath;    
    var proofDocumentType = "";
    //5360 = last UserDocumentId before sca mulitple proof category (staging)
    //6124 = last UserDocumentId before sca mulitple proof category (live)
    if (typeof (e.attributes.proofdocumenttype) != 'undefined' && (typeof (e.attributes.userdocumentid) == 'undefined' || parseInt(e.attributes.userdocumentid.value) > 6124)) {
        if (e.attributes.proofdocumenttype.value == 1) {
            proofDocumentType = "/" + "DirectorsDriversLicense";
        }
        else if (e.attributes.proofdocumenttype.value == 2) {
            proofDocumentType = "/" + "AccreditationDocumentations";
        }
        else if (e.attributes.proofdocumenttype.value == 3) {
            proofDocumentType = "/" + "ProofOfBusinessAddress";
        }
        else if (e.attributes.proofdocumenttype.value == 4) {
            proofDocumentType = "/" + "PublicLiabilityInsurance";
        }
    }

    var imagePath = proofDocumentURL + "/" + "UserDocuments" + "/" + guid + proofDocumentType;
    var SRC = imagePath + "/" + e.className;

    $("#loading-image").css("display", "");

    $('#imgFile').attr("src", SRC).load(function () {
        fileLogoWidth = this.width;
        fileLogoHeight = this.height;

        $('#popupProof').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= fileLogoHeight) {
            $("#imgFile").closest('div').height($(window).height() - 150);
            $("#imgFile").closest('div').css('overflow-y', 'scroll');
            $("#imgFile").height(fileLogoHeight);

            //$("#imgFile").height($(window).height() - 150);
        }
        else {
            $("#imgFile").height(fileLogoHeight);
            $("#imgFile").closest('div').removeAttr('style');
        }

        if (screen.width <= fileLogoWidth || fileLogoWidth >= screen.width - 250) {
            //$("#imgFile").width(screen.width - 10);
            //$('#popupProof').find(".modal-dialog").width(screen.width - 10);
            $('#popupProof').find(".modal-dialog").width(screen.width - 250);
            $("#imgFile").width(fileLogoWidth);
        }
        else {
            $("#imgFile").width(fileLogoWidth);
            $('#popupProof').find(".modal-dialog").width(fileLogoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgFile").unbind("error");
    $('#imgFile').attr('src', SRC).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}

//$('#popupProof').on('shown.bs.modal', function () {
//    if ($(window).height() <= fileLogoHeight)
//        $("#imgFile").height($(window).height() - 150);
//    else
//        $("#imgFile").height(fileLogoHeight);

//    if (screen.width <= fileLogoWidth) {
//        $("#imgFile").width(screen.width - 10);
//        $('#popupProof').find(".modal-dialog").width(screen.width - 10);
//    }
//    else {
//        $("#imgFile").width(fileLogoWidth);
//        $('#popupProof').find(".modal-dialog").width(fileLogoWidth);
//    }
//});

//Proof of Download Document
function DownloadDocument(e) {
    //505 = last UserDocumentId before sca mulitple proof category (Local)
    //5360 = last UserDocumentId before sca mulitple proof category (staging)
    //6124 = last UserDocumentId before sca mulitple proof category (live)
    //$("#loading-image").show();
    var foldername = USERID;
    if (typeof (e.attributes.proofdocumenttype) != 'undefined' && (typeof (e.attributes.userdocumentid) == 'undefined' || parseInt(e.attributes.userdocumentid.value) > 6124)) {
        if (e.attributes.proofdocumenttype.value == 1) {
            foldername = foldername + "\\" + "DirectorsDriversLicense";
        }
        else if (e.attributes.proofdocumenttype.value == 2) {
            foldername = foldername + "\\" + "AccreditationDocumentations";
        }
        else if (e.attributes.proofdocumenttype.value == 3) {
            foldername = foldername + "\\" + "ProofOfBusinessAddress";
        }
        else if (e.attributes.proofdocumenttype.value == 4) {
            foldername = foldername + "\\" + "PublicLiabilityInsurance";
        }
    }
    var FileName = e.id;
    window.location.href = BaseURL + 'ViewDownloadFile/User?FileName=' + e.className + '&FolderName=' + foldername;

    //$("#loading-image").hide();
}

function DeleteFileFromUserOnCancel(fileNames, guid) {
    $.ajax(
        {
            url: BaseURL + 'DeleteFileFromFolder/User',
            data: { fileName: fileNames, FolderName: guid },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {

            },
        });
}



function DeleteFileFromSignatureOnUpload(fileNames, guid, oldFileName) {
    $.ajax(
        {
            url: BaseURL + 'DeleteSignatureFileFromFolder/User',
            data: { fileName: fileNames, FolderName: guid, OldFileName: oldFileName },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {

            },
        });
}

function DeleteFileFromLogoOnUpload(fileNames, guid, oldLogo, url) {
    var deleteURL = url ? url + "User/DeleteLogoFromFolder" : BaseURL + 'DeleteLogoFromFolder/User';
    $.ajax(
        {
            url: deleteURL,
            data: { fileName: fileNames, FolderName: guid, OldLogo: oldLogo },
            contentType: 'application/json',
            method: 'get',
            success: function (data) {

            },
        });
}

function DeleteFileFromDatabase(UserDocumentID, userId, documentpath, documentType) {
    var tableId = 'tblDocuments';
    //505 = last UserDocumentId before sca mulitple proof category (Local)
    //5360 = last UserDocumentId before sca mulitple proof category (staging)
    //6124 = last UserDocumentId before sca mulitple proof category (live)
    if (typeof (documentType) != 'undefined' && UserDocumentID > 6124) {
        if (documentType == 1) {
            documentpath = "DirectorsDriversLicense" + "\\" + documentpath;
            tableId = 'tblDirectorsDriversLicense';
        }
        else if (documentType == 2) {
            documentpath = "AccreditationDocumentations" + "\\" + documentpath;
            tableId = 'tblAccreditationDocumentations';
        }
        else if (documentType == 3) {
            documentpath = "ProofOfBusinessAddress" + "\\" + documentpath;
            tableId = 'tblProofOfBusinessAddress';
        }
        else if (documentType == 4) {
            documentpath = "PublicLiabilityInsurance" + "\\" + documentpath;
            tableId = 'tblPublicLiabilityInsurance';
        }
    }

    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
            {
                url: BaseURL + 'DeleteFileByID/User',
                data: { UserDocumentID: UserDocumentID, UserId: userId, Documentpath: documentpath },
                contentType: 'application/json',
                method: 'get',
                success: function () {
                    $("#" + tableId + "").find('tr').each(function () {
                        if ($(this).attr('id') == UserDocumentID)
                            $(this).remove();
                    });

                    $("#" + tableId + " tr").each(function () {
                        var trNumber = $(this).index() + 1;
                        $(this).find('td.tdCount').html(trNumber);
                    });

                    $(".alert").hide();
                    $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
                    $("#successMsgRegion").show();

                }
            });
    }
}

function DeleteFileFromFolder(fileDelete, documentType) {

    var FolderName = USERID;
    var tableId = 'tblDocuments';

    if (typeof (documentType) != 'undefined') {
        if (documentType == 1) {
            fileDelete = "DirectorsDriversLicense" + "\\" + fileDelete;
            tableId = 'tblDirectorsDriversLicense';
        }
        else if (documentType == 2) {
            fileDelete = "AccreditationDocumentations" + "\\" + fileDelete;
            tableId = 'tblAccreditationDocumentations';
        }
        else if (documentType == 3) {
            fileDelete = "ProofOfBusinessAddress" + "\\" + fileDelete;
            tableId = 'tblProofOfBusinessAddress';
        }
        else if (documentType == 4) {
            fileDelete = "PublicLiabilityInsurance" + "\\" + fileDelete;
            tableId = 'tblPublicLiabilityInsurance';
        }
    }

    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
            {
                url: BaseURL + 'DeleteFileFromFolder/User',
                data: { fileName: fileDelete, FolderName: FolderName },
                method: 'get',
                success: function () {
                    document.getElementById(tableId).deleteRow(fileDelete.split("\\")[1]);
                    $("#" + tableId + "").find('tr').each(function () {
                        var trNumber = $(this).index() + 1;
                        $(this).find('td.tdCount').html(trNumber);
                    });
                    if (typeof (documentType) != 'undefined') {
                        $("[file='" + fileDelete.split("\\")[1] + "']").attr('value', '0');
                    }
                    else {
                        $("[name='FileNamesCreate']").each(function () {
                            if ($(this).attr('id') == fileDelete)
                                $(this).remove();
                        });
                    }

                    $(".alert").hide();
                    $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
                    $("#successMsgRegion").show();

                    return false;
                }
            });
    }

    //var FolderName = USERID;
    //if (confirm('Are you sure you want to delete this file ?')) {
    //    $.ajax(
    //    {
    //        url: BaseURL + 'DeleteFileFromFolder/User',
    //        data: { fileName: fileDelete, FolderName: FolderName },
    //        method: 'get',
    //        success: function () {
    //            document.getElementById("tblDocuments").deleteRow(fileDelete);
    //            $("#tblDocuments tr").each(function () {
    //                var trNumber = $(this).index() + 1;
    //                $(this).find('td.tdCount').html(trNumber);
    //            });
    //            $("[name='FileNamesCreate']").each(function () {

    //                if ($(this).attr('id') == fileDelete)
    //                    $(this).remove();
    //            });

    //            $(".alert").hide();
    //            $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
    //            $("#successMsgRegion").show();

    //            return false;
    //        }
    //    });
    //}
}

function deleteImage(imageId) {
    var FolderName = USERID;
    var OldFileName = oldFileName;
    var fileDelete = $('#imgSign').attr('class');
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
       {
           url: BaseURL + 'DeleteSignatureFileFromFolder/User',
           data: { fileName: fileDelete, FolderName: FolderName, OldFileName: OldFileName },
           contentType: 'application/json',
           method: 'get',
           success: function () {
               var sign = $('#imgSign').attr('class');
               $("[name='Signature']").each(function () {
                   $(this).remove();
               });
               $('#imgSign').removeAttr('src');
               $('#imgSign').removeAttr('class');
               $('#popupbox').modal('hide');
               $("#imgSignature").hide();
               $(".alert").hide();
               $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
               $("#successMsgRegion").show();

               return false;
           }
       });
    }
}

function deleteLogoImage(imageId) {
    var FolderName = USERID;
    var Oldlogo = oldLogo;
    var fileDelete = $('#imgLG').attr('class');
    if (confirm('Are you sure you want to delete this file ?')) {
        $.ajax(
       {
           url: BaseURL + 'DeleteLogoFromFolder/User',
           data: { fileName: fileDelete, FolderName: FolderName, OldLogo: Oldlogo },
           contentType: 'application/json',
           method: 'get',
           success: function () {
               var sign = $('#imgLG').attr('class');
               $("[name='Signature']").each(function () {
                   $(this).remove();
               });
               $("[name='Logo']").each(function () {
                   $(this).remove();
               });
               $('#imgLG').removeAttr('src');
               $('#imgLG').removeAttr('class');
               $('#popupboxlogo').modal('hide');
               $("#imgViewLogo").hide();
               $(".alert").hide();
               $("#successMsgRegion").html(closeButton + "File has been Deleted successfully.");
               $("#successMsgRegion").show();

               return false;
           }
       });
    }
}

function isDecimal(event, precision, exponent, obj) {
    var $this = $(obj);
    if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
       ((event.which < 48 || event.which > 57) &&
       (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(obj).val();

    if (text.split('.').length < 2 && event.which != 8 && event.which != 0) {
        if (text.split('.')[0].length >= precision && event.which != 46) { //&& (text.split('.').length > 1 ? text.split('.')[1].length : text.split('.')[1].length) >= exponent) {
            return false;
        }
    }



    if ((event.which == 46) && (text.indexOf('.') == -1)) {
        setTimeout(function () {
            if ($this.val().substring($this.val().indexOf('.')).length > 3) {
                $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
            }
        }, 1);
    }

    if ((text.indexOf('.') != -1) && (text.substring(text.indexOf('.')).length > exponent) && (event.which != 0 && event.which != 8) && ($(obj)[0].selectionStart >= text.length - 2)) {
        event.preventDefault();
    }
}


//Change by create and edit
//function DeleteDocumentFolderOnCancel() {
//    var guid = USERID;
//    var Name = [];
//    Name = document.getElementsByName("FileNamesCreate");
//    var Sign = document.getElementsByName("Signature");
//    var SignName = Sign[0].id;
//    if (Name.length > 0) {
//        for (var i = 0; i < Name.length; i++) {
//            var docname = Name[i].id;
//            DeleteFileFromUserOnCancel(docname, guid);
//        }
//    }
//    if (SignName != null && SignName != "") {
//        DeleteFileFromUserOnCancel(SignName, guid);
//    }

//}


//function DeleteDocumentFolderOnCancel() {
//    var guid = '@Model.Guid';
//    $.ajax(
// {
//     url: '@Url.Action("DeleteDocumentFolderOnCancel", "User")',
//     data: { Guid: guid },
//     contentType: 'application/json',
//     method: 'get',
//     success: function (data) {
//     },
// });
//}

function isDecimal(event, precision, exponent, obj) {
    var $this = $(obj);
    if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
       ((event.which < 48 || event.which > 57) &&
       (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(obj).val();

    if (text.split('.').length < 2 && event.which != 8 && event.which != 0) {
        if (text.split('.')[0].length >= precision && event.which != 46) { //&& (text.split('.').length > 1 ? text.split('.')[1].length : text.split('.')[1].length) >= exponent) {
            return false;
        }
    }



    if ((event.which == 46) && (text.indexOf('.') == -1)) {
        setTimeout(function () {
            if ($this.val().substring($this.val().indexOf('.')).length > 3) {
                $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
            }
        }, 1);
    }

    if ((text.indexOf('.') != -1) && (text.substring(text.indexOf('.')).length > exponent) && (event.which != 0 && event.which != 8) && ($(obj)[0].selectionStart >= text.length - 2)) {
        event.preventDefault();
    }
}

function PrintDecimal(value) {
    if (typeof (value) == 'number') {
        if (value === parseInt(value, 10)) {
            return value + '.00';
        } else { return value.toFixed(2); }
    } else { return '0.00'; }
}

//Email configuration not required
function EmailAccountConfigureErrorMessage() {
    $(".alert").hide();
    $("#errorMsgRegion").removeClass("alert-success");
    $("#errorMsgRegion").addClass("alert-danger");
    $("#errorMsgRegion").html(closeButton + 'Please configure your email account');
    $("#errorMsgRegion").show();
}

$("#UnitTypeId").change(function () {
    if ($('#UnitTypeId option:selected').val() == "") {
        $('#lblUnitNumber').removeClass("required");
        $('#lblUnitTypeID').removeClass("required");
        $('#lblStreetNumber').addClass("required");
    }
    else {
        $('#lblUnitNumber').addClass("required");
        $('#lblUnitTypeID').addClass("required");
        $('#lblStreetNumber').removeClass("required");
    }
});

function GetClientNumberOnRAMChange(userTypeId, resellerId, userId, url) {
    $.ajax({
        type: 'get',
        url: url,
        dataType: 'json',
        data: { userTypeId: userTypeId, resellerId: resellerId, userId: userId },
        success: function (data) {
            if (data) {
                if (userTypeId == data.userTypeIdOfPrefix) {
                    if (data.clientNumber && data.clientNumber != null && data.clientNumber != "") {
                        $("#ClientNumber").val(data.clientNumber);
                        $("#errorMsgRegion").hide();
                    }
                    else {
                        showErrorMessage("Reseller or RAM don't have client code.");
                        $("#ClientNumber").val("");
                    }

                }
                else {
                    if (data.clientNumber && data.clientNumber != null && data.clientNumber != "") {
                        $("#ClientNumber").val(data.clientNumber);
                        showErrorMessage("Selected account manager don't have client code, so displying client number is based on reseller." + '<br/>' + "When account manager set its own client code then client number will be set accordingly.");
                    }
                    else {
                        showErrorMessage("Reseller or RAM don't have client code.");
                        $("#ClientNumber").val("");
                    }
                }
            }
        },
        error: function (ex) {
            alert('Failed to get client number.');
        }
    });
}

function FillDropDownUser(id, url, value, hasSelect, callback, defaultText) {
    $.ajax(
        {
            url: url,
            contentType: 'application/json',
            dataType: 'json',
            method: 'get',
            cache: false,
            success: function (success) {
                var data = JSON.parse(success.replace(/&quot;/g, '"'))
                var options = '';
                if (hasSelect == true) {
                    if (defaultText == undefined || defaultText == null)
                        defaultText = 'Select';

                    options = "<option value=''>" + defaultText + "</option>";
                }

                $.each(data, function (i, val) {
                    options += '<option value = "' + val.Value + '" >' + val.Text + '</option>'
                });

                $("#" + id).html(options);

                if (value && value != '' && value != 0) {
                    $("#" + id).val(value);
                }

                if ($('#' + id).attr('selval') && $('#' + id).attr('selval') > 0) {
                    $("#" + id).val($('#' + id).attr('selval'));
                }

                if ($("#" + id).selectpicker != undefined) {
                    $("#" + id).selectpicker('refresh');
                }

                if (callback != undefined) {
                    callback();
                    //setDropDownWidth(id);
                }
            }
        });
}

function CheckSpecialCharInFileName(filename) {
    if (filename.indexOf('[') != -1 || filename.indexOf('^') != -1 || filename.indexOf(']') != -1) {
        return true;
    } else {
        return false;
    }
}