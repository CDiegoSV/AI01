using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


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

        #region References

        [Header("Game Agents References")]

        public List<EnemyNPC> enemyNPCs;

        public PlayersAvatar playersAvatar;

        #endregion

        #region LocalVariables

        [SerializeField] protected GameStates _currentState;

        protected bool _gameStarted;
        protected bool _gameIsPaused;

        #endregion

        #region UnityMethods

        private void Start()
        {
            if(playersAvatar == null)
            {
                playersAvatar = GameObject.FindFirstObjectByType<PlayersAvatar>();
            }
            if(playersAvatar.GameManager == null)
            {
                playersAvatar.GameManager = this;
            }

            StateMechanic(GameStates.GAME);
        }

        #endregion

        #region PublicMethods

        public void StateMechanic(GameStates nextState)
        {
            switch(_currentState)
            {
                case GameStates.NONE:
                    ProceedToNextState(nextState);
                    break;

                case GameStates.GAME:
                    if (nextState != GameStates.GAME)
                    {
                        ProceedToNextState(nextState);
                    }
                    break;

                case GameStates.VICTORY:
                    if (nextState == GameStates.GAME)
                    {
                        ProceedToNextState(nextState);
                    }
                    break;
                case GameStates.DEFEAT:
                    if(nextState == GameStates.GAME)
                    {
                        ProceedToNextState(nextState);
                    }
                    break;
                case GameStates.PAUSE:
                    if (_currentState == GameStates.GAME || _currentState == GameStates.PAUSE)
                    {
                        ProceedToNextState(nextState);
                    }
                    break;
            }
        }

        public void OnPAUSE(InputAction.CallbackContext value)
        {
            if (value.performed && _gameStarted)
            {
                if (!_gameIsPaused)
                {
                    StateMechanic(GameStates.PAUSE);
                }
                else
                {
                    StateMechanic(GameStates.GAME);
                }
            }
        }
        #endregion

        #region LocalMethods

        protected void PlayerReturnToSpawn()
        {
            if (playersAvatar.gameObject.transform.position != playersAvatar.InitialPosition)
            {
                playersAvatar.ReturnToInitialPosition();
            }
        }

        protected void ProceedToNextState(GameStates nextState)
        {
            FinalizeGameStateMachine();
            _currentState = nextState;
            InitializeGameStateMachine();
        }

        protected void InitializeGameStateMachine()
        {
            switch (_currentState)
            {
                case GameStates.GAME:
                    InitializeGameState();
                    break;

                case GameStates.VICTORY:
                    InitializeVictoryState();
                    break; 

                case GameStates.DEFEAT:
                    InitializeDefeatState();
                    break;
            }
        }

        protected void FinalizeGameStateMachine()
        {
            switch (_currentState)
            {
                case GameStates.GAME:
                    FinalizeGameState();
                    break;

                case GameStates.VICTORY:
                    FinalizeVictoryState();
                    break;

                case GameStates.DEFEAT:
                    FinalizeDefeatState();
                    break;
            }
        }
        #endregion

        #region GameStateMachine

        #region GameState

        protected void InitializeGameState()
        {
            if (!_gameStarted)
            {
                PlayerReturnToSpawn();
                _gameStarted = true;
            }
        }

        protected void FinalizeGameState()
        {

        }

        #endregion

        #region VictoryState

        protected void InitializeVictoryState()
        {
            _gameStarted = false;

        }

        protected void FinalizeVictoryState()
        {

        }

        #endregion

        #region DefeatState

        protected void InitializeDefeatState()
        {
            PlayerReturnToSpawn();
            StateMechanic(GameStates.GAME);
        }

        protected void FinalizeDefeatState()
        {

        }

        #endregion

        #region PauseState

        protected void InitializePauseState()
        {
            _gameIsPaused = true;
        }

        protected void FinalizePauseState()
        {
            _gameIsPaused = false;
        }

        #endregion

        #endregion

        #region GettersSetters

        public bool IsPaused {  get { return _gameIsPaused; } }

        #endregion

    }
}