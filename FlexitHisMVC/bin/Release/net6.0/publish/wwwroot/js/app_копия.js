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

console.log("HERE!");
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


    $.each(parsedJSON.data[0].organizations, function () {
        const { id, organizationID, organizationName } = this;
        const onClickHandler = `localStorage.selectedOrganization='${organizationID}'; localStorage.selectedOrganizationName='${organizationName}'; $('#selectedOrganizationName').text('${organizationName}'); $('#organizationLogo').text('${organizationName}'); organizationChanged();`;

        const dropdownItem = $(`
    <a class="dropdown-item" id="${id}" onclick="${onClickHandler}">
      ${organizationName}
    </a>
  `);

        $("#selectOrganizationDropdownItems").append(dropdownItem);
    });

    if (localStorage.selectedOrganization != null) {

        $("#selectedOrganizationName").text(localStorage.selectedOrganizationName)
        $('#organizationLogo').text(localStorage.selectedOrganizationName)



    }
    else {


        if (parsedJSON.data[0].organizations.length > 0) {

            $("#selectedOrganizationName").text(parsedJSON.data[0].organizations[0].organizationName)
            $('#organizationLogo').text(parsedJSON.data[0].organizations[0].organizationName)
            localStorage.selectedOrganization = parsedJSON.data[0].organizations[0].organizationID;
            localStorage.selectedOrganizationName = parsedJSON.data[0].organizations[0].organizationName;

        }
        else {

            $("#newPatientButton").hide();
            $("#selectedOrganizationName").hide();
            $('#organizationLogo').hide()
            $("#kassaButton").hide();
            $("#policlinicButton").hide();

            if (!parsedJSON.data[0].personal.isAdmin) {
                $("#organizationSelector").text("Heç bir müəssisəyə icazəniz yoxdur, zəhmət olmasa çağrı mərkəzimizə müraciət edin");
            }
            //$('#warningModal').modal('show')

            //$('#organizationSelector').text('Heç bir xəstəxanaya icazəniz yoxdur, zəhmət olmasa çağrı mərkəzimizə müraciət edin');
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


//    if (!localStorage.selectedOrganization) {
//        if (parsedJSON.data[0].organizations.length > 0) {
//            $("#fullName").text(`${parsedJSON.data[0].personal.name} ${parsedJSON.data[0].personal.surname}`)
//            $("#selectOrganizationDropdownButton").text(parsedJSON.data[0].organizations[0].organizationName)
//            $('#organizationLogo').text(parsedJSON.data[0].organizations[0].organizationName)
//            localStorage.selectedOrganization = parsedJSON.data[0].organizations[0].organizationID;
//            localStorage.selectedOrganizationName = parsedJSON.data[0].organizations[0].organizationName;
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
//        $("#selectOrganizationDropdownButton").text(localStorage.selectedOrganizationName)
//        $('#organizationLogo').text(localStorage.selectedOrganizationName)
//    }
//    $.each(parsedJSON.data[0].organizations, function () {
//        $("#selectOrganizationDropdownItems").append(`<a class="dropdown-item" id="${this.id}" onclick="localStorage.selectedOrganization='${this.id}';localStorage.selectedOrganizationName='${this.organizationName}'; $('#selectOrganizationDropdownButton').text('${this.organizationName} '); $('#organizationLogo').text('${this.organizationName} ');">${this.organizationName}</a>`)

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