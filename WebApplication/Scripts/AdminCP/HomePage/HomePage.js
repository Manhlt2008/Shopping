var HomePageController = (function () {
    var actions = {};

    var url = {};

    actions.init = function (options) {
        var self = this;

        donetypingRegister();

        if (!options) {
            options = {};
        }

        url = {
            findAllByProductName: options.url.findAllByProductName,
            findHomePageByTypeIdAndProductId: options.url.findHomePageByTypeIdAndProductId,
            saveHomePage: options.url.saveHomePage,
            increaseIOrder: options.url.increaseIOrder,
            decreaseIOrder: options.url.decreaseIOrder
        }

        $(".auto").autoNumeric("init",
        {
            mRound: "C",
            vMin: 0
        });

        if ($('#addProductDropdownList').length) {
            $('#addProductDropdownList').select2();
        }

        registerSelectTypeEvent();
    }

    actions.IncreaseIOrder = function (element) {
        var homePageId = $(element).attr('data-homepage-id');

        var options = {
            url: url.increaseIOrder,
            data: {
                homePageId: parseInt(homePageId)
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
        var homePageId = $(element).attr('data-homepage-id');

        var options = {
            url: url.decreaseIOrder,
            data: {
                homePageId: parseInt(homePageId)
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

    actions.addMoreProduct = function () {
        var productId = $('#addProductDropdownList').select2().val();
        var typeHomePageName = $("#button-add-homepage").attr("data-type");
        var typeHomePageId = $("#button-add-homepage").attr("data-type-id");

        var homePage = {
            Id: 0,
            ProductId: parseInt(productId),
            ProductName : "",
            IOrder: 0,
            TypeHomePageId: parseInt(typeHomePageId),
            TypeHomePageName: "",
            Status: 1
        }
        console.log(homePage);
        var options = {
            url: url.findHomePageByTypeIdAndProductId,
            data: {
                homepage: JSON.stringify(homePage)
            },
            beforeSubmit: showRequest,
            type: 'post',
            success: function (response) {
                console.log(response);
                showResponse();
                $(".text-danger").remove();
                if (response.data != null) {
                    messageTextDanger($("#addProductDropdownList"), "Sản phẩm này đã có trong danh sách!");
                    return;
                } else {
                    $.ajax({
                        url: url.saveHomePage,
                        data: {
                            homepage: JSON.stringify(homePage)
                        },
                        beforeSubmit: showRequest,
                        type: 'post',
                        success: function (response) {
                            showResponse();
                            console.log(response);
                            if (response.resultCode == 0) {
                                window.location.reload();
                            } else {
                            }
                        }
                    });
                }
            }
        }

        $.ajax(options);
    }

    var donetypingRegister = function () {
        ; (function ($) {
            $.fn.extend({
                donetyping: function (callback, timeout) {
                    timeout = timeout || 1e3; // 1 second default timeout
                    var timeoutReference,
                        doneTyping = function (el) {
                            if (!timeoutReference) return;
                            timeoutReference = null;
                            callback.call(el);
                        };
                    return this.each(function (i, el) {
                        var $el = $(el);
                        // Chrome Fix (Use keyup over keypress to detect backspace)
                        // thank you @palerdot
                        $el.is(':input') && $el.on('keyup keypress paste', function (e) {
                            // This catches the backspace button in chrome, but also prevents
                            // the event from triggering too preemptively. Without this line,
                            // using tab/shift+tab will make the focused element fire the callback.
                            if (e.type == 'keyup' && e.keyCode != 8) return;

                            // Check if timeout has been set. If it has, "reset" the clock and
                            // start over again.
                            if (timeoutReference) clearTimeout(timeoutReference);
                            timeoutReference = setTimeout(function () {
                                // if we made it here, our timeout has elapsed. Fire the
                                // callback
                                doneTyping(el);
                            }, timeout);
                        }).on('blur', function () {
                            // If we can, fire the event since we're leaving the field
                            doneTyping(el);
                        });
                    });
                }
            });
        })(jQuery);

    }

    var registerSelectTypeEvent = function () {
        $(".select2-input").donetyping(function () {
            selectQueryEvents(this);
        });
    }

    var selectQueryEvents = function (e) {
        var query = $(e).val();

        if (query != undefined && query.trim().length > 0) {
            $('#addProductDropdownList').html("");

            $.ajax({
                type: "POST",
                url: url.findAllByProductName,
                data: { "query": query },
                dataType: "json",
                success: function (data) {
                    products = data;
                    for (var i = 0; i < data.length; i++) {
                        var obj = data[i];
                        $('#addProductDropdownList')
                            .append($('<option value="' + obj.Id + '">' + obj.Name + '</option>'));
                    }

                    $('#addProductDropdownList').select2().select2("open");
                }
            });

        }
    }

    return actions;
})();

function messageTextDanger(element, message) {
    $(element).after('<div class="text-danger"> ' + message + '</div>');
}

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
