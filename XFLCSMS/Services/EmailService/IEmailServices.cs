using XFLCSMS.Models.Email;

namespace XFLCSMS.Services.EmailService
{
    public interface IEmailServices
    {
        void SendEmail(EmailDto request);
    }
}
