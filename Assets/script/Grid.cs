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

    public Grid(int width, int height)
    {
        _witdh = width;
        _height = height;

    }
    public void CreateGrid(Vector3 startPos, float nodeSize)
    {
        _nodes = new Node[_witdh, _height];
        for (int x = 0; x < _witdh; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 pos = startPos + new Vector3(x * nodeSize, y * nodeSize, 0);
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

}