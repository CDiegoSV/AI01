using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Dante.Agents
{

    public class EnemyNPCFactory : MonoBehaviour
    {
        #region References
        [Header("Parameters")]
        [SerializeField] protected GameObject enemyPrefab;
        [SerializeField] protected EnemyNPC_SO[] enemiesScriptableObject;

        [Header("Runtime Variables")]
        [SerializeField] protected List<GameObject> enemyInstancesGameObjects;

        #endregion

        #region RuntimeVariables

        GameObject enemyInstanceGameObject;

        #endregion

        #region PublicMethods

        public void CreateEnemies()
        {
            DeleteEnemies();
            foreach (EnemyNPC_SO enemy in enemiesScriptableObject)
            {
                enemyInstanceGameObject = Instantiate(enemyPrefab);
                enemyInstanceGameObject.transform.position = enemy.spawnParameters.position;
                enemyInstanceGameObject.transform.rotation = Quaternion.Euler(enemy.spawnParameters.rotation);
                enemyInstanceGameObject.transform.parent = this.gameObject.transform;

                enemyInstancesGameObjects.Add(enemyInstanceGameObject);
            }
        }

        public void DeleteEnemies()
        {
            for(int i = enemyInstancesGameObjects.Count -1; i >= 0; i--)
            {
                enemyInstanceGameObject = enemyInstancesGameObjects[i];
                enemyInstancesGameObjects.Remove(enemyInstanceGameObject);
                DestroyImmediate(enemyInstanceGameObject);
            }
            enemyInstancesGameObjects.Clear();
        }

        #endregion
    }
}
