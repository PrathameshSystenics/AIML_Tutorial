// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/stepdatahub").build();


    connection.start().then(function () {
        console.log("connected")
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("SendMessage", function (message) {
        if (message === "<next>") {
            $("#message-box").append($("<br>"))
        }
        $("#message-box").append($("<div>").html(message))
    })


    $("#runprocessbtn").click(function () {
        var processidtext = $("#processid-text").val();

        let connectionid = ""
        connection.invoke("GetConnectionId").then(function (value) {
            $.ajax({
                url: `/api/runprocess/${processidtext}?connectionid=${value}`,
                method: "GET",
                complete: function (d) {
                    console.log("completed")
                },
                success: function (data) {
                    console.log("Run Process Call data -> ", data)

                },
                error: function (e) {
                    connection.stop();
                    console.log(e)
                }
            })

            /* var stream = connection.stream("StreamStepMessage")
             stream.subscribe({
                 next: function (data) {
                     console.log(data)
                 },
                 complete: function () {
                     console.log("Streaming Completed")
                 },
                 error: function (err) {
                     console.log("Error:-", err)
                 }
             })*/

        })






    })
})