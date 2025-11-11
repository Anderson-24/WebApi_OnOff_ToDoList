using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApi_OnOff_ToDoList.Application.DTOs;
using WebApi_OnOff_ToDoList.Infrastructure.Context;
using WebApi_OnOff_ToDoList.Domain.Entities;
using WebApi_OnOff_ToDoList.Application.Interfaces;

namespace WebApi_OnOff_ToDoList.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IConfiguration config, ILogger<AuthService> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Intento de login con campos vacíos");
                    return null;
                }

                var user = await _context.TblUsers
                    .FirstOrDefaultAsync(u => u.email == request.Email && u.isActive);

                if (user == null)
                {
                    _logger.LogWarning("Intento de login fallido: usuario no encontrado ({Email})", request.Email);
                    return null;
                }

                if (user.passwordHash != request.Password)
                {
                    _logger.LogWarning("Intento de login fallido: contraseña incorrecta ({Email})", request.Email);
                    return null;
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("Usuario {Email} autenticado correctamente", user.email);

                return new LoginResponse
                {
                    Token = token,
                    FullName = user.fullName,
                    Email = user.email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el login del usuario {Email}", request.Email);
                return null;
            }
        }

        private string GenerateJwtToken(TblUser user)
        {
            try
            {
                var jwtKey = _config["JwtSettings:Key"];
                var jwtIssuer = _config["JwtSettings:Issuer"];

                if (string.IsNullOrEmpty(jwtKey))
                    throw new InvalidOperationException("No se encontró la clave JWT en la configuración.");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.email),
                        new Claim("FullName", user.fullName)
                    };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtIssuer,
                    claims: claims,
                    expires: DateTime.Now.AddHours(4),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token JWT para el usuario {Email}", user.email);
                throw;
            }
        }
    }
}
