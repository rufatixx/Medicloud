// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showLoading() {
	$('#loadingModal').show();

}
function hideLoading() {
	$('#loadingModal').hide();
}


window.addEventListener('beforeunload', function (event) {

	showLoading();

});


function formatCustomDateTime(dateString) {
	const date = new Date(dateString);
	const day = date.getDate();
	const monthNames = [
		"yanvar", "fevral", "mart", "aprel", "may", "iyun",
		"iyul", "avqust", "sentyabr", "oktyabr", "noyabr", "dekabr"
	];
	const month = monthNames[date.getMonth()];
	const year = date.getFullYear();

	const hours = date.getHours().toString().padStart(2, '0');
	const minutes = date.getMinutes().toString().padStart(2, '0');
	const seconds = date.getSeconds().toString().padStart(2, '0');

	return `${day} ${month} ${year} ${hours}:${minutes}:${seconds}`;
}
//$(window).on("pageshow", function (event) {
//	if (event.originalEvent.persisted) {
//		hideLoading();
//	}
//});

window.onpopstate = function () {
	hideLoading();
	console.log("heree popstate")
};