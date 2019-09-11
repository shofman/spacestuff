using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CommandManager {
  private readonly List<ICommand> _commands = new List<ICommand>();

  public bool hasCommandsInProgress {
    get { return _commands.Any(command => !command.isCompleted); }
  }

  public void addCommand(ICommand newCommand) {
    Debug.Log("we are adding a command");
      _commands.Add(newCommand);
  }

  public void processCommands() {
    Debug.Log("we are processing commands" + _commands.Count);
    foreach(ICommand command in _commands.Where(command => !command.isCompleted)) {
      command.execute();
    }
  }
}