
var json = localStorage.getItem("json")
var pageData;
var parsedJSON = JSON.parse(json)

var foundPatients;

//var today = new Date().toISOString().split('T')[0];
////$("#bdate")[0].setAttribute('max', today);
//$(document).ready(function () {
//    document.getElementById("datefield").setAttribute("max", today);

//});


// $('#systemModal').modal('show');
// $('#systemModalTitle').text("Yüklənir...");
// $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//   <span class="sr-only">Loading...</span>
// </div></center>`);

//$('#systemModalTitle').text("Yüklənir...");
//$('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
//    <span class="sr-only">Loading...</span>
//  </div></center>`);
//$('#systemModalBtn').attr("hidden", "");
var $dropdown = $("#requestType");
var $servicesDropdown = $("#services");
var $policlinicDropdown = $("#departments");
var $doctorDropdown = $("#doctors");
var $refererDropdown = $("#referer");
showLoading();

$.ajax({
    type: 'POST',
    url: `/newPatient/getPageModel`,
    data: { organizationID: parseInt(localStorage.selectedOrganization) },
    dataType: 'json',
    success: function (data, status, xhr) {
        if (typeof (Storage) !== "undefined") {
            localStorage.requestToken = data.requestToken;
        } else {
            // Sorry! No Web Storage support..
        }

        pageData = data;

        $dropdown.empty(); // Clear existing options
        $.each(data.requestTypes, function () {
            $dropdown.append($("<option />").val(this.id).text(this.name));
        });

        $servicesDropdown.empty(); // Clear existing options
        if ($dropdown.val() == 1) {
            $.each(data.services, function () {
                if (this.serviceTypeID === 1) {
                    $servicesDropdown.append($(`<option id=${this.id}/>`).val(this.price).text(this.name));
                }
            });
        }

        $doctorDropdown.empty(); // Clear existing options
        $.each(data.personal, function () {
            $doctorDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} (${this.speciality})`));
        });

        $refererDropdown.empty(); // Clear existing options
        $.each(data.referers, function () {
            $refererDropdown.append($("<option />").val(this.id).text(`${this.name} ${this.surname} ${this.father}`));
        });

        $("#price").val($("#services").val());
        hideLoading();
    },
    error: function (jqXhr, textStatus, errorMessage) {
        if (jqXhr.status == "401") {
            localStorage.clear();
            $('#systemModalTitle').text("Sessiyanız başa çatıb");
            $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
            $('#systemModalBtn').removeAttr("hidden");
        } else {
            $('#warningModal').show();
            $('#warningText').text(jqXhr.status);
        }
    }
});


function serachForPatient() {

    showLoading();
    $('#systemModalTitle').text("Yüklənir...");
    $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
    $('#systemModalBtn').attr("hidden", "");
    $.ajax({
        type: 'POST',
        url: `/NewPatient/SearchForPatient`,
        data: {fullNamePattern: $("#fullNamePattern").val() ,organizationID:parseInt(localStorage.selectedOrganization) },
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
           

            hideLoading()



            //$('p').append(data.name + ' ' + data.surname);
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback
            hideLoading()

            if (jqXhr.status == "401") {
                localStorage.clear()
                $('#systemModalTitle').text("Sessiyanız başa çatıb");
                $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                $('#systemModalBtn').removeAttr("hidden");
            }
            else {
                $('#warningModal').show()
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

        showLoading()

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
                        "organizationID": parseInt(localStorage.selectedOrganization),
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
                            hideLoading()
                            $('#warningModal').modal("show")
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
                            hideLoading
                            $('#warningModal').modal("show")
                            $('#warningText').text('Xəstə artıq mövcuddur, zəhmət olmasa axtarış bölməsindən istifadə edin');
                            break;
                      
                        default:
                            hideLoading()
                            $('#warningModal').modal("show")
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
                    hideLoading()
                    $('#warningModal').show()
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
