public interface ICommand {
  bool isCompleted { get; set; }
  void execute();
  void cancel();
}