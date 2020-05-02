namespace Resources
{
    public interface IResourceSite
    {
        int MaxAmount { get; set; }
        int Amount { get; set; }
        float Depletion { get; }
        string ResourceName { get; }
        int ResourceId { get; }
    }
}