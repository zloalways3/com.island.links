using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
    public Button[] levelButtons;  
    public Sprite lockedSprite;    
    public Sprite activeSprite;    
    public Sprite passedSprite;    
    public NextLevelButton NextLevelButton;
    public NextLevelButton ReplayLevelButton;

    private void Awake()
    {
        NextLevelButton.LevelButtonController = this;
        ReplayLevelButton.LevelButtonController = this;
    }

    private void OnEnable()
    {
        InitializeButtons();
    }

    public void InitializeButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button button = levelButtons[i];
            int levelStatus;

            if (i == 0)
            {
                levelStatus = PlayerPrefs.GetInt("Level" + i, 1); 
            }
            else
            {
                levelStatus = PlayerPrefs.GetInt("Level" + i, 0); 
            }

            switch (levelStatus)
            {
                case 0: // Заблокировано
                    SetButtonState(button, lockedSprite, false);
                    break;
                case 1: // Активно
                    SetButtonState(button, activeSprite, true);
                    break;
                case 2: // Пройдено
                    SetButtonState(button, passedSprite, true);
                    break;
            }
        }
    }

    void SetButtonState(Button button, Sprite sprite, bool interactable)
    {
        button.image.sprite = sprite;  
        button.interactable = interactable;  
    }


    public void LevelButtonInvoke(int nextLevelButtonNumber)
    {
        levelButtons[nextLevelButtonNumber].onClick.Invoke();
    }
}
