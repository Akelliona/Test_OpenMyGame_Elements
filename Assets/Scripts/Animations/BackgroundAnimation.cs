using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField]
    float minBalloonTimeFly;
    [SerializeField]
    float maxBalloonTimeFly;

    [SerializeField]
    float minRelativeHeight;
    [SerializeField]
    float maxRelativeHeight;

    [SerializeField]
    List<GameObject> balloonVariants;

    [SerializeField]
    int maxBalloonsOnField = 3;

    List<GameObject> balloons = new List<GameObject>();

    float minDeltaForGeneration = 1f;
    float timeLastGenerateBalloon = 0;

    private void Start()
    {
        GenerateBalloon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeLastGenerateBalloon + minDeltaForGeneration && balloons.Count < maxBalloonsOnField) {
            timeLastGenerateBalloon = Time.time;
            GenerateBalloon();
        }
    }

    void GenerateBalloon()
    {
        var randomVariant = balloonVariants[Random.Range(0, balloonVariants.Count)];
        var height = Random.Range(minRelativeHeight, maxRelativeHeight) * Camera.main.orthographicSize * 2 - Camera.main.orthographicSize;
        bool rightMove = Random.Range(0, 10) % 2 == 0;

        var xBallonSize = randomVariant.GetComponent<SpriteRenderer>().bounds.size.x;
        float leftBorder = -Camera.main.orthographicSize / Screen.height * Screen.width - xBallonSize;

        Vector3 pos = new Vector3(rightMove ? leftBorder : -leftBorder, height, 0);

        var balloon = Instantiate(randomVariant, pos, Quaternion.identity);
        balloons.Add(balloon);

        balloon.transform.DOMoveX(rightMove ? -leftBorder : leftBorder, Random.Range(minBalloonTimeFly, maxBalloonTimeFly))
            .OnUpdate(() => { balloon.transform.position = new Vector3(balloon.transform.position.x, height + Mathf.Sin(2 * (balloon.transform.position.x - pos.x)) * 0.5f, balloon.transform.position.z); })
            .OnComplete(() => DestroyBalloon(balloon));
    }

    void DestroyBalloon(GameObject balloon)
    {
        balloons.Remove(balloon);
        Destroy(balloon);
    }
}
