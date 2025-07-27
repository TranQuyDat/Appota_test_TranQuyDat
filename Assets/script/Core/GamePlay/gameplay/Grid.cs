using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Node
{
    private int _x;
    private int _y;
    private Vector3 _pos;
    public Node(int x, int y, Vector3 pos)
    {
        _x = x;
        _y = y;
        _pos = pos;
    }

    public int X => _x;
    public int Y => _y;
    public Vector3 Pos => _pos;
}
public class Grid
{
    private int _witdh;
    private int _height;
    private Node[,] _nodes;
    private Vector2 _nodeSize;

    public Grid(int width, int height, Vector2 nodeSize)
    {
        _witdh = width;
        _height = height;
        _nodeSize = nodeSize;
    }
    public void CreateGrid(Vector3 startPos)
    {
        _nodes = new Node[_witdh, _height];
        for (int x = 0; x < _witdh; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 pos = startPos + new Vector3(x * _nodeSize.x, y * _nodeSize.y, 0);
                _nodes[x, y] = new Node(x, y, pos);
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= _witdh || y < 0 || y >= _height)
        {
            return null;
        }
        return _nodes[x, y];
    }

    public int Width => _witdh;
    public int Height => _height;
    public Vector2 NodeSize => _nodeSize;

}