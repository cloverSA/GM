namespace GammaStressAgent.BaseService
{
    public interface IStresser
    {
        void Start();
        bool Stop();
    }

    public interface ILoadCommand
    {
        void Execute();
        bool Undo();
    }

    public class StresserCommand : ILoadCommand
    {
        private IStresser stresser;

        public StresserCommand(IStresser loader)
        {
            this.stresser = loader;
        }

        public void Execute()
        {
            this.stresser.Start();
        }

        public bool Undo()
        {
            return this.stresser.Stop();
        }
    }
}
