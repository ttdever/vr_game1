using UnityEngine;
using UnityEngine.UI;

public class ResourcesController : MonoBehaviour
{
    [SerializeField] private ResourceModel resources;
    [SerializeField] private Text woodCountText;
    [SerializeField] private Text hungerText;
    private AudioManager _audioManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource")) {
            Resource resource = other.GetComponent<Resource>();
            ResourceType type = resource.type;

            switch (type) {
                case ResourceType.Food:
                    if (resources.Hunger > 0)
                    {
                        resources.Hunger--;
                    }
                    _audioManager.Play("Apple");
                    break;
                case ResourceType.Wood:
                    resources.Wood++;
                    _audioManager.Play("Pick");
                    break;
            }
            
            Destroy(other.gameObject, 0.2f);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(AddHunger), 0f, 120f);
        _audioManager = FindObjectOfType<AudioManager>();
        resources.Wood = 100;
    }

    private void Update()
    {
        woodCountText.text = "Wood Cont: " + resources.Wood;
        hungerText.text = "Hunger: " + resources.Hunger;
    }

    private void AddHunger()
    {
        resources.Hunger++;
        if (resources.Hunger > 100)
        {
            Debug.Log("Dead");
        }
    }

    public int GetWoodCount()
    {
        return resources.Wood;
    }

    public void DecreaseWoodCount(int minus)
    {
        resources.Wood -= minus;
    }
}


public struct ResourceModel 
{
    public int Hunger;
    public int Wood;
}