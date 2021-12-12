using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FieldMaterialChanger : MonoBehaviour
{
    [SerializeField] private GameObject vfxHandler;
    [SerializeField] private Material lightMaterial;
    [SerializeField] private Material darkMaterial;

    [SerializeField] private VisualEffect smokeVfx;

    [SerializeField] private List<VisualEffect> selectionVfx;
    
    private MeshRenderer mesh;
    private FieldType _type = FieldType.Unlit;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // public void ToggleMaterial(bool? select = null)
    // {
    //     if (select.HasValue) selected = select.Value;
    //     else selected = !selected;
    //
    //     var newMaterial = selected ? lightMaterial : darkMaterial;
    //     
    //     mesh.material = newMaterial;
    // }

    public void SetMaterial(FieldType type)
    {
        var newMaterial = type switch
        {
            FieldType.Unlit => darkMaterial,
            FieldType.Lit => lightMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (type == FieldType.Unlit && _type == FieldType.Lit)
        {
            smokeVfx.Play();
            AudioManager.queueSound(AudioType.UnTile);
        }

        _type = type;
        mesh.material = newMaterial;
    }

    public void SetHighlightColor(LogicOperation opr)
    {
        foreach (var vfx in selectionVfx)
        {
            Vector4 color = opr switch
            {
                LogicOperation.EMPTY => Color.white,
                LogicOperation.OR => Color.green,
                LogicOperation.AND => Color.cyan,
                LogicOperation.NOT => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(opr), opr, null)
            };
            vfx.SetVector4("Color", color);
        }
    }
    
    public void SetHighlight(bool on)
    {
        // vfxHandler.SetActive(on);

        foreach (var vfx in selectionVfx)
        {
            if (on)
            {
                vfx.Play();
            }
            else
            {
                vfx.Stop();
            }
        }
    }
    
}
