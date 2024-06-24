//$("#appTitle").text("HiS - Menu");
//$("#appBrand").html(` <svg xmlns="http://www.w3.org/2000/svg" width="130" height="56" viewBox="0 0 130 56" fill="none">
//                <rect width="56" height="56" rx="8" fill="url(#paint0_linear_537_6491)" />
//                <path d="M50.5671 14.209C43.2331 17.0635 43.2331 24.7877 43.2331 24.7877V31.3492C43.2331 34.1616 42.1714 36.6307 40.0796 38.6781C38.092 40.6083 35.6076 41.5821 32.6777 41.5821C30.3995 41.5821 28.3606 40.8881 26.5975 39.519C24.7756 40.8881 22.7558 41.5821 20.5852 41.5821C17.6925 41.5821 15.209 40.6133 13.2073 38.7045C11.0939 36.667 10.0298 34.1954 10.0298 31.3492V18.1157C10.0297 17.7525 10.1332 17.3967 10.3282 17.0899C10.5232 16.7831 10.8017 16.5378 11.1312 16.3827L12.6417 15.6672C14.645 14.7243 16.8327 14.2345 19.0482 14.2329V31.3492C19.0482 31.9722 19.1608 32.4566 19.3422 32.611C19.5633 32.7999 19.902 33.0145 20.5844 33.0145C21.2668 33.0145 21.6063 32.7999 21.8266 32.611C22.008 32.4566 22.1206 31.9722 22.1206 31.3492V18.1157C22.1205 17.7525 22.224 17.3967 22.419 17.0899C22.614 16.7831 22.8925 16.5378 23.222 16.3827L24.7325 15.6672C26.7358 14.7243 28.9235 14.2345 31.139 14.2329V31.3492C31.139 31.9722 31.2516 32.4566 31.433 32.611C31.6541 32.7999 31.9928 33.0145 32.6752 33.0145C33.3576 33.0145 33.6971 32.7999 33.9174 32.611C34.1013 32.4566 34.2114 31.9722 34.2114 31.3492V18.1157C34.2116 17.7522 34.3155 17.3964 34.511 17.0895C34.7064 16.7827 34.9854 16.5375 35.3153 16.3827L36.8258 15.6672C38.8291 14.7243 41.0168 14.2345 43.2323 14.2329L50.5671 14.209Z" fill="white" />
//                <path d="M76.1056 40V14.6555H80.2821V25.1503H92.8115V14.6555H96.988V40H92.8115V29.1126H80.2821V40H76.1056ZM102.623 40V22.1518H106.55V40H102.623ZM119.822 40.4284C117.109 40.4284 114.908 39.6787 113.218 38.1795C111.552 36.6564 110.672 34.8359 110.577 32.7179H114.753C114.896 33.9316 115.455 34.8716 116.431 35.5379C117.43 36.2043 118.549 36.5374 119.786 36.5374C121.024 36.5374 122.035 36.2519 122.821 35.6807C123.63 35.0858 124.034 34.3005 124.034 33.3248C124.034 31.6113 122.868 30.3739 120.536 29.6123L117.43 28.5771C113.29 27.2683 111.219 24.829 111.219 21.2594C111.219 19.0938 111.957 17.3803 113.432 16.1191C114.932 14.8578 116.883 14.2272 119.287 14.2272C121.785 14.2272 123.772 14.953 125.248 16.4046C126.723 17.8325 127.544 19.5221 127.711 21.4735H123.534C123.273 20.355 122.737 19.5221 121.928 18.9748C121.119 18.4036 120.215 18.1181 119.215 18.1181C118.121 18.1181 117.216 18.3917 116.502 18.9391C115.788 19.4626 115.419 20.1647 115.396 21.0452C115.396 21.9495 115.669 22.6872 116.217 23.2584C116.764 23.8057 117.633 24.2698 118.823 24.6505L121.964 25.65C126.152 27.0065 128.246 29.5171 128.246 33.182C128.246 35.3238 127.461 37.0729 125.89 38.4294C124.344 39.762 122.321 40.4284 119.822 40.4284Z" fill="#4F4F4F" />
//                <path d="M105.514 10.8893C106.841 10.5337 108.055 11.7481 107.7 13.0752L106.9 16.0613C106.544 17.3884 104.885 17.8329 103.914 16.8614L101.728 14.6755C100.756 13.7039 101.201 12.045 102.528 11.6894L105.514 10.8893Z" fill="#2AAEAD" />
//                <rect x="64" y="6" width="1" height="46" fill="#BDBDBD" />
//                <defs>
//                    <linearGradient id="paint0_linear_537_6491" x1="0" y1="0" x2="57.5583" y2="1.65028" gradientUnits="userSpaceOnUse">
//                        <stop stop-color="#2AAEAD" />
//                        <stop offset="1" stop-color="#75D1B2" />
//                    </linearGradient>
//                </defs>
//            </svg>`);
var json = localStorage.getItem("json")
var parsedJSON = JSON.parse(json)
// $('#systemModal').modal('show');
// $('#systemModalTitle').text("Yüklənir...");
// $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//   <span class="sr-only">Loading...</span>
// </div></center>`);
// $('#systemModalBtn').attr("hidden","");
//$('#systemModal').modal('show');
//$('#systemModalTitle').text("Yüklənir...");
//$('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//    <span class="sr-only">Loading...</span>
//  </div></center>`);
//$('#systemModalBtn').attr("hidden", "");


 
//if (localStorage.json) {
//    var json = localStorage.json;
//    var parsedJSON = JSON.parse(json);

//    $("#fullName").text(parsedJSON.data[0].personal.name + " " + parsedJSON.data[0].personal.surname)

//    if (localStorage.selectedOrganization != null) {

//        if (json.data[0].organizations.length > 0) {
//            $("#selectOrganizationDropdownButton").text(json.data[0].organizations[0].organizationName)
//            $('#organizationLogo').text(parsedJSON.data[0].organizations[0].organizationName)
//            localStorage.selectedOrganization = parsedJSON.data[0].organizations[0].id;
//            localStorage.selectedOrganizationName = parsedJSON.data[0].organizations[0].organizationName;
//            $.each(parsedJSON.data[0].organizations, function () {
//                $("#selectOrganizationDropdownItems").append(`<a class="dropdown-item" id="${this.id}" onclick="localStorage.selectedOrganization='${this.id}';localStorage.selectedOrganizationName='${this.organizationName}'; $('#selectOrganizationDropdownButton').text('${this.organizationName} '); $('#organizationLogo').text('${this.organizationName} ');">${this.organizationName}</a>`)

//            });
//        }
//        else {
//            $('#systemModalTitle').text("Sizin heç bir xəstəxanaya icazəniz yoxdur");
//            $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa texniki dəstək xidmətimizə müraciət edin</p>`);
//            $('#systemModalBtn').removeAttr("hidden");
//            $('#systemModal').modal('show')
//        }


//    }
//    else {
//        $("#selectOrganizationDropdownButton").text(localStorage.selectedOrganizationName)
//        $('#organizationLogo').text(localStorage.selectedOrganizationName)
//    }


//    //if (localStorage.lastActivePage) {


//    //    $("#main-content").load(localStorage.lastActivePage)

//    //}
//    //else {
//    //    $("#main-content").load("/menu")
//    //}
//}
//else {

//    $('#systemModalTitle').text("Sessiyanız başa çatıb");
//    $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
//    $('#systemModalBtn').removeAttr("hidden");
//    $('#systemModal').modal('show')
//    //window.location.replace("/");

//}

// end date picker settings
//function newOrderModalClick(){
//  $('#systemModal').modal('show');
//  $('#systemModalTitle').text("Yüklənir...");
//  $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//    <span class="sr-only">Loading...</span>
//  </div></center>`);
//  $('#systemModalBtn').attr("hidden","");


//  $.ajax({
//    type: 'POST',
//      url: "http://127.0.0.1:5000/api/ds/get/models",
//    data: {userToken:localStorage.getItem("userToken"), requestToken:localStorage.getItem("requestToken")},
//    dataType: 'json',
//    success: function (data,status,xhr) {   // success callback function
//    //  var json = JSON.stringify(data)
//    //alert(data.name)



//  switch (data.status) {
//  case 1:
//$('#systemModal').modal('hide');
//  $('#newOrderModal').modal('show')

//  if (typeof(Storage) !== "undefined") {

//  localStorage.requestToken = data.requestToken
//  } else {

//  // Sorry! No Web Storage support..
//  }

//  //alert(JSON.parse(json).name)
//  //document.cookie = "jsonData="+data;
//  $("#productModels").html('');
//  data.data.forEach(element =>{

//  var o = new Option(element.name, element.id);
//  $(o).html(element.name);
//  $("#productModels").append(o);
//  }
//  );



//  //alert(getCookie("jsonData"))
//  break;
//      default:
//          localStorage.clear()
//  $('#systemModalTitle').text("Sessiyanız başa çatıb");
//  $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
//  $('#systemModalBtn').removeAttr("hidden");
//  //window.location.replace("file:///Users/rufat/Desktop/Dekor%20Stone/login/dekor_stone.html");
//  break;

//  }

//        //$('p').append(data.name + ' ' + data.surname);
//    },
//    error: function (jqXhr, textStatus, errorMessage) { // error callback

//      $('#warningModal').modal('show')
//      $('#warningText').text('Xəta, internetə bağlı olduğunuzdan əmin olun');
//      //  $('#alert').text('Error: ' + errorMessage);
//    }
//  });
//}


function getCookie(cname) {
  var name = cname + "=";
  var decodedCookie = decodeURIComponent(document.cookie);
  var ca = decodedCookie.split(';');
  for(var i = 0; i <ca.length; i++) {
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
