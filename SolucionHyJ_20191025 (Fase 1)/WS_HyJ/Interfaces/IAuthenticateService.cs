using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;

namespace WS_HyJ.Seguridad
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginDTO request, out string token);
    }
}
