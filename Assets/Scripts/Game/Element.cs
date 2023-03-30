using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType 
{
    None,
    Fire,
    Water
}

public class Element : MonoBehaviour
{
    [SerializeField]
    ElementType type;
    public Vector2Int pos { get; private set; }

    public ElementType Type { get { return type; } private set { type = value; } }

    Animator animator;
    public Action onDestroyAnimationEnd;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(0, -1, UnityEngine.Random.value);
    }

    public void UpdatePos(int i, int j)
    {
        pos = new Vector2Int(i, j);
        GetComponent<SpriteRenderer>().sortingOrder = i * 10 + j; 
    }

    public void Destroy()
    {
        animator.SetTrigger("die");        
    }

    public void OnDestroyAnimationFinished()
    {
        onDestroyAnimationEnd?.Invoke();
        Destroy(gameObject);
    }
}
