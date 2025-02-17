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

//$(window).on("pageshow", function (event) {
//	if (event.originalEvent.persisted) {
//		hideLoading();
//	}
//});

window.onpopstate = function () {
	hideLoading();
	console.log("heree popstate")
};