var ProductViewManager = function (options) {
    if (!options || !options.id) {
        window.location = "/404";
        return;
    }

    var url = {
        deleteUrl: options.url.deleteUrl
    };

    options = $.extend({
        name: "",
        quantity: 1
    }, options);

    options.quantity = options.quantity < 0 ? 0 : options.quantity;

    this.product = options;
}

ProductViewManager.prototype.validations = function () {
    var validator = {};
    var self = this;

    validator.quantity = function (e) {
        var quantity = parseInt($('#input-quantity').val());
        var isValid = true;

        if (!quantity || quantity <= 0) {
            messageDanger(ShopMessage.product.view.quantityRequire);
            quantity = 1;
            $(e).focus();

            isValid = false;
        } else if (quantity > self.product.quantity) {
            messageDanger(ShopMessage.product.view.quantityTooMuch(self.product.quantity));
            quantity = self.product.quantity;
            $(e).focus();
            isValid = false;

        }

        $(e).val(quantity);
        return isValid;
    }

    return validator;
};

ProductViewManager.prototype.initEvents = function () {
    var self = this;
    $("#input-quantity").on("change", function (e) {
        self.validations().quantity();
    });

    $("#button-cart").on("click", function (e) {
        if (self.validations().quantity()) {
            const productId = $("#product_id").val();
            const productName = $("#product_name").val();
            CartController.add({ id: productId, quantity: 1, name: productName });
        }
    });

    $("#button-review").on("click", function (e) {
        self.writeComment();
    });
}

ProductViewManager.prototype.writeComment = function () {
    var orderCode = $("#txtOrderCode").val();
    var review = $("#input-review").val();
    var self = this;

    if (!orderCode || orderCode.trim() == "") {
        messageDanger(ShopMessage.product.view.provideOrderCode);
        $("#txtOrderCode").focus();
        return;
    }

    if (!review || review.length < 25) {
        messageDanger(ShopMessage.product.view.provideReview);
        $("#input-review").focus();
        return;
    }

    var checkValidCode = new Promise(function (resolve, reject) {
        $.ajax({
            type: "POST",
            url: "/Product/IsAllowWriteComment",
            data: { "code": orderCode, "productId": self.product.id },
            dataType: "json",
            success: function (data) {
                resolve(data);

            },
            error: function (data, textStatus, errorThrown) {
                reject();
            }
        });
    });

    checkValidCode.then(function (data) {
        if (!data) {
            messageDanger("Connection Fail");
            $("#txtOrderCode").focus();
            return;
        }

        if (!data.IsAllow) {
            messageDanger(ShopMessage.product.view.provideInvalidOrderCode);
            $("#txtOrderCode").focus();
            return;
        }

        var dataObj = {
            ReviewMessage: review.trim(),
            Code: orderCode,
            ProductId: self.product.id
        }

        $.ajax({
            type: "POST",
            url: "/Product/WriteComment",
            data: dataObj,
            dataType: "json",
            success: function (res) {
                var submitReviewMessage = $("#submitReviewMessage");
                $(submitReviewMessage).show();
                $(submitReviewMessage).removeClass();

                if (!res || !res.Status) {
                    $(submitReviewMessage).addClass("alert alert-danger");
                    $("#submitReviewMessageContent").text(ShopMessage.product.view.writeReviewError);
                    return;
                }

                $(submitReviewMessage).addClass("alert alert-success");
                $("#submitReviewMessageContent").text(ShopMessage.product.view.writeReviewSuccess);

                $("#txtOrderCode").val("");
                $("#input-review").val("");
            },
            error: function (data, textStatus, errorThrown) {
                messageDanger(ShopMessage.error.connectionFail);
            }
        });

    }).catch(function (e) { });


}

ProductViewManager.prototype.deleteReview = function (reviewId) {
    var self = this;

    if (!confirm(ShopMessage.product.view.confirmDeleteReview)) {
        return;
    }

    $.ajax({
        type: "POST",
        url: self.product.url.deleteUrl,
        data: { "reviewId": reviewId},
        dataType: "json",
        success: function (data) {
            if (data && data.IsSuccess) {
                messageSuccess(ShopMessage.product.view.deleteReviewSuccess);
            } else {
                messageDanger(ShopMessage.error.connectionFail);
            }
            location.reload();
        },
        error: function (data, textStatus, errorThrown) {
        }
    });
}