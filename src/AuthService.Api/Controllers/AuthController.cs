using AutoMapper;
using AuthService.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Api.Controllers;
using AuthService.Application.Models;

namespace Kudobox.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("usuario/cadastrar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignUp(UserSignUpResource userSignUpResource)
        {
            try
            {
                var user = _mapper.Map<UserSignUpResource, User>(userSignUpResource);

                var userCreateResult = await _userManager.CreateAsync(user, userSignUpResource.Password);

                if (userCreateResult.Succeeded)
                {
                    return Created(string.Empty, string.Empty);
                }

                return Problem(userCreateResult.Errors.First().Description, null, 500);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "AuthController > SignUp - Erro Interno");
                return InternalServerError();
            }
        }

        [HttpPost("usuario/gerar-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignIn(UserLoginResource userLoginResource)
        {
            try
            {
                var user = _userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);
                if (user is null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                var userSigninResult = await _userManager.CheckPasswordAsync(user, userLoginResource.Password);

                if (userSigninResult)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    return Ok(GenerateJwt(user, roles));
                }

                return BadRequest("Email ou senha incorreto.");
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "AuthController > SignIn - Erro Interno");
                return InternalServerError();
            }
        }

        [HttpPost("criar-regra")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest("Role name should be provided.");
                }

                var newRole = new Role
                {
                    Name = roleName
                };

                var roleResult = await _roleManager.CreateAsync(newRole);

                if (roleResult.Succeeded)
                {
                    return Ok();
                }

                return Problem(roleResult.Errors.First().Description, null, 500);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "AuthController > SignIn - Erro Interno");
                return InternalServerError();
            }
        }

        [HttpPost("usuario/{userEmail}/regra")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddUserToRole(string userEmail, [FromBody] string roleName)
        {
            try
            {
                var user = _userManager.Users.SingleOrDefault(u => u.UserName == userEmail);

                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return Problem(result.Errors.First().Description, null, 500);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "AuthController > SignIn - Erro Interno");
                return InternalServerError();
            }
        }

        private TokenModel GenerateJwt(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpirationInDays));

            var tokenSecurity = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            TokenModel token = new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity),
                Expires = expires
            };

            return token;
        }
    }
}
