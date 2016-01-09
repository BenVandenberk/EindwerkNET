$(function () {
    $('body').append(
        $('<div>').css('display', 'none')
                  .attr('id', 'div_error')
        );

    $('#img_help').click(function () {
        $('#div_help').toggle(500);
    });
});

function error(message) {
    $('#div_error').empty();
    $('#div_error').html(message);
    $('#div_error').dialog({
        title: 'Oops!',
        modal: true,
        buttons: [
        {
          text: "Ok",
          icons: {
              primary: "ui-icon-heart"
          },
          click: function () {
              $(this).dialog("close");
          }      
        }
        ]
    });
}

function sameDay(date, date2) {
    var datum = new Date(date);
    var datum2 = new Date(date2);
    
    if (datum.getDate() != datum2.getDate()) {
        return false;
    }
    if (datum.getMonth() != datum2.getMonth()) {
        return false;
    }
    if (datum.getYear() != datum2.getYear()) {
        return false;
    }

    return true;
}