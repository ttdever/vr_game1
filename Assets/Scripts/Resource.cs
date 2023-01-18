using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] public ResourceType type;
    
}

public enum ResourceType {
    Food,
    Wood,
    Heat
}


