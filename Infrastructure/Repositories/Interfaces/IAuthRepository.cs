using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.DTOs.CommonDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
       
        GeneralApiRespDTO RegisterUser(RegisterUserDTO userDTO, int userType);
        GeneralApiRespDTO Login(string email, string password);
        //Task<ResendOtpResponseDTO> RequestOtpForgotPassword(ForgotPasswordRequestDTO request);
        //Task<int> ResetPassword(ResetPasswordRequestDto request);

    }
}