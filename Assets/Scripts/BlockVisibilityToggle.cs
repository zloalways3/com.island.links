using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlockVisibilityToggle : MonoBehaviour
{
    public event Action OnBlocksToggled;

    [FormerlySerializedAs("_currentBlocks")][SerializeField] private List<GameObject> blocksToHide;
    [FormerlySerializedAs("_targetBlocks")][SerializeField] private List<GameObject> blocksToShow;
    [SerializeField] private Button toggleButton;

    private void Awake()
    {
        toggleButton.onClick.AddListener(ToggleBlocksVisibility);
    }

    private void ToggleBlocksVisibility()
    {
        foreach (GameObject block in blocksToHide)
        {
            block.SetActive(false);
        }
        foreach (GameObject block in blocksToShow)
        {
            block.SetActive(true);
        }

        OnBlocksToggled?.Invoke();
    }
}
