namespace RaceWriterBot.Domain.ValueObjects
{
    public class Registration
    {
        public Registration(UserId userId, string name, DateTime registrationTime)
        {
            UserId = userId;
            Name = name;
            RegistrationTime = registrationTime;
        }

        public UserId UserId { get; }
        public string Name { get; }
        public DateTime RegistrationTime { get; }
    }
}
