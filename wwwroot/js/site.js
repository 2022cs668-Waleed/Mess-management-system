// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Common AJAX helpers
(function () {
    // Ensure jQuery sets X-Requested-With header (should be default, but enforce)
    $.ajaxSetup({
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    window.showLoading = function () {
        // implement a simple loading indicator
        if (!$('#globalLoading').length) {
            $('body').append('<div id="globalLoading" style="position:fixed;top:10px;right:10px;z-index:9999;">Loading...</div>');
        }
        $('#globalLoading').show();
    };

    window.hideLoading = function () {
        $('#globalLoading').hide();
    };

    window.showAlert = function (message, type) {
        // type: success, danger, info
        var alertHtml = '<div class="alert alert-' + (type || 'info') + ' alert-dismissible fade show" role="alert">' +
            message + '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>';
        $('.container-fluid').first().prepend(alertHtml);
    };

    window.handleAjaxError = function (xhr, status, error) {
        if (xhr.status === 401) {
            // For AJAX calls, avoid full redirect. Instead show login required message and reload after short delay.
            showAlert('Your session has expired or you are not authorized. Redirecting to login...', 'danger');
            setTimeout(function () { window.location = '/Account/Login'; }, 1200);
            return;
        }
        var msg = 'An error occurred';
        try {
            var json = JSON.parse(xhr.responseText);
            msg = json.message || xhr.statusText || msg;
        } catch (e) {
            msg = xhr.responseText || xhr.statusText || msg;
        }
        showAlert(msg, 'danger');
    };
})();

// Sidebar Toggle
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Active menu highlighting
    var currentPath = window.location.pathname;
    $('.sidebar a').each(function () {
        var href = $(this).attr('href');
        if (currentPath.indexOf(href) !== -1 && href !== '/') {
            $(this).addClass('active');
        }
    });
});

// Form Validation
function validateForm(formId) {
    let isValid = true;
    $(`#${formId} [required]`).each(function () {
        if ($(this).val() === '') {
            $(this).addClass('is-invalid');
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });
    return isValid;
}

// Date Formatting
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
}

// Currency Formatting
function formatCurrency(amount) {
    return 'Rs ' + parseFloat(amount).toFixed(2);
}

// Confirmation Dialog
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// DataTable Initialization (if using DataTables)
function initializeDataTable(tableId) {
    if ($.fn.DataTable) {
        $(`#${tableId}`).DataTable({
            responsive: true,
            pageLength: 10,
            ordering: true,
            searching: true,
            language: {
                search: "_INPUT_",
                searchPlaceholder: "Search..."
            }
        });
    }
}
