$(document).ready(function () {
    $("#login").click(function () {
        var model = {
            Email: $("#username").val(),
            Password: $("#password").val(),
        };

        $.ajax({
            url: "/Login/Login",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            var redirectUrl = res.redirectUrl;
            window.location.href = redirectUrl;
        });
    });
});