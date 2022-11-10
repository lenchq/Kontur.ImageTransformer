using System.Net;

namespace Kontur.ImageTransformer.Extensions
{
    public static class StatusHelper
    {
        public static int InternalServerError()
        {
            return (int) HttpStatusCode.InternalServerError;
        }

        public static int NotFound()
        {
            return (int) HttpStatusCode.NotFound;
        }

        public static int NoContent()
        {
            return (int) HttpStatusCode.NoContent;
        }
        public static int BadRequest()
        {
            return (int) HttpStatusCode.BadRequest;
        }

        public static int Ok()
        {
            return (int) HttpStatusCode.OK;
        }

        public static int RequestTimeout()
        {
            return (int) HttpStatusCode.RequestTimeout;
        }
    }
}