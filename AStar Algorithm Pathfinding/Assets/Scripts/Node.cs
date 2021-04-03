using System.Drawing;

[System.Serializable]
public class Node
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsWalkable { get; private set; }

    public int GCost = int.MaxValue;
    public int HCost;
    public int FCost;

    public Node PreviousNode;

    public Node(Point nodePosition, bool isWalkable)
    {
        X = nodePosition.X;
        Y = nodePosition.Y;

        IsWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }
}