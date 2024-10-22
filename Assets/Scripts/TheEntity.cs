using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TheEntity : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float proximityChaseDistance = 2f; // Horizontal proximity for chase activation
    [SerializeField] private float loseChaseDistance = 10f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip chaseSfx;
    [SerializeField] private AudioClip screamSfx;

    private AudioSource audioSource;
    private AudioSource chaseAudioSource;
    private bool isChasing = false;
    private bool isPausing = false;
    private Transform player;
    private Animator runningCrawlAnimator;

    private float originalAnimatorSpeed = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        chaseAudioSource = transform.Find("ChaseAudioSource").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        runningCrawlAnimator = transform.Find("Running Crawl").GetComponent<Animator>();
        agent.speed = normalSpeed;
        GoToNextWaypoint();

        if (runningCrawlAnimator != null)
        {
            originalAnimatorSpeed = runningCrawlAnimator.speed;
        }
    }

    void Update()
    {
        if (isChasing && !isPausing)
        {
            ChasePlayer();
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }

        CheckForPlayer();
    }

    void GoToNextWaypoint()
    {
        currentWaypointIndex = Random.Range(0, waypoints.Length);
        agent.destination = waypoints[currentWaypointIndex].position;

        PlayFootstepAudio();
    }

    void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position); // Full 3D distance
        float verticalDistanceToPlayer = Mathf.Abs(player.position.y - transform.position.y); // Vertical distance only

        // Draw the ray in the scene view for debugging
        Debug.DrawRay(transform.position, directionToPlayer * chaseDistance, Color.red);

        // Proximity-based chase (full distance + vertical constraint)
        if (distanceToPlayer <= proximityChaseDistance && verticalDistanceToPlayer <= 3.0f && !isChasing)
        {
            StartChase();
        }
        // Vision-based chase using line of sight and full distance, but add a vertical constraint
        else if (Vector3.Dot(transform.forward, directionToPlayer) > 0 && distanceToPlayer < chaseDistance && verticalDistanceToPlayer <= 3.0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Player") && !isChasing)
                {
                    StartChase();
                }
            }
        }

        // Stop chase if player escapes (again, use full 3D distance + vertical check)
        if (isChasing && (distanceToPlayer > loseChaseDistance || verticalDistanceToPlayer > 3.0f))
        {
            StopChase();
        }
    }



    void StartChase()
    {
        isChasing = true;
        StartCoroutine(StartChaseSequence());
    }

    void StopChase()
    {
        isChasing = false;
        StartCoroutine(FadeOutChaseAudio(1f));
        agent.speed = normalSpeed;

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.speed = originalAnimatorSpeed;
        }
        GoToNextWaypoint();
    }

    IEnumerator StartChaseSequence()
    {
        isPausing = true;
        agent.isStopped = true;

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = false;
        }

        if (screamSfx != null)
        {
            audioSource.PlayOneShot(screamSfx);
        }

        yield return new WaitForSeconds(1f);

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = true;
            runningCrawlAnimator.speed = originalAnimatorSpeed * 3f;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        ResetChaseAudio();
        PlayChaseAudio();

        isPausing = false;
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        if (!chaseAudioSource.isPlaying)
        {
            PlayChaseAudio();
        }
    }

    void PlayFootstepAudio()
    {
        if (footstepSfx && !isChasing && !isPausing)
        {
            if (audioSource.clip != footstepSfx)
            {
                audioSource.clip = footstepSfx;
                audioSource.loop = true;
                audioSource.volume = 0.5f;
                audioSource.Play();
            }
        }
    }

    void PlayChaseAudio()
    {
        if (chaseSfx && isChasing && !isPausing)
        {
            if (chaseAudioSource.clip != chaseSfx)
            {
                chaseAudioSource.clip = chaseSfx;
                chaseAudioSource.loop = true;
            }
            chaseAudioSource.Play();
        }
    }

    void ResetChaseAudio()
    {
        chaseAudioSource.volume = 1f;
    }

    IEnumerator FadeOutChaseAudio(float fadeDuration)
    {
        float startVolume = chaseAudioSource.volume;

        while (chaseAudioSource.volume > 0)
        {
            chaseAudioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        chaseAudioSource.Stop();
        chaseAudioSource.volume = startVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDeathScene();
        }
        else if (other.CompareTag("Door"))
        {
            Door door = other.GetComponent<Door>();
            if (door != null)
            {
                door.ToggleDoor(); // Interact with the door
            }
        }
    }

    void OpenDeathScene()
    {
        SceneManager.LoadScene("Death");
    }
}
