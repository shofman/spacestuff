using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Invokes the notificaton method
public class EndTurn : MonoBehaviour
{
    /**
     * Attachable script so that end turn notifier can be called from clicking button
     */
    public void callEndTurnNotifier() {
        EndTurnNotifier.instance().notify();
    }
}