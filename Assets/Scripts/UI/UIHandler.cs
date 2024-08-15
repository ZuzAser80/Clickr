using Assets.Scripts.Sides;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiHandler : ITickable
{
    private TextMeshProUGUI _countText;
    private Image _slider;
    private CameraFieldSide _currentSide;
    private Button _button;

    public UiHandler (TextMeshProUGUI text, CameraFieldSide side, Image slider, Button button) {
        _countText = text;
        _currentSide = side;
        _slider = slider;
        _button = button;
        _button.onClick.AddListener(delegate { _currentSide.Click.Invoke(); });
    }

    public void Tick()
    {
        if(Input.GetKeyDown(KeyCode.Space)) { _button.onClick.Invoke(); }
        _countText.text = _currentSide.GetCount().ToString();
        _slider.fillAmount = _currentSide.GetTimer();
    }
}