using System.Collections;

namespace Alt
{
    public enum Color : byte
    {
        BLACK = 0,
        RED = 1
    }

    public enum Direction : byte
    {
        NONE = 0,
        LEFT = 1,
        RIGHT = 2
    }

    public class Node<TKey, TValue> : IEnumerable<Node<TKey, TValue>>
    {
        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Parent = null;
            Left = null;
            Right = null;
            Color = Color.RED;
        }

        public Node(Node<TKey, TValue> node)
        {
            Key = node.Key;
            Value = node.Value;
            Parent = node.Parent;
            Left = node.Left;
            Right = node.Right;
            Color = node.Color;
        }

        public bool HasParent() => Parent != null;
        public bool HasLeftChild() => Left != null;
        public bool HasRightChild() => Right != null;

        public int GetChildrenCount()
        {
            int count = Left != null ? 1 : 0;
            count += Right != null ? 1 : 0;
            return count;
        }
        public Direction GetDirection()
        {
            if (Parent == null)
            {
                return Direction.NONE;
            }
            else
            {
                return this.Parent.Left == this ? Direction.LEFT : Direction.RIGHT;
            }
        }

        public Node<TKey, TValue>? GetChild(Direction direction)
        {
            if (direction == Direction.NONE)
            {
                return null;
            }
            else
            {
                return direction == Direction.LEFT ? Left : Right;
            }
        }

        public void SetChild(Direction direction, Node<TKey, TValue>? node)
        {
            if (direction == Direction.NONE)
            {
                return;
            }
            else
            {
                if (direction == Direction.LEFT)
                {
                    Left = node;
                }
                else
                {
                    Right = node;
                }
                if (node != null)
                {
                    node.Parent = this;
                }
            }
        }

        public Direction GetChildDirection(Node<TKey, TValue> childNode)
        {
            if (childNode == null)
            {
                return Direction.NONE;
            }
            else
            {
                return childNode == Left ? Direction.LEFT : Direction.RIGHT;
            }
        }

        public Node<TKey, TValue>? GetInOrderPredecessor()
        {
            return Left?.GetMaximumSubtreeNode();
        }

        public Node<TKey, TValue>? GetInOrderSuccessor()
        {
            return Right?.GetMinimumSubtreeNode();
        }

        public void RemoveFromTree()
        {
            if (Parent != null)
            {
                Parent.SetChild(GetDirection(), null);
            }
        }

        public IEnumerator<Node<TKey, TValue>> GetEnumerator()
        {
            if (Left != null)
            {
                foreach (var node in Left)
                {
                    yield return node;
                }
            }
            yield return this;
            if (Right != null)
            {
                foreach (var node in Right)
                {
                    yield return node;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Node<TKey, TValue>? GetMaximumSubtreeNode()
        {
            var max = this;
            while (max.Right != null)
            {
                max = max.Right;
            }
            return max;
        }

        private Node<TKey, TValue> GetMinimumSubtreeNode()
        {
            var min = this;
            while (min.Left != null)
            {
                min = min.Left;
            }
            return min;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public Color Color { get; set; }
        public Node<TKey, TValue>? Parent { get; set; }
        public Node<TKey, TValue>? Left { get; set; }
        public Node<TKey, TValue>? Right { get; set; }
    }
}
