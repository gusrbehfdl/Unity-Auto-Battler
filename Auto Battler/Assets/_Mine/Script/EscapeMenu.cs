using Sirenix.OdinInspector;
using UnityEngine;

public class EscapeMenu : MonoBehaviour {
    [SerializeField, Required] TimeControlUI _timeControlUI;

    public void OnClickResume() {
        Close();
    }

    public void OpenClose() {
        if (gameObject.activeInHierarchy) {
            Close();
        } else {
            Open();
        }
    }

    void Open() {
        gameObject.SetActive(true);
        if (!TimeControlUI.IsPaused()) {
            _timeControlUI.OnPlayPause();
        }
    }

    void Close() {
        gameObject.SetActive(false);
        if (TimeControlUI.IsPaused()) {
            _timeControlUI.OnPlayPause();
        }
    }
}
