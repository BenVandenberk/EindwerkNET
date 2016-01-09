var departementKlanten = [];
var huidigeDepKlantId;

$(function () {
    $('#sel_klanten').autocomplete({
        source: [],
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    });

    $('.selDate').datepicker({
        dateFormat: "dd/mm/yy"
    });

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
            departementSelectionChanged();
        }
    });

    $('#sel_departement').change(departementSelectionChanged);

    $('#btn_toon').click(toonUurRegistraties);
    $('#btn_factureer').click(postSelectie);

    $('#tbl_uurregistraties').jtable({
        title: 'Gevonden Uurregistraties',
        selectingCheckboxes: true,
        selecting: true,
        multiselect: true,
        loadingAnimationDelay: 0,
        actions: {
            listAction: '/Factuurs/ZoekUurRegistraties'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            Titel: {
                title: "Titel",
                width: '30%'
            },
            Omschrijving: {
                title: "Omschrijving",
                width: '40%'
            },
            Gebruiker: {
                title: "Gebruiker",
                width: "30%"
            }
        }
    });

    if (errorMessage != "") {
        error(errorMessage);
    }
});

function departementSelectionChanged() {
    var depId = $('#sel_departement').val();
    $.ajax({
        method: "POST",
        url: "/Factuurs/AlleDepartementKlanten",
        data: "departementId=" + depId,
        dataType: "json",
        success: function (data, textStatus, jqXHR) { // ERROR!!!!
            departementKlanten = data;
            var klanten = [];
            for (var i = 0; i < data.length; i++) {
                klanten[i] = data[i].Bedrijfsnaam;
            }
            $('#sel_klanten').autocomplete('option', 'source', klanten);
        }
    });
}

function getDepartementKlantId() {
    var naam = $('#sel_klanten').val();

    for (var i = 0; i < departementKlanten.length; i++) {
        if (departementKlanten[i].Bedrijfsnaam == naam) {
            return departementKlanten[i].Id;
        }
    }

    return -1;
}

function toonUurRegistraties() {    
    huidigeDepKlantId = getDepartementKlantId();
    var van = $('#in_van').val();
    var tot = $('#in_tot').val();

    if (van != "" && tot != "") {
        var vanDate = new Date(van);
        var totDate = new Date(tot);
        if (vanDate > totDate) {
            error("Datum 'van' moet eerder zijn dan datum 'tot'");
            return;
        }
    }

    if (huidigeDepKlantId >= 0) {
        $('#tbl_uurregistraties').jtable('load', { departementKlantId: huidigeDepKlantId, van: van, tot: tot });
    } else {
        error("Ongeldige klant. Controleer of je een voorgestelde klant hebt aangeklikt");
    }
}

function postSelectie() {
    var idString = "";
    var selectedRows = $('#tbl_uurregistraties').jtable('selectedRows');

    if (selectedRows.length == 0) {
        error("Er is geen enkele uurregistratie geselecteerd om te factureren");
        return;
    }

    var records = [];
    var i = 0;
    selectedRows.each(function () {
        records[i++] = $(this).data('record');
    });

    for (var j = 0; j < records.length; j++) {
        if (j != records.length - 1) {
            idString += records[j].Id + ",";
        } else {
            idString += records[j].Id;
        }
    }

    var form = $('<form action="' + url + '" method="post" style="display:none">' +
    '<input type="text" name="uurRegistratieIds" value="' + idString + '" />' +
    '<input type="text" name="departementKlantId" value="' + huidigeDepKlantId + '" />' +
    '</form>');
    $('body').append(form);
    form.submit();
}