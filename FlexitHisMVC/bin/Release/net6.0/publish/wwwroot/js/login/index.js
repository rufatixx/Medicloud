//$(document).ready(function () {
//    // Enter key event listener for sign in
//    $('#username, #passBox').keypress(function (event) {
//        if (event.which === 13) {
//            event.preventDefault();
//            signIn($('#signInButton'));
//        }
//    });

//    function signIn(btn) {
//        let $btn = $(btn);
//        let phone = $("#phone").val();
//        let password = $("#passBox").val();
//        let $alert = $("#alert");

//        // Disable button and show loading state
//        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true" id="signInSpinner"></span> Yüklənir...');

//        // Validate inputs
//        if (username.length === 0) {
//            showError('Xahiş olunur NÖMRƏNİZİN düzgünlüyünə diqqət edəsiniz!');
//            return;
//        }

//        if (!password) {
//            showError('Xahiş olunur ŞİFRƏNİZİN düzgünlüyünə diqqət edəsiniz!');
//            return;
//        }

//        // Ajax POST request
//        $.ajax({
//            type: 'POST',
//            url: '/Login/SignIn',
//            data: { phone: phone, pass: password },
//            dataType: 'json',
//            success: function (data, status, xhr) {
//                handleLoginSuccess(data, xhr.status);
//            },
//            error: function (jqXhr, textStatus, errorMessage) {
//                handleLoginError(jqXhr.status);
//            }
//        });

//        function showError(message) {
//            $alert.text(message).removeAttr("hidden");
//            resetButton();
//        }

//        function resetButton() {
//            $btn.prop('disabled', false).text('Daxil ol');
//        }

//        function handleLoginSuccess(data, statusCode) {
//            resetButton();
//            if (statusCode === 200) {
//                if (typeof (Storage) !== "undefined") {
//                    localStorage.json = JSON.stringify(data);
//                    window.location.replace("/");
//                } else {
//                    showError('Brauzerinizi yeniləyin, dəstəklənmir');
//                }
//            } else {
//                showError('İstifadəçi mövcud deyil');
//            }
//        }

//        function handleLoginError(statusCode) {
//            resetButton();
//            if (statusCode === 401) {
//                showError('İstifadəçi mövcud deyil');
//            } else {
//                showError('Xəta, internetə bağlı olduğunuzdan əmin olun');
//            }
//        }
//    }
//    window.signIn = signIn; // Make the logout function globally accessible

//    // The getCookie function is commented out as it's not used in the signIn function
//    // If you need it for other parts of your application, you can uncomment it.
//    // function getCookie(cname) {
//    //     ...
//    // }
//});
