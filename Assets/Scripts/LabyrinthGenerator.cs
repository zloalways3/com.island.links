using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
    public Texture2D labyrinthImage; 
    public GameObject wallPrefab; 
    public float wallSize; 

    void Start()
    {
        int width = labyrinthImage.width;
        int height = labyrinthImage.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = labyrinthImage.GetPixel(x, y);

                if (pixelColor.r < 0.5f)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector2(x * wallSize, y * wallSize), Quaternion.identity);
                    wall.transform.localScale = new Vector2(wallSize, wallSize);
                }
            }
        }
    }
}