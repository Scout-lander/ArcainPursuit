using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs; // List of different chunk prefabs
    public int chunkSize = 120;            // Size of each chunk
    public int renderDistance = 2;        // How many chunks around the player to render
    public Transform player;              // Reference to the player transform
    public float checkInterval = 0.5f;    // Time in seconds between position checks

    private Vector2 currentChunkCoord;    // The current chunk coordinate the player is in
    private Dictionary<Vector2, GameObject> spawnedChunks = new Dictionary<Vector2, GameObject>();
    private Queue<GameObject> chunkPool = new Queue<GameObject>(); // Object pool for chunks
    private float checkTimer = 0f;
    private Vector2 lastPlayerPosition;   // To track player's movement direction

    void Start()
    {
        UpdateChunksBasedOnMovement(); // Correct method name used here
        lastPlayerPosition = new Vector2(player.position.x, player.position.z);
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            Vector2 newChunkCoord = new Vector2(
                Mathf.FloorToInt(player.position.x / chunkSize),
                Mathf.FloorToInt(player.position.z / chunkSize)
            );

            if (newChunkCoord != currentChunkCoord)
            {
                currentChunkCoord = newChunkCoord;
                UpdateChunksBasedOnMovement();
            }
        }
    }

    void UpdateChunksBasedOnMovement()
    {
        Vector2 playerPosition = new Vector2(player.position.x, player.position.z);
        Vector2 movementDirection = (playerPosition - lastPlayerPosition).normalized; // Direction the player is moving
        lastPlayerPosition = playerPosition;

        // Determine which chunks to spawn based on the movement direction
        List<Vector2> chunksToSpawn = GetChunksToSpawn(movementDirection);

        foreach (var chunkCoord in chunksToSpawn)
        {
            if (!spawnedChunks.ContainsKey(chunkCoord))
            {
                SpawnChunk(chunkCoord);
            }
        }

        // Keep existing chunks within render distance
        HashSet<Vector2> chunksToKeep = new HashSet<Vector2>(chunksToSpawn);

        // Deactivate chunks that are out of render distance
        List<Vector2> chunksToRemove = new List<Vector2>();
        foreach (var chunk in spawnedChunks.Keys)
        {
            if (!chunksToKeep.Contains(chunk))
            {
                DeactivateChunk(chunk);
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            spawnedChunks.Remove(chunk);
        }
    }

    List<Vector2> GetChunksToSpawn(Vector2 movementDirection)
    {
        List<Vector2> chunksToSpawn = new List<Vector2>();

        // Convert direction to isometric grid directions (diagonal steps in a diamond shape)
        Vector2[] possibleDirections = new Vector2[]
        {
            new Vector2(1, 0),   // Right
            new Vector2(-1, 0),  // Left
            new Vector2(0, 1),   // Up
            new Vector2(0, -1)   // Down
        };

        foreach (var direction in possibleDirections)
        {
            // Check if the player is moving towards this direction
            if (Vector2.Dot(movementDirection, direction) > 0)
            {
                // Add chunks in this direction within render distance
                for (int i = 1; i <= renderDistance; i++)
                {
                    Vector2 chunkCoord = currentChunkCoord + direction * i;
                    chunksToSpawn.Add(chunkCoord);
                }
            }
        }

        return chunksToSpawn;
    }

    void SpawnChunk(Vector2 chunkCoord)
    {
        GameObject chunk = GetChunkFromPool();
        if (chunk == null)
        {
            chunk = Instantiate(ChooseChunkPrefab(chunkCoord));
        }

        // Position chunk according to isometric grid
        float xPos = (chunkCoord.x - chunkCoord.y) * (chunkSize / 2);
        float zPos = (chunkCoord.x + chunkCoord.y) * (chunkSize / 4); // Adjust for isometric height scaling
        chunk.transform.position = new Vector3(xPos, 0, zPos);
        chunk.SetActive(true);
        spawnedChunks.Add(chunkCoord, chunk);
    }

    void DeactivateChunk(Vector2 chunkCoord)
    {
        if (spawnedChunks.TryGetValue(chunkCoord, out GameObject chunk))
        {
            chunk.SetActive(false);
            chunkPool.Enqueue(chunk);
        }
    }

    GameObject GetChunkFromPool()
    {
        if (chunkPool.Count > 0)
        {
            return chunkPool.Dequeue();
        }
        return null;
    }

    GameObject ChooseChunkPrefab(Vector2 chunkCoord)
    {
        // Example of random selection
        int randomIndex = Random.Range(0, chunkPrefabs.Count);
        return chunkPrefabs[randomIndex];
    }
}
