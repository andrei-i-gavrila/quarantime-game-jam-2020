using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Buildings;
using DefaultNamespace;
using DefaultNamespace.UI;
using Resources;
using Runtime.Resources;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectBuildPosition : MonoBehaviour
{

    public bool building;
    private GameObject template;
    public Camera camera;
    public GameObject radialMenu;
    public Inventory inventory;
    
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
        if (template != null)
        {
            var collisionList = template.GetComponent<CollisionList>();
            if (collisionList.CollisionCount() > 0)
            {
                // TODO highlight
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var price = template.GetComponent<BuildingPrice>().price;
                    for (int i = 0; i < price.Length; i++)
                    {
                        if (price[i] > 0)
                        {
                            inventory.TakeResource(i, price[i]);
                        }
                    }
                    Destroy(template.GetComponent<Rigidbody>());
                    var b = template.GetComponent<IBuilding>();
                    if (b != null)
                    {
                        b.Place();
                    }
                    template = null;
                    building = false;
                }
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
        
    }

    public void SetPrefab(GameObject gameObject)
    {
        var price = gameObject.GetComponent<BuildingPrice>().price;
        for (int i = 0; i < price.Length; i++)
        {
            if (price[i] > 0 && inventory.GetAmount(i) < price[i])
            {
                return;
            }
        }
        this.radialMenu.SetActive(false);
        this.template = Instantiate(gameObject);
        this.building = true;
    }
    
}
