
$('.accessSettings').hide();
$('.generalSettings').show()

showLoading();

$("#depIsActive").prop("disabled", true);
$("#isRandevuActive").prop("disabled", true);
$('#docRequired').attr('disabled', true);
$('#gender').attr('disabled', true);
$('#depType').attr('disabled', true);
$("#depBuilding").attr('disabled', true);



var allOrganizationsWithBuildings;

$("#departments").empty();
$("#buildingsInDep").empty();

//$.get(
//    "/Admin/Company/GetAllOrganizationsWithBuildings",

//    function (data) {
//        allOrganizationsWithBuildings = data;
//        if (data.organizations.length > 0) {

//            $.each(data.organizations, function () {

//                $("#departments").append($("<option />").val(this.id).text(this.organizationName));




//            });
//            getPageData();
//        }




//    });
getPageData();

var groupsInBuilding;
var companiesInBuilding;
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



function getPageData() {
  
    //showLoading();
    //$("#companiesInBuilding").empty();
    //$("#cGroups").empty();
   
    //$("#companiesIsActive").prop('checked', true);
    //$("#groupsIsActive").prop('checked', true);

    //$.ajax({
    //    type: 'POST',
    //    url: `/admin/companies/getCompanyGroups`,
    //    data: { organizationID: localStorage.selectedOrganization },
    //    dataType: 'json',
    //    success: function (data, status, xhr) {   // success callback function
    //        //  var json = JSON.stringify(data)
    //        if (data.status != 4) {
    //            if (typeof (Storage) !== "undefined") {

    //                localStorage.requestToken = data.requestToken
    //            } else {

    //                // Sorry! No Web Storage support..
    //            }
          

    //        groupsInBuilding = data.data;

    //        //alert(data.requestTypes[0].name)
    //        $.each(data.data, function () {
    //            if(this.isActive == 1)
    //            {
    //             $("#cGroups").append($(`<a class='list-group-item list-group-item-action companyGroupInBuildingItem' id='selectedCompanyGroup${this.id}'  onclick='{selectGroup(this,${this.id})}' />`).text(this.name));
    //            }
                
    //            $(".cGroupsAll").append($("<option />").val(this.id).text(this.name));
               
    //        });


    //        $.ajax({
    //            type: 'POST',
    //            url: `/admin/companies/getCompanies`,
    //            data: { organizationID: localStorage.selectedOrganization },
    //            dataType: 'json',
    //            success: function (data, status, xhr) {   // success callback function
    //                //  var json = JSON.stringify(data)
    //                if (data.status != 4) {
    //                    if (typeof (Storage) !== "undefined") {
        
    //                        localStorage.requestToken = data.requestToken
    //                         //alert(data.requestTypes[0].name)
    //                $.each(data.data, function () {
    //                    if(this.isActive == 1)
    //                    {
    //                        $("#companiesInBuilding").append($(`<a class='list-group-item list-group-item-action companyInBuildingItem' id='selectedCompany${this.id}'  onclick='{selectCompany(this,${this.id})}' />`).text(this.name));
    //                    }
                       
    //                });
        
    //    companiesInBuilding = data.data;
        
    //                    } else {
        
    //                        // Sorry! No Web Storage support..
    //                    }
    //                }
        
        
        
                   
    //                hideLoading();
        
    //            },
    //            error: function (jqXhr, textStatus, errorMessage) { // error callback
        
    //                hideLoading();
        
    //                switch (jqXhr.status) {
    //                    case 401:
    //                        localStorage.clear()
    //                        $('#systemModalTitle').text("Sessiyanız başa çatıb");
    //                        $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
    //                        $('#systemModalBtn').removeAttr("hidden");
    //                        $('#systemModal').modal("show");
    //                        break;
    //                    case 0:
    //                        $('#warningModal').modal('show')
    //                        $('#warningTitle').text(`Şəbəkə xətası`);
    //                        $('#warningText').text(`Serverlərimizlə əlaqə yoxdur`);
    //                        break;
    //                    default:
    //                        $('#warningModal').modal('show')
    //                        $('#warningTitle').text(`Server xətası`);
    //                        $('#warningText').text(`Status: ${jqXhr.status}`);
    //                        break;
        
    //                }
        
    //                //  $('#alert').text('Error: ' + errorMessage);
    //            }
        
    //        });
    //    }
    //    else{
    //        $('#warningModal').modal('show')
    //                $('#warningTitle').text(`Server xətası`);
                  
    //    }

    //    },
    //    error: function (jqXhr, textStatus, errorMessage) { // error callback

    //        hideLoading();

    //        switch (jqXhr.status) {
    //            case 401:
    //                localStorage.clear()
    //                $('#systemModalTitle').text("Sessiyanız başa çatıb");
    //                $('#systemModalText').html(`<p id="systemModalText">Zəhmət olmasa yenidən giriş edin</p>`);
    //                $('#systemModalBtn').removeAttr("hidden");
    //                $('#systemModal').modal("show");
    //                break;
    //            case 0:
    //                $('#warningModal').modal('show')
    //                $('#warningTitle').text(`Şəbəkə xətası`);
    //                $('#warningText').text(`Serverlərimizlə əlaqə yoxdur`);
    //                break;
    //            default:
    //                $('#warningModal').modal('show')
    //                $('#warningTitle').text(`Server xətası`);
    //                $('#warningText').text(`Status: ${jqXhr.status}`);
    //                break;

    //        }

    //        //  $('#alert').text('Error: ' + errorMessage);
    //    }

    //});

}

function organizationChanged() {
   

    $(".companySettings").hide();
    $(".groupSettings").hide();

   
    getPageData();

}



function filterGroups(){
    $("#cGroups").empty()
    if ($("#groupsIsActive").is(':checked')) {
        $.each(groupsInBuilding, function () {
            if(this.isActive == 1)
            {
             $("#cGroups").append($(`<a class='list-group-item list-group-item-action companyGroupInBuildingItem' id='depTypeInBuilding${this.typeID}'  onclick='{selectGroup(this,${this.id})}' />`).text(this.name));
            }
         
            
         });
    }
    else{
    
        $.each(groupsInBuilding, function () {
            if(this.isActive == 0)
            {
             $("#cGroups").append($(`<a class='list-group-item list-group-item-action companyGroupInBuildingItem' id='depTypeInBuilding${this.typeID}'  onclick='{selectGroup(this,${this.id})}' />`).text(this.name));
            }
         
            
         });
    }





}
function filterCompanies(){
    $("#companiesInBuilding").empty()
    if ($("#companiesIsActive").is(':checked')) {
        $.each(companiesInBuilding, function () {
            if(this.isActive == 1)
            {
                $("#companiesInBuilding").append($(`<a class='list-group-item list-group-item-action companyInBuildingItem'   onclick='{selectCompany(this,${this.id})}' />`).text(this.name));
            }
         
            
         });
    }
    else{
    
        $.each(companiesInBuilding, function () {
            if(this.isActive == 0)
            {
                $("#companiesInBuilding").append($(`<a class='list-group-item list-group-item-action companyInBuildingItem'  onclick='{selectCompany(this,${this.id})}' />`).text(this.name));
            }
         
            
         });
    }





}
function selectGroup(obj, groupID) {
    $(".companyGroupInBuildingItem").removeClass("active");
    $(".companyInBuildingItem").removeClass("active");
    $(obj).addClass("active");
    $(".groupSettings").show();
    $(".companySettings").hide();
    $(".groupSettings").attr("id", groupID);
    $.each(groupsInBuilding, function () {
        if(this.id == groupID)
        {
            $("#groupNameInSettings").val(this.name);
            $("#selectedCompanyGroupTypeInSettings").val(this.type);
            switch (this.isActive) {
                case 0:
                    $('#groupIsActive').prop('checked', false);
                    break;

                case 1:
                    $('#groupIsActive').prop('checked', true);
                    break;
            }
        }
     
        
     });

}
function selectCompany(obj, companyID) {
    $(".companyGroupInBuildingItem").removeClass("active");
    $(".companyInBuildingItem").removeClass("active");
    $(obj).addClass("active");
    $(".companySettings").show();
    $(".groupSettings").hide();
    $(".companySettings").attr("id", companyID);
    $.each(companiesInBuilding, function () {
        if(this.id == companyID)
        {
            $("#companyNameInSettings").val(this.name);
            $("#selectedCompanyGroupInSettings").val(this.groupID);
            switch (this.isActive) {
                case 0:
                    $('#companyIsActive').prop('checked', false);
                    break;

                case 1:
                    $('#companyIsActive').prop('checked', true);
                    break;
            }
        }
     
        
     });
   
}

function settingsBoxClicked(box) {
    switch (box) {
        case 1:
            $('.accessSettings').hide();
            $('.generalSettings').show()
            $(".generalSettingsButton").addClass("active")
            $(".accessSettingsButton").removeClass("active")
            break;

        case 2:
            $('.generalSettings').hide();
            $('.accessSettings').show();
            $(".accessSettingsButton").addClass("active")
            $(".generalSettingsButton").removeClass("active")
            break;
    }
}
async function insertCGroup() {

    showLoading();

    $("#depIsActive").prop("disabled", true);
    $("#isRandevuActive").prop("disabled", true);
    $('#docRequired').attr('disabled', true);
    $('#gender').attr('disabled', true);
    $.ajax({
        type: 'POST',
        url: `/admin/companies/insertCompanyGroup`,
        data: {
           
            organizationID: localStorage.selectedOrganization,
            cGroupName: $("#groupName").val(),
            cGroupType: $("#cGroupType").val(),
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            $('#addCompanyGroup').modal('hide')
            $('#warningModal').modal('show')
            if (data) {
                $('#warningText').text("Əlavə olundu");
                getPageData()
            }
            else {
                $('#warningText').text("Xəta baş verdi");
            }

            hideLoading();

        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback
            $('#addCompanyGroup').modal('hide')
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
            hideLoading();
        }

    });

}
async function insertCompany() {

    showLoading();


    $.ajax({
        type: 'POST',
        url: `/admin/companies/insertCompany`,
        data: {
         
            organizationID: localStorage.selectedOrganization,
            companyName: $("#companyName").val(),
            cGroupID: $("#cGroupsInModal").val(),
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)
            $('#addCompany').modal('hide')
            $('#warningModal').modal('show')
            if (data) {
                $('#warningText').text("Əlavə olundu");
                getPageData()
            }
            else {
                $('#warningText').text("Xəta baş verdi");
            }
           


            hideLoading();

        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback
            $('#addCompanyGroup').modal('hide')
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
            hideLoading();
        }

    });

}
function updateCompanyGroup() {
    showLoading();
    
    var groupIsActive = 0
    if ($("#groupIsActive").is(':checked')) {
        groupIsActive = 1;  // checked
    }
   

    $.ajax({
        type: 'POST',
        url: `/admin/companies/updateCompanyGroup`,
        data: {
         
            id: $(".groupSettings").prop('id'),
            organizationID: localStorage.selectedOrganization,
            name: $("#groupNameInSettings").val(),
            groupTypeId: $("#groupTypeIdInSettings").val(),
            isActive: groupIsActive,
           
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)

            if (typeof (Storage) !== "undefined") {

                localStorage.requestToken = data.requestToken
            } else {

                // Sorry! No Web Storage support..
            }
hideLoading();
            getPageData()
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback
            hideLoading();

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

function updateCompany() {
    showLoading();
    
    var companyIsActive = 0
    if ($("#companyIsActive").is(':checked')) {
        companyIsActive = 1;  // checked
    }
   

    $.ajax({
        type: 'POST',
        url: `/admin/companies/updateCompany`,
        data: {
           
            id: $(".companySettings").prop('id'),
            organizationID: localStorage.selectedOrganization,
            name:$("#companyNameInSettings").val(),
            isActive: companyIsActive,
           
        },
        dataType: 'json',
        success: function (data, status, xhr) {   // success callback function
            //  var json = JSON.stringify(data)

            if (typeof (Storage) !== "undefined") {

                localStorage.requestToken = data.requestToken
            } else {

                // Sorry! No Web Storage support..
            }
hideLoading();
            getPageData()
        },
        error: function (jqXhr, textStatus, errorMessage) { // error callback
            hideLoading();

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

