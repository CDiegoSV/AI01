using Dante.Agents;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.Dijkstra {

    #region Structs

    [System.Serializable]
    public struct Route
    {
        public List<Node> routeNodes;
        public float distance;
    }

    #endregion

    public class Dijkstra : MonoBehaviour
    {
        #region References
        [Header("References")]

        [SerializeField] protected GameObject _nodePrefab;
        [SerializeField] protected GameObject _connectionPrefab;
        [SerializeField] protected GameManager _gameManager;

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

        [Header("Route Nodes")]
        [SerializeField] protected GameObject _initialNodeGameObject;
        [SerializeField] protected GameObject _destinyNodeGameObject;
        [Space(10)]
        [Tooltip("Shows the route with the most distance eficient route to the destiny node.")]
        [SerializeField] protected Route _mostEficientRoute;
        [Tooltip("Shows the routes with the most distance eficient route to the destiny node.")]
        [SerializeField] protected List<Route> _mostEficientRoutes;
        [Space(10)]

        #endregion

        #region RuntimeVariables

        [SerializeField] protected List<Node> nodeList;
        [SerializeField] protected List<Connection> connectionList;
        [SerializeField] public List<Route> routesList;
        [SerializeField] public List<Route> efectiveRoutesList;

        protected bool newConnection;
        protected bool obstacleDetected;

        protected Node _initialNode;
        protected Node _destinyNode;

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
            for (int i = 0; i < _horizontalNodes; i++)
            {
                for (int j = 0; j < _verticalNodes; j++)
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
            foreach (Node node in transform.GetChild(0).GetComponentsInChildren<Node>())
            {
                DestroyImmediate(node.gameObject);
            }
            nodeList.Clear();

            foreach (Connection connection in transform.GetChild(1).GetComponentsInChildren<Connection>())
            {
                DestroyImmediate(connection.gameObject);
            }
            connectionList.Clear();
            ClearRoutes();
        }

        public void CreateGraphConnections()
        {
            foreach (Node node in nodeList)
            {
                if (node.State == NodeState.INACTIVE)
                    continue;

                foreach (Node neighbor in nodeList)
                {
                    if (neighbor == node || neighbor.State == NodeState.INACTIVE)
                        continue;

                    Vector3 offset = neighbor.transform.position - node.transform.position;
                    float dx = Mathf.Abs(offset.x);
                    float dz = Mathf.Abs(offset.z);
                    float distance = offset.magnitude;

                    if (distance > diagonalDistance + 0.1f)
                        continue;

                    bool isHorizontal = Mathf.Approximately(dx, horizontalDistance) && Mathf.Approximately(dz, 0f);
                    bool isVertical = Mathf.Approximately(dz, verticalDistance) && Mathf.Approximately(dx, 0f);
                    bool isDiagonal = Mathf.Approximately(dx, horizontalDistance) && Mathf.Approximately(dz, verticalDistance);

                    if (!(isHorizontal || isVertical || isDiagonal))
                        continue;

                    bool connectionExists = connectionList.Exists(c => (c.nodeA == node && c.nodeB == neighbor) ||
                        (c.nodeA == neighbor && c.nodeB == node));

                    if (connectionExists)
                        continue;

                    Vector3 direction = offset.normalized;
                    if (Physics.Raycast(node.transform.position, direction, out RaycastHit hit1, distance + 0.1f, _obstacleLayerMask)
                        || Physics.Raycast(neighbor.transform.position, -direction, out RaycastHit hit2, distance + 0.1f, _obstacleLayerMask))
                        continue;

                    GameObject tempConnection = Instantiate(_connectionPrefab, transform.GetChild(1));
                    Connection newConn = tempConnection.GetComponent<Connection>();
                    newConn.nodeA = node;
                    newConn.nodeB = neighbor;
                    connectionList.Add(newConn);
                    node.connections.Add(newConn);
                    neighbor.connections.Add(newConn);
                }
            }
        }

        public void ReduceRedundantConnections()
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                Node node = nodeList[i];
                if (node == null && node.State == NodeState.ACTIVE) continue;

                if (node.connections.Count == 2)
                {
                    Node a = GetOtherNode(node.connections[0], node);
                    Node b = GetOtherNode(node.connections[1], node);

                    Vector3 dirA = (a.transform.position - node.transform.position).normalized;
                    Vector3 dirB = (b.transform.position - node.transform.position).normalized;
                    float dot = Vector3.Dot(dirA, dirB);

                    //if (Mathf.Abs(dot - 1f) < 0.01f || Mathf.Abs(dot + 1f) < 0.01f)
                    if (dot < -0.8f)
                    {
                        ReplaceConnection(a, b, node.connections[0], node.connections[1], false);
                        DestroyImmediate(node.gameObject);
                        nodeList[i] = null;
                    }
                }

                else if (node.connections.Count == 8)
                {
                    List<Node> neighbors = new List<Node>();
                    foreach (Connection c in node.connections)
                    {
                        neighbors.Add(GetOtherNode(c, node));
                    }

                    for (int x = 0; x < neighbors.Count; x++)
                    {
                        for (int y = x + 1; y < neighbors.Count; y++)
                        {
                            Node a = neighbors[x];
                            Node b = neighbors[y];
                            if (neighbors[x].connections.Count == 8 && neighbors[y].connections.Count == 8)
                            {
                                

                                if (a.connections.Exists(c => GetOtherNode(c, a) == b)) continue;

                                GameObject newObj = Instantiate(_connectionPrefab, transform.GetChild(1));
                                Connection newConn = newObj.GetComponent<Connection>();
                                newConn.nodeA = a;
                                newConn.nodeB = b;
                                a.connections.Add(newConn);
                                b.connections.Add(newConn);
                                connectionList.Add(newConn);
                            }

                            Vector3 dirA = (a.transform.position - node.transform.position).normalized;
                            Vector3 dirB = (b.transform.position - node.transform.position).normalized;
                            float dot = Vector3.Dot(dirA, dirB);

                            //if (Mathf.Abs(dot - 1f) < 0.01f || Mathf.Abs(dot + 1f) < 0.01f)
                            if (dot < -0.8f)
                            {
                                ReplaceConnection(a, b, node.connections[x], node.connections[y], true);
                                nodeList[i] = null;
                            }
                        }
                    }

                    foreach (Connection c in node.connections)
                    {                        
                        Node other = GetOtherNode(c, node);
                        other.connections.Remove(c);
                        connectionList.Remove(c);
                        DestroyImmediate(c.gameObject);
                    }

                    DestroyImmediate(node.gameObject);
                    nodeList[i] = null;
                }
            }

            nodeList.RemoveAll(n => n == null);

            foreach (Node node in nodeList)
            {
                if (node.connections.Count == 2 && node.State == NodeState.ACTIVE)
                {
                    Node a = GetOtherNode(node.connections[0], node);
                    Node b = GetOtherNode(node.connections[1], node);

                    Vector3 dirA = (a.transform.position - node.transform.position).normalized;
                    Vector3 dirB = (b.transform.position - node.transform.position).normalized;
                    float dot = Vector3.Dot(dirA, dirB);

                    if (node != null && (node.connections.Count == 2) && (Mathf.Abs(dot - 1f) < 0.01f || Mathf.Abs(dot + 1f) < 0.01f))
                    {
                        ReduceRedundantConnections();
                        break;
                    }
                }
                else if (node != null && node.State == NodeState.ACTIVE && node.connections.Count == 8)
                {
                    ReduceRedundantConnections();
                    break;
                }
            }
        }


        public void GetRoutes()
        {
            ClearRoutes();

            float initialLastCloseDistance = Mathf.Infinity;
            float destinyLastCloseDistance = Mathf.Infinity;

            foreach (Node node in nodeList)
            {
                if(Vector3.Distance(_initialNodeGameObject.transform.position, node.gameObject.transform.position) < initialLastCloseDistance && node.State == NodeState.ACTIVE)
                {
                    initialLastCloseDistance = Vector3.Distance(_initialNodeGameObject.transform.position, node.gameObject.transform.position);
                    _initialNode = node;
                }
                if (Vector3.Distance(_destinyNodeGameObject.transform.position, node.gameObject.transform.position) < destinyLastCloseDistance && node.State == NodeState.ACTIVE)
                {
                    destinyLastCloseDistance = Vector3.Distance(_destinyNodeGameObject.transform.position, node.gameObject.transform.position);
                    _destinyNode = node;
                }
            }
            Route newRoute = new Route();
            newRoute.routeNodes = new List<Node>();
            _initialNode.ExploreRoutes(newRoute, _destinyNode, this, newRoute.distance);
            GetMostEficientRoutes();
            SetRouteToTheAgent();
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
            if (nodeHits >= 4)
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

        protected Node GetOtherNode(Connection c, Node center)
        {
            return c.nodeA == center ? c.nodeB : c.nodeA;
        }

        protected void ReplaceConnection(Node n1, Node n2, Connection oldA, Connection oldB, bool keepConnections)
        {
            GameObject newObj = Instantiate(_connectionPrefab, transform.GetChild(1));
            Connection newConn = newObj.GetComponent<Connection>();
            newConn.nodeA = n1;
            newConn.nodeB = n2;

            n1.connections.Remove(oldA);
            n1.connections.Remove(oldB);
            n1.connections.Add(newConn);

            n2.connections.Remove(oldA);
            n2.connections.Remove(oldB);
            n2.connections.Add(newConn);

            if (!keepConnections)
            {
                connectionList.Remove(oldA);
                connectionList.Remove(oldB);
                DestroyImmediate(oldA.gameObject);
                DestroyImmediate(oldB.gameObject);
            }

            connectionList.Add(newConn);
        }

        
        protected void ClearRoutes()
        {
            routesList.Clear();
            efectiveRoutesList.Clear();
            _mostEficientRoutes.Clear();
            _mostEficientRoute = new Route();
        }

        protected void GetMostEficientRoutes()
        {
            _mostEficientRoute.routeNodes = new List<Node>();
            _mostEficientRoute.distance = Mathf.Infinity;


            foreach (Route route in efectiveRoutesList)
            {
                if (route.distance < _mostEficientRoute.distance)
                {
                    _mostEficientRoute.routeNodes = route.routeNodes;
                    _mostEficientRoute.distance = route.distance;
                }
            }
            
            _mostEficientRoutes.Add(_mostEficientRoute);
            foreach (Route route in efectiveRoutesList)
            {
                if ((_mostEficientRoute.distance == route.distance) && (_mostEficientRoute.routeNodes == route.routeNodes)) continue;
                else if(_mostEficientRoute.routeNodes != route.routeNodes && _mostEficientRoute.distance == route.distance)
                {
                    _mostEficientRoutes.Add(route);
                }
            }
            foreach(Node node in _mostEficientRoute.routeNodes)
            {
                node.ChangeColor(node.ActiveMaterial);
            }
        }

        protected void SetRouteToTheAgent()
        {
            _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints = new Waypoints[_mostEficientRoute.routeNodes.Count + 1];

            for (int i = 0; i < _mostEficientRoute.routeNodes.Count; i++)
            {
                _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[i].waypointState = WaypointState.Moving;
                _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[i].waypoint = _mostEficientRoute.routeNodes[i].transform.position;
                _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[i].speedMPS = 5f;
            }
            _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[_mostEficientRoute.routeNodes.Count].waypointState = WaypointState.PauseWaypoint;
            _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[_mostEficientRoute.routeNodes.Count].waypoint = _mostEficientRoute.routeNodes[0].transform.position;
            _gameManager.enemyNPCs[0].enemyNPC_Behaviours.moveWaypoints_SO.waypoints[_mostEficientRoute.routeNodes.Count].speedMPS = 0f;
        }

        #endregion

        #region GettersSetters

        public Node InitialNode { get { return _initialNode; } }

        public Node DestinyNode { get { return _destinyNode; } }

        #endregion

    }
}
