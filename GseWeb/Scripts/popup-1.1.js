 $(function () {
        $("#popup").dialog({
            autoOpen: false,
            title: 'Title',
            width: 'auto',
            height: 'auto',
            modal: true,
            draggable: false,
            resizable: false,
        });
    });

    function getPopup(Url) {
        $.get(Url)
        .success(function (data) {
            $('#popup').html(data);
            $('#popup').dialog('open');
            // Posizione popup
            if ($('#popup').closest('.ui-dialog').offset().top < 80) {
                $('#popup').closest('.ui-dialog').css({ 'top': '80px' });
            }
        });
    }

    function closePopup() {
        $('#popup').dialog('close')
    }