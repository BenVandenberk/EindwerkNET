$(function () {    

    $('#tbl_details').jtable({
        title: 'Uurregistratie - Details',
        actions: {
            listAction: '/UurRegistraties/ListDetails?uurRegistratieId=' + id,
            createAction: '/UurRegistraties/NieuwUurRegistratieDetail?uurRegistratieId=' + id,
            deleteAction: '/UurRegistraties/DeleteDetail',
            updateAction: '/UurRegistraties/UpdateDetail'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            StartTijd: {
                title: 'Starttijd',
                width: '20%',
                input: function (data) {
                    if (data.record) {
                        var formatted = format(data.record.StartTijd);
                        return '<input type="text" name="StartTijd" id="dtp_start" class="dtp validate[funcCall[checkStartTijd],required]" value="' + formatted + '" />';
                    } else {
                        return '<input type="text" name="StartTijd" id="dtp_start" class="dtp validate[funcCall[checkStartTijd],required]" value="" />';
                    }
                },
                display: function (data) {
                    return format(data.record.StartTijd);
                }
            },
            EindTijd: {
                title: 'Eindtijd',
                width: '20%',
                input: function (data) {
                    if (data.record) {
                        var formatted = format(data.record.EindTijd);
                        return '<input type="text" name="EindTijd" id="dtp_eind" class="dtp validate[funcCall[checkEindTijd],required]" value="' + formatted + '" />';
                    } else {
                        return '<input type="text" name="EindTijd" id="dtp_eind" class="dtp validate[funcCall[checkEindTijd],required]" value="" />';
                    }
                },
                display: function (data) {
                    return format(data.record.EindTijd);
                }
            },
            TariefId: {
                list: false,
                title: 'Type werk',
                options: '/UurRegistraties/AlleTarieven'
            },
            TypeWerk: {
                title: 'Type werk',
                width: '30%',
                create: false,
                edit: false
            },
            UurWaarde: {
                title: 'Uurwaarde',
                width: '20%',
                create: false,
                edit: false,
                display: function (data) {
                    return "€ " + data.record.UurWaarde;
                }
            },
            TeFactureren: {
                title: 'Te Factureren',
                width: '10%',
                display: function (data) {
                    return data.record.TeFactureren ? "Ja" : "Nee";
                },
                type: 'checkbox',
                values: { 'false': 'Nee', 'true': 'Ja' },
                defaultValue: 'true'
            }
        },        
        formCreated: function (event, data) {
            data.form.validationEngine();
            data.form.parent().css('width', '450px');
            data.form.css('margin-top', '50px');

            $('.dtp').datetimepicker({
                format: 'd/m/Y H:i'
            });
            if (data.record) {
                var typeWerk = data.record.TypeWerk;                
                $("#Edit-TariefId option").filter(function () {
                    return this.text == typeWerk;
                }).attr('selected', true);
            }
        },
        rowUpdated: function (event, data) {
            $('#tbl_details').jtable('reload');
        },
        formSubmitting: function (event, data) {
            return data.form.validationEngine('validate');
        },
        formClosed: function (event, data) {
            data.form.validationEngine('hide');
            data.form.validationEngine('detach');
        }
    });

    $('#tbl_details').jtable('load');

    $('#btn_terug').click(function () {
        var gebKlanId = $('#hdn_gebKlanId').val();
        var gebDepId = $('#hdn_gebDepId').val();
        var depId = $('#hdn_depId').val();

        var form = $('<form action="/UurRegistraties/Index" method="post" style="display:none">' +
        '<input type="text" name="gebruikerKlantId" value="' + gebKlanId + '" />' +
        '<input type="text" name="gebruikerDepartementId" value="' + gebDepId + '" />' +
        '<input type="text" name="departementId" value="' + depId + '" />' +
        '<input type="text" name="gefactureerd" value="' + gefactureerd + '" />' +
        '</form>');
        $('body').append(form);
        form.submit();
    });
});

function format(dateString) {
    if (dateString.search('Date') > 0) {
        return moment(dateString.replace(/\D/g, ''), 'x').format('DD/MM/YYYY  HH:mm');
    } else {
        return dateString;
    }
}

// GEBRUIKT DOOR validationEngine
function checkStartTijd(field, rules, i, options) {
    var americanStartDate = moment(field.val(), 'DD/MM/YYYY  HH:mm').format('MM/DD/YYYY HH:mm');
    var startDate = new Date(americanStartDate);
    
    var americanEindDate = moment($('#dtp_eind').val(), 'DD/MM/YYYY  HH:mm').format('MM/DD/YYYY HH:mm');
    var eindDate = new Date(americanEindDate);

    if (startDate >= eindDate) {
        return "De starttijd moet chronologisch voor de eindtijd liggen";
    }
    if (!sameDay(startDate, eindDate)) {
        return "De starttijd en de eindtijd moeten op dezelfde dag vallen";
    }
}

// GEBRUIKT DOOR validationEngine
function checkEindTijd(field, rules, i, options) {
    var americanEindDate = moment(field.val(), 'DD/MM/YYYY  HH:mm').format('MM/DD/YYYY HH:mm');
    var eindDate = new Date(americanEindDate);

    var americanStartDate = moment($('#dtp_start').val(), 'DD/MM/YYYY  HH:mm').format('MM/DD/YYYY HH:mm');
    var startDate = new Date(americanStartDate);

    if (startDate >= eindDate) {
        return "De starttijd moet chronologisch voor de eindtijd liggen";
    }
    if (!sameDay(startDate, eindDate)) {
        return "De starttijd en de eindtijd moeten op dezelfde dag vallen";
    }
}