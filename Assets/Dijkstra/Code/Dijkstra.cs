using System.Collections.Generic;
using UnityEngine;

namespace Dante.Dijkstra {

	public class Dijkstra : MonoBehaviour
	{
        #region References
        [Header("References")]

        [SerializeField] protected GameObject _nodePrefab;
        [SerializeField] protected GameObject _connectionPrefab;

        #endregion

        #region Knobs
        [Header("Knobs")]
        [SerializeField] protected int _horizontalNodes;
        [SerializeField] protected int _verticalNodes;
        [Space(10)]
        [SerializeField] protected float _areaWidth;
        [SerializeField] protected float _areaHeight;
        [Space(10)]
        [Tooltip("Node obstacle detection layer.")]
        [SerializeField] protected LayerMask _obstacleLayerMask;
        [Space(10)]
        #endregion

        #region RuntimeVariables

        [SerializeField] protected List<Node> nodeList;
        [SerializeField] protected List<Connection> connectionList;

        protected bool newConnection;


        /// <summary>
        /// Horizontal distance of the nodes.
        /// </summary>
        [SerializeField, HideInInspector] protected float horizontalDistance;
        /// <summary>
        /// Vertical distance of the nodes.
        /// </summary>
        [SerializeField, HideInInspector] protected float verticalDistance;
        /// <summary>
        /// Diagonal distance between nodes.
        /// </summary>
        [SerializeField, HideInInspector] protected float diagonalDistance;

        #endregion

        #region UnityMethods


        #endregion

        #region PublicMethods

        /// <summary>
        /// Instanciate the graph nodes with the parameters given.
        /// </summary>
        public void InstanceNodes()
        {
            ClearAllLists();
            horizontalDistance = _areaWidth / (_horizontalNodes - 1f);
            verticalDistance = _areaHeight / (_verticalNodes - 1f);
            diagonalDistance = Mathf.Sqrt(Mathf.Pow(horizontalDistance, 2) + Mathf.Pow(verticalDistance, 2));
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

        public void ClearAllLists()
        {
            horizontalDistance = 0;
            diagonalDistance = 0;
            foreach(Node node in transform.GetChild(0).GetComponentsInChildren<Node>())
            {
                DestroyImmediate(node.gameObject);
            }
            nodeList.Clear();

            foreach (Connection connection in transform.GetChild(1).GetComponentsInChildren<Connection>())
            {
                DestroyImmediate(connection.gameObject);
            }
            connectionList.Clear();
        }

        public void CreateGraphConnections()
        {
            foreach(Node node in nodeList)
            {
                foreach (Node neighbor in nodeList)
                {
                    if (neighbor != node && neighbor.State != NodeState.INACTIVE)
                    {
                        if (neighbor.connections.Count > 0)
                        {
                            foreach(Connection connection in neighbor.connections)
                            {
                                if(connection.nodeA !=  node || connection.nodeA != neighbor)
                                {
                                    if (connection.nodeB != node || connection.nodeB != neighbor)
                                    {
                                        newConnection = true;
                                    }
                                    else
                                    {
                                        newConnection = false;
                                    }
                                }
                                else
                                {
                                    newConnection = false;
                                }
                            }
                            if (Vector3.Distance(node.transform.position, neighbor.transform.position) <= diagonalDistance && newConnection)
                            {
                                if (Physics.Raycast(node.transform.position,
                                    (node.transform.position - neighbor.transform.position).normalized, out RaycastHit nodeHit,
                                        Vector3.Distance(node.transform.position, neighbor.transform.position), _obstacleLayerMask))
                                {
                                    Debug.Log("Obstacle Hit in Connection at: " + nodeHit.point);
                                }
                                if (Physics.Raycast(neighbor.transform.position,
                                    (neighbor.transform.position - node.transform.position).normalized, out RaycastHit neighborHit,
                                        Vector3.Distance(node.transform.position, neighbor.transform.position), _obstacleLayerMask))
                                {
                                    Debug.Log("Obstacle Hit in Connection at: " + nodeHit.point);
                                }
                                else
                                {
                                    GameObject tempConnection = Instantiate(_connectionPrefab, transform.GetChild(1));
                                    tempConnection.GetComponent<Connection>().nodeA = node;
                                    tempConnection.GetComponent<Connection>().nodeB = neighbor;
                                    connectionList.Add(tempConnection.GetComponent<Connection>());
                                    node.connections.Add(tempConnection.GetComponent<Connection>());
                                    neighbor.connections.Add(tempConnection.GetComponent<Connection>());
                                }
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(node.transform.position, neighbor.transform.position) <= diagonalDistance)
                            {
                                if (Physics.Raycast(node.transform.position,
                                    (node.transform.position - neighbor.transform.position).normalized, out RaycastHit nodeHit,
                                        Vector3.Distance(node.transform.position, neighbor.transform.position), _obstacleLayerMask))
                                {
                                    Debug.Log("Obstacle Hit in Connection at: " + nodeHit.point);
                                }
                                if (Physics.Raycast(neighbor.transform.position,
                                    (neighbor.transform.position - node.transform.position).normalized, out RaycastHit neighborHit,
                                        Vector3.Distance(node.transform.position, neighbor.transform.position), _obstacleLayerMask))
                                {
                                    Debug.Log("Obstacle Hit in Connection at: " + nodeHit.point);
                                }
                                else
                                {
                                    GameObject tempConnection = Instantiate(_connectionPrefab, transform.GetChild(1));
                                    tempConnection.GetComponent<Connection>().nodeA = node;
                                    tempConnection.GetComponent<Connection>().nodeB = neighbor;
                                    connectionList.Add(tempConnection.GetComponent<Connection>());
                                    node.connections.Add(tempConnection.GetComponent<Connection>());
                                    neighbor.connections.Add(tempConnection.GetComponent<Connection>());
                                }
                            }
                        }
                    }
                    break;
                }
            }
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
                node.GetComponent<Node>().StateMechanic(NodeState.INACTIVE);
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
                node.GetComponent<Node>().StateMechanic(NodeState.INACTIVE);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() + " Set to Inactive");
                return;
            }
            else
            {
                node.GetComponent<Node>().StateMechanic(NodeState.ACTIVE);
                //Debug.Log("Node " + new Vector2(node.transform.position.x, node.transform.position.z).ToString() + " Set to Active");
                return;
            }
        }

        

        #endregion

        #region GettersSetters



        #endregion
    }
}
