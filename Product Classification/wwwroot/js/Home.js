$(function () {
    var classifybtn = $("#classify-btn");
    var resultbox = $("#classification-result")

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


            // ajax call to get the model result
            $.ajax({
                url: "/api/result",
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
                    showToast("Danger", "Some Error Occured while Getting the Result")
                },
                success: function (data) {
                    if (data?.status === "Error") {
                        showToast("Danger", data?.content)
                    } else {
                        resultbox.html(data?.content)
                    }
                }
            })
        }
    })
})