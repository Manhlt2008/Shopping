//#region [Event Hanlders]

$("#chkListAllProducts").change(function () {
    if (this.checked) {
        window.location = $(this).attr("data-url");
    }
});

var currentCategory = $("#currentCategory").val();

if ($("#category_select").length) {
    $("#category_select").select2({
        allowClear: true,
        placeholder: "Select..."
    }).on("change", function (e) {
        window.location = e.val;
    }).select2("data", { text: currentCategory });
}

function txtDelete(name) {
    return (confirm("Are you sure to delete " + name + "? This action cannot be undo."));
}

function updateQuantityProduct(element) {
    //Clear noty
    $(".text-danger").empty();
    var productName = $(element).data("product-name");
    var productionId = $(element).data("id");
    var quantity = $(element).data("product-quantity");

    bootbox.dialog({
        autoOpen: false,
        autoClose: false,
        modal: true,
        closeOnEscape: false,
        close: function () {
            return false;
        },
        beforeClose: function (event, ui) { return false; },
        title: "Cập nhật số lượng cho sản phẩm [" + productName + "]",
        message: "<div id='modal-dialog'>" + $(".modal-update-quantity-product").html() + "</div><script>$('[name=input-quantity]').val(" + quantity + ");</script>",
        buttons: {
            success: {
                label: "Cập nhật",
                className: "btn-success",
                callback: function (event) {

                    var quantityInput = $("#modal-dialog [name=input-quantity]").val();
                    if (quantityInput == null || quantity.length == 0) {
                        messageTextDanger($("#modal-dialog [name=input-quantity]"), "Vui lòng nhập số lượng")
                        return;
                    }
                    if (parseInt(quantityInput) == null || parseInt(quantityInput) == undefined) {
                        messageTextDanger($("#modal-dialog [name=input-quantity]"), "Kiểu dữ liệu không hợp lệ");
                        return;
                    }
                    if (parseInt(quantityInput) < 0) {
                        messageTextDanger($("#modal-dialog [name=input-quantity]"), "Số lượng không đc âm");
                        return;
                    }
                    if (parseInt(quantityInput) > 999999) {
                        messageTextDanger($("#modal-dialog [name=input-quantity]"), "Số lượng quá lớn");
                        return;
                    }

                    var options = {
                        url: "/Product/UpdateQuantity",
                        data: {
                            id: parseInt(productionId),
                            quantity: parseInt(quantityInput)
                        },
                        beforeSubmit: showRequest,
                        success: function (response) {
                            if (response.resultCode == 0) {
                                messageSuccess("Thao tác thành công!");
                                window.location.reload();
                            } else {
                                messageDanger(response.message);
                            }
                        },
                        type: 'post'
                    };

                    $.ajax(options);
                }
            },
            cancel: {
                label: "Hủy",
                className: "btn-default",
                callback: function () {
                }
            }
        }
    }
        );
}

function showModalSetHomepage(element) {
    var productName = $(element).data("product-name");
    var productId = $(element).data("product-id");
    $("#set-homepage-product-name").text(productName);
    $("#product-id").val(productId);
    //Get homepage
    var options = {
        url: "/HomePage/GetHomePageProduct",
        data: {
            productId: productId
        },
        beforeSubmit: showRequest,
        success: function (response) {
            $("#set-homepage-modal #content-set-homepage").empty();
            $("#set-homepage-modal #content-set-homepage").html(response);

            if ($(".datepicker") && $(".datepicker").length) {
                $(".datepicker").datepicker({
                    format: "dd-mm-yyyy"
                });

                var Ids = [];

                $(".datepicker").each(function () {
                    var id = $(this).data("id");
                    if (Ids.indexOf(id) == -1) {
                        Ids.push(id);
                        if (($('#input-dpStart-' + id).length) && ($('#input-dpEnd-' + id).length)) {
                            $('#input-dpStart-' + id).datepicker().on('changeDate', function (e) {
                                $('#input-dpEnd-' + id).datepicker('setStartDate', e.date);
                            });

                            $('#input-dpEnd-' + id).datepicker().on('changeDate', function (e) {
                                $('#input-dpStart-' + id).datepicker('setEndDate', e.date)
                            });
                        }
                    }
                })
            }
        },
        type: 'post'
    };

    $.ajax(options);
    $("#set-homepage-modal").modal("show");
}

function setHomePage(element) {
    //Get data
    var homePages = [];
    $("legend").each(function () {

        var typeHomepageId = $(this).data("type-homepage-id");
        var homePageId = $("#homepage-id-" + typeHomepageId).val();
        var iOrder = $("#input-iorder-" + typeHomepageId).val();
        var status = 2;

        if ($("#input-status-" + typeHomepageId).is(":checked")) {
            status = 1;
        }
        var productId = $("#product-id").val();

        var homePage = {
            Id: parseInt(homePageId),
            ProductId: parseInt(productId),
            TypeHomePageId: parseInt(typeHomepageId),
            TypeHomePageName: "",
            Status: parseInt(status)
        }

        homePages.push(homePage);
    })

    if (homePages.length > 0) {
        var options = {
            url: "/HomePage/SaveListHomePages",
            data: {
                homepages: JSON.stringify(homePages)
            },
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                console.log(response);
                if (response.resultCode == 0) {
                    messageSuccess("Cập nhật trang chủ thành công!");
                    window.location.reload();
                } else {
                    messageDanger(response.message);
                }
            }
        };

        $.ajax(options);
    }
}

function showRequest(formData, jqForm, options) {
    $("#result-search-account").empty();
    $("#form-search :input").attr("disabled", true);//lock input
    $("#form-search button").attr("disabled", true);//lock button
}

function messageTextDanger(element, message) {
    $(element).after('<div class="text-danger"> ' + message + '</div>');
}
function validateNumber(e) {
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
}

//Show message noty Success
function messageSuccess(message) {
    $('.sticky').click(function () {
        $.stickyNote(message, $.extend({}, defaults, { classList: 'stickyNote-success' }), callback)
    });
    $('.sticky').trigger("click");
}

// Show message noty Fail
function messageDanger(message) {
    $('.sticky').click(function () {
        $.stickyNote(message, $.extend({}, defaults, { classList: 'stickyNote-warning' }), callback)
    });
    $('.sticky').trigger("click");
}
//#endregion