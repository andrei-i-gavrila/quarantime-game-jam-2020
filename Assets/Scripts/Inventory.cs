using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int ResourceCount = 3;
    private float[] ProductionModifiers = {1f, 1f, 1f};

    // TODO: Get Amount / Capacity cache (emit from storage to update)
    private List<IResourceStorage>[] _storages;
    private List<IProducent>[] _producents;
    private bool _changed;
    private bool _init;

    private void Start()
    {
        if (!_init) Init();
        InvokeRepeating(nameof(ProduceResources), 0f, 1f);
    }

    public int GetAmount(int resource)
    {
        if (!_init) Init();
        return _storages[resource].Count > 0
            ? _storages[resource].Aggregate(0, (sum, unit) => sum + unit.StoredResource(resource))
            : 0;
    }

    public int GetCapacity(int resource)
    {
        if (!_init) Init();
        return _storages[resource].Count > 0
            ? _storages[resource].Aggregate(0, (sum, unit) => sum + unit.StorageCapacity)
            : 0;
    }

    public void RegisterStorage(int resource, IResourceStorage storage)
    {
        if (!_init) Init();
        _storages[resource].Add(storage);
    }

    public void UnregisterStorage(int resource, IResourceStorage storage)
    {
        if (!_init) Init();
        _storages[resource].Remove(storage);
    }

    public void RegisterProducent(int resource, IProducent producent)
    {
        if (!_init) Init();
        _producents[resource].Add(producent);
    }

    public void UnregisterProducent(int resource, IProducent producent)
    {
        if (!_init) Init();
        _producents[resource].Remove(producent);
    }

    private void ProduceResources()
    {
        if (!_init) Init();
        for (int resource = 0; resource < ResourceCount; resource++)
        {
            var amount = _producents[resource].Aggregate(0, (sum, producent) =>
                sum + producent.Produce(ProductionModifiers[resource]));
            StoreResource(resource, amount);
        }
    }

    private void StoreResource(int resource, int amount)
    {
        var done = false;
        _storages[resource].ForEach(storage =>
        {
            if (!done && storage.AddResource(resource, amount))
            {
                done = true;
            }
        });
    }

    private void Init()
    {
        _storages = new List<IResourceStorage>[ResourceCount];
        _producents = new List<IProducent>[ResourceCount];
        for (int i = 0; i < ResourceCount; i++)
        {
            _storages[i] = new List<IResourceStorage>();
            _producents[i] = new List<IProducent>();
        }

        _init = true;
    }
}