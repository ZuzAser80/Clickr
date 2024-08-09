using Assets.Scripts.Sides;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIHandler : ITickable
{
    private TextMeshProUGUI _countText;
    private Image _slider;
    private Side _currentSide;
    private Button _button;

    public UIHandler (TextMeshProUGUI text, Side side, Image slider, Button button) {
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