using Dante.Dijkstra;
using UnityEngine;

public class Connection : MonoBehaviour
{
    #region Runtime Variables

    [Header("Runtime Variables")]
    [SerializeField] public Node nodeA;
    [SerializeField] public Node nodeB;
    [SerializeField] protected float distanceBetweenNodes;

    protected Vector3 origin;
    protected Vector3 direction;

    #endregion

    #region Debug

    [SerializeField, HideInInspector] GameObject nodeALabel;
    [SerializeField, HideInInspector] GameObject nodeBLabel;
    [SerializeField, HideInInspector] GameObject distanceLabel;

    #endregion

    #region UnityMethods

    private void OnDrawGizmos()
    {
        if(nodeA != null && nodeB != null)
        {
            origin = nodeA.transform.position;
            direction = nodeB.transform.position - origin;
            Debug.DrawRay(origin, direction, Color.magenta);
            distanceBetweenNodes = direction.magnitude;
            transform.position = origin + direction / 2f;
            
            nodeALabel.name = nodeA.transform.GetChild(0).name;
            nodeBLabel.name = nodeB.transform.GetChild(0).name;
            distanceLabel.name = distanceBetweenNodes.ToString();
            
        }
    }

    #endregion

    #region Public Methods

    public Node OpposingNode(Node value)
    {
        if(value == nodeA ||  value == nodeB)
        {
            if(value == nodeA)
            {
                return nodeB;
            }
            if(value == nodeB)
            {
                return nodeB;
            }
        }
        Debug.LogError(this.name + " " + gameObject.name + " - Node " + value.name +
            " is asking for a connection not valid with " + nodeA.name + " - " + nodeB.name + ".", value.gameObject);
        return null;
    }

    #endregion


}
