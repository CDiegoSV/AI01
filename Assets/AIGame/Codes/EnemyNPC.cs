using System.Collections;
using UnityEngine;

namespace Dante.Agents
{

    public class EnemyNPC : Agent
    {
        #region Knobs

        public EnemyNPC_SO enemyNPC_Behaviours;

        #endregion

        #region References

        protected GameManager _gameManager;

        #endregion

        #region Local Variables

        protected PatrolBehaviours _currentPatrolBehaviour;

        protected Quaternion _startRotation;
        protected Quaternion _endRotation;

        protected bool isRotating;
        protected float elapsedTime;

        protected int _currentEnemyBehaviourIndex;
        protected Coroutine _enemyBehaviourCoroutine;

        protected LayerMask _obstacleLayerMask;
        protected LayerMask _playerLayerMask;

        #endregion

        #region UnityMethods

        private void FixedUpdate()
        {
            switch (_currentPatrolBehaviour.stateMechanic)
            {
                case StateMechanics.MOVE:
                    ExecutingMoveSubStateMachine();
                    break;

                case StateMechanics.TURN:
                    ExecutingTurnSubStateMachine();
                    break;
            }
        }

        private void OnEnable()
        {
            InitializeAgent();
        }

        #endregion

        #region Local Methods

        #region Rotation
        protected void RotateToDestiny()
        {
            if (isRotating && _currentPatrolBehaviour.durationTime > 0.1f)
            {
                elapsedTime += Time.fixedDeltaTime;

                float t = elapsedTime / _currentPatrolBehaviour.durationTime;

                if(t >= 1 )
                {
                    _rigidbody.rotation = Quaternion.Euler(_currentPatrolBehaviour.destinyDirection);
                    isRotating = false;
                }
                else
                {
                    RigidbodyRotation(_startRotation, _endRotation, t);
                }
            }
            else if(isRotating)
            {
                _rigidbody.rotation = Quaternion.Euler(_currentPatrolBehaviour.destinyDirection);
                isRotating = false;
            }
        }

        public void InitializeRotation()
        {
            _startRotation = _rigidbody.rotation;
            _endRotation = Quaternion.Euler(_currentPatrolBehaviour.destinyDirection);

            elapsedTime = 0;
            isRotating = true;
        }

        #endregion

        protected override void InitializeAgent()
        {
            _obstacleLayerMask = LayerMask.GetMask("Obstacle");
            _playerLayerMask = LayerMask.GetMask("Avatar");

            InitializePatrolBehaviour();
        }

        protected void InitializePatrolBehaviour()
        {
            StopAllCoroutines();

            _currentEnemyBehaviourIndex = 0;

            if (enemyNPC_Behaviours.patrolBehaviours.Length > 0)
            {
                _currentPatrolBehaviour = enemyNPC_Behaviours.patrolBehaviours[0];
            }
            else
            {
                _currentPatrolBehaviour.stateMechanic = StateMechanics.STOP;
                _currentPatrolBehaviour.durationTime = -1;
            }

            InitializeSubStateMachine();

            if (_currentPatrolBehaviour.durationTime > 0 && gameObject.activeSelf)
            {
                StartCoroutine(TimerForEnemyBehaviour());
            }
        }

        protected virtual void GoToNextEnemyBehaviour()
        {
            _currentEnemyBehaviourIndex++;
            if(_currentEnemyBehaviourIndex >= enemyNPC_Behaviours.patrolBehaviours.Length)
            {
                _currentEnemyBehaviourIndex = 0;
            }
            _currentPatrolBehaviour = enemyNPC_Behaviours.patrolBehaviours[_currentEnemyBehaviourIndex];

            InitializeSubStateMachine();
            
            if(_currentPatrolBehaviour.durationTime > 0)
            {
                Invoke("InvokeCoroutine", 0.1f);
            }
        }

        protected void InvokeCoroutine()
        {
            _enemyBehaviourCoroutine = StartCoroutine(TimerForEnemyBehaviour());
        }

        protected void InitializeSubStateMachine()
        {
            switch (_currentPatrolBehaviour.stateMechanic)
            {
                case StateMechanics.MOVE:
                    InitializeMoveSubStateMachine();
                    break;

                case StateMechanics.TURN:
                    InitializeTurnSubStateMachine();
                    break;

                case StateMechanics.STOP:
                    InitializeStopSubStateMachine();
                    break;
            }
        }

        protected void FinalizeSubStateMachine()
        {
            switch (_currentPatrolBehaviour.stateMechanic)
            {
                case StateMechanics.MOVE:
                    FinalizeMoveSubStateMachine();
                    break;

                case StateMechanics.TURN:
                    FinalizeTurnSubStateMachine();
                    break;

                case StateMechanics.STOP:
                    FinalizeStopSubStateMachine();
                    break;
            }
        }

        

        #region Coroutines

        protected IEnumerator TimerForEnemyBehaviour()
        {
            yield return new WaitForSeconds(_currentPatrolBehaviour.durationTime);

            FinalizeSubStateMachine();

            GoToNextEnemyBehaviour();

            if(_enemyBehaviourCoroutine != null)
            {
                StopCoroutine(_enemyBehaviourCoroutine);
            }
        }

        #endregion

        #endregion

        #region PublicMethods

        public void VerifyCollisionOfPlayerAvatar(Transform playerAvatarTransform)
        {
            if (Physics.Raycast(transform.position, (playerAvatarTransform.position -  transform.position).normalized, out RaycastHit hit,
                Vector3.Distance(transform.position, playerAvatarTransform.position),  _obstacleLayerMask))
            {
                Debug.DrawRay(transform.position, (playerAvatarTransform.position - transform.position).normalized * hit.distance, Color.yellow);
                Debug.Log("Raycast hit an obstacle.");
            }
            else if(Physics.Raycast(transform.position, (playerAvatarTransform.position - transform.position).normalized,
                Vector3.Distance(transform.position, playerAvatarTransform.position), _playerLayerMask))
            {
                Debug.DrawRay(transform.position, (playerAvatarTransform.position - transform.position).normalized * hit.distance, Color.red);
                Debug.Log("Raycast hit the player.");
                _gameManager.StateMechanic(GameStates.DEFEAT);
            }

        }

        #endregion

        #region SubStateMachineStates

        #region MOVE_State

        protected void InitializeMoveSubStateMachine()
        {
            _fsm.StateMechanic(StateMechanics.MOVE);
        }

        protected void ExecutingMoveSubStateMachine()
        {

        }

        protected void FinalizeMoveSubStateMachine()
        {

        }

        #endregion

        #region TURN_State

        protected void InitializeTurnSubStateMachine()
        {
            _fsm.StateMechanic(StateMechanics.TURN);
            InitializeRotation();
        }

        protected void ExecutingTurnSubStateMachine()
        {
            RotateToDestiny();
        }

        protected void FinalizeTurnSubStateMachine()
        {

        }

        #endregion

        #region STOP_State

        protected void InitializeStopSubStateMachine()
        {
            _fsm.StateMechanic(StateMechanics.STOP);
        }

        protected void ExecutingStopSubStateMachine()
        {

        }

        protected void FinalizeStopSubStateMachine()
        {

        }

        #endregion

        #endregion SubstateMachineStates

        #region GettersSetters

        public GameManager GameManager
        {
            set { _gameManager = value; }
        }

        #endregion
    }
}
