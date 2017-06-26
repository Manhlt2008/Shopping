CurrencyConverter = (function () {
    var actions = {};

    actions.toVietnamDong = function (price) {
        var unit = " VND";

        if (!price || isNaN(price)) {
            return "0,000" + unit;
        }

        return numeral(price).format("0,000") + unit;
    }

    return actions;
})();

DateTimeUtil = (function () {
    var actions = {};

    actions.getDateNow = function () {
        if (!Date.now) {
            return new Date().getTime();
        }
    }

    return actions;
})();

LoadingAnimationUtil = (function () {
    var loader = '<div class="loader"></div>';

    var actions = {};

    actions.loading = function (loadingSelector) {
        if (loadingSelector) {
            var $loadingSelector = $(loadingSelector);
            if ($loadingSelector.length > 0) {
                $loadingSelector.html(loader);
            }
        }
    };

    actions.stopLoading = function (loadingSelector) {
        if (loadingSelector) {
            var $loadingSelector = $(loadingSelector);
            if ($loadingSelector.length > 0) {
                var loaderItem = $loadingSelector.children('div.loader');
                if (loaderItem.length > 0) {
                    loaderItem.remove();
                }
            }
        }
    };

    return actions;
})();

(function () {
    $(document).ready(function () {
        $(".input-search").on('keypress', function (e) {
            if (e.which === 13) {
                var query = $(this).val();
                if (query && query.trim().length > 0) {
                    window.location.href = "/Product/Category?query=" + query;
                }
            }
        });

        $("#btn-search").on('click', function (e) {
            var query = $(this).prev().val();
            if (query && query.trim().length > 0) {
                window.location.href = "/Product/Category?query=" + query;
            }
        });
    });
})();