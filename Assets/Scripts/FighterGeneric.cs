using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FighterGeneric : MonoBehaviour
{

    public Transform Target;
    public bool HasFlag;
    public bool isAlive;
    public int RespawnTime;
    public string Team;
    public Transform SpawnZone;
    public Material PrimaryMaterial;
    public Material SecondaryMaterial;
    public float Speed;
    public float DetectionRadius;

    public string etat = "";

    protected int Agro = 0;
    [SerializeField]
    protected Manager GameManager;
    [SerializeField]
    protected FlagBehaviour EnemyFlag;
    [SerializeField]
    protected FlagBehaviour TeamFlag;
    protected Rigidbody Body;
    
    [SerializeField]
    protected Vector3 Direction;    
    [SerializeField]
    protected List<FighterGeneric> ListFighters = new List<FighterGeneric>();
    [SerializeField]
    protected List<FighterGeneric> ClosestFighters = new List<FighterGeneric>();    

    protected FSM FiniteStateMachine;

    // Use this for initialization
    protected virtual void Start()
    {
        Body = gameObject.GetComponent<Rigidbody>();
        FiniteStateMachine = new FSM(RushFlag);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        DetectOthers();

        FiniteStateMachine.RunFSM();
    }

    protected virtual void LateUpdate()
    {
        if(HasFlag) //clignote quand a le drapeau
        {
            if(GetComponent<MeshRenderer>().material.color == PrimaryMaterial.color)
            {
                GetComponent<MeshRenderer>().material = SecondaryMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = PrimaryMaterial;
            }
        }
    }
    #region states

    abstract protected void RushFlag();    

    protected void Win()
    {
        GameManager.GameDone(EnemyFlag);
        FiniteStateMachine.SetState(Stop);        
    }   

    abstract protected void ChaseEnemyFlagBearer();        

    abstract protected void Dodge();

    abstract protected void Attack();

    abstract protected void ReturnBase();
    
    protected void Stop()
    {
        etat = "Stop";
        Body.velocity = Vector3.zero;
    }

    protected void Die()
    {
        isAlive = false;
        if (HasFlag)
        {
            Debug.Log("je suis mort avec le drapeau");
            EnemyFlag.InitFlag();
            HasFlag = false;
        }        
        FiniteStateMachine.SetState(Stop);
        GameManager.Resurect(Team, RespawnTime);
        gameObject.SetActive(false);
    }
    #endregion

    public void Idle()
    {
        FiniteStateMachine.SetState(Stop);
    }

    public void Go()
    {
        FiniteStateMachine.SetState(RushFlag);
    }    

    protected void MoveTo(Transform target)
    {
        Target = target;
        Direction = target.position - gameObject.transform.position;
        if (Direction.magnitude < 0.1)
        {
            Direction = Vector3.zero;
        }
        Body.velocity = Direction.normalized * Speed;
    }

    protected void DetectOthers()//npc dans la zone de detection
    {
        List<FighterGeneric> closeFightersListTemp = new List<FighterGeneric>();
        List<FighterGeneric> closeFightersFullList = new List<FighterGeneric>();
        closeFightersFullList.AddRange(GameManager.TeamKnight);
        closeFightersFullList.AddRange(GameManager.TeamSamurai);

        foreach (FighterGeneric fighter in closeFightersFullList)
        {
            float Distance = Vector3.Distance(fighter.transform.position, gameObject.transform.position);
            if (Distance <= DetectionRadius && Distance > 0 && fighter.gameObject.activeInHierarchy)
            {
                closeFightersListTemp.Add(fighter);
            }
        }
        ClosestFighters = new List<FighterGeneric>(closeFightersListTemp);
    }    

    public void ResetFighter()
    {
        HasFlag = false;
        isAlive = true;
        GetComponent<MeshRenderer>().material = PrimaryMaterial;
    }

    public void InitFighter(Manager _GameManager, Transform _SpawnZone, FlagBehaviour _TeamFlag, FlagBehaviour _EnemyFlag)
    {
        HasFlag = false;
        isAlive = true;
        TeamFlag = _TeamFlag;
        EnemyFlag = _EnemyFlag;
        SpawnZone = _SpawnZone;
        GameManager = _GameManager;
        GetComponent<MeshRenderer>().material = PrimaryMaterial;
    }
}
