using AdminGateway.Models;
using AdminGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminGateway.Controllers
{
    public class BaseController : Controller
    {
        private TokenData _tokenData = null;
        private readonly JwtTokenService _jwtTokenService;

        public BaseController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        protected TokenData TokenData
        {
            get
            {
                if (_tokenData == null)
                {
                    _tokenData = _jwtTokenService.ExtractDataFromToken(User.Identity);
                }

                return _tokenData;
            }
        }
    }
}
