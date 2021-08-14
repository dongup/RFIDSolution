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