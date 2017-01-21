using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestStateMachine
{
    [TestClass]
    public class TransitionTests
    {
        private StateMachine.StateMachine _machine;
        private IState _nextState;
        private IState _currentState;

        [TestInitialize]
        public void TestInitialize()
        {
            _currentState = A.Fake<IState>();
            _nextState = A.Fake<IState>();
            _machine = new StateMachine.StateMachine(_currentState);
        }

        [TestMethod]
        public void State_OnEnterMethodCalledOnlyOnce()
        {
            _machine.DoLogic();
            _machine.DoLogic();
            _machine.DoLogic();

            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void State_OnEnterMethodCalledWhenFirstTimeInState()
        {
            _machine.DoLogic();
            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void State_OnStateTransition_CurrentStateChanged()
        {
            PrepareStateChangeOnDoLogic();

            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _nextState);
            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _nextState.OnEnter()).MustHaveHappened(Repeated.Never);

            _machine.DoLogic();
            A.CallTo(() => _nextState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void State_OnStateTransition_CurrentStateChangedRaised()
        {
            IState stateChangedByEvent = null;
            PrepareStateChangeOnDoLogic();
            _machine.StateChanged += (sender, state) =>
            {
                stateChangedByEvent = state;
            };

            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _nextState);
            Thread.Sleep(200);
            Assert.AreEqual(stateChangedByEvent, _nextState);
        }

        [TestMethod]
        public void State_OnStateTransition_OnLeaveIsCalled()
        {
            PrepareStateChangeOnDoLogic();

            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _nextState);
            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _currentState.OnLeave()).MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => _nextState.OnEnter()).MustHaveHappened(Repeated.Never);
        }

        [TestMethod]
        public void State_OnStateTransition_OnEnterIsCalledForNextState()
        {
            PrepareStateChangeOnDoLogic();

            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _nextState);
            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _nextState.OnEnter()).MustHaveHappened(Repeated.Never);

            _machine.DoLogic();
            A.CallTo(() => _currentState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _nextState.OnEnter()).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void PrepareStateChangeOnDoLogic()
        {
            //Make DoLogic request a state transition
            A.CallTo(() => _currentState.DoLogic()).Invokes(() => _machine.MoveToState(_nextState));
        }
    }
}
