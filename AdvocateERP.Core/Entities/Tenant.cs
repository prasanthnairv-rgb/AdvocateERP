namespace AdvocateERP.Core.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Add other properties like Address, SubscriptionLevel, etc. later
    }
}