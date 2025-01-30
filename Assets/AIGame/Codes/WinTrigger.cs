using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject winPanel;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            winPanel.SetActive(true);
        }
    }
}
