namespace Resources
{
    public interface IResourceStorage
    {
        int Stored { get; }
        int StorageCapacity { get; }
        bool Accepts(int resource);
        bool AddResource(int resource, int amount);
        bool TakeResource(int resource, int amount);
        int StoredResource(int resource);
    }
}