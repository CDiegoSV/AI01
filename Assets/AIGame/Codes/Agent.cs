using UnityEngine;


namespace Dante.Agents
{

    public class Agent : MonoBehaviour
    {
        #region Knobs

        [SerializeField] protected float _movementSpeed;
        
        #endregion

        #region References

        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected FiniteStateMachine _fsm;

        #endregion

        #region Local Variables

        protected Vector3 _movementDirection;
        protected float _currentMovementSpeed;

        #endregion

        #region UnityMethods

        //void Start()
        //{
        
        //}

        //void Update()
        //{
        
        //}

        #endregion

        #region Local Methods

        protected void RigidbodyMovement()
        {
            _rigidbody.linearVelocity = _movementDirection * _currentMovementSpeed;
        }

        protected void RigidbodyRotation()
        {
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(_movementDirection), 5 * Time.fixedDeltaTime);
        }

        #endregion
    }
}