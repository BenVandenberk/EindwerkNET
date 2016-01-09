$(function () {
    $('#tbl_klanten').jtable({
        title: 'Klanten',
        selecting: true,
        selectOnRowClick: true,
        sorting: true,
        defaultSorting: 'Bedrijfsnaam ASC',
        actions: {
            listAction: '/Klants/List',            
            updateAction: '/Klants/Update',
            deleteAction: '/Klants/Delete'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            Ondernemingsnummer: {
                title: 'Ondernemingsnummer',
                width: '20%'
            },
            Bedrijfsnaam: {
                title: 'Bedrijfsnaam',
                width: '20%'
            },
            Adres: {
                title: 'Adres',
                width: '30%'
            },
            Postcode: {
                title: 'Postcode',
                width: '10%'
            },
            Plaats: {
                title: 'Plaats',
                width: '20%'
            }
        },
        selectionChanged: function () {
            klantSelectionChanged();
        }
    });

    $('#tbl_departementen').jtable({
        title: 'Gekoppelde Departementen',
        sorting: true,
        defaultSorting: 'Omschrijving ASC',
        actions: {
            listAction: '/Klants/ListDepartementen',
            createAction: function (postData, jtParams) {
                    return $.Deferred(function ($dfd) {
                            $.ajax({
                                url: '/Klants/KoppelDepartementAanKlant?klantId=' + selectedKlantId().Id,
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
            deleteAction: function (postData, jtParams) {
                return $.Deferred(function ($dfd) {
                    $.ajax({
                        url: '/Klants/OntkoppelDepartementVanKlant?klantId=' + selectedKlantId().Id,
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
            }
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            DepartementId: {
                title: 'Departement',
                options: function (data) {
                    data.clearCache();
                    var klantId = selectedKlantId().Id;
                    return '/Klants/MogelijkeDepartementen?klantId=' + klantId;                    
                },
                list: false
            },
            Code: {
                title: 'Code',
                width: '30%',
                create: false
            },
            Omschrijving: {
                title: 'Omschrijving',
                width: '70%',
                create: false
            }
        }
    });    

    $('#tbl_klanten').jtable('load');
    $('#tbl_klanten .jtable').css('cursor', 'pointer');

    $('#btn_maak').button();
});

function reloadKlanten() {
    $('#tbl_klanten').jtable('reload');    
}

function successHandler(data) {
    if (data.Result == 'ERROR') {
        error(data.Message);
    }
}

function klantSelectionChanged() { 
    var result = selectedKlantId();
    var klantId = result.Id;
    var naam = result.Naam;
    if (klantId >= 0) {        
        $('#tbl_departementen').jtable('load', { klantId: klantId });
        $('#tbl_departementen').find('.jtable-title-text').text(naam + ' - Gekoppelde Departementen');        
    }
}

function selectedKlantId() {
    var selectedRow = $('#tbl_klanten').jtable('selectedRows');
    if (selectedRow.length == 0) {
        return -1;
    }
    var row = selectedRow[0];
    var record = $(row).data('record');
    return {Id : record.Id, Naam: record.Bedrijfsnaam };
}