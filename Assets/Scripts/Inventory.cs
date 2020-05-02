using System.Collections.Generic;
using System.Linq;
using Buildings;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public const int ResourceCount = 3;
    
    private List<StorageUnit>[] _storages;
    private bool _changed;

    private void Awake()
    {
        _storages = new List<StorageUnit>[ResourceCount];
        for (int i = 0; i < ResourceCount; i++)
        {
            _storages[i] = new List<StorageUnit>();
        }
    }

    public int GetAmount(int resource)
    {
        
        return _storages[resource].Count > 0
            ? _storages[resource].Aggregate(0, (sum, unit) => sum + unit.Stored)
            : 0;
    }

    public int GetCapacity(int resource)
    {
        return _storages[resource].Count > 0
            ? _storages[resource].Aggregate(0, (sum, unit) => sum + unit.StorageCapacity)
            : 0;
    }

    public void RegisterStorage(int resource, StorageUnit storage)
    {
        _storages[resource].Add(storage);
    }

    public void UnregisterStorage(int resource, StorageUnit storage)
    {
        _storages[resource].Remove(storage);
    }
}
