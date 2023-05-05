using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    #region State Machine Fields
    private W_MovementBaseState currentState;
    public W_IdleState IdleState = new W_IdleState();
    public W_WalkState WalkState = new W_WalkState();
    public W_AutoAttackState AAState = new W_AutoAttackState();
    public W_AbilityOne AbilityOneState = new W_AbilityOne();
    public W_AbilityTwo AbilityTwoState = new W_AbilityTwo();
    public W_AbilityThree AbilityThreeState = new W_AbilityThree();
    public W_DeathState DeathState = new W_DeathState();

    #endregion

    #region State Machine Bools
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool doingAutoAttack;
    [HideInInspector] public bool doingAbility1;
    [HideInInspector] public bool doingAbility2;
    [HideInInspector] public bool doingAbility3;
    [HideInInspector] public bool isDead;
    #endregion

    #region Movement Fields
    private WarlordController inputAction;
    private NavMeshAgent navMeshAgent;
    private Camera mainCamera;

    private Vector3 lastVelocity;
    private float maxChange = 0.1f;
    #endregion

    #region Auto Attack Fields
    [SerializeField] private float autoAttackRange;
    #endregion

    [HideInInspector] public Animator anim;
    private int layerAttackable;

    private void Awake()
    {
        inputAction = new WarlordController();
        anim = GetComponent<Animator>();
        mainCamera = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        layerAttackable = LayerMask.NameToLayer("Attackable");

        lastVelocity = navMeshAgent.velocity;

        SwitchState(IdleState);
    }

    private void OnEnable()
    {
        inputAction.W_Controller.Enable();
    }

    private void OnDisable()
    {
        inputAction.W_Controller.Disable();
    }

    public void SwitchState(W_MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }




    private void Update()
    {
        HandleMovement();
        CheckForMovement();

        currentState.UpdateState(this);
    }

    private void HandleMovement()
    {
        if (!inputAction.W_Controller.Movement.WasPressedThisFrame())
        {

            return;
        }
        else if (inputAction.W_Controller.Movement.WasPressedThisFrame())
        {

            RaycastHit hit;
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.destination = hit.point;

            }
        }


    }

    private void CheckForMovement()
    {
        //Method checks if the player moved, based on his velocity change

        Vector3 velocity = navMeshAgent.velocity;
        float dif = Mathf.Abs((velocity - lastVelocity).magnitude) / Time.fixedDeltaTime;
        if (dif > maxChange)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void OnAutoAttack()
    {

        //Can only attack something "Attackable"
        //if player rightklicks on something with the layer "attackable", do aa
        //else do nothing

        //range depends on the warlord
        //don't know how to handle different warlords right now

        RaycastHit hit;
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.layer == layerAttackable)
            {
                if (Vector3.Distance(transform.position, hit.point) <= autoAttackRange)
                {
                    doingAutoAttack = true;
                    DoAutoAttack();
                    
                  
                }
            }
        }
    }

    public void OnAbility1()
    {
        Debug.Log("Ability1");
        doingAbility1 = true;
    }

    public void OnAbility2()
    {
        Debug.Log("Ability2");
        doingAbility2 = true;
    }

    public void OnAbility3()
    {
        Debug.Log("Ability3");
        doingAbility3 = true;
    }

    public void DoAutoAttack()
    {
        //Do damage based on current stats
        Debug.Log("AutoAttack");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoAttackRange);
    }
}
