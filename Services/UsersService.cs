using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Okta.Auth.Sdk;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using OktaScimDemo.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OktaAbstractionsClientConfiguration = Okta.Sdk.Abstractions.Configuration.OktaClientConfiguration;

namespace OktaScimDemo.Services
{
    public class UsersService
    {
        private readonly IConfiguration configuration;

        public UsersService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IUser> RegisterUser(RegisterUserRequest model)
        {
            var clientConfig = GetOktaClientConfig();
            var client = new OktaClient(clientConfig);

            var roleCustomAttributeName = configuration["Okta:CustomAttributes:Role:Name"];
            var roleCustomAttributeValue = configuration["Okta:CustomAttributes:Role:Guest"];

            var userProfile = new UserProfile
            {
                Login = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            userProfile.SetProperty(roleCustomAttributeName, roleCustomAttributeValue);

            var options = new CreateUserWithPasswordOptions
            {
                Profile = userProfile,
                Password = model.Password,
            };

            var result = await client.Users.CreateUserAsync(options);
            return result;
        }

        public async Task<IAuthenticationResponse> AuthenticateUserAsync(LogInRequest model)
        {
            var options = new AuthenticateOptions
            {
                Username = model.Email,
                Password = model.Password,
            };

            var clientConfiguration = GetOktaAuthClientConfig();
            var client = new AuthenticationClient(clientConfiguration);

            var result = await client.AuthenticateAsync(options);
            return result;
        }

        public async Task<string> GetUserIdAsync(string email)
        {
            var clientConfig = GetOktaClientConfig();
            var client = new OktaClient(clientConfig);

            var user = await client.Users.GetUserAsync(email);
            return user.Id;
        }

        public async Task<IUser> GetUserAsync(string userId)
        {
            var clientConfig = GetOktaClientConfig();
            var client = new OktaClient(clientConfig);

            var user = await client.Users.GetUserAsync(userId);
            return user;
        }

        public async Task<IUser> UpdateUserAsync(string userId, UpdateUserRequest model)
        {
            var clientConfig = GetOktaClientConfig();
            var client = new OktaClient(clientConfig);

            var user = await client.Users.GetUserAsync(userId);
            user.Profile.FirstName = model.FirstName;
            user.Profile.LastName = model.LastName;

            var result = await client.Users.UpdateUserAsync(user, user.Id, false);
            return result;
        }

        public string GenerateJwt(string userId)
        {
            var userClaims = new List<Claim>
            {
                    new Claim("userId", userId),
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(signingCredentials: signingCredentials, claims: userClaims);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private OktaClientConfiguration GetOktaClientConfig()
        {
            var clientConfig = new OktaClientConfiguration
            {
                Token = configuration["Okta:ApiToken"],
                ClientId = configuration["Okta:ClientId"],
                OktaDomain = configuration["Okta:Domain"],
            };
            return clientConfig;
        }

        private OktaAbstractionsClientConfiguration GetOktaAuthClientConfig()
        {
            var clientConfig = new OktaAbstractionsClientConfiguration
            {
                OktaDomain = configuration["Okta:Domain"],
                Token = configuration["Okta:ApiToken"]
            };
            return clientConfig;
        }
    }
}
