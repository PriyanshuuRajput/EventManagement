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
            <p style='margin:0;'>Booking Confirmed </p>
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



        public static string ResetPassword(
          string resetLink,
          int expiryMinutes = 15)
        {
            return $@"
<div style='font-family:Segoe UI, Arial, sans-serif; background:#f4f6f8; padding:30px;'>
    <div style='max-width:600px; margin:auto; background:#ffffff; border-radius:10px;
                box-shadow:0 6px 25px rgba(0,0,0,0.08); overflow:hidden;'>

        <!-- Header -->
        <div style='background:#dc3545; padding:24px; text-align:center; color:#ffffff;'>
            <h2 style='margin:0;'>EventiGO</h2>
            <p style='margin:6px 0 0;'>Password Reset Request</p>
        </div>

        <!-- Body -->
        <div style='padding:30px;'>

            <h3 style='margin-top:0; color:#333;'>Reset your password</h3>

            <p style='color:#555; font-size:15px; line-height:1.6;'>
                We received a request to reset your password for your EventiGO account.
            </p>

            <p style='color:#555; font-size:15px;'>
                Click the button below to create a new password:
            </p>

            <div style='margin:15px 5px;'>
                <a href='{resetLink}'
                   style='background:#dc3545; color:#ffffff; padding:10px 20px;
                          text-decoration:none; font-size:15px; font-weight:600;
                          border-radius:6px; display:inline-block;'>
                    Reset Password
                </a>
            </div>

            <p style='color:#777; font-size:14px;'>
                This link will expire in <b>{expiryMinutes} minutes</b>.
            </p>

            <p style='color:#777; font-size:14px;'>
                If you did not request this, please ignore this email.
            </p>

            <hr style='border:none; border-top:1px solid #eee; margin:30px 0;' />

           

            <p style='margin-top:30px; font-size:14px;'>
                Regards,<br/>
                <b>EventiGO Team</b>
            </p>

        </div>

        <!-- Footer -->
        <div style='background:#f8f9fa; padding:12px; text-align:center;
                    font-size:12px; color:#888;'>
            © {DateTime.UtcNow.Year} EventiGO. All rights reserved.
        </div>

    </div>
</div>";
        }
    }
}
