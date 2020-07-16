$(document).ready(() => {
    $.fn.datepicker.defaults.format = 'mm/dd/yyyy';
    window.modal = $('#modalBackdrop');
    $('[data-toggle=popover]').popover();
    $('form[data-request=ajax]').xSubmit();
    $('input[type=file]').xUpload();

    $.fn.initModalLink('body');

}).ajaxStart(() => {
    $('form fieldset').disabled();
    $('div[role=toolbar]').find('a, button').disabled()
}).ajaxComplete(() => {
    $('form fieldset').enabled();
    $('div[role=toolbar]').find('a, button').enabled()
}).ajaxError((e, jqxhr, settings, thrownError) => {
    window.console.log('Error', jqxhr.responseText);
    alert(jqxhr.responseText);
});

$.fn.initModalLink = function (target, callback = {}) {
    $(target).find('a[data-target=modal]').on('click', e => {
        e.preventDefault();
        var link = $(e.target);
        var opt = {
            'url': link.attr('href')
        }

        $.ajax(opt).done((data, status, jqXHR) => {
            $(data).dialog(link.attr('tile') || 'Your action is required', callback);
        })
    });
};

$.extend($.fn.bootstrapTable.defaults, {
    classes: 'table table-hover',
    widthUnit: '%',
    sidePagination: 'server',
    toolbar: '#toolbar',
    showPaginationSwitch: false,
    silentSort: false,
    search: true,
    idField: 'id',
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
        errorClass: 'invalid-feedback',
        validClass: 'valid',
        errorElement: 'small',
        focusInvalid: true,
        errorContainer: $([]),
        errorLabelContainer: $([]),
        onsubmit: true,
        ignore: ':hidden',              // default for ignore in jquery.validate.js
        ignoreTitle: false,
        onfocusin: function (element, event) {
            this.lastActive = element;
        },
        errorPlacement: function (error, element) {
            var parent = $(element).closest('.input-group');
            if (parent != null && parent.length > 0) {
                element = parent;
            }
            error.insertAfter(element);
        }
    }
});