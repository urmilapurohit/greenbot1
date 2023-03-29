var ww = document.body.clientWidth;


$(document).ready(function() {
	$(".menu li a").each(function() {
		if ($(this).next().length > 0) {
			$(this).addClass("parent");
		};
	})
	$(".menu li li a").each(function() {
		if ($(this).next().length > 0) {
			$(this).removeClass("parent");
		};
	})
	
	$(".menu-toggle").click(function(e) {
		e.preventDefault();
		$(this).toggleClass("active");
		$(".menu").toggle();
	});
	adjustMenu();
})

$(window).bind('resize orientationchange', function() {
	ww = document.body.clientWidth;
	adjustMenu();
});

var adjustMenu = function() {
	if (ww < 993) {
		$(".menu-toggle").css("display", "block");
		
		if (!$(".menu-toggle").hasClass("active")) {
			$(".menu").hide();
		} else {
			$(".menu").show();
		}
		$(".menu li").unbind('mouseenter mouseleave');
		
		$(".menu li a.parent").unbind('click').bind('click', function(e) {
			var istoggle =$(this).parent("li").hasClass("hover");
			
			$(".menu li").removeClass("hover");
			// must be attached to anchor element to prevent bubbling
			e.preventDefault();
			if(istoggle){
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
		$(".menu li").unbind('mouseenter mouseleave').bind('mouseenter mouseleave', function() {
		 	// must be attached to li so that mouseleave is not triggered when hover over submenu
		 	$(this).toggleClass('hover');
		 });
	}
}

