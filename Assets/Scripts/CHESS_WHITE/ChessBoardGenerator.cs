using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class ChessBoardGenerator : MonoBehaviour
    {
        [Header("Board")]
        public GameObject whiteTilePrefab;
        public GameObject blackTilePrefab;
        public int boardSize = 8;
        public float tileSize = 20f;

        [Header("Chess Pieces")]
        public GameObject[] chessPiecePrefabs;
        public int pieceCount = 16;

        void Start()
        {
            GenerateBoard();
            SpawnChessPieces();
        }

        void GenerateBoard()
        {
            float offset = (boardSize - 1) * tileSize / 2f;

            for (int x = 0; x < boardSize; x++)
            {
                for (int z = 0; z < boardSize; z++)
                {
                    GameObject tilePrefab = (x + z) % 2 == 0
                        ? whiteTilePrefab
                        : blackTilePrefab;

                    Vector3 position = transform.position + new Vector3(
                        x * tileSize - offset,
                        0.05f,
                        z * tileSize - offset
                    );

                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                    tile.transform.parent = transform;
                }
            }
        }

        void SpawnChessPieces()
        {
            bool[,] occupied = new bool[boardSize, boardSize];
            float offset = (boardSize - 1) * tileSize / 2f;

            for (int i = 0; i < pieceCount; i++)
            {
                int x;
                int z;

                do
                {
                    x = Random.Range(0, boardSize);
                    z = Random.Range(0, boardSize);
                }
                while (occupied[x, z]);

                occupied[x, z] = true;

                Vector3 position = transform.position + new Vector3(
                    x * tileSize - offset,
                    2f,
                    z * tileSize - offset
                );

                GameObject piecePrefab = chessPiecePrefabs[
                    Random.Range(0, chessPiecePrefabs.Length)
                ];

                Instantiate(piecePrefab, position, Quaternion.identity, transform);
            }
        }
    }
}