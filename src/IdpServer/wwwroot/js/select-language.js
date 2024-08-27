function formatCountry(state) {
    if (!state.id) { return state.text; }

    var baseUrl = "https://cdn.ahi.apps.yokogawa.com/languages/_flags";
    var $state = $(
        '<span><img class="country-flag" /> <span></span></span>'
    );

    $state.find("span").text(state.text);
    $state.find("img").attr("src", baseUrl + "/" + state.element.value.toLowerCase() + ".svg");
    return $state;
};

$(document).ready(function () {
    $("#language").change(function () {
        $("#languageForm").submit();
    });

    $("#language").select2({
        tags: true,
        minimumResultsForSearch: Infinity,
        selectionCssClass: 'select2-language-selection',
        templateResult: formatCountry,
        templateSelection: formatCountry
    });
});
