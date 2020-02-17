$(document).ready(() => {
    $.fn.datepicker.defaults.format = "mm/dd/yyyy";

    var settings = {
        validClass: "is-valid",
        errorClass: "is-invalid"
    };
    $.validator.setDefaults(settings);
    $.validator.unobtrusive.options = settings;

});

/**
 * Extension for bootstrapTable 
 * formatting Date
 */
$.fn.bootstrapTable.formatDate = function (value, row, index) {
    return value == null ? "" : moment(value).format('MMM/DD/YYYY');
};

$.extend($.fn.bootstrapTable.defaults, {
    classes: 'table table-hover',
    sidePagination: 'server',
    toolbar: '#toolbar',
    showPaginationSwitch: false,
    search: true,
    searchOnEnterKey: true,
    showRefresh: true,
    showColumns: true,
    showToggle: true,
    sortStable: true,
    pagination: true,
    maintainMetaData: true
});

//$.extend($.fn.bootstrapTable.columnDefaults, {
//    align: 'center',
//    valign: 'middle'
//})