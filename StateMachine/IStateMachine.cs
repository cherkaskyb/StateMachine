using System;

namespace StateMachine
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        EventHandler<IState> StateChanged { get; set; }

        void DoLogic();
        void MoveToState(IState nextState);
    }
}