using UnityEngine;

public class ObjectIDComponent : MonoBehaviour
{
    public int ID;
    public Color lineColor = Color.white;

    public bool isConnected;

    public bool IsConnected
    {
        get { return isConnected; }
        set { isConnected = value; }
    }
}

