using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ORC
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GridTile m_gridTilePrefab;
        
        private void Start()
        {
            var grid = new GridBase(24, 12, 1);
            
        }
    }

    public class GridTile : MonoBehaviour
    {
        
    }

    public class GridBase
    {
        private int m_width;
        private int m_height;
        private int m_tileSize;
        private int[,] m_grid;
        
        public GridBase(int width, int height, int tileSize)
        {
            m_width = width;
            m_height = height;
            m_tileSize = tileSize;
            m_grid = new int[width, height];
            
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    m_grid[x, y] = 0;
                }
            }
        }
    }
}
