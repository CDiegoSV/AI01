using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Dante.Dijkstra {
	public class Dijkstra : MonoBehaviour
	{
        #region References
        [Header("References")]

        [SerializeField] protected GameObject _nodePrefab;

        #endregion

        #region Knobs
        [Header("Knobs")]
        [SerializeField] protected int _horizontalNodes;
        [SerializeField] protected int _verticalNodes;
        [Space(10)]
        [SerializeField] protected int _areaWidth;
        [SerializeField] protected int _areaHeight;
        [Space(10)]
        [Tooltip("Node obstacle detection layer.")]
        [SerializeField] protected LayerMask _obstacleLayerMask;
        #endregion

        #region RuntimeVariables

        [SerializeField, HideInInspector] protected List<Node> nodeList = new List<Node>();


        /// <summary>
        /// Horizontal distance of the nodes.
        /// </summary>
        protected float horizontalDistance;
        /// <summary>
        /// Vertical distance of the nodes.
        /// </summary>
        protected float verticalDistance;
        /// <summary>
        /// Diagonal distance between nodes.
        /// </summary>
        protected float diagonalDistance;

        #endregion

        #region EditorButtons



        #endregion

        #region UnityMethods


        #endregion

        #region PublicMethods

        /// <summary>
        /// Instanciate the graph nodes with the parameters given.
        /// </summary>
        public void InstanceNodes()
        {
            ClearAllNodesInTheList();
            horizontalDistance = _areaWidth / _horizontalNodes;
            verticalDistance = _areaHeight / _verticalNodes;
            for(int i = 0; i < _horizontalNodes; i++)
            {
                for(int j = 0; j < _verticalNodes; j++)
                {
                    GameObject nodeInstance = GameObject.Instantiate(_nodePrefab, transform.GetChild(0), false);
                    nodeInstance.transform.localPosition = new Vector3((i * horizontalDistance) * -1, 0, j * verticalDistance);
                    VerifyNodeState(nodeInstance);
                    nodeList.Add(nodeInstance.GetComponent<Node>());
                }
            }
        }

        public void ClearAllNodesInTheList()
        {
            horizontalDistance = 0;
            diagonalDistance = 0;
            foreach(Node node in nodeList)
            {
                DestroyImmediate(node.gameObject);
            }
            nodeList.Clear();
        }



        #endregion

        #region LocalMethods

        protected void VerifyNodeState(GameObject node)
        {
            int nodeHits = 0;
            if (Physics.Raycast(node.transform.position + Vector3.up * horizontalDistance,
                (node.transform.position - node.transform.position + Vector3.down * horizontalDistance).normalized, out RaycastHit upHit,
                horizontalDistance, _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (node.transform.position - node.transform.position + Vector3.up * horizontalDistance).normalized * upHit.distance, Color.yellow);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() +" Up Raycast hit an obstacle.");
                node.GetComponent<Node>()?.StateMechanic(NodeState.INACTIVE);
                return;
            }

            if (Physics.Raycast(node.transform.position + Vector3.right * horizontalDistance,
                (node.transform.position - node.transform.position + Vector3.left * horizontalDistance).normalized, out RaycastHit rightHit,
                horizontalDistance, _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (node.transform.position - node.transform.position + Vector3.right * horizontalDistance).normalized * rightHit.distance, Color.yellow);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() +" Right Raycast hit an obstacle.");
                nodeHits++;
            }
            if (Physics.Raycast(node.transform.position + Vector3.left * horizontalDistance,
                (node.transform.position - node.transform.position + Vector3.right * horizontalDistance).normalized, out RaycastHit leftHit,
                horizontalDistance, _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (node.transform.position - node.transform.position + Vector3.left * horizontalDistance).normalized * leftHit.distance, Color.yellow);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() +" Left Raycast hit an obstacle.");
                nodeHits++;
            }
            if (Physics.Raycast(node.transform.position + Vector3.forward * horizontalDistance,
                (node.transform.position - node.transform.position + Vector3.back * horizontalDistance).normalized, out RaycastHit fwdHit,
                horizontalDistance, _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (node.transform.position - node.transform.position + Vector3.forward * horizontalDistance).normalized * fwdHit.distance, Color.yellow);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() +" Forward Raycast hit an obstacle.");
                nodeHits++;
            }
            if (Physics.Raycast(node.transform.position + Vector3.back * horizontalDistance,
                (node.transform.position - node.transform.position + Vector3.forward * horizontalDistance).normalized, out RaycastHit bwdHit,
                horizontalDistance, _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (node.transform.position - node.transform.position + Vector3.back * horizontalDistance).normalized * bwdHit.distance, Color.yellow);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() +" Backward Raycast hit an obstacle.");
                nodeHits++;
            }
            if(nodeHits >= 4)
            {
                node.GetComponent<Node>()?.StateMechanic(NodeState.INACTIVE);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() + " Set to Inactive");
                return;
            }
            else
            {
                node.GetComponent<Node>()?.StateMechanic(NodeState.ACTIVE);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() + " Set to Active");
                return;
            }
        }

        #endregion

        #region GettersSetters



        #endregion
    }
}
