using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tileMap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public TextMeshProUGUI scoreText;
    private int currScore = 0;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;

        tileMap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }

        scoreText.text = "Current Score: " + currScore.ToString();
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        activePiece.Initialize(this, spawnPosition, data, _cam);

        if (IsValidPosition(activePiece, spawnPosition))
            SetPiece(activePiece);
        else
            GameOver();
    }

    private void GameOver()
    {
        this.tileMap.ClearAllTiles();
        currScore = 0;
        scoreText.text = "Current Score: " + currScore.ToString();
        print("GAMEOVER!!!");
    }

    public void SetPiece(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tileMap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void ClearPiece(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tileMap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition) || tileMap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public int ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;

        // Bound check
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++;
            }
            else
                row++;
        }

        // Score calculation & Update
        currScore += (linesCleared * 100);
        scoreText.text = "Current Score: " + currScore.ToString();

        return linesCleared;
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tileMap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
            tileMap.SetTile(new Vector3Int(col, row, 0), null);
        
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                TileBase above = tileMap.GetTile(new Vector3Int(col, row + 1, 0));
                tileMap.SetTile(new Vector3Int(col, row, 0), above);
            }
            row++;
        }
    }
}
