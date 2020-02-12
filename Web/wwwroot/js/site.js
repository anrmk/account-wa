$(document).ready(() => {
    $.fn.datepicker.defaults.format = "mm/dd/yyyy";

    var settings = {
        validClass: "is-valid",
        errorClass: "is-invalid"
    };
    $.validator.setDefaults(settings);
    $.validator.unobtrusive.options = settings;
});
