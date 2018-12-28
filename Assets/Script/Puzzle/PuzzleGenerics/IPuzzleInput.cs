/// <summary>
/// Interface to apply on every Input item
/// </summary>
public interface IPuzzleInput : ISelectable
{
    void Init(IPuzzle _puzzle, IPuzzleInputData _data);
}

public interface IPuzzleInputData { }