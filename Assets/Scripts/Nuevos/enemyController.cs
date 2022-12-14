using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{

    public GM gm;

    public List<Unit> iaUnits = new List<Unit>();
    public List<Unit> playerUnits = new List<Unit>();


    public IEnumerator enemyTurn()
    {
        getAllUnits();
        Debug.Log("corutinea");
        

        foreach(Unit unit in iaUnits)
        {
            //if (gm.selectedUnit != null)
            //{ 
            //    gm.selectedUnit.isSelected = false;
            //}

            //metodo pa seleccionar donde mover la unidad
            Node posicion = new Node(true, Vector3.zero, 0, 0);
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
}
