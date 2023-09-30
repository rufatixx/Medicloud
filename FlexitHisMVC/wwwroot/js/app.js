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


if (localStorage.json) {
    var json = localStorage.json;
    var parsedJSON = JSON.parse(json);

    if (parsedJSON.data[0].personal.isAdmin) {
        $("#adminButton").show();
        $(".userType").text("Admin");

    } else {
        $("#adminButton").hide();
        $(".userType").text("İstifadəçi");
    }


    $.each(parsedJSON.data[0].hospitals, function () {
        const { id, hospitalID, hospitalName } = this;
        const onClickHandler = `localStorage.selectedHospital='${hospitalID}'; localStorage.selectedHospitalName='${hospitalName}'; $('#selectedHospitalName').text('${hospitalName}'); $('#hospitalLogo').text('${hospitalName}'); hospitalChanged();`;

        const dropdownItem = $(`
    <a class="dropdown-item" id="${id}" onclick="${onClickHandler}">
      ${hospitalName}
    </a>
  `);

        $("#selectHospitalDropdownItems").append(dropdownItem);
    });

    if (localStorage.selectedHospital != null) {

        $("#selectedHospitalName").text(localStorage.selectedHospitalName)
        $('#hospitalLogo').text(localStorage.selectedHospitalName)



    }
    else {


        if (parsedJSON.data[0].hospitals.length > 0) {

            $("#selectedHospitalName").text(parsedJSON.data[0].hospitals[0].hospitalName)
            $('#hospitalLogo').text(parsedJSON.data[0].hospitals[0].hospitalName)
            localStorage.selectedHospital = parsedJSON.data[0].hospitals[0].hospitalID;
            localStorage.selectedHospitalName = parsedJSON.data[0].hospitals[0].hospitalName;

        }
        else {

            $("#newPatientButton").hide();
            $("#selectedHospitalName").hide();
            $('#hospitalLogo').hide()
            $("#kassaButton").hide();
            $("#policlinicButton").hide();

            if (!parsedJSON.data[0].personal.isAdmin) {
                $("#hospitalSelector").text("Heç bir xəstəxanaya icazəniz yoxdur, zəhmət olmasa çağrı mərkəzimizə müraciət edin");
            }
            //$('#warningModal').modal('show')

            //$('#hospitalSelector').text('Heç bir xəstəxanaya icazəniz yoxdur, zəhmət olmasa çağrı mərkəzimizə müraciət edin');
            //$("#warningModalButton").show();
            //$("#warningModalButton").text("Çıxış");
            //$("#warningModalButton").on("click", function () {

            //    $.post("/login/logout", function (data) {

            //    });
            //    $("#warningModalButton").hide();
            //    localStorage.clear();
            //    window.location.replace("/login");

            //});
        }

    }


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
function servicesChanged(a) {
    //alert($(a).val())
    $("#price").val($(a).val());
}



function showLoading() {
    $('#loadingModal').show();

    $('#loadingModalText').html(`<center><div class="spinner-grow text-light" style="width: 7rem; height: 7rem;" role="status">
   
  </div></center>`);

}
function hideLoading() {
    $('#loadingModal').hide();
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




    $(".fullName").text(parsedJSON.data[0].personal.name + " " + parsedJSON.data[0].personal.surname)


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

        $.ajax({
            url: "/login/logout",
            method: "POST",
            success: function (data) {
                // Request completed successfully
                $("#warningModalButton").hide();
                localStorage.clear();
                window.location.replace("/login");
            },
            error: function (xhr, status, error) {
                // Request failed
                console.error(error);
            },
            complete: function () {
                // Request complete (success or error)
                // This code will be executed after the request is finished
            }
        });

    });
}