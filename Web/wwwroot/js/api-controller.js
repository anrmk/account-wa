/*!
* v1.0.1
* https://github.com/anrmk
*
* Copyright 2020 Aziz Nurmukhamedov
*/

//Get company summary range
$.fn.getCompanySummaryRange = function (id) {
    return $.ajax(`/api/company/${id}/summaryrange`);
};

//Get bulk customers
$.fn.getBulkCustomers = function (id, from, to) {
    return $.ajax({
        url: '/api/customer',
        //url: '/api/customer/bulk',
        data: { 'id': id, 'from': from, 'to': to }
    });
}

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
        'processData': false
    };

    $.ajax(options).done((data, status, jqXHR) => {
        callback(this, data, status, jqXHR);
    }).fail((jqXHR, status) => {
        callback(this, null, status, jqXHR);
    });
}

//Submit form using jquery ajax
$.fn.ajaxSubmit = function (opt, callback) {
    var $form = this;

    if ($form.valid()) {
        var data = $form.serializeJSON();

        var options = $.extend({
            'url': $form.attr('action'),
            'type': $form.attr('method'),
            'data': data,
            //'processData': false,
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

$.fn.close = function () {
    $(this).closest('.card').fadeOut();
}

$.fn.randomDate = function (from, to) {
    const fromTime = from.getTime();
    const toTime = to.getTime();
    return new Date(fromTime + Math.random() * (toTime - fromTime));
}

$.fn.disabled = function () {
    $(this).attr('disabled', 'disabled');
}

$.fn.enabled = function () {
    $(this).removeAttr('disabled');
}