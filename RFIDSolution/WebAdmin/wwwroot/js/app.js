function validForm(formId) {
    var form = document.getElementById(formId)

    // Loop over them and prevent submission
    if (!form.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
        return false;
    }

    return true;
}

//Đặt tiêu đề cho trang
setTitle = (title) => {
    document.title = title;
    //Hỏi trước khi thoát nếu đang thêm kí lục mực
    if (window.location.pathname == "/paste/use") {
        window.onbeforeunload = confirmExit;
    }
    else if (window.location.pathname == "/sokumen/use") {

    }
    else {
        window.onbeforeunload = undefined;
    }
};
goback = () => { history.back(); };

function printDiv(id) {
    $(id).printThis();
}


//Hỗ trợ download file
downloadFromUrl = (url, fileName) => {
    const anchorElement = document.createElement('a');
    anchorElement.href = url + "?v=" + Date.now();
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}

//Ngăn submit form khi nhấn enter
$('body').on('keydown', 'input, select', function (e) {
    if (e.key === "Enter") {
        var self = $(this), form = self.parents('form:eq(0)'), focusable, next;

        focusable = form.find('input,a,select,button,textarea').filter(':visible').filter(':enabled');
        next = focusable.eq(focusable.index(this) + 1);
        if (next.length) {
            next.focus();
        } else {
            //form.submit();
        }
        return false;
    }
});
