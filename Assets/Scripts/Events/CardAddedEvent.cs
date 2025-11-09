
public class CardAddedEvent : GenericEvent
{
    public Card CardAdded { get; private set; }
    public CardBehavior Behavior { get; private set; }
    public bool IsNew { get; private set; }
    public Player PlayerC { get; private set; }

    public CardAddedEvent(Card card, CardBehavior behavior, bool IsNewCard, Player player)
    {
        CardAdded = card;
        Behavior = behavior;
        IsNew = IsNewCard;
        PlayerC = player;
        EventMessage = "Card " + card.cardName + " added to player with total stacks: " + behavior.itemStacks;
    }
}