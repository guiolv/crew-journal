// CrewJournal — DTOs de runtime (TDD secao 4). Sem Dictionary (serializacao).
// Listas de Entry no lugar de Dictionary. C# 5 compativel.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CrewJournal.Logic
{
    [DataContract]
    public class ResourceEntry
    {
        [DataMember] public ResourceId id;
        [DataMember] public int amount;
    }

    [DataContract]
    public class RepEntry
    {
        [DataMember] public string islandId;
        [DataMember] public int value;
    }

    [DataContract]
    public class PriceEntry
    {
        [DataMember] public ResourceId id;
        [DataMember] public float mult;
    }

    [DataContract]
    public class Stats
    {
        [DataMember] public int hp;
        [DataMember] public int maxHp;
        [DataMember] public int atk;
        [DataMember] public int def;
        [DataMember] public int speed;
    }

    [DataContract]
    public class CharacterData
    {
        [DataMember] public string id;
        [DataMember] public string displayName;
        [DataMember] public int age;
        [DataMember] public int level;
        [DataMember] public Stats stats;
        [DataMember] public List<string> jobs;
        [DataMember] public List<string> traits;
        [DataMember] public int nav;
        [DataMember] public int cook;
        [DataMember] public int medic;
        [DataMember] public int carpenter;
        [DataMember] public int fighter;
        [DataMember] public int shooter;
        [DataMember] public bool alive;
        [DataMember] public int morale;
        [DataMember] public int loyalty;
        [DataMember] public int battles;
        [DataMember] public int kills;
        [DataMember] public int hireCost;
        [DataMember] public int wage;

        public CharacterData()
        {
            jobs = new List<string>();
            traits = new List<string>();
            stats = new Stats();
            alive = true;
        }

        public int BestSeamanship()
        {
            return Math.Max(nav, Math.Max(fighter, shooter));
        }
    }

    [DataContract]
    public class IslandData
    {
        [DataMember] public string id;
        [DataMember] public string displayName;
        [DataMember] public IslandArchetype archetype;
        [DataMember] public float x;
        [DataMember] public float y;
        [DataMember] public int danger;
        [DataMember] public List<PriceEntry> prices;
        [DataMember] public List<string> recruitIds;

        public IslandData()
        {
            prices = new List<PriceEntry>();
            recruitIds = new List<string>();
        }
    }

    [DataContract]
    public class ShipData
    {
        [DataMember] public string defId;
        [DataMember] public int hull;
        [DataMember] public int maxHull;
        [DataMember] public int crewCap;
        [DataMember] public int cargoCap;
        [DataMember] public float speed;
        [DataMember] public int modules;
    }

    [DataContract]
    public class MissionData
    {
        [DataMember] public string id;
        [DataMember] public string title;
        [DataMember] public MissionType type;
        [DataMember] public MissionStatus status;
        [DataMember] public string fromIslandId;
        [DataMember] public string targetIslandId;
        [DataMember] public int reward;
        [DataMember] public int danger;
    }

    [DataContract]
    public class JournalEntry
    {
        [DataMember] public int day;
        [DataMember] public string text;
    }

    [DataContract]
    public class DeathRecord
    {
        [DataMember] public string name;
        [DataMember] public int dayJoined;
        [DataMember] public int dayDied;
        [DataMember] public string job;
        [DataMember] public int battles;
        [DataMember] public int kills;
        [DataMember] public string cause;
    }

    [DataContract]
    public class RelationshipData
    {
        [DataMember] public string aId;
        [DataMember] public string bId;
        [DataMember] public int affinity;
        [DataMember] public int trust;
    }

    [DataContract]
    public class WorldData
    {
        [DataMember] public int seed;
        [DataMember] public int day;
        [DataMember] public List<IslandData> islands;

        public WorldData()
        {
            islands = new List<IslandData>();
        }
    }

    [DataContract]
    public class GameData
    {
        [DataMember] public WorldData world;
        [DataMember] public List<CharacterData> crew;
        [DataMember] public List<CharacterData> roster;
        [DataMember] public ShipData ship;
        [DataMember] public List<ResourceEntry> resources;
        [DataMember] public List<RepEntry> reputation;
        [DataMember] public int notoriety;
        [DataMember] public List<MissionData> missions;
        [DataMember] public List<JournalEntry> journal;
        [DataMember] public List<DeathRecord> memorial;
        [DataMember] public List<RelationshipData> relations;
        [DataMember] public string currentIslandId;
        [DataMember] public int nextCharIndex;
        [DataMember] public int nextMissionIndex;

        public GameData()
        {
            world = new WorldData();
            crew = new List<CharacterData>();
            roster = new List<CharacterData>();
            resources = new List<ResourceEntry>();
            reputation = new List<RepEntry>();
            missions = new List<MissionData>();
            journal = new List<JournalEntry>();
            memorial = new List<DeathRecord>();
            relations = new List<RelationshipData>();
        }

        public int GetResource(ResourceId id)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].id == id)
                {
                    return resources[i].amount;
                }
            }
            return 0;
        }

        public void SetResource(ResourceId id, int amount)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].id == id)
                {
                    resources[i].amount = Math.Max(0, amount);
                    return;
                }
            }
            ResourceEntry e = new ResourceEntry();
            e.id = id;
            e.amount = Math.Max(0, amount);
            resources.Add(e);
        }

        public void AddResource(ResourceId id, int delta)
        {
            SetResource(id, GetResource(id) + delta);
        }

        public int GetRep(string islandId)
        {
            for (int i = 0; i < reputation.Count; i++)
            {
                if (reputation[i].islandId == islandId)
                {
                    return reputation[i].value;
                }
            }
            return 0;
        }

        public void SetRep(string islandId, int value)
        {
            int clamped = Math.Max(-100, Math.Min(100, value));
            for (int i = 0; i < reputation.Count; i++)
            {
                if (reputation[i].islandId == islandId)
                {
                    reputation[i].value = clamped;
                    return;
                }
            }
            RepEntry e = new RepEntry();
            e.islandId = islandId;
            e.value = clamped;
            reputation.Add(e);
        }

        public IslandData FindIsland(string id)
        {
            for (int i = 0; i < world.islands.Count; i++)
            {
                if (world.islands[i].id == id)
                {
                    return world.islands[i];
                }
            }
            return null;
        }
    }
}
