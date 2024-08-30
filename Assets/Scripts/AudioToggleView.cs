using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggleView : MonoBehaviour
{
    [SerializeField] private Image _toggleIndicatorImage;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private Sprite _mutedSprite;
    [SerializeField] private Sprite _unmutedSprite;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _clickSoundSource;
    [SerializeField] private bool _controlsSound;

    private bool _isToggledOn = true;
    private Vector3 _initialImagePosition;

    void Start()
    {
        _toggleButton.onClick.AddListener(HandleToggleButtonClick);
        _initialImagePosition = _toggleIndicatorImage.rectTransform.position;

        _audioMixer.GetFloat("SoundsVolume", out float soundsValue);
        _audioMixer.GetFloat("MusicsVolume", out float musicValue);
        if (_controlsSound)
        {
            if (soundsValue == -80f)
            {
                MoveIndicatorToStart();
                SetImageToMuted();
                _isToggledOn = false;
            }
            else
            {
                MoveIndicatorRight();
                SetImageToUnmuted();
                _isToggledOn = true;
            }
        }
        else
        {
            if (musicValue == -80f)
            {
                MoveIndicatorToStart();
                SetImageToMuted();
                _isToggledOn = false;
            }
            else
            {
                MoveIndicatorRight();
                SetImageToUnmuted();
                _isToggledOn = true;
            }
        }
    }

    private void HandleToggleButtonClick()
    {
        _clickSoundSource.Play();
        if (_isToggledOn)
        {
            MoveIndicatorToStart();
            SetImageToMuted();
            SetMixerVolumeToMin();
        }
        else
        {
            MoveIndicatorRight();
            SetImageToUnmuted();
            SetMixerVolumeToMax();
        }
        _isToggledOn = !_isToggledOn;
    }

    private void MoveIndicatorToStart()
    {
        _toggleIndicatorImage.rectTransform.position = _initialImagePosition;
    }

    private void MoveIndicatorRight()
    {
        _toggleIndicatorImage.rectTransform.position += new Vector3(0.35f, 0f, 0f);
    }

    private void SetImageToMuted()
    {
        _toggleIndicatorImage.sprite = _mutedSprite;
    }

    private void SetImageToUnmuted()
    {
        _toggleIndicatorImage.sprite = _unmutedSprite;
    }

    private void SetMixerVolumeToMin()
    {
        if (_controlsSound)
        {
            _audioMixer.SetFloat("SoundsVolume", -80);
        }
        else
        {
            _audioMixer.SetFloat("MusicsVolume", -80);
        }
    }

    private void SetMixerVolumeToMax()
    {
        if (_controlsSound)
        {
            _audioMixer.SetFloat("SoundsVolume", 0);
        }
        else
        {
            _audioMixer.SetFloat("MusicsVolume", 0);
        }
    }
}
