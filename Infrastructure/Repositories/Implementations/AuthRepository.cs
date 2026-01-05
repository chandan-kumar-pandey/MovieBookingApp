using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DTOs.CommonDTOs;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {

        private readonly AppDbContext _context;

        private readonly PasswordHashingService passwordHashing;
        private readonly TokenService _tokenService;
        //private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger, PasswordHashingService pH, TokenService tokenService)
        {
            //_userRepository = userRepository;
            _context = context;
            _logger = logger;
            //_emailService = emailService;
            passwordHashing = pH;
            _tokenService = tokenService;
        }

        public GeneralApiRespDTO RegisterUser(RegisterUserDTO userDTO, int role)
        {

            bool emailExists = _context.UserDetails.Any(u => u.Email == userDTO.Email);
            bool phoneExists = _context.UserDetails.Any(u => u.ContactNumber == userDTO.ContactNumber);


            if (emailExists || phoneExists)
            {
                _logger.LogWarning("Attempt to register with existing Email or Phone. Email: {Email}, Phone: {Phone}", userDTO.Email, userDTO.ContactNumber);

                var respBody = new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "Email or Phone number already exists."
                };

                return respBody;
            }

            // Validate email formatting if an email was provided
            if (!string.IsNullOrWhiteSpace(userDTO.Email) && !IsValidEmail(userDTO.Email))
            {
                _logger.LogWarning("Attempt to register with invalid email format. Email: {Email}", userDTO.Email);

                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "Invalid email format."
                };
            }

            // Validate phone number length if a phone number was provided
            if (!string.IsNullOrWhiteSpace(userDTO.ContactNumber))
            {
                // Normalize to digits only to allow common separators and international '+' prefix
                var digitsOnly = new string(userDTO.ContactNumber.Where(char.IsDigit).ToArray());

  
                if (digitsOnly.Length != 10)
                {
                    _logger.LogWarning("Attempt to register with invalid phone length. Phone: {Phone}", userDTO.ContactNumber);

                    return new GeneralApiRespDTO
                    {
                        Status = 0,
                        Message = "Invalid phone number length."
                    };
                }
            }

            var user = new UserDetails
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = passwordHashing.HashPassword(userDTO.PasswordHash),
                UserType = role,
                ContactNumber = userDTO.ContactNumber,
            };

            _context.UserDetails.Add(user);

            var changes = _context.SaveChanges();

            if (changes > 0)
            {
                _logger.LogInformation("New Admin/Examiner is registered with UserId : {@userid} at {@time}", user.UserId, DateTime.UtcNow);

                return new GeneralApiRespDTO
                {
                    Status = 1,
                    Message = "User registered successfully.",
                    Id = user.UserId
                };
            }
            else
            {
                _logger.LogError("Failed to save new user to the database. Email: {Email}", user.Email);

                return new GeneralApiRespDTO
                {
                    Status = 0,
                    Message = "Failed to register user. Please try again."
                };
            }
        }

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailAttr = new EmailAddressAttribute();
            return emailAttr.IsValid(email);
        }

        public GeneralApiRespDTO? Login(string email, string password)
        {
            var user = _context.UserDetails.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed. No user found with Email: {Email}", email);
                return new GeneralApiRespDTO
                {
                    Status = -1,
                    Message = "User not found.",
                };
            }
            bool isPasswordValid = passwordHashing.VerifyPassword(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login attempt failed. Invalid password for Email: {Email}", email);
                return new GeneralApiRespDTO
                {
                    Status = -1,
                    Message = "Invalid Password or User-ID",
                };
            }
            _logger.LogInformation("User logged in successfully. UserId: {UserId}", user.UserId);

            var token = _tokenService.GenerateJwtToken(user);

            return new GeneralApiRespDTO
            {
                Status = 1,
                Message = "User LoggedIn Successfully!!",
                Data = new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ContactNumber = user.ContactNumber,
                    UserType = user.UserType,
                    UserId = user.UserId,
                    Token = token
                }
            };
        }
    }
}
