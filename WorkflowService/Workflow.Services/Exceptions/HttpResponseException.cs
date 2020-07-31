using System;
using System.Net;

namespace Workflow.Services.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        public HttpResponseException(HttpStatusCode statusCode, object value = null)
        {
            Status = statusCode;
            Value = value;
        }
    }
}
