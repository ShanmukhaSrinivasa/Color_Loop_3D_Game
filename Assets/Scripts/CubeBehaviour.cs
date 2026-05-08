using System.Collections;
using UnityEngine;

public enum CubeType {  Normal, Armored, bomb}

public class CubeBehaviour : MonoBehaviour
{
    [Header("Tactical Settings")]
    public CubeType type = CubeType.Normal;
    public int health = 1;
    public int incomingDamage = 0;
    public Material myColor;

    public bool isdead = false;

    [Header("Juice Settings")]
    public ParticleSystem deathParticle;
    public float shrinkSpeed = 15f;

    public void Initialize(CubeType cubeType, Material mat)
    {
        type = cubeType;
        myColor = mat;

        Renderer rend = GetComponent<Renderer>();

        if (type == CubeType.Armored)
        {
            health = 2;
            rend.material.color = new Color(myColor.color.r * 0.4f, myColor.color.g * 0.4f, myColor.color.b * 0.4f);
        }
        else if (type == CubeType.bomb)
        {
            health = 1;

            rend.material.color = Color.black;
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", new Color(0.8f, 0.1f, 0.1f) * 2.5f);
        }
    }

    public bool CanBeTarget()
    {
        if (type == CubeType.bomb)
        {
            return false;
        }

        return health > 0 && incomingDamage == 0 && !isdead;
    }

    public void TakeDamage()
    {
        if (isdead)
        {
            return;
        }

        health--;

        if (incomingDamage > 0)
        {
            incomingDamage--;
        }
        else
        {
            if (type != CubeType.bomb && LevelManager.Instance != null && LevelManager.Instance.queueManager != null)
            {
                LevelManager.Instance.queueManager.RemoveAmmoFromSystem(myColor);
            }
        }

        if (health <= 0)
        {
            isdead = true;
            ExplodAndDie();
        }
        else if (type == CubeType.Armored && health == 1)
        {
            GetComponent<Renderer>().material.color = myColor.color;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPopSound();
            }
        }
    }

    public void ExplodAndDie()
    {
        gameObject.tag = "Untagged";


        AudioManager.Instance.PlayPopSound();

        CameraShake.Instance.ShakeCamera(0.1f, 0.15f);


        Collider[] hitcolliders = Physics.OverlapSphere(transform.position, 2f);

        foreach (var hitcollider in hitcolliders)
        {
            CubeBehaviour  neighbor = hitcollider.GetComponent<CubeBehaviour>();

            if (neighbor != null && neighbor != this && !neighbor.isdead)
            {
                if (this.type == CubeType.bomb)
                {
                    if (neighbor.type != CubeType.bomb)
                    {
                        neighbor.TakeDamage();
                    }
                }
                else if (neighbor.type == CubeType.bomb)
                {
                    neighbor.TakeDamage();
                }
            }

        }

        if(deathParticle != null)
        {
            ParticleSystem burst = Instantiate(deathParticle, transform.position, Quaternion.identity);

            var main = burst.main;
            main.startColor = (type == CubeType.bomb) ? Color.red : GetComponent<Renderer>().material.color;

            burst.Play();

            Destroy(burst.gameObject, 2f);
        }

        if (LevelManager.Instance != null && LevelManager.Instance.queueManager != null)
        {
            LevelManager.Instance.queueManager.CheckWinLoseConditions();
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
