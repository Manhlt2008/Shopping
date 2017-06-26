var SaveOrderController = (function () {
    var actions = {};

    var url = {};

    var products = [];
    var users = [];
    var userId = -1;

    var orderInfo = {};

    var isAddMoreProduct = false;

    var registerSelectTypeEvent = function () {
        $(".select2-input").donetyping(function () {
            if (isAddMoreProduct) {
                selectQueryEvents(this);
            } else {
                searchUserEvents(this);
            }
        });

        $("#userSelection").on("select2-selecting", function (e) {
            if (!isAddMoreProduct) {
                var obj = JSON.parse(e.val);

                if (obj) {
                    $("#lblClientDetialEmail").html($("<a href='mailto:" + obj.email + "'>" + obj.email + "</a>"));
                    $("#lblClientDetialPhone").html($("<b>" + obj.phone + "</b>"));
                }

                userId = obj.id;
            }
        });
    }

    var getRowElements = function (productId) {
        return {
            txtUnitPrice: $("#txtPrice-" + productId),
            aUnitPrice: $("#aPrice-" + productId),
            quantity: $("#txtQuantity-" + productId),
            discount: $("#txtDiscount-" + productId),
            amount: $("#amount-" + productId),
            origindata: $("#origindata-" + productId),
            txtTotalPrice: $("#txtTotalPrice"),
            orderItemsTable: $("#orderItemsTable"),
            tr: $("#tr-" + productId)
        }
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

                    registerSelectTypeEvent();
                }
            });

        }
    }

    var searchUserEvents = function (e) {
        var query = $(e).val();

        if (query != undefined && query.trim().length > 0) {
            $('#userSelection').html("");

            $.ajax({
                type: "POST",
                url: url.searchUser,
                data: { "query": query },
                dataType: "json",
                success: function (data) {
                    data = JSON.parse(data);
                    users = data;

                    for (var i = 0; i < data.length; i++) {
                        var obj = data[i];
                        var text = obj.FirstName + ' ' + obj.LastName + ' (' + obj.Phone + ' - ' + obj.Email + ')';
                        var value = {
                            id: obj.Id,
                            email: obj.Email,
                            phone: obj.Phone
                        };

                        $('#userSelection').append($('<option value=\'' + JSON.stringify(value) + '\'>' + text + '</option>'));
                    }

                    $("#userSelection").select2("destroy");
                    $('#userSelection').select2().select2("open");

                    registerSelectTypeEvent();
                }
            });

        }
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

    actions.init = function (options) {
        var self = this;

        donetypingRegister();

        orderInfo = JSON.parse($("#orderInfo").val());


        if (!options) {
            options = {};
        }

        url = {
            findAllByProductName: options.url.findAllByProductName,
            view: options.url.view,
            categoryView: options.url.categoryView,
            updateOrder: options.url.updateOrder,
            searchUser: options.url.searchUser,
            updatePage: options.url.updatePage
        }

        $(".auto").autoNumeric("init",
        {
            mRound: "C",
            vMin: 0
        });

        $('#addProductModal').on('hidden.bs.modal', function () {
            isAddMoreProduct = false;
        });

        if ($('#addProductDropdownList').length) {
            $('#addProductDropdownList').select2();
        }

        if ($('#userSelection').length) {
            $('#userSelection').select2();
        }

        registerSelectTypeEvent();

    }

    actions.remove = function (e) {
        var elements = getRowElements($(e).attr("data-productid"));
        var originData = JSON.parse($(elements.origindata).val());


        if (!confirm(ShopMessage.order.update.removeProductConfirm(originData.name))) {
            return;
        }

        $(elements.tr).remove();
    }

    actions.enableEditPrice = function (e) {
        var elements = getRowElements($(e).attr("data-productid"));
        var originData = JSON.parse($(elements.origindata).val());

        if (!confirm(ShopMessage.order.update.updateProductPriceConfirm(originData.name))) {
            return;
        }

        $(elements.txtUnitPrice).show();
        $(elements.aUnitPrice).hide();
    }

    actions.discardChange = function (e) {
        var elements = getRowElements($(e).attr("data-productid"));

        var originData = JSON.parse($(elements.origindata).val());

        if (!confirm(ShopMessage.order.update.discardChangeConfirm(originData.name))) {
            return;
        }

        $(elements.txtUnitPrice).val(originData.price);
        $(elements.aUnitPrice).text(CurrencyConverter.toVietNamDong(originData.price));
        $(elements.quantity).val(originData.quantity);
        $(elements.discount).val(originData.discount);
        $(elements.amount).text(CurrencyConverter.toVietNamDong(parseInt(originData.price) * parseInt(originData.quantity)));

        $(elements.txtUnitPrice).hide();
        $(elements.aUnitPrice).show();
    }

    actions.submit = function () {
        var info = JSON.parse($("#orderInfo").val());

        var prices = $('[name="prices"]');
        var quantities = $('[name="quantities"]');
        var discounts = $('[name="discounts"]');
        var products = $('[name="products"]');

        if (!(prices.length == quantities.length &&
            quantities.length == discounts.length &&
            discounts.length == products.length)) {
            return;
        }

        var oderDetial = [];

        for (var i = 0; i < prices.length; i++) {
            var priceObj = prices[i];
            var quantityObj = quantities[i];
            var discountObj = discounts[i];

            var orderDetailId = $(priceObj).attr("data-orderdetailid");
            var productId = $(priceObj).attr("data-productid");
            var price = $(priceObj).autoNumeric("get");
            var quantity = $(quantityObj).val();
            var discount = $(discountObj).val();


            var obj = {
                OrderDetailId: parseInt(orderDetailId),
                ProductId: parseInt(productId),
                Quantity: parseInt(quantity),
                Price: parseFloat(price),
                Discount: parseFloat(discount)
            }

            oderDetial.push(obj);
        }

        var postData = {
            OrderId: info.OrderId,
            Status: $("#lblUpdateOrderStatus").val(),
            OrderDetails: oderDetial,
            UserId: userId
        };

        $.ajax({
            type: "POST",
            url: url.updateOrder,
            data: JSON.stringify(postData),
            contentType: 'application/json',
            dataType: "json",
            traditional: true,
            success: function (res) {
                if (res && res.IsSuccess) {
                    if (postData.OrderId != -1) {
                        NotificationUtil.successMessage(ShopMessage.order.update.saveSuccess);
                    } else {
                        NotificationUtil.successMessage(ShopMessage.order.create.saveSuccess);
                        $("#ClientDetailOrderId").html($("<b>" + res.Id + "</b>"));
                        $("#ClientDetailOrderCode").html($("<b>" + res.Code + "</b>"));

                        if (confirm(ShopMessage.order.create.confirmIsContinueCreate)) {
                            location.reload();
                        } else {
                            location.href = url.updatePage + "/" + res.Id;
                        }
                    }
                } else {
                    NotificationUtil.warningMessage(ShopMessage.error.connectionFail);
                }
            }
        });

    }

    actions.addMoreProduct = function () {
        var productId = $('#addProductDropdownList').select2().val();

        var product;

        for (var i = 0; i < products.length; i++) {
            if (products[i].Id == productId) {
                product = products[i];
            }
        }

        if (product) {
            var productObjs = $('[name="products"]');

            for (var i = 0; i < productObjs.length; i++) {
                var obj = JSON.parse($(productObjs[i]).val());
                if (obj.id == product.Id) {
                    NotificationUtil.warningMessage(ShopMessage.order.update.duplicatedItem);
                    return;
                }
            }

            var html = '<tr id="tr-' + product.Id + '">' +
                '           <td>' +
                '               <input type="hidden" id="origindata-' + product.Id + '" name="products"' +
                '                   value=\'{"price": "' + product.Price + '", "quantity": "1", "discount": "0", "name": "' + product.Name + '"}\' />' +
                '               <a href="' + url.view + '/' + product.Id + '" target="_blank">' +
                '                   ' + product.Name +
                '               </a>' +
                '           </td>' +
                '           <td>' +
                '               <a href="' + url.categoryView + '/' + product.Category.Id + '">' +
                '                   ' + product.Category.Name +
                '               </a>' +
                '           </td>' +
                '           <td style="text-align: right">' +
                '               <input type="text" name="prices" data-orderdetailid="-1" class="form-control auto" id="txtPrice-' + product.Id + '"' +
                '                   data-productid="' + product.Id + '" value="' + product.Price + '" style="display: none" data-v-min="0" data-v-max="99999999"' +
                '                    onchange="SaveOrderController.updatePrice(this)" />' +
                '               <a href="javascript:void(0);" title="Update price" class="admincp-editable" id="aPrice-' + product.Id + '"' +
                '                   onclick="SaveOrderController.enableEditPrice(this)" data-productid="' + product.Id + '">' +
                '                   ' + CurrencyConverter.toVietNamDong(product.Price) +
                '               </a>' +
                '           </td>' +
                '           <td>' +
                '               <input type="number" name="quantities" data-orderdetailid="-1" class="form-control" id="txtQuantity-' + product.Id + '"' +
                '                   data-productid="' + product.Id + '" value="1" onchange="SaveOrderController.updatePrice(this)" />' +
                '           </td>' +
                '           <td>' +
                '               <input type="number" name="discounts" data-orderdetailid="-1" class="form-control" id="txtDiscount-' + product.Id + '"' +
                '                   data-productid="' + product.Id + '" value="0"  onchange="SaveOrderController.updatePrice(this)" />' +
                '           </td>' +
                '           <td style="text-align: right">' +
                '               <span id="amount-' + product.Id + '">' +
                '                   ' + CurrencyConverter.toVietNamDong(product.Price) +
                '               </span>' +
                '           </td>' +
                '           <td style="text-align: center">' +
                '               <a href="javascript:void(0);" title="Remove item ' + product.Name + '"' +
                '                   data-orderdetailid="-1" data-productid="' + product.Id + '"' +
                '                   onclick="SaveOrderController.remove(this)">' +
                '                   <span class="glyphicon glyphicon-trash"></span>' +
                '               </a>' +
                '               <a href="javascript:void(0);" title="Discard changes"' +
                '                   data-orderdetailid="-1" data-productid="' + product.Id + '"' +
                '                   onclick="SaveOrderController.discardChange(this)">' +
                '                   <span class="glyphicon glyphicon-floppy-remove"></span>' +
                '               </a>' +
                '           </td>' +
                        
                '       </tr>';

            $("#itemsTbody").append($(html));

            if ($('#addProductDropdownList').length) {
                $('#addProductDropdownList').select2();
            }

            $(".select2-input")
                .donetyping(function () {
                    selectQueryEvents(this);

                });

            $(".auto")
                .autoNumeric("init",
                {
                    mRound: "C",
                    vMin: 0
                });

            NotificationUtil.successMessage(ShopMessage.order.update.addedProduct);

        }
    }

    actions.updatePrice = function (e) {
        var productId = $(e).attr("data-productid");
        var price = $("#txtPrice-" + productId).autoNumeric("get");
        var discount = $("#txtDiscount-" + productId).val();
        var quantity = $("#txtQuantity-" + productId).val();

        var amount = parseFloat(price) * parseFloat(quantity) * (1 - (parseFloat(discount) / 100));

        $("#amount-" + productId).text(CurrencyConverter.toVietNamDong(amount.toFixed(0)));
    }

    actions.closeModal = function () {
        isAddMoreProduct = false;
    }

    actions.openModal = function () {
        isAddMoreProduct = true;

    }

    return actions;
})();