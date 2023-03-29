var trackData = [];
var undoData = [];
var cntrlIsPressed = false;
var found_names = [];
var copyDiv = [];
pdfItemsStr = pdfItemsStr.replace(/\\/g, '/');
var selectedDiv = [];
var selectable = false;
var pdfItems = [];
var foo;
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
offsetX = 0,
    offsetY = 0;
var formFields = {};
var div = document.getElementById('holder');
var $ActiveDiv = null;
var $CopyDiv = null;
var oldSelect = [];
var ctrlDown = false, ctrlKey = 17, cmdKey = 91, vKey = 86, cKey = 67;
var data = JSON.parse(jsonData);
var withoutBg = false;
var withoutBorder = false;
data.push({ label: "select", value: 'select' });

$(document).keydown(function (event) {
    if (event.which == "17")
        cntrlIsPressed = true;
});

$(document).keyup(function () {
    cntrlIsPressed = false;
});
$(document).ready(function () {

    $(".caustam-editor-outer").onPositionChanged(function () {
        $('.caustam-editor-outer').getNiceScroll().resize();
    });
    $("#outliner-li").click(function () {
        setTimeout(function () {
            if (selectedDiv.length > 1) {
                //clickItemOutliner("select")
            } else {
                clickItemOutliner($(".items.active").attr('id'), false)
            }

        }, 300)


    })
    $('#popupDocPreview').on('hidden.bs.modal', function () {
        $("#divViewer").html('');
    })
    $(window).on("load", function () {
        setTimeout(function () {
            addScroll()
        }, 1000);
    });
    setHeight();
    $(window).on("resize", function () {
        setHeight();
    })
    $('section#content').addClass('editor');
    //#region Set pdf-editor height dynamically
    function setHeight() {
        var headerHeight = $("#header").innerHeight();
        var windowHeight = $(window).innerHeight();
        var footerHeight = $("#footer").innerHeight();
        var ht = (headerHeight + footerHeight + $(".doc-editor-header").innerHeight() + $(".upper-toolbar").innerHeight() + 47);
        var hwt = windowHeight - ht;
        $(".caustam-editor-outer").height(hwt)
    }
    //#endregion

    $("#selectable").selectable();

    function addScroll() {
        $(".caustam-editor-outer").niceScroll({
            cursorcolor: "#c1c1c1",
            background: "#f2f2f2",
            cursorwidth: 15,
            cursorborder: "none",
            cursorborderradius: 0,
            autohidemode: false,
            bouncescroll: false,
            horizrailenabled: true,
            railvalign: "bottom"
        });

        $(".outliner_list").niceScroll({
            cursorcolor: "#c1c1c1",
            background: "#f2f2f2",
            cursorwidth: 5,
            cursorborder: "none",
            cursorborderradius: 0,
            autohidemode: true,
            bouncescroll: false,
            horizrailenabled: true,
            railvalign: "bottom"
        });
        //$(".editor-panes").niceScroll({
        //    cursorcolor: "#c1c1c1",
        //    background: "#f2f2f2",
        //    cursorwidth: 5,
        //    cursorborder: "none",
        //    cursorborderradius: 0,
        //    autohidemode: true,
        //    bouncescroll: false,
        //    horizrailenabled: true,
        //    railvalign: "bottom"
        //});
    }

    $("#onOffSwitchChkChecked").change(function () {
        var ischeck = $('#onOffSwitchChkChecked').prop('checked');
        for (var k = 0; k < selectedDiv.length; k++) {
            $(selectedDiv[k]).find('[type="checkbox"]').prop('checked', ischeck);
        }
    })

    $("#wrapper").addClass("custom-wrapper");
    $(".pdf-name").text(pdfname);
    if (pdfUrl != '') {
        init(pdfloadurl);
    }
    else {
        alert('File is not available.');
    }
    if (stateId > 0) {
        $("#folder-name").text(stateName)
    } else if (stateId == -1 || stateId == "") {
        $("#folder-name").text('STC')
    } else if (stateId == -2) {
        $("#folder-name").text('CES')
    } else if (stateId == 0) {
        $("#folder-name").text('Custom')
    }


    $("#zoom-dropdown").val("100");
    disabledIcon();
    $('input[type=radio][name=aspectratio]').change(function () {
        var val = $("input[name='aspectratio']:checked").val();
        setAspectRatio(val, $(selectedDiv[0]));
    });

    $("#onOffSwitchDrawEnable").change(function () {
        if ($(this).prop("checked")) {
            for (var k = 0; k < selectedDiv.length; k++) {
                var id = $(selectedDiv[k]).attr('id');
                $('#' + id).find("canvas.jSignature").add($('#' + id).filter("canvas.jSignature")).data("jSignature.this").settings["readOnly"] = false;
                $('#' + id).find("canvas.jSignature").width($('#' + id).width())
                $('#' + id).find("canvas.jSignature").height($('#' + id).height())
            }
        } else {
            for (var k = 0; k < selectedDiv.length; k++) {
                var id = $(selectedDiv[k]).attr('id');
                $('#' + id).find("canvas.jSignature").add($('#' + id).filter("canvas.jSignature")).data("jSignature.this").settings["readOnly"] = true;
                $('#' + id).find("canvas.jSignature").width(0)
                $('#' + id).find("canvas.jSignature").height(0)
            }
        }
    })

    $("#left-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            $(selectedDiv[k]).find(".image").css("left", "0")
            $(selectedDiv[k]).find(".image").css("right", "")
            $(selectedDiv[k]).find(".image").removeClass("img-center")
            if ($(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                $(selectedDiv[k]).find(".image").removeClass("img-center-middle")
                $(selectedDiv[k]).find(".image").addClass("img-middle");
            }
            $(selectedDiv[k]).attr("align", "1")
        }
    })
    $("#right-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            $(selectedDiv[k]).find(".image").css("right", "0")
            $(selectedDiv[k]).find(".image").css("left", "")
            if ($(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                $(selectedDiv[k]).find(".image").removeClass("img-center-middle")
                $(selectedDiv[k]).find(".image").addClass("img-middle");
            }
            $(selectedDiv[k]).find(".image").removeClass("img-center")
            $(selectedDiv[k]).attr("align", "2")
        }
    })
    $("#center-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            if (!$(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                if ($(selectedDiv[k]).find(".image").hasClass("img-middle")) {
                    $(selectedDiv[k]).find(".image").addClass("img-center-middle")
                    $(selectedDiv[k]).find(".image").removeClass("img-center")
                } else {
                    $(selectedDiv[k]).find(".image").addClass("img-center")
                }
            }
            $(selectedDiv[k]).attr("align", "3")
            $(selectedDiv[k]).find(".image").css("left", "")
            $(selectedDiv[k]).find(".image").css("right", "")
        }

    })
    $("#top-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            $(selectedDiv[k]).find(".image").css("top", "0")
            $(selectedDiv[k]).find(".image").css("bottom", "")
            $(selectedDiv[k]).find(".image").removeClass("img-middle")
            $(selectedDiv[k]).attr("align", "4")
            if ($(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                $(selectedDiv[k]).find(".image").removeClass("img-center-middle")
                $(selectedDiv[k]).find(".image").addClass("img-center");
            }
        }

    })
    $("#bottom-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            $(selectedDiv[k]).find(".image").css("bottom", $(".document_imageUpload").height())
            $(selectedDiv[k]).find(".image").css("top", "")
            $(selectedDiv[k]).find(".image").removeClass("img-middle")
            $(selectedDiv[k]).attr("align", "5")

            if ($(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                $(selectedDiv[k]).find(".image").removeClass("img-center-middle")
                $(selectedDiv[k]).find(".image").addClass("img-center");
            }
        }
    })

    $("#middle-align-img").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            if (!$(selectedDiv[k]).find(".image").hasClass("img-center-middle")) {
                if ($(selectedDiv[k]).find(".image").hasClass("img-center")) {
                    $(selectedDiv[k]).find(".image").addClass("img-center-middle")
                    $(selectedDiv[k]).find(".image").removeClass("img-middle")
                } else {
                    $(selectedDiv[k]).find(".image").addClass("img-middle")
                }
            }
            $(selectedDiv[k]).attr("align", "6")
            $(selectedDiv[k]).find(".image").css("top", "")
            $(selectedDiv[k]).find(".image").css("bottom", "")
        }
    })


    $("#fill-width").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            var width = $(selectedDiv[k]).outerWidth();
            $(selectedDiv[k]).find(".image").width(width);
            var aspectratio = parseInt($(selectedDiv[0]).find('img').attr("width")) / parseInt($(selectedDiv[0]).find('img').attr("height"));
            var newHeight = width / aspectratio;
            $(selectedDiv[k]).find(".image").height(newHeight);
            $(selectedDiv[k]).attr("fill-width", "1");
            $(selectedDiv[k]).attr("fill-height", "");
            $(selectedDiv[k]).attr("fill-content", "");
            if (parseInt($(selectedDiv[0]).find('.image').css('max-width')) < width) {
                $(selectedDiv[k]).find('.image').css('max-width', width + "px")
            }
        }
        //$("#fill-height").addClass("disabled");
        //$("#fill-content").addClass("disabled");
    })

    $("#fill-height").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            var height = $(selectedDiv[k]).outerHeight() - $(".document_imageUpload").height();
            var aspectratio = parseInt($(selectedDiv[0]).find('img').attr("width")) / parseInt($(selectedDiv[0]).find('img').attr("height"));
            $(selectedDiv[k]).find(".image").height(height);
            var newWidth = height * aspectratio;
            $(selectedDiv[k]).find(".image").width(newWidth);
            $(selectedDiv[k]).attr("fill-height", "1");
            $(selectedDiv[k]).attr("fill-width", "");
            $(selectedDiv[k]).attr("fill-content", "");
            if (parseInt($(selectedDiv[0]).find('.image').css('max-height')) < height) {
                $(selectedDiv[k]).find('.image').css('max-height', height + "px")
            }
        }
        //$("#fill-width").addClass("disabled");
        //$("#fill-content").addClass("disabled");

    })

    $("#fill-box-image").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            var imgHeight = $(selectedDiv[k]).find(".image").height();
            var imgWidth = $(selectedDiv[k]).find(".image").width();
            if (imgWidth > 350) {
                $(selectedDiv[k]).width(350);
            } else {
                $(selectedDiv[k]).width(imgWidth);
            }
            if (imgHeight > 150) {
                $(selectedDiv[k]).height(150 + $(".document_imageUpload").height());
            } else {
                $(selectedDiv[k]).height(imgHeight + $(".document_imageUpload").height());
            }
            if ($(selectedDiv[k]).attr("fill-height") == 1) {
                $("#fill-height").click();
            }
            else if ($(selectedDiv[k]).attr("fill-width") == 1) {

                $("#fill-width").click();

            } else if ($(selectedDiv[k]).attr("fill-content") == 1) {

                $("#fill-content").click();
            }
        }
    })

    $("#fill-content").click(function () {
        for (var k = 0; k < selectedDiv.length; k++) {
            var width = $(selectedDiv[k]).outerWidth();
            var height = $(selectedDiv[k]).outerHeight() - $(".document_imageUpload").height();
            $(selectedDiv[k]).find(".image").height(height);
            $(selectedDiv[k]).find(".image").width(width);
            $(selectedDiv[k]).attr("fill-content", "1");
            $(selectedDiv[k]).attr("fill-width", "");
            $(selectedDiv[k]).attr("fill-height", "");
            if (parseInt($(selectedDiv[0]).find('.image').css('max-width')) < width) {
                $(selectedDiv[k]).find('.image').css('max-width', width + "px")
            }
            if (parseInt($(selectedDiv[0]).find('.image').css('max-height')) < height) {
                $(selectedDiv[k]).find('.image').css('max-height', height + "px")
            }
        }
        //$("#fill-height").addClass("disabled");
        //$("#fill-width").addClass("disabled");
    })

    $("#lineWidth-plus").click(function () {
        var lineWidth = $("#field_lineWidth").val();
        $("#field_lineWidth").val(parseInt(lineWidth) + 1);
        for (var k = 0; k < selectedDiv.length; k++) {
            var drawId = $(selectedDiv[k]).attr('id');
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = parseInt(lineWidth) + 1;
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
        }
    })

    $("#lineWidth-minus").click(function () {
        var lineWidth = $("#field_lineWidth").val();
        $("#field_lineWidth").val(parseInt(lineWidth) - 1);
        for (var k = 0; k < selectedDiv.length; k++) {
            var drawId = $(selectedDiv[k]).attr('id');
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = parseInt(lineWidth) - 1;
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
        }
    })

    $("#field_lineWidth").focusout(function () {
        var lineWidth = $("#field_lineWidth").val();
        $("#field_lineWidth").val(lineWidth);
        for (var k = 0; k < selectedDiv.length; k++) {
            var drawId = $(selectedDiv[k]).attr('id');
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = parseInt(lineWidth);
            $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
        }
    })
    $("#field_lineWidth").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#field_lineWidth").focusout();
        }
    })


    $("#filter-outliner-bypage").change(function () {
        var page = $(this).val();
        found_names = [];
        $(".added").each(function (k, div) {
            if (page != 0 && $(div).attr("pagenumber") == page) {
                setOutlinerValueForPage(div)
            }
            if (page == 0) {
                getOutlinerData();
            }
        })
        populateOutlinerDropdown()
    })


    $("#filter-textbox").click(function () {

        if (!$(this).closest('li').hasClass("active")) {
            filterOutliner(textHashCode, true, false, "Textbox")
            $(this).closest('li').addClass("active")
        } else {
            filterOutliner(textHashCode, false, false, "Textbox")
            $(this).closest('li').removeClass("active")
        }
        populateOutlinerDropdown()

    })
    $("#filter-textarea").click(function () {
        if (!$(this).closest('li').hasClass("active")) {
            filterOutliner(textHashCode, true, true, "Textarea")
            $(this).closest('li').addClass("active")
        } else {
            filterOutliner(textHashCode, false, true, "Textarea")
            $(this).closest('li').removeClass("active")
        }
        populateOutlinerDropdown()

    })
    $("#filter-signature").click(function () {
        if (!$(this).closest('li').hasClass("active")) {
            filterOutliner(buttonHashCode, true, false, "Signature")
            $(this).closest('li').addClass("active")
        } else {
            filterOutliner(buttonHashCode, false, false, "Signature")
            $(this).closest('li').removeClass("active")
        }
        populateOutlinerDropdown()

    })
    $("#jobFields").click(function () {
        oldFieldName = $("#jobFieldValue").val();
    })
    $("#filter-checkbox").click(function () {
        if (!$(this).closest('li').hasClass("active")) {
            filterOutliner(checkBoxHashCode, true, false, "Checkbox")
            $(this).closest('li').addClass("active")
        } else {
            filterOutliner(checkBoxHashCode, false, false, "Checkbox")
            $(this).closest('li').removeClass("active")
        }
        populateOutlinerDropdown()

    })
    $(".edit-link").click(function () {
        var span = $(this).parent().find(".pdf-name")
        var $input = $("<input>", {
            val: span.text(),
            type: "text"
        });
        $input.addClass("form-control txtrenamefile");
        span.replaceWith($input);
        $input.select();
        $(".edit-link").hide();
        $("#saveFilename").show();
        $("#cancelFilename").show();
        $(".pdf-input").removeClass("remove-icon")
    })


    //#region copy paste
    $("#copy").click(function () {
        copyDiv = [];
        copyDiv.push.apply(copyDiv, selectedDiv)
        disabledIcon();
    })
    $("#paste").click(function () {
        for (var k = 0; k < copyDiv.length; k++) {
            var $CopyDiv = $(copyDiv[k]);
            var _inputType = $CopyDiv.attr('data-type');

            var $inputControl = null;
            if (_inputType == 't') {
                $inputControl = $CopyDiv.find('input[type="text"]').first();
            }
            else if (_inputType == 'ta') {
                $inputControl = $CopyDiv.find('textarea').first();
            }
            else if (_inputType == 'c') {
                $inputControl = $CopyDiv.find('input[type="checkbox"]').first();
            }
            else if (_inputType == 'sa' || _inputType == "btn" || _inputType == 'sa' || _inputType == "im") {
                $inputControl = $CopyDiv;
            }
            var _canvas = $('#the-svg'),
                _canvasTop = _canvas.offset().top,
                _canvasLeft = _canvas.offset().left,
                _inputValue = $inputControl.val(),
                _fieldName = $inputControl.attr("fieldname")
            if (_fieldName != undefined) {
                if (_fieldName.includes("undefined")) {
                    var i = generateFieldName();
                    _fieldName = "undefined_" + i;
                    $("#jobFields").val('select')
                } else {
                    _fieldName = _fieldName.split("_")[0] + "_" + _fieldName.split("_")[1];
                    var fieldname_cp = _fieldName
                    var fieldnameCount = 2
                    $('#jobFields').val(_fieldName)
                    while (true) {
                        if ($("[fieldname='" + fieldname_cp + "']").length > 0) {
                            fieldname_cp = _fieldName + "_" + fieldnameCount;
                            fieldnameCount++;
                        } else
                            break;
                    }
                    if (fieldnameCount > 2)
                        _fieldName = _fieldName + "_" + (fieldnameCount - 1);
                    $('#jobFieldValue').val(_fieldName)
                }
            }
            _elementLeft = $CopyDiv.offset().left - _canvasLeft + 10,
                _elementTop = $CopyDiv.offset().top - _canvasTop + 10,
                _elementWidth = $CopyDiv.outerWidth() + 'px',
                _elementHeight = $CopyDiv.outerHeight() + 'px',
                _maxLength = $inputControl.prop('maxlength'),
                _fontSize = $inputControl.css('font-size'),
                _fontName = $inputControl.css('font-family'),
                _horiAlign = $inputControl.attr("horialign"),
                _vertAlign = $inputControl.attr("vertalign"),
                _aspectRatio = $inputControl.attr("aspectratio"),
                _pageNum = $CopyDiv.attr("pagenumber");
            _textColor = $inputControl.css("color");
            _isBold = $inputControl.css('font-weight') == "700" ? true : false;
            _isItalic = $inputControl.css('font-style') == "italic" ? true : false;
            _bgcolor = $CopyDiv.css('background-color');
            _bordercolor = $CopyDiv.css('border-left-color');
            _align = $inputControl.css('text-align');

            var div = AddInitialControlOnPageLoad(_inputType, _inputValue, _fieldName, _elementLeft, _elementTop, _elementWidth, _elementHeight, _maxLength, _fontSize, _fontName, _horiAlign, _vertAlign, _aspectRatio, _pageNum, _textColor, _isBold, _isItalic, '', _bgcolor, _bordercolor, _align);
            if (div.data('type') == 't' || div.data('type') == 'c') {
                setOutlinerValue(div, div.find('input').attr('fieldname'))
            } else if (div.data('type') == 'ta') {
                setOutlinerValue(div, div.find('textarea').attr('fieldname'))
            } else {
                setOutlinerValue(div, div.attr('fieldname'))
            }
            populateOutlinerDropdown()
            trackData.push(div);
        }
        foo.disable();
        foo.enable();
        if ($(selectedDiv[0]).hasClass("grouping")) {
            $("#ungroupElement").click();
        }
        selectedDiv = [];
        for (var l = 0; l < trackData.length; l++) {
            if (l >= (trackData.length - copyDiv.length)) {
                selectedDiv.push(trackData[l]);
            }
        }
        for (var l = 0; l < copyDiv.length; l++) {
            $(copyDiv[l]).removeClass("blueBorder");
        }
        for (var l = 0; l < selectedDiv.length; l++) {
            $(selectedDiv[l]).addClass("blueBorder");
        }
        copyDiv = [];
        $("#groupElement").click();
        disabledIcon();
        $(".outliner_list").getNiceScroll().resize();
    })
    //#endregion

    //#region Paging Section

    $("#document-paging").change(function () {
        if ($(this).val() == 0) {
            $(".page-icon").hide();
        } else {
            disabledPageIcon(1)
        }
        $("#the-svg").find('.svg').remove();
        $(".added").remove();
        $("[class^='groupingDiv']").remove()
        init(pdfloadurl);
        $("#current-page").val(1);
        //setTimeout(function () { foo.enable()},500)

    })
    $(".caustam-editor-outer").scroll(function () {
        if ($("#document-paging").val() == 0) {
            setPageNumber();
        }

    })

    controlClick();

    $("#next-page").click(function () {
        $("#the-svg").find('.svg').remove();
        $(".added").remove();
        $("[class^='groupingDiv']").remove()
        var currentpge = parseInt($("#current-page").val());
        disabledPageIcon(currentpge + 1)
        $("[pagenumber='" + currentpge + "']").remove();
        renderPage(currentpge + 1, pdfDoc.numPages);
        $("#current-page").val(currentpge + 1);
        //setTimeout(function () { foo.enable() }, 500)
    })
    $("#previous-page").click(function () {
        $("#the-svg").find('.svg').remove();
        $(".added").remove();
        $("[class^='groupingDiv']").remove()
        var currentpge = parseInt($("#current-page").val());
        disabledPageIcon(currentpge - 1)
        $("[pagenumber='" + currentpge + "']").remove();
        renderPage(currentpge - 1, pdfDoc.numPages);
        $("#current-page").val(currentpge - 1);
        //setTimeout(function () { foo.enable() }, 500)
    })
    $("#current-page").keydown(function (event) {
        if (event.which == "13")
            $("#current-page").focusout();
    })
    var pageNumber;
    $("#current-page").focus(function () {
        pageNumber = $(this).val();
    })

    $("#current-page").focusout(function () {
        if ($("#current-page").val() <= pdfDoc.numPages) {
            if ($("#document-paging").val() == 0) {

                var headerHeight = $("#header").height() + $(".doc-editor-header").innerHeight() + $(".upper-toolbar").innerHeight();
                var offset = $($("#the-svg").find('.svg')[$("#current-page").val() - 1]).offset().top - headerHeight;
                var scroll = $('.caustam-editor-outer').scrollTop();
                if (offset > window.innerHeight) {
                    // Not in view so scroll to it

                    $('.caustam-editor-outer').animate({ scrollTop: (offset + scroll) }, 1000);
                }
                if (offset < 0) {

                    $('.caustam-editor-outer').animate({ scrollTop: Math.abs(offset + scroll) }, 1000);
                }

            } else {
                $("#the-svg").find('.svg').remove();
                $(".added").remove();
                $("[class^='groupingDiv']").remove()
                var currentPage = parseInt($("#current-page").val());
                renderPage(currentPage, pdfDoc.numPages);
                disabledPageIcon(currentPage);
            }
        } else {
            $("#current-page").val(pageNumber)
        }

    })




    //#endregion
    //#region group ungroup section
    $("#groupElement").click(function () {
        if (!$(selectedDiv[0]).hasClass("grouping")) {

            var minLeft = $("#holderParentDiv").width();
            var maxLeft = 0;
            var minTop = $("#the-svg").height();
            var maxTop = 0;
            for (var i = 0; i < selectedDiv.length; i++) {
                var divleft = parseFloat($(selectedDiv[i]).css("left"));
                if (divleft < minLeft) {
                    minLeft = divleft
                }
                var divright = divleft + $(selectedDiv[i]).width();
                if (divright > maxLeft) {
                    maxLeft = divright;
                }
                var divtop = parseFloat($(selectedDiv[i]).css("top"));
                if (divtop < minTop) {
                    minTop = divtop;
                }
                var divbottom = divtop + $(selectedDiv[i]).height();
                if (divbottom > maxTop) {
                    maxTop = divbottom;
                }
                $(selectedDiv[i]).addClass("grouping")
            }
            if (selectedDiv.length > 0) {
                var width = maxLeft - minLeft;
                var height = maxTop - minTop;
                var groupWidthHeightFlag = true;
                minTop = minTop;
                minLeft = minLeft;
                var className = generateGroup();
                var div = '<div class="' + className + '" style="position:absolute;top:' + minTop + 'px;left:' + minLeft + 'px;border:2px solid blue;width:' + (parseInt(width) + 10) + 'px;height:' + (parseInt(height) + 10) + 'px;"></div>';
                $("#holder").append(div);
                var left;
                var top;
                for (var i = 0; i < selectedDiv.length; i++) {
                    $('.' + className).append($(selectedDiv[i]));
                    left = parseFloat($(selectedDiv[i]).css("left"));
                    top = parseFloat($(selectedDiv[i]).css("top"));
                    $(selectedDiv[i]).css("top", top - minTop);
                    $(selectedDiv[i]).css("left", left - minLeft);
                    $(selectedDiv[i]).draggable('disable')
                    if (i == 0) {
                        width = $(selectedDiv[i]).outerWidth();
                        height = $(selectedDiv[i]).outerHeight();
                    } else {
                        if (groupWidthHeightFlag) {
                            if (width == $(selectedDiv[i]).outerWidth() && height == $(selectedDiv[i]).outerHeight()) {
                                $("#field_width").val($(selectedDiv[i]).outerWidth());
                                $("#field_height").val($(selectedDiv[i]).outerHeight());
                            } else {
                                $("#field_width").val('');
                                $("#field_height").val('');
                                groupWidthHeightFlag = false
                            }
                        }
                    }


                }

                var click = {
                    x: 0,
                    y: 0
                };
                $('.' + className).draggable({
                    start: function (event) {
                        click.x = event.clientX;
                        click.y = event.clientY;
                    },

                    drag: function (event, ui) {

                        // This is the parameter for scale()
                        var zoom = parseFloat($("#zoom-dropdown").val() / 100)

                        var original = ui.originalPosition;

                        // jQuery will simply use the same object we alter here
                        ui.position = {
                            left: (event.clientX - click.x + original.left) / zoom,
                            top: (event.clientY - click.y + original.top) / zoom
                        };

                    },
                    cancel: 'canvas',
                    containment: '.ui-droppable',
                    handle: $(".grouping"),
                    scroll: true
                })
                $(".ungroup").show();
                $(".group").hide();
                disabledIcon();
            }
        }
    })
    $("#ungroupElement").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            var left = parseFloat($(selectedDiv[i]).parent().css("left"));
            var top = parseFloat($(selectedDiv[i]).parent().css("top"));
            $(selectedDiv[i]).parent().find('.added').each(function () {
                var childLeft = parseFloat($(this).css("left"));
                var childTop = parseFloat($(this).css("top"));
                $(this).css("position", "absolute")
                $(this).css("left", (left + childLeft) + "px")
                $(this).css("top", (top + childTop) + "px")
            })
            if ($(selectedDiv[i]).hasClass("grouping")) {
                var cnt = $(selectedDiv[i]).parent().contents();
                $(selectedDiv[i]).parent().replaceWith(cnt);

            }
            $(selectedDiv[i]).parent().children().removeClass("grouping")
            if ($(selectedDiv[i]).hasClass('ui-draggable'))
                $(selectedDiv[i]).draggable('enable')
        }
        $(".ungroup").hide();
        $(".group").show();
        disabledIcon();
    })
    //#endregion

    //#region font properties and color
    $("#FontFamily").change(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).css("font-family", $("#FontFamily").val())
        }
    })
    $("#field_maxlength").focusout(function () {
        var maxlength = $("#field_maxlength").val();
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').attr("maxlength", maxlength);
            $($(selectedDiv[i])).find('textarea').attr("maxlength", maxlength);
        }

    })
    $("#text-bold").click(function () {
        if ($("#text-bold").hasClass("active")) {
            for (i = 0; i < selectedDiv.length; i++) {
                $($(selectedDiv[i])).find('input').css("font-weight", "");
                $($(selectedDiv[i])).find('textarea').css("font-weight", "");
            }
            $("#text-bold").removeClass("active");
        } else {
            for (i = 0; i < selectedDiv.length; i++) {
                $($(selectedDiv[i])).find('input').css("font-weight", "bold");
                $($(selectedDiv[i])).find('textarea').css("font-weight", "bold");
            }
            $("#text-bold").addClass("active");
        }

    })
    $("#text-italic").click(function () {
        if ($("#text-italic").hasClass("active")) {
            for (i = 0; i < selectedDiv.length; i++) {
                $($(selectedDiv[i])).find('input').css("font-style", "");
                $($(selectedDiv[i])).find('textarea').css("font-style", "");
            }
            $("#text-italic").removeClass("active");
        } else {
            for (i = 0; i < selectedDiv.length; i++) {
                $($(selectedDiv[i])).find('input').css("font-style", "italic");
                $($(selectedDiv[i])).find('textarea').css("font-style", "italic");
            }
            $("#text-italic").addClass("active");
        }

    })
    $("#text-left").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').css("text-align", "left")
            $($(selectedDiv[i])).find('textarea').css("text-align", "left")
        }
        $("#text-left").addClass("active")
        $("#text-center").removeClass("active")
        $("#text-right").removeClass("active")
    });
    $("#text-center").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').css("text-align", "center")
            $($(selectedDiv[i])).find('textarea').css("text-align", "center")
        }
        $("#text-center").addClass("active")
        $("#text-right").removeClass("active")
        $("#text-left").removeClass("active")
    });
    $("#text-right").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').css("text-align", "right")
            $($(selectedDiv[i])).find('textarea').css("text-align", "right")
        }
        $("#text-right").addClass("active")
        $("#text-center").removeClass("active")
        $("#text-left").removeClass("active")
    });
    $("#font-size-plus").click(function () {
        var size = $("#font-size").val().match(/\d/g);
        size = size.join("");
        size = parseInt(size) + 1;;
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').css("font-size", size + "px")
            $($(selectedDiv[i])).find('textarea').css("font-size", size + "px")
        }
        $("#font-size").val(size + "px")
    })
    $("#font-size-minus").click(function () {
        var size = $("#font-size").val().match(/\d/g);
        size = size.join("");
        size = parseInt(size) - 1;;
        for (i = 0; i < selectedDiv.length; i++) {
            $($(selectedDiv[i])).find('input').css("font-size", size + "px")
            $($(selectedDiv[i])).find('textarea').css("font-size", size + "px")
        }
        $("#font-size").val(size + "px")
    })
    $("#line-height-plus").click(function () {
        var lineHeight = $("#line-height").val().match(/\d/g);
        lineHeight = lineHeight.join("");
        lineHeight = parseInt(lineHeight) + 3;
        for (i = 0; i < selectedDiv.length; i++) {
            //$(selectedDiv[i]).find('input').css("line-height", size + "%")
            $($(selectedDiv[i])).find('textarea').css("line-height", lineHeight + "px")
        }
        $("#line-height").val(lineHeight + "px")
    })
    $("#line-height-minus").click(function () {
        var lineHeight = $("#line-height").val().match(/\d/g);
        lineHeight = lineHeight.join("");
        lineHeight = parseInt(lineHeight) - 3;
        for (i = 0; i < selectedDiv.length; i++) {
            //$(selectedDiv[i]).find('input').css("line-height", size + "%")
            $($(selectedDiv[i])).find('textarea').css("line-height", lineHeight + "px")
        }
        $("#line-height").val(lineHeight + "px")
    })
    $("#text-color-chk").change(function () {
        var checked = $(this).prop("checked");
        if (checked) {
            $("#text-color-choice").show();
        } else {
            $("#text-color-choice").hide();
        }
    });
    $("#bg-color-chk").change(function () {
        var checked = $(this).prop("checked");
        if (checked) {
            $("#bg-color-choice").show();
        } else {
            $("#bg-color-choice").hide();
        }
    })
    $("#border-color-chk").change(function () {
        var checked = $(this).prop("checked");
        if (checked) {
            $("#border-color-choice").show();
        } else {
            $("#border-color-choice").hide();
        }
    })
    //#endregion

    //#region zoom 
    $("#zoom-dropdown").change(function () {
        var zoom = $("#zoom-dropdown").val();
        var newZoom = parseInt(zoom);
        $(".caustam-editor-con").css("transform", "scale(" + newZoom / 100 + ")")
        $(".caustam-editor-con").css("transform-origin", "left top")
        $("#zoom-dropdown").val(newZoom);
        $(".caustam-editor-outer").getNiceScroll().resize();
        disabledIcon();
    })
    $("#zoom-in").click(function () {
        var zoom = $("#zoom-dropdown").val();
        var newZoom = parseInt(zoom) + 25;
        //$(".caustam-editor-con").zoomTo({ targetsize: 0.75, duration: 600 });
        $(".caustam-editor-con").css("transform", "scale(" + newZoom / 100 + ")")
        $(".caustam-editor-con").css("transform-origin", "left top")
        $("#zoom-dropdown").val(newZoom);
        disabledIcon();
        $(".caustam-editor-outer").getNiceScroll().resize();
        setPageNumber();

    })
    $("#zoom-out").click(function () {
        var zoom = $("#zoom-dropdown").val();
        var newZoom = parseInt(zoom) - 25;
        //$(".caustam-editor-con").zoomTo({ targetsize: , duration: 600 });
        $(".caustam-editor-con").css("transform", "scale(" + newZoom / 100 + ")")
        $(".caustam-editor-con").css("transform-origin", "left top")
        $("#zoom-dropdown").val(newZoom);
        disabledIcon();
        $(".caustam-editor-outer").getNiceScroll().resize();
        setPageNumber();
    })
    //#endregion

    //#region toolbar
    $("#undo").click(function () {
        var div = trackData.pop();
        if ($("#holder").find(div).length == 0) {
            var click = {
                x: 0,
                y: 0
            };
            div.removeClass("blueBorder")
            $("#holder").append(div);
            $('.added .control').click(function () {
                $(this).focus();
            });
            div.click(function (event) {
                InputControlClick($(this), event);
            });
            div.draggable({
                start: function (event) {
                    click.x = event.clientX;
                    click.y = event.clientY;
                },

                drag: function (event, ui) {

                    // This is the parameter for scale()
                    var zoom = parseFloat($("#zoom-dropdown").val() / 100)

                    var original = ui.originalPosition;

                    // jQuery will simply use the same object we alter here
                    ui.position = {
                        left: (event.clientX - click.x + original.left) / zoom,
                        top: (event.clientY - click.y + original.top) / zoom
                    };

                },
                cancel: 'canvas',
                containment: '.ui-droppable',
                scroll: true
            }).resizable({
                containment: '.ui-droppable',
                handles: 'nw,ne,sw,se',
                stop: function (event, ui) {
                    setTimeout(function () {
                        setAspectRatioResize()
                        if ($(ui.element).data('type') == "im") {
                            if ($(ui.element).attr("fill-height") == 1) {
                                $("#fill-height").click();
                            }
                            else if ($(ui.element).attr("fill-width") == 1) {

                                $("#fill-width").click();

                            } else if ($(ui.element).attr("fill-content") == 1) {

                                $("#fill-content").click();
                            }
                        }
                    }, 500)
                },
                resize: function (event, ui) {

                    var changeWidth = ui.size.width - ui.originalSize.width; // find change in width
                    var newWidth = ui.originalSize.width + changeWidth / parseFloat($("#zoom-dropdown").val() / 100); // adjust new width by our zoomScale

                    var changeHeight = ui.size.height - ui.originalSize.height; // find change in height
                    var newHeight = ui.originalSize.height + changeHeight / parseFloat($("#zoom-dropdown").val() / 100); // adjust new height by our zoomScale

                    ui.size.width = newWidth;
                    ui.size.height = newHeight;
                    if ($(ui.element).data('type') == "dw") {
                        $(ui.element).find(".jSignature").attr("width", newWidth + "px")
                        $(ui.element).find(".jSignature").attr("height", newHeight + "px")
                        $('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    }
                }
            });
            if (div.data('type') == 't' || div.data('type') == 'c') {
                $('#jobFields').val(div.find('input').attr('fieldname').split("_")[0] + "_" + div.find('input').attr('fieldname').split("_")[1])
                $('#jobFieldValue').val("");
                setOutlinerValue(div, div.find('input').attr('fieldname'))
            } else if (div.data('type') == 'ta') {
                $('#jobFields').val(div.find('textarea').attr('fieldname').split("_")[0] + "_" + div.find('textarea').attr('fieldname').split("_")[1])
                setOutlinerValue(div, div.find('textarea').attr('fieldname'))
            } else {
                $('#jobFields').val(div.attr('fieldname').split("_")[0] + "_" + div.attr('fieldname').split("_")[1])
                setOutlinerValue(div, div.attr('fieldname'))
            }
        } else {
            div.remove();
            removeOutlinerValue(div);
        }
        populateOutlinerDropdown();

        undoData.push(div);
        disabledIcon();
        $(".outliner_list").getNiceScroll().resize();

    })
    $("#redo").click(function () {
        var div = undoData.pop();
        if ($("#holder").find(div).length == 0) {
            div.removeClass("blueBorder")
            $("#holder").append(div);
            var click = {
                x: 0,
                y: 0
            };
            div.draggable({
                start: function (event) {
                    click.x = event.clientX;
                    click.y = event.clientY;
                },

                drag: function (event, ui) {

                    // This is the parameter for scale()
                    var zoom = parseFloat($("#zoom-dropdown").val() / 100)

                    var original = ui.originalPosition;

                    // jQuery will simply use the same object we alter here
                    ui.position = {
                        left: (event.clientX - click.x + original.left) / zoom,
                        top: (event.clientY - click.y + original.top) / zoom
                    };

                },
                cancel: 'canvas',
                containment: '.ui-droppable',
                scroll: true
            }).resizable({
                containment: '.ui-droppable',
                handles: 'nw,ne,sw,se',
                stop: function (event, ui) {
                    if ($(ui.element).data('type') == "im") {
                        if ($(ui.element).attr("fill-height") == 1) {
                            $("#fill-height").click();
                        }
                        else if ($(ui.element).attr("fill-width") == 1) {

                            $("#fill-width").click();

                        } else if ($(ui.element).attr("fill-content") == 1) {

                            $("#fill-content").click();
                        }
                    }
                },
                resize: function (event, ui) {

                    var changeWidth = ui.size.width - ui.originalSize.width; // find change in width
                    var newWidth = ui.originalSize.width + changeWidth / parseFloat($("#zoom-dropdown").val() / 100); // adjust new width by our zoomScale

                    var changeHeight = ui.size.height - ui.originalSize.height; // find change in height
                    var newHeight = ui.originalSize.height + changeHeight / parseFloat($("#zoom-dropdown").val() / 100); // adjust new height by our zoomScale

                    ui.size.width = newWidth;
                    ui.size.height = newHeight;
                    if ($(ui.element).data('type') == "dw") {
                        $(ui.element).find(".jSignature").attr("width", newWidth + "px")
                        $(ui.element).find(".jSignature").attr("height", newHeight + "px")
                        $('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    }
                }
            });
            div.click(function (event) {
                InputControlClick($(this), event);
            });
            $('.added .control').click(function () {
                $(this).focus();
            });
            if (div.data('type') == 't' || div.data('type') == 'c') {
                $('#jobFields').val(div.find('input').attr('fieldname').split("_")[0] + "_" + div.find('input').attr('fieldname').split("_")[1])
                $('#jobFieldValue').val("");
                setOutlinerValue(div, div.find('input').attr('fieldname'))
            } else if (div.data('type') == 'ta') {
                $('#jobFields').val(div.find('textarea').attr('fieldname').split("_")[0] + "_" + div.find('textarea').attr('fieldname').split("_")[1])
                setOutlinerValue(div, div.find('textarea').attr('fieldname'))
            } else {
                $('#jobFields').val(div.attr('fieldname').split("_")[0] + "_" + div.attr('fieldname').split("_")[1])
                setOutlinerValue(div, div.attr('fieldname'))
            }
        } else {
            div.remove();
            removeOutlinerValue(div);
        }
        populateOutlinerDropdown()

        if (trackData.length == 5) {
            trackData.splice(1, 1)
        }
        trackData.push(div);
        disabledIcon();
        $(".outliner_list").getNiceScroll().resize();
    })
    $("#delete").click(function () {

        if (selectedDiv.length > 0) {
            for (var k = 0; k < selectedDiv.length; k++) {
                if ($(selectedDiv[k]).hasClass("grouping")) {
                    $("#ungroupElement").click();
                }
                $(selectedDiv[k]).remove();
                if (trackData.length == 5) {
                    trackData.splice(1, 1)
                }
                trackData.push($(selectedDiv[k]));

                disabledIcon();
                removeOutlinerValue($(selectedDiv[k]))
                selectedDiv.splice(k, 1);
                k--;
            }
            populateOutlinerDropdown()

        } else {
            alert("Please select field to delete")
        }
    })
    $("#toFront").click(function () {
        var pagenumber = $(selectedDiv[0]).attr("pagenumber")
        var minleft = $(selectedDiv[0]).position().left;
        var maxleft = $(selectedDiv[0]).position().left + $(selectedDiv[0]).width();
        var mintop = $(selectedDiv[0]).position().top;
        var maxtop = $(selectedDiv[0]).position().top + $(selectedDiv[0]).height();
        $("[pagenumber=" + pagenumber + "]").each(function () {
            var div = $(this);
            if (div != $(selectedDiv[0])) {
                var divMinLeft = div.position().left;
                var divMaxLeft = div.position().left + div.width();
                var divMinTop = div.position().top;
                var divMaxTop = div.position().top + div.height();
                if ((((divMinLeft > minleft && divMinLeft < maxleft) || (divMaxLeft >= minleft && divMaxLeft < maxleft)) || ((minleft > divMinLeft && minleft < divMaxLeft) || (maxleft >= divMinLeft && maxleft < divMaxLeft))) && (((divMinTop > mintop && divMinTop < maxtop) || (divMaxTop > mintop && divMaxTop < maxtop)) || ((mintop > divMinTop && mintop < divMaxTop) || (maxtop >= divMinTop && maxtop < divMinTop)))) {
                    div.css("z-index", "0");
                }
            }
        });
        $(selectedDiv[0]).css("z-index", "2");
    })
    $("#toBack").click(function () {
        var pagenumber = $(selectedDiv[0]).attr("pagenumber")
        var minleft = $(selectedDiv[0]).position().left;
        var maxleft = $(selectedDiv[0]).position().left + $(selectedDiv[0]).width();
        var mintop = $(selectedDiv[0]).position().top;
        var maxtop = $(selectedDiv[0]).position().top + $(selectedDiv[0]).height();

        $("[pagenumber=" + pagenumber + "]").each(function () {
            var div = $(this);
            if (div != $(selectedDiv[0])) {
                var divMinLeft = div.position().left;
                var divMaxLeft = div.position().left + div.width();
                var divMinTop = div.position().top;
                var divMaxTop = div.position().top + div.height();
                if ((((divMinLeft > minleft && divMinLeft < maxleft) || (divMaxLeft >= minleft && divMaxLeft < maxleft)) || ((minleft > divMinLeft && minleft < divMaxLeft) || (maxleft >= divMinLeft && maxleft < divMaxLeft))) && (((divMinTop > mintop && divMinTop < maxtop) || (divMaxTop > mintop && divMaxTop < maxtop)) || ((mintop > divMinTop && mintop < divMaxTop) || (maxtop >= divMinTop && maxtop < divMinTop)))) {
                    div.css("z-index", "2");
                }
            }
        });
        $(selectedDiv[0]).css("z-index", "0")
    })
    //#endregion


    //#region input tye properties like width,height,pos
    $("#field_autosize").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            if ($(selectedDiv[i]).data('type') == "t") {

                $(selectedDiv[i]).width(198);
                $(selectedDiv[i]).height(28);
                $("#field_width").val(198);
                $("#field_height").val(28);
                $(selectedDiv[i]).attr("height", "28px")
                $(selectedDiv[i]).attr("width", "198px")

            } else if ($(selectedDiv[i]).data('type') == "ta") {

                $(selectedDiv[i]).width(298);
                $(selectedDiv[i]).height(98);
                $("#field_width").val(298);
                $("#field_height").val(98);
                $(selectedDiv[i]).attr("height", "298px")
                $(selectedDiv[i]).attr("width", "98px")

            } else if ($(selectedDiv[i]).data('type') == "sa" || $(selectedDiv[i]).data('type') == "btn" || $(selectedDiv[i]).data('type') == "dw") {

                $(selectedDiv[i]).width(258);
                $(selectedDiv[i]).height(98);
                $("#field_width").val(258);
                $("#field_height").val(98);
                $(selectedDiv[i]).attr("height", "98px")
                $(selectedDiv[i]).attr("width", "258px")

            } else if ($(selectedDiv[i]).data('type') == "c") {

                $(selectedDiv[i]).width(28);
                $(selectedDiv[i]).height(28);
                $("#field_width").val(28);
                $("#field_height").val(28);
                $(selectedDiv[i]).attr("height", "28px")
                $(selectedDiv[i]).attr("width", "28px")

            }
        }
    })
    $("#align-to-left").click(function () {
        if ($("#align-to-left").css('opacity') == 1) {
            var min = $(window).width();
            for (var i = 0; i < selectedDiv.length; i++) {
                if ($(selectedDiv[i]).position().left < min) {
                    min = $(selectedDiv[i]).position().left;
                }
            }
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("left", min);
            }
            $("#field_left").val(min);
            setHeightWidthGroupingDiv()
        }

    });
    $("#align-to-top").click(function () {
        if ($("#align-to-top").css('opacity') == 1) {
            var min = $(window).height();
            for (var i = 0; i < selectedDiv.length; i++) {
                if ($(selectedDiv[i]).position().top < min) {
                    min = $(selectedDiv[i]).position().top;
                }
            }
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("top", min);
            }
            $("#field_top").val(min);
            setHeightWidthGroupingDiv()
        }

    });
    $("#align-justify").click(function () {
        if ($("#align-justify").css('opacity') == 1) {
            var min = $(window).width();
            var center = $(selectedDiv[0]).position().left + ($(selectedDiv[0]).width() / 2)
            for (var i = 1; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("left", center - ($(selectedDiv[i]).width() / 2));
            }
            setHeightWidthGroupingDiv()
        }
    })
    $("#align-middle").click(function () {
        if ($("#align-middle").css('opacity') == 1) {
            var min = $(window).height();
            var center = $(selectedDiv[0]).position().top + ($(selectedDiv[0]).height() / 2)
            for (var i = 1; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("top", center - ($(selectedDiv[i]).height() / 2));
            }
            setHeightWidthGroupingDiv()
        }
    })

    $("#align-to-bottom").click(function () {
        if ($("#align-to-bottom").css('opacity') == 1) {
            var max = 0;
            for (var i = 0; i < selectedDiv.length; i++) {
                var top = $(selectedDiv[i]).position().top + $(selectedDiv[i]).height();
                if (top > max) {
                    max = top;
                }
            }
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("top", max - $(selectedDiv[i]).height());
            }
            setHeightWidthGroupingDiv()
        }
    });
    $("#align-to-right").click(function () {
        if ($("#align-to-right").css('opacity') == 1) {
            var max = 0;
            for (var i = 0; i < selectedDiv.length; i++) {
                var left = $(selectedDiv[i]).position().left + $(selectedDiv[i]).width();
                if (left > max) {
                    max = left;
                }
            }
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).css("left", max - $(selectedDiv[i]).width());
            }
            setHeightWidthGroupingDiv()
        }
    });
    $("#horizontal-distribute-center").click(function () {
        if ($("#horizontal-distribute-center").css('opacity') == 1 && selectedDiv.length > 2) {
            var totalWidth = $(selectedDiv[0]).parent().width()
            var elementWidth = 0;
            for (var i = 0; i < selectedDiv.length; i++) {
                elementWidth = elementWidth + $(selectedDiv[i]).outerWidth();
            }
            var totalSpace = totalWidth - elementWidth;
            var equalSpace = totalSpace / (selectedDiv.length - 1);
            var copy = selectedDiv;
            var max = totalWidth;
            for (var i = 0; i < selectedDiv.length; i++) {
                var j = i;
                while (j > 0) {
                    if ($(selectedDiv[j]).position().left < $(selectedDiv[j - 1]).position().left) {
                        var div = selectedDiv[j - 1];
                        selectedDiv[j - 1] = selectedDiv[j];
                        selectedDiv[j] = div;
                    }
                    j--;
                }
            }
            var firstElementWidth = $(selectedDiv[0]).position().left + $(selectedDiv[0]).outerWidth()
            for (var i = 1; i < selectedDiv.length; i++) {
                selectedDiv[i].css("left", firstElementWidth + equalSpace)
                firstElementWidth = $(selectedDiv[i]).position().left + $(selectedDiv[i]).outerWidth();
            }
        }
        setHeightWidthGroupingDiv()
        selectedDiv = copy;
    })
    $("#vertical-distribute-center").click(function () {
        if ($("#vertical-distribute-center").css('opacity') == 1 && selectedDiv.length > 2) {
            var totalHeight = $(selectedDiv[0]).parent().height()
            var elementHeight = 0;
            for (var i = 0; i < selectedDiv.length; i++) {
                elementHeight = elementHeight + $(selectedDiv[i]).outerHeight();
            }
            var totalSpace = totalHeight - elementHeight;
            var equalSpace = totalSpace / (selectedDiv.length - 1)
            var copy = selectedDiv;
            for (var i = 0; i < selectedDiv.length; i++) {
                var j = i;
                while (j > 0) {
                    if ($(selectedDiv[j]).position().top < $(selectedDiv[j - 1]).position().top) {

                        var div = selectedDiv[j - 1];
                        selectedDiv[j - 1] = selectedDiv[j];
                        selectedDiv[j] = div;
                    }
                    j--;
                }
            }
            var firstElementHeight = $(selectedDiv[0]).position().top + $(selectedDiv[0]).outerHeight()
            for (var i = 1; i < selectedDiv.length; i++) {
                selectedDiv[i].css("top", firstElementHeight + equalSpace)
                firstElementHeight = $(selectedDiv[i]).position().top + $(selectedDiv[i]).outerHeight();
            }
        }
        setHeightWidthGroupingDiv();
        selectedDiv = copy;
    })
    $("#rotate-90").click(function () {
        for (i = 0; i < selectedDiv.length; i++) {
            var angle = getAngle($(selectedDiv[i]));
            $(selectedDiv[i]).css('transform', 'rotate(' + (angle + 90) + 'deg)');

        }
    })

    $("#angle-minus").click(function () {
        var angle = parseInt($("#field_angle").val())
        $("#field_angle").val(angle - 1);
        $(selectedDiv[0]).css('transform', 'rotate(' + (angle - 1) + 'deg)');
    })
    $("#angle-plus").click(function () {
        var angle = parseInt($("#field_angle").val())
        $("#field_angle").val(angle + 1);
        angle = getAngle($(selectedDiv[0]));
        $(selectedDiv[0]).css('transform', 'rotate(' + (angle + 1) + 'deg)');
    })
    $("#width-plus").click(function () {
        if ($("#field_width").val() == "")
            $("#field_width").val(0)
        var width = (parseInt($("#field_width").val()) + 1) < 0 ? 0 : parseInt($("#field_width").val()) + 1;
        var maxWidth = 0;
        $("#field_width").val(width);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("width", width)
            $(selectedDiv[i]).attr("width", width + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("height", width)
                $(selectedDiv[i]).attr("height", width + "px")
                $("#field_height").val(width)
            }
        }
        setHeightWidthGroupingDiv();
    })
    $("#field_width").focusout(function () {
        var width = parseInt($("#field_width").val());
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("width", width)
            $(selectedDiv[i]).attr("width", width + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("height", width)
                $(selectedDiv[i]).attr("height", width + "px")
                $("#field_height").val(width)
            }
        }
        setHeightWidthGroupingDiv();
    })

    $("#field_width").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#field_width").focusout();
        }
    });

    $("#field_height").focusout(function () {
        var height = parseInt($("#field_height").val());
        var maxHeight = 0;
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("height", height)
            $(selectedDiv[i]).attr("height", height + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("width", height)
                $(selectedDiv[i]).attr("width", height + "px")
                $("#field_width").val(height)
            }
        }
        setHeightWidthGroupingDiv();
    })

    $("#field_height").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#field_height").focusout();
        }
    });
    $("#width-minus").click(function () {
        if ($("#field_width").val() == "")
            $("#field_width").val(0)
        var width = (parseInt($("#field_width").val()) - 1) < 0 ? 0 : parseInt($("#field_width").val()) - 1;
        var maxWidth = 0;
        $("#field_width").val(width);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("width", width)
            $(selectedDiv[i]).attr("width", width + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("height", width)
                $(selectedDiv[i]).attr("height", width + "px")
                $("#field_height").val(width)
            }
        }
        setHeightWidthGroupingDiv();
    })
    $("#height-plus").click(function () {
        if ($("#field_height").val() == "")
            $("#field_height").val(0)
        var height = (parseInt($("#field_height").val()) + 1) < 0 ? 0 : (parseInt($("#field_height").val()) + 1);
        $("#field_height").val(height);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("height", height)
            $(selectedDiv[i]).attr("height", height + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("width", height)
                $(selectedDiv[i]).attr("width", height + "px")
                $("#field_width").val(height)
            }
        }
        setHeightWidthGroupingDiv();
    })
    $("#height-minus").click(function () {
        if ($("#field_height").val() == "")
            $("#field_height").val(0)
        var height = (parseInt($("#field_height").val()) - 1) < 0 ? 0 : (parseInt($("#field_height").val()) - 1);
        $("#field_height").val(height);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("height", height)
            $(selectedDiv[i]).attr("height", height + "px")
            if ($(selectedDiv[i]).find("input[type='checkbox']").length > 0) {
                $(selectedDiv[i]).css("width", height)
                $(selectedDiv[i]).attr("width", height + "px")
                $("#field_width").val(height)
            }
        }
        setHeightWidthGroupingDiv();
    })

    $("#pos-left-plus").click(function () {
        var left = parseInt($("#field_left").val()) + 1
        $("#field_left").val(left);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("left", left)
            $(selectedDiv[i]).attr("left", left + "px")
        }
    })
    $("#pos-left-minus").click(function () {
        var left = parseInt($("#field_left").val()) - 1
        $("#field_left").val(left);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("left", left)
            $(selectedDiv[i]).attr("left", left + "px")
        }
    })
    $("#field_left").focusout(function () {
        var left = parseInt($("#field_left").val());
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("left", left)
            $(selectedDiv[i]).attr("left", left + "px")
        }
    })
    $("#field_top").focusout(function () {
        var top = parseInt($("#field_top").val());
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("top", top)
            $(selectedDiv[i]).attr("top", top + "px")
        }
    })
    $("#pos-top-plus").click(function () {
        var top = parseInt($("#field_top").val()) + 1
        $("#field_top").val(top);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("top", top)
            $(selectedDiv[i]).attr("top", top + "px")
        }
    })
    $("#pos-top-minus").click(function () {
        var top = parseInt($("#field_top").val()) - 1
        $("#field_top").val(top);
        for (var i = 0; i < selectedDiv.length; i++) {
            $(selectedDiv[i]).css("top", top)
            $(selectedDiv[i]).attr("top", top + "px")
        }
    })

    //#endregion

    $('#btnBack').click(function () {
        document.location.href = document.referrer;
    })
    $("#jSignature").jSignature();
    $("#the-svg").click(function () {
        if (issvgClick) {
            foo.enable();
            $("#ungroupElement").click();
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).removeClass("blueBorder")
                selectedDiv.splice(i, 1)
                i--;
            }
            $("#align-to-left").addClass("disabled");
            $("#align-to-top").addClass("disabled");
            $("#align-middle").addClass("disabled");
            $("#horizontal-distribute-center").addClass("disabled");
            $("#vertical-distribute-center").addClass("disabled");
            $("#align-to-bottom").addClass("disabled");
            $("#align-justify").addClass("disabled");
            $("#align-to-right").addClass("disabled");
            $("[class^='groupingDiv']").css("border", "");
            disabledIcon();
            removeallValue();
        } else {
            issvgClick = true;
        }
        controlClick();
    });
    $("#saveFilename").click(function () {
        $.ajax({
            url: renameFilenamUrl,
            type: "POST",
            data: JSON.stringify({ docTemplateId: $('#DocumentTemplateId').val(), fileName: $('.txtrenamefile').val() }),
            contentType: 'application/json',
            success: function (response) {
                if (response.status) {
                    var $span = $("<span>", {
                        text: response.filename
                    });
                    $span.addClass("pdf-name");
                    var obj = $(".pdf-input").find('.txtrenamefile')
                    $(obj).replaceWith($span);
                    $(".edit-link").show();
                    $(".pdf-name").text(response.filename);
                    $("#saveFilename").hide();
                    $("#cancelFilename").hide();
                    var array = pdfloadurl.split('?')
                    pdfloadurl = response.filepath + "?" + array[1];
                    pdfUrl = response.filepath;
                    $(".pdf-input").addClass("remove-icon")
                }
            },
            error: function () {

            }
        })
    })
    $("#cancelFilename").click(function () {
        var $span = $("<span>", {
            text: pdfname
        });
        $span.addClass("pdf-name");
        var obj = $(".pdf-input").find('.txtrenamefile')
        $(obj).replaceWith($span);
        $(".edit-link").show();
        $(".pdf-name").text(pdfname);
        $("#saveFilename").hide();
        $("#cancelFilename").hide();
        $(".pdf-input").addClass("remove-icon")
    })
    //$("#imageUpload").change(function () {
    //    if ($(this).files && $(this).files[0]) {
    //        var reader = new FileReader();
    //        reader.onload = function (e) {
    //            
    //            $(this).closest('div').css('background-image', e.target.result)
    //        };
    //        reader.readAsDataURL($(this).files[0]);
    //    }
    //})
});


function disabledPageIcon(currentpge) {
    if (currentpge > 1) {
        $("#previous-page").show();
    } else {
        $("#previous-page").hide();
    }
    if (currentpge == pdfDoc.numPages) {
        $("#next-page").hide();
    } else {
        $("#next-page").show();
    }
}
function setOutliner(anchor, event) {
    var shiftKey = event.shiftKey;
    var ctrlKey = event.ctrlKey;
    event.preventDefault();
    var start = 0;
    var end = 0;
    $("#ungroupElement").click();
    if (shiftKey) {
        $(anchor).closest('div').find('a').each(function (k, data) {
            if ($(this).hasClass('active'))
                start = k;
            if ($(this)[0] == $(anchor)[0])
                end = k;
        })
        if (start > end) {
            $(anchor).closest('div').find('a').each(function (k, data) {
                if (k <= start) {
                    if (k >= end) {
                        $(this).addClass("active")
                    }
                }

            })
        } else {
            $(anchor).closest('div').find('a').each(function (k, data) {
                if (k <= end) {
                    if (k >= start) {
                        $(this).addClass("active")
                    }
                }

            })
        }
        $("#job_field_label").text("")
        selectedDiv = [];
    }
    else if (ctrlKey) {
        $(anchor).addClass('active');
        $("#job_field_label").text("")
        selectedDiv = [];
    }
    else {
        $(anchor).closest('div').find('.active').removeClass('active');
        $(anchor).addClass('active');
        $("#job_field_label").text($(".items.active").text())
    }
    if (oldSelect.length > 0) {
        for (var j = 0; j < oldSelect.length; j++) {
            for (i = 0; i < selectedDiv.length; i++) {
                if ($(selectedDiv[i]).attr("fieldname") == oldSelect[j]) {
                    selectedDiv.splice(i, 1);
                }
            }
            $("[fieldname='" + oldSelect[j] + "']").closest('div').removeClass("blueBorder")
        }

    }
    $(anchor).closest('div').find('.active').each(function () {
        if ($(this).attr('id') != 'select') {
            flagOutliner = false;
            var fieldname = $(this).attr('id');
            if (fieldname.indexOf("Textbox_") != -1) {
                fieldname = fieldname.replace("Textbox", "undefined")
            }
            if (fieldname.indexOf("Textarea_") != -1) {
                fieldname = fieldname.replace("Textarea", "undefined")
            }
            if (fieldname.indexOf("Signature_") != -1) {
                fieldname = fieldname.replace("Signature", "undefined")
            }
            if (fieldname.indexOf("Checkbox_") != -1) {
                fieldname = fieldname.replace("Checkbox", "undefined")
            }
            if (fieldname.indexOf("Image_") != -1) {
                fieldname = fieldname.replace("Image", "undefined")
            }
            if (fieldname.indexOf("Draw_") != -1) {
                fieldname = fieldname.replace("Draw", "undefined")
            }
            if ($("#document-paging").val() == 1) {
                if ($("[fieldname='" + fieldname + "']").length == 0) {
                    OutlinerNextPage(fieldname, event)
                } else {
                    OutlinerField(fieldname, shiftKey, ctrlKey, event)
                }
            } else {
                OutlinerField(fieldname, shiftKey, ctrlKey, event)
            }
            oldSelect.push(fieldname)
        } else {
            oldSelect = [];
        }
    })
    if (shiftKey || ctrlKey) {
        //if (ctrlKey) {
        selectedDiv = [];
        $(".blueBorder").each(function (k, data) {

            selectedDiv.push(data);
        })
        //}
        $("#groupElement").click();
    }
}

var issvgClick = true;
function setSelectable() {
    foo = new Selectables({
        elements: '.added',
        selectedClass: 'blueBorder',
        enable: true,
        zone: '#holder',
        start: function (e) {
            $("#ungroupElement").click();
            selectedDiv = [];
        },
        onSelect: function (e) {

            selectedDiv.push($(e));
        }
        ,
        stop: function (e) {
            selectable = true;
            cntrlIsPressed = true;
            for (i = 0; i < selectedDiv.length; i++) {
                InputControlClick($(selectedDiv[i]), e);

            }
            selectable = false;
            cntrlIsPressed = false;
            disabledIcon();

            $("#groupElement").click();
            heightlightSelectionOutliner();
            if (selectedDiv.length > 1)
                issvgClick = false;
        }
    });
}

function heightlightSelectionOutliner() {
    $('#field_outliner_dropdown').find('.items').removeClass("active")
    for (var k = 0; k < selectedDiv.length; k++) {
        $this = $(selectedDiv[k]);
        if ($this.find('input[type=text]').length > 0) {
            var fieldName = $this.find('input[type=text]').attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Textbox"), true)
            } else {
                clickItemOutliner(fieldName, true)
            }
        }
        else if ($this.find('textarea').length > 0) {
            var fieldName = $this.find('textarea').attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Textarea"), true)
            } else {
                clickItemOutliner(fieldName, true)
            }
        }
        else if ($this.find('input[type=checkbox]').length > 0) {
            var fieldName = $this.find('input[type=checkbox]').attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Checkbox"), true)
            } else {
                clickItemOutliner(fieldName, true)
            }
        }
        if ($this.attr('data-type') == 'sa' || $this.attr('data-type') == "btn") {
            var fieldName = $this.attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Signature"), true)
            } else {
                clickItemOutliner(fieldName, true)
            }
        }
        if ($this.attr('data-type') == 'dw') {
            var fieldName = $this.attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Draw"), true)
            } else {
                clickItemOutliner(fieldName, true)
            }
        }
        if ($this.attr('data-type') == 'im') {
            var fieldName = $this.attr('fieldname');
            if (fieldName.indexOf("undefined") != -1) {
                clickItemOutliner(fieldName.replace("undefined", "Image"), true)

            } else {
                clickItemOutliner(fieldName, true)
            }
        }
    }
}
function readURL(input) {
    if (input.files && input.files[0]) {
        var _URL = window.URL || window.webkitURL;
        var img;
        var reader = new FileReader();
        var width;
        var height;
        img = new Image();
        img.onload = function () {
            width = this.width;
            height = this.height;
            reader.onload = function (e) {
                $(input).parent().parent().find('.image').attr('src', e.target.result);
                $(input).parent().parent().find('.image').attr('width', width);
                $(input).parent().parent().find('.image').css('max-width', width);
                $(input).parent().parent().find('.image').css('width', width);
                $(input).parent().parent().find('.image').attr('height', height);
                $(input).parent().parent().find('.image').css('max-height', height);
                $(input).parent().parent().find('.image').css('height', height);
                $(input).parent().parent().find('.image').show();
                $('input[type=radio][name=aspectratio]').change();
            };
            reader.readAsDataURL(input.files[0]);
        };
        img.src = _URL.createObjectURL(input.files[0]);
    }
}



function filterOutliner(hashcode, isAddFilter, isTextArea, fieldType) {
    $.each(pdfItems, function (j, data) {
        if (data.Type.toString() == hashcode && data.IsTextArea == isTextArea) {
            if (isAddFilter) {
                if (data.FieldName.indexOf("undefined") != -1) {
                    found_names.push(data.FieldName.replace("undefined", fieldType))
                } else {
                    found_names.push(data.FieldName)
                }

            } else {
                if (data.FieldName.indexOf("undefined") != -1) {
                    var index = $.inArray(data.FieldName.replace("undefined", fieldType), found_names);
                    if (index != -1) {
                        found_names.splice(index, 1);
                    }
                } else {
                    var index = $.inArray(data.FieldName, found_names);
                    if (index != -1) {
                        found_names.splice(index, 1);
                    }
                }

            }
        }
    })
}

function populateOutlinerDropdown() {
    $('#field_outliner_dropdown').html('');
    found_names = found_names.filter(function (item, index) {
        return found_names.indexOf(item) === index;
    });
    //$.unique(found_names)
    $('#field_outliner_dropdown').append('<a class="items" id="select">select</a>');
    $.each(found_names, function (i, p) {
        $('#field_outliner_dropdown').append('<a class="items" id="' + p + '" onclick="setOutliner(this,event);">' + p + '</a>');
    });
}
function populateOutlinerDropdownForSelectFieldName() {
    $('#field_outliner_dropdown').html('');

    //$.unique(found_names)
    $('#field_outliner_dropdown').append('<a class="items" id="select">select</a>');
    $.each(found_names, function (i, p) {
        $('#field_outliner_dropdown').append('<a class="items" id="' + p + '" onclick="setOutliner(this,event);">' + p + '</a>');
    });
}

function setPageNumber() {
    $("#the-svg").find('.svg').each(function () {
        if ((($(this).position().top - $(".doc-editor-header").innerHeight() - $(".upper-toolbar").innerHeight() - $("#header").height() - 30) <= 0)) {
            $("#current-page").val($(this).attr("pagenumber"));
        }
    })
}

function getAngle(div) {
    var matrix = div.css('transform');
    var angle = 0;
    if (matrix !== 'none') {
        var values = matrix.split('(')[1].split(')')[0].split(',');
        var a = values[0];
        var b = values[1];
        angle = Math.round(Math.atan2(b, a) * (180 / Math.PI));
    }
    return angle;
}
function disabledIcon() {
    if ($("#zoom-dropdown").val() == "150") {
        $("#zoom-in").addClass("disabled")
    }
    else {
        $("#zoom-in").removeClass("disabled")
    }
    if ($("#zoom-dropdown").val() == "25") {
        $("#zoom-out").addClass("disabled")
    } else {
        $("#zoom-out").removeClass("disabled")
    }
    if (trackData.length > 0) {
        $("#undo").removeClass("disabled")
    } else {
        $("#undo").addClass("disabled")
    }
    if (undoData.length > 0) {
        $("#redo").removeClass("disabled")
    } else {
        $("#redo").addClass("disabled")
    }
    $("#align-to-left").addClass("disabled");
    $("#align-justify").addClass("disabled");
    $("#align-to-right").addClass("disabled");
    $("#align-to-top").addClass("disabled");
    $("#align-middle").addClass("disabled");
    $("#horizontal-distribute-center").addClass("disabled");
    $("#vertical-distribute-center").addClass("disabled");
    $("#align-to-bottom").addClass("disabled");
    if (selectedDiv.length > 0) {
        $("#delete").removeClass("disabled")
        $("#toFront").removeClass("disabled")
        $("#toBack").removeClass("disabled")
        $("#groupElement").addClass("disabled")
        $("#copy").removeClass("disabled")
        if (copyDiv.length > 0) {
            $("#paste").removeClass("disabled")
        } else {
            $("#paste").addClass("disabled")
        }
        $(".wdht-properties").find(".wdht").show();
        $(".pos-properties").show();
        $("#text-li").removeClass("disabled")
        $("#properties-li").removeClass("disabled")
        $("#rotate-90").removeClass("disabled");

        $(".field-label").show();
        var isAllCheckbox = true;
        var isAllImage = true;
        var isAllDraw = true;
        for (var k = 0; k < selectedDiv.length; k++) {
            if ($(selectedDiv[k]).find('[type="checkbox"]').length == 0) {
                isAllCheckbox = false;
            }
            if ($(selectedDiv[k]).find('.image').length == 0) {
                isAllImage = false;
            }
            if ($(selectedDiv[k]).data('type') != "dw") {
                isAllDraw = false
            }
        }
        if (isAllCheckbox) {
            $(".checkbox-properties").show();
        } else {
            $(".checkbox-properties").hide();
        }
        if (isAllImage) {
            $("#fill-width").removeClass("disabled")
            $("#fill-height").removeClass("disabled")
            $("#fill-content").removeClass("disabled")
            $("#fill-box-image").removeClass("disabled")
            $(".image-align-properties").show();
        } else {
            $("#fill-width").addClass("disabled")
            $("#fill-height").addClass("disabled")
            $("#fill-content").addClass("disabled")
            $("#fill-box-image").addClass("disabled")
            $(".image-align-properties").hide();
        }
        if (isAllDraw) {
            $(".draw-properties").show();
        } else {
            $(".draw-properties").hide();
        }

    } else {
        $("#delete").addClass("disabled")
        $("#toFront").addClass("disabled")
        $("#toBack").addClass("disabled")
        $("#copy").addClass("disabled")
        $("#paste").addClass("disabled")
        $("#fill-width").addClass("disabled")
        $("#fill-height").addClass("disabled")
        $("#fill-content").addClass("disabled")
        $("#fill-box-image").addClass("disabled")
        //$("#text-li").addClass("disabled")
        //$("#properties-li").addClass("disabled")
        $("#groupElement").addClass("disabled")
        //$("#text-li").removeClass("active")
        //$("#properties-li").removeClass("active")
        //$("#outliner-li").addClass("active")
        //$("#text-pane").removeClass("active")
        //$("#properties-pane").removeClass("active")
        //$('#outliner-pane').addClass('active');
        //clickItemOutliner('select')
    }
    if (selectedDiv.length > 1) {
        $("#toFront").addClass("disabled")
        $("#toBack").addClass("disabled")
        $("#field_maxlength").val('');
        $("#jobFields").val('');
        //clickItemOutliner('select')
        $("#text-color-spectrum").css('background-color', "rgb(0,0,0)")
        $("#bg-color-spectrum").css('background-color', "rgb(207,243,207)")
        $("#border-color-spectrum").css('background-color', "rgb(0,0,255)")
        $("#text-bold").removeClass("active")
        $("#text-underline").removeClass("active")
        $("#text-italic").removeClass("active")
        $("#groupElement").removeClass("disabled")
        //$(".wdht-properties").find(".wdht").hide();
        $(".pos-properties").hide();
        $(".image-properties").hide();
        $(".field-label").hide();
        $("#align-to-left").removeClass("disabled");
        $("#align-justify").removeClass("disabled");
        $("#align-to-right").removeClass("disabled");
        $("#align-to-top").removeClass("disabled");
        $("#align-middle").removeClass("disabled");
        $("#horizontal-distribute-center").removeClass("disabled");
        $("#vertical-distribute-center").removeClass("disabled");
        $("#align-to-bottom").removeClass("disabled");
    }
    flagGroup = false;
    for (i = 0; i < selectedDiv.length; i++) {
        if ($(selectedDiv[i]).hasClass("grouping")) {
            flagGroup = true;
            break;
        }
    }
    if (!flagGroup && selectedDiv.length > 1) {
        $("#rotate-90").removeClass("disabled");
    } else if (flagGroup) {
        $("#rotate-90").addClass("disabled");
    }
}

function removeallValue() {
    $("#field_maxlength").val('');
    $("#jobFields").val("select");
    $("#field_width").val("");
    $("#field_height").val("");
    $("#field_left").val("");
    $("#field_top").val("");
    $("#field_outliner_dropdown").find('a').removeClass("active");
    $("#field_outliner_dropdown").find("#select").addClass("active");
    $("#field_lineWidth").val("")
}
function init(fileName) {
    //$("#loading-image").show();
    var path = fileName;
    $.ajaxSetup({ cache: false });
    PDFJS.workerSrc = pdfWorkerJsUrl; //'../Scripts/pdf/pdf.worker.js';
    PDFJS.getDocument(path).then(function (pdfDoc_) {
        pdfDoc = pdfDoc_;
        $("#total-page").text(pdfDoc.numPages);
        var options = "<option value = '0'> All Pages</options>";
        if (pdfDoc.numPages > 1) {
            for (var k = 0; k < pdfDoc.numPages; k++) {
                options = options + "<option value='" + (k + 1) + "'>" + (k + 1) + " Pages</options>";
            }
        }
        $("#filter-outliner-bypage").append(options);
        if (pdfDoc.numPages == 0)
            $("#loading-image").hide();
        renderPage(1, pdfDoc.numPages);
    });
}

function setOutlinerValueForPage(div) {

    if ($(div).data('type') == "t")
        found_names.push($(div).find('input').attr('fieldname').replace("undefined", "Textbox"))
    else if ($(div).data('type') == "ta")
        found_names.push($(div).find('textarea').attr('fieldname').replace("undefined", "Textarea"))
    else if ($(div).data('type') == "sa" || $(div).data('type') == "btn")
        found_names.push($(div).attr('fieldname').replace("undefined", "Signature"))
    else if ($(div).data('type') == "c")
        found_names.push($(div).find('input').attr('fieldname').replace("undefined", "Checkbox"))
    else if ($(div).data('type') == "im")
        found_names.push($(div).attr('fieldname').replace("undefined", "Image"))
    else if ($(div).data('type') == "dw")
        found_names.push($(div).attr('fieldname').replace("undefined", "Draw"))
}
function generateGroup() {
    var k = 0;
    while (true) {
        if ($(".groupingDiv" + k).length > 0) {
            k++;
        } else {
            break;
        }
    }
    return "groupingDiv" + k;
}
function removeOptions(selectbox) {
    var i;
    for (i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}
var scale = 2;
var containerWidth = 0;
function renderPage(pageNum, totalPage) {
    $("#loading-image").show();
    pageRendering = true;

    // Using promise to fetch the page

    pdfDoc.getPage(pageNum)
        .then(function (page) {
            // Get viewport (dimensions)
            //var viewport = page.getViewport($('#the-svg').width() / page.getViewport(1.0).width);

            var viewport = page.getViewport(scale);

            // Get div#the-svg
            var container = document.getElementById('the-svg');

            if (containerWidth < viewport.width) {
                containerWidth = viewport.width;
            }

            // Set dimensions
            container.style.width = containerWidth + 'px';
            container.style.height = viewport.height + 'px';

            // SVG rendering by PDF.js
            var operatorList = page.getOperatorList()
                .then(function (opList) {
                    var svgGfx = new PDFJS.SVGGraphics(page.commonObjs, page.objs);
                    return svgGfx.getSVG(opList, viewport);
                })
                .then(function (svg) {
                    $(svg).attr('class', 'svg');
                    $(svg).attr('pagenumber', pageNum);
                    container.appendChild(svg);
                    offsetY = $(svg).offset().top - $('#the-svg').offset().top;
                    viewport.offsetY = offsetY;
                    container.style.height = offsetY + $(svg).height() + 'px';
                    //setupForm(page, viewport, --pageNum);
                    setupForm(page, viewport, pageNum);
                    applyDrop(container);
                    pageNum++;
                    if ($("#document-paging").val() == 0) {
                        if (pageNum <= totalPage) {
                            renderPage(pageNum, totalPage)
                        } else {
                            $("#loading-image").hide();
                        }
                    } else {
                        $("#loading-image").hide();
                    }

                });


            //pageNum++;
        });

}

function setupForm(content, viewport, pageNum) {

    content.getAnnotations().then(function (items) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var type = 't';
            switch (item.subtype) {
                case 'Widget':

                    if (item.fieldType == 'Btn') {
                        if (item.fieldFlags & 32768) {
                            type = 'r';
                        } else if (item.fieldFlags & 65536) {
                            type = 'btn';
                        } else {
                            type = 'c';
                        }
                    }

                    if ((type == 'r' || type == 'c') && item.fullName) {
                        $.each(item.fullName, function (i) {
                            if (i == (item.fullName.length - 1) && validation.isGlyphsNumber(this)) {
                            }
                        });
                    }
                    var value = '';
                    var fieldName = '';
                    var maxLength = 50;
                    var fontSize = '14px';

                    var fontName = eleType[type].font;
                    var horiAlign = "Left";
                    var vertAlign = "Bottom";
                    var aspectRatio = 0;
                    var isTextArea = false;
                    var textColor = "rgb(86, 91, 93)";
                    var bgcolor = "rgb(203,247,203)";
                    var bordercolor = "rgb(0,0,255)";
                    var align = "left";
                    var image = "";
                    isBold = false,
                        isItalic = false;
                    //if (!item.fullName[0].includes('undefined') && /_\d/.test(item.fullName[0])) {
                    //    item.fullName[0] = item.fullName[0].substring(0, item.fullName[0].lastIndexOf("_"));
                    //}
                    var len = $('input[fieldname="' + item.fullName[0] + '"]').length;
                    var copylen = len;
                    for (var pdfi = 0; pdfi < pdfItems.length; pdfi++) {

                        //if (item.fullName[0] != undefined && item.fullName[0].includes(pdfItems[pdfi].FieldName)) {
                        if (item.fullName[0] != undefined && item.fullName[0] == pdfItems[pdfi].FieldName) {
                            if (type == 'c') {
                                if (len > 0) {
                                    len--;
                                    continue;
                                }
                                var count = 1
                                for (var j = 0; j < i; j++) {
                                    if (items[j].fullName == item.fullName[0]) {
                                        if (copylen > 0) {
                                            copylen--;
                                            continue;
                                        }
                                        count++;
                                    }
                                }

                                value = pdfItems[pdfi].AvailableValues[pdfItems[pdfi].AvailableValues.length - count];
                            }
                            else {
                                value = pdfItems[pdfi].Value
                            }
                            fieldName = pdfItems[pdfi].FieldName;


                            if (fieldName.indexOf('Pre_') == 0)
                                fieldName = fieldName.substr(4);
                            else if (fieldName.indexOf('Conn_') == 0)
                                fieldName = fieldName.substr(5);

                            if ((type == 't' || type == 'ta') && pdfItems[pdfi].PdfItemProperties != null) {
                                if (parseInt(pdfItems[pdfi].PdfItemProperties.MaxLength) != 0)
                                    maxLength = parseInt(pdfItems[pdfi].PdfItemProperties.MaxLength);

                                if (parseInt(pdfItems[pdfi].PdfItemProperties.FontSize) != 0)
                                    fontSize = pdfItems[pdfi].PdfItemProperties.FontSize + 'px';

                                fontName = pdfItems[pdfi].PdfItemProperties.FontName;

                                if (pdfItems[pdfi].PdfItemProperties.TextColor != null && pdfItems[pdfi].PdfItemProperties.TextColor != '')
                                    textColor = pdfItems[pdfi].PdfItemProperties.TextColor;

                                if (pdfItems[pdfi].PdfItemProperties.BackgroundColor != null && pdfItems[pdfi].PdfItemProperties.BackgroundColor != '')
                                    bgcolor = pdfItems[pdfi].PdfItemProperties.BackgroundColor;

                                if (pdfItems[pdfi].PdfItemProperties.BorderColor != null && pdfItems[pdfi].PdfItemProperties.BorderColor != '')
                                    bordercolor = pdfItems[pdfi].PdfItemProperties.BorderColor;

                                if (pdfItems[pdfi].PdfItemProperties.Alignment != null && pdfItems[pdfi].PdfItemProperties.Alignment != '')
                                    align = pdfItems[pdfi].PdfItemProperties.Alignment;

                                isBold = pdfItems[pdfi].PdfItemProperties.Bold;
                                isItalic = pdfItems[pdfi].PdfItemProperties.Italic;

                            }

                            if (pdfItems[pdfi].ReadOnly) {
                                type = 'sa';

                                if (pdfItems[pdfi].PdfItemProperties.HoriAlign != null && pdfItems[pdfi].PdfItemProperties.HoriAlign != '')
                                    horiAlign = pdfItems[pdfi].PdfItemProperties.HoriAlign;

                                if (pdfItems[pdfi].PdfItemProperties.VertAlign != null && pdfItems[pdfi].PdfItemProperties.VertAlign != '')
                                    vertAlign = pdfItems[pdfi].PdfItemProperties.VertAlign;

                                if (pdfItems[pdfi].PdfItemProperties.BackgroundColor != null && pdfItems[pdfi].PdfItemProperties.BackgroundColor != '')
                                    bgcolor = pdfItems[pdfi].PdfItemProperties.BackgroundColor;

                                if (pdfItems[pdfi].PdfItemProperties.BorderColor != null && pdfItems[pdfi].PdfItemProperties.BorderColor != '')
                                    bordercolor = pdfItems[pdfi].PdfItemProperties.BorderColor;

                                aspectRatio = pdfItems[pdfi].PdfItemProperties.AspectRatio;
                            }

                            if (pdfItems[pdfi].IsTextArea) {
                                type = 'ta';
                                var newline = String.fromCharCode(13, 10);
                                value = value.replaceAll('//n', newline);
                            }
                            if (pdfItems[pdfi].IsImageField) {
                                type = 'im'
                                image = pdfItems[pdfi].Value
                            }

                            if (pdfItems[pdfi].IsDraw) {
                                type = 'dw'
                                image = pdfItems[pdfi].Value
                            }
                            break;



                        }
                    }
                    var rect = PDFJS.Util.normalizeRect(
                        viewport.convertToViewportRectangle(item.rect));
                    var elementLeft = (rect[0]);
                    var elementTop = (rect[1] + viewport.offsetY);
                    var elementWidth = (rect[2] - rect[0]) + 'px';
                    var elementHeight = (rect[3] - rect[1]) + 'px';
                    AddInitialControlOnPageLoad(type, value, fieldName, elementLeft, elementTop, elementWidth, elementHeight, maxLength, fontSize, fontName, horiAlign, vertAlign, aspectRatio, pageNum, textColor, isBold, isItalic, '', bgcolor, bordercolor, align, image);

                    break;
            }
        }

        setInputValuesAtInit();
    });
    $(".caustam-editor-outer").getNiceScroll().resize();


}
String.prototype.replaceAll = function (find, replace) {
    var result = this;
    do {
        var split = result.split(find);
        result = split.join(replace);
    } while (split.length > 1);
    return result;
};
function OutlinerField(fieldname, shiftkey, ctrlKey, event) {
    var headerHeight = $("#header").innerHeight() + $(".doc-editor-header").innerHeight() + $(".upper-toolbar").innerHeight();

    if ($("[fieldname='" + fieldname + "']").offset() != undefined) {
        var offset = $("[fieldname='" + fieldname + "']").offset().top - headerHeight;
        headerHeight = headerHeight + + $(".pageing-block").innerHeight() + $("#footer").innerHeight() + 15;
        var scroll = $('.caustam-editor-outer').scrollTop();
        if (offset > (window.innerHeight - headerHeight)) {
            offset = offset - $(".pageing-block").height()
            $('.caustam-editor-outer').animate({ scrollTop: (offset + scroll) }, 1000);
        }
        if (offset < 0) {
            $('.caustam-editor-outer').animate({ scrollTop: Math.abs(offset + scroll) }, 1000);
        }
    }
    //var offset = $("[fieldname='" + fieldname + "']").offset().top - headerHeight;
    //headerHeight = headerHeight + + $(".pageing-block").innerHeight() + $("#footer").innerHeight() + 15;
    //var scroll = $('.caustam-editor-outer').scrollTop();
    //if (offset > (window.innerHeight - headerHeight)) {
    //    offset = offset - $(".pageing-block").height()
    //    $('.caustam-editor-outer').animate({ scrollTop: (offset + scroll) }, 1000);
    //}
    //if (offset < 0) {
    //    $('.caustam-editor-outer').animate({ scrollTop: Math.abs(offset + scroll) }, 1000);
    //}
    $("[fieldname='" + fieldname + "']").closest('div').addClass("blueBorder");
    if ($("[fieldname='" + fieldname + "']").length > 0) {
        if (!shiftkey && !ctrlKey) {
            for (var i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).removeClass("blueBorder")
            }
            selectedDiv = []
        }
        for (i = 0; i < $("[fieldname='" + fieldname + "']").length; i++) {
            var outlinerDiv = $($("[fieldname='" + fieldname + "']")[i]).closest('div');
            cntrlIsPressed = true;
            selectable = true;
            InputControlClick(outlinerDiv, event)

            selectedDiv.push(outlinerDiv);
            selectable = false;
        }
    }
    cntrlIsPressed = false;
    disabledIcon();
    disabledPageIcon();
    //InputControlClick($("[fieldname='" + fieldname + "']").closest('div'))

}

function BindExportValue(jobFieldVal, value) {

    $.get(getExportFieldValueUrl + '?jobFieldVal=' + jobFieldVal, function (response) {

        if (response.length > 0) {
            var $ddl = $('#ddlExportValue');
            $ddl.html('');
            var data = response;
            for (var i = 0; i < data.length; i++) {

                var exportValue = data[i].ExportValue;
                var exportValueId = data[i].ExportValueID;
                if (value == exportValue) {
                    var option = '<option value="' + exportValue + '"id="' + exportValueId + '" selected="selected">' + exportValue + '</option>';
                    $ddl.append(option);
                }
                else {
                    var option = '<option value="' + exportValue + '"id="' + exportValueId + '">' + exportValue + '</option>';
                    $ddl.append(option);
                }
            }

            $('.export').show();
        }
    });
}

function OnCheckboxChange(obj) {

    var checked = obj.checked;
    var fieldName = $(obj).attr('fieldname');
    $(':checkbox[fieldname="' + fieldName + '"]').prop('checked', false);
    obj.checked = checked;
}
$("#holder").keydown(function (e) {
    if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = true;
    if (e.keyCode == 46) {
        if (selectedDiv) {
            for (i = 0; i < selectedDiv.length; i++) {
                $(selectedDiv[i]).remove();
                if (trackData.length == 5) {
                    trackData.splice(1, 1)
                }
                trackData.push($(selectedDiv[i]));
                selectedDiv.slice(i, 1);
                disabledIcon();

            }
        }
    } else if (e.keyCode == 38 && selectedDiv.length == 1) {
        $("#pos-top-minus").click();
        return false;
    } else if (e.keyCode == 40 && selectedDiv.length == 1) {
        $("#pos-top-plus").click();
        return false;
    } else if (e.keyCode == 37 && selectedDiv.length == 1) {
        $("#pos-left-minus").click();
        return false;
    } else if (e.keyCode == 39 && selectedDiv.length == 1) {
        $("#pos-left-plus").click();
        return false;
    }
    if (ctrlDown && e.keyCode == cKey) {
        $CopyDiv = $ActiveDiv;
        return false;
    }

    if (ctrlDown && e.keyCode == vKey && $CopyDiv != null) {
        var _inputType = $CopyDiv.attr('data-type');

        var $inputControl = null;
        if (_inputType == 't') {
            $inputControl = $CopyDiv.find('input[type="text"]').first();
        }
        else if (_inputType == 'ta') {
            $inputControl = $CopyDiv.find('textarea').first();
        }
        else if (_inputType == 'c') {
            $inputControl = $CopyDiv.find('input[type="checkbox"]').first();
        }
        else if (_inputType == 'sa' || _inputType == "btn") {
            $inputControl = $CopyDiv;
        }


        var _canvas = $('#the-svg'),
            _canvasTop = _canvas.offset().top,
            _canvasLeft = _canvas.offset().left,
            _inputValue = $inputControl.val(),
            _fieldName = $inputControl.attr("fieldname")
        if (_fieldName.includes("undefined")) {
            var i = generateFieldName();
            _fieldName = "undefined_" + i;
        }
        _elementLeft = $CopyDiv.offset().left - _canvasLeft + 10,
            _elementTop = $CopyDiv.offset().top - _canvasTop + 10,
            _elementWidth = $CopyDiv.width() + 2 + 'px',
            _elementHeight = $CopyDiv.height() + 2 + 'px',
            _maxLength = $inputControl.prop('maxlength'),
            _fontSize = $inputControl.closest('.added').css('font-size'),
            _fontName = $inputControl.closest('.added').css('font-family'),
            _horiAlign = $inputControl.attr("horialign"),
            _vertAlign = $inputControl.attr("vertalign"),
            _aspectRatio = $inputControl.attr("aspectratio"),
            _pageNum = $inputControl.attr("pagenumber");
        _textColor = $inputControl.css("color");
        _isBold = $inputControl.css('font-weight') == "bold" ? true : false;
        _isItalic = $inputControl.css('font-style') == "italic" ? true : false;

        _bgcolor = $CopyDiv.css('background-color');
        _bordercolor = $CopyDiv.css('border-left-color');
        _align = $CopyDiv.css('text-align');
        AddInitialControlOnPageLoad(_inputType, _inputValue, _fieldName, _elementLeft, _elementTop, _elementWidth, _elementHeight, _maxLength, _fontSize, _fontName, _horiAlign, _vertAlign, _aspectRatio, _pageNum, _textColor, _isBold, _isItalic, '', _bgcolor, _bordercolor, _align);
        ctrlDown = false
        return false;
    }
}).keyup(function (e) {
    if (e.keyCode == ctrlKey || e.keyCode == cmdKey) ctrlDown = false;
});


$("#btnDownloadFile").click(function () {
    //$("#popupSelectOptionForDocumentPreviewDownload").modal({ backdrop: 'static', keyboard: false });
    //$(".preview-block").hide();
    //$(".download-block").show();
    window.location.href = downloadDocumentUrl + "?docTemplateId=" + $('#DocumentTemplateId').val() + "&isWithoutBg=" + $("#without-bg-color").prop("checked") + "&isWithoutBorder=" + $("#without-border-color").prop("checked");

})

$('#btnSaveFile').click(function (e) {

    var allFieldNames = [];
    //$('.added').each(function () {
        
    //});

    $('.groupingDiv').each(function () {
        var left = $(this).position().left - 10;
        var top = $(this).position().top - 10;
        $(this).children('div').each(function () {
            var childLeft = $(this).position().left;
            var childTop = $(this).position().top;
            $(this).css("position", "absolute")
            $(this).css("left", (left + childLeft) + "px")
            $(this).css("top", (top + childTop) + "px")
        })
        var cnt = $(this).contents();
        $(this).replaceWith(cnt);
    })
    e.preventDefault();
    var data = [];
    $('.added').each(function () {

        var $this = $(this);
        var divwidth = 0;
        var divheight = 0;
        var angle = getAngle($this);

        if (angle == 90 || angle == 270) {
            divwidth = $this.height();
            divheight = $this.width();
        } else {
            divwidth = $this.width();
            divheight = $this.height();
        }
        var pageNumber = parseInt($(this).attr('pagenumber'));
        var canvastop = $('#the-svg').offset().top;
        var canvasleft = $('#the-svg').offset().left;

        var divtop = $this.offset().top - ($($('#the-svg').find('.svg')[pageNumber - 1]).offset().top - canvastop);
        var divleft = $this.offset().left;

        divtop = ((divtop - canvastop) / scale) + canvastop;
        divleft = ((divleft - canvasleft) / scale) + canvasleft;

        canvastop = canvastop + $($('#the-svg').find('.svg')[pageNumber - 1]).height() / scale;
        divtop = divtop + (divheight + 2) / scale;

        var llx = divleft - canvasleft;
        var lly = canvastop - divtop;
        var obj = {
            type: $this.data('type'),
            'font-size': $this.css('font-size'),
            llx: llx,
            lly: lly,        // childOffset.top - $this.height(),
            urx: llx + (divwidth + 2) / scale,
            ury: lly + (divheight + 2) / scale,
            'font-family': $this.css('font-family'),
            group: $this.data('group'),
            check: $this.find('input[type=checkbox]').is(":checked"),
            radio: $this.find("input[type=radio]:checked").val() == "on" ? true : false,
            pagenumber: pageNumber,
            //underline: $this.find('input[type=text]').length>0? $this.find('input[type=text]').css('text-decoration'):'none',
            //fontWeighttxt: $this.find('textarea').length>0? $this.find('textarea').css('font-weight'):'normal',
            //fontStyletxt: $this.find('textarea').length>0? $this.find('textarea').css('font-style'):'normal',
            //underlinetxt: $this.find('textarea').length>0? $this.find('textarea').css('text-decoration'):'none',
        }


        var generalFieldName;

        if (obj.type == 't') {
            generalFieldName = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').attr('fieldname') : null;
            var newFieldName;
            if (!generalFieldName || allFieldNames.indexOf(generalFieldName) !== -1) {
                var i = generateFieldName();
                newFieldName = "undefined_" + i;
                $this.find('input[type=text]').attr('fieldname', newFieldName);
                obj['fieldname'] = newFieldName;
            }
            else
                obj['fieldname'] = generalFieldName;

            //obj['fieldname'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').attr('fieldname') : null;
            obj['value'] = $this.find('input[type=text]').val().toLowerCase() == "off" ? "" : $this.find('input[type=text]').val();
            //obj['value'] = $this.find('input[type=text]').val();
            obj['vertalign'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').attr('vertalign') : null;
            obj['horialign'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').attr('horialign') : null;
            obj['aspectratio'] = $('input[type="radio"][name="aspectratio"]:checked').val();
            obj['maxlength'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').attr('maxlength') : null;
            obj['textcolor'] = $this.find('input[type=text]').length > 0 ? rgb2hex($this.find('input[type=text]').css('color')) : null;
            obj['bgcolor'] = $this.find('input[type=text]').length > 0 ? rgb2hex($this.css('background-color')) : null;
            obj['bordercolor'] = $this.find('input[type=text]').length > 0 ? rgb2hex($this.css('border-left-color')) : null;
            var bold = $this.find('input[type=text]').css('font-weight') == "700" ? 'bold' : 'normal'
            obj['fontWeight'] = $this.find('input[type=text]').length > 0 ? bold : 'normal';
            obj['fontStyle'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').css('font-style') : 'normal';
            obj['textDecoration'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').css('text-decoration') : 'none';
            obj['textAlign'] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').css('text-align') : 'left';
            obj["font-size"] = $this.find('input[type=text]').length > 0 ? $this.find('input[type=text]').css('font-size') : '10';

        }
        else if (obj.type == 'ta') {
            generalFieldName = $this.find('textarea').length > 0 ? $this.find('textarea').attr('fieldname') : null;
            var newFieldName;
            if (!generalFieldName || allFieldNames.indexOf(generalFieldName) !== -1) {
                var i = generateFieldName();
                newFieldName = "undefined_" + i;
                $this.find('textarea').attr('fieldname', newFieldName);
                obj['fieldname'] = newFieldName;
            }
            else
                obj['fieldname'] = generalFieldName;

            obj['fieldname'] = $this.find('textarea').length > 0 ? $this.find('textarea').attr('fieldname') : null;
            obj['value'] = $this.find('textarea').val();
            obj['maxlength'] = $this.find('textarea').length > 0 ? $this.find('textarea').attr('maxlength') : null;
            obj['textcolor'] = $this.find('textarea').length > 0 ? rgb2hex($this.find('textarea').css('color')) : null;
            obj['bgcolor'] = $this.find('textarea').length > 0 ? rgb2hex($this.css('background-color')) : null;
            obj['bordercolor'] = $this.find('textarea').length > 0 ? rgb2hex($this.css('border-left-color')) : null;
            var bold = $this.find('textarea').css('font-weight') == "700" ? 'bold' : 'normal'
            obj['fontWeight'] = $this.find('textarea').length > 0 ? bold : 'normal';
            obj['fontStyle'] = $this.find('textarea').length > 0 ? $this.find('textarea').css('font-style') : 'normal';
            obj['textDecoration'] = $this.find('textarea').length > 0 ? $this.find('textarea').css('text-decoration') : 'normal';
            obj['textAlign'] = $this.find('textarea').length > 0 ? $this.find('textarea').css('text-align') : 'left';
            obj["font-size"] = $this.find('textarea').length > 0 ? $this.find('textarea').css('font-size') : '10';
            var lineHeight = $this.find('textarea').css('line-height').match(/\d/g).join("");
            obj["line-height"] = $this.find('textarea').length > 0 ? lineHeight : '18';

        }
        else if (obj.type == 'c') {
            generalFieldName = $this.find('input[type=checkbox]').length > 0 ? $this.find('input[type=checkbox]').attr('fieldname') : null;
            var newFieldName;
            if (!generalFieldName || allFieldNames.indexOf(generalFieldName) !== -1) {
                var i = generateFieldName();
                newFieldName = "undefined_" + i;
                $this.find('input[type=checkbox]').attr('fieldname', newFieldName);
                obj['fieldname'] = newFieldName;
            }
            else
                obj['fieldname'] = generalFieldName;

            obj['value'] = $this.find('input[type=checkbox]').val();
            obj["check"] = $this.find('input[type=checkbox]').prop('checked');
        }
        else if (obj.type == 'sa' || obj.type == 'btn' || obj.type == 'dw') {
           
            datapair = $this.jSignature("getData", "image")
            obj['sign'] = datapair.join(",");
            obj['base30string'] = $this.jSignature("getData", "base30").join(",")

            generalFieldName = $this.length > 0 ? $this.attr('fieldname') : null;
            var newFieldName;
            if (!generalFieldName || allFieldNames.indexOf(generalFieldName) !== -1) {
                var i = generateFieldName();
                newFieldName = "undefined_" + i;
                $this.attr('fieldname', newFieldName);
                obj['fieldname'] = newFieldName;
            }
            else
                obj['fieldname'] = generalFieldName;

            //obj['fieldname'] = $this.length > 0 ? $this.attr('fieldname') : null;
            obj['vertalign'] = $this.length > 0 ? $this.attr('vertalign') : null;
            obj['horialign'] = $this.length > 0 ? $this.attr('horialign') : null;
            obj['aspectratio'] = $this.length > 0 ? $this.attr('aspectratio') : null;
            var id = $this.attr('id');
            obj['lineWidth'] = $("#" + id).find("canvas.jSignature").add($("#" + id).filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'];
            obj['bgcolor'] = $this.css('background-color') != '' ? rgb2hex($this.css('background-color')) : null;
            obj['bordercolor'] = $this.css('border-left-color') != '' ? rgb2hex($this.css('border-left-color')) : null;
        }
        else if (obj.type == 'im') {
            datapair = $this.find('.image').attr('src') != "#" ? $this.find('.image').attr('src') : "";
            obj['sign'] = datapair;

            generalFieldName = $this.find('.image').length > 0 ? $this.attr('fieldname') : null;
            var newFieldName;
            if (!generalFieldName || allFieldNames.indexOf(generalFieldName) !== -1) {
                var i = generateFieldName();
                newFieldName = "undefined_" + i;
                $this.attr('fieldname', newFieldName);
                obj['fieldname'] = newFieldName;
            }
            else
                obj['fieldname'] = generalFieldName;

            //obj['fieldname'] = $this.find('.image').length > 0 ? $this.attr('fieldname') : null;
            obj['aspect-ratio'] = $this.find('.image').length > 0 ? $this.find('.image').attr("width") / $this.find('.image').attr("height") : 1;
            if ($this.attr("fill-width") == 1) {
                obj['fill-option'] = 1
            }
            if ($this.attr("fill-height") == 1) {
                obj['fill-option'] = 2
            }
            if ($this.attr("fill-content") == 1) {
                obj['fill-option'] = 3
            }
            obj['width'] = $this.find('.image').length > 0 ? $this.find('.image').width() : 0;
            obj['height'] = $this.find('.image').length > 0 ? $this.find('.image').height() : 0;
            obj["align"] = $this.attr("align") != "" ? $this.attr("align") : "1";
        }
        data.push(obj);

        allFieldNames.push(generalFieldName);

    });
    $.ajax({
        url: saveOpenDocumentTemplateUrl + '?docTemplateId=' + $('#DocumentTemplateId').val(),
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.status) {
                showSuccessMessage("Document Template has been updated successfully.");
                $(".caustam-editor-outer").getNiceScroll().resize();
                //setTimeout(function () { window.open(jobSettingUrl, "_self"); }, 300);
            }
            else {
                showErrorMessage(response.error);
            }
        },
        error: function () {
            showErrorMessage("Document Template has not been updated.");
        }
    });
});
function signatureChange(obj) {
    clickSignId = $(obj).attr('id');

    var bol = $('#' + clickSignId).jSignature("isModified")
    if (bol) {
        if ($(obj).find("#redoSignBtn").length == 0) {
            $(obj).append('<input type="button" class="reset" onclick=resetClick() id="resetBtn" value="Clear Draw" style="position: absolute; top: auto; margin: 0px !important; left: 10px;">');
            //$(obj).append('<input type="button" class="redo" onclick=redoClick(this) id="redoSignBtn" value="redo last stroke" style="position: absolute; top: auto; margin: 0px !important; left: 136px;">');
        }
    }
}

function resetClick() {
    $('#' + clickSignId).jSignature('reset');
}
function AddInitialControlOnPageLoad(inputType, inputValue, fieldName, elementLeft, elementTop, elementWidth, elementHeight, maxLength, fontSize, fontName, horiAlign, vertAlign, aspectRatio, pageNum, textColor, isBold, isItalic, isDraggable, bgcolor, bordercolor, align, imageBase64) {
    inputValue = inputValue != undefined ? inputValue : '';
    var type = inputType
    var $canvas = $('#the-svg');
    var canvasTop = $canvas.offset().top;
    var canvasLeft = $canvas.offset().left;
    elementLeft += canvasLeft;
    elementTop += canvasTop;

    var fontFamily = fontName != '' ? 'font-family:' + fontName : '';
    var fontWeight = isBold == true ? 'font-weight: bold;' : '';
    var fontStyle = isItalic == true ? 'font-style: italic;' : '';
    var fontAlign = "text-align : left;"
    fontSize = Math.round(parseInt(fontSize, 10)) + "px"
    if (align == "right") {
        fontAlign = "text-align : right;"
    } else if (align == "center") {
        fontAlign = "text-align : center;"
    }
    var i = generateSignatureId();
    if (type == "t") {
        if (inputValue.toLowerCase() == "off")
            inputValue = '';
        var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted ' + bordercolor + ';opacity:0.8;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';' + fontFamily + ' ;font-size: ' + fontSize + '"><input class="control" value="' + (inputValue) + '" type="text" maxlength="' + maxLength + '"  fieldname="' + fieldName + '" style="width:100%; height:100%; background:transparent; border:0px; ' + fontWeight + fontStyle + fontAlign + ' color:' + textColor + ';"></input></div>');
    }
    else if (type == "ta") {
        var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted ' + bordercolor + ';opacity:0.8;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';' + fontFamily + ' ;font-size: ' + fontSize + '"><textarea class="control" value="' + (inputValue) + '" fieldname="' + fieldName + '" maxlength="' + maxLength + '" style="width:100%; height:100%; background:transparent; border:0px; ' + fontWeight + fontStyle + fontAlign + ' color:' + textColor + ';"></textarea></div>');
    }
    else if (type == "r") {
        var $div = $('<div data-type=' + type + ' class="added"  style="border: 1px dotted rgb(0, 0, 0);width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;"><input id="radbtn" class="control" type="radio" style="width:100%; height:100%; background:transparent"></input></div>');
    }
    else if (type == "c") {
        var checked = inputValue.toLowerCase() == "yes" ? 'checked=checked' : '';
        //<input class="control" value="'+ inputValue +'" type="checkbox" ' + checked + '" fieldname="'+ fieldName +'" onchange="OnCheckboxChange(this)" style="width:100%; height:100%; background:transparent"></input>
        var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted rgb(0, 0, 0);width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;"><label class="checkbox"><input type = "checkbox" class="icheck" value="' + inputValue + '"' + checked + '" fieldname="' + fieldName + '" onchange="OnCheckboxChange(this)"><span></span></label ></div>');
    }
    else if (type == 'sa' || type == "btn") {

        var $div = $('<div tabindex=1  id="jSignature' + i + '" data-type= "' + type + '"fieldname="' + fieldName + '" class="added signature" onclick=signatureChange(this) style="border: 1px dotted ' + bordercolor + ';opacity:0.8;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';"></div>');
    }
    else if (type == "dw") {
        var $div = $('<div tabindex=1 id="jSignature' + i + '" data-type= "' + type + '"fieldname="' + fieldName + '" class="added drawing" onclick=signatureChange(this) style="border: 1px dotted ' + bordercolor + ';opacity:0.8;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';"></div>');
    }
    else if (type == 'im') {
        if (imageBase64 != '' && imageBase64.split(',')[1] != "") {
            var $div = $('<div data-type=' + type + ' fieldname= "' + fieldName + '" class="added"  style="overflow:hidden;border: 1px dotted rgb(0, 0, 0);opacity:1;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';"><img src="' + imageBase64 + '" class="image" /><div class="fileUpload primary document_imageUpload"><span>Upload</span><input type="file" class="upload" value="Upload" accept="image/x-png,image/gif,image/jpeg" onchange="readURL(this);"/></div></div>');
        } else {
            var $div = $('<div data-type=' + type + ' fieldname= "' + fieldName + '" class="added"  style="overflow:hidden;border: 1px dotted rgb(0, 0, 0);opacity:1;width:' + elementWidth + ';height:' + elementHeight + ';position:absolute;background:' + bgcolor + ';"><img class="image"  alt="No Image" style="display:none;"/><div class="fileUpload primary document_imageUpload"><span>Upload</span><input type="file" class="upload" value="Upload" accept="image/x-png,image/gif,image/jpeg" onchange="readURL(this);"/></div></div>');
        }

    }
    $div.attr('pagenumber', pageNum);

    $('#holder').append($div);

    if (inputType == 'ta') {
        $div.find('textarea').val(inputValue);
    }

    $div.offset({ left: elementLeft, top: elementTop });

    $div.click(function (event) {
        // alert(2);
        setTimeout(InputControlClick($(this), event), 100);
        //InputControlClick($(this), event);

    });

    if ($div.data('type') == "sa" || type == "btn") {
        $('#jSignature' + i).jSignature();
        var margin = ($div.height() * 15) / 100;
        $div.find('canvas').css('margin-top', margin)
        $div.find('canvas').css('margin-bottom', margin)
        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas($div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])

    }
    if ($div.data('type') == "dw") {
        $('#jSignature' + i).jSignature();
        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = false;
        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas($div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])

    }
    $('.added .control').click(function () {
        $(this).focus();
    });
    var click = {
        x: 0,
        y: 0
    };
    $div.draggable({
        start: function (event) {
            click.x = event.clientX;
            click.y = event.clientY;
        },

        drag: function (event, ui) {

            // This is the parameter for scale()
            var zoom = parseFloat($("#zoom-dropdown").val() / 100);

            var original = ui.originalPosition;

            // jQuery will simply use the same object we alter here
            ui.position = {
                left: (event.clientX - click.x + original.left) / zoom,
                top: (event.clientY - click.y + original.top) / zoom
            };

        },
        cancel: 'canvas',
        containment: '.ui-droppable',
        scroll: true
    }).resizable({
        containment: '.ui-droppable',
        handles: 'nw,ne,sw,se',
        stop: function (event, ui) {
            setTimeout(function () {

                setAspectRatioResize()
                if ($(ui.element).data('type') == "im") {
                    if ($(ui.element).attr("fill-height") == 1) {
                        $("#fill-height").click();
                    }
                    else if ($(ui.element).attr("fill-width") == 1) {

                        $("#fill-width").click();

                    } else if ($(ui.element).attr("fill-content") == 1) {

                        $("#fill-content").click();
                    }
                }
            }, 500)
        },
        resize: function (event, ui) {
            var changeWidth = ui.size.width - ui.originalSize.width; // find change in width
            var newWidth = ui.originalSize.width + changeWidth / parseFloat($("#zoom-dropdown").val() / 100); // adjust new width by our zoomScale

            var changeHeight = ui.size.height - ui.originalSize.height; // find change in height
            var newHeight = ui.originalSize.height + changeHeight / parseFloat($("#zoom-dropdown").val() / 100); // adjust new height by our zoomScale

            ui.size.width = newWidth;
            ui.size.height = newHeight;
            if ($(ui.element).data('type') == "dw") {
                $(ui.element).find(".jSignature").attr("width", newWidth + "px")
                $(ui.element).find(".jSignature").attr("height", newHeight + "px")
                $('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
            }
        }
    });
    return $div;
}

//#region outliner select when page one by one
function OutlinerPreviousPage(fieldname, event) {
    if ($("#the-svg .svg").is(':visible')) {
        if ($("#current-page").val() > 1 && $("[fieldname='" + fieldname + "']").length == 0) {
            for (i = parseInt($("#current-page").val()); i >= 1; i--) {
                if ($("[fieldname='" + fieldname + "']").length == 0) {
                    if ($("#current-page").val() > 0) {
                        $("#previous-page").click();
                        setTimeout(function () { OutlinerPreviousPage(fieldname) }, 200);
                    }

                } else {
                    OutlinerField(fieldname, false, false, event)

                }
                break;
            }
        }
        else if ($("[fieldname='" + fieldname + "']").length != 0) {
            OutlinerField(fieldname, false, false, event)
            return;
        }
    }
    else {
        setTimeout(function () { OutlinerPreviousPage(fieldname, event) }, 200);
    }
}


function OutlinerNextPage(fieldname, event) {
    if ($("#the-svg .svg").is(':visible')) {
        if ($("#current-page").val() == pdfDoc.numPages && $("[fieldname='" + fieldname + "']").length == 0) {
            OutlinerPreviousPage(fieldname, event)
            return;
        }
        else if ($("[fieldname='" + fieldname + "']").length != 0) {
            OutlinerField(fieldname, false, false, event)
            return;
        }
        for (i = parseInt($("#current-page").val()); i <= pdfDoc.numPages; i++) {
            if ($("[fieldname='" + fieldname + "']").length == 0) {
                $("#next-page").click();
                setTimeout(function () { OutlinerNextPage(fieldname, event) }, 200);
            } else {

                OutlinerField(fieldname, false, false, event)
            }
            break;
        }
    }
    else {
        setTimeout(function () { OutlinerNextPage(fieldname, event) }, 200);
    }
}
//#endregion

function InputControlClick($this, event) {
    if (!$this.hasClass("grouping") && !$this.is('[class*="groupingDiv"]') && !$this.hasClass("ui-resizable-handle") && !$this.hasClass("fileUpload")) {
        $("#ungroupElement").click();
    }
    if (!$this.hasClass("ui-resizable-handle") && !$this.hasClass("fileUpload") && !$this.is('[class*="groupingDiv"]')) {
        if (!selectable) {
            if (cntrlIsPressed) {
                for (var i = 0; i < selectedDiv.length; i++) {
                    if ($(selectedDiv[i])[0] == $this[0]) {
                        $this.removeClass("blueBorder")
                        selectedDiv.splice(i, 1);
                        flag = true;
                    }
                }
                if (!flag) {
                    $this.addClass("blueBorder")

                    selectedDiv.push($this);
                }
            } else {
                if (event != undefined && !event.shiftKey) {
                    for (var i = 0; i < selectedDiv.length; i++) {
                        $($(selectedDiv[i])).removeClass("blueBorder")
                    }
                    selectedDiv = [];
                }
                $this.addClass("blueBorder")

                selectedDiv.push($this);
            }
        }
        var flag = false;
        var textColor = '';
        var bgcolor = '';
        var bordercolor = '';
        var top = $('#holderParentDiv').offset().top;
        if ($('#holderParentDiv').scrollTop() <= 0) {
            top = $('#holderParentDiv').offset().top + 30;
        }
        $("#field-properties").show();
        if ($this.find('input[type="checkbox"]').length > 0) {
            $('#jobFieldValue').val($this.find('input').attr('fieldname'))
            $(".color-properties").hide();
            BindExportValue($this.find('input[type="checkbox"]').attr('fieldname'), $this.find('input[type="checkbox"]').val());
            if ($this.find('input[type=checkbox]').prop("checked")) {
                $('#onOffSwitchChkChecked').prop('checked', true);
            } else {
                $('#onOffSwitchChkChecked').prop('checked', false);
            }
        }
        if ($this.data('type') == "btn" || $this.data('type') == "sa" || $this.data('type') == "dw") {
            $('#jobFieldValue').val($this.attr('fieldname'))
            $('#ddlLineWidth').val($this.attr('lineWidth'))
            var margin = ($this.height() * 15) / 100;
            $this.find('canvas').css('margin-top', margin)
            $this.find('canvas').css('margin-bottom', margin)
            bgcolor = rgb2hex($this.css('background-color'));
            $("#bg-color-spectrum").css('background-color', bgcolor)
            bordercolor = rgb2hex($this.css('border-left-color'));
            $("#border-color-spectrum").css('background-color', bordercolor)
        }

        if ($this.find('input[type="text"]').length > 0 || $this.find('textarea').length > 0) {
            $(".color-properties").show()
            if ($this.find('input[type="text"]').length > 0)
                $('#jobFieldValue').val($this.find('input').attr('fieldname'))
            else
                $('#jobFieldValue').val($this.find('textarea').attr('fieldname'))
            var isbold = false;
            var isitalic = false;
            var fontAlignment = "left";
            var fontsize = 14;
            if ($this.find('input[type="text"]').length > 0)
                textColor = rgb2hex($this.find('input[type="text"]').css('color'));
            else
                textColor = rgb2hex($this.find('textarea').css('color'));

            if ($this.find('input[type="text"]').length > 0)
                isbold = $this.find('input[type="text"]').css('font-weight') == "700" ? true : false
            else
                isbold = $this.find('textarea').css('font-weight') == "700" ? true : false

            if ($this.find('input[type="text"]').length > 0)
                isitalic = $this.find('input[type="text"]').css('font-style') == "italic" ? true : false
            else
                isitalic = $this.find('textarea').css('font-style') == "italic" ? true : false
            if ($this.find('input[type="text"]').length > 0)
                fontsize = $this.find('input[type="text"]').css('font-size');
            else
                fontsize = $this.find('textarea').css('font-size');

            $("#font-size").val(parseInt(fontsize) + "px");
            $("#text-color-spectrum").css('background-color', textColor)
            bgcolor = rgb2hex($this.css('background-color'));
            $("#bg-color-spectrum").css('background-color', bgcolor)
            bordercolor = rgb2hex($this.css('border-left-color'));
            $("#border-color-spectrum").css('background-color', bordercolor)
            if (isbold) {
                $("#text-bold").addClass("active")
            } else {
                $("#text-bold").removeClass("active")
            }
            if (isitalic) {
                $("#text-italic").addClass("active")
            }
            else {
                $("#text-italic").removeClass("active")
            }
            if ($this.find('input[type="text"]').length > 0) {
                if ($this.find('input').css('text-align') == "right") {
                    $("#text-right").addClass("active");
                    $("#text-center").removeClass("active");
                    $("#text-left").removeClass("active");
                } else if ($this.find('input').css('text-align') == "center") {
                    $("#text-center").addClass("active");
                    $("#text-left").removeClass("active");
                    $("#text-right").removeClass("active");
                } else {
                    $("#text-left").addClass("active");
                    $("#text-center").removeClass("active");
                    $("#text-right").removeClass("active");
                }
            } else {
                if ($this.find('textarea').css('text-align') == "right") {
                    $("#text-right").addClass("active");
                    $("#text-center").removeClass("active");
                    $("#text-left").removeClass("active");
                } else if ($this.find('textarea').css('text-align') == "center") {
                    $("#text-center").addClass("active");
                    $("#text-left").removeClass("active");
                    $("#text-right").removeClass("active");
                } else {
                    $("#text-left").addClass("active");
                    $("#text-center").removeClass("active");
                    $("#text-right").removeClass("active");
                }
            }


            $("#FontFamily").val($this.css("font-family").toLowerCase())
            $('.export').hide();



            //$("#fullPicker").val(textColor);
        }
        $("#bg-color-spectrum").spectrum({
            color: $("#bg-color-spectrum").css('background-color'),
            showInput: true,
            className: "full-spectrum bg-color-spectrum",
            showInitial: true,
            showPalette: true,
            showSelectionPalette: true,
            maxSelectionSize: 10,
            preferredFormat: "hex",
            localStorageKey: "spectrum.demo",
            move: function (color) {

            },
            show: function (color) {
                ischange = false;
                previousColor = color;
            },
            beforeShow: function () {

            },
            hide: function () {
                if (ischange && previousColor) {
                    $("#bg-color-spectrum").css("background-color", $(".bg-color-spectrum").find(".sp-input").val());
                    for (i = 0; i < selectedDiv.length; i++) {
                        $(selectedDiv[i]).css("background-color", $(".bg-color-spectrum").find(".sp-input").val())
                    }
                }
            },
            change: function () {
                ischange = true;
                $("#bg-color-spectrum").css("background-color", $(".bg-color-spectrum").find(".sp-input").val());
                for (i = 0; i < selectedDiv.length; i++) {
                    $(selectedDiv[i]).css("background-color", $(".bg-color-spectrum").find(".sp-input").val())
                }
            },
            palette: [
                ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)"],
                ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                    "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
                ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                    "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                    "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                    "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                    "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                    "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                    "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                    "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                    "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                    "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
            ]
        });
        $("#text-color-spectrum").spectrum({
            color: $("#text-color-spectrum").css('background-color'),
            showInput: true,
            className: "full-spectrum text-color-spectrum",
            showInitial: true,
            showPalette: true,
            showSelectionPalette: true,
            maxSelectionSize: 10,
            preferredFormat: "hex",
            localStorageKey: "spectrum.demo",
            move: function (color) {

            },
            show: function (color) {
                ischange = false;
                previousColor = color;
            },
            beforeShow: function () {

            },
            hide: function () {
                if (ischange && previousColor) {
                    $("#text-color-spectrum").css("background-color", $(".text-color-spectrum").find(".sp-input").val());
                    for (i = 0; i < selectedDiv.length; i++) {
                        $(selectedDiv[i]).find('input').css("color", $(".text-color-spectrum").find(".sp-input").val())
                        $(selectedDiv[i]).find('textarea').css("color", $(".text-color-spectrum").find(".sp-input").val())
                    }
                }
            },
            change: function () {
                ischange = true;
                $("#text-color-spectrum").css("background-color", $(".text-color-spectrum").find(".sp-input").val());
                for (i = 0; i < selectedDiv.length; i++) {
                    $(selectedDiv[i]).find('input').css("color", $(".text-color-spectrum").find(".sp-input").val())
                    $(selectedDiv[i]).find('textarea').css("color", $(".text-color-spectrum").find(".sp-input").val())
                }
            },
            palette: [
                ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)"],
                ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                    "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
                ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                    "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                    "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                    "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                    "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                    "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                    "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                    "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                    "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                    "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
            ]
        });
        $("#border-color-spectrum").spectrum({
            color: $("#border-color-spectrum").css('background-color'),
            showInput: true,
            className: "full-spectrum border-color-spectrum",
            showInitial: true,
            showPalette: true,
            showSelectionPalette: true,
            maxSelectionSize: 10,
            preferredFormat: "hex",
            localStorageKey: "spectrum.demo",
            move: function (color) {

            },
            show: function (color) {
                ischange = false;
                previousColor = color;
            },
            beforeShow: function () {

            },
            hide: function () {
                if (ischange && previousColor) {
                    $("#border-color-spectrum").css("background-color", $(".border-color-spectrum").find(".sp-input").val());
                    for (i = 0; i < selectedDiv.length; i++) {
                        $(selectedDiv[i]).css("border", "1px dotted " + $(".border-color-spectrum").find(".sp-input").val())
                        $(selectedDiv[i]).removeClass("blueBorder");
                    }
                }
            },
            change: function () {
                $("#border-color-spectrum").css("background-color", $(".border-color-spectrum").find(".sp-input").val());
                for (i = 0; i < selectedDiv.length; i++) {
                    $(selectedDiv[i]).css("border", "1px dotted " + $(".border-color-spectrum").find(".sp-input").val())
                    $(selectedDiv[i]).removeClass("blueBorder");
                }
            },
            palette: [
                ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)"],
                ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                    "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
                ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                    "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                    "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                    "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                    "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                    "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                    "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                    "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                    "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                    "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
            ]
        });



        if (selectedDiv.length != 1) {
            $("#align-to-left").removeClass("disabled");
            $("#align-justify").removeClass("disabled");
            $("#align-to-right").removeClass("disabled");
            $("#align-to-top").removeClass("disabled");
            $("#align-middle").removeClass("disabled");
            $("#horizontal-distribute-center").removeClass("disabled");
            $("#vertical-distribute-center").removeClass("disabled");
            $("#align-to-bottom").removeClass("disabled");

        } else {
            $("#align-to-left").addClass("disabled");
            $("#align-justify").addClass("disabled");
            $("#align-to-right").addClass("disabled");
            $("#align-to-top").addClass("disabled");
            $("#align-middle").addClass("disabled");
            $("#horizontal-distribute-center").addClass("disabled");
            $("#vertical-distribute-center").addClass("disabled");
            $("#align-to-bottom").addClass("disabled");
        }
        $("#ddlExportValue").change(function () {
            for (var i = 0; i < selectedDiv.length; i++) {
                var selectedExportValue = $('#ddlExportValue option:selected').attr("id");
                var fieldname = $('#jobFieldValue').val() + "_" + selectedExportValue;
                $(selectedDiv[i]).find('input[type=checkbox]').attr('fieldname', fieldname);
                $(selectedDiv[i]).find('input[type=checkbox]').val($('#ddlExportValue option:selected').val());
                $('#jobFieldLabel').val($('#jobFields').val());
                $("#job_field_label").text(fieldname);
                $('#jobFields').val(fieldname);
                //if ($("[fieldname='" + oldFieldName + "']").length <= 0) {
                //    found_names.splice($.inArray(oldFieldName, found_names), 1);
                //}
                found_names.pop();
                found_names.push(fieldname);
                populateOutlinerDropdownForSelectFieldName();
                
                //newFieldName = ui.item.value + "_" + selectedExportValue;
            }
            //source: data,
            //    select: function(event, ui) {
            //        if (ui.item) {
            //            event.preventDefault();

            //            $('#jobFieldValue').val(ui.item.value);
            //            $('#jobFieldLabel').val(ui.item.label)
            //            $('#jobFields').val(ui.item.label);
            //            BindExportValue(ui.item.value);
            //            setTimeout(function () {

            //                }
            //            }, 1000);
            //        }
            //    }


        });
        if ($this.find('input[type="checkbox"]').length > 0) {

            $('#jobFields').autocomplete({
                source: data,
                select: function (event, ui) {
                    if (ui.item) {
                        event.preventDefault();

                        $('#jobFieldValue').val(ui.item.value);
                        $('#jobFieldLabel').val(ui.item.label)
                        $('#jobFields').val(ui.item.label);
                        BindExportValue(ui.item.value);
                        setTimeout(function () {
                            var newFieldName = '';
                            for (var i = 0; i < selectedDiv.length; i++) {

                                if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
                                    var j = generateFieldName();
                                    $(selectedDiv[i]).find('input[type=checkbox]').attr('fieldname', "undefined_" + j);
                                    found_names.push("Checkbox_" + j);
                                    newFieldName = "Checkbox_" + j;


                                } else {
                                    var selectedExportValue = $('#ddlExportValue option:selected').attr("id");
                                    var fieldname = $('#jobFieldValue').val() + "_" + selectedExportValue;
                                    $(selectedDiv[i]).find('input[type=checkbox]').attr('fieldname', fieldname);
                                    $(selectedDiv[i]).find('input[type=checkbox]').val($('#ddlExportValue option:selected').val());
                                    found_names.pop();
                                    found_names.push(ui.item.value + "_" + selectedExportValue);
                                    newFieldName = ui.item.value + "_" + selectedExportValue;

                                }
                            }
                            


                            //if ($("[fieldname='" + oldFieldName + "']").length <= 0) {
                            ////        found_names.splice($.inArray(oldFieldName, found_names), 1);
                            ////    }
                            populateOutlinerDropdownForSelectFieldName();
                            //populateOutlinerDropdown()
                            clickItemOutliner(newFieldName, false)
                            //$("#field_outliner_dropdown").val(newFieldName)
                            $('#jobFieldLabel').val($('#jobFields').val())
                            $("#job_field_label").text(newFieldName);
                            $('#jobFields').val(newFieldName);

                        }

                            , 1000);
                        

                    }
                }
            });
        } else {
            $('#jobFields').autocomplete({
                source: data,
                select: function (event, ui) {

                    var fieldname_select = "";
                    if (ui.item) {
                        var fieldnameCount = 2
                        $('#jobFields').val(ui.item.label)
                        var fieldname_select = ui.item.value;
                        if (fieldname_select.split("_").length > 1)
                            fieldname_select = fieldname_select.split("_")[0] + "_" + fieldname_select.split("_")[1];
                        else
                            fieldname_select = fieldname_select.split("_")[0];
                        var fieldname_cp = fieldname_select;
                        while (true) {
                            if ($("[fieldname='" + fieldname_select + "_" + (fieldnameCount - 1) + "']").length > 0) {
                                fieldname_cp = fieldname_select + "_" + fieldnameCount;
                                fieldnameCount++;
                            } else {
                                fieldname_cp = fieldname_select + "_1"
                                break;
                            }
                        }
                        if (fieldnameCount > 2)
                            fieldname_select = fieldname_select + "_" + (fieldnameCount - 1);
                        else
                            fieldname_select = fieldname_cp
                        $('#jobFieldValue').val(fieldname_select)

                        event.preventDefault();
                        $('#jobFieldValue').val(fieldname_select);
                        $('#jobFieldLabel').val(ui.item.label)
                        $('#jobFields').val(ui.item.label);
                        $('#ddlExportValue').val('');

                    }

                    var newFieldName = '';
                    for (var i = 0; i < selectedDiv.length; i++) {
                        newFieldName = setOutlinerValue($(selectedDiv[i]), fieldname_select)
                    }
                    if ($("[fieldname='" + oldFieldName + "']").length <= 0) {
                        var index = -1;
                        var div = $("[fieldname='" + newFieldName + "']").closest("div");
                        if (div.data('type') == 't') {
                            index = $.inArray(oldFieldName.replace("undefined", "Textbox"), found_names);
                        } else if (div.data('type') == 'ta') {
                            index = $.inArray(oldFieldName.replace("undefined", "Textarea"), found_names);
                        } else if (div.data('type') == 'sa' || div.data('type') == 'btn') {
                            index = $.inArray(oldFieldName.replace("undefined", "Signature"), found_names);
                        } else if (div.data('type') == 'im') {
                            index = $.inArray(oldFieldName.replace("undefined", "Image"), found_names);
                        } else if (div.data('type') == 'dw') {
                            index = $.inArray(oldFieldName.replace("undefined", "Draw"), found_names);
                        }
                        if (index != -1) {
                            found_names.splice(index, 1);
                        }
                    }
                    populateOutlinerDropdown()
                    clickItemOutliner(newFieldName, false)
                    $('#jobFieldLabel').val($('#jobFields').val())
                    $("#job_field_label").text(newFieldName);

                }
            });

        }




        $("#field_width").val($this.outerWidth()); //border
        $("#field_height").val($this.outerHeight());
        $("#field_left").val($this.position().left);
        $("#field_top").val($this.position().top);
        $("#field_angle").val("0")
        //$(".added").removeClass("redBorder");
        if ($this.find('input[type=text]').length > 0) {
            $(".image-properties").hide();
            var fieldName = $this.find('input[type=text]').attr('fieldname');
            if (!selectable && !$this.hasClass("grouping")) {
                if (fieldName.indexOf("undefined") != -1) {
                    clickItemOutliner(fieldName.replace("undefined", "Textbox"), false)
                    //$("#field_outliner_dropdown").val(fieldName.replace("undefined", "Textbox"))
                    $("#job_field_label").text(fieldName.replace("undefined", "Textbox"))
                } else {
                    clickItemOutliner(fieldName, false)
                    //$("#field_outliner_dropdown").val(fieldName)
                    $("#job_field_label").text(fieldName)
                }
            }

            $("#job_field_type").text("TextBox")
            if (fieldName == undefined || fieldName.includes('undefined')) {
                $('#jobFields').val('select');
            }
            else {
                $('#jobFields').val(fieldName.split("_")[0] + "_" + fieldName.split("_")[1])
                for (var i = 0; i < data.length; i++) {
                    if (data[i].value == fieldName) {
                        $('#jobFields').val(data[i].label)
                        $("#job_field_label").text(data[i].label)
                        break;
                    }
                }
            }

            $("#field_maxlength").val($this.find('input[type=text]').attr('maxlength'));
            $(".line-height-div").hide();
        }
        else if ($this.find('textarea').length > 0) {
            $(".image-properties").hide();
            var fieldName = $this.find('textarea').attr('fieldname');
            if (!selectable && !$this.hasClass("grouping")) {
                if (fieldName != undefined) {
                    if (fieldName.indexOf("undefined") != -1) {
                        clickItemOutliner(fieldName.replace("undefined", "Textarea"), false)
                        //$("#field_outliner_dropdown").val(fieldName.replace("undefined", "Textarea"))
                        $("#job_field_label").text(fieldName.replace("undefined", "Textarea"))
                    } else {
                        clickItemOutliner(fieldName, false)
                        $("#field_outliner_dropdown").val(fieldName)
                        $("#job_field_label").text(fieldName);
                    }
                }
            }

            $("#job_field_type").text("Textarea");
            if (fieldName == undefined || fieldName.includes('undefined')) {
                $('#jobFields').val('select');
            } else {
                $('#jobFields').val(fieldName.split("_")[0] + "_" + fieldName.split("_")[1])
                for (var i = 0; i < data.length; i++) {
                    if (data[i].value == fieldName) {
                        $('#jobFields').val(data[i].label)
                        $("#job_field_label").text(data[i].label)
                        break;
                    }
                }
            }
            $("#field_maxlength").val($this.find('textarea').attr('maxlength'));
            $(".line-height-div").show();
            $("#line-height").val($this.find('textarea').css('line-height'));

        }
        else if ($this.find('input[type=checkbox]').length > 0) {
            $(".image-properties").hide();
            var fieldName = $this.find('input[type=checkbox]').attr('fieldname');
            if (!selectable && !$this.hasClass("grouping")) {
                if (fieldName.indexOf("undefined") != -1) {
                    clickItemOutliner(fieldName.replace("undefined", "Checkbox"), false)
                    $("#job_field_label").text(fieldName.replace("undefined", "Checkbox"))
                } else {
                    clickItemOutliner(fieldName)
                    $("#job_field_label").text(fieldName, false)
                }
            }

            $("#job_field_type").text("Checkbox");

            if (fieldName == undefined || fieldName.includes('undefined')) {
                $('#jobFields').val("select");
            } else {
                $('#jobFields').val(fieldName.split("_")[0] + "_" + fieldName.split("_")[1])
                for (var i = 0; i < data.length; i++) {
                    if (data[i].value == fieldName) {
                        $('#jobFields').val(data[i].label)
                        $("#job_field_label").text(data[i].label)
                        break;
                    }
                }
            }
            $(".line-height-div").hide();
        }

        if ($this.attr('data-type') == 'sa' || $this.attr('data-type') == "btn" || $this.attr('data-type') == "dw") {
            $(".image-properties").hide();
            var fieldName = $this.attr('fieldname');
            if (!selectable && !$this.hasClass("grouping")) {
                if (fieldName.indexOf("undefined") != -1 && !$this.attr('data-type') == "dw") {
                    clickItemOutliner(fieldName.replace("undefined", "Signature"), false)
                    $("#job_field_label").text(fieldName.replace("undefined", "Signature"))
                } else if (fieldName.indexOf("undefined") != -1 && $this.attr('data-type') == "dw") {
                    clickItemOutliner(fieldName.replace("undefined", "Draw"), false)
                    $("#job_field_label").text(fieldName.replace("undefined", "Draw"))
                }
                else {
                    clickItemOutliner(fieldName, false)
                    $("#job_field_label").text(fieldName)
                }
            }
            var drawId = $this.attr('id');
            $("#field_lineWidth").val($("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'])
            if ($this.attr('data-type') == "dw") {
                var isReadOnly = $("#" + drawId).find("canvas.jSignature").add($("#" + drawId).filter("canvas.jSignature")).data("jSignature.this").settings['readOnly']
                if (isReadOnly)
                    $("#onOffSwitchDrawEnable").prop("checked", false);
                else
                    $("#onOffSwitchDrawEnable").prop("checked", true);
                $("#job_field_type").text("Draw")
            }
            else
                $("#job_field_type").text("Signature")

            if (fieldName == undefined || fieldName.includes('undefined')) {
                $('#jobFields').val("select");
            } else {
                $('#jobFields').val(fieldName.split("_")[0] + "_" + fieldName.split("_")[1])
                for (var i = 0; i < data.length; i++) {
                    if (data[i].value == fieldName) {
                        $('#jobFields').val(data[i].label)
                        $("#job_field_label").text(data[i].label)
                        break;
                    }
                }
            }
            if ($this.attr('lineWidth') != undefined) {
                $('#ddlLineWidth').val($this.attr('lineWidth'));
            } else {
                $('#ddlLineWidth').val(2);
            }
            if (!$this.attr('data-type') == "dw") {
                $this.find("canvas.jSignature").add($this.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
                $this.find("canvas.jSignature").add($this.filter("canvas.jSignature")).data("jSignature.this").resetCanvas($this.find("canvas.jSignature").add($this.filter("canvas.jSignature")).data("jSignature.this").settings['data'])
            }
            $('input[type="radio"][name="aspectratio"][value="' + $this.attr('aspectratio') + '"]').prop('checked', true)
            $(".line-height-div").hide();
        }
        if ($this.attr('data-type') == 'im') {
            $(".image-properties").show();
            var fieldName = $this.attr('fieldname');
            $('#jobFieldValue').val(fieldName)
            if (!selectable && !$this.hasClass("grouping")) {
                if (fieldName.indexOf("undefined") != -1) {
                    clickItemOutliner(fieldName.replace("undefined", "Image"), false)
                    $("#job_field_label").text(fieldName.replace("undefined", "Image"))
                    $('#jobFields').val("select");

                } else {
                    clickItemOutliner(fieldName, false)
                    $("#job_field_label").text(fieldName)
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].value == fieldName) {
                            $('#jobFields').val(data[i].label)
                            $("#job_field_label").text(data[i].label)
                            break;
                        }
                    }
                }
            }
            $("#job_field_type").text("Image")
            if ($this.attr("aspectratio") == 100) {
                $('input:radio[name=aspectratio]').val(['1']);
            } else {
                $('input:radio[name=aspectratio]').val(['0']);
            }
        }
        if ($this.hasClass("grouping")) {
            $("[class^='groupingDiv']").css("border", "");
            $this.parent().css("border", "2px solid blue")
            $this.parent().children(".added").addClass("blueBorder")
            selectedDiv = [];
            for (i = 0; i < $this.parent().children(".added").length; i++) {

                selectedDiv.push($this.parent().children(".added")[i]);
            }
            $(".ungroup").show();
            $(".group").hide();
            //clickItemOutliner("select")
            disabledIcon();
        } else {
            $("[class^='groupingDiv']").css("border", "");
            $(".ungroup").hide();
            $(".group").show();
        }
        if (!selectable)
            disabledIcon();
        $ActiveDiv = $this;

    }
}


function serachOutliner() {
    if ($("#searchOutliner").val() != "") {
        found_names = found_names.filter(function (item) {
            return item.toLowerCase().includes($("#searchOutliner").val().toLowerCase())
        })

    } else {
        getOutlinerData();
    }
    populateOutlinerDropdown();

}

function clickItemOutliner(fieldname, isRemoveActive) {

    $('#field_outliner_dropdown').find('.items').each(function () {
        if ($(this).attr('id') == fieldname) {
            $(this).addClass('active');
            var scrollTop = $('#field_outliner_dropdown').scrollTop();
            var topOutliner = $(this).offset().top;
            topOutliner = topOutliner - $("#header").innerHeight() - $(".doc-editor-header").innerHeight() - $(".upper-toolbar").innerHeight() - $(".custom-tabs").innerHeight() - ($("#outliner-pane .form-section").innerHeight() - 15 - $("#field_outliner_dropdown").innerHeight());
            if (topOutliner > parseInt($("#field_outliner_dropdown").css("max-height"))) {
                $('#field_outliner_dropdown').animate({ scrollTop: topOutliner + scrollTop }, 500)
            }
            if (topOutliner < 0) {
                $('#field_outliner_dropdown').animate({ scrollTop: Math.abs(topOutliner + scrollTop) }, 500);
            }
        } else {
            if (!isRemoveActive)
                $(this).removeClass('active')
        }
    })
}
function generateFieldName() {
    var i = 1;
    while (true) {
        j = i;
        contain = false;
        $('.added').each(function () {
            if ($(this).find("input").attr('fieldname') != undefined && $(this).find("input").attr('fieldname').split("_")[1] == i) {
                contain = true;
            }

            else if ($(this).attr('fieldname') != undefined && $(this).attr('fieldname').split("_")[1] == i) {
                contain = true;
            }

            else if ($(this).find("textarea").attr('fieldname') != undefined && $(this).find("textarea").attr('fieldname').split("_")[1] == i) {
                contain = true;
            }
            else
                contain = false;
            if (contain == true) {
                i++;
            }
        });
        if (j == i) {
            break;
        }
    }
    return i;
}
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
    $.each(pdfItems, function (k, data) {

        try {

            var FieldName = data.FieldName.substring(0, data.FieldName.lastIndexOf("_"));


            if (!FieldName.includes('undefined') && /_\d/.test(FieldName)) {
                data.FieldName = data.FieldName.substring(0, data.FieldName.lastIndexOf("_"));
            }

            //data.FieldName=data.FieldName.substring(0, data.FieldName.lastIndexOf("_"));
            if (data.Type.toString() == textHashCode || data.Type.toString() == buttonHashCode) {

                if (data.Value.includes("jsignature")) {

                    $('div[fieldname="' + data.FieldName + '"]').jSignature("setData", "data:" + data.Value);
                    $('div[fieldname="' + data.FieldName + '"]').find("canvas.jSignature").add($('div[fieldname="' + data.FieldName + '"]').filter("canvas.jSignature")).data("jSignature.this").settings['lineWidth'] = data.lineWidth;
                    $('div[fieldname="' + data.FieldName + '"]').find("canvas.jSignature").add($('div[fieldname="' + data.FieldName + '"]').filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('div[fieldname="' + data.FieldName + '"]').find("canvas.jSignature").add($('div[fieldname="' + data.FieldName + '"]').filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    $('div[fieldname="' + data.FieldName + '"]').attr('lineWidth', data.lineWidth)
                    if (!data.Value.split(",")[1] == "" && !data.IsDraw) {
                        $('div[fieldname="' + data.FieldName + '"]').append('<input type="button" onclick=undoClick(this) class="undo" id="undoSignBtn" value="undo last stroke" style="position: absolute; top: auto; margin: 0px !important; left: 0px;">')
                        $('div[fieldname="' + data.FieldName + '"]').append('<input type="button" class="redo" onclick=redoClick(this) id="redoSignBtn" value="redo last stroke" style="position: absolute; top: auto; margin: 0px !important; left: 136px;">')
                    }
                } else if ($('input[fieldname="' + data.FieldName + '"]').length > 0) {

                    $('input[fieldname="' + data.FieldName + '"]').val(data.Value);
                    data.PdfItemProperties && data.PdfItemProperties.MaxLength && $('input[fieldname="' + data.FieldName + '"]').attr('maxLength', data.PdfItemProperties.MaxLength);
                } else if ($('div[fieldname="' + data.FieldName + '"]').find('img').length > 0) {
                    var image = $('div[fieldname="' + data.FieldName + '"]')
                    if (data.PdfItemProperties.FillOption == 1) {
                        image.find(".image").attr("width", $(image).width())
                        image.find(".image").attr("height", $(image).width() / data.PdfItemProperties.AspectRatio)
                        $(image).attr("fill-width", "1")
                    } else if (data.PdfItemProperties.FillOption == 2) {
                        image.find(".image").attr("width", ($(image).height() - 30) * data.PdfItemProperties.AspectRatio)
                        image.find(".image").attr("height", $(image).height() - 30)
                        $(image).attr("fill-height", "1")
                    } else if (data.PdfItemProperties.FillOption == 3) {
                        image.find(".image").attr("width", $(image).width())
                        image.find(".image").attr("height", $(image).height())
                        $(image).attr("fill-content", "1")
                    }
                    if (data.PdfItemProperties.ImageAlign == 2) {
                        $(image).find(".image").css("right", "0")
                    } else if (data.PdfItemProperties.ImageAlign == 3) {
                        $(image).find(".image").addClass("img-center")
                    } else if (data.PdfItemProperties.ImageAlign == 4) {
                        $(image).find(".image").css("top", "0")
                    } else if (data.PdfItemProperties.ImageAlign == 5) {
                        $(image).find(".image").css("bottom", "30px")
                    } else if (data.PdfItemProperties.ImageAlign == 6) {
                        $(image).find(".image").addClass("img-middle")
                    }


                    //if (data.PdfItemProperties.AspectRatio == 100) {
                    //    setAspectRatio(1, image);
                    //} else {
                    //    setAspectRatio(0, image);
                    //}
                }
                if (data.PdfItemProperties.LineHeight) {
                    $('textarea[fieldname="' + data.FieldName + '"]').attr('lineHeight', data.PdfItemProperties.LineHeight);
                    $('textarea[fieldname="' + data.FieldName + '"]').css('line-height', data.PdfItemProperties.LineHeight);
                }
            }
            else if (data.Type.toString() == radioButtonHashCode) {
                var values = data.AvailableValues;
                values = $.grep(values, function (e, index) {
                    return e != null ? e.toLowerCase() != 'off' : '';
                });
                $.each($('input[fieldname="' + data.FieldName + '"]'), function (i) {
                    this.value = ((values.length - (i + 1)) > -1 && values.length - (i + 1) < values.length) ? values[values.length - (i + 1)] : '';
                });
                //$('input[name="' + data.FieldName + '"]').filter('[value=' + data.Value + ']').prop('checked', true);
                $('input[fieldname="' + data.FieldName + '"][value="' + data.Value + '"]').prop('checked', true);
            }
            else if (data.Type.toString() == checkBoxHashCode) {
                var values = data.AvailableValues;
                if ($('input[fieldname="' + data.FieldName + '"]').length == 1) {
                    $.each($('input[fieldname="' + data.FieldName + '"]'), function (i) {

                        this.value = values[0];
                        if (data.Value == this.value)
                            $(this).prop('checked', true);
                    });
                }
            }
        }
        catch (err) { }
    });
    $('input[type="checkbox"]').click(function () {
        if ($(this).prop('checked'))
            $('input[fieldname="' + $(this).attr('fieldname') + '"]').not($(this)).prop('checked', false);
    });
    getOutlinerData();
    populateOutlinerDropdown();
    //
    setSelectable();
    //$('.added').each(function () {
    //    $(this).click(function () {
    //        InputControlClick($(this));
    //    });

    //})
    //$("#field_outliner").autocomplete('option', 'source', found_names)

};
$("#selectable li").draggable({
    helper: "clone",
    start: function (event, ui) {
        $(ui.helper).css('z-index', 9);
    },
    scroll: true
});


function setAspectRatio(value, div) {
    if (value == 1) {
        div.attr("aspectratio", 100);
        div.find("img").css("object-fit", "fill");
        div.find("img").width("100%");
        div.find("img").height("100%")
    }
    else if (value == 0) {

        //div.find("img").css("object-fit", "cover");
        var wd = div.find("img").attr("width");
        var ht = div.find("img").attr("height");
        var width = div.width(); //border minus
        var height = div.height(); //border minus
        var aspectRatio;
        //if (ht < wd) {
        aspectRatio = ht / wd;
        var newht = width * aspectRatio;
        if (newht > height) {
            aspectRatio = wd / ht;
            var newwd = height * aspectRatio;
            var perwd = (newwd * 100) / width;
            div.find("img").width(perwd + "%");
            div.attr("aspectratio", perwd)
            div.attr("fittosize", 0)
        } else {
            var perht = (newht * 100) / height;
            div.find("img").height(perht + "%");
            div.attr("aspectratio", perht)
            div.attr("fittosize", 1)
        }
        //}
        //else {
        //    aspectRatio = wd / ht;
        //    var newwd = height * aspectRatio;
        //    if (newwd > width) {
        //        aspectRatio = ht / wd;
        //        var newht = width * aspectRatio;
        //        var perht = (newht * 100) / height;
        //        div.find("img").height(perht + "%");
        //        div.attr("aspectratio", perht)
        //        div.attr("fittosize", 1)
        //    } else {
        //        var perwd = (newwd * 100) / width;
        //        div.find("img").width(perwd + "%");
        //        div.attr("aspectratio", perwd)
        //        div.attr("fittosize", 0)
        //    }
        //}

    }
}

function getOutlinerData() {
    found_names = [];
    $.each(pdfItems, function (j, data) {
        if (data.FieldName.indexOf("undefined") != -1) {

            if (data.Type.toString() == textHashCode) {
                if (data.IsTextArea) {
                    var fieldname = data.FieldName.replace("undefined", "Textarea")
                    found_names.push(fieldname)
                } else {
                    var fieldname = data.FieldName.replace("undefined", "Textbox")
                    found_names.push(fieldname)
                }
            } else if (data.Type.toString() == buttonHashCode) {
                if ($("[fieldname='" + data.FieldName + "']").data('type') == 'im') {
                    var fieldname = data.FieldName.replace("undefined", "Image")
                    found_names.push(fieldname)
                } else if ($("[fieldname='" + data.FieldName + "']").data('type') == "dw") {
                    var fieldname = data.FieldName.replace("undefined", "Draw")
                    found_names.push(fieldname)
                }
                else {
                    var fieldname = data.FieldName.replace("undefined", "Signature")
                    found_names.push(fieldname)
                }
            } else if (data.Type.toString() == checkBoxHashCode) {
                var fieldname = data.FieldName.replace("undefined", "Checkbox")
                found_names.push(fieldname)
            }
        } else {
            found_names.push(data.FieldName)
        }

    })
}
function generateSignatureId() {
    var i = 1
    $('.jSignature').each(function () {

        if ($(this).parent().attr('id').includes(i)) {
            i++;
        }
    });
    return i;
}
function applyDrop(container) {

    $(container).droppable({
        over: function (event, ui) {
            if (true) {
                $(this).addClass("droppable-above").removeClass("droppable-below");
            }
            else {
                $(this).removeClass("droppable-above").addClass("droppable-below");
            }
        },
        classes: {
            "ui-droppable-active": "",
            "ui-droppable-hover": ""
        },
        drop: function (event, ui) {
            if ($(ui.helper).attr('id') != "draggable") {

                var type = $(ui.helper).data('type');
                if ($(ui.helper).hasClass("added")) {
                    var left = $(ui.helper).offset().left;
                    var top = $(ui.helper).offset().top;
                    var IsHeightGreater = false;
                    $('#the-svg').find('.svg').each(function (index) {

                        if (IsHeightGreater) {
                            top = $(this).offset().top;
                            $(ui.helper).attr('pagenumber', ++index);
                            return false;
                        }
                        var bottom = $(this).height() + $(this).offset().top;
                        if (bottom >= top) {
                            if (bottom < top + $(ui.helper).height()) {
                                if (bottom >= top + $(ui.helper).height() / 2) {
                                    top = parseInt(bottom - $(ui.helper).height());
                                    $(ui.helper).attr('pagenumber', ++index);
                                    return false;
                                }
                                else {
                                    IsHeightGreater = true;
                                }
                            }
                            else {
                                $(ui.helper).attr('pagenumber', ++index);
                                return false;
                            }
                        }
                    })

                    $(ui.helper).offset({ left: left, top: top });
                    return;
                }
                foo.disable();
                var j = generateFieldName()

                if (type == "t") {
                    var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted rgb(0, 0, 0);opacity:0.8;width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;background:' + eleType[type].color + ';font-family:' + 'arialmt' + '"><input fieldname="undefined_' + j + '" class="control" type="text" style="width:100%;height:100%;background:transparent;border:0px" maxlength="50"></input></div>');//onclick="handler(this)"
                    found_names.push("Textbox_" + j);
                }
                else if (type == "ta") {
                    var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted rgb(0, 0, 0);opacity:0.8;width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;background:' + eleType[type].color + ';font-family:' + 'arialmt' + '"><textarea  fieldname="undefined_' + j + '" class="control" style="width:100%;height:100%;background:transparent;border:0px" maxlength="50"></textarea></div>');
                    found_names.push("Textarea_" + j);
                }
                else if (type == "r") {
                    var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted rgb(0, 0, 0);width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;"><input id="radbtn"  fieldname="undefined_' + j + '" class="control" type="radio" style="width:100%;height:100%;background:transparent"></input></div>');
                    found_names.push("Radio_" + j);
                }
                else if (type == "c") {
                    //<input class="control" type="checkbox" onchange="OnCheckboxChange(this)" style="width:100%; height:100%; background:transparent"></input>
                    var $div = $('<div data-type=' + type + ' class="added" style="border: 1px dotted rgb(0, 0, 0);width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;"><label class="checkbox"><input type = "checkbox"  fieldname="undefined_' + j + '" class="icheck" " onchange="OnCheckboxChange(this)"><span></span></label ></div>');
                    found_names.push("Checkbox_" + j);
                }
                else if (type == "sa") {
                    var i = generateSignatureId();
                    var $div = $('<div tabindex=1 id="jSignature' + i + '" fieldname="undefined_' + j + '" data-type=' + type + ' class="added signature" onclick=signatureChange(this) style="border: 1px dotted rgb(0, 0, 0);opacity:0.8;width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;background:' + eleType[type].color + ';"></div>');
                    found_names.push("Signature_" + j);
                }
                else if (type == "im") {

                    var $div = $('<div data-type=' + type + ' fieldname= "undefined_' + j + '" class="added"  style="border: 1px dotted rgb(0, 0, 0);overflow:hidden;opacity:1;width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;background:' + eleType[type].color + ';"><img src="#" class="image" style="display:none;"/><div class="fileUpload primary document_imageUpload"><span>Upload</span><input type="file" class="upload" value="Upload" accept="image/x-png,image/gif,image/jpeg" onchange="readURL(this);"/></div></div>');
                    found_names.push("Image_" + j);

                } else if (type == "dw") {
                    var i = generateSignatureId();
                    var $div = $('<div tabindex=1 id="jSignature' + i + '" fieldname="undefined_' + j + '" enable=false  data-type=' + type + ' class="added drawing" onclick=signatureChange(this) style="border: 1px dotted rgb(0, 0, 0);opacity:0.8;width:' + eleType[type].width + ';height:' + eleType[type].height + ';position:absolute;background:' + eleType[type].color + ';"></div>');
                    found_names.push("Draw_" + j);

                }
                populateOutlinerDropdown();


                if ($div != undefined) {
                    $('#holder').append($div);
                    if (trackData.length == 5) {
                        trackData.splice(1, 1)
                    }
                    trackData.push($div);
                    disabledIcon();

                    var left = $(ui.helper).offset().left;
                    var top = $(ui.helper).offset().top;
                    var IsHeightGreater = false;
                    if ((left + $div.width()) > ($("#the-svg").width() + $(".custom-list").width())) {
                        left = $("#the-svg").width() + $(".custom-list").width() - $div.width();
                    }
                    $('#the-svg').find('.svg').each(function (index) {

                        if (IsHeightGreater) {
                            top = $(this).offset().top;
                            $div.attr('pagenumber', ++index);
                            return false;
                        }
                        var bottom = $(this).height() + $(this).offset().top;
                        if (bottom >= top) {
                            if (bottom < top + $div.height()) {
                                if (bottom >= top + $div.height() / 2) {
                                    top = parseInt(bottom - $div.height());
                                    $div.attr('pagenumber', ++index);
                                    return false;
                                }
                                else {
                                    IsHeightGreater = true;
                                }
                            }
                            else {
                                $div.attr('pagenumber', ++index);
                                return false;
                            }
                        }
                    });

                    $div.offset({ left: left, top: top });

                    $div.click(function (event) {

                        InputControlClick($(this), event);
                    });

                    if ($div.data('type') == "sa") {

                        $('#jSignature' + i).jSignature({ 'lineWidth': 2 });
                        var margin = ($div.height() * 15) / 100;
                        $div.find('canvas').css('margin-top', margin)
                        $div.find('canvas').css('margin-bottom', margin)
                        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
                        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas($div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    } if ($div.data('type') == "dw") {
                        $('#jSignature' + i).jSignature({ 'lineWidth': 2 });
                        $('#jSignature' + i).find("canvas.jSignature").width(0)
                        $('#jSignature' + i).find("canvas.jSignature").height(0)
                        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['readOnly'] = true;
                        $div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").resetCanvas($div.find("canvas.jSignature").add($div.filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                    }
                    $('.added .control').click(function () {
                        $(this).focus();
                    });
                    var click = {
                        x: 0,
                        y: 0
                    };
                    $div.draggable({
                        start: function (event) {
                            click.x = event.clientX;
                            click.y = event.clientY;
                        },

                        drag: function (event, ui) {

                            // This is the parameter for scale()
                            var zoom = parseFloat($("#zoom-dropdown").val() / 100)

                            var original = ui.originalPosition;

                            // jQuery will simply use the same object we alter here
                            ui.position = {
                                left: (event.clientX - click.x + original.left) / zoom,
                                top: (event.clientY - click.y + original.top) / zoom
                            };

                        },
                        cancel: 'canvas',
                        containment: '.ui-droppable',
                        scroll: true
                    }).resizable({
                        containment: '.ui-droppable', handles: 'nw,ne,sw,se',
                        stop: function (event, ui) {
                            setTimeout(function () {

                                setAspectRatioResize()
                                if ($(ui.element).data('type') == "im") {
                                    if ($(ui.element).attr("fill-height") == 1) {
                                        $("#fill-height").click();
                                    }
                                    else if ($(ui.element).attr("fill-width") == 1) {

                                        $("#fill-width").click();

                                    } else if ($(ui.element).attr("fill-content") == 1) {

                                        $("#fill-content").click();
                                    }
                                }
                            }, 500)
                        },
                        resize: function (event, ui) {

                            var changeWidth = ui.size.width - ui.originalSize.width; // find change in width
                            var newWidth = ui.originalSize.width + changeWidth / parseFloat($("#zoom-dropdown").val() / 100); // adjust new width by our zoomScale

                            var changeHeight = ui.size.height - ui.originalSize.height; // find change in height
                            var newHeight = ui.originalSize.height + changeHeight / parseFloat($("#zoom-dropdown").val() / 100); // adjust new height by our zoomScale

                            ui.size.width = newWidth;
                            ui.size.height = newHeight;
                            if ($(ui.element).data('type') == "dw") {
                                $(ui.element).find(".jSignature").attr("width", newWidth + "px")
                                $(ui.element).find(".jSignature").attr("height", newHeight + "px")
                                $('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").resetCanvas($('#' + $(ui.element).attr('id')).find("canvas.jSignature").add($('#' + $(ui.element).attr('id')).filter("canvas.jSignature")).data("jSignature.this").settings['data'])
                            }


                        }
                    });
                }
                foo.enable();
            }
        }
    });
}
prop: ['height', 'width', 'maxlength', 'font-size', 'font-family', 'job-field', 'bold', 'italic', 'underline']

var color = "#EDC201";
var eleType = {
    t: {
        ele: 'Textbox',
        height: '30px',
        width: '200px',
        color: '#cbf7cb',
        font: 'Arial',
        prop: ['height', 'width', 'maxlength', 'font-size', 'font-family', 'job-field', 'colorpicker', 'bold', 'italic', 'jobFieldName']
    },
    ta: {
        ele: 'Textarea',
        height: '100px',
        width: '300px',
        color: '#cbf7cb',
        font: 'Arial',
        prop: ['height', 'width', 'maxlength', 'font-size', 'font-family', 'job-field', 'colorpicker', 'bold', 'italic', 'jobFieldName']
    },
    c: {
        ele: 'Checkbox',
        height: '30px',
        width: '30px',
        prop: ['height', 'width', 'job-field', 'export-value', 'jobFieldName']
    },
    r: {
        ele: 'Radio',
        height: '30px',
        width: '30px',
        prop: ['height', 'width', 'group', 'jobFieldName']
    },
    sa: {
        ele: 'Signature',
        height: '100px',
        width: '260px',
        color: '#cbf7cb',
        prop: ['height', 'width', 'job-field', 'aspectratio', 'signlineWidth']
    },
    im: {
        ele: 'Images',
        height: '130px',
        width: '260px',
        color: '#cbf7cb',
        prop: ['height', 'width', 'job-field', 'aspectratio', 'signlineWidth']
    },
    btn: {
        ele: 'Signature',
        height: '100px',
        width: '260px',
        color: '#cbf7cb',
        prop: ['height', 'width', 'job-field', 'aspectratio', 'signlineWidth']
    },
    dw: {
        ele: 'Draw',
        height: '100px',
        width: '260px',
        color: '#cbf7cb',
        prop: ['height', 'width', 'job-field', 'aspectratio', 'signlineWidth']
    },

}


var jobFields = $('#jobFields')[0].outerHTML.replace(/\n/g, "<br />");
function CheckProperty(e) {
    if ($(e).hasClass('active'))
        $(e).removeClass('active');
    else
        $(e).addClass('active');
}

var mousePos = {
    x: 0,
    y: 0
};

$("body").mousemove(function (e) {
    mousePos.x = e.pageX;
    mousePos.y = e.pageY;
});

//$('#holder').keyup(function (e) {
//    if (e.keyCode == 46) {
//        if (selectedDiv) {
//            selectedDiv.remove();

//        }
//    } else if (e.keyCode == 38 && selectedDiv.length == 1) {
//        $("#pos-top-minus").click();
//    } else if (e.keyCode == 40 && selectedDiv.length == 1) {
//        $("#pos-top-plus").click();
//    } else if (e.keyCode == 37 && selectedDiv.length == 1) {
//        $("#pos-left-minus").click();
//    } else if (e.keyCode == 39 && selectedDiv.length == 1) {
//        $("#pos-left-plus").click();
//    }

//});

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
}

var hexDigits = new Array
    ("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f");

//Function to convert rgb color to hex format
function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}

function hex(x) {
    return isNaN(x) ? "00" : hexDigits[(x - x % 16) / 16] + hexDigits[x % 16];
}


function setOutlinerValue(div, value) {
    if (div.find('input[type=text]').length > 0) {
        if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            //var j = generateFieldName();
            var j = value.split('_')[1];
            div.find('input').attr('fieldname', "undefined_" + j);
            found_names.push("Textbox_" + j);
            newFieldName = "Textbox_" + j;
        } else {
            div.find('input').attr('fieldname', $('#jobFieldValue').val());
            div.find('input').val($('#ddlExportValue option:selected').val());
            found_names.push(value);
            newFieldName = value;
        }
    } else if (div.find('textarea').length > 0) {
        if ($('#jobFieldsValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            //var j = generateFieldName();
            var j = value.split('_')[1];
            div.find('textarea').attr('fieldname', "undefined_" + j);
            found_names.push("Textarea_" + j);
            newFieldName = "Textarea_" + j;
        }
        else {
            div.find('textarea').attr('fieldname', $('#jobFieldValue').val());
            found_names.push(value);
            newFieldName = value;
        }
    } else if (div.attr("data-type") == 'sa' || div.attr("data-type") == 'btn') {
        if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            var j = value.split('_')[1];
            div.attr('fieldname', "undefined_" + j);
            found_names.push("Signature_" + j);
            newFieldName = "Signature_" + j;
        }
        else {
            div.attr('fieldname', $('#jobFieldValue').val());
            found_names.push(value);
            newFieldName = value;
        }
    }
    else if (div.attr("data-type") == 'im') {
        if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            var j = value.split('_')[1];
            div.attr('fieldname', "undefined_" + j);
            found_names.push("Image_" + j);
            newFieldName = "Image_" + j;
        }
        else {
            div.attr('fieldname', $('#jobFieldValue').val());
            found_names.push(value);
            newFieldName = value;
        }
    } else if (div.attr("data-type") == 'dw') {
        if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            var j = value.split('_')[1];
            div.attr('fieldname', "undefined_" + j);
            found_names.push("Draw_" + j);
            newFieldName = "Draw_" + j;
        }
        else {
            div.attr('fieldname', $('#jobFieldValue').val());
            found_names.push(value);
            newFieldName = value;
        }
    } else if (div.attr("data-type") == 'c') {
        if ($('#jobFieldValue').val() == "select" || $('#jobFieldValue').val() == "" || $('#jobFields').val() == "select") {
            var j = value.split('_')[1];
            div.attr('fieldname', "undefined_" + j);
            $(div).find('[type="checkbox"]').attr('fieldname', "undefined_" + j)
            //var j = value.split('_')[1];
            found_names.push("Checkbox_" + j);
            newFieldName = "Checkbox_" + j;
        }
        else {
            div.attr('fieldname', $('#jobFieldValue').val());
            found_names.push(value);
            newFieldName = value;
        }
    }
    return newFieldName;
}

function setAspectRatioResize() {
    $('input[type=radio][name=aspectratio]').change();
}
function removeOutlinerValue(div) {
    var index = -1;
    if (div.data('type') == 't') {
        if ($("[fieldname='" + div.find('input').attr("fieldname") + "']").length <= 0) {
            if (div.find('input').attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.find('input').attr("fieldname").replace("undefined", "Textbox"), found_names);

            } else {
                index = $.inArray(div.find('input').attr("fieldname"), found_names);
            }
        }

    } else if (div.data('type') == 'ta') {
        if ($("[fieldname='" + div.find('textarea').attr("fieldname") + "']").length <= 0) {
            if (div.find('textarea').attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.find('textarea').attr("fieldname").replace("undefined", "Textarea"), found_names);
            } else {
                index = $.inArray(div.find('textarea').attr("fieldname"), found_names);
            }
        }

    } else if (div.data('type') == 'sa' || div.data('type') == 'btn') {
        if ($("[fieldname='" + div.attr("fieldname") + "']").length <= 0) {
            if (div.attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.attr("fieldname").replace("undefined", "Signature"), found_names);
            } else {
                index = $.inArray(div.attr("fieldname"), found_names);
            }
        }

    } else if (div.data('type') == 'c') {

        if ($("[fieldname='" + div.find('input').attr("fieldname") + "']").length <= 0) {
            if (div.find('input').attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.find('input').attr("fieldname").replace("undefined", "Checkbox"), found_names);
            } else {
                index = $.inArray(div.find('input').attr("fieldname"), found_names);
            }
        }

    } else if (div.data('type') == 'im') {

        if ($("[fieldname='" + div.attr("fieldname") + "']").length <= 0) {
            if (div.attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.attr("fieldname").replace("undefined", "Image"), found_names);
            } else {
                index = $.inArray(div.attr("fieldname"), found_names);
            }

        }
    } else if (div.data('type') == 'dw') {

        if ($("[fieldname='" + div.attr("fieldname") + "']").length <= 0) {
            if (div.attr("fieldname").indexOf("undefined") != -1) {
                index = $.inArray(div.attr("fieldname").replace("undefined", "Draw"), found_names);
            } else {
                index = $.inArray(div.attr("fieldname"), found_names);
            }

        }
    }

    if (index != -1) {

        found_names.splice(index, 1);
    }
}

function setHeightWidthGroupingDiv() {
    var minLeft = $("#holderParentDiv").width();
    var maxLeft = 0;
    var minTop = $("#the-svg").height();
    var maxTop = 0;
    if ($(selectedDiv[0]).hasClass("grouping")) {
        for (var i = 0; i < selectedDiv.length; i++) {
            var divleft = parseFloat($(selectedDiv[i]).css("left"));
            if (divleft < minLeft) {
                minLeft = divleft
            }
            var divright = divleft + $(selectedDiv[i]).outerWidth();
            if (divright > maxLeft) {
                maxLeft = divright;
            }
            var divtop = parseFloat($(selectedDiv[i]).css("top"));
            if (divtop < minTop) {
                minTop = divtop;
            }
            var divbottom = divtop + $(selectedDiv[i]).outerHeight();
            if (divbottom > maxTop) {
                maxTop = divbottom;
            }
        }
    }
    $(selectedDiv[0]).parent().width(maxLeft - minLeft)
    $(selectedDiv[0]).parent().height(maxTop - minTop)
}

function controlClick() {
    $(document).on("click", ".added", function (event) {
        if (event.shiftKey) {
            for (var k = 1; k < selectedDiv.length; k++) {
                if (selectedDiv[k][0] == selectedDiv[k - 1][0]) {
                    selectedDiv.splice(k, 1);
                    k--;
                }
            }
            $("#groupElement").click();
        }
        heightlightSelectionOutliner();
    })
}

function PreviewDocument(isWithoutbgColor) {

    $('#divViewer').load(previewDocumentUrl + "?docTemplateId=" + $('#DocumentTemplateId').val() + "&isWithoutBg=" + isWithoutbgColor);
    $("#popupSelectOptionForDocumentPreviewDownload").modal('toggle');
    $('#popupDocPreview').modal({ backdrop: 'static', keyboard: false });
    withoutBg = isWithoutbgColor;
}