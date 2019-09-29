using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    class Maze
    {
        public bool[,] array;

        public Maze(bool[,] inputArray)
        {
            array = inputArray;
        }
    }

    class Node
    {
        public int x;
        public int y;
        public List<bool> neighbors = new List<bool>();
        public List<Node> friends = new List<Node>();

        public int value = 0;

        public Node(int _y,int _x,List<bool> _neighbors)
        {
            y = _y;
            x = _x;
            neighbors = _neighbors;
        }

        public Node()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 10 x 10

            bool[,] mazeArray = new bool[10, 10] {
                { false,true,false,false,false,false,false,false,false,false},
                { false,true,false,false,false,true,true,true,true,false},
                { false,true,true,true,true,true,false,false,false,false},
                { false,true,false,true,false,true,true,true,true,false},
                { false,true,true,true,false,false,false,false,true,false}, 
                { false,true,false,true,true,true,true,false,true,false}, 
                { false,true,false,false,false,false,true,true,true,false}, 
                { false,true,false,false,false,false,false,false,false,false}, 
                { false,true,true,true,true,true,true,true,true,false},
                { false,false,false,false,false,false,false,false,true,false}
            };

            var maze = new Maze(mazeArray);

            var solvedPath = Solve(maze);

            foreach(Node n in solvedPath)
            {
                Console.WriteLine(n.x.ToString() + ',' + n.y.ToString());
            }
            Console.WriteLine("Number of used Nodes: "+solvedPath.Count.ToString());
            Console.ReadLine();
        }

        static int amountOfPaths(List<bool> avaliables)
        {
            int counter = 0;
            foreach(bool possible in avaliables)
            {
                if (possible)
                    counter++;
            }
            return counter;
        }

        static bool checkForNoBarriers(bool x, Node node1, Node node2, Maze maze)
        {

            if (x)
            {
                if(node1.x > node2.x)
                {
                    for (int i = node2.x; i >= node1.x; i++)
                    {
                        if (!maze.array[node1.y, i])
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = node1.x; i >= node2.x; i++)
                    {
                        if (!maze.array[node1.y, i])
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (node1.y > node2.y)
                {
                    for (int i = node2.y; i >= node1.y; i++)
                    {
                        if (!maze.array[i, node1.x])
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = node1.y; i <= node2.y; i++)
                    {
                        if (!maze.array[i, node1.x])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static void ConnectNodes(Node node1, Node node2)
        {
            //Console.WriteLine("Connecting "+node1.x.ToString()+','+node1.y.ToString() + " and "+node2.x.ToString()+','+node2.y.ToString()+" together.");
            node1.friends.Add(node2);
            //Console.WriteLine("Node 1 friends: " + node1.friends[node1.friends.Count - 1].x.ToString() + ',' + node1.friends[node1.friends.Count - 1].y.ToString());
            node2.friends.Add(node1);
            //Console.WriteLine("Node 2 friends: " + node2.friends[node2.friends.Count - 1].x.ToString() + ','+ node2.friends[node2.friends.Count - 1].y.ToString());
        }

        static List<Node> ReversePath(List<Node> path)
        {
            List<Node> newPath = new List<Node>();

            for (int i = path.Count-1; i >= 0; i--)
            {
                newPath.Add(path[i]);
            }

            return newPath;
        }

        static List<Node> Solve(Maze maze)
        {
            List<Node> nodes = new List<Node>();

            int x = 0;
            int y = 0;

            #region Nodes

            foreach (bool position in maze.array)
            {
                if (position)
                {
                    bool createNode = false;

                    bool north;
                    bool east;
                    bool west;
                    bool south;

                    if(y == 0)
                    {
                        north = false;
                        createNode = true;
                    }
                    else
                    {
                        north = maze.array[y - 1, x];
                    }

                    if(y == maze.array.GetLength(0) - 1)
                    {
                        south = false;
                        createNode = true;
                    }
                    else
                    {
                        south = maze.array[y + 1, x];
                    }

                    if (x == 0)
                    {
                        west = false;
                        createNode = true;
                    }
                    else
                    {
                        west = maze.array[y, x - 1];
                    }

                    if (x == maze.array.GetLength(1) - 1)
                    {
                        east = false;
                        createNode = true;
                    }
                    else
                    {
                        east = maze.array[y, x + 1];
                    }

                    var neighbors = new List<bool> { north, east, south, west }; // get neighbors

                    int count = amountOfPaths(neighbors);

                    if(count == 3)
                    {
                        createNode = true;
                    }
                    else if(count == 2)
                    {
                        if (neighbors[0] == false && neighbors[2] == false || neighbors[1] == false && neighbors[3] == false)
                        {

                        }
                        else
                        {
                            createNode = true;
                        }
                    }
                    else if(count == 1)
                    {
                        createNode = true;
                    }

                    if (createNode)
                    {
                        nodes.Add(new Node(y, x, neighbors));
                    }
                }
                x++;
                if(x == 10)
                {
                    x = 0;
                    y++;
                }
            }

            #endregion

            #region Connections

            foreach (Node primaryNode in nodes)
            {
                foreach (Node secondaryNode in nodes)
                {
                    if (primaryNode != secondaryNode)
                    {
                        if (primaryNode.x == secondaryNode.x)
                        {
                            if (checkForNoBarriers(false,primaryNode,secondaryNode,maze))
                            {
                                ConnectNodes(primaryNode, secondaryNode);
                            }
                        }
                        else if(primaryNode.y == secondaryNode.y)
                        {
                            if (checkForNoBarriers(true,primaryNode,secondaryNode,maze))
                            {
                                ConnectNodes(primaryNode, secondaryNode);
                            }
                        }
                    }

                }

            }

            #endregion

            Node startPoint = new Node();
            Node endPoint = new Node();

            List<Node> currentPath = new List<Node>();
            List<Node> unUsedNodes = nodes;

            foreach (Node node in nodes)
            {
                if(node.y == 0)
                {
                    startPoint = node;
                }
                if(node.y == maze.array.GetLength(0) - 1)
                {
                    endPoint = node;
                }
            }

            currentPath.Add(endPoint);
            unUsedNodes.Remove(endPoint);

            Node Neo = new Node();
            bool solving = true;

            while (solving)
            {
                Node[] mindlessNodes = endPoint.friends.ToArray();
                foreach (Node mindlessNode in mindlessNodes)
                {
                    if (!unUsedNodes.Contains(mindlessNode))
                    {
                        List<Node> temporaryList = new List<Node>(mindlessNodes);
                        temporaryList.Remove(mindlessNode);
                        mindlessNodes = temporaryList.ToArray();
                    }
                }

                if (endPoint.friends.Count == 1)
                {
                    Neo = endPoint.friends[0];
                }
                else
                {
                    foreach (Node friend in mindlessNodes)
                    {
                        if (friend.value < 1 && friend.friends.Count > 1)
                        {
                            Neo = friend;
                        }
                    }
                }

                Neo.value = 1;
                endPoint = Neo;

                currentPath.Add(Neo);
                unUsedNodes.Remove(Neo);

                if (Neo == startPoint)
                {
                    solving = false;
                }
            }
            

            currentPath = ReversePath(currentPath);
            return currentPath;
        }
    }
}
