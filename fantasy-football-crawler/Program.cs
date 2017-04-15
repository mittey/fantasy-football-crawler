namespace fantasy_football_crawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int pagecount;
            if (!int.TryParse(args[0], out pagecount)) return;
            if (pagecount <= 0) return;
            var teamIdDictionary = FootballHtmlParser.GetTeamIdDictionary(pagecount);
            var fc = new FootballCrawler(teamIdDictionary);
            fc.DoCrawling();
        }
    }
}