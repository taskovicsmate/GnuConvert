
namespace GnuConvert.ExceptionHandling
{
    public sealed class DomainException : AppException
    {
        public DomainException(string code, string message, Exception? inner = null)
            : base(code, message, inner) { }
    }

    public sealed class PersistenceException : AppException
    {
        public PersistenceException(string code, string message, Exception? inner = null)
            : base(code, message, inner) { }
    }

    public sealed class ConversionException : AppException
    {
        public ConversionException(string code, string message, Exception? inner = null)
            : base(code, message, inner) { }
    }

    public sealed class ConfigurationException : AppException
    {
        public ConfigurationException(string code, string message, Exception? inner = null)
            : base(code, message, inner) { }
    }
}
