$(function () {

    // Departementen ophalen
    $.ajax({
        method: "POST",
        url: "/Factuurs/AlleDepartementen",
        dataType: "json",
        success: function (data, statusText, jqXHR) {
            $('#sel_departement').empty();
            for (var i = 0; i < data.length; i++) {
                $('#sel_departement').append(
                    $('<option>').attr('value', data[i].Id)
                                 .html(data[i].Omschrijving)
                    );
            }
            // NA TERUGKOMEN VAN DETAIL STAAT HERSTELLEN
            if (id != "") {
                $('#sel_departement').val(id);
            }
            departementSelectionChanged();
        }
    });

    $('#sel_departement').change(departementSelectionChanged);

    $('#tbl_facturen').jtable({
        title: 'Facturen',
        sorting: true,
        defaultSorting: 'Factuurnummer ASC',
        actions: {
            listAction: '/Factuurs/ListFacturen'
        },
        fields: {
                Id: {
                    key: true,
                    list: false
                },
                Factuurnummer: {
                    title: 'Factuurnummer',
                    width: '15%'
                },
                Factuurdatum: {
                    title: 'Factuurdatum',
                    width: '15%',
                    display: function (data) {
                        return format(data.record.Factuurdatum);
                    }
                },
                Klant: {
                    title: 'Klant',
                    width: '20%'
                },
                Departement: {
                    title: 'Departement',
                    width: '20%'
                },
                Totaal: {
                    title: 'Totaal',
                    width: '10%',
                    display: function (data) {
                        return "€ " + data.record.Totaal;
                    }
                },
                Details: {
                    title: 'Details',
                    width: '10%',
                    display: function (data) {
                        return $('<button>').data('factuurId', data.record.Id)
                                                    .click(toonFactuurClick)
                                                    .html('Toon');
                    },
                    sorting: false
                }
            
            }
        }
    );
});

function departementSelectionChanged() {
    var depId = $('#sel_departement').val();
    $('#tbl_facturen').jtable('load', { departementId: depId });
}

function toonFactuurClick() {
    var factuurId = $(this).data('factuurId')
    var postUrl = url.replace('__id__', factuurId);
    window.location.href = postUrl;    
}

function format(dateString) {
    if (dateString.search('Date') > 0) {
        return moment(dateString.replace(/\D/g, ''), 'x').format('DD/MM/YYYY');
    } else {
        return dateString;
    }
}

