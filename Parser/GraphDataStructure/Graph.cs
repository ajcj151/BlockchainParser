﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDataStructure
{
    public class Graph
    {
        private LinkedList<GraphNode> _nodeSet;

        public Graph()
        {
            this._nodeSet = new LinkedList<GraphNode>();
        }
        public Graph(LinkedList<GraphNode> set)
        {
            if (set == null)
            {
                this._nodeSet = new LinkedList<GraphNode>();
            }
            else
            {
                this._nodeSet = set;
            }
        }

        public void addGraphNode(GraphNode node)
        {
            this._nodeSet.AddLast(node);
        }

        public void addGraphNode(string node)
        {
            this._nodeSet.AddLast(new GraphNode(node));
        }

        public void addDirectedEdge(GraphNode from, GraphNode to)
        {
            from.Neighbors.AddFirst(to);
        }

        public void addDirectedEdge(string from, string to)
        {
            GraphNode fromNode = new GraphNode(from);
            GraphNode toNode = new GraphNode(to);

            this._nodeSet.ElementAt(getGraphNode(fromNode)).Neighbors.AddFirst(this._nodeSet.ElementAt(getGraphNode(toNode)));
        }

        public int getGraphNode(GraphNode node)
        {
            for (int i = 0; i < this._nodeSet.Count(); i++)
            {
                if (this._nodeSet.ElementAt(i).Address.Equals(node.Address))
                {
                    return i;
                }
            }
            return 0;
        }

        public bool exists(GraphNode node)
        {
            return this._nodeSet.Find(node) != null;
        }

        public void removeNode(GraphNode node)
        {
            var nodeToRemove = this._nodeSet.ElementAt(getGraphNode(node));

            if (nodeToRemove != null)
            {
                this._nodeSet.Remove(nodeToRemove);

                //foreach (var item in this._nodeSet)
                //{
                //    item.Neighbors.Remove(nodeToRemove);
                //}
            }
        }

        public void removeEndNodes()
        {
            var endNodes = findEndNodes();

            foreach(var node in endNodes)
            {
                if(node.Neighbors.Count < 1)
                    removeNode(node);
            }
        }

        public List<GraphNode> findEndNodes()
        {
            var nodesToRemoveList = new List<GraphNode>();

            foreach(var node in this._nodeSet)
            {
                if(node.Neighbors.Count < 1)
                {
                    nodesToRemoveList.Add(node);
                }
            }

            return nodesToRemoveList;
        }

        public void displayList()
        {
            foreach (var gnode in this._nodeSet)
            {
                Console.Write(gnode.Address + ":---->");

                foreach (var neighbor in gnode.Neighbors)
                {
                    Console.Write(neighbor.Address + "---->");
                }

                Console.WriteLine();
            }
        }

        public void writeListToFile()
        {
            foreach (var gnode in this._nodeSet)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Windows v2\AddressGraph2.txt", true))
                {

                    file.Write(gnode.Address + ":---->");

                    foreach (var neighbor in gnode.Neighbors)
                    {
                        file.Write(neighbor.Address + " , ");
                    }

                    file.WriteLine();
                }
            }
        }

        public static Graph populate(string publicAddress)
        {
            int count = 0; 
            Queue<string> nextAddresses = new Queue<string>();
            GraphDataStructure.Graph graphList = new GraphDataStructure.Graph();

            graphList.addGraphNode(publicAddress);
            nextAddresses.Enqueue(publicAddress);

            while (nextAddresses.Count > 0)
            {
                Database.DBConnect getLists = new Database.DBConnect();
                string current = nextAddresses.Dequeue();

                var sendersList = getLists.getSentTo(current);
                var reciverList = getLists.getRecivedFrom(current);

                foreach (var sender in sendersList)
                {
                    nextAddresses.Enqueue(sender.target);
                    graphList.addGraphNode(sender.target);

                    graphList.addDirectedEdge(sender.source, sender.target);
                    // Console.WriteLine(sender.source + "     " + sender.target);
                }

                foreach (var reciver in reciverList)
                {
                    nextAddresses.Enqueue(reciver.source);
                    graphList.addGraphNode(reciver.source);

                    graphList.addDirectedEdge(reciver.source, reciver.target);
                    //  Console.WriteLine(reciver.source + "    " + reciver.target);
                }

                count++;
            }

            return graphList;
        }
    }
}
