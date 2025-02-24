using System;
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

        [SerializeField] protected Animator _animator;

        #endregion

        #region LocalVariables

        protected States _currentState;

        #endregion

        #region Public Methods

        public void EnteredState(States value)
        {
            //Debug.Log("FSM - EnteredState(): Entered the finite state " +  value.ToString());
            _currentState = value;
            //Invoke("CleanAnimatorValues", 0.1f);
            CleanAnimatorValues();
        }

        public void StateMechanic(StateMechanics value)
        {
            _animator.SetBool(value.ToString(), true);
        }

        #endregion

        #region Local Methods

        protected void CleanAnimatorValues()
        {
            foreach (StateMechanics state in (StateMechanics[])Enum.GetValues(typeof(StateMechanics)))
            {
                _animator.SetBool(state.ToString(), false);
            }
        }

        #endregion
    }

}