$(function () {

    // Evaluate the Responses
    $("#evaluate").on("click", function () {

        // when the model is selected then only call the event
        if ($(".options-model input[type=radio]:checked").val()) {

            $(this).text("Evaluating..... Please Wait");
            $(this).prop("disabled", true)

            // Capturing the Events emitted by the server
            var eventsource = new EventSource("/Evaluate/EvalResult");

            // Listening to the event sent by the server
            eventsource.onmessage = function (event) {

                var val = JSON.parse(event.data);

                if (val.Status === "Error") {
                    showToast("Danger", "Select the Model First")
                    eventsource.close();
                    return;
                }

                var container = $("#eval-result")

                // Check if description exists (skip any null descriptions)
                if (val.Description) {

                    // Create a card for each description
                    var card = $('<div class="card mb-3"></div>');
                    var cardBody = $('<div class="card-body"></div>');

                    // Add the description text
                    var descriptionEl = $('<p class="card-text"></p>').html(val.Description);
                    cardBody.append(descriptionEl);

                    // Create labels for Expected and Actual answers
                    var expectedLabel = $('<span class="badge mr-2"></span>')
                        .text('Expected: ' + val.Expected)
                        // Add the same background if needed
                        .addClass("bg-warning text-dark");

                    var actualLabel = $('<span class="badge"></span>')
                        .text('Result: ' + val.Result)
                        .addClass((val.Status === "Correct") ? "bg-success text-white" : "bg-danger text-white");

                    // Append labels to the card body
                    cardBody.append(expectedLabel).append("</br>").append(actualLabel).append("</br>");

                    card.append(cardBody);
                    container.append(card);
                    container.animate({ scrollTop: container.prop("scrollHeight") }, 500)
                }

                // Retrieve overall accuracy from the object with status "Complete"
                // (Assuming the accuracy is stored in the last item with status "Complete")
                var overallAccuracy = 0;
                var totalQuestion = 0;
                var totalCorrect = 0;
                var incorrect = 0;

                if (val.Status === "Complete") {
                    overallAccuracy = val.Accuracy;
                    totalQuestion = val.Total;
                    totalCorrect = val.Correct;
                    incorrect = totalQuestion - totalCorrect;

                    $("#evaluate").text("Evaluating..... Please Wait");
                    $("#evaluate").prop("hidden", true)

                    showToast("Success", "Successfully Evaluated")

                    eventsource.close();

                    // displaying the dashboard after completing the event
                    var accuracyhtml = $("<div></div>").html("Score : " + overallAccuracy).addClass("fw-bold")
                    var totalqueshtml = $("<div></div>").html("Total Questions : " + totalQuestion).addClass("fw-bold")
                    var correcthtml = $("<div></div>").html("Total Correct : " + totalCorrect).addClass("fw-bold text-sucess");
                    var incorrecthtml = $("<div></div>").html("Incorrect : " + incorrect).addClass("fw-bold text-danger")
                    container.prepend([accuracyhtml, totalqueshtml, correcthtml, incorrecthtml]);
                }


            }

            // if the event found any error
            eventsource.onerror = function (event) {
                showToast("Danger", "Error Occured while getting the Result")
                eventsource.close();
            }


        } else {
            showToast("Danger", "Select the Model First")
        }

    });


    // When the Model is Selected then Calls the api request to the Server
    $(".options-model-eval").on("change", function () {
        $.ajax({
            url: "/api/setmodel",
            method: "POST",
            data: { "ModelName": $(".options-model input[type=radio]:checked").val() },
            success: function (data) {
                showToast("Success", data?.Content)
            }
        })
    })


})