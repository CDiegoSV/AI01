using UnityEngine;

namespace Dante.Agents
{
    #region Enums

    public enum States
    {
        IDLE, MOVING, TURNING
    }

    public enum StateMechanics
    {
        STOP, MOVE, TURN
    }

    #endregion

    public class FiniteStateMachine : MonoBehaviour
    {
        #region References

        [SerializeField] Animator animator;

        #endregion

        #region LocalVariables

        protected States _currentState;

        #endregion

        public void EnteredState(States value)
        {
            Debug.Log("FSM - EnteredState(): Entered the finite state " +  value.ToString());
            _currentState = value;
        }

        public void StateMechanic(StateMechanics value)
        {
            animator.SetBool(value.ToString(), true);
        }
    }

}