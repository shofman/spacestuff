using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public int wealth;
    public Color allegiance;

    public GameObject universe;

    private CommandManager commandManager;

    // protected void Awake() {
    // }

    protected virtual void Start() {
        wealth = calculateWealth();
        commandManager = new CommandManager();
    }

    void Update() {}

    public virtual void spendMoney(int moneyToSpend) {
        wealth -= moneyToSpend;

        if (wealth < 0) {
            Debug.Log("WE SHOULD NOT BE HERE - WEALTH WAS REDUCED BELOW ZERO");
            // Should
            wealth = 0;
        }
    }

    public int getWealth() {
        return wealth;
    }

    public void addCommand(ICommand newCommand) {
        if (commandManager != null) {
            commandManager.addCommand(newCommand);
        }
    }

    public void processUnfinishedCommands() {
        if (commandManager != null) {
            commandManager.processCommands();
        }
    }

    public Color getAllegiance() {
        return allegiance;
    }

    protected int calculateWealth() {
        List<GameObject> playersPlanets = universe.GetComponent<Universe>().findAllPlanetsBelongingTo(allegiance);
        int amount = 0;
        foreach (GameObject planet in playersPlanets) {
            amount += planet.GetComponent<Planet>().getSpiceValue();
        }
        return amount;
    }

    public virtual int TurnOrder
    {
        get { return -1; }
    }

    public virtual string PlayerName {
        get { return ""; }
    }
}
