using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    public class UCTNode
    {
        public UCTNode parent;
        public Dictionary<Action, UCTNode> children;
        public Action actionID;
        public int wins;
        public int visited;
        public double actionValue;
        public int level;
        public UCTNode()
        {
            level = 0;
            wins = 0;
            visited = 0;
            actionValue = 0;
            parent = null;
            children = new Dictionary<Action, UCTNode>();

        }
        public UCTNode(UCTNode parentNode, Action actionToThis)
        {
            level = parentNode.level + 1;
            parent = parentNode;
            children = new Dictionary<Action, UCTNode>();
            actionID = actionToThis;
        }
        public override string ToString()
        {
            return wins.ToString() + ":" + visited.ToString() + ":" + actionValue.ToString();
        }
        public void update(bool win)
        {
            if(win)
            {
                wins++;
                double visit = 1 / (double)visited;
                double tempaction = 1 - actionValue;
                tempaction = tempaction * visit;
                actionValue = tempaction + actionValue;
            }
            else
            {
                double visit = 1 / (double)visited;
                double tempaction = 0 - actionValue;
                tempaction = tempaction * visit;
                actionValue = tempaction + actionValue;
            }
        }
    }
}
