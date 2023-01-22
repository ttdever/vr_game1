using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToSpawn;
    
    private int listSize = 0;
    
    private Vector3 _startSpawnBound;
    private Vector3 _endSpawnBound;
    
    void Start()
    {
        listSize = _objectsToSpawn.Count;
        InvokeRepeating(nameof(SpawnRandomObject), 0, 120f);
        Collider m_Collider = GetComponent<Collider>();
        _startSpawnBound = m_Collider.bounds.min;
        _endSpawnBound = m_Collider.bounds.max;
    }

    private void SpawnRandomObject()
    {
        var random = Random.Range(0, listSize - 1);
        var randomSpawnCoordinates = new Vector3(
            Random.Range(_startSpawnBound.x, _endSpawnBound.x),
            Random.Range(_startSpawnBound.y, _endSpawnBound.y),
            Random.Range(_startSpawnBound.z, _endSpawnBound.z));

        Instantiate(_objectsToSpawn[random], randomSpawnCoordinates, Quaternion.identity);
    }
}
