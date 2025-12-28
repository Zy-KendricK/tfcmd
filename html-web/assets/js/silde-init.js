// This swiper is for ( Swiper Menu Category )
var swiperMenuCategory = new Swiper(".swiper-menu-category", {
	slidesPerView: 4,
	spaceBetween: 25,
	loop: true,
	
	breakpoints:{
		0: {
			slidesPerView: 1,
			centeredSlides: true,
		},
		591: {
			slidesPerView: 2,
		},
		767: {
			slidesPerView: 4,
		},
		991: {
			slidesPerView: 4,
		},
		1366: {
			slidesPerView: 4,
		},
	},
});

// This swiper is for ( Instagram Post )
var swiperInsta = new Swiper(".swiper-insta", {
	slidesPerView: 8,
	loop: true,
	
	breakpoints: {
		0: {
			slidesPerView: 2.5,
			centeredSlides: true,
		},
		591: {
			slidesPerView: 3,
		},
		767: {
			slidesPerView: 4,
		},
		991: {
			slidesPerView: 6,
		},
		1366: {
			slidesPerView: 8,
		},
	},
});

// This swiper is for ( Catogary Slider )
var swiperCatogary = new Swiper(".swiper-catogary", {
	slidesPerView: 1,
	spaceBetween: 30,
	loop: true,
	
	breakpoints:{
		0:{
			slidesPerView: 1.4,
			spaceBetween: 15,
		},
		591:{
			slidesPerView: 2,
			spaceBetween: 30,
		},
		767:{
			slidesPerView: 3,
		},
		991: {
			slidesPerView: 4,
		},
		1366: {
			slidesPerView: 5,
		},
	},
});

// This swiper is for ( Post Gallery Swiper )
var swiperPpostGallery = new Swiper(".post-gallery-swiper", {
	spaceBetween: 30,
	slidesPerView: 3,
	navigation: {
		nextEl: ".swiper-button-next",
		prevEl: ".swiper-button-prev",
	},
	breakpoints: {
		0: {
			slidesPerView: 1,
		},
		768:{
			slidesPerView: 2,
		},
		991: {
			slidesPerView: 3,
		},
	},
});

// This swiper is for ( Post Thumbs Swiper )
var swiperPostThumbs = new Swiper(".post-thumbs-swiper", {
	spaceBetween: 10,
	slidesPerView: 6,
	freeMode: true,
	watchSlidesProgress: true,
	breakpoints: {
		0: {
			slidesPerView: 3,
		},
		768:{
			slidesPerView: 4,
		},
		1024: {
			slidesPerView: 5,
		},
		1280: {
			slidesPerView: 6,
		},
	},
});

var swiperPost = new Swiper(".post-swiper", {
	spaceBetween: 10,
	navigation: {
		nextEl: ".swiper-button-next",
		prevEl: ".swiper-button-prev",
	},
	thumbs: {
		swiper: swiperPostThumbs,
	},
});