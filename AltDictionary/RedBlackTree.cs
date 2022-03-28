using Microsoft.Toolkit.Diagnostics;
using System.Collections;

namespace Alt
{
    public class RedBlackTree<TKey, TValue> : IEnumerable<Node<TKey, TValue>>, ICollection<Node<TKey, TValue>>
    {
        public RedBlackTree(IComparer<TKey>? comparer)
        {
            Root = null;
            this.comparer = comparer ?? Comparer<TKey>.Default;
            count = 0;
        }

        public RedBlackTree() : this(null) { }

        public Node<TKey, TValue>? Root { get; set; }

        public int Count => count;

        public bool IsReadOnly => false;

        public Node<TKey, TValue>? GetNode(TKey key)
        {
            return FindKeyInSubtree(Root, key);
        }

        public bool Contains(TKey key)
        {
            return FindKeyInSubtree(Root, key) != null;
        }

        public bool Add(TKey key, TValue value)
        {
            Node<TKey, TValue> node = new(key, value);
            if (Root == null)
            {
                Insert(node, null, Direction.NONE);
                return true;
            }
            else
            {
                var parent = FindFutureParent(Root, key);
                if (parent != null)
                {
                    var direction = comparer.Compare(key, parent.Key) < 0 ? Direction.LEFT : Direction.RIGHT;
                    Insert(node, parent, direction);
                    return true;
                }
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            var node = FindKeyInSubtree(Root, key);
            if (node != null)
            {
                Delete(node);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            Root = null;
            count = 0;
        }

        public int GetHeight()
        {
            return GetHeightFromNode(Root);
        }

        public bool IsBalanced()
        {
            return IsTreeBalancedFromNode(Root);
        }

        public IEnumerator<Node<TKey, TValue>> GetEnumerator()
        {
            if (Root != null)
            {
                return Root.GetEnumerator();
            }
            else
            {
                return (IEnumerator<Node<TKey, TValue>>)Enumerable.Empty<Node<TKey, TValue>>();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Node<TKey, TValue>? FindKeyInSubtree(Node<TKey, TValue>? node, TKey key)
        {
            if (node == null)
            {
                return null;
            }
            else
            {
                return comparer.Compare(key, node.Key) == 0 ? node : (comparer.Compare(key, node.Key) > 0 ? FindKeyInSubtree(node.Right, key) : FindKeyInSubtree(node.Left, key));
            }
        }

        private Node<TKey, TValue>? FindFutureParent(Node<TKey, TValue> node, TKey key)
        {
            if (node == null)
            {
                return null;
            }
            else
            {
                if (comparer.Compare(key, node.Key) < 0)
                {
                    return node.Left == null ? node : FindFutureParent(node.Left, key);
                }
                else if (comparer.Compare(key, node.Key) > 0)
                {
                    return node.Right == null ? node : FindFutureParent(node.Right, key);
                }
                return null;
            }
        }

        private void RotateSubtree(Node<TKey, TValue> P, Direction direction)
        {
            if (P != null)
            {
                var G = P.Parent;
                var S = P.GetChild(Opposite(direction));
                if (S != null)
                {
                    var C = S.GetChild(direction);
                    P.SetChild(Opposite(direction), C);
                    S.SetChild(direction, P);
                    if (G != null)
                    {
                        G.SetChild(G.GetChildDirection(P), S);
                    }
                    else
                    {
                        Root = S;
                        S.Parent = null;
                    }
                }
            }
        }

        private void Insert(Node<TKey, TValue> N, Node<TKey, TValue>? P, Direction direction)
        {
            count++;
            if (P == null)
            {
                Root = N;
                N.Parent = null;
                return;
            }
            P.SetChild(direction, N);
            do
            {
                if (P.Color == Color.BLACK)
                {
                    return;
                }
                if (P.Parent == null)
                {
                    P.Color = Color.BLACK;
                    return;
                }
                var G = P.Parent;
                var U = G.GetChild(Opposite(P.GetDirection()));
                if (U == null || U.Color == Color.BLACK)
                {
                    if (N == P.GetChild(Opposite(direction)))
                    {
                        RotateSubtree(P, direction);
                        N = P;
                        P = G.GetChild(direction);
                    }
                    RotateSubtree(G, Opposite(direction));
                    if (P != null)
                    {
                        P.Color = Color.BLACK;
                    }
                    G.Color = Color.RED;
                    return;
                }
                else
                {
                    P.Color = Color.BLACK;
                    U.Color = Color.BLACK;
                    G.Color = Color.RED;
                    N = G;
                }
            }
            while ((P = N.Parent) != null);
        }

        private void Delete(Node<TKey, TValue> N)
        {
            count--;
            if (Root == N && N.GetChildrenCount() == 0)
            {
                Root = null;
                return;
            }
            if (N.GetChildrenCount() == 2)
            {
                var R = N.GetInOrderPredecessor();
                if (R != null)
                {
                    SwapPlacesAndColors(N, R);
                }
            }
            if (N.GetChildrenCount() == 1)
            {
                if (N.Left != null)
                {
                    ReplaceWithChildAndPaintBlack(N, Direction.LEFT);
                }
                else if (N.Right != null)
                {
                    ReplaceWithChildAndPaintBlack(N, Direction.RIGHT);
                }
                return;
            }
            // N is a leaf
            else
            {
                if (N.Color == Color.RED)
                {
                    N.RemoveFromTree();
                    return;
                }
                else
                {
                    DeleteBlackLeaf(N);
                }
            }
        }

        private void DeleteBlackLeaf(Node<TKey, TValue> N)
        {
            var P = N.Parent;
            var direction = N.GetDirection();
            if (P != null)
            {
                N.RemoveFromTree();
                do
                {
                    var S = P.GetChild(Opposite(direction));
                    var D = S.GetChild(Opposite(direction));
                    var C = S.GetChild(direction);

                    if (S.Color == Color.RED)
                    {
                        RotateSubtree(P, direction);
                        P.Color = Color.RED;
                        S.Color = Color.BLACK;
                        S = C;
                        D = S.GetChild(Opposite(direction));
                        if (D != null && D.Color == Color.RED)
                        {
                            RotateSubtree(P, direction);
                            S.Color = P.Color;
                            P.Color = Color.BLACK;
                            D.Color = Color.BLACK;
                            return;
                        }
                        C = S.GetChild(direction);
                        if (C != null && C.Color == Color.RED)
                        {
                            RotateSubtree(S, Opposite(direction));
                            S.Color = Color.RED;
                            C.Color = Color.BLACK;
                            D = S;
                            S = C;
                            RotateSubtree(P, direction);
                            S.Color = P.Color;
                            P.Color = Color.BLACK;
                            D.Color = Color.BLACK;
                            return;
                        }
                        else
                        {
                            S.Color = Color.RED;
                            P.Color = Color.BLACK;
                            return;
                        }
                    }

                    if (D != null && D.Color == Color.RED)
                    {
                        RotateSubtree(P, direction);
                        S.Color = P.Color;
                        P.Color = Color.BLACK;
                        D.Color = Color.BLACK;
                        return;
                    }

                    if (C != null && C.Color == Color.RED)
                    {
                        RotateSubtree(S, Opposite(direction));
                        S.Color = Color.RED;
                        C.Color = Color.BLACK;
                        D = S;
                        S = C;
                        RotateSubtree(P, direction);
                        S.Color = P.Color;
                        P.Color = Color.BLACK;
                        D.Color = Color.BLACK;
                        return;
                    }

                    if (P.Color == Color.RED)
                    {
                        S.Color = Color.RED;
                        P.Color = Color.BLACK;
                        return;
                    }

                    S.Color = Color.RED;
                    N = P;
                }
                while ((P = N.Parent) != null);
            }
        }

        private static Direction Opposite(Direction direction)
        {
            if (direction == Direction.NONE)
            {
                return direction;
            }
            else
            {
                return direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
            }
        }

        private void SwapPlacesAndColors(Node<TKey, TValue> a, Node<TKey, TValue> b)
        {
            // swap left connection
            var c = a.Left;
            a.Left = b.Left;
            if (a.Left != null)
            {
                a.Left.Parent = a;
            }
            b.Left = c;
            if (b.Left != null)
            {
                b.Left.Parent = b;
            }
            // swap right connection
            c = a.Right;
            a.Right = b.Right;
            if (a.Right != null)
            {
                a.Right.Parent = a;
            }
            b.Right = c;
            if (b.Right != null)
            {
                b.Right.Parent = b;
            }
            // swap parents
            c = a.Parent;
            a.Parent = b.Parent;
            if (a.Parent != null)
            {
                if (a.GetDirection() == Direction.LEFT)
                {
                    a.Parent.Left = a;
                }
                else
                {
                    a.Parent.Right = a;
                }
            }
            b.Parent = c;
            if (b.Parent != null)
            {
                if (b.GetDirection() == Direction.LEFT)
                {
                    b.Parent.Left = b;
                }
                else
                {
                    b.Parent.Right = b;
                }
            }
            // swap colors
            (b.Color, a.Color) = (a.Color, b.Color);

            // has the root changed?
            if (a == Root)
            {
                Root = b;
            }
        }

        private void ReplaceWithChildAndPaintBlack(Node<TKey, TValue> a, Direction direction)
        {
            if (direction == Direction.LEFT && a.Left != null)
            {
                var b = a.Left;
                b.Parent = a.Parent;
                if (b.Parent != null)
                {
                    b.Parent.Left = b;
                }
                b.Color = Color.BLACK;
                if (Root == a)
                {
                    Root = b;
                }
            }
            else if (direction == Direction.RIGHT && a.Right != null)
            {
                var b = a.Right;
                b.Parent = a.Parent;
                if (b.Parent != null)
                {
                    b.Parent.Right = b;
                }
                b.Color = Color.BLACK;
                if (Root == a)
                {
                    Root = b;
                }
            }
        }

        public void Add(Node<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(Node<TKey, TValue> item)
        {
            return Contains(item.Key);
        }

        public bool Contains(TKey key, TValue value)
        {
            var node = GetNode(key);
            if (value != null)
            {
                return node != null && value.Equals(node.Value);
            }
            else
            {
                return node != null && node.Value == null;
            }
        }

        public void CopyTo(Node<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException("Array value cannot be null.");
            }
            if (arrayIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Array index must be a non-negative integer.");
            }
            if (array.Length - arrayIndex < count)
            {
                throw new ArgumentException("Array cannot hold all elements.");
            }
            for (int i = 0, j = arrayIndex; i < count; i++)
            {
                foreach (var item in this)
                {
                    array[j++] = item;
                }
            }
        }

        public bool Remove(Node<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        private int GetHeightFromNode(Node<TKey, TValue>? node)
        {
            if (node == null)
            {
                return 0;
            }
            else
            {
                return 1 + Math.Max(GetHeightFromNode(node.Left), GetHeightFromNode(node.Right));
            }
        }

        private bool IsTreeBalancedFromNode(Node<TKey, TValue>? node)
        {
            if (node == null)
            {
                return true;
            }
            else
            {
                return Math.Abs(GetHeightFromNode(node.Left) - GetHeightFromNode(node.Right)) <= 1 && IsTreeBalancedFromNode(node.Left) && IsTreeBalancedFromNode(node.Right);
            }
        }

        private readonly IComparer<TKey> comparer;
        private int count;
    }
}