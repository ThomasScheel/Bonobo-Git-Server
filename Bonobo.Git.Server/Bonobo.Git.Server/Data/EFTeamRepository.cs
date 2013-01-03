using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Bonobo.Git.Server.Models;

namespace Bonobo.Git.Server.Data
{
    public class EFTeamRepository : ITeamRepository
    {
        public IList<TeamModel> GetAllTeams()
        {
            using (var db = new DataEntities())
            {
                return db.Team.Include("Members")
                    .Include("Repositories")
                    .Select(this.CreateViewModel)
                    .ToList();
            }
        }

        public IList<TeamModel> GetTeams(string username)
        {
            using (var db = new DataEntities())
            {
                return db.Team.Include("Members")
                    .Include("Repositories")
                    .Where(team => team.Members.Any(member => member.Username == username))
                    .Select(this.CreateViewModel)
                    .ToList();
            }
        }

        public TeamModel GetTeam(string name)
        {
            if (name == null) throw new ArgumentException("name");

            using (var db = new DataEntities())
            {
                return db.Team.Include("Members")
                    .Include("Repositories")
                    .Where(team => team.Name == name)
                    .Select(this.CreateViewModel)
                    .FirstOrDefault();
            }
        }

        public void Delete(string name)
        {
            if (name == null) throw new ArgumentException("name");

            using (var db = new DataEntities())
            {
                var team = db.Team.FirstOrDefault(i => i.Name == name);
                if (team != null)
                {
                    team.Repositories.Clear();
                    team.Members.Clear();
                    db.DeleteObject(team);
                    db.SaveChanges();
                }
            }
        }

        public bool Create(TeamModel model)
        {
            if (model == null) throw new ArgumentException("team");
            if (model.Name == null) throw new ArgumentException("name");

            using (var database = new DataEntities())
            {
                var team = new Team
                {
                    Name = model.Name,
                    Description = model.Description
                };
                database.AddToTeam(team);
                if (model.Members != null)
                {
                    AddMembers(model.Members, team, database);
                }
                try
                {
                    database.SaveChanges();
                }
                catch (UpdateException)
                {
                    return false;
                }
            }

            return true;
        }

        public void Update(TeamModel model)
        {
            if (model == null) throw new ArgumentException("team");
            if (model.Name == null) throw new ArgumentException("name");

            using (var db = new DataEntities())
            {
                var team = db.Team.FirstOrDefault(i => i.Name == model.Name);
                if (team != null)
                {
                    team.Description = model.Description;
                    team.Members.Clear();
                    if (model.Members != null)
                    {
                        AddMembers(model.Members, team, db);
                    }
                    db.SaveChanges();
                }
            }
        }

        private void AddMembers(string[] members, Team team, DataEntities database)
        {
            var users = database.User.Where(i => members.Contains(i.Username));
            foreach (var item in users)
            {
                team.Members.Add(item);
            }
        }

        private TeamModel CreateViewModel(Team team)
        {
            return new TeamModel
            {
                Name = team.Name,
                Description = team.Description,
                Members = team.Members.Select(member => member.Username).ToArray(),
                Repositories = team.Repositories.Select(repo => repo.Name).ToArray(),
            };
        }
    }
}