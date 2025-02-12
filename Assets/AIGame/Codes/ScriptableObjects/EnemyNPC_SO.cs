using System;
using UnityEngine;

namespace Dante.Agents
{

    #region Structs

    [Serializable]
    public struct PatrolBehaviours
    {
        public StateMechanics stateMechanic;
        [SerializeField] public Vector3 destinyDirection;
        public float durationTime;
    }

    [Serializable]
    public struct VisionConeParameters
    {
        public float distance;
        public float fieldOfView;
    }

    [Serializable]
    public struct SpawnParameters
    {
        public Vector3 position;
        public Vector3 rotation;
    }

    #endregion

    [CreateAssetMenu(fileName = "EnemyNPC_SO", menuName = "Scriptable Objects/EnemyNPC_SO")]
    public class EnemyNPC_SO : ScriptableObject
    {
        [SerializeField] public PatrolBehaviours[] patrolBehaviours;

        [SerializeField] public MoveWaypoints_SO moveWaypoints_SO;

        [SerializeField] public VisionConeParameters visionConeParameters;

        [SerializeField] public SpawnParameters spawnParameters;
    }
}
