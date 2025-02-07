$(function () {

    // Evaluate the Responses
    $("#evaluate").on("click", function () {

        // when the model is selected then only call the event
        if ($(".options-model input[type=radio]:checked").val()) {

            $(this).text("Evaluating..... Please Wait");
            $(this).prop("disabled", true)

            // Capturing the Events emitted by the server
            var eventsource = new EventSource("/Evaluate/EvaluateAndStreamResults");

            // Listening to the event sent by the server
            eventsource.addEventListener("result", function (event) {
                var val = JSON.parse(event.data);
                console.log(val)

                var container = $("#eval-result")

                // Check if description exists (skip any null descriptions)
                if (val?.EvaluationData?.Description) {

                    // Create a card for each description
                    var card = $('<div class="card mb-3"></div>');
                    var cardBody = $('<div class="card-body"></div>');

                    // Add the description text
                    var descriptionEl = $('<p class="card-text"></p>').html(val?.EvaluationData?.Description);
                    cardBody.append(descriptionEl);

                    // Create labels for Expected and Actual answers
                    var expectedLabel = $('<div class="alert alert-success p-0 p-2 mb-1" role="alert"></div>')
                        .text('Expected: ' + val?.EvaluationData?.Answer)
                        // Add the same background if needed
                        .addClass("");

                    // Checking if result contains the Thinking Steps.
                    var thinkingsteps = new String(val?.Result).split("</think>");

                    var actualLabel = $('<div class="alert mr-2 p-0 p-2 mb-1" role="alert"></div>')
                        .addClass(val.IsCorrect ? "alert-success" : "alert-danger");

                    var thinkingdivbox = $('<div class="bg-light text-black mb-2 p-3"></div>')
                    thinkingdivbox.append($('<p class="fw-bold m-0">Thinking Result:</p>'))

                    if (thinkingsteps.length === 2) {
                        let modelthinkingresult_withoutTag = thinkingsteps[0].replace("<think>", "").replace("</think>", "")

                        actualLabel.html('Result: ' + thinkingsteps[1])
                        thinkingdivbox.append(modelthinkingresult_withoutTag);
                        cardBody.append(thinkingdivbox);
                    } else {
                        actualLabel.html('Result: ' + val?.Result);
                    }

                    // Append labels to the card body
                    cardBody.append(expectedLabel).append(actualLabel);

                    card.append(cardBody);
                    container.append(card);

                    // Scroll to Bottom whenever the data arrives
                    $('html,body').animate({ scrollTop: 9999 }, 'slow');
                }

            })


            eventsource.addEventListener("error", function (event) {
                showToast("Danger", "Some Error Occured While Evaluating")
                console.log(event)
                $("#evaluate").text("Evaluate");
                $("#evaluate").prop("disabled", false)

                eventsource.close();
            })

            eventsource.addEventListener("completed", function (event) {
                var val = JSON.parse(event.data);
                console.log(val);

                $("#evaluate").prop("hidden", true)
                showToast("Success", "Successfully Evaluated the Model");

                // displaying the dashboard after completing the event
                var accuracyhtml = $("<div></div>").html("Score : " + val?.Accuracy).addClass("fw-bold")
                var totalqueshtml = $("<div></div>").html("Total Questions : " + val?.Total).addClass("fw-bold")
                var correcthtml = $("<div></div>").html("Total Correct : " + val?.Correct).addClass("fw-bold text-sucess");
                var incorrecthtml = $("<div></div>").html("Incorrect : " + val?.InCorrect).addClass("fw-bold text-danger")

                $("#eval-result").prepend([accuracyhtml, totalqueshtml, correcthtml, incorrecthtml]);

                eventsource.close();

            })

        } else {
            showToast("Danger", "Select the Model First")
        }

    });


    // When the Model is Selected then Calls the api request to the Server
    $(".options-model").on("change", function () {
        $.ajax({
            url: "/Evaluate/SetModelForEvaluation",
            method: "POST",
            data: { "ModelName": $(".options-model input[type=radio]:checked").val() },
            success: function (data) {
                showToast("Success", data?.Content)
            }
        })
    })


})