/**
 * divider.js
 *
 * Copyright (C) 2012 efilefolders.com
 * MIT Licensed
 *
 * ****************************************
 *
 **/

var $status = { unloaded: 0, loading: 1, loaded: 2 };

var $statusMoveFile = { unloadedMoveFile: 0, loadingMoveFile: 1, loadedMoveFile: 2 };

var $bookshelfPromises = new Hashtable;

var $divider_caches = new Hashtable;
var $dividers = new Hashtable;
var $dividers_front = null, $dividers_back = null;
var $current_divider, $current_divider_nth = null;
var $total_page_numb = null;

var $book_contents = {}; // store all pageviews
var $page_promises = new Hashtable; // store all pages promises

var $divider_details = {};

var $items_auto_source = [];

var viewStatus = { self: false, book: true };
var $current_status = $status.loaded;

var $current_status_moveFile = $status.loadedMoveFile;

var current_view_status = viewStatus.self;

//var shared = false;

function getFitWidth(width, count) {
    var unit = Math.floor((width - 30) / count);
    return Math.min(unit, 100);
}

function initdividers(p) {
    var container = $("#dividers");
    var width = $('#shelf .flipbook').width() - 50;
    var unit = getFitWidth(width, p.tabsList.length);
    for (var i = 0; i < p.tabsList.length; i++) {
        var divider = p.tabsList[i];
        var divider_1 = new Hashtable;
        divider_1["name"] = divider.Name;
        divider_1["color"] = divider.Color;
        divider_1["id"] = divider.Id;
        divider_1["nth"] = $dividers.Count + 1;
        divider_1["cached"] = false;

        $dividers.Add(divider_1["nth"], divider_1);

        container.append(
            adddivider(divider_1["id"], divider_1["nth"], divider_1["name"], divider_1["color"], unit)
            );
    }
};

function adddivider(id, nth, name, bcolor, width) {
    var $divider = $("<li id=" + id + " nth=" + nth + " class='divider' style='width:" + width + "px;'" + " title='" + name + "'>" + name + "</li>");
    $divider.css("background", bcolor);

    $divider.click(function (e) {
        selectdivider(e.currentTarget);
    }).bind("inittoc", inittoc).bind("initedtoc", initedtoc);

    return $divider;
}

function switchView(status) {
    if (status === viewStatus.self) {
        if (!$("#shelf").is(":visible")) {
            $("#shelf").fadeIn('slow');
        }

        if ($("#book-wrapper .bar").is(":visible")) {
            $("#book-wrapper .bar").fadeOut('slow');
        }

        if ($("#book-wrapper .tool").is(":visible")) {

            $("#book-wrapper .tool").fadeOut('slow');
        }


        if ($("#book-zoom").is(":visible")) {
            $("#book-zoom").fadeOut('slow');
        }
    }
    else {
        if ($("#shelf").is(":visible")) {
            $("#shelf").fadeOut('slow');
        }

        if (!$("#book-wrapper .bar").is(":visible")) {
            $("#book-wrapper .bar").fadeIn('slow');
        }

        if (!$("#book-wrapper .tool").is(":visible")) {

            $("#book-wrapper .tool").fadeIn('slow');
        }

        if (!$("#book-zoom").is(":visible")) {
            $("#book-zoom").fadeIn('slow');
        }
    }

    current_view_status = status;
}

function selectdivider(target) {
    //if ($('.sj-book').turn('zoom') > 1) return;
    switchView(viewStatus.book);

    if ($current_status === $status.loading) return;

    $current_status = $status.loading;
    bookshelf.loading.apply();

    if (parseInt($(target).attr("nth")) != $current_divider_nth) {
        $current_divider_nth = parseInt($(target).attr("nth"));
    }
    else {
        if (current_view_status == viewStatus.self) {
            switchView(viewStatus.book);
        }
        bookshelf.loaded.apply();
        return;
    }

    $current_divider = $(target);

    var dividers_front = $("#dividers-front");
    var dividers_back = $("#dividers-back");

    dividers_front.empty();
    dividers_back.empty();

    $dividers_front = new Hashtable;
    $dividers_back = new Hashtable;
    var items = $dividers.Items;
    var $divider;
    var unit = getFitWidth($('.sj-book').height(), $dividers.Count);

    for (nth = $current_divider_nth + 1; nth <= $dividers.Count; nth++) {
        $divider = $("<li id=divider" + items[nth]["id"] + " nth=" + nth + " class='divider-back' style='background-color:" + items[nth]["color"] + ";width:" + unit + "px;'" + " title='" + items[nth]["name"] + "'>" + items[nth]["name"] + "</li>");
        $divider.css({ 'margin-top': unit - 39 });

        //$divider.hover(function (event) {
        //    if ($current_status === $status.loading) return;
        //    $(this).animate({ 'margin-top': unit - 54, 'padding-bottom': 15 });
        //}, function (event) {
        //    $(this).animate({ 'margin-top': unit - 39, 'padding-bottom': 0 });
        //});

        dividers_back.append($divider);
        $dividers_back.Add(nth, items[nth]);
        dividers_back.css("padding-top", unit * $current_divider_nth);

        $divider.click(function (e) {
            if ($current_status === $status.loading) return;
            selectdivider(e.currentTarget);
        }).bind("inittoc", inittoc).bind("initedtoc", initedtoc);
    }

    for (nth = 1; nth <= $current_divider_nth; nth++) {
        $divider = $("<li id=divider" + items[nth]["id"] + " nth=" + nth + " class='divider-front' style='background-color:" + items[nth]["color"] + ";width:" + unit + "px;'" + " title='" + items[nth]["name"] + "'>" + items[nth]["name"] + "</li>");
        $divider.css({ 'margin-top': unit - 39 });

        //$divider.hover(function (event) {
        //    if ($current_status === $status.loading) return;
        //    $(this).animate({ 'margin-top': unit - 54, 'padding-bottom': 15 });
        //}, function (event) {
        //    $(this).animate({ 'margin-top': unit - 39, 'padding-bottom': 0 });
        //});
        dividers_front.append($divider);
        $dividers_front.Add(nth, items[nth]);

        $divider.click(function (e) {
            if ($current_status === $status.loading) return;
            selectdivider(e.currentTarget);
        }).bind("inittoc", inittoc).bind("initedtoc", initedtoc);
    }

    //initialize toc of divider
    if ($divider != null) {
        $current_divider = $divider;
        trigger("inittoc", $divider, items[$current_divider_nth]);
    }

    $("#item-filter-input").val("");
}


function BindDividerOnIndexSearch(isSelect, isClear, documentId, isSort) {
   
    var totalCount = 0;
    var dividerId = $('#toc').find('li').attr('divider');
    var book = $('.sj-book');

    if (!isSort) {
        //var items = selectedItem;
        var items = $dividers.Items[$current_divider_nth].files;
        if (isSelect) {
            $.each(items, function (i, e) {
                if (e.Id && documentId && parseInt(documentId) == parseInt(e.Id)) {
                    totalCount = e.Count;
                    return true;
                }
            });
            items = $.grep(items, function (e) {
                if (e.Id && documentId && parseInt(e.Id) == documentId) {
                    return true;
                }
                else
                    return false;
            });
        }
        else {
            // totalCount = totalPage;
        }

        var toc = $("#toc");
        toc.empty();
        $total_page_numb = 1;

        $items_auto_source = [];
        $divider_details = [];
        $page_promises = new Hashtable;
        toc.data('items', items);

        if (items.length > 0) {

            for (var i = 0; i < items.length; i++) {
                $items_auto_source.push({ value: items[i].Id, label: (items[i].DisplayName ? items[i].DisplayName : items[i].Name), path: items[i].FullName, start: $total_page_numb, count: items[i].Count });
                for (var j = 1; j <= items[i].Count; j++) {
                    var pagekey = items[i].Id + '-' + j;
                    $page_promises.Add(pagekey, new PDFJS.Promise());
                }
                if (items.length != 1) {
                    totalCount = totalCount + items[i].Count;
                }
                else {
                    totalCount = items[i].Count;
                }
                addItem(toc, items[i]);
               
            }            
            // Hover effect to show/hide edit icons
            assignHoverOnIndex();
        }
        else {
            var totalPages = book.turn('pages');
            for (var k = totalPages; k >= 1; k--) {
                book.turn('removePage', k);
            }
        }
        initfilters($items_auto_source);

        if (isClear) {
            var notes = pageNote;            
            if (notes.length > 0) {
                for (var i = 0; i < notes.length; i++) {
                    $items_auto_source.push({                        
                        value: dividerId, isNote: true, label: 'Page Note', start: $total_page_numb,
                        count: notes.length, pages: notes                        
                    });                   
                    totalCount = totalCount + 1;
                    addNote(toc, null, notes[i], dividerId);
                }
            }            
            //initfilters($items_auto_source);
        }
        
    }
    else
        totalCount = totalPage;

    var pagesPromise = PDFJS.Promise.all($bookshelfPromises.GetValues());
    pagesPromise.then(function (bookshelfPromises) {
        bookshelf.loaded.apply();
    });
    
    $total_page_numb = totalCount;
    if ($total_page_numb % 2 === 1) {
        $total_page_numb++;
    }

    // total pages
    $('#slider-book').slider('option', 'value', 1);
    $('#slider-bar-number').val(1 + '/' + $total_page_numb);
    $('#slider-book').slider('option', 'max', $total_page_numb);

    //flip the old pages to the first page
    var pages = book.turn('pages');
    var currentPage = book.turn('page');
    if (pages > 1 && currentPage > 1) {
        book.turn('page', 1);
    }

    // then turn the new pages
    book.turn('pages', $total_page_numb, { id: dividerId, th: $current_divider_nth });

    if (totalCount > 0)
        book.turn('page', 1);

    if ($page_promises.Count === 0) {
        bookshelf.loaded.apply();
    }
}

function initfilters(item_source) {
    $("#item-filter-input").autocomplete({
        minLength: 0,
        source: item_source,
        focus: function (event, ui) {
            $("#item-filter-input").val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            ClearDividerSearch();
            var documentId = ui.item.value;

            $("#item-filter-input").val(ui.item.label);
            $("#item-id").val(documentId);
            BindDividerOnIndexSearch(true, false, documentId);           
            $('#clearID').show();
               
            return false;
        },
        close: function (event, ui) {
            //$("#item-filter-input").val("");
            //  var items = toc.data('items');   
        }

    })
		.data("autocomplete")._renderItem = function (ul, item) {
		    if (item.label != "Page Note") {
		        return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append("<a>" + item.label + "</a>")
                    .appendTo(ul);
		    }
		    else {		       
		        //$(".ui-autocomplete ui-menu ui-widget ui-widget-content ui-corner-all").hide();
		    }
		};
}

function initnotes() {
}

//initialize the toc items
function inittoc(e, target) {
    if (target["cached"] === false) {
        //download tab content
        bookshelf.initdivider(target["id"]);
    }
    else {
        initedtoc(e, target);
    }
}

var selectedItem = [];
var pageNote = [];
var totalPage = 0;

//initialize the toc items
function initedtoc(e, target) {
   
    if ($current_divider_nth === null || target === null) return;

    var items = target["files"]; // name,fullname, path,id,count

    selectedItem = items;

    //items = [];
    var toc = $("#toc");
    toc.empty();
    $total_page_numb = 1; // 初始页数为1，表示目录页， // 1th: 1, item.start = 1, item.end = 1+ page count(1)-1
    // 2th: 2 ;item.start = 2, total(2)+pagecount(2)-1 // 3th: 1 item.start = 4, total(2)+pagecount(2)-1
    // 所以如果总数total_page_numb为even偶数，则应该加1

    var book = $('.sj-book');
    $items_auto_source = [];
    $divider_details = [];
    $page_promises = new Hashtable;
    toc.data('items', items);
    // init toc with all files of a tab, 
    // if be pdf format, that are the pdf file names    
    if (items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            //alert(items[i].FullName);
            //$items_auto_source.push({ value: items[i].Id, label: (items[i].DisplayName ? items[i].DisplayName : items[i].Name), path: items[i].FullName, start: $total_page_numb, count: items[i].Count });
            $items_auto_source.push({ value: items[i].Id, label: (items[i].DisplayName ? items[i].DisplayName : items[i].Name), path: items[i].FullName, start: $total_page_numb, count: items[i].Count, Order: items[i].DocumentOrder });
            //add pages promises for this item
            for (var j = 1; j <= items[i].Count; j++) {
                var pagekey = items[i].Id + '-' + j;
                $page_promises.Add(pagekey, new PDFJS.Promise());
            }

            addItem(toc, items[i]);
        }

        // Put condition if the URL is share to another User and hide  edit icon
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        var flag = true;
        var urlLength = url.length;
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == "ISEDIT" && urlparam[1] == "false") {
                flag = false;
            }
        }

        if (flag) {
            // Hover effect to show/hide edit icons
            assignHoverOnIndex();
        }       
    }
    else {
        var totalPages = book.turn('pages');
        for (var k = totalPages; k >= 1; k--) {
            book.turn('removePage', k);
        }
    }
    initfilters($items_auto_source);

    //add documentloader linker
    //if (items.length === 0) {
    //    var loader = { Id: target.fid,Idd: target.id, Name: 'Upload Documents', path: '', start: 0, count: 0 };
    //    addLoadItem(toc, loader);
    //}

    var pagesPromise = PDFJS.Promise.all($bookshelfPromises.GetValues());
    pagesPromise.then(function (bookshelfPromises) {
        bookshelf.loaded.apply();
    });

    // add note pages, if we could append these notes into the pdf,
    // that would be very good, I think we could use javascript pdf maker library to do it
    // these would be parts of pages in pdf files
    var notes = target["notes"];
    pageNote = notes;

    if (notes.length > 0) {
        for (var i = 0; i < notes.length; i++) {
            $items_auto_source.push({
                value: target["id"], isNote: true, label: 'Page Notes', start: $total_page_numb,
                count: notes.length, pages: notes
            });
            addNote(toc, target, notes[i]);
        }
    }

    //if (notes.length > 0) {
    //    $items_auto_source.push({
    //        value: target["id"], isNote: true, label: 'Page Notes', start: $total_page_numb,
    //        count: notes.length, pages: notes
    //    });
    //    addNote(toc, target, notes);
    //}

    //initfilters($items_auto_source);

    $total_page_numb = target.total;
    totalPage = target.total;
    if ($total_page_numb % 2 === 1) {
        $total_page_numb++;
    }

    // total pages
    $('#slider-book').slider('option', 'value', 1);
    $('#slider-bar-number').val(1 + '/' + $total_page_numb);
    $('#slider-book').slider('option', 'max', $total_page_numb);

    //flip the old pages to the first page
    var pages = book.turn('pages');
    var currentPage = book.turn('page');
    if (pages > 1 && currentPage > 1) {
        book.turn('page', 1);
    }

    // then turn the new pages
    book.turn('pages', $total_page_numb, { id: target["id"], th: $current_divider_nth });

    if (target.total > 0)
        book.turn('page', 1);

    if ($page_promises.Count === 0) {
        bookshelf.loaded.apply();
    }
    $('#clearID').hide();
}

function getCopyrightPage(copyright, dividers) {
    var $page = $('<div class="fixed hard static back-side" disable="true"></div>').append(copyright).append(dividers);
    return $page;
}

function getBlankPage(hard, disable) {
    return $("<div class=" + hard + " disable=" + disable + "></div>");
}

function getBlankPage() {
    return $("<div/>");
}

// add one of item(file name) in a divider
function addItem(toc, item) {
    var $pagenum = $("<a href='#' class='li-a'>" + $total_page_numb + "</a>");

    // Index and Filter both are display same name 
    var $span = $("<span class='li-span itemName' href='#' >" + (item.DisplayName ? item.DisplayName : item.Name) + "</span>");
    //var $span = $("<span class='li-span itemName' href='#' >" + item.DisplayName + "</span>");

    var $txt = $("<input class='txtContent' id='DocName' onkeydown='adjustWidth(this)'  type='text' maxlength='50'  value='" + (item.DisplayName ? item.DisplayName : item.Name) + "' style='display:none;height:20px;min-width:50px;max-width:300px;'/>");
    //var $txt = $("<input class='txtContent' id='DocName'  type='text' maxlength='50'  value='" + item.DisplayName + "' style='display:none;width:280px;height:20px   '/>");

    var $item = $("<li id=" + (item.value ? item.value : item.Id) + " divider=" + item.Idd + " fileName=" + item.FullName + " pages=" + item.Count + "  order=" + item.DocumentOrder + " class='toc-li doc' onmouseover =''></li>");
     
    var $aEdit;

    if (bookshelfEdit.allowEdit)
        $aEdit = $(" &nbsp;&nbsp; <img src='images/icon-edit.png' class='ic-edit-search editContent' style='height:17px;width:17px;display:none;cursor:pointer;vertical-align:middle;margin:-3px 0px 0px 5px;' align='middle' title='Edit'>");

    $aSave = $("<a class='saveContent' href='javascript:void(0)' style='display:none;text-decoration:none;cursor:pointer;'><img src='images/icon-save.png' class='ic-save-search ' style='height:25px;width:20px;vertical-align: middle;' title='Save'> </a>");
    $aCancel = $("<a class='cancelContent' href='javascript:void(0)'  style='display:none;text-decoration:none;cursor:pointer;'><img src='images/icon-cancel.png' class='ic-cancel-search ' style='height:25px;width:20px;vertical-align: middle;' title='Cancel'> </a>");
    $aError = $("<span class='error' id='spanError'  style='display:none;'> Document Name is Required </a>");
    //var $item = $("<li id=" + item.Id + " divider=" + item.Idd + " pages=" + item.Count + " class='toc-li' ></li>");

    $item.data('item', item);
    $item.append($span);
    $item.append($txt);
    $item.append($aEdit);
    $item.append($aSave);
    $item.append($aCancel);
    $item.append($aError);
    $item.append($pagenum);

    item.start = $total_page_numb;

    $total_page_numb += item.Count; //计算后的$total_page_numb为下也项的起始页书，如果本项为最后一页，应该-1，作为总数

    toc.append($item);

    clickElement($pagenum, function (e) {
        $('.sj-book').turn('page', parseInt($pagenum.text()));
    });

    if (bookshelfEdit.allowEdit) {
        clickElement($aEdit, function (e) {
            adjustWidth($(this).parent().find('.txtContent')[0]);
            $(this).parent().find('.saveContent,.cancelContent,.txtContent,').show();
            $(this).parent().find('.itemName').hide();
            $(this).hide();
        });
    }

    clickElement($aSave, function (e) {

        var newDividerId, oldDividerId, oldPath, documentId,sourceNth,documnetOrder;

        $('.flashMoveFile').css("height", $(window).height());
        bookshelf.loadingMoveFile.apply();

        var $this = $(this),
	    itemAll = $item.data('item'),
        newName = $this.parent().find('.txtContent').val();
        if (newName == undefined || newName == "") {
            CheckValidation($this.parent().find('.txtContent'));
            return false;
        }
        else {
            CheckValidation($this.parent().find('.txtContent'));
        }

        newDividerId = $item.data('item').Idd;
        oldDividerId = $item.data('item').Idd;
        oldPath = $item.data('item').FullName;
        documentId = $item.data('item').Id;
        sourceNth = $current_divider_nth;
        documnetOrder = $item.data('item').DocumentOrder;
        
       

        $.ajax(
            {
                type: "POST",
                url: 'EFileFolderJSONService.asmx/MoveRenameFile',
                data: "{ newDividerId:'" + newDividerId + "', oldPath:'" + oldPath + "', oldDividerId:'" + oldDividerId + "', documentId:'" + documentId + "', documentOrder:'" + documnetOrder + "', isMove:'" + 0 + "', renameFile:'" + newName + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result && result.d) {
                        if (result.d[0].Value == "true") {
                            $.each($dividers.Items[sourceNth].files, function (i, e) {
                                if (e.Id && documentId && parseInt(e.Id) == parseInt(documentId)) {
                                    // reset object to new value (reset new documentId and start value)
                                    $.each(result.d, function (i, element) {
                                        if (element.Key) {
                                            e[element.Key] = element.Value;
                                        }
                                    });
                                }
                            });
                            selectedItem = $dividers.Items[sourceNth].files;
                            BindDividerOnIndexSearch(false, true);
                        }
                        else {
                            alert(result.d[0].Value);
                        }
                    }
                    bookshelf.loadedMoveFile.apply();
                }
            });

        //$.ajax(
        //    {
        //        type: "POST",
        //        url: 'EFileFolderJSONService.asmx/UpdateFileName',
        //        data: { path: itemAll.FullName, newName: newName, docId: itemAll.Id },
        //        success: function (data) {

        //            $this.parent().data('item').DisplayName = newName;
        //            $this.parent().find('.itemName').html(newName);
        //            resetEditIndex();
        //            $this.parent().find('.editContent').show();

        //            // rename data source item name
        //            $.each($items_auto_source, function (i, e) {
        //                if (itemAll.Id && parseInt(e.value) == parseInt(itemAll.Id)) {
        //                    e.label = newName;
        //                }
        //            });
                    
        //        }
        //    });
    });

    clickElement($aCancel, function (e) {
        $(this).parent().find('.txtContent').val((item.DisplayName ? item.DisplayName : item.Name));
        CheckValidation($(this).parent().find('.txtContent'));
        resetEditIndex();
        $(this).parent().find('.editContent').show();
        //$('#toc li').find('.editContent,.txtContent,.cancelContent,.saveContent,.error').hide();
        //$('#toc li').find('.itemName').show();
    });


    //merge the page list to $divider_details, without duplicate items
    $divider_details = $divider_details.concat([item]);

    var path = item.FullName;
    if (item.FullName.indexOf("%") >= 0) {
        path = item.FullName.replace(/\%/g, '$');
    }
    else if (item.FullName.indexOf('$') >= 0) {
        path = item.FullName.replace(/\\\u0024/g, '%');
    }

    PDFView.open(item.Id, path, 0); //executing by async

    $bookshelfPromises.Add(item.Id, new PDFJS.Promise());

}

// add one of item(file name) in a divider
function addLoadItem(toc, item) {
    var $span = $("<a class='li-span' target='_blank'>" + (item.DisplayName ? item.DisplayName : item.Name) + "</a>");
    var $item = $("<div id=" + item.Id + " divider=" + item.Idd + " class='toc-li'>" + (item.DisplayName ? item.DisplayName : item.Name) + "</div>");
    // $item.append($span);

    toc.append($span);

    clickElement($span, function (e) {
        //window.open("WebFlashViewer.aspx?V=2&ID=238", "", "toolbar=no,menubar=no,location=no,Resizable=yes,fullscreen=yes,status=no");
        window.open("Office/DocumentLoader.aspx?id=" + item.Id + "&dd=" + item.Idd);
    });
}

// add one of item in a divider
function addNote(toc, target, notes, dividerId) {
    var $pagenum = $("<a href='#' class='li-a'>" + $total_page_numb + "</a>");
    //var $span = $("<span class='li-span'>" + 'Page Note' + "</span>");
    var $span = $("<span class='li-span'>" + notes.Title + "</span>");

    //var $item = $("<li id=" + notes.Id + " divider=" + target['id'] + " pages=" + notes.length + " class='toc-li notes'></li>");

    if (target == null)
        var $item = $("<li id=" + notes.Id + " divider=" + dividerId + " pages=" + notes.length + " class='toc-li notes'></li>");
    else
        var $item = $("<li id=" + notes.Id + " divider=" + target['id'] + " pages=" + notes.length + " class='toc-li notes'></li>");

    $item.append($span);
    $item.append($pagenum);

    notes.start = $total_page_numb;

    //$total_page_numb += notes.length;
    $total_page_numb += 1;

    toc.append($item);

    clickElement($pagenum, function (e) {
        $('.sj-book').turn('page', parseInt($pagenum.text()));
    });

    $divider_details.push(notes);

}

function initshadow(e, args) {
    var shadow = $(e.currentTarget);
    var pagea = $('<div class="front-side"/>');
    var pageb = $('<div class="back-side"/>');

    var copyright = $('<div class="copyright"><span class="text-insert-effect">\
						Copyright 2012 - All rights reserved<br>www.efilefolders.com\
						</span></div>'),
		divider_back = $('<ul id="dividers-back"></ul>'),
		front_side = $('<div class="front-side-content"><ul id="dividers-front"></ul></div>'),

	    front_side_container = $('<div class="front-side-container">    \
                  <div class="table-content">\
                    <div class="table-content-title"><span class="text-insert-effect text-bold-effect">Table Contents</span></div>\
                    <div class="table-content-filter">\
                      <input id="item-filter-input" name="item-filter-input" type="text" value="" maxlength="20"  class="input-filter-text" placeholder="search here..."/>\
                      <input type="hidden" id="item-id"/>\
                    <a href="javascript:void(0)" style"text-decoration:none;display:none" id="clearID"> <img src="images/icon-cancel.png" title="Clear Search" style="height:25px;width:25px;vertical-align: middle;"></a></div>\
                    <div class="toc-wrapper">\
                      <ol id="toc" jqxb-templatecontainer="itemTemplate" jqxb-datasource="divideritems">\
                        <li class="toc-li" jqxb-template="itemTemplate" jqxb-templateitemidprfx="itemtrow">\
                        	<span class="li-span" jqxb-itemdatamember="name" jqxb-bindedattribute="text"></span>\
                            <a class="li-a" href="#" jqxb-itemdatamember="startpage" jqxb-bindedattribute="text"></a>\
                        </li>\
                      </ol>\
                    </div>\
                  </div>\
                  <div class="toc-logo"><span class="text-insert-effect">www.efilefolders.com</span></div></div></div>');
    pagea.css(args.a);
    pageb.css(args.b);

    pagea.append(front_side).append(front_side_container);
    pageb.append(copyright).append(divider_back);

    shadow.append(pagea);
    shadow.append(pageb);

    pagea.find('#clearID').click(function () {
        ClearDividerSearch();        
        pagea.find('#clearID').css("display", "none");
        $(".ui-autocomplete ui-menu ui-widget ui-widget-content ui-corner-all").html('');
        $("#item-filter-input").val("");
        BindDividerOnIndexSearch(false, true);       

        //$items_auto_source = [];


        //var items = $("#toc").data('items');
        //var toc1 = $('#toc').html('');
        ////var notes = $items_auto_source;

        //var target = $dividers.Items[$current_divider_nth];
        //var notes = target["notes"];


        //$total_page_numb = 1;
        //for (var i = 0; i < items.length; i++) {
        //    $items_auto_source.push({ value: items[i].Id, label: (items[i].DisplayName ? items[i].DisplayName : items[i].Name), path: items[i].FullName, start: $total_page_numb, count: items[i].Count });
        //    //add pages promises for this item
        //    for (var j = 1; j <= items[i].Count; j++) {
        //        var pagekey = items[i].Id + '-' + j;
        //        $page_promises.Add(pagekey, new PDFJS.Promise());
        //    }
        //}

        //for (var j = 0; j < items.length; j++) {
        //    addItem(toc1, items[j]);
        //}
        //// Hover effect to show/hide edit icons
        //assignHoverOnIndex();

        //if (notes.length > 0) {
        //    addNote(toc1, target, notes);
        //}
    });

    pagea.find('#item-filter-input').keyup(function () {
        var searchValue = $("#item-filter-input").val();
        if (searchValue != null && searchValue != undefined) {
            if (searchValue != '') {
                pagea.find('#clearID').css("display", "");                                
            }
            else {
                ClearDividerSearch();
                pagea.find('#clearID').css("display", "none");                
                $(".ui-autocomplete ui-menu ui-widget ui-widget-content ui-corner-all").html('');
                BindDividerOnIndexSearch(false, true);                
            };
        }
    });
   
    var sourceId, targetId, dividerId, sourceOrder, targetOrder;

    if (bookshelfEdit.allowEdit) {
        $("#toc").sortable({


            //  items: ':not(.notes)',
            items: 'li:not(.txtContent,.notes)',

            start: function (event, ui) {

                var start_pos = ui.item.index();
                ui.item.data('start_pos', start_pos);
                sourceId = $("#toc li").eq(start_pos).attr("id");
                // sourceOrder = $("#toc li").eq(start_pos).attr("order");
                dividerId = $("#toc li").eq(start_pos).attr("divider");

            },
            update: function (event, ui) {

                var start_pos = ui.item.data('start_pos');
                var end_pos = ui.item.index();

                if (end_pos > start_pos) {

                    targetId = $("#toc li").eq(end_pos - 1).attr("id");
                    // targetOrder = $("#toc li").eq(end_pos - 1).attr("order");
                }
                else {
                    targetId = $("#toc li").eq(end_pos + 1).attr("id");
                    //targetOrder = $("#toc li").eq(end_pos +  1).attr("order");
                }

            },


            stop: function (event, ui) {

                $('.flashMoveFile').css("height", $(window).height());
                bookshelf.loadingMoveFile.apply();

                if (targetId != "" && targetId != undefined) {
                    $.ajax(
                  {
                      type: "POST",
                      url: 'EFileFolderJSONService.asmx/UpdateDocumentOrder1',
                      data: "{ sourceId:'" + sourceId + "', targetId:'" + targetId + "', dividerId:'" + dividerId + "', SourceDocOrder:'" + 0 + "', TargetDocOrder:'" + 0 + "'}",
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      success: function (data) {

                          var docObj = JSON.parse(data.d);

                          if (docObj !== undefined && docObj.length > 0) {


                              //change Documentorder in $items_auto_source
                              $.each(docObj, function (i, e) {
                                  $.each($items_auto_source, function (j, item) {
                                      if (e.DocumentID && item.value && parseInt(item.value) == parseInt(e.DocumentID)) {
                                          item.Order = e.DocumentOrder;
                                      }
                                  });
                              });


                              //var tempArray=[];
                              //var minOrder = 0;
                              //var tempObj;
                              //var length = $items_auto_source.length;
                              //for (var i = 0; i < length; i++) {
                              //    $.each($items_auto_source, function (i, e) {
                              //        if (!e.isNote) {
                              //            if (minOrder == 0) {
                              //                minOrder = e.Order;
                              //                tempObj = e;
                              //            }
                              //            else {
                              //                if (e.Order && minOrder && parseInt(e.Order) < parseInt(minOrder)) {
                              //                    minOrder = e.Order;
                              //                    tempObj = e;
                              //                }
                              //            }
                              //        }
                              //    });
                              //    tempArray.push(tempObj);
                              //    $items_auto_source = $.grep($items_auto_source, function (e) {
                              //        if (e.value && tempObj.value && parseInt(e.value) == parseInt(tempObj.value)) {
                              //            return false;
                              //        }
                              //        else
                              //            return true;
                              //    });
                              //    minOrder = 0;
                              //}
                              //$items_auto_source = $.grep($items_auto_source, function (e) {
                              //    tempArray.push(e);
                              //    return false;
                              //});

                              //sorting document by Documentorder  in $items_auto_source 
                              $items_auto_source = SortArray($items_auto_source, true);

                              var pageStart;
                              pageStart = 1;
                              //change pageStart in $items_auto_source
                              $.each($items_auto_source, function (i, item) {
                                  item.start = pageStart;
                                  pageStart += item.count;
                              });

                              //change Pageno. in li.
                              $.each($items_auto_source, function (i, e) {
                                  $("#toc li").each(function (j, item) {
                                      if (e.value && item.id && parseInt(item.id) == parseInt(e.value) && item.lastChild.innerHTML != undefined) {
                                          item.lastChild.innerHTML = e.start;
                                          //item.lastChild.innerHtml = e.start;
                                      }
                                      //alert(item.innerText + " " + item.label);
                                  });

                              });

                              BindDividerOnIndexSearch(false, false, 0, true);
                              //change Documentorder in $divider_item
                              $.each($dividers.Items[$current_divider_nth].files, function (j, item) {
                                  $.each($items_auto_source, function (i, e) {

                                      if (item.Id && e.value && parseInt(e.value) == parseInt(item.Id)) {
                                          item.DocumentOrder = e.Order;
                                          item.start = e.start;
                                      }
                                  });
                              });

                              //$dividers.Items[$current_divider_nth].files.sort(function (a, b) {
                              //    debugger;

                              //    return (a.DocumentOrder > b.DocumentOrder) ? 1 : -1;
                              //});

                              //sorting document by Documentorder  in  $dividers.Items[$current_divider_nth].files
                              $dividers.Items[$current_divider_nth].files = SortArray($dividers.Items[$current_divider_nth].files, false);
                          }
                      },
                      error: function (result) {
                          console.log('Failed' + result.responseText);
                      }
                  });
                }
                targetId = "";
                sourceId = "";
                bookshelf.loadedMoveFile.apply();

                ClearDividerSearch();


            },
        });
    }
}

function ClearDividerSearch() {
    $('#results-count').text("");
    $('#search-item-text').val("");
    $('#book-search-items').empty();
    $('.textLayer').find('.selected').remove();
}

function SortArray(arrayObj, isChildArray) {

    var tempArray = [];
    var minOrder = 0;
    var tempObj;
    var length = arrayObj.length;
    for (var i = 0; i < length; i++) {
        $.each(arrayObj, function (i, e) {
            if (!e.isNote) {
                if (minOrder == 0) {
                    if (isChildArray)
                        minOrder = e.Order;
                    else
                        minOrder = e.DocumentOrder;

                    tempObj = e;
                }
                else {

                    if (isChildArray) {
                        if (e.Order && minOrder && parseInt(e.Order) < parseInt(minOrder)) {
                            minOrder = e.Order;
                            tempObj = e;
                        }
                    }
                    else {
                        if (e.DocumentOrder && minOrder && parseInt(e.DocumentOrder) < parseInt(minOrder)) {
                            minOrder = e.DocumentOrder;
                            tempObj = e;
                        }
                    }
                }
            }
        });

        if (tempObj) {
            tempArray.push(tempObj);
            arrayObj = $.grep(arrayObj, function (e) {
                if (isChildArray) {
                    if (e.value && tempObj.value && parseInt(e.value) == parseInt(tempObj.value)) {
                        return false;
                    }
                    else
                        return true;
                }
                else {
                    if (e.Id && tempObj.Id && parseInt(e.Id) == parseInt(tempObj.Id)) {
                        return false;
                    }
                    else
                        return true;
                }
            });
        }
        minOrder = 0;
        tempObj = null;
    }
    arrayObj = $.grep(arrayObj, function (e) {
        tempArray.push(e);
        return false;
    });
    return tempArray;
}

function updateshadow(e, args) {
    
    var newDividerId, oldDividerId, oldPath, documentId,newDocumentId;
    var sourceNth, targetNth, currentDocument;

    if (bookshelfEdit.allowEdit) {
        $("#dividers-front li,#dividers-back li").droppable({
            //$("#dividers-front > li").droppable({
            //var $tab_items = $("#dividers-front > li", $tabs).droppable({
            //tolerance: 'pointer',
            tolerance: "touch",
            drop: function (event, ui) {

                $('.flashMoveFile').css("height", $(window).height());
                bookshelf.loadingMoveFile.apply();

                newDividerId = $(this).attr('id').split('divider')[1];
                oldDividerId = ui.draggable.attr('divider');
                oldPath = ui.draggable.attr('fileName');
                documentId = ui.draggable.attr('id');
                sourceNth = $current_divider_nth;
                targetNth = $(this).attr('nth');


                if (newDividerId && oldDividerId && parseInt(oldDividerId) == parseInt(newDividerId)) {
                    bookshelf.loadedMoveFile.apply();
                    return false;
                }

                $.ajax(
                {
                    type: "POST",
                    url: 'EFileFolderJSONService.asmx/MoveRenameFile',
                    data: "{ newDividerId:'" + newDividerId + "', oldPath:'" + oldPath + "', oldDividerId:'" + oldDividerId + "', documentId:'" + documentId + "', documentOrder:'" + 0 + "', isMove:'" + 1 + "', renameFile:'" + "" + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        if (result && result.d) {
                            if (result.d[0].Value == "true") {

                                $dividers.Items[sourceNth].files = $.grep($dividers.Items[sourceNth].files, function (e) {
                                    if (e.Id && documentId && parseInt(e.Id) == parseInt(documentId)) {
                                        if ($dividers.Items[targetNth].files) {

                                            // reset object to new value (reset new documentId and start value)
                                            $.each(result.d, function (i, element) {
                                                if (element.Key == "Id")
                                                    newDocumentId = element.Value;
                                                if (element.Key) {
                                                    e[element.Key] = element.Value;
                                                }
                                            });
                                            var lastFileObj = $dividers.Items[targetNth].files[$dividers.Items[targetNth].files.length - 1];

                                            if (lastFileObj) {
                                                var newStart = lastFileObj.start + lastFileObj.Count;
                                                e.start = newStart;
                                            }
                                            else
                                                e.start = 1;

                                            // add document in target tab
                                            $dividers.Items[targetNth].total = $dividers.Items[targetNth].total + e.Count;
                                            $dividers.Items[targetNth].files.push(e);
                                        }
                                        return false;
                                    }
                                    else
                                        return true;
                                });

                                $dividers.Items[sourceNth].qotes = $.grep($dividers.Items[sourceNth].qotes, function (e) {
                                    if (e.PageKey) {
                                        var oldDocumentId = e.PageKey.split('-')[0];
                                        var pageNumber = e.PageKey.split('-')[1];
                                        if (oldDocumentId && documentId && parseInt(documentId) == parseInt(oldDocumentId)) {
                                            if (newDocumentId && pageNumber) {
                                                e.PageKey = newDocumentId + "-" + pageNumber;
                                                $dividers.Items[targetNth].qotes.push(e);
                                                return false;
                                            }
                                        }
                                        else
                                            return true;
                                    }
                                });

                                selectedItem = $dividers.Items[sourceNth].files;
                                BindDividerOnIndexSearch(false, true);
                            }
                            else {
                                alert(result.d[0].Value);
                            }
                        }
                        bookshelf.loadedMoveFile.apply();
                    }
                });
            },
            start: function (event, ui) {
            },
            stop: function (event, ui) {
            }
        });
    }

    var shadow = $(e.currentTarget),
		pagea = shadow.children('.front-side');
    pageb = shadow.children('.back-side');

    pagea.css(args.a);
    pageb.css(args.b);
}

function CheckValidation(fieldObj) {
    if (fieldObj.val() == "") {
        fieldObj.css('border', '1px solid red');
    }
    else {
        fieldObj.css('border', '1px solid black');
    }
}

function resetEditIndex() {
    $('#toc li').find('.editContent,.txtContent,.cancelContent,.saveContent').hide();
    $('#toc li').find('.itemName').show();
}

function assignHoverOnIndex() {
    // Hover effect to show/hide edit icons
    $('#toc li').hover(function () {
        $('#toc li').find('.editContent').hide();
        $(this).find('.editContent').show();
    }, function () {
        resetEditIndex();
        $(this).find('.txtContent').val($(this).find('.itemName').html());
        CheckValidation($(this).find('.txtContent'));
    });
}
function adjustWidth(obj) {
    var width = (obj.value.length) * 8.0
    obj.style.width = width > 300 ? 300 : width + "px";
}