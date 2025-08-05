using RaceWriterBot.Application.DTOs;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public interface IMessageParser
    {
        MessageDTO HandleChatMember(ChatMemberUpdated myChatMember);
        MessageDTO HandleMessage(Message message);
    }
}