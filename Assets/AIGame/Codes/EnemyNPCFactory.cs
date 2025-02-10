using UnityEngine;
using System.Collections.Generic;

namespace Dante.Agents
{

    public class EnemyNPCFactory : MonoBehaviour
    {
        #region References
        [Header("Parameters")]
        [SerializeField] protected GameObject enemyPrefab;
        [SerializeField] protected GameObject visionConePrefab;
        [SerializeField] protected EnemyNPC_SO[] enemiesScriptableObject;

        [Header("Runtime Variables")]
        [SerializeField] protected List<GameObject> enemyInstancesGameObjects;
        
        #endregion

        #region RuntimeVariables

        protected GameObject _enemyInstanceGameObject;

        protected GameObject _currentVisionCone;
        [SerializeField] protected List<GameObject> _currentEnemyVisionCones;

        #endregion

        #region PublicMethods

        public void CreateEnemies()
        {
            DeleteEnemies();
            foreach (EnemyNPC_SO enemy in enemiesScriptableObject)
            {
                _enemyInstanceGameObject = Instantiate(enemyPrefab, this.gameObject.transform);
                _enemyInstanceGameObject.transform.position = enemy.spawnParameters.position;
                _enemyInstanceGameObject.transform.localRotation = Quaternion.Euler(enemy.spawnParameters.rotation);
                _enemyInstanceGameObject.GetComponent<EnemyNPC>().enemyNPC_Behaviours = enemy;
                

                enemyInstancesGameObjects.Add(_enemyInstanceGameObject);

                CreateVisionCones(enemy);
            }
        }

        public void DeleteEnemies()
        {
            if (enemyInstancesGameObjects.Count > 0)
            {
                for (int i = enemyInstancesGameObjects.Count - 1; i >= 0; i--)
                {
                    _enemyInstanceGameObject = enemyInstancesGameObjects[i];
                    enemyInstancesGameObjects.Remove(_enemyInstanceGameObject);
                    DestroyImmediate(_enemyInstanceGameObject);
                }
                enemyInstancesGameObjects.Clear();
            }
        }

        public void CreateVisionCones(EnemyNPC_SO enemyNPC_SO)
        {
            if(enemyNPC_SO.visionConeParameters.fieldOfView % 10 == 0)
            {
                for (int i = 0; i < (enemyNPC_SO.visionConeParameters.fieldOfView * 0.1f) -1; i++)
                {
                    _currentVisionCone = Instantiate(visionConePrefab, _enemyInstanceGameObject.transform.GetChild(2));
                    _currentVisionCone.GetComponent<Enemy_NPC_VisionCones>().enemyNPC = _enemyInstanceGameObject.GetComponent<EnemyNPC>();
                    _currentEnemyVisionCones.Add(_currentVisionCone);
                }
                int signIterator = 1;
                int degrees = 0;

                for (int j = 0; j < _currentEnemyVisionCones.Count; j++)
                {

                    _currentEnemyVisionCones[j].transform.localScale = new Vector3(enemyNPC_SO.visionConeParameters.distance, 1, enemyNPC_SO.visionConeParameters.distance);

                    _currentEnemyVisionCones[j].transform.localRotation = Quaternion.Euler(0, degrees * signIterator, 0);

                    if (j > 0)
                    {
                        signIterator *= -1;
                    }
                    if (signIterator > 0)
                    {
                        degrees += 10;
                    }
                }
                if (_currentEnemyVisionCones.Count % 2 == 0)
                {
                    _enemyInstanceGameObject.transform.GetChild(2).localRotation = Quaternion.Euler(new Vector3(0, -5, 0));
                }
                _currentEnemyVisionCones.Clear();
                _currentEnemyVisionCones = new List<GameObject>();
            }
            else
            {
                Debug.LogWarning("CreateVisionCones() - Field of View of " + enemyNPC_SO.name + " is not a multiple of 10.");
            }
        }
        #endregion
    }
}
