var ReviewManageController = (function () {
    var actions = {};
    var self = this;

    var url = {};

    var currentProduct = $("#currentProduct").val();

    actions.init = function (options) {
        if (!options) {
            options = {};
        }

        url = {
            approve: options.approveUrl,
            approveAndUpdate: options.approveAndUpdateUrl
        };

        if ($("#product_select").length) {
            $("#product_select").select2({
                allowClear: true,
                placeholder: "Select..."
            }).on("change", function (e) {
                window.location = e.val;
            }).select2("data", { text: currentProduct });
        }

        $("#chkListAllReviews").change(function () {
            if (this.checked) {
                window.location = $(this).attr("data-url");
            }
        });
    }

    actions.approve = function (reviewId) {
        if (!confirm(ShopMessage.review.manage.confirmApproveReview)) {
            return;
        }

        $.ajax({
            type: "POST",
            url: url.approve,
            data: { "reviewId": reviewId },
            dataType: "json",
            success: function (data) {
                if (data && data.IsSuccess) {
                    NotificationUtil.successMessage("Approve review successfully.");
                    location.reload();
                    return false;
                }
                NotificationUtil.successMessage("Connection error. Please try again.");
                return false;
            },
            error: function (data, textStatus, errorThrown) {
            }
        });

        return false;
    }

    actions.enableEdit = function (reviewId) {
        var inputMessage = $("#inputMessage-" + reviewId);
        var staticMessage = $("#staticMessage-" + reviewId);
        var area = $("#editArea-" + reviewId);

        $(area).show();
        $(staticMessage).hide();
    }

    actions.saveMessage = function (reviewId) {
        var inputMessage = $("#inputMessage-" + reviewId);
        var staticMessage = $("#staticMessage-" + reviewId);
        var area = $("#editArea-" + reviewId);

        $(area).hide();
        $(staticMessage).show();

        $(staticMessage).text($(inputMessage).val().trim());
        $(inputMessage).val($(inputMessage).val().trim());
    }

    actions.cancelMessage = function (reviewId) {
        var inputMessage = $("#inputMessage-" + reviewId);
        var staticMessage = $("#staticMessage-" + reviewId);
        var area = $("#editArea-" + reviewId);

        $(area).hide();
        $(staticMessage).show();

        $(staticMessage).text($(staticMessage).text().trim());
        $(inputMessage).val($(staticMessage).text().trim());
    }

    actions.saveAndApprove = function (reviewId) {
        if (!confirm(ShopMessage.review.manage.confirmUpdateMessageAndApproveReview)) {
            return;
        }

        var inputMessage = $("#inputMessage-" + reviewId).val().trim();

        $.ajax({
            type: "POST",
            url: url.approveAndUpdate,
            data: { "ReviewId": reviewId, "Message": inputMessage },
            dataType: "json",
            success: function (data) {
                if (data && data.IsSuccess) {
                    NotificationUtil.successMessage("Update and Approve review successfully.");
                    location.reload();
                    return false;
                }
                NotificationUtil.successMessage("Connection error. Please try again.");
                return false;
            },
            error: function (data, textStatus, errorThrown) {
            }
        });

    }

    return actions;
})();