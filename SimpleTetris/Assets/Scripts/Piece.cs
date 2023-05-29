using UnityEngine;

public class Piece : MonoBehaviour
{
    #region Properties
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    public bool IsTetris;

    #region Drag Drop Properties
    public bool isDraggable = true;

    private bool isDragged = false;
    private Camera _cam;
    #endregion
    #endregion

    public void Initialize(Board board, Vector3Int position, TetrominoData data, Camera camera)
    {
        this.board = board;
        this.data = data;
        this.position = position;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;


        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.ClearPiece(this);
        lockTime += Time.deltaTime;

        if (IsTetris)
        {
            if (Input.GetKeyDown(KeyCode.Z))
                Rotate(-1);
            else if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.UpArrow)))
                Rotate(1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                while (Move(Vector2Int.down))
                    continue;
                Lock();
            }

            if (Time.time > moveTime)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    Move(Vector2Int.left);
                else if (Input.GetKey(KeyCode.RightArrow))
                    Move(Vector2Int.right);

                if (Input.GetKey(KeyCode.DownArrow))
                    Move(Vector2Int.down);
            }

            if (Time.time >= stepTime)
                Step();
        }

        board.SetPiece(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
            Lock();
    }

    private void Lock()
    {
        board.SetPiece(this);
        int linesCleared = board.ClearLines();

        // Decreases step delay as more lines are cleared
        stepDelay = linesCleared > 0 ? stepDelay - linesCleared * 0.01f : stepDelay;

        board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        if (board.IsValidPosition(this, newPosition))
        {
            position = newPosition;
            
            // Reset movement & lock timer when piece is set
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
            return true;
        }

        return false;
    }

    private void Rotate(int dir)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + dir, 0, 4);

        ApplyRotationMatrix(dir);

        if (!TestWallKicks(rotationIndex, dir))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-dir);
        }
    }

    private void ApplyRotationMatrix(int dir)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * dir) + (cell.y * Data.RotationMatrix[1] * dir));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * dir) + (cell.y * Data.RotationMatrix[3] * dir));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * dir) + (cell.y * Data.RotationMatrix[1] * dir));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * dir) + (cell.y * Data.RotationMatrix[3] * dir));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];
            if (Move(translation))
                return true;
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0)
            wallKickIndex--;

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
}
