using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : MonoBehaviour
{
    [SerializeField]
    private ResourceModel resources;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Resource") {
            Resource resource = other.GetComponent<Resource>();
            ResourceType type = resource.type;

            switch (type) {
                case ResourceType.Food:
                    resources.Food++;
                    break;
                case ResourceType.Wood:
                    resources.Wood++;
                    break;
                case ResourceType.Heat:
                    resources.Heat++;
                    break;
            }
        }
    }
}


public struct ResourceModel 
{
    public int Food;
    public int Wood;
    public int Heat;
}