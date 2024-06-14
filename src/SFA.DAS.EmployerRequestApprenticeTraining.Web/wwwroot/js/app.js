// AUTOCOMPLETE

let $id = 'search-location'
let $input = $('#' + $id);

let $submitOnConfirm = $input.data('submit-on-selection');
let $defaultValue = $input.data('default-value');
let $name = $input.attr('name');

if ($input.length > 0) {
    $input.wrap('<div id="autocomplete-container"></div>');
    let container = document.querySelector('#autocomplete-container');
    let apiUrl = '/locations';
    $(container).empty();
    function getSuggestions(query, updateResults) {
        let results = [];
        $.ajax({
            url: apiUrl,
            type: "get",
            dataType: 'json',
            data: { searchTerm: query }
        }).done(function (data) {
            results = data.locations.map(function (r) {
                return { name: r.name };
            });
            updateResults(results);
        });
    }
    function onConfirm(selectedItem) {
        let currentElement = this.element;

        // traverse up the DOM to find the nearest form
        while (currentElement && currentElement.tagName.toLocaleLowerCase() !== 'form') {
            currentElement = currentElement.parentElement;
        }

        if (currentElement && currentElement.tagName.toLocaleLowerCase() === 'form' && $submitOnConfirm) {
            setTimeout(function () {
                currentElement.submit();
            }, 200);
        }
    }

    // Initialize accessibleAutocomplete with the custom input template
    accessibleAutocomplete({
        element: container,
        id: $id,
        name: $name,
        displayMenu: 'overlay',
        showNoOptionsFound: false,
        minLength: 3,
        source: getSuggestions,
        placeholder: "",
        onConfirm: onConfirm,
        defaultValue: $defaultValue,
        confirmOnBlur: false,
        autoselect: true,
        templates: {
            inputValue: function (suggestion) {
                if (typeof suggestion === 'string') {
                    return suggestion;
                }
                return suggestion && suggestion.name ? suggestion.name : '';
            },
            suggestion: function (suggestion) {
                if (typeof suggestion === 'string') {
                    return '';
                }

                return suggestion.name;
            }
        }
    });
}