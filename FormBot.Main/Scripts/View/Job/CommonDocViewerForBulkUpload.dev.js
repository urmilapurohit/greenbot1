var undoData = [];
var clickSignId;
//'use strict';
var count = 0;


pdfItemsStrBulk = pdfItemsStrBulk.replace(/\\/g, '/').replace(/\/"/g, '\\"');
var pdfItemsBulk = [];
if (pdfItemsStrBulk)
    pdfItemsBulk = JSON.parse(pdfItemsStrBulk);
var pdfDoc = null,
    pageNum = 1,
    pageRendering = false,
    pageNumPending = null,
    pageLeftPending = 0,
    pageTopPending = 0,
    pdfPageWidth = 0,
    pdfPageHeight = 0,
    pdfRotate = 0,
    CurrentPageNo = 1;
var formFields = {};
var div = document.getElementById('viewerForBulk');

/**
* Asynchronously downloads PDF.
*/
function init(fileName) {
    $("#loading-image").show();
    var path = fileName;
    $.ajaxSetup({ cache: false });
    PDFJS.workerSrc = pdfWorkerJsPath //'../Scripts/pdf/pdf.worker.js';
    PDFJS.getDocument(path).then(function (pdfDoc_) {
        pdfDoc = pdfDoc_;
        // document.getElementById('page_count').textContent = pdfDoc.numPages;
        $("#total-page").text(pdfDoc.numPages);
        if (pdfDoc.numPages == 0)
            $("#loading-image").hide();
        $("#viewerForBulk").html('');
        if ($("#document-paging").val() == 0) {
            for (var i = 1; (pdfDoc.numPages == 1 ? i <= pdfDoc.numPages : i < pdfDoc.numPages); i++) {
                // Initial/first page rendering
                renderPage(i, 0, 0, true);
            }
        }
        else
            renderPage(CurrentPageNo, 0, 0, true);
        
    });
}

function removeOptions(selectbox) {
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}
function jobSignChange() {
    //var bol = $('#jSignature').jSignature("isModified")
    //if (bol) {
    //    if ($('#jSignature').find("#redoSignBtn").length == 0) {
            
    //        //$('#jSignature').append('<input type="button" class="redo" onclick=redoClick() id="redoSignBtn" value="redo last stroke" style="position: absolute; top: auto; margin: 0px !important; left: 136px;">');
    //    }
    //}
}

function resetClick() {
    $('#jSignature').jSignature('reset');
    $("#SendSignSmsFirstname").removeAttr('disabled');
    $("#SendSignSmsLastname").removeAttr('disabled');
    $("#SendSignSmsMobileNo").removeAttr('disabled');
    $("#SendSignSmsEmail").removeAttr('disabled');
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = false;
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
}
function redoClick() {
    var data;
    for (var j = 0; j < undoData.length; j++) {
        if (undoData[j].id == id) {
            data = undoData[j].value.pop();
            if (!(data == undefined)) {
                $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'].push(data);
            }
        }
    }
    if (!(data == undefined)) {
        $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
    }

}
function undoClick() {

    var Signdata = {};
    Signdata.value = [];
    var flag = false;
    var data = $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'].pop();
    for (var j = 0; j < undoData.length; j++) {
        if (undoData[j].id == id) {
            undoData[j].value.push(data);
            flag = true;
            break;
        }
    }
    if (!flag) {
        Signdata.id = id;
        Signdata.value.push(data);
        undoData.push(Signdata);
    }
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
}
function addSelectBoxForPageAcroFieldsToForm(page) {
    return;
    page.getAnnotations().then(
        function (items) {
            
            var selectField = document.getElementById("selectField");
            removeOptions(selectField);
            var optionsHTML = [];
            if (items.length == 0) {
                optionsHTML.push("<option value=\"console.log('no fields clicked - doing nothing');\">No fields</option>");
            }
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                switch (item.subtype) {
                    case 'Widget':

                        if (item.fieldType != 'Tx' && item.fieldType != 'Btn' && item.fieldType != 'Ch') {
                            break;
                        }
                        var fieldName;
                        if (item.fieldType == 'Tx') {
                            fieldName = 'Inputfield with key ' + item.fullName + ' (position: ' + item.rect + ')';
                        }
                        if (item.fieldType == 'Btn') {

                            if (item.fieldFlags & 32768) {
                                fieldName = 'Radiobutton with key ' + item.fullName + ' (position: ' + item.rect + ')';
                            } else if (item.fieldFlags & 65536) {
                                fieldName = 'Pushbutton with key ' + item.fullName + ' (position: ' + item.rect + ')';
                            } else {
                                fieldName = 'Checkbox with key ' + item.fullName + ' (position: ' + item.rect + ')';
                            }
                        }
                        if (item.fieldType == 'Ch') {
                            fieldName = 'Selectbox with key ' + item.fullName + ' (position: ' + item.rect + ')';
                        }

                        x = item.rect[0];
                        if (pdfRotate == 90) {
                            y = pdfPageWidth - item.rect[1];
                        }
                        else if (pdfRotate == 0) {
                            y = pdfPageHeight - item.rect[1];
                        }
                        else {
                            //TODO: other rotates
                        }
                        optionsHTML.push("<option value=\"queueRenderPage(" + pageNum + "," + x + "," + y + ");\">" + fieldName + "</option>");
                }
            }
            selectField.innerHTML = optionsHTML.join('\n');
        });
}

function renderPage(pageNum, xOffset, yOffset, renderAcroFieldSelect) {
    pageRendering = true;
    // Using promise to fetch the page
    
    pdfDoc.getPage(pageNum).then(function (page) {
        var scale = 1.5;
        var viewport = page.getViewport(scale);
        var pageDisplayWidth = viewport.width;
        var pageDisplayHeight = viewport.height;
        var pageDivHolder = document.createElement('div');
        pageDivHolder.className = 'pdfpage';
        pageDivHolder.setAttribute("pagenumber", pageNum);
        pageDivHolder.style.width = (pageDisplayWidth + 1) + 'px';
        pageDivHolder.style.height = (pageDisplayHeight + 5) + 'px';
        div.appendChild(pageDivHolder);

        // Prepare canvas using PDF page dimensions
        var canvas = document.createElement('canvas');
        var context = canvas.getContext('2d');
        canvas.width = pageDisplayWidth;
        canvas.height = pageDisplayHeight;
        pageDivHolder.appendChild(canvas);

        $('#popupViewer').find('.vertical-align-center').css('width', (pageDisplayWidth + 30) + 'px');

        // Render PDF page into canvas context
        var renderContext = {
            canvasContext: context,
            viewport: viewport
        };
        var renderTask = page.render(renderContext);

        // Wait for rendering to finish
        renderTask.promise.then(function () {
                if ($("#document-paging").val() == 0) {
                if (renderAcroFieldSelect) {
                    addSelectBoxForPageAcroFieldsToForm(page);
                }
                pageRendering = false;
                if (pageNumPending !== null) {
                    // New page rendering is pending
                    renderPage(pageNumPending, pageLeftPending, pageTopPending);
                    pageNumPending = null;
                    pageLeftPending = 0;
                    pageTopPending = 0;
                }
            }
        });
        // Prepare and populate form elements layer
        var formDiv = document.createElement('div');
        pageDivHolder.appendChild(formDiv);
        setupForm(formDiv, page, viewport);
        // Update page counters



        if (pageNum >= pdfDoc.numPages) {
            //document.getElementById('page_num').textContent = 1;
            $("#loading-image").hide();
            return;
        }
        if ($("#document-paging").val() == 1) {
            $("#loading-image").hide();

            $(".left-document-section").getNiceScroll().resize();
        }
        pageNum++;
        queueRenderPage(pageNum, 0, 0, true);

    });
}


/**
 * If User not Select job Field then always its
 * Generate new Fieldname For Pdf Item
 */
var fieldNames = [];
function generateSignatureId() {
    var i = 1
    $('.jSignature').each(function () {

        if ($(this).parent().attr('id').includes(i)) {
            i++;
        }
    });
    return i;
}
function setupForm(div, content, viewport) {
    function bindInputItem(input, item) {
        if (input.name in formFields) {
            var value = formFields[input.name];
            if (input.type == 'checkbox') {
                input.checked = value;
            }
            else if (this.type == 'radio') { input.checked = value; }
            else if (!input.type || input.type == 'text') {
                input.value = value;
            }
        }
        input.onchange = function pageViewSetupInputOnBlur() {
            if (input.type == 'checkbox') {
                formFields[input.name] = input.checked;
            }
            else if (this.type == 'radio') { formFields[input.name] = input.checked; }
            else if (!input.type || input.type == 'text') {
                formFields[input.name] = input.value;
            }
        };
    }
    function createElementWithStyle(tagName, item) {
        var element = document.createElement(tagName);
        var rect = PDFJS.Util.normalizeRect(
          viewport.convertToViewportRectangle(item.rect));
        element.style.left = Math.floor(rect[0]) + 'px';
        element.style.top = Math.floor(rect[1] + viewport.offsetY) + 'px';
        element.style.width = Math.ceil(rect[2] - rect[0]) + 'px';
        element.style.height = Math.ceil(rect[3] - rect[1]) + 'px';
        return element;
    }
    function assignFontStyle(element, item) {
        var fontStyles = '';
        if ('fontSize' in item) {
            fontStyles += 'font-size: ' + Math.round(item.fontSize *
                                                     viewport.fontScale) + 'px;';
        }
        switch (item.textAlignment) {
            case 0:
                fontStyles += 'text-align: left;';
                break;
            case 1:
                fontStyles += 'text-align: center;';
                break;
            case 2:
                fontStyles += 'text-align: right;';
                break;
        }
        element.setAttribute('style', element.getAttribute('style') + fontStyles);
    }
    content.getAnnotations().then(function (items) {

        for (var i = 0; i < items.length; i++) {
            if (fieldNames.indexOf(items[i].fullName[0]) == -1) {
                fieldNames.push(items[i].fullName[0]);
            }
            else
                continue;

            var item = items[i];
            switch (item.subtype) {
                case 'Widget':
                    if (item.fieldType != 'Tx' && item.fieldType != 'Btn' &&
                        item.fieldType != 'Ch') {
                        break;
                    }

                    var inputDiv = createElementWithStyle('div', item);
                    if (!item.fieldValue.includes("jsignature") || item.fieldValue.includes("image")) {
                        inputDiv.className = 'inputHint';
                    }
                    else {
                        inputDiv.className = 'inputHint sign';
                    }
                    var j = generateSignatureId();
                    //if ($("#PageFrom").val() == "SignatureRequest" && (!item.fieldValue.includes("jsignature") || item.fieldValue.includes("image"))) {
                        div.appendChild(inputDiv);
                    //}
                    var input;

                    //item.flags = getInheritableProperty(annotation, 'Ff') || 0;
                    if (item.fieldType == 'Tx') {
                        if (item.fieldValue.includes("jsignature") || item.fieldValue.includes("image")) {
                            //if ($("#PageFrom").val() == "SignatureRequest") {
                                input = createElementWithStyle('div', item);
                                input.id = "jSignature" + j;
                                input.style = "border: 1px dotted rgb(0, 0, 0) !important;opacity:0.8;";
                                input.setAttribute("name", item.fullName);
                            //}

                        } else {
                            input = createElementWithStyle('input', item);
                        }
                    }
                    else if (item.fieldFlags & 65537) {// && ($("#PageFrom").val() == "SignatureRequest")) {
                        input = createElementWithStyle('div', item);
                        input.id = "jSignature" + j;
                        input.style = "border: 1px dotted rgb(0, 0, 0) !important;opacity:0.8;";
                        input.setAttribute("name", item.fullName);
                        input.setAttribute("onclick", "signatureChange(this)");
                    }   //ashish
                    else if (item.fieldType == 'Btn') {
                        input = createElementWithStyle('input', item);
                        if (item.fieldFlags & 32768) {
                            input.type = 'radio';
                            // radio button is not supported
                        } else if (item.fieldFlags & 65536) {
                            input.type = 'button';
                            // pushbutton is not supported
                        } else {
                            input.type = 'checkbox';
                        }
                    }
                    if (item.fieldType == 'Ch') {
                        input = createElementWithStyle('select', item);
                        // select box is not supported
                    }

                    if ((input.type == 'radio' || input.type == 'checkbox') && item.fullName) {
                        $.each(item.fullName, function (i) {
                            if (i == (item.fullName.length - 1) && validation.isGlyphsNumber(this)) {
                            }
                            else
                                input.name += ((i > 0) ? "." : "") + this;
                        });

                        //for (var i = 0; i < item.fullName.length-1 ; i++)
                        //{

                        //}
                    }
                    else
                        input.name = item.fullName[0];

                    if (item.fieldType == "Btn" && item.fieldFlags) {
                        //if ($("#PageFrom").val() == "SignatureRequest" && (!item.fieldValue.includes("jsignature") || item.fieldValue.includes("image"))) {
                            inputDiv.appendChild(input);
                        //}
                        //if ($("#PageFrom").val() == "SignatureRequest") {
                            $('#jSignature' + j).jSignature({ 'lineWidth': 2 });//ashish
                        //}
                    } else {
                        input.title = item.alternativeText;
                        input.className = 'inputControl';
                        assignFontStyle(input, item);
                        bindInputItem(input, item);
                        div.appendChild(input);
                    }

                    break;
            }
        }
        setInputValuesAtInit();
    });
}
/**
 * If another page rendering in progress, waits until the rendering is
 * finised. Otherwise, executes rendering immediately.
 */
function queueRenderPage(num, xOffset, yOffset) {
    if (pageRendering) {
        pageNumPending = num;
        pageLeftPending = xOffset;
        pageTopPending = yOffset;
    } else {
        renderPage(num, xOffset, yOffset, false);
    }
}

//This Function is used to open popup when click on signature 
function signatureChange(obj) {
    clickSignId = $(obj).attr('id');
    var lineWidth = $(obj).attr('lineWidth');
    var sign = $("#" + clickSignId).jSignature('getData', 'base30');
    $('#popupBulkUploadSignature').modal({ backdrop: 'static', keyboard: false });
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = lineWidth;
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['signatureLine'] = true;
    $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])

    if (sign[1] == "")
    {
        $('#jSignature').jSignature('reset');
    } else
    {
        $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
        $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
        $('#jSignature').jSignature('setData', 'data:' + sign.join(","));
    }
    $('#captureSignDetail').hide();
    
}

$('#popupSignature').off().on('shown.bs.modal', function () {

    $.ajax({
        url: getCaptureSignDetail,
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ jobDocId: $('#jobDocId').val(), fieldname: $('#' + clickSignId).attr('name') }),
        success: function (data) {
            if (data.status) {
                if ($('#' + clickSignId).parent().css('opacity') == 0) {
                    $('[name="signType"][value="M"]').prop('checked', true);
                    $('#popupSignature .modal-body').css('height', '400px');
                    $('[name="signType"][value="D"]').prop('disabled', true);
                    $('[name="signType"]').parent().parent().parent().parent().prev().find('h4').html('Send SMS');
                    $('#mobileNumberForSms').show();
                    $('#btnSaveSign').hide();
                    $("#btnSendSignatureUrl").show();
                    //$('#btnSaveSign').val('Send')
                    $('#jSignature').hide();
                }
                else {
                    $('[name="signType"][value="D"]').prop('checked', true)
                    $('#popupSignature .modal-body').css('height', '600px');
                    $('[name="signType"][value="D"]').prop('disabled', false)
                    $('[name="signType"]').parent().parent().parent().parent().prev().find('h4').html('Draw Signature');
                    $('#mobileNumberForSms').hide();
                    $('#btnSaveSign').show();
                    $("#btnSendSignatureUrl").hide();
                    //$('#btnSaveSign').val('Save')
                    $('#jSignature').show();
                }
                if ($('#jSignature .jSignature').length > 1) {
                    $('#jSignature .jSignature').hide()
                    $($('#jSignature .jSignature')[0]).show()
                }
                
                if (data.isData)
                {
                    $('#captureSignDetail').show();
                    $("#captureSignDetail").css('margin-top', "15px");
                    $("#signCaptureName").text(data.firstname + " " + data.lastname);
                    $("#signCaptureMobile").text(data.mobilenumber);
                    $('#signCaptureEmail').text(data.email);
                    $('#SendSignSmsFirstname').val(data.firstname)
                    $("#SendSignSmsFirstname").attr("disabled", "disabled");
                    $('#SendSignSmsLastname').val(data.lastname)
                    $("#SendSignSmsLastname").attr("disabled", "disabled");
                    $('#SendSignSmsMobileNo').val(data.mobilenumber)
                    $("#SendSignSmsMobileNo").attr("disabled", "disabled");
                    $('#SendSignSmsEmail').val(data.email)
                    $("#SendSignSmsEmail").attr("disabled", "disabled");
                } 
                else
                {
                    $('#captureSignDetail').hide();
                    $("#SendSignSmsFirstname").val('');
                    $("#SendSignSmsLastname").val('');
                    $("#SendSignSmsMobileNo").val('');
                    $("#SendSignSmsEmail").val('');
                    resetClick();
                }
                var widthVal = $('#jSignature .jSignature').width();
                $('#jSignature .jSignature').height(220);
                $('#jSignature .jSignature').width(widthVal);
                $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").$controlbarLower.css('margin-bottom', '5.0em')
                $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = 2;
                $('#jSignature').find('canvas').attr('width', widthVal + 'px !important');
                $('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#jSignature').find("canvas.jSignature").add($('#jSignature').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                $('#jSignature').append('<input type="button" class="reset" onclick=resetClick() id="resetBtn" value="Clear Signature" style="position: absolute; top: auto; margin: 0px !important; left: 10px;">');
            }          
        }
    });
    
})




function getInheritableProperty(annotation, name) {
    var item = annotation;
    while (item && !item.has(name)) {
        item = item.get('Parent');
    }
    if (!item)
        return null;
    return item.get(name);
}
$(document).ready(function myfunction() {
    
    $('[name="signType"]').change(function () {
        
        var type = $(this).val();
        if (type == 'D') {
            $(this).parent().parent().parent().parent().prev().find('h4').html('Draw Signature');
            $('#mobileNumberForSms').hide();
            $('#jSignature').show();
            //$('#btnSaveSign').val('Save')
            $('#btnSaveSign').show();
            $("#btnSendSignatureUrl").hide();
            $('#popupSignature .modal-body').css('height', '600px');
        } else {
            $(this).parent().parent().parent().prev().find('h4').html('Send SMS');
            $('#jSignature').hide();
            $('#mobileNumberForSms').show();
            //$('#btnSaveSign').val('Send')
            $('#btnSaveSign').hide();
            $("#btnSendSignatureUrl").show();
            $('#popupSignature .modal-body').css('height', '400px');
            $('#popupSignature .modal-body').css('height', '400px');
        }
        
    })
    var base64;
    $('#btnSave').click(function () {
        $.each(pdfItemsBulk, function (k, data) {
            if ($('input[name="' + data.FieldName + '"]') || $('div[name="' + data.FieldName + '"]').length > 0) {
                if (data.Type.toString() == texthashCode || data.Type.toString() == buttonhashCode) {
                    if (data.Value.includes("base64")) {

                    }
                    else if (data.Value.includes("jsignature") ) {
                        datapair = $('div[name="' + data.FieldName + '"]').jSignature("getData", "base30")
                        data.Value = datapair.join(",")
                        data.Base64 = $('div[name="' + data.FieldName + '"]').jSignature("getData", "image").join(",")
                        
                    }
                    else if (data.IsTextArea) {
                        data.Value = $.trim($('textarea[name="' + data.FieldName + '"]').val());
                    }else {
                        data.Value = $.trim($('input[name="' + data.FieldName + '"]').val());
                    }

                }
                else if (data.Type.toString() == radiohashCode) {
                    data.Value = $('input[name="' + data.FieldName + '"]:checked').val() ? $('input[name="' + data.FieldName + '"]:checked').val() : '';
                }
                else if (data.Type.toString() == checkhashCode) {
                    //if ($('input[name="' + data.FieldName + '"]').prop('checked'))
                    //    data.Value = $('input[name="' + data.FieldName + '"]').val();
                    //else
                    //    data.Value = "";
                    //data.Value = $('input[name="' + data.FieldName + '"]:checked').val() ? $('input[name="' + data.FieldName + '"]:checked').val() : '';
                    data.Value = $('input[name="' + data.FieldName + '"]').val() ? $('input[name="' + data.FieldName + '"]').val() : '';
                }
            }
        });

        var obj = {};
        obj.Data = JSON.stringify(pdfItemsBulk);
        obj.PDFURL = $('#PDFURL').val();
        obj.PDFSource = $('#PDFSource').val();
        obj.JobId = $('#JobId').val();
        obj.DocId = $('#DocId').val();
        obj.JobDocId = $("#jobDocId").val();
        obj.Base64 = base64;
        var data = JSON.stringify({ documentCollectionView: obj, lstCaptureUserSign: signDetail });
        $.ajax({
            url: urlSaveDocument,
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8', // Not to set any content header
            processData: false, // Not to process data
            data: data,
            success: function (result) {
                showMessage1("Document has been saved successfully.", false);
                signDetail.map(signature => signature.IsSave = true);
                SearchHistory();
            },
            error: function (result) {
                showMessage1("Document has not been saved.", true);
            }

        });
        return false;
        // $('#pdfData').val(JSON.stringify(pdfItems));
    });
    
});

var validation = {
    isGlyphsNumber: function (str) {
        var pattern = /^`+\d+$/;
        return pattern.test(str);  // returns a boolean
    }
};
 

//Own Function to set input values at Initialization time.
var setInputValuesAtInit = function () {
    count++;
    if (count == pdfDoc.numPages || $("#document-paging").val() == 1) {
        $.each(pdfItemsBulk, function (k, data) {

        try {
            if (data.Type.toString() == texthashCode || data.Type.toString() == buttonhashCode) {
                if (data.Value.includes('base64')) {
                        $('div[name="' + data.FieldName + '"]').parent().css('opacity', '0');
                }
                if (data.Value.includes("jsignature")) {
                    if ($('div[name="' + data.FieldName + '"]').length > 0) {

                        for (i = 0; i < $('div[name="' + data.FieldName + '"]').length; i++) {
                            var div = $($('div[name="' + data.FieldName + '"]')[i]);
                            if (data.PdfItemProperties.BackgroundColor != null && data.PdfItemProperties.BackgroundColor != 'rgb(203, 247, 203)') {
                                div.css('background-color', data.PdfItemProperties.BackgroundColor)
                            } else {
                                div.css('background-color', '#dde4ff')
                            }
                            if (data.Value.split(',')[1] == "") {

                                div.append("<div style=\"display:table;width:100%;height:100%\"><p id=\"noSigntext\">Click Here To Sign </p>");
                                div.find("canvas.jSignature").hide();

                            }
                            else {
                                var height = div.height() / 220;
                                var width = div.width() / 568;
                                document.getElementById(div.attr('id')).children[1].getContext('2d').scale(width, height);

                                div.attr('scale', width + ',' + height);
                                div.attr('lineWidth', data.lineWidth);
                                div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = 4;
                                div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
                                div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas(div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                                div.jSignature("setData", "data:" + data.Value);
                            }
                        }
                    }
                } else {
                    if (data.ReadOnly) {

                        $('input[name="' + data.FieldName + '"]').hide()
                        $('input[name="' + data.FieldName + '"]').prev().hide()
                    }
                    $('input[name="' + data.FieldName + '"]').val(data.Value.replace(/\/"/g, '"'));
                    if (data.PdfItemProperties.TextColor != null) {
                        $('input[name="' + data.FieldName + '"]').css('color', data.PdfItemProperties.TextColor)
                    } else {
                        $('input[name="' + data.FieldName + '"]').css('color', 'black')
                    }
                    if (data.PdfItemProperties.BackgroundColor != null && data.PdfItemProperties.BackgroundColor != 'rgb(203, 247, 203)') {
                        $('input[name="' + data.FieldName + '"]').css('background-color', data.PdfItemProperties.BackgroundColor)
                    } else {
                        $('input[name="' + data.FieldName + '"]').css('background-color', '#dde4ff')
                    }
                    if (data.PdfItemProperties.BorderColor != null) {
                        $('input[name="' + data.FieldName + '"]').css('border', data.PdfItemProperties.BorderColor)
                    }
                    if (data.PdfItemProperties.Alignment != null) {
                        $('input[name="' + data.FieldName + '"]').css('text-align', data.PdfItemProperties.Alignment)
                    }
                    if (data.PdfItemProperties.FontSize != null) {
                        $('input[name="' + data.FieldName + '"]').css('font-size', data.PdfItemProperties.FontSize + "px");
                    }
                    if (data.PdfItemProperties.Bold) {
                        $('input[name="' + data.FieldName + '"]').css('font-weight', "bold");
                    }
                    if (data.PdfItemProperties.Italic) {
                        $('input[name="' + data.FieldName + '"]').css('font-style', "italic");
                    }
                    if (data.PdfItemProperties.FontName) {
                        $('input[name="' + data.FieldName + '"]').css('font-family', data.PdfItemProperties.FontName);
                    }
                    
                    data.PdfItemProperties && data.PdfItemProperties.MaxLength && $('input[fieldname="' + data.FieldName + '"]').attr('maxLength', data.PdfItemProperties.MaxLength);
                }
            }
            else if (data.Type.toString() == radiohashCode) {
                var values = data.AvailableValues;
                values = $.grep(values, function (e, index) {
                    return e != null ? e.toLowerCase() != 'off' : '';
                });
                $.each($('input[name="' + data.FieldName + '"]'), function (i) {
                    this.value = ((values.length - (i + 1)) > -1 && values.length - (i + 1) < values.length) ? values[values.length - (i + 1)] : '';
                });

                $('input[name="' + data.FieldName + '"][value="' + data.Value + '"]').prop('checked', true);
            } else if (data.Type.toString() == checkhashCode) {
                if (data.FieldName.includes("IsSameAsOwnerAddress")) {
                    if (data.Value == "True") {
                        $('input[name="' + data.FieldName + '"]').prop('checked', true);
                    } else {
                        $('input[name="' + data.FieldName + '"]').prop('checked', false);
                    }
                } else {
                    var values = data.AvailableValues;
                    if ($('input[name="' + data.FieldName + '"]').length == 1) {
                        $.each($('input[name="' + data.FieldName + '"]'), function (i) {
                            var len = $('[id^=' + data.FieldName + '_]').length
                            //$(this).attr("id", data.FieldName + "_" + (len + 1))
                            $(this).attr("id", data.FieldName)
                            this.value = values[0];
                            //if (data.Value == this.value)
                            //    $(this).prop('checked', true);
                            if (data.Value == "on" || data.Value == values[0]) {
                                $(this).prop('checked', true);
                                $(this).val('on');
                            }
                            else {
                                $(this).prop('checked', false);
                                $(this).val('');
                            }
                        });
                    }
                    else {
                        $.each($('input[name="' + data.FieldName + '"]'), function (i) {
                            var len = $('[id^=' + data.FieldName + '_]').length
                            //$(this).attr("id", data.FieldName + "_" + (len + 1))
                            $(this).attr("id", data.FieldName)
                            this.value = values[values.length - (i + 1)];
                            if (data.Value == this.value)
                                $(this).prop('checked', true);
                        });
                    }
                }
            }
        }
        catch (err) { }
    });
    $('input[type="checkbox"]').click(function () {
        //if ($(this).prop('checked'))
        //    $('input[name="' + $(this).attr('name') + '"]').not($(this)).prop('checked', false);
        if ($(this).prop('checked')) {
            $('input[name="' + $(this).attr('name') + '"]').val('on');
            $('input[name="' + $(this).attr('name') + '"]').prop('checked', true);
        }
        else {
            $('input[name="' + $(this).attr('name') + '"]').val('');
            $('input[name="' + $(this).attr('name') + '"]').prop('checked', false);
        }
    });
    }

    $("input[type=checkbox]").each(function () {
        var $lbl = $("<label>");
        $lbl.attr("for", this.id);
        $lbl.attr("style", $(this).attr("style"));
        $lbl.css("position", "absolute");
        $(this).after($lbl);
    });
    $(".left-document-section").getNiceScroll().resize();
    //OutlinerField();
    disabledPageIcon(1);
    $("#document-paging").hide();
};

function showMessage1(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegion1" : "successMsgRegion1";
    var inVisible = isError ? "successMsgRegion1" : "errorMsgRegion1";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}
function showMessage2(msg, isError) {
    $(".alert").hide();
    var visible = isError ? "errorMsgRegion2" : "successMsgRegion2";
    var inVisible = isError ? "successMsgRegion2" : "errorMsgRegion2";
    $("#" + inVisible).hide();
    $("#" + visible).html(closeButton + msg);
    $("#" + visible).show();
}

$('.modal').on('hidden.bs.modal', function (e) {
    if ($('.modal').hasClass('in')) {
        $('body').addClass('modal-open');
    }
});

//#region Paging Section

$("#document-paging").change(function () {
    if ($(this).val() == 0) {
        $(".page-icon").hide();
    } else {
        disabledPageIcon(1);
    }
    // $("#the-svg").find('.svg').remove();
    //$(".added").remove();
    count = 0;
    init($("#PDFURL").val() + "?h=" + GetDateTimeTick());
    $("#current-page").val(1);
    setTimeout(function () {
        ReloadBodySection();
        OutlinerField();
    }, 500);
});
$(".document-load-section .left-document-section").scroll(function () {
    if ($("#document-paging").val() == 0) {
        setPageNumber();
        disabledPageIcon(parseInt($("#current-page").val()));
    }
});

$("#next-page").click(function () {
    var currentpge = parseInt($("#current-page").val());
    if (TotalCount != currentpge) {
        disabledPageIcon(currentpge + 1);
        CurrentPageNo = currentpge + 1;
        $("#current-page").val(currentpge + 1);
        $("#current-page").focusout();
    }
    //$("[pagenumber='" + currentpge + "']").remove();
    //renderPage(currentpge + 1, 0, 0, true, pdfDoc.numPages);
    //init($("#PDFURL").val() + "?h=" + GetDateTimeTick());
    //setTimeout(function () {
    //    ReloadBodySection();
    //    OutlinerField();
    //}, 500);
});
$("#previous-page").click(function () {
    var currentpge = parseInt($("#current-page").val());
    if (1 < currentpge) {
        disabledPageIcon(currentpge - 1);
        //$("[pagenumber='" + currentpge + "']").remove();
        //renderPage(currentpge - 1, 0, 0, true, pdfDoc.numPages);
        CurrentPageNo = currentpge - 1;
        //init($("#PDFURL").val() + "?h=" + GetDateTimeTick());
        $("#current-page").val(currentpge - 1);
        $("#current-page").focusout();
    }
    //setTimeout(function () {
    //    ReloadBodySection();
    //    OutlinerField();
    //}, 500);
});
$("#current-page").keydown(function (event) {
    if (event.which == "13")
        $("#current-page").focusout();
});
var pageNumber;
$("#current-page").focus(function () {
    pageNumber = $(this).val();
});
$("#current-page").focusout(function () {
    var currentPage1 = parseInt($("#current-page").val());
    if (currentPage1 > 0) {
        if ($("#current-page").val() <= pdfDoc.numPages) {
            if ($("#document-paging").val() == 0) {
                var headerHeight = $("#header").innerHeight() + $(".title").innerHeight() + 20 + 10;
                var offset = $($("#viewerForBulk").find('.pdfpage')[$("#current-page").val() - 1]).offset().top - headerHeight;
                var scroll = $('.document-load-section .left-document-section').scrollTop();
                if (offset > window.innerHeight) {
                    // Not in view so scroll to it
                    $('.document-load-section .left-document-section').animate({ scrollTop: (offset + scroll) }, 100);
                }
                if (offset < 0) {

                    $('.document-load-section .left-document-section').animate({ scrollTop: Math.abs(offset + scroll) }, 100);
                }
            } else {
                //$("#the-svg").find('.svg').remove();
                //$(".added").remove();
                var currentPage = parseInt($("#current-page").val());
                CurrentPageNo = currentPage;
                init($("#PDFURL").val() + "?h=" + GetDateTimeTick());
                //renderPage(currentpge, 0, 0, true, pdfDoc.numPages);
                disabledPageIcon(currentPage);
                setTimeout(function () {
                    ReloadBodySection();
                    OutlinerField();
                }, 500);
            }
        } else {
            $("#current-page").val(pageNumber);
        }

        if ($("#current-page").val() < 1) {
            $("#current-page").val(1);
        }
        else if ($("#current-page").val() > pdfDoc.numPages) {
            $("#current-page").val(pdfDoc.numPages);
        }
    }
    disabledPageIcon(currentPage1);
});
function disabledPageIcon(currentpge) {
    if (currentpge < 1) {
        currentpge = 1;
        $("#current-page").val(1);
    }
    if (currentpge > 1) {
        $("#previous-page").show();
    } else {
        $("#previous-page").hide();
    }
    if (currentpge >= pdfDoc.numPages) {
        $("#next-page").hide();
    } else {
        $("#next-page").show();
    }
}
//var scale = 2;
//function renderPage(pageNum, totalPage) {
//    $("#loading-image").show();
//    pageRendering = true;

//    // Using promise to fetch the page

//    pdfDoc.getPage(pageNum)
//        .then(function (page) {
//            // Get viewport (dimensions)
//            //var viewport = page.getViewport($('#the-svg').width() / page.getViewport(1.0).width);

//            var viewport = page.getViewport(scale);

//            // Get div#the-svg
//            //var container = document.getElementById('the-svg');

//            // Set dimensions
//           // container.style.width = viewport.width + 'px';
//            //container.style.height = viewport.height + 'px';

//            // SVG rendering by PDF.js
//            var operatorList = page.getOperatorList()
//                .then(function (opList) {
//                    var svgGfx = new PDFJS.SVGGraphics(page.commonObjs, page.objs);
//                    return svgGfx.getSVG(opList, viewport);
//                })
//                .then(function (svg) {
//                    $(svg).attr('class', 'svg');
//                    $(svg).attr('pagenumber', pageNum);
//                    container.appendChild(svg);
//                    offsetY = $(svg).offset().top - $('#the-svg').offset().top;
//                    viewport.offsetY = offsetY;
//                    container.style.height = offsetY + $(svg).height() + 'px';
//                    //setupForm(page, viewport, --pageNum);
//                    setupForm(page, viewport, pageNum);
//                    //applyDrop(container);
//                    pageNum++;
//                    if ($("#document-paging").val() == 0) {
//                        if (pageNum <= totalPage) {
//                            renderPage1(pageNum, totalPage);
//                        } else {
//                            $("#loading-image").hide();
//                        }
//                    } else {
//                        $("#loading-image").hide();
//                    }

//                });


//            //pageNum++;
//        });

//}
function setPageNumber() {
    //$("#the-svg").find('.svg').each(function () {
    //    if ((($(this).position().top - $(".doc-editor-header").innerHeight() - $(".upper-toolbar").innerHeight() - $("#header").height() - 30) <= 0)) {
    //        $("#current-page").val($(this).attr("pagenumber"));
    //    }
    //});
    $("#viewerForBulk").find('.pdfpage').each(function () {
        if ((($(this).position().top - $("#header").innerHeight() - $(".title").innerHeight() - 20 - 10) <= 0)) {
            $("#current-page").val($(this).attr("pagenumber"));
        }
    });
}
//function init1(fileName) {
//    //$("#loading-image").show();
//    var path = fileName;
//    $.ajaxSetup({ cache: false });
//    PDFJS.workerSrc = pdfWorkerJsUrl; //'../Scripts/pdf/pdf.worker.js';
//    PDFJS.getDocument(path).then(function (pdfDoc_) {
//        pdfDoc = pdfDoc_;
//        $("#total-page").text(pdfDoc.numPages);
//        if (pdfDoc.numPages == 0)
//            $("#loading-image").hide();
//        renderPage(1, pdfDoc.numPages);
//    });
//}
function GetDateTimeTick() {
    var yourDate = new Date();  // for example
    // the number of .net ticks at the unix epoch
    var epochTicks = 621355968000000000;
    // there are 10000 .net ticks per millisecond
    var ticksPerMillisecond = 10000;
    // calculate the total number of .net ticks for your date
    return (epochTicks + (yourDate.getTime() * ticksPerMillisecond));
}
//#endregion


//function setOutliner(anchor) {
//    //$(".items").click(function () {
//    $(anchor).closest('div').find('.active').removeClass('active');
//    $(anchor).addClass('active');
//    if (oldSelect != "") {
//        for (i = 0; i < selectedDiv.length; i++) {
//            if ($(selectedDiv[i]).attr("fieldname") == oldSelect) {
//                selectedDiv.splice(i, 1);
//            }
//        }
//        $("[fieldname='" + oldSelect + "']").closest('div').removeClass("blueBorder")
//    }
//    if ($(anchor).attr('id') != 'select') {
//        flagOutliner = false;
//        var fieldname = $(anchor).attr('id');
//        if (fieldname.indexOf("Textbox_") != -1) {
//            fieldname = fieldname.replace("Textbox", "undefined")
//        }
//        if (fieldname.indexOf("Textarea_") != -1) {
//            fieldname = fieldname.replace("Textarea", "undefined")
//        }
//        if (fieldname.indexOf("Signature_") != -1) {
//            fieldname = fieldname.replace("Signature", "undefined")
//        }
//        if (fieldname.indexOf("Checkbox_") != -1) {
//            fieldname = fieldname.replace("Checkbox", "undefined")
//        }
//        if (fieldname.indexOf("Image_") != -1) {
//            fieldname = fieldname.replace("Image", "undefined")
//        }
//        if ($("#document-paging").val() == 1) {
//            if ($("[fieldname='" + fieldname + "']").length == 0) {
//                OutlinerNextPage(fieldname)


//            } else {
//                OutlinerField(fieldname, oldSelect)
//            }
//        } else {
//            OutlinerField(fieldname, oldSelect)
//        }

//        oldSelect = fieldname

//    } else {
//        oldSelect = "";
//    }
//}
//function OutlinerNextPage(fieldname) {
//    if ($("#the-svg .svg").is(':visible')) {
//        if ($("#current-page").val() == pdfDoc.numPages && $("[fieldname='" + fieldname + "']").length == 0) {
//            OutlinerPreviousPage(fieldname)
//            return;
//        }
//        else if ($("[fieldname='" + fieldname + "']").length != 0) {
//            OutlinerField(fieldname)
//            return;
//        }
//        for (i = parseInt($("#current-page").val()); i <= pdfDoc.numPages; i++) {
//            if ($("[fieldname='" + fieldname + "']").length == 0) {
//                $("#next-page").click();
//                setTimeout(function () { OutlinerNextPage(fieldname) }, 200);
//            } else {

//                OutlinerField(fieldname)
//            }
//            break;
//        }
//    }
//    else {
//        setTimeout(function () { OutlinerNextPage(fieldname) }, 200);
//    }
//}