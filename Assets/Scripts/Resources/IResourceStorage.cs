namespace Resources
{
    public interface IResourceStorage
    {
        bool Accepts(int resource);
        bool AddResource(int resource, int amount);
        bool TakeResource(int resource, int amount);
    }
}