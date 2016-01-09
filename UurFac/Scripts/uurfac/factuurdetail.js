$(function () {
    $("#btn_bevestigen").click(function () {
        window.history.back();
    });

    $('#btn_annuleren').click(function () {
        var facId = $('#hdn_id').val();

        var form = $('<form action="/Factuurs/Annuleer" method="post" style="display:none">' +
        '<input type="text" name="factuurId" value="' + facId + '" />' +
        '</form>');
        $('body').append(form);
        form.submit();
    });

    $('#btn_terug').click(function() {
        var depId = $('#hdn_depId').val();

        var form = $('<form action="/Factuurs/Index" method="post" style="display:none">' +
        '<input type="text" name="departementId" value="' + depId + '" />' +
        '</form>');
        $('body').append(form);
        form.submit();
    });
});