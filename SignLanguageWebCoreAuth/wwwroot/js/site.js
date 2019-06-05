// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    console.log("Script");
    $(".logout").click(function () {
        console.log("Logout");
        $.ajax({
            url: "/Login/LogOff",
            type: "POST",
            contentType: "application/json; charset=utf-8",
        }).done(function (res) {
            var redirectUrl = res.redirectUrl;
            window.location.href = redirectUrl;
        });
    });
});
