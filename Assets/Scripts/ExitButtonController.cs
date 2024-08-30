using UnityEngine;
using UnityEngine.UI;

public class ExitButtonController : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(Application.Quit);
    }
}
