using UnityEditor;
using UnityEngine;

namespace Dante.Dijkstra {
	[CustomEditor (typeof(Dijkstra))]
	public class Dijkstra_Editor : Editor
	{
        #region References
        

        [SerializeField] protected Dijkstra _dijkstra;

        #endregion

        #region RuntimeVariables


        #endregion

        #region UnityMethods

        public override void OnInspectorGUI()
        {
            if( _dijkstra == null)
            {
                _dijkstra = (Dijkstra)target;
            }

            base.OnInspectorGUI();


            if (GUILayout.Button("Probe Nodes"))
            {
                ProbeNodesButton();
            }
            if (GUILayout.Button("Clear All"))
            {
                _dijkstra.ClearAllNodesInTheList();
            }
        }

        #endregion

        #region PublicMethods



        #endregion

        #region LocalMethods

        protected void ProbeNodesButton()
        {
            _dijkstra.InstanceNodes();
        }

        #endregion

        #region GettersSetters



        #endregion
    }
}
