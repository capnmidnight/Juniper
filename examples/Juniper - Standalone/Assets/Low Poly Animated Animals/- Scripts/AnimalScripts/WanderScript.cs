using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LowPolyAnimalPack
{
  [RequireComponent(typeof(Animator)), RequireComponent(typeof(CharacterController))]
  public class WanderScript : MonoBehaviour
  {
    private const float contingencyDistance = 1f;

    [Header("Animation States"), Space(5)]
    [SerializeField]
    private IdleState[] idleStates;
    [SerializeField]
    private MovementState[] movementStates;
    [SerializeField]
    private AnimalState[] attackingStates;
    [SerializeField]
    private AnimalState[] deathStates;

    [Space(), Header("AI"), Space(5)]
    [SerializeField]
    private string species = "NA";

    [SerializeField, Tooltip("This specific animal stats asset, create a new one from the asset menu under (LowPolyAnimals/NewAnimalStats)")]
    AnimalStats ScriptableAnimalStats;

    [SerializeField, Tooltip("How far away from it's origin this animal will wander by itself.")]
    private float wanderZone = 10f;
    public float MaxDistance
    {
      get
      {
        return wanderZone;
      }
      set
      {
#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
        wanderZone = value;
      }
    }

    // [SerializeField, Tooltip("How dominent this animal is in the food chain, agressive animals will attack less dominant animals.")]
    // private int dominance = 1;
    private int originalDominance = 0;

    [SerializeField, Tooltip("How far this animal can sense a predator.")]
    private float awareness = 30f;

    [SerializeField, Tooltip("How far this animal can sense it's prey.")]
    private float scent = 30f;
    private float originalScent = 0f;

    // [SerializeField, Tooltip("How many seconds this animal can run for before it gets tired.")]
    // private float stamina = 10f;

    // [SerializeField, Tooltip("How much this damage this animal does to another animal.")]
    // private float power = 10f;

    // [SerializeField, Tooltip("How much health this animal has.")]
    // private float toughness = 5f;

    // [SerializeField, Tooltip("Chance of this animal attacking another animal."), Range(0f, 100f)]
    // private float agression = 0f;
    private float originalAgression = 0f;

    // [SerializeField, Tooltip("How quickly the animal does damage to another animal (every 'attackSpeed' seconds will cause 'power' amount of damage).")]
    // private float attackSpeed = 0.5f;

    // [SerializeField, Tooltip("If true, this animal will attack other animals of the same specices.")]
    // private bool territorial = false;

    // [SerializeField, Tooltip("Stealthy animals can't be detected by other animals.")]
    // private bool stealthy = false;

    [SerializeField, Tooltip("If true, this animal will never leave it's zone, even if it's chasing or running away from another animal.")]
    private bool constainedToWanderZone = false;

    [SerializeField, Tooltip("This animal will be peaceful towards species in this list.")]
    private string[] nonAgressiveTowards;

    private static List<WanderScript> allAnimals = new List<WanderScript>();
    public static List<WanderScript> AllAnimals { get { return allAnimals; } }

    [Space(), Header("Surface Rotation"), Space(5)]
    [SerializeField, Tooltip("If true, this animal will rotate to match the terrain. Ensure you have set the layer of the terrain as 'Terrain'.")]
    private bool matchSurfaceRotation = false;
    [SerializeField, Tooltip("How fast the animnal rotates to match the surface rotation.")]
    private float surfaceRotationSpeed = 2f;

    [Space(), Header("Debug"), Space(5)]
    [SerializeField, Tooltip("If true, AI changes to this animal will be logged in the console.")]
    private bool logChanges = false;
    [SerializeField, Tooltip("If true, gizmos will be drawn in the editor.")]
    private bool showGizmos = true;
    [SerializeField]
    private bool drawWanderRange = true;
    [SerializeField]
    private bool drawScentRange = true;
    [SerializeField]
    private bool drawAwarenessRange = true;

    private Color distanceColor = new Color(0f, 0f, 205f);
    private Color awarnessColor = new Color(1f, 0f, 1f, 1f);
    private Color scentColor = new Color(1f, 0f, 0f, 1f);
    private Animator animator;
    private CharacterController characterController;
    private NavMeshAgent navMeshAgent;
    private Vector3 origin;
    private int totalIdleStateWeight;
    private int currentState = 0;
    private bool dead = false;
    private bool moving = false;
    private bool useNavMesh = false;
    private Vector3 targetLocation = Vector3.zero;
    private float currentTurnSpeed = 0f;
    private bool attacking = false;

    public void OnDrawGizmosSelected()
    {
      if (!showGizmos)
        return;

      if (drawWanderRange)
      {
        // Draw circle of radius wander zone
        Gizmos.color = distanceColor;
        Gizmos.DrawWireSphere(origin == Vector3.zero ? transform.position : origin, wanderZone);

        Vector3 IconWander = new Vector3(transform.position.x, transform.position.y + wanderZone, transform.position.z);
        Gizmos.DrawIcon(IconWander, "ico-wander", true);
      }

      if (drawAwarenessRange)
      {
        //Draw circle radius for Awarness.
        Gizmos.color = awarnessColor;
        Gizmos.DrawWireSphere(transform.position, awareness);


        Vector3 IconAwareness = new Vector3(transform.position.x, transform.position.y + awareness, transform.position.z);
        Gizmos.DrawIcon(IconAwareness, "ico-awareness", true);
      }

      if (drawScentRange)
      {
        //Draw circle radius for Scent.
        Gizmos.color = scentColor;
        Gizmos.DrawWireSphere(transform.position, scent);

        Vector3 IconScent = new Vector3(transform.position.x, transform.position.y + scent, transform.position.z);
        Gizmos.DrawIcon(IconScent, "ico-scent", true);
      }

      if (!Application.isPlaying)
        return;

      // Draw target position.
      if (useNavMesh)
      {
        if (navMeshAgent.remainingDistance > 1f)
        {
          Gizmos.DrawSphere(navMeshAgent.destination + new Vector3(0f, 0.1f, 0f), 0.2f);
          Gizmos.DrawLine(transform.position, navMeshAgent.destination);
        }
      }
      else
      {
        if (targetLocation != Vector3.zero)
        {
          Gizmos.DrawSphere(targetLocation + new Vector3(0f, 0.1f, 0f), 0.2f);
          Gizmos.DrawLine(transform.position, targetLocation);
        }
      }
    }

    private void Awake()
    {
      if (idleStates.Length == 0 && movementStates.Length == 0)
      {
        Debug.LogError(string.Format("{0} has no idle or movement states state.", gameObject.name));
        enabled = false;
        return;
      }

      foreach (IdleState state in idleStates)
      {
        totalIdleStateWeight += state.stateWeight;
      }

      origin = transform.position;
      animator = GetComponent<Animator>();
      animator.applyRootMotion = false;
      characterController = GetComponent<CharacterController>();
      navMeshAgent = GetComponent<NavMeshAgent>();
      originalDominance = ScriptableAnimalStats.dominance;
      originalScent = scent;
      originalAgression = ScriptableAnimalStats.agression;

      if (navMeshAgent)
      {
        useNavMesh = true;
        navMeshAgent.stoppingDistance = contingencyDistance;
      }

      if (matchSurfaceRotation && transform.childCount > 0)
      {
        transform.GetChild(0).gameObject.AddComponent<SurfaceRotation>().SetRotationSpeed(surfaceRotationSpeed);
      }

      allAnimals.Add(this);
    }

    private void Start()
    {
      if (AnimalManager.Instance.PeaceTime)
      {
        SetPeaceTime(true);
      }

      StartCoroutine(InitYield());
    }

    private void OnDestroy()
    {
      allAnimals.Remove(this);
    }

    private IEnumerator InitYield()
    {
      yield return new WaitForSeconds((Random.Range(0, 200) / 100));
      DecideNextState(false, true);
    }

    private void DecideNextState(bool wasIdle, bool firstState = false)
    {
      attacking = false;

      // Look for a predator.
      if (awareness > 0)
      {
        for (int i = 0; i < allAnimals.Count; i++)
        {
          if (allAnimals[i].dead == true || allAnimals[i] == this || allAnimals[i].species == species || allAnimals[i].ScriptableAnimalStats.dominance <= ScriptableAnimalStats.dominance || allAnimals[i].ScriptableAnimalStats.stealthy)
          {
            continue;
          }

          if (Vector3.Distance(transform.position, allAnimals[i].transform.position) > awareness)
          {
            continue;
          }

          if (useNavMesh)
          {
            RunAwayFromAnimal(allAnimals[i]);
          }
          else
          {
            NonNavMeshRunAwayFromAnimal(allAnimals[i]);
          }

          if (logChanges)
          {
            Debug.Log(string.Format("{0}: Found predator ({1}), running away.", gameObject.name, allAnimals[i].gameObject.name));
          }

          return;
        }
      }

      // Look for pray.
      if (ScriptableAnimalStats.dominance > 0)
      {
        for (int i = 0; i < allAnimals.Count; i++)
        {
          if (allAnimals[i].dead == true || allAnimals[i] == this || (allAnimals[i].species == species && !ScriptableAnimalStats.territorial) || allAnimals[i].ScriptableAnimalStats.dominance > ScriptableAnimalStats.dominance || allAnimals[i].ScriptableAnimalStats.stealthy)
          {
            continue;
          }

          int p = System.Array.IndexOf(nonAgressiveTowards, allAnimals[i].species);
          if (p > -1)
          {
            continue;
          }

          if (Vector3.Distance(transform.position, allAnimals[i].transform.position) > scent)
          {
            continue;
          }

          if (Random.Range(0, 100) > ScriptableAnimalStats.agression)
          {
            continue;
          }

          if (logChanges)
          {
            Debug.Log(string.Format("{0}: Found prey ({1}), chasing.", gameObject.name, allAnimals[i].gameObject.name));
          }

          ChaseAnimal(allAnimals[i]);
          return;
        }
      }

      if (wasIdle && movementStates.Length > 0)
      {
        if (logChanges)
        {
          Debug.Log(string.Format("{0}: Wandering.", gameObject.name));
        }
        BeginWanderState();
        return;
      }
      else if (idleStates.Length > 0)
      {
        if (logChanges)
        {
          Debug.Log(string.Format("{0}: Idling.", gameObject.name));
        }
        BeginIdleState(firstState);
        return;
      }

      // Backup selection.
      if (idleStates.Length == 0)
      {
        BeginWanderState();
      }
      else if (movementStates.Length == 0)
      {
        BeginIdleState();
      }
    }

    private void BeginIdleState(bool firstState = false)
    {
      if (!firstState)
      {
        int randomValue = Random.Range(0, totalIdleStateWeight);
        for (int i = 0; i < idleStates.Length; i++)
        {
          if (randomValue < idleStates[i].stateWeight)
          {
            currentState = i;
            break;
          }

          randomValue = randomValue - idleStates[i].stateWeight;
        }
      }

      if (idleStates.Length == 0)
      {
        BeginWanderState();
        return;
      }

      if (!string.IsNullOrEmpty(idleStates[currentState].animationBool))
      {
        animator.SetBool(idleStates[currentState].animationBool, true);
      }

      float stateTime = firstState ?
        (Random.Range(50f, idleStates[currentState].minStateTime * 100f)) / 100f
        : (Random.Range(idleStates[currentState].minStateTime * 100f, idleStates[currentState].maxStateTime * 100f)) / 100f;

      StartCoroutine(IdleState(stateTime));
    }

    private IEnumerator IdleState(float stateTime)
    {
      moving = false;

      yield return new WaitForSeconds(stateTime);

      if (!string.IsNullOrEmpty(idleStates[currentState].animationBool))
      {
        animator.SetBool(idleStates[currentState].animationBool, false);
      }

      DecideNextState(true);
    }

    private void BeginWanderState()
    {
      Vector3 target = RandonPointInRange();

      int slowestMovementState = 0;
      for (int i = 0; i < movementStates.Length; i++)
      {
        if (movementStates[i].moveSpeed < movementStates[slowestMovementState].moveSpeed)
        {
          slowestMovementState = i;
        }
      }
      currentState = slowestMovementState;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, true);
      }

      if (useNavMesh)
      {
        StartCoroutine(MovementState(target));
      }
      else
      {
        StartCoroutine(NonNavMeshMovementState(target));
      }
    }

    private IEnumerator MovementState(Vector3 target)
    {
      moving = true;

      navMeshAgent.speed = movementStates[currentState].moveSpeed;
      navMeshAgent.angularSpeed = movementStates[currentState].turnSpeed;
      navMeshAgent.SetDestination(target);

      float timeMoving = 0f;
      while ((navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || timeMoving < 0.1f) && timeMoving < movementStates[currentState].maxStateTime)
      {
        timeMoving += Time.deltaTime;
        yield return null;
      }

      navMeshAgent.SetDestination(transform.position);

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      DecideNextState(false);
    }

    private IEnumerator NonNavMeshMovementState(Vector3 target)
    {
      moving = true;
      targetLocation = target;
      currentTurnSpeed = movementStates[currentState].turnSpeed;

      float walkTime = 0f;
      float timeUntilAbortWalk = Vector3.Distance(transform.position, target) / movementStates[currentState].moveSpeed;

      while (Vector3.Distance(transform.position, target) > contingencyDistance && walkTime < timeUntilAbortWalk)
      {
        characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * movementStates[currentState].moveSpeed);

        Vector3 relativePos = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (currentTurnSpeed / 10));
        currentTurnSpeed += Time.deltaTime;

        walkTime += Time.deltaTime;
        yield return null;
      }

      targetLocation = Vector3.zero;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      DecideNextState(false);
    }

    private void RunAwayFromAnimal(WanderScript predator)
    {
      moving = true;

      Quaternion startRotation = transform.rotation;
      transform.rotation = Quaternion.LookRotation(transform.position - predator.transform.position);
      Vector3 areaAwayFromPredator = transform.position + transform.forward * 5f;
      NavMeshHit hit;
      NavMesh.SamplePosition(areaAwayFromPredator, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
      Vector3 target = hit.position;
      transform.rotation = startRotation;

      if (constainedToWanderZone && Vector3.Distance(target, origin) > wanderZone)
      {
        target = RandonPointInRange();
      }

      int fastestMovementState = 0;
      for (int i = 0; i < movementStates.Length; i++)
      {
        if (movementStates[i].moveSpeed > movementStates[fastestMovementState].moveSpeed)
        {
          fastestMovementState = i;
        }
      }
      currentState = fastestMovementState;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, true);
      }

      StartCoroutine(RunAwayState(target, predator));
    }

    private IEnumerator RunAwayState(Vector3 target, WanderScript predator)
    {
      navMeshAgent.speed = movementStates[currentState].moveSpeed;
      navMeshAgent.angularSpeed = movementStates[currentState].turnSpeed;
      navMeshAgent.SetDestination(target);

      float timeMoving = 0f;
      while ((navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || timeMoving < 0.1f) && timeMoving < ScriptableAnimalStats.stamina)
      {
        timeMoving += Time.deltaTime;
        yield return null;
      }

      navMeshAgent.SetDestination(transform.position);

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      if (timeMoving > ScriptableAnimalStats.stamina || predator.dead || Vector3.Distance(transform.position, predator.transform.position) > awareness)
      {
        BeginIdleState();
      }
      else
      {
        RunAwayFromAnimal(predator);
      }
    }

    private void NonNavMeshRunAwayFromAnimal(WanderScript predator)
    {
      moving = true;

      Quaternion startRotation = transform.rotation;
      transform.rotation = Quaternion.LookRotation(transform.position - predator.transform.position);
      targetLocation = transform.position + transform.forward * 5f;
      transform.rotation = startRotation;

      if (constainedToWanderZone && Vector3.Distance(targetLocation, origin) > wanderZone)
      {
        targetLocation = RandonPointInRange();
      }

      int fastestMovementState = 0;
      for (int i = 0; i < movementStates.Length; i++)
      {
        if (movementStates[i].moveSpeed > movementStates[fastestMovementState].moveSpeed)
        {
          fastestMovementState = i;
        }
      }
      currentState = fastestMovementState;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, true);
      }

      StartCoroutine(NonNavMeshRunAwayState(targetLocation, predator));
    }

    private IEnumerator NonNavMeshRunAwayState(Vector3 target, WanderScript predator)
    {
      currentTurnSpeed = movementStates[currentState].turnSpeed;

      float walkTime = 0f;
      float timeUntilAbortWalk = Vector3.Distance(transform.position, target) / movementStates[currentState].moveSpeed;

      while (Vector3.Distance(transform.position, target) > contingencyDistance && walkTime < timeUntilAbortWalk && ScriptableAnimalStats.stamina > 0)
      {
        characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * movementStates[currentState].moveSpeed);

        Vector3 relativePos = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (currentTurnSpeed / 10));
        currentTurnSpeed += Time.deltaTime;

        walkTime += Time.deltaTime;
        ScriptableAnimalStats.stamina -= Time.deltaTime;
        yield return null;
      }

      targetLocation = Vector3.zero;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      if (ScriptableAnimalStats.stamina <= 0 || predator.dead || Vector3.Distance(transform.position, predator.transform.position) > awareness)
      {
        BeginIdleState();
      }
      else
      {
        NonNavMeshRunAwayFromAnimal(predator);
      }
    }

    private void ChaseAnimal(WanderScript prey)
    {
      Vector3 target = prey.transform.position;
      prey.BeginChase(this);

      int fastestMovementState = 0;
      for (int i = 0; i < movementStates.Length; i++)
      {
        if (movementStates[i].moveSpeed > movementStates[fastestMovementState].moveSpeed)
        {
          fastestMovementState = i;
        }
      }
      currentState = fastestMovementState;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, true);
      }

      if (useNavMesh)
      {
        StartCoroutine(ChaseState(prey));
      }
      else
      {
        StartCoroutine(NonNavMeshChaseState(prey));
      }
    }

    private IEnumerator ChaseState(WanderScript prey)
    {
      moving = true;

      navMeshAgent.speed = movementStates[currentState].moveSpeed;
      navMeshAgent.angularSpeed = movementStates[currentState].turnSpeed;
      navMeshAgent.SetDestination(prey.transform.position);

      float timeMoving = 0f;
      bool gotAway = false;
      while ((navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || timeMoving < 0.1f) && timeMoving < ScriptableAnimalStats.stamina)
      {
        navMeshAgent.SetDestination(prey.transform.position);

        timeMoving += Time.deltaTime;

        if (Vector3.Distance(transform.position, prey.transform.position) < 2f)
        {
          if (logChanges)
          {
            Debug.Log(string.Format("{0}: Caught prey ({1})!", gameObject.name, prey.gameObject.name));
          }

          if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
          {
            animator.SetBool(movementStates[currentState].animationBool, false);
          }

          AttackAnimal(prey);
          yield break;
        }

        if (constainedToWanderZone && Vector3.Distance(transform.position, origin) > wanderZone)
        {
          gotAway = true;
          navMeshAgent.SetDestination(transform.position);
          break;
        }

        yield return null;
      }

      navMeshAgent.SetDestination(transform.position);

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      if (timeMoving > ScriptableAnimalStats.stamina || prey.dead || Vector3.Distance(transform.position, prey.transform.position) > scent || gotAway)
      {
        BeginIdleState();
      }
      else
      {
        ChaseAnimal(prey);
      }
    }

    private IEnumerator NonNavMeshChaseState(WanderScript prey)
    {
      moving = true;
      targetLocation = prey.transform.position;
      currentTurnSpeed = movementStates[currentState].turnSpeed;

      float walkTime = 0f;
      bool gotAway = false;
      float timeUntilAbortWalk = Vector3.Distance(transform.position, targetLocation) / movementStates[currentState].moveSpeed;


      while (Vector3.Distance(transform.position, targetLocation) > contingencyDistance && walkTime < timeUntilAbortWalk && ScriptableAnimalStats.stamina > 0)
      {
        characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * movementStates[currentState].moveSpeed);
        targetLocation = prey.transform.position;

        Vector3 relativePos = targetLocation - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (currentTurnSpeed / 10));
        currentTurnSpeed += Time.deltaTime;

        walkTime += Time.deltaTime;
        ScriptableAnimalStats.stamina -= Time.deltaTime;

        if (Vector3.Distance(transform.position, prey.transform.position) < 2f)
        {
          if (logChanges)
          {
            Debug.Log(string.Format("{0}: Caught prey ({1})!", gameObject.name, prey.gameObject.name));
          }

          if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
          {
            animator.SetBool(movementStates[currentState].animationBool, false);
          }

          AttackAnimal(prey);
          yield break;
        }

        if (constainedToWanderZone && Vector3.Distance(transform.position, origin) > wanderZone)
        {
          gotAway = true;
          targetLocation = transform.position;
          break;
        }

        yield return null;
      }

      targetLocation = Vector3.zero;

      if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
      {
        animator.SetBool(movementStates[currentState].animationBool, false);
      }

      if (ScriptableAnimalStats.stamina <= 0 || prey.dead || Vector3.Distance(transform.position, prey.transform.position) > scent || gotAway)
      {
        BeginIdleState();
      }
      else
      {
        ChaseAnimal(prey);
      }
    }

    private void AttackAnimal(WanderScript target)
    {
      attacking = true;

      if (logChanges)
      {
        Debug.Log(string.Format("{0}: Attacking {1}!", gameObject.name, target.gameObject.name));
      }

      if (useNavMesh)
      {
        navMeshAgent.SetDestination(transform.position);
      }
      else
      {
        targetLocation = transform.position;
      }

      if (attackingStates.Length > 0)
      {
        currentState = Random.Range(0, attackingStates.Length);

        if (!string.IsNullOrEmpty(attackingStates[currentState].animationBool))
        {
          animator.SetBool(attackingStates[currentState].animationBool, true);
        }
      }

      StartCoroutine(MakeAttack(target));
    }

    private IEnumerator MakeAttack(WanderScript target)
    {
      target.GetAttacked(this);

      float timer = 0f;
      while (!target.dead)
      {
        timer += Time.deltaTime;

        if (timer > ScriptableAnimalStats.attackSpeed)
        {
          target.TakeDamage(ScriptableAnimalStats.power);
          timer = 0f;
        }

        yield return null;
      }

      if (!string.IsNullOrEmpty(attackingStates[currentState].animationBool))
      {
        animator.SetBool(attackingStates[currentState].animationBool, false);
      }

      DecideNextState(false);
    }

    private void GetAttacked(WanderScript attacker)
    {
      if (attacking)
      {
        return;
      }

      if (logChanges)
      {
        Debug.Log(string.Format("{0}: Getting attacked by {1}!", gameObject.name, attacker.gameObject.name));
      }
      StopAllCoroutines();

      StartCoroutine(TurnToLookAtTarget(attacker.transform));

      if (ScriptableAnimalStats.agression > 0)
      {
        if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
        {
          animator.SetBool(movementStates[currentState].animationBool, false);
        }

        AttackAnimal(attacker);
      }
      else
      {
        if (moving)
        {
          if (useNavMesh)
          {
            navMeshAgent.SetDestination(transform.position);
          }
          else
          {
            targetLocation = transform.position;
          }

          if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
          {
            animator.SetBool(movementStates[currentState].animationBool, false);
          }

          moving = false;
        }
        else
        {

          if (idleStates.Length > 0 && !string.IsNullOrEmpty(idleStates[currentState].animationBool))
          {
            animator.SetBool(idleStates[currentState].animationBool, false);
          }
        }
      }
    }

    private void TakeDamage(float damage)
    {
      ScriptableAnimalStats.toughness -= damage;

      if (ScriptableAnimalStats.toughness <= 0)
      {
        Die();
      }
    }

    public void Die()
    {
      if (logChanges)
      {
        Debug.Log(string.Format("{0}: Died!", gameObject.name));
      }

      StopAllCoroutines();
      dead = true;


      if (useNavMesh)
      {
        navMeshAgent.SetDestination(transform.position);
      }
      else
      {
        targetLocation = transform.position;
      }

      foreach (AnimalState state in idleStates)
      {
        if (!string.IsNullOrEmpty(state.animationBool))
        {
          animator.SetBool(state.animationBool, false);
        }
      }

      foreach (AnimalState state in movementStates)
      {
        if (!string.IsNullOrEmpty(state.animationBool))
        {
          animator.SetBool(state.animationBool, false);
        }
      }

      foreach (AnimalState state in attackingStates)
      {
        if (!string.IsNullOrEmpty(state.animationBool))
        {
          animator.SetBool(state.animationBool, false);
        }
      }

      if (deathStates.Length > 0)
      {
        currentState = Random.Range(0, deathStates.Length);

        if (!string.IsNullOrEmpty(deathStates[currentState].animationBool))
        {
          animator.SetBool(deathStates[currentState].animationBool, true);
        }
      }
      else
      {
        foreach (Transform child in transform)
        {
          SkinnedMeshRenderer renderer = child.GetComponent<SkinnedMeshRenderer>();
          if (renderer)
          {
            renderer.enabled = false;
          }
        }
      }
    }

    public void SetPeaceTime(bool peace)
    {
      if (peace)
      {
        ScriptableAnimalStats.dominance = 0;
        scent = 0f;
        ScriptableAnimalStats.agression = 0f;
      }
      else
      {
        ScriptableAnimalStats.dominance = originalDominance;
        scent = originalScent;
        ScriptableAnimalStats.agression = originalAgression;
      }
    }

    private Vector3 RandonPointInRange()
    {
      Vector3 randomPoint = origin + Random.insideUnitSphere * wanderZone;
      return new Vector3(randomPoint.x, transform.position.y, randomPoint.z);
    }

    private IEnumerator TurnToLookAtTarget(Transform target)
    {
      while (true)
      {
        Vector3 direction = target.position - transform.position;

        if (Vector3.Angle(direction, transform.forward) < 1f)
        {
          break;
        }

        float step = 2f * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        yield return null;
      }
    }

    private void BeginChase(WanderScript chasingAnimal)
    {
      if (attacking)
      {
        return;
      }

      StartCoroutine(ChaseCheck(chasingAnimal));
    }

    private IEnumerator ChaseCheck(WanderScript chasingAnimal)
    {
      while (Vector3.Distance(transform.position, chasingAnimal.transform.position) > awareness)
      {
        yield return new WaitForSeconds(0.5f);
      }

      StopAllCoroutines();
      if (moving)
      {
        if (useNavMesh)
        {
          navMeshAgent.SetDestination(transform.position);
        }
        else
        {
          targetLocation = transform.position;
        }

        if (!string.IsNullOrEmpty(movementStates[currentState].animationBool))
        {
          animator.SetBool(movementStates[currentState].animationBool, false);
        }

        moving = false;
      }
      else
      {
        if (idleStates.Length > 0 && !string.IsNullOrEmpty(idleStates[currentState].animationBool))
        {
          animator.SetBool(idleStates[currentState].animationBool, false);
        }
      }

      DecideNextState(false);
    }
  }
}