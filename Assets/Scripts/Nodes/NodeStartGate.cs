using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(Node))]
[ExecuteInEditMode]
public class NodeStartGate : MonoBehaviour
{
    private Node _node;

    [SerializeField]
    private float _splineWidth = 10;

    [SerializeField]
    private float _forwardPadding = 0;

    private void OnEnable()
    {
        Build();
    }

    private void Awake()
    {
        Build();
    }

    private void Build()
    {
        if(GetComponentsInChildren<Transform>().Length == 1)
        {
            _node = GetComponent<Node>();
            Node.Connection connection = _node.GetConnections()[0];

            SplinePoint point = connection.computer.GetPoint(connection.pointIndex);
            SplineResult result = connection.computer.Evaluate(connection.computer.GetPointPercent(connection.pointIndex) + .005f);

            GameObject go = new GameObject();
            go.transform.SetParent(this.transform, false);
            go.name = "StartGate";
            go.transform.LookAt(result.position);
            go.transform.Translate(Vector3.forward * _forwardPadding);

            BoxCollider box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(_splineWidth, _splineWidth, 1f);
            box.isTrigger = true;
            box.tag = "NodeStartGate";
        }
    }
}
