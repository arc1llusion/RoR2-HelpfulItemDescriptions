using BepInEx;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HelpfulItemDescriptions
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("net.pandastic.arc1llusion", "Helpful Item Descriptions", "0.1")]
    public class HelpfulItemDescriptions : BaseUnityPlugin
    {
        private bool _debug = true;

        private Regex _languageItemPickupRegex = new Regex("(ITEM)_[a-z]+_(PICKUP)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private Dictionary<string, ItemIndex> _itemFriendlyNameMapping = new Dictionary<string, ItemIndex>();
        private Dictionary<ItemIndex, ItemBuffEvaluator> _itemCalculations = new Dictionary<ItemIndex, ItemBuffEvaluator>();

        public void Awake()
        {
        }

        public void Start()
        {
            PlayerCharacterMasterController.onPlayerAdded += (PlayerCharacterMasterController pcmc) =>
            {
                //Inventory isn't loaded immediately. Do something when it does
                StartCoroutine(WatchInventory(pcmc));
            };

            PlayerCharacterMasterController.onPlayerRemoved += (PlayerCharacterMasterController pcmc) =>
            {

            };

            StartCoroutine(InitializeFriendlyNameMappings());
        }

        //The Update() method is run on every frame of the game.
        public void Update()
        {
            //This if statement checks if the player has currently pressed F2, and then proceeds into the statement:
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Chat.AddMessage("Hello");
                //We grab a list of all available Tier 3 drops:
                var dropList = Run.instance.availableTier3DropList;

                //Randomly get the next item:
                var nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

                //Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then finally drop it infront of the player.
                PickupDropletController.CreatePickupDroplet(dropList[nextItem], transform.position, transform.forward * 20f);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                PlayerCharacterMasterController.instances[0].master.GiveMoney(uint.MaxValue / 2);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                PlayerCharacterMasterController.instances[0].master.inventory.GiveItem(ItemIndex.Bear, 1);
                PlayerCharacterMasterController.instances[0].master.inventory.GiveItem(ItemIndex.CritGlasses, 1);
                PlayerCharacterMasterController.instances[0].master.inventory.GiveItem(ItemIndex.IgniteOnKill, 1);
            }
        }

        public IEnumerator WatchInventory(PlayerCharacterMasterController pcmc)
        {
            yield return new WaitUntil(() => { return pcmc.master.inventory != null; });

            InitializeItemCalculations();

            string name = pcmc.GetDisplayName() ?? pcmc.name;

            DebugChat("Configuring Player " + name + " inventory");

            On.RoR2.Language.GetString_string += (orig, self) =>
            {
                //if ((self.ToLower().StartsWith("item") || self.ToLower().StartsWith("equipment")) && self.ToLower().EndsWith("pickup"))
                if (_languageItemPickupRegex.IsMatch(self))
                {
                    var ii = GetItemFromFriendlyName(self);
                    var ic = GetItemCalculation(ii);

                    DebugChat("Item Index: " + ii.ToString());

                    if (ic != null)
                    {
                        var results = ic.Evaluate(pcmc.master.inventory.GetItemCount(ii));
                        var newDescription = orig(self) + Environment.NewLine + Environment.NewLine;

                        foreach (var r in results)
                        {
                            newDescription += String.Format("{0} modified by {1}{2}{3}", r.Property, r.Result.ToString("0.##"), r.Unit, Environment.NewLine);
                        }

                        return newDescription;
                    }
                }

                return orig(self);
            };
        }

        private void DebugChat(object message)
        {
            if (_debug)
            {
                Chat.AddMessage(message?.ToString());
            }
        }

        private void InitializeItemCalculations()
        {
            _itemCalculations.Add(ItemIndex.Bear, new ItemBuffEvaluator(ItemIndex.Bear, ItemCatalog.GetItemDef(ItemIndex.Bear), new List<BuffPropertyItem>()
            {
                { new BuffPropertyItem("Chance to block damage", 15, 0, ItemFormulas.Hyperbolic, "%") }
            }));

            _itemCalculations.Add(ItemIndex.CritGlasses, new ItemBuffEvaluator(ItemIndex.CritGlasses, ItemCatalog.GetItemDef(ItemIndex.CritGlasses), new List<BuffPropertyItem>()
            {
                { new BuffPropertyItem("Crit Chance", 10, 10, ItemFormulas.Linear, "%") }
            }));

            _itemCalculations.Add(ItemIndex.IgniteOnKill, new ItemBuffEvaluator(ItemIndex.IgniteOnKill, ItemCatalog.GetItemDef(ItemIndex.IgniteOnKill), new List<BuffPropertyItem>()
            {
                { new BuffPropertyItem("Damage", 150, 75, ItemFormulas.Linear, "%") },
                { new BuffPropertyItem("Radius", 12, 4, ItemFormulas.Linear, "m") }
            }));
        }

        private ItemBuffEvaluator GetItemCalculation(ItemIndex index)
        {
            if (_itemCalculations.Keys.Contains(index))
            {
                DebugChat("Found IC for " + index.ToString());
                return _itemCalculations[index];
            }
            return null;
        }

        private IEnumerator InitializeFriendlyNameMappings()
        {
            yield return new WaitUntil(() => { return ItemCatalog.allItems.Count() > 0; });
            foreach (var item in ItemCatalog.allItems)
            {
                var friendlyName = String.Format(System.Globalization.CultureInfo.InvariantCulture, "ITEM_{0}_PICKUP", item.ToString().ToUpper(System.Globalization.CultureInfo.InvariantCulture));
                _itemFriendlyNameMapping.Add(friendlyName, item);
            }
        }

        private ItemIndex GetItemFromFriendlyName(string friendlyName)
        {
            if (_itemFriendlyNameMapping.Keys.Contains(friendlyName))
            {
                return _itemFriendlyNameMapping[friendlyName];
            }

            return ItemIndex.None;
        }
    }
}
