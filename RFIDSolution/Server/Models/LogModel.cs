using Microsoft.AspNetCore.Http;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.WebApi.Models
{
    public class LogModel : LogEntity
    {
        private HttpContext _context;
        public LogModel() : base()
        {

        }

        public LogModel(HttpContext httpContext, UserEntity user) : base()
        {
            if (user == null) return;

            _context = httpContext;
            RequestIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
            Method = httpContext.Request.Method;
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            RequestUrl = httpContext.Request.Path;

            switch (Method)
            {
                case "PUT":
                    Level = LogLevelEnum.Put;
                    LogContent = $"User {user.UserName} update data at ip {RequestIpAddress} ";
                    break;
                case "DELETE":
                    Level = LogLevelEnum.Delete;
                    LogContent = $"User {user.UserName} deleted data at ip {RequestIpAddress} ";
                    break;
                case "POST":
                    Level = LogLevelEnum.Info;
                    LogContent = $"User {user.UserName} added data at ip {RequestIpAddress} ";
                    break;
                default:
                    Level = LogLevelEnum.Info;
                    break;
            }
        }

        protected string GetRequestUrl()
        {
            string schema = _context.Request.Scheme;
            string host = _context.Request.Host.Host;
            int port = (int)_context.Request.Host.Port;
            string path = _context.Request.Path.Value;
            string query = _context.Request.QueryString.Value;
            string request_url = $"{path}{query}";
            return request_url;
        }
    }
}
