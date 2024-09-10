using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public string targetSortingLayer = "Player"; // Set this to the sorting layer used for dynamic sorting
    public int sortingOrderOffset = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the object is on the correct sorting layer for dynamic sorting
        if (spriteRenderer.sortingLayerName != targetSortingLayer)
        {
            Debug.LogWarning($"{gameObject.name} is not on the target sorting layer: {targetSortingLayer}. Sorting order may not work as intended.");
        }
    }

    private void Update()
    {
        // Only adjust sorting order if the object is on the target sorting layer
        if (spriteRenderer.sortingLayerName == targetSortingLayer)
        {
            spriteRenderer.sortingOrder = (int)(-transform.position.y * 100) + sortingOrderOffset;
        }
    }
}
