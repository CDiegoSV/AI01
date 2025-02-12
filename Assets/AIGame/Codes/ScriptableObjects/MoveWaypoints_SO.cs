using System;
using UnityEngine;

namespace Dante.Agents
{
    #region Enums

    [Serializable]
    public enum WaypointState
    {
        Moving, PauseWaypoint
    }

    #endregion

    #region Structs

    [Serializable]
    public struct Waypoints
    {
        public WaypointState waypointState;
        public Vector3 waypoint;
        public float speedMPS;
    }

    #endregion

    [CreateAssetMenu(fileName = "MoveWaypoints_SO", menuName = "Scriptable Objects/MoveWaypoints_SO")]
    public class MoveWaypoints_SO : ScriptableObject
    {
        [SerializeField] public Waypoints[] waypoints;
    }
}
