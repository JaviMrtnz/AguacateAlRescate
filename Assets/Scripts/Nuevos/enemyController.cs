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
        float currentDistance = 0;
        float minDistance = 500;
        int enemyDamage;
        int unitDamage;
        List<Unit> enemyUnits = new List<Unit>();
        List<Unit> alliedUnits = new List<Unit>();
        Node bestNode = null;
        

        Node origin = Pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
        List<Node> walkableNodes = unit.nGetWalkableTiles();

        foreach(Node node in walkableNodes)
        {
            //Debug.Log(node.worldPosition.x);
            int currentScore = 0;
            enemyUnits = gmScript.checkEnemies(node);
            alliedUnits = gmScript.checkAllies(node);

            if (enemyUnits.Count == 0)
            {
                //no hay enemigos alrededor de la casilla
                foreach (Unit playerEnemy in playerUnits)
                {
                    enemyDamage = unit.attackDamage - playerEnemy.armor;
                    if (playerEnemy.health <= enemyDamage)
                    {
                        currentDistance = Mathf.Abs(node.worldPosition.x - playerEnemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - playerEnemy.transform.position.y);
                        Debug.Log(currentDistance + "tropa");

                    }
                    else
                    {
                        if (playerEnemy.isKing)
                            currentDistance = Mathf.Abs(node.worldPosition.x - playerEnemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - playerEnemy.transform.position.y);
                        Debug.Log(currentDistance + "rey");

                    }
                    if (currentDistance <= minDistance && currentDistance > 0)
                    {
                        minDistance = currentDistance;
                        bestNode = node;
                        
                    }
                }
            }
            else if (enemyUnits.Count == 1)
            {
                //hay un enemigo
                unitDamage = enemyUnits[0].defenseDamage - unit.armor;
                enemyDamage = unit.attackDamage - enemyUnits[0].armor;
                //lo puedo matar
                if (enemyUnits[0].health <= enemyDamage)
                {
                    currentScore = 50;
                }
                //se queda vivo pero tocado
                else if (unitDamage <= enemyDamage)
                {
                    currentScore = 20;
                }
                //es el rey, a muerte
                else if (enemyUnits[0].isKing)
                {
                    currentScore = 30;
                }
                //malisimo
                else currentScore = 1;
            }
            else if (enemyUnits.Count > 1)
            {
                
                //hay enemigos alrededor de la casilla
                foreach (Unit playerEnemy in enemyUnits)
                {
                    enemyDamage = unit.attackDamage - playerEnemy.armor;
                    // unitDamage = playerEnemy.defenseDamage - unit.armor;
                    //tiene poca vida y puede morir
                    if (playerEnemy.health <= enemyDamage)
                    {
                        currentScore = 50;
                    }
                }
            }
            //más aliados que enemigos
            if (alliedUnits.Count >= enemyUnits.Count)
            {
                currentScore += alliedUnits.Count;
            }
            //más enemigos, corred insensatos
            else currentScore = 1;

            if (currentScore >= maxScore)
            {
                maxScore = currentScore;
                bestNode = node;
            }

        }
        return bestNode;
    }
}
