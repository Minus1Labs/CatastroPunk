
/*
Puzzles that use the reverse mechanic should implement this. Implementers should
handle all further logic from this function. Assume that bullets have already been
deducted, etc.

For now (9/7/25), we don't need to sanity check if the player is able to reverse something,
but maybe we could for anti-cheat reasons in the future.

@param strength : haven't decided if this should be a thing yet, but maybe 
    the player could obtain upgrades to the reverse bulllets through clears
    to unlock certain puzzles
    OR
    this is upgraded through the card system and only affects enemies. Strength 
    could be how far the enemy is set back, or how long they are CC'd.
*/
public interface IReversible
{
    public enum ReversibleState
    {
        UNINITIATED,
        REVERSIBLE,
        REVERSED,
    }
    void Reverse(int strength);
}