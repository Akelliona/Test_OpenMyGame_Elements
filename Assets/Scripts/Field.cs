using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum GameEventType
{
    MoveStart,
    MoveEnd
}

public class Field : MonoBehaviour
{
    [SerializeField]
    List<GameObject> elementPrefabs;

    Vector2 elementDistance = new Vector2(0.765f, 0.765f);
    List<List<Element>> view = new List<List<Element>>();

    int width = 0;
    int height = 0;

    Action<GameEventType> onEvent;
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

            while (view.Last().Count() < width) {
                view.Last().Add(null);
            }
        }
    }

    public void ClearLevel()
    {
        StopAllCoroutines();

        foreach (var row in view) {
            foreach (var column in row) {
                if (column != null) {
                    Destroy(column.gameObject);
                }
            }
        }

        view.Clear();

        height = 0;
        width = 0;
    }

    public bool IsClear()
    {
        int remains = 0;
        foreach (var row in view) {
            foreach (var column in row) {
                if (column != null) {
                    ++remains;
                }
            }
        }

        return remains == 0;
    }

    Vector3 GetPosFromIndex(int i, int j)
    {
        return transform.position + new Vector3((i + 0.5f - width * 0.5f) * elementDistance.x,
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
            Swap(pos, newPos);
        }
    }

    public void MoveLeft(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.left;

        if (newPos.x >= 0) {
            Swap(pos, newPos);
        }
    }

    public void MoveUp(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.up;

        if (view.Count > newPos.y && view[newPos.y].Count > newPos.x && view[newPos.y][newPos.x] != null) {
            Swap(pos, newPos);
        }
    }

    public void MoveDown(Element element)
    {
        var pos = element.pos;
        var newPos = pos + Vector2Int.down;

        if (newPos.y >= 0 && view[newPos.y].Count > newPos.x) {
            Swap(pos, newPos);
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

    void Swap(Vector2Int pos1, Vector2Int pos2)
    {
        onEvent?.Invoke(GameEventType.MoveStart);

        var e1 = view[pos1.y][pos1.x];
        var e2 = view[pos2.y][pos2.x];

        Move(e1, pos2);
        Move(e2, pos1);

        StartCoroutine(Normalize());
    }

    IEnumerator Normalize()
    {
        do {
            yield return new WaitForSeconds(1f);
            FallDown();
            yield return new WaitForSeconds(1f);
        } while (Blow());

        onEvent?.Invoke(GameEventType.MoveEnd);
    }

    void FallDown()
    {
        for(int x = 0; x < view[0].Count; ++x) {
            var lastFoundIdx = 0;
            for(int y = 0; y < view.Count; ++y) {
                lastFoundIdx = Math.Max(lastFoundIdx, y);
                var e = view[y][x];
                if (e != null) {
                    continue;
                }

                for (int k = lastFoundIdx + 1; k < view.Count; ++k) {
                    var found = view[k][x];
                    if (found != null) {
                        Move(found, new Vector2Int(x, y));
                        view[k][x] = null;
                        lastFoundIdx = k;
                        break;
                    }
                }

                if (view[y][x] == null) {
                    break;
                }
            }
        }
    }

    bool Blow()
    {
        List<Element> forBlow = new List<Element>();

        for (int y = 0; y < view.Count; ++y) {
            int sameInLine = 1;
            int startIdx = 0;
            for (int x = 0; x < view[y].Count; ++x) {
                if (view[y][x] != null) {
                    startIdx = x;
                    break;
                } else {
                    startIdx = x + 1;
                }
            }

            for (int x = startIdx + 1; x < view[y].Count; ++x) {
                if (view[y][startIdx] == null) {
                    startIdx = x + 1;
                    sameInLine = 0;
                    continue;
                }

                if (view[y][x] == null || view[y][x].Type != view[y][startIdx].Type) {
                    if (sameInLine >= 3) {
                        for (int k = startIdx; k < startIdx + sameInLine; ++k) {
                            forBlow.Add(view[y][k]);
                        }
                    }

                    startIdx = view[y][x] == null ? x + 1 : x;
                    sameInLine = view[y][x] == null ? 0 : 1;
                    continue;
                }

                if (view[y][x].Type == view[y][startIdx].Type) {
                    ++sameInLine;
                }
            }

            if (sameInLine >= 3) {
                for (int k = startIdx; k < startIdx + sameInLine; ++k) {
                    forBlow.Add(view[y][k]);
                }
            }
        }

        for (int x = 0; x < width; ++x) {
            int sameInLine = 0;
            int startIdx = 0;

            for (int y = 0; y < view.Count; ++y) {
                if (view[y][x] == null) {
                    break;
                }

                if (view[y][x].Type != view[startIdx][x].Type) {
                    if (sameInLine >= 3) {
                        for (int k = startIdx; k < startIdx + sameInLine; ++k) {
                            forBlow.Add(view[k][x]);
                        }
                    }

                    startIdx = y;
                    sameInLine = 1;
                    continue;
                }

                if (view[y][x].Type == view[startIdx][x].Type) {
                    ++sameInLine;
                }

            }

            if (sameInLine >= 3) {
                for (int k = startIdx; k < startIdx + sameInLine; ++k) {
                    if (!forBlow.Contains(view[k][x])) {
                        forBlow.Add(view[k][x]);
                    }
                }
            }
        }

        foreach(var e in forBlow) {
            var pos = e.pos;
            Destroy(e.gameObject);
            view[pos.y][pos.x] = null;
        }

        return forBlow.Count > 0;
    }

    public void OnEventRegister(Action<GameEventType> action)
    {
        onEvent += action;
    }

    public void OnEventUnregister(Action<GameEventType> action)
    {
        onEvent -= action;
    }
}
