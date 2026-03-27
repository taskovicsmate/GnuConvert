
namespace GnuConvert.ExceptionHandling
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }

        protected AppException(string errorCode, string message, Exception? inner = null)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
