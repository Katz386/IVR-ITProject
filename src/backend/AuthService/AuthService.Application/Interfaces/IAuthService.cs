using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);

    }
}
