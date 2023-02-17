using BE.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BE.Helpers
{
    public class CustomControllerBase : ControllerBase
    {
        public ActionResult ConvertResponse<T>(ApiRespones<T> respones)
        {
            switch (respones.Status)
            {
                case HttpStatusCode.OK:
                    respones.Message = "SUCCESS";
                    return Ok(respones);
                case HttpStatusCode.Created:
                    respones.Message = "CREATED";
                    return Ok(respones);
                case HttpStatusCode.NoContent:
                    respones.Message = "NO CONTENT";
                    return Ok(respones);
                case HttpStatusCode.BadRequest:
                    return BadRequest(respones);
                case HttpStatusCode.Unauthorized:
                    respones.Message = "UNAUTHORIZED";
                    return Unauthorized(respones);  
                case HttpStatusCode.Forbidden:
                    respones.Message = "FORBIDDEN";
                    return BadRequest(respones);
                case HttpStatusCode.NotFound:
                    respones.Message = "NOT FOUND";
                    return NotFound(respones);
                default:
                    return BadRequest(respones);
            }
        }
    }
}
