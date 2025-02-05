using UnityEngine;
using UnityEngine.InputSystem;

namespace Dante.Agents
{

    public class PlayersAvatar : Agent
    {
        #region References


        #endregion

        #region Local Variables

        #endregion

        #region UnityMethods

        private void FixedUpdate()
        {
            RigidbodyMovement();

            if(_movementDirection != Vector3.zero)
            {
                RigidbodyRotation();
            }
        }

        #endregion

        #region Local Methods

        #endregion

        #region Public Methods

        public void OnMOVE(InputAction.CallbackContext value)
        {
            if(value.performed)
            {
                _fsm.StateMechanic(StateMechanics.MOVE);
                _movementDirection = new Vector3(value.ReadValue<Vector2>().x, 0, value.ReadValue<Vector2>().y).normalized;
                _currentMovementSpeed = _movementSpeed;
            }
            else if(value.canceled)
            {
                _fsm.StateMechanic(StateMechanics.STOP);
                _movementDirection = Vector3.zero;
                _currentMovementSpeed = 0f;
            }
        }

        #endregion
    }
}
