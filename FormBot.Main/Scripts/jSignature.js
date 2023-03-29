(function ($) {
    var methods = {
        init: function (options) {
            if (!document.createElement('canvas').getContext) {
                alert("Oops, you need a newer browser to use this.");
                return;
            }

            var settings = {
                'width': '220',
                'height': '50',
                'color': '#000',
                'lineWidth': 1,
                'bgcolor': '#fff'
            };

            return this.each(function () {
                if (options) {
                    $.extend(settings, options);
                }

                var canvas = $("<canvas id='doc-previewer' width='" + settings.width + "' height='" + settings.height + "'></canvas>").appendTo($(this))[0];
                // Check for compatibility
                if (canvas && canvas.getContext) {
                    var ctx = canvas.getContext("2d");
                    ctx.lineWidth = settings.lineWidth;
                    ctx.strokeStyle = ctx.fillStyle = settings.color;

                    // Add custom class if defined
                    if (settings.cssclass && $.trim(settings.cssclass) != "") {
                        $(canvas).addClass(settings.cssclass);
                    }
                    var x;
                    var y;
                    var hasMoved;

                    canvas.ontouchstart = canvas.onmousedown = function (e) {
                        ctx.beginPath();
                        hasMoved = false;
                        cdIsmouseIn = true;
                        var first = (e.changedTouches && e.changedTouches.length > 0 ? e.changedTouches[0] : e);
                        // Mobile Device Quirks
                        // Android: clientY == screenY
                        // IOS 4.3: clientY == pageY
                        x = (first.screenX != first.clientX ? first.clientX : first.screenX) - $(this).offset().left + (first.pageY != first.clientY ? $(window).scrollLeft() : 0);
                        y = (first.screenY != first.clientY ? first.clientY : first.screenY) - $(this).offset().top + (first.pageY != first.clientY ? $(window).scrollTop() : 0);
                        ctx.moveTo(x, y);
                        if ($.isFunction(settings.mousedown)) {
                            settings.mousedown();
                        }
                    }

                    canvas.ontouchend = canvas.onmouseup = function (e) {
                        if (!hasMoved) {
                            ctx.fillRect(x, y, (settings.lineWidth < 2 ? 2 : settings.lineWidth), (settings.lineWidth < 2 ? 2 : settings.lineWidth));
                        }
                        if ($.isFunction(settings.mouseup)) {
                            settings.mouseup();
                        }
                        x = null;
                        y = null;
                        ctx.closePath();
                    }

                    canvas.onmousemove = canvas.ontouchmove = function (e) {
                        if (x == null || y == null) {
                            return;
                        }

                        if (cdIsmouseIn == true) {
                            hasMoved = true;
                            if (e.changedTouches && e.changedTouches.length > 0) {
                                var first = e.changedTouches[0];
                                x = first.pageX - $(window).scrollLeft();
                                y = first.pageY - $(window).scrollTop();
                            }
                            else {
                                x = e.clientX;
                                y = e.clientY;
                            }
                            x -= $(this).offset().left - $(window).scrollLeft();
                            y -= $(this).offset().top - $(window).scrollTop();
                            ctx.lineTo(x, y);
                            ctx.moveTo(x, y);

                        } ctx.stroke();
                        e.stopPropagation();
                        e.preventDefault();
                    }
                }
            });
        },
        clear: function () {
            var canvas = $(this).children("canvas");
            var ctx = canvas[0].getContext("2d");
            var color = ctx.strokeStyle;
            var lineWidth = ctx.lineWidth;
            var w = $(canvas).attr("width");
            canvas.attr("width", 0).attr("width", w);
            ctx.strokeStyle = color;
            ctx.lineWidth = lineWidth;
            ctx.beginPath();
            return $(this);
        },
        getData: function () {
            var canvas = $(this).children("canvas");
            if (canvas.length) return canvas[0].toDataURL();
            else return;
        },
        importData: function (dataurl) {
            var img = new Image();
            var cv = $(this).children("canvas")[0];
            img.src = dataurl;
            img.onload = function () {
                var dw = (img.width < cv.width) ? img.width : cv.width;
                var dh = (img.height < cv.height) ? img.height : cv.height;
                cv.getContext("2d").drawImage(img, 0, 0, dw, dh);
            }
        }
    };

    $.fn.jSignature = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.jSignature');
        }
    };
})(jQuery);
