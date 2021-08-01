namespace HunterCombatMR.Interfaces.System
{
    public interface IMessageHandler<TMessage>
        where TMessage : struct
    {
        bool HandleMessage(TMessage message);
    }
}