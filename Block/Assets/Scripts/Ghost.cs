using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tileMap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tileMap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tileMap.SetTile(tilePosition, null);
        }
    }
    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
            cells[i] = trackingPiece.cells[i];
    }
    private void Drop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int bottom = -board.boardSize.y / 2 - 1;

        board.ClearPiece(trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;
            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else 
                break;
        }

        board.SetPiece(trackingPiece);
    }
    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tileMap.SetTile(tilePosition, tile);
        }
    }
}
