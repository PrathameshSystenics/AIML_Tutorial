$(function () {
    // Initialize Bootstrap modal
    var productModal = new bootstrap.Modal($('#productDetailsModal'));

    // Handle view details button click
    $('.view-details').on('click', function () {
        var button = $(this);
        var productId = button.data('product-id');

        // Show loading state
        button.prop('disabled', true);
        $('#modalContent').addClass('d-none');
        $('#modalLoading').removeClass('d-none');
        productModal.show();

        // Fetch product details
        $.ajax({
            url: '/Search/GetProductDetails/' + productId,
            method: 'GET',
            success: function (product) {
                // Update modal content
                $('#modalTitle').text(product.title);
                $('#modalDescription').text(product.description);
                $('#modalCategory').text(product.category);
                $('#modalProductId').text(product.id);

                // Show content
                $('#modalLoading').addClass('d-none');
                $('#modalContent').removeClass('d-none');
            },
            error: function () {
                productModal.hide();
                showToast("Danger","Failed to load product details. Please try again.")
            },
            complete: function () {
                button.prop('disabled', false);
            }
        });
    });

    // Form validation and handling
    $('#searchForm').on('submit', function (e) {
        var count = parseInt($('#productCount').val());
        if (count < 1) {
            e.preventDefault();
            showToast("Warning","Enter Number Greater then 1");
            return false;
        }
    });

    // Reset modal content when hidden
    $('#productDetailsModal').on('hidden.bs.modal', function () {
        $('#modalContent').addClass('d-none');
        $('#modalLoading').addClass('d-none');
        $('#modalTitle, #modalDescription, #modalCategory, #modalProductId').text('');
    });
})