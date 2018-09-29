using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visual_game
{
    public class CardInfo
    {
        public int shownIndex;
        public int pileNum;
        public CardInfo()
        {
            shownIndex = -1;
            pileNum = -1;
        }
        public CardInfo(int index, int pileNumber)
        {
            shownIndex = index;
            pileNum = pileNumber;
        }
        public override string ToString()
        {
            return pileNum.ToString()+":"+shownIndex.ToString();
        }
    }
}
