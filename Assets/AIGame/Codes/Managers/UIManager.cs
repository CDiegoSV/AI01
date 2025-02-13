using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dante.Agents
{
    public class UIManager : MonoBehaviour
    {

        #region References

        [SerializeField] protected GameManager gameManager;

        #endregion

        #region PublicVariables

        public static UIManager Instance;

        #endregion

        #region LocalVariables

        [SerializeField] protected GameObject victoryPanel;

        #endregion

        #region UnityMethods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        #region PublicMethods

        public void VictoryPanel()
        {
            victoryPanel.SetActive(true);
        }

        public void PlayAgainButton()
        {
            SceneManager.LoadScene(1);
        }

        public void MenuButton()
        {
            SceneManager.LoadScene(0);
        }

        public void QuitButton()
        {
            Application.Quit();
        }

        #endregion

        #region LocalMethods

        #endregion
    }
}
