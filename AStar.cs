using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds information about a
// certain position, so its used in pathfinding algorithm
class Node
{
    //every node may hae different values, 
    //according to game/application
    public enum Value
    {
        FREE,
        BLOCKED
    }
    //Node have X and Y positions (horizontal and vertical)
    public int posX;
    public int posY;

    // G is a basic *cost* value to go from one node to another
    public int g = 0;

    //H is a heuristic that *estimates* the cost of the closest path. A-B
    public int f = 0;

    // Nodes have references to other nodes so it is possible to build a path.
    public Node parent = null;

    public Value value;

    //Constructors
    public Node(int posX, int posY)
    {
        this.posX = posY;
        this.posY = posX;

        value = Value.FREE;
    }
}

public class AStar : MonoBehaviour
{
    //Constants
    private const int MAP_SIZE = 6;

    // Variables
    private List<string> map;
    private Node[,] nodeMap;

    // Start is called before the first frame update
    void Start()
    {
        map = new List<string>();
        map.Add("G-----");
        map.Add("XXXXX-");
        map.Add("S-X-X-");
        map.Add("--X-X-");
        map.Add("--X-X-");
        map.Add("------");

        //Parse the map.
        nodeMap = new Node[MAP_SIZE, MAP_SIZE];
        Node start = null;
        Node goal = null;

        for (int y=0; y<MAP_SIZE; y++)
        {
            for (int x=0; x<MAP_SIZE; x++)
            {
                Node node = new Node(x, y);

                char currentChar = map[y][x];
                if (currentChar == 'X')
                {
                    node.value = Node.Value.BLOCKED;
                }else if (currentChar == 'G')
                {
                    goal = node;
                }else if (currentChar == 'S')
                {
                    start = node;
                }

                nodeMap[x, y] = node;

            }
        }
        //Executing algorithm
        List<Node> nodePath = ExecuteAStar (start, goal);

        //Burning path into map


        //Print the map
        string mapString = "";
        foreach (string mapRow in map)
        {
            mapString += mapRow + '\n';
        }
        Debug.Log(mapString);
    }
    
    private List<Node> ExecuteAStar (Node start, Node goal)
    {
        //This list holds potential best path nodes that should be visited. Starts with origin
        List<Node> openList = new List<Node>() { start };


        //Keeps tracks of visited nodes.
        List<Node> closedList = new List<Node>();

        //Initialise the start node
        start.g = 0;
        start.f = start.g + CalculateHeuristicValue(start, goal);

        while(openList.Count > 0)
        {
            //first of all, get the node with the lowest estimated cost to reach target.
            Node current = openList[0];
            foreach(Node node in openList)
            {
                if(node.f < current.f)
                {
                    current = node;
                }
            }

            //check if the target has been reached.
            if(current == goal)
            {
                return BuildPath(goal);
            }
            //make sure current node will not be revisited
            openList.Remove(current);
            closedList.Add(current);

            // execute the algorithm in the current nodes neighbours.
            List<Node> neighbours = GetNeighbourNodes(current);
            foreach(Node neighbour in neighbours)
            {
                if (closedList.Contains(neighbour))
                {
                    // if neighbour has been visited, ignore it
                    continue;
                }

                if (!openList.Contains(neighbour))
                {
                    //if neighbour hasnt been visited, add it.
                    openList.Add(neighbour);
                }

                // Calculate a new G Value and verify is this value is better than whatever is stored in the neighbour.
                int candidateG = current.g + 1;
                if(candidateG >= neighbour.g)
                {
                    // if g value is greater or equal , then its not a good path(better path calculated).
                    continue;
                }
                else
                {
                    // otherwise we found a better way to reach this neighbour
                    // initialise its values.
                    neighbour.parent = current;
                    neighbour.g = candidateG;
                    neighbour.f = neighbour.g + CalculateHeuristicValue(neighbour, goal);
                    // f = g + h
                }
            }
        }
        // If reached this point, theres no more nodes to search. Algorithm failed
        return new List<Node>();
    }

    private List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();
        //Verify al possible neighbours, since we can only mmove horizonally and vertically, check these 4 possibilities
        // Note that if a node is blocked it cannot be visited.
        if(node.posX -1 >= 0)
        {
            Node candidate = nodeMap[node.posX - 1, node.posY];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbours.Add(candidate);
            }
        }
        if(node.posX +1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            if(candidate.value != Node.Value.BLOCKED)
            {
                neighbours.Add(candidate);
            }
        }
        if(node.posY -1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            if(candidate.value != Node.Value.BLOCKED)
            {
                neighbours.Add(candidate);
            }
        }
        if(node.posY + 1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            if(candidate.value != Node.Value.BLOCKED)
            {
                neighbours.Add(candidate);
            }
        }

        return neighbours;
    }
    
    private int CalculateHeuristicValue (Node node1, Node node2)
    {
        return Mathf.Abs(node1.posX - node2.posX) + Mathf.Abs(node1.posY - node2.posY);
    }

    private List<Node> BuildPath(Node node)
    {
        List<Node> path = new List<Node>() { node };
        while(node.parent != null)
        {
            node = node.parent;
            path.Add(node);
        }
        return path;
    }
}
