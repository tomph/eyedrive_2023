using System;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

public class ThreeWayJunction : MonoBehaviour
{
    [SerializeField]
    private Node _connectedNode;

    public JunctionAddressEvent ON_JUNCTION_TRIGGERED = new JunctionAddressEvent();
    public UnityEvent ON_JUNCTION_EXIT = new UnityEvent();

    private JunctionAddress _address;

    private void Awake()
    {
        _address = new JunctionAddress(_connectedNode);

        JunctionGate[] gates = GetComponentsInChildren<JunctionGate>();
        foreach(JunctionGate g in gates)
        {
            g.ON_EXIT.AddListener(OnGateExited);
            g.ON_ENTERED.AddListener(OnGateEntered);
        }
    }

    private void OnGateExited(GateInfo obj)
    {
        ON_JUNCTION_EXIT.Invoke();
    }

    private void OnGateEntered(GateInfo info)
    {
        _address.index = info.index;
        _address.direction = info.direction;
        _address.exit = info.exit;

        ON_JUNCTION_TRIGGERED.Invoke(_address);
    }
}

public class JunctionAddress
{
    public Node node;
    public Node exit;
    public int index;
    public Intention direction;

    public JunctionAddress(Node connectedNode)
    {
        node = connectedNode;
    }
}

[Serializable]
public class JunctionAddressEvent : UnityEvent<JunctionAddress>
{
    
}