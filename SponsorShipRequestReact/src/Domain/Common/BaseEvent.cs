
namespace TechAssessment.Domain.Common;

public interface INotification { }

public abstract class BaseEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
