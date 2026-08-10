namespace L2.Admin.Exceptions;

public sealed class AdminRepositoryException(string message, Exception innerException)
    : Exception(message, innerException);
