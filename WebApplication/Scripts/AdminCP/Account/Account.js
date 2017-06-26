﻿//***********************************
//         LOAD ACCOUNT             *
//***********************************
$(document).ready(function () {
    if ($(".select2").length > 0) {
        $(".select2").select2({
            allowClear: true,
            placeholder: "Select..."
        })
    }
    //set form submit
    var options = {
        target: "#result-search-account",
        beforeSubmit: showRequest,
        success: showResponse,
        type: 'post',
        resetForm: false
    };

    $('#form-search').ajaxForm(options); // id of the form we wish to submit
    $("#form-search").submit();
})

function showRequest(formData, jqForm, options) {
    $("#result-search-account").empty();
    $("#form-search :input").attr("disabled", true);//lock input
    $("#form-search button").attr("disabled", true);//lock button
}

function showResponse(responseText, statusText, xhr, $form) {
    $("#form-search :input").attr("disabled", false);//lock input
    $("#form-search button").attr("disabled", false);//lock button
}

function Search_Click() {
    $("#form-search").submit();
}

//***********************************
//         ACTIVE/INACTIVE          *
//***********************************
var status_active = 1;
var status_inactive = 2;
function activeOrDeactiveAccount(element) {
    var email = $(element).data("email");
    var status = $(element).data("status");
    var text = "";
    if (status == status_active) {
        text = "Bạn muốn kích hoạt tài khoản [" + email + "] ?";
    } else if (status == status_inactive) {
        text = "Bạn muốn định chỉ hoạt động tài khoản [" + email + "] ?";
    }
    var options = {
        url: "/Admin/UpdateStatus",
        data: {
            email: email,
            status: status
        },
        beforeSubmit: showRequest,
        success: function (response) {
            if (response.resultCode == 0) {
                messageSuccess("Thao tác thành công!");
            } else {
                messageDanger(response.message);
            }

        },
        type: 'post'
    };

    bootbox.dialog({
        message: text,
        title: "Đình chỉ hoạt động tài khoản",
        buttons: {
            danger: {
                label: "Hủy",
                className: "btn-default",
                callback: function () {
                }
            },
            success: {
                label: "Đồng ý",
                className: "btn-default",
                callback: function () {
                    $.ajax(options);
                }
            },
        }
    });
}

//***********************************
//         CHANGE PASSWORD          *
//***********************************
function showModalChangePassword(element) {
    // Reset value 
    $("#input-password-new").val("");
    $("#input-password-new-confirm").val("");
    // Set email value
    var email = $(element).data("email");
    $("#email-modal-title").text(email);
    $("#email-change-password").val(email)
    // Show modal
    $("#change-password-modal").modal("show");
}

function changePassword() {
    // Get data
    var inputOldPassword= $("#input-password-old").val();
    var inputNewPassword = $("#input-password-new").val();
    var inputNewPasswordConfirm = $("#input-password-new-confirm").val();
    var email = $("#email-change-password").val();
    var isValid = true;
    $('.alert, .text-danger').remove();
    // Validation
    $("#input-password-old").blur(function () {
        if (inputOldPassword.lengh() < 6) {
            messageTextDanger($("#input-password-old"), "Mật khẩu không được nhỏ hơn 6 ký tự");
            $(".text-danger").css("font-size", "9pt");
            isValid = false;
        }
    });
    $("#input-password-new").blur(function () {
        if (inputNewPassword.lengh() < 6) {
            messageTextDanger($("#input-password-new"), "Mật khẩu không được nhỏ hơn 6 ký tự");
            $(".text-danger").css("font-size", "9pt");
            isValid = false;
        }
    });
    if (!inputNewPassword) {
        messageTextDanger($("#input-password-new"), "Vui lòng nhập mật khẩu mới");
        $(".text-danger").css("font-size", "9pt")
        isValid = false;
    } else
        if (!isValidLength(inputNewPassword.trim())) {
            messageTextDanger($("#input-password-new"), "Mật khẩu từ  6 đền 20 kí tự !")
            isValid = false;
        } else
            if (!inputNewPasswordConfirm) {
                messageTextDanger($("#input-password-new-confirm"), "Vui lòng nhập mật khẩu xác nhận");
                $(".text-danger").css("font-size", "9pt")
                isValid = false;
            } else
                if (inputNewPassword.trim() != inputNewPasswordConfirm.trim()) {
                    messageTextDanger($("#input-password-new-confirm"), "Mật khẩu mới và mật khẩu xác nhận không trùng khớp");
                    $(".text-danger").css("font-size", "9pt")
                    isValid = false;
                }


    if (isValid) {// Valid
        var options = {
            url: "/Admin/ChangePassword/",
            data: {
                email: email,
                passwordNew: inputNewPassword,
                passwordNewConfirm: inputNewPasswordConfirm
            },
            beforeSend: beforeSend,
            type: "POST",
            success: function (data) {
                console.log(data)
                afterSend();
                switch (data.resultCode) {
                    case 0:// Success
                        messageSuccess("Đổi mật khẩu thành công");

                        // Close modal
                        $("#change-password-modal").modal("hide");
                        $(".modal-backdrop").remove();
                        break;
                    default: // Fail
                        messageDanger(data.message);
                        break;
                }
            }
        }
        // Change password
        $.ajax(options);
    }
}


//***********************************
//         UPDATE INFO              *
//***********************************

function showModalUpdateInfo(element) {
    // Init data 
    $("#input-dob").datepicker({
        format: "dd-mm-yyyy"
    });
    // Set email
    var email = $(element).data("email");
    $("#email-modal-title-update-info").text(email);
    $("#email-update-info").val(email);

    // Set data 
    var options = {
        url: "/Admin/GetAccount/",
        beforeSend: beforeSend,
        data: {
            email: email
        },
        type: "POST",
        success: function (response) {
            afterSend();
            console.log(response);
            if (response && response.resultCode == 0) {
                // Set data to modal
                var user = response.data;
                if (user != null) {

                    $("#update-info-email").text(user.Email);
                    $("#input-firstname").val(user.Firstname);
                    $("#input-lastname").val(user.Lastname);

                    $('[name="input-gender"]').each(function () {
                        if ($(this).val() == user.Gender)
                            $(this).attr("checked", true);
                        else
                            $(this).attr("checked", false);
                    })
                    $("#input-dob").val(getDateFormatDDMMYYYY(user.DateOfBirth, "-"));
                    $("#input-phone").val(user.Phone);
                    $("#input-address").val(user.Address);
                    //Show modal
                    $("#update-info-modal").modal("show");
                }

            }
        }
    }

    $.ajax(options);
}

// Update info
function UpdateInfo() {
    var firstname = $("#input-firstname").val();
    var lastname = $("#input-lastname").val();
    var gender = $('input[name="input-gender"]:checked').val();
    var dob = $("#input-dob").val();
    var phone = $("#input-phone").val();
    var address = $("#input-address").val();
    var email = $("#email-update-info").val();

    var isValid = true;

    if (dob) {
        dob = dob.split("-").reverse().join("-");
    }
    dob = dob + " 00:00:00.000";
    var options = {
        url: "/Admin/UpdateInfo/",
        beforeSend: beforeSend,
        data: {
            Firstname: firstname,
            Lastname: lastname,
            Gender: parseInt(gender),
            DateOfBirth: dob,
            Phone: phone,
            Address: address,
            Email: email
        },
        type: "POST",
        success: function (data) {
            afterSend();
            switch (data.resultCode) {

                case 0://thành công
                    messageSuccess("Cập nhật thông tin tài khoản thành công");
                    // Close modal
                    $("#update-info-modal").modal("hide");
                    $(".modal-backdrop").remove();
                    break;
                default:
                    messageDanger(data.message);
                    break;
            }
        }
    }

    $('.alert, .text-danger').remove();

    if (!firstname && firstname.trim().length == 0) {
        messageTextDanger($("#input-firstname"), "Họ không được phép để trống");
        $(".text-danger").css("font-size", "9pt");
        isValid = false;
    }

    if (!lastname && lastname.trim().length == 0) {
        messageTextDanger($("#input-lastname"), "Tên không được phép để trống");
        $(".text-danger").css("font-size", "9pt");
        isValid = false;
    }

    if (isValid) {
        $.ajax(options);
    }
}

// Effect
//"/Date(708109200000)/" To DD-MM-YYYY
function getDateFormatDDMMYYYY(date, delimiter) {
    var date = new Date(parseInt(date.substr(6)));
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day;
    }
    var month = (date.getMonth() + 1)
    if (month < 10) {
        month = "0" + month;
    }
    var year = date.getFullYear();

    return day + delimiter + month + delimiter + year;
}

function isValidLength(str) {
    return (str && str.length > 1 && str.length < 250);
}

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


var defaults = {
    position: 'top-right', // top-left, top-right, bottom-left, or bottom-right
    speed: 'fast', // animations: fast, slow, or integer
    allowdupes: false, // true or false
    autoclose: 3000,  // delay in milliseconds. Set to 0 to remain open.
    classList: '' // arbitrary list of classes. Suggestions: success, warning, important, or info. Defaults to ''.
};

//Show message noty Success
function messageSuccess(message) {
    $('.sticky').click(function () {
        $.stickyNote(message, $.extend({}, defaults, { classList: 'stickyNote-success' }), function callback(r) { })
    });
    $('.sticky').trigger("click");
}

// Show message noty Fail
function messageDanger(message) {
    $('.sticky').click(function () {
        $.stickyNote(message, $.extend({}, defaults, { classList: 'stickyNote-warning' }), function callback(r) { })
    });
    $('.sticky').trigger("click");
}

function messageTextDanger(element, message) {
    $(element).after('<div class="text-danger"> ' + message + '</div>');
}
