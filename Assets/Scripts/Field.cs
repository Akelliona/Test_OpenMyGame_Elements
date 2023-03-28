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
}
