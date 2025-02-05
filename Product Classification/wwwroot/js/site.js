// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    
    // Shows the Toast
    function showToast(status, body) {
        let classnames = ""

        // Selects the Classname based on the status
        if (status === "Warning") {
            classnames = "bg-warning text-black"
        } else if (status === "Danger") {
            classnames = "bg-danger text-white"
        }
        else if (status === "Success") {
            classnames = "bg-success text-white"
        } else {
            classnames = "bg-primary text-white"
        }

        // Displays the Bootstrap Toast
        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(document.getElementById('toast'))
        $("#toast").addClass(classnames)

        // Sets the bootstrap body.
        $("#toast .toast-body").html(body)
        toastBootstrap.show()
    }

})