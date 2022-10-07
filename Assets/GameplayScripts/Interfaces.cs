namespace GameplayScripts
{
    public interface IWorkable
    {
        void StartWork(Customer customer);
        void DoneWork();
        void Empty();
    }

    public interface IRepairable
    {
        public void INeedRepair();
    }
}