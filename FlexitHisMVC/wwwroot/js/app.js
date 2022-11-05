//var json = localStorage.getItem("json");
//var parsedJSON = JSON.parse(json);

//if (parsedJSON !== null && parsedJSON !== "") {

   

//    //$("#product-end-date")[0].setAttribute('min', today);

//    //-----


//}
//else {
  
//    //$('#systemModal').modal('show')
//    //window.location.replace("file:///Users/rufat/Desktop/Dekor%20Stone/login/dekor_stone.html");
//}




function showLoading() {
    $('#loadingModal').modal('show');

    $('#loadingModalText').html(`<center><div class="spinner-grow text-light" style="width: 7rem; height: 7rem;" role="status">
   
  </div></center>`);
   
}
function hideLoading() {
    $('#loadingModal').modal('hide');
}

    if (localStorage.json) {
        var json = localStorage.json;
var parsedJSON = JSON.parse(json);


        $("#fullName").text(parsedJSON.data[0].personal.name + " " + parsedJSON.data[0].personal.surname)

        if (!localStorage.selectedHospital) {
            $("#selectHospitalDropdownButton").text(parsedJSON.hospitals[0].hospitalName)
            $('#hospitalLogo').text(parsedJSON.hospitals[0].hospitalName)
            localStorage.selectedHospital = parsedJSON.hospitals[0].id;
            localStorage.selectedHospitalName = parsedJSON.hospitals[0].hospitalName;
        }
        else {
            $("#selectHospitalDropdownButton").text(localStorage.selectedHospitalName)
            $('#hospitalLogo').text(localStorage.selectedHospitalName)
        }
        $.each(parsedJSON.hospitals, function () {
            $("#selectHospitalDropdownItems").append(`<a class="dropdown-item" id="${this.id}" onclick="localStorage.selectedHospital='${this.id}';localStorage.selectedHospitalName='${this.hospitalName}'; $('#selectHospitalDropdownButton').text('${this.hospitalName} '); $('#hospitalLogo').text('${this.hospitalName} ');">${this.hospitalName}</a>`)

        });
        //if (localStorage.lastActivePage) {


        //    $("#main-content").load(localStorage.lastActivePage)

        //}
        //else {
        //    $("#main-content").load("/menu")
        //}
    }
    else {
        
        $('#systemModalTitle').text("Sessiyanız başa çatıb");
        $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
        $('#systemModalBtn').removeAttr("hidden");
        $('#systemModal').modal('show')
        //window.location.replace("/");

    }
   

function Routing(obj,link) {
  //alert($(obj).val())
    if (typeof (Storage) !== "undefined") {

        localStorage.lastActivePage = link
    } else {

        // Sorry! No Web Storage support..
    }

  $(".nav-link").removeClass('active');
    $(obj).addClass("active")

    $("#main-content").load(link)
}
(function ($) {

    "use strict";

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });

    };
    fullHeight();

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

})(jQuery);
function logout() {
    $('#warningModal').modal('show')
    $('#warningText').text('Çıxış etməyinizdən əminsiniz?');
    $("#warningModalButton").text("Bəli");
    $("#warningModalButton").on("click", function () {
        localStorage.json = '';
        window.location.replace("/login");
    });
}