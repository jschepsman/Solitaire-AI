using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;

namespace visual_game
{
    public class NewState
    {
        public GamePiles gamePiles;
        public int[] hidden;
        public int rank;
        public Dictionary<Card, CardLocation> cardsLocation;
        public Dictionary<Card, CardLocation> shownUnusedOrder;
        public List<Card> unUsed;

        public int lastMoved;

        public List<List<Card>> shownOrder;
        HashSet<int> pastGames;

        public NewState(OldGame currentGame)
        {
            gamePiles = new GamePiles();
            hidden = new int[] {currentGame.hidden[0], currentGame.hidden[1], currentGame.hidden[2], currentGame.hidden[3], currentGame.hidden[4]
                , currentGame.hidden[5], currentGame.hidden[6] };
            rank = currentGame.rank;
            cardsLocation = new Dictionary<Card, CardLocation>();
            shownUnusedOrder = new Dictionary<Card, CardLocation>();
            unUsed = new List<Card>();
            pastGames = new HashSet<int>();
            lastMoved = currentGame.lastMoved;

            shownOrder = new List<List<Card>>(7);
            for(int i = 0; i < 7; i++)
            {
                shownOrder.Add(new List<Card>(i));
            }
            foreach(KeyValuePair<Card,CardLocation> cardinfo in currentGame.cardsLocation)
            {
                if(currentGame.faceUp(cardinfo.Key)|| cardinfo.Value.pileNumber> 6)
                {
                    int pilenum = cardinfo.Value.pileNumber;
                    int index = pilenum < 7 ? cardinfo.Value.cardIndex - hidden[pilenum] : cardinfo.Value.cardIndex;
                    cardsLocation.Add(cardinfo.Key,new CardLocation(pilenum, index));
                }
                else
                {
                    unUsed.Add(cardinfo.Key);
                    cardsLocation.Add(cardinfo.Key, new CardLocation(-1, -1));
                    shownUnusedOrder.Add(cardinfo.Key, new CardLocation(-1, -1));
                }
            }
            for(int pn =0; pn < 12; pn++)
            {
                int start = pn < 7 ? hidden[pn] : 0;
                for(int i = start; i < currentGame.gamePiles[pn].Count; i++)
                {
                    gamePiles[pn].Add(currentGame.gamePiles[pn][i]);
                }
            }

        }

        public bool IsDeadEnd()
        {
            return IsWin() || IsLoss();
        }

        private bool IsLoss()
        {
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    
                    CardLocation cl = cardsLocation[moveTo[i]];
                    bool check = cl.pileNumber > 0?gamePiles[cl.pileNumber].Count <= cl.cardIndex:false;
                    //if (isVaidAction(cl.cardIndex, cl.pileNumber, pn, lastMoved))
                    if (cl.cardIndex != -1 && NotLoop(cl.pileNumber, cl.cardIndex, pn) && AddToList(moveTo[i], cl, pn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        internal bool IsWin()
        {
            return gamePiles[7].Count == 13 && gamePiles[8].Count == 13 && gamePiles[9].Count == 13 && gamePiles[10].Count == 13;
        }
        internal void Swap(NewAction a)
        {
            if(!gamePiles[a.fromPile].Contains(a.moveCard))
            {
                //find swapTo
                Card swapTo = a.moveCard;
                CardLocation swapToCL = cardsLocation[swapTo];
                int shouldBePile = a.movedShownPile;
                int shouldBeIndex = a.movedShownIndex;
                //find swapFrom
                Card swapFrom = shownOrder[shouldBePile][shouldBeIndex];
                CardLocation swapFromCL = cardsLocation[swapFrom];

                if(unUsed.Contains(swapTo))
                {
                    shownOrder[shouldBePile][shouldBeIndex] = swapTo;
                    unUsed.Remove(swapTo);
                    gamePiles[swapFromCL.pileNumber][swapFromCL.cardIndex] = swapTo;
                    shownUnusedOrder[swapTo] = new CardLocation(shouldBePile, shouldBeIndex);
                    cardsLocation[swapTo] = swapFromCL;

                    unUsed.Add(swapFrom);
                    shownUnusedOrder[swapFrom] = new CardLocation(-1, -1);
                    cardsLocation[swapFrom] = new CardLocation(-1, -1);

                }
                else
                {
                    //update cardPiles
                    gamePiles[swapToCL.pileNumber][swapToCL.cardIndex] = swapFrom;
                    gamePiles[swapFromCL.pileNumber][swapFromCL.cardIndex] = swapTo;
                    //update locations
                    cardsLocation[swapFrom] = swapToCL;
                    cardsLocation[swapTo] = swapFromCL;
                    //update shownorder
                    shownOrder[shouldBePile][shouldBeIndex] = swapTo;
                    shownOrder[shownUnusedOrder[swapTo].pileNumber][shownUnusedOrder[swapTo].cardIndex] = swapFrom;
                    //update shownunusedorder
                    CardLocation temp = shownUnusedOrder[swapTo];
                    shownUnusedOrder[swapTo] = shownUnusedOrder[swapFrom];
                    shownUnusedOrder[swapFrom] = temp;
                }

            }
            if (a.ontoCard != null && !gamePiles[a.toPile].Contains(a.ontoCard))
            {
                //find swapTo
                Card swapTo = a.ontoCard;
                CardLocation swapToCL = cardsLocation[swapTo];
                int shouldBePile = a.ontoShownPile;
                int shouldBeIndex = a.ontoShownIndex;
                //find swapFrom
                Card swapFrom = shownOrder[shouldBePile][shouldBeIndex];
                CardLocation swapFromCL = cardsLocation[swapFrom];

                if (unUsed.Contains(swapTo))
                {
                    shownOrder[shouldBePile][shouldBeIndex] = swapTo;
                    unUsed.Remove(swapTo);
                    gamePiles[swapFromCL.pileNumber][swapFromCL.cardIndex] = swapTo;
                    shownUnusedOrder[swapTo] = new CardLocation(shouldBePile, shouldBeIndex);
                    cardsLocation[swapTo] = swapFromCL;

                    unUsed.Add(swapFrom);
                    shownUnusedOrder[swapFrom] = new CardLocation(-1, -1);
                    cardsLocation[swapFrom] = new CardLocation(-1, -1);

                }
                else
                {
                    //update cardPiles
                    gamePiles[swapToCL.pileNumber][swapToCL.cardIndex] = swapFrom;
                    gamePiles[swapFromCL.pileNumber][swapFromCL.cardIndex] = swapTo;
                    //update locations
                    cardsLocation[swapFrom] = swapToCL;
                    cardsLocation[swapTo] = swapFromCL;
                    //update shownorder
                    shownOrder[shouldBePile][shouldBeIndex] = swapTo;
                    shownOrder[shownUnusedOrder[swapTo].pileNumber][shownUnusedOrder[swapTo].cardIndex] = swapFrom;
                    //update shownunusedorder
                    CardLocation temp = shownUnusedOrder[swapTo];
                    shownUnusedOrder[swapTo] = shownUnusedOrder[swapFrom];
                    shownUnusedOrder[swapFrom] = temp;
                }
            }
        }
        internal Card MakeMove(NewAction a)
        {
            ApplyMove(cardsLocation[a.moveCard].cardIndex, gamePiles[a.fromPile], gamePiles[a.toPile]);
            Card c = UpdateLocation(gamePiles[a.fromPile], gamePiles[a.toPile], cardsLocation[a.moveCard].cardIndex);
            pastGames.Add(gamePiles.GetHashCode());
            return c;
        }

        public Card UpdateLocation(CardPile fromPile, CardPile toPile, int index)
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
                    if (true)
                    {
                        pastGames = new HashSet<int>();
                        Random r = new Random();
                        int ri = r.Next(unUsed.Count);
                        c = unUsed[ri];
                        fromPile.Add(c);

                        cardsLocation[c].pileNumber = fromPile.pileNumber;
                        cardsLocation[c].cardIndex = 0;
                        unUsed.RemoveAt(ri);
                        

                        shownOrder[fromPile.pileNumber].Add( c);
                        shownUnusedOrder[c] = new CardLocation(fromPile.pileNumber, shownOrder[fromPile.pileNumber].Count-1);
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
        public List<NewAction> FindMoves()
        {
            List<NewAction> list = new List<NewAction>();
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    if (cl.cardIndex != -1 && NotLoop(cl.pileNumber, cl.cardIndex, pn) && AddToList(moveTo[i], cl, pn))
                    {
                        Card ontoCard = gamePiles[pn].Count != 0 ? gamePiles[pn][gamePiles[pn].Count - 1] : null;
                        int movedindex = moveTo[i] != null && shownUnusedOrder.ContainsKey(moveTo[i]) ? shownUnusedOrder[moveTo[i]].cardIndex : -1;
                        int movedpile = moveTo[i] != null && shownUnusedOrder.ContainsKey(moveTo[i]) ? shownUnusedOrder[moveTo[i]].pileNumber : -1;
                        int ontoindex = ontoCard != null && shownUnusedOrder.ContainsKey(ontoCard) ? shownUnusedOrder[ontoCard].cardIndex : -1;
                        int ontopile = ontoCard != null && shownUnusedOrder.ContainsKey(ontoCard) ? shownUnusedOrder[ontoCard].pileNumber : -1;

                        list.Add(new NewAction(moveTo[i],ontoCard , cl.pileNumber, pn,movedindex, movedpile, ontoindex , ontopile));
                    }
                }
            }
            return list;
        }
   
        private bool AddToList(Card card, CardLocation cl, int toPile)
        {
            if (cl.pileNumber < 7)
            {
                if (toPile < 7)
                {
                    return CheckFieldtoField(card, cl, toPile);
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
                    return CheckStacktoField(card, cl, toPile);
                }
            }
            else
            {
                return DeckPlayable(cl.cardIndex);
            }
            return false;
        }
        private bool NotLoop(int pileFrom, int index, int pileTo)
        {
            if (pileFrom == pileTo)
            {
                return false;
            }
            if (pileFrom < 7 && index == 0 && hidden[pileFrom] > 0)
            {
                return true;
            }
            GamePiles afterGame = gamePiles.clone();
            ApplyMove(index, afterGame[pileFrom], afterGame[pileTo]);
            if (pastGames.Contains(afterGame.GetHashCode()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool CheckStacktoField(Card card, CardLocation cl, int toPile)
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
                if (FaceUp(j) && (clj.pileNumber < 7) ||
                    (clj.pileNumber < 11 && clj.pileNumber > 6 && gamePiles[clj.pileNumber].Count - 1 == clj.cardIndex)
                    || (clj.pileNumber == 11) && DeckPlayable(clj.cardIndex))
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
                if (FaceUp(j) && (clj.pileNumber < 7) ||
                    (clj.pileNumber < 11 && clj.pileNumber > 6 && gamePiles[clj.pileNumber].Count - 1 == clj.cardIndex)
                    || (clj.pileNumber == 11) && DeckPlayable(clj.cardIndex))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckFieldtoField(Card card, CardLocation cl, int toPile)
        {
            //Action temp = new Action(card, toPile, cl.pileNumber, gamePiles[toPile].Count !=0? gamePiles[toPile][gamePiles[toPile].Count - 1]:null);
            if (!FaceUp(card) || (card.value == 12 && cl.cardIndex == 0 && hidden[cl.pileNumber] == 0))
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                Card king = new Card(i, 12);
                CardLocation clk = cardsLocation[king];
                if (cl.cardIndex == 0)
                {
                    if (hidden[cl.pileNumber] > 0)
                    {
                        return true;
                    }
                    else if (clk.pileNumber == 11 && DeckPlayable(clk.cardIndex))
                    {
                        return true;
                    }
                    else if (clk.pileNumber < 7 && clk.pileNumber > -1 && hidden[clk.pileNumber] > 0)
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
                else if (FaceUp(reverse) &&
                    ((clr.cardIndex < 7) ||
                    (clr.pileNumber > 6 && clr.cardIndex < 11 && rank < reverse.value && gamePiles[clr.pileNumber].Count - 1 == clr.cardIndex) ||
                    (clr.pileNumber == 11 && DeckPlayable(clr.cardIndex))))
                {
                    return true;
                }

            }

            return false;

        }
        public bool FaceUp(Card card)
        {

            CardLocation cl = cardsLocation[card];
            if (cl.cardIndex == -1)
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
                return DeckPlayable(cl.cardIndex);
            }
        }
        public bool DeckPlayable(int index)
        {
            return index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || lastMoved == 0 && index == 0 || index == gamePiles[11].Count - 1;
        }
        private void ApplyMove(int index, CardPile fromPile, CardPile toPile)
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
    }
}
