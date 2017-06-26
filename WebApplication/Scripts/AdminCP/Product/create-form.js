/**
 * Created by Henry - Product Handler
 * 
 */

//#region [Constant Values]
var ERROR = '0';
var NAME_EMPTY = '1';
var NAME_DUPLICATED = '2';
var NAME_VALIDATE = '3';
//#endregion

var validatorUtil = new ValidatorUtil();

//#region [Controllers]
var Validator = function () {
    var actions = {};

    actions.validateCategory = function () {
        var category = $("#category_select");

        if (!category) {
            return false;
        }

        if ($(category).val() <= 0) {
            return false;
        }

        return true;

    };

    actions.validateProductName = function (callback) {
        var name = $("[name='Name']");
        var productId = $("[name='Id']").val();

        if (!name) {
            return callback(ERROR);
        }
        if ((name).val().length === 0) {
            return callback(NAME_EMPTY);
        }

        $.ajax({
            type: "POST",
            url: "/Product/IsNameDuplicated",
            data: { "name": $(name).val(), "productId": productId },
            dataType: "json",
            success: function (data) {
                if (data) {
                    if (data.isDuplicated === false) {
                        return callback(NAME_VALIDATE);
                    }
                }
                return callback(NAME_DUPLICATED);

            },
            error: function (data, textStatus, errorThrown) {
                return callback(NAME_DUPLICATED);
            }
        });
    };

    actions.validateProductPrice = function () {
        var price = $("[name='Price']");

        if (price) {
            if (parseFloat($(price).val()) < 0) {
                $(price).focus();
                return false;
            }

            return true;;
        }

        return false;
    };

    actions.validateProductQuantity = function () {
        var quantity = $("[name='Quantity']");

        if (quantity) {
            if (parseInt($(quantity).val()) < 0) {
                $(quantity).focus();
                return false;
            }

            return true;;
        }

        return false;
    };

    actions.validateShortDescription = function() {
        var shortDescription = $("[name='ShortDescription']");

       if (shortDescription) {
           if ($(shortDescription).val().trim() == "") {
               $(shortDescription).focus();
               return false;
           }

           return true;;
       }

        return false;
    }

    return actions;
};
//#endregion

//#region [Events]
$("#btnSubmit").click(function (e) {
    e.preventDefault();
    var validator = new Validator();
    var htmlCategory = "CategoryId";
    var htmlName = "Name";
    var htmlPrice = "Price";
    var htmlQuantity = "Quantity";
    var htmlShortDescription = "short-description";

    //#region callback handler
    var nameValidateCallback = function (message) {
        switch (message) {
            case NAME_DUPLICATED:
            case ERROR:
                validatorUtil.UpdateInputFormUi(true, htmlName, ValidateMessages.Validate.Product.Name.NameDuplicated);
                break;
            case NAME_EMPTY:
                validatorUtil.UpdateInputFormUi(true, htmlName, ValidateMessages.Validate.Product.Name.NameEmpty);
                break;
            case NAME_VALIDATE:
                validatorUtil.UpdateInputFormUi(false, htmlName, null);

                //#region Validate Price
                if (!validator.validateProductPrice()) {
                    validatorUtil.UpdateInputFormUi(true, htmlPrice, ValidateMessages.Validate.Product.Price.Invalid);
                    return;
                }

                validatorUtil.UpdateInputFormUi(false, htmlPrice, "");
                //#endregion

                //#region Validate Short Derscription
                if (!validator.validateShortDescription()) {
                    validatorUtil.UpdateInputFormUi(true, htmlShortDescription, ValidateMessages.Validate.Product.ShortDescription.Empty);
                    return;
                }

                validatorUtil.UpdateInputFormUi(false, htmlShortDescription, "");
                //#endregion

                //#region Validate Quantity
                if (!validator.validateProductQuantity()) {
                    validatorUtil.UpdateInputFormUi(true, htmlQuantity, ValidateMessages.Validate.Product.Quantity.Invalid);
                    return;
                }

                validatorUtil.UpdateInputFormUi(false, htmlQuantity, "");
                //#endregion

                // Binding CKEditor Data
                $("[name='Description']").val(CKEDITOR.instances.editor.document.getBody().getHtml());

                $("#createProductForm").submit();
                break;
        }
    }

    //#region [Validate Category]
    if (!validator.validateCategory()) {
        validatorUtil.UpdateInputFormUi(true, htmlCategory, ValidateMessages.Validate.Product.Category.Invalid);
        return;
    }

    validatorUtil.UpdateInputFormUi(false, htmlCategory, null);
    //#endregion

    validator.validateProductName(nameValidateCallback);
});

function addMoreImage() {
    var imageDiv = '<div class="col-sm-4">' +
                   '    <div class="fileupload fileupload-new" data-provides="fileupload">' +
                   '        <div class="fileupload-preview img-thumbnail" style="width: 180px; height: 120px;"></div>' +
                   '        <div>' +
                   '            <span class="btn btn-default btn-file">' +
                   '                <span class="fileupload-new">Select image</span><span class="fileupload-exists">Change</span><input type="file" name="Gallery" />' +
                   '            </span>' +
                   '            <a href="#" class="btn btn-default fileupload-exists" data-dismiss="fileupload">Remove</a>' +
                   '        </div>' +
                   '    </div>' +
                   '</div>';

    $("#galleryFieldset").append($(imageDiv));

    $("#galleryFieldset").animate({ scrollTop: $("#galleryFieldset")[0].scrollHeight });
};

function deleteCover() {
    if (!confirm("Bạn có chắc sẽ xóa hình này để tải lại hình mới lên?")) {
        return;
    }

    $("#divCoverUpload").show();
    $("#div-display-saved-image").hide();
    $('[name="CoverId"').val(-1);
}

function deleteGallery(id) {
    if (!confirm("Bạn có chắc sẽ xóa hình này để tải lại hình mới lên?")) {
        return;
    }

    $("#gallary-" + id).hide();
    $("#gallary-id-" + id).val(-1);
}
//#endregion