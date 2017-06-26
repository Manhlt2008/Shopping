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
        shoppingCartDetail: $(".added-products-cart"),
        cartTableContent: $("#cart-table-content"),
        cartTableMessage: $("#cart-table-message"),
        subTotal: $("#subTotal"),
        tax: $("#tax"),
        total: $("#total"),
        shoppingCartCount: $("#shopping-cart-count"),
        menuCartEmpty: $("#menu-cart-empty"),
        menuCartItems: $("#menu-cart-items")
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
            '    <p class="text-center">Không có sản phẩm nào.</p>' +
            '</li>';

        $(elements.shoppingCartDetail).empty();
        $(elements.shoppingCartDetail).html(html);
        $(elements.shoppingCartCount).html(0);

        $(elements.menuCartEmpty).show();
        $(elements.menuCartItems).empty();
        $(elements.menuCartItems).hide();
    }

    actions.showAllProductView = function () {
        var loadingSelector = '.loading-selector';

        const self = this;

        if (!cart.getAllItems() || cart.getAllItems().length === 0) {
            self.clearAllProductView();
            return;
        }

        const loadProduct = new Promise(function (resolve, reject) {
            cart.fetchAllItems(loadingSelector, function (isSuccess, data) {
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

            $(elements.menuCartEmpty).hide();
            $(elements.menuCartItems).empty();

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
               '   <div class="b-option-cart__items__img">' +
               '       <div class="view view-sixth">' +
               '           <img data-retina="" src="/Image/View/' + product.Cover + '" alt="' + product.Name + '" style="width: 50px; height: 50px; object-fit: contain;">' +
               '           <div class="b-item-hover-action f-center mask">' +
               '               <div class="b-item-hover-action__inner">' +
               '                   <div class="b-item-hover-action__inner-btn_group">' +
               '                       <a href="/Product/View/' + product.Id + '" class="b-btn f-btn b-btn-light f-btn-light info"><i class="fa fa-link"></i></a>' +
               '                   </div>' +
               '               </div>' +
               '           </div>' +
               '       </div>' +
               '   </div>' +
               '   <div class="b-option-cart__items__descr">' +
               '       <strong class="b-option-cart__descr__title f-option-cart__descr__title">' +
               '       <a class="wrap-text" href="/Product/View/' + product.Id + '" title="' + product.Name + '">' + product.Name + '</a></strong>' +
               '       <span class="b-option-cart__descr__cost f-option-cart__descr__cost">' + quantity + ' x ' + CurrencyConverter.toVietnamDong(product.Price) + '</span>' +
               '   </div>' +
               '   <i class="fa fa-times b-icon--fa" style="right: 25px" onclick="CartController.remove({id : ' + product.Id + '});"></i>' +
               '</li>';

                const cartItem =
                   '<div class="b-blog-short-post b-blog-short-post--popular b-blog-short-post--w-img b-blog-short-post--img-hover-bordered f-blog-short-post--w-img row f-blog-short-post--popular">' +
                   '   <div class="b-blog-short-post__item col-md-12 col-sm-6 col-xs-12">' +
                   '       <div class="b-blog-short-post__item_img">' +
                   '           <a href="/Product/View/' + product.Id + '">' +
                   '               <img class="img-fit-60" data-retina="" src="/Image/View/' + product.Cover + '" alt="' + product.Name + '">' +
                   '           </a>' +
                   '       </div>' +
                   '       <div class="b-remaining">' +
                   '           <div class="b-blog-short-post__item_text f-blog-short-post__item_text">' +
                   '               <a href="/Product/View/' + product.Id + '">' + product.Name + '</a>' +
                   '           </div>' +
                   '           <div class="f-blog-short-post__item_price f-primary-b">' +
                   '               ' + quantity + ' x ' + CurrencyConverter.toVietnamDong(product.Price) +
                   '           </div>' +
                   '       </div>' +
                   '   </div>' +
                   '</div>';

                $(elements.shoppingCartDetail).append($(li));
                $(elements.menuCartItems).append($(cartItem));
            }

            $(elements.shoppingCartCount).html(totalQuantity);

            $(elements.menuCartItems).show();
            LoadingAnimationUtil.stopLoading(loadingSelector);
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
        const successMessage = `Đã thêm sản phẩm <a href="/Product/View/${id}" title='${name}'>${name}</a> vào <a href="/Order/Cart" title="Xem Giỏ Hàng">Giỏ Hàng</a> của bạn.`;
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
        self.loadShoppingCartTable();
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
        var loadingSelector = '.loading-selector';

        const self = this;

        cart.fetchAllItems(loadingSelector, function (isSuccess, products) {
            if (isSuccess && products && products.length > 0) {

                var tableBody = $(elements.cartTableContent).children('tbody');
                tableBody.children('tr').not(':first').remove();

                var totalPrice = 0;

                for (let i = 0; i < products.length; i++) {
                    const product = products[i];
                    const item = cart.findOneByProductId(product.Id);

                    totalPrice += item.quantity * product.Price;

                    var tr =
                        '<tr>' +
                        '    <td>' +
                        '        <div class="b-href-with-img">' +
                        '            <a class="c-primary" href="/Product/View/' + product.Id + '">' +
                        '                <img class="img-fit-60" data-retina="" src="/Image/View/' + product.Cover + '" alt="' + product.Name + '">' +
                        '                <p>' +
                        '                    <div class="f-title-small" style="width:78%;float:right;">' + product.Name + '</div>' +
                        '                    <span class="f-title-smallest " style="clear: both;float:right;">' + product.Category.Name + '</span>' +
                        '                </p>' +
                        '            </a>' +
                        '        </div>' +
                        '    </td>' +
                        '    <td><span class="f-primary-b c-default f-title-medium"><span class="j-product-price">' + CurrencyConverter.toVietnamDong(product.Price) + '</span></span></td>' +
                        '    <td class="f-center">' +
                        '        <div class="b-product-card__info_count">' +
                        '            <input type="number" min="1" max="10"' + product.quantity + '" name="quantity[' + product.Id + ']" class="form-control form-control--secondary j-product-count" ' +
                            '        value="' + item.quantity + '" id="cart-q"' + product.Id + '" onchange="CartController.update({id : ' + product.Id + '})">' +
                        '        </div>' +
                        '    </td>' +
                        '    <td><span class="f-primary-b c-default f-title-medium"><span class="j-product-total ">' + CurrencyConverter.toVietnamDong(product.Price * item.quantity) + '</span></span></td>' +
                        '    <td><span class="f-primary-b f-center"><a href="javascript:void(0);" onclick="CartController.remove({id : ' + product.Id + '});">X</a></span></td>' +
                        '</tr>';

                    tableBody.append($(tr));
                }

                $(elements.subTotal).text(CurrencyConverter.toVietnamDong((totalPrice)));
                $(elements.tax).text(CurrencyConverter.toVietnamDong(parseInt(totalPrice * 0.1)));
                $(elements.total).text(CurrencyConverter.toVietnamDong(parseInt(totalPrice * 1.1)));

                LoadingAnimationUtil.stopLoading(loadingSelector);
                $(elements.cartTableMessage).hide();
                $(elements.cartTableContent).show();
            } else {
                LoadingAnimationUtil.stopLoading(loadingSelector);

                $(elements.cartTableContent).hide();
                $(elements.cartTableContent).children('tbody').empty();
                $(elements.cartTableMessage).show();
            }

            if (!self.isEmptyCart()) {
                $('#checkout-panel').show();
                $('#addForm').show();
            } else {
                $('#checkout-panel').hide();
                $('#addForm').hide();
            }
        });
    }

    actions.isLogin = function (callback) {
        $.ajax({
            type: "POST",
            url: "/Authentication/Ack",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                if (callback) {
                    return callback(data == "1");
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

    actions.isEmptyCart = function () {
        if (cart.getAllItems().length == 0) {
            return true;
        }

        return false;
    };

    actions.checkout = function (callback) {
        var self = this;

        if (self.isEmptyCart()) {
            alert("Giỏ hàng đang trống, không thể tiến hành giao dịch.");
            return;
        }

        self.isLogin(function (isLogin) {
            if (!isLogin) {
                var check = confirm("Vui lòng đăng nhập trước khi tiếp tục.");
                if (check) {
                    window.location.href = "/Authentication/Login";
                }

                return;
            }
            self.isAllowCreateOrder(function (isSuccess, data) {
                if (isSuccess && data.IsAllow) {
                    if (!confirm(ShopMessage.product.cart.confirmCheckout)) {
                        return;
                    }

                    var submitData = {
                        items: cart.getAllItems(),
                        DeliveringInfo: {
                            IsUsingDefaultAddress: $('[name="payoption"]').val() === 'CurrentAddress',
                            FirstName: $("#firstName").val(),
                            LastName: $("#lastName").val(),
                            Address: $("#address").val(),
                            Province: $("#province").val(),
                            District: $("#district").val(),
                            Phone: $("#phone").val(),
                            Dob: new Date(),
                            Gender: $("#sex :selected").val(),
                            Mail: $("#email").val()
                        },
                        PaymentMethod: $('[name="PaymentMethod"]:checked').val()
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
                                const message = 'Hóa đơn <a href="/Order/View/' +
                                    data.Data.id +
                                    '">#' +
                                    data.Data.code +
                                    '</a> đã được khỏi tạo thành công';
                                messageSuccess(message);

                                self.clear();

                                var payment = false;

                                try {
                                    payment = JSON.parse(data.Payment);
                                } catch (err) {

                                }


                                if (payment) {
                                    window.location.href = payment.redirectURL;
                                } else {
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
                } else {
                    messageDanger("<strong>Thông Báo</strong>: Do PhuNuMart đang trong giai đoạn thử nghiệm, nên chúng tôi chỉ tiếp nhận 500 hóa đơn mỗi ngày. Hiện quý khách không thể tiến hành thanh toán, xin vui lòng quay trở lại vào ngày mai.");
                }
            });
        });

    }

    actions.isAllowCreateOrder = function (callback) {
        $.ajax({
            type: "POST",
            url: "/Order/IsAllowCreateOrder",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
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