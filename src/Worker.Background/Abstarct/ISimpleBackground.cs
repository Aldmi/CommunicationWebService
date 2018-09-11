namespace Worker.Background.Abstarct
{
    public interface ISimpleBackground : IBackground
    {
        string Key { get; }
    }
}