namespace Pyxis.Messaging.Command
{
    public interface IMessageDispatcher<in T> where T : IMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>
        ///     Returns true if message is handled succesfully.
        ///     Returns false 
        /// </returns>
        bool Dispatch(T command);
    }
}