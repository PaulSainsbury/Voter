using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Voter.Hubs
{
    [HubName("vote")]
    public class VoteHub : Hub
    {
        public class Vote
        {
            public string connectionId { get; set; }
            public int speed { get; set; }
            public int preparation { get; set; }
            public int techValue { get; set; }
            public int demos { get; set; }
            public string comments { get; set; }
            public int numberOfVotes { get; set; }
        }

        static List<Vote> votes = new List<Vote>();

        public List<Vote> getVotes()
        {
            return votes;
        }

        public void resetVotes()
        {
            votes = new List<Vote>();
            
            Clients.updateVotes(new Vote());
        }

        public void setVote(Vote v)
        {
            string currentConnectionId = this.Context.ConnectionId;
            Vote storedVote = votes.Find(a => a.connectionId == currentConnectionId);
            if (storedVote  == null)
            {
                storedVote  = new Vote();
                storedVote .connectionId = currentConnectionId;
                votes.Add(storedVote );
            };
            storedVote.speed = v.speed;
            storedVote.preparation = v.preparation;
            storedVote.techValue = v.techValue;
            storedVote.demos = v.demos;
            storedVote.comments = v.comments;

            SendResults();
        }

        public void setupConnection(string oldConnectionId)
        {
            string currentConnectionId = this.Context.ConnectionId;
            Vote storedVote = votes.Find(a => a.connectionId == oldConnectionId);
            if (storedVote == null)
            {
                storedVote = new Vote();
            }
            storedVote.connectionId = currentConnectionId;

            Caller.setValue(storedVote);
            SendResults();
        }


        private void SendResults()
        {
            Vote total = new Vote();
            if (votes.Count > 0)
            {
                foreach (Vote v in votes)
                {
                    total.speed += v.speed;
                    total.preparation += v.preparation;
                    total.techValue += v.techValue;
                    total.demos += v.demos;
                    if (!String.IsNullOrWhiteSpace(v.comments))
                    {
                        if (!String.IsNullOrWhiteSpace(total.comments))
                        {
                            total.comments += "<br/>";
                        }
                        total.comments += v.comments.Replace("\n", "<br/>");
                    }
                }
                total.numberOfVotes = votes.Count();
            }
            Clients.updateVotes(total);
        }
    }
}