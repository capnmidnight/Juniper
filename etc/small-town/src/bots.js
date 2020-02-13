/* global require, module, exports */
﻿var ShopKeep = require( "./shopkeep.js" );
var Scavenger = require( "./scavenger.js" );
var Aggressor = require( "./aggressor.js" );
var AIBody = require( "./aibody.js" );
var Mule = require( "./mule.js" );
var loadBots = require( "./ScriptedBot.js" ).loadFromDir;

module.exports = function ( db ) {
  new ShopKeep( db, "General-Store", 9000,
      {
        "scrap-metal": 100,
        "crucible": 10,
        "bird": 100,
        "towel": 15,
        "blanket": 10,
        "wool-coat": 5,
        "brown-cloak": 5,
        "green-cloak": 5,
        "red-cape": 2,
        "fur-coat": 1,
        "rope": 15,
        "braided-belt": 10,
        "leather-belt": 5,
        "work-belt": 5,
        "utility-belt": 2,
        "t-shirt": 15,
        "hawaiian": 10,
        "dress-shirt": 5,
        "flannel": 5,
        "under-armor": 5,
        "puffy-shirt": 2,
        "leather-shirt": 2,
        "gold": 1000
      },
  {
    "scrap-metal": { "gold": 1 },
    "crucible": { "gold": 10 },
    "bird": { "gold": 1 },
    "towel": { "gold": 5 },
    "blanket": { "gold": 10 },
    "wool-coat": { "gold": 20 },
    "brown-cloak": { "gold": 20 },
    "green-cloak": { "gold": 20 },
    "red-cape": { "gold": 50 },
    "fur-coat": { "gold": 100 },
    "rope": { "gold": 5 },
    "braided-belt": { "gold": 10 },
    "leather-belt": { "gold": 20 },
    "work-belt": { "gold": 20 },
    "utility-belt": { "gold": 50 },
    "t-shirt": { "gold": 5 },
    "hawaiian": { "gold": 10 },
    "dress-shirt": { "gold": 20 },
    "flannel": { "gold": 20 },
    "under-armor": { "gold": 20 },
    "puffy-shirt": { "gold": 50 },
    "leather-shirt": { "gold": 50 }
  }, null, "Roland" );

  new ShopKeep( db, "Armorer", 9000,
      {
        "leather-helm": 10,
        "fur-hat": 5,
        "steel-helm": 3,
        "crystal-helm": 1,
        "sunglasses": 10,
        "goggles": 5,
        "visor": 1,
        "wood-slats": 10,
        "reflective-vest": 7,
        "chain-mail": 3,
        "imperial-breastplate": 1,
        "coarse-pants": 10,
        "jeans": 7,
        "action-pants": 3,
        "fur-bracer": 10,
        "imperial-bracer": 1,
        "leather-gloves": 10,
        "mittens": 10,
        "work-gloves": 5,
        "gauntlet": 2,
        "shin-guards": 10,
        "leather-shins": 7,
        "bronze-leggings": 3,
        "steel-guards": 1,
        "chucks": 10,
        "dress-shoes": 7,
        "hiking-boots": 5,
        "steel-toed": 3,
        "steel-boots": 1,
        "gold": 1000
      },
  {
    "leather-helm": { "gold": 10 },
    "fur-hat": { "gold": 20 },
    "steel-helm": { "gold": 33 },
    "crystal-helm": { "gold": 100 },
    "sunglasses": { "gold": 10 },
    "goggles": { "gold": 20 },
    "visor": { "gold": 100 },
    "wood-slats": { "gold": 10 },
    "reflective-vest": 7,
    "chain-mail": { "gold": 33 },
    "imperial-breastplate": 1,
    "coarse-pants": { "gold": 10 },
    "jeans": { "gold": 15 },
    "action-pants": { "gold": 33 },
    "fur-bracer": { "gold": 10 },
    "imperial-bracer": { "gold": 100 },
    "leather-gloves": { "gold": 10 },
    "mittens": { "gold": 10 },
    "work-gloves": { "gold": 20 },
    "gauntlet": { "gold": 50 },
    "shin-guards": { "gold": 10 },
    "leather-shins": { "gold": 15 },
    "bronze-leggings": { "gold": 33 },
    "steel-guards": { "gold": 100 },
    "chucks": { "gold": 10 },
    "dress-shoes": { "gold": 15 },
    "hiking-boots": { "gold": 20 },
    "steel-toed": { "gold": 33 },
    "steel-boots": { "gold": 100 }
  }, null, "Pauline" );

  new ShopKeep( db, "Weapon-Smith", 9000,
      {
        "needle": 100,
        "stick": 100,
        "screwdriver": 100,
        "handsaw": 10,
        "shovel": 10,
        "pickaxe": 10,
        "butcher-knife": 10,
        "hunting-knife": 10,
        "sword": 10,
        "axe": 10,
        "steel-sword": 5,
        "cutlass": 5,
        "excalibur": 1,
        "katana": 1,
        "gold": 1000
      },
  {
    "needle": { "gold": 1 },
    "stick": { "gold": 1 },
    "screwdriver": { "gold": 3 },
    "handsaw": { "gold": 10 },
    "shovel": { "gold": 10 },
    "pickaxe": { "gold": 10 },
    "butcher-knife": { "gold": 10 },
    "hunting-knife": { "gold": 10 },
    "sword": { "gold": 10 },
    "axe": { "gold": 10 },
    "steel-sword": { "gold": 20 },
    "cutlas": { "gold": 25 },
    "excalibur": { "gold": 150 },
    "katana": { "gold": 1000 }
  }, null, "Karen" );

  new ShopKeep( db, "Food-Court", 9000,
      {
        "egg": 100,
        "water": 100,
        "apple": 10,
        "peach": 10,
        "blueberries": 10,
        "crab": 20,
        "lobster": 20,
        "small-potion": 20,
        "apple-pie": 10,
        "peach-pie": 10,
        "blueberry-pie": 10,
        "large-potion": 5,
        "gold": 1000
      },
  {
    "egg": { "gold": 1 },
    "water": { "gold": 1 },
    "apple": { "gold": 10 },
    "peach": { "gold": 10 },
    "blueberries": { "gold": 10 },
    "crab": { "gold": 5 },
    "lobster": { "gold": 5 },
    "small-potion": { "gold": 5 },
    "apple-pie": { "gold": 10 },
    "peach-pie": { "gold": 10 },
    "blueberry-pie": { "gold": 10 },
    "large-potion": { "gold": 25 }
  }, null, "Chef" );

  new Scavenger( db, "Main-Square", 10, null, null, "Badger" );

  new Mule( db, "General-Store", 15, "naaay", null, null, null, "mule" );
  new Mule( db, "General-Store", 15, "woof", null, null, null, "dog" );
  new Mule( db, "General-Store", 15, "baaaa", null, null, null, "goat" );
  new Mule( db, "General-Store", 25, "naaay", null, null, null, "horse" );
  new Mule( db, "General-Store", 25, "moo", null, null, null, "cow" );
  new Mule( db, "General-Store", 25, "moooo", null, null, null, "ox" );
  new Mule( db, "General-Store", 25, "heee-haw", null, null, null, "donkey" );
  new Mule( db, "General-Store", 25, "ptewie", null, null, null, "camel" );
  new Mule( db, "General-Store", 25, "hooowl", null, null, null,
      "White-Fang" );
  new Mule( db, "General-Store", 50, "Elephants never forget", null, null,
      null, "elephant" );

  new Aggressor( db, "Sewer-1", 10, { "hide": 1 }, null, "rat-1" );
  new Aggressor( db, "Sewer-1", 10, { "hide": 1 }, null, "rat-2" );
  new Aggressor( db, "Sewer-2", 10, { "hide": 1 }, null, "rat-3" );
  new Aggressor( db, "Sewer-2", 10, { "hide": 1 }, null, "rat-4" );
  new Aggressor( db, "Sewer-2", 10, { "hide": 1 }, null, "rat-5" );

  loadBots( db, "./data/bots/" );
};
