var ValidatorUtil = function() {
    var actions = {};

    actions.UpdateInputFormUi = function updateUi(isInvalid, name, message) {
        var validatorClassName = "parsley-validated parsley-error";
        var div = $("#validator-" + name.trim().toLowerCase() + "-invalid");
        var divInvalidMessage = $("#validator-" + name.trim().toLowerCase() + "-invalid-message");
        var divTarget = $("[name='" + name.trim() + "']");

        if (div && divInvalidMessage && divTarget) {
            if (isInvalid) {
                $(div).show();
                $(divInvalidMessage).text(message);
                $(divTarget).addClass(validatorClassName);
            } else {
                $(div).hide();
                $(divInvalidMessage).text("");
                $(divTarget).removeClass(validatorClassName);
            }
        }
    }

    return actions;
}

var NotificationUtil = (function() {
    var defaults = {
        position: 'top-right', // top-left, top-right, bottom-left, or bottom-right
        speed: 'fast', // animations: fast, slow, or integer
        allowdupes: false, // true or false
        autoclose: 5 * 1000,  // delay in milliseconds. Set to 0 to remain open.
        classList: '' // arbitrary list of classes. Suggestions: success, warning, important, or info. Defaults to ''.
    };

    var actions = {};

    actions.successMessage = function(message) {
        $.stickyNote(message, $.extend({}, defaults, { classList: "stickyNote-success" }));
    }

    actions.warningMessage = function(message) {
        $.stickyNote(message, $.extend({}, defaults, { classList: "stickyNote-warning" }));
    }

    return actions;
})();

var ValidateMessages = {
    Validate: {
        Category: {
            Name: {
                NameEmpty: "Tên danh mục không được phép để trống.",
                NameDuplicated: "Tên danh mục đã tồn tại. Vui lòng chọn tên khác."
            }
        },
        Product: {
            Name: {
                NameEmpty: "Tên sản phẩm không được phép để trống.",
                NameDuplicated: "Tên sản phẩm đã tồn tại. Vui lòng chọn một tên khác."
            },
            Price: {
                Invalid: "Giá không được phép nhỏ hơn 0."
            },
            Quantity: {
                Invalid: "Số lượng không được phép nhỏ hơn 0."
            },
            Category: {
                Invalid: "Vui lòng chọn danh mục."
            },
            ShortDescription: {
                Empty: "Vui lòng nhập mô tả ngắn."
            }
        }
    }
}

var CurrencyConverter = (function() {
    var actions = {};

    actions.toVietNamDong = function (price) {
        if (!price) {
            return "0 đ";
        }
//        return ((price) + "").replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "1,") + " đ";

        return numeral(price).format("0,000") + " đ";
    }

    return actions;
})();

var DateTimeUtils = (function() {
    var actions = {};

    actions.toVietnameseFormat = function (dateStr, isIncludeTime) {
        var self = this;
        var date = new Date(dateStr);
        var str = self.roundDecimal(date.getDate()) + "/" + self.roundDecimal((date.getMonth() + 1)) + "/" + self.roundDecimal(date.getFullYear());

        if (isIncludeTime) {
            str += " " + self.roundDecimal(date.getHours()) + ":" + self.roundDecimal(date.getMinutes()) + ":" + self.roundDecimal(date.getSeconds());
        }

        return str;
    }

    actions.roundDecimal = function(number) {
        var str = number.toString();

        if (str.length >= 2) {
            return str;
        } else {
            return "0" + str;
        }
    }

    return actions;
})();

var SettingManager = (function() {
    var actions = {};

    actions.init = function() {
        $("#settingActionSelect").select2({
            allowClear: true,
            placeholder: "Select..."
        });

        $("#settingActionSelect")
            .on("select2-selecting",
                function(e) {
                    window.location.href = e.val;
                });
    }

    return actions;
})();

SettingManager.init();