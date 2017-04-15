﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using fantasy_football_crawler.Models;
using Newtonsoft.Json;

namespace fantasy_football_crawler
{
    public class FootballCrawler
    {
        private readonly List<long> _teamIdList;
        private Dictionary<long, int> _playerInTeamCount;

        public FootballCrawler(List<long> teamIdList)
        {
            _teamIdList = teamIdList;
        }

        public void DoCrawling()
        {
            if (_teamIdList == null || _teamIdList.Count == 0) return;
            var i = 0;
            foreach (var id in _teamIdList)
            {
                Console.WriteLine(++i);
                var team = GetSingleTeamPlayers(id);
                CountPlayersInTeams(team);
            }
            UploadToDatabase();
        }

        private void UploadToDatabase()
        {
            using (var context = new FantasyFootballEF())
            {
                if (_playerInTeamCount == null) return;
                var now = DateTime.Now;
                foreach (var playerCount in _playerInTeamCount)
                {
                    var selectedPlayer =
                        context.player.Where(pl => pl.id == playerCount.Key).Select(pl => pl).FirstOrDefault();
                    if (selectedPlayer == null)
                        continue;
                    context.player_in_teams_info.Add(new player_in_teams_info
                    {
                        count = playerCount.Value,
                        date = now,
                        player_id = playerCount.Key
                    });
                }
                context.SaveChanges();
            }
        }

        private void CountPlayersInTeams(IEnumerable<long> team)
        {
            if (_playerInTeamCount == null)
                _playerInTeamCount = new Dictionary<long, int>();
            if (_playerInTeamCount.Count == 0)
                foreach (var player in team)
                    _playerInTeamCount.Add(player, 1);
            else
                foreach (var player in team)
                    if (_playerInTeamCount.ContainsKey(player))
                        _playerInTeamCount[player]++;
                    else
                        _playerInTeamCount.Add(player, 1);
        }

        private static IEnumerable<long> GetSingleTeamPlayers(long teamId)
        {
            const string baseUrl = "https://www.sports.ru/fantasy/football/team/json/{0}.json";
            var playersList = new List<long>();
            var errorCount = 0;
            const int maxTries = 100;
            while (true)
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var teamLineupJsonStr = webClient.DownloadString(string.Format(baseUrl, teamId));
                        dynamic teamLineupDynamic = JsonConvert.DeserializeObject(teamLineupJsonStr);
                        foreach (var player in teamLineupDynamic.players)
                            playersList.Add(long.Parse(player.id.ToString()));
                        return playersList;
                    }
                }
                catch (Exception)
                {
                    if (++errorCount == maxTries) throw;
                }
        }
    }
}