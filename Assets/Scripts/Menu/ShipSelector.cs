
using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    [SerializeField]
    private Transform[] _ships;

    private int _index = 0;

    // Start is called before the first frame update
    void Start()
    {
        _index = 0;
        StaticEyeDriveSession.INSTANCE.ship = _index;
        Draw();
    }

    public void OnNext()
    {
        _index = (int)Mathf.Repeat(_index + 1, _ships.Length);
        StaticEyeDriveSession.INSTANCE.ship = _index;
        Draw();
    }

    public void OnPrevious()
    {
        _index = (int)Mathf.Repeat(_index - 1, _ships.Length);
        StaticEyeDriveSession.INSTANCE.ship = _index;
        Draw();
    }

    private void Draw()
    {
        for (int i = 0; i < _ships.Length; i++) _ships[i].gameObject.SetActive(i == _index);
    }

}
