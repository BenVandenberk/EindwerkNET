$(function () {
    $('#GekozenDepartement').change(departementSelectionChanged);
    $('#sel_gebruikers').change(gebruikerDepartementSelectionChanged);

    $('#tbl_klanten').jtable({
        title: 'Klanten',
        selecting: true,
        selectOnRowClick: true,
        sorting: true,
        defaultSorting: 'Bedrijfsnaam ASC',
        actions: {
            listAction: '/UurRegistraties/ListGebruikerKlanten'
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
                width: '80%'
            }
        },
        selectionChanged: klantSelectionChanged
    });

    $('#tbl_uurregistraties').jtable({
        title: 'Uurregistraties',
        sorting: true,
        defaultSorting: 'Titel ASC',
        actions: {
            listAction: '/UurRegistraties/ListUurRegistraties?gefactureerd=true',
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            Titel: {
                title: 'Titel',
                width: '20%',
                inputClass: 'validate[required]'
            },
            Omschrijving: {
                title: 'Omschrijving',
                width: '70%',
                input: function (data) {
                    if (data.record) {
                        return '<textarea name="Omschrijving" style="width:350px" class="validate[required]">' + data.value + '</textarea>';
                    } else {
                        return '<textarea name="Omschrijving" style="width:350px" class="validate[required]"></textarea>';
                    }
                }
            },
            Details: {
                title: 'Details',
                width: '10%',
                display: function (data) {
                    return $('<button>').data('uurRegistratieId', data.record.Id)
                                              .click(uurRegistratieDetailsClick)
                                              .html('Details');
                },
                create: false,
                edit: false,
                sorting: false
            }
        },
        selectionChanged: function () {
            gebruikerSelectionChanged();
        },
        formCreated: function (event, data) {
            data.form.validationEngine();
            data.form.parent().css('width', '400px');
            data.form.css('margin-top', '30px');
        },
        formSubmitting: function (event, data) {
            return data.form.validationEngine('validate');
        },
        formClosed: function (event, data) {
            data.form.validationEngine('hide');
            data.form.validationEngine('detach');
        }
    });

    $('#tbl_klanten .jtable').css('cursor', 'pointer');

    // STAAT HERSTELLEN
    if (depId != "") {
        $('#GekozenDepartement').val(depId);
    }

    departementSelectionChanged();
});

function departementSelectionChanged() {
    var depId = $('#GekozenDepartement').val();

    // Gebruiker combobox inladen
    $.ajax({
        method: "POST",
        url: "/UurRegistraties/ListGebruikers",
        dataType: "json",
        data: "departementId=" + depId,
        success: function (data, statusText, jqXHR) {            
            if (data.Records.length == 0) {
                emptyKlanten();
                emptyUurRegistraties();
                $("#sel_gebruikers").empty();
            } else {
                var gebruikerDepartementen = data.Records;
                var option;
                $("#sel_gebruikers").empty();
                for (var i = 0; i < gebruikerDepartementen.length; i++) {
                    option = $("<option>").attr("value", gebruikerDepartementen[i].Id)
                                              .html(gebruikerDepartementen[i].Gebruiker);
                    $("#sel_gebruikers").append(option);
                }
                if (gebDepId != "") { // STAAT HERSTELLEN
                    $("#sel_gebruikers").val(gebDepId);
                    gebDepId = "";
                }
                gebruikerDepartementSelectionChanged();
            }
        }
    });
}

function gebruikerDepartementSelectionChanged() {
    var gebrDepId = $('#sel_gebruikers').val();

    // Klanten tabel inladen
    $('#tbl_klanten').jtable('load', { gebruikerDepartementId: gebrDepId }, function () {
        // STAAT HERSTELLEN
        if (gebKlantId != "") {
            var row;
            $('#tbl_klanten tr').each(function (index, element) {
                if ($(this).data('record-key')) {
                    if ($(this).data('record-key') == gebKlantId) {
                        row = $(this).get();
                        return false;
                    }
                }
            });
            if (row) {
                $('#tbl_klanten').jtable('selectRows', $(row));
                gebKlantId = "";
            }
        }
    });
    emptyUurRegistraties();
}

function klantSelectionChanged() {
    var result = selectedGebruikerKlantId();
    var gebruikerKlantId = result.Id;
    var naam = result.Naam;
    if (gebruikerKlantId >= 0) {
        $('#tbl_uurregistraties').jtable('load', { gebruikerKlantId: gebruikerKlantId });
        $('#tbl_uurregistraties').find('.jtable-title-text').text(naam + ' - Uurregistraties');
    }
}

function selectedGebruikerKlantId() {
    var selectedRow = $('#tbl_klanten').jtable('selectedRows');
    if (selectedRow.length == 0) {
        return -1;
    }
    var row = selectedRow[0];
    var record = $(row).data('record');
    var gebruiker = $('#sel_gebruikers > option:selected').html();
    return { Id: record.Id, Naam: gebruiker + " -> " + record.Bedrijfsnaam };
}

function uurRegistratieDetailsClick(event) {
    var uurRegistratieId = $(this).data('uurRegistratieId')
    var postUrl = url.replace('__id__', uurRegistratieId);

    var form = $('<form action="' + postUrl + '" method="post" style="display:none">' +
    '<input type="text" name="uurRegistratieId" value="' + uurRegistratieId + '" />' +
    '<input type="text" name="gefactureerd" value="true" />' +
    '</form>');
    $('body').append(form);
    form.submit();
}

function emptyUurRegistraties() {
    // UurRegistratietabel leegmaken
    var ids = []
    $('#tbl_uurregistraties tr').each(function (index, element) {
        if ($(this).data('record-key')) {
            ids.push($(this).data('record-key'));
        }
    })

    for (var i = 0; i < ids.length; i++) {
        $('#tbl_uurregistraties').jtable('deleteRecord', {
            key: ids[i],
            clientOnly: true,
            animationsEnabled: false
        });
    }
    $('#tbl_uurregistraties').find('.jtable-title-text').text('Uurregistraties');
}

function emptyKlanten() {
    // Klantentabel leegmaken
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