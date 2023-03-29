// menu

var ww = '';


$(document).ready(function () {
    $(".menu li a").each(function () {
        if ($(this).next().length > 0) {
            $(this).addClass("parent");
        };
    })
    $(".menu li li a").each(function () {
        if ($(this).next().length > 0) {
            $(this).removeClass("parent");
        };
    })

    $(".menu-toggle").click(function (e) {
        e.preventDefault();
        $(this).toggleClass("active");
        $(".menu").toggle('1000');
    });

    ww = document.body.clientWidth;
    adjustMenu();
})

$(window).bind('resize orientationchange', function () {
    ww = document.body.clientWidth;
    adjustMenu();
});

var adjustMenu = function () {
    if (ww < 993) {
        $(".menu-toggle").css("display", "block");

        if (!$(".menu-toggle").hasClass("active")) {
            $(".menu").hide();
        } else {
            $(".menu").show();
        }
        $(".menu li").unbind('mouseenter mouseleave');

        $(".menu li a.parent").unbind('click').bind('click', function (e) {
            var istoggle = $(this).parent("li").hasClass("hover");

            $(".menu li").removeClass("hover");
            // must be attached to anchor element to prevent bubbling
            e.preventDefault();
            if (istoggle) {
                $(this).parent("li").addClass("hover");
            }
            $(this).parent("li").toggleClass("hover");
        });
    }
    else if (ww >= 993) {
        $(".menu-toggle").css("display", "none");
        $(".menu").show();
        $(".menu li").removeClass("hover");
        $(".menu li a").unbind('click');
        $(".menu li").unbind('mouseenter mouseleave').bind('mouseenter mouseleave', function () {
            // must be attached to li so that mouseleave is not triggered when hover over submenu
            $(this).toggleClass('hover');
        });
    }
}


// date-pick ==============================
$(document).ready(function () {
    //$('#date-pick, #date-pick1, .date-pick').datepicker({
	//	format: "dd/mm/yyyy",
	//	autoclose: true
	//});  

 	$('.dropdown-toggle').dropdown();
    //$('#date-pick, #date-pick1, .date-pick').datepicker({
    //    format: "dd/mm/yyyy",
    //    autoclose: true
    //}).on('change', function () {
    //    $(this).valid();
    //});




    // mail_filter

    $('.ic-mail-filter ').click(function (e) {
        e.stopPropagation();
        $('.mail-filterbox').slideToggle("50");
    });


    // search_filter
    $("#searchlink").click(function (e) {
        $(".checkboxgroup").slideToggle();
        e.preventDefault();
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });


    // Jtoggle
    $(".jtoggle").click(function (e) {
        e.preventDefault();
        $(".find_job_leftpanel").toggleClass("active");
        $(".jtoggle").toggleClass("active");
        $(".job_calendar").toggleClass("push");


    });

    $(".option-box").on("click", function (e) {
        e.stopImmediatePropagation();
    });
    $('.modal').on('hidden.bs.modal', function (e) {
        if ($('.modal').hasClass('in')) {
            $('body').css({ "padding-right": $(window).outerWidth() - $('body').outerWidth() }).addClass('modal-open');
        } else {
            $('body').removeAttr("style");
        }
    });
});


function ConvertDateToTick(objDate, currentFormat) {
    if (objDate != null) {
        switch (currentFormat) {
            case 'dd/mm/yyyy':
                var splitDate = objDate.split("/");
                var formatDate = new Date(splitDate[2], splitDate[1] - 1, splitDate[0]);
                return formatDate.getTime();
                break;
            case 'mm/dd/yyyy':
                var splitDate = objDate.split("/");
                var formatDate = new Date(splitDate[2], splitDate[0] - 1, splitDate[1]);
                return formatDate.getTime();
                break;
            case 'yyyy/mm/dd':
                var splitDate = objDate.split("/");
                var formatDate = new Date(splitDate[0], splitDate[1] - 1, splitDate[2]);
                return formatDate.getTime();
                break;
            case 'dd-mm-yyyy':
                var splitDate = objDate.split("-");
                var formatDate = new Date(splitDate[2], splitDate[1] - 1, splitDate[0]);
                return formatDate.getTime();
                break;
            case 'mm-dd-yyyy':
                var splitDate = objDate.split("-");
                var formatDate = new Date(splitDate[2], splitDate[0] - 1, splitDate[1]);
                return formatDate.getTime();
                break;
            case 'yyyy-mm-dd':
                var splitDate = objDate.split("-");
                var formatDate = new Date(splitDate[0], splitDate[1] - 1, splitDate[2]);
                return formatDate.getTime();
                break;
            default:
                return '';
        }
    }
    else {
        return '';
    }
}

function ConvertToDateWithFormat(data, format) {
    if (data != null) {
        var date = new Date(parseInt(data.replace('/Date(', '')));
        switch (format) {
            case 'dd/mm/yyyy':
                return ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear();
                break;
            case 'mm/dd/yyyy':
                return ("0" + (date.getMonth() + 1)).slice(-2) + '/' + ("0" + date.getDate()).slice(-2) + '/' + date.getFullYear();
                break;
            case 'yyyy/mm/dd':
                return date.getFullYear() + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + ("0" + date.getDate()).slice(-2);
                break;
            case 'dd-mm-yyyy':
                return ("0" + date.getDate()).slice(-2) + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + date.getFullYear();
                break;
            case 'mm-dd-yyyy':
                return ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2) + '-' + date.getFullYear();
                break;
            case 'yyyy-mm-dd':
                return date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2);
                break;
            case 'dd/mm/yyyy hh:mm:ss':
                return ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
                break;
            default:
                return '';
        }
    }
    else {
        return '';
    }
}




