using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Voter.Hubs
{
    public class Vote : Hub
    {
        public class Answer 
        {
            public string connectionid {get;set;}
            public int vote {get;set;}

        }
        static List<Answer> votes = new List<Answer>();

        public List<Answer> getVotes()
        {
            return votes;
        }

        public void resetVotes()
        {
            votes = new List<Answer>();
            Clients.updateVotes("Votes reset");
        }

        public void setVote(int? vote)
        {
            string currentConnectionId = this.Context.ConnectionId;
            Answer ans = votes.Find(a => a.connectionid == currentConnectionId);
            if (ans == null)
            {
                ans = new Answer();
                ans.connectionid = currentConnectionId;
                votes.Add(ans);
            }
            ans.vote = vote.HasValue ? vote.Value : 0;
            double average = 0;
            if (votes.Count > 0)
            {
                foreach (Answer a in votes)
                {
                    average += a.vote;
                }
                average = average / votes.Count;
            }
            Clients.updateVotes(average + " over " + votes.Count + " votes");
        }
    }
}