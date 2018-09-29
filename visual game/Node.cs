using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gameObjects;
using System.Collections;

namespace visual_game
{
    public interface IContainer<T>
    {
        void Update(T container);
    }
    public class EmptyContainer : IContainer<EmptyContainer>
    {
        public EmptyContainer()
        {

        }

        public void Update(EmptyContainer container)
        { 
        }
    }
    public class Node<T, U> where T : IEquatable<T> where U : IContainer<U>
    {
        public Node<T, U> parent;
        public Dictionary<T, Node<T, U>> children;
        public int wins;
        public int visited;
        public double value;
        public int level;
        public U container;
        public Node()
        {
            level = 0;
            wins = 0;
            visited = 0;
            value = 0;
            parent = null;
            children = new Dictionary<T, Node<T,U>>();
        }
        public Node(Node<T,U> parentNode)
        {
            level = parent.level + 1;
            parent = parentNode;
            wins = 0;
            visited = 0;
            value = 0;
            children = new Dictionary<T, Node<T,U>>();
        }
        public void UpdateContainer(U u)
        {
            container.Update(u);
        }
        public override string ToString()
        {
            return level.ToString()+":" + wins.ToString() + ":" + visited.ToString() + ":" + value.ToString();
        }
        public void AddChild(T child)
        {
            children.Add(child, new Node<T, U>(this));
        }
        public void Update(bool win)
        {
            double visit = 1 / (double)visited;
            double tempaction = 0;
            if (win)
            {
                wins++;
                tempaction = 1 - value;

            }
            else
            {
                tempaction = 0 - value;
            }
            tempaction = tempaction * visit;
            value = tempaction + value;
        }
    }
}
