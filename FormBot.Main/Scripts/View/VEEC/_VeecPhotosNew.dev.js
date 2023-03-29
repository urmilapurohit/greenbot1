function clpsAllVeecPhotos_click() {
    $('#accordion8').find('.collapse').collapse('hide');

}
function lnkDwnldVeecPhotos_click() {

    downloadAllAsVEECZip(0);
}
function lnkDwnldAllVeecPhotos_click() {
    
    
    downloadAllAsVEECZip(1);
}
var urlPhoto = BaseURL + 'UploadVeecPhoto';
$('#uploadBtnPhotoVeec').fileupload({
    url: urlPhoto,
    dataType: 'json',
    done: function (e, data) {
        var UploadFailedFiles = [];
        UploadFailedFiles.length = 0;
        var UploadFailedFilesType = [];
        UploadFailedFilesType.length = 0;
        var UploadFailedFilesName = [];
        UploadFailedFilesName.length = 0;
        var ul = $('#accordion8').find('[data-IsDeleted="False"]').find('.parentInput:checked').closest('.panel').find('ul');
        //formbot start

        //  var $checkedInput = $('#divVisitList').find('[data-IsDeleted="False"]').find('.parentInput:checked');
       

        var totalCount = 0;
        var ul=$('#accordion8').find('[data-IsDeleted="False"]').find('.parentInput:checked').closest('.panel').find('ul');
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
            showMessageForVeecPhoto(UploadFailedFilesType.length + " " + "Uploaded file is not .jpg , .jpeg or .png extension.", true);
        }
        if (UploadFailedFiles.length > 0) {
            showMessageForVeecPhoto(UploadFailedFiles.length + " " + "File has not been uploaded.", true);
        }
        if (UploadFailedFilesName.length > 0) {
            showMessageForVeecPhoto(UploadFailedFilesName.length + " " + "Uploaded filename is too big.", true);
        }
        if (UploadFailedFilesName.length == 0 && UploadFailedFiles.length == 0 && UploadFailedFilesType.length == 0) {
            showMessageForVeecPhoto("File has been uploaded successfully.", false);
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
                    showMessageForVeecPhoto("Please upload small filename.", true);
                    return false;
                }
            }
        }
        if (data.files.length > 1) {
            for (var i = 0; i < data.files.length; i++) {
                if (data.files[i].size > parseInt(MaxImageSize)) {
                    showMessageForVeecPhoto(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxsize + "MB", true);
                    return false;
                }
            }
        }
        else {
            if (data.files[0].size > parseInt(MaxImageSize)) {
                showMessageForVeecPhoto("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxsize + "MB", true);
                return false;
            }
        }
        if (data.files.length == 1) {
            if (mimeType != "image") {
                showMessageForVeecPhoto("Please upload a file with .jpg , .jpeg or .png extension.", true);
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
function uploadBtnVeecPhoto_click() {
    
    var $checkedInput = $('#accordion8').find('[data-IsDeleted="False"]').find('.parentInput:checked').add($('#headingReference').find('input:checked')).add($('.defaultPanel:checked'));
    if ($checkedInput.length != 1) {
        alert("Please select one active parent.");
        return false;
    }
    UploadData.folderId = $checkedInput.data('vcpid');
    UploadData.veecAreaId =  $checkedInput.data('veecareaid');
    UploadData.areaName = $checkedInput.data('veecareaname');
    UploadData.folder = $checkedInput.data('folder');
    UploadData.parentFolder = $checkedInput.data('parentfolder');
    $('#uploadBtnPhotoVeec').data().blueimpFileupload.options.formData = getData();
    document.getElementById('uploadBtnPhotoVeec').click();
    return false;

}

function lnkUncheckVeecPhotos_click() {
    $('#accordion8').find('input[type=checkbox]').removeAttr('checked');
}
function lnkDeleteVeecPhotos_click() {
    if (!confirm("Are you sure you want to delete these files?")) return !1; var c = $("#accordion8").find(".panel-body").find("input[type=checkbox]:checked").closest("li"), f = [], d = [], e = $("#accordion8").find(".deleteVeecVisit").find("input[type=checkbox]:checked"); $.each(e, function (a, e) { d.push($(this).data("veecareaid")) }); $.each(c, function (a, d) { f.push($(this).data("vclid")); }); a = f.join(","); var g = d.join(",");
    $.ajax({
        url: urlDeleteVeecPhotos, data: { veecPhotoId: a, veecAreaId: g, veecId: veecId }, contentType: "application/json", method: "get", success: function (a) {
            var d = !1; if ("true" == a) return showMessageForVeecPhoto("Photos has been deleted successfully.", !1), $.each(c, function (a, e) {
                1 == $(this).closest(".panel").closest(".collapse").closest(".panel").find(".visitParent").find(".submission").length && (d = !0); $(this).closest(".panel").find(".totalCount").text(parseInt($(this).closest(".panel").find(".totalCount").text()) -
                1); $(this).remove()
            }), d && ReloadSTCModule(), e.closest(".panel").remove(), !1
        }
    })
}

function downloadAllAsVEECZip(isdwnlAll) {
    var mainPnl = $('#accordion8').find('[id*=pnlMain]');
    var allData = {};
    var data = [];
    allData.vclid = [];

    $.each(mainPnl, function (i, e) {
        
        var obj = {}, checkListItems = [];
        var $pnl = $(this).find('#accordion02').children();
        obj.id = $(this).find('.panel-heading').find('a').data('name');
        var pnlCheck = {};
        
        $.each($pnl, function (i, e) {
           
            $.each(isdwnlAll == 0 ? $(this).find('.panel-body').find('input[type=checkbox]:checked') : $(this).find('.panel-body').find('input[type=checkbox]'), function (i, e) {
                if ($(this).parent().data('vclid')) {
                    allData.vclid.push($(this).parent().data('vclid'));
                    //allData.vclid.push($(this).parent().data('vclid'));
                }

            });
            
        });
        
            data.push(obj);

    });


    
    allData.vp = data;
    allData.vclid = allData.vclid.join(',') ;
    allData.isDownloadAll = isdwnlAll === 0 ? false : true;
    
    var flag = true;
    $(".submission-add > .veec-visit-list-check").each(function () {
        if ($(this).is(":checked")) {

        } else {
            flag = false;
        }
    });
    if (isdwnlAll == 1) {
        flag = true;
    }
    if (allData.vp.length > 0) {
       
        window.location.href = urlDownloadVeecFolderPhotos + "?veecId=" + VEECID + "&photos=" + JSON.stringify(allData) + "&isall=" + flag;
    }
    else
        alert("Please select photo first.");



    return false;
}
function ReloadSTCModule() {
    $("#loading-image").show();
}

function getData() {
    return {
        VeecId: veecId, UserId: USERID, folder: UploadData.folderId, isRef: UploadData.isRef,
        veecScId: UploadData.veecScId, IsDefault: UploadData.isDefault, ClassType: UploadData.Type,
        PdfLocationId: UploadData.PdfLocationId, ClassTypeId: UploadData.ClassTypeId,
        PdfName: UploadData.PdfName, veecAreaId: UploadData.veecAreaId, veecAreaName: UploadData.areaName,
        folderName: UploadData.folder, parentFolder: UploadData.parentFolder
    };
}

function showMessageForVeecPhoto(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegionVeecPhoto" : "successMsgRegionVeecPhoto";
    var inVisible = isError ? "successMsgRegionVeecPhoto" : "errorMsgRegionVeecPhoto";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}


$('#accordion8 .panel-heading').find('input[type=checkbox]').change(function () {
   
    if (!this.checked) {
        $(this).parent().next().find('[type=checkbox]').removeAttr('checked');
        //$(this).closest('.panel').find('.panel-body').find('[type=checkbox]').removeAttr('checked');
        //$(this).closest('[id*=pnlMain]').find('[id*=headingAssets]').find('[type=checkbox]').removeAttr('checked');
    }
    else {
            $(this).parent().next().find('[type=checkbox]').prop('checked', 'checked');
        }
});
$('#accordion8 .panel-body').find('input[type=checkbox]').change(function () {
    
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

