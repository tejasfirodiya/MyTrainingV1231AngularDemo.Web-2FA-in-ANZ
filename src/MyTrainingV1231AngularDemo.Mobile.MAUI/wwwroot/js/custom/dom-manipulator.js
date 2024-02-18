"use strict";

jQuery.fn.clearAllAttributes = function () {
    return this.each(function () {
        var attributes = $.map(this.attributes, function (item) {
            return item.name;
        });
        var img = $(this);
        $.each(attributes, function (i, item) {
            img.removeAttr(item);
        });
    });
}

// Class Definition
var BlazorDomManipulatorService = function () {
    var setAttribute = function (jquerySelector, attribute, value) {
        $(jquerySelector).attr(attribute, value);
    }

    var clearAllAttributes = function (jquerySelector) {
        $(jquerySelector).clearAllAttributes();
    }

    var clearModalBackdrop = function (jquerySelector) {
        $('.modal-backdrop').remove();
    }

    // Public Functions
    return {
        setAttribute: setAttribute,
        clearAllAttributes: clearAllAttributes,
        clearModalBackdrop: clearModalBackdrop
    };
}();