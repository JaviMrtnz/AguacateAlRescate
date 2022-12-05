using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    public struct BattleAIInfo
    {
        public Unit enemigo;
        public List<Unit> enemiesInRange;
    }
}
