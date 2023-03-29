var undoData = [];
var clickSignId;
//'use strict';
var count = 0;


pdfItemsStr = pdfItemsStr.replace(/\\/g, '/');
var pdfItems = [];
if (pdfItemsStr)
    pdfItems = JSON.parse(pdfItemsStr);
var pdfDoc = null,
    pageNum = 1,
    pageRendering = false,
    pageNumPending = null,
    pageLeftPending = 0,
    pageTopPending = 0,
    pdfPageWidth = 0,
    pdfPageHeight = 0,
    pdfRotate = 0;
var formFields = {};
var div = document.getElementById('viewer');

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
        if (pdfDoc.numPages == 0)
            $("#loading-image").hide();
        for (var i = 1; (pdfDoc.numPages == 1 ? i <= pdfDoc.numPages : i < pdfDoc.numPages); i++) {
            // Initial/first page rendering
            renderPage(i, 0, 0, true);
        }
    });
}

function removeOptions(selectbox) {
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
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

var screenWidth = $(window).width();
var screenHeight = $(window).height();
var maxwidth = 0;
var finalWidth = 0;
var finalHeight = 0;

function renderPage(pageNum, xOffset, yOffset, renderAcroFieldSelect) {
    pageRendering = true;
    // Using promise to fetch the page
    pdfDoc.getPage(pageNum).then(function (page) {
        var scale = 1.5;
        var viewport = page.getViewport(scale);
        var pageDisplayWidth = viewport.width;
        var pageDisplayHeight = viewport.height;

        if (maxwidth < viewport.width)
            maxwidth = viewport.width;

        var pageDivHolder = document.createElement('div');
        pageDivHolder.className = 'pdfpage';
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
        });
        // Prepare and populate form elements layer
        var formDiv = document.createElement('div');
        pageDivHolder.appendChild(formDiv);
        setupForm(formDiv, page, viewport);
        // Update page counters
        if (screenWidth > 900) {
            if (finalWidth > 900) {
                finalWidth = 900;
            }
            else {
                if (finalWidth < pageDisplayWidth)
                    finalWidth = pageDisplayWidth;
            }
        }
        else {
            finalWidth = 800;
        }

        finalHeight = screenHeight - 64;

        //$("#popupViewerDoc").find('.modal-content').width(pageDisplayWidth);

        $("#popupDocPreview").find('.modal-dialog').width(finalWidth);
        $("#popupDocPreview").find('.modal-dialog').height(finalHeight);
        $("#popupDocPreview").find('.modal-dialog').css("overflow-x", 'scroll');
        $("#popupDocPreview").find('.modal-dialog').css("overflow-y", 'scroll');

        //$("#popupDocPreview").find('.modal-content').width(maxwidth);

        if (pageNum >= pdfDoc.numPages) {
            //document.getElementById('page_num').textContent = 1;
            $("#loading-image").hide();

            return;
        }
        pageNum++;
        queueRenderPage(pageNum, 0, 0, true);

    });
}


/**
 * If User not Select job Field then always its
 * Generate new Fieldname For Pdf Item
 */
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
                    div.appendChild(inputDiv);
                    var input;
                    //item.flags = getInheritableProperty(annotation, 'Ff') || 0;
                    if (item.fieldType == 'Tx') {
                        if (item.fieldValue.includes("jsignature") || item.fieldValue.includes("image")) {
                            input = createElementWithStyle('div', item);
                            input.id = "jSignature" + j;
                            input.style = "border: 1px dotted rgb(0, 0, 0) !important;opacity:0.8;";
                            input.setAttribute("name", item.fullName)

                        } else if (item.fieldFlags == 4096) {
                            input = createElementWithStyle('textarea', item);
                        }
                        else {
                            input = createElementWithStyle('input', item);
                        }
                    }
                    else if (item.fieldFlags & 65537) {
                        input = createElementWithStyle('div', item);
                        input.id = "jSignature" + j;
                        input.style = "border: 1px dotted rgb(0, 0, 0) !important;opacity:0.8;";
                        input.setAttribute("name", item.fullName)
                        input.setAttribute("onclick", "signatureChange(this)")
                    }
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
                        inputDiv.appendChild(input);
                        $('#jSignature' + j).jSignature({ 'lineWidth': 2 });

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
        $("#popupDocPreview").find(".modal-content").width(maxwidth)
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
function getInheritableProperty(annotation, name) {
    var item = annotation;
    while (item && !item.has(name)) {
        item = item.get('Parent');
    }
    if (!item)
        return null;
    return item.get(name);
}
var validation = {
    isGlyphsNumber: function (str) {
        var pattern = /^`+\d+$/;
        return pattern.test(str);  // returns a boolean
    }
};
var setInputValuesAtInit = function () {
    count++;
    if (count == pdfDoc.numPages) {
        $.each(pdfItems, function (k, data) {

            try {
                if (data.Type.toString() == texthashCode || data.Type.toString() == buttonhashCode) {
                    if (data.Value.includes("base64")) {
                        $('div[name="' + data.FieldName + '"]').parent().css("opacity", 0)
                    }
                    if (data.Value.includes("jsignature")) {
                        if ($('div[name="' + data.FieldName + '"]').length > 0) {

                            for (i = 0; i < $('div[name="' + data.FieldName + '"]').length; i++) {
                                var div = $($('div[name="' + data.FieldName + '"]')[i]);
                                if (data.PdfItemProperties.BackgroundColor != null) {
                                    div.css('background-color', data.PdfItemProperties.BackgroundColor)
                                } else {
                                    div.css('background-color', '#dde4ff')
                                }
                                if (!data.IsDraw) {
                                    if (data.Value.split(',')[1] == "") {
                                        div.append("<div style=\"display:table;width:100%;height:100%\"><p id=\"noSigntext\">Click Here To Sign </p>");
                                        div.find("canvas.jSignature").hide();
                                    }
                                    else {

                                        var height = div.height() / 150;
                                        var width = div.width() / 720;
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
                        }
                    } else {
                        if (data.ReadOnly) {

                            $('input[name="' + data.FieldName + '"]').hide();
                            $('input[name="' + data.FieldName + '"]').prev().hide();
                        }
                        if (data.IsTextArea) {
                            var newline = String.fromCharCode(13, 10);
                            data.Value = data.Value.replaceAll('//n', newline);
                            setValueInTextareaTextBox('textarea', data)
                        } else {
                            setValueInTextareaTextBox('input', data)

                        }
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
                    var values = data.AvailableValues;
                    if ($('input[name="' + data.FieldName + '"]').length == 1) {
                        $.each($('input[name="' + data.FieldName + '"]'), function (i) {
                            var len = $('[id^=' + data.FieldName + '_]').length
                            $(this).attr("id", data.FieldName + "_" + (len + 1))
                            this.value = values[0];
                            if (data.Value == this.value)
                                $(this).prop('checked', true);
                        });
                    }
                    else {
                        $.each($('input[name="' + data.FieldName + '"]'), function (i) {
                            var len = $('[id^=' + data.FieldName + '_]').length
                            $(this).attr("id", data.FieldName + "_" + (len + 1))
                            this.value = values[values.length - (i + 1)];
                            if (data.Value == this.value)
                                $(this).prop('checked', true);
                        });
                    }
                }
                if (data.IsImageField || data.FieldName.includes("Reseller_Signature")) {
                    if ((data.Base64 == null || data.Base64 == "") && !data.FieldName.includes("Reseller_Signature")) {
                        var parent = $('div[name="' + data.FieldName + '"]').parent()
                        $('div[name="' + data.FieldName + '"]').remove();
                        var div = $('<div name= "' + data.FieldName + '"><img src="" class="image" style="display:none;"/><div class="fileUpload primary document_imageUpload"><span>Upload</span><input type="file" class="upload" value="Upload" accept="image/x-png,image/gif,image/jpeg" onchange="readURL(this);"/></div></div>');
                        $(parent).append(div);

                    } else {
                        $('div[name="' + data.FieldName + '"]').parent().remove();
                    }
                }
                if (data.IsDraw) {
                    var $div = $('div[name="' + data.FieldName + '"]')
                    var signatureid = $div.attr('id');
                    var ctx = $div.find("canvas.jSignature")[0].getContext('2d');
                    ctx.scale(0.75, 0.75);
                    ctx.restore();
                    $div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = data.lineWidth * 0.75;
                    $div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = false;
                    $div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas(div.find("canvas.jSignature").add(div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    $div.attr("onclick", "").unbind("click");
                    $div.append('<input type="button" class="reset" onclick=clearSignature(this) id="clearSign" value="Clear Signature" style="position: absolute; top: auto; margin: 0px !important; left: 10px;">');
                    $div.jSignature("setData", "data:" + data.Value);
                    ctx.scale(1.33, 1.33);
                    ctx.restore();
                }
            }
            catch (err) { }
        });
        $('inputControl[type="checkbox"]').click(function () {
            if ($(this).prop('checked'))
                $('input[name="' + $(this).attr('name') + '"]').not($(this)).prop('checked', false);
        });
    }

    $(".inputControl[type=checkbox]").each(function () {
        var $lbl = $("<label>");
        $lbl.attr("for", this.id);
        $lbl.attr("class", "inputControl");
        $lbl.attr("style", $(this).attr("style"));
        $lbl.css("position", "absolute");
        $(this).after($lbl);
    });

    if (withoutBg) {
        $("#divViewer").find(".inputControl").css("background-color", "#ffffff");
        $("#divViewer").find('[id*="jSignature"]').css("background-color", "#ffffff");
        $("#divViewer").find('[id*="jSignature"]').find(".jSignature").css("background-color", "#ffffff");
    }

};

function setValueInTextareaTextBox(input, data) {
    $(input + '[name="' + data.FieldName + '"]').val(data.Value);
    if (data.PdfItemProperties.TextColor != null) {
        $(input + '[name="' + data.FieldName + '"]').css('color', data.PdfItemProperties.TextColor);
    } else {
        $(input + '[name="' + data.FieldName + '"]').css('color', 'black')
    }
    if (data.PdfItemProperties.BackgroundColor != null) {
        $(input + '[name="' + data.FieldName + '"]').css('background-color', data.PdfItemProperties.BackgroundColor)
    } else {
        $(input + '[name="' + data.FieldName + '"]').css('background-color', '#dde4ff')
    }
    if (data.PdfItemProperties.BorderColor != null) {
        $(input + '[name="' + data.FieldName + '"]').css('border', "1px solid " + data.PdfItemProperties.BorderColor)
    }
    if (data.PdfItemProperties.Alignment != null) {
        $(input + '[name="' + data.FieldName + '"]').css('text-align', data.PdfItemProperties.Alignment)
    }
    if (data.PdfItemProperties.FontSize != null) {
        $(input + '[name="' + data.FieldName + '"]').css('font-size', Math.round(data.PdfItemProperties.FontSize) + "px");
    }
    if (data.PdfItemProperties.Bold) {
        $(input + '[name="' + data.FieldName + '"]').css('font-weight', "bold");
    }
    if (data.PdfItemProperties.Italic) {
        $(input + '[name="' + data.FieldName + '"]').css('font-style', "italic");
    }
    if (data.PdfItemProperties.FontName) {
        $(input + '[name="' + data.FieldName + '"]').css('font-family', data.PdfItemProperties.FontName);
    }

    data.PdfItemProperties && data.PdfItemProperties.MaxLength && $(input + '[fieldname="' + data.FieldName + '"]').attr('maxLength', data.PdfItemProperties.MaxLength);
}

function clearSignature(input) {
    $(input).parent().jSignature('reset')
}