using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using RFIDSolution.WebApi.Models;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;

namespace RFIDSolution.Middlewares
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CustomMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, UserManager<UserEntity> userService, AppDbContext db)
        {
            //Console.WriteLine("========================Custom middle ware======================");
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = new UserEntity();
            if (token != null)
                user = await attachUserToContext(context, userService, token);

            if(context.Request.Method == "PUT" || context.Request.Method == "DELETE")
            {
                context.Request.EnableBuffering();
                int bufferSize = 1024;
                string body = "";
                // Leave the body open so the next middleware can read it.
                using (var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: bufferSize,
                    leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    // Do some processing with body…

                    // Reset the request body stream position so the next middleware can read it
                    context.Request.Body.Position = 0;
                }

                LogModel log = new LogModel(context);
                log.RequestUserId = user.Id;
                log.Token = token;
                log.RequestBody = body;

                Console.WriteLine("=========Saving log=========");
                db.DetachAllEntities();
                db.LOG.Add(log);
                db.SaveChanges();
            }
            await _next(context);
        }
     
        public async Task ReplaceBody(HttpContext httpContext, ResponseModel<bool> rspns)
        {
            var originBody = httpContext.Response.Body;
            try
            {
                var memStream = new MemoryStream();
                httpContext.Response.Body = memStream;

                memStream.Position = 0;

                //Custom logic to modify response
                string newResponseBody = JsonConvert.SerializeObject(rspns);

                var memoryStreamModified = new MemoryStream();
                var sw = new StreamWriter(memoryStreamModified);
                sw.Write(newResponseBody);
                sw.Flush();
                memoryStreamModified.Position = 0;

                await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
            }
            finally
            {
                httpContext.Response.Body = originBody;
            }
        }

        private async Task<UserEntity> attachUserToContext(HttpContext context, UserManager<UserEntity> userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authSigningKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string userName = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                UserEntity user = await userService.FindByNameAsync(userName);
                if(user == null)
                {
                    context.Response.StatusCode = 401;
                }
                else
                {
                    // attach user to context on successful jwt validation
                    context.Items["User"] = user;
                }
                return user;
            }
            catch
            {
                return new UserEntity();
            }
        }
    }
}


