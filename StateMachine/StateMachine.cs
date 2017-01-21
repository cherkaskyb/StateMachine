using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine
{
    public class StateMachine : IStateMachine
    {
        public IState CurrentState { get; private set; }
        public EventHandler<IState> StateChanged { get; set; }

        public StateMachine(IState firstState)
        {
            IsFirstTimeInState = true;
            CurrentState = firstState;
        }

        public StateMachine(IState firstState, IStateMachineLogger logger): this(firstState)
        {
            Logger = logger;
            CurrentState = firstState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstState"></param>
        /// <param name="autoStateTransition">Map StateResults to IState. 
        /// if current StateResult deos not require auto transition, return null</param>
        /// <param name="logger">Null for no logging</param>
        public StateMachine(IState firstState, Func<StateResult, IState> autoStateTransition, 
            IStateMachineLogger logger)
            : this(firstState, logger)
        {
            AutoStateTransition = autoStateTransition;
        }

        /// <summary>
        /// Main state logic
        /// </summary>
        /// <exception cref="NullStateException"></exception>
        public void DoLogic()
        {
            ThrowIfCurrentStateIsNull();

            if (IsFirstTimeInState)
            {
                #region Logging
                if (Logger != null)
                {
                    Logger.LogStateMachineMessage(string.Format("[StateMachine] First time in state {0}", CurrentState.GetType()));
                }
                #endregion
                IsFirstTimeInState = false;
                CurrentState.OnEnter();
            }

            StateResult result = CurrentState.DoLogic();
            HandleStateResult(result);
        }

        public void MoveToState(IState nextState = null)
        {
            #region Logging
            if (Logger != null)
            {
                Logger.LogStateMachineMessage(string.Format("[StateMachine] Moving from state:  {0} to state: {1}",
                    CurrentState.GetType(), nextState.GetType()));
            }
            #endregion

            ThrowIfCurrentStateIsNull();

            CurrentState.OnLeave();
            ChangeState(nextState);
        }

        #region Private methods

        private void ThrowIfCurrentStateIsNull()
        {
            if (CurrentState == null)
            {
                #region Logging
                if (Logger != null)
                {
                    Logger.LogStateMachineMessage("[StateMachine] Current state is null");
                }
                #endregion
                throw new NullStateException();
            }
        }

        private void HandleStateResult(StateResult result)
        {
            if (AutoStateTransition == null)
            {
                return;
            }

            var proposedNextState = AutoStateTransition(result);
            if (proposedNextState != null)
            {
                MoveToState(proposedNextState);
            }
        }

        private void ChangeState(IState newState)
        {
            IsFirstTimeInState = true;
            CurrentState = newState;
            var handler = StateChanged;
            if (handler != null)
            {
                Task.Run(() => handler.Invoke(this, newState));
            }
        } 

        #endregion

        #region Fields

        private bool IsFirstTimeInState { get; set; }
        private IStateMachineLogger Logger { get; set; }
        private Func<StateResult, IState> AutoStateTransition { get; set; }

        #endregion
    }
}
