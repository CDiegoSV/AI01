using Dante.Agents;
using UnityEngine;

public class Enemy_NPC_VisionCones : MonoBehaviour
{
    #region References

    public EnemyNPC enemyNPC;

    #endregion

    #region UnityMethods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger detected the player.");
            enemyNPC.VerifyCollisionOfPlayerAvatar(other.transform);
        }
    }

    #endregion

}
