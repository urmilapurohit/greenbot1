$(document).ready(function () {
    var count = $('#intPhoto li').length;
    $('#spanPhoto').html('(' + count + ')');
    var count = $('#SerialNumber li').length;
    $('#spanSerial').html('(' + count + ')');
    ShowHidePhotos();
});
$(function () {
    'use strict';
    var BaseURL = ProjectImagePath + 'Job/';
    var USERID = Model_JobID;
    var urlPhoto = BaseURL + 'UploadPhoto/Job';
    var urlSerial = BaseURL + 'UploadSerial/Job';
    //mulitiple proof upload
    $('#uploadBtnPhoto').fileupload({
        url: urlPhoto,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            var UploadFailedFilesType = [];
            UploadFailedFilesType.length = 0;
            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {
                    var rowcount = $('#intPhoto').length;
                    var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var documentId = "document" + count;
                    var ids = data.result[i].FileName.replace("%", "$");
                    var sortFileName = data.result[i].FileName.length;
                    if (sortFileName > 20) {
                        var path = ids.substring(0, 20) + '...';
                    }
                    else {
                        path = ids;
                    }
                    var content = "<li class='checkbox' style='margin-left:2px;'>"
                    content += "<label> <input type='checkbox' name='chkBtnPhoto' value=\"" + ids + "\" >";
                    if (isJobPhotoView == 'true') {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' onclick='OpenPhoto(this);' title=\"" + ids + "\"  class=\"" + ids + "\" id=\"" + ids + "\">" + path + " </a>";
                    }
                    if (isJobPhotoView == 'false') {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' title=\"" + ids + "\" class=\"" + ids + "\" id=\"" + ids + "\">" + path + " </a>";
                    }
                    content += "</label>"
                    content += "</li>"

                    $('#intPhoto').append(content);
                    var count = $('#intPhoto li').length;
                    $('#spanPhoto').html('(' + count + ')');

                    $('<input type="hidden">').attr({
                        name: 'FileNamesCreate',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');

                    totalInstallation = totalInstallation + 1;
                    OpecityOfDownloadAllPhotos($(".downloadInstallation"), totalInstallation);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "NotImage") {
                    UploadFailedFilesType.push(data.result[i].FileName);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);

                }
            }
            if (UploadFailedFilesType.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesType.length + " " + "Uploaded file is not .jpg , .jpeg or .png extension.");
                $("#errorMsgRegion").show();

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
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0 && UploadFailedFilesType.length == 0) {
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
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');

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
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');

                    return false;
                }
            }
            if (data.files.length == 1) {
                if (mimeType != "image") {
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + "Please upload a file with .jpg , .jpeg or .png extension.");
                    $("#errorMsgRegion").show();
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');

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

    $('#uploadBtnSerial').fileupload({

        url: urlSerial,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            var UploadFailedFilesType = [];
            UploadFailedFilesType.length = 0;
            var UploadFailedFilesName = [];
            UploadFailedFilesName.length = 0;
            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {
                    var rowcount = $('#SerialNumber').length;
                    var count = rowcount + 1;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var documentId = "document" + count;
                    var ids = data.result[i].FileName.replace("%", "$");
                    var sortFileName = data.result[i].FileName.length;
                    if (sortFileName > 20) {
                        var path = ids.substring(0, 20) + '...';
                    }
                    else {
                        path = ids;
                    }
                    var content = "<li class='checkbox' style='margin-left:2px;'>"
                    content += "<label> <input type='checkbox' name='chkBtnSerial' value=\"" + ids + "\" >";
                    if (isJobPhotoView == 'true') {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' title=\"" + ids + "\" onclick='OpenSerial(this);' class=\"" + ids + "\" id=\"" + ids + "\">" + path + " </a>";
                    }
                    if (isJobPhotoView == 'false') {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' title=\"" + ids + "\" class=\"" + ids + "\" id=\"" + ids + "\">" + path + " </a>";
                    }
                    content += "</label>"
                    content += "</li>"

                    $('#SerialNumber').append(content);
                    var count = $('#SerialNumber li').length;
                    $('#spanSerial').html('(' + count + ')');

                    $('<input type="hidden">').attr({
                        name: 'FileNamesCreate',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');

                    totalSerialNumber = totalSerialNumber + 1;
                    OpecityOfDownloadAllPhotos($(".downloadSerialNumber"), totalSerialNumber);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "NotImage") {
                    UploadFailedFilesType.push(data.result[i].FileName);
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesName.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);

                }
            }
            if (UploadFailedFilesType.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesType.length + " " + "Uploaded file is not .jpg , .jpeg or .png extension.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFiles.length + " " + "File has not been uploaded.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;
            }
            if (UploadFailedFilesName.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesName.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;
            }
            if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0 && UploadFailedFilesType.length == 0) {
                if (count - 1 == 0) {
                    checkBusinessRulesForPhotos();
                }
                else {
                    $(".alert").hide();
                    $("#errorMsgRegion").hide();
                    $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                    $("#successMsgRegion").show();

                }
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
            if (data.files.length == 1) {
                if (mimeType != "image") {
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + "Please upload a file with .jpg , .jpeg or .png extension.");
                    $("#errorMsgRegion").show();
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');

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

$("#btnClosePopUpPhotoBox").click(function () {
    $('#popupPhotoBox').modal('toggle');
});

function OpenPhoto(e) {
    var USERID = Model_JobID;
    var guid = USERID;
    var proofDocumentURL = UploadedDocumentPath;
    var imagePath = proofDocumentURL + "/" + "JobDocuments" + "/" + guid;
    var SRC = imagePath + "/" + e.className;

    $("#loading-image").css("display", "");

    $('#imgPhoto').attr("src", SRC).load(function () {
        logoWidth = this.width;
        logoHeight = this.height;
        $('#popupPhotoBox').modal({ backdrop: 'static', keyboard: false });
        if ($(window).height() <= logoHeight) {
            $("#imgPhoto").closest('div').height($(window).height() - 150);
            $("#imgPhoto").closest('div').css('overflow-y', 'scroll');
            $("#imgPhoto").height(logoHeight);
        }
        else {
            $("#imgPhoto").height(logoHeight);
            $("#imgPhoto").closest('div').removeAttr('style');
        }
        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            $('#popupPhotoBox').find(".modal-dialog").width(screen.width - 250);
            $("#imgPhoto").width(logoWidth);
        }
        else {
            $("#imgPhoto").width(logoWidth);
            $('#popupPhotoBox').find(".modal-dialog").width(logoWidth);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgPhoto").unbind("error");
    $('#imgPhoto').attr('src', SRC).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}

$("#btnClosePopUpSerialBox").click(function () {
    $('#popupSerialBox').modal('toggle');
});

function OpenSerial(e) {
    var USERID = Model_JobID;
    var guid = USERID;
    var proofDocumentURL = UploadedDocumentPath;
    var imagePath = proofDocumentURL + "/" + "JobDocuments" + "/" + guid;
    var SRC = imagePath + "/" + e.className;

    $("#loading-image").css("display", "");

    $('#imgSerial').attr("src", SRC).load(function () {
        logoWidth1 = this.width;
        logoHeight1 = this.height;
        $('#popupSerialBox').modal({ backdrop: 'static', keyboard: false });
        if ($(window).height() <= logoHeight1) {
            $("#imgSerial").closest('div').height($(window).height() - 150);
            $("#imgSerial").closest('div').css('overflow-y', 'scroll');
            $("#imgSerial").height(logoHeight1);
        }
        else {
            $("#imgSerial").height(logoHeight1);
            $("#imgSerial").closest('div').removeAttr('style');
        }
        if (screen.width <= logoWidth1 || logoWidth1 >= screen.width - 250) {
            $('#popupSerialBox').find(".modal-dialog").width(screen.width - 250);
            $("#imgSerial").width(logoWidth1);
        }
        else {
            $("#imgSerial").width(logoWidth1);
            $('#popupSerialBox').find(".modal-dialog").width(logoWidth1);
        }
        $("#loading-image").css("display", "none");
    });
    $("#imgSerial").unbind("error");
    $('#imgSerial').attr('src', SRC).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}
function DeletePhoto() {
    if ($('input[name="chkBtnPhoto"]:checked').length > 0) {
        if (confirm('Are you sure you want to delete this file ?')) {
            photo = $.map($('input[name="chkBtnPhoto"]:checked'), function (n, i) {
                return n.value;
            }).join('?');
            DeletePhotoFromFolder(photo);
        }
    }
}

function DeletePhotoFromFolder(fileDelete) {
    var USERID = Model_JobID;
    var FolderName = USERID;
    var BaseURL = ProjectImagePath + 'Job/';
    $.ajax(
    {
        url: BaseURL + 'DeleteFileFromFolderPhoto/Job',
        data: { fileName: fileDelete, FolderName: FolderName },
        method: 'get',
        success: function () {
            var str = fileDelete;
            var str_array = str.split('?');

            $.each($("#intPhoto li"), function (i, e) {
                $.each(str_array, function (j, u) {
                    if ($(e).find('a').attr('id') == u) {
                        $(e).closest("li").remove();

                        totalInstallation = totalInstallation - 1;
                        OpecityOfDownloadAllPhotos($(".downloadInstallation"), totalInstallation);
                    }
                });
            });
            var count = $('#intPhoto li').length;
            $('#spanPhoto').html('(' + count + ')');
            $(".alert").hide();
            $("#successMsgRegion").html(closeButton + "File has been deleted successfully.");
            $("#successMsgRegion").show();
            return false;
        }
    });
}

function DeleteSerial() {
    if ($('input[name="chkBtnSerial"]:checked').length > 0) {
        if (confirm('Are you sure you want to delete this file ?')) {
            var output;
            output = $.map($('input[name="chkBtnSerial"]:checked'), function (n, i) {
                return n.value;
            }).join('?');
            DeleteSerialFromFolder(output);
        }
    }
}

function DeleteSerialFromFolder(fileDelete) {
    var FolderName = Model_JobID;
    var BaseURL = ProjectImagePath + 'Job/';
    $.ajax(
    {
        url: BaseURL + 'DeleteFileFromFolderPhoto/Job',
        data: { fileName: fileDelete, FolderName: FolderName },
        method: 'get',
        success: function () {
            var str = fileDelete;
            var str_array = str.split('?');
            $.each($("#SerialNumber li"), function (i, e) {
                $.each(str_array, function (j, u) {
                    if ($(e).find('a').attr('id') == u) {
                        $(e).closest("li").remove();
                        totalSerialNumber = totalSerialNumber - 1;
                        OpecityOfDownloadAllPhotos($(".downloadSerialNumber"), totalSerialNumber);
                    }
                });
            });
            var count = $('#SerialNumber li').length;
            $('#spanSerial').html('(' + count + ')');
            if (count == 0) {
                $.ajax({
                    url: urlGetSTCStatusDescription,
                    type: "GET",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8', // Not to set any content header
                    processData: false, // Not to process data
                    data: "id=" + Model_JobID,
                    success: function (result) {
                        if (result.IsSuccess) {
                            if (result.STCStatusName != null && result.STCStatusName != undefined && result.STCStatusName != '') {
                                $('#spanSTCStatus').text(result.STCStatusName.trim());
                            }
                            if (result.STCDescription != null && result.STCDescription != undefined && result.STCDescription != '') {
                                $('#STCDescription').html(result.STCDescription.trim());
                            }
                            if (result.STCStatusId != null && result.STCStatusId != undefined && result.STCStatusId != '') {
                                $('#STCStatusId').val(result.STCStatusId.trim());
                            }
                        }
                    }
                });
            }
            $(".alert").hide();
            $("#successMsgRegion").html(closeButton + "File has been deleted successfully.");
            $("#successMsgRegion").show();
            return false;
        }
    });
}

function checkBusinessRulesForPhotos() {
    EnableDropDownbyUsertype();
    $("#BasicDetails_JobType").removeAttr("disabled");
    var panelData = '';
    var xmlPanel = '';
    var inverterData = '';
    var xmlInverter = '';
    if (PanelXml.length > 0) {
        var jsonp = JSON.stringify(PanelXml);
        var sName = '';
        var obj = $.parseJSON(jsonp);
        $.each(obj, function () {
            sName += '<panel><Brand>' + htmlEncode(this['Brand']) + '</Brand>' + '<Model>' + htmlEncode(this['Model']) + '</Model>' +
             '<NoOfPanel>' + this['Count'] + '</NoOfPanel></panel>';
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
    var data = JSON.stringify($('form').serializeToJson());
    $.ajax({
        url: urlCheckBusinessRules,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8', // Not to set any content header
        processData: false, // Not to process data
        data: data,
        success: function (result) {
            if (result.IsSuccess) {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();
                if (result.STCStatusName != null && result.STCStatusName != undefined && result.STCStatusName != '') {
                    $('#spanSTCStatus').text(result.STCStatusName.trim());
                }
                if (result.STCDescription != null && result.STCDescription != undefined && result.STCDescription != '') {
                    $('#STCDescription').html(result.STCDescription.trim());
                }
                if (result.STCStatusId != null && result.STCStatusId != undefined && result.STCStatusId != '') {
                    $('#STCStatusId').val(result.STCStatusId.trim());
                }
            }
            else if (result == "syntaxError") {
                $(".alert").hide();
                $("#errorMsgRegion").hide();
                $("#successMsgRegion").html(closeButton + "File has been uploaded successfully.");
                $("#successMsgRegion").show();
            }
        }
    });
}
function ShowHidePhotos() {
    totalInstallation = lstUserDocument_Count;
    totalSerialNumber = lstSerialDocument_Count;
    OpecityOfDownloadAllPhotos($(".downloadInstallation"), totalInstallation);
    OpecityOfDownloadAllPhotos($(".downloadSerialNumber"), totalSerialNumber);
}
function OpecityOfDownloadAllPhotos(obj, count) {
    if (count > 0) {
        obj.css('opacity', '1');
        obj.css('pointer-events', 'visible');
    }
    else {
        obj.css('opacity', '0.5');
        obj.css('pointer-events', 'none');
    }
}

function DownLoadAllPhotoesAsZip(isInstallation) {
    var photo = [];
    if (isInstallation) {
        $.each($("#intPhoto").find('li'), function (i, e) {
            if ($(e).find('a').attr('id')) {
                photo.push($(e).find('a').attr('id'));
            }
        });
    }
    else {
        $.each($("#SerialNumber").find('li'), function (i, e) {
            if ($(e).find('a').attr('id')) {
                photo.push($(e).find('a').attr('id'));
            }
        });
    }
    window.location.href = urlDownLoadAllPhotoesAsZip + "?jobid=" + Model_JobID + "&photosArray=" + JSON.stringify(photo) + "&isInstallation=" + isInstallation;
    return false;
}
