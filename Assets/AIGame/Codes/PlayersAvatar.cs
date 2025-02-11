using UnityEngine;
using UnityEngine.InputSystem;

namespace Dante.Agents
{

    public class PlayersAvatar : Agent
    {
        #region References

        [SerializeField] protected GameManager _gameManager;

        #endregion

        #region Local Variables

        protected Vector3 initialPosition;

        #endregion

        #region UnityMethods

        private void Awake()
        {
            initialPosition = transform.position;
        }

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

        public void ReturnToInitialPosition()
        {
            transform.position = initialPosition;
        }

        public void OnMOVE(InputAction.CallbackContext value)
        {
            if(value.performed && !_gameManager.IsPaused)
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

        #region GettersSetters

        public Vector3 InitialPosition
        {
            get { return initialPosition; }
        }

        public GameManager GameManager { set { _gameManager = value; } get { return _gameManager; } }

        #endregion
    }


}
