using System;
using System.Linq;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.AccessGranted.Patches;

[HarmonyPatch(typeof(LootCategories))]
public static class LootCategoriesPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.PrimaryGuns), MethodType.Getter)]
    public static void AddAllPrimaryGuns(LootCategory __result)
    {
        AddAllItemsOfType<DatabasePrimaryGun>(__result, chance: 2.0);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.SecondaryGuns), MethodType.Getter)]
    public static void AddAllSecondaryGuns(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseSecondaryGun>(__result, chance: 2.0);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Melee), MethodType.Getter)]
    public static void AddAllMelee(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseMelee>(__result, chance: 2.0);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Food), MethodType.Getter)]
    public static void AddAllFood(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseConsumable>(__result, chance: 2.0, filter: consumable => !consumable.ishealing);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Medicine), MethodType.Getter)]
    public static void AddAllMedicine(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseConsumable>(__result, chance: 2.0, filter: consumable => consumable.ishealing);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Throwables), MethodType.Getter)]
    public static void AddAllThrowables(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseThrowable>(__result, chance: 2.0);
    }

    private static void AddAllItemsOfType<T>(LootCategory lootCategory, double chance = 1.0, Func<T, bool> filter = null) where T : DatabaseItem
    {
        // Retrieve existing item entries
        var existingEntries = lootCategory.loots
            .Select(lootChance => lootChance.itemID)
            .ToHashSet();

        // Add all matching items into the loot table
        foreach (var item in ItemsBase.instance.item)
        {
            // Filter out items that do not match type and fail to pass the provided filter
            if (item is not T fullItemType || filter?.Invoke(fullItemType) == false) continue;
            // Filter out items that are not free and cannot be spawned
            if (item.PriceLocking != PriceLocking.Free || item.simplePropPrefab == null) continue;
            // Ignore items that already have entries
            if (existingEntries.Contains(item.itemID)) continue;
            // Add item to the loot table
            lootCategory.loots.Add(new LootChance(item.itemID, chance));
        }
    }
}
