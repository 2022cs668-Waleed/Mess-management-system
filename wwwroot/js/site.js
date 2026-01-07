// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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

// AJAX Helper Functions
function showLoading() {
    $('body').append('<div class="spinner-overlay"><div class="spinner-border text-light" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function hideLoading() {
    $('.spinner-overlay').remove();
}

function showAlert(message, type = 'success') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    $('.container-fluid').prepend(alertHtml);
    
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);
}

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

// AJAX Error Handler
function handleAjaxError(xhr, status, error) {
    hideLoading();
    showAlert('An error occurred: ' + error, 'danger');
    console.error('AJAX Error:', xhr.responseText);
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
