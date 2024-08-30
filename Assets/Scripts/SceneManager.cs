using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject Levels => _levels;
    public GameObject Winner => _winner;

    [SerializeField] private GameObject _levels;
    [SerializeField] private GameObject _winner;
}
