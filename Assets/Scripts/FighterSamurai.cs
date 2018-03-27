using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSamurai : FighterGeneric
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();        
    }

    protected override void RushFlag()
    {
        etat = "Rush Flag";
        int EnemyNbr = 0;
        int AllyNbr = 0;

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterKnight>() != null && fighter.isAlive)
            {
                EnemyNbr++;
            }

            if (fighter.GetComponent<FighterSamurai>() != null && fighter.isAlive)
            {
                AllyNbr++;
            }
        }

        if (EnemyNbr > 0)
        {
            FiniteStateMachine.SetState(Dodge);
        }

        if (AllyNbr > 2 && EnemyNbr * EnemyNbr < AllyNbr)
        {
            FiniteStateMachine.SetState(Attack);
        }

        if (TeamFlag.Captured && EnemyFlag.Captured)
        {
            if (HasFlag)
            {
                FiniteStateMachine.SetState(ReturnBase);
            }
            else
            {
                FiniteStateMachine.SetState(ChaseEnemyFlagBearer);
            }
        }

        if (TeamFlag.Captured)
        {
            FiniteStateMachine.SetState(ChaseEnemyFlagBearer);
        }

        if (EnemyFlag.Captured)
        {
            if (HasFlag)
            {
                FiniteStateMachine.SetState(ReturnBase);
            }
            else
            {
                FiniteStateMachine.SetState(ReturnBase);
            }

        }

        MoveTo(EnemyFlag.transform);
    }

    protected override void Dodge()
    {
        etat = "Dodge";
        int EnemyNbr = 0;
        Vector3 direction = new Vector3();

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterKnight>() != null)
            {
                EnemyNbr++;

                Target = fighter.transform;
            }
        }

        if(EnemyNbr == 0)
        {
            FiniteStateMachine.SetState(RushFlag);
        }

        if(HasFlag)
        {
            if((Mathf.Abs(transform.position.y) - Mathf.Abs(Target.position.y))  < 0 )
            {
                if (transform.position.y <= 0)
                {
                    direction =  Vector3.up + SpawnZone.position - transform.position;
                }
                else
                {
                    direction = Vector3.down + SpawnZone.position - transform.position;
                }
            }
            else
            {
                if (transform.position.y <= 0)
                {
                    direction = Vector3.down + SpawnZone.position - transform.position;
                }
                else
                {
                    direction = Vector3.up + SpawnZone.position - transform.position;
                }
            } 
        }
        else
        {
            direction = transform.position - Target.position;
        }        

        Body.velocity = direction.normalized * Speed;

    }

    protected override void ReturnBase()
    {
        etat = "ReturnBase";

        if (HasFlag && Vector3.Distance(SpawnZone.transform.position, transform.position) <= 1.5 && !TeamFlag.Captured)
        {
            Debug.Log("je gagne");
            FiniteStateMachine.SetState(Win);
        }

        if (!EnemyFlag.Captured)
        {
            FiniteStateMachine.SetState(RushFlag);
        }

        if(EnemyFlag.Captured && TeamFlag.Captured)
        {
            FiniteStateMachine.SetState(ChaseEnemyFlagBearer);
        }

        if(Vector3.Distance(SpawnZone.transform.position, transform.position) <= 2 && !HasFlag)
        {
            Body.velocity = Vector3.zero;
        }
        else
        {
            MoveTo(SpawnZone);
        }
        
    }

    protected override void ChaseEnemyFlagBearer()
    {
        etat = "Chase Enemy Flag Bearer";

        bool chase = false;

        foreach (FighterGeneric fighter in GameManager.TeamKnight)
        {
            if (fighter.HasFlag && fighter.isAlive)
            {
                chase = true;
                Target = fighter.transform;
            }
        }

        if (!chase)
        {
            if (EnemyFlag.Captured)
            {
                FiniteStateMachine.SetState(ReturnBase);
            }
            else
            {
                FiniteStateMachine.SetState(RushFlag);
            }
        }
        else
        {
            MoveTo(Target);
        }
    }

    protected override void Attack()
    {
        etat = "Attack";

        int EnemyNbr = 0;
        int AllyNbr = 0;
        float distance = DetectionRadius;

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterKnight>() != null && fighter.isAlive)
            {
                EnemyNbr++;

                if (Vector3.Distance(transform.position, fighter.transform.position) < distance)
                {
                    Target = fighter.transform;
                    distance = Vector3.Distance(transform.position, fighter.transform.position);
                }
            }

            if (fighter.GetComponent<FighterSamurai>() != null && fighter.isAlive)
            {
                AllyNbr++;
            }
        }

        if (AllyNbr < 2 || EnemyNbr * EnemyNbr >= AllyNbr)
        {
            FiniteStateMachine.SetState(Dodge);
        }

        if (EnemyNbr == 0)
        {            
            FiniteStateMachine.SetState(RushFlag);
        }

        MoveTo(Target);
    }
        
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<FighterKnight>() != null)
        {
            FiniteStateMachine.SetState(Die);            
        }
    }
}
