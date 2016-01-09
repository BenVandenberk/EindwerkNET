$(function () {
    $('#tbl_departement').jtable({
        title: 'Departementen',
        sorting: true,
        actions: {
            listAction: '/Departements/List',
            createAction: '/Departements/Create',
            updateAction: '/Departements/Update',
            deleteAction: '/Departements/Delete'
        },
        fields: {
            Id: {
                key: true,
                list: false
            },
            Code: {
                title: 'Code',
                width: '40%',
                inputClass: 'validate[required]'
            },
            Omschrijving: {
                title: 'Omschrijving',
                width: '60%',
                inputClass: 'validate[required]'
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

    $('#tbl_departement').jtable('load');
});