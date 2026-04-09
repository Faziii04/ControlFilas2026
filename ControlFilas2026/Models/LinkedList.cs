using System;
using System.Collections.Generic;
using System.Text;

namespace ControlFilas2026.Models
{
    public class LinkedList
    {
        private Node? head;
        private Node? tail;

        public int Count { get; private set; }

        public void EnqueueNormal(Ticket ticket)
        {
            Append(ticket);
        }

        public void EnqueueSpecial(Ticket ticket)
        {
            if (head is null)
            {
                Append(ticket);
                return;
            }

            int normalStreak = 0;
            var current = head;

            while (current is not null)
            {
                if (current.Value.IsSpecial)
                {
                    normalStreak = 0;
                }
                else
                {
                    normalStreak++;

                    if (normalStreak == 2)
                    {
                        bool nextIsSpecial = current.Next is not null && current.Next.Value.IsSpecial;

                        if (!nextIsSpecial)
                        {
                            var specialNode = new Node(ticket)
                            {
                                Next = current.Next
                            };

                            current.Next = specialNode;

                            if (tail == current)
                            {
                                tail = specialNode;
                            }

                            Count++;
                            return;
                        }

                        normalStreak = 0;
                    }
                }

                current = current.Next;
            }

            Append(ticket);
        }

        public bool TryDequeue(out Ticket? ticket)
        {
            if (head is null)
            {
                ticket = null;
                return false;
            }

            ticket = head.Value;
            head = head.Next;

            if (head is null)
            {
                tail = null;
            }

            Count--;
            return true;
        }

        public void Clear()
        {
            head = null;
            tail = null;
            Count = 0;
        }

        private void Append(Ticket ticket)
        {
            var node = new Node(ticket);

            if (tail is null)
            {
                head = node;
                tail = node;
            }
            else
            {
                tail.Next = node;
                tail = node;
            }

            Count++;
        }

        private sealed class Node
        {
            public Node(Ticket value)
            {
                Value = value;
            }

            public Ticket Value { get; }
            public Node? Next { get; set; }
        }
    }
}
