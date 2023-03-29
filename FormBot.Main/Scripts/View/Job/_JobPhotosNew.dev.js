$(document).ready(function () {
    if (IsSPVInstallationVerified == 'True' && (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) && isUploadPhotoAfterTrade=="False") {
        $('.IsSPVinstallationVerified').css("display", "none");
    }
    if (ProjectSession_UserTypeId == 2 || ProjectSession_UserTypeId == 4 || ProjectSession_UserTypeId == 5) {
        if ($('#STCStatusId').val() != 10 && $('#STCStatusId').val() != 12 && $('#STCStatusId').val() != 14 && $('#STCStatusId').val() != 17) {
            $('.isdelete').css("display", "none");
        }
    }
    if (ProjectSession_UserTypeId != 1 && ProjectSession_UserTypeId != 3) {
        $('.AccessOnlyForFSAFCO').css("display", "none");
    }
});

$('#btnYes').click(function () {
    uploadPhotoAfterWarning();
});

function uploadPhotoAfterWarning() {
    var $checkedInput = $('#divVisitList').find('[data-IsDeleted="False"]').find('.parentInput:checked').add($('#headingReference').find('input:checked')).add($('.defaultPanel:checked'));
    var isRef = false;
    isRef = ($('#headingReference').find('input:checked').length > 0) ? true : false;
    isDefault = ($('.defaultPanel:checked').length > 0) ? true : false;
    UploadData.cId = (isRef || isDefault) ? "" : $checkedInput.data('cid');
    UploadData.isRef = isRef;
    UploadData.isDefault = isDefault;
    UploadData.jobScId = (isRef || isDefault) ? "" : $checkedInput.data('jobscid');
    UploadData.Type = isDefault ? $checkedInput.data('type') : "";
    UploadData.PdfLocationId = $checkedInput.data('loc');
    UploadData.ClassTypeId = $checkedInput.data('ct');
    UploadData.PdfName = $checkedInput.data('nm');
    $('#uploadBtnPhoto').data().blueimpFileupload.options.formData = getData();
    document.getElementById('uploadBtnPhoto').click();
    $('#popuponuploadphoto').modal('toggle');
    return false;
}
$("#btnClosepopupboxUploadPhoto").click(function () {
    $('#popuponuploadphoto').modal('toggle');
});

function MakeDefaultFolderAsDefaultSubmission() {
        var defaultSpan = "<span class=submission>STC Submission <i class='sprite-img submission-icon'></i></span>";
        $("#divVisitList").find("#pnlMainDefault").find('.visitParent').prepend(defaultSpan);
}
var logoWidthPhoto, logoHeightPhoto;
var UploadData = {};
function ViewPhoto(src, isDocumentPhoto) {
    src = uploadPath + src;
    logoWidthPhoto = 0;
    logoHeightPhoto = 0;
    $("#loading-image").css("display", "");
    $('#imgSlide').attr('src', src).load(function () {
        logoWidthPhoto = this.width;
        logoHeightPhoto = this.height;
        $('#mdlSlideShow').modal();
        $("#loading-image").css("display", "none");
    });
    $("#mdlSlideShow").unbind("error");
    $('#mdlSlideShow').attr('src', src).error(function () {
        alert('Image does not exist.');
        $("#loading-image").css("display", "none");
    });
    if (isDocumentPhoto) {
        $("#mdlSlideShow").find('#btnPrev').hide();
        $("#mdlSlideShow").find('#btnNext').hide();
    }
    else {
        $("#mdlSlideShow").find('#btnPrev').show();
        $("#mdlSlideShow").find('#btnNext').show();
    }
}

function setPreNextButtons(index, maxLength, src) {
    ViewPhoto(src);
    $('#hdrFileName').text(src.split('\\').slice(src.split('\\').length - 1));
    $('#btnPrev,#btnNext').removeAttr('disabled');
    if (index >= maxLength)
        $('#btnNext').attr('disabled', 'disabled');
    else if (index == 0)
        $('#btnPrev').attr('disabled', 'disabled');
    else
        $('#btnPrev,#btnNext').removeAttr('disabled');

    if (index == 0 && maxLength == 0)
        $('#btnPrev,#btnNext').attr('disabled', 'disabled');
}
var urlPhoto = BaseURL + 'UploadReferencePhoto';
$('#divVisitList .panel-heading input[type=checkbox]').change(function () {
    $(this).closest('.panel').find('[type=checkbox]').prop('checked', this.checked);

    if ($(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]').length == $(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]:checked').length) {
        if ($(this).closest('[id*=pnlMain]').find('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]').length != 0)
            $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').prop('checked', 'checked');
    }
    else {
        $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').removeAttr('checked');
    }
});

$(document).on('click', '.navbar-collapse.in', function (e) {
    if ($(e.target).is('a') && $(e.target).attr('class') != 'dropdown-toggle') {
        $(this).collapse('hide');
    }
});
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
        var ul = UploadData.isRef ? $('#ulPreference') : UploadData.isDefault ? $('#pnlMainDefault').find('.defaultPanel:checked').closest('.panel').find('ul') : $('#divVisitList').find('[data-IsDeleted="False"]').find('.parentInput:checked').closest('.panel').find('ul');
        
        var totalCount = 0;
        for (var i = 0; i < data.result.uploadStatus.length; i++) {
            var path = data.result.uploadStatus[i].Path;
            var withlocationclass = '';
            var IsUnderInstallationArea = '';
            if (data.result.uploadStatus[i].Status == true) {
                if (data.result.uploadStatus[i].latitude > 0 || data.result.uploadStatus[i].longitude > 0) {
                    withlocationclass = 'with-location';
                }
                else {
                    withlocationclass = '';
                }
                if (data.result.uploadStatus[i].isUnderInstallationArea == 'False') {
                    IsUnderInstallationArea = "with-IsUnderInstallationArea";
                }
                else {
                    IsUnderInstallationArea = '';
                }
                var li = $('<li/>')
                    .attr('data-path', path)
                    .attr('data-vclid', data.result.uploadStatus[i].AttachmentID)
                    .appendTo(ul);
                var chk = $('<input/>')
                    .attr('type', 'checkbox')
                    .addClass('visit-list-check')
                    .appendTo(li);
                var aaa = $('<a/>')
                    .addClass(withlocationclass)
                    .addClass(IsUnderInstallationArea)
                    .text(" " + data.result.uploadStatus[i].FileName)
                    .attr('href', uploadPath + "//" + path)
                    .attr('data-lightbox', 'property')
                    .attr('data-lat', data.result.uploadStatus[i].latitude)
                    .attr('data-lon', data.result.uploadStatus[i].longitude)
                    .attr('data-isunderinstallationarea', data.result.uploadStatus[i].isUnderInstallationArea)
                    .attr('data-date', data.result.uploadStatus[i].createdDate)
                    .attr('data-PhotoTypeId', data.result.PhotoTypeId)
                    .attr('title',' ')
                    .appendTo(li);
                var span = $('<span></span>').appendTo(aaa);
                totalCount++;
                SearchHistory();
            }

            if ($('#ulPreference').children('li').length > 1) {
                if ($('#liNoData').length > 0) {
                    $('#liNoData').remove();
                }
            }
            if (data.result.uploadStatus[i].Status == false && data.result.uploadStatus[i].Message == "NotImage") {
                UploadFailedFilesType.push(data.result.uploadStatus[i].FileName);
            }
            else if (data.result.uploadStatus[i].Status == false && data.result.uploadStatus[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result.uploadStatus[i].FileName);
            }
            else if (data.result.uploadStatus[i].Status == false) {
                UploadFailedFiles.push(data.result.uploadStatus[i].FileName);
            }
        }

        ul.closest('.panel').find('.totalCount').text(parseInt(ul.closest('.panel').find('.totalCount').text()) + totalCount);
        if (UploadFailedFilesType.length > 0) {
            showMessageForJobPhoto(UploadFailedFilesType.length + " " + "Uploaded file is not .jpg , .jpeg or .png extension.", true);
        }
        if (UploadFailedFiles.length > 0) {
            showMessageForJobPhoto(UploadFailedFiles.length + " " + "File has not been uploaded.", true);
        }
        if (UploadFailedFilesName.length > 0) {
            showMessageForJobPhoto(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", true);
        }
        if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0 && UploadFailedFilesType.length == 0) {
            showMessageForJobPhoto("File has been uploaded successfully.", false);

            if (data.result.isSPVRequired == true) {
                ShowSPV();
            }
            else {
                HideSPV();
            }
        }
        if ($(ul).closest('.panel').closest('.collapse').closest('.panel').find('.visitParent').find('.submission').length == 1) {
            ReloadSTCModule();
        }
        $("#loading-image").show();
        setTimeout(function () {
            getDocuments(false);
            $("#loading-image").hide();
        }, 500);
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
                    showMessageForJobPhoto("Please upload small filename.", true);
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessageForJobPhoto("Please upload file that not conatain <> ^ [] .",true)
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessageForJobPhoto(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true);
                    return false;
                } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                    showMessageForJobPhoto("Please upload file that not conatain <> ^ [] .", true)
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessageForJobPhoto("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true);
                return false;
            }
        }
        if (data.files.length == 1) {
            var ext = data.files[0].name.toLowerCase().split('.').length > 0 ? data.files[0].name.toLowerCase().split('.')[data.files[0].name.toLowerCase().split('.').length - 1] : '';
            if (mimeType != "image" && ext != "heic") {
                showMessageForJobPhoto("Please upload a file with .jpg , .jpeg or .png extension.", true);
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
    formData: getData(),
    change: function (e, data) {
        $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
    }
}).prop('disabled', !$.support.fileInput)


function getData() {
    return {
        JobId: jobId, UserId: USERID, folder: UploadData.cId, isRef: UploadData.isRef,
        jobScId: UploadData.jobScId, IsDefault: UploadData.isDefault, ClassType: UploadData.Type,
        PdfLocationId: UploadData.PdfLocationId, ClassTypeId: UploadData.ClassTypeId,
        PdfName: UploadData.PdfName, IsAllowAccesstoUploadGPSPhoto: isAllowAccesstoUploadGPSPhoto
    };
}

function downloadAllAsZip(isdwnlAll) {
    var mainPnl = $('[id*=pnlMain]');
    var allData = {};
    var data = [];
    allData.vclid = [];
    allData.vsid = [];

    $.each(mainPnl, function (i, e) {
        var obj = {}, checkListItems = [];
        obj.pl = [];
        var $pnl = $(this).find('[id*=accordion]').children();
        obj.id = $(this).find('.panel-heading').find('a').data('name');
        $.each($pnl, function (i, e) {
            var pnlCheck = {};
            pnlCheck.p = [];
            pnlCheck.s = [];
            pnlCheck.fn = $(this).find('.panel-heading').find('a').data('name');
            $.each(isdwnlAll == 0 ? $(this).find('.panel-body').find('input[type=checkbox]:checked') : $(this).find('.panel-body').find('input[type=checkbox]'), function (i, e) {
                if ($(this).parent().data('vclid')) {
                    pnlCheck.p.push($(this).parent().data('vclid'));
                    allData.vclid.push($(this).parent().data('vclid'));
                }
                if ($(this).parent().data('vsid')) {
                    pnlCheck.s.push($(this).parent().data('vsid'));
                    allData.vsid.push($(this).parent().data('vsid'));
                }
            });

            pnlCheck.s = pnlCheck.s.join(',');
            pnlCheck.p = pnlCheck.p.join(',');

            if ((pnlCheck.p.length || pnlCheck.s.length) > 0)
                obj.pl.push(pnlCheck);
        });
        if (obj.pl.length > 0 && isdwnlAll === 0) {
            data.push(obj);
        }
        else if (isdwnlAll === 1) {
            data.push(obj);
        }
    });

    var refLi = isdwnlAll == 0 ? $('#ulPreference').find('input[type=checkbox]:checked') : $('#ulPreference').find('input[type=checkbox]');
    var defaultPhotos = isdwnlAll == 0 ? $('#ulPreference').find('input[type=checkbox]:checked') : $('#ulPreference').find('input[type=checkbox]');
    var RefPhotos = [];
    if (refLi.length > 0) {
        $.each(refLi, function (i, e) {
            RefPhotos.push($(this).parent().data('vclid'));
        });
    }

    allData.vp = data;
    allData.rp = RefPhotos.join(',');
    allData.vclid = allData.vclid.join(',') + "," + allData.rp;
    allData.vsid = allData.vsid.join(',');
    allData.isDownloadAll = isdwnlAll === 0 ? false : true;
    var refno = $('#BasicDetails_RefNumber').val();
    var flag = true;
    $(".submission-add > .visit-list-check").each(function () {
        if ($(this).is(":checked")) {

        } else {
            flag = false;
        }
    });
    if (isdwnlAll==1) {
        flag = true;
    }
    if (allData.vp.length > 0 || allData.rp.length > 0) {
        showStayMsg = false;
        window.location.href = urlDownloadJobPhotos + "?jobid=" + jobId + "&photos=" + encodeURIComponent(JSON.stringify(allData)) + "&refno=" + refno + "&isall=" + flag;
        SearchHistory();
    }
    else {
        showStayMsg = true;
        alert("Please select photo first.");
    }
    return false;
}
function clpsAll_click() {
    $('.collapse').collapse('hide');
}
function lnkDwnld_click() {    
    downloadAllAsZip(0);
}
function lnkDwnldAll_click() {    
    downloadAllAsZip(1);
}
function uploadBtnPhoto_click() {  
    var $checkedInput = $('#divVisitList').find('[data-IsDeleted="False"]').find('.parentInput:checked').add($('#headingReference').find('input:checked')).add($('.defaultPanel:checked'));
    var isRef = false;
    isRef = ($('#headingReference').find('input:checked').length > 0) ? true : false;
    isDefault = ($('.defaultPanel:checked').length > 0) ? true : false;
    if ($checkedInput.length != 1) {
        alert("Please select one active parent.");
        return false;
    }
    if ($checkedInput.data('ct') == 1 && IsSPVRequired.toLowerCase() == "true") {
        $('#popuponuploadphoto').modal('toggle');
    }
    else {
            UploadData.cId = (isRef || isDefault) ? "" : $checkedInput.data('cid');
            UploadData.isRef = isRef;
            UploadData.isDefault = isDefault;
            UploadData.jobScId = (isRef || isDefault) ? "" : $checkedInput.data('jobscid');
            UploadData.Type = isDefault ? $checkedInput.data('type') : "";
            UploadData.PdfLocationId = $checkedInput.data('loc');
            UploadData.ClassTypeId = $checkedInput.data('ct');
        UploadData.PdfName = $checkedInput.data('nm');
            $('#uploadBtnPhoto').data().blueimpFileupload.options.formData = getData();
            document.getElementById('uploadBtnPhoto').click();
            return false;
    }
}

function lnkGeneratePDF_click(checkbox) {
    $("#loading-image").show();
    var SelectedVisitCount = 0;
    var SelectedDiv = null;
    var VisitChecklistItemIdsVal = "";
    var isAjaxCall = false;
    var JsonData;
    $("div[id^='pnlMain']").not("#pnlMainDefault").each(function (text,value) {
        if ($(value).find(".panel .panel-heading .visit-list-check:checked").length > 0) {
            SelectedVisitCount++;
            SelectedDiv = $(value);
        }
    });
    if (SelectedVisitCount == 1 && !CheckRefAndDefaultPhotoStatus()) {
        $($(SelectedDiv).find(".panel .panel-heading .visit-list-check:checked")).each(function (text, value) {
            if (VisitChecklistItemIdsVal != "")
                VisitChecklistItemIdsVal += "," + $(value).attr("data-cid");
            else
                VisitChecklistItemIdsVal = $(value).attr("data-cid");
        });
        JsonData = {
            VisitChecklistItemIds: VisitChecklistItemIdsVal,
            SchedulingId: $(SelectedDiv).find(".visitParent .visit-list-check").attr("data-jobscid"),
            JobId: JobId,
            UserId: USERID,
            UniqueVisitId: $(SelectedDiv).find(".visitParent .visit-list-check").attr("data-uniquevisistid")
        }
        isAjaxCall = true;
    }
    else if (SelectedVisitCount > 1 && !CheckRefAndDefaultPhotoStatus())
        alert("Please select only one folder details.");
    else if (SelectedVisitCount == 0 && !CheckRefAndDefaultPhotoStatus())
        alert("Please select one folder.");
    else if (SelectedVisitCount == 0 && CheckRefAndDefaultPhotoStatus()) {
        if ($(".pdf-generated-get-details.ref:checked").length > 0) {
            JsonData = {
                VisitChecklistItemIds: 0,
                SchedulingId: 0,
                JobId: JobId,
                UserId: USERID,
                UniqueVisitId: $(".pdf-generated-get-details.ref").attr("data-uniquevisistid"),
                IsReference: true
            };
            isAjaxCall = true;
        }
        else if ($("#collapseAssetsInstallation").find(".panel .panel-heading .visit-list-check:checked").length > 0) {
            var ClassTypeVal = "";
            $($("#collapseAssetsInstallation").find(".panel .panel-heading .visit-list-check:checked")).each(function (text, value) {
                if (ClassTypeVal != "")
                    ClassTypeVal += "," + $(value).attr("data-type");
                else
                    ClassTypeVal = $(value).attr("data-type");
            });
            JsonData = {
                VisitChecklistItemIds: 0,
                SchedulingId: 0,
                JobId: JobId,
                UserId: USERID,
                UniqueVisitId: $(".pdf-generated-get-details.def").attr("data-uniquevisistid"),
                IsDefault: true,
                ClassType: ClassTypeVal
            };
            isAjaxCall = true;
        }
    }
    else if (SelectedVisitCount > 0 && CheckRefAndDefaultPhotoStatus()) {
        alert("Please select only one folder.");
    }
    if (isAjaxCall) {
        $.ajax({
            url: urlGenerateAllImagesOfVisistInPdf,
            type: 'POST',
            async: false,
            cache: false,
            data: JSON.stringify(JsonData),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                showMessageForJobPhoto("PDF generated successfully.", false);
                getDocuments(false);
                $("#loading-image").hide();
            },
            error: function (data) {
                alert(data);
                $("#loading-image").hide();
            }
        });
    }
    else
        $("#loading-image").hide();
    SelectedDiv = null;
    VisitChecklistItemIdsVal = "";
}

function lnkUncheck_click() {
    $('#divVisitList').find('input[type=checkbox]').removeAttr('checked');
}
function lnkDelete_click() {
    if (!confirm("Are you sure you want to delete these files?")) return !1; var c = $("#accordion").find(".panel-body").find("input[type=checkbox]:checked").closest("li"), f = [], b = [], d = [], e = $("#divVisitList").find(".deleteJobVisit").find("input[type=checkbox]:checked"); $.each(e, function (a, e) { d.push($(this).data("jobscid")) }); $.each(c, function (a, d) { f.push($(this).data("vclid")); b.push($(this).data("vsid")) }); a = f.join(","); var l = b.join(","); var g = d.join(",");
    $.ajax({
        url: urlDeleteCheckListPhotos, data: { checkListIds: a, sigIds: l, pdelete: g, jobId: jobId }, contentType: "application/json", method: "get", success: function (a) {
            var d = !1; if ("true" == a.status) {
                if (a.isSPVRequired.toString() == "true") {
                    ShowSPV();
                }
                else {
                    HideSPV();
                }
            }; if ("true" == a.status) return showMessageForJobPhoto("Photos has been deleted successfully.", !1), $.each(c, function (a, e) {
                1 == $(this).closest(".panel").closest(".collapse").closest(".panel").find(".visitParent").find(".submission").length && (d = !0); $(this).closest(".panel").find(".totalCount").text(parseInt($(this).closest(".panel").find(".totalCount").text()) -
                    1); $(this).remove()
            }), d && ReloadSTCModule(), e.closest(".panel").remove(), !1;
        }
    })
}
function CheckRefAndDefaultPhotoStatus() {
    if ($(".pdf-generated-get-details.ref:checked").length > 0 || $("#collapseAssetsInstallation").find(".panel .panel-heading .visit-list-check:checked").length > 0)
        return true;
    else
        return false;
}
$('#accordion .panel-body').find('input[type=checkbox]').change(function () {
    if (!this.checked) {
        $(this).closest('.panel').find('.panel-heading').find('[type=checkbox]').removeAttr('checked');
        $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').removeAttr('checked');
    }
    else {
        if ($(this).closest('.panel-body').find('input[type=checkbox]').length == $(this).closest('.panel-body').find('input[type=checkbox]:checked').length) {
            $(this).closest('.panel').find('.panel-heading').find('[type=checkbox]').prop('checked', 'checked');

            if ($(this).closest('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]').length == $(this).closest('[id*=accordion]').find('.panel-heading').find('input[type=checkbox]:checked').length) {
                $(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').prop('checked', 'checked');
            }
        }
    }
});
function showMessageForJobPhoto(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegionJobPhoto" : "successMsgRegionJobPhoto";
    var inVisible = isError ? "successMsgRegionJobPhoto" : "errorMsgRegionJobPhoto";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}
(function () {
    var $, Lightbox, LightboxOptions;
    $ = jQuery;
    LightboxOptions = (function () {
        function LightboxOptions() {
            this.fadeDuration = 0;
            this.fitImagesInViewport = true;
            this.resizeDuration = 0;
            this.showImageNumberLabel = true;
            this.wrapAround = false;
        }

        LightboxOptions.prototype.albumLabel = function (curImageNum, albumSize) {
            return "Image " + curImageNum + " of " + albumSize;
        };

        return LightboxOptions;

    })();

    Lightbox = (function () {
        function Lightbox(options) {
            this.options = options;
            this.album = [];
            this.SelfieAlbum = [];
            //this.UserSelfieAlbum = [];
            this.currentImageIndex = void 0;
            this.init();
        }

        Lightbox.prototype.init = function () {
            this.enable();
            return this.build();
        };

        Lightbox.prototype.enable = function () {
            var _this = this;
            return $('body').on('click', 'a[rel^=lightbox], area[rel^=lightbox], a[data-lightbox], area[data-lightbox]', function (e) {
                _this.start($(e.currentTarget));
                
                return false;
              
                
            });
        };
        Lightbox.prototype.GetSelfieImage = function ($url) {
            debugger;
          
            try
            {
                $cont = this.$lightbox.find('.lb-outerContainerSelfie');
                $ControlPanel = this.$lightbox.find('.lb-closeContainerSelfie');
                $isAllowVisibleInstallerSelfie = isAllowVisibleInstallerSelfie;
                $phototypeid = this.album != null && this.album.length > 0 ? this.album[0].phototypeid : 0;
                if (isAllowVisibleInstallerSelfie == "False" || ((isAllowVisibleInstallerSelfie == "False" && ((ProjectSession_UserTypeId != "1" && ProjectSession_UserTypeId != "3"))) || ($phototypeid != 2 || UserDocs == "" )) ){
                    $($cont).css({
                        visibility: "hidden"
                    });
                    $($ControlPanel).css({
                        visibility: "hidden"
                    });
                    $(".lb-outerContainer").css("position", "fixed");
                    $(".lb-closeContainer").css("position", "absolute");
                    $(".lb-data").css("display", "block");
                    return;
                }
                else {
                    $($cont).css({
                        visibility: "visible"
                    });
                    $($ControlPanel).css({
                        visibility: "visible"
                    });
                    $(".lb-outerContainer").css("position", "");
                    $(".lb-closeContainer").css("position", "relative");
                    $(".lb-data").css("display", "flex");
                    var strDocs = UserDocs.split(',');
                    strDocs.forEach(file => {
                        if (file.trim().length !== 0)
                            this.SelfieAlbum.push( file.trim());
                    });
                    this.SetSelfieImage(this.SelfieAlbum[0]);
                    this.currentSelfieImageIndex = 0;
                    return;
                    
                }
               
            }
            catch (ex)
            {
                alert(ex.message);
            }
        };

        Lightbox.prototype.SetSelfieImage = function (urli) {

            debugger;
            try {
                //let urli = $url.replace($url.substring($url.lastIndexOf('\\') + 1), "UserSelfie.jpg");

                this.OuterDiv = this.$outerContainerSelfie.find(".lb-container");
                //alert(urli);
                this.OuterDiv.html(urli.includes(".pdf") || urli.includes(".doc") || urli.includes(".docx") ? '<iframe class="lb-imageSelfie" style=\"position: relative; height: 100%; width: 100%; frameborder = "0"" src=\"https://docs.google.com/viewer?url=' + urli + '&embedded=true"></iframe>' :  urli.includes(".jpg") || urli.includes(".jpeg") || urli.includes(".png")? '<img class="lb-imageSelfie" style=\"position: relative; height: 100%; width: 100%; frameborder = "0""  src="' + urli + '" ></img>':"");
            } catch (ex) {
                alert(ex.message);
            }
           // this.setImageWidthHeight(urli);
        };
        Lightbox.prototype.setImageWidthHeight = function (urli) {
            debugger;
            var $image_selfie, preloader_selfie,
                _this = this;
            var q = new Date().getTime();
            this.disableKeyboardNav();
            $image_selfie = this.$lightbox.find('.lb-imageSelfie');
            //alert("Image from changeImage");
            this.sizeOverlay();
            this.$overlay.fadeIn(this.options.fadeDuration);
            $('.lb-loader').fadeIn('slow');
            this.$lightbox.find('.lb-imageSelfie, .lb-nav, .lb-dataContainer-selfie, .lb-numbers, .lb-caption-selfie').hide();
            this.$lightbox.find('.arrow-prevSelfie, .arrow-nextSelfie').addClass('disableNavigation');
            this.$lightbox.find('.lb-closeContainerSelfie').removeAttr('style').css('position', 'relative');
            this.$outerContainer.addClass('animating');
            this.$outerContainerSelfie.addClass('animating');
            preloader_selfie = new Image();
            preloader_selfie.onload = function () {
                var $preloader_selfie, imageHeightSelfie, imageWidthSelfie, maxImageHeightSelfie, maxImageWidthSelfie, windowHeightSelfie, windowWidth;
                $image_selfie.attr('src', urli /*+ '?v=' + q*/);
                $preloader_selfie = $(preloader_selfie);
                $image_selfie.width(preloader_selfie.width);
                $image_selfie.height(preloader_selfie.height);
                if (_this.options.fitImagesInViewport) {
                    windowWidth = $(window).width();
                    windowHeightSelfie = $(window).height();
                    maxImageWidthSelfie = windowHeightSelfie - _this.containerTopPadding - _this.containerBottomPadding - 110;//windowWidth - _this.containerLeftPadding - _this.containerRightPadding - 20;
                    maxImageHeightSelfie = windowHeightSelfie - _this.containerTopPadding - _this.containerBottomPadding - 110;
                    if ((preloader_selfie.width > maxImageWidthSelfie) || (preloader_selfie.height > maxImageHeightSelfie)) {
                        if ((preloader_selfie.width / maxImageWidthSelfie) > (preloader_selfie.height / maxImageHeightSelfie)) {
                            imageWidthSelfie = maxImageWidthSelfie;
                            imageHeightSelfie = parseInt(preloader_selfie.height / (preloader_selfie.width / imageWidthSelfie), 10);
                            $image_selfie.width(imageWidthSelfie);
                            $image_selfie.height(imageHeightSelfie);
                        } else {
                            imageHeightSelfie = maxImageHeightSelfie;
                            imageWidthSelfie = parseInt(preloader_selfie.width / (preloader_selfie.height / imageHeightSelfie), 10);
                            $image_selfie.width(imageWidthSelfie);
                            $image_selfie.height(imageHeightSelfie);
                        }
                    }
                }
                return _this.sizeContainer($image_selfie.width(), $image_selfie.height());
            };
            this.setAngle(0);
            preloader_selfie.src = urli /*+ '?v=' + q*/;
           
        };

        Lightbox.prototype.build = function () {
            debugger;
            
            console.log('built');
            var _this = this;

            if ($('#lightboxOverlay').length) {

                $('#lightboxOverlay').remove();
                $('#lightbox').remove();
            }
            $("<div id='lightboxOverlay' class='lightboxOverlay'></div><div id='lightbox' class='lightbox'>          <div class='lb-outerContainer'><div class='lb-container'><img class='lb-image' src='' /><div class='lb-loader'><a class='lb-cancel'></a></div></div></div>            <div class='lb-outerContainerSelfie'><div id='' class='lb-container' id='CompareFile' style=\"position: relative; height: 100%; width: 100%; frameborder = '0'\">" +
                "<iframe ID='iframeCompare' class='lb-imageSelfie'  src='' style=\"position: relative; height: 100%; width: 100%; frameborder = '0' \" /><div class='lb-loader'><a class='lb-cancel'></a></div></div></div><div class='lb-dataContainer'><div class='lb-data'>" +
                "<div class='lb-closeContainer'><div class='lb-details'style='margin-bottom: 10px;'><span class='lb-caption'></span></div><a class='lb-close sprite-img'></a><a class='lb-rotate sprite-img'></a><a class='arrow-next sprite-img'></a><a class='arrow-prev sprite-img'></a><a class='lb-save sprite-img'></a></div>              <div class='lb-closeContainerSelfie'><div class='lb-details'style='margin-bottom: 10px;'><span class='lb-caption-selfie'>Installer Profile Pictures</span></div><a class='lb-close sprite-img'></a><a class='lb-rotateSelfie sprite-img'></a><a class='arrow-nextSelfie sprite-img'></a><a class='arrow-prevSelfie sprite-img'></a><a class='lb-save sprite-img'></a></div></div></div></div>").appendTo($('body'));
            //$("<div id='lightboxOverlay' class='lightboxOverlay'></div><div id='lightbox' class='lightbox'>          <div class='lb-outerContainer'><div class='lb-container'><img class='lb-image' src='' /><div class='lb-loader'><a class='lb-cancel'></a></div></div></div>            <div class='lb-outerContainer'><div id='' class='lb-container'><img class='lb-imageSelfie' src='' /><div class='lb-loader'><a class='lb-cancel'></a></div></div></div><div class='lb-dataContainer'><div class='lb-data'>" +
            //                   "<div class='lb-closeContainer'><div class='lb-details'style='margin-bottom: 10px;'><span class='lb-caption'></span></div><a class='lb-close sprite-img'></a><a class='lb-rotate sprite-img'></a><a class='arrow-next sprite-img'></a><a class='arrow-prev sprite-img'></a><a class='lb-save sprite-img'></a></div>              <div class='lb-closeContainer'><div class='lb-details'style='margin-bottom: 10px;'><span class='lb-caption'></span></div><a class='lb-close sprite-img'></a><a class='lb-rotate sprite-img'></a><a class='arrow-next sprite-img'></a><a class='arrow-prev sprite-img'></a><a class='lb-save sprite-img'></a></div></div></div></div>").appendTo($('body'));
            $(".lb-closeContainer").draggable();
            $(".lb-closeContainerSelfie").draggable();
            $(".lb-details").mousedown(function (e) {
                e.stopPropagation();
            });
            this.$lightbox = $('#lightbox');
            this.$overlay = $('#lightboxOverlay');
            this.$outerContainer = this.$lightbox.find('.lb-outerContainer');
            this.$outerContainerSelfie = this.$lightbox.find('.lb-outerContainerSelfie');
            this.$container = this.$lightbox.find('.lb-container');
            this.containerTopPadding = parseInt(this.$container.css('padding-top'), 10);
            this.containerRightPadding = parseInt(this.$container.css('padding-right'), 10);
            this.containerBottomPadding = parseInt(this.$container.css('padding-bottom'), 10);
            this.containerLeftPadding = parseInt(this.$container.css('padding-left'), 10);
            this.$overlay.hide();//.on('click', function() {
            this.$lightbox.hide();//.hide();//.on('click', function(e) {

            ///selfie
            this.$lightbox.find('.arrow-prevSelfie').off('click').on('click', function () {
                if (_this.currentSelfieImageIndex === 0)
                {
                    _this.SetSelfieImage(_this.SelfieAlbum[_this.SelfieAlbum.length - 1]);
                    _this.currentSelfieImageIndex=_this.SelfieAlbum.length - 1;
                }
                else
                {
                    _this.SetSelfieImage(_this.SelfieAlbum[_this.currentSelfieImageIndex - 1]);
                    _this.currentSelfieImageIndex -= 1;
                }
                return false;
            });
            this.$lightbox.find('.arrow-nextSelfie').off('click').on('click', function () {
                if (_this.currentSelfieImageIndex === _this.SelfieAlbum.length - 1)
                {
                    _this.SetSelfieImage(_this.SelfieAlbum[0]);
                    _this.currentSelfieImageIndex = 0;
                }
                else
                {
                    _this.SetSelfieImage(_this.SelfieAlbum[_this.currentSelfieImageIndex + 1]);
                    _this.currentSelfieImageIndex += 1;


                }
                return false;
            });
            this.$lightbox.find('.lb-rotateSelfie').off('click').on('click', function () {
                $cont = _this.$lightbox.find('.lb-outerContainerSelfie');
                $image = _this.$lightbox.find('.lb-image');

                if ($($cont).attr('angle') == null) {
                    $($cont).attr('angle', 0);
                }
                var value = Number($($cont).attr('angle'));
                value += 90;

                if (value == 0) {
                    $($cont).rotate(0);
                } else {
                    $($cont).rotate({ animateTo: value });
                }
                $($cont).attr('angle', value);

                return false;
            });
            this.$lightbox.find('.lb-save').off('click').on('click', function () {
                var matrix = _this.$lightbox.find('.lb-image').css("transform");
                var src = _this.$lightbox.find('.lb-image').attr("src");
                var angle = (_this.$lightbox.find('.lb-outerContainerSelfie').attr('angle') % 360);
                $.ajax({
                    url: urlSaveImage,
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: JSON.stringify({ Angle: angle, Src: src }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.result == true) {
                            alert("Image Saved.");
                        }
                        else {
                            alert("Error Occured");
                        }
                    },
                    error: function (data) {
                        if (data.error) {
                            alert("Error");
                        }
                    },
                });
            });
            this.$lightbox.find('.lb-loader, .lb-close').off('click').on('click', function () {
                _this.end();
                $('body').removeClass("HideOverflow");
                return false;
            });



            ///Visit Images
            this.$lightbox.find('.arrow-prev').off('click').on('click', function () {
                if (_this.currentImageIndex === 0) {
                    _this.changeImage(_this.album.length - 1);
                } else {
                    _this.changeImage(_this.currentImageIndex - 1);
                }
                return false;
            });
            this.$lightbox.find('.arrow-next').off('click').on('click', function () {
                if (_this.currentImageIndex === _this.album.length - 1) {
                    _this.changeImage(0);
                } else {
                    _this.changeImage(_this.currentImageIndex + 1);
                }
                return false;
            });
            this.$lightbox.find('.lb-rotate').off('click').on('click', function () {
                $cont = _this.$lightbox.find('.lb-outerContainer');
                $image = _this.$lightbox.find('.lb-image');

                if ($($cont).attr('angle') == null) {
                    $($cont).attr('angle', 0);
                }
                var value = Number($($cont).attr('angle'));
                value += 90;

                if (value == 0) {
                    $($cont).rotate(0);
                } else {
                    $($cont).rotate({ animateTo: value });
                }
                $($cont).attr('angle', value);

                return false;
            });
            this.$lightbox.find('.lb-save').off('click').on('click', function () {
                var matrix = _this.$lightbox.find('.lb-image').css("transform");
                var src = _this.$lightbox.find('.lb-image').attr("src");
                var angle = (_this.$lightbox.find('.lb-outerContainer').attr('angle') % 360);
                $.ajax({
                    url: urlSaveImage,
                    type: 'POST',
                    async: false,
                    cache: false,
                    data: JSON.stringify({ Angle: angle, Src: src }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.result == true) {
                            alert("Image Saved.");
                        }
                        else {
                            alert("Error Occured");
                        }
                    },
                    error: function (data) {
                        if (data.error) {
                            alert("Error");
                        }
                    },
                });
            });
            this.$lightbox.find('.lb-loader, .lb-close').off('click').on('click', function () {
                _this.end();
                $('body').removeClass("HideOverflow");
                return false;
            });

        };

        Lightbox.prototype.setAngle = function (angle) {

            $cont = this.$lightbox.find('.lb-outerContainer');
            if ($($cont).attr('angle') != angle) {
                $($cont).rotate({ animateTo: angle });
                $($cont).attr('angle', angle);
            }
        }

        Lightbox.prototype.start = function ($link) {
            debugger;
           
            var $window, a, dataLightboxValue, i, imageNumber, left, top, _i, _j, _len, _len1, _ref, _ref1;
            $(window).on("resize", this.sizeOverlay);
            $('select, object, embed').css({
                visibility: "hidden"
            });
            this.$overlay.width($(document).width()).height($(document).height()).fadeIn(this.options.fadeDuration);
            this.album = [];
            imageNumber = 0;
            dataLightboxValue = $link.attr('data-lightbox');
            if (dataLightboxValue) {
                _ref = $link.closest('.panel').find($link.prop("tagName") + '[data-lightbox="' + dataLightboxValue + '"]');
                for (i = _i = 0, _len = _ref.length; _i < _len; i = ++_i) {
                    a = _ref[i];
                    this.album.push({
                        link: $(a).attr('href'),
                        title: $(a).attr('title'),
                        date: $(a).data('date'),
                        lat: $(a).data('lat'),
                        lon: $(a).data('lon'),
                        phototypeid: $(a).data('phototypeid')
                    });
                    if ($(a).attr('href') === $link.attr('href')) {
                        imageNumber = i;
                    }
                }
            } else {
                if ($link.attr('rel') === 'lightbox') {
                    this.album.push({
                        link: $link.attr('href'),
                        title: $link.attr('title')
                    });
                } else {
                    _ref1 = $($link.prop("tagName") + '[rel="' + $link.attr('rel') + '"]');
                    for (i = _j = 0, _len1 = _ref1.length; _j < _len1; i = ++_j) {
                        a = _ref1[i];
                        this.album.push({
                            link: $(a).attr('href'),
                            title: $(a).attr('title')
                        });
                        if ($(a).attr('href') === $link.attr('href')) {
                            imageNumber = i;
                        }
                    }
                }
            }
            $window = $(window);
            top = $window.scrollTop() + $window.height() / 10;
            left = $window.scrollLeft();
            this.$lightbox.css({
                top: top + 'px',
                left: left + 'px'
            }).fadeIn(this.options.fadeDuration);
            this.setAngle(0);
            this.changeImage(imageNumber);
            this.GetSelfieImage($link.attr('href'));
        };

        Lightbox.prototype.changeImage = function (imageNumber) {
            debugger;
            var $image, preloader,
              _this = this;
            var q = new Date().getTime();
            this.disableKeyboardNav();
            $image = this.$lightbox.find('.lb-image');
            //alert("Image from changeImage");
            this.sizeOverlay();
            this.$overlay.fadeIn(this.options.fadeDuration);
            $('.lb-loader').fadeIn('slow');
            this.$lightbox.find('.lb-image, .lb-nav, .lb-dataContainer, .lb-numbers, .lb-caption').hide();
            this.$lightbox.find('.arrow-prev, .arrow-next').addClass('disableNavigation');
            this.$lightbox.find('.lb-closeContainer').removeAttr('style').css('position', 'relative');
            this.$outerContainer.addClass('animating');
            this.$outerContainerSelfie.addClass('animating');
            preloader = new Image();
            preloader.onload = function () {
                var $preloader, imageHeight, imageWidth, maxImageHeight, maxImageWidth, windowHeight, windowWidth;
                $image.attr('src', _this.album[imageNumber].link /*+ '?v=' + q*/);
                $preloader = $(preloader);
                $image.width(preloader.width);
                $image.height(preloader.height);
                if (_this.options.fitImagesInViewport) {
                    windowWidth = $(window).width();
                    windowHeight = $(window).height();
                    maxImageWidth = windowHeight - _this.containerTopPadding - _this.containerBottomPadding - 110;//windowWidth - _this.containerLeftPadding - _this.containerRightPadding - 20;
                    maxImageHeight = windowHeight - _this.containerTopPadding - _this.containerBottomPadding - 110;
                    if ((preloader.width > maxImageWidth) || (preloader.height > maxImageHeight)) {
                        if ((preloader.width / maxImageWidth) > (preloader.height / maxImageHeight)) {
                            imageWidth = maxImageWidth;
                            imageHeight = parseInt(preloader.height / (preloader.width / imageWidth), 10);
                            $image.width(imageWidth);
                            $image.height(imageHeight);
                        } else {
                            imageHeight = maxImageHeight;
                            imageWidth = parseInt(preloader.width / (preloader.height / imageHeight), 10);
                            $image.width(imageWidth);
                            $image.height(imageHeight);
                        }
                    }
                }
                return _this.sizeContainer($image.width(), $image.height());
            };
            this.setAngle(0);
            preloader.src = this.album[imageNumber].link /*+ '?v=' + q*/;
            this.currentImageIndex = imageNumber;
        };

        Lightbox.prototype.sizeOverlay = function () {

            return $('#lightboxOverlay').width($(document).width()).height($(document).height());
        };

        Lightbox.prototype.sizeContainer = function (imageWidth, imageHeight) {

            var newHeight, newWidth, oldHeight, oldWidth,
              _this = this;
            oldWidth = this.$outerContainer.outerWidth();
            oldWidthSelfie = this.$outerContainerSelfie.outerWidth();
            oldHeight = this.$outerContainer.outerHeight();
            oldHeightSelfie = this.$outerContainerSelfie.outerHeight();
            newWidth = imageWidth + this.containerLeftPadding + this.containerRightPadding;
            newHeight = imageHeight + this.containerTopPadding + this.containerBottomPadding;
            this.$outerContainer.animate({
                width: newWidth,
                height: newHeight
            }, this.options.resizeDuration, 'swing');
            this.$outerContainerSelfie.animate({
                width: newWidth,
                height: newHeight
            }, this.options.resizeDuration, 'swing');
            setTimeout(function () {
                //_this.$lightbox.find('.lb-dataContainer').width(newWidth);
                _this.$lightbox.find('.lb-prevLink').height(newHeight);
                _this.$lightbox.find('.lb-nextLink').height(newHeight);
                _this.showImage();
            }, this.options.resizeDuration);
        };

        Lightbox.prototype.showImage = function () {
            //alert("from show image");
            this.$lightbox.find('.lb-loader').hide();
            this.$lightbox.find('.lb-image').fadeIn('slow');
            this.updateNav();
            this.updateDetails();
            this.preloadNeighboringImages();
            this.enableKeyboardNav();
            resizeImage();
        };

        Lightbox.prototype.updateNav = function () {

            this.$lightbox.find('.lb-nav').show();
            if (this.album.length > 1) {
                if (this.options.wrapAround) {
                    this.$lightbox.find('.arrow-prev, .arrow-next').removeClass('disableNavigation');
                } else {
                    if (this.currentImageIndex > 0) {
                        this.$lightbox.find('.arrow-prev').removeClass('disableNavigation');
                    }
                    if (this.currentImageIndex < this.album.length - 1) {
                        this.$lightbox.find('.arrow-next').removeClass('disableNavigation');
                    }
                }
            }
        };

        Lightbox.prototype.updateDetails = function () {
            var _this = this,
                _album = _this.album[this.currentImageIndex];
            if (typeof this.album[this.currentImageIndex].title !== 'undefined' && this.album[this.currentImageIndex].title !== "") {
                var labelfordisplayLatLong = null;
                if (_album.lat == "" || _album.lon == "") {
                    // || _album.lat == "0" || _album.lon == "0") {
                    labelfordisplayLatLong = "";
                }
                else
                {
                    labelfordisplayLatLong = _album.lat + "," + _album.lon;
                }
                   
                this.$lightbox.find('.lb-caption').html('<a href="#" title="Show on map" onclick="showMap(' + _album.lat + ',' + _album.lon + ') ">' + labelfordisplayLatLong + "      " + _album.date + '</a>').fadeIn('fast');
            }
            if (this.album.length > 1 && this.options.showImageNumberLabel) {
                this.$lightbox.find('.lb-number').text(this.options.albumLabel(this.currentImageIndex + 1, this.album.length)).fadeIn('fast');
            } else {
                this.$lightbox.find('.lb-number').hide();
            }
            this.$outerContainer.removeClass('animating');
            this.$outerContainerSelfie.removeClass('animating');
            this.$lightbox.find('.lb-dataContainer').fadeIn(this.resizeDuration, function () {
                return _this.sizeOverlay();
            });
        };

        Lightbox.prototype.preloadNeighboringImages = function () {
            var preloadNext, preloadPrev;
            if (this.album.length > this.currentImageIndex + 1) {
                preloadNext = new Image();
                preloadNext.src = this.album[this.currentImageIndex + 1].link;
            }
            if (this.currentImageIndex > 0) {
                preloadPrev = new Image();
                preloadPrev.src = this.album[this.currentImageIndex - 1].link;
            }
        };

        Lightbox.prototype.enableKeyboardNav = function () {
            $(document).on('keyup.keyboard', $.proxy(this.keyboardAction, this));
        };

        Lightbox.prototype.disableKeyboardNav = function () {
            $(document).off('.keyboard');
        };

        Lightbox.prototype.keyboardAction = function (event) {

            var KEYCODE_ESC, KEYCODE_LEFTARROW, KEYCODE_RIGHTARROW, key, keycode;
            KEYCODE_ESC = 27;
            KEYCODE_LEFTARROW = 37;
            KEYCODE_RIGHTARROW = 39;
            keycode = event.keyCode;
            key = String.fromCharCode(keycode).toLowerCase();
            //if (keycode === KEYCODE_ESC || key.match(/x|o|c/)) {
            //    this.end();
            //} else
            if (key === 'p' || keycode === KEYCODE_LEFTARROW) {
                if (this.currentImageIndex !== 0) {
                    this.changeImage(this.currentImageIndex - 1);
                }
            } else if (key === 'n' || keycode === KEYCODE_RIGHTARROW) {
                if (this.currentImageIndex !== this.album.length - 1) {
                    this.changeImage(this.currentImageIndex + 1);
                }
            }
        };

        Lightbox.prototype.end = function () {

            this.disableKeyboardNav();
            $(window).off("resize", this.sizeOverlay);
            this.$lightbox.fadeOut(this.options.fadeDuration);
            this.$overlay.fadeOut(this.options.fadeDuration);
            return $('select, object, embed').css({
                visibility: "visible"
            });
        };

        return Lightbox;

    })();

    $(function () {
        var lightbox, options;
        options = new LightboxOptions();
        return lightbox = new Lightbox(options);
    });



}).call(this);
var geocoder;
var map;
var location1;
var location2;
function showMap(lat, lon) {
   
            $(".lb-close").click();
            $('#txtSource').val(lat + "," + lon);
            $('#txtDestination').val($('#txtAddress').val());



            loadMapScript(1);
   /* initialize(lat, lon)*/;
        $("#distance").html('');
        $("#errorMsgRegionMap").hide();
        $('#pMap').modal({ backdrop: 'static', keyboard: false });
       

    setTimeout(function () {
        //alert(1);
         GetrouteWithMarkerWithoutRoute(1);
        createLine();
    }, 1000);

}
function GetrouteWithMarkerWithoutRoute(isFromPhotoView = 0) {
    $("#dvDistance").html('');
    $("#errorMsgRegionMap").hide();
    source = document.getElementById("txtSource").value;
    destination = document.getElementById("txtDestination").value;
    if (source != "" && destination != "") {
        var India = new google.maps.LatLng(51.508742, -0.120850);
        var mapOptions = {
            zoom: 4,
            center: India
        };
        if (isFromPhotoView == 1) {
            map = new google.maps.Map(document.getElementById('dMap'), mapOptions);
        }
        else {
            map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
        }
        var latdes = $("#lblLatitude").text();
        var longdes = $("#lblLongitude").text();
        var latsource = $("#txtSource").val().split(",")[0];
        var longsource = $("#txtSource").val().split(",")[1];
        var locations = [
           [latdes, longdes],
            [ latsource, longsource]
           
        ];

        var infowindow = new google.maps.InfoWindow();
        var marker, i;
        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][0], locations[i][1]),
                map: map
            });
        }
    }
    else {
        $("#errorMsgRegionMap").html(closeButton + " Both Source and Destination  address are  required");
        $("#errorMsgRegionMap").show();
    }
}

var address;
    var address2;
    function createLine() {
        address = $("#txtSource").val();
        address2 = $("#txtDestination").val();
        var latdes = $("#lblLatitude").text();
        var longdes = $("#lblLongitude").text();
        var latsource = $("#txtSource").val().split(",")[0];
        var longsource = $("#txtSource").val().split(",")[1];
        var destlatlong=[];
        destlatlong.push(latdes);
        destlatlong.push(longdes);
        var temp, temp2;
                temp = $("#txtSource").val().split(",");
                if (address2) {
                        temp2 = destlatlong;
                        var polyline = new google.maps.Polyline({
                            path: [new google.maps.LatLng(latdes, longdes), new google.maps.LatLng(latsource, longsource)],
                            strokeColor: "#FF5E56",
                            strokeOpacity: 0.6,
                            strokeWeight: 5
                        });
                        location1 = temp;
                        location2 = destlatlong;
                        var lengthInMeters = distance(location1[0], location1[1], location2[0], location2[1],"K");
                        document.getElementById('distance').innerHTML += "Distance between installation address and captured photo location is " + lengthInMeters + " meters long<br>";
                      
                    polyline.setMap(map);
                        plotMap(location1, location2);
                    //});
                    address = address2;
                } else {
                    location1 = convertLocationToLatLong(temp.toUrlValue());
                    plotMap(location1);
                }
               
    
}
function distance(lat1, lon1, lat2, lon2, unit) {
    if ((lat1 == lat2) && (lon1 == lon2)) {
        return 0;
    }
    else {
        var radlat1 = Math.PI * lat1 / 180;
        var radlat2 = Math.PI * lat2 / 180;
        var theta = lon1 - lon2;
        var radtheta = Math.PI * theta / 180;
        var dist = Math.sin(radlat1) * Math.sin(radlat2) + Math.cos(radlat1) * Math.cos(radlat2) * Math.cos(radtheta);
        if (dist > 1) {
            dist = 1;
        }
        dist = Math.acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;
        if (unit == "K") { dist = dist * 1.609344 }
        
        //dist = dist * 1000;
        dist= Math.round(dist * 1000);
        return dist;
    }
}
function convertLocationToLatLong(location) {
    var locationdest = location.split(',').map(function (item) {
        return parseFloat(item);
    });
    return locationdest;
}
function GetAddress() {
    var lat = $("#txtSource").val().split(',')[0];
    var lng = $("#txtSource").val().split(',')[1];
    var latlng = new google.maps.LatLng(lat, lng);
    var geocoder = geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[1]) {
                alert("Location: " + results[1].formatted_address);
            }
        }
    });
}
function plotMap(location1, location2) {
    var loc1 = new google.maps.LatLng(location1[0], location1[1]);
    if (location2) {
        var loc2 = new google.maps.LatLng(location2[0], location2[1]);
    }
    var bounds = new google.maps.LatLngBounds();
    bounds.extend(loc1);
    if (loc2) {
        bounds.extend(loc2);
    }
    map.fitBounds(bounds);
    setZoom();
}

function setZoom() {
    google.maps.event.addListener(map, 'zoom_changed', function () {
        zoomChangeBoundsListener =
            google.maps.event.addListener(map, 'bounds_changed', function (event) {
                if (this.getZoom() > 15 && this.initialZoom == true) {
                    // Change max/min zoom here
                    this.setZoom(15);
                    this.initialZoom = false;
                }
                google.maps.event.removeListener(zoomChangeBoundsListener);
            });
    });
    map.initialZoom = true;
}
function LoadIsSPVRequired() {
    $.ajax({
        url: urlGetIsSPV,
        type: "GET",
        data: { jobId: jobId },
        dataType: "json",
        success: function (Data) {
            $("div.line-no").removeClass("verified")
            $("div.line-no").removeClass("unverified")
            $("div.line-no").removeClass("installationVerified")
            $("div.line-no").removeClass("notverified")
            $("#GlobalisAllowedSPV").val(Data.GlobalisAllowedSPV)
            if (Data.IsSPVRequired) {
                IsSPVRequired = "True";
            }
            else {
                IsSPVRequired = "False";
            }
            if ($('#SerialNumberPanel').length > 0) {
                //LoadCommonSerialNumber()
                if (Data.IsSPVRequired) {
                     VerifyUnVerifySerialNumber();
                   // var unverifiedSerialNumber = serialNumber.find(serialNumber => serialNumber.IsVerified == null);
                    if ( $("#GlobalisAllowedSPV").val().toLowerCase() == "true" && IsSPVRequired == "True")
                        $(".isSPVRequired").show();
                    else
                        $(".isSPVRequired").hide();
                } else {
                    $(".isSPVRequired").hide();
                    $("#SPVLabel").hide();
                }

            }
            else {
                VerifyUnVerifySerialNumber();
            }
        }
    });
}

function lnkIgnore200mtrRule_click() {
    $.ajax({
        type: 'post',
        url: urlIgnore200mtrValidation,
        dataType: 'json',
        data: { jobId: JobId },
        async: false,
        success: function (data) {
            if (data.status) {
                ReloadJobPhotoSection(JobId);
                setTimeout(function () {
                    if (data.result == "1") {
                        showMessageForJobPhoto("Successfully removed 200mtr validation rule.", false);
                        if (data.isSPVRequired.toString() == "True" || data.isSPVRequired.toString() == "true") {
                            ShowSPV();
                        }
                        else {
                            HideSPV();
                        }
                    }
                    if (data.result == "2")
                        showMessageForJobPhoto("Successfully removed 200mtr validation but there is from SCA/Global/Manufacturer leval spv has been off.", true);
                    if (data.result == "3")
                        showMessageForJobPhoto("Successfully removed 200mtr validation but job panel must be one for do SPV product verification.", true);

                }, 1000);
                

            }
            else {
                showMessageForJobPhoto("Something went wrong in removing 200mtr validation.", true);
            }

        },
        error: function (ex) {
            alert('Failed to remove 200mtr validation rule.' + ex);
        }
    });
}
$('#divVisitList .panel-heading input[type=checkbox]').change(function () {
    var $checkedrestorevisit = $('.visitId').find('.visit-list-check:checked').add();
    if ($checkedrestorevisit.data('isdeleted') == "True") {
        $('#restoreDeletedVisit').show();
    }
    else if ($checkedrestorevisit.length != 1) {
        $('#restoreDeletedVisit').hide();
    }

});
function restoreDeletedVisit() {
    var jobscids = [];
    $.each($(".visitId .visit-list-check:checked"), function () {
        jobscids.push($(this).val());
    });
    var jobScId = jobscids.join(",");
    $.ajax({
        url: urlrestoreDeletedVisit,
        method: 'GET',
        data: { jobscId: jobScId },
        cache: false,
        async: false,
        success: function (Data) {
        }
    });
}
function lnkDeletedCheckListitem_click() {
    $.get(urldeletedChecklist + jobId, function (data) {
        $('#mdlChecklistItmesdata').empty();
        $('#mdlChecklistItmesdata').append(data);
        $('#mdlChecklistItmes').modal({ backdrop: 'static', keyboard: false });
    });
}
$("#restorebtn").click(function () {
    var vcphotoids = [];
    var $checkeditems = $('.restore').find('.visit-list-check:checked').add();
    var jobScId = $checkeditems.data('jobscid');
    var vclId = $checkeditems.data('cid');

    var $checkedchecklistitem = $('.restore-photo').find('.visit-list-check:checked').add();
    if ($checkedchecklistitem.length != 1) {
        alert("Please select one Check list Item.");
        return false;
    }

    $.each($(".photo .visit-list-check:checked"), function () {
        vcphotoids.push($(this).val());
    });
    var vcphotoId = vcphotoids.join(",");
    vclId = vclId != null ? vclId : $checkedchecklistitem.data('cid');
    var type = $checkedchecklistitem.data('ct');
    var isDefault = false;
    var isReference = false;
    if (vclId == 'Default_Photo_Serial' || vclId == 'Default_Photo_installation') {
        type = $checkedchecklistitem.data('type');
        isDefault = true;
        vclId = null;
    }
    if (vclId == 'Reference_Photo') {
        isReference = true;
        vclId = null;
    }

    urlRestoreData = urlRestoreDataforReferenceAndDefault + vcphotoId + '&jobid=' + jobId + '&jobscId=' + jobScId + '&vclId=' + vclId + '&type=' + type + '&isReference=' + isReference + '&isDefault=' + isDefault;
    $('#restoreChecklistItmes').modal('hide');

    $.ajax({
        url: urlRestoreData,
        method: 'GET',
        cache: false,
        async: false,
        success: function (data) {
            if (data) {
                $("#mdlChecklistItmesdata").load(urldeletedChecklist + jobId);
                ReloadJobPhotoSection1(jobId);
                $('#mdlChecklistItmes').modal('show');
                showMessageForJobPhoto("File has been restored successfully.", false);
            } else {
                $('#mdlChecklistItmes').modal('show');
            }
        }
    });
});
$("#cancelbtn").click(function () {
    $('#restoreChecklistItmes').modal('hide');
    $('#mdlChecklistItmes').modal('show');
});

$("#mdlcloseclitems").click(function () {
    $('#mdlChecklistItmes').modal('hide');
});
function ReloadJobPhotoSection1(jobId) {
    $("#loading-image").show();
    setTimeout(function () {
        $.get(urlreloadphoto + jobId, function (data) {
            $('#loadNewJobPhoto').empty();
            $('#loadNewJobPhoto').append(data);
            showMessageForJobPhoto("File has been restored successfully.", false);
        });
    }, 500);
}
