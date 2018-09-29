using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visual_game
{
    public class NewUCTNode
    {
        internal Dictionary<NewAction, NewUCTNode> children;
        internal NewUCTNode parent;
        internal double visited;

        public double Score { get; internal set; }
        internal NewAction ActionID { get; set; }
        public NewUCTNode()
        {
            parent = null;
            children = new Dictionary<NewAction, NewUCTNode>();
            visited = 0;
            Score = 0;
            ActionID = null;
        }

        public NewUCTNode(NewUCTNode node, NewAction action)
        {
            parent = node;
            children = new Dictionary<NewAction, NewUCTNode>();
            visited = 0;
            Score = 0;
            ActionID = action;
        }
        public override string ToString()
        {
            return visited.ToString() + ":" + Score.ToString();
        }
    }
}
