using UnityEngine;
using UnityEditor;

namespace Dante.Agents
{

    [CustomEditor(typeof(EnemyNPCFactory))]
    public class EnemyNPCFactory_Editor : Editor
    {
        EnemyNPCFactory enemyNPCFactory;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(enemyNPCFactory == null )
            {
                enemyNPCFactory = (EnemyNPCFactory)target;
            }

            if(GUILayout.Button("Create Enemies"))
            {
                enemyNPCFactory.DeleteEnemies();
                enemyNPCFactory.CreateEnemies();
            }
            if (GUILayout.Button("Delete Enemies"))
            {
                enemyNPCFactory.DeleteEnemies();
            }
        }
    }
}