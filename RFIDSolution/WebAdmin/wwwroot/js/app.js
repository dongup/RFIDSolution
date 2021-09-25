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