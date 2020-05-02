using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuildPosition : MonoBehaviour
{

    public bool building;
    public GameObject template;
    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            building = true;
        };
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            building = false;
        };
        
        if (building)
        {
            // template.SetActive(true);
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Debug.Log("----");
            Debug.Log(ray);
            int mask = LayerMask.GetMask("Terrain");
            
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask)) {
                Debug.Log(hit);
                Debug.Log(hit.transform.position);
                var objectHit = hit.point;

                template.transform.position = objectHit;
            }
        }
        else
        {
            // template.SetActive(false);
        }
        
    }
}
