mergeInto(LibraryManager.library, {
    CopyToClipboardWebGL: function (textPtr) {
        var text = UTF8ToString(textPtr);
        
        var textArea = document.createElement('textarea');
        textArea.value = text;
        textArea.style.position = 'fixed';
        textArea.style.top = '-99999px';
        textArea.style.left = '-99999px';
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        
        try {
            document.execCommand('copy');
        } catch (err) {
        }
        
        document.body.removeChild(textArea);
    }
});