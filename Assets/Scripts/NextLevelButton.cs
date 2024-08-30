using System;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    public LevelButtonController LevelButtonController { get; set; }

    [SerializeField] private GameObject _levelsBlock;
    [SerializeField] private GameObject _scene;
    [SerializeField] private BlockVisibilityToggle _blockVisibilityToggle;
    [SerializeField] private PopupButtons _popupButtons;

    private ObjectConnectorManager _connectorManager;
    private int _currentLevelNumber = -1;  

    private void Awake()
    {
        _blockVisibilityToggle.OnBlocksToggled += GoToNextLevel;
    }

    private void OnDisable()
    {
       
    }

    public void SubscibeToCompletedLevel(ObjectConnectorManager objectConnectorManager)
    {
        _connectorManager = objectConnectorManager;
        _connectorManager.OnLevelCompleted += UpdateCurrentLevelIndex;
    }

    private void UpdateCurrentLevelIndex(int levelNumber)
    {
        _currentLevelNumber = levelNumber;
    }

    private void GoToNextLevel()
    {
        for (int i = 0; i < _scene.transform.childCount; i++)
        {
            GameObject level = _scene.transform.GetChild(i).gameObject;
            Destroy(level);
        }

        if(_popupButtons == PopupButtons.Next)
        {
            if (_currentLevelNumber == LevelButtonController.levelButtons.Length)
            {
                _levelsBlock.SetActive(true);
                return;
            }

            LevelButtonController.LevelButtonInvoke(_currentLevelNumber);
        }
        else
        {
            if (_currentLevelNumber > 0)
            {
                LevelButtonController.LevelButtonInvoke(_currentLevelNumber - 1);
            }
        }
    }
}
