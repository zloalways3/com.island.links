using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConnectorManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _toDeactivate;
    [SerializeField] private List<GameObject> _toActivate;
    [SerializeField] private List<ObjectIDComponent> _component;

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    private bool IsWinner = false;

    public int LevelNumber { get; set; }

    public event Action<int> OnLevelCompleted;

    private void Awake()
    {
        _toActivate = new List<GameObject>();
        SceneManager sceneManager = GetComponentInParent<SceneManager>();
        _toDeactivate.Add(sceneManager.Levels);
        _toActivate.Add(sceneManager.Winner);
    }

    private void Update()
    {
        CheckAllConnected();
        ChangeBlock();
    }

    private void CheckAllConnected()
    {
        if (IsWinner) return;
        foreach (ObjectIDComponent component in _component)
        {
            if (!component.isConnected)
            {
                return;
            }
        }

        IsWinner = true;
    }

    private void ChangeBlock()
    {
        if (IsWinner)
        {
            //Debug.Log("WIN-2");
            SaveCompletedLevel(LevelNumber - 1);

            OnLevelCompleted?.Invoke(LevelNumber);
            Debug.Log("Current level " + LevelNumber);

            foreach (GameObject block in _toDeactivate)
            {
                block.SetActive(false);
            }
            foreach (GameObject block in _toActivate)
            {
                block.SetActive(true);
            }
            IsWinner = false;
            ResetAllConnections();
        }
    }

    public void AddLine(LineRenderer lineRenderer)
    {
        if (!_lineRenderers.Contains(lineRenderer))
        {
            _lineRenderers.Add(lineRenderer);
        }
    }

    public void RemoveLine(LineRenderer lineRenderer)
    {
        if (_lineRenderers.Contains(lineRenderer))
        {
            _lineRenderers.Remove(lineRenderer);
        }
    }

    public void ResetAllConnections()
    {
        foreach (var component in _component)
        {
            component.isConnected = false;
        }
    }

    private void SaveCompletedLevel(int levelIndex)
    {

        PlayerPrefs.SetInt("Level" + levelIndex, 2); 

        if (levelIndex + 1 < 8 && PlayerPrefs.GetInt("Level" + (levelIndex + 1)) != 2)
        {
            PlayerPrefs.SetInt("Level" + (levelIndex + 1), 1); 
        }
    }
}
