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

abstract class Swipe : ISwipe
{
    public float minDistanceToRegisterSwipe = 0.55f;

    bool[] swipeRegistered = new bool[4];
    Element ISwipe.Target { get => target; set => target = value; }

    Element target;
    public bool GetSwipe(ISwipe.Type type)
    {
        return swipeRegistered[(int)type];
    }

    abstract public void Update();

    protected void GetElementFromClickPos(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        target = hit.collider ? hit.collider.GetComponent<Element>() : null;
    }

    protected void ClearRegisteredSwipes()
    {
        for (int i = 0; i < swipeRegistered.Length; i++) {
            swipeRegistered[i] = false;
        }
    }
    protected void RegisterSwipe(Vector3 endPos)
    {
        var delta = endPos - target.transform.position;
        delta.z = 0;

        if (target != null && delta.sqrMagnitude > minDistanceToRegisterSwipe * minDistanceToRegisterSwipe) {
            ISwipe.Type type = ISwipe.Type.Right;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
                type = delta.x > 0 ? ISwipe.Type.Right : ISwipe.Type.Left;
            } else {
                type = delta.y > 0 ? ISwipe.Type.Top : ISwipe.Type.Bottom;
            }

            swipeRegistered[(int)type] = true;
        }
    }
}
class MouseSwipe : Swipe
{
    override public void Update()
    {
        ClearRegisteredSwipes();

        if (Input.GetMouseButtonDown(0)) {
            var startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new UnityEngine.Vector2(startPos.x, startPos.y);

            GetElementFromClickPos(mousePos2D);
        } else if(Input.GetMouseButtonUp(0)) {
            var endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RegisterSwipe(endPos);       
        }        
    }

}

class AndroidSwipe : Swipe
{    override public void Update()
    {
        ClearRegisteredSwipes();

        if (Input.touchCount >= 0) {
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began: {
                    var startPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    var pos = new UnityEngine.Vector2(startPos.x, startPos.y);

                    GetElementFromClickPos(pos);
                    } break;
                case TouchPhase.Ended: {
                        var endPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        RegisterSwipe(endPos);
                    } break;
            }
        }
    }
}

public class InputPlayerController : MonoBehaviour
{
    [SerializeField]
    Field field;

    bool blocked;

    ISwipe swipe = new MouseSwipe();

    private void Start()
    {
        swipe = Input.touchSupported ? new AndroidSwipe() : new MouseSwipe();
    }

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
