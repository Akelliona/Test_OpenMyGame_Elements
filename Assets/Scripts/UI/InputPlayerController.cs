using UnityEngine;
using UnityEngine.UI;

interface ISwipe 
{
    enum Type 
    {
        Left,
        Right, 
        Top,
        Bottom
    }

    public void Update();
    public bool GetSwipe(Type type);

    public Element Target { get; protected set; }
}
class MouseSwipe : ISwipe 
{
    public float minDistanceToRegisterSwipe = 0.55f;
    Vector3 startPos;

    Element target;

    bool[] swipeRegistered = new bool[4];

    Element ISwipe.Target { get => target; set => target = value; }

    void ISwipe.Update()
    {
        for(int i = 0; i < swipeRegistered.Length; i++) {
            swipeRegistered[i] = false;
        }

        if (Input.GetMouseButtonDown(0)) {
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new UnityEngine.Vector2(startPos.x, startPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            target = hit.collider ? hit.collider.GetComponent<Element>() : null;
        } else if(Input.GetMouseButtonUp(0)) {
            var endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var delta = endPos - startPos;
            if (target != null && delta.sqrMagnitude > minDistanceToRegisterSwipe * minDistanceToRegisterSwipe) {
                ISwipe.Type type = ISwipe.Type.Right;
                if ( Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
                    type = delta.x > 0 ? ISwipe.Type.Right : ISwipe.Type.Left;                    
                } else {
                    type = delta.y > 0 ? ISwipe.Type.Top : ISwipe.Type.Bottom;
                }
                
                swipeRegistered[(int)type] = true;
            }            
        }        
    }
    bool ISwipe.GetSwipe(ISwipe.Type type)
    {
        return swipeRegistered[(int)type];
    }
}

public class InputPlayerController : MonoBehaviour
{
    [SerializeField]
    Field field;

    bool blocked;

    ISwipe swipe = new MouseSwipe();

    void Update()
    {
        swipe.Update();

        if (blocked)
            return;

        if (swipe.GetSwipe(ISwipe.Type.Left)) {
            field.MoveLeft(swipe.Target);
        } else if (swipe.GetSwipe(ISwipe.Type.Right)) {
            field.MoveRight(swipe.Target);
        } else if (swipe.GetSwipe(ISwipe.Type.Top)) {
            field.MoveUp(swipe.Target);
        } else if (swipe.GetSwipe(ISwipe.Type.Bottom)) {
            field.MoveDown(swipe.Target);
        }
    }

    public void Block()
    {
        blocked = true;
    }

    public void Unblock()
    {
        blocked = false;
    }

    public bool IsBlocked()
    {
        return blocked;
    }

}
