$(function () {

    var chattextarea = $(".chat-textarea");
    chattextarea.trigger("focus");

    // When the button is clicked then call the chatbot event stream
    $("#chat-button").on("click", initiateChatConversation)
    $(document).on("keypress", function (e) {
        chattextarea.trigger("focus")
        if (e.key === "Enter") {
            e.preventDefault();
            initiateChatConversation()
            chattextarea.val("")
        }
    })

    function initiateChatConversation() {
        if (chattextarea.val().trim() === "" || chattextarea.val().trim().length === 0) {
            return;
        }
        let selectedmodel = $("#modelSelect").val();

        if (selectedmodel.trim().length === 0) {
            showToast("Warning", "Select the Model First")
            return;
        }

        navigateToBottom()
        enabledisableButton("#chat-button", true)
        createUserMessageBox(chattextarea.val())

        let currentassistantmessagebox = createAssistantMessageBox($("#modelSelect option:selected").text());

        const chathistory = toChatHistory("#message-area");

        eventSource = new PostEventSource('/Chat/ChatCompletions', {
            payload: {
                modelId: selectedmodel,
                messages: chathistory
            },
            withCredentials: true,
            reconnectInterval: 1000
        });

        eventSource.addEventListener('chat', function (e) {
            const assistantmessage = JSON.parse(e.data)
            currentassistantmessagebox.children(".loader").remove()
            currentassistantmessagebox.append(assistantmessage?.Content)
        });

        eventSource.addEventListener("complete", function (e) {
            currentassistantmessagebox.removeClass("ongoing");
            var markdownmessage = marked.parseInline(currentassistantmessagebox.html().trim());
            currentassistantmessagebox.html(markdownmessage)
            eventSource.close();
            enabledisableButton("#chat-button", false)
            currentassistantmessagebox = null;
            navigateToBottom();
        })

        eventSource.addEventListener("error", function (e) {
            const errormessage = JSON.parse(e.data);
            createAssistantErrorBox(errormessage?.message ? errormessage?.message : "Error Occured while getting the Response from the Model. Please Try Again Later", currentassistantmessagebox);
            navigateToBottom()
            enabledisableButton("#chat-button", false)
            eventSource.close();
        })
    }
    function enabledisableButton(buttonid, disable = false) {
        disable ? $(buttonid).attr("disabled", true) : $(buttonid).attr("disabled", false);
    }

    function createAssistantErrorBox(message, lastfailedassistantelement) {
        const errordivbox = `
            <div class="d-flex mb-3">
                <div class="alert alert-danger p-2">
                    <i class="bi bi-info-circle"></i>
                    ${message}
                </div>
            </div>
        `
        $(lastfailedassistantelement).parents(".d-flex").first().remove()
        $("#message-area").append(errordivbox)
    }

    function navigateToBottom() {
        $('#message-area').animate({ scrollTop: 9999 }, 'slow');
    }

    function createAssistantMessageBox(selectedmodel) {
        const assistantmessagediv = `
         <div class="d-flex mb-3">
                <div class="bg-success text-white rounded p-2 d-flex align-items-center justify-content-center me-2" style="width: 40px; height: 40px; min-width: 40px;">
                    AI
                </div>
                <div class="bg-white rounded p-3 shadow-sm card-border-gray" style="max-width: 80%;">
                    <div class="fw-bold mb-1">${selectedmodel}</div>
                    <div class="message-body ongoing" data-role="assistant">
                        <div class="loader"></div>
                    </div>
                </div>
            </div>
        `

        $("#message-area").append(assistantmessagediv);
        return $("#message-area .message-body").last()
    }
    function createUserMessageBox(message) {
        const messageinmarkdown = marked.parseInline(message)
        const usermessagediv = `<div class="d-flex mb-3 justify-content-end">
            <div class="bg-light rounded p-3 shadow-sm card-border-gray" style="max-width: 80%;">
                <div class="fw-bold mb-1">You</div>
                <div class="message-body" data-role="user">${messageinmarkdown}</div>
            </div>
            <div class="bg-primary text-white rounded p-2 d-flex align-items-center justify-content-center ms-2" style="width: 40px; height: 40px; min-width: 40px;">
                U
            </div>
        </div>`
        var messagearea = $("#message-area")
        messagearea.append(usermessagediv)
        $(".chat-textarea").val("")
    }

    function toChatHistory(messagebodyelementid) {
        var messagearea = $(messagebodyelementid);

        const chathistory = []

        var messagesinabody = messagearea.find(".message-body").not(".ongoing")

        messagesinabody.each((index, element) => {
            let currentelement = $(element)
            if (currentelement.data("role")) {
                chathistory.push({
                    Role: currentelement.data("role"),
                    Content: currentelement.html()
                })
            }
        })
        return chathistory;
    }

    // When the Model is change the Bootstrap Modal load
    $("#modelSelect").on("change", function (e) {
        if ($("#message-area .message-body").length > 1) {
            const bsmodal = new bootstrap.Modal('#modelChangeWarningModal')
            bsmodal.show();
        }

        // When Clicked on Yes Reloads the Page with the currently selected Model
        $("#confirmModelChange").on("click", function () {
            let currentmodel = $("#modelSelect").val()
            window.location.href = "/Chat?model=" + currentmodel
        })
    })
})