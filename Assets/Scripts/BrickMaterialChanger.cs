using UnityEngine;

public class BrickMaterialChanger : MonoBehaviour
{
    [SerializeField] private Material emptyMaterial;
    [SerializeField] private Material sumMaterial;
    [SerializeField] private Material andMaterial;
    [SerializeField] private Material notMaterial;
    public void ChangeMaterial(LogicOperation operation)
    {
        switch(operation)
        {
            case LogicOperation.OR:
                GetComponent<MeshRenderer>().material = sumMaterial;
                break;
            case LogicOperation.AND:
                GetComponent<MeshRenderer>().material = andMaterial;
                break;
            case LogicOperation.NOT:
                GetComponent<MeshRenderer>().material = notMaterial;
                break;
            case LogicOperation.EMPTY:
                GetComponent<MeshRenderer>().material = emptyMaterial;
                break;
        }
    }
}
