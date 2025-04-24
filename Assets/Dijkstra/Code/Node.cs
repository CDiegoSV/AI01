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
        [SerializeField] protected Material _routedNodeMaterial;

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
            if (route.routeNodes.Contains(this))
                return;

            GetComponent<Renderer>().material = _routedNodeMaterial;

            Route newRoute = new Route();
            newRoute.routeNodes = new List<Node>(route.routeNodes);
            newRoute.routeNodes.Add(this);
            newRoute.distance = route.distance + distance;

            dijkstra.routesList.Add(newRoute);

            if (this == destinyNode)
            {
                dijkstra.efectiveRoutesList.Add(newRoute);
                return;
            }

            foreach (Connection connection in this.connections)
            {
                Node nextNode = connection.OpposingNode(this);
                nextNode.ExploreRoutes(newRoute, destinyNode, dijkstra, connection.Distance);
            }
        }

        public void ChangeColor(Material material)
        {
            GetComponent<Renderer>().material = material;
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
        public Material ActiveMaterial { get { return _activeNodeMaterial; } }

        #endregion
    }
}
