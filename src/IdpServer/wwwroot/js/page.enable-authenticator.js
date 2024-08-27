$(document).ready(function () {
    $("#Token").on('input', function () {
        let value = $(this).val();
        if (value) value = value.trim();

        $('button[type="submit"]').prop('disabled', !Boolean(value));
    });
});
