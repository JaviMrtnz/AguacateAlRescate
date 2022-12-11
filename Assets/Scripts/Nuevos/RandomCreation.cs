using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCreation : MonoBehaviour
{
    [SerializeField] public GameObject king;
    [SerializeField] public GameObject knight;
    [SerializeField] public GameObject archer;
    [SerializeField] public GameObject dragon;
    [SerializeField] public GameObject village;
    

    [SerializeField] public List<GameObject> spawnpoints;


    private GameObject unitParent;

    
    Node currentNode;
    

    // Start is called before the first frame update
    void Start()
    {
        unitParent = GameObject.FindGameObjectWithTag("unitParent").gameObject;
        

        int rand = Random.Range(0, spawnpoints.Count);
        Instantiate(king, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
        //Debug.Log(spawnpoints[rand].transform.position);
        currentNode = Pathfinding.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
        //Debug.Log(currentNode);
        currentNode.walkable = false;
        currentNode.hasUnit = true;
        spawnpoints.RemoveAt(rand);

        for (int i = 0; i < 3; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(knight, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            //currentNode = Pathfinding.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            //currentNode.walkable = false;
            //currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }

        for (int i = 0; i < 2; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(archer, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            
            //currentNode = Pathfinding.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            //currentNode.walkable = false;
            //currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }

        for (int i = 0; i < 2; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(dragon, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            //currentNode = Pathfinding.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            //currentNode.walkable = false;
            //currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }


        /*for (int i = 0; i < 3; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(village, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            currentNode = Pathfinding.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            currentNode.walkable = false;
            spawnpoints.RemoveAt(rand);
        }*/

       
    }
        
}