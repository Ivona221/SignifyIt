$(document).ready(function () {

    $(".open-users-modal").click(function () {
        $("#usersModal").modal("show");
    });

    $("#saveUser").click(function () {
        var model = {
            Username: $("#username").val(),
            Email: $("#email").val(),
            Password: $("#password").val(),
            VerifyPassword: $("#repeat-password").val()
        };

        $.ajax({
            url: "/Users/CreateUser",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            $("#usersModal").modal("hide");
            switch (res["messageТype"]) {
                case 0:
                    Notifications().success("Success", res["message"]);
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
                case 1:
                    Notifications().error("Error", res["message"]);
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
                default:
                    Notifications().success("Error", "Unknown Error");
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                    break;
            }
        });
    });
});