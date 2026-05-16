
using Entities.Enums;

namespace Business.Services
{
public interface IOtpService
{
    Task SendOtpAsync(string phoneNumber);
        Task<OtpVerifyResult> VerifyOtpAsync(string phoneNumber, string otp);

    }
}