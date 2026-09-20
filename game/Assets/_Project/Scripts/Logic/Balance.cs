// CrewJournal — parametros de balanceamento (GED). Tudo editavel sem mexer em logica.
using System;

namespace CrewJournal.Logic
{
    public static class Balance
    {
        public static int StartMoney = 100;
        public static int StartFood = 8;
        public static int StartWater = 8;
        public static int StartWood = 2;
        public static int StartMetal = 0;
        public static int StartMedicine = 1;

        public static int BasePriceFood = 5;
        public static int BasePriceWater = 4;
        public static int BasePriceWood = 15;
        public static int BasePriceMetal = 25;
        public static int BasePriceMedicine = 20;

        public static int FoodPerCrewPerDay = 1;
        public static int WaterPerCrewPerDay = 1;

        public static int BoatPrice = 500;
        public static int MediumPrice = 2500;
        public static int RepairPerHull = 4;

        // GED secao 18: reputacao ajusta preco. Amigavel desconta, hostil sobretaxa.
        public static float RepPriceFactor(int rep)
        {
            if (rep >= 60) return 0.85f;
            if (rep >= 20) return 0.95f;
            if (rep <= -60) return 1.30f;
            if (rep <= -20) return 1.15f;
            return 1.0f;
        }

        public static int BasePriceOf(ResourceId id)
        {
            switch (id)
            {
                case ResourceId.Food: return BasePriceFood;
                case ResourceId.Water: return BasePriceWater;
                case ResourceId.Wood: return BasePriceWood;
                case ResourceId.Metal: return BasePriceMetal;
                case ResourceId.Medicine: return BasePriceMedicine;
                default: return 1;
            }
        }

        public static readonly string[] FirstNames = new string[]
        {
            "Carlos", "Mira", "Joao", "Nair", "Tomas", "Lia", "Ruy", "Sara",
            "Bento", "Ines", "Dora", "Felix", "Guto", "Hana", "Ivo", "Jade"
        };

        public static readonly string[] Nicknames = new string[]
        {
            "Mao de Ferro", "Olho Vivo", "Pe de Vento", "Barba Ruiva",
            "Corvo", "Anzol", "Maruja", "Trovoador", "Calmaria", "Sereia"
        };

        public static readonly string[] Jobs = new string[]
        {
            "Navegador", "Cozinheiro", "Medico", "Carpinteiro", "Combatente", "Atirador"
        };

        public static readonly string[] Traits = new string[]
        {
            "corajoso", "covarde", "ambicioso", "generoso", "leal",
            "impulsivo", "inteligente", "preguicoso", "desconfiado"
        };

        public static readonly string[] IslandNames = new string[]
        {
            "Porto Salgado", "Baia Cinzenta", "Recife Alto", "Ponta Serena",
            "Ilha do Farol", "Enseada Funda", "Atol Quebrado", "Costa Negra"
        };
    }
}
