using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderTree : MonoBehaviour
{
    [SerializeField] public GameObject[] trees;

    [SerializeField] public List<GameObject> points;

    public GameObject treeParent;

    Node currentNode;

    // Start is called before the first frame update
    void Start()
    {
        treeParent = GameObject.FindGameObjectWithTag("treeParent").gameObject;

        for(int i = 0; i< points.Count;i++)
        {
            Instantiate(trees[0], points[i].transform.position, Quaternion.identity, treeParent.transform);
            currentNode = Pathfinding.grid.NodeFromWorldPoint(points[i].transform.position);
            currentNode.walkable = false;
            currentNode.hasTree = true;
        }

        
    }
}