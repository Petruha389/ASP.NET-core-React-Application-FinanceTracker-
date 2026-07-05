namespace FinanceTracker.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object id)
        : base($"Сущность '{entityName}' с идентификатором '{id}' не найдена.") { }
}