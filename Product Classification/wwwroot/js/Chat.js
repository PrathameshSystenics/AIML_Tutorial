$(function () {
    var chattextarea = $("#chat-textarea");

    // When the button is clicked then call the chatbot event stream
    $("#chat-button").on("click", function () {


        if (chattextarea.val().trim() === "" || chattextarea.val().trim().length === 0) {
            return;
        }

        showHideButton("#chat-button", true)
        let selectedmodel = $("#modelSelect").val();
        selectedmodel = "GeminiFlash2";

        let eventsource = new EventSource("/Chat/ChatCompletions",)

    })



    function showHideButton(buttonid, tohide = false) {
        tohide ? $(buttonid).hide() : $(buttonid).show();
    }
})