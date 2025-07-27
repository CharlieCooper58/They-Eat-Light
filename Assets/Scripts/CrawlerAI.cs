using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerAI : MonoBehaviour
{
    NavMeshAgent agent;
    LightSensitiveComponent lightSensitiveComponent;

    Animator animator;

    enum CrawlerState
    {
        idle,
        agitated,
        patrol,
        rage
    }

    [SerializeField] float moveSpeed;
    CrawlerState state;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lightSensitiveComponent = GetComponent<LightSensitiveComponent>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float ratio = lightSensitiveComponent.GetIrritationRatio();
        switch (state)
        {
            case CrawlerState.idle:
                if(ratio > 0.5f)
                {
                    PlayCrawlerAgitatedEffects();
                    state = CrawlerState.agitated;
                }
                break;
            case CrawlerState.agitated:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(lightSensitiveComponent.scanDirection, transform.up)), Time.deltaTime);
                
                if (ratio < 0.5f)
                {
                    state = CrawlerState.idle;
                }
                else if(ratio > 1f)
                {
                    state = CrawlerState.patrol;
                }
                break;
            case CrawlerState.patrol:
                agent.Move(agent.speed * lightSensitiveComponent.scanDirection * Time.deltaTime);
                
                break;
        }
    }

    public virtual void PlayCrawlerAgitatedEffects()
    {
        animator.Play("Agitated");
        GameManager.instance.soundManager.PlaySoundByName("Agitated Chitter", transform.position, 2f, .05f);
    }
    public virtual void PlayCrawlerIdleEffects()
    {
        animator.Play("Idle");
    }


}
