// CrewJournal — economia (TDD secao 19, GED 10-11). Preco nunca abaixo do piso.
using System;

namespace CrewJournal.Logic
{
    public static class EconomySystem
    {
        public static int PriceFor(ResourceId id, float islandMult, int rep)
        {
            float raw = Balance.BasePriceOf(id) * islandMult * Balance.RepPriceFactor(rep);
            int min = Math.Max(1, Balance.BasePriceOf(id) / 2);
            int final = (int)Math.Ceiling(raw);
            if (final < min) final = min;
            return final;
        }

        public static int SellPriceFor(ResourceId id, float islandMult, int rep)
        {
            int buy = PriceFor(id, islandMult, rep);
            int sell = buy * 6 / 10;
            if (sell < 1) sell = 1;
            return sell;
        }

        public static int RepairCost(int missingHull)
        {
            if (missingHull <= 0) return 0;
            return missingHull * Balance.RepairPerHull;
        }
    }
}
