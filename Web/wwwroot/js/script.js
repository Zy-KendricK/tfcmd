(function($){
	"use strict";
	
	function elementExists(selector) {
        return $(selector).length > 0;
    }
	
	// Function for search functionality
    function searchModal() {
		if (!elementExists(".search-btn")){return;}
		
        // When the "search-btn" is clicked, show the search modal
		$(".search-btn").click(function() {
			$(".search-modal").addClass("show");
		});
		// When the "search-close" is clicked, hide the search modal
		$(".search-close").click(function() {
			$(".search-modal").removeClass("show");
		});
    }
	
	// Function for fixed header functionality
    function fixedHeader() {
		if (!elementExists(".fixed-header")){return;}
		
        // Add a scroll event listener to the window
		$(window).scroll(function() {
				var scrollPosition = $(this).scrollTop();
				var header = $('.fixed-header');

				// If scrolled more than 500 pixels, add the "fixed" class to the header
				if (scrollPosition > 500) {
					header.addClass('fixed');
				} else {
					// Otherwise, remove the "fixed" class
					header.removeClass('fixed');
				}
		})
    }
	
	// Function for locking screen functionality
    function navbarToggler() {
        if (!elementExists(".navbar-toggler")){return;}
		
        // Toggle the class "screen-fixed" on the body when ".navbar-toggler" is clicked
		$(".navbar-toggler").click(function() {
			$("body").toggleClass("screen-fixed");
		});
    }
	
	// Function for mobile navigation functionality
    function mobileNavigation() {
        if (!elementExists(".navbar-nav")){return;}
		
		if ($(window).width() <= 992) {
			// Add a click event handler to anchor elements within ".navbar-nav"
			$(".navbar-nav > li > a").on('click', function() {
				var $listParent = $(this).parent();

				// Toggle the "open" class on the parent list item
				if ($listParent.hasClass("open")) {
					$listParent.removeClass("open");
				} else {
					$(".navbar-nav > li").removeClass("open");
					$listParent.addClass("open");
				}
			});
		}
    }
	
	// Function for menu category navigation
    function menuCategoryNav() {
        if (!elementExists("[data-name]")){return;}
			
		// Add hover event handlers to elements with a "data-name" attribute
		$('[data-name]').hover(function() {
			var $this = $(this);
			var target = $this.data('name');

			// Manage the "active" and "show" classes
			$('[data-name]').removeClass('active');
			$this.addClass('active');
			$('[data-target]').removeClass('show');
			$('[data-target="' + target + '"]').addClass('show');
		});
    }
	
	// Function for Bootstrap validation
    function bsValidation() {
		if (!elementExists(".was-validated")){return;}
			
		// Fetch all forms with the class "needs-validation"
		const forms = document.querySelectorAll('.needs-validation')

		// Loop over them and prevent submission on invalid forms
		Array.from(forms).forEach(form => {
			form.addEventListener('submit', event => {
				if (!form.checkValidity()) {
					event.preventDefault()
					event.stopPropagation()
				}
				form.classList.add('was-validated')
			}, false)
		});
    }
	
	// Function for mobile number input restriction
    function mobileNumber() {
        if (!elementExists(".mobile-number")){return;}
		
		// Add input event listener to elements with the class "mobile-number"
		$('.mobile-number').on('input', function() {
			var inputVal = $(this).val();
			var numericVal = inputVal.replace(/\D/g, ''); // Remove non-numeric characters

			// Limit the input to 10 characters
			if (numericVal.length > 10) {
				$(this).val(numericVal.slice(0, 10));
			} else {
				$(this).val(numericVal);
			}
		});
    }
	
	// Function for website preloader
    function websitePreloader() {
		// Create HTML content for the preloader
		var preloaderContent = `<div id="preloader">
			<div class="preloader-swapping">
				<img src="assets/images/preloader.png" alt="">
			</div>
		</div>`;

		// Append the preloader to the body
		$('body').append(preloaderContent);

		// Set a timeout to fade out and remove the preloader after 2 seconds
		setTimeout(function() {
			$('#preloader').addClass('active').fadeOut(500);
		}, 2000);
    }
	
	// Function for scroll-to-top button
    function windowTop() {
        // Create HTML content for the scroll-to-top button
		var windowTopContent = `<button type="button" class="window-top">TOP</button>`;
		
		// Append the button to the body
		$('body').append(windowTopContent);
		
		var btn = $('.window-top');

		// Add a scroll event listener to the window
		$(window).scroll(function() {
			if ($(window).scrollTop() > 300) {
				btn.addClass('show');
			} else {
				btn.removeClass('show');
			}
		});

		// Scroll to the top of the page when the button is clicked
		btn.on('click', function(e){
			e.preventDefault();
			$('html, body').animate({scrollTop: 0}, 300);
		});
    }
	
	// Add a scroll event listener to elements with the class "split-box"
	function imageSplit(){		
		// Define a function to check if an element is in the viewport
		function isScrolledIntoView(elem) {
			const viewportTop = $(window).scrollTop();
			const viewportBottom = viewportTop + $(window).height();
			const elemTop = elem.offset().top;
			const elemBottom = elemTop + elem.height();
			return elemBottom <= viewportBottom && elemTop >= viewportTop;
		}
		
		$('.split-effect').each(function() {
			if (isScrolledIntoView($(this))) {
				$(this).addClass('split-show');
			}
		});
	}
	
	// Initialize functions when the document is ready
    $(document).ready(function() {
        searchModal();
        navbarToggler();
        mobileNavigation();
        bsValidation();
        mobileNumber();
        windowTop();
        menuCategoryNav();
        websitePreloader();
    });
	
    // Reinitialize when the window is resized
    $(window).on('resize', function() {
        mobileNavigation();
    });

    // Reinitialize when the window is scrolled
    $(window).on('scroll', function() {
        fixedHeader();
        imageSplit();
    });
	
})(jQuery);