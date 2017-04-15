namespace fantasy_football_crawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region old

            //const string url = "https://www.sports.ru/fantasy/football/team/create/52.json";
            //var playersJsonString = string.Empty;
            //using (var webClient = new WebClient())
            //{
            //    playersJsonString = webClient.DownloadString(url);
            //}
            //dynamic playersDynamic = JsonConvert.DeserializeObject(playersJsonString);
            //var players = new List<PlayerFromJson>();
            //using (var context =  new FantasyFootballEF())
            //{
            //    foreach (var obj in playersDynamic.players)
            //    {
            //        context.player.Add(new player()
            //        {
            //            id = obj.id != null ? long.Parse(obj.id.ToString()) : 0,
            //            name = obj.name != null ? obj.name.ToString() : string.Empty
            //        });
            //        Console.WriteLine(obj.name);
            //    }
            //    Console.WriteLine("SAVE STARTED!");
            //    context.SaveChanges();
            //    Console.WriteLine("DONE!");
            //}
            //Console.ReadLine();

            //            const string url = "https://www.sports.ru/fantasy/football/tournament/ratings/leaders/52.html?p=2";
            //            var wc = new WebClient();
            //            var page = wc.DownloadString(url);
            //            var doc = new HtmlDocument();
            //            doc.LoadHtml(page);
            //            var teamUrls = doc.DocumentNode.SelectSingleNode("//table[@class='stat-table']").Descendants("tr").Skip(1)
            //                .Select(tr => tr.Elements("td").Select(td => td).ElementAt(1).FirstChild.Attributes["href"].Value)
            //                .ToList();
            //            var teamIdList = new List<long>();
            //            foreach (var teamUrl in teamUrls)
            //            {
            //                string s = string.Empty;
            //                foreach (var c in teamUrl.ToCharArray())
            //                {
            //                    if (char.IsDigit(c))
            //                        s += c;
            //                }
            //                teamIdList.Add(long.Parse(s));

            #endregion

            var pagecount = 0;
            if (!int.TryParse(args[0], out pagecount)) return;
            if (pagecount <= 0) return;
            var teamIdList = FootballHtmlParser.GetTeamIdList(pagecount);
            var fc = new FootballCrawler(teamIdList);
            fc.DoCrawling();
        }
    }
}