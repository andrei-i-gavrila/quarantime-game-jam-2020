using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionList : MonoBehaviour
{

    private HashSet<GameObject> colliding = new HashSet<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherGameObject = other.gameObject;
        if (otherGameObject.layer != 9)
        {
            colliding.Add(otherGameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        var otherGameObject = other.gameObject;
        if (otherGameObject.layer != 9)
        {
            colliding.Remove(otherGameObject);
        }
    }

    public int CollisionCount()
    {
        return colliding.Count;
    }
}
