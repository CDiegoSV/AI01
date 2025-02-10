using UnityEngine;


namespace Dante.Agents
{
    #region enums

    public enum GameStates
    {
        NONE, GAME, VICTORY, DEFEAT, PAUSE
    }

    #endregion

    public class GameManager : MonoBehaviour
    {

        #region LocalVariables

        protected GameStates _currentState;

        #endregion

        #region PublicMethods

        public void StateMechanic(GameStates nextState)
        {

        }

        #endregion

        #region LocalMethods

        protected void InitializeGameStateMachine()
        {
            switch (_currentState)
            {
                case GameStates.GAME:
                    break;

                case GameStates.VICTORY:
                    break; 

                case GameStates.DEFEAT:
                    break;
            }
        }

        protected void FinalizeGameStateMachine()
        {
            switch (_currentState)
            {
                case GameStates.GAME:
                    break;

                case GameStates.VICTORY:
                    break;

                case GameStates.DEFEAT:
                    break;
            }
        }
        #endregion

        #region GameStateMachine

        #region GameState

        protected void InitializeGameState()
        {

        }

        protected void ExecutingGameState()
        {

        }

        protected void FinalizeGameState()
        {

        }

        #endregion

        #region VictoryState

        protected void InitializeVictoryState()
        {

        }

        protected void ExecutingVictoryState()
        {

        }

        protected void FinalizeVictoryState()
        {

        }

        #endregion

        #region DefeatState

        protected void InitializeDefeatState()
        {

        }

        protected void ExecutingDefeatState()
        {

        }

        protected void FinalizeDefeatState()
        {

        }

        #endregion

        #region PauseState

        protected void InitializePauseState()
        {

        }

        protected void ExecutingPauseState()
        {

        }

        protected void FinalizePauseState()
        {

        }

        #endregion

        #endregion

    }
}