$(document).ready(function () {

    $("#signUp").click(function () {
        var model = {
            Username: $("#username").val(),
            Email: $("#email").val(),
            Password: $("#password").val(),
            VerifyPassword: $("#verifyPassword").val()
        };
        Notification.showLoading();
        $.ajax({
            url: "/SignUp/SignUp",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            var redirectUrl = res.redirectUrl;
            window.location.href = redirectUrl;
        });
    });
});
