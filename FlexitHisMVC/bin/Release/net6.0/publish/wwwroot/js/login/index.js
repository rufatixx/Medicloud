//$(document).ready(function () {
//    if (localStorage.json) {

//        //alert(localStorage.json);
//        window.location.replace("app");

//    }

//});


function signIn(btn) {

    btn.disabled = true
    btn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true" id="signInSpinner" ></span> Yüklənir...'
    //btn.innerText = "Yüklənir..."

    var username = $("#username").val()
    if (username.length > 0) {


        var pass = $("#passBox").val()
        if (pass !== null && pass !== '') {

            $("#alert").attr("hidden", "");
            $.ajax({
                type: 'POST',
                url: '/Login/SignIn',
                data: { username: username, pass: pass },
                dataType: 'json',
                success: function (data, status, xhr) {   // success callback function
                    //  var json = JSON.stringify(data)
                    //alert(data.name)
                    btn.disabled = false
                    btn.innerText = 'Daxil ol'
                    $("#alert").removeAttr("hidden");

                    if (xhr.status == 200) {
                        $("#alert").attr("hidden", "");
                        if (typeof (Storage) !== "undefined") {
                            // Code for localStorage/sessionStorage.

                            localStorage.json = JSON.stringify(data);
                            //localStorage.userToken = data.userToken
                            //localStorage.requestToken = data.requestToken
                        } else {
                            $("#alert").removeAttr("hidden");
                            $('#alert').text('Brauzerinizi yeniləyin, dəstəklənmir');
                            // Sorry! No Web Storage support..
                        }

                        //alert(JSON.parse(json).name)
                        //document.cookie = "jsonData="+data;
                        window.location.replace("/");
                    }
                    else {
                        $('#alert').text('İstifadəçi mövcud deyil');
                    }
                    //switch (data.status) {
                    //    case 1:
                         
                    //        //alert(getCookie("jsonData"))
                    //        break;
                    //    case 2:
                          
                    //        break;
                    //    case 3:
                    //        $('#alert').text('İstifadəçi deaktiv edilib xahiş olunur administratorla əlaqə saxlayasınız!');
                    //        break;
                    //    default:
                    //        $('#alert').text('Xəta, biraz sonra yenidən cəht edin...');
                    //        break;

                    //}

                    //$('p').append(data.name + ' ' + data.surname);
                },
                error: function (jqXhr, textStatus, errorMessage) { // error callback
                    btn.disabled = false
                    btn.innerText = 'Daxil ol'
                    $("#alert").removeAttr("hidden");
                    if (jqXhr.status == 401) {
                        $('#alert').text('İstifadəçi mövcud deyil');
                    }
                    else {
                        $('#alert').text('Xəta, internetə bağlı olduğunuzdan əmin olun');
                    }
                    
                    //  $('#alert').text('Error: ' + errorMessage);
                }
            });


        } else {
            btn.disabled = false
            btn.innerText = 'Daxil ol'
            $("#alert").removeAttr("hidden");

            $('#alert').text('Xahiş olunur ŞİFRƏNİZİN düzgünlüyünə diqqət edəsiniz!');
        }


    }
    else {
        btn.disabled = false
        btn.innerText = 'Daxil ol'
        $("#alert").removeAttr("hidden");

        $('#alert').text('Xahiş olunur NÖMRƏNİZİN düzgünlüyünə diqqət edəsiniz!');
    }




}
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
