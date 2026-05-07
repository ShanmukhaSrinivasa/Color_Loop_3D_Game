using System.Collections;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    [Header("Juice Settings")]
    public ParticleSystem deathParticle;
    public float shrinkSpeed = 15f;

    public void ExplodAndDie()
    {
        AudioManager.Instance.PlayPopSound();

        CameraShake.Instance.ShakeCamera(0.1f, 0.15f);


        if(deathParticle != null)
        {
            ParticleSystem burst = Instantiate(deathParticle, transform.position, Quaternion.identity);

            var main = burst.main;
            main.startColor = GetComponent<Renderer>().material.color;

            burst.Play();

            Destroy(burst.gameObject, 2f);
        }

        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float progress = 0f;

        while(progress < 1f)
        {
            progress += Time.deltaTime * shrinkSpeed;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}
