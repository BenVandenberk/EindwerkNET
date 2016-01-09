$(function () {
    $('#tbl_gebruikers').jtable({
        title: 'Gebruikers',
        sorting: true,
        openChildAsAccordion: true,
        actions: {
            listAction: '/Gebruikers/List',
            createAction: '/Gebruikers/Create',
            updateAction: '/Gebruikers/Update',
            deleteAction: '/Gebruikers/Delete'
        },
        fields: {
            GebruikerDepartementen: {
                title: '',
                width: '5%',
                sorting: false,
                edit: false,
                create: false,
                display: function (gebruikerData) {
                    var $img = $('<img src="/Content/images/list_metro.png" title="Edit Departementen" class="pointer"/>');
                    $img.click(function () {
                        if (gebruikerData.record.Rol == 0) {
                            $('#dialog').dialog({
                                width: '500px'
                            });
                        } else {
                            $('#tbl_gebruikers').jtable('openChildTable',
                                    $img.closest('tr'),
                                    {
                                        title: gebruikerData.record.Login + ' - Departementen',
                                        actions: {
                                            listAction: '/Gebruikers/ListDepartementen?gebruikerId=' + gebruikerData.record.Id,
                                            deleteAction: '/Gebruikers/OntkoppelDepartement?gebruikerId=' + gebruikerData.record.Id,
                                            createAction: '/Gebruikers/KoppelDepartement?gebruikerId=' + gebruikerData.record.Id
                                        },
                                        fields: {
                                            Id: {
                                                type: 'hidden',
                                                key: true
                                            },
                                            DepartementId: {
                                                title: 'Departement',
                                                options: function (data) {
                                                    data.clearCache();
                                                    return '/Gebruikers/ListMogelijkeDepartementen?gebruikerId=' + gebruikerData.record.Id;
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
                                    }, function (data) {
                                        data.childTable.jtable('load');
                                    });
                        }
                    });
                    return $img;
                }
            },
            Id: {
                key: true,
                list: false
            },
            Login: {
                title: 'Login',
                width: '30%',
                create: true,
                edit: false,
                inputClass: 'validate[required]'
            },
            Voornaam: {
                title: 'Voornaam',
                width: '20%',
                inputClass: 'validate[required]'
            },
            Achternaam: {
                title: 'Achternaam',
                width: '20%',
                inputClass: 'validate[required]'
            },
            Rol: {
                title: 'Rol',
                width: '20%',
                options: { '0': 'Administrator', '1': 'Departement Administrator', '2': 'User' },
                optionsSorting: 'value'
            },
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

    $('#tbl_gebruikers').jtable('load');
});