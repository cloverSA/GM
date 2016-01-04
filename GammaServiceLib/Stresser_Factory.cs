namespace GammaStressAgent
{
    //abstract factory
    public abstract class StressItemFactory
    {
        public abstract StresserCommand GetCpuStresser(string source_name);
        public abstract StresserCommand GetMemStresser(string source_name);
    }

    public class GammaStressItemFactory : StressItemFactory
    {
        public override StresserCommand GetCpuStresser(string source_name)
        {
            return new StresserCommand(new CpuLoader(source_name));
        }
        public override StresserCommand GetMemStresser(string source_name)
        {

            return new StresserCommand(new MemLoader(source_name));
        }

    }
}
