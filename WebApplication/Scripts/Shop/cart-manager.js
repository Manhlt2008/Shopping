var CartController = (function () {
    var cart = new ShoppingCart({});

    var actions = {};

    var defaultData = {
        cartTotal: "0 item(s) - 0 đ",
        cartTotal2: "0",
        cartTotal3: "0 item(s)"
    }

    var elements = {
        cartTotal: $("#cart-total"),
        cartTotal2: $("#cart-total2"),
        cartTotal3: $("#cart-total3"),
        shoppingCartDetail: $("#shopping-cart-detail"),
        cartTableContent: $("#cart-table-content"),
        subTotal: $("#subTotal"),
        tax: $("#tax"),
        total: $("#total")
    }

    actions.cart = cart;
    actions.defaultData = defaultData;
    actions.elements = elements;

    actions.init = function (isRequestReloadTable) {
        var self = this;

        self.isRequestReloadTable = isRequestReloadTable;

        self.showAllProductView();
    };

    actions.clearAllProductView = function () {
        $(elements.cartTotal).text(defaultData.cartTotal);
        $(elements.cartTotal2).text(defaultData.cartTotal2);
        $(elements.cartTotal3).text(defaultData.cartTotal3);

        const html =
            '<li>' +
            '    <p class="text-center">Giỏ hàng của bạn đang trống !</p>' +
            '</li>';

        $(elements.shoppingCartDetail).empty();
        $(elements.shoppingCartDetail).html(html);

    }

    actions.showAllProductView = function () {
        const self = this;

        if (!cart.getAllItems() || cart.getAllItems().length === 0) {
            self.clearAllProductView();
            return;
        }

        const loadProduct = new Promise(function (resolve, reject) {
            cart.fetchAllItems(function (isSuccess, data) {
                if (!isSuccess) {
                    reject();
                } else {
                    resolve(data);
                }
            });
        });

        loadProduct.then(function (products) {
            if (products.length === 0) {
                self.clearAllProductView();
                return;
            }

            $(elements.shoppingCartDetail).empty();

            var totalPrice = 0;
            var totalQuantity = 0;

            for (let i = 0; i < products.length; i++) {
                const product = products[i];
                var quantity = 1;

                var item = cart.findOneByProductId(product.Id);
                if (item !== null) {
                    quantity = parseInt(item.quantity) || 1;
                }

                totalPrice += quantity * product.Price;
                totalQuantity += quantity;

                const li =
                    '<li>' +
                    '    <div>' +
                    '        <table class="table">' +
                    '            <tbody>' +
                    '               <tr>' +
                    '                   <td class="text-center"> ' +
                    '                       <div class="image">' +
                    '                           <a href="/Product/View/' + product.Id + '">' +
                    '                               <img src="/Image/View/' + product.Cover + '" ' +
                    '                                   alt="' + product.Name + '" title="' + product.Name + '" class="img-thumbnail">' +
                    '                           </a></div>' +
                    '                   </td>' +
                    '                   <td class="text-left">' +
                    '                       <div class="name">' +
                    '                           <a href="/Product/View/' + product.Id + '">' + product.Name + '</a>' +
                    '                       </div>' +
                    '                       <div> x ' + quantity + ' <span class="price-cart">' + CurrencyConverter.toVietnamDong(product.Price * quantity) + '</span></div>' +
                    '                   </td>' +
                    '                   <td class="text-right"></td>' +
                    '                   <td class="text-center">' +
                    '                       <button type="button" onclick="CartController.remove({id : ' + product.Id + '});" title="Xóa khỏi giỏ hàng" class="btn btn-danger btn-xs">' +
                    '                           <i class="fa fa-times"></i>' +
                    '                       </button>' +
                    '                   </td>' +
                    '               </tr>' +
                    '           </tbody>' +
                    '       </table>' +
                    '   </div>' +
                    '</li>';

                $(elements.shoppingCartDetail).append($(li));


            }

            var li =
                '<li>' +
                '   <div>' +
                '       <table class="table total">' +
                '           <tbody>' +
                '               <tr>' +
                '                   <td class="text-right"><strong>Tạm Tính</strong></td>' +
                '                   <td class="text-right">' + CurrencyConverter.toVietnamDong(totalPrice) + '</td>' +
                '               </tr>' +
                '               <tr>' +
                '                   <td class="text-right"><strong>VAT (10%)</strong></td>' +
                '                   <td class="text-right">' + CurrencyConverter.toVietnamDong(parseInt(totalPrice * 0.1)) + '</td>' +
                '               </tr>' +
                '               <tr>' +
                '                   <td class="text-right"><strong>Tổng Cộng</strong></td>' +
                '                   <td class="text-right">' + CurrencyConverter.toVietnamDong(parseInt(totalPrice * 1.1)) + '</td>' +
                '               </tr>' +
                '           </tbody>' +
                '       </table>' +
                '       <p class="text-right">' +
                '           <a class="btn btn-primary" href="/Order/Cart">Xem Giỏ Hàng</a>' +
                '       </p>' +
                '   </div>' +
                '</li>';

            $(elements.shoppingCartDetail).append($(li));

            $(elements.cartTotal).text(totalQuantity + " item(s) - " + CurrencyConverter.toVietnamDong(parseInt(totalPrice * 1.1)) + "");
            $(elements.cartTotal2).text(totalQuantity);
            $(elements.cartTotal3).text(totalQuantity + " item(s)");

        }).catch(function (e) {
            self.clearAllProductView();
            console.error(e);
        });
    };

    actions.add = function (object) {
        const self = this;

        cart.append(object);

        const id = object.id;
        const name = object.name;
        const successMessage = `Success: Đã thêm sản phẩm <a href="/Product/View/${id}" title='${name}'>${name}</a> vào <a href="/Order/Cart" title="Xem Giỏ Hàng">Giỏ Hàng</a> của bạn!`;
        messageSuccess(successMessage);

        self.showAllProductView();

        if (self.isRequestReloadTable) {
            self.loadShoppingCartTable();
        }
    };

    actions.update = function (object) {
        const self = this;

        if (!object) {
            return;
        }

        const item = cart.findOneByProductId(object.id);

        if (!item) {
            console.error("Added Product Not Found");
            return;
        }

        item.quantity = $("[name='quantity[" + item.id + "]']").val();

        cart.update(item);
        self.showAllProductView();

        if (self.isRequestReloadTable) {
            self.loadShoppingCartTable();
        }

    }

    actions.remove = function (object) {
        const self = this;

        if (!object) {
            return;
        }

        const item = cart.findOneByProductId(object.id);

        if (!item) {
            console.error("Added Product Not Found");
            return;
        }

        cart.remove(item);
        self.showAllProductView();
    }

    actions.clear = function () {
        const self = this;

        cart.clearAll();
        self.clearAllProductView();

        if (self.isRequestReloadTable) {
            self.loadShoppingCartTable(true);
        }
    }

    actions.loadShoppingCartTable = function () {
        const self = this;

        cart.fetchAllItems(function (isSuccess, products) {
            if (isSuccess && products && products.length > 0) {
                $(elements.cartTableContent).show();
                var tableBody = $(elements.cartTableContent).children('tbody');
                tableBody.empty();

                var totalPrice = 0;

                for (let i = 0; i < products.length; i++) {
                    const product = products[i];
                    const item = cart.findOneByProductId(product.Id);

                    totalPrice += item.quantity * product.Price;

                    var tr =
                        '<tr id=cartTr"' + product.Id + '">' +
                            '   <td class="text-center">' +
                            '       <div class="image">' +
                            '           <a href="/Product/View/' + product.Id + '">' +
                            '               <img src="/Image/View/' + product.Cover + '" ' +
                            '               alt="' + product.Name + '" style="width: 100px; height: 100px;"' +
                            '               title="' + product.Name + '" class="img-thumbnail">' +
                            '           </a>' +
                            '       </div>' +
                            '   </td>' +
                            '   <td class="text-left">' +
                            '       <a href="/Product/View/' + product.Id + '">' + product.Name + '</a>' +
                            '   </td>' +
                            '   <td class="text-left">' +
                            '       <a href="/Product/Category/' + product.Category.Id + '">' + product.Category.Name + '</a>' +
                            '   </td>' +
                            '   <td class="text-left">' +
                            '       <div class="input-group btn-block" style="max-width: 200px;">' +
                            '           <p class="clearfix">' +
                            '               <input type="text" name="quantity[' + product.Id + ']" value="' + item.quantity + '" ' +
                            '                   min="1" max="' + product.quantity + '" size="1" class="form-control cart-q" id="cart-q"' +
                            '                   onchange="CartController.update({id : ' + product.Id + '})">' +
                            '           </p>' +
                            '           <div>' +
                            '               <button type="button" data-toggle="tooltip" title="" class="btn btn-primary" ' +
                            '                   onclick="CartController.update({id : ' + product.Id + '})">' +
                            '                   <i class="fa fa-refresh"></i>' +
                            '               </button>' +
                            '               <button type="button" data-toggle="tooltip" title="" class="btn btn-danger" ' +
                            '                   onclick="CartController.remove({id : ' + product.Id + '});" data-original-title="Remove">' +
                            '                   <i class="fa fa-times-circle"></i>' +
                            '               </button>' +
                            '           </div>' +
                            '       </div>' +
                            '   </td>' +
                            '   <td class="text-right">' +
                            '       <div class="price">' +
                            '           ' + CurrencyConverter.toVietnamDong(product.Price) +
                            '       </div>' +
                            '   </td>' +
                            '   <td class="text-right">' +
                            '       <div class="price price-total">' +
                            '           ' + CurrencyConverter.toVietnamDong(product.Price * item.quantity) +
                            '       </div>' +
                            '   </td>' +
                            '</tr>';

                    tableBody.append($(tr));
                }

                $(elements.subTotal).text(CurrencyConverter.toVietnamDong((totalPrice)));
                $(elements.tax).text(CurrencyConverter.toVietnamDong(parseInt(totalPrice * 0.1)));
                $(elements.total).text(CurrencyConverter.toVietnamDong(parseInt(totalPrice * 1.1)));
            }
        });
    }

    actions.checkout = function (callback) {
        var self = this;
        if (!confirm(ShopMessage.product.cart.confirmCheckout)) {
            return;
        }

        // TODO: Binding address here
        var submitData = {
            items: cart.getAllItems(),
            DeliveringInfo: {
                IsUsingDefaultAddress: true,
                FirstName: $("#firstName").val(),
                LastName: $("#lastname").val(),
                Address: $("#address").val(),
                Province: $("#province").val(),
                District: $("#district").val(),
                Phone: $("#phone").val(),
                Dob: new Date(),
                Gender: $("sex").val(),
                Mail: $("#email").val
            }
        }

        $.ajax({
            type: "POST",
            url: "/Order/Checkout",
            contentType: "application/json",
            data: JSON.stringify(submitData),
            dataType: "json",
            success: function (data) {
                if (data.Status === 0) {
                    alert(ShopMessage.product.cart.requireLogin);
                    window.location.href = "/Authentication/Login";

                    if (callback) {
                        return callback(false, data);
                    } else {
                        return null;
                    }
                }

                if (data.id !== 0) {
                    const message = 'Hóa đơn <a href="/Order/View/' + data.Data.id + '">#' + data.Data.code + '</a> đã được khỏi tạo thành công';
                    messageSuccess(message);

                    var payment = JSON.parse(data.Payment);

                    if (payment) {
                        window.location.href = payment.redirectURL;
                    } else {

                        //self.clear();
                        window.location.reload();
                    }
                }

                if (callback) {
                    return callback(true, data);
                }

            },
            error: function (data, textStatus, errorThrown) {
                alert(ShopMessage.product.cart.requireLogin);
                window.location.href = "/Authentication/Login";
                if (callback) {
                    return callback(false, []);
                }
            }
        });
    }

    return actions;
})();

CartController.init(true);