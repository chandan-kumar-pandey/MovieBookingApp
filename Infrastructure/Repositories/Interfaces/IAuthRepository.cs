using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
       
        int RegisterUser(RegisterUserDTO userDTO, int userType);
        //UserDetails? Login(string email, string password);
        //Task<ResendOtpResponseDTO> RequestOtpForgotPassword(ForgotPasswordRequestDTO request);
        //Task<int> ResetPassword(ResetPasswordRequestDto request);

    }
}