var accordiantabName = "";
var isaccordianLoaded = false;
var isSTCLoaded = false;
var isCESLoaded = false;
var isOtherLoaded = false;
var PreviousActive = "";

$('#btnUploadCESDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('uploadCESBtnDocument').click();
});
$('#btnCreateCESDocument').click(function (e) {
    typeOfFile = "CES";
    e.preventDefault();
    $('#mdlcreateDocsConfirm').show();
    $('#mdlcreateDocsExist').hide();
    $('#mdlcreateDocs').hide();
    $('#popupDocumentCreateConfirmTabular').modal('show');
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
        $("#loading-image").show();
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
                /*var documentspath = Path.split('\\');*/
                var documentName = data.result[i].FileName;
                $("#ddlviewdocumentsName").append($("<option></option>").val(data.result[i].AttachmentID).attr('data-id', 'ces_' + data.result[i].AttachmentID).attr('data-path', path).html(documentName));
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
                    .appendTo(tr);

                var aViewDocForCES = $('<a/>')
                    .attr('href', 'javascript:void(0);')
                    .attr('role', 'button')
                    .attr('title', 'CES Document')
                    .attr('class', 'document-color')
                    .attr('data-filename', data.result[i].Path.split('\\').pop())
                    .attr('onclick', 'viewDocumentonPage("CES","","","' + path + '",0,"ces_' + data.result[i].AttachmentID + '","' + data.result[i].AttachmentID + '");')
                    .text(data.result[i].FileName)
                    .appendTo(td);



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
                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

                if ((USERType == "1" || USERType == "3") || $('#STCStatusId').val() == 10 || $('#STCStatusId').val() == 12 || $('#STCStatusId').val() == 14 || $('#STCStatusId').val() == 17) {
                    var aDelete = $('<a/>')
                        .addClass('sprite-img delete')
                        .attr('href', '#')
                        .attr('title', 'Delete')
                        .appendTo(td1);
                }


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
                var documentName = data.result[i].FileName;
                $("#ddlviewdocumentsName").append($("<option></option>").val(data.result[i].AttachmentID).attr('data-id', 'ces_' + data.result[i].AttachmentID).attr('data-path', path).html(documentName));
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
                    .appendTo(tr);
                var aViewDocForCES = $('<a/>')
                    .attr('href', 'javascript:void(0);')
                    .attr('title', 'Other Document')
                    .attr('class', 'document-color')
                    .attr('role', 'button')
                    .attr('data-filename', data.result[i].Path.split('\\').pop())
                    .attr('onclick', "viewDocumentonPage('OTHER','','','" + encodeURIComponent(path) + "',0,'other_" + data.result[i].AttachmentID + "','" + data.result[i].AttachmentID + "');")
                    .text(data.result[i].FileName)
                    .appendTo(td);
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

                var aDownload = $('<a/>')
                    .addClass('sprite-img download pop')
                    .attr('href', 'javascript:void(0)')
                    .attr('data-container', "body")
                    .attr('data-toggle', "popover")
                    .attr('data-placement', "bottom")
                    .attr('data-content', "<a href=javascript:void(0); class='editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",1)>Download edit</a><a href=javascript:void(0); class='non-editable-pdf pdf-down' onclick=DownloadJobDocuments(" + data.result[i].AttachmentID + ",0)>Download final</a>")
                    .appendTo(td1);

                var aDelete = $('<a/>')
                    .addClass('sprite-img delete')
                    .attr('href', '#')
                    .attr('title', 'Delete')
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
    assignEvent();
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
            params = "'CES','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("ces_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
        }
        if (type == "OTHER") {
            tr.attr("id", "other_" + data[i].JobDocumentId);
            tr.attr("isreczip", data[i].IsRecZip);
            params = "'OTHER','" + "" + "','" + "" + "','" + data[i].Path + "','" + 0 + "','" + ("other_" + data[i].JobDocumentId) + "','" + data[i].JobDocumentId + "'";
        }
        tr.appendTo($tbody);
        var tdForCheckbox = $('<td/>')
            .css("width", "2%")
            .appendTo(tr);

        if (((data[i].IsSPVXml == null || !data[i].IsSPVXml) || ((data[i].IsRecZip == null || !data[i].IsRecZip))) || ((data[i].IsRecZip || data[i].IsSPVXml) && (USERType == "1" || USERType == "3"))) {
            var td = $('<td/>')

                .appendTo(tr);
            if (type == "CES") {
                var aViewDocForCES = $('<a/>')
                    .attr('href', 'javascript:void(0);')
                    .attr('role', 'button')
                    .attr('title', 'CES Document')
                    .attr('class', 'document-color')
                    .attr('data-filename', data[i].Path.split('\\').pop())
                    .data('type', type)
                    .attr('onclick', 'viewDocumentonPage(' + params + ')')
                    .text(data[i].Path.split('\\').pop())
                    .appendTo(td);
            }
            if (type == "OTHER") {
                var aViewDocForCES = $('<a/>')
                    .attr('href', 'javascript:void(0);')
                    .attr('role', 'button')
                    .attr('title', 'Other Document')
                    .attr('class', 'document-color')
                    .attr('data-filename', data[i].Path.split('\\').pop())
                    .data('type', type)
                    .attr('onclick', 'viewDocumentonPage(' + params + ')')
                    .text(data[i].Path.split('\\').pop())
                    .appendTo(td);
            }


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
            var aDelete = $('<a/>')
                .addClass('sprite-img delete')
                .attr('href', '#')
                .attr('title', 'Delete')
                .appendTo(td1);
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
        url: urlCheckBusinessRulesTabular,
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

            if (doneCallBack) {
                showMessage("Document has been Uploaded successfully.", false, 'STC');
                accordiantabName = "STC";
                doneCallBack();
            }

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
    $('#mdlcreateDocsConfirm').show();
    $('#mdlcreateDocsExist').hide();
    $('#mdlcreateDocs').hide();
    $('#popupDocumentCreateConfirmTabular').modal('show');
});
$('#ddlCatCreateDoc').change(function () {
    if (this.value && this.value > 0) {
        $.get(urlGetDocumentsByStateId + "?stateId=" + this.value, function (response) {
            var $tbody = $('#tbodyItemsCreateDoc');
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
$('#btnAddDocTabular').click(function (e) {

    e.preventDefault();
    var $selectedLi = $('#tbodyItemsCreateDoc').find('input[type=checkbox]:checked').closest('li'),
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
                //var td = $('<td/>')
                //    .text(data[i].name)
                //    .appendTo(tr);
                if (typeOfFile.toUpperCase() == "CES") {
                    $("#ddlviewdocumentsName").append($("<option></option>").val(data[i].JobDocumentId).attr('data-id', 'ces_' + data[i].JobDocumentId).attr('data-path', data[i].Path).html(data[i].name));
                    var td9 = $('<td/>')

                        .appendTo(tr);
                    var td = $('<a/>')
                        .attr('href', 'javascript:void(0);')
                        .attr('role', 'button')
                        .attr('title', 'CES Document')
                        .attr('class', 'document-color')
                        .attr('data-filename', data[i].Path)
                        .attr('onclick', "viewDocumentonPage('CES','','','" + data[i].Path + "',0,'ces_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                        .text(data[i].name)
                        .appendTo(td9);
                }
                if (typeOfFile.toUpperCase() == "OTHER") {
                    $("#ddlviewdocumentsName").append($("<option></option>").val(data[i].JobDocumentId).attr('data-id', 'other_' + data[i].JobDocumentId).attr('data-path', data[i].Path).html(data[i].name));
                    var td9 = $('<td/>')

                        .appendTo(tr);
                    var td = $('<a/>')
                        .attr('href', 'javascript:void(0);')
                        .attr('role', 'button')
                        .attr('title', 'OTHER Document')
                        .attr('class', 'document-color')
                        .attr('data-filename', data[i].Path)
                        .attr('onclick', "viewDocumentonPage('OTHER','','','" + data[i].Path + "',0,'other_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                        .text(data[i].name)
                        .appendTo(td9);
                }
                var td2 = $('<td/>')
                    .addClass('action')
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
                showMessage("Document has been created successfully.", false, typeOfFile);
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
            $('#popupDocumentCreateConfirmTabular').modal('hide');
            SearchHistory();
        }
    });
});
$('#btnClear').click(function (e) {
    e.preventDefault();
});
$('#tbodyCES,#tbodyOther').on('click', '.view', function (e) {
    e.preventDefault();
    var $this = $(this);
    var path = encodeURIComponent($this.closest('tr').data('path'));
    $('#loading-image').show();
    setTimeout(function () {
        if (isImage(path)) {
            viewimgonDocPage(path);
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

    $.ajax({
        type: 'GET',
        url: urlCheckJobDocuments,
        dataType: 'json',
        async: false,
        data: { jobDocId: id, isEditable: isEditable },
        success: function (result) {


            if (result.status) {
                window.location.href = urlDownloadJobDocuments + "?jobDocId=" + id + "&isEditable=" + isEditable;
            }
            else {
                alert("File not exists!!!");
            }
        },
        error: function (ex) {
            alert('File not exists');
        }
    });
    var checkfileexist = urlCheckJobDocuments + "?jobDocId=" + id + "&isEditable=" + isEditable;


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
    $.ajax({
        type: 'GET',
        url: urlisExistsDownloadAllDocumentsNew,
        dataType: 'json',
        async: false,
        data: { jobid: JobId, type: type },
        success: function (result) {

            if (result.response) {
                alert('Files Download successful');
                window.location.href = urlDownloadAllDocumentsNew + "?jobid=" + JobId + "&type=" + type;

            } else {
                //var File = result.folder.split('\\');
                alert('Multiple files exists with same name');
            }



        },
        error: function (ex) {

            alert('File not exists');
        }
    });

    SearchHistory();

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
    $("#nextDocumentTemplateTabular").click(function () {

        if ($("#docTemplateIdTabular").val() == 1) {

            var isCES = 0;
            if (typeOfFile.toLowerCase() == "ces")
                isCES = "true";
            else
                isCES = "false";

            $.get(urlGetAllDocumentTemplate + '?jobTypeId=' + parseInt($("#BasicDetails_JobType").val()) + "&solarCompanyId=" + $("#BasicDetails_SolarCompanyId").val() + "&isSTC=false" + "&isCES=" + isCES, function (response) {
                var $tbody = $('#tbodyDocItemsForJobTabular');
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
            $('#mdlcreateDocsConfirm').hide();
            $('#mdlcreateDocsExist').show();
            $('#mdlcreateDocs').hide();
            //$('#popupDocumentCreateConfirmTabular').modal('toggle');
            //$('#DocumentTemplate').modal('show');
        }
        else {
            $.get(urlGetStateList, function (response) {
                if (response) {
                    var states = JSON.parse(response).Table;
                    var options = "";
                    for (var i = 0; i < states.length; i++) {
                        options += '<option value=' + states[i].StateID + '>' + states[i].Name + '</option>'
                    }
                    $('#ddlCatCreateDoc').html('');
                    $('#ddlCatCreateDoc').append(options);
                    $('#ddlCatCreateDoc').change();
                }
            });
            $('#mdlcreateDocsConfirm').hide();
            $('#mdlcreateDocsExist').hide();
            $('#mdlcreateDocs').show();
            //$('#popupDocumentCreateConfirmTabular').modal('hide');
            //$('#mdlcreateDocs').modal('show');
        }
    });
    $("#btnPopupDocumentCreateConfirmtabular").click(function () {
        $('#popupDocumentCreateConfirmTabular').modal('toggle');
    });

    $("#btndocumentTampleClose").click(function () {
        $('#DocumentTemplate').modal('toggle');
    });
    $('#btnAddExistingDocTabular').click(function (e) {
        e.preventDefault();
        var $selectedLi = $('#tbodyDocItemsForJobTabular').find('input[type=checkbox]:checked').closest('li'),
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
                if (failed.length > 0) {
                    var message = "";
                    for (var i = 0; i < failed.length; i++) {
                        message += '- ' + failed[i].message + '\n';
                    }
                    alert(message);
                }
                else {
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

                        if (typeOfFile.toUpperCase() == "CES") {
                            $("#ddlviewdocumentsName").append($("<option></option>").val(data[i].JobDocumentId).attr('data-id', 'ces_' + data[i].JobDocumentId).attr('data-path', data[i].Path).html(data[i].name));
                            var td9 = $('<td/>')

                                .appendTo(tr);
                            var td = $('<a/>')
                                .attr('href', 'javascript:void(0);')
                                .attr('role', 'button')
                                .attr('title', 'CES Document')
                                .attr('class', 'document-color')
                                .attr('data-filename', data[i].Path)
                                .attr('onclick', "viewDocumentonPage('CES','','','" + data[i].Path + "',0,'ces_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                                .text(data[i].name)
                                .appendTo(td9);
                        }
                        if (typeOfFile.toUpperCase() == "OTHER") {
                            $("#ddlviewdocumentsName").append($("<option></option>").val(data[i].JobDocumentId).attr('data-id', 'other_' + data[i].JobDocumentId).attr('data-path', data[i].Path).html(data[i].name));
                            var td9 = $('<td/>')

                                .appendTo(tr);
                            var td = $('<a/>')
                                .attr('href', 'javascript:void(0);')
                                .attr('role', 'button')
                                .attr('title', 'OTHER Document')
                                .attr('class', 'document-color')
                                .attr('data-filename', data[i].Path)
                                .attr('onclick', "viewDocumentonPage('OTHER','','','" + data[i].Path + "',0,'other_" + data[i].JobDocumentId + "','" + data[i].JobDocumentId + "');")
                                .text(data[i].name)
                                .appendTo(td9);
                        }
                        var td2 = $('<td/>')
                            .addClass('action')
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
                    showMessage("Document has been created successfully.", false, typeOfFile);
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
        if (response.response) {

            accordiantabName = "STC";
            getDocuments(false);
            showMessage("Document has been Created successfully.", false, 'STC');
            SearchHistory();
        }
        else {
            showMessage("Something went to wrong. Please try after sometimes!", true, 'STC');
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

function viewimgonDocPage(imgfullPath) {
    
    /*$('#showphotoOnPage').show();*/
    $('#imgonPageview').attr('src', imgfullPath);
    $('#onpagedivViewer').css('border', '18px solid #f0f0f0');
    $('#onpagedivViewer').html('');
    $('#onpagedivViewer').append('<div class="row" id="showphotoOnPage">');
    /*$('#onpagedivViewer').append('<div class="row" id="showphotoOnPage">');  */
    $('<img style="width: 100%;"/>').attr('src', "" + imgfullPath + "").attr('alt', "").appendTo($('#showphotoOnPage'));
    /*$('#onpagedivViewer').append('</div >');*/
    $('#onpagedivViewer').show();
    /*$('loading-image').hide();*/
    $("#docActionSection").css('visibility', 'visible');
}

//function ViewPreviousNext() {
//    
//    var ElementID = '';
//    $('#collapseReference li').each(function (i) {
//        var index = $(this).index();
//        var text = $(this).text();
//        var value = $(this).attr('value');
//        alert('Index is: ' + index + ' and text is ' + text + ' and Value ' + value);
//    });
//}

function viewDocumentonPage(stage, state, provName, name, id, obj, jobDocumentId) {
    
    /*$("#loading-image").show();*/

    $('#showphotoOnPage').hide();
    $("#onpageViewerDoc").show();
    var StcStatusId = $("#STCStatusId").val();
    $("#btnSendRequest").show();
    var data;
    var path;
    var documentname;
    if (obj.indexOf("trDoc") > -1) {
        data = $("#divDocumentsSTC").find('table').find('#' + obj).data('data');
        path = $("#divDocumentsSTC").find('table').find('#' + obj).find('td').html();
        documentname = $("#divDocumentsSTC").find('table').find('#' + obj).find('td').find('a').data('filename');
        //if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {
        //    
        //    $("#popupViewerDoc #btnSave").show();
        //}
        //else {
        //    $("#popupViewerDoc #btnSave").hide();
        //}
    }
    else if (obj.indexOf("ces") > -1) {
        data = $("#tbodyCES #" + obj).data('data');
        path = $("#tbodyCES #" + obj).find('td:eq(1)').html();
        documentname = $("#tbodyCES #" + obj).find('td:eq(1)').find('a').data('filename');
        //if (USERType == 1 || USERType == 3 || isSaveDocAfterTradeJob == "True" || StcStatusId == "10" || StcStatusId == "12" || StcStatusId == "14" || StcStatusId == "17") {
        //    
        //    $("#popupViewerDoc #btnSave").show();
        //}
        //else {
        //    $("#popupViewerDoc #btnSave").hide();
        //}
    }
    else if (obj.indexOf("other") > -1) {
        data = $("#tbodyOther #" + obj).data('data');
        path = $("#tbodyOther #" + obj).find('td:eq(1)').html();
        documentname = $("#tbodyOther #" + obj).find('td:eq(1)').find('a').data('filename');
        //$("#popupViewerDoc #btnSave").show();
    }
    path = decodeURI(path);
    if (path != null) {
        
        if (data == null || data == 'undefined') {
            var pathname = name.split('\\').slice(name.split('\\').length - 1);
            documentname = pathname[0];
        }
        if ((path != null && path != 'undefined') && isImage(documentname)) {
            /*ViewPhotoDoc(data.Path);*/
            path = uploadPath + data.Path;
            viewimgonDocPage(path);
            $('#btnPrev,#btnNext').attr('disabled', 'disabled');
            $("#ddlviewdocumentsName").val(jobDocumentId);
        }
        else {
            if ((data == null || data == 'undefined') && isImage(documentname)) {
                documentname = name.split('\\').slice(name.split('\\').length - 1);
                path = uploadPath + name;
                viewimgonDocPage(path);
                $('#btnPrev,#btnNext').attr('disabled', 'disabled');
                $("#ddlviewdocumentsName").val(jobDocumentId);
            }
            else {
                $("#docActionSection").css('visibility', 'hidden');
                $("#onpageViewerDoc").data("data", null);
                $("#onpageViewerDoc").data("data", data);
                $("#onpageViewerDoc").attr("data-hdnIsUpload", $('tr#' + obj).find('.hdnIsUpload').attr('val'));

                if (eJobId) {
                    $("#loading-image").show();                    
                    $.ajaxSetup({
                        async: false, cache: false, success: function () {
                            $("#" + PreviousActive).css("background-color", ""); $("#" + obj).css("background-color", "#ebdb9f");
                            $("#ddlviewdocumentsName").val(jobDocumentId);

                            $("#lblOpenDocumentName").text(': ' + $('tr#' + obj).find(".document-signature-send-request").attr("data-filename"))
                        }
                    });                   
                    JobDocumentName = $('tr#' + obj).find(".document-signature-send-request").attr("data-filename");
                    $('#onpagedivViewer').css('border', '18px solid #f0f0f0');
                    if (data == null) {
                        $('#onpagedivViewer').load(url_OnPageViewer + "?jobid=" + eJobId + "&docId=" + id + "&isClassic=false" + "&jobDocumentPath=" + encodeURIComponent(name) + "&jobDocumentId=" + jobDocumentId);
                    } else {
                        $('#onpagedivViewer').load(url_OnPageViewer + "?jobid=" + eJobId + "&docId=" + id + "&isClassic=false" + "&jobDocumentPath=" + encodeURIComponent(data.Path) + "&jobDocumentId=" + jobDocumentId);
                    }

                    //$('#onpageViewerDoc').modal({ backdrop: 'static', keyboard: false });
                    $("#onpageViewerDoc").find(".doc-title span").html(JobDocumentName);
                    var SignatureRequestPopup = $('tr#' + obj).find(".document-signature-send-request");
                }
            }
        }

        $.get(urlGetDocumentsddlListAll + "?id=" + JobId + "&distributorID=" + DistributorId + "&stage=stc&jobTypeId=1", "", function (response) {
            var responseData = htmlDecode(response),                
                data = JSON.parse(responseData),
                allList = data.Table;
            $.each(allList, function (data, value) {                
                if (value.JobDocumentId == jobDocumentId) {
                    i = data;
                    $("#" + PreviousActive).css("background-color", ""); $("#" + obj).css("background-color", "#ebdb9f");
                    $("#ddlviewdocumentsName").val(jobDocumentId);

                    $("#lblOpenDocumentName").text(': ' + $('tr#' + obj).find(".document-signature-send-request").attr("data-filename"))
                    var previous = i - 1;
                    var next = i + 1;
                    if (i == 0) {
                        previous = allList.length - 1;
                    }
                    else if (i == allList.length - 1) {
                        next = 0;
                    }
                    if (allList[next].JobDocumentId > 0) {                      
                            if (allList[next].Type == "CES") {
                                $('#forward-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[next].Path) + '",0,"ces_' + allList[next].JobDocumentId + '","' + allList[next].JobDocumentId + '");');
                            }
                            else if (allList[next].Type == "STC") {
                                $('#forward-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[next].Path) + '",0,"trDoc_' + data + '","' + allList[next].JobDocumentId + '");');
                            }
                            else if (allList[next].Type == "OTHER") {
                                $('#forward-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[next].Path) + '",0,"other_' + allList[next].JobDocumentId + '","' + allList[next].JobDocumentId + '");');
                            }                        
                    }
                    if (allList[previous].JobDocumentId > 0) {                        
                            if (allList[previous].Type == "CES") {
                                $('#backwards-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[previous].Path) + '",0,"ces_' + allList[previous].JobDocumentId + '","' + allList[previous].JobDocumentId + '");');
                            }
                            else if (allList[previous].Type == "STC") {
                                $('#backwards-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[previous].Path) + '",0,"trDoc_' + data + '","' + allList[previous].JobDocumentId + '");');
                            }
                            else if (allList[previous].Type == "OTHER") {
                                $('#backwards-icon-doc').attr('onclick', 'viewDocumentonPage("","","","' + replacestrings(allList[previous].Path) + '",0,"other_' + allList[previous].JobDocumentId + '","' + allList[previous].JobDocumentId + '");');
                            }                        
                    }

                }
            });
        });

    }
    PreviousActive = obj;
}

function replacestrings(Replacestring) {

    var oldSubstring = "\\";
    var newSubstring = "\\\\";
    var newString = "";
    var index = 0;
    while (index < Replacestring.length) {
        var currentIndex = Replacestring.indexOf(oldSubstring, index);
        if (currentIndex === -1) {
            newString += Replacestring.substring(index);
            break;
        } else {
            newString += Replacestring.substring(index, currentIndex) + newSubstring;
            index = currentIndex + oldSubstring.length;
        }
    }

    return newString;

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
    else if (obj.indexOf("other") > -1) {
        data = $("#tbodyOther #" + obj).data('data');
        path = $("#tbodyOther #" + obj).find('td:eq(1)').html();
        $("#popupViewerDoc #btnSave").show();
    }
    path = decodeURI(path);
    if (isImage(path)) {
        /*ViewPhotoDoc(data.Path);*/
        viewimgonDocPage(path);
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

$('#headingAssetsDefault').click(function () {

    accordiantabName = "CES";
    getDocuments(false);
});

$('#headingReference').click(function () {

    accordiantabName = "STC";
    getDocuments(false);
});

$('#headingAssets1').click(function () {

    accordiantabName = "OTHER";
    getDocuments(false);
});

function getDocuments(isUpload) {

    $.get(urlGetDocumentsListAll + "?id=" + JobId + "&distributorID=" + DistributorId + "&stage=stc&jobTypeId=1", "", function (response) {
        LoadDocuments(response, isUpload);
    });
}

function htmlDecode(input) {

    var doc = new DOMParser().parseFromString(input, "text/html");
    return doc.documentElement.textContent;
}

$('#ddlviewdocumentsName').change(function () {
    var obj = $(this).find(':selected').attr('data-id');
    var JobDocumentId = $('#ddlviewdocumentsName').val();
    var path = $(this).find(':selected').attr('data-path');
    viewDocumentonPage("", "", "", path, 0, obj, JobDocumentId)
});

function ViewPreviousNextDoc(stage, state, provName, path, id, obj, jobDocumentId) {    
    var docPath = "";
    if ($('#ddlviewdocumentsName').find(':selected').html() != 'Select') {
        docPath = $('#ddlviewdocumentsName').find(':selected').attr('data-path');
    }
    viewDocumentonPage(stage, "", "", docPath, 0, obj, jobDocumentId);
}

function LoadDocuments(response, isUpload, Flag) {

    if (response) {
        var responseData = htmlDecode(response),
            //newdata = newdata1.replaceAll("\\", "\\\\"),
            data = JSON.parse(responseData),
            cesData = data.Table,
            stcData = data.Table1,
            CESList = cesData.filter(e => e.Type === "CES"),
            STCList = cesData.filter(e => e.Type === "STC"),
            OtherList = cesData.filter(e => e.Type === "OTHER"),
            $tbodyCES = $('#tbodyCES'),
            $tbodySTC = $('#tbodySTC'),
            $tbodyOther = $('#tbodyOther');



        if (accordiantabName == "CES") {

            setFiles($('#tbodyCES'), CESList, 'CES');
            //$.each(CESList, function (Data, Result) {
            //    

            //    var documentspath = Result.Path.split('\\');
            //    var documentName = documentspath[documentspath.length - 1];
            //    $("#ddlviewdocumentsName").append($("<option></option>").val(Result.JobDocumentId).attr('data-id', 'ces_' + Result.JobDocumentId).attr('data-path', Result.Path).html(documentName));

            //})
            //isCESLoaded = true;            
        }
        else if (accordiantabName == "OTHER") {
            setFiles($tbodyOther, OtherList, 'OTHER');
            //$.each(OtherList, function (Data, Result) {
            

            //    var documentspath = Result.Path.split('\\');
            //    var documentName = documentspath[documentspath.length - 1];
            //    $("#ddlviewdocumentsName").append($("<option></option>").val(Result.JobDocumentId).attr('data-id', 'other_' + Result.JobDocumentId).attr('data-path', Result.Path).html(documentName));

            //})

            //isOtherLoaded = true;
        }
        else if (accordiantabName == "STC") {
            //isSTCLoaded = true;
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
                    var td1 = $('<td class="tdDoc ' + ((d.IsExist && d.IsExist == true) ? '' : '') + '">').attr('style', 'text-align:left!important;word-break: break-all; padding-left:4%');
                    var aViewDocForSTC = $('<a/>')
                        .attr('href', 'javascript:void(0);')
                        .attr('title', 'STC Document')
                        .attr('class', 'document-color')
                        .attr('data-filename', d.Name)
                        .attr('role', 'button')
                        .attr('onclick', 'viewDocumentonPage(' + params + ')')
                        .text(d.Name)
                        .appendTo(td1);

                    var tr = $('<tr id="trDoc_' + i + '" class="job-document-' + d.JobDocumentId + '" >')
                        .append(td1)
                        .append(td);
                    tr.data('data', d);

                    if (imageTypes.indexOf($(tr).find('td').html().toLowerCase().split('.').pop()) > -1) {
                        OpacityIncreaseDecrease($(tr).find('.ganret'), 0);
                    }

                    $tbodySTC.append(tr);

                    if (isUpload) {
                        //showMessage("Document has been created successfully.", false, 'STC');
                        BindddlForSTCUploadCreate(d.JobDocumentId, d.Path, d.Name, i);
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


    }
    assignEvent();
    PopoverEnable();
}
function DownloadGPSLogFiles() {
    window.location.href = urlDownloadGPSLogs + "?jobId=" + JobId;
}

$('#btnGroupDrop2').click(function (e) {
    if (!uploadbtndoc) {
        $('.arrow_doc_up').hide();
        $('.arrow_doc_down').show();
        uploadbtndoc = true;
    }
    else {
        $('.arrow_doc_up').show();
        $('.arrow_doc_down').hide();
        uploadbtndoc = false;
    }
});
$('#btnGroupDrop1').click(function (e) {
    if (!createbtndoc) {
        $('.arrow_doc_up_cr').hide();
        $('.arrow_doc_down_cr').show();
        createbtndoc = true;
    }
    else {
        $('.arrow_doc_up_cr').show();
        $('.arrow_doc_down_cr').hide();
        createbtndoc = false;
    }
});
$('#btnddlUploadCESDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnUploadCESDocument').click();
});
$('#btnddlUploadSTCDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnSTCDocument').click();
});
$('#btnddlUploadOtherDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnUploadOther').click();
});
$('#btnddlCreateCESDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnCreateCESDocument').click();
});
$('#btnddlCreateSTCDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnCreateSTCDocument').click();
});
$('#btnddlCreateOtherDocument').click(function (e) {
    e.preventDefault();
    document.getElementById('btnCreateDoc').click();
});


function ClearHighLight(Currentelement) {
    if (Currentelement == "liDocs") {
        $("#liPhotos").css({ 'font-weight': '' });
        $("#liPhotos > span").css({ 'font-weight': '' });
        $("#liSerials").css({ 'font-weight': '' });
        $("#liSerials > span").css({ 'font-weight': '' });
        $("#liSummary").css({ 'font-weight': '' });
    }
    else if (Currentelement == "liPhotos") {
        $("#liDocs").css({ 'font-weight': '' });
        $("#liDocs > span").css({ 'font-weight': '' });
        $("#liSerials").css({ 'font-weight': '' });
        $("#liSerials > span").css({ 'font-weight': '' });
        $("#liSummary").css({ 'font-weight': '' });
    }
    else if (Currentelement == "liSerials") {
        $("#liDocs").css({ 'font-weight': '' });
        $("#liDocs > span").css({ 'font-weight': '' });
        $("#liPhotos").css({ 'font-weight': '' });
        $("#liPhotos > span").css({ 'font-weight': '' });
        $("#liSummary").css({ 'font-weight': '' });
    }
    else if (Currentelement == "liSummary") {
        $("#liDocs").css({ 'font-weight': '' });
        $("#liDocs > span").css({ 'font-weight': '' });
        $("#liPhotos").css({ 'font-weight': '' });
        $("#liPhotos > span").css({ 'font-weight': '' });
        $("#liSerials").css({ 'font-weight': '' });
        $("#liSerials > span").css({ 'font-weight': '' });
    }
}

function LoadddlDocumentList(response) {
    if (response) {
        var responseData = htmlDecode(response),
            //newdata = newdata1.replaceAll("\\", "\\\\"),
            data = JSON.parse(responseData),
            cesData = data.Table,
            stcData = data.Table1,
            CESList = cesData.filter(e => e.Type === "CES"),
            STCList = cesData.filter(e => e.Type === "STC"),
            OtherList = cesData.filter(e => e.Type === "OTHER");
        if (!isCESLoaded) {
            $.each(CESList, function (Data, Result) {

                var documentspath = Result.Path.split('\\');
                var documentName = documentspath[documentspath.length - 1];
                $("#ddlviewdocumentsName").append($("<option></option>").val(Result.JobDocumentId).attr('data-id', 'ces_' + Result.JobDocumentId).attr('data-path', Result.Path).html(documentName));

            })
            isCESLoaded = true;
        }

        if (!isOtherLoaded) {
            $.each(OtherList, function (Data, Result) {

                var documentspath = Result.Path.split('\\');
                var documentName = documentspath[documentspath.length - 1];
                $("#ddlviewdocumentsName").append($("<option></option>").val(Result.JobDocumentId).attr('data-id', 'other_' + Result.JobDocumentId).attr('data-path', Result.Path).html(documentName));

            })

            isOtherLoaded = true;
        }

        if (!isSTCLoaded) {
            $.each(STCList, function (Data, Result) {

                var documentspath = Result.Path.split('\\');
                var documentName = documentspath[documentspath.length - 1];
                $("#ddlviewdocumentsName").append($("<option></option>").val(Result.JobDocumentId).attr('data-id', 'trDoc_' + Data).attr('data-path', Result.Path).html(documentName));

            })
            isSTCLoaded = true;
        }


    }
}

function CallddlListOnTabLoad() {
    $.get(urlGetDocumentsddlListAll + "?id=" + JobId + "&distributorID=" + DistributorId + "&stage=stc&jobTypeId=1", "", function (response) {
        LoadddlDocumentList(response);
    });
}


function BindddlForSTCUploadCreate(JobDocumentId, Path, name, index) {
    $("#ddlviewdocumentsName").append($("<option></option>").val(JobDocumentId).attr('data-id', 'trDoc_' + index).attr('data-path', Path).html(name));
}