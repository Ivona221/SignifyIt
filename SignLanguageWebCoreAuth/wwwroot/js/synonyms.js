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
        //Notification.showLoading();
        $.ajax({
            url: "/Synonyms/Create",
            type: "POST",
            data: JSON.stringify(model),          
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            $("#synonymsModal").modal("hide");
            switch (res["messageТype"]) {
                case 0:
                    Notifications().success("Успешно зачувување", res["message"]);
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
                case 1:
                    Notifications().error("Грешка", res["message"]);                 
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
                case 2:
                    Notifications().info("Информација", res["message"]);
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
                default:
                    Notifications().error("Грешка", "Непозната грешка");
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
            }
        });
    });
});