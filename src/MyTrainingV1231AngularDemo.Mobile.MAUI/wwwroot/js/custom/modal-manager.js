
// Class Definition
var BlazorModalManagerService = function () {
    var show = function (id) {
        $("#" + id).modal('show')
    }

    var hide = function (id) {
        $("#" + id).modal('hide')
    }

    // Public Functions
    return {
        show: show,
        hide: hide
    };
}();