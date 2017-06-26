var OrderManageController = (function () {
    var actions = {};

    var url = {};
    var searchObj = { "filterOptions": [3], "queryString": "" };

    var cacheOrder;
    var selectedOrder;

    actions.init = function (options) {
        var self = this;


        if (!options) {
            options = {};
        }

        url = {
            findAllByUserNameOrOrderCode: options.url.findAllByUserNameOrOrderCode,
            productDetail: options.url.productDetail,
            orderDetail: options.url.orderDetail,
            updateStatus: options.url.updateStatus,
            updateOrder: options.url.updateOrder
        }

        if ($("#filterOptionSelect").length) {
            $("#filterOptionSelect").select2({
                allowClear: true,
                placeholder: "Select..."
            }).on("change", function (e) {
                //                console.log(e.val);
            });
        }

        $("#queryText").keyup(function (e) {
            if (e.keyCode == 13) {
                self.search();
            }
        });

        $("#btnUpdateOrderDetail").on("click", function (e) {
            var url = $(this).attr("data-url");
            location.replace(url);
        });

        self.search({
            obj: searchObj
        });

    }

    actions.getStatusString = function (status) {
        switch (status) {
            case 1:
                return "Mới";
            case 2:
                return "Từ chối";
            case 3:
                return "Đã thanh toán";
            case 4:
                return "Đang vận chuyển";
            case 5:
                return "Hoàn tất";
            default:
                return "";
        }
    }

    actions.search = function (obj) {
        var self = this;

        var filterOptions = $("#filterOptionSelect").select2().val();
        var query = $("#queryText").val().trim();
        var tableBody = $("#orderTbody");

        if (!obj) {
            obj = { "filterOptions": filterOptions, "queryString": query };
        }

        searchObj = obj;

        $.ajax({
            type: "POST",
            url: url.findAllByUserNameOrOrderCode,
            data: obj,
            dataType: "json",
            traditional: true,
            success: function (data) {
                var ordersObj = JSON.parse(data);

                var orders = ordersObj.Orders;
                cacheOrder = orders;

                $(tableBody).empty();

                for (var i = 0; i < orders.length; i++) {
                    var order = orders[i];

                    var html = '<tr>' +
                        '           <td>' +
                        '               <a  data-toggle="modal" href="#orderDetailModal" onclick="OrderManageController.viewOrderDetail(' + order.OrderId + ')" title="View Order Detail" target="_blank">' +
                        '                   ' + order.Code +
                        '               </a>' +
                        '           </td>' +
                        '           <td>' +
                        '               <a href="#">' +
                        '                   ' + order.Account.FirstName + ' ' + order.Account.LastName +
                        '               </a>' +
                        '           </td>' +
                        '           <td>' + CurrencyConverter.toVietNamDong(order.TotalPrice) + '</td>' +
                        '           <td>' + DateTimeUtils.toVietnameseFormat(order.CreatedDate, true) + '</td>' +
                        '           <td>' + self.getStatusString(parseInt(order.Status)) + '</td>' +
                        '           <td>' +
                        '               <a data-toggle="modal" href="#orderDetailModal" title="View Order Details" onclick="OrderManageController.viewOrderDetail(' + order.OrderId + ')">' +
                        '                   <span class="glyphicon glyphicon-eye-open"></span>' +
                        '               </a>' +
                        '               <a data-toggle="modal" href="#updateOrderStatusModal" title="Update Order Status" ' +
                        '                   onclick="OrderManageController.viewOrderStatus(' + i + ')">' +
                        '                       <span class="li_truck"></span>' +
                        '               </a>' +
                        '               <a href="' + url.updateOrder + '/' + order.OrderId + '" title="Update order">' +
                        '                   <span class="glyphicon glyphicon-edit"></span>' +
                        '               </a>' +
                        '           </td>' +
                        '       </tr>';

                    $(tableBody).append($(html));
                }

                if ($('#orderTable').length > 0) {
                    //if ($('#orderTable').hasClass('dataTable')) {
                    //    $('#orderTable').dataTable().fnDestroy();
                    //}

                    $('#orderTable').dataTable({
                        "sPaginationType": "bootstrap",
                        "autoWidth": false,
                        "bDestroy": true,
                        "bServerSide": false,
                        "bLengthChange": false
                    });
                }
            },
            error: function (data, textStatus, errorThrown) {
            }
        });

        // Prevent form submit
        return false;
    }

    actions.viewOrderDetail = function (orderId) {
        var self = this;

        var lblOrderCode = $("#lblOrderCode");
        var lblOrderAccount = $("#lblOrderAccount");
        var lblOrderCreatedDate = $("#lblOrderCreatedDate");
        var lblOrderStatus = $("#lblOrderStatus");
        var tblOrderDetails = $("#tblOrderDetails");
        var tblOrderDetailsBody = $("#tblOrderDetailsBody");
        var lblOrderTotalPrice = $("#lblOrderTotalPrice");
        var btnUpdateOrderDetail = $("#btnUpdateOrderDetail");

        $.ajax({
            type: "POST",
            url: url.orderDetail,
            data: { "orderId": orderId },
            dataType: "json",
            success: function (data) {
                var order = JSON.parse(data);

                $(lblOrderCode).text(order.Code);
                $(lblOrderAccount).text(order.Account.FirstName + " " + order.Account.LastName);
                $(lblOrderCreatedDate).text(DateTimeUtils.toVietnameseFormat(order.CreatedDate));
                $(lblOrderStatus).text(self.getStatusString(order.Status));
                $(lblOrderTotalPrice).text(CurrencyConverter.toVietNamDong(order.TotalPrice));
                $(btnUpdateOrderDetail).attr("data-url", url.updateOrder + "/" + order.OrderId);

                //#region Table Body

                $(tblOrderDetailsBody).html("");

                for (var i = 0; i < order.OrderDetail.length; i++) {
                    var detail = order.OrderDetail[i];

                    var html = "<tr>" +
                        "           <td>" +
                        '               <a href="' + url.productDetail + '/' + detail.ProductId + '" target="_blank" title="' + detail.ProductName + '">' +
                        '                   ' + detail.ProductName +
                        '               </a>' +
                        "           </td>" +
                        "           <td style=\"text-align: center;\">" + detail.Quantity + "</td>" +
                        "           <td style=\"text-align: right;\">" + CurrencyConverter.toVietNamDong(detail.OriginUnitPrice) + "</td>" +
                        "           <td style=\"text-align: center;\">" + detail.Discount + "%</td>" +
                        "           <td style=\"text-align: right;\">" + CurrencyConverter.toVietNamDong(parseInt(detail.OriginUnitPrice) * parseInt(detail.Quantity)) + "</td>" +
                        "       </tr>";

                    $(tblOrderDetailsBody).append(html);
                }


                //#endregion

                console.log(order);
            }
        });
    }

    actions.viewOrderStatus = function (index) {
        var order = cacheOrder[index];

        var code = $("#lblUpdateOrderStatus-Code");
        var status = $("#lblUpdateOrderStatus-Status");

        $(code).text(order.Code);
        $(status).val(order.Status);

        selectedOrder = order;
    }

    actions.updateStatus = function () {
        var self = this;
        if (!confirm(ShopMessage.order.manage.confirmUpdateStatus)) {
            return;
        }

        var code = $("#lblUpdateOrderStatus-Code");
        var status = $("#lblUpdateOrderStatus-Status");

        $.ajax({
            type: "POST",
            url: url.updateStatus,
            data: { "orderId": selectedOrder.OrderId, "newStatus": $(status).val() },
            dataType: "json",
            success: function (data) {
                if (data && data.IsSuccess) {
                    NotificationUtil.successMessage(ShopMessage.order.manage.updateStatusSuccess);

                    self.search({
                        obj: searchObj
                    });

                } else {
                    NotificationUtil.warningMessage(ShopMessage.error.connectionFail);
                }
            }
        });
    }

    return actions;
})();