using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    public class NewNodeState
    {
        public NewUCTNode node;
        private OldGame initgame;
        public NewState game;
        public NewNodeState(NewUCTNode rootNode, OldGame currentGame)
        {
            this.node = rootNode;
            this.initgame = currentGame;
            game = new NewState(currentGame);
        }

        public bool IsDeadEnd()
        {
            return game.IsDeadEnd();
        }

        internal void Reset(NewUCTNode rootNode)
        {
            game = new NewState(initgame); 
        }

        internal List<NewAction> FindUnusedActions()
        {
            List<NewAction> fullList = game.FindMoves();
            List<NewAction> returnList = new List<NewAction>();
            for (int i =0; i < fullList.Count; i++)
            {
                if(!node.children.ContainsKey(fullList[i]))
                {
                    returnList.Add(fullList[i]);
                }
            }
            return returnList;
        }

        internal void ExploreNew(NewAction a)
        {
            //make action a
            //see if action showed a new card if so add a card to shown
            Card card = game.MakeMove(a);

            //add child a
            node.children.Add(a, new NewUCTNode(node,a));
            //change node to node.children[a]
            node = node.children[a];
            
        }

        internal void ExploreOld(NewAction a)
        {
            //swap cards if they are hidden cards
            game.Swap(a);

            //make action a
            //see if action showed a new card if so add a card to shown
            Card card = game.MakeMove(a);
            //see if action showed a new card if so add a card to shown
            //change node to node.children[a]
            node = node.children[a];
        }
        internal void Update()
        {
            double score = game.IsWin() ? 1 : 0;
            double tempaction = 0;
            double visit = 0;
            while (node.parent != null)
            {
                node.visited++;
                visit = 1 / (double)node.visited;
                tempaction = score - node.Score;
                tempaction = tempaction * visit;
                node.Score = tempaction + node.Score;
                node = node.parent;
            }
             node.visited++;
            visit = 1 / (double)node.visited;
            tempaction = score - node.Score;
            tempaction = tempaction * visit;
            node.Score = tempaction + node.Score;
        }
    }
}
