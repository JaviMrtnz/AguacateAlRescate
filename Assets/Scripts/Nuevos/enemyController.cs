using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{

    

    public GameObject gm;
    public GM gmScript;

    public List<Unit> iaUnits = new List<Unit>();
    public List<Unit> playerUnits = new List<Unit>();

    public void Start()
    {
        
        //gm = GetComponent<GM>();
        //gm = GameObject.Find("GameMaster");
        //gmScript = gm.GetComponent<GM>();

    }


    public IEnumerator enemyTurn()
    {
        getAllUnits();
        //Debug.Log("corutinea");
        

        foreach(Unit unit in iaUnits)
        {
            //if (gm.selectedUnit != null)
            //{ 
            //    gm.selectedUnit.isSelected = false;
            //}

            //metodo pa seleccionar donde mover la unidad
            Node posicion = checkNodeScore(unit);
            unit.Move(posicion);
            
        }
        yield return new WaitForSeconds(1);
    }

    private void getAllUnits()
    {
        iaUnits.Clear();
        playerUnits.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (enemy.playerNumber == 2)
            {
                iaUnits.Add(enemy);
            }
            else
            {
                playerUnits.Add(enemy);
            }
        }
    }

    Node checkNodeScore(Unit unit)
    {
        int maxScore = 0;
        int currentScore = 0;
        List<Unit> enemyNumber = new List<Unit>();
        Node bestNode = null;
        

        Node origin = Pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
        List<Node> walkableNodes = unit.nGetWalkableTiles();

        foreach(Node node in walkableNodes)
        {
            currentScore = 0;
            enemyNumber = gmScript.checkEnemies(node);
           


            if (enemyNumber.Count == 1)
                currentScore = 20;
            if (enemyNumber.Count > 1)
                currentScore = 70;
            if (enemyNumber.Count == 0)
                currentScore = 50;
            if (currentScore >= maxScore)
            {
                maxScore = currentScore;
                bestNode = node;

            }
        }
        return bestNode;
    }
}
