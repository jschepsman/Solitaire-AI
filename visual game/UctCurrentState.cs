using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    public class UctCurrentState
    //gameUCT.gamePiles[move.card.suite+7].Count != move.card.value && move.newStack == move.card.suite+7
    {
        public UCTNode node;
        public State gameUCT;
        public State initState;
        public List<List<Card>> shownList;
        public UctCurrentState(UCTNode currentNode, OldGame currentGame, List<List<Card>> shownoderList)
        {
            if(shownoderList == null)
            {
                shownList = new List<List<Card>>(7);

                for (int pile = 0; pile < 7; pile++)
                {
                    shownList.Add(new List<Card>(pile));
                }
            }
            else
            {
                shownList = shownoderList;
            }

            node = currentNode;
            currentNode.visited++;
            gameUCT = new State(currentGame);
            initState = new State(currentGame);
        }

        internal List<NewAction> FindUnusedActions()
        {
            throw new NotImplementedException();
        }

        public void Explore(Action move)
        {
            bool breakpoint = gameUCT.gamePiles[move.card.suite + 7].Count != move.card.value && move.newStack == move.card.suite + 7;
            if (!node.children.ContainsKey(move))
            {
                ExploreNew(move);
                //Console.WriteLine("New Node level {0}", node.level);
            }
            else
            {
                //Swap
                if (!gameUCT.gamePiles[move.oldStack].Contains(move.card))
                {
                    //swap card being moved
                    Card temp = FindSwap(move.card, move.cardinfo);
                    gameUCT.swap(move.card, temp);
                }
                if (move.ontoCard != null && !gameUCT.gamePiles[move.newStack].Contains(move.ontoCard))
                {
                    //swap card that is being moved onto
                    Card temp = FindSwap(move.ontoCard, move.ontoCardInfo);
                    gameUCT.swap(move.ontoCard, temp);
                }
                ExploreOld(move);
                Console.WriteLine("Old Node level {0}", node.level);
            }
        }
        public Card FindSwap(Card swapCard, CardInfo level)
        {
            Card temp = shownList[level.pileNum][level.shownIndex];
            shownList[level.pileNum][level.shownIndex] = swapCard;
            return temp;
        }
        public void ExploreRoot(Action move)
        {
            bool breakpoint = gameUCT.gamePiles[move.card.suite + 7].Count != move.card.value && move.newStack == move.card.suite + 7;
            if (!node.children.ContainsKey(move))
            {
                ExploreNew(move);
                //Console.WriteLine("New Node level {0}", node.level);
            }
            else
            {
                //swap
                if(!gameUCT.gamePiles[move.oldStack].Contains(move.card))
                {
                    //swap card being moved
                    Card temp = FindSwap(move.card, move.cardinfo);
                    gameUCT.swap(move.card, temp);
                }
                if(move.ontoCard!= null&&!gameUCT.gamePiles[move.newStack].Contains(move.ontoCard))
                {
                    //swap card that is being moved onto
                    Card temp = FindSwap(move.ontoCard, move.ontoCardInfo);
                    gameUCT.swap(move.ontoCard, temp);
                }
                ExploreOld(move);
                Console.WriteLine("Old node level {0}", node.level);
            }
            
        }
        public void ExploreNew(Action move)
        {
            Card card = gameUCT.makeMove(move, node.level + 1);
            if(card!= null)
            {
                CardInfo cf = gameUCT.revealCardOrder[card];
                if(shownList[cf.pileNum].Count > cf.shownIndex)
                {
                    shownList[cf.pileNum][cf.shownIndex] = card;
                }
                else
                {
                    shownList[cf.pileNum].Add(card);
                }
            }
            node.children.Add(move, new UCTNode(node, move));
            node = node.children[move];
            node.visited++;
        }

        public void ExploreOld(Action move)
        {
            node = node.children[move];
            node.visited++;
            Card card = gameUCT.makeMove(move, node.level);
            if(card != null)
            {
                CardInfo temp = gameUCT.revealCardOrder[card];
                shownList[temp.pileNum][temp.shownIndex] = card;
            }
        }
        public bool IsDeadEnd()
        {
            return gameUCT.isWin() || gameUCT.isLoss();
        }
        public List<Action> unusedMoves()
        {
            List<Action> moveList = gameUCT.findMoves();
            foreach (Action m in node.children.Keys)
            {
                moveList.Remove(m);
            }
            return moveList;
        }
        public bool IsWin()
        {
            return gameUCT.isWin();
        }
        public void UpdateWin(int reward)
        {
            UCTNode temp = node;
            while (temp.parent != null)
            {
                temp.update(reward == 1);
                temp = temp.parent;
            }
            temp.wins++;
        }
        public List<Action> VisitRootNode(OldGame game)
        {
            List<Move> gameMoves = game.FindMoveList();
            List<Action> returnList = new List<Action>();
            foreach (Move m in gameMoves)
            {
                Action temp = new Action(game.gamePiles[m.from][m.index], m.to, m.from, game.gamePiles[m.to].Count==0? game.gamePiles[m.to][game.gamePiles[m.to].Count - 1]:null);
                returnList.Add(temp);
            }
            foreach (Action m in node.children.Keys)
            {
                
                returnList.Remove(m);
            }
            return returnList;
        }
        public List<Action> MoveToAction(OldGame game)
        {
            List<Move> moves = game.FindMoveList();
            List<Action> moveList = new List<Action>();
            foreach (Move m in moves)
            {
                moveList.Add(new Action(game.gamePiles[m.from][m.index], m.to, m.from,(game.gamePiles[m.to].Count!=0? game.gamePiles[m.to][game.gamePiles[m.to].Count - 1]:null)));
            }
            foreach (Action m in node.children.Keys)
            {
                moveList.Remove(m);
            }
            return moveList;
        }
    }
}
