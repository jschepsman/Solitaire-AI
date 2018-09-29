using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
namespace visual_game
{
    public class Action: IEquatable<Action>
    {
        public Card ontoCard;
        public int newStack;
        public int oldStack;
        public Card card;
        public CardInfo ontoCardInfo;
        public CardInfo cardinfo;

        public Action(Card movedCard, int stackNew, int stackOld, Card onto)
        {
            card = movedCard;
            newStack = stackNew;
            oldStack = stackOld;
            ontoCard = onto;
            ontoCardInfo = new CardInfo();
            cardinfo = new CardInfo();

        }
        public Action(Card movedCard, int stackNew, int stackOld, Card onto, CardInfo movedCardInfo, CardInfo ontoCardInfo)
        {
            card = movedCard;
            newStack = stackNew;
            oldStack = stackOld;
            ontoCard = onto;
            ontoCardInfo = movedCardInfo;
            cardinfo = ontoCardInfo;

        }
        public bool Equals(Action other)
        {
            return newStack == other.newStack && oldStack == other.oldStack && card.Equals(other.card);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ card.GetHashCode();
                hash = hash * 16777619 ^ newStack.GetHashCode();
                hash = hash * 16777619 ^ oldStack.GetHashCode();
                hash = hash * 16777619 ^ (ontoCard == null ? 0 : ontoCard.GetHashCode());
                return hash;
            }
        }
        public override string ToString()
        {
            return card.ToString() + "%" + oldStack.ToString() +"%" + newStack.ToString();
        }
    }
}
