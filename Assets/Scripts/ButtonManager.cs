using UnityEngine;
using Zenject;

public class ButtonManager : ITickable {

    private float _timer;
    private int _count => _side.Count;

    private Side _side;

    public ButtonManager(Side side) {
        _side = side;
    }

    public float GetCount() { return _count; }

    public float GetTimer() { return _timer; }

    public void Tick() {
        _timer -= Time.deltaTime;
    }

}