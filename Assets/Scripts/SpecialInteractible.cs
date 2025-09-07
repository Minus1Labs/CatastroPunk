
/*
Interactibles that can't be reversed or interacted with normally should
implement this.

Ex: Door requires explosion to open. explosion can be from any source


*/
public interface ISpecialInteractible
{
    public enum SpecialInteractType
    {
        EXPLOSION,
        CORROSIVE,
        PUZZLE_ONLY
    }

    public void SpecialInteract(SpecialInteractType it);
}