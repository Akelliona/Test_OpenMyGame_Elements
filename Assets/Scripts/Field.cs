using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    List<GameObject> elementPrefabs;

    Vector2 elementDistance = new Vector2(0.765f, 0.765f); 
    List<List<Element>> view = new List<List<Element>>();

    int width = 0;
    int height = 0;

    public void CreateLevelFrom(string[] level)
    {
        height = level.Length;
        foreach (var row in level) {
            view.Add(new List<Element>());
            var columns = row.Split(' ');
            width = Math.Max(width, columns.Length);
            foreach (var column in columns) {
                ElementType type = (ElementType)int.Parse(column);
                switch (type) {
                    case ElementType.None:
                        view.Last().Add(null);
                        break;
                    default:
                        var i = view.Last().Count();
                        var j = view.Count() - 1;

                        var newElement = Instantiate(GetElementPrefab(type), GetPosFromIndex(i, j), Quaternion.identity, transform);
                        var elementComp = newElement.GetComponent<Element>();
                        elementComp.UpdatePos(i, j);

                        view.Last().Add(elementComp);
                        break;
                }
            }

            while(view.Last().Count() < width) {
                view.Last().Add(null);
            }
        }
    }

    public void ClearLevel()
    {
        foreach(var row in view) {
            foreach(var column in row) {
                if (column != null) {
                    Destroy(column.gameObject);
                }
            }
        }

        view.Clear();
        height = 0;
        width = 0;
    }

    Vector3 GetPosFromIndex(int i, int j)
    {
        return transform.position + new Vector3( (i + 0.5f - width * 0.5f) * elementDistance.x,
            j * elementDistance.y);
    }

    GameObject GetElementPrefab(ElementType type)
    {
        return elementPrefabs.Find(x => x.GetComponent<Element>().Type == type);
    }

    public void MoveRight(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.right;

        if (newPos.x < width) {
            var other = view[newPos.y][newPos.x];

            Move(element, newPos);
            Move(other, pos);
        }
    }

    public void MoveLeft(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.left;

        if (newPos.x >= 0) {
            var other = view[newPos.y][newPos.x];

            Move(element, newPos);
            Move(other, pos);
        }
    }

    public void MoveUp(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.up;

        if (view.Count > newPos.y && view[newPos.y].Count > newPos.x && view[newPos.y][newPos.x] != null) {
            var other = view[newPos.y][newPos.x];

            Move(element, newPos);
            Move(other, pos);
        }
    }

    public void MoveDown(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.down;

        if (newPos.y >= 0 && view[newPos.y].Count > newPos.x) {
            var other = view[newPos.y][newPos.x];

            Move(element, newPos);
            Move(other, pos);
        }
    }

    void Move(Element e, Vector2Int pos)
    {
        view[pos.y][pos.x] = e;
        if (e != null) {
            e.transform.position = GetPosFromIndex(pos.x, pos.y);
            e.UpdatePos(pos.x, pos.y);
        }
    }
}
