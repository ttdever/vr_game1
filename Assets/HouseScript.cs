using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HouseScript : MonoBehaviour
{
    [SerializeField] private Material alphaMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private List<GameObject> planned;
    

    private GameObject _pickedPiece;
    private ResourcesController _resourcesController;
    private readonly int _pricePerPiece = 2;
    private int _hits = 0;
    private AudioManager _audioManager;
    
    private void Start()
    {
        _resourcesController = FindObjectOfType<ResourcesController>();
        
        foreach (var plannedPiece in planned)
        {
            plannedPiece.GetComponent<MeshRenderer>().material = alphaMaterial;
            plannedPiece.GetComponent<Collider>().isTrigger = true;
        }

        _pickedPiece = planned[0];
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         PickNextPiece();
    //     }
    // }

    private void PickNextPiece()
    {
        _hits = 0;
        _pickedPiece.GetComponent<MeshRenderer>().material = normalMaterial;
        _pickedPiece.GetComponent<Collider>().isTrigger = false;
        planned.Remove(_pickedPiece);

        if (planned.Count > 0)
        {
            _pickedPiece = planned[0];
        }
        else
        {
            Debug.Log("The Industrial Revolution and its consequences have been a disaster for the human race");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hammer") && _resourcesController.GetWoodCount() > 0)
        {
            _resourcesController.DecreaseWoodCount(1);
            _hits++;
            _audioManager.Play("Hammer");
            if (_hits >= _pricePerPiece)
            {
                PickNextPiece();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer") && _resourcesController.GetWoodCount() > 0)
        {
            _resourcesController.DecreaseWoodCount(1);
            _hits++;
            _audioManager.Play("Hammer");
            if (_hits >= _pricePerPiece)
            {
                PickNextPiece();
            }
        }
    }
}
