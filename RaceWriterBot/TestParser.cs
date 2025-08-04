using System.Text.RegularExpressions;

namespace RaceWriterBot.Presentation.Handlers
{
    public class TestParser
    {
        private (string Name, List<string> Slots)? ParseSmart(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // Ищем все слоты — время или число
            var matches = Regex.Matches(input, @"\b(\d{1,2}:\d{2}|\d{1,2})\b");

            if (matches.Count == 0) return null;

            // Считаем, что имя — всё до первого совпадения
            var firstSlotIndex = matches[0].Index;
            var name = input.Substring(0, firstSlotIndex).Trim();
            if (string.IsNullOrWhiteSpace(name)) return null;

            var slots = new List<string>();

            foreach (Match match in matches)
            {
                var value = match.Value;
                if (TimeOnly.TryParse(value, out var time))
                    slots.Add(time.ToString("HH:mm"));
                else if (int.TryParse(value, out var numSlot))
                    slots.Add($"Slot #{numSlot}");
            }

            return (name, slots);
        }
    }
}