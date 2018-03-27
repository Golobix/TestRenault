using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterKnight : FighterGeneric
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

        DetectEnemyContact();        
    }

    protected override void ReturnBase()
    {
        etat = "Return Base";

        int EnemyNbr = 0;

        if (HasFlag && Vector3.Distance(SpawnZone.transform.position, transform.position) <= 1.5 && !TeamFlag.Captured) //condition de victoire
        {
            Debug.Log("je gagne");
            FiniteStateMachine.SetState(Win);
        }

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterSamurai>() != null && fighter.isAlive)
            {
                EnemyNbr++;
            }
        }

        if (EnemyNbr >= 3)
        {
            FiniteStateMachine.SetState(Dodge);
        }       

        if (!EnemyFlag.Captured)
        {
            FiniteStateMachine.SetState(RushFlag);
        }

        if (HasFlag)
        {
            MoveTo(SpawnZone);
        }
        else
        {
            Target.position = SpawnZone.position;
            Target.position = Target.position + new Vector3(0, transform.position.y, 0);  //retour a la zone de spawn en ligne droite
            MoveTo(Target);
        }

    }

    protected override void RushFlag()
    {
        etat = "Rush Flag";

        int EnemyNbr = 0;

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterSamurai>() != null && fighter.isAlive) //nombres d'ennemis dans npc proches
            {
                EnemyNbr++;
            }
        }

        if (EnemyNbr >= 3)
        {
            FiniteStateMachine.SetState(Dodge);
        }

        if (0 < EnemyNbr && EnemyNbr < 3)
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
            bool MinimalDistance = false;

            foreach (FighterGeneric knight in GameManager.TeamKnight)
            {
                if (Vector3.Distance(transform.position, EnemyFlag.transform.position) < Vector3.Distance(knight.transform.position, EnemyFlag.transform.position) && knight.isAlive) //cherhce qui est le plus pres du drapeau ennemi
                {
                    MinimalDistance = true;
                }
            }

            if (MinimalDistance)
            {
                FiniteStateMachine.SetState(RushFlag);
            }
            else
            {
                FiniteStateMachine.SetState(ChaseEnemyFlagBearer);
            }            
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

    protected override void ChaseEnemyFlagBearer()
    {
        etat = "Chase Enemy Flag Bearer";

        bool chase = false;

        foreach(FighterGeneric fighter in GameManager.TeamSamurai)
        {
            if(fighter.HasFlag && fighter.isAlive)
            {
                chase = true;
                Target = fighter.transform;
            }
        }

        if(!chase) //l'ennemis n'a plus le drapeau
        {
            if(EnemyFlag.Captured)
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
        float distance = DetectionRadius;

        foreach (FighterGeneric fighter in ClosestFighters)
        {
            if (fighter.GetComponent<FighterSamurai>() != null)
            {
                EnemyNbr++;

                if (Vector3.Distance(transform.position, fighter.transform.position) < distance)
                {
                    Target = fighter.transform;
                    distance = Vector3.Distance(transform.position, fighter.transform.position); //cherche ennemi le plus proche
                }
            }
        }

        if(EnemyNbr > 2)
        {
            FiniteStateMachine.SetState(Dodge);
        }

        if (EnemyNbr == 0)
        {
            FiniteStateMachine.SetState(RushFlag);
        }

        MoveTo(Target);
    }

    protected override void Dodge()
    {
        etat = "Dodge";

        int EnemyNbr = 0;
        float distance = DetectionRadius;
        Vector3 direction = new Vector3();
        
        foreach(FighterGeneric fighter in ClosestFighters)
        {
            if(fighter.GetComponent<FighterSamurai>() != null)
            {
                EnemyNbr++;
                
                if(Vector3.Distance(transform.position, fighter.transform.position) < distance)
                {
                    Target = fighter.transform;
                    distance = Vector3.Distance(transform.position, fighter.transform.position); //cherche ennemi le plus proche
                }
            }            
        }

        if(EnemyNbr < 3)
        {
            FiniteStateMachine.SetState(RushFlag);
        }

        direction = transform.position - Target.position;

        Body.velocity =  direction.normalized * Speed;

    }

    private void DetectEnemyContact() 
    {
        Agro = 0;
        
        foreach( FighterGeneric fighter in ClosestFighters)
            {
                if(Vector3.Distance(fighter.transform.position, transform.position) < 2 && fighter.GetComponent<FighterSamurai>() != null) //nbre d'ennemis dans une zone restreinte, pour faire mourrir chevalier
                {
                    Agro++;
                }
            }

            if(Agro == 3)
            {                
                FiniteStateMachine.SetState(Die);
            }              
    }    
    
}
