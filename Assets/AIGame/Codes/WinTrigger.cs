using Dante.Agents;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] protected GameManager gameManager;

    [SerializeField] protected GameObject winPanel;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.StateMechanic(GameStates.VICTORY);
        }
    }
}
