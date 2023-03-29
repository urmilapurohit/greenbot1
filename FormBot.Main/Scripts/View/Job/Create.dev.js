


$(document).on('ajaxStart', function () {
    count++;
    if (count === 1) {
        $("#loading-image").show();
    }
}).on('ajaxComplete', function () {
    //count is decrementing and on same time checking whether it becomes zero
    if (!--count) {
        $("#loading-image").hide();
    }
});

$('#loading-image').show();
$('#divSchedulingEdit').on('load', function () {
    $('#loading-image').delay(9000).fadeOut(); // Time in milliseconds.
});

var SRCSign;
var isSendMessageRights = isSendMessage;
//var jobIDForScheduling = RouteId;
var isEmailAlreadyLoaded = false;
var pageIndex = 1;
var pageSize = 10;
var IsLast = false;
var count = 0;
var isParentInvoice = false;
function onclickEvent() {
    
    var notes = $("#Notes").val();
    var jobID = $("#BasicDetails_JobID").val();
    var title = $("#hdnJobTitle").val();
    var obj = {};
    obj.JobID = jobID;
    obj.Notes = notes;
    obj.JobTitle = title;
    var dataNotes = JSON.stringify(obj);
    var parameters = $('#frmIndex').serialize();
    if (notes == "") {
        $(".alert").hide();
        $("#errorMsgRegion").addClass("alert-danger");
        $("#errorMsgRegion").html(closeButton + "Please enter job notes.");
        $("#errorMsgRegion").show();

    }
    else {
        $.ajax({
            url: urlSaveJobNotes,
            type: "post",
            //data: { notes: notes, jobID: jobID, title: title },
            data: dataNotes,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $(".alert").hide();
                    $("#popuperrorMsgRegion").removeClass("alert-danger");
                    $("#popuperrorMsgRegion").addClass("alert-success");
                    $("#popuperrorMsgRegion").html(closeButton + "Job notes has been saved successfully.");
                    $("#popuperrorMsgRegion").show();

                    $("#Notes").val("");
                    $("#Notes").focus();
                    $.ajax({
                        type: 'POST',
                        url: urlJobList,
                        data: parameters + "&pageIndex=" + 1 + "&jobID=" + jobID,
                        dataType: 'json',
                        async: false,
                        success: function (response) {
                            $("#tblCase tr").remove();
                            var trHTML = '';
                            if (response.PagedList != null) {
                                $.each(response.PagedList, function (i, item) {
                                    if (isDelete == 'false') {
                                        trHTML +=
                                               '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                                               '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                               '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                                '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                                        '</tr>';
                                    }
                                    else {
                                        trHTML +=
                                               '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                                               '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                               '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                                '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                                        '</tr>';
                                    }
                                });
                            }
                            $('#tblCase').append(trHTML);
                            $(".color:odd").css("background-color", "#fff");
                            $(".color1:odd").css("background-color", "#fff");
                        },
                        error: function (req, status, error) {
                            alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
                        }
                    });
                }
                else {
                    $(".alert").hide();
                    $("#popuperrorMsgRegion").removeClass("alert-success");
                    $("#popuperrorMsgRegion").addClass("alert-danger");
                    //$("#popuperrorMsgRegion").html(closeButton + "The field cannot contain symbols like < > ^ [ ].");
                    $("#popuperrorMsgRegion").html(closeButton + "Job note has not been saved.");
                    $("#popuperrorMsgRegion").show();

                }
            },
        });
    }
}

function scrolldvIndividual(obj) {
    $("#dvIndividual").on("scroll", function (e) {
        $('#tblHeaderScroll').scrollLeft($(tblScroll).scrollLeft());
        var $o = $(e.currentTarget);
        if ($o[0].scrollHeight - $o.scrollTop() <= $o.outerHeight()) {
            GetRecords();
        }
    });

}

function GetRecords() {
    if (!IsLast) {
        pageIndex++;
        var parameters = $('#frmIndex').serialize();
        var jobID = $("#BasicDetails_JobID").val();
        $.ajax({
            type: 'POST',
            url: urlJobList,
            data: parameters + "&pageIndex=" + pageIndex + "&jobID=" + jobID,
            dataType: 'json',
            async: false,
            success: OnSuccess,
            error: function (req, status, error) {
                alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
            }
        });
    }
}

function OnSuccess(response) {
    var trHTML = '';
    if (response.PagedList.length > 0) {
        $.each(response.PagedList, function (i, item) {
            if (isDelete == 'false') {
                trHTML +=
                       '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                       '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                       '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                        '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                '</tr>';
            }
            else {
                trHTML +=
                       '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                       '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                       '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                        '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                '</tr>';
            }
        });
    }
    else {
        IsLast = true;
    }
    $('#tblCase').append(trHTML);
    $(".color:odd").css("background-color", "#fff");
    $(".color1:odd").css("background-color", "#fff");
}

function DeleteJobNotes(jobNotesId) {
    var result = confirm("Are you sure you want to delete this note?");
    var parameters = $('#frmIndex').serialize();
    var jobID = $("#BasicDetails_JobID").val();
    if (result) {
        $.ajax({
            url: urlDeleteJobNotes,
            type: "POST",
            async: false,
            data: JSON.stringify({ jobNotesId: jobNotesId }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $.ajax({
                        type: 'POST',
                        url: urlJobList,
                        data: parameters + "&pageIndex=" + 1 + "&jobID=" + jobID,
                        dataType: 'json',
                        async: false,
                        success: function (response) {
                            $("#tblCase tr").remove();
                            var trHTML = '';
                            if (response.PagedList != null) {
                                $.each(response.PagedList, function (i, item) {
                                    if (isDelete == 'false') {
                                        trHTML +=
                                               '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td></tr>' +
                                               '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                               '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                                '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                                        '</tr>';
                                    }
                                    else {
                                        trHTML +=
                                               '<tr class="notesTR color">' + '<td colspan="2" class="notes">' + item.Notes + '</td>' + '<td style="text-align:center;width:34px;"><a href="javascript:DeleteJobNotes(' + item.JobNotesID + ')" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;" title="Delete">&nbsp; &nbsp; &nbsp; &nbsp;</a></td>' + '</tr>' +
                                               '<tr class="createdTR color1">' + '<td class="createdBy">' + '<b style="color:#555;">Created By : </b>' + item.Created + '</td>' +
                                               '<td class="createdDate">' + '<b style="color:#555;">Created Date : </b>' + item.strCreatedDate + '</td>' +
                                                '<td>' + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + '</td>' + 
                                        '</tr>';
                                    }
                                });
                            }
                            $('#tblCase').append(trHTML);
                            setTimeout(function () {
                                if ($('#tblCase tr').length == 0) {
                                    var trHTML = '';
                                    trHTML +=
                                                  '<tr style="border:none;padding: 10px 20px;height: 34px;text-align:center;">' + '<td colspan="3" style="padding-top:10px;">' + 'No Records Found' + '</td>' + '</tr>';
                                    $('#tblCase').append(trHTML);
                                }
                            }, 200);
                            $(".color:odd").css("background-color", "#fff");
                            $(".color1:odd").css("background-color", "#fff");
                        },
                        error: function (req, status, error) {
                            alert("Could not get patient name properly. Status = " + status + " Error=" + error + " Req=" + req);
                        }
                    });
                    $(".alert").hide();
                    $("#successMsgRegion").removeClass("alert-danger");
                    $("#successMsgRegion").addClass("alert-success");
                    $("#successMsgRegion").html(closeButton + "Job notes deleted successfully.");
                    $("#successMsgRegion").show();

                }
            },
        });
    }
}

// $(function () {
//     'use strict';


$("#loading-image").show();
$('#divSchedulingEdit').load(urlJobSchedulingDetail + "?id=" + ModelId, function (response, status, xhr) {
    if (status == "error") {
        window.location.href = urlLogin;
    }
});
$("#loading-image").hide();
$(document).ready(function () {
    $('#uploadBtnJobSignature').fileupload({

        url: urlUpload,
        dataType: 'json',
        done: function (e, data) {
            var UploadFailedFiles = [];
            UploadFailedFiles.length = 0;

            for (var i = 0; i < data.result.length; i++) {
                if (data.result[i].Status == true) {

                    var OldEleSign = Electricians_Signature;
                    var guid = Model_Guid;
                    var signName = $('#imgSign').attr('class');
                    $("[name='Signature']").each(function () {
                        $(this).remove();
                    });

                    if (signName != null && signName != "") {
                        DeleteFileFromFolderAndElectrician(signName, guid, OldEleSign);
                    }
                    var proofDocumentURL = UploadedDocumentPath;
                    var imagePath = proofDocumentURL + "JobDocuments" + "/" + guid;
                    var SRC = imagePath + "/" + data.result[i].FileName.replace("%", "$");

                    SRCSign = SRC;
                    //$('#imgSign').attr('src', SRC);
                    $('#imgSign').attr('class', data.result[i].FileName.replace("%", "$"));

                    $('#imgSignature').show();


                    $('<input type="hidden">').attr({
                        name: 'Signature',
                        value: data.result[i].FileName.replace("%", "$"),
                        id: data.result[i].FileName.replace("%", "$"),
                    }).appendTo('form');

                }
                else {
                    UploadFailedFiles.push(data.result[i].FileName.replace("%", "$"));
                }
            }
            if (UploadFailedFiles.length > 0) {
                $(".alert").hide();
                $("#successMsgRegion").hide();
                $("#errorMsgRegion").html(closeButton + "File has not been uploaded.");
                $("#errorMsgRegion").show();

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
            var documentType = data.files[0].type.split("/");
            var mimeType = documentType[0];
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
                } else if (CheckSpecialCharInFileName(data.files[0].name)) {
                    ShowErrorMsgForFileName("Please upload file that not conatain <> ^ [] .")
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
                value: Model_Guid,
                id: Model_Guid,
            }).appendTo('form');
            return true;

        },
        formData: { userId: Model_Guid }
    });
});

//generateInvoice


$("ul.create-job-menu").on("click", "li", function () {
    $('ul.create-job-menu').each(function () {
        $(this).find('li').each(function () {
            $(this).removeClass("active");
        });
    });

    $("#invoiceDetail").css("display", "none");


    if ($("#BasicDetails_JobID").val() == "0") {
        $("#ActiveJob").addClass("active");
    } else {
        //$(this).addClass("active");
        if ($("#hasJobID").val() == "0") {
            $("#hasJobID").val($("#BasicDetails_JobID").val());
        }
        LoadTabContent($(this).index());
    }

    //$("#ActiveJob").get(prevSelectedTab).addClass("active");
    prevSelectedTab = $(this).index();
    return false;
});


// });

function LoadTabContent(index) {
    $('ul.create-job-menu').each(function () {
        $(this).find('li').each(function () {
            $(this).removeClass("active");
        });
    });
    if (index == 0) {
        $("#ActiveJob").addClass("active");
        //$("ul.create-job-menu li").get(index);
        $("#jobTitle").html("Job Details");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        $(document).attr('title', 'GreenBot - ' + 'Job Details');
        $("#invoice").hide();
        $("#history").hide();
        $("#createJobNotes").hide();
        $("#createJobView").show();
        $(".assign").show();
        $("#EmailConversation").hide();
        $("#createJobMessage").hide();
        IsLast = false;
        pageIndex = 1;
        isParentInvoice = false;
    } else if (index == 1) {
        $("#aInvoice").addClass("active");
        $("#jobTitle").html("Invoice");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        $(document).attr('title', 'GreenBot - ' + 'Invoice');
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();
        $("#history").hide();
        $(".assign").hide();
        IsLast = false;
        pageIndex = 1;
        $('#invoice').load(urlGetJobInvoice, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {
                $('#invoice').show();
                $("#EmailConversation").hide();
                $("#invoiceDetail").css("display", "none");
                loadInvoice();
                //setTimeout(function(e){ loadInvoice() },1500);
            }
            if (statusTxt == "error")
                alert("Error: " + xhr.status + ": " + xhr.statusText);
        });
    } else if (index == 2) {
        
        $("#aNotes").addClass("active");
        $("#jobTitle").html("Notes");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        if (isNotesView == 'True') {
            $("#jobTitle").html("Notes");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'Notes');
            $("#createJobView").hide();
            $("#createJobNotes").show();
            
            $.ajax({
                type: "GET",
                //url: urlGetJobNotes + RouteId,
                url: urlGetJobNotes + QueryId,
                cache: false,
                async: true,
                ifModified: true,
                success: function (Data) {
                    $("#createJobNotes").html(Data);
                },
                error: function (Error) {
                    //console.log(Error);
                }
            });

            $("#createJobMessage").hide();
            $("#invoice").hide();
            $("#history").hide();
            $("#EmailConversation").hide();
            $(".assign").hide();
            IsLast = false;
            pageIndex = 1;
            setTimeout(function () {
                $("#Notes").focus();
                $(".color:odd").css("background-color", "#fff");
                $(".color1:odd").css("background-color", "#fff");
                $('#hdnJobTitle').val(BasicDetails_Title);
                if ($('#tblCase tr').length == 0) {
                    var trHTML = '';
                    trHTML +=
                                  '<tr style="border:none;padding: 10px 20px;height: 34px;text-align:center;">' + '<td colspan="3" style="padding-top:10px;">' + 'No Records Found' + '</td>' + '</tr>';
                    $('#tblCase').append(trHTML);
                }
            }, 2000);
        } else {
            $("#ActiveJob").addClass("active");
        }
        isParentInvoice = false;
    } else if (index == 3) {
        if (isHistoryView == 'True') {
            $("#aHistory").addClass("active");
            $("#jobTitle").html("History");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'History');
            $("#createJobView").hide();
            $("#createJobNotes").hide();
            $("#createJobMessage").hide();
            $("#EmailConversation").hide();
            $("#invoice").hide();
            $("#history").show();
            $('#history').load(urlShowHistory + RouteId);
            $(".assign").hide();
            IsLast = false;
            pageIndex = 1;
        } else { $("#ActiveJob").addClass("active"); }
        isParentInvoice = false;
    } else if (index == 4) {
        if (IsUserEmailAccountConfigured == 'False') {
            EmailAccountConfigureErrorMessage();
            LoadTabContent(prevSelectedTab);
            //return false;
        }
        else {
            if (isMessageView == 'True') {
                $("#aMessage").addClass("active");
                $("#jobTitle").html("Message");
                $("#jobTitle1").html("<span>" + Model_Header + "</span>");
                $(document).attr('title', 'GreenBot - ' + 'Message');
                $("#createJobView").hide();
                $("#createJobNotes").hide();
                $("#createJobMessage").load(urlLoadMessageTab + $("#BasicDetails_JobID").val());
                $("#createJobMessage").show();
                $("#invoice").hide();
                $("#history").hide();
                $(".assign").hide();
                $("#EmailConversation").hide();
                IsLast = false;
                pageIndex = 1;
            } else { $("#ActiveJob").addClass("active"); }
            isParentInvoice = false;
        }
    }
    else if (index == 5) {
        if (IsUserEmailAccountConfigured == 'False') {
            EmailAccountConfigureErrorMessage();
            LoadTabContent(prevSelectedTab);
            //return false;
        }
        else {
            $("#aEmail").addClass("active");
            $("#jobTitle").html("Email");
            $("#jobTitle1").html("<span>" + Model_Header + "</span>");
            $(document).attr('title', 'GreenBot - ' + 'Email');
            $("#createJobView").hide();
            $("#createJobNotes").hide();
            $("#createJobMessage").hide();
            $("#EmailConversation").load(urlGetMessageForJobEmailConversationForPreAndConn + $("#BasicDetails_JobID").val());
            $("#EmailConversation").show();
            $("#invoice").hide();
            $("#history").hide();
            $(".assign").hide();
            isParentInvoice = false;
        }
    }
}


var jobInvoiceDetail = jobInvoiceDetail || [];

jobInvoiceDetail = (function () {
    var pub = {};

    pub.loadInvoiceDetailData = function (invoiceID) {
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();

        $("#history").hide();
        $(".assign").hide();
        if ($("#invoice"))
            $("#invoice").css("display", "none");
        if ($("#invoiceDetailList")) {
            $('#invoiceDetailList').html('');
            var jobID = 0;
            if ($("#hasJobID"))
                jobID = $("#hasJobID").val();

            $.ajax({
                url: urlGetJobInvoiceDetail,
                type: "GET",
                dataType: "html",
                data: { invoiceID: invoiceID, jobID: jobID },
                async: true,
                success: function (data) {
                    $('#invoiceDetailList').html(data);
                },
                complete: function (data) {
                    $("#invoiceDetail").css("display", "block");
                    if ($('#invoiceDetailListTable')) {
                        var table = $('#invoiceDetailListTable').DataTable();
                        table.destroy();
                    }
                    pub.loadtable(invoiceID);
                    if ($("#hdnJobInvoiceID")) {
                        $("#hdnJobInvoiceID").val(invoiceID);
                    }
                    $('body').attr('style', 'overflow:scroll;padding-right:0px;');
                },
                error: function (data) {
                    alert(data)
                }
            });
            isParentInvoice = true;
        }
    }

    pub.loadtable = function (invoiceID) {


        var isInvoiced = '', invoiceType = '', queryString = '';

        if ($('#drdInvoiced')) {
            isInvoiced = $('#drdInvoiced').val();
        }
        if ($('#drdInvoiceType')) {
            invoiceType = $('#drdInvoiceType').val();
        }

        if (invoiceID != '')
            queryString += 'invoiceID=' + invoiceID + '&';

        if (isInvoiced != '')
            queryString += 'isInvoiced=' + isInvoiced + '&';

        if (invoiceType != '')
            queryString += 'jobInvoiceType=' + invoiceType;

        if (queryString != '')
            queryString = '?' + queryString;

        var chkCount = 0;

        var jobID = '';
        if ($("#hasJobID"))
            jobID = $("#hasJobID").val();


        $('#invoiceDetailListTable').DataTable({

            paging: false,
            //scrollY: 200,
            //scrollCollapse: true,
            language: {
                sLengthMenu: "Page Size: _MENU_"
            },
            columns: [

                    {

                        'data': 'JobInvoiceDetailID',
                        "render": function (data, type, full, meta) {
                            //console.log("JobInvoiceDetailID = "+full.JobInvoiceDetailID);
                            if ($('#chkAll').prop('checked') == true) {
                                return '<input type="checkbox" id="chk_' + full.JobInvoiceDetailID + '" checked>';
                            }
                            else {
                                return '<input type="checkbox" id="chk_' + full.JobInvoiceDetailID + '">';
                            }
                        },
                        "orderable": false
                    },
                    {
                        'data': 'TimeStart',
                        "render": function (data) {
                            //return this.ConvertToDate(data);
                            return ConvertToDateWithFormat(data, GetDateFormat)
                        }
                    },
                    {
                        'data': 'JobInvoiceType',
                        "render": function (data, type, full, meta) {
                            if (full.JobInvoiceType == 1) {
                                return '<img src="/images/invoice-type-part.png" alt="Part" />'
                            }
                            else if (full.JobInvoiceType == 2) {
                                return '<img src="/images/invoice-type-time.png" alt="Time" />'
                            }
                            else if (full.JobInvoiceType == 3) {
                                return '<img src="/images/invoice-type-payment.png" alt="Payment" />'
                            }
                            else {
                                return '';
                            }
                        }, "orderable": false
                    },
                    { 'data': 'Description' },
                    { 'data': 'StaffName', "orderable": false },
                    //{ 'data': 'JobScheduleLabel' },
                    {
                        'data': 'Quantity', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.Quantity);
                        },
                    },
                    {
                        'data': 'Sale', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.Sale);
                        },
                    },
                    {
                        'data': 'SubTotal', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.SubTotal);
                        },
                    },
                    {
                        'data': 'cost', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.cost);
                        },
                    },
                    {
                        'data': 'Profit', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.Profit);
                        },
                    },
                    {
                        'data': 'Margin', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            if (typeof (full.Margin) == 'number') {
                                if (full.Margin === parseInt(full.Margin, 10)) {
                                    return full.Margin;
                                } else { return full.Margin.toFixed(0); }
                            }
                            else {
                                return '0';
                            }
                        },
                    },
                    {
                        'data': 'Tax', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.Tax);
                        },
                    },
                    {
                        'data': 'Total', "orderable": false, "sClass": "dt-right",
                        "render": function (data, type, full, meta) {
                            return PrintDecimal(full.Total);
                        },
                    },
                    {
                        'data': 'IsBillable',
                        "render": function (data, type, full, meta) {
                            return '<img src="/images/' + full.IsBillableImage + '" alt="" />'
                        }, "orderable": false
                    },
                    {
                        'data': 'IsInvoiced',
                        "render": function (data, type, full, meta) {

                            if (full.IsInvoiced == true) {
                                return 'Yes'
                            }
                            else if (full.IsInvoiced == false) {
                                return 'No'
                            }
                            else {
                                return '';
                            }
                        }, "orderable": false
                    },
                    {
                        'data': 'JobInvoiceDetailID',
                        className: 'hide_column',
                    },
                    {
                        'data': 'JobInvoiceType',
                        className: 'hide_column',
                    },
                    {
                        'data': 'Payments',
                        className: 'hide_column',
                    },
                    {
                        'data': 'Remaning',
                        className: 'hide_column',
                    },
                    {
                        'data': 'TaxAmountConsider',
                        className: 'hide_column',
                    }
            ],
            dom: "<<'table-responsive tbfix't><'bottom'l><'clear'>>",
            bServerSide: true,
            sAjaxSource: urlGetInvoiceDetailListing + queryString + "&&jobID=" + jobID,

            "fnDrawCallback": function (row, data, start, end, display) {

                chkCount = 0;
                var api = this.api(), data;
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                        i : 0;
                };
                //$("#generateInvoice").attr("href", urlCreateReport+ invoiceID + '&jobID=' + jobID)
                $("#generateInvoice").attr("data-id", urlCreateReport + invoiceID + '&jobID=' + jobID)
                $("#generateInvoice").attr("style", "cursor:pointer;");
                $("#exportCSV").attr("href", urlCreateCSV + invoiceID);
               

                $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
                $(".paging a.previous").html("&nbsp");
                $(".paging a.next").html("&nbsp");
                $('.grid-bottom span:first').attr('id', 'spanMain');
                $("#spanMain span").html("");
                $(".ellipsis").html("...");

                if ($(".paging").find("span").length > 1) {
                    $("#invoiceDetailListTable_length").show();
                } else if ($("#invoiceDetailListTable").find("tr").length > 11) { $("#invoiceDetailListTable_length").show(); } else { $("#invoiceDetailListTable_length").hide(); }

                //--------To show display records from total records-------------------
                var table = $('#invoiceDetailListTable').DataTable();
                var info = table.page.info();

                //if($("invoceDetailSummary") && $("#invoiceDetailListTable").dataTable().fnSettings().aoData.length > 0)
                //{
                //   // $('#invoiceDetailListTable tr:last').after($('#invoceDetailSummary tbody').html());
                //}

                $('#invoiceDetailListTable th:first').removeClass('sorting_asc');

                $('#invoiceDetailListTable tbody').on('click', 'tr td:not(:first-child)', function (e) {

                    var jobInvoiceDetailID = $(e.currentTarget).closest('tr').find('td:eq(15)').html();
                    var jobInvoiceDetailType = $(e.currentTarget).closest('tr').find('td:eq(16)').html();

                    var jobID = '';
                    if ($("#hasJobID"))
                        jobID = $("#hasJobID").val();

                    var xeroApprovedDate = '';
                    if ($("#hdnXeroApprovedDate"))
                        xeroApprovedDate = $("#hdnXeroApprovedDate").val();


                    if (jobInvoiceDetailType == '1' && jobInvoiceDetailID != '' && jobID != ''
                        && xeroApprovedDate == '') {
                        GetJobInvoiceDetailById(jobInvoiceDetailID, jobID);
                    }
                    else if (jobInvoiceDetailType == '2' && jobInvoiceDetailID != '' && xeroApprovedDate == '') {
                        loadAddTime(jobInvoiceDetailID)
                    }
                    else if (jobInvoiceDetailType == '3' && jobInvoiceDetailID != '') {
                        PaymentPopup(jobInvoiceDetailID)
                    }
                });
            },
            "fnServerParams": function (aoData) {
                var table = $('#invoiceDetailListTable').DataTable();

                $('#chkAll').on('click', function () {
                    var rows = table.rows({ 'search': 'applied' }).nodes();
                    $('input[type="checkbox"]', rows).prop('checked', this.checked);
                    chkCount = this.checked ? $('#invoiceDetailListTable >tbody >tr').length : 0;
                });

                $('#invoiceDetailListTable tbody').on('change', 'input[type="checkbox"]', function () {

                    if (this.checked) {
                        chkCount++;
                        if (chkCount == $('#invoiceDetailListTable >tbody >tr').length) {
                            $('#chkAll').prop('checked', this.checked)
                        }
                    }
                    else {
                        chkCount--;
                        $('#chkAll').prop('checked', this.checked)
                    }
                });

                //setTimeout(function(){ console.log("123"); $("btnInvoiceAction").attr("aria-expanded","true");},4000);
            },
            initComplete: function (settings, json) {
            },
        });


    }
    //return pub;
    return {
        loadData: function (invoiceID) { pub.loadInvoiceDetailData(invoiceID); },
        loadInvoiceDetailGrid: function (invoiceID) { pub.loadtable(invoiceID) }
    };

})();

function deleteInvoice(invoiceID) {
    var result = confirm("Are you sure you want to delete this invoice ?");
    if (result) {
        $.ajax({
            url: urlDeleteInvoice,
            type: "POST",
            async: false,
            data: JSON.stringify({ InvoiceID: invoiceID }),
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
                    //$("#datatable").dataTable().fnDraw();
                }
                $("#datatable").dataTable().fnDraw();
            },
        });
    }
}


function DownloadReport(obj) {
    var JobId = $("#hasJobID").val();
    var fileName = $(obj).attr('filename');
    window.location.href = urlViewDownloadInvoiceReportFile + fileName + '&JobID=' + JobId;

}

$(document).on("click", "#AddInvoice", function () {
    jobInvoiceDetail.loadData('0');
})

function loadInvoice() {
    //$("#AddInvoice").click(function(){

    //});
    isParentInvoice = false;
    var LogInID = LoggedInUserId;

    if ($('#datatable')) {

        var table = $('#datatable').DataTable();
        table.destroy();
    }
    $('#datatable').DataTable({
        iDisplayLength: 10,
        lengthMenu: GetPageSize,
        order: [[1, "asc"]],
        //sorting:true,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
                 {
                     'data': 'JobInvoiceID',
                     "orderable": false,
                     "render": function (data, type, full, meta) {
                         if (LogInID == full.CreatedBy) {
                             return '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-outgoing.png) no-repeat center;cursor:default; text-decoration:none;margin-left:-30%;"></a>';
                         } else { return '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-incoming.png) no-repeat center;cursor:default; text-decoration:none;margin-left:-30%;"></a>'; }
                     },
                 },
                { 'data': 'InvoiceNumber', "orderable": true },

                {
                    'data': 'CreatedDate',
                    "render": function (data) {
                        return ConvertToDateWithFormat(data, GetDateFormat)
                    }
                },
                { 'data': 'Created' },
                { 'data': 'InvoicedTo' },
                {
                    'data': 'FileName',
                    "render": function (data, type, full, meta) {
                        if (full.Status == 1) {
                            return "<p style='color:green;'>" + full.FileName + '</p>';
                        } else if (full.Status == 2) {
                            return "<p style='color:gray;'>" + full.FileName + '</p>';
                        } else if (full.Status == 3) {
                            return "<p style='color:orange;'>" + full.FileName + '</p>';
                        } else if (full.Status == 4) {
                            return "<p style='color:red;'>" + full.FileName + '</p>';
                        } else { return "<p style='color:red;'></p>"; }

                    }, "orderable": false
                },
                {
                    'data': 'InvoiceTotal',
                    "orderable": true,
                    "sClass": "dt-right",
                    'render': function (data, type, full, meta) {
                        return PrintDecimal(full.InvoiceTotal);
                    },
                },
                {
                    'data': 'InvoiceAmountDue',
                    "orderable": true,
                    "sClass": "dt-right",
                    'render': function (data, type, full, meta) {
                        return PrintDecimal(full.InvoiceAmountDue);
                    },
                },
                {
                    'data': 'JobInvoiceID',
                    "render": function (data, type, full, meta) {
                        var pdf = '';
                        if (full.FileExist == '1') {
                            pdf = '<a class="action view" href="javascript:void(0)" style="background:url(../../Images/pdf.png) no-repeat center; text-decoration:none;margin-left:18px;" onclick="DownloadReport(this)" filename="' + full.InvoiceNumber + '.pdf"></a>';
                        }
                        else if (full.FileExist == '0') {
                            pdf = '<a class="action view" style="background:none;text-decoration:none;margin-left:18px;"></a>'
                        }
                        var sent = '';
                        if (full.Sent == 1) {
                            sent = '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-right.png) no-repeat center; cursor:default;text-decoration:none;margin-left:18px;"></a>';
                        } else { sent = '<a href="javascript:void(0)" class="action view" style="background:url(../../Images/ic-wrong.png) no-repeat center; cursor:default;text-decoration:none;margin-left:18px;"></a>'; }

                        var edit = '&nbsp;<a href="javascript:void(0);" class="action view" style="background:url(../../Images/edit-icon.png) no-repeat center; text-decoration:none;margin-left:18px;" onclick="jobInvoiceDetail.loadData(' + full.JobInvoiceID + ')"></a>';
                        var space = '&nbsp;&nbsp;&nbsp;';
                        return pdf + space + sent + edit + '<a href="javascript:void(0)" onclick="deleteInvoice(' + full.JobInvoiceID + ')" class="action view" style="background:url(../../Images/delete-icon.png) no-repeat center; text-decoration:none;margin-left:18px;"></a>';
                    }, "orderable": false
                },
        ],
        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        bServerSide: true,
        sAjaxSource: urlGetInvoiceList + $("#BasicDetails_JobID").val(),
        "fnDrawCallback": function () {
            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length > 1) {
                $("#datatable_length").show();
            } else if ($("#datatable").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            var info = table.page.info();

            if (info.recordsTotal == 0) {
                document.getElementById("invoiceNumRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b>  Invoice(s) found';
            }
            else {
                document.getElementById("invoiceNumRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b>  Invoice(s) found';
            }
            //------------------------------------------------------------------------------------------------------------------------------
        }
    });

    $(".dataTables_empty").css("text-align", "left");

}

var modelHeading = "";
initSample();

$(".date-pick").keydown(function (e) {
    if (e.which == 9) {
        return true;
    }
    return false;
});

ddaccordion.init({
    headerclass: "expandable",
    contentclass: "sidebar-list",
    revealtype: "click",
    collapseprev: false,
    defaultexpanded: [],
    onemustopen: false,
    animatedefault: false,
    persiststate: false,
    toggleclass: ["", "openheader"],
    togglehtml: ["prefix", "", ""],
    animatespeed: "normal",
    oninit: function (headers, expandedindices) {
    },
    onopenclose: function (header, index, state, isuseractivated) {
    }
});

function LoadPreApprovalOrConnection(PreApprovalsOrConnectionsId) {
    $("#PreApprovalConnOrStcClicked").val(PreApprovalsOrConnectionsId);
    $("#preapprovalsAndConnectionContent").empty();
    var distributor = $('#JobInstallationDetails_DistributorID').val();
    $.ajax({
        url: urlApplyForPreApprovalAndConnection,
        type: "GET",
        data: { "jobID": Model_JobId, preApprOrConne: PreApprovalsOrConnectionsId, distributorID: distributor },
        cache: false,
        async: true,
        success: function (Data) {
            $("#preapprovalsAndConnectionmodal").modal({ backdrop: 'static', keyboard: false });
            $("#preapprovalsAndConnectionContent").html(Data);
        }
    });
}

//  $(document).ready(function (e) {

if (Model_JobId > 0) {
    $('<input type="hidden">').attr({
        name: 'Signature',
        value: Model_Signature,
        id: Model_Signature,
    }).appendTo('form');
}
else
    $("#btnJobPrint").hide();

$("#btnJobPrint").click(function () {
    //var jobID = $("#BasicDetails_JobID").val();
    var jobID = QueryId;
    var url = urlPrint + jobID;
    window.open(url);
});
$('#btnBack').click(function () {
    if (isParentInvoice) {

        $("#jobTitle").html("Invoice");
        $("#jobTitle1").html("<span>" + Model_Header + "</span>");
        $(document).attr('title', 'GreenBot - ' + 'Invoice');
        $("#createJobView").hide();
        $("#createJobNotes").hide();
        $("#createJobMessage").hide();
        $("#history").hide();
        $(".assign").hide();
        IsLast = false;
        pageIndex = 1;
        $('#invoice').load(urlGetJobInvoice);
        $('#invoice').show();
        $("#EmailConversation").hide();
        $("#invoiceDetail").css("display", "none");
        setTimeout(function () { loadInvoice(); }, 1500);
    } else {
        window.location.href = urlIndex;
    }
});
$('#date-pick, #date-pick1, .date-pick').datepicker({

    format: GetDateFormat,
    autoclose: true
}).on('change', function () {
    $(this).valid();
});

modelHeading = Model_Header;

$("#BasicDetails_strSoldByDate").keydown(function (e) {
    if (e.which == 9) {
        return true;
    }
    return false;
});
$('#date-pick, #date-pick1, .date-pick').datepicker({

    format: GetDateFormat,
    autoclose: true
}).on('change', function () {
    $(this).valid();
});


$(".date-pick").keydown(function (e) {
    if (e.which == 9) {
        return true;
    }
    return false;
});


//   });

function LoadStc() {
    $.ajax({
        url: url_STCJobPopup,
        type: "POST",
        data: { jobId: Model_JobId, IsSubmissionScreen: 0 },
        async: false,
        success: function (Data) {
            $("#STcBasicDetails").html(Data);
            $('#StcModal').modal({ backdrop: 'static', keyboard: false });

        }
    });
}