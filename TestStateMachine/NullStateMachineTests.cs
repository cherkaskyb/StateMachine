using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMachine;
using StateMachine.Exceptions;

namespace TestStateMachine
{
    [TestClass]
    public class NullStateMachineTests
    {
        [TestMethod]
        public void DoLogic_WhenFirstStateIsNull_NullStateException()
        {
            try
            {
                IStateMachine machine = new StateMachine.StateMachine(null);
                machine.DoLogic();
                Assert.Fail("Exception not thrown");
            }
            catch (NullStateException)
            {
                // OK
            } catch (Exception e)
            {
                Assert.Fail("Wrong Exception thrown. Message: " + e.Message);
            }
        }

        [TestMethod]
        public void MoveToState_WhenFirstStateIsNull_NullStateException()
        {
            try
            {
                IStateMachine machine = new StateMachine.StateMachine(null);
                machine.MoveToState(null);
                Assert.Fail("Exception not thrown");
            }
            catch (NullStateException)
            {
                // OK
            }
            catch (Exception e)
            {
                Assert.Fail("Wrong Exception thrown. Message: " + e.Message);
            }
        }


    }
}
