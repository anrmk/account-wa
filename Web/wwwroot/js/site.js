$(document).ready(() => {
    $.fn.datepicker.defaults.format = "mm/dd/yyyy";
    window.modal = $('#modalBackdrop');
    $('[data-toggle=popover]').popover();

    $.fn.initModalLink('body');

}).ajaxStart(() => {
    $('form fieldset').disabled();
    $('div[role=toolbar]').find('a, button').disabled()
}).ajaxComplete(() => {
    $('form fieldset').enabled();
    $('div[role=toolbar]').find('a, button').enabled()
}).ajaxError((e, jqxhr, settings, thrownError) => {
    window.console.log("Error", jqxhr.responseText);
    alert(jqxhr.responseText);
});

$.fn.initModalLink = function (target, callback = {}) {
    $(target).find('a[data-target=modal]').on('click', e => {
        e.preventDefault();
        var opt = {
            'url': $(e.currentTarget).attr('href')
        }

        $.ajax(opt).done((data, status, jqXHR) => {
            $(data).dialog('Your action is required', callback);
        })
    });
};


$.fn.dialog = function (header, callback) {
    callback = callback || function () { };
    $.when(
        $('.modal .modal-title').text(header),
        $('.modal .modal-body').empty().html(this),

        window.modal.modal('show').off('shown.bs.modal').on('shown.bs.modal', (e) => {
            var form = $('.modal .modal-content form').on('submit');

            var submitBtn = $('.modal .modal-footer #modalSubmitBtn');
            if (form.length == 1) {
                submitBtn.attr('form', form.attr('id')).removeAttr('hidden');
            } else {
                submitBtn.attr('hidden', 'hidden');
            }
            callback("shown.bs.modal", e, this);
        }).off('hidden.bs.modal').on('hidden.bs.modal', (e) => {
            this.empty();
            callback("hidden.bs.modal", e, this);
        })
    ).done((e) => {
        callback("modal.on.load", e, this);
    });
    return window.modal;
};

/**
 * Extension for bootstrapTable 
 * formatting Date
 */
$.fn.bootstrapTable.formatDate = function (value, row, index) {
    return value == null ? "" : new Date(value).toLocaleDateString();
    //return value == null ? "" : moment(value, 'MM-DD-YYYY').format('MM-DD-YYYY');
};

$.fn.bootstrapTable.formatDateTime = function (value, row, index) {
    return value == null ? "" : new Date(value).toLocaleString();
};

$.fn.bootstrapTable.formatCurrency = function (value) {
    return value == null ? "" : "$" + value.toFixed(2).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,")
};

$.extend($.fn.bootstrapTable.defaults, {
    classes: 'table table-hover',
    widthUnit : "%",
    sidePagination: 'server',
    toolbar: '#toolbar',
    showPaginationSwitch: false,
    silentSort: false,
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
    maintainMetaData: true,
    filterOptions: {
        filterAlgorithm: 'or'
    }
});

//$.extend($.fn.bootstrapTable.columnDefaults, {
//    align: 'center',
//    valign: 'middle'
//})

$.extend($.serializeJSON.defaultOptions, {
    'parseNumbers': true,
    'useIntKeysAsArrayIndex': false,
    'customTypes': {
        'string:nullable': function (str) { return str || null; },
        'number:nullable': function (str) { return Number(str) || null; },
        //'date:month': function (str) {
        //    const [year, month, day] = str.split("-");
        //    return new Date(year, month - 1, day ?? 1).toDateString();
        //}
    }
});

$.extend($.validator, {
    defaults: {
        messages: {},
        groups: {},
        rules: {},
        errorClass: "is-invalid",
        validClass: "valid",
        errorElement: "label",
        focusInvalid: true,
        errorContainer: $([]),
        errorLabelContainer: $([]),
        onsubmit: true,
        ignore: ":hidden",              // default for ignore in jquery.validate.js
        ignoreTitle: false,
        onfocusin: function (element, event) {
            this.lastActive = element;
        }
    }
});

//var settings = {
//    debug: true,
//    validClass: 'is-valid',
//    errorClass: 'is-invalid'
//};
//$.validator.setDefaults(settings);
//$.validator.unobtrusive.options = settings;