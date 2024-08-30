using UnityEngine;

public class BackMenuLevelView : MonoBehaviour
{
    [SerializeField] private BlockVisibilityToggle _blockVisibilityToggle;
    public GameObject _scene;

    private void Awake()
    {
        _blockVisibilityToggle.OnBlocksToggled += OnButtonClick;
    }

    private void OnButtonClick()
    {
        for (int i = 0; i < _scene.transform.childCount; i++)
        {
            GameObject level = _scene.transform.GetChild(i).gameObject;
            Destroy(level);
        }
    }
}
