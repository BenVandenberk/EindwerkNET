$(function () {
    $('#tbl_tarief').jtable({
        title: 'Type Werk',
        sorting: true,
        defaultSorting: 'TypeWerk ASC',
        actions: {
            listAction: '/Tariefs/List',
            createAction: '/Tariefs/Create',
            updateAction: '/Tariefs/Update',
            deleteAction: '/Tariefs/Delete'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            TypeWerk: {
                title: 'Type Werk',
                width: '40%',
                inputClass: 'validate[required]'
            },
            GeldigVanaf: {
                title: 'Geldig Vanaf',
                type: 'date',
                width: '30%',
                displayFormat: "dd/mm/yy",
                inputClass: 'validate[required]'
            },
            TariefUurWaarde: {
                title: 'Uurwaarde',
                width: '30%',
                display: function (data) {
                    return "€ " + data.record.TariefUurWaarde;
                },
                inputClass: 'validate[custom[number],min[0],max[1000],required]'
            }
        },
        formCreated: function (event, data) {
            data.form.validationEngine();
            data.form.parent().css('width', '300px');
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

    $('#tbl_tarief').jtable('load');
});