if (CKEDITOR.env.ie && CKEDITOR.env.version < 9)
    CKEDITOR.tools.enableHtml5Elements(document);

CKEDITOR.config.height = 150;
CKEDITOR.config.width = 'auto';

var SettingFooterController = (function () {
    var actions = {};

    var initEditor = (function () {
        
        var wysiwygareaAvailable = isWysiwygareaAvailable(),
            isBBCodeBuiltIn = !!CKEDITOR.plugins.get('bbcode');

        return function (name) {
            var editorElement = CKEDITOR.document.getById(name);

            // :(((
            if (isBBCodeBuiltIn) {
                editorElement.setHtml(
                    'Hello world!\n\n' +
                    'I\'m an instance of [url=http://ckeditor.com]CKEditor[/url].'
                );
            }

            // Depending on the wysiwygare plugin availability initialize classic or inline editor.
            if (wysiwygareaAvailable) {
                CKEDITOR.replace(name);
            } else {
                editorElement.setAttribute('contenteditable', 'true');
                CKEDITOR.inline(name);

                // TODO we can consider displaying some info box that
                // without wysiwygarea the classic editor may not work.
            }
        };

        function isWysiwygareaAvailable() {
            // If in development mode, then the wysiwygarea must be available.
            // Split REV into two strings so builder does not replace it :D.
            if (CKEDITOR.revision == ('%RE' + 'V%')) {
                return true;
            }

            return !!CKEDITOR.plugins.get('wysiwygarea');
        }
    })();

    actions.init = function () {
        initEditor('aboutEditor');
    }

    return actions;
})();