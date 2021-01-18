using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domains.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

using NIIC.Application.ApplicationSettings;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security
{
    
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IOptions<Jwt> _jwt;

        public JwtGenerator(IOptions<Jwt> jwt)
        {
            _jwt = jwt;
        }
  
        public string CreateToken(AppUser user)
        {
            //build token to return it 
            //take list of user claims to return it inside our token -->userName
            //store token in local storage 
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // generate signing credentials
            //because each token has to be signed by api before it leaves the api

           
            //set out key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.Key));
            
            //select strong algorithm 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //above creds allows the server to validate each request very fast way without query the db 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),//pass our claims
                Expires = DateTime.UtcNow.AddMinutes(_jwt.Value.DurationInMinutes), // token expire after
                SigningCredentials = creds //
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
