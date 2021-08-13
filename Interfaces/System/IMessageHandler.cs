namespace HunterCombatMR.Interfaces.System
{
    public interface IMessageHandler<TMessage>
    {
        bool HandleMessage(TMessage message);
    }
}