namespace Resources
{
    public interface IResource
    {
        int MaxAmount { get; set; }
        int Amount { get; set; }
        float Depletion { get; }
    }
}