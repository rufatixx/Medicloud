
document.addEventListener('DOMContentLoaded', function () {
    if (localStorage.length > 0) {

        var json = localStorage.getItem("json")

        var parsedJSON = JSON.parse(json)


        if (!localStorage.selectedKassaName) {
            //alert(parsedJSON.data[0].kassaList[0].name)
            $("#selectedKassaButton").text(parsedJSON.data[0].kassaList[0].name);
            localStorage.selectedKassaID = parsedJSON.data[0].kassaList[0].id
            localStorage.selectedKassaName = parsedJSON.data[0].kassaList[0].name
        }
        else {
            $("#selectedKassaButton").text(localStorage.selectedKassaName)

        }


        $.each(parsedJSON.data[0].kassaList, function () {


            $("#selectedKassaDropdown").append(`<a class="dropdown-item" id="${this.id}" onclick="localStorage.selectedKassaID='${this.id}';localStorage.selectedKassaName='${this.name}'; $('#selectedKassaButton').text('${this.name} ');">${this.name}</a>`)


        });
    }
    else {
        localStorage.clear();

        $('#systemModalTitle').text("Sessiyanız başa çatıb");
        $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
        $('#systemModalBtn').removeAttr("hidden");
        //$('#systemModal').modal('show')
        //window.location.replace("file:///Users/rufat/Desktop/Dekor%20Stone/login/dekor_stone.html");
    }


  

}, false);
function debtorPatientChanged(a) {
    //alert($(a).val())
    $("#debtorPrice").val($(a).val());
    //localStorage.selectedDebtorPatientID = data.data[0].id


}
function getPaymentTypes() {
    $.ajax({
        type: 'POST',
        url: `/Kassa/GetPaymentTypes`,
        data: {hospitalID: localStorage.selectedHospital },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            //alert(data.name)



    
                    //alert(JSON.parse(json).name)
                    //document.cookie = "jsonData="+data;
                    $.each(data, function () {

                        $("#pType").append($(`<option  />`).val(this.id).text(`${this.name}`));


                    });
                    localStorage.selectedPaymentTypeID = data.data[0].id;

            

            //$('p').append(data.name + ' ' + data.surname);
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback

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
}

function incomeClicked() {
    $('#systemModal').modal('show');
    $('#systemModalTitle').text("Yüklənir...");
    $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
    $('#systemModalBtn').attr("hidden", "");


    $.ajax({
        type: 'POST',
        url: `/Kassa/GetDebtorPatients`,
        data: { hospitalID: localStorage.selectedHospital },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            //alert(data.name)



            if (data.length > 0) {

                $('#systemModal').modal('hide');
                $('#newPayment').modal('show')

                if (typeof (Storage) !== "undefined") {

                    localStorage.requestToken = data.requestToken
                } else {

                    // Sorry! No Web Storage support..
                }

                //alert(JSON.parse(json).name)
                //document.cookie = "jsonData="+data;
                $("#debtorPatients").empty();
                $.each(data, function () {

                    $("#debtorPatients").append($(`<option id='${this.id}'/>`).val(this.price).text(`${this.name} ${this.surname} ${this.father} `));


                });
                localStorage.selectedDebtorPatientID = data[0].id
                $("#debtorPrice").val(data[0].price);
                getPaymentTypes();
                //alert(getCookie("jsonData"))
            }
            else {
                $('#systemModal').modal('hide');
                $('#warningModal').modal('show')
                $('#warningText').text('Ödəniş gözləyən xəstə yoxdur');
                //window.location.replace("file:///Users/rufat/Desktop/Dekor%20Stone/login/dekor_stone.html");


            }



            //$('p').append(data.name + ' ' + data.surname);
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback

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
}


function insertPayment() {

    $('#newPayment').modal('hide');
    $('#systemModal').modal('show');
    $('#systemModalTitle').text("Yüklənir...");
    $('#systemModalText').html(`<center><div class="spinner-border text-dark mx-auto" role="status">
    <span class="sr-only">Loading...</span>
  </div></center>`);
    $('#systemModalBtn').attr("hidden", "");
    $.ajax({
        type: 'POST',
        url: `/Kassa/AddIncome`,
        data: {
            kassaID: localStorage.selectedKassaID,
            payment_typeID: $('#pType').val(), patientID: $('#debtorPatients option:selected').attr('id')
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            //alert(data.name)

  
                if (data) {
                    $('#systemModal').modal('hide')
                    $('#warningModal').modal('show')
                    $('#warningText').text('Odəniş əlavə olundu');

                    //var newKassaSum = parseFloat(kassaSum) + parseFloat(payment)
                    //kassaSum = newKassaSum
                    //$("#kassaSumText").text("Cəmi: " + kassaSum + " AZN");

                    if (typeof (Storage) !== "undefined") {
                        localStorage.requestToken = data.requestToken
                    } else {

                        // Sorry! No Web Storage support..
                    }

                }
                else {
                    $('#systemModal').modal('hide');
                    $('#warningModal').modal('show')
                    $('#warningText').text('Xəta, biraz sonra yenidən cəhd edin');

                }
            



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