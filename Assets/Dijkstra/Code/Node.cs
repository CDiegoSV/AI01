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

        #endregion

        #region LocalMethods

        protected void SetIconsToNodeGameObject(NodeState nodeState)
        {
            transform.GetChild(0).gameObject.name = new Vector2(transform.position.x, transform.position.z).ToString();
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



        #endregion
    }
}
