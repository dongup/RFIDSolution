using Microsoft.AspNetCore.Http;
using RFIDSolution.Shared.DAL.Entities;

namespace RFIDSolution.WebApi.Models
{
    public class LogModel : LogEntity
    {
        private HttpContext _context;
        public LogModel() : base()
        {

        }

        public LogModel(HttpContext httpContext) : base()
        {
            _context = httpContext;
            RequestIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
            Method = httpContext.Request.Method;
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            RequestUrl = httpContext.Request.Path;

            switch (Method)
            {
                case "PUT":
                    Level = LogLevel.Put;
                    LogContent = $"Ip {RequestIpAddress} đã update dữ liệu";
                    break;
                case "DELETE":
                    Level = LogLevel.Delete;
                    LogContent = $"Ip {RequestIpAddress} đã xóa dữ liệu";
                    break;
                default:
                    Level = LogLevel.Info;
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
