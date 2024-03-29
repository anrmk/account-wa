﻿/*!
* v1.0.2
* https://github.com/anrmk
*
* Copyright 2020 Aziz Nurmukhamedov
*/

//Obsolete
//Get company summary range
$.fn.getCompanySummaryRange = function (id) {
    return $.ajax(`/api/company/${id}/summaryrange`);
};

//Obsolete
//Get bulk customers
$.fn.getBulkInvoices = function (id, from, to) {
    return $.ajax({
        url: '/api/invoice/unpaid',
        data: { 'id': id, 'from': from, 'to': to }
    });
}

$.fn.xLink = function (opt = {}) {
    this.on('click', e => {
        e.preventDefault();
        var $link = $(e.currentTarget);
        var $data = (typeof window[$link.attr('beforeSend') || 'xLinkBeforeSend'] === 'function') ? window[$link.attr('beforeSend') || 'xLinkBeforeSend'](this) : {}
        var options = $.extend({
            'url': $link.attr('href'),
            'type': $link.attr('method') || 'GET',
            'data': $data,
            'contentType': 'application/json; charset=utf-8',
            'traditional': true,
            'complete': (jqXHR, status) => {
                jqXHR.data = $data;
                $link.trigger('xLinkComplete', [jqXHR, status]);
            }
        }, opt);

        $.ajax(options);
    }).on('xLinkComplete', (e, jqXHR, status) => {
        e.preventDefault();
        var $link = $(e.currentTarget);
        var func = $link.attr('rel') || 'xLinkComplete';

        if (status === 'success') {
            $(`<div>${jqXHR.responseText}</div>`).dialog({ 'title': $link.attr('title') || 'Your action is required' });
        }
        if (typeof window[func] === 'function') {
            window[func](e, jqXHR, status);
        }
    });
    return this;
};

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
            var $data = $form.serializeJSON();
            var options = $.extend({
                'url': $form.attr('action'),
                'type': $form.attr('method'),
                'data': JSON.stringify($data),
                'contentType': 'application/json; charset=utf-8',
                'traditional': true,
                'beforeSend': (jqXHR, settings) => {
                    var func = $form.attr('beforeSend') || 'xSubmitBeforeSend';
                    if (typeof window[func] === 'function') {
                        jqXHR.data = $data;
                        window[func](jqXHR, settings);
                    }
                },
                'complete': (jqXHR, status) => {
                    jqXHR.data = $data;
                    $form.trigger('xSubmitComplete', [jqXHR, status]);
                }
            }, opt);
            $.ajax(options);
        }
    }).on('xSubmitComplete', (e, jqXHR, status) => {
        var $form = $(e.currentTarget);
        var func = $form.attr('rel') || 'xSubmitComplete';
        if (func === 'dialog') {
            if (status === 'success') {
                $(`<div>${jqXHR.responseText}</div>`).dialog({ 'title': 'Your action is required'});
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

$.fn.dialog = function (opt = {}) {
    var options = $.extend({
        'title': 'Your action is required',
        'content': this,
        'onShown': (e) => {
            var target = $(e.currentTarget);
            var links = target.find('a[data-request=ajax]').xLink();
            var form = target.find('form[data-request=ajax]').xSubmit();
            var formId = form.attr('id');

            var submitBtn = target.find('.modal-footer #modalSubmitBtn');
            if (form.length == 1 && form.attr('action') !== undefined && formId !== '00000000-0000-0000-0000-000000000000') {
                submitBtn.attr('form', formId).removeAttr('hidden');
            } else {
                submitBtn.attr('hidden', 'hidden');
            }

            if (typeof (window['xModalShown']) === 'function') {
                window['xModalShown'](e);
            }
        },
        'onHidden': (e) => {
            $(e.currentTarget).find('.modal-body').empty();
            if (typeof (window['xModalHidden']) === 'function') {
                window['xModalHidden'](e);
            }
        }
    }, opt);

    if (window.dialog === undefined) {
        window.dialog = $('.modal');
        window.dialog
            .on('shown.bs.modal', (e) => options.onShown(e))
            .on('hidden.bs.modal', (e) => options.onHidden(e));
    }

    window.dialog.find('.modal-title').text(options.title);
    window.dialog.find('.modal-body').html(options.content);
    window.dialog.modal('show');
        
    return window.dialog;
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
    return $(this);
}

$.fn.enabled = function () {
    $(this).removeAttr('disabled').removeClass('disabled');
    return $(this);
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