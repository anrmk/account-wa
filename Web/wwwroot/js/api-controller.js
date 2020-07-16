/*!
* v1.0.2
* https://github.com/anrmk
*
* Copyright 2020 Aziz Nurmukhamedov
*/

//Get company summary range
$.fn.getCompanySummaryRange = function (id) {
    return $.ajax(`/api/company/${id}/summaryrange`);
};

//Get bulk customers
$.fn.getBulkInvoices = function (id, from, to) {
    return $.ajax({
        url: '/api/invoice/unpaid',
        data: { 'id': id, 'from': from, 'to': to }
    });
}


$.fn.uploadFile = function (callback) {
    var formData = new FormData();
    $.each(this.prop('files'), function (key, value) {
        formData.append(key, value);
    });

    var options = {
        'url': this.attr('action'),
        'type': this.attr('method'),
        'data': formData,
        'cache': false,
        'contentType': false,
        'processData': false,
        'complete': (jqXHR, status) => {

        }
    };

    $.ajax(options).done((data, status, jqXHR) => {
        callback(this, data, status, jqXHR);
    }).fail((jqXHR, status) => {
        callback(this, null, status, jqXHR);
    });
}

$.fn.xUpload = function (opt = {}) {
    this.on('change', (e) => {
        e.preventDefault();
        var $target = $(e.currentTarget);
        var formData = new FormData();
        $.each($target.prop('files'), function (key, value) {
            formData.append(key, value);
        });

        var options = {
            'url': $target.attr('action'),
            'type': $target.attr('method'),
            'data': formData,
            'cache': false,
            'contentType': false,
            'processData': false,
            'complete': (jqXHR, status) => {
                $target.trigger('xUploadComplete', [jqXHR, status]).val(null);
            }
        };
        $.ajax(options);
    }).on('xUploadComplete', (e, jqXHR, status) => {
        e.preventDefault();
        var func = $(e.currentTarget).attr('rel') || 'xUploadComplete';
        if (typeof window[func] === 'function') {
            window[func](e, jqXHR, status);
        }
    });
};

$.fn.xSubmit = function (opt = {}) {
    this.on('submit', (e) => {
        e.preventDefault();
        var $form = $(e.currentTarget);
        if ($form.valid()) {
            var options = $.extend({
                'url': $form.attr('action'),
                'type': $form.attr('method'),
                'data': JSON.stringify($form.serializeJSON()),
                'contentType': 'application/json; charset=utf-8',
                'complete': (jqXHR, status) => {
                    $form.trigger('xSubmitComplete', [jqXHR, status]);
                }
            }, opt);
            $.ajax(options);
        }
    }).on('xSubmitComplete', (e, jqXHR, status) => {
        e.preventDefault();
        var func = $(e.currentTarget).attr('rel') || 'xSubmitComplete';
        if (func === 'dialog') {
            if (status === 'success') {
                $(`<div>${jqXHR.responseText}</div>`).dialog();
            }
        } else if (typeof window[func] === 'function') {
            window[func](e, jqXHR, status);
        }
    });
    return this;
};

//Submit form using jquery ajax
$.fn.ajaxSubmit = function (opt, callback) {
    var $form = this;

    if ($form.valid()) {
        var data = $form.serializeJSON();

        var options = $.extend({
            'url': $form.attr('action'),
            'type': $form.attr('method'),
            'data': data,
            'traditional': true,
            'contentType': 'application/json; charset=utf-8'
        }, opt);

        if (options.type.toLowerCase() === 'post') {
            options.data = JSON.stringify(data);
        }

        $.ajax(options).done((data, status, jqXHR) => {
            callback($form, data, status, jqXHR);
        }).fail((jqXHR, status) => {
            callback($form, null, status, jqXHR);
        });
    }
}

$.fn.dialog = function (header, callback) {
    callback = callback || function () { };
    $.when(
        $('.modal .modal-title').text(header),
        $('.modal .modal-body').empty().html(this),

        window.modal.modal('show').off('shown.bs.modal').on('shown.bs.modal', (e) => {
            var form = $('.modal .modal-content form').xSubmit();
            var formId = form.attr('id');

            var submitBtn = $('.modal .modal-footer #modalSubmitBtn');
            if (form.length == 1 && form.attr('action') !== undefined && formId !== '00000000-0000-0000-0000-000000000000') {
                submitBtn.attr('form', formId).removeAttr('hidden');
            } else {
                submitBtn.attr('hidden', 'hidden');
            }
            callback('shown.bs.modal', e, this);
        }).off('hidden.bs.modal').on('hidden.bs.modal', (e) => {
            this.empty();
            callback('hidden.bs.modal', e, this);
        })
    ).done((e) => {
        callback('modal.on.load', e, this);
    });
    return window.modal;
};

$.fn.guid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

$.fn.close = function () {
    $(this).closest('.card').fadeOut();
}

$.fn.randomDate = function (from, to) {
    const fromTime = from.getTime();
    const toTime = to.getTime();
    return new Date(fromTime + Math.random() * (toTime - fromTime));
}

$.fn.disabled = function () {
    $(this).attr('disabled', 'disabled').addClass('disabled');
}

$.fn.enabled = function () {
    $(this).removeAttr('disabled').removeClass('disabled');
}

$.fn.check = function () {
    $(this).prop('checked', true);
}

$.fn.uncheck = function () {
    $(this).prop('checked', false);
}

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