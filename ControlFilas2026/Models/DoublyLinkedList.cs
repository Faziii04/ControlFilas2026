namespace ControlFilas2026.Models;

public class DoublyLinkedList
{
    private Node? head;
    private Node? tail;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    public void AddLast(Ticket value)
    {
        var node = new Node(value);

        if (tail is null)
        {
            head = tail = node;
        }
        else
        {
            node.Previous = tail;
            tail.Next = node;
            tail = node;
        }

        Count++;
    }

    public void InsertAt(int index, Ticket value)
    {
        if (index <= 0 || head is null)
        {
            AddFirst(value);
            return;
        }

        if (index >= Count)
        {
            AddLast(value);
            return;
        }

        var current = head;
        int currentIndex = 0;

        while (current is not null && currentIndex < index)
        {
            current = current.Next;
            currentIndex++;
        }

        if (current is null)
        {
            AddLast(value);
            return;
        }

        var node = new Node(value)
        {
            Previous = current.Previous,
            Next = current
        };

        if (current.Previous is not null)
        {
            current.Previous.Next = node;
        }

        current.Previous = node;

        if (current == head)
        {
            head = node;
        }

        Count++;
    }

    public int FindInsertIndexForSpecial()
    {
        int index = 0;
        int normals = 0;
        var current = head;

        while (current is not null)
        {
            if (current.Value.IsSpecial)
            {
                normals = 0;
            }
            else
            {
                normals++;
                if (normals == 2)
                {
                    var next = current.Next;
                    bool nextIsSpecial = next is not null && next.Value.IsSpecial;
                    if (!nextIsSpecial)
                    {
                        return index + 1;
                    }

                    normals = 0;
                }
            }

            current = current.Next;
            index++;
        }

        return index;
    }

    public bool TryRemoveFirst(out Ticket? value)
    {
        if (head is null)
        {
            value = null;
            return false;
        }

        var node = head;
        RemoveNode(node);
        value = node.Value;
        return true;
    }

    private void AddFirst(Ticket value)
    {
        var node = new Node(value);

        if (head is null)
        {
            head = tail = node;
        }
        else
        {
            node.Next = head;
            head.Previous = node;
            head = node;
        }

        Count++;
    }

    private void RemoveNode(Node node)
    {
        if (node.Previous is null)
        {
            head = node.Next;
        }
        else
        {
            node.Previous.Next = node.Next;
        }

        if (node.Next is null)
        {
            tail = node.Previous;
        }
        else
        {
            node.Next.Previous = node.Previous;
        }

        Count--;
    }

    private sealed class Node
    {
        public Node(Ticket value)
        {
            Value = value;
        }

        public Ticket Value { get; }
        public Node? Next { get; set; }
        public Node? Previous { get; set; }
    }
}
