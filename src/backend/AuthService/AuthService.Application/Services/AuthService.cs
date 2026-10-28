using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Interfaces;
using AuthService.Domain.Models;
using SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IAuthRepository _authRepository;
        private readonly IServiceBus _serviceBus;

        public AuthService(IJwtService jwtService, IAuthRepository authRepository, IServiceBus serviceBus)
        {
            _jwtService = jwtService;
            _authRepository = authRepository;
            _serviceBus = serviceBus;
        }

        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            if (request.Username.Length > 20 || request.Password.Length > 64)
                return new SignInResponse { Message = "Incorrect password or username", StatusCode = 401, Token = "" };

            if (!Regex.IsMatch(request.Username, @"^[a-zA-Zа-яА-Я0-9!@#$%^&*]+$") || !Regex.IsMatch(request.Password, @"^[a-zA-Zа-яА-Я0-9!@#$%^&*]+$"))
                return new SignInResponse { Message = "Incorrect username or password", StatusCode = 401, Token = "" };

            var user = await _authRepository.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return new SignInResponse { Message = "Wrong username or password", StatusCode = 401, Token = "" };

            if (user.VerifyPassword(request.Password))
            {
                return new SignInResponse { Message = "Success!", StatusCode = 200, Token = _jwtService.GenerateUserToken(user) };
            }
            else
            {
                return new SignInResponse { Message = "Wrong username or password", StatusCode = 401, Token = "" };
            }
        }

        public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 7)
                return new SignUpResponse { Message = "The password must be longer than 7", StatusCode = 400, Token = "" };

            if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 3)
                return new SignUpResponse { Message = "Username must be longer than 3", StatusCode = 400, Token = "" };

            if (request.Username.Length > 20 || request.Password.Length > 64)
                return new SignUpResponse { Message = "Incorrect password or username", StatusCode = 400, Token = "" };

            if (!Regex.IsMatch(request.Username, @"^[a-zA-Zа-яА-Я0-9!@#$%^&*]+$") || !Regex.IsMatch(request.Password, @"^[a-zA-Zа-яА-Я0-9!@#$%^&*]+$"))
                return new SignUpResponse { Message = "Incorrect username or password", StatusCode = 400, Token = "" };

            var user = await _authRepository.GetUserByUsernameAsync(request.Username);

            if (user != null)
                return new SignUpResponse { Message = "User is already exist", StatusCode = 400, Token = "" };

            Guid guid;
            user = null;

            do
            {
                guid = Guid.NewGuid();
                user = await _authRepository.GetUserByGUIDAsync(guid);
            } while (user != null);

            user = new Domain.Models.User
            {
                PasswordHash = User.HashPassword(request.Password),
                Username = request.Username,
                UID = guid,
                Role = "User"
            };
            try
            {
                if (await _authRepository.AddUserAsync(user))
                {
                    string token = _jwtService.GenerateUserToken(user);

                    await _serviceBus.PublishAsync(new AccountCreated(Guid.NewGuid(), guid, DateTime.UtcNow));

                    return new SignUpResponse { Message = "Success!", StatusCode = 201, Token = token };
                }
                else
                {
                    return new SignUpResponse { Message = "Failed to create a new user", StatusCode = 500, Token = "" };

                }
            }
            catch (Exception ex)
            {
                return new SignUpResponse { Message = "Unhandled registration error", StatusCode = 500, Token = "" };
            }
        }
    }
}
