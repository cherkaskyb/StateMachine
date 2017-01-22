using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachine;
using FakeItEasy;

namespace TestStateMachine
{
    [TestClass]
    public class AutoTransitionTests
    {
        private StateMachine.StateMachine _machine;
        private IState _defaultState;
        private IState _errorState;
        private IState _currentState;

        [TestInitialize]
        public void TestInitialize()
        {
            _currentState = A.Fake<IState>();
            _defaultState = A.Fake<IState>();
            _errorState = A.Fake<IState>();

            Func<StateResult, IState> autoTransitionFunc = stateResult =>
            {
                if (stateResult == StateResult.Default)
                {
                    return _defaultState;
                }

                if (stateResult == StateResult.Error)
                {
                    return _errorState;
                }

                return null;
            };
            _machine = new StateMachine.StateMachine(_currentState, autoTransitionFunc, null);
        }

        [TestMethod]
        public void AutoTransition_TransitionToDefault_WhenResultIsDefault()
        {
            A.CallTo(() => _currentState.DoLogic()).Returns(StateResult.Default);
            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _defaultState);
            A.CallTo(() => _currentState.OnLeave()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AutoTransition_TransitionToError_WhenResultIsError()
        {
            A.CallTo(() => _currentState.DoLogic()).Returns(StateResult.Error);
            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _errorState);
            A.CallTo(() => _currentState.OnLeave()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AutoTransition_NoTransition_WhenResultIsOk()
        {
            A.CallTo(() => _currentState.DoLogic()).Returns(StateResult.Ok);
            _machine.DoLogic();
            Assert.AreEqual(_machine.CurrentState, _currentState);
            A.CallTo(() => _currentState.OnLeave()).MustHaveHappened(Repeated.Never);
        }
    }
}
