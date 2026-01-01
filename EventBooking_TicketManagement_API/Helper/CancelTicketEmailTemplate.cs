namespace EventBooking_TicketManagement_API.Helpers
{
    public static class CancelTicketEmailTemplate
    {
        public static string CancelHtml(
            string eventName,
            string venue,
            DateTime eventDate,
            string bookingNumber,
            int ticketCount)
        {
            return $@"
<div style='font-family:Arial;background:#f5f7fa;padding:20px'>
  <div style='max-width:600px;margin:auto;background:white;border-radius:10px;
              box-shadow:0 6px 18px rgba(0,0,0,.12);overflow:hidden'>

    <div style='background:#dc3545;color:white;padding:16px;text-align:center'>
      <h3 style='margin:0'>EventiGO</h3>
      <p style='margin:0'>Ticket Cancelled </p>
    </div>

    <div style='padding:22px'>
      <p>Hello,</p>

      <p>Your booking has been <b>successfully cancelled</b>.</p>

      <hr/>

      <p>
        <b>Event:</b> {eventName}<br/>
        <b>Venue:</b> {venue}<br/>
        <b>Date:</b> {eventDate:dd MMM yyyy, hh:mm tt}<br/>
        <b>Booking ID:</b> {bookingNumber}<br/>
        <b>Tickets:</b> {ticketCount}
      </p>

      <p style='font-size:14px;color:#555'>
        If eligible, your refund will be processed within <b>3–5 business days</b>
        to the original payment method.
      </p>

      <p style='font-size:14px;color:#555'>
        If you have any questions, feel free to contact our support team.
      </p>

      <p>Thanks,<br/>EventiGO Team</p>
    </div>

    <div style='background:#f0f0f0;padding:10px;text-align:center;
                font-size:12px;color:#777'>
      © EventiGO | Smart Event Booking Platform
    </div>

  </div>
</div>";
        }
    }
}
