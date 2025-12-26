namespace EventBooking_TicketManagement_API.Helper
{
    public static class TicketTemplate
    {
        public static string TicketHtml(
            string eventName,
            string venue,
            DateTime eventDate,
            int ticketCount,
            string bookingNumber,
            decimal totalAmount,
            string qrSrc
        )
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <title>EventiGO Ticket</title>

    <!-- Bootstrap CDN -->
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
</head>

<body class='bg-light py-2'>

<div class='container'>
    <div class='row justify-content-center'>
        <div class='col-md-6'>

            <div class='card shadow-sm overflow-hidden'>

                <!-- HEADER -->
<div class=""text-white text-center ""
     style=""
        background: linear-gradient(135deg, #dc3545, #b02a37);
     "">
    <div class=""mt-1"">
        <span class=""badge bg-light text-danger fw-semibold px-3 py-1"">
           ENTRY PASS
        </span>
    </div>

    <div class=""small mt-1"">
        Please present this ticket at the venue
    </div>
</div>


                <!-- BODY -->
                <div class='card-body px-4 py-3'>

                    <h6 class='fw-bold fs-5 mb-2'>{eventName}</h6>

                    <p class='text-muted small mb-2'>
                        <b>Venue:</b> {venue}<br/>
                        <b>Date:</b> {eventDate:dd MMM yyyy}<br/>
                        <b>Time:</b> {eventDate:hh:mm tt}
                    </p>

                    <hr class='my-2'/>

                    <div class='row small'>
                        <div class='col-6'>
                            <b>Booking ID</b>
                            <div>{bookingNumber}</div>
                        </div>
                        <div class='col-6 text-end'>
                            <b>Tickets</b>
                            <div>{ticketCount}</div>
                        </div>
                    </div>

                    <div class='mt-2 small'>
                        <b>Total Amount:</b> ₹{totalAmount:N2}
                    </div>

                    <!-- QR -->
                    <div class='text-center mt-3'>
                        <img src='{qrSrc}' class='img-fluid' style='max-width:150px'/>
                        <div class='text-muted small mt-1'>
                            Scan at entry
                        </div>
                    </div>

                </div>

                <!-- TERMS -->
                <div class='px-4 pb-2'>
                    <hr class='my-2'/>
                    <div class='small text-muted'>
                        <b>Terms & Conditions:</b>
                        <ul class='mb-0 ps-3'>
                            <li>Valid only for the mentioned event and date.</li>
                            <li>Entry allowed via QR code scan only.</li>
                            <li>Ticket is non-transferable and non-refundable.</li>
                            <li>Organizer reserves the right to admission.</li>
                        </ul>
                    </div>
                </div>

                <!-- FOOTER -->
                <div class='bg-light text-center py-2 small text-muted'>
                    © EventiGO · All rights reserved
                </div>

            </div>

        </div>
    </div>
</div>

</body>
</html>";
        }
    }
}
