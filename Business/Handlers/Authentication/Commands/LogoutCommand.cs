using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Authentication.Commands
{
    public class LogoutCommand : IRequest { }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IHttpContextAccessor _http;

        public LogoutCommandHandler(IHttpContextAccessor http)
        {
            _http = http;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _http.HttpContext!.SignOutAsync("MyCookieAuth");
            return Unit.Value;
        }

    }
}

