using SWEN1_MCTG.Classes;

public static class BattleQueue
{
    private static Queue<User> _queue = new Queue<User>();

    public static void Enqueue(User user)
    {
        _queue.Enqueue(user);
    }

    public static User Dequeue()
    {
        return _queue.Dequeue();
    }

    public static int Count => _queue.Count;
}