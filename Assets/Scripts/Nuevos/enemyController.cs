using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{

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
 
        
        foreach (Unit unit in iaUnits)
        {

            yield return new WaitForSeconds(1);
            if (gmScript.selectedUnit != null)
            {
                gmScript.selectedUnit.isSelected = false;
            }
            gmScript.selectedUnit = unit;

            //metodo pa seleccionar donde mover la unidad
            Node posicion = checkNodeScore(unit);
            unit.Move(posicion);
            
        }
       
    }

    public void getAllUnits()
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
        float currentDistance = 0;
        float minDistance = 500;
        List<Unit> enemyNumber = new List<Unit>();
        Node bestNode = null;
        

        Node origin = Pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
        List<Node> walkableNodes = unit.nGetWalkableTiles();

        foreach(Node node in walkableNodes)
        {
            //Debug.Log(node.worldPosition.x);
            currentScore = 0;
            enemyNumber = gmScript.checkEnemies(node);
           
            if(enemyNumber.Count == 0)
            {
                //no hay enemigos alrededor de la casilla
                foreach (Unit playerEnemy in playerUnits)
                {
                    //Debug.Log("entra al foreach");
                    if (playerEnemy.health <= unit.attackDamage)
                    {
                        currentDistance = Mathf.Abs(node.worldPosition.x - playerEnemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - playerEnemy.transform.position.y);
                        Debug.Log(playerEnemy);
                    }
                    else
                    {
                        if (playerEnemy.isKing)
                            currentDistance = Mathf.Abs(node.worldPosition.x - playerEnemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - playerEnemy.transform.position.y);
                    }
                    if (currentDistance <= minDistance)
                    {
                        minDistance = currentDistance;
                        bestNode = node;
                    }
                }
                
            }
            //else
            //{
            //    //hay enemigos alrededor de la casilla

            //}

        }
        return bestNode;
    }
}
