using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField]
    float minBalloonTimeFly;
    [SerializeField]
    float maxBalloonTimeFly;

    [SerializeField]
    Vector3 leftBottomCorner;
    [SerializeField]
    Vector3 rightUpCornerCorner;

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
        if(Time.time > timeLastGenerateBalloon + minDeltaForGeneration && balloons.Count < maxBalloonsOnField) {
            timeLastGenerateBalloon = Time.time;
            GenerateBalloon();
        }
    }

    void GenerateBalloon()
    {
        var randomVariant = balloonVariants[Random.Range(0, balloonVariants.Count)];
        var height = Random.Range(leftBottomCorner.y, rightUpCornerCorner.y);
        bool rightMove = Random.Range(0, 10) % 2 == 0;

        Vector3 pos = rightMove ? leftBottomCorner : rightUpCornerCorner;
        pos.y = height;

        var balloon = Instantiate(randomVariant, pos, Quaternion.identity, transform);
        balloons.Add(balloon);

        balloon.transform.DOMoveX(rightMove ? rightUpCornerCorner.x : leftBottomCorner.x, Random.Range(minBalloonTimeFly, maxBalloonTimeFly))
            .OnUpdate(() => { balloon.transform.position = new Vector3(balloon.transform.position.x, height + Mathf.Sin(2 * (balloon.transform.position.x - pos.x)) * 0.5f , balloon.transform.position.z); })
            .OnComplete(() => DestroyBalloon(balloon));
    }

    void DestroyBalloon(GameObject balloon)
    {
        balloons.Remove(balloon);
        Destroy(balloon);
    }
}
