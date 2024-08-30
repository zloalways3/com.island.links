using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _levelPrefab;
    [SerializeField] private GameObject _sceneContainer;
    [SerializeField] private Transform _parentTransform; 
    [SerializeField] private BlockVisibilityToggle _blockVisibilityToggle; 
    [SerializeField] private int lvlnum; 

    private Text _buttonText; 
    private LevelButtonController _levelButtonController; 

    private void Awake()
    {
        _blockVisibilityToggle.OnBlocksToggled += SpawnLevel;
        _levelButtonController = GetComponentInParent<LevelButtonController>();
        _buttonText = GetComponent<Button>().GetComponentInChildren<Text>();

        if (_buttonText != null)
        {
            _buttonText.text = lvlnum.ToString();
        }
    }

    private void SpawnLevel()
    {
        ObjectConnectorManager objectConnectorManager = Instantiate(_levelPrefab, Vector3.zero, Quaternion.identity, _parentTransform).GetComponent<ObjectConnectorManager>();
        objectConnectorManager.LevelNumber = lvlnum;
        _levelButtonController.NextLevelButton.SubscibeToCompletedLevel(objectConnectorManager);
        _levelButtonController.ReplayLevelButton.SubscibeToCompletedLevel(objectConnectorManager);
    }
}
