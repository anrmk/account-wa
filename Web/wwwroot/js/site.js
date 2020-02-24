$(document).ready(() => {
    $.fn.datepicker.defaults.format = "mm/dd/yyyy";
    window.modal = $('#modalBackdrop');

    var settings = {
        validClass: "is-valid",
        errorClass: "is-invalid"
    };
    $.validator.setDefaults(settings);
    $.validator.unobtrusive.options = settings;

});

$.fn.dialog = function (header, callback) {
    callback = callback || function () { };
    $.when(
        $('.modal .modal-title').text(header),
        $('.modal .modal-body').empty().html(this),
        //$('.modal .modal-footer').empty().html(footer),

        window.modal.modal('show').on('shown.bs.modal', (e) => {
            var form = $('.modal form');
            var submitBtn = $('.modal #modalSubmitBtn');
            if (form.length > 0) {
                submitBtn.attr('form', form.attr('id')).show();
            } else {
                submitBtn.hide();
            }
            //$(e.currentTarget).find('select.chosen-select').chosen();
        }).on('hidden.bs.modal', (e) => {
            this.empty();
        })
    ).done(callback(window.modal));
    return window.modal;
}

/**
 * Extension for bootstrapTable 
 * formatting Date
 */
$.fn.bootstrapTable.formatDate = function (value, row, index) {
    return value == null ? "" : moment(value, 'MM-DD-YYYY').format('MM-DD-YYYY');
};

$.extend($.fn.bootstrapTable.defaults, {
    classes: 'table table-hover',
    sidePagination: 'server',
    toolbar: '#toolbar',
    showPaginationSwitch: false,
    search: true,
    idField: "id",
    pageSize: 10,
    pageList: [10, 100, 500, 'All'],
    clickToSelect: true,
    //showSearchButton: true,
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