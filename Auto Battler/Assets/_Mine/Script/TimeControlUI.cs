using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeControlUI : MonoBehaviour {
    [Header("General")]
    [SerializeField, Required] float _step = 0.5f;
    [SerializeField, Required, MinValue(0f)] float _minSpeed = 0.5f;
    [SerializeField, Required] float _maxSpeed = 2f;

    [Header("UI")]
    [SerializeField, Required] Button _slowButton, _fastButton;
    [SerializeField, Required] Image _playPauseImage;
    [SerializeField, Required] Sprite _playSprite;
    [SerializeField, Required] Sprite _pauseSprite;
    [SerializeField, Required] TextMeshProUGUI _text;

    bool _hasStarted = false;
    float _lastNonPauseTimeScale = 1f;

    private void Start() {
        _hasStarted = true;
        OnEnable();
    }

    private void OnEnable() {
        if (!_hasStarted) {
            return;
        }
        Time.timeScale = 1f;
        UpdatePlayPauseSprite();
        UpdateSlowFastInteractable();
        UpdateText();
    }

    void UpdatePlayPauseSprite() {
        _playPauseImage.sprite = IsPaused() ? _playSprite : _pauseSprite;
    }

    void UpdateSlowFastInteractable() {
        var ts = Time.timeScale;
        _slowButton.interactable = ts > _minSpeed;
        _fastButton.interactable = ts < _maxSpeed;
    }

    public static bool IsPaused() {
        return Mathf.Approximately(Time.timeScale, 0f);
    }

    void UpdateText() {
        _text.text = IsPaused() ? "Paused" :
            Mathf.Approximately(Time.timeScale, 1f) ? "" :
            $"x {Time.timeScale:F2}";
    }

    #region Button Events
    public void OnClickSlow() {
        if (IsPaused()) {
            Time.timeScale = _lastNonPauseTimeScale;
        }
        float targetTimeScale = Mathf.Max(Time.timeScale - _step, _minSpeed);
        Time.timeScale = targetTimeScale;
        UpdatePlayPauseSprite();
        UpdateSlowFastInteractable();
        UpdateText();
    }

    public void OnPlayPause() {
        if (!IsPaused()) {
            _lastNonPauseTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        } else {
            Time.timeScale = _lastNonPauseTimeScale;
        }
        UpdatePlayPauseSprite();
        UpdateText();
    }

    public void OnClickFast() {
        if (IsPaused()) {
            Time.timeScale = _lastNonPauseTimeScale;
        }
        float targetTimeScale = Mathf.Min(Time.timeScale + _step, _maxSpeed);
        Time.timeScale = targetTimeScale;
        UpdatePlayPauseSprite();
        UpdateSlowFastInteractable();
        UpdateText();
    }
    #endregion
}
