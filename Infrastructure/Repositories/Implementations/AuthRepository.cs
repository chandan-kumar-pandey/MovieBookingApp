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

namespace Infrastructure.Repositories.Implementations
{
    public class AuthRepository:IAuthRepository
    {

        private readonly AppDbContext _context;

        private readonly PasswordHashingService passwordHashing;
        //private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger,  PasswordHashingService pH)
        {
            //_userRepository = userRepository;
            _context = context;
            _logger = logger;
            //_emailService = emailService;
            passwordHashing = pH;
        }

        public int RegisterUser(RegisterUserDTO userDTO, int role)
        {

            bool emailExists = _context.UserDetails.Any(u => u.Email == userDTO.Email);
            bool phoneExists = _context.UserDetails.Any(u => u.ContactNumber == userDTO.ContactNumber);


            if (emailExists || phoneExists)
            {
                _logger.LogWarning("Attempt to register with existing Email or Phone. Email: {Email}, Phone: {Phone}", userDTO.Email, userDTO.ContactNumber);
                return -1;

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

            _logger.LogInformation("New Admin/Examiner is registered with UserId : {@userid} at {@time}", user.UserId, DateTime.UtcNow);
            return _context.SaveChanges();
        }
    }
}
