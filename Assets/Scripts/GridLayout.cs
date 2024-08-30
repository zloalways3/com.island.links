using UnityEngine;

[ExecuteInEditMode]
public class GridLayout : MonoBehaviour
{
    public int rows = 2; 
    public int columns = 2;  
    public Vector2 cellSize = new Vector2(1, 1);  
    public Vector2 spacing = new Vector2(0.5f, 0.5f); 

    void Start()
    {
        ArrangeChildrenInGrid();
    }

    void Update()
    {
        ArrangeChildrenInGrid();
    }

    void ArrangeChildrenInGrid()
    {
        int childCount = transform.childCount;
        float gridWidth = (columns - 1) * (cellSize.x + spacing.x);
        float gridHeight = (rows - 1) * (cellSize.y + spacing.y);
        Vector3 startPosition = new Vector3(-gridWidth / 2, gridHeight / 2, 0);

        for (int i = 0; i < childCount; i++)
        {
            int row = i / columns;
            int column = i % columns;

            Vector3 position = new Vector3(
                column * (cellSize.x + spacing.x),
                -row * (cellSize.y + spacing.y),
                0);

            Transform child = transform.GetChild(i);
            child.localPosition = startPosition + position;
            child.localScale = new Vector3(cellSize.x, cellSize.y, 1);
        }
    }
}
