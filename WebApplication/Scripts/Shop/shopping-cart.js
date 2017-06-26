var ShoppingCart = function (options) {
    if (!options) {
        options = {
            userId: "guest"
        };
    }

    var self = this;

    //#region [Cookies]
    /*!
     * JavaScript Cookie v2.1.2
     * https://github.com/js-cookie/js-cookie
     *
     * Copyright 2006, 2015 Klaus Hartl & Fagner Brack
     * Released under the MIT license
     */
    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            define(factory);
        } else if (typeof exports === 'object') {
            module.exports = factory();
        } else {
            var OldCookies = self.Cookies;
            var api = self.Cookies = factory();
            api.noConflict = function () {
                self.Cookies = OldCookies;
                return api;
            };
        }
    }(function () {
        function extend() {
            var i = 0;
            var result = {};
            for (; i < arguments.length; i++) {
                var attributes = arguments[i];
                for (var key in attributes) {
                    result[key] = attributes[key];
                }
            }
            return result;
        }

        function init(converter) {
            function api(key, value, attributes) {
                var result;
                if (typeof document === 'undefined') {
                    return;
                }

                // Write

                if (arguments.length > 1) {
                    attributes = extend({
                        path: '/'
                    }, api.defaults, attributes);

                    if (typeof attributes.expires === 'number') {
                        var expires = new Date();
                        expires.setMilliseconds(expires.getMilliseconds() + attributes.expires * 864e+5);
                        attributes.expires = expires;
                    }

                    try {
                        result = JSON.stringify(value);
                        if (/^[\{\[]/.test(result)) {
                            value = result;
                        }
                    } catch (e) { }

                    if (!converter.write) {
                        value = encodeURIComponent(String(value))
                            .replace(/%(23|24|26|2B|3A|3C|3E|3D|2F|3F|40|5B|5D|5E|60|7B|7D|7C)/g, decodeURIComponent);
                    } else {
                        value = converter.write(value, key);
                    }

                    key = encodeURIComponent(String(key));
                    key = key.replace(/%(23|24|26|2B|5E|60|7C)/g, decodeURIComponent);
                    key = key.replace(/[\(\)]/g, escape);

                    return (document.cookie = [
                        key, '=', value,
                        attributes.expires && '; expires=' + attributes.expires.toUTCString(), // use expires attribute, max-age is not supported by IE
                        attributes.path && '; path=' + attributes.path,
                        attributes.domain && '; domain=' + attributes.domain,
                        attributes.secure ? '; secure' : ''
                    ].join(''));
                }

                // Read

                if (!key) {
                    result = {};
                }

                // To prevent the for loop in the first place assign an empty array
                // in case there are no cookies at all. Also prevents odd result when
                // calling "get()"
                var cookies = document.cookie ? document.cookie.split('; ') : [];
                var rdecode = /(%[0-9A-Z]{2})+/g;
                var i = 0;

                for (; i < cookies.length; i++) {
                    var parts = cookies[i].split('=');
                    var cookie = parts.slice(1).join('=');

                    if (cookie.charAt(0) === '"') {
                        cookie = cookie.slice(1, -1);
                    }

                    try {
                        var name = parts[0].replace(rdecode, decodeURIComponent);
                        cookie = converter.read ?
                            converter.read(cookie, name) : converter(cookie, name) ||
                            cookie.replace(rdecode, decodeURIComponent);

                        if (this.json) {
                            try {
                                cookie = JSON.parse(cookie);
                            } catch (e) { }
                        }

                        if (key === name) {
                            result = cookie;
                            break;
                        }

                        if (!key) {
                            result[name] = cookie;
                        }
                    } catch (e) { }
                }

                return result;
            }

            api.set = api;
            api.get = function (key) {
                return api(key);
            };
            api.getJSON = function () {
                return api.apply({
                    json: true
                }, [].slice.call(arguments));
            };
            api.defaults = {};

            api.remove = function (key, attributes) {
                api(key, '', extend(attributes, {
                    expires: -1
                }));
            };

            api.withConverter = init;

            return api;
        }

        return init(function () { });
    }));
    //#endregion

    const userId = options.userId || "guest";

    const cookieName = "phununews-" + userId;

    var addedItem = self.Cookies.getJSON(cookieName);

    if (!addedItem || !addedItem.length) {
        self.Cookies.set(cookieName, {});
        addedItem = [];
    }

    this.items = addedItem;
    this.cookieName = cookieName;
    this.Cookies = self.Cookies;
}

ShoppingCart.prototype.append = function (obj) {
    const self = this;

    if (!obj) {
        return;
    }

    var isFound = false;

    for (var i = 0; i < self.items.length && !isFound; i++) {
        if (obj.id === self.items[i].id) {
            var quantity = parseInt(obj.quantity) || 1;
            self.items[i].quantity = parseInt(self.items[i].quantity) + quantity;
            isFound = true;
        }
    }

    if (!isFound) {
        self.items.push(obj);
    }

    self.Cookies.set(self.cookieName, self.items);
}

ShoppingCart.prototype.remove = function (obj) {
    var self = this;

    if (!obj) {
        return;
    }

    for (var i = 0; i < self.items.length; i++) {
        if (obj.id === self.items[i].id) {
            self.items.splice(i, 1);
            break;
        }
    }

    self.Cookies.set(self.cookieName, self.items);
}

ShoppingCart.prototype.update = function (obj) {
    var self = this;

    if (!obj) {
        return;
    }

    for (var i = 0; i < self.items.length; i++) {
        if (obj.id === self.items[i].id) {
            self.items[i].quantity = obj.quantity;
            break;
        }
    }

    self.Cookies.set(self.cookieName, self.items);
}

ShoppingCart.prototype.getAllItems = function () {
    var self = this;

    return self.items;
}

ShoppingCart.prototype.clearAll = function () {
    var self = this;

    self.Cookies.set(self.cookieName, []);
}

ShoppingCart.prototype.fetchAllItems = function (loadingSelector, callback) {
    LoadingAnimationUtil.loading(loadingSelector);

    const self = this;

    const ids = [];

    for (let i = 0; i < self.items.length; i++) {
        ids.push(self.items[i].id);
    }

    $.ajax({
        type: "POST",
        url: "/Product/FindAllByIds",
        contentType: "application/json",
        data: JSON.stringify({ "ids": ids }),
        dataType: "json",
        success: function (data) {
            return callback(true, data);

        },
        error: function (data, textStatus, errorThrown) {
            return callback(false, []);
        }
    });
}

ShoppingCart.prototype.findOneByProductId = function (id) {
    const self = this;
    for (let i = 0; i < self.items.length; i++) {
        if (self.items[i].id == id) {
            return self.items[i];
        }
    }
    return null;
}