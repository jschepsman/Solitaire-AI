using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;

namespace visual_game
{
    public class UCTGame
    {
        public GamePiles gamePiles;
        private int rank;
        public Dictionary<Card, CardLocation> cardsLocation;
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

        public UCTGame(OldGame game)
        {
            rank = game.rank;
            unUsed = new List<Card>();
            gamePiles = new GamePiles();
            hidden = new int[] {game.hidden[0], game.hidden[1], game.hidden[2], game.hidden[3], game.hidden[4]
                , game.hidden[5], game.hidden[6] };
            cardsLocation = new Dictionary<Card, CardLocation>();
            //load all seen cards into piles/locations
            //loads all unseen cards into unseen list
            for(int pi = 0; pi< 7; pi++ )
            {
                for(int ci = 0; ci<hidden[pi]; ci++)
                {
                    unUsed.Add(game.gamePiles[pi][ci]);
                    cardsLocation.Add(game.gamePiles[pi][ci], new CardLocation(-1, -1));
                }
                for(int ci = hidden[pi]; ci< game.gamePiles[pi].Count;ci++)
                {
                    gamePiles[pi].Add(game.gamePiles[pi][ci]);
                    cardsLocation.Add(game.gamePiles[pi][ci], new CardLocation(pi, ci-hidden[pi]));
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

        public List<Move> findMoves()
        {
            List<Move> list = new List<Move>();
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    if (isVaidMove(cl.cardIndex, cl.pileNumber, pn, lastMoved))
                    {
                        list.Add(new Move(cl.cardIndex, cl.pileNumber, pn));
                    }
                }
            }
            return list;
        }
        public bool isVaidMove(int index, int pileFrom, int pileTo, int lastMoved)
        {
            if(index == -1)
            {
                return false;
            }
            if (pileFrom == pileTo)
            {
                return false;
            }

            if (pileTo < 7)
            {
                //moves to the field
                //move from field
                if (pileFrom < 7)
                {
                    if (gamePiles[pileFrom][index].value == 12 && index == 0 && hidden[pileFrom] == 0)
                    {
                        return false;
                    }
                    else if (notLoop(pileFrom, index, pileTo))
                    {
                        return true;
                    }

                }
                //move from found
                else if (pileFrom < 11)
                {
                    if (index == gamePiles[pileFrom].Count - 1 && gamePiles[pileFrom].Count - 1 > rank && notLoop(pileFrom, index, pileTo))
                    {
                        return true;
                    }
                }
                else
                {
                    if (index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || index == gamePiles[pileFrom].Count - 1 || lastMoved == 0 && index == 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                //moves to the found
                //from the field
                if (pileFrom < 7)
                {

                    if (gamePiles[pileFrom].Count - 1 == index && notLoop(pileFrom, index, pileTo))
                    {
                        return true;
                    }

                }
                //from deck
                else
                {
                    if (index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || index == gamePiles[pileFrom].Count - 1 || lastMoved == 0 && index == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool notLoop(int pileFrom, int index, int pileTo)
        {
            if (pileFrom < 7 && index == 0 && hidden[pileFrom] >0)
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
        public bool isLoss()
        {
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    if (isVaidMove(cl.cardIndex, cl.pileNumber, pn, lastMoved))
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
        public Card makeMove(Move move, Card card)
        {
            applyMove(move.index, gamePiles[move.from], gamePiles[move.to]);
            Card c =updateLocation(gamePiles[move.from], gamePiles[move.to], move.index, card);
            pastGames.Add(gamePiles.GetHashCode());
            return c;
        }
        public Card makeMove(Move move)
        {
            applyMove(move.index, gamePiles[move.from], gamePiles[move.to]);
            Card c = updateLocation(gamePiles[move.from], gamePiles[move.to], move.index, null);
            pastGames.Add(gamePiles.GetHashCode());
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
                    if(shownCard == null)
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
    }
}
