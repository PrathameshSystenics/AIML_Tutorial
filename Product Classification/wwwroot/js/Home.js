$(function () {
    var classifybtn = $("#classify-btn");
    var resultbox = $("#classification-result")
    var thinkingresultbox = $("#thinking-result")

    classifybtn.on("click", function () {

        if ($("#inputprompt").val() == "") {
            showToast("Warning", "Input or Prompt Cannot be empty")

        } else {
            // setting the loading in the classify button
            $("#classify-btn .classify-btn-text").text("Classifying...")
            $("#classify-btn .spinner").removeClass("visually-hidden");
            classifybtn.prop("disabled", true)

            // getting the model value which is currently selected
            let value = $(".options-model input[type=radio]:checked").val()

            // empty the result stored in the resultbox and thinking result box
            resultbox.html("")
            thinkingresultbox.html("")

            // ajax call to classify category
            $.ajax({
                url: "/Home/ClassifyProductCategory",
                method: "POST",
                dataType: "json",
                data: { "ModelName": value, "UserInput": $("#inputprompt").val() },
                complete: function (data) {
                    // setting the button as it is
                    $("#classify-btn .classify-btn-text").text("Classify")
                    $("#classify-btn .spinner").addClass("visually-hidden");
                    classifybtn.prop("disabled", false)
                },
                error: function (err) {
                    showToast("Danger", err?.responseJSON.content ? err?.responseJSON.content : "Some Error Occured While Getting the Result");
                },
                success: function (data) {
                    if (data?.status === "Error") {
                        showToast("Danger", data?.content)
                    } else {
                        let modelresult = new String(data?.content).split("</think>");

                        // if the result contains the thinking steps then show them in different html box
                        if (modelresult.length === 2) {
                            thinkingresultbox.html(modelresult[0])
                            resultbox.html(modelresult[1])
                        } else {
                            resultbox.html(data?.content)
                        }

                    }
                }
            })
        }
    })
})