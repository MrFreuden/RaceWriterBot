using RaceWriterBot.Domain.Interfaces;

namespace RaceWriterBot.Domain.ValueObjects
{
    public class StringInput : IStateInput
    {
        public string Value { get; }

        public StringInput(string value)
        {
            Value = value;
        }
        public Type GetInputType() => typeof(string);
    }
}
