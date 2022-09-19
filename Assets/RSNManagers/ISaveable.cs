namespace RSNManagers
{
    public interface ISaveable<in T>
    {
        object CaptureState();
        void RestoreState(T state);
    }
}