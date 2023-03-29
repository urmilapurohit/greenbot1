$(document).ready(function () {
    MakeDefaultFolderAsDefaultSubmission();
});

function MakeDefaultFolderAsDefaultSubmission() {
    var isVisitDefault = false;
    $.each($("#divVisitList").find('.panel'), function (i, e) {
        if ($(this).attr('id') != "pnlReference" && $(this).attr('id') != "pnlMainDefault") {
            if ($(this).find('.visitParent').find('.submission').length > 0) {
                isVisitDefault = true;
            }
        }
    });

    if (!isVisitDefault) {
        var defaultSpan = "<span class=submission>STC Submission <i class='sprite-img submission-icon'></i></span>";
        $("#divVisitList").find("#pnlMainDefault").find('.visitParent').prepend(defaultSpan);
    }
}

var logoWidthPhoto, logoHeightPhoto;

var UploadData = {};
var urlPhoto = BaseURL + 'UploadReferencePhoto';

$('.veecChecklistPhoto .panel-heading input[type=checkbox]').change(function () {
    
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
        //formbot start

        //  var $checkedInput = $('#divVisitList').find('[data-IsDeleted="False"]').find('.parentInput:checked');


        var totalCount = 0;

        for (var i = 0; i < data.result.length; i++) {

            var path = data.result[i].Path;

            if (data.result[i].Status == true) {
                var li = $('<li/>')
                    //.attr('data-path', data.result[i].Path)
                    .attr('data-path', path)
                    .attr('data-vclid', data.result[i].AttachmentID)
                    .appendTo(ul);
                var chk = $('<input/>')
                    .attr('type', 'checkbox')
                    .addClass('visit-list-check')
                    .appendTo(li);
                var aaa = $('<a/>')
                    .addClass('ui-all')
                    .text(" " + data.result[i].FileName)
                    .attr('href', uploadPath + "//" + path)
                    .attr('data-lightbox', 'property')
                    .appendTo(li);
                totalCount++;
            }

            if ($('#ulPreference').children('li').length > 1) {
                if ($('#liNoData').length > 0) {
                    $('#liNoData').remove();
                }
            }
            if (data.result[i].Status == false && data.result[i].Message == "NotImage") {
                UploadFailedFilesType.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false && data.result[i].Message == "BigName") {
                UploadFailedFilesName.push(data.result[i].FileName);
            }
            else if (data.result[i].Status == false) {
                UploadFailedFiles.push(data.result[i].FileName);

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
        }

        if ($(ul).closest('.panel').closest('.collapse').closest('.panel').find('.visitParent').find('.submission').length == 1) {
            ReloadSTCModule();
        }

        $("#loading-image").show();
        setTimeout(function () {
            //getDocuments(false);
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
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessageForJobPhoto(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true);
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
            if (mimeType != "image") {
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
        VeecId: veecId, UserId: USERID, folder: UploadData.cId, isRef: UploadData.isRef,
        veecScId: UploadData.veecScId, IsDefault: UploadData.isDefault, ClassType: UploadData.Type,
        PdfLocationId: UploadData.PdfLocationId, ClassTypeId: UploadData.ClassTypeId,
        PdfName: UploadData.PdfName
    };
}

function downloadAllAsZip(isdwnlAll) {
    var mainPnl = $('#veecCheckListPhoto [id*=pnlMain]');
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
        
        window.location.href = urlDownloadVeecPhotos + "?veecId=" + VEECID + "&photos=" + JSON.stringify(allData) + "&refno=" + refno + "&isall=" + flag;
    }
    else
        alert("Please select photo first.");



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


    UploadData.cId = (isRef || isDefault) ? "" : $checkedInput.data('cid');
    UploadData.isRef = isRef;
    UploadData.isDefault = isDefault;
    UploadData.veecScId = (isRef || isDefault) ? "" : $checkedInput.data('jobscid');
    UploadData.Type = isDefault ? $checkedInput.data('type') : "";
    UploadData.PdfLocationId = $checkedInput.data('loc');
    UploadData.ClassTypeId = $checkedInput.data('ct');
    UploadData.PdfName = $checkedInput.data('nm');

    $('#uploadBtnPhoto').data().blueimpFileupload.options.formData = getData();
    document.getElementById('uploadBtnPhoto').click();
    return false;

}

function lnkUncheck_click() {
    $('#divVisitList').find('input[type=checkbox]').removeAttr('checked');
}
function lnkDelete_click() {
    if (!confirm("Are you sure you want to delete these files?")) return !1; var c = $("#accordion").find(".panel-body").find("input[type=checkbox]:checked").closest("li"), f = [], b = [], d = [], e = $("#divVisitList").find(".deleteJobVisit").find("input[type=checkbox]:checked"); $.each(e, function (a, e) { d.push($(this).data("jobscid")) }); $.each(c, function (a, d) { f.push($(this).data("vclid")); b.push($(this).data("vsid")) }); a = f.join(","); var l = b.join(","); var g = d.join(",");
    $.ajax({
        url: urlDeleteCheckListPhotos, data: { checkListIds: a, sigIds: l, pdelete: g, jobId: veecId }, contentType: "application/json", method: "get", success: function (a) {
            var d = !1; if ("true" == a) return showMessageForJobPhoto("Photos has been deleted successfully.", !1), $.each(c, function (a, e) {
                1 == $(this).closest(".panel").closest(".collapse").closest(".panel").find(".visitParent").find(".submission").length && (d = !0); $(this).closest(".panel").find(".totalCount").text(parseInt($(this).closest(".panel").find(".totalCount").text()) -
                1); $(this).remove()
            }), d && ReloadSTCModule(), e.closest(".panel").remove(), !1
        }
    })
}


function ReloadSTCModule() {
    $("#loading-image").show();
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

        Lightbox.prototype.build = function () {
            console.log('built');
            var _this = this;

            if ($('#lightboxOverlay').length) {

                $('#lightboxOverlay').remove();
                $('#lightbox').remove();
            }
            $("<div id='lightboxOverlay' class='lightboxOverlay'></div><div id='lightbox' class='lightbox'><div class='lb-outerContainer'><div class='lb-container'><img class='lb-image' src='' /><div class='lb-loader'><a class='lb-cancel'></a></div></div></div><div class='lb-dataContainer'><div class='lb-data'>" +
                               "<div class='lb-closeContainer'><div class='lb-details'style='margin-bottom: 10px;'><span class='lb-caption'></span></div><a class='lb-close sprite-img'></a><a class='lb-rotate sprite-img'></a><a class='arrow-next sprite-img'></a><a class='arrow-prev sprite-img'></a><a class='lb-save sprite-img'></a></div></div></div></div>").appendTo($('body'));
            $(".lb-closeContainer").draggable();
            $(".lb-details").mousedown(function (e) {
                e.stopPropagation();
            });
            this.$lightbox = $('#lightbox');
            this.$overlay = $('#lightboxOverlay');
            this.$outerContainer = this.$lightbox.find('.lb-outerContainer');
            this.$container = this.$lightbox.find('.lb-container');
            this.containerTopPadding = parseInt(this.$container.css('padding-top'), 10);
            this.containerRightPadding = parseInt(this.$container.css('padding-right'), 10);
            this.containerBottomPadding = parseInt(this.$container.css('padding-bottom'), 10);
            this.containerLeftPadding = parseInt(this.$container.css('padding-left'), 10);
            this.$overlay.hide();//.on('click', function() {
            //
            //    _this.end();
            //    return false;
            //});
            //$('.lightbox:visible').hide();
            this.$lightbox.hide();//.hide();//.on('click', function(e) {
            //
            //    if ($(e.target).attr('id') === 'lightbox') {
            //        _this.end();
            //    }
            //    return false;
            //});
            //this.$outerContainer.on('click', function(e) {
            //
            //    if ($(e.target).attr('id') === 'lightbox') {
            //        _this.end();
            //    }
            //    return false;
            //});
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
                //if(value == 270){
                //    value = -90;
                //}
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
                        lon: $(a).data('lon')
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
        };

        Lightbox.prototype.changeImage = function (imageNumber) {

            var $image, preloader,
              _this = this;
            var q = new Date().getTime();
            this.disableKeyboardNav();
            $image = this.$lightbox.find('.lb-image');

            this.sizeOverlay();
            this.$overlay.fadeIn(this.options.fadeDuration);
            $('.lb-loader').fadeIn('slow');
            this.$lightbox.find('.lb-image, .lb-nav, .lb-dataContainer, .lb-numbers, .lb-caption').hide();
            this.$lightbox.find('.arrow-prev, .arrow-next').addClass('disableNavigation');
            this.$lightbox.find('.lb-closeContainer').removeAttr('style').css('position', 'relative');
            this.$outerContainer.addClass('animating');
            preloader = new Image();
            preloader.onload = function () {
                var $preloader, imageHeight, imageWidth, maxImageHeight, maxImageWidth, windowHeight, windowWidth;
                $image.attr('src', _this.album[imageNumber].link + '?v=' + q);
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
            preloader.src = this.album[imageNumber].link + '?v=' + q;
            this.currentImageIndex = imageNumber;
        };

        Lightbox.prototype.sizeOverlay = function () {

            return $('#lightboxOverlay').width($(document).width()).height($(document).height());
        };

        Lightbox.prototype.sizeContainer = function (imageWidth, imageHeight) {

            var newHeight, newWidth, oldHeight, oldWidth,
              _this = this;
            oldWidth = this.$outerContainer.outerWidth();
            oldHeight = this.$outerContainer.outerHeight();
            newWidth = imageWidth + this.containerLeftPadding + this.containerRightPadding;
            newHeight = imageHeight + this.containerTopPadding + this.containerBottomPadding;
            this.$outerContainer.animate({
                width: newWidth,
                height: newHeight
            }, this.options.resizeDuration, 'swing');
            setTimeout(function () {
                _this.$lightbox.find('.lb-dataContainer').width(newWidth);
                _this.$lightbox.find('.lb-prevLink').height(newHeight);
                _this.$lightbox.find('.lb-nextLink').height(newHeight);
                _this.showImage();
            }, this.options.resizeDuration);
        };

        Lightbox.prototype.showImage = function () {

            //$('body').animate({ scrollTop: 0 }, "slow");
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
                this.$lightbox.find('.lb-caption').html('<a href="#" title="Show on map" onclick=showMap(' + _album.lat + ',' + _album.lon + ') >' + _album.lat + "," + _album.lon + "      " + _album.date + '</a>').fadeIn('fast');
            }
            if (this.album.length > 1 && this.options.showImageNumberLabel) {
                this.$lightbox.find('.lb-number').text(this.options.albumLabel(this.currentImageIndex + 1, this.album.length)).fadeIn('fast');
            } else {
                this.$lightbox.find('.lb-number').hide();
            }
            this.$outerContainer.removeClass('animating');
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

function showMap(lat, lon) {
    $('#MapPopup').modal({ backdrop: 'static', keyboard: false });

    setTimeout(function () {
        var myLatlng = new google.maps.LatLng(lat, lon);
        var myOptions = {
            zoom: 15,
            center: myLatlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        map = new google.maps.Map(document.getElementById("dMap"), myOptions);
        var marker = new google.maps.Marker({
            position: myLatlng,
            map: map,
            title: "Image"
        });
    }, 500);

}