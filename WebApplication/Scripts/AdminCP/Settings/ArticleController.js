var SettingsArticleController = (function() {
    var actions = {};

    actions.init = function() {
        
    }

    actions.submitArticleChanges = function () {
        var html = CKEDITOR.instances.editor.getData();
        $("#txtContent").text(html);
        return true;
    }

    return actions;
})();