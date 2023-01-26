using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class JunctionGate : MonoBehaviour
{
    public GateInfoEvent ON_ENTERED = new GateInfoEvent();
    public GateInfoEvent ON_EXIT = new GateInfoEvent();

    [SerializeField]
    private int nodeIndex;

    [SerializeField]
    private Node _exitNode;

    [SerializeField]
    private Intention direction;

    private GateInfo info;

    private void Awake()
    {
        info = new GateInfo(nodeIndex, direction, _exitNode);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<BaseFollower>() != null)
        {
            ON_ENTERED.Invoke(info);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<BaseFollower>() != null)
        {
            ON_EXIT.Invoke(info);
        }
    }
}

public class GateInfo
{
    public int index;
    public Intention direction;
    public Node exit;

    public GateInfo(int Index, Intention Direction, Node Exit)
    {
        index = Index;
        direction = Direction;
        exit = Exit;
    }
}


public class GateInfoEvent : UnityEvent<GateInfo>
{
}

