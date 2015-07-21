$(function() {

    $('input[data-autocomplete="country"]').each(function (key, element) {
        $(element).typeahead({
            onSelect: function(item) {
                $(element).parent().closest('table').find('input[data-autocomplete="capital"]').val(item.value);
            },
            ajax: {
                url: '/api/Country/GetCountries',
                displayField: 'Name',
                valueField: 'Capital',
                method: "get",
                preDispatch: function(query) {
                    return {
                        query: query
                    }
                }
            },
            items: 20
        });
    });

});