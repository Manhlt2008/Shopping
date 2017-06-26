var ShopMessage = (function () {
    var messages = {};

    var product = {};
    product.view = {};

    product.view.quantityTooMuch = function (max) {
        return `Còn <b>${max}<b> sản phẩm trong kho hàng.`;
    }

    product.view.quantityRequire = "Số lượng không hợp lệ.";
    product.view.provideOrderCode = "Vui lòng sử dụng mã hóa đơn để tiến hành viết nhận xét cho sản phẩm này.";
    product.view.provideInvalidOrderCode = "Mã hóa đơn không hợp lệ.";
    product.view.provideReview = "Nhận xét của bạn phải trên 25 ký tự.";
    product.view.writeReviewSuccess = "Bài viết của bạn đã gửi thành công. Đang chờ xác nhận từ webmaster.";
    product.view.writeReviewError = "Submmiting to server fail. Please try again.";
    product.view.confirmDeleteReview = "Bạn có muốn xóa sản phẩm này?";
    product.view.deleteReviewSuccess = "Xóa thành công.";

    product.cart = {};

    product.cart.confirmCheckout = "Bạn có chắc chắn tiến hành thanh toán?";
    product.cart.requireLogin = "Vui lòng đăng nhập trước khi tiếp tục.";

    messages.product = product;

    var review = {};

    review.manage = {};

    review.manage.confirmApproveReview = "Bạn có chắc chắn xác nhận bài viết này?";
    review.manage.confirmUpdateMessageAndApproveReview = "Bạn có chắc chắn lưu thay đổi và xác nhận bài viết này?";

    messages.review = review;

    var order = {};

    order.manage = {};

    order.manage.confirmUpdateStatus = "Bạn có chắc chắn sẽ cập nhật trạng thái cho đơn hàng này?";
    order.manage.updateStatusSuccess = "Cập nhật trạng thái thành công.";

    order.update = {};

    order.update.discardChangeConfirm = function (productName) {
        return `Bạn có chắc chắn hủy tất cả thay đổi trên ${productName}?`;
    }
    order.update.updateProductPriceConfirm = function (productName) {
        return `Bạn có chắc chắn cập nhật giá cho ${productName}?`;
    }
    order.update.removeProductConfirm = function (productName) {
        return `Bạn có chắc chắn sẽ xóa ${productName}?`;
    }
    order.update.saveSuccess = "Cập nhật hóa đơn hoàn tất.";
    order.update.duplicatedItem = "Sản phẩm đã được thêm vào. Vui lòng chọn sản phẩm khác.";
    order.update.addedProduct = "Đã thêm sản phẩm thành công.";

    order.create = {};
    order.create.saveSuccess = "Tạo hóa đơn hoàn tất.";
    order.create.confirmIsContinueCreate = "Tạo hóa hơn hoàn tất. Bạn muốn tiếp tục tạo hóa đơn, hay cập nhật hóa đơn vừa tạo?";

    messages.order = order;

    var settings = {};

    settings.article = {};
    settings.article.updateSuccess = "Lưu hoàn tất.";

    messages.settings = settings;

    messages.error = {
        connectionFail: "Cannot connect to server. Please check your network connection and try again."
    }

    return messages;
})();