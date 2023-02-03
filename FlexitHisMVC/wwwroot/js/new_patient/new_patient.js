
var json = localStorage.getItem("json")
var pageData;
var parsedJSON = JSON.parse(json)

var foundPatients;

//var today = new Date().toISOString().split('T')[0];
////$("#bdate")[0].setAttribute('max', today);
//$(document).ready(function () {
//    document.getElementById("datefield").setAttribute("max", today);

//});

var $dropdown = $("#requestType");
var $servicesDropdown = $("#services");
var $policlinicDropdown = $("#departments");
var $doctorDropdown = $("#doctors");
var $refererDropdown = $("#referer");
// $('#systemModal').modal('show');
// $('#systemModalTitle').text("Yüklənir...");
// $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//   <span class="sr-only">Loading...</span>
// </div></center>`);
// $('#systemModalBtn').attr("hidden","");
$('#systemModal').modal('show');
$('#systemModalTitle').text("Yüklənir...");
$('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
$('#systemModalBtn').attr("hidden", "");

$.ajax({
    type: 'POST',
    url: `/newPatient/getPageModel`,
    data: { hospitalID: parseInt(localStorage.selectedHospital) },
    dataType: 'json',
    success: function (data, status, xhr) {   // success callback function
        //  var json = JSON.stringify(data)

        if (typeof (Storage) !== "undefined") {

            localStorage.requestToken = data.requestToken
        } else {

            // Sorry! No Web Storage support..
        }
       
        pageData = data;
        //alert(data.requestTypes[0].name)
        $.each(data.requestTypes, function () {
            $dropdown.append($("<option />").val(this.id).text(this.name));
        });
        $.each(data.services, function () {
            //if (this.depID == data.departments[0].id) {
            //    $servicesDropdown.append($(`<option id=${this.id} />`).val(this.price).text(this.name));
            //}
            $servicesDropdown.append($(`<option id=${this.id} />`).val(this.price).text(this.name));
        });
        //$.each(data.departments, function () {
        //    $policlinicDropdown.append($("<option />").val(this.id).text(this.name));
        //});
        $.each(data.personal, function () {
            //if (this.depID == data.departments[0].id) {
            //    $doctorDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} ${this.father}`));
            //}
            $doctorDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} (${this.speciality})`));
        });
        $.each(data.referers, function () {
            $refererDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} ${this.father}`));
        });
        $("#price").val($("#services").val());
        $('#systemModal').modal('hide');

       

        //$('p').append(data.name + ' ' + data.surname);
    },
    error: function (jqXhr, textStatus, errorMessage) { // error callback


        if (jqXhr.status == "401") {
            localStorage.clear()
            $('#systemModalTitle').text("Sessiyanız başa çatıb");
            $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
            $('#systemModalBtn').removeAttr("hidden");
        }
        else {
            $('#warningModal').modal('show')
            $('#warningText').text(jqXhr.status);
        }
        //  $('#alert').text('Error: ' + errorMessage);
    }

});


function serachForPatient() {

    $('#systemModal').modal('show');
    $('#systemModalTitle').text("Yüklənir...");
    $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
    $('#systemModalBtn').attr("hidden", "");
    $.ajax({
        type: 'POST',
        url: `/NewPatient/SearchForPatient`,
        data: {fullNamePattern: $("#fullNamePattern").val() ,hospitalID:parseInt(localStorage.selectedHospital) },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)

            if (typeof (Storage) !== "undefined") {

                localStorage.requestToken = data.requestToken
            } else {

                // Sorry! No Web Storage support..
            }

            $("#foundPatients").empty();
            foundPatients = data;
            //alert(data.requestTypes[0].name)
            $.each(data, function () {
                var bDate = this.bDate.split('T')[0];
                $("#foundPatients").append($("<option />").val(this.id).text(`${this.name} ${this.surname} ${this.father} (${bDate})`));
            });
           
            
            $('#systemModal').modal('hide');



            //$('p').append(data.name + ' ' + data.surname);
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback


            if (jqXhr.status == "401") {
                localStorage.clear()
                $('#systemModalTitle').text("Sessiyanız başa çatıb");
                $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                $('#systemModalBtn').removeAttr("hidden");
            }
            else {
                $('#warningModal').modal('show')
                $('#warningText').text(jqXhr.status);
            }
            //  $('#alert').text('Error: ' + errorMessage);
        }

    });

}
function DeleteSelectedPatient() {

    $("#name").prop("disabled", false);
    $("#surname").prop("disabled", false);
    $("#father").prop("disabled", false);
    $("#clientPhone").prop("disabled", false);
    $("#fin").prop("disabled", false);
    $("#bDate").prop("disabled", false);
    $(`#gender`).prop("disabled", false);

    $("#name").val("");
    $("#surname").val("");
    $("#father").val("");
  
    $("#clientPhone").val("");
    $("#fin").val("");
    $("#bDate").val("");

    var foundPatientID = 0;
    $("#selectedPatientForm").hide();
}
function foundPatientClicked(item) {

    $("#selectedPatientForm").show()
    $("#name").prop("disabled", true);
    $("#surname").prop("disabled", true);
    $("#father").prop("disabled", true);

    $("#clientPhone").prop("disabled", true);
    $("#fin").prop("disabled", true);



    $("#bDate").prop("disabled", true);
    $(`#gender`).prop("disabled", true);

  
    $.each(foundPatients, function () {
        if (this.id == $(item).val()) {
           
            $("#name").val(this.name);
            $("#surname").val(this.surname);
            $("#father").val(this.father);
            var lastNine = this.phone.toString().slice(-9);
            $("#clientPhone").val(lastNine);
            $("#fin").val(this.fin);

            var today = new Date(this.bDate).toISOString().split('T')[0];

            $("#bDate").val(today);
            $(`#gender option[value="${this.genderID}"]`).attr("selected", "selected");
            //$("#gender").prop("selectedIndex", this.genderID);

        }
    });
    $('#patientSearch').modal('hide');

}
function AddPatient() {


    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.getElementsByClassName('needs-validation');
    // Loop over them and prevent submission
    var validation = Array.prototype.filter.call(forms, function (form) {
       

        if (form.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
            //alert('sucess');
        }
        //else {
        //    form.reportValidity()
        //    //alert('asas');
        //}
        form.classList.add('was-validated');

    
    });
   
    if (forms[0].checkValidity()) {

        var foundPatientID = $("#foundPatients").val() || 0
        var name = $('#name').val()
        var surname = $('#surname').val()
        var father = $('#father').val()
        var phone = $('#clientPhone').val()
        var genderID = $('#gender').val()
        var bDate = $('#bDate').val()
        var fin = $("#fin").val();
        var requestTypeID = $("#requestType").val();
        var priceGroupID = $("#priceGroup").val();
        var serviceID = $("#services").children(":selected").attr("id");
        var depID = $("#departments").val();
        var depDocID = $("#doctors").val();
        var referDocID = $("#referer").val();
       
        var note = $("#note").val();

        var date = new Date(bDate);
            var isoBdate = date.toISOString()
           
            $('#systemModal').modal('show');
            $('#systemModalTitle').text("Yüklənir...");
            $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
        $('#systemModalBtn').attr("hidden", "");

        $.ajax({
            headers: {
                "Content-Type": "application/json"
            },
                type: 'POST',
            url: `/NewPatient/AddPatient`,
                data: 

                    JSON.stringify({
                      
                        "name": name,
                        "surname": surname,
                        "father": father,
                        "clientPhone": parseInt("00994" + phone),
                        "genderID": parseInt(genderID),
                        "fin": fin,
                        "requestTypeID": parseInt(requestTypeID),
                        "priceGroupID": parseInt(priceGroupID),
                        "hospitalID": parseInt(localStorage.selectedHospital),
                        "serviceID": parseInt(serviceID),
                        "foundPatientID": parseInt(foundPatientID),
                        //"depID": parseInt(depID),
                        "docID": parseInt(depDocID),
                        "referDocID": parseInt(referDocID),
                        "birthDate": bDate,
                        "note": note

                       
                    }),

                

               
          
                success: function (data, status, xhr) {   // success callback function
                    //  var json = JSON.stringify(data)
                    //alert(data.name)



                    switch (data.status) {
                        case 1:
                            $('#systemModal').modal('hide')
                            $('#warningModal').modal('show')
                            $('#warningText').text('Məlumatlar qeydə alındı');
                            //var newKassaSum = parseFloat(kassaSum) + parseFloat(payment)
                            //kassaSum = newKassaSum
                            //$("#kassaSumText").text("Cəmi: " + kassaSum + " AZN");

                            if (typeof (Storage) !== "undefined") {
                                localStorage.requestToken = data.requestToken
                            } else {

                                // Sorry! No Web Storage support..
                            }

                            //alert(JSON.parse(json).name)
                            //document.cookie = "jsonData="+data;

                            //alert(getCookie("jsonData"))
                            break;
                        case 2:
                            if (typeof (Storage) !== "undefined") {
                                localStorage.requestToken = data.requestToken
                            } else {

                                // Sorry! No Web Storage support..
                            }
                            $('#systemModal').modal('hide');
                            $('#warningModal').modal('show')
                            $('#warningText').text('Xəstə artıq mövcuddur, zəhmət olmasa axtarış bölməsindən istifadə edin');
                            break;
                      
                        default:
                            $('#systemModal').modal('hide');
                            $('#warningModal').modal('show')
                            $('#warningText').text('Xəta, biraz sonra yenidən cəhd edin');
                            break;

                    }

                    //$('p').append(data.name + ' ' + data.surname);
                },
            error: function (jqXhr, textStatus, errorMessage) {

                //alert(jqXhr.status)// error callback

                if (jqXhr.status == 401) {
                    localStorage.clear()
                    $('#systemModalTitle').text("Sessiyanız başa çatıb");
                    $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                    $('#systemModalBtn').removeAttr("hidden");
                }
                else {
                    $('#systemModal').modal('hide');
                    $('#warningModal').modal('show')
                    $('#warningText').text('Xəta, biraz sonra yenidən cəhd edin');
                    //  $('#alert').text('Error: ' + errorMessage);
                }
                }
                
            });
            // alert(`
            // cfn: `+clientFullName+`
            // cp: `+clientPhone+`
            // kvm: `+kvm+`
            // price: `+price+`
            // payment: `+payment+`
            // note: `+note+`
            // productModel: `+productModel+`
            // productEndDate: `+isoEndDate+`
            //
            //   `)
        
    }
    
   


}

function servicesChanged(a) {
    //alert($(a).val())
    $("#price").val($(a).val());
}

function depChanged(dep) {
  
    $servicesDropdown.empty();
    $doctorDropdown.empty();
    $.each(pageData.services, function () {
       
        if (this.depID == $(dep).val()) {
            $servicesDropdown.append($(`<option id=${this.id} />`).val(this.price).text(this.name));
        }

    });
  
    $.each(pageData.personal, function () {
        if (this.depID == $(dep).val()) {
            $doctorDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} ${this.father}`));
        }

    });
    $("#price").val($("#services").val());
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
