using UnityEngine;
using UnityEngine.InputSystem;

namespace Dante.Agents
{
    public class ControllerInputHandler : MonoBehaviour
    {

        #region References



        #endregion

        #region Local Variables

        protected PlayerInput _playerInput;
        [SerializeField] protected PlayersAvatar _playersAvatar;

        #endregion

        #region Unity Methods
        void Start()
        {
            InitializeControllerInputHandler();
        }

        #endregion

        #region Local Methods

        protected void InitializeControllerInputHandler()
        {
            _playerInput = GetComponent<PlayerInput>();

        }

        #endregion

        #region CallbackContext Methods
        public void OnMove(InputAction.CallbackContext value)
        {
            _playersAvatar?.OnMOVE(value);
        }


        #endregion
    }
}

