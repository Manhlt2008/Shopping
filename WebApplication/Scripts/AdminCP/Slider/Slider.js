var SliderController = (function () {
    var actions = {};

    var url = {};

    actions.init = function (options) {
        var self = this;

        if (!options) {
            options = {};
        }

        url = {
            listBannerActive:   options.url.listBannerActive,
            listBannerInActive: options.url.listBannerInActive,
            listSliderActive:   options.url.listSliderActive,
            listSliderInActive: options.url.listSliderInActive,
            activeOrInActive:   options.url.activeOrInActive,
            inactiveSlider:     options.url.inactiveSlider,
            increaseIOrder:     options.url.increaseIOrder,
            decreaseIOrder:     options.url.decreaseIOrder
        }
    }

    actions.listBannerSliderActive = function (url) {
        var options = {
            url: url,
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                $("#result-table-active").empty();
                $("#result-table-active").html(response);
            }
        }

        $.ajax(options);
    }

    actions.listBannerSliderInActive = function (url) {
        var options = {
            url: url,
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                $("#result-table-inactive").empty();
                $("#result-table-inactive").html(response);
            }
        }

        $.ajax(options);
    }

    actions.saveSlider = function (actionType) {
        //Clear validation
        $(".text-danger").remove();

        //Element
        var elementType = $("[name='Type']");
        var elementTypeSelected = $("[name='Type'] :selected");
        var elementTitle = $("[name='Title']");
        var elementDescription = $("[name='Description']");
        var elementImage = $("[name='Image']");
        var elementGoToLink = $("[name='GoToLink']");

        //GetValue
        var type = elementTypeSelected.val();
        var title = elementTitle.val();
        var description = CKEDITOR.instances.editor.getData();
        var image = elementImage.val();
        var goToLink = elementGoToLink.val();

        var isValid = true;
        //Validation
        if (!type && type.length == 0) {
            messageTextDanger(elementType, "Vui lòng chọn loại hiển thị!");
            isValid = false;
        }

        /*if (!title && title.length == 0) {
            messageTextDanger(elementTitle, "Vui lòng nhập tiêu đề!");
            isValid = false;
        }

        if (!description && description.length == 0) {
            messageTextDanger(elementDescription, "Vui lòng nhập mô tả!");
            isValid = false;
        }*/

        if (!goToLink && goToLink.length == 0) {
            messageTextDanger(elementGoToLink, "Vui lòng nhập đường dẫn!");
            isValid = false;
        }

        if (actionType == 'Create') {
            if (!image && image.length == 0) {
                messageTextDanger(elementImage, "Vui lòng chọn một hình ảnh!");
                isValid = false;
            }
        }

        elementDescription.val(CKEDITOR.instances.editor.document.getBody().getHtml());

        if (isValid) {
            $("#createSliderOrBannerForm").submit();
        }
    }
    // Banner Active
    actions.listBannerActive = function () {
        actions.listBannerSliderActive(url.listBannerActive);
    }
    // Banner InActive
    actions.listBannerInActive = function () {
        actions.listBannerSliderInActive(url.listBannerInActive);
    }
    // Slider Active
    actions.listSliderActive = function () {
        actions.listBannerSliderActive(url.listSliderActive);
    }
    //Slider In Active
    actions.listSliderInActive = function () {
        actions.listBannerSliderInActive(url.listSliderInActive);
    }

    actions.activeOrInActive = function (element) {
        var status_active = 1;
        var status_inactive = 2;

        var sliderId = $(element).data("id");
        var title = $(element).data("title");
        var status = $(element).data("status");
        var text = "";
        if (status == status_active) {
            text = "Bạn muốn kích hoạt Slider/Banner [" + title + "] ?";
        } else if (status == status_inactive) {
            text = "Bạn muốn tạm ngưng Slider/Banner [" + title + "] ?";
        }
        var options = {
            url: url.activeOrInActive,
            data: {
                sliderId: parseInt(sliderId),
                status: parseInt(status)
            },
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                console.log(response)
                if (response.resultCode == 0) {
                    messageSuccess("Thao tác thành công!");
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                } else {
                    messageDanger(response.message);
                }

            },
        };

        bootbox.dialog({
            message: text,
            title: "Kích hoạt hay tạm ngưng Slider/Banner",
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

    actions.IncreaseIOrder = function (element) {
        var sliderId = $(element).attr('data-slider-id');

        var options = {
            url: url.increaseIOrder,
            data: {
                sliderId: parseInt(sliderId)
            },
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                console.log(response);
                showResponse();
                if (response.resultCode == 0) {
                    window.location.reload();
                }
            }
        }

        $.ajax(options);
    }

    actions.DecreaseIOrder = function (element) {
        var sliderId = $(element).attr('data-slider-id');

        var options = {
            url: url.decreaseIOrder,
            data: {
                sliderId: parseInt(sliderId)
            },
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                console.log(response);
                showResponse();
                if (response.resultCode == 0) {
                    window.location.reload();
                }
            }
        }

        $.ajax(options);
    }

    return actions;
})();

function showRequest(formData, jqForm, options) {
    $("input").attr("disabled", true);//lock input
    $("button").attr("disabled", true);//lock button
    $("select").attr("disabled", true);//lock select
}

function showResponse(responseText, statusText, xhr, $form) {
    $("input").attr("disabled", false);//lock input
    $("button").attr("disabled", false);//lock button
    $("select").attr("disabled", false);//lock select
}



// Effect
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
