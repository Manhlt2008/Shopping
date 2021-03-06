﻿function Register(event) {
    event.preventDefault();
    $(".text-danger").remove();

    //Get data
    var firstName = $("#form-register-input-firstname").val();
    var lastName = $("#form-register-input-lastname").val();
    var email = $("#form-register-input-email").val();
    var password = $("#form-register-input-password").val();
    var passwordConfirm = $("#form-register-input-password-confirm").val();
    var isCondition = $("#checkbox-condition").val();
    var isValid = true;

    //Validation 
    //First name
    if (!firstName && !firstName.trim()) {
        messageTextDanger($("#form-register-input-firstname"), "Vui lòng nhập họ.");
        isValid = false;
    } else if (firstName.trim().length < 1) {
        messageTextDanger($("#form-register-input-firstname"), "Họ phải có ít nhất một ký tự.");
        isValid = false;
    } else if (firstName.trim().length > 250) {
        messageTextDanger($("#form-register-input-firstname"), "Họ không được quá 250 ký tự.");
        isValid = false;
    }
    //Last name
    if (!lastName && !lastName.trim()) {
        messageTextDanger($("#form-register-input-lastname"), "Vui lòng nhập tên.");
        isValid = false;
    } else if (!isValidLength(lastName.trim())) {
        messageTextDanger($("#form-register-input-lastname"), "Độ dài tên phải nằm từ khoảng 1 đến 250 ký tự.");
        isValid = false;
    }

    //Email
    if (!email && !email.trim()) {
        messageTextDanger($("#form-register-input-email"), "Vui lòng nhập email.");
        isValid = false;
    } else
        if (!validateEmail(email)) {
            messageTextDanger($("#form-register-input-email"), "Email không đúng định dạng.");
            isValid = false;
        }

    //Password
    if (!password && !password.trim()) {
        messageTextDanger($("#form-register-input-password"), "Vui lòng nhập mật khẩu.");
        isValid = false;
    } else
        if (password.trim().length < 6 && password.trim().length > 30) {
            messageTextDanger($("#form-register-input-password"), "Độ dài họ [6..30] kí tự.");
            isValid = false;
        }

    //Password confirm
    if (passwordConfirm.trim() != password.trim()) {
        messageTextDanger($("#form-register-input-password-confirm"), "Mật khẩu không trùng khớp.");
        isValid = false;
    }

    //Check condition
    if (!$("#checkbox-condition").is(":checked")) {
        $(".condition").css("color", "#c7254e");
        isValid = false;
    } else {
        $(".condition").css("color", "#888");
    }

    if (isValid) {
        $.ajax({
            url: "/Authentication/IsValidEmail",
            data: {
                email: email
            },
            type: "POST",
            success: function (response) {
                if (response.resultCode == 0) {
                    $("#form-register").submit();
                } else {
                    messageTextDanger($("#input-email"), "Email này đã được sử dụng.");
                    return;
                }
            }
        });
    }
}

// Change password
function ChangePassword() {
    var passwordOld = $("#input-password-old").val();
    var passwordNew = $("#input-password-new").val();
    var lengthOld = $("#input-password-old").val().trim().length;
    var lengthNew = $("#input-password-new").val().trim().length;
    var passwordNewConfirm = $("#input-password-new-confirm").val();
    var isValid = true;
    var options = {
        url: "/User/ChangePassword/",
        data: {
            passwordOld: passwordOld,
            passwordNew: passwordNew,
            passwordNewConfirm: passwordNewConfirm
        },
        beforeSend: beforeSend,
        type: "POST",
        success: function (data) {
            afterSend();
            switch (data.resultCode) {

                case 0://thành công
                    messageSuccess("Đổi mật khẩu thành công.");
                    window.location.replace("/");
                    break;
                default:
                    messageDanger(data.message);
                    break;
            }
        }
    }

    $('.alert, .text-danger').remove();

    if (!isValidLength(passwordOld.trim()) || lengthOld<6) {
        messageTextDanger($("#input-password-old"), "Mật khẩu từ 6 đến 20 kí tự.");
        isValid = false;
    }

    if (!isValidLength(passwordNew.trim()) || lengthNew < 6) {
        messageTextDanger($("#input-password-new"), "Mật khẩu từ 6 đến 20 kí tự.");
        isValid = false;
    }

    if (passwordNew.trim() !== passwordNewConfirm.trim()) {
        messageTextDanger($("#input-password-new-confirm"), "Mật khẩu mới và mật khẩu mới xác nhận không trùng khớp.");
        isValid = false;
    }

    if (isValid) {
        $.ajax(options);
    }
}

// Update info
function UpdateInfo() {
    var firstname = $("#input-firstname").val();
    var lastname = $("#input-lastname").val();
    var gender = $('input[name="input-gender"]:checked').val();
    var dob = $("#input-dob").val();
    var phone = $("#input-phone").val();
    var province = $("#select-provinces").val();
    var district = $("#select-districts").val();
    var ward = $("#select-wards").val();
    var address = $("#input-address").val();
    var isValid = true;

    if (dob) {
        dob = dob.split("-").reverse().join("-");
    }
    dob = dob + " 00:00:00.000";
    var options = {
        url: "/User/UpdateInfo/",
        beforeSend: beforeSend,
        data: {
            Firstname: firstname,
            Lastname: lastname,
            Gender: parseInt(gender),
            DateOfBirth: dob,
            Phone: phone,
            Province: province,
            District: district,
            Ward: ward,
            Address: address
        },
        type: "POST",
        success: function (data) {
            afterSend();
            switch (data.resultCode) {

                case 0://thành công
                    messageSuccess("Cập nhật thông tin tài khoản thành công.");
                    window.location.reload();
                    break;
                default:
                    messageDanger(data.message);
                    break;
            }
        }
    }

    $('.alert, .text-danger').remove();

    if (!firstname && firstname.trim().length == 0) {
        messageTextDanger($("#input-firstname"), "Họ không được phép để trống.");
        $(".text-danger").css("font-size", "9pt")
        isValid = false;
    }

    if (!lastname && lastname.trim().length == 0) {
        messageTextDanger($("#input-lastname"), "Tên không được phép để trống.");
        $(".text-danger").css("font-size", "9pt")
        isValid = false;
    }

    if (isValid) {
        $.ajax(options);
    }
}

// Login
function Login(url) {
    var password = $("#input-password").val();
    var email = $("#input-email").val();

    var options = {
        url: url,
        beforeSend: beforeSend,
        data: {
            email: email,
            password: password
        },
        type: "POST",
        success: function (data) {
            $('.alert, .text-danger').remove();
            afterSend();

            switch (data.resultCode) {
                case 0:
                    messageSuccess("Đăng nhập thành công.");
                    if (data.callbackUrl) {
                        window.location.replace(data.callbackUrl);
                    } else {
                        window.location.replace('/');
                    }
                    break;
                default:
                    messageDanger("E-mail hoặc mật khẩu không chính xác. Vui lòng thử lại lần nữa.");
                    break;
            }
        }
    }

    $.ajax(options);

    // Prevent form submit
    return false;
}
// Login
function Login() {
    event.preventDefault();
    $(".text-danger").remove();

    var email = $("#input-email").val();
    var password = $("#input-password").val();
    var isValid = true;
    //Email
    if (!email && !email.trim()) {
        messageTextDanger($("#input-email"), "Vui lòng nhập email.");
        isValid = false;
    } else
        if (!validateEmail(email)) {
            messageTextDanger($("#input-email"), "Email không đúng định dạng.");
            isValid = false;
        }

    //Password
    if (!password && !password.trim()) {
        messageTextDanger($("#input-password"), "Vui lòng nhập mật khẩu.");
        isValid = false;
    } else
        if (password.trim().length < 6 && password.trim().length > 30) {
            messageTextDanger($("#input-password"), "Độ dài họ [6..30] kí tự.");
            isValid = false;
        }

    var options = {
        url: "/Authentication/Login/",
        beforeSend: beforeSend,
        data: {
            email: email,
            password: password
        },
        type: "POST",
        success: function (data) {
            $('.alert, .text-danger').remove();
            afterSend();

            switch (data.resultCode) {
                case 0:
                    messageSuccess("Đăng nhập thành công.");
                    if (data.callbackUrl) {
                        window.location.replace(data.callbackUrl);
                    } else {
                        window.location.replace('/');
                    }
                    break;
                default:
                    messageDanger("E-mail hoặc mật khẩu không chính xác. Vui lòng thử lại lần nữa.");
                    break;
            }
        }
    }
    if (isValid) {
        $.ajax(options);
    }

    // Prevent form submit
    //return false;
}
// Fortgot password
function ForgotPassword() {
    var email = $("#input-email").val();
    var options = {
        url: "/Authentication/ForgotPassword/",
        data: {
            email: email
        },
        beforeSend: beforeSend,
        type: "POST",
        success: function (data) {
            afterSend();
            $('.alert, .text-danger').remove();

            switch (data.resultCode) {
                case 0:
                    $("#success-modal").modal("show");
                    setTimeout(function () {
                        window.location.replace("/");
                    }, 4000);
                    break;

                default:
                    messageDanger(data.message);
                    break;
            }
        }
    }

    $.ajax(options);
}

// Reset Password
function ResetPassword() {
    var email = $("#input-email").val();
    var token = $("#input-token").val();
    var inputNewPassword = $("#input-password-new").val();
    var inputNewPasswordConfirm = $("#input-password-new-confirm").val();
    var isValid = true;

    var options = {
        url: "/Authentication/ResetPassword/",
        data: {
            email: email,
            token: token,
            passwordNew: inputNewPassword,
            passwordNewConfirm: inputNewPasswordConfirm
        },
        beforeSend: beforeSend,
        type: "POST",
        success: function (data) {
            afterSend();

            switch (data.resultCode) {
                case 0:
                    $("#success-modal").modal("show");
                    setTimeout(function () {
                        window.location.replace("/");
                    }, 4000);
                    break;
                default:
                    messageDanger(data.message);
                    break;
            }
        }
    }
    $('.alert, .text-danger').remove();

    if (!inputNewPassword) {
        messageTextDanger($("#input-password-new"), "Vui lòng nhập mật khẩu mới.");
        $(".text-danger").css("font-size", "9pt")
        isValid = false;
    } else
        if (!inputNewPasswordConfirm) {
            messageTextDanger($("#input-password-new-confirm"), "Vui lòng nhập mật khẩu xác nhận.");
            $(".text-danger").css("font-size", "9pt")
            isValid = false;
        } else
            if (inputNewPassword.trim() != inputNewPasswordConfirm.trim()) {
                messageTextDanger($("#input-password-new-confirm"), "Mật khẩu mới và mật khẩu xác nhận không trùng khớp.");
                $(".text-danger").css("font-size", "9pt")
                isValid = false;
            }

    if (isValid) {
        $.ajax(options);
    }
}
function appendSelect(element, text, value, isSelected) {

    $(element)
          .append($("<option></option>")
           .attr("value", value).attr("selected", isSelected)
            .text(text));
}

// Get Province
function getAllProvinces(provinceSelected) {
    var options = {
        url: "/Location/GetAllProvinces/",
        beforeSend: beforeSend,
        type: "GET",
        success: function (response) {
            afterSend();
            var provinces = response.provinces;

            if (provinces && provinces.length > 0) {
                $("#select-provinces").empty();
                $("#select-provinces").append($("<option></option>"));
                $.each(provinces, function (key, value) {

                    if (provinceSelected && provinceSelected == value.ProvinceId) {
                        console.log(provinceSelected, value.ProvinceId);
                        appendSelect($("#select-provinces"), value.Name, value.ProvinceId, true);
                    } else {
                        appendSelect($("#select-provinces"), value.Name, value.ProvinceId, false);
                    }
                });

            }

            $("#select-provinces").selectmenu("refresh");
        }
    }

    $.ajax(options);
}

//GetDistrictsByProvince
function getDistrictsByProvince(provinceId, districtIdSelected) {

    var options = {
        url: "/Location/GetDistrictsByProvince/",
        beforeSend: beforeSend,
        type: "GET",
        data: {
            provinceId: provinceId
        },
        success: function (response) {
            afterSend();
            var districts = response.districts;
            $("#select-districts").empty();
            $("#select-districts").append($("<option></option>"));
            if (districts && districts.length > 0) {

                $.each(districts, function (key, value) {
                    if (districtIdSelected && districtIdSelected == value.DistrictId) {
                        appendSelect($("#select-districts"), value.Name, value.DistrictId, true);
                    } else {
                        appendSelect($("#select-districts"), value.Name, value.DistrictId, false);
                    }
                });
            }

            $("#select-districts").selectmenu("refresh");
        }
    }

    $.ajax(options);
}

//GetWardsByDistrict
function getWardsByDistrict(districtId, wardIdSelected) {

    var options = {
        url: "/Location/GetWardsByDistrict/",
        beforeSend: beforeSend,
        type: "GET",
        data: {
            districtId: districtId
        },
        success: function (response) {
            afterSend();
            var wards = response.wards;

            $("#select-wards").empty();
            $("#select-wards").append($("<option></option>"));
            if (wards && wards.length > 0) {

                $.each(wards, function (key, value) {
                    if (wardIdSelected && wardIdSelected == value.WardId) {
                        appendSelect($("#select-wards"), value.Name, value.WardId, true);
                    } else {
                        appendSelect($("#select-wards"), value.Name, value.WardId, false);
                    }
                });
            } else { }

            $("#select-wards").selectmenu("refresh");
        }
    }

    $.ajax(options);
}

// BeforeSend - AfterSend request
function beforeSend() {
    $("input").attr("disabled", true);
    $("button").attr("disabled", true);
    $("select").attr("disabled", true);
    $("texarea").attr("disabled", true);
}

function afterSend() {
    $("input").attr("disabled", false);
    $("button").attr("disabled", false);
    $("select").attr("disabled", false);
    $("texarea").attr("disabled", false);
}

//Show message noty Success
function messageSuccess(message) {
    CmsNotification.successMessage(message);
}

// Show message noty Fail
function messageDanger(message) {
    CmsNotification.dangerMessage(message);
}

function messageTextDanger(element, message) {
    $(element).after('<span style="color: #c7254e; font-weight: bold; font-size: small;" class="text-danger">' + message + '</span>');
}

// Vadiation
function isValidLength(str) {
    return (str && str.length >= 1 && str.length < 20);
}

function reverse(s) {
    var o = '';
    for (var i = s.length - 1; i >= 0; i--)
        o += s[i];
    return o;
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

CurrencyConverter = (function () {
    var actions = {};

    actions.toVietnamDong = function (price) {
        var unit = " VND";

        if (!price || isNaN(price)) {
            return "0,000" + unit;
        }

        return numeral(price).format("0,000") + unit;
    }

    return actions;
})();

CmsNotification = (function () {
    var actions = {};

    var NOTIFICATION_TIME_OUT = 3 * 1000;
    var NOTIFICATION_FADE_OUT = 1 * 1000;

    //Show message notification Success
    actions.successMessage = function messageSuccess(message) {
        var alertObject = $('<div class="alert alert-success main-alert-panel"><i class="fa fa-check-circle"></i> ' + message + '<button type="button" class="close" data-dismiss="alert">&times;</button></div>');

        $('#content').parent().before($(alertObject))
        setTimeout(function () {
            $(alertObject).fadeOut(NOTIFICATION_FADE_OUT);

            setTimeout(function () {
                $(alertObject).remove();
            }, NOTIFICATION_FADE_OUT);
        }, NOTIFICATION_TIME_OUT);
    }

    // Show message notification Fail
    actions.dangerMessage = function messageDanger(message) {
        var alertObject = $('<div class="alert alert-danger main-alert-panel"><i class="fa fa-exclamation-circle"></i> ' + message + '<button type="button" class="close" data-dismiss="alert">&times;</button></div>');

        $('#content').parent().before($(alertObject));
        setTimeout(function () {
            $(alertObject).fadeOut(NOTIFICATION_FADE_OUT);

            setTimeout(function () {
                $(alertObject).remove();
            }, NOTIFICATION_FADE_OUT);
        }, NOTIFICATION_TIME_OUT);
    }

    return actions;
})();

(function () {

    var provinceSelected = $("#input-province-selected").val();
    var districtSelected = $("#input-district-selected").val();
    var wardSelected = $("#input-ward-selected").val();

    $(document).ready(function () {
        $("#top-menu-input-search").on('keypress', function (e) {
            if (e.which === 13) {
                var query = $(this).val();
                window.location.href = "/Product/Category?query=" + query;
            }
        });

        //Province
        getAllProvinces(provinceSelected);

        //District
        if (provinceSelected) {
            getDistrictsByProvince(provinceSelected, districtSelected);

            // Ward
            if (districtSelected) {
                getWardsByDistrict(districtSelected, wardSelected);
            }
        }

        //Change Province;
        $("#select-provinces").selectmenu({
            change: function (event, ui) {
                var provinceId = $("#select-provinces :selected").val();
                getDistrictsByProvince(provinceId);
                //Reset ward
                getWardsByDistrict("000");
            }
        });
        //Change district
        $("#select-districts").selectmenu({
            change: function (event, ui) {
                var districtId = $("#select-districts :selected").val();
                getWardsByDistrict(districtId);
            }
        });
    })
})();
