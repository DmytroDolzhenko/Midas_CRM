namespace Midas.Core
{
    public interface IOwnedEntity
    {
        Guid OwnerId { get; }
    }
}
