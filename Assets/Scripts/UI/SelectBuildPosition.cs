using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DefaultNamespace.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectBuildPosition : MonoBehaviour
{

    public bool building;
    public GameObject template;
    public Camera camera;
    public GameObject radialMenu;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Destroy(template);
            radialMenu.SetActive(true);
        };
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(template);
            template = null;
            building = false;
        };
        var collisionList = template.GetComponent<CollisionList>();
        if (collisionList.CollisionCount() > 0)
        {
            // TODO highlight
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(template.GetComponent<Rigidbody>());
                template = null;
                building = false;
            }
        }

        if (building)
        {
            // template.SetActive(true);
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            int mask = LayerMask.GetMask("Terrain");
            
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mask)) {
                var objectHit = hit.point;

                template.transform.position = objectHit;
            }
        }
        else
        {
            // template.SetActive(false);
        }
        
    }

    public void SetPrefab(GameObject gameObject)
    {
        this.radialMenu.SetActive(false);
        this.template = Instantiate(gameObject);
        this.building = true;
    }
    
}
