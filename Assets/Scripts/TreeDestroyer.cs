using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeDestroyer : MonoBehaviour
{
    [SerializeField] private GameObject chopParticle;
    private Terrain _terrain;
    private TerrainData _terrainData;
    private TerrainData _terrainDataBackup;
    private TerrainCollider _terrainCollider;
    private readonly Dictionary<TreeInstance, int> _damagedTrees = new Dictionary<TreeInstance, int>();
    private const float MaxDistanceToTree = 3.0f;
    private int _damage;
    private Rigidbody _parentRb;

    private void Awake()
    {
        _parentRb = gameObject.transform.parent.GetComponent<Rigidbody>();
        _terrain = FindObjectOfType<Terrain>();
        _terrainData = _terrain.terrainData;
        _terrainDataBackup = _terrainData;
        _terrainCollider = _terrain.GetComponent<TerrainCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain") && _parentRb.velocity.magnitude > 26f)
            DestroyTree(transform.position);
    }

    private void DestroyTree(Vector3 positionOfHit)
    {
        var instances = _terrainData.treeInstances.ToList();
        if (instances.Count <= 0) return;

        var treeInstanceToDestroyIndex = FindClosestTreeInstance(positionOfHit);
        var selectedTreePosition =
            Vector3.Scale(instances[treeInstanceToDestroyIndex].position, _terrainData.size) +
            _terrain.transform.position;
        if (Vector3.Distance(selectedTreePosition, positionOfHit) > MaxDistanceToTree) return;

        var selectedTree = instances[treeInstanceToDestroyIndex];
        if (!_damagedTrees.ContainsKey(selectedTree))
        {
            _damage = 3;
            _damagedTrees.Add(selectedTree, _damage);
        }
        else
        {
            _damage = _damagedTrees[selectedTree];
        }

        _damage--;
        Destroy(Instantiate(chopParticle, positionOfHit, Quaternion.identity), 1.0f);

        if (_damage <= 0)
        {
            instances.RemoveAt(treeInstanceToDestroyIndex);
            _terrainData.treeInstances = instances.ToArray();
            _terrainCollider.enabled = false;
            _terrainCollider.enabled = true;
            _damagedTrees.Remove(selectedTree);
        }
        else
        {
            _damagedTrees[selectedTree] = _damage;
        }
    }

    private int FindClosestTreeInstance(Vector3 hitPoint)
    {
        var treeInstances = _terrainData.treeInstances;
        var selected = 0;

        var terrainPosition = _terrain.transform.position;
        for (var i = 0; i < treeInstances.Length; i++)
        {
            var currentTreeWorldPosition =
                Vector3.Scale(treeInstances[i].position, _terrainData.size) + terrainPosition;
            var selectedTreeWorldPosition =
                Vector3.Scale(treeInstances[selected].position, _terrainData.size) + terrainPosition;

            var fromHitToTree = Vector3.Distance(hitPoint, currentTreeWorldPosition);
            var fromHitToSelected = Vector3.Distance(hitPoint, selectedTreeWorldPosition);
            if (fromHitToTree < fromHitToSelected) selected = i;
        }

        return selected;
    }
}