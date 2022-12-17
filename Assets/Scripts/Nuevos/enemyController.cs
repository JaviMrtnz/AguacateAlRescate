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

    }


    public IEnumerator enemyTurn()
    {

        getIaUnits();
        getPlayerUnits();

        foreach (Unit unit in iaUnits)
        {
            getPlayerUnits();
            yield return new WaitForSeconds(1);
            if (gmScript.selectedUnit != null)
            {
                gmScript.selectedUnit.isSelected = false;
            }
            gmScript.selectedUnit = unit;

            //metodo pa seleccionar donde mover la unidad
            Node posicion = checkNodeScore(unit);
            



            if (!unit.hasMoved)
            {
                unit.Move(posicion);
            }
            yield return new WaitForSeconds(1);

            unit.GetEnemies();

            if (!unit.hasAttacked)
            {
                List<Unit> enemies = unit.enemiesInRange;
                if (enemies.Count > 0)
                {
                    unit.Attack(enemies[0]);
                    if(enemies[0].health <= 0)
                    {
                        //Hay que intentar quitar de playerUnits el monigote que ha eliminado si no da error en la linea de comprobar la
                        //distancia con los enemigos linea 129
                        
                    }
                }
            }

        }
        gmScript.EndTurn();
    }

    public void getIaUnits()
    {
        iaUnits.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (enemy.playerNumber == 2)
            {
                
                iaUnits.Add(enemy);
            }

        }
    }

    public void getPlayerUnits()
    {
        playerUnits.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (enemy.playerNumber == 1)
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
        List<Unit> enemyUnits; //= new List<Unit>();
        List<Unit> alliedUnits; //= new List<Unit>();
        Node bestNode = null;
        

        //Node origin = Pathfinding.grid.NodeFromWorldPoint(unit.transform.position);
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
                        currentDistance = Mathf.Abs(node.gridX - playerEnemy.transform.position.x) + Mathf.Abs(node.gridY - playerEnemy.transform.position.y);
                        //Debug.Log(currentDistance + "tropa");

                    }
                    else
                    {
                        if (playerEnemy.isKing)
                            currentDistance = Mathf.Abs(node.gridX - playerEnemy.transform.position.x) + Mathf.Abs(node.gridY - playerEnemy.transform.position.y);
                        //Debug.Log(currentDistance + "rey");

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
