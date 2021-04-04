namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// Send email broker request.
    /// </summary>
    public interface ISendEmailRequest
    {
        string Email { get; }
        string Subject { get; set; }
        string Text { get; }

        static object CreateObj(string email, string subject, string text)
        {
            return new
            {
                Email = email,
                Subject = subject,
                Text = text
            };
        }
    }
}
