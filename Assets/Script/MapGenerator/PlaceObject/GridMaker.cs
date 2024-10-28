using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    public tileInfo tilePrefab; // prefab cho tile
    public float tileSize = 1f; // kích thước của tile
    public GameObject[,] tilesMatrix;
    public List<tileInfo> tiles;

    void Start()
    {
        tilePrefab = Resources.Load<tileInfo>("TilePrefab");
        tiles = new List<tileInfo>();


        if (tilePrefab != null)
        {
            CreateGrid();
        }
        else
        {
            Debug.Log("no tile");
        }
        
    }

    void CreateGrid()
    {
        // Lấy kích thước của GameObject chính
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 objectSize = renderer.bounds.size;

            // Tính toán số lượng tile theo kích thước của GameObject chính
            int width = Mathf.CeilToInt(objectSize.x / tileSize);
            int height = Mathf.CeilToInt(objectSize.z / tileSize);
            tilesMatrix = new GameObject[width, height];
            // Tính toán vị trí bắt đầu từ góc dưới bên trái của GameObject chính
            Vector3 startPosition = renderer.bounds.min;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    // Tính toán vị trí của từng tile
                    Vector3 tilePosition = startPosition + new Vector3(x * tileSize +0.5f, 0, z * tileSize + 0.5f);
                    tileInfo newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, gameObject.transform);
                    newTile.x = x;
                    newTile.y = z;
                    newTile.isEmpty = true;
                    tiles.Add(newTile);
                    tilesMatrix[x,z] = newTile.gameObject;
                }
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy Renderer trên GameObject chính.");
        }
    }
}
