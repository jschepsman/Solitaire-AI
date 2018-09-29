using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;

namespace visual_game
{
    public class State
    {
        public GamePiles gamePiles;
        private int rank;
        public Dictionary<Card, CardLocation> cardsLocation;
        public Dictionary<Card, CardInfo> revealCardOrder;
        public List<Card> unUsed;
        public int lastMoved;

        HashSet<int> pastGames;

        public int[] hidden;

        public int Rank
        {
            get
            {
                return rank;
            }

            set
            {
                rank = value;
            }
        }

        public HashSet<int> PastGames
        {
            get
            {
                return pastGames;
            }

            set
            {
                pastGames = value;
            }
        }

        public State(OldGame game)
        {
            revealCardOrder = new Dictionary<Card, CardInfo>();
            rank = game.rank;
            unUsed = new List<Card>();
            gamePiles = new GamePiles();
            hidden = new int[] {game.hidden[0], game.hidden[1], game.hidden[2], game.hidden[3], game.hidden[4]
                , game.hidden[5], game.hidden[6] };
            cardsLocation = new Dictionary<Card, CardLocation>();
            //load all seen cards into piles/locations
            //loads all unseen cards into unseen list
            for (int pi = 0; pi < 7; pi++)
            {
                for (int ci = 0; ci < hidden[pi]; ci++)
                {
                    unUsed.Add(game.gamePiles[pi][ci]);
                    cardsLocation.Add(game.gamePiles[pi][ci], new CardLocation(-1, -1));
                }
                for (int ci = hidden[pi]; ci < game.gamePiles[pi].Count; ci++)
                {
                    gamePiles[pi].Add(game.gamePiles[pi][ci]);
                    cardsLocation.Add(game.gamePiles[pi][ci], new CardLocation(pi, ci - hidden[pi]));
                }
            }
            for (int pi = 7; pi < 12; pi++)
            {
                for (int ci = 0; ci < game.gamePiles[pi].Count; ci++)
                {
                    gamePiles[pi].Add(game.gamePiles[pi][ci]);
                    cardsLocation.Add(game.gamePiles[pi][ci], new CardLocation(pi, ci));
                }
            }

            lastMoved = game.lastMoved;
            pastGames = new HashSet<int>();
            for(int i =0; i < unUsed.Count; i++)
            {
                revealCardOrder.Add(unUsed[i], new CardInfo());
            }
        }
        public override string ToString()
        {
            string finalString = "";
            for (int pn = 0; pn < 12; pn++)
            {
                finalString += "-";
                if (pn < 7)
                {
                    finalString += hidden[pn].ToString();
                }
                foreach (Card c in gamePiles[pn])
                {
                    finalString += ":" + c.ToString();
                }
            }
            return finalString;
        }

        public List<Action> findMoves()
        {
            List<Action> list = new List<Action>();
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    if (cl.cardIndex != -1&& notLoop(cl.pileNumber, cl.cardIndex, pn) && addToList(moveTo[i], cl, pn))
                    {
                        list.Add(new Action(moveTo[i], pn, cl.pileNumber, (gamePiles[pn].Count != 0 ? gamePiles[pn][gamePiles[pn].Count - 1] : null),
                           revealCardOrder.ContainsKey(moveTo[i])? revealCardOrder[moveTo[i]]: new CardInfo(), gamePiles[pn].Count != 0&&revealCardOrder.ContainsKey(gamePiles[pn][gamePiles[pn].Count - 1]) ? revealCardOrder[gamePiles[pn][gamePiles[pn].Count - 1]] : new CardInfo()));
                    }
                }
            }
            return list;
        }

        private bool notLoop(int pileFrom, int index, int pileTo)
        {
            if(pileFrom == pileTo)
            {
                return false;
            }
            if (pileFrom < 7 && index == 0 && hidden[pileFrom] > 0)
            {
                return true;
            }
            GamePiles afterGame = gamePiles.clone();
            applyMove(index, afterGame[pileFrom], afterGame[pileTo]);
            if (pastGames.Contains(afterGame.GetHashCode()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool addToList(Card card, CardLocation cl, int toPile)
        {
            if (cl.pileNumber < 7)
            {
                if (toPile < 7)
                {
                    return checkFieldtoField(card, cl, toPile);
                }
                else
                {
                    return gamePiles[cl.pileNumber].Count - 1 == cl.cardIndex;
                }
            }
            else if (cl.pileNumber < 11)
            {
                if (toPile < 7)
                {
                    return checkStacktoField(card, cl, toPile);
                }
            }
            else
            {
                return deckPlayable(cl.cardIndex);
            }
            return false;
        }
        private bool checkStacktoField(Card card, CardLocation cl, int toPile)
        {
            //Action temp = new Action(card, toPile, cl.pileNumber, gamePiles[toPile].Count!=0? gamePiles[toPile][gamePiles[toPile].Count - 1]:null);
            if (card.value < rank + 1 || cl.cardIndex != gamePiles[cl.pileNumber].Count - 1)
            {
                return false;
            }
            //  if a primary card is marked unavailable because it's moving from a suit stack to a build stack
            //		3.  we may want to place a card on the primary once it's in the build stack
            for (int i = 0; i < 2; i++)
            {
                Card j = new Card(i == 0 ? (card.suite + 1) % 4 : (card.suite + 3) % 4, card.value - 1);
                CardLocation clj = cardsLocation[j];
                if (faceUp(j) && (clj.pileNumber < 7) ||
                    (clj.pileNumber < 11 && clj.pileNumber > 6 && gamePiles[clj.pileNumber].Count - 1 == clj.cardIndex)
                    || (clj.pileNumber == 11) && deckPlayable(clj.cardIndex))
                {
                    return true;
                }
            }

            //		4.  we may also want to move the card under the primary from a suit stack to a build stack
            Card under = gamePiles[cl.pileNumber][cl.cardIndex - 1];
            CardLocation clunder = cardsLocation[under];
            for (int i = 0; i < 2 && under.value > rank; i++)
            {
                Card j = new Card(i == 0 ? (under.suite + 1) % 4 : (under.suite + 3) % 4, under.value - 1);
                CardLocation clj = cardsLocation[j];
                if (faceUp(j) && (clj.pileNumber < 7) ||
                    (clj.pileNumber < 11 && clj.pileNumber > 6 && gamePiles[clj.pileNumber].Count - 1 == clj.cardIndex)
                    || (clj.pileNumber == 11) && deckPlayable(clj.cardIndex))
                {
                    return true;
                }
            }
            return false;
        }
        private bool checkFieldtoField(Card card, CardLocation cl, int toPile)
        {
            //Action temp = new Action(card, toPile, cl.pileNumber, gamePiles[toPile].Count !=0? gamePiles[toPile][gamePiles[toPile].Count - 1]:null);
            if (!faceUp(card) || (card.value == 12 && cl.cardIndex == 0 && hidden[cl.pileNumber] == 0))
            {
                return false;
            }
            for(int i =0; i<4;i++)
            {
                Card king = new Card(i, 12);
                CardLocation clk = cardsLocation[king];
                if (cl.cardIndex == 0)
                {
                    if(hidden[cl.pileNumber] > 0)
                    {
                        return true;
                    }
                    else if (clk.pileNumber == 11 && deckPlayable(clk.cardIndex))
                    {
                        return true;
                    }
                    else if (clk.pileNumber < 7 && clk.pileNumber >-1 && hidden[clk.pileNumber] > 0)
                    {
                        return true;
                    }
                    
                }
            }

            int underIndex = cl.cardIndex - 1;
            if (underIndex >= 0)
            {
                //check if the card can be moved to stack
                Card under = gamePiles[cl.pileNumber][underIndex];
                CardLocation clunder = cardsLocation[under];
                Card reverse = new Card((card.suite + 2) % 4, card.value);
                CardLocation clr = cardsLocation[reverse];
                //		1.  the card under the primary card may be able to be placed in a suit stack
                if (gamePiles[under.suite + 7].Count == under.value)
                {
                    return true;
                }
                //		2.  we may want to place a different card on the card under the 
                //           primary card (same color and value as the primary)
                else if (faceUp(reverse) &&
                    ((clr.cardIndex < 7) ||
                    (clr.pileNumber > 6 && clr.cardIndex < 11 && rank < reverse.value && gamePiles[clr.pileNumber].Count - 1 == clr.cardIndex) ||
                    (clr.pileNumber == 11 && deckPlayable(clr.cardIndex))))
                {
                    return true;
                }

            }

            return false;

        }
        public bool faceUp(Card card)
        {
            
            CardLocation cl = cardsLocation[card];
            if(cl.cardIndex == -1)
            {
                return false;
            }
            if (cl.pileNumber < 7)
            {
                return true;
            }
            else if (cl.pileNumber < 11)
            {
                return cl.cardIndex == gamePiles[cl.pileNumber].Count - 1 && card.value >= rank;
            }
            else
            {
                return deckPlayable(cl.cardIndex);
            }
        }
        public bool deckPlayable(int index)
        {
            return index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || lastMoved == 0 && index == 0 || index == gamePiles[11].Count - 1;
        }

        public bool isLoss()
        {
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    //if (isVaidAction(cl.cardIndex, cl.pileNumber, pn, lastMoved))
                    if (cl.cardIndex != -1 && notLoop(cl.pileNumber, cl.cardIndex, pn) && addToList(moveTo[i], cl, pn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool isWin()
        {
            return gamePiles[7].Count == 13 && gamePiles[8].Count == 13 && gamePiles[9].Count == 13 && gamePiles[10].Count == 13;
        }
        private void applyMove(int index, CardPile fromPile, CardPile toPile)
        {
            if (fromPile.pileNumber < 7)
            {
                while (fromPile.Count > index)
                {
                    Card temp = fromPile[index];
                    fromPile.RemoveAt(index);
                    toPile.Add(temp);
                }

            }
            else
            {
                Card temp = fromPile[index];
                fromPile.RemoveAt(index);
                toPile.Add(temp);
            }

        }

        public Card makeMove(Action move, Card card, int level)
        {
            applyMove(cardsLocation[move.card].cardIndex, gamePiles[move.oldStack], gamePiles[move.newStack]);
            Card c = updateLocation(gamePiles[move.oldStack], gamePiles[move.newStack], cardsLocation[move.card].cardIndex, card);
            pastGames.Add(gamePiles.GetHashCode());
            if (c != null)
            {
                revealCardOrder[c] = new CardInfo(move.oldStack + 1 - hidden[move.oldStack],move.oldStack);
            }
            return c;
        }
        public Card makeMove(Action move, int level)
        {
            applyMove(cardsLocation[move.card].cardIndex, gamePiles[move.oldStack], gamePiles[move.newStack]);
            Card c = updateLocation(gamePiles[move.oldStack], gamePiles[move.newStack], cardsLocation[move.card].cardIndex, null);
            pastGames.Add(gamePiles.GetHashCode());
            if(c!= null)
            {
                revealCardOrder[c] = new CardInfo(move.oldStack  - (hidden[move.oldStack] + 1), move.oldStack);
            }
            return c;
        }
        public Card updateLocation(CardPile fromPile, CardPile toPile, int index, Card shownCard)
        {
            Card c = null;
            if (fromPile.pileNumber == 11)
            {
                lastMoved = index;
                for (int i = index; i < fromPile.Count; i++)
                {
                    cardsLocation[fromPile[i]].cardIndex = i;
                }
                pastGames = new HashSet<int>();
            }
            else if (fromPile.pileNumber < 7)
            {
                if (0 == fromPile.Count && hidden[fromPile.pileNumber] != 0)
                {
                    if (shownCard == null)
                    {
                        pastGames = new HashSet<int>();
                        Random r = new Random();
                        int ri = r.Next(unUsed.Count);
                        c = unUsed[ri];
                        fromPile.Add(c);

                        cardsLocation[c].pileNumber = fromPile.pileNumber;
                        cardsLocation[c].cardIndex = 0;
                        unUsed.RemoveAt(ri);
                        hidden[fromPile.pileNumber]--;
                    }
                    else
                    {
                        c = shownCard;
                        fromPile.Add(c);

                        cardsLocation[c].pileNumber = fromPile.pileNumber;
                        cardsLocation[c].cardIndex = 0;
                        unUsed.Remove(c);
                        hidden[fromPile.pileNumber]--;
                    }

                }
            }
            for (int i = 0; i < toPile.Count; i++)
            {
                cardsLocation[toPile[i]].cardIndex = i;
                cardsLocation[toPile[i]].pileNumber = toPile.pileNumber;
            }
            if (toPile.pileNumber > 6)
            {
                if (gamePiles[7].Count > rank && gamePiles[8].Count > rank && gamePiles[9].Count > rank && gamePiles[10].Count > rank)
                {
                    rank++;
                    pastGames = new HashSet<int>();
                }
            }
            return c;
        }
        public Move fixMove(Move move)
        {
            if (move.from < 7)
            {
                move.index = move.index + hidden[move.from];
                return move;
            }
            else
            {
                return move;
            }
        }
        public void swap(Card cardA, Card cardB)
        {
            CardLocation oldCL = cardsLocation[cardA];
            CardLocation newCL = cardsLocation[cardB];
            gamePiles[newCL.pileNumber][newCL.cardIndex] = cardA;
            if(oldCL.pileNumber == -1)
            {
                unUsed.Remove(cardA);
                unUsed.Add(cardB);
            }
            else
            {
                gamePiles[oldCL.pileNumber][oldCL.cardIndex] = cardB;
            }
            cardsLocation[cardA] = newCL;
            cardsLocation[cardB] = oldCL;
        }
    }
}
