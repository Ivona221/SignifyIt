$(document).ready(function () {

    $(".open-synonyms-modal").click(function () {
        $("#synonymsModal").modal("show");
    });

    $("#saveSynonym").click(function () {
        var model = {
            Phrase: $("#phrase").val(),
            RelatedWord: $("#synonym").val(),
            Tag: $("#category").children("option:selected").val()
        };

        $.ajax({
            url: "/Synonyms/Create",
            type: "POST",
            data: JSON.stringify(model),          
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            $("#synonymsModal").modal("hide");
            switch (res["MessageТype"]) {
                case 0:
                    $(".alert-success").text(res["Message"]);
                    $(".alert-success").fadeIn(300).delay(2000).fadeOut(400);
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                    break;
                case 1:
                    $(".alert-error").text(res["Message"]);
                    $(".alert-error").fadeIn(300).delay(2000).fadeOut(400);
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                    break;
                case 2:
                    $(".alert-info").text(res["Message"]);
                    $(".alert-info").fadeIn(300).delay(2000).fadeOut(400);
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                    break;
                default:
                    $(".alert-info").text("Непозната грешка");
                    $(".alert-info").fadeIn(300).delay(2000).fadeOut(400);
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                    break;
            }
        });
    });
});