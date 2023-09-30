
$('#accessSettings').hide();
$('#generalSettings').show()

showLoading();

$("#depIsActive").prop("disabled", true);
$("#isRandevuActive").prop("disabled", true);
$('#docRequired').attr('disabled', true);
$('#gender').attr('disabled', true);
$('#depType').attr('disabled', true);
$("#depBuilding").attr('disabled', true);


var allHospitalsWithBuildings;

$("#departments").empty();
$("#buildingsInDep").empty();








var depsInfoInBuilding;
var docRequired = new Array();
docRequired[0] = {
    "name": "var",
    "id": 1

}
docRequired[1] = {
    "name": "yox",
    "id": 0

}
var gender = new Array();
gender[0] = {
    "name": "Kişi",
    "id": 1

}
gender[1] = {
    "name": "Qadın",
    "id": 2

}
gender[2] = {
    "name": "Hamısı",
    "id": 0

}
$.each(docRequired, function () {
    $("#docRequired").append($("<option />").val(this.id).text(this.name));



});
$.each(gender, function () {
    $("#gender").append($("<option />").val(this.id).text(this.name));



});

getPageData();

function getPageData() {
    showLoading();

    $("#depTypesInBuilding").empty();
    $("#depsInBuilding").empty();
    $("#insertDepType").empty();
    $("#depType").empty();
    var selectedBuildingID = $("#buildings").val();
    $("#buildings").empty();
    $.ajax({
        type: 'POST',
        url: `/admin/departments/getBuildings`,
        data: { hospitalID: localStorage.selectedHospital },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            if (data.status != 4) {
                if (typeof (Storage) !== "undefined") {
                    localStorage.requestToken = data.requestToken;
                } else {
                    // Sorry! No Web Storage support..
                }
            }
            
            deps = data.data;
            deps.reverse();
            $.each(data.data, function () {
                $("#buildings").append($("<option />").val(this.id).text(this.name));
                $("#depBuilding").append($("<option />").val(this.id).text(this.name));
            });

            // Set the selected building option if it exists
            if (selectedBuildingID && $("#buildings option[value='" + selectedBuildingID + "']").length > 0) {
                $("#buildings").val(selectedBuildingID);
                $("#depBuilding").val(selectedBuildingID);
            }


             $.ajax({
        type: 'POST',
        url: `/admin/departments/getDepartmentsInfoByBuilding`,
        data: { userToken: localStorage.getItem("userToken"), requestToken: localStorage.getItem("requestToken"), buildingID: $("#buildings").val() },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            if (data.status != 4) {
                if (typeof (Storage) !== "undefined") {

                    localStorage.requestToken = data.requestToken
                } else {

                    // Sorry! No Web Storage support..
                }
            }
            depsInfoInBuilding = data.data;
            var depTypesInBuilding = [];



            // Department type mapping      

            depTypesInBuilding = [...new Map(depsInfoInBuilding.map(item =>
                [item['type'], item])).values()]
            $.each(depTypesInBuilding, function () {

                $("#depTypesInBuilding").append($(`<a class='list-group-item list-group-item-action depTypesInBuildingItem' id='depTypeInBuilding${this.typeID}'  onclick='{$(".depTypesInBuildingItem").removeClass("active");$(this).addClass("active");selectDepTypeInBuilding(${this.typeID})}' />`).text(this.type));
            });


            $.ajax({
                type: 'POST',
                url: `/admin/departments/getDepartmentTypes`,
                data: { userToken: localStorage.getItem("userToken"), requestToken: localStorage.getItem("requestToken") },
                dataType: 'json',
                success: function (data, status, xhr) {   // success callback function
                    //  var json = JSON.stringify(data)
                    if (data.status != 4) {
                        if (typeof (Storage) !== "undefined") {

                            localStorage.requestToken = data.requestToken
                        } else {

                            // Sorry! No Web Storage support..
                        }
                    }

                    deps = data.data;
                    //alert(data.requestTypes[0].name)
                    $.each(data.data, function () {
                        $("#insertDepType").append($("<option />").val(this.id).text(this.name));
                        $("#depType").append($("<option />").val(this.id).text(this.name));


                    });

                    // Əgər şöbə seçilibsə səyfə tam sıfırlanmaması üçün
                    if ($(".selectedDepCard").prop('id') != "") {
                        $.each(depsInfoInBuilding, function () {
                            if (this.id == $(".selectedDepCard").prop('id')) {

                                selectDepTypeInBuilding(this.typeID);
                                selectDepInBuilding(this.id);


                                $(".depTypesInBuildingItem").removeClass("active");
                                $(`#depTypeInBuilding${this.typeID}`).addClass("active");

                                $(".depsInBuildingItems").removeClass("active");
                                $(`#depInBuilding${this.id}`).addClass("active");
                            }


                        });


                    }



                    hideLoading();



                },
                error: function (jqXhr, textStatus, errorMessage) { // error callback


                    if (jqXhr.status == "401") {
                        localStorage.clear()
                        $('#systemModalTitle').text("Sessiyanız başa çatıb");
                        $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                        $('#systemModalBtn').removeAttr("hidden");
                        $('#systemModal').modal("show");
                    }
                    else {
                        $('#warningModal').modal('show')
                        $('#warningText').text(jqXhr.status);
                    }
                    //  $('#alert').text('Error: ' + errorMessage);
                }

            });


        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback


            if (jqXhr.status == "401") {
                localStorage.clear()
                $('#systemModalTitle').text("Sessiyanız başa çatıb");
                $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                $('#systemModalBtn').removeAttr("hidden");
                $('#systemModal').modal("show");
            }
            else {
                $('#warningModal').modal('show')
                $('#warningText').text(jqXhr.status);
            }
            //  $('#alert').text('Error: ' + errorMessage);
        }

    });


        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback

            hideLoading();

            switch (jqXhr.status) {
                case 401:
                    localStorage.clear()
                    $('#systemModalTitle').text("Sessiyanız başa çatıb");
                    $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                    $('#systemModalBtn').removeAttr("hidden");
                    $('#systemModal').modal("show");
                    break;
                case 0:
                    $('#warningModal').modal('show')
                    $('#warningTitle').text(`Şəbəkə xətası`);
                    $('#warningText').text(`Serverlərimizlə əlaqə yoxdur`);
                    break;
                default:
                    $('#warningModal').modal('show')
                    $('#warningTitle').text(`Server xətası`);
                    $('#warningText').text(`Status: ${jqXhr.status}`);
                    break;

            }

            //  $('#alert').text('Error: ' + errorMessage);
        }

    });

   

    
}

function selectDepTypeInBuilding(typeID) {
    // $(".depTypesInBuildingItem").removeClass("active");
    // $(button).addClass("active");

    $("#depsInBuilding").empty();
    $.each(depsInfoInBuilding, function () {
        if (this.typeID == typeID) {
            if (this.isActive == $("#depIsActiveFilter").val()) {
                $("#depsInBuilding").append($(`<a class='list-group-item list-group-item-action depsInBuildingItems' id='depInBuilding${this.id}' onclick='{$(".depsInBuildingItems").removeClass("active");
                $(this).addClass("active");selectDepInBuilding(${this.id});}' />`).text(this.name));
            }
        }

    });
}


function hospitalChanged() {
    //$("#depIsActive").prop("disabled", true);
    //$("#isRandevuActive").prop("disabled", true);
    //$('#docRequired').attr('disabled', true);
    //$('#gender').attr('disabled', true);
    //$('#depType').attr('disabled', true);
    //$("#depBuilding").attr('disabled', true);
    //$("#buildings").empty();
    //$("#depBuilding").empty();
   
    //$.each(allHospitalsWithBuildings.buildings, function () {
    //    if ($("#departments").val() == this.hospitalID) {
    //        $("#buildings").append($("<option />").val(this.id).text(this.name));
    //        $("#depBuilding").append($("<option />").val(this.id).text(this.name));

    //    }

    //});
    getPageData();
   
}
function buildingChanged() {
    $("#depIsActive").prop("disabled", true);
    $("#isRandevuActive").prop("disabled", true);
    $('#docRequired').attr('disabled', true);
    $('#gender').attr('disabled', true);
    $('#depType').attr('disabled', true);
    $("#depBuilding").attr('disabled', true);
    getPageData();

}

function selectDepInBuilding(depID) {


    $('#depBuilding').removeAttr('disabled');
    $('#depType').removeAttr('disabled');
    $('#docRequired').removeAttr('disabled');
    $('#gender').removeAttr('disabled');

    $.each(depsInfoInBuilding, function () {

        if (this.id == depID) {

            $(".selectedDepCard").attr("id", depID);
            $('#depBuilding').val(this.buildingID);
            $("#depType").val(this.typeID);
            $("#docRequired").val(this.docIsRequired);
            $("#gender").val(this.genderID);
            $("#depIsActive").prop("disabled", false);
            $("#isRandevuActive").prop("disabled", false);
            switch (this.isActive) {
                case 0:
                    $('#depIsActive').prop('checked', false);
                    break;

                case 1:
                    $('#depIsActive').prop('checked', true);
                    break;
            }
            switch (this.isRandevuActive) {
                case 0:
                    $('#isRandevuActive').prop('checked', false);
                    break;

                case 1:
                    $('#isRandevuActive').prop('checked', true);
                    break;
            }

        }


    });
}
function settingsBoxClicked(box) {
    switch (box) {
        case 1:
            $('#accessSettings').hide();
            $('#generalSettings').show()
            $("#generalSettingsButton").addClass("active")
            $("#accessSettingsButton").removeClass("active")
            break;

        case 2:
            $('#generalSettings').hide();
            $('#accessSettings').show();
            $("#accessSettingsButton").addClass("active")
            $("#generalSettingsButton").removeClass("active")
            break;
    }
}
async function insertDep() {

    showLoading();

    $("#depIsActive").prop("disabled", true);
    $("#isRandevuActive").prop("disabled", true);
    $('#docRequired').attr('disabled', true);
    $('#gender').attr('disabled', true);
    $.ajax({
        type: 'POST',
        url: `/admin/departments/insertDepartment`,
        data: {
           
            buildingID: $("#buildings").val(),
            name: $("#name").val(),
            depTypeID: $("#insertDepType").val()
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)

            if (typeof (Storage) !== "undefined") {

                localStorage.requestToken = data.requestToken
            } else {

                // Sorry! No Web Storage support..
            }
            getPageData();
            $('#addDep').modal('hide')
            $('#warningModal').modal('show')
            $('#warningText').text("Əlavə olundu");

            hideLoading();

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
            hideLoading();
        }

    });

}
function updateDep() {

    var depIsActive = 0;
    var isRandevuActive = 0
    if ($("#depIsActive").is(':checked')) {
        depIsActive = 1;  // checked
    }
    if ($("#isRandevuActive").is(':checked')) {
        isRandevuActive = 1;  // checked
    }


    //   console.log(
    //     $(".selectedDepCard").prop('id'),
    //         $('#depBuilding').val(),
    //     $("#depType").val(),
    //     $("#docRequired").val(),
    //     $("#gender").val(),
    //     depIsActive,
    //     isRandevuActive


    //   );
    $.ajax({
        type: 'POST',
        url: `/admin/departments/updateDepartment`,
        data: {
            userToken: localStorage.getItem("userToken"),
            requestToken: localStorage.getItem("requestToken"),
            id: $(".selectedDepCard").prop('id'),
            gender: $('#gender').val(),
            buildingID: $('#depBuilding').val(),
            depTypeID: $("#depType").val(),
            drIsRequired: $("#docRequired").val(),
            isActive: depIsActive,
            isRandevuActive: isRandevuActive
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)

            if (typeof (Storage) !== "undefined") {

                localStorage.requestToken = data.requestToken
            } else {

                // Sorry! No Web Storage support..
            }

            getPageData()
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback


            if (jqXhr.status == "401") {
                localStorage.clear()
                $('#systemModalTitle').text("Sessiyanız başa çatıb");
                $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
                $('#systemModalBtn').removeAttr("hidden");
                $('#systemModal').modal("show");
            }
            else {
                $('#warningModal').modal('show')
                $('#warningText').text(jqXhr.status);
            }

            //  $('#alert').text('Error: ' + errorMessage);
        }

    });



    //     $("#depIsActive").prop("disabled", true);
    // $("#isRandevuActive").prop("disabled", true);
    // $('#docRequired').attr('disabled', true);
    // $('#gender').attr('disabled', true);

}

