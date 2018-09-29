//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace visual_game
//{
//public class RandomAction: IEquatable<RandomAction>
//    {
//        public int action;
//        public RandomContainer container;
//        public RandomAction (int i)
//        {
//            action = i;
//        }

//        public bool Equals(RandomAction other)
//        {
//            return other.action == action ;
//        }
//    }
//    public class RandomContainer : IEquatable<RandomContainer>
//    {
//        public bool end, win;
//        public RandomContainer(bool win,bool loss)
//        {
//            end = win || loss;
//            this.win = win;
//        }
//        public bool Equals(RandomContainer other)
//        {
//            return win == other.win && other.end == end;
//        }
//    }

//    public class RandomGame
//    {
//        Random ran;
//        bool
//        public RandomGame()
//        {
//            ran = new Random();
//        }

//        public List<RandomAction> GetMoves()
//        {
//            List<RandomAction> list = new List<RandomAction>();
//            int size = ran.Next(1,5);
//            for(int i=0;i<size;i++)
//            {
//                list.Add(new RandomAction(i));
//            }
//            return list;
//        }
//        public bool IsWin()
//        {

//        }
//    }
//}
