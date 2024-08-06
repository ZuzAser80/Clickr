using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIHandler : ITickable
{
    private TextMeshProUGUI _countText;
    private Slider _slider;
    private Side _currentSide;
    private Button _button;
    private ButtonManager _manager;

    public UIHandler (TextMeshProUGUI text, Side side, Slider slider, Button button, ButtonManager manager) {
        _countText = text;
        _currentSide = side;
        _slider = slider;
        _button = button;
        _manager = manager;
        _button.onClick.AddListener(delegate { _currentSide.Click.Invoke(); });
    }

    public void Tick()
    {
        _countText.text = _manager.GetCount().ToString();
        _slider.value = _manager.GetTimer();

    }
}