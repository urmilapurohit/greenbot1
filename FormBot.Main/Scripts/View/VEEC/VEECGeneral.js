$(document).ready(function () {

    $('.decimal').keypress(function (event) {
        var $this = $(this);
        if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
            (event.which > 31) &&
          ((event.which < 48 || event.which > 57) &&
          (event.which != 0 && event.which != 8))) {
            event.preventDefault();
        }

        var text = $(this).val();
        if ((event.which == 46) && (text.indexOf('.') == -1)) {
            setTimeout(function () {
                if ($this.val().substring($this.val().indexOf('.')).length > 5) {
                    alert('hi');
                    $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
                }
            }, 1);
        }

        if ((text.indexOf('.') != -1) &&
            (text.substring(text.indexOf('.')).length > 2) &&
            (event.which != 0 && event.which != 8) &&
        ($(this)[0].selectionStart >= text.length - 2)) {
            event.preventDefault();
        }

        if ($this.val().indexOf('.') == -1 && ($this.val().length > 4) && event.which != 46) {
            event.preventDefault();
        }
    });
})

function showMessageForVEEC(msg, isError,successClass,errorClass) {
    $(".alert").hide();
    if(!isError){
        $("#"+successClass).removeClass("alert-danger");
        $("#"+successClass).addClass("alert-success");
        $("#"+successClass).html(closeButton + msg );
        $("#"+successClass).show();
    } else {
        $("#"+errorClass).addClass("alert-danger");
        $("#" + errorClass).removeClass("alert-success");
        $("#" + errorClass).html(closeButton + msg);
        $("#" + errorClass).show();
    }
}