using UnityEngine;

public class ScaleToFitScreen : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        float worldScreenHeight = Camera.main.orthographicSize * 2;

        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        float aspect = Mathf.Max(worldScreenWidth / sr.sprite.bounds.size.x, worldScreenHeight / sr.sprite.bounds.size.y);

        transform.position = new Vector3(0, 0.5f* (aspect * sr.sprite.bounds.size.y - worldScreenHeight), 0);
        transform.localScale = new Vector3(
            aspect,
            aspect, 1);
    }

}