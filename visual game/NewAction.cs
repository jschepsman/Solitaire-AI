using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;

namespace visual_game
{
    public class NewAction:IEquatable<NewAction>
    {
        internal int fromPile;
        internal int toPile;
        internal Card moveCard;
        internal Card ontoCard;
        internal int movedShownIndex;
        internal int ontoShownIndex;
        internal int movedShownPile;
        internal int ontoShownPile;
        public NewAction(Card cardToBeMoved, Card cardMovedOnto, int movedCL, int ontoCL, int shownMovedIndex, int shownMovedPile,
            int shownOntoIndex, int shownOntoPile)
        {
            fromPile = movedCL;
            toPile = ontoCL;
            moveCard= cardToBeMoved;
            ontoCard= cardMovedOnto;
            movedShownIndex = shownMovedIndex;
            ontoShownIndex = shownOntoIndex;
            movedShownPile = shownMovedPile;
            ontoShownPile = shownOntoPile;
        }

        public bool Equals(NewAction other)
        {
            return fromPile == other.fromPile && toPile == other.toPile && moveCard.Equals(other.moveCard);
        }
        public override string ToString()
        {
            return fromPile.ToString() + ":" + moveCard.ToString() + ":" + toPile.ToString();
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ moveCard.GetHashCode();
                hash = hash * 16777619 ^ fromPile.GetHashCode();
                hash = hash * 16777619 ^ toPile.GetHashCode();
                hash = hash * 16777619 ^ (ontoCard == null ? 0 : ontoCard.GetHashCode());
                return hash;
            }
        }
    }
}
