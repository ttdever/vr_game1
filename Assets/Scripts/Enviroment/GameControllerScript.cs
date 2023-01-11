using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] trees;
    
    void Start()
    {
       // CreateTreesInstances();
    }

    private void CreateTreesInstances()
    {
        //Create treeParent:
        GameObject treeParent = new GameObject("TreeParent");
        treeParent.transform.parent = transform;
        
        //Calculate trees positions
        TerrainData terrainData = GetComponent<Terrain>().terrainData;
        Vector3 currentPosition = transform.position;
        float terrainWidth = terrainData.size.x;
        float terrainHeight = terrainData.size.z;
        float y = terrainData.size.y;

        //Spawn trees:
        foreach (TreeInstance tree in terrainData.treeInstances)
        {
            Vector3 position = new Vector3( (tree.position.x) * terrainWidth + currentPosition.x,
                                            (tree.position.y) * y + currentPosition.y, 
                                            (tree.position.z) * terrainHeight + currentPosition.z);
            Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            
            GameObject spawnedTree = Instantiate(trees[tree.prototypeIndex], position, Quaternion.identity, treeParent.transform);
            spawnedTree.transform.localScale = scale;
        }
    }
}
