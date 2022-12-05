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

//if (localStorage.length > 0) {


//    if (!localStorage.selectedHospital) {
//        if (parsedJSON.data[0].hospitals.length > 0) {
//            $("#fullName").text(`${parsedJSON.data[0].personal.name} ${parsedJSON.data[0].personal.surname}`)
//            $("#selectHospitalDropdownButton").text(parsedJSON.data[0].hospitals[0].hospitalName)
//            $('#hospitalLogo').text(parsedJSON.data[0].hospitals[0].hospitalName)
//            localStorage.selectedHospital = parsedJSON.data[0].hospitals[0].hospitalID;
//            localStorage.selectedHospitalName = parsedJSON.data[0].hospitals[0].hospitalName;
//        }
//        else {
//            //$('#systemModalTitle').text("Sizin heç bir xəstəxanaya icazəniz yoxdur");
//            //$('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa texniki dəstək xidmətimizə müraciət edin</p>`);
//            //$('#systemModalBtn').show();
//            //$('#systemModalBtn').text("Çıxış");
//            //$('#systemModal').modal('show')
//        }
//    }
//    else {
//        $("#selectHospitalDropdownButton").text(localStorage.selectedHospitalName)
//        $('#hospitalLogo').text(localStorage.selectedHospitalName)
//    }
//    $.each(parsedJSON.data[0].hospitals, function () {
//        $("#selectHospitalDropdownItems").append(`<a class="dropdown-item" id="${this.id}" onclick="localStorage.selectedHospital='${this.id}';localStorage.selectedHospitalName='${this.hospitalName}'; $('#selectHospitalDropdownButton').text('${this.hospitalName} '); $('#hospitalLogo').text('${this.hospitalName} ');">${this.hospitalName}</a>`)

//    });




//}
//else {
//    localStorage.clear();
//    $('#systemModalTitle').text("Sessiyanız başa çatıb");
//    $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
//    $('#systemModalBtn').removeAttr("hidden");
//    //$('#systemModal').modal('show')
//    //window.location.replace("file:///Users/rufat/Desktop/Dekor%20Stone/login/dekor_stone.html");
//}


if (localStorage.json) {
    var json = localStorage.json;
    var parsedJSON = JSON.parse(json);

 
   
   
    $("#fullName").text(parsedJSON.data[0].personal.name + " " + parsedJSON.data[0].personal.surname)
    
   
}
else {


    $('#systemModalTitle').text("Sessiyanız başa çatıb");
    $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
    $('#systemModalBtn').removeAttr("hidden");
    $('#systemModal').modal('show')
    //window.location.replace("/");

}


function Routing(obj, link) {
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
    $("#warningModalButton").show();
    $("#warningModalButton").text("Bəli");
    $("#warningModalButton").on("click", function () {

        $.post("/login/logout", function (data) {

        });
        $("#warningModalButton").hide();
        localStorage.clear();
        window.location.replace("/login");

    });
}