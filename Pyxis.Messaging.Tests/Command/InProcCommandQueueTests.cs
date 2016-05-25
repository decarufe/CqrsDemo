using System.Threading;
using NUnit.Framework;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Tests.Command
{
    [TestFixture]
    public class InProcCommandQueueTests
    {
        private InProcCommandQueue _commandQueue;
        private TestDispatcher _dispatcher;
        private const int WaitDelay = 50;

        [SetUp]
        public void SetUp()
        {
            _dispatcher = new TestDispatcher();
            _commandQueue = new InProcCommandQueue(_dispatcher);
            
        }

        [Test]
        public void TestThatCommandIsDispactchedRightAwayForCommandType()
        {
            _commandQueue.QueueCommand(new CommandType("mycommand"));
            Thread.Sleep(WaitDelay);
            Assert.IsTrue(_dispatcher.Called);
        }

        [Test]
        public void TestThatCommandIsDispactchedRightAwayForCommand()
        {
            var testCommand = new Messaging.Command.Command(new CommandType("mycommand"));
            _commandQueue.QueueCommand(testCommand);
            Thread.Sleep(WaitDelay);
            Assert.IsTrue(_dispatcher.Called);
        }

        [Test]
        public void TestThatFollowupCommandCanBeQueued()
        {
            var testCommand = new Messaging.Command.Command(new CommandType("mycommand"));
            _commandQueue.QueueFollowupCommand(new CommandType("mycommand"), testCommand);
            Thread.Sleep(WaitDelay);
            Assert.IsTrue(_dispatcher.Called);
        }
    }

    class TestDispatcher : IMessageDispatcher<ICommand>
    {
        public bool Called { get; set; }
        public bool Dispatch(ICommand command)
        {
            Called = true;
            return true;
        }
    }
}
