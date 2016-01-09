$(function () {
    $('#GekozenDepartement').change(departementSelectionChanged);

    $('#tbl_gebruikers').jtable({
        title: 'Gebruikers',
        sorting: true,
        defaultSorting: 'Achternaam ASC',
        selecting: true,
        selectOnRowClick: true,
        actions: {
            listAction: '/GebruikerKlants/ListGebruikers'
        },
        fields: {            
            Id: {
                key: true,
                list: false
            },
            Voornaam: {
                title: 'Voornaam',
                width: '20%'
            },
            Achternaam: {
                title: 'Achternaam',
                width: '20%'
            },
            Rol: {
                title: 'Rol',
                width: '20%',
                options: { '0': 'Administrator', '1': 'Departement Administrator', '2': 'User' },
                optionsSorting: 'value'
            },
        },
        selectionChanged: function () {
            gebruikerSelectionChanged();
        }
    });

    $('#tbl_gebruikers .jtable').css('cursor', 'pointer');

    $('#tbl_klanten').jtable({
        title: 'Klanten',
        sorting: true,
        defaultSorting: 'Bedrijfsnaam ASC',
        actions: {
            listAction: '/GebruikerKlants/ListKlanten',
            createAction: function (postData, jtParams) {
                return $.Deferred(function ($dfd) {
                    $.ajax({
                        url: '/GebruikerKlants/KoppelKlantAanGebruiker?gebruikerDepId=' + selectedGebruikerDepId().Id,
                        type: 'POST',
                        dataType: 'json',
                        data: postData,
                        success: function (data) {
                            $dfd.resolve(data);
                        },
                        error: function () {
                            $dfd.reject();
                        }
                    });
                });
            },
            deleteAction: '/GebruikerKlants/OntkoppelKlantVanGebruiker'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            depKlantId: {
                list: false,
                options: function (data) {
                    data.clearCache();
                    var gebruikerDepId = selectedGebruikerDepId().Id;
                    return 'GebruikerKlants/MogelijkeKlantenVoorGebruiker?gebruikerDepId=' + gebruikerDepId;
                },
                title: "Klant"
            }, 
            Ondernemingsnummer: {
                title: 'Ondernemingsnummer',
                width: '20%',
                create: false
            },
            Bedrijfsnaam: {
                title: 'Bedrijfsnaam',
                width: '80%',
                create: false
            }
        }
    });

    departementSelectionChanged();
});

function departementSelectionChanged() {
    var depId = $('#GekozenDepartement').val();    
    $('#tbl_gebruikers').jtable('load', { departementId: depId });


    // Onderste tabel leegmaken
    var ids = []
    $('#tbl_klanten tr').each(function (index, element) {
        if ($(this).data('record-key')) {
            ids.push($(this).data('record-key'));
        }
    })
    
    for (var i = 0; i < ids.length; i++) {
        $('#tbl_klanten').jtable('deleteRecord', {
            key: ids[i],
            clientOnly: true,
            animationsEnabled: false
        });
    }
}

function gebruikerSelectionChanged() {
    var result = selectedGebruikerDepId();
    var gebruikerDepId = result.Id;
    var naam = result.Naam;
    if (gebruikerDepId >= 0) {
        $('#tbl_klanten').jtable('load', { gebruikerDepId: gebruikerDepId });
        $('#tbl_klanten').find('.jtable-title-text').text(naam + ' - Gekoppelde Klanten');
    }
}

function selectedGebruikerDepId() {
    var selectedRow = $('#tbl_gebruikers').jtable('selectedRows');
    if (selectedRow.length == 0) {
        return -1;
    }
    var row = selectedRow[0];
    var record = $(row).data('record');
    return { Id: record.Id, Naam: record.Voornaam + ' ' + record.Achternaam };
}