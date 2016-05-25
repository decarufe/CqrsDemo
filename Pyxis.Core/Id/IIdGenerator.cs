namespace Pyxis.Core.Id
{
    public interface IIdGenerator
    {
        string GenerateId(string seed = "");
    }
}
