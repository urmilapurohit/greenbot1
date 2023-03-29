$(document).ready(function () {
    distributorID = $('#JobInstallationDetails_DistributorID').val();
    nmi = $('#JobInstallationDetails_NMI').val();

    prepareDocs(JobStage_STC);

    $('ul#ulJobStage').find('li').click(function () {
        $('ul#ulJobStage').find('li').removeClass('resp-tab-active');
        $(this).addClass("resp-tab-active");
        prepareDocs($(this).attr('val'));
        if (Model_JobID > 0) {
            if ($(this).attr('val') == "Other") {
                $('#Other').show();
                $('#otherTable').show();
            }
            else if ($(this).attr('val') == "PreApprovals" || $(this).attr('val') == "Connections" || $(this).attr('val') == "STC") {
                $('#Other').hide();
                $('#otherTable').hide();
            }
        }
    });
});



function generateDocument(stage, state, provName, name, id, obj) {
    if (jobId) {

        var isValid = true;
        if ($('tr#' + obj).find('.hdnIsExists').attr('val') == '1' && $('tr#' + obj).find('.hdnIsUpload').attr('val') == '1') {
            if (!confirm('File has been already uploaded. If you are generating file then uploaded file will be removed and new file will be generated. Are you still want to generate ?'))
                isValid = false;
        }
        if (isValid) {
            $("#loading-image").show();
            $.ajaxSetup({ async: true, cache: false });
            $.get(urlGenerateDocument + "?id=" + jobId + "&documentId=" + id + "&stage=" + stage + "&state=" + state + "&provName=" + provName + "&name=" + name + "&isDeleteFirst=" + $('tr#' + obj).find('.hdnIsUpload').attr('val'), "", function (data) {
                if (stage == 'STC') {
                    $("#loading-image").show();
                    checkBusinessRulesForDocument(obj);
                }
                else {
                    IncreaseCountOfDocumentOnGenerate();
                    showMessage("Document has been generated successfully.", false);
                    setViewIcon(('tr#' + obj), false, false);
                }
            });
        }
        //$("#loading-image").hide();
    }
}
function uploadDocument(stage, state, provName, name, id, objId) {
    if (jobId) {

        var isValid = true;
        var obj = $('#' + objId).closest('tr').attr('id');
        if ($('tr#' + obj).find('.hdnIsExists').attr('val') == '1' && $('tr#' + obj).find('.hdnIsUpload').attr('val') == '0') {
            if (!confirm('File has been already generated. If you are uploading file then generated file will be removed and new file will be uploaded. Are you still want to upload ?'))
                isValid = false;
        }
        else if ($('tr#' + obj).find('.hdnIsExists').attr('val') == '1' && $('tr#' + obj).find('.hdnIsUpload').attr('val') == '1') {
            if (!confirm('File has been already uploaded. If you are uploading file then uploaded file will be removed and new file will be uploaded. Are you still want to upload ?'))
                isValid = false;
        }
        if (isValid) {
            $('.fileUpload1').unbind('fileuploadsubmit').bind('fileuploadsubmit', function (e, data) {
                data.formData = { id: jobId, documentid: id, stage: stage, state: state, provName: provName, name: name };
            });

            doneCallBack = function () {
                if (stage == 'STC') {
                    checkBusinessRulesForDocument(obj);
                }
                else {
                    showMessage("Document has been generated successfully.", false);
                    setViewIcon(('tr#' + obj), false, false);
                }
            }

            $('#' + objId).siblings('.fileUpload1').click();
        }
        //$("#loading-image").hide();
    }
    return false;
}



function downloadDocument(stage, state, provName, name, id, obj, isExist) {
    if (jobId) {
        if ($('tr#' + obj).find('.hdnIsExists') && $('tr#' + obj).find('.hdnIsExists').attr('val') && $('tr#' + obj).find('.hdnIsExists').attr('val') == '1') {
            window.location.href = urlDownloadSTCDocument + "?jobid=" + jobId + "&docId=" + id;
        }
    }
    return false;
}


function deleteDocument(stage, state, provName, name, id, obj) {
    if (jobId) {
        if ($('tr#' + obj).find('.hdnIsExists') && $('tr#' + obj).find('.hdnIsExists').attr('val') && $('tr#' + obj).find('.hdnIsExists').attr('val') == '1') {
            if (confirm('Are you sure you want to delete this file ?')) {
                $.ajaxSetup({ async: false, cache: false });
                $.get(urlDeleteDocument + "?id=" + jobId + "&documentId=" + id + "&stage=" + stage + "&state=" + state + "&provName=" + provName + "&name=" + name, "", function (data) {

                    if (stage == 'STC') {
                        $.ajax({
                            url: urlGetSTCStatusDescription,
                            type: "GET",
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8', // Not to set any content header
                            processData: false, // Not to process data
                            data: "id=" + Model_JobID,
                            success: function (result) {
                                if (result.IsSuccess) {
                                    if (result.STCStatusName) {
                                        $('#spanSTCStatus').text(result.STCStatusName.trim());
                                    }

                                    if (result.STCDescription) {
                                        $('#STCDescription').html(result.STCDescription.trim());
                                    }

                                    if (result.STCStatusId) {
                                        $('#STCStatusId').val(result.STCStatusId.trim());
                                    }
                                }
                            }
                        });
                    }
                    showMessage("Document has been deleted successfully.", false);
                    setViewIcon(('tr#' + obj), true, false);
                    DecreaseCountOfDocumentOnDelete();
                });
            }
        }
    }
}

function assignEvent() {
    $("#loading-image").show();
   
    var BaseURL = ProjectImagePath + 'Job/';
    var urlDocument = BaseURL + 'UploadDocument/Job';
    $('.fileUpload1').fileupload({
        url: urlDocument,
        dataType: 'json',
        done: function (e, data) {
            if (doneCallBack)
                doneCallBack();
            doneCallBack = undefined;
        },

        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {
                        showMessage(" " + data.files[i].name + " Maximum document size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB.", true);
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showMessage("Please upload file that not conatain <> ^ [] .", true)
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    showMessage("Maximum  document size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB.", true);
                    return false;
                }
            }
            if (data.files.length == 1) {

                var ext = data.files[0].name.toLowerCase().split('.').length > 0 ? data.files[0].name.toLowerCase().split('.')[data.files[0].name.toLowerCase().split('.').length - 1] : '';
                if (data.files[0].type != "application/pdf" && ext != 'pdf') {
                    showMessage("Please upload a document with .pdf extension.", true);
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true)
                    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                    $('body').animate({ scrollTop: 0 }, 'slow');
                    return false;
                }
            }
            return true;
        },
        change: function (e, data) {
        }
    }).prop('disabled', !$.support.fileInput)
  .parent().addClass($.support.fileInput ? undefined : 'disabled');
}

function setViewIcon(obj, isRemove, isUpload) {
    var removeCls = isRemove ? "" : "disView";
    var addCls = isRemove ? "disView" : "";
    var removedownloadCls = isRemove ? "" : "notdownload";
    var adddownloadCls = isRemove ? "notdownload" : "";
    var tdaddCls = isRemove ? "" : "semiBold";
    var tdremoveCls = isRemove ? "semiBold" : "";
    $(obj).find('.view').removeClass(removeCls).addClass(addCls);
    $(obj).find('.download_doc').removeClass(removedownloadCls).addClass(adddownloadCls);
    $(obj).find('.delete').removeClass(removeCls.replace('View', 'Delete')).addClass(addCls.replace('View', 'Delete'));
    $(obj).find('.tdDoc').removeClass(tdremoveCls).addClass(tdaddCls);
    $(obj).find('.hdnIsExists').attr('val', (isRemove ? '0' : '1'));
    $(obj).find('.hdnIsUpload').attr('val', (isUpload ? '1' : '0'));
}
function showMessage(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegion" : "successMsgRegion";
    var inVisible = isError ? "successMsgRegion" : "errorMsgRegion";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();

}
$(function () {
    'use strict';
    var BaseURL = ProjectImagePath + 'Job/';
    var USERID = Model_JobID;
    var urlDocumentOther = BaseURL + 'UploadDocumentOther/Job';
    //mulitiple proof upload
    $('#uploadBtnDocument').fileupload({

        url: urlDocumentOther,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;
            var UploadFailedFilesType = [];
            UploadFailedFilesType.length = 0;

            //formbot start
            for (var i = 0; i < data.result.length; i++) {

                if (data.result[i].Status == true) {
                    var rowcount = $('#otherTblDoc').length;
                    var documentType = data.result[i].MimeType.split("/");
                    var mimeType = documentType[0];
                    var ids = data.result[i].FileName.replace("%", "$");
                    var sortFileName = data.result[i].FileName.length;
                    if (sortFileName > 20) {
                        var path = ids.substring(0, 20) + '...';
                    }
                    else {
                        path = ids;
                    }
                    var content = "<li class='checkbox' style='margin-left:2px;'>"
                    content += "<label> <input type='checkbox' name='chkBtnDocument' value=\"" + ids + "\" \>";
                    if (mimeType == "image") {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' title=\"" + ids + "\" class=\"" + ids + "\" id=\"" + ids + "\" onclick='OpenOtherDocument(this);'>" + path + " </a>";
                    }
                    else {
                        content += "<a href='javascript:void(0)' style='text-decoration:none;' title=\"" + ids + "\" class=\"" + ids + "\" id=\"" + ids + "\" onclick='OpenOtherDocumentDownload(this);'>" + path + " </a>";
                    }
                    content += "</label>"
                    content += "</li>"

                    $('#otherTblDoc').append(content);
                    $('<input type="hidden">').attr({
                        name: 'FileNamesCreate',
                        value: data.result[i].FileName,
                        id: data.result[i].FileName,
                    }).appendTo('form');

                    IncreaseCountOfDocumentOnGenerate();
                }
                else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                    UploadFailedFilesType.push(data.result[i].FileName);
                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName);

                }
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
            if (UploadFailedFilesType.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + UploadFailedFilesType.length + " " + "Uploaded filename is too big.");
                $("#errorMsgRegion").show();
                $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                $('body').animate({ scrollTop: 0 }, 'slow');

                return false;
            }
            else {
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
});

function DeleteOtherDocument() {
    if ($('input[name="chkBtnDocument"]:checked').length > 0) {
        if (confirm('Are you sure you want to delete this file ?')) {
            $('input[name="chkBtnDocument"]:checked').each(function () {
                DeleteDocumetFromFolder(this.value);
            });
        }
    }
}

function OpenOtherDocument(e) {

    var path = UploadedDocumentPath;
    var fullPath = JobDocumentsToSavePath;
    var USERID = Model_JobID;
    var imagePath = path + fullPath + USERID + "/" + "Other";
    SRC = imagePath + "/" + e.className;

    $("#loading-image").css("display", "");
    $('#imgDocument').attr("src", SRC).load(function () {

        logoWidth = this.width;
        logoHeight = this.height;

        $('#popupDocumentBox').modal({ backdrop: 'static', keyboard: false });

        if ($(window).height() <= logoHeight) {
            //$("#imgDocument").height($(window).height() - 150);

            $("#imgDocument").closest('div').height($(window).height() - 150);
            $("#imgDocument").closest('div').css('overflow-y', 'scroll');
            $("#imgDocument").height(logoHeight);
        }
        else {
            $("#imgDocument").height(logoHeight);
            $("#imgDocument").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidth || logoWidth >= screen.width - 250) {
            //$("#imgDocument").width(screen.width - 10);
            //$('#popupDocumentBox').find(".modal-dialog").width(screen.width - 10);

            $('#popupDocumentBox').find(".modal-dialog").width(screen.width - 250);
            $("#imgDocument").width(logoWidth);
        }
        else {
            $("#imgDocument").width(logoWidth);
            $('#popupDocumentBox').find(".modal-dialog").width(logoWidth);
        }

        $("#loading-image").css("display", "none");
    });
    $("#imgDocument").unbind("error");
    $('#imgDocument').attr('src', SRC).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });


}

$("#btnClosePopUpDocumentBox").click(function () {
    $('#popupDocumentBox').modal('toggle');
});

//Proof of Download Document
function OpenOtherDocumentDownload(e) {
    var BaseURL = ProjectImagePath + 'Job/';
    var foldername = Model_JobID;
    window.location.href = BaseURL + 'ViewDownloadFile/Job?FileName=' + encodeURIComponent(e.className) + '&FolderName=' + foldername;
}

function DeleteDocumetFromFolder(fileDelete) {
    var USERID = Model_JobID;
    var FolderName = USERID;
    var BaseURL = ProjectImagePath + 'Job/';
    $.ajax(
    {
        url: BaseURL + 'DeleteFileFromFolderDocument/Job',
        data: { fileName: fileDelete, FolderName: FolderName },
        method: 'get',
        success: function () {
            $.each($("#otherTblDoc").find('li'), function (i, e) {
                if ($(e).find('a').attr('id') == fileDelete) {
                    //success
                    $(this).remove();
                    DecreaseCountOfDocumentOnDelete();
                }
            });
            // $("li:has('a'):contains(" + fileDelete + ")").remove();
            $(".alert").hide();
            $("#successMsgRegion").html(closeButton + "File has been deleted successfully.");
            $("#successMsgRegion").show();

            return false;
        }
    });
}

function checkBusinessRulesForDocument(objDoc) {

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
            //$("#BasicDetails_strInstallationDate").val(dateaa);
            if (result.IsSuccess) {

                showMessage("Document has been generated successfully.", false);
                setViewIcon(('tr#' + objDoc), false, false);
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
            else if (result.IsError) {
                showMessage("Document has been generated successfully.", false);
                setViewIcon(('tr#' + objDoc), false, false);
            }
            IncreaseCountOfDocumentOnGenerate();
        }
    });
}

function ShowHideDownloadAllActiveButton(count) {
    if (count) {
        totalActiveTabDocumentCount = count;
        OpacityIncreaseDecrease($('.downloadAllActive'), totalActiveTabDocumentCount);
    }
    else
        OpacityIncreaseDecrease($('.downloadAllActive'), 0);
}

function ShowHideDownloadAllButton(count) {
    if (count) {
        totalDocumentCount = count;
        OpacityIncreaseDecrease($('.downloadAll'), totalDocumentCount);
    }
    else
        OpacityIncreaseDecrease($('.downloadAll'), 0);
}

function DecreaseCountOfDocumentOnDelete() {
    if (totalActiveTabDocumentCount > 0)
        totalActiveTabDocumentCount = totalActiveTabDocumentCount - 1;
    if (totalDocumentCount > 0)
        totalDocumentCount = totalDocumentCount - 1;
    OpacityIncreaseDecrease($('.downloadAllActive'), totalActiveTabDocumentCount);
    OpacityIncreaseDecrease($('.downloadAll'), totalDocumentCount);
}

function IncreaseCountOfDocumentOnGenerate() {
    totalActiveTabDocumentCount = totalActiveTabDocumentCount + 1;
    totalDocumentCount = totalDocumentCount + 1;
    OpacityIncreaseDecrease($('.downloadAllActive'), totalActiveTabDocumentCount);
    OpacityIncreaseDecrease($('.downloadAll'), totalDocumentCount);
}

function OpacityIncreaseDecrease(obj, count) {
    if (count > 0) {
        obj.css('opacity', '1');
        obj.css('pointer-events', 'visible');
    }
    else {
        obj.css('opacity', '0.5');
        obj.css('pointer-events', 'none');
    }
}

function downloadAllActiveTabAsZip() {
    $.each($("#ulJobStage").find('li'), function (i, e) {
        if ($(e).attr('class') == 'resp-tab-active') {
            var tabValue = $(e).attr('val');
            window.location.href = urlDownloadAllAndActiveTabDocument + "?jobid=" + jobId + "&distributorID=" + distributorID + "&stage=" + tabValue + "&jobTypeId=" + jobTypeId + "&isAll=" + 0;
            return false;
        }
    });
}

function downloadAllAsZip() {
    var stage = [];
    $.each($("#ulJobStage").find('li'), function (i, e) {
        if ($(e).attr('val').toLowerCase() != 'stc') {
            stage.push($(e).attr('val'));
        }
    });
    window.location.href = urlDownloadAllAndActiveTabDocument + "?jobid=" + jobId + "&distributorID=" + distributorID + "&stage=" + stage.join(',') + "&jobTypeId=" + jobTypeId + "&isAll=" + 1;
    return false;
}



