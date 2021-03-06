﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace fantasy_football_crawler
{
    public static class FootballHtmlParser
    {
        public static List<long> GetTeamIdList(int pagesToParseCount)
        {
            const string baseUrl = "https://www.sports.ru/fantasy/football/tournament/ratings/leaders/52.html";
            var resultList = GetTeamIdListForSinglePage(baseUrl);
            if (pagesToParseCount <= 1) return resultList;
            for (var pageNumber = 2; pageNumber <= pagesToParseCount; pageNumber++)
            {
                var nextPageUrl = baseUrl + "?p=" + pageNumber;
                resultList.AddRange(GetTeamIdListForSinglePage(nextPageUrl));
            }
            return resultList;
        }

        public static Dictionary<long, int> GetTeamIdDictionary(int pagesToParseCount)
        {
            const string baseUrl = "https://www.sports.ru/fantasy/football/tournament/ratings/leaders/52.html";
            var resultResult = GetTeamIdDictionaryForSinglePage(baseUrl);
            if (pagesToParseCount <= 1) return resultResult;
            for (var pageNumber = 2; pageNumber <= pagesToParseCount; pageNumber++)
            {
                var nextPageUrl = baseUrl + "?p=" + pageNumber;
                var singlePageDictionary = GetTeamIdDictionaryForSinglePage(nextPageUrl);
                foreach (var entry in singlePageDictionary)
                    resultResult.Add(entry.Key, entry.Value);
            }
            return resultResult;
        }

        private static List<long> GetTeamIdListForSinglePage(string url)
        {
            using (var wc = new WebClient())
            {
                var page = wc.DownloadString(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(page);
                var teamUrls =
                    doc.DocumentNode.SelectSingleNode("//table[@class='stat-table']").Descendants("tr").Skip(1)
                        .Select(
                            tr => tr.Elements("td").Select(td => td).ElementAt(1).FirstChild.Attributes["href"].Value)
                        .ToList();
                wc.Dispose();
                return
                    teamUrls.Select(
                            teamUrl =>
                                teamUrl.ToCharArray()
                                    .Where(char.IsDigit)
                                    .Aggregate(string.Empty, (current, c) => current + c))
                        .Select(long.Parse)
                        .ToList();
            }
        }

        private static Dictionary<long, int> GetTeamIdDictionaryForSinglePage(string url)
        {
            using (var wc = new WebClient())
            {
                var page = wc.DownloadString(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(page);
                var teamUrls =
                    doc.DocumentNode.SelectSingleNode("//table[@class='stat-table']").Descendants("tr").Skip(1)
                        .Select(
                            tr => tr.Elements("td").Select(td => td).ElementAt(1).FirstChild.Attributes["href"].Value)
                        .ToList();
                var teamPlaces =
                    doc.DocumentNode.SelectSingleNode("//table[@class='stat-table']").Descendants("tr").Skip(1)
                        .Select(
                            tr => tr.Elements("td").Select(td => td).ElementAt(0).InnerText)
                        .ToList();
                var tmp = teamPlaces;
                var resultDictionary = new Dictionary<long, int>();
                for (var i = 0; i < teamUrls.Count; i++)
                {
                    var teamId =
                        long.Parse(teamUrls[i].ToCharArray()
                            .Where(char.IsDigit)
                            .Aggregate(string.Empty, (current, c) => current + c));
                    var teamPlace =
                        int.Parse(teamPlaces[i].ToCharArray()
                            .Where(char.IsDigit)
                            .Aggregate(string.Empty, (current, c) => current + c));
                    resultDictionary.Add(teamId, teamPlace);
                }
                return resultDictionary;
            }
        }
    }
}