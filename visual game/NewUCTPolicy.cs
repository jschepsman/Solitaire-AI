using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    
    public class NewUCTPolicy
    {
        NewUCTNode rootNode;
        Random r;
        public NewUCTPolicy()
        {
            r = new Random();
            rootNode = new NewUCTNode();
        }
        //create UCT Tree
        public void BuildUCTTree(int numRollouts, OldGame game)
        {
            NewNodeState currentState = new NewNodeState(rootNode, game);
            for (int t = 0; t < numRollouts; t++)
            {
                //find moves from old game
                List<NewAction> rootMoves = ConvertToActions(game);
                NewAction ra = null;
                //2st
                //currentState.node.visited++;
                if (rootMoves.Count == 0)
                {
                    ra = BestChild(rootNode);
                    currentState.ExploreOld(ra);
                }
                else
                {
                    ra = RandomUnusedMove(rootMoves);
                    currentState.ExploreNew(ra);
                }
                Rollout(currentState);
                currentState.Reset(rootNode);
            }


        }

        private List<NewAction> ConvertToActions(OldGame game)
        {
            List<NewAction> actionList = new List<NewAction>();
            List<Move> list = game.FindMoveList();
            foreach(Move m in list)
            {

                NewAction a = new NewAction(game.gamePiles[m.from][m.index], game.gamePiles[m.to].Count==0?null:game.gamePiles[m.to][game.gamePiles[m.to].Count-1], m.from,m.to,-1,-1,-1,-1);
                if(!rootNode.children.ContainsKey(a))
                {
                    actionList.Add(a);
                }
            }
            return actionList;
        }

        //one Roll out
        public void Rollout(NewNodeState currentState)
        {
            while(!currentState.IsDeadEnd())
            {
                List<NewAction> unusedActions = currentState.FindUnusedActions();
                NewAction a;
                //1
                //currentState.node.visited++;
                if (unusedActions.Count != 0)
                {
                    a = RandomUnusedMove(unusedActions);
                    currentState.ExploreNew(a);
                }
                else
                {
                    a = BestChild(currentState.node);
                    currentState.ExploreOld(a);
                }
                //2nd
                //currentState.node.visited++;

            }
            //1
            currentState.node.visited++;
            currentState.Update();
        }

        //move selection
        //choose best child move
        private NewAction BestChild(NewUCTNode node)
        {
            List<NewAction> bestActions = null;
            double bestScore = double.NegativeInfinity;
            foreach (KeyValuePair<NewAction, NewUCTNode> kvp in node.children)
            {
                double score = AvgQCT(kvp.Value);

                if (bestActions == null || score >= bestScore)
                {
                    if (score > bestScore)
                    {
                        bestActions = new List<NewAction>();
                        bestScore = score;
                    }
                    bestActions.Add(kvp.Value.ActionID);
                }
            }
            int i = r.Next(bestActions.Count);
            return bestActions[i];
        }

        private NewAction RandomUnusedMove(List<NewAction> list)
        {
            int i = r.Next(list.Count);
            return list[i];
        }
        //choose best move
        public NewAction BestMove(OldGame game, int rollouts)
        {
            rootNode = new NewUCTNode();
            BuildUCTTree(rollouts, game);
            return BestRootMove();
        }
        public double AvgQCT(NewUCTNode node)
        {
            double returnVal = 0;
            returnVal = Math.Log(node.parent.visited) / (double)node.visited;
            returnVal = Math.Sqrt(returnVal);
            return node.Score + returnVal;
        }
        public NewAction BestRootMove()
        {
            List<NewAction> testList = new List<NewAction>();
            List<NewAction> bestActions = null;
            double bestScore = double.NegativeInfinity;
            List<NewAction> bestActions2 = null;
            double bestScore2 = double.NegativeInfinity;
            NewAction actionTest = null;
            foreach (KeyValuePair<NewAction, NewUCTNode> kvp in rootNode.children)
            {
                if(kvp.Value.children.Count >0)
                {
                    actionTest = BestBasicChild(kvp.Value);
                    NewUCTNode testNode = kvp.Value.children[actionTest];
                    while ((actionTest.moveCard.Equals(kvp.Key.moveCard) && actionTest.toPile == kvp.Key.fromPile) && testNode.children.Count > 0)
                    {
                        actionTest = BestBasicChild(testNode);
                        testNode = testNode.children[actionTest];
                    }
                }
                double score = kvp.Value.Score;
                if (actionTest != null &&(!rootNode.children.ContainsKey(actionTest)|| actionTest.fromPile == 11))
                {
                    testList.Add(kvp.Key);
                    if (bestActions == null || score >= bestScore)
                    {
                        if (score > bestScore)
                        {
                            bestActions = new List<NewAction>();
                            bestScore = score;
                        }
                        bestActions.Add(kvp.Value.ActionID);
                    }
                }
                if (bestActions2 == null || score >= bestScore2)
                {
                    if (score > bestScore2)
                    {
                        bestActions2 = new List<NewAction>();
                        bestScore2 = score;
                    }
                    bestActions2.Add(kvp.Value.ActionID);
                }
            }
            if (bestActions != null &&bestActions.Count > 0)
            {
                int i = r.Next(bestActions.Count);
                return bestActions[i];

            }
            else
            {
                int i = r.Next(bestActions2.Count);
                return bestActions2[i];
            }

        }

        private NewAction BestBasicChild(NewUCTNode node)
        {
            List<NewAction> bestActions = null;
            double bestScore = double.NegativeInfinity;
            foreach (KeyValuePair<NewAction, NewUCTNode> kvp in node.children)
            {
                double score = node.Score;

                if (bestActions == null || score >= bestScore)
                {
                    if (score > bestScore)
                    {
                        bestActions = new List<NewAction>();
                        bestScore = score;
                    }
                    bestActions.Add(kvp.Value.ActionID);
                }
            }
            int i = r.Next(bestActions.Count);
            return bestActions[i];
        }
    }
}
