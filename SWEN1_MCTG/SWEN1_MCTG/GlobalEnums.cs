namespace SWEN1_MCTG;

public static class GlobalEnums
{
    // Spell element types
    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }

    // Monster types
    public enum MonsterType
    {
        Dragon,
        Goblin,
        Wizard,
        Knight,
        Ork,
        FireElve,
        Kraken
    }

    // Coin types
    public enum CoinType
    {
        Bronze = 1,
        Silver = 3,
        Gold = 5,
        Platinum = 10,
        Diamond = 20
    }

    // Round results
    public enum RoundResults
    {
        Loss,
        Draw,
        Win
    }
}