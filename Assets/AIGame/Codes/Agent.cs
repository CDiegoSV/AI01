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

        protected virtual void InitializeAgent()
        {

        }

        protected void RigidbodyMovement()
        {
            _rigidbody.linearVelocity = _movementDirection * _currentMovementSpeed;

            if (_movementDirection != Vector3.zero)
            {
                RigidbodyRotation();
            }
        }

        /// <summary>
        /// Smoothly rotates the rigidbody with the movementDirection vector.
        /// </summary>
        protected void RigidbodyRotation()
        {
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(_movementDirection), 5 * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Rotates the rigidbody between two quaternions by t.
        /// </summary>
        /// <param name="startRotation"></param>
        /// <param name="endRotation"></param>
        /// <param name="t"></param>
        protected void RigidbodyRotation(Quaternion startRotation, Quaternion endRotation ,float t)
        {
            _rigidbody.rotation = Quaternion.Lerp(startRotation, endRotation, t);
        }

        #endregion
    }
}