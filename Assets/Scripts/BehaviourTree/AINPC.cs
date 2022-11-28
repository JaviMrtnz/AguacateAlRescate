//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AINPC : AIBase
//{
//    /// <summary>
//    /// Tutorial minuto 25:50 dejo de escribir 
//    /// </summary>
//    enum NodeUpdateResult
//    {
//        Running,
//        Success,
//        Failure
//    }

//    interface IAINode
//    {
//        void Init();
//        NodeUpdateResult Update();
//    }

//    //NODOS
//    class AISequenceNode : IAINode
//    {
//        public AISequenceNode(IList<IAINode> i_nodes)
//        {
//            m_subNodes = i_nodes;

//        }

//        public void Init()
//        {
//            foreach (IAINode node in m_subNodes)
//            {
//                node.Init();
//            }
//        }

//        public NodeUpdateResult Update()
//        {
//            foreach(IAINode node in m_subNodes)
//            {
//                NodeUpdateResult subNodeResult = node.Update();
//                if(subNodeResult == NodeUpdateResult.Running)
//                {
//                    return NodeUpdateResult.Running;
//                }
//                else if (subNodeResult == NodeUpdateResult.Failure)
//                {
//                    return NodeUpdateResult.Failure;
//                }  
//            }
//            return NodeUpdateResult.Success;
//        }

//        IList<IAINode> m_subNodes;
//    }

//    class AIMoveNode : IAINode
//    {
//        public AIMoveNode(BattleAIInfo i_battleAIInfo, GameObject i_pawn/*, aqui pone el enemy controller*/)
//        {
//            m_battleInfo = i_battleAIInfo;
//            m_pawn = i_pawn;
//        }

//        public void Init()
//        {
            
//        }

//        public NodeUpdateResult Update()
//        {
//            //Estoy cerca? Si?--> Success No?--> intento acercarme

//            /*if (Comprobacion de distancia) <= attackRadius)
//            {
//                return NodeUpdateResult.Success;
//            }
//            else
//            {
//                (Codigo del Movimiento)
//                return NodeUpdateResult.Running;
//            }
//            */

//        }

//        BattleAIInfo m_battleInfo;
//        GameObject m_pawn;
//    }



//    //Fin NODOS
//    IAINode m_root;


//    // Start is called before the first frame update
//    void Start()
//    {
//        List<IAINode> sequenceList = new List<IAINode>();
//        //IAINode move = new AIMoveNode(Aqui en el tutorial pone el controlador de movimiento del enemigo "min 25:30"); //Nodo Movimiento
//        //IAINode attack = ...;//Nodo Ataque

//        //sequenceList.Add(move);
//        //sequenceList.Add(attack);

//        m_root = new AISequenceNode(sequenceList);
//        m_root.Init();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        m_root.Update();    }
//}
