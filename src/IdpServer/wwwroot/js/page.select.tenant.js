$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'PAGE.SELECT_TENANT.FIELD.TENANT'
    ];
    let tenantRequired = 'Tenant is required.'
    getTranslations(keys, function (data) {
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        tenantRequired = requiredError.replace('{0}', data?.['PAGE.SELECT_TENANT.FIELD.TENANT'] ?? 'Tenant');
    });

    $("#user_tenant").select2({
        tags: true,
        minimumResultsForSearch: Infinity,
        selectionCssClass: 'select2-tenant-selection',
        dropdownCssClass: 'select2-tenant-dropdown',
    });
    $(".selection").click(function () {
        var arrow = document.getElementsByTagName('b')[0];
        arrow?.classList?.toggle('toggle-arrow');
    });
    $(window).mousedown(function (e) {
        const isBlur = ['select2-selection__rendered', 'select2-selection__placeholder'].includes(e.target.className);
        if (!isBlur) {
            var arrow = document.getElementsByTagName('b')[0];
            $(arrow)?.removeClass('toggle-arrow');
        }
    });

    $("#user_tenant").change(function () {
        if ($(this).val().replace(/0/g, '').replace(/-/g, '') !== '') {
            $('.form-label, .select2-tenant-selection').removeClass('val-error');
            $('.val-error-message').text('');
            $(this).closest('.form-group').find('label').removeClass("required");
        } else {
            $('.form-label, .select2-tenant-selection').addClass('val-error');
            $('.val-error-message').text(tenantRequired);
        }
    });

    $("#selectTenantForm").submit(function (e) {
        const selectTenant = $('#user_tenant');
        if (selectTenant.val().replace(/0/g, '').replace(/-/g, '') === '') {
            $('.form-label, .select2-tenant-selection').addClass('val-error');
            $('.val-error-message').text(tenantRequired);
            e.preventDefault();
        }
    });
});
