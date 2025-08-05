using Telegram.Bot.Types;

namespace RaceWriterBot.Application.Interfaces
{
    public interface IMessageHandler
    {
        Task ProcessChatMember(ChatMemberUpdated myChatMember);
        Task ProcessMessage(Message message);
    }
}
