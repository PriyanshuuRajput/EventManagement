
        window.startRazorpay = (options, dotnetHelper) => {
            options.handler = function (response) {
                dotnetHelper.invokeMethodAsync(
                    "OnPaymentSuccess",
                    response.razorpay_payment_id,
                    response.razorpay_order_id,
                    response.razorpay_signature
                );
            };

        options.modal = {
            ondismiss: function () {
            console.log("Payment popup closed");
                }
            };

        var rzp = new Razorpay(options);
        rzp.open();
        };

 
