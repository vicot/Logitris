using UnityEngine;

public class Field
{
    public GameObject visual { get; }
    private LogicOperation _operator;
    public LogicOperation Operator
    {
        get { return _operator;}
        set { _operator = value; materialChanger.SetHighlightColor(_operator); }
    }
    private FieldMaterialChanger materialChanger;

    private FieldType _type;
    public FieldType Type
    {
        get { return _type; }
        set { _type = value; materialChanger.SetMaterial(_type); }
    }

    public Field(GameObject obj)
    {
        visual = obj;
        materialChanger = visual.GetComponent<FieldMaterialChanger>();
        Type = FieldType.Unlit;
    }
    
    public void SetHighlight(bool on)
    {
        materialChanger.SetHighlight(on);
    }

}