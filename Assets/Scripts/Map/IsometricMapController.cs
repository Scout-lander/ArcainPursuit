using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricMapController : MonoBehaviour
{
    public List<GameObject> terrainChunks; // List of chunk prefabs
    public GameObject player;              // Player object reference
    public float checkerRadius = 1f;       // Radius to check for existing chunks
    public LayerMask terrainMask;          // Layer mask for chunks
    public GameObject currentChunk;        // The chunk the player is currently in
    private Vector3 playerLastPosition;    // To track player movement direction

    [Header("Chunk Settings")]
    public float chunkSize = 120f;         // Size of each chunk in the isometric layout
    public GameObject terrainParent;       // Parent GameObject for all spawned chunks

    [Header("Optimization")]
    public List<GameObject> spawnedChunks; // List of all spawned chunks
    private GameObject latestChunk;        // Reference to the latest spawned chunk
    public float maxOpDist = 300f;         // Distance beyond which chunks are deactivated
    private float optimizerCooldown;       // Timer for the optimization check
    public float optimizerCooldownDur = 1f; // Duration between optimization checks

    void Start()
    {
        playerLastPosition = player.transform.position;

        // Ensure currentChunk is set correctly initially
        UpdateCurrentChunk();

        // Check if terrainParent is set, if not, create a new parent object
        if (terrainParent == null)
        {
            terrainParent = new GameObject("TerrainChunks");
        }
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            UpdateCurrentChunk(); // Update current chunk if it's not set
            if (!currentChunk) return; // If still not set, skip
        }

        // Calculate player movement direction
        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        // If the player is not moving, skip checking
        if (moveDir == Vector3.zero) return;

        // Normalize the direction to determine chunk spawning
        Vector3 moveDirection = moveDir.normalized;

        // Determine chunk positions to spawn based on the primary movement direction
        List<Vector3> positionsToCheck = GetSpawnPositions(moveDirection);

        // Check and spawn chunks in the calculated positions
        foreach (var position in positionsToCheck)
        {
            CheckAndSpawnChunk(position);
        }
    }

    void UpdateCurrentChunk()
    {
        // Use a raycast or overlap check to update the current chunk based on player's position
        Collider2D hit = Physics2D.OverlapCircle(player.transform.position, checkerRadius, terrainMask);
        if (hit)
        {
            currentChunk = hit.gameObject;
        }
    }

    List<Vector3> GetSpawnPositions(Vector3 moveDirection)
    {
        // Define possible isometric directions
        Vector3 right = new Vector3(1, 0, 0.5f);
        Vector3 left = new Vector3(-1, 0, -0.5f);
        Vector3 up = new Vector3(0.5f, 0, 1);
        Vector3 down = new Vector3(-0.5f, 0, -1);
        Vector3 rightUp = right + up;    // Diagonal Right Up
        Vector3 rightDown = right + down; // Diagonal Right Down
        Vector3 leftUp = left + up;     // Diagonal Left Up
        Vector3 leftDown = left + down;  // Diagonal Left Down

        List<Vector3> positionsToCheck = new List<Vector3>();

        // Check main directions and add adjacent diagonal directions as needed
        if (Vector3.Dot(moveDirection, right) > 0.5f) // Moving Right
        {
            positionsToCheck.Add(currentChunk.transform.position + right * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + rightUp * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + rightDown * chunkSize);
        }
        if (Vector3.Dot(moveDirection, left) > 0.5f) // Moving Left
        {
            positionsToCheck.Add(currentChunk.transform.position + left * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + leftUp * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + leftDown * chunkSize);
        }
        if (Vector3.Dot(moveDirection, up) > 0.5f) // Moving Up
        {
            positionsToCheck.Add(currentChunk.transform.position + up * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + rightUp * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + leftUp * chunkSize);
        }
        if (Vector3.Dot(moveDirection, down) > 0.5f) // Moving Down
        {
            positionsToCheck.Add(currentChunk.transform.position + down * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + rightDown * chunkSize);
            positionsToCheck.Add(currentChunk.transform.position + leftDown * chunkSize);
        }

        return positionsToCheck;
    }

    void CheckAndSpawnChunk(Vector3 spawnPosition)
    {
        // Check if a chunk already exists at the desired position
        Collider2D hit = Physics2D.OverlapCircle(spawnPosition, checkerRadius, terrainMask);
        if (!hit)
        {
            SpawnChunk(spawnPosition);
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity, terrainParent.transform);
        spawnedChunks.Add(latestChunk);

        // Update current chunk if the player is close to the new chunk
        if (Vector3.Distance(player.transform.position, spawnPosition) < chunkSize)
        {
            currentChunk = latestChunk;
        }
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur; // Reset the cooldown
        }
        else
        {
            return; // Skip optimization if cooldown is not complete
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            float opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false); // Deactivate chunks beyond the maximum distance
            }
            else
            {
                chunk.SetActive(true); // Activate chunks within the maximum distance
            }
        }
    }
}
