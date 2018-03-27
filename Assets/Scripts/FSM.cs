using System.Collections;
using System.Collections.Generic;


public class FSM
{
    
	public delegate void state();
        
	private state activeState;
        
	public FSM()
    {

    }

    /// <summary>
    /// Constructeur avec état initial
    /// </summary>
    /// <param name="startingState">L'état initial</param>
    public FSM(state startingState)
    {
        activeState = startingState;
    }

    /// <summary>
    /// Change l'état actuel pour un nouvel état
    /// </summary>
    /// <param name="newState">Le nouvel état</param>
    public void SetState(state newState)
    {
        activeState = newState;
    }    

    public void RunFSM()
    {
        if (activeState != null)
        {
            activeState();
        }
    }
}
