using System.Collections.Generic;
using UnityEngine;

namespace Dante.Dijkstra {

    #region enums

	public enum NodeState
	{
		NONE,
		ACTIVE,
		INACTIVE
	}

    #endregion


    public class Node : MonoBehaviour
	{
        #region References

        [SerializeField] protected Material _activeNodeMaterial;
        [SerializeField] protected Material _inactiveNodeMaterial;

        [SerializeField] protected NodeState state;

        #endregion


        #region RuntimeVariables

        [SerializeField] public List<Connection> connections;
        protected Route newRoute;


        #endregion

        #region UnityMethods



        #endregion

        #region PublicMethods

        public void StateMechanic(NodeState nodeState)
        {
            switch (nodeState)
            {
                case NodeState.ACTIVE:
                    if (state != NodeState.ACTIVE)
                    {
                        state = NodeState.ACTIVE;
                        GetComponent<Renderer>().material = _activeNodeMaterial;
                        SetIconsToNodeGameObject(state);
                    }
                    break;

                case NodeState.INACTIVE:
                    if (state != NodeState.INACTIVE)
                    {
                        state = NodeState.INACTIVE;
                        GetComponent<Renderer>().material = _inactiveNodeMaterial;
                        SetIconsToNodeGameObject(state);
                    }
                    break;
            }
        }

        public void ExploreRoutes(Route route, Node destinyNode, Dijkstra dijkstra, float distance)
        {
            if(this == destinyNode)
            {
                newRoute = new Route();
                newRoute.routeNodes = new List<Node>(route.routeNodes);
                newRoute.routeNodes.Add(destinyNode);
                newRoute.distance = route.distance;
                newRoute.distance += distance;
                dijkstra.routesList.Add(newRoute);
                dijkstra.efectiveRoutesList.Add(newRoute);
                Debug.Log("Return destinyNode reached in current route.");
                return;
            }

            if(route.routeNodes.Count > 0)
            {
                foreach (Node node in route.routeNodes)
                {
                    if (this != node)
                    {
                        continue;
                    }
                    else
                    {
                        Debug.Log("Return Node Found in current route.");
                        return;
                    }
                }
            }

            newRoute = new Route();
            newRoute.routeNodes = new List<Node>(route.routeNodes);
            newRoute.routeNodes.Add(this);
            newRoute.distance = route.distance;
            newRoute.distance += distance;
            dijkstra.routesList.Add(newRoute);
            foreach (Connection connection in connections)
            {
                connection.OpposingNode(this)?.ExploreRoutes(newRoute, destinyNode, dijkstra, connection.Distance);
            }
        }

        #endregion

        #region LocalMethods

        protected void SetIconsToNodeGameObject(NodeState nodeState)
        {
            transform.GetChild(0).gameObject.name = /*"Node:" + */new Vector2(transform.position.x, transform.position.z).ToString();
            transform.GetChild(1).gameObject.name = state.ToString();

            switch (nodeState)
            {
                case NodeState.ACTIVE:
                    IconManager.SetIcon(transform.GetChild(1).gameObject, IconManager.LabelIcon.Green);
                    IconManager.SetIcon(transform.GetChild(0).gameObject, IconManager.LabelIcon.Teal);
                    break;

                case NodeState.INACTIVE:
                    IconManager.SetIcon(transform.GetChild(1).gameObject, IconManager.LabelIcon.Gray);
                    IconManager.SetIcon(transform.GetChild(0).gameObject, IconManager.LabelIcon.Gray);
                    break;
            }
            

        }

        #endregion

        #region GettersSetters

        public NodeState State
        {
            get { return state; }
        }

        #endregion
    }
}
