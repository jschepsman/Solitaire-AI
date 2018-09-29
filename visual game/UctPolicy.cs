using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    public class UctPolicy
    {
        public Random r;
        public UctPolicy()
        {
            r = new Random();
        }
        public Action uctBestAction(OldGame game, int rollOuts)
        {
            UCTNode rootNode = new UCTNode();
            //set currentState
            UctCurrentState currentState = new UctCurrentState(rootNode, game,null);
            for (int pile = 0; pile < 7; pile++)
            {
                currentState.shownList.Add(new List<Card>(pile));
            }
            for (int i = 0; i < rollOuts; i++)
            {
                Console.WriteLine("roolout {0} Starting", i);
                currentState = new UctCurrentState(rootNode, game, currentState.shownList);


                List<Action> actions = currentState.MoveToAction(game);
                Action nextAction;
                //root node gets actions from game

                //sample actions
                if (actions.Count == 0)
                {
                    nextAction = bestQCTMove(currentState.node);
                }

                else
                {
                    int index = r.Next(actions.Count);
                    nextAction = actions[index];
                }
                //transition state
                currentState.ExploreRoot(nextAction);
                //continue till will win/loss

                while (!currentState.IsDeadEnd())
                {
                    //find actions 
                    actions = currentState.unusedMoves();
                    //sample actions
                    if (actions.Count == 0)
                    {
                        nextAction = bestQCTMove(currentState.node);
                        //if best action has a card moved as hidden current gameplay 
                    }
                    else
                    {
                        int index = r.Next(actions.Count);
                        nextAction = actions[index];
                    }
                    //transition state
                    currentState.Explore(nextAction);
                }
                //update 
                if(currentState.IsWin())
                {
                    currentState.UpdateWin(1);
                }
                else
                {
                    currentState.UpdateWin(0);
                }
            }
            //return best root move
            return bestRootMove(rootNode);
        }
        public Action bestQCTMove(UCTNode node)
        {
            List<Action> bestMove = new List<Action>();
            double bestScore = double.NegativeInfinity;
            foreach (KeyValuePair<Action, UCTNode> kvp in node.children)
            {
                double tempScore = avgQCT(kvp.Value);
                if (bestMove.Count == 0 || bestScore <= tempScore)
                {
                    if (bestScore < tempScore)
                    {
                        bestMove = new List<Action>();
                    }
                    bestMove.Add(kvp.Key);
                    bestScore = tempScore;
                }
            }
            int rand = r.Next(bestMove.Count);
            return bestMove[rand];
        }
        public double avgQCT(UCTNode node)
        {
            double returnVal = 0;
            returnVal = Math.Log10((double)node.parent.visited) / (double)node.visited;
            returnVal = Math.Sqrt(returnVal);
            return node.actionValue+ returnVal;
        }
        public Action bestRootMove(UCTNode node)
        {
            List<Action> bestMove = null;
            double bestScore = double.NegativeInfinity;
            foreach (KeyValuePair<Action, UCTNode> kvp in node.children)
            {
                double tempScore = kvp.Value.actionValue;
                //double tempScore = kvp.Value.actionValue;
                if (bestMove == null || bestScore <= tempScore)
                {
                    if (bestScore < tempScore)
                    {
                        bestMove = new List<Action>();
                    }
                    bestMove.Add(kvp.Key);
                    bestScore = tempScore;
                }
            }
            int rand = r.Next(bestMove.Count);
            return bestMove[rand];
        }
        public List<Action> CurrentGameMoves(UCTNode node)
        {
            UCTNode currentNode = node;
            List<Action> list = new List<Action>();
            while (currentNode.parent != null)
            {
                list.Add(currentNode.actionID);
                currentNode = currentNode.parent;
            }
            list.Reverse();
            return list;
        }

    }
}
