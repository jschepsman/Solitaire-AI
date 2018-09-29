using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;

namespace visual_game
{
    public class OldGame
    {
        public GamePiles gamePiles;
        public int rank;
        public Dictionary<Card, CardLocation> cardsLocation;
        public List<Card> unUsed;
        public int lastMoved;
        

        HashSet<string> pastGames;

        public int[] hidden;

        public OldGame()
        {
            rank = 0;
            pastGames = new HashSet<string>();
            lastMoved = 24;
            gamePiles = new GamePiles();
            hidden = new int[] { 0, 1, 2, 3, 4, 5, 6, };
            unUsed = new List<Card>();
            cardsLocation = new Dictionary<Card, CardLocation>();
            initGame();
        }

        public OldGame(string gameString)
        {
            pastGames = new HashSet<string>();
            rank = 1;
            gamePiles = new GamePiles();
            hidden = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            string game = gameString;
            string[] firstSplit = game.Split('-');
            lastMoved = int.Parse(firstSplit[0]);
            unUsed = new List<Card>();
            cardsLocation = new Dictionary<Card, CardLocation>();
            for (int i = 1; i < 13; i++)
            {
                string[] pile = firstSplit[i].Split(':');
                if(pile[0] != "" && i < 8)
                {
                    hidden[i - 1] = int.Parse(pile[0]);
                }
                
                for (int j = 1; j < pile.Count(); j++)
                {
                    int suite = int.Parse(pile[j].Substring(0, 1));
                    int value;
                    if (pile[j].Count() == 2)
                    {
                        value = int.Parse(pile[j].Substring(1, 1));
                    }
                    else
                    {
                        value = int.Parse(pile[j].Substring(1, 2));
                    }
                    Card card = new Card(suite, value);
                    cardsLocation.Add(new Card(suite, value), new CardLocation(-1, -1));
                    gamePiles[i - 1].Add(card);
                }
            }
            int lowCount = rank;
            for(int i = 7; i <11; i++)
            {
                if(lowCount> gamePiles[i].Count && gamePiles[i].Count>2)
                {
                    lowCount = gamePiles[i].Count;
                }
            }
            rank = lowCount;
            scan();
        }

        public void initGame()
        {
            for(int suite =0; suite < 4; suite++)
            {
                for (int value =0; value < 13; value++)
                {
                    cardsLocation.Add(new Card(suite, value), new CardLocation(-1, -1));
                    unUsed.Add(new Card(suite, value));
                }
            }
            shuffle(unUsed);
            deal();
            scan();

        }
        public void shuffle(List<Card> pile)
        {
            Random ran = new Random();
            for(int i = pile.Count-1; i > 0; i--)
            {
                int j = ran.Next(i);
                Card temp = pile[i];
                pile[i] = pile[j];
                pile[j] = temp;
            }
        }
        public void deal()
        {
            for(int i =0; i < 7; i++)
            {
                for(int j = i+1; j>0; j--)
                {
                    gamePiles[i].Add(unUsed[0]);
                    cardsLocation[unUsed[0]].pileNumber = i;
                    cardsLocation[unUsed[0]].cardIndex = j-1;
                    unUsed.RemoveAt(0);
                }
            }
            int unUsedCount = unUsed.Count;
            for (int i =0; i < unUsedCount; i++)
            {
                gamePiles[11].Add(unUsed[0]);
                cardsLocation[unUsed[0]].pileNumber = 11;
                cardsLocation[unUsed[0]].cardIndex = gamePiles[11].Count - 1;
                unUsed.RemoveAt(0);
                
            }
        }
        public void scan()
        {
            for(int pn=0; pn<12; pn++)
            {
                for(int pi =0; pi < gamePiles[pn].Count; pi++)
                {
                    CardLocation temp = cardsLocation[gamePiles[pn][pi]];
                    temp.pileNumber = pn;
                    temp.cardIndex = pi;
                }
            }
        }
        public override string ToString()
        {
            string finalString = lastMoved.ToString();
            for(int pn =0; pn <12; pn++)
            {
                finalString += "-";
                if(pn<7)
                {
                    finalString += hidden[pn].ToString();
                }
                foreach(Card c in gamePiles[pn])
                {
                    finalString += ":" + c.ToString();
                }
            }
            return finalString;
        }

        //public List<Move> findMoves()
        //{
        //    List<Move> list = new List<Move>();
        //    for(int pn = 0;pn<11;pn++)
        //    {
        //        Card[] moveTo = gamePiles[pn].cardsOnto();
        //        for (int i = 0; i < moveTo.Count(); i++)
        //        {
        //            CardLocation cl = cardsLocation[moveTo[i]];
        //            if (isVaidMove(cl.cardIndex, cl.pileNumber, pn, lastMoved))
        //            {
        //                list.Add(new Move(cl.cardIndex, cl.pileNumber, pn));
        //            }
        //        }
        //    }
        //    return list;
        //}
        //public bool isVaidMove(int index, int pileFrom, int pileTo, int lastMoved)
        //{

        //    if (pileFrom == pileTo)
        //    {
        //        return false;
        //    }

        //    if(pileTo < 7)
        //    {
        //        //moves to the field
        //        //move from field
        //        if(pileFrom < 7)
        //        {
        //            if(gamePiles[pileFrom][index].value == 12 && index == 0)
        //            {
        //                return false;
        //            }
        //            else if(hidden[pileFrom] <= index && notLoop(pileFrom, index, pileTo))
        //            {
        //                return true;
        //            }
                    
        //        }
        //        //move from found
        //        else if (pileFrom < 11)
        //        {
        //            if(index == gamePiles[pileFrom].Count-1 && gamePiles[pileFrom].Count - 1> rank && notLoop(pileFrom, index, pileTo))
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            if (index % 3 == 2 || (index >= lastMoved-1 && index % 3 == (lastMoved - 1) % 3) || index == gamePiles[pileFrom].Count - 1 || lastMoved == 0 && index == 0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //moves to the found
        //        //from the field
        //        if (pileFrom < 7)
        //        {
                    
        //            if (hidden[pileFrom] <= index && gamePiles[pileFrom].Count-1 == index && notLoop(pileFrom, index, pileTo))
        //            {
        //                return true;
        //            }
                    
        //        }
        //        //from deck
        //        else
        //        {
        //            if (index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || index == gamePiles[pileFrom].Count - 1|| lastMoved == 0&& index ==0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //private bool notLoop(int pileFrom, int index, int pileTo)
        //{
        //    if(pileFrom < 7 && index == hidden[pileFrom])
        //    {
        //        return true;
        //    }
        //    if(pileTo < 11 && pileTo > 6 && rank >= gamePiles[pileFrom][index].value)
        //    {
        //        return true;
        //    }
        //    GamePiles afterGame = gamePiles.clone();
        //    applyMove(index, afterGame[pileFrom], afterGame[pileTo]);
        //    if (pileFrom < 7 && afterGame[pileFrom].Count == hidden[pileFrom] && hidden[pileFrom] > 0)
        //    {
        //        return true;
        //    }
        //    foreach(Card c in afterGame[pileTo].cardsOnto())
        //    {
        //        if(gamePiles[11].Contains(c))
        //        {
        //            CardLocation cl = cardsLocation[c];
        //            return isVaidMove(cl.cardIndex, cl.pileNumber, pileTo, lastMoved);
        //        }
        //    }
        //    if (pastGames.Contains(afterGame.ToString()))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        public bool endOfGame()
        {
            return isLoss() || isWin();
        }
        public bool isOldLoss()
        {
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    bool check1 = valid1stMoves(moveTo[i], pn);
                    bool check2 = checkMove(moveTo[i], pn); //true; // checkMove(moveTo[i], pn);
                    if (check1 && check2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool isLoss()
        {
            return lossCheck1();// || lossCheck2();
            
        }
        public bool lossCheck1()
        {
            Dictionary<Card, bool> movable = new Dictionary<Card, bool>();
            Dictionary<Card, bool> buildable = new Dictionary<Card, bool>();
            Dictionary<Card, bool> mCanidateable = new Dictionary<Card, bool>();
            Dictionary<Card, bool> suiteable = new Dictionary<Card, bool>();
            bool openStack = false;
            CardLocation cl;
            for (int i = 0; i < 7; i++)
            {
                if (gamePiles[i].Count == 0) openStack = true;
            }
            for (int suite = 0; suite < 4; suite++)
            {
                for (int value = 0; value < 13; value++)
                {
                    Card card = new Card(suite, value);
                    Card build1 = new Card(card.suite % 2 != 0 ? 0 : 1, card.value + 1);
                    Card build2 = new Card(card.suite % 2 != 0 ? 2 : 3, card.value + 1);
                    cl = cardsLocation[card];
                    bool isTopCard = gamePiles[cl.pileNumber].Count - 1 == cl.cardIndex;
                    bool isBuildCard = (cl.pileNumber < 7);
                    bool isSuiteCard = cl.pileNumber == card.suite + 7;
                    bool isDeck = cl.pileNumber == 11;

                    if (isBuildCard && isTopCard)
                    {
                        addedToDict(mCanidateable, card, true);
                        addedToDict(buildable, card, true);
                    }
                    //if (isBuildCard && !isTopCard && faceUp(card) && ((card.value == 12 && cl.cardIndex != 0) || (card.value != 12 && checkTopFieldPiles(build1, build2, cl.pileNumber))))
                    //{
                    //    addedToDict(mCanidateable, card, true);
                    //}
                    if (isBuildCard && faceUp(card))
                    {
                        addedToDict(mCanidateable, card, true);
                    }
                    if (isSuiteCard)
                    {
                        addedToDict(suiteable, card, true);
                        if (isTopCard)
                        {
                            addedToDict(mCanidateable, card, true);
                        }
                    }
                    if (isDeck && deckPlayable(cl.cardIndex))
                    {
                        addedToDict(mCanidateable, card, true);
                    }

                }
            }
            bool changed = true;
            int count = 0;
            int[] lowestIndex = new int[] { 26, 26 };
            while (changed)
            {
                changed = false;
                count++;
                for (int suite = 0; suite < 4; suite++)
                {
                    for (int value = 0; value < 13; value++)
                    {
                        Card card = new Card(suite, value);
                        cl = cardsLocation[card];
                        bool isTopCard = gamePiles[cl.pileNumber].Count - 1 == cl.cardIndex;
                        Card cardAbove = null;
                        if (!isTopCard)
                        {
                            cardAbove = gamePiles[cl.pileNumber][cl.cardIndex + 1];
                        }
                        bool isBuildStack = cl.pileNumber < 7 || inDict(buildable, card);
                        bool isDeck = cl.pileNumber == 11;
                        bool isSuitCard = (cl.pileNumber == card.suite + 7 || suiteable.ContainsKey(card));
                        Card suiteableCard = new Card(card.suite, card.value - 1);
                        Card build1 = new Card(card.suite % 2 != 0 ? 0 : 1, card.value + 1);
                        Card build2 = new Card(card.suite % 2 != 0 ? 2 : 3, card.value + 1);
                        CardLocation clB1 = null;
                        CardLocation clB2 = null;
                        bool isBuild1Top = false;
                        bool isBuild2Top = false;
                        if (build1.value <= 12)
                        {
                            clB1 = cardsLocation[build1];
                            clB2 = cardsLocation[build2];
                            isBuild1Top = clB1.cardIndex == gamePiles[clB1.pileNumber].Count - 1;
                            isBuild2Top = clB2.cardIndex == gamePiles[clB2.pileNumber].Count - 1;
                        }


                        //  *there are two ways a card can become suit-stackable
                        //      1.An ace is a move - candiate( case 1)
                        //      2.A non - ace is a move - candidate whose suit-card is suit - stackable
                        //            and is either not in the build stack, or whose card_above is movable ( case 2 )

                        if (!inDict(suiteable, card) &&
                            inDict(mCanidateable, card) &&
                            (card.value == 0 ||
                            (inDict(suiteable, suiteableCard) &&
                            (!(cl.pileNumber < 7) || isTopCard || (!isTopCard && inDict(movable, cardAbove))))))
                        {
                            addedToDict(suiteable, card, true);
                            changed = true;
                        }

                        //*there are four ways a card can become movable
                        //  *1.An ace is a move - candidate(case 1)
                        //  * 2.A non - ace is a move - candidate( in a build stack and is either the top card or it's card_above is movable )
                        //   * or in the deck and its suit - card is suit - stackable(case 2)
                        //  * 3.A king is a move - candidate, and open - stack is true(case 3)
                        //  * 4.A non - king is a move - candidate, and one of its build cards is buildable (case 4)
                        if (!inDict(movable, card) && inDict(mCanidateable, card) &&
                            (card.value == 0 ||
                            (isDeck && inDict(suiteable, suiteableCard)) ||
                            (isBuildStack && inDict(suiteable, suiteableCard) && (isTopCard || inDict(movable, cardAbove))) ||
                            (card.value == 12 && openStack) ||
                            (card.value != 12 && (
                              ((inDict(buildable, build1) && (clB1.pileNumber == 11 || (isBuild1Top || inDict(movable, gamePiles[clB1.pileNumber][clB1.cardIndex + 1])))) ||
                              (inDict(buildable, build2) && (clB2.pileNumber == 11 || (isBuild2Top || inDict(movable, gamePiles[clB2.pileNumber][clB2.cardIndex + 1])))))))))
                        {
                            changed = true;
                            addedToDict(movable, card, true);
                            if (isDeck)
                            {
                                lowestIndex[1] = cl.cardIndex < lowestIndex[1] ? cl.cardIndex : lowestIndex[1];
                                lowestIndex[0] = lowestIndex[1] < lowestIndex[0] ? (lowestIndex[1] + lowestIndex[0]) - (lowestIndex[1] = lowestIndex[0]) : lowestIndex[0];
                            }
                        }

                        //*there are nine ways a card can become a move - candidate
                        //  * 1.A card in the deck is the last card in the deck (case 1)
                        //  *2.A card in the deck whose(zero - based) location in the deck MOD 3 is 2(case 2)
                        //  * 3.A card in the deck whose(zero - based) location in the deck MOD 3 is 0
                        //    * and at least one card before it is movable (case 3)
                        //  *4.A card in the deck whose(zero - based) location in the deck MOD 3 is 1
                        //     * and at least two cards before it is movable (case 4)
                        //	*5.A card in the deck and the card directly above it is movable (case 5)
                        //	*6.A card is in a build stack and is the top card in that stack (case 6)
                        //	*7.A card is in a build stack and the card above it is movable (case 7)
                        //  *8.A card is in a suit stack and is the top card in that stack (case 8)
                        //  *9.A card is in a suit stack and the cards above it is movable (case 9)
                        if (!inDict(mCanidateable, card) &&
                                ((isDeck && isTopCard) || //1
                                (isDeck && cl.cardIndex % 3 == 2) || //2
                                (isDeck && cl.cardIndex % 3 == 0 && (lowestIndex[0] < cl.cardIndex || lowestIndex[1] < cl.cardIndex)) ||  //3
                                (isDeck && cl.cardIndex % 3 == 1 && (lowestIndex[0] < cl.cardIndex && lowestIndex[1] < cl.cardIndex)) || //4
                                (isDeck && cardAbove.value >= 0 && inDict(movable, cardAbove)) || //5
                                (isBuildStack && isTopCard) || //6
                                (isBuildStack && cardAbove != null && inDict(movable, cardAbove)) || //7
                                (isSuitCard && isTopCard) ||//8
                                (isSuitCard && cardAbove != null && inDict(movable, card))))//9
                        {
                            changed = true;
                            addedToDict(mCanidateable, card, true);
                        }
                        if (!inDict(buildable, card) &&
                            ((isBuildStack && isTopCard) ||
                            (isBuildStack && cardAbove != null && inDict(movable, cardAbove)) ||
                            (inDict(mCanidateable, card) && card.value == 12 && openStack && !inDict(buildable, card)) ||
                            (inDict(mCanidateable, card) && card.value != 12 && (inDict(buildable, build1) || inDict(buildable, build2)))))
                        {
                            if(isBuildStack)
                            { 
                                if((isTopCard || inDict(movable, cardAbove)))
                                { 
                                    changed = true;
                                    addedToDict(buildable, card, true);
                                }
                                
                            }
                            else
                            {
                                changed = true;
                                addedToDict(buildable, card, true);
                            }

                        }
                        if (!openStack &&
                                ((gamePiles[0].Count != 0 && inDict(movable, gamePiles[0][0])) ||
                                (gamePiles[1].Count != 0 && inDict(movable, gamePiles[1][0])) ||
                                (gamePiles[2].Count != 0 && inDict(movable, gamePiles[2][0])) ||
                                (gamePiles[3].Count != 0 && inDict(movable, gamePiles[3][0])) ||
                                (gamePiles[4].Count != 0 && inDict(movable, gamePiles[4][0])) ||
                                (gamePiles[5].Count != 0 && inDict(movable, gamePiles[5][0])) ||
                                (gamePiles[6].Count != 0 && inDict(movable, gamePiles[6][0]))))
                        {
                            openStack = true;
                            changed = true;
                        }
                    }
                }
            }
            if (suiteable.Count != 52)
            {
                return true;
            }
            return false;
        }

        private bool checkTopFieldPiles(Card build1, Card build2, int pileNum)
        {
            for(int i = 0; i < 7; i++ )
            {
                if(i!=pileNum && gamePiles[i].Count > 0)
                {
                    if(gamePiles[i][gamePiles[i].Count-1].Equals(build1)|| gamePiles[i][gamePiles[i].Count - 1].Equals(build2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool lossCheck2()
        {
           
            bool twinInSameStack;
            bool onSuitBlocked;
            bool twinOnSuiteBlocked;
            bool offSuitBlocked;
            CardLocation clTwin;
            for (int i = 0; i < 7; i++)
            {
                twinInSameStack = false;
                onSuitBlocked = false;
                twinOnSuiteBlocked = false;
                offSuitBlocked = false;
                for (int j = 1; j < gamePiles[i].Count; j++)
                {
                    Card card = gamePiles[i][j];
                    CardLocation cl = cardsLocation[card];
                    
                    Card twin = new Card((card.suite + 2) % 4, card.value);
                    cl = cardsLocation[card];
                    clTwin = cardsLocation[twin];
                    for (int k = j - 1; k >= 0; k--)
                    {
                        Card currCard = gamePiles[i][k];
                        Card build1 = new Card(card.suite % 2 != 0 ? 0 : 1, card.value - 1);
                        Card build2 = new Card(card.suite % 2 != 0 ? 2 : 3, card.value - 1);
                        if (currCard.Equals(twin)) twinInSameStack = true;
                        if (currCard.suite == card.suite && currCard.value < card.value) onSuitBlocked = true;
                        if (twinInSameStack && currCard.suite == twin.suite && currCard.value < twin.value) twinOnSuiteBlocked = true;
                        if (build1.Equals(currCard) || build2.Equals(currCard))
                        {
                            if (!faceUp(currCard) && twinInSameStack) offSuitBlocked = true;
                        }

                    }
                    if (offSuitBlocked && twinOnSuiteBlocked && onSuitBlocked)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool isNewLoss()
        {

            CardLocation cl;
            bool twinInSameStack;
            bool onSuitBlocked;
            bool twinOnSuiteBlocked;
            bool offSuitBlocked;
            CardLocation clTwin;
            for (int i = 0; i < 7; i++)
            {
                twinInSameStack = false;
                onSuitBlocked = false;
                twinOnSuiteBlocked = false;
                offSuitBlocked = false;
                for (int j = 1; j < gamePiles[i].Count; j++)
                {
                    Card card = gamePiles[i][j];
                    Card twin = new Card((card.suite + 2) % 4, card.value);
                    cl = cardsLocation[card];
                    clTwin = cardsLocation[twin];
                    for (int k = j - 1; k >= 0; k--)
                    {
                        Card currCard = gamePiles[i][k];
                        if (currCard.Equals(twin)) twinInSameStack = true;
                        if (currCard.suite == card.suite && currCard.value < card.value) onSuitBlocked = true;
                        if (onSuitBlocked && currCard.suite == twin.suite && currCard.value < twin.value) twinOnSuiteBlocked = true;
                        Card build1 = new Card(card.suite % 2 != 0 ? 0 : 1, card.value - 1);
                        Card build2 = new Card(card.suite % 2 != 0 ? 2 : 3, card.value - 1);
                        if (build1.Equals(currCard) || build2.Equals(currCard))
                        {
                            if (!faceUp(currCard) && twinInSameStack) offSuitBlocked = true;
                        }

                    }
                    if (offSuitBlocked && twinOnSuiteBlocked && onSuitBlocked)
                    {
                        return true;
                    }
                }
            }
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    cl = cardsLocation[moveTo[i]];
                    if (addToList(moveTo[i], cl, pn))
                    {
                        return false;
                    }
                }
            }
            return true;

        }
        public void addedToDict(Dictionary<Card, bool> dict, Card card, bool value)
        {
            if(dict.ContainsKey(card))
            {
                dict[card] = value;
            }
            else
            {
                dict.Add(card, value);
            }
        }
        public bool inDict(Dictionary<Card,bool> dict, Card card)
        {
            if (dict.ContainsKey(card))
            {
                return dict[card];
            }
            else
            {
                return false;
            }
        }
        public bool isWin()
        {
            return gamePiles[7].Count == 13 && gamePiles[8].Count == 13 && gamePiles[9].Count == 13 && gamePiles[10].Count == 13;
        }

        private void applyMove(int index, CardPile fromPile, CardPile toPile)
        {
            if(fromPile.pileNumber < 7)
            {
                while(fromPile.Count > index)
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
        public void makeMove(Move move)
        {
            applyMove(move.index, gamePiles[move.from], gamePiles[move.to]);
            updateLocation(gamePiles[move.from], gamePiles[move.to], move.index);
            pastGames.Add(gamePiles.ToString());
        }
        public void updateLocation(CardPile fromPile, CardPile toPile, int index)
        {
            if(fromPile.pileNumber ==11)
            {
                lastMoved = index;
                for(int i = index; i<fromPile.Count; i++)
                {
                    cardsLocation[fromPile[i]].cardIndex = i;
                }
                pastGames = new HashSet<string>();
            }
            else if (fromPile.pileNumber< 7)
            {
                if(hidden[fromPile.pileNumber] == fromPile.Count && hidden[fromPile.pileNumber] != 0)
                {
                    pastGames = new HashSet<string>();
                    hidden[fromPile.pileNumber]--;
                }
            }
            for(int i = 0; i < toPile.Count; i ++)
            {
                cardsLocation[toPile[i]].cardIndex = i;
                cardsLocation[toPile[i]].pileNumber = toPile.pileNumber;
            }
            if(toPile.pileNumber > 6)
            {
                if(gamePiles[7].Count > rank && gamePiles[8].Count > rank && gamePiles[9].Count > rank && gamePiles[10].Count>rank)
                {
                    rank++;
                    pastGames = new HashSet<string>();
                }
            }

        }

        public List<Move> newFindMoves()
        {

            List<Move> list = new List<Move>();
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    bool check1 = valid1stMoves(moveTo[i], pn);
                    bool check2 = checkMove(moveTo[i], pn); //true; // checkMove(moveTo[i], pn);
                    if (check1 && check2)
                    {
                        list.Add(new Move(cl.cardIndex, cl.pileNumber, pn));
                    }
                }
            }
            return list;
        }
        public bool valid1stMoves(Card card, int toGamePile)
        {
            CardLocation cl = cardsLocation[card];
            if(toGamePile < 7 &&
                ((cl.pileNumber == 11 && deckPlayable(cl.cardIndex))||
                (cl.pileNumber > 6 && stackPlayable(cl.pileNumber,cl.cardIndex)  && card.value >= rank && (card.value != 0) && card.value !=1)  ||
                (cl.pileNumber < 7&&faceUp(card))
                 )
              )
            {
                if(card.value == 12 
                    && (cl.pileNumber == 11 || cl.cardIndex != 0) )
                {
                    return true;
                }
                else if (card.value != 12 && card.value != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Card j = new Card(i == 0 ? (card.suite + 1) % 4 : (card.suite + 3) % 4, card.value + 1);
                        CardLocation clj = cardsLocation[j];
                        if (clj.pileNumber < 7 &&
                            gamePiles[clj.pileNumber][gamePiles[clj.pileNumber].Count - 1].Equals(j) &&
                            clj.pileNumber != cl.pileNumber )
                        {
                            return true;
                        }
                    }
                }
            }
            if(cl.pileNumber == 11 && deckPlayable(cl.cardIndex)|| (cl.pileNumber < 7 && cl.cardIndex == gamePiles[cl.pileNumber].Count-1))
            {
                if(card.value == 0)
                {
                    return true;
                }
                else if(gamePiles[toGamePile].Count > 0 &&gamePiles[toGamePile][gamePiles[toGamePile].Count-1].value+1 == card.value)
                {
                    return true;
                }
            }

            return false;
        }

        public bool checkMove(Card card, int toGamePile)
        {
            if(card.value ==0 && toGamePile <7)
            {
                return false;
            }
            CardLocation cl = cardsLocation[card];
            bool meaningless = false;
            // there are two types of meaningless actions (actions that can be trivially undone):
            // 1. moving a card from one face-up build stack card to another
            if(cl.pileNumber < 7 && toGamePile < 7 && faceUp(card) && hidden[cl.pileNumber] != cl.cardIndex)
            {
                meaningless = true;
            }
            // 2. moving a card down from a suit stack to a build stack
            else if(toGamePile < 7 && cl.pileNumber > 6 && cl.pileNumber < 11)
            {
                meaningless = true;
            }

            // generate secondary actions (and mark them)

            // there four different types of secondary actions -
            // each based on a primary action that is now marked as not available
            //  if a primary card is marked unavailable because it's moving back and forth on the build stacks
            //		1.  the card under the primary card may be able to be placed in a suit stack
            //		2.  we may want to place a different card on the card under the 
            //			primary card (same color and value as the primary)
            //  if a primary card is marked unavailable because it's moving from a suit stack to a build stack
            //		3.  we may want to place a card on the primary once it's in the build stack
            //		4.  we may also want to move the card under the primary from a suit stack to a build stack
            if(meaningless)
            {
                int underIndex = cl.cardIndex - 1;
                if(cl.pileNumber < 7)
                {
                    if(underIndex >= 0)
                    {
                        Card under = gamePiles[cl.pileNumber][underIndex];
                        Card twin = new Card(card.suite > 1 ? card.suite % 2 : card.suite + 2, card.value);

                        if (under.value == gamePiles[under.suite+7].Count)
                        {
                            return true;
                        }
                        else if ((faceUp(twin) && cardsLocation[twin].pileNumber < 6) ||
                            (cardsLocation[twin].pileNumber == 11 && deckPlayable(cardsLocation[twin].cardIndex))||
                            (cardsLocation[twin].pileNumber< 11 && cardsLocation[twin].pileNumber > 6 
                            && cardsLocation[twin].cardIndex == gamePiles[twin.suite+7].Count-1))
                        {
                            return true;
                        }
                    }

                }
                else if (cl.pileNumber < 11 && cl.pileNumber > 6)
                {
                    Card build_1 = new Card(card.suite % 2 != 0 ? 0 : 1, card.value - 1);
                    Card build_2 = new Card(card.suite % 2 != 0 ? 2 : 3, card.value - 1);
                    CardLocation cl1 = cardsLocation[build_1];
                    CardLocation cl2 = cardsLocation[build_2];
                    if (build_1.value != 0 && build_1.value != 12 &&
                        (faceUp(build_1) && cl1.pileNumber < 7) ||
                        (cl1.pileNumber == 11 && deckPlayable(cl1.cardIndex))||
                        (cl1.pileNumber < 11 && cl1.pileNumber > 6 && gamePiles[build_1.suite+7].Count-1 == cl1.cardIndex))
                    {
                        return true;
                    }
                    else if (build_2.value != 0 && build_2.value != 12 &&
                        (faceUp(build_2) && cl2.pileNumber < 7) ||
                        (cl2.pileNumber == 11 && deckPlayable(cl2.cardIndex)) ||
                        (cl2.pileNumber < 11 && cl2.pileNumber > 6 && gamePiles[build_2.suite + 7].Count - 1 == cl2.cardIndex))
                    {
                        return true;
                    }
                    else
                    {
                        if(underIndex > 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Card under = gamePiles[cl.pileNumber][underIndex];
                                Card j = new Card(i == 0 ? (under.suite + 1) % 4 : (under.suite + 3) % 4, under.value - 1);
                                if(j.value != 0)
                                {
                                    CardLocation clj = cardsLocation[j];
                                    if(clj.pileNumber < 6 && cardsLocation[j].cardIndex == gamePiles[clj.pileNumber].Count - 1)
                                    {
                                        return true;
                                    }

                                }
                            }
                        }
                        else
                        {
                            for(int i = 0; i < 4; i++)
                            {
                                Card j = new Card(i, 12);
                                //12 not already in an empty stop or on top
                                CardLocation clj = cardsLocation[j];
                                if(clj.pileNumber == 11 && deckPlayable(clj.cardIndex)|| 
                                    (clj.cardIndex < 7 && clj.cardIndex != 0))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }
        public bool deckPlayable(int index)
        {
            return index % 3 == 2 || (index >= lastMoved - 1 && index % 3 == (lastMoved - 1) % 3) || lastMoved == 0 && index == 0|| index == gamePiles[11].Count-1; 
        }
        public bool stackPlayable(int pileNumber, int index)
        {
            return gamePiles[pileNumber].Count - 1 == index && gamePiles[pileNumber][gamePiles[pileNumber].Count - 1].value >= rank;
        }
        public bool faceUp(Card card)
        {
            CardLocation cl = cardsLocation[card];
            if (cl.pileNumber < 7)
            {
                return hidden[cl.pileNumber] <= cl.cardIndex;
            }
            else if (cl.pileNumber < 11 )
            {
                return cl.cardIndex == gamePiles[cl.pileNumber].Count - 1 && card.value >= rank;
            }
            else
            {
                return deckPlayable(cl.cardIndex);
            }
        }
        public bool emptyStack()
        {
            for(int i = 0; i < 7; i++)
            {
                if(gamePiles[i].Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //new tests
        public List<Move> FindMoveList()
        {

            List<Move> list = new List<Move>();
            for (int pn = 0; pn < 11; pn++)
            {
                Card[] moveTo = gamePiles[pn].cardsOnto();
                for (int i = 0; i < moveTo.Count(); i++)
                {
                    CardLocation cl = cardsLocation[moveTo[i]];
                    if (addToList(moveTo[i], cl, pn))
                    {
                        list.Add(new Move(cl.cardIndex, cl.pileNumber, pn));
                    }
                }
            }
            return list;
        }
        public bool addToList(Card card, CardLocation cl, int toPile)
        {
            if (cl.pileNumber < 7)
            {
                if(toPile < 7)
                {
                    return checkFieldtoField(card, cl, toPile);
                }
                else
                {
                    return gamePiles[cl.pileNumber].Count-1 == cl.cardIndex;
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
            if(card.value < rank+1 || cl.cardIndex != gamePiles[cl.pileNumber].Count-1)
            {
                return false;
            }
            //  if a primary card is marked unavailable because it's moving from a suit stack to a build stack
            //		3.  we may want to place a card on the primary once it's in the build stack
            for (int i =0; i < 2; i++)
            {
                Card j = new Card(i == 0 ? (card.suite + 1) % 4 : (card.suite + 3) % 4, card.value - 1);
                CardLocation clj = cardsLocation[j];
                if(faceUp(j)&&(clj.pileNumber<7)||
                    (clj.pileNumber < 11 && clj.pileNumber>6&&gamePiles[clj.pileNumber].Count -1 == clj.cardIndex)
                    ||(clj.pileNumber == 11) && deckPlayable(clj.cardIndex))
                {
                    return true;
                }
            }

            //		4.  we may also want to move the card under the primary from a suit stack to a build stack
            Card under = gamePiles[cl.pileNumber][cl.cardIndex-1];
            CardLocation clunder = cardsLocation[under];
            for (int i = 0; i < 2; i++)
            {
                if(under.value > rank)
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

            }
                return false;
        }

        private bool checkFieldtoField(Card card, CardLocation cl, int toPile)
        {

            if(!faceUp(card) || card.value == 12 && cl.cardIndex == 0)
            {
                return false;
            }
            if (hidden[cl.pileNumber] == cl.cardIndex)
            {
                return true;
            }
            int underIndex = cl.cardIndex - 1;
            if (underIndex >= 0)
            {
                //check if the card can be moved to stack
                Card under = gamePiles[cl.pileNumber][underIndex];
                CardLocation clunder = cardsLocation[under];
                Card reverse = new Card((card.suite+2)%4, card.value);
                CardLocation clr = cardsLocation[reverse];
                //		1.  the card under the primary card may be able to be placed in a suit stack
                if(gamePiles[under.suite+7].Count == under.value)
                {
                    return true;
                }
                //		2.  we may want to place a different card on the card under the 
                //           primary card (same color and value as the primary)
                else if (faceUp(reverse)&&
                    ((clr.pileNumber < 7&& (clr.cardIndex != 0 || (clr.cardIndex == 0&& kingMove()) ) ) ||
                    (clr.pileNumber > 6&&clr.cardIndex < 11 && rank+1 < reverse.value && gamePiles[clr.pileNumber].Count-1 == clr.cardIndex)||
                    (clr.pileNumber ==11 && deckPlayable(clr.cardIndex))))
                {
                    return true;
                }

            }

            return false;
            
        }

        private bool kingMove()
        {
            bool openPile = false;
            for(int pileNum =0; pileNum<7;pileNum++)
            {
                if(gamePiles[pileNum].Count == 0)
                {
                    openPile = true;
                }
            }
            if(!openPile)
            {
                for (int i = 0; i < 4; i++)
                {
                    Card king = new Card(i, 12);
                    CardLocation cl = cardsLocation[king];
                    if (cl.pileNumber == 11)
                    {
                        if (deckPlayable(cl.cardIndex))
                        {
                            return true;
                        }
                    }
                    else if (cl.pileNumber > 6)
                    {
                        if (rank < 12)
                        {
                            return true;
                        }

                    }
                    else
                    {
                        if (cl.cardIndex != 0 && faceUp(king)) 
                        {

                            return true;
                        }

                    }
                }
            }

            return false;
        }
    }


}
