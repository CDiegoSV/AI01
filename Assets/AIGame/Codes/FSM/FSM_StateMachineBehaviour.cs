using UnityEngine;


namespace Dante.Agents
{

    public class FSM_StateMachineBehaviour : StateMachineBehaviour
    {
        public States state;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Entered State: " + state.ToString());
            //Debug.Log("State = " + state.ToString());
            animator.gameObject.GetComponent<FiniteStateMachine>().EnteredState(state);
        }

    }
}
