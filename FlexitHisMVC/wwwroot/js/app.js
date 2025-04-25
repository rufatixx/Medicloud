"use strict";

$(function () {
	const $adminButton = $("#adminButton");
	const $userType = $(".userType");
	const $selectOrganizationDropdownItems = $("#selectOrganizationDropdownItems");
	const $selectedOrganizationName = $("#selectedOrganizationName");
	const $organizationLogo = $('#organizationLogo');
	const $newPatientButton = $("#newPatientButton");
	const $kassaButton = $("#kassaButton");
	const $policlinicButton = $("#policlinicButton");
	const $organizationSelector = $("#organizationSelector");
	const $systemModal = $('#systemModal');
	const $warningModal = $('#warningModal');
	const $warningModalButton = $("#warningModalButton");
	const $warningText = $('#warningText');
	const $fullName = $(".fullName");

	function parseJSON(json) {
		try {
			return JSON.parse(json);
		} catch (e) {
			console.error("Error parsing JSON:", e);
			return null;
		}
	}

	//    function updateOrganizationSelection(organization) {
	//        localStorage.setItem('selectedOrganization', organization.organizationID);
	//        localStorage.setItem('selectedOrganizationName', organization.organizationName);
	//        $selectedOrganizationName.text(organization.organizationName);
	//        $organizationLogo.text(organization.organizationName);
	//        // Save to cookie and wait for it to complete
	//        saveUserSetting("organizationID", organization.organizationID)
	//            .then(() => {
	//                // After saving selectedOrganization, save the next setting
	//                return saveUserSetting("organizationName", organization.organizationName);
	//            })
	//            .then(() => {
	//                // After all settings are saved, refresh the page
	//                console.log('All settings saved, refreshing page.');
	//                window.location.reload();
	//            })
	//            .fail((error) => {
	//                // Handle any errors that occurred during the save operations
	//                console.error("Error saving settings: ", error);
	//            });
	//    }

	//    function populateOrganizationList(organizations) {
	//        organizations.forEach(organization => {
	//            const dropdownItem = $(`<a class="dropdown-item" id="${organizationid}">${organization.organizationName}</a>`);
	//            dropdownItem.on('click', () => updateOrganizationSelection(organization));
	//            $selectOrganizationDropdownItems.append(dropdownItem);
	//        });
	//    }

	//    function setInitialOrganization(organizations) {
	//        if (organizations.length > 0) {
	//            updateOrganizationSelection(organizations[0]);
	//        } else {
	//            $newPatientButton.hide();
	//            $selectedOrganizationName.hide();
	//            $organizationLogo.hide();
	//            $kassaButton.hide();
	//            $policlinicButton.hide();
	//            $organizationSelector.text("Heç bir xəstəxanaya icazəniz yoxdur, zəhmət olmasa çağrı mərkəzimizə müraciət edin");
	//        }
	//    }

	//function initialize() {
	//    const json = localStorage.getItem("json");
	//    if (json) {
	//        const parsedJSON = parseJSON(json);
	//        if (!parsedJSON) return;

	//        const personal = parsedJSON.data[0].personal;
	//        const organizations = parsedJSON.data[0].organizations;

	//        if (personal.isAdmin) {
	//            $adminButton.show();
	//            $userType.text("Admin");
	//        } else {
	//            $adminButton.hide();
	//            $userType.text("İstifadəçi");
	//        }

	//        populateOrganizationList(organizations);

	//        if (localStorage.getItem('selectedOrganization')) {
	//            $selectedOrganizationName.text(localStorage.getItem('selectedOrganizationName'));
	//            $organizationLogo.text(localStorage.getItem('selectedOrganizationName'));
	//        } else {
	//            setInitialOrganization(organizations);
	//        }

	//        $fullName.text(`${personal.name} ${personal.surname}`);
	//    } else {
	//        $systemModal.modal('show');
	//    }
	//}


	//initialize();

});

function showLoading() {
	$('#loadingModal').show();

	$('#loadingModalText').html(`<center><div class="spinner-grow text-light" style="width: 7rem; height: 7rem;" role="status">
   
  </div></center>`);

}
function hideLoading() {
	$('#loadingModal').hide();
}

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


function SwitchOrganization(organizationId) {
	showLoading()
	$.ajax({
		url: "/login/SwitchOrganization",
		method: "POST",
		data: { organizationId: organizationId },
		success: function (data) {
			window.location.reload();
		},
		error: function (xhr, status, error) {
			// Request failed
			console.error(error);
			hideLoading()
		}
	});

}


function saveUserSetting(key, value) {
	return $.get('/Profile/SaveUserSetting', { key: key, value: value })
		.done(function (response) {
			console.log('Data saved to cookie:', response);
		})
		.fail(function (error) {
			console.error('Error saving data to cookie:', error);
		});
}