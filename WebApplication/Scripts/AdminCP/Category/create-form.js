//#region [Constant Values]
var ERROR = '0';
var NAME_EMPTY = '1';
var NAME_DUPLICATED = '2';
var NAME_VALIDATE = '3';
//#endregion

//#region [Controllers]
var Validator = function () {
    var actions = {};

    actions.validateCategoryName = function (callback) {
        var name = $("[name='Name']").val();
        var categoryId = $("#txtCategoryId").val();

        if (!name) {
            return callback(ERROR);
        }
        if (name.length === 0) {
            return callback(NAME_EMPTY);
        }

        $.ajax({
            type: "POST",
            url: "/Category/IsNameDuplicated",
            data: { "name": name, "categoryId" : categoryId },
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

    return actions;
};
//#endregion

//#region [Events]
$("#btnSubmit").click(function (e) {
    e.preventDefault();
    var validator = new Validator();
    var validatorUtil = new ValidatorUtil();
    //#region callback handler
    var nameValidateCallback = function (message) {
        switch (message) {
            case NAME_DUPLICATED:
            case ERROR:
                validatorUtil.UpdateInputFormUi(true, "Name", ValidateMessages.Validate.Category.Name.NameDuplicated);
                break;
            case NAME_EMPTY:
                validatorUtil.UpdateInputFormUi(true, "Name", ValidateMessages.Validate.Category.Name.NameEmpty);
                break;
            case NAME_VALIDATE:
                validatorUtil.UpdateInputFormUi(false, "Name", null);
                $("#createCategoryForm").submit();
                break;
        }
    }

    validator.validateCategoryName(nameValidateCallback);
});

function txtDelete(name) {
    return confirm("Are you sure to delete " + name + "?");
};
//#endregion