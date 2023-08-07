namespace OktaAutomation.Extensions
{
    public static class HclExtensions
    {
        public static List<string> SelectResource(this List<string> source, int position)
        {
            var startingPoint = source.Skip(position).ToList();
            var endIndex = GetResourceEndIndex(startingPoint);

            return startingPoint.Take(endIndex).ToList();
        }

        private static int GetResourceEndIndex(List<string> start)
        {
            var index = 0;
            var bracketCount = 0;

            var startingIndex = start.FindIndex(0, start.Count, x => x.Contains("resource"));
            var lines = start.GetRange(startingIndex, start.Count - startingIndex);
            foreach (var line in lines)
            {
                index++;
                if (line.Contains('{'))
                {
                    bracketCount++;
                }

                if (line.Contains('}'))
                {
                    bracketCount--;
                }

                if (bracketCount == 0)
                {
                    return index;
                }
            }
            return 0;
        }
    }
}
