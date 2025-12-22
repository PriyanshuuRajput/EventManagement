namespace EventBooking_TicketManagement_API.Helpers
{
    public static class EmailTemplates
    {
        public static string BookingConfirmation(
            string eventName,
            int ticketCount,
            string bookingNumber)
        {
            return $@"
<div style='font-family:Arial, sans-serif; background:#f5f7fa; padding:20px;'>
    <div style='max-width:600px; margin:auto; background:white; border-radius:10px;
                box-shadow:0 4px 10px rgba(0,0,0,0.08); overflow:hidden;'>

        <div style='background:#dc3545; padding:20px; text-align:center; color:white;'>
            <h2 style='margin:0;'>EventiGO</h2>
            <p style='margin:0;'>Booking Confirmed 🎟</p>
        </div>

        <div style='padding:25px;'>
            <p>Hello,</p>

            <p>Your booking has been <b>successfully confirmed</b>.</p>

            <p>
                <b>Event:</b> {eventName}<br/>
                <b>Tickets:</b> {ticketCount}<br/>
                <b>Booking No:</b> {bookingNumber}
            </p>

            <p>Please show the QR code below at the entry gate:</p>

            <div style='text-align:center; margin:20px 0;'>
                <img src='cid:bookingQr' width='200' />
            </div>

            <p style='font-size:14px; color:#555;'>
                This QR allows entry for all booked tickets.<br/>
                Re-entry is supported.
            </p>

            <p>Thanks,<br/>EventiGO Team</p>
        </div>

        <div style='background:#f0f0f0; padding:10px; text-align:center;
                    font-size:12px; color:#777;'>
            © EventiGO | Smart Event Booking Platform
        </div>

    </div>
</div>";
        }
    }
}
