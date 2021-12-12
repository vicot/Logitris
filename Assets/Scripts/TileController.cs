using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TileController : MonoBehaviour
{
    [SerializeField] private BrickManager brickManager;
    [SerializeField] private TextMeshProUGUI bricksLeftLabel;

    [SerializeField] private FieldManager fieldManager;

    private Brick currentBrick;
    private List<Field> _fields = new List<Field>();

 
    private void FetchBrick()
    {
        transform.position = Vector3.zero;
        currentBrick = brickManager.GetNextBrick();
        currentBrick.Obj.transform.parent = transform;
    }
    
    private void FixedUpdate()
    {
        if(currentBrick is null) FetchBrick();
        
        var screenPos = Mouse.current.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 100.0f, LayerMask.GetMask("ground")))
        {
            var newX = math.clamp(hit.point.x, -10f, -1f);
            transform.position = new Vector3(newX+1.0f, 2.5f, hit.point.z);

            //clear old highlights and set new

            var oldFields = _fields;
            _fields = fieldManager.FindFields(ray, currentBrick);

            foreach (var field in oldFields)
            {
                if(_fields.All(f => f.visual.name != field.visual.name))
                    field.SetHighlight(false);
            }
            
            foreach (var field in _fields)
            {
                if(oldFields.All(f => f.visual.name != field.visual.name))
                    field.SetHighlight(true);
            }
        }
        
    }

    IEnumerator FinishGame(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void DropBrick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (currentBrick is null) return;
        if (_fields.Count == 0) return;

        
        fieldManager.ApplyBrick(currentBrick, _fields);
        brickManager.DecreaseBricks();
        bricksLeftLabel.text = brickManager.BricksLeft().ToString();
        Destroy(currentBrick.Obj);

        if (brickManager.BricksLeft() == 0)
        {
            StartCoroutine(FinishGame(1.0f));
            return;
        }
        
        FetchBrick();
    }
}
