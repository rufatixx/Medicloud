const validation = {
    min4: '^.{4,}$',
    min7: '^.{7,}$',
    min6: '^.{6,}$',
    min8: '^.{8,}$',
    not_empty: '\\S',
    only_numbers: '^\\d+$',
    only_float_numbers: '[\\-.\\d]',
    only_letters: '^[a-zA-Z]+$',
    only_letters_and_numbers: '^[0-9a-zA-Z]+$',
    only_azeri_letters: '^[A-z\u00C7\u018F\u011E\u0049\u0130\u00D6\u015E\u00DC\u00E7\u0259\u011F\u0131\u0069\u00F6\u015F\u00FC\\s-]+$',
    has_lowercase: '(.*[a-z])',
    has_uppercase: '(.*[A-Z])',
    has_number: '(.*[0-9])',
    has_special_character: '(.*[!@#$%^&*_])',
    no_numbers: '^[^0-9]+$',
    no_special_characters: '(.*[!@#$%^&*_])',
    e_mail: '^\\w+([.-]?\\w+)*@\\w+([.-]?\\w+)*(\\.\\w{2,15})+$',
    only_password_characters: '^[0-9a-zA-Z.*[!@#$%^&*_]+$',
    allowed_mobile_extensions: '^0(?:50|51|55|70|77|99|10|60)[0-9]{7}$',
    promo_code: '^[0-9A-Z]{6}$',
};

setLoaderStatus(true);
window.onload = function () {
    switch (detectCurrentPage()) {
        case 'front_page':
            frontPageFunctions();
            enableFaqAccordion();
            break;
        case 'referral_dashboard':
            referralDashboardFunctions();
            break;
        case 'single':
            singlePageFunctions();
            break;
        case 'resend_me':
            resend_me_functions();
            setLoaderStatus(false);
            break;
        case 'error_page':
            setLoaderStatus(false);
            enableFaqAccordion();
            // cronFunction();
            break;
        case 'ads_from':
            window.location.href = settings.site_url + '/' + window.location.search;
            break;
        case 'success_page':
        default:
            setLoaderStatus(false);
    }
};

function enableFaqAccordion() {
    const questions = document.querySelectorAll(".faq .card-heading");
    questions.forEach((question) => question.addEventListener("click", () => {
        if (question.parentNode.classList.contains("active")) {
            question.parentNode.classList.toggle("active")
        }
        else {
            questions.forEach(question => question.parentNode.classList.remove("active"));
            question.parentNode.classList.add("active")
        }
    }))
}

function resend_me_functions() {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(0,0,0,0);
    const difference = tomorrow - today;

    const error_field = document.querySelector("#validation_error");
    const resend_me_button = document.querySelector("#resend_me_button");
    const email = document.querySelector("#resend_me_email");
    const local_loading = document.querySelector("#local_loading");

    defineStyle();

    // resend_me_button.addEventListener('click', function () {
    //     resendData();
    // });

    document.getElementById("resend_me_form").onsubmit = function() {
        resendData();
        return false;
    };

    function defineStyle() {
        if(getCookie('sending_limit') === 'active'){
            resend_me_button.classList.add('disabled');
            resend_me_button.disabled = true;
            resend_me_button.innerHTML = 'Bugün məktub göndərilib';
        } else {
            resend_me_button.classList.remove('disabled');
            resend_me_button.disabled = false;
        }
    }

    function resendData() {

        let data = {};

        /** Clear error notes on field focus */
        email.addEventListener('focus', function () {
            error_field.innerText = '';
        });

        /** Send data if validation is OK */
        if (registerValidation()) sendData(data);

        function registerValidation() {
            let emailOK = false;

            if (email.value) {
                if (validate(email.value, validation.e_mail)) {
                    data.email = email.value;
                    emailOK = true;
                } else {
                    error_field.innerText = settings.error_list.email_error;
                    delete data[email];
                    emailOK = false;
                }
            } else {
                error_field.innerText = settings.error_list.missing_email_error;
                delete data[email];
                emailOK = false;
            }

            return emailOK;
        }

        function sendData(data) {
            local_loading.classList.add('display');



            axios.post(settings.endpoint_list.resend_me_endpoint, data)
                .then(response => {
                    if (notProdMode()) console.log(response);
                    setCookie('sending_limit', 'active', difference / 1000 / 3600 / 24);
                    defineStyle();
                    resend_me_button.innerHTML = 'Məktub göndərildi';
                    window.location.href = settings.site_url + '/' + 'success-page';
                })
                .catch(error => {
                    window.setTimeout(function () {
                        local_loading.classList.remove('display');
                    }, 1000);
                    if (error.response.status === 404) {
                        error_field.innerText = settings.error_list.email_not_found_error;
                    }
                    if (error.response.status === 400 && error.response.data.detail === 'Bad Request') {
                        error_field.innerText = settings.error_list.email_exists_error;
                    }
                    if (error.response.status === 422 && error.response.data.detail[0].loc[1] === 'email') {
                        error_field.innerText = settings.error_list.email_error;
                    }
                    if (notProdMode()) {
                        console.log(error.response.status);
                        console.log(error.response.data.detail);
                    }
                })
        }
    }
}

function frontPageFunctions() {
    displayUserCount();

    function displayUserCount() {
        const user_count = document.querySelector(".user_count");
        user_count.innerText = "11000+";
        functionsAfterAxios();
        // axios.get(settings.endpoint_list.user_count_endpoint)
        //     .then(response => {
        //         user_count.innerText = response.data;
        //         functionsAfterAxios();
        //     })
        //     .catch(error => {
        //         user_count.innerText = 8363;
        //         if (notProdMode()) {
        //             console.log(error.response.status);
        //             console.log(error.response.data);
        //         }
        //         functionsAfterAxios();
        //     })
    }

    function functionsAfterAxios() {
        // checkPromoCodeFromUrlParams();
        // enableRegistration();
        // enablePhoneNumberMask();
        // enablePromoCodeMask();
        setLoaderStatus(false);
        slideInHeadlineImage();
    }

    function slideInHeadlineImage() {
        const headline_image = document.querySelector(".headline-image");
        headline_image && headline_image.classList.add("ready");
    }

    function checkPromoCodeFromUrlParams() {
        let url = new URL(window.location.href);
        let promo = url.searchParams.get('promo');
        const promo_code = document.querySelector("#promo_code");
        if (promo_code) {
            if (promo && (promo !== 'null')) {
                promo_code.value = promo;
                promo_code.disabled = true;
                promo_code.classList.add("disabled");
            } else {
                promo_code.value = null;
                promo_code.disabled = false;
                promo_code.classList.remove("disabled");
            }
        }
    }

    function enableRegistration() {
        const registerButton = document.querySelector("#register_button");
        const registration_form = document.querySelector("#registration_form");
        const error_field_1 = document.querySelector("#validation_error_1");
        const error_field_2 = document.querySelector("#validation_error_2");
        const error_field_3 = document.querySelector("#validation_error_3");
        const register_loading = document.querySelector("#register_loading");

        // registerButton.addEventListener('click', function () {
        //     sendRegistrationData()
        // });

        document.getElementById("registration_form").onsubmit = function() {
            sendRegistrationData();
            return false;
        };


        function sendRegistrationData() {
            let data = {};

            const email = document.querySelector("#email");
            const phone_number = document.querySelector("#phone_number");
            const promo_code = document.querySelector("#promo_code");

            /** Clear error notes on field focus */
            email.addEventListener('focus', function () {
                error_field_1.innerText = '';
            });
            phone_number.addEventListener('focus', function () {
                error_field_2.innerText = '';
            });
            promo_code.addEventListener('focus', function () {
                error_field_3.innerText = '';
            });

            /** Send registration data if validation is OK */
            if (registerValidation()) sendData(data);

            function registerValidation() {
                let emailOK = false;
                let phoneNumberOK = false;
                let promoOK = false;

                if (email.value) {
                    if (validate(email.value, validation.e_mail)) {
                        data.email = email.value;
                        emailOK = true;
                    } else {
                        error_field_1.innerText = settings.error_list.email_error;
                        delete data[email];
                        emailOK = false;
                    }
                } else {
                    error_field_1.innerText = settings.error_list.missing_email_error;
                    delete data[email];
                    emailOK = false;
                }

                if (phone_number.value) {
                    if (validate(formatPhoneNumber(phone_number.value), validation.allowed_mobile_extensions)) {
                        data.phone_number = reformatPhoneNumber(phone_number.value);
                        phoneNumberOK = true;
                    } else {
                        error_field_2.innerText = settings.error_list.phone_number_error;
                        delete data[phone_number];
                        phoneNumberOK = false;
                    }
                } else {
                    error_field_2.innerText = settings.error_list.missing_phone_number_error;
                    delete data[phone_number];
                    phoneNumberOK = false;
                }

                if (promo_code.value) {
                    if (validate(promo_code.value, validation.promo_code)) {
                        data.promo_code = promo_code.value;
                        promoOK = true;
                    } else {
                        error_field_3.innerText = settings.error_list.promo_code_error;
                        delete data[promo_code];
                        promoOK = false;
                    }
                } else {
                    // error_field_3.innerText = settings.error_list.missing_promo_code_error;
                    delete data[promo_code];
                    promoOK = true;
                    // promoOK = false;
                }

                return emailOK && phoneNumberOK && promoOK;
            }

            function sendData(data) {
                register_loading.classList.add('display');
                axios.post(settings.endpoint_list.registration_endpoint, data)
                    .then(response => {
                        if (notProdMode()) console.log(response);
                        window.location.href = settings.site_url + '/' + 'success-page';
                    })
                    .catch(error => {
                        window.setTimeout(function () {
                            register_loading.classList.remove('display');
                        }, 1000);

                        if (error.response.status === 400 && error.response.data.detail === 'Bad Request') {
                            error_field_1.innerText = settings.error_list.email_exists_error;
                        }
                        if (error.response.status === 422 && error.response.data.detail[0].loc[1] === 'phone_number') {
                            error_field_2.innerText = settings.error_list.phone_number_error;
                        }
                        if (error.response.status === 422 && error.response.data.detail[0].loc[1] === 'email') {
                            error_field_1.innerText = settings.error_list.email_error;
                        }
                        if (error.response.status === 400 && error.response.data.detail === 'invalid promo code') {
                            error_field_3.innerText = settings.error_list.promo_code_error;
                        }
                        if (notProdMode()) {
                            console.log(error.response.status);
                            console.log(error.response.data.detail);
                        }
                    })
            }
        }
    }

    function enablePhoneNumberMask() {
        const $input = document.querySelector('[data-js="input"]');
        if ($input) {
            $input.addEventListener('input', handleInput, false);

            function handleInput(e) {
                e.target.value = phoneMask(e.target.value)
            }

            function phoneMask(phone) {
                return phone.replace(/\D/g, '')
                    .replace(/^(\d)/, '($1')
                    .replace(/^(\(\d{3})(\d)/, '$1) $2')
                    .replace(/(\d{3})(\d{1,5})/, '$1-$2')
                    .replace(/(-\d{2})(\d{1,5})/, '$1-$2')
                    .replace(/(-\d{2})\d+?$/, '$1');
            }
        }

    }

    function enablePromoCodeMask() {
        const $input = document.querySelector('[data-js="promo_input"]');
        if ($input) {
            $input.addEventListener('input', handleInput, false);

            function handleInput(e) {
                e.target.value = promoMask(e.target.value)
            }

            function promoMask(promoInput) {
                return promoInput.replace(/[^a-z0-9]/gi, '').toUpperCase();
            }
        }
    }
}

function referralDashboardFunctions() {
    getReferralDashboardData();

    function getReferralDashboardData() {
        axios.get(settings.endpoint_list.dashboard_data_endpoint, {
            headers: {
                'Authorization': `Bearer ${getTokenFromCurrentUrl()}`,
                'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
            }
        })
            .then(response => {
                if (notProdMode()) console.log(response);
                activateReferralDashboardFunctions(response.data);
                setLoaderStatus(false);
                // let link = createLink(response.data.promo_code);
            })
            .catch(error => {
                if (notProdMode()) console.log(error);
                window.location.href = settings.site_url;
            })
    }
}

function singlePageFunctions() {
    activateSinglePageFunctions();
    setLoaderStatus(false);
}

function setLoaderStatus(status) {
    if (status) {
        document.body.classList.remove('loaded');
        document.body.classList.add('loaded_hiding');
    } else {
        document.body.classList.add('loaded');
        document.body.classList.remove('loaded_hiding');
    }
}

function detectCurrentPage() {
    if (document.querySelector('#referral_dashboard')) return 'referral_dashboard';
    if (document.querySelector('#single')) return 'single';
    if (document.querySelector('#front_page')) return 'front_page';
    if (document.querySelector('#success_page')) return 'success_page';
    if (document.querySelector('#error_page')) return 'error_page';
    if (document.querySelector('#resend_me')) return 'resend_me';
    if (document.querySelector('#ads_from')) return 'ads_from';
    // Other pages should be listed here if used in js code;
}


function getCurrentUrl() {
    return new URL(window.location.href);
}

function getTokenFromCurrentUrl() {
    return getCurrentUrl().searchParams.get("token");
}

function getPromoCodeFromCurrentUrl() {
    return getCurrentUrl().searchParams.get("promo");
}

function activateReferralDashboardFunctions(data) {
    const referral_sharing_link = document.querySelector('.referral_sharing_link');
    const referral_sharing_link_text = document.querySelector('.referral_sharing_link_text');
    const referral_sharing_promo = document.querySelector('.referral_sharing_promo');
    const referrals_summary_email = document.querySelector('.referrals_summary_email');
    const referrals_summary_invited_number = document.querySelector('.referrals_summary_invited_number');
    const referrals_summary_income_number = document.querySelector('.referrals_summary_income_number');

    const referral_sharing_link_copy_button = document.querySelector('.referral_sharing_link_copy_button');
    const referral_sharing_promo_copy_button = document.querySelector('.referral_sharing_promo_copy_button');

    const welcome_bonus = document.querySelector('.welcome_bonus');
    const referral_bonus = document.querySelector('.referral_bonus');

    const referrals_pop_up_array = document.querySelectorAll('.referrals_pop_up');
    const referrals_pop_up_overlay_array = document.querySelectorAll('.referrals_pop_up_overlay');
    const referrals_pop_up_content_close_button_array = document.querySelectorAll('.referrals_pop_up_content_close_button');

    const addtoany_list = document.querySelector('.addtoany_list');

    const post_button_links = document.querySelectorAll('.post_button');

    printReferralDashboardData();
    referral_sharing_link_text.value += ' Promo kodu qeyd etməyi unutma: ' + data.promo_code;
    setSharingAttributes(referral_sharing_link.value, referral_sharing_link_text.value);
    enableEventListeners();
    addPromoToPostLinks();

    function addPromoToPostLinks() {
        if(data.promo_code) {
            post_button_links.forEach(item => {
                item.href = item.href + '?promo=' + data.promo_code;
            })
        }
    }

    function printReferralDashboardData() {
        referral_sharing_link.value = data.link;
        referral_sharing_link.disabled = true;
        referral_sharing_promo.value = data.promo_code;
        referral_sharing_promo.disabled = true;
        referrals_summary_email.textContent = data.email;
        referrals_summary_invited_number.textContent = data.counter;
        referrals_summary_income_number.textContent = welcome_bonus.textContent + ' + ' + data.counter * referral_bonus.textContent;
    }

    function enableEventListeners() {
        referral_sharing_link_copy_button.addEventListener('click', function () {
            copyToClipboard(referral_sharing_link.value);
            setPopUpStatus(true, 0);
            setSharingAttributes(referral_sharing_link.value, referral_sharing_link_text.value);
        });
        referral_sharing_promo_copy_button.addEventListener('click', function () {
            copyToClipboard(referral_sharing_promo.value);
            setPopUpStatus(true, 1);
        });
    }

    function copyToClipboard(text) {
        let temporaryInput = document.body.appendChild(document.createElement("input"));
        temporaryInput.value = text;
        temporaryInput.select();
        document.execCommand('copy');
        temporaryInput.parentNode.removeChild(temporaryInput);
    }

    function setSharingAttributes(url, text) {
        addtoany_list.setAttribute("data-a2a-url", url);
        addtoany_list.setAttribute("data-a2a-title", text);
    }

    function setPopUpStatus(status, index) {
        referrals_pop_up_overlay_array[index].addEventListener('click', function () {
            setPopUpStatus(false, index);
        });
        referrals_pop_up_content_close_button_array[index].addEventListener('click', function () {
            setPopUpStatus(false, index);
        });
        if (status) {
            referrals_pop_up_array[index].classList.add('show');
        } else {
            referrals_pop_up_array[index].classList.remove('show');
        }
    }
}

function activateSinglePageFunctions() {
    const addtoany_list = document.querySelector('.addtoany_list');

    addPromoToAllLinks();
    setSharingAttributes();

    function addPromoToAllLinks() {
        document.querySelectorAll('a').forEach(item => {
            if(getPromoCodeFromCurrentUrl()) {
                item.href = item.href + '?promo=' + getPromoCodeFromCurrentUrl();
            }

        })
    }

    function setSharingAttributes() {
        addtoany_list.setAttribute("data-a2a-url", addtoany_list.getAttribute('data-a2a-url') + '?promo=' + getPromoCodeFromCurrentUrl());
    }
}


/*====================================================================================================================*/
/* MAIN FUNCTIONS */

/*====================================================================================================================*/


function getPromoFromField() {
    const promo = document.querySelector("#promo_field");
    const link = document.querySelector("#wpsp-120 a");
    if (promo) {
        link.href = link.href + '?promo=' + promo.value;

    }


}


/*====================================================================================================================*/
/* HELPER FUNCTIONS */

/*====================================================================================================================*/
function formatPhoneNumber(number) {
    return number.match(/\d+/g).join('');
}

function reformatPhoneNumber(number) {
    return '994' + number.match(/\d+/g).join('').slice(1);
}

function validate(value, rule) {
    let Regex = new RegExp(rule);
    return Regex.test(value);
}

// function createLink(promo_code) {
//     return 'https://www.virtualkart.az/?promo=' + promo_code;
// }

/*====================================================================================================================*/
/* COOKIE FUNCTIONS */

/*====================================================================================================================*/

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    let nameEQ = name + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}











