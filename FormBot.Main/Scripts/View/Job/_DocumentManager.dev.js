$('#btnUploadCESDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('uploadCESBtnDocument').click();
});
$('#btnCreateCESDocument').click(function (e) {
    typeOfFile = "CES";
    e.preventDefault();
    $('#popupDocumentCreateConfirm').modal('show');
});
$('#btnCreateCESPdf').click(function (e) {
    var id = [];
    var pdfname = [];
    $("[data-folder=CES]").each(function (i, e) {
        id.push($(this).attr('data-cid'));
        pdfname.push($(this).attr('data-nm'));

    });
    var docid = [];
    var docpath = [];
    $('#tbodyCES').children().each(function (i, e) {
        docid.push($(this).data('id'));
        docpath.push($(this).data('path'));
    });
    var obj = { checkListIds: id, JobId: JobId, UserId: USERID, type: "2", pdfname: pdfname, docid: docid, docpath: docpath };
    $.ajax({
        url: urlUploadCESPdf,
        data: JSON.stringify(obj),
        contentType: "application/json",
        dataType: 'json',
        method: "post",
        success: function (a) {
            getDocuments(false);
        }
    })
});
var logoHeightDoc, logoWidthDoc;
//add docs
$('#uploadCESBtnDocument').fileupload({
    url: urlDocumentCES,
    dataType: 'json',
    done: function (e, data) {
        var UploadFailedFiles = [];
        UploadFailedFiles.length = 0;
        var UploadFailedFilesName = [];
        UploadFailedFilesName.length = 0;

        var UploadFailedFilesType = [];
        UploadFailedFilesType.length = 0;
        var $tbodyCES = $('#tbodyCES');
        //formbot start
        for (var i = 0; i < data.result.length; i++) {
            if (data.result[i].Status == true) {

                var path = 'JobDocuments\\' + JobId + '\\CES\\' + data.result[i].FileName;

                var tr = $('<tr/>')
                    .data('data', data.result[i])
                    .data('path', path)
                    .data('id', data.result[i].AttachmentID)
                    .attr('id', 'ces_' + data.result[i].AttachmentID)
                    .addClass("job-document-" + data.result[i].AttachmentID)
                    .appendTo($tbodyCES);
                var tdForSelectFileCheckBox = $('<td/>')
                    .css("width", "2%")
                    //.html('<input class="chkDocument" data-jobdocid="' + data.result[i].AttachmentID +'" title="Select Document" type="checkbox">')
                    .appendTo(tr);
                var td = $('<td/>')
                    .text(data.result[i].FileName)
                    .appendTo(tr);
                var td2 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var td1 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var aSendMailDocSignForCES = $('<a/>')
                    .addClass('icon sprite-img email-ic document-signature-send-request')
                    .attr('href', 'javascript:void(0);')
                    .attr('title', 'Signature Mail')
                    .data('type', "CES")
                    .attr('data-filename', data.result[i].FileName)
                    .attr('onclick', 'viewDocument("CES","","","' + path + '",0,"ces_' + data.result[i].AttachmentID + '","' + data.result[i].AttachmentID + '");')
                    .appendTo(td1);

                if (!(allowdViewTypes.indexOf(path.toLowerCase().split('.').pop()) > -1)) {
                    OpacityIncreaseDecrease(aSendMailDocSignForCES, 0);
                }
                if ((USERType == "1" || USERType == "3") || $('#STCStatusId').val() == 10 || $('#STCStatusId').val() == 12 || $('#STCStatusId').val() == 14 || $('#STCStatusId').val() == 17) {
                    var aDelete = $('<a/>')
                        .addClass('sprite-img delete')
                        .attr('href', '#')
                        .attr('title', 'Delete')
                        .appendTo(td1);
                }
                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

            }
            else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                UploadFailedFilesType.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result[i].FileName);
            }
            else {
                UploadFailedFiles.push(data.result[i].FileName);

            }
            PopoverEnable();
            if ($tbodyCES.children().length == 0) {
                $("#divDocumentsCES").find('.table-responsive').hide();
                $("#divDocumentsCES").find('.noDocumentCES').show();
            }
            else {
                $("#divDocumentsCES").find('.table-responsive').show();
                $("#divDocumentsCES").find('.noDocumentCES').hide();
            }

            if ($("#tbodyCES").find('tr').length == 1) {
                ReloadSTCModule();
            }

        }
        if (UploadFailedFiles.length > 0) {
            showMessage(UploadFailedFiles.length + " " + "File has not been uploaded.", true, 'CES');
            return false;
        }
        if (UploadFailedFilesType.length > 0) {
            showMessage(UploadFailedFilesType.length + " " + "Uploaded file type is not supported.", true, 'CES');
            return false;
        }
        else {
            showMessage("File has been uploaded successfully.", false, 'CES');
        }
        SearchHistory();
    },
    progressall: function (e, data) {

    },
    singleFileUploads: false,
    send: function (e, data) {
        if (data.files.length == 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].name.length > 50) {
                    showMessage("Please upload small filename.", true, 'CES');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'CES')
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true, 'CES');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'CES')
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true, 'CES');
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
    formData: { UserId: USERID, JobId: JobId, Type: "CES" },
    change: function (e, data) {
        $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
    }
}).prop('disabled', !$.support.fileInput);
//add docs
$('#uploadOthreBtnDocument').fileupload({
    url: urlDocumentCES,
    dataType: 'json',
    done: function (e, data) {
        var UploadFailedFiles = [];
        UploadFailedFiles.length = 0;
        var UploadFailedFilesName = [];
        UploadFailedFilesName.length = 0;
        var UploadFailedFilesType = [];
        UploadFailedFilesType.length = 0;
        var $tbodyCES = $('#tbodyOther');
        //formbot start
        for (var i = 0; i < data.result.length; i++) {
            if (data.result[i].Status == true) {
                var path = 'JobDocuments\\' + JobId + '\\OTHER\\' + data.result[i].FileName;
                var tr = $('<tr/>')
                    .data('data', data.result[i])
                    .data('path', path)
                    .data('id', data.result[i].AttachmentID)
                    .attr("id", "other_" + data.result[i].AttachmentID)
                    .addClass("job-document-" + data.result[i].AttachmentID)
                    .appendTo($tbodyCES);
                var tdForSelectFileCheckBox = $('<td/>')
                    .css("width", "2%")
                    //.html('<input class="chkDocument" data-jobdocid="' + data.result[i].AttachmentID + '" title="Select Document" type="checkbox">')
                    .appendTo(tr);
                var td = $('<td/>')
                    .text(data.result[i].FileName)
                    .appendTo(tr);
                var td2 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var td1 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var aSendMailDocSignForOther = $('<a/>')
                    .addClass('icon sprite-img email-ic document-signature-send-request')
                    .attr('href', 'javascript:void(0);')
                    .attr('title', 'Signature Mail')
                    .data('type', "CES")
                    .attr('data-filename', data.result[i].FileName)
                    .attr('onclick', "viewDocument('OTHER','','','" + encodeURIComponent(path) + "',0,'other_" + data.result[i].AttachmentID + "','" + data.result[i].AttachmentID + "');")
                    .appendTo(td1);
                if (!(allowdViewTypes.indexOf(path.toLowerCase().split('.').pop()) > -1)) {
                    OpacityIncreaseDecrease(aSendMailDocSignForOther, 0);
                }

                var aDelete = $('<a/>')
                    .addClass('sprite-img delete')
                    .attr('href', '#')
                    .attr('title', 'Delete')
                    .appendTo(td1);

                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

            }

            else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                UploadFailedFilesType.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result[i].FileName);
            }
            else {
                UploadFailedFiles.push(data.result[i].FileName);
            }
        }
        if (UploadFailedFiles.length > 0) {
            showMessage(UploadFailedFiles.length + " " + "File has not been uploaded.", true, 'Other');
            return false;
        }
        if (UploadFailedFilesType.length > 0) {
            showMessage(UploadFailedFilesType.length + " " + "Uploaded file type is not supported.", true, 'Other');
            return false;
        }
        else {
            showMessage("File has been uploaded successfully.", false, 'Other');
        }
        PopoverEnable();
        SearchHistory();
    },
    progressall: function (e, data) {

    },
    singleFileUploads: false,
    send: function (e, data) {
        if (data.files.length == 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].name.length > 50) {
                    showMessage("Please upload small filename.", true, 'Other');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, "Other")
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true, 'Other');
                    return false;
                }
                else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, "Other")
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true, 'Other');
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
    formData: { UserId: USERID, JobId: JobId, Type: "OTHER" },
    change: function (e, data) {
    }
}).prop('disabled', !$.support.fileInput);
//getDocuments(false);
$('#btnSTCDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnUploadSTCDocument').click();
});
// set docs
function setFiles(tbody, data, type) {
    var $tbody = tbody;
    $tbody.html('');
    $tbody.attr("totalCount", data.length);
    for (var i = 0; i < data.length; i++) {
        var tr = $('<tr/>').data("data", data[i])
            .data('path', data[i].Path)
            .data('id', data[i].JobDocumentId);
        tr.addClass("job-document-" + data[i].JobDocumentId);
        if (type == "CES") {
            tr.attr("id", "ces_" + data[i].JobDocumentId);
        }
        if (type == "PNLINVC") {
            tr.attr("id", "pnlinvoice_" + data[i].JobDocumentId);
        }
        if (type == "ELECBILL") {
            tr.attr("id", "elecbill_" + data[i].JobDocumentId);
        }
        if (type == "OTHER") {
            tr.attr("id", "other_" + data[i].JobDocumentId);
            tr.attr("isreczip", data[i].IsRecZip);
        }
        tr.appendTo($tbody);
        var tdForCheckbox = $('<td/>')
            .css("width", "2%")
            .appendTo(tr);

        if (((data[i].IsSPVXml == null || !data[i].IsSPVXml) || ((data[i].IsRecZip == null || !data[i].IsRecZip))) || ((data[i].IsRecZip || data[i].IsSPVXml) && (USERType == "1" || USERType == "3"))) {
            var td = $('<td/>')
                .text(data[i].Path.split('\\').pop())
                .appendTo(tr);

            var td1 = $('<td/>')
                .addClass('action')
                .appendTo(tr);
        }
        var td1 = $('<td/>')
            .addClass('action')
            .appendTo(tr);
        if (type == "CES") {
            params = "'CES','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("ces_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
            var aSendMailDocSignForCES = $('<a/>')
                .addClass('icon sprite-img email-ic document-signature-send-request')
                .attr('href', 'javascript:void(0);')
                .attr('title', 'Signature Mail')
                .attr('data-filename', data[i].Path.split('\\').pop())
                .data('type', type)
                .attr('onclick', 'viewDocument(' + params + ')')
                .appendTo(td1);

            if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                OpacityIncreaseDecrease(aSendMailDocSignForCES, 0);
            }
        }
        if (type == "PNLINVC") {
            params = "'PNLINVC','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("pnlinvoice_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
            var aSendMailDocSignForPnlInvoice = $('<a/>')
                .addClass('icon sprite-img email-ic document-signature-send-request')
                .attr('href', 'javascript:void(0);')
                .attr('title', 'Signature Mail')
                .attr('data-filename', data[i].Path.split('\\').pop())
                .data('type', type)
                .attr('onclick', 'viewDocument(' + params + ')')
                .appendTo(td1);
            if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                OpacityIncreaseDecrease(aSendMailDocSignForPnlInvoice, 0);
            }
        }
        if (type == "ELECBILL") {
            params = "'ELECBILL','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("elecbill_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
            var aSendMailDocSignForElecBill = $('<a/>')
                .addClass('icon sprite-img email-ic document-signature-send-request')
                .attr('href', 'javascript:void(0);')
                .attr('title', 'Signature Mail')
                .attr('data-filename', data[i].Path.split('\\').pop())
                .data('type', type)
                .attr('onclick', 'viewDocument(' + params + ')')
                .appendTo(td1);
            if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                OpacityIncreaseDecrease(aSendMailDocSignForElecBill, 0);
            }
        }
        if (((data[i].IsSPVXml == null || !data[i].IsSPVXml) || ((data[i].IsRecZip == null || !data[i].IsRecZip))) || ((data[i].IsRecZip || data[i].IsSPVXml) && (USERType == "1" || USERType == "3"))) {
            if (type == "OTHER") {//&& data[i].isUpload != true) {
                params = "'OTHER','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("other_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
                var aSendMailDocSignForOther = $('<a/>')
                    .addClass('icon sprite-img email-ic document-signature-send-request')
                    .attr('href', 'javascript:void(0);')
                    .attr('data-filename', data[i].Path.split('\\').pop())
                    .attr('title', 'Signature Mail')
                    .data('type', type)
                    .attr('onclick', 'viewDocument(' + params + ')')
                    .appendTo(td1);
                if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                    OpacityIncreaseDecrease(aSendMailDocSignForOther, 0);
                }
            }

            var aDelete = $('<a/>')
                .addClass('sprite-img delete')
                .attr('href', '#')
                .attr('title', 'Delete')
                .appendTo(td1);

            if (data[i].IsRecZip || data[i].IsSPVXml) {
                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('onclick', "onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ")")
                    .appendTo(td1);
            }
            else {
                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",0)>Download final</a>")
                    .appendTo(td1);
            }
        }
        if ((data[i].IsRecZip == true || data[i].IsSPVXml == true) && !(USERType == "1" || USERType == "3")) {
            $("#other_" + data[i].JobDocumentId).remove();
        }
    }
    if (type == "CES" && $(tbody).children().length == 0) {
        $("#divDocumentsCES").find('.table-responsive').hide();
        $("#divDocumentsCES").find('.noDocumentCES').show();
    }
    else if (type == "CES" && $(tbody).children().length > 0) {
        $("#divDocumentsCES").find('.table-responsive').show();
        $("#divDocumentsCES").find('.noDocumentCES').hide();
    }
    if (type == "PNLINVC" && $(tbody).children().length == 0) {
        $("#divDocumentsPnlInvoice").find('.table-responsive').hide();
        $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').show();
    }
    else if (type == "PNLINVC" && $(tbody).children().length > 0) {
        $("#divDocumentsPnlInvoice").find('.table-responsive').show();
        $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').hide();
    }
    if (type == "ELECBILL" && $(tbody).children().length == 0) {
        $("#divDocumentsElecBill").find('.table-responsive').hide();
        $("#divDocumentsElecBill").find('.noDocumentElecBill').show();
    }
    else if (type == "ELECBILL" && $(tbody).children().length > 0) {
        $("#divDocumentsElecBill").find('.table-responsive').show();
        $("#divDocumentsElecBill").find('.noDocumentElecBill').hide();
    }
    PopoverEnable();
    var StcStatusId = $("#STCStatusId").val();
    if ((USERType != "1" && USERType != "3") && StcStatusId != "10" && StcStatusId != "12" && StcStatusId != "14" && StcStatusId != "17") {
        $("#tbodyCES .delete").hide();
    }
}
function generateDocument(stage, state, provName, name, id, obj, jobDocumentId) {
    var data = $("#divDocumentsSTC").find('table').find('#' + obj).data('data');

    if (eJobId) {
        var isValid = true;
        if (isValid) {
            $("#loading-image").show();
            $.ajaxSetup({ async: true, cache: false });
            $.get(urlGenerateDocument + "?id=" + eJobId + "&documentId=" + 0 + "&stage=" + data.Stage + "&state=" + "" + "&provName=" + "" + "&name=" + encodeURIComponent(data.Path) + "&isDeleteFirst=" + $('tr#' + obj).find('.hdnIsUpload').attr('val') + "&isClassic=false" + "&jobDocumentId=" + data.JobDocumentId, "", function (data) {
                showMessage("Document has been generated successfully.", false, 'STC');
                SearchHistory();
            });
        }
    }
}

function downloadDocument(stage, state, provName, name, id, obj, isExist, isEditable) {
    var data = $("#divDocumentsSTC").find('table').find('#' + obj).data('data');
    if (eJobId) {
        if ($('tr#' + obj).find('.hdnIsExists') && $('tr#' + obj).find('.hdnIsExists').attr('val') && $('tr#' + obj).find('.hdnIsExists').attr('val') == '1') {
            window.location.href = urlDownloadSTCDocument + "?jobid=" + eJobId + "&docId=" + id + "&isClassic=false" + "&jobDocumentPath=" + encodeURIComponent(data.Path) + "&isEditable=" + isEditable;
        }
    }
    $(".pop").popover('hide');
    SearchHistory();
    return false;
}
function uploadDocument(stage, state, provName, name, id, objId) {
    if (eJobId) {
        var isValid = true;
        if (isValid) {
            $('.fileUpload1').unbind('fileuploadsubmit').bind('fileuploadsubmit', function (e, data) {
                data.formData = { id: eJobId, documentid: id, stage: stage, state: state, provName: provName, name: name, isClassic: false };
            });

            doneCallBack = function () {
                if (stage == 'STC') {
                    $("#divDocumentsSTC").find('.table-responsive').show();
                    $('#trDoc_1').css('display', '');
                    $("#divDocumentsSTC").find('.noDocument').hide();
                }
                else {
                    showMessage("Document has been generated successfully.", false, 'STC');
                    setViewIcon(('tr#' + obj), false, false);
                }
            }

            $('.fileUpload1').click();
        }
    }
    return false;
}
function doneCallBack() {
    getDocuments(true);
}
function checkBusinessRulesForDocument(objDoc) {
    var data = CommonDataForSave();
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

                showMessage("Document has been generated successfully.", false, 'STC');

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

                showMessage("Document has been generated successfully.", false, 'STC');
                setViewIcon(('tr#' + objDoc), false, false);
            }
        }
    });
}
function deleteDocument(stage, state, provName, name, id, obj) {
    var data = $("#divDocumentsSTC").find('table').find('#' + obj).data('data');
    if (eJobId) {
        if ($('tr#' + obj).find('.hdnIsExists') && $('tr#' + obj).find('.hdnIsExists').attr('val') && $('tr#' + obj).find('.hdnIsExists').attr('val') == '1') {
            if (confirm('Are you sure you want to delete this file ?')) {
                $.ajaxSetup({ async: false, cache: false });
                $.get(urlDeleteDocument + "?id=" + eJobId + "&documentId=" + id + "&stage=" + stage + "&state=" + state + "&provName=" + provName + "&name=" + name + "&isClassic=false" + "&jobDocumentPath=" + data.Path + "&jobDocumentId=" + data.JobDocumentId, "", function (data) {

                    if (stage == 'STC') {

                        $("#divDocumentsSTC").find('table').find('#' + obj).remove();

                        if ($("#divDocumentsSTC").find('table').find('tr') && $("#divDocumentsSTC").find('table').find('tr').length > 0) {
                            $("#divDocumentsSTC").find('.table-responsive').show();
                            $("#divDocumentsSTC").find('.noDocument').hide();
                        }
                        else {
                            $("#divDocumentsSTC").find('.table-responsive').hide();
                            $("#divDocumentsSTC").find('.noDocument').show();

                            $("#loading-image").show();
                            setTimeout(function () {
                                ReloadSTCJobScreen(eJobId);
                            }, 500);
                        }
                    }
                    showMessage("Document has been deleted successfully.", false, 'STC');
                    SearchHistory();
                    //setViewIcon(('tr#' + obj), true, false);
                });
            }
        }
    }
}
function showMessage(msg, isError, type) {
    $(".alert").hide();
    var visible, inVisible;
    if (type.toUpperCase() == 'CES') {
        visible = isError ? "errorMsgRegionCES" : "successMsgRegionCES";
        inVisible = isError ? "successMsgRegionCES" : "errorMsgRegionCES";
    }
    else if (type.toUpperCase() == 'STC') {
        visible = isError ? "errorMsgRegionSTC" : "successMsgRegionSTC";
        inVisible = isError ? "successMsgRegionSTC" : "errorMsgRegionSTC";
    }
    else if (type.toUpperCase() == 'OTHER') {
        visible = isError ? "errorMsgRegionOther" : "successMsgRegionOther";
        inVisible = isError ? "successMsgRegionOther" : "errorMsgRegionOther";
    }
    else if (type.toUpperCase() == 'PNLINVC') {
        visible = isError ? "errorMsgRegionPnlInvoice" : "successMsgRegionPnlInvoice";
        inVisible = isError ? "successMsgRegionPnlInvoice" : "errorMsgRegionPnlInvoice";
    }
    else if (type.toUpperCase() == 'ELECBILL') {
        visible = isError ? "errorMsgRegionElecBill" : "successMsgRegionElecBill";
        inVisible = isError ? "successMsgRegionElecBill" : "errorMsgRegionElecBill";
    }

    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
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
function setViewIcon(obj, isRemove, isUpload) {
    var removeCls = isRemove ? "" : "disView";
    var addCls = isRemove ? "disView" : "";
    var removedownloadCls = isRemove ? "" : "notdownload";
    var adddownloadCls = isRemove ? "notdownload" : "";
    var tdaddCls = isRemove ? "" : "semiBold";
    var tdremoveCls = isRemove ? "semiBold" : "";
    $(obj).find('.view').removeClass(removeCls).addClass(addCls).addClass("sprite-img");
    $(obj).find('.download_doc').removeClass(removedownloadCls).addClass(adddownloadCls);
    $(obj).find('.delete').removeClass(removeCls.replace('View', 'Delete')).addClass(addCls.replace('View', 'Delete'));
    $(obj).find('.tdDoc').removeClass(tdremoveCls).addClass(tdaddCls);
    $(obj).find('.hdnIsExists').attr('val', (isRemove ? '0' : '1'));
    $(obj).find('.hdnIsUpload').attr('val', (isUpload ? '1' : '0'));
}
function assignEvent() {
    $("#loading-image").show();

    var urlDocument = BaseURL + 'UploadDocument';
    urlDocument = urlDocument.replace('Index/', '');

    $('#btnUploadSTCDocument').fileupload({
        url: urlDocument,
        dataType: 'json',
        done: function (e, data) {
            if (doneCallBack)
                doneCallBack();
            SearchHistory();
        },
        progressall: function (e, data) {
        },
        singleFileUploads: false,
        send: function (e, data) {
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(ProjectSession_MaxImageSize)) {
                        showMessage(" " + data.files[i].name + " Maximum document size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB.", true, 'STC');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showMessage("Please upload file that not conatain <> ^ [] .", true, "STC")
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(ProjectSession_MaxImageSize)) {
                    showMessage("Maximum  document size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB.", true, 'STC');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, "STC")
                    return false;
                }
            }
            if (data.files.length == 1) {

                var ext = data.files[0].name.toLowerCase().split('.').length > 0 ? data.files[0].name.toLowerCase().split('.')[data.files[0].name.toLowerCase().split('.').length - 1] : '';
                if ((data.files[0].type != "application/pdf" && ext != 'pdf') && mimeType != "image" && ext != "heic") {
                    showMessage("Please upload a document with .pdf extension or image file.", true, 'STC');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    showMessage(" " + data.files[i].name + "Please upload file that not conatain <> ^ [] .", true, "STC")
                    return false;
                }
            }
            return true;
        },
        formData: { id: eJobId, stage: "STC", isClassic: false },
        change: function (e, data) {
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
}
$('#btnCreateDoc').click(function (e) {
    typeOfFile = "other"
    e.preventDefault();
    $('#popupDocumentCreateConfirm').modal('show');
});
$('#ddlCat').change(function () {
    if (this.value && this.value > 0) {
        $.get(urlGetDocumentsByStateId + "?stateId=" + this.value, function (response) {
            var $tbody = $('#tbodyItems');
            $tbody.html('');
            if (response) {
                var data = JSON.parse(response).Table;
                for (var i = 0; i < data.length; i++) {
                    var li = $('<li/>').addClass('list-group-item').data({ 'name': data[i].Name, 'path': data[i].Path }).appendTo($tbody);
                    var $div = $('<div/>').addClass('checkbox').appendTo(li);
                    var $lable = $('<label/>').css({ 'word-wrap': 'break-word' }).appendTo($div);
                    var input = '<input type="checkbox">' + data[i].Name;
                    $lable.html(input);
                }
            }
        });
    }
});
//add docs
$('#btnAddDoc').click(function (e) {

    e.preventDefault();
    var $selectedLi = $('#tbodyItems').find('input[type=checkbox]:checked').closest('li'),
        data = [];

    if ($selectedLi.length < 1) {
        alert("Please select atleast one document");
        return false;
    }


    for (var i = 0; i < $selectedLi.length; i++) {
        var $li = $($selectedLi[i]);
        data.push({
            name: $li.data('name'),
            path: $li.data('path'),
            TemplateName: $li.data('name'),
        });
    }
    var obj = {
        jobId: JobId,
        docs: data,
        UserId: USERID,
        fillData: $('#ddlFillData').val(),
        UseNewDocTemplate: false,
        type: typeOfFile
    }
    $.post(urlAddOtherDocuments, obj, function (response) {

        if (response) {
            $tbody = $('#tbodyOther')
            if (typeOfFile.toLowerCase() == "ces")
                $tbody = $('#tbodyCES')

            var failed = response.filter(e => e.JobDocumentId === 0),
                data = response.filter(e => e.JobDocumentId != 0);
            for (var i = 0; i < data.length; i++) {
                var tr = $('<tr/>')
                    .data('data', data[i])
                    .data('path', data[i].Path)
                    .data('id', data[i].JobDocumentId);


                if (typeOfFile.toUpperCase() == "CES") {
                    tr.attr("id", "ces_" + data[i].JobDocumentId);
                }
                if (typeOfFile.toUpperCase() == "OTHER") {
                    tr.attr("id", "other_" + data[i].JobDocumentId);
                }
                tr.addClass("job-document-" + data[i].JobDocumentId);
                tr.appendTo($tbody);
                var tdForSelectFileCheckBox = $('<td/>')
                    .css("width", "2%")
                    //.html('<input class="chkDocument" data-jobdocid="' + data[i].JobDocumentId + '" title="Select Document" type="checkbox">')
                    .appendTo(tr);
                var td = $('<td/>')
                    .text(data[i].name)
                    .appendTo(tr);
                var td1 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                if (typeOfFile.toUpperCase() == "CES") {//&& data[i].isUpload != true) {
                    var aSendMailDocSignForCES = $('<a/>')
                        .addClass('icon sprite-img email-ic document-signature-send-request')
                        .attr('href', 'javascript:void(0);')
                        .attr('title', 'Signature Mail')
                        .data('type', typeOfFile.toUpperCase())
                        .attr('data-filename', data[i].name)
                        .attr('onclick', "viewDocument('CES','','','" + data[i].Path + "',0,'ces_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                        .appendTo(td1);
                }
                if (typeOfFile.toUpperCase() == "OTHER") {//&& data[i].isUpload != true) {
                    var aSendMailDocSignForOther = $('<a/>')
                        .addClass('icon sprite-img email-ic document-signature-send-request')
                        .attr('href', 'javascript:void(0);')
                        .attr('title', 'Signature Mail')
                        .data('type', "CES")
                        .attr('data-filename', data[i].name)
                        .attr('onclick', "viewDocument('OTHER','','','" + data[i].Path + "',0,'other_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                        .appendTo(td1);
                    if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                        OpacityIncreaseDecrease(aSendMailDocSignForOther, 0);
                    }
                }
                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",0)>Download final</a>")
                    .appendTo(td1);
                if ((USERType == "1" || USERType == "3") || $('#STCStatusId').val() == 10 || $('#STCStatusId').val() == 12 || $('#STCStatusId').val() == 14 || $('#STCStatusId').val() == 17) {
                    var aDelete = $('<a/>')
                        .addClass('sprite-img delete')
                        .attr('href', '#')
                        .attr('title', 'Delete')
                        .appendTo(td1);
                }
            }
            if (failed.length > 0) {
                var message = "";
                for (var i = 0; i < failed.length; i++) {
                    message += '- ' + failed[i].message + '\n';
                }
                alert(message);
            }
            else {
                showMessage("Document has been uploaded successfully.", false, typeOfFile);
            }

            if (typeOfFile.toLowerCase() == "ces") {
                if ($tbody.children().length == 0) {
                    $("#divDocumentsCES").find('.table-responsive').hide();
                    $("#divDocumentsCES").find('.noDocumentCES').show();
                }
                else {
                    $("#divDocumentsCES").find('.table-responsive').show();
                    $("#divDocumentsCES").find('.noDocumentCES').hide();
                }

                if ($("#tbodyCES").find('tr').length == 1) {
                    ReloadSTCModule();
                }
            }
            PopoverEnable();
            $('#mdlDocs').modal('hide');
            SearchHistory();
        }
    });
});
$('#btnClear').click(function (e) {
    e.preventDefault();
});
$('#tbodyCES,#tbodyOther,#tbodyPnlInvoice,#tbodyElecBill').on('click', '.view', function (e) {
    e.preventDefault();
    var $this = $(this);
    var path = encodeURIComponent($this.closest('tr').data('path'));
    $('#loading-image').show();
    setTimeout(function () {
        if (isImage(path)) {
            ViewPhotoDoc(path);
            $('#btnPrev,#btnNext').attr('disabled', 'disabled');
            $('#hdrFileName').text(path.split('\\').slice(path.split('\\').length - 1));
        }
        else {
            viewDocumentOther($this.closest('tr').data('id'), $this.data('type'));
        }
    }, 500);
});
function isImage(path) {
    return (imageTypes.indexOf(path.toLowerCase().split('.').pop()) > -1) ? true : false;
}
function ViewPhotoDoc(path) {
    path = UploadedDocumentPath + path;
    $("#loading-image").css("display", "");
    $('#imgDocImage').attr('src', path).load(function () {
        logoWidthDoc = this.width;
        logoHeightDoc = this.height;
        $('#popupViewer').modal({ backdrop: 'static', keyboard: false });
        if ($(window).height() <= logoHeightDoc) {
            $("#imgDocImage").closest('div').height($(window).height() - 205);
            $("#imgDocImage").closest('div').css('overflow-y', 'scroll');
            $("#imgDocImage").height(logoHeightDoc);
        }
        else {
            $("#imgDocImage").height(logoHeightDoc);
            $("#imgDocImage").closest('div').removeAttr('style');
        }

        if (screen.width <= logoWidthDoc || logoWidthDoc >= screen.width - 250) {

            $('#popupViewer').find(".modal-dialog").width(screen.width - 250);
            $("#imgDocImage").width(logoWidthDoc);
        }
        else {
            $("#imgDocImage").width(logoWidthDoc);
            $('#popupViewer').find(".modal-dialog").width(logoWidthDoc);
        }
        $("#loading-image").css("display", "none");
    });

    $("#popupViewer").unbind("error");

    $('#imgDocImage').attr('src', path).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
}
$('#divDocumentsCES').on('click', '.delete', function (e) {
    e.preventDefault();

    var $tr = $(this).closest('tr');
    deleteFile($tr, true, 'CES');
});
function deleteFile($tr, hardDelete, type) {
    var id = $tr.data('id');
    var path = $tr.data('path');
    var jobId = JobId;
    var obj = {
        id: $tr.data('id'),
        path: $tr.data('path'),
        deleteFile: hardDelete,
        JobId: jobId
    }
    if (confirm('Are you sure you want to delete this file ?')) {
        deleteDoc(obj).then(function (response) {
            if (response) {
                $tr.remove();
                if (type == 'CES') {
                    if ($('#tbodyCES').children().length == 0) {
                        $("#divDocumentsCES").find('.table-responsive').hide();
                        $("#divDocumentsCES").find('.noDocumentCES').show();
                    }
                    else {
                        $("#divDocumentsCES").find('.table-responsive').show();
                        $("#divDocumentsCES").find('.noDocumentCES').hide();
                    }
                    if ($("#tbodyCES").find('tr').length == 0) {
                        ReloadSTCModule();
                    }
                }
                if (type == 'PNLINVC') {
                    if ($('#tbodyPnlInvoice').children().length == 0) {
                        $("#divDocumentsPnlInvoice").find('.table-responsive').hide();
                        $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').show();
                    }
                    else {
                        $("#divDocumentsPnlInvoice").find('.table-responsive').show();
                        $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').hide();
                    }
                    if ($("#tbodyPnlInvoice").find('tr').length == 0) {
                        ReloadSTCModule();
                    }
                }
                if (type == 'ELECBILL') {
                    if ($('#tbodyElecBill').children().length == 0) {
                        $("#divDocumentsElecBill").find('.table-responsive').hide();
                        $("#divDocumentsElecBill").find('.noDocumentElecBill').show();
                    }
                    else {
                        $("#divDocumentsElecBill").find('.table-responsive').show();
                        $("#divDocumentsElecBill").find('.noDocumentElecBill').hide();
                    }
                    if ($("#tbodyElecBill").find('tr').length == 0) {
                        ReloadSTCModule();
                    }
                }
                if (type == 'Other') {
                    if ($('tr[isreczip="true"]').length == 0) {
                        $("#onOffSwitchGenerateRecZip").prop('checked', false);
                    }
                    else {
                        $("#onOffSwitchGenerateRecZip").prop('checked', true);
                    }
                }
                SearchHistory();

                showMessage("File has been deleted Successfully.", false, type);
            }
            else {
                showMessage("There is some error while deleting file.", true, type);
            }

        });;
    }
}
$('#tbodyOther').on('click', '.delete', function (e) {
    e.preventDefault();
    var $tr = $(this).closest('tr');
    if ($('tr[isreczip="true"]').length == 1) {
        $("#onOffSwitchGenerateRecZip").prop('checked', false);
    }
    deleteFile($tr, true, 'Other');
});
function DownloadJobDocuments(id, isEditable) {
    window.location.href = urlDownloadJobDocuments + "?jobDocId=" + id + "&isEditable=" + isEditable;
    $(".pop").popover('hide');
    SearchHistory();
}

function deleteDoc(obj) {
    return new Promise(function (resolve) {
        $.ajaxSetup({ async: false, cache: false });
        $.post(urlDeleteDocumentNew, obj, function (data) {
            resolve(data);
        });
    });
}
$('#btnDownloadAllCES,#btnDownloadAllOther').click(function (e) {
    e.preventDefault();
    var type = $(this).data('type');
    var $tbody;
    if (type == "ces")
        $tbody = $('#tbodyCES');
    if (type == "stc")
        $tbody = $('#tbodySTC');
    else
        $tbody = $('#tbodyOther');
    if ($tbody.find('tr').length > 0) {
        window.location.href = urlDownloadAllDocumentsNew + "?jobid=" + JobId + "&type=" + type;
        SearchHistory();
    }
    else {
        alert("No documents available.");
        return false;
    }
});


$("#btnDownloadJobFullPack").click(function (e) {
    console.log(JobId);
    window.location.href = urlGetFullJobPack + "?JobID=" + eJobId;
});

$('#btnUploadOther').click(function (e) {
    e.preventDefault();
    document.getElementById('uploadOthreBtnDocument').click();
});
$(document).ready(function () {
    //fnDocViewerOuterPopup();   
    $("#btnDownloadGPSLogs").click(function (e) {
        $.ajax({
            type: 'GET',
            url: urlGPSLogFilesExists,
            dataType: 'json',
            async: false,
            data: { jobId: JobId },
            success: function (result) {
                debugger;

                if (result.status) {
                    window.location.href = urlDownloadGPSLogs + "?jobId=" + JobId;
                }
                else {
                    alert("File not exists!!!");
                }


            },
            error: function (ex) {
                alert('File not exists');
            }
        });
    });
    $("#nextDocumentTemplate").click(function () {
        if ($("#docTemplateId").val() == 1) {

            var isCES = 0;
            if (typeOfFile.toLowerCase() == "ces")
                isCES = "true";
            else
                isCES = "false";

            $.get(urlGetAllDocumentTemplate + '?jobTypeId=' + parseInt($("#BasicDetails_JobType").val()) + "&solarCompanyId=" + $("#BasicDetails_SolarCompanyId").val() + "&isSTC=false" + "&isCES=" + isCES, function (response) {
                var $tbody = $('#tbodyDocItemsForJob');
                $tbody.html('');
                if (response) {
                    var data = response;
                    for (var i = 0; i < data.length; i++) {
                        var li = $('<li/>').addClass('list-group-item').data({ 'path': data[i].Path, 'name': data[i].Path.split('/').pop(), 'tempName': data[i].DocumentTemplateName }).appendTo($tbody);
                        var $div = $('<div/>').addClass('checkbox').appendTo(li);
                        var $lable = $('<label/>').css({ 'word-wrap': 'break-word' }).appendTo($div);
                        var input = '<input type="checkbox" >' + data[i].DocumentTemplateName;
                        $lable.html(input);
                    }
                }
            });
            $('#popupDocumentCreateConfirm').modal('toggle');
            $('#DocumentTemplate').modal('show');
        }
        else {
            $.get(urlGetStateList, function (response) {
                if (response) {
                    var states = JSON.parse(response).Table;
                    var options = "";
                    for (var i = 0; i < states.length; i++) {
                        options += '<option value=' + states[i].StateID + '>' + states[i].Name + '</option>'
                    }
                    $('#ddlCat').html('');
                    $('#ddlCat').append(options);
                    $('#ddlCat').change();
                }
            });
            $('#popupDocumentCreateConfirm').modal('hide');
            $('#mdlDocs').modal('show');
        }
    });
    $("#btnPopupDocumentCreateConfirm").click(function () {
        $('#popupDocumentCreateConfirm').modal('toggle');
    });

    $("#btndocumentTampleClose").click(function () {
        $('#DocumentTemplate').modal('toggle');
    });
    $('#btnAddExistingDoc').click(function (e) {
        e.preventDefault();
        var $selectedLi = $('#tbodyDocItemsForJob').find('input[type=checkbox]:checked').closest('li'),
            data = [];
        if ($selectedLi.length < 1) {
            alert("Please select atleast one document");
            return false;
        }
        for (var i = 0; i < $selectedLi.length; i++) {
            var $li = $($selectedLi[i]);
            data.push({
                name: $li.data('name'),
                path: $li.data('path'),
                TemplateName: $li.data('tempName') + ".pdf",
            });
        }
        var obj = {
            jobId: JobId,
            docs: data,
            UserId: USERID,
            fillData: true,
            UseNewDocTemplate: true,
            type: typeOfFile
        }
        $.post(urlAddOtherDocuments, obj, function (response) {
            if (response) {
                $tbody = $('#tbodyOther')
                if (typeOfFile.toLowerCase() == "ces")
                    $tbody = $('#tbodyCES')

                var failed = response.filter(e => e.JobDocumentId === 0),
                    data = response.filter(e => e.JobDocumentId != 0);

                for (var i = 0; i < data.length; i++) {
                    var tr = $('<tr/>')
                        .data('data', data[i])
                        .data('path', data[i].Path)
                        .data('id', data[i].JobDocumentId);

                    if (typeOfFile.toUpperCase() == "CES") {
                        tr.attr("id", "ces_" + data[i].JobDocumentId);
                    }
                    if (typeOfFile.toUpperCase() == "OTHER") {
                        tr.attr("id", "other_" + data[i].JobDocumentId);
                    }
                    tr.addClass("job-document-" + data[i].JobDocumentId);
                    tr.appendTo($tbody);

                    var tdForSelectFileCheckBox = $('<td/>')
                        .css("width", "2%")
                        .appendTo(tr);

                    var td = $('<td/>')
                        .text(data[i].name)
                        .appendTo(tr);

                    var td1 = $('<td/>')
                        .addClass('action')
                        .appendTo(tr);

                    if (typeOfFile.toUpperCase() == "CES") {//&& data[i].isUpload != true) {
                        var aSendMailDocSignForCES = $('<a/>')
                            .addClass('icon sprite-img email-ic document-signature-send-request')
                            .attr('href', 'javascript:void(0);')
                            .attr('title', 'Signature Mail')
                            .data('type', typeOfFile.toUpperCase())
                            .attr('data-filename', data[i].name)
                            //.attr('onclick', 'ShowDocumentSendSignateReuqest(' + data[i].JobDocumentId + ',"CES");')
                            .attr('onclick', "viewDocument('CES','','','" + data[i].Path + "',0,'ces_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                            .appendTo(td1);
                    }
                    if (typeOfFile.toUpperCase() == "OTHER") {//&& data[i].isUpload != true) {

                        var aSendMailDocSignForOther = $('<a/>')
                            .addClass('icon sprite-img email-ic document-signature-send-request')
                            .attr('href', 'javascript:void(0);')
                            .attr('title', 'Signature Mail')
                            .data('type', typeOfFile.toUpperCase())
                            .attr('data-filename', data[i].name)
                            .attr('onclick', "viewDocument('OTHER','','','" + data[i].Path + "',0,'other_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                            .appendTo(td1);

                        if (!(allowdViewTypes.indexOf(data[i].Path.toLowerCase().split('.').pop()) > -1)) {
                            OpacityIncreaseDecrease(aSendMailDocSignForOther, 0);
                        }
                    }
                    var aDownload = $('<a/>')
                        .addClass('sprite-img download pop')
                        .attr('href', 'javascript:void(0)')
                        .attr('data-container', "body")
                        .attr('data-toggle', "popover")
                        .attr('data-placement', "bottom")
                        .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data[i].JobDocumentId + ",0)>Download final</a>")
                        .appendTo(td1);
                    if ((USERType == "1" || USERType == "3") || $('#STCStatusId').val() == 10 || $('#STCStatusId').val() == 12 || $('#STCStatusId').val() == 14 || $('#STCStatusId').val() == 17) {

                        var aDelete = $('<a/>')
                            .addClass('sprite-img delete')
                            .attr('href', '#')
                            .attr('title', "delete")
                            .appendTo(td1);
                    }
                }
                if (failed.length > 0) {
                    var message = "";
                    for (var i = 0; i < failed.length; i++) {
                        message += '- ' + failed[i].message + '\n';
                    }
                    alert(message);
                }
                else {
                    showMessage("Document has been uploaded successfully.", false, typeOfFile);
                }

                if ($("#tbodyCES").find('tr').length == 0) {
                    $("#divDocumentsCES").find('.table-responsive').hide();
                    $("#divDocumentsCES").find('.noDocumentCES').show();
                }
                else {
                    $("#divDocumentsCES").find('.table-responsive').show();
                    $("#divDocumentsCES").find('.noDocumentCES').hide();
                }

                if (typeOfFile.toLowerCase() == "ces") {
                    if ($("#tbodyCES").find('tr').length == 1) {
                        ReloadSTCModule();
                    }
                }
                PopoverEnable();
                $('#DocumentTemplate').modal('hide');
                SearchHistory();
            }
        });
    });
});
$("#btnCreateSTCDocument").click(function () {
    $.get(urlGetAllDocumentTemplate + '?jobTypeId=' + parseInt($("#BasicDetails_JobType").val()) + "&solarCompanyId=" + $("#BasicDetails_SolarCompanyId").val() + "&isSTC=true", function (response) {
        var $tbody = $('#tbodySTCItems');
        $tbody.html('');
        if (response) {
            var data = response;
            for (var i = 0; i < data.length; i++) {
                var li = $('<li/>').addClass('list-group-item').data({ 'path': data[i].Path, 'name': data[i].Path.split('/').pop(), 'tempName': data[i].DocumentTemplateName }).appendTo($tbody);
                //var li = $('<li/>').addClass('list-group-item').data({ 'path': data[i].Path, 'name': data[i].DocumentTemplateName}).appendTo($tbody);
                var $div = $('<div/>').addClass('checkbox').appendTo(li);
                var $lable = $('<label/>').css({ 'word-wrap': 'break-word' }).appendTo($div);
                //var input = '<input type="checkbox">' + data[i].Path.split('/').pop();
                var input = '<input type="checkbox">' + data[i].DocumentTemplateName;
                $lable.html(input);
            }
        }
    });
    $('#CrateSTCDocument').modal('show');
});

$("#btnAddSTCExistingDoc").click(function (e) {
    e.preventDefault();
    var $selectedLi = $('#tbodySTCItems').find('input[type=checkbox]:checked').closest('li'),
        data = [];
    if ($selectedLi.length < 1) {
        alert("Please select atleast one document");
        return false;
    }
    for (var i = 0; i < $selectedLi.length; i++) {
        var $li = $($selectedLi[i]);
        data.push({
            name: $li.data('name'),
            path: $li.data('path'),
            stage: 'STC',
            TemplateName: $li.data('tempName') + ".pdf",
        });
    }
    var obj = {
        jobId: JobId,
        docs: data,
        fillData: true,
    };
    $.post(urlUploadSTCDocument, obj, function (response) {
        $('#CrateSTCDocument').modal('hide');
        if (response) {
            doneCallBack();
            SearchHistory();
        }
        else {
            showMessage("Something went to wrong. Please try after sometimes!", false, 'STC');
        }
    });
});
function PopoverEnable(e) {
    if (typeof (e) != "undefined") {
        e = e || window.event;
        e.preventDefault();
    }
    $(".pop").popover('hide');
    $(".pop").popover({ html: true })
}
function ShowDocumentSendSignateReuqest(JobDocId, type = "STC") {
    $('#SendMailDocumentSignatureRequest').modal({ backdrop: 'static', keyboard: false });
    $("#JobDocumentPreview").load(urlDocumentPreview + "?jobdocid=" + JobDocId + "&type=0");
}
function ShowPopupForEmailTamplete(PopupButton) {
    if ($("#DocumentSignatureRequestTableBody input[type='checkbox']:checked").length > 0) {
        SelectedUsersForSendMailRequestForDocumentSignature = [];
        $("#DocumentSignatureRequestTableBody input[type='checkbox']:checked").each(function (text, value) {
            if ($(value).attr("id") == "ChkInstaller")
                SelectedUsersForSendMailRequestForDocumentSignature.push(2);
            else if ($(value).attr("id") == "ChkDesigner")
                SelectedUsersForSendMailRequestForDocumentSignature.push(4);
            else if ($(value).attr("id") == "ChkElectrician")
                SelectedUsersForSendMailRequestForDocumentSignature.push(3);
            else if ($(value).attr("id") == "ChkHomeOwner")
                SelectedUsersForSendMailRequestForDocumentSignature.push(1);
            else if ($(value).attr("id") == "ChkSolarCompany")
                SelectedUsersForSendMailRequestForDocumentSignature.push(6);
        });
        FillDropDown('lstEmailTemplate', urlGetEmailTemplateList, null, true, null);
        $('#SendMailDocumentSignatureRequest').modal('toggle');
        $('#popupSendEmailForBulkSignature').modal({ backdrop: 'static', keyboard: false });
    }
    else {
        alert("Please select any one option.");
    }
}
function sendEmailForSignatureRequest() {
    if ($("#lstEmailTemplate").val() == "") {
        alert("please select email template ")
        return;
    }
    var data = {};
    $.ajax({
        url: urlSendEmailForSignatureRequest,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        data: JSON.stringify({ sendEmailRequest: data, emailTemplateId: $("#lstEmailTemplate").val(), jobDocId: $('#SendMailDocumentSignatureRequest').attr("jobdocid"), typesList: SelectedUsersForSendMailRequestForDocumentSignature }),
        success: function (response) {
            $('#popupSendEmailForBulkSignature').modal('toggle');
            $("#loading-image").hide();
            SearchHistory();
        },
        error: function () {
        }
    });
}
function ToggleModelPopup(modalname) {
    $(modalname).modal("toggle");
}
function fnDocViewerOuterPopup() {
    $("#DocViewerOuterPopup").html("");
    $("#DocViewerOuterPopup").load(urlDocViewerOuterPopup);
}
function viewDocument(stage, state, provName, name, id, obj, jobDocumentId) {
    var StcStatusId = $("#STCStatusId").val();
    $("#btnSendRequest").show();
    var data;
    var path;
    if (obj.indexOf("trDoc") > -1) {
        data = $("#divDocumentsSTC").find('table').find('#' + obj).data('data');
        path = $("#divDocumentsSTC").find('table').find('#' + obj).find('td').html();
        if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {
            $("#popupViewerDoc #btnSave").show();
        }
        else {
            $("#popupViewerDoc #btnSave").hide();
        }
    }
    else if (obj.indexOf("ces") > -1) {
        data = $("#tbodyCES #" + obj).data('data');
        path = $("#tbodyCES #" + obj).find('td:eq(1)').html();
        if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {
            $("#popupViewerDoc #btnSave").show();
        }
        else {
            $("#popupViewerDoc #btnSave").hide();
        }
    }
    else if (obj.indexOf("pnlinvoice") > -1) {
        data = $("#tbodyPnlInvoice #" + obj).data('data');
        path = $("#tbodyPnlInvoice #" + obj).find('td:eq(1)').html();
        //if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {            
        $("#popupViewerDoc #btnSave").show();
        //}
        //else {
        //    $("#popupViewerDoc #btnSave").hide();
        //}
    }
    else if (obj.indexOf("elecbill") > -1) {
        data = $("#tbodyElecBill #" + obj).data('data');
        path = $("#tbodyElecBill #" + obj).find('td:eq(1)').html();
        //if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {            
        $("#popupViewerDoc #btnSave").show();
        //}
        //else {
        //    $("#popupViewerDoc #btnSave").hide();
        //}
    }
    else if (obj.indexOf("other") > -1) {
        data = $("#tbodyOther #" + obj).data('data');
        path = $("#tbodyOther #" + obj).find('td:eq(1)').html();
        $("#popupViewerDoc #btnSave").show();
    }
    path = decodeURI(path);
    if (isImage(path)) {
        ViewPhotoDoc(data.Path);
        $('#btnPrev,#btnNext').attr('disabled', 'disabled');
        $('#hdrFileName').text(data.Path.split('\\').slice(data.Path.split('\\').length - 1));
    }
    else {
        $("#popupViewerDoc").data("data", null);
        $("#popupViewerDoc").data("data", data);
        $("#popupViewerDoc").attr("data-hdnIsUpload", $('tr#' + obj).find('.hdnIsUpload').attr('val'));

        if (eJobId) {
            if (obj.indexOf("trDoc") > -1) {
                if ($('tr#' + obj).find('.hdnIsExists') && $('tr#' + obj).find('.hdnIsExists').attr('val') && $('tr#' + obj).find('.hdnIsExists').attr('val') == '1') {
                    $("#loading-image").show();
                    $.ajaxSetup({ async: false, cache: false });
                    JobDocumentName = $('tr#' + obj).find(".document-signature-send-request").attr("data-filename");
                    $('#divViewer').load(url_Viewer + "?jobid=" + eJobId + "&docId=" + id + "&isClassic=false" + "&jobDocumentPath=" + encodeURIComponent(data.Path) + "&jobDocumentId=" + jobDocumentId);
                    $('#popupViewerDoc').modal({ backdrop: 'static', keyboard: false });
                    $("#popupViewerDoc").find(".modal-title span").html(JobDocumentName);
                    if (isJobDocumentManagerSave == 'false') {
                        $('#divViewer').find('form').find('#btnSave').hide();
                    }
                }
            }
            else {
                $("#loading-image").show();
                $.ajaxSetup({ async: false, cache: false });
                JobDocumentName = $('tr#' + obj).find(".document-signature-send-request").attr("data-filename");
                $('#divViewer').load(url_Viewer + "?jobid=" + eJobId + "&docId=" + id + "&isClassic=false" + "&jobDocumentPath=" + encodeURIComponent(data.Path) + "&jobDocumentId=" + jobDocumentId);
                $('#popupViewerDoc').modal({ backdrop: 'static', keyboard: false });
                $("#popupViewerDoc").find(".modal-title span").html(JobDocumentName);
                if (isJobDocumentManagerSave == 'false') {
                    $('#divViewer').find('form').find('#btnSave').hide();
                }
            }
            var SignatureRequestPopup = $('tr#' + obj).find(".document-signature-send-request");
        }
    }
}

function viewDocumeneCES(path) {
    if (path) {
        $("#loading-image").show();
        $.ajaxSetup({ async: false, cache: false });
        $('#divViewer').load(url_Viewer + "?path=" + path);
        $('#popupViewerDoc').modal({ backdrop: 'static', keyboard: false });
        if (isJobDocumentManagerSave == 'false') {
            $('#divViewer').find('form').find('#btnSave').hide();
        }
    }
}
function viewDocumentOther(id, type) {
    if (eJobId) {
        $("#loading-image").show();
        $.ajaxSetup({ async: false, cache: false });
        $('#divViewer').load(url_DocViewer + "?jobDocId=" + id + "&jobid=" + eJobId);
        $('#popupViewerDoc').modal({ backdrop: 'static', keyboard: false });
        if (isJobDocumentManagerSave == 'false') {
            $('#divViewer').find('form').find('#btnSave').hide();
        }
    }
}

function getDocuments(isUpload) {
    $.get(urlGetDocumentsListAll + "?id=" + JobId + "&distributorID=" + DistributorId + "&stage=stc&jobTypeId=1", "", function (response) {
        LoadDocuments(response, isUpload);
    });
}

function htmlDecode(input) {
    var doc = new DOMParser().parseFromString(input, "text/html");
    return doc.documentElement.textContent;
}

function LoadDocuments(response, isUpload) {
    if (response) {
        var data = JSON.parse(htmlDecode(response).replaceAll("\\", "\\\\")),
            cesData = data.Table,
            stcData = data.Table1,
            CESList = cesData.filter(e => e.Type === "CES"),
            STCList = cesData.filter(e => e.Type === "STC"),
            OtherList = cesData.filter(e => e.Type === "OTHER"),
            PnlInvoiceList = cesData.filter(e => e.Type === "PNLINVC"),
            ElecBillList = cesData.filter(e => e.Type === "ELECBILL"),
            $tbodyCES = $('#tbodyCES'),
            $tbodySTC = $('#tbodySTC'),
            $tbodyOther = $('#tbodyOther');
            $tbodyPnlInvoice = $('#tbodyPnlInvoice');
            $tbodyElecBill = $('#tbodyElecBill');
            setFiles($('#tbodyCES'), CESList, 'CES');
            setFiles($tbodyOther, OtherList, 'OTHER');
            setFiles($tbodyPnlInvoice, PnlInvoiceList, 'PNLINVC');
            setFiles($tbodyElecBill, ElecBillList, 'ELECBILL');
        if (STCList.length > 0) {

            if ($("#divDocumentsSTC").find('table').find('tr') && $("#divDocumentsSTC").find('table').find('tr').length > 0) {
                $("#divDocumentsSTC").find('table').find('tr').remove();
            }

            for (var i = 0; i < STCList.length; i++) {
                var d = STCList[i];
                d.Stage = 'STC';
                var name = d.Name || d.Path.split('\\').pop();
                d.Name = name;
                d.IsExist = true;
                var params = "'" + d.Stage + "','" + "" + "','" + "" + "','" + escape(d.Path) + "','" + 0 + "','" + ('trDoc_' + i) + "','" + d.JobDocumentId + "'";
                var exParams = "'" + d.Stage + "','" + "" + "','" + "" + "','" + escape(d.Path) + "','" + d.Id + "','" + "uploadBtnPhoto_" + d.DocumentId + "'";
                var td = $('<td>').attr('class', 'action');
                var DocumentStatusClass = "";
                if (d.IsCompleted == true)
                    DocumentStatusClass = "document-status-green";
                else if (d.SentEmailStatus == 1)
                    DocumentStatusClass = "document-status-orange";

                td.append('<input type="hidden" class="hdnIsExists" val="' + (d.IsExist == true ? 1 : 0) + '" />')
                    .append('<input type="hidden" class="hdnIsUpload" val="' + (d.isUpload == true ? 1 : 0) + '" />')

                var docFunctions = (isJobDocumentManagerSignatureMail == 'True') ? '<a href="javascript:void(0);" class="' + DocumentStatusClass + ' icon sprite-img email-ic document-signature-send-request" data-filename="' + d.Name + '" onclick="' + "viewDocument(" + params + ");" + '" title = "Signature Mail">' : '';
                td.append(docFunctions);
                docFunctions = (isJobDocumentManagerGenerate == 'True') ? '<a class="ganret sprite-img" href="' + "javascript:void(0);" + '"onclick="' + "generateDocument(" + params + ");" + '" title="Generate">' : '';
                td.append(docFunctions);
                docFunctions = (isJobDocumentManagerView == 'True') ? '<a class="sprite-img download pop ' + ((d.IsExist && d.IsExist == true) ? '' : 'notdownload') + '" href="' + "javascript:void(0);" + '" data-container="body" data-toggle="popover" data-placement="bottom" data-content="<a href=&quot;javascript:void(0);&quot; class=&quot;editable-pdf pdf-down&quot; onclick=&quot;downloadDocument(' + params + ',1)&quot;>Download edit</a><a href=&quot;javascript:void(0);&quot; class=&quot;non-editable-pdf pdf-down&quot; onclick=&quot;downloadDocument(' + params + ',0)&quot;>Download final</a>">' : '';
                td.append(docFunctions);
                docFunctions = (isJobDocumentManagerDelete == 'True') ? '<a class="delete sprite-img' + ((d.IsExist && d.IsExist == true) ? '' : 'disDelete') + '" href="' + "javascript:void(0);" + '"onclick="' + "deleteDocument(" + params + ");" + '" title="Delete">' : '';
                td.append(docFunctions);

                var tr = $('<tr id="trDoc_' + i + '" class="job-document-' + d.JobDocumentId + '" >')
                    .append($('<td class="tdDoc ' + ((d.IsExist && d.IsExist == true) ? 'semiBold' : '') + '">').attr('style', 'text-align:left!important;word-break: break-all;').html(d.Name))
                    .append(td);
                tr.data('data', d);

                if (imageTypes.indexOf($(tr).find('td').html().toLowerCase().split('.').pop()) > -1) {
                    OpacityIncreaseDecrease($(tr).find('.ganret'), 0);
                }

                $tbodySTC.append(tr);

                if (isUpload) {
                    showMessage("Document has been uploaded successfully.", false, 'STC');
                }

                $("#divDocumentsSTC").find('.table-responsive').show();
                $("#divDocumentsSTC").find('.noDocument').hide();
            }
            //reloadSTCdocument
            if (isUpload && $("#divDocumentsSTC").find('table').find('tr') && $("#divDocumentsSTC").find('table').find('tr').length == 1) {
                $("#loading-image").show();
                setTimeout(function () {
                    ReloadSTCJobScreen(eJobId);
                }, 500);
            }
        }
        else {
            $("#divDocumentsSTC").find('.table-responsive').hide();
            $("#divDocumentsSTC").find('.noDocument').show();
        }
    }
    assignEvent();
    PopoverEnable();
}
function DownloadGPSLogFiles() {
    window.location.href = urlDownloadGPSLogs + "?jobId=" + JobId;
}

$('#btnUploadPnlInvoiceDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('uploadPnlInvoiceBtnDocument').click();
});

$('#btnUploadElecBillDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('uploadElecBillBtnDocument').click();
});

$('#uploadPnlInvoiceBtnDocument').fileupload({
    url: urlDocumentCES,
    dataType: 'json',
    done: function (e, data) {
        var UploadFailedFiles = [];
        UploadFailedFiles.length = 0;
        var UploadFailedFilesName = [];
        UploadFailedFilesName.length = 0;

        var UploadFailedFilesType = [];
        UploadFailedFilesType.length = 0;
        var $tbodyPnlInvoice = $('#tbodyPnlInvoice');
        //formbot start
        for (var i = 0; i < data.result.length; i++) {
            if (data.result[i].Status == true) {

                var path = 'JobDocuments\\' + JobId + '\\Panel Invoice\\' + data.result[i].FileName;

                var tr = $('<tr/>')
                    .data('data', data.result[i])
                    .data('path', path)
                    .data('id', data.result[i].AttachmentID)
                    .attr('id', 'pnlinvoice_' + data.result[i].AttachmentID)
                    .addClass("job-document-" + data.result[i].AttachmentID)
                    .appendTo($tbodyPnlInvoice);
                var tdForSelectFileCheckBox = $('<td/>')
                    .css("width", "2%")
                    //.html('<input class="chkDocument" data-jobdocid="' + data.result[i].AttachmentID +'" title="Select Document" type="checkbox">')
                    .appendTo(tr);
                var td = $('<td/>')
                    .text(data.result[i].FileName)
                    .appendTo(tr);
                var td2 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var td1 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var aSendMailDocSignForPnlInvoice = $('<a/>')
                    .addClass('icon sprite-img email-ic document-signature-send-request')
                    .attr('href', 'javascript:void(0);')
                    .attr('title', 'Signature Mail')
                    .data('type', "PNLINVC")
                    .attr('data-filename', data.result[i].FileName)
                    .attr('onclick', 'viewDocument("PNLINVC","","","' + path + '",0,"pnlinvoice_' + data.result[i].AttachmentID + '","' + data.result[i].AttachmentID + '");')
                    .appendTo(td1);

                if (!(allowdViewTypes.indexOf(path.toLowerCase().split('.').pop()) > -1)) {
                    OpacityIncreaseDecrease(aSendMailDocSignForPnlInvoice, 0);
                }
                if (USERType == "1" || USERType == "3") {
                    var aDelete = $('<a/>')
                        .addClass('sprite-img delete')
                        .attr('href', '#')
                        .attr('title', 'Delete')
                        .appendTo(td1);
                }

                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

            }
            else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                UploadFailedFilesType.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result[i].FileName);
            }
            else {
                UploadFailedFiles.push(data.result[i].FileName);

            }
            PopoverEnable();
            if ($tbodyPnlInvoice.children().length == 0) {
                $("#divDocumentsPnlInvoice").find('.table-responsive').hide();
                $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').show();
            }
            else {
                $("#divDocumentsPnlInvoice").find('.table-responsive').show();
                $("#divDocumentsPnlInvoice").find('.noDocumentPnlInvoice').hide();
            }

            if ($("#tbodyPnlInvoice").find('tr').length == 1) {
                ReloadSTCModule();
            }

        }
        if (UploadFailedFiles.length > 0) {
            showMessage(UploadFailedFiles.length + " " + "File has not been uploaded.", true, 'PNLINVC');
            return false;
        }
        if (UploadFailedFilesType.length > 0) {
            showMessage(UploadFailedFilesType.length + " " + "Uploaded file type is not supported.", true, 'PNLINVC');
            return false;
        }
        else {
            showMessage("File has been uploaded successfully.", false, 'PNLINVC');
        }
        SearchHistory();
    },
    progressall: function (e, data) {

    },
    singleFileUploads: false,
    send: function (e, data) {
        if (data.files.length == 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].name.length > 50) {
                    showMessage("Please upload small filename.", true, 'PNLINVC');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'PNLINVC')
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true, 'PNLINVC');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'PNLINVC')
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true, 'PNLINVC');
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
    formData: { UserId: USERID, JobId: JobId, Type: "PNLINVC" },
    change: function (e, data) {
        $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
    }
}).prop('disabled', !$.support.fileInput);

$('#uploadElecBillBtnDocument').fileupload({
    url: urlDocumentCES,
    dataType: 'json',
    done: function (e, data) {
        var UploadFailedFiles = [];
        UploadFailedFiles.length = 0;
        var UploadFailedFilesName = [];
        UploadFailedFilesName.length = 0;

        var UploadFailedFilesType = [];
        UploadFailedFilesType.length = 0;
        var $tbodyElecBill = $('#tbodyElecBill');
        //formbot start
        for (var i = 0; i < data.result.length; i++) {
            if (data.result[i].Status == true) {

                var path = 'JobDocuments\\' + JobId + '\\Electricity Bill\\' + data.result[i].FileName;

                var tr = $('<tr/>')
                    .data('data', data.result[i])
                    .data('path', path)
                    .data('id', data.result[i].AttachmentID)
                    .attr('id', 'elecbill_' + data.result[i].AttachmentID)
                    .addClass("job-document-" + data.result[i].AttachmentID)
                    .appendTo($tbodyElecBill);
                var tdForSelectFileCheckBox = $('<td/>')
                    .css("width", "2%")
                    //.html('<input class="chkDocument" data-jobdocid="' + data.result[i].AttachmentID +'" title="Select Document" type="checkbox">')
                    .appendTo(tr);
                var td = $('<td/>')
                    .text(data.result[i].FileName)
                    .appendTo(tr);
                var td2 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var td1 = $('<td/>')
                    .addClass('action')
                    .appendTo(tr);
                var aSendMailDocSignForElecBill = $('<a/>')
                    .addClass('icon sprite-img email-ic document-signature-send-request')
                    .attr('href', 'javascript:void(0);')
                    .attr('title', 'Signature Mail')
                    .data('type', "PNLINVC")
                    .attr('data-filename', data.result[i].FileName)
                    .attr('onclick', 'viewDocument("ELECBILL","","","' + path + '",0,"elecbill_' + data.result[i].AttachmentID + '","' + data.result[i].AttachmentID + '");')
                    .appendTo(td1);

                if (!(allowdViewTypes.indexOf(path.toLowerCase().split('.').pop()) > -1)) {
                    OpacityIncreaseDecrease(aSendMailDocSignForElecBill, 0);
                }

                if (USERType == "1" || USERType == "3") {
                    var aDelete = $('<a/>')
                        .addClass('sprite-img delete')
                        .attr('href', '#')
                        .attr('title', 'Delete')
                        .appendTo(td1);
                }

                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

            }
            else if (data.result[i].Status == false && data.result[i].Message == "File Type Not Allowed.") {
                UploadFailedFilesType.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result[i].FileName);
            }
            else {
                UploadFailedFiles.push(data.result[i].FileName);

            }
            PopoverEnable();
            if ($tbodyElecBill.children().length == 0) {
                $("#divDocumentsElecBill").find('.table-responsive').hide();
                $("#divDocumentsElecBill").find('.noDocumentElecBill').show();
            }
            else {
                $("#divDocumentsElecBill").find('.table-responsive').show();
                $("#divDocumentsElecBill").find('.noDocumentElecBill').hide();
            }

            if ($("#tbodyElecBill").find('tr').length == 1) {
                ReloadSTCModule();
            }

        }
        if (UploadFailedFiles.length > 0) {
            showMessage(UploadFailedFiles.length + " " + "File has not been uploaded.", true, 'ELECBILL');
            return false;
        }
        if (UploadFailedFilesType.length > 0) {
            showMessage(UploadFailedFilesType.length + " " + "Uploaded file type is not supported.", true, 'ELECBILL');
            return false;
        }
        else {
            showMessage("File has been uploaded successfully.", false, 'ELECBILL');
        }
        SearchHistory();
    },
    progressall: function (e, data) {

    },
    singleFileUploads: false,
    send: function (e, data) {
        if (data.files.length == 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].name.length > 50) {
                    showMessage("Please upload small filename.", true, 'ELECBILL');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'ELECBILL')
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true, 'ELECBILL');
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessage("Please upload file that not conatain <> ^ [] .", true, 'ELECBILL')
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true, 'ELECBILL');
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
    formData: { UserId: USERID, JobId: JobId, Type: "ELECBILL" },
    change: function (e, data) {
        $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
    }
}).prop('disabled', !$.support.fileInput);

$('#divDocumentsPnlInvoice').on('click', '.delete', function (e) {
    e.preventDefault();

    var $tr = $(this).closest('tr');
    deleteFile($tr, true, 'PNLINVC');
});

$('#divDocumentsElecBill').on('click', '.delete', function (e) {
    e.preventDefault();

    var $tr = $(this).closest('tr');
    deleteFile($tr, true, 'ELECBILL');
});