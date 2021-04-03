using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    private List<Node> nodeList = new List<Node>();

    private Tilemap _floorTilemap;

    private Tilemap _collisionsTilemap;

    private Point _tilemapMaximalPoints;

    private Point _tilemapMinimalPoints;


    public void Configure(Tilemap floorTilemap, Tilemap collisionsTilemap)
    {
        _floorTilemap = floorTilemap;

        _collisionsTilemap = collisionsTilemap;
    }

    #region Pathfinding

    public List<Node> FindObject(Point objectPosition, Point startPosition)
    {
        nodeList = GenerateNodes();

        Node endNode = GetNode(new Point(objectPosition.X, objectPosition.Y));

        Node startNode = GetNode(new Point(Mathf.RoundToInt(startPosition.X), Mathf.RoundToInt(startPosition.Y)));

        if (startNode == null || endNode == null)
        {
            return null;
        }

        else
        {
            GetTilemapSize();

            List<Node> openNodes = new List<Node>();

            List<Node> closedNodes = new List<Node>();

            Node currentNode = startNode;

            currentNode.GCost = 0;

            currentNode.HCost = ReturnNodeDistanceCost(startNode, endNode);

            currentNode.CalculateFCost();

            openNodes.Add(currentNode);

            while (openNodes.Count > 0)
            {
                currentNode = ReturnNodeWithLowestFCost(openNodes);

                if (currentNode == endNode)
                {
                    return ReturnPath(endNode);
                }

                openNodes.Remove(currentNode);

                closedNodes.Add(currentNode);

                foreach (Node nearNode in ReturnNearNodes(currentNode))
                {
                    if (!closedNodes.Contains(nearNode) && nearNode.IsWalkable)
                    {
                        int moveCost = currentNode.GCost + ReturnNodeDistanceCost(currentNode, nearNode);

                        if (moveCost < nearNode.GCost)
                        {
                            nearNode.GCost = moveCost;
                            nearNode.HCost = ReturnNodeDistanceCost(nearNode, endNode);
                            nearNode.PreviousNode = currentNode;
                            nearNode.CalculateFCost();

                            if (!openNodes.Contains(nearNode))
                            {
                                openNodes.Add(nearNode);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    private List<Node> ReturnPath(Node endNode)
    {
        List<Node> finalPath = new List<Node>();

        Node currentNode = endNode;

        while (currentNode.PreviousNode != null)
        {
            finalPath.Add(currentNode.PreviousNode);
            currentNode = currentNode.PreviousNode;
        }

        finalPath.Reverse();

        return finalPath;
    }

    #endregion

    #region Nodes

    private List<Node> GenerateNodes()
    {
        List<Node> nodesList = new List<Node>();

        foreach (var position in _collisionsTilemap.cellBounds.allPositionsWithin)
        {
            if (_floorTilemap.HasTile(position))
            {
                nodesList.Add(new Node(new Point(position.x, position.y), true));
            }

            else if (_collisionsTilemap.HasTile(position))
            {
                nodesList.Add(new Node(new Point(position.x, position.y), false));
            }
        }

        return nodesList;
    }

    private Node GetNode(Point nodePosition)
    {
        foreach (Node node in nodeList)
        {
            if (node.X == nodePosition.X && node.Y == nodePosition.Y)
            {
                return node;
            }
        }

        return null;
    }

    private List<Node> ReturnNearNodes(Node node)
    {
        List<Node> nearNodes = new List<Node>();

        if (node.X + 1 <= _tilemapMaximalPoints.X)
        {
            nearNodes.Add(GetNode(new Point(node.X + 1, node.Y))); // RIGHT NODE
        }

        if (node.X + 1 <= _tilemapMaximalPoints.X && node.Y + 1 <= _tilemapMaximalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X + 1, node.Y + 1))); // UPPER RIGHT NODE
        }

        if (node.X + 1 <= _tilemapMaximalPoints.X && node.Y - 1 >= _tilemapMinimalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X + 1, node.Y - 1))); // BOTTOM RIGHT NODE
        }

        if (node.X - 1 >= _tilemapMinimalPoints.X)
        {
            nearNodes.Add(GetNode(new Point(node.X - 1, node.Y))); // LEFT NODE
        }

        if (node.X - 1 >= _tilemapMinimalPoints.X && node.Y + 1 <= _tilemapMaximalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X - 1, node.Y + 1))); // UPPER LEFT NODE
        }

        if (node.X - 1 >= _tilemapMinimalPoints.X && node.Y - 1 >= _tilemapMinimalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X - 1, node.Y - 1))); // UPPER LEFT NODE
        }

        if (node.Y + 1 <= _tilemapMaximalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X, node.Y + 1))); // MIDDLE UPPER NODE
        }

        if (node.Y - 1 >= _tilemapMinimalPoints.Y)
        {
            nearNodes.Add(GetNode(new Point(node.X, node.Y - 1))); // MIDDLE BOTTOM NODE
        }

        return nearNodes;
    }

    private int ReturnNodeDistanceCost(Node firstNode, Node secondNode)
    {
        int diagonalCost = 14;
        int straightCost = 10;

        int distanceX = Mathf.Abs(firstNode.X - secondNode.X);
        int distanceY = Mathf.Abs(firstNode.Y - secondNode.Y);

        int straightMoves = Mathf.Abs(distanceX - distanceY);

        return diagonalCost * Mathf.Min(distanceX, distanceY) + straightCost * straightMoves;
    }

    private Node ReturnNodeWithLowestFCost(List<Node> nodeList)
    {
        Node nodeWithLowestFCost = nodeList[0];

        foreach (Node node in nodeList)
        {
            if (node.FCost < nodeWithLowestFCost.FCost || node.FCost == nodeWithLowestFCost.FCost && node.HCost < nodeWithLowestFCost.HCost && node.IsWalkable)
            {
                nodeWithLowestFCost = node;
            }
        }

        return nodeWithLowestFCost;
    }

    #endregion

    #region Tilemaps

    private void GetTilemapSize()
    {
        _tilemapMaximalPoints = new Point(_collisionsTilemap.cellBounds.xMax, _collisionsTilemap.cellBounds.yMax);
        _tilemapMinimalPoints = new Point(_collisionsTilemap.cellBounds.xMin, _collisionsTilemap.cellBounds.yMin);
    }

    #endregion
}

