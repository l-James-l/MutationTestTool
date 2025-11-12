namespace Core.Interfaces;

public interface ISolutionProfileDeserializer
{
    void LoadSlnProfileIfPresent(string slnFilePath);
}