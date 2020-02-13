/* global require, module, exports */
var Thing = require( "./thing.js" );
var assert = require( "assert" );
var format = require( "util" ).format;
var fs = require( "fs" );
// Item class
//
//  - db: the table that holds all of this type of Thing.
//  - id: the name of the Thing being created.
//  - description: a description of the item, for printing in
//          the room or inventory.
//  - equipType: the way in which the item can be used. See
//          Item.equipTypes for a list of such types.
//  - strength: for whatever equipType is chosen, this is
//          how well the item can do it.
function Item ( db, id, description, equipType, strength ) {
  Thing.call( this, db, "items", id, description );
  this.equipType = equipType || "none";
  this.strength = strength || 0;
  this.name = id;
}

Item.prototype = Object.create( Thing.prototype );
module.exports = Item;

Item.equipTypes = [ "head", "eyes", "shoulders", "torso",
  "pants", "belt", "shirt", "forearms", "gloves", "shins",
  "boots", "tool", "necklace", "bracelet" ];
Item.armorTypes = [ "head", "torso", "forearms", "gloves", "shins", "boots" ];
Item.consumeTypes = [ "food", "scroll" ];

Item.prototype.copy = function () {
  var itm = Thing.prototype.copy.call( this );
  var stub = itm.id.toLocaleLowerCase() + "-";
  var i = 0;
  while ( this.db[stub + i] ) {
    ++i;
  }
  itm.id += stub + i;
  itm.name = this.name;
  this.db[itm.id] = itm;
  return itm;
};

Item.parse = function ( db, type, text ) {
  var parts = text.split( ' ' );
  var id = parts.shift();
  var strength = parts[0] * 1;
  if ( strength === parts[0] )
    parts.shift();
  var description = parts.join( " " );
  var itm = new Item( db, id, description, type, strength );
  db[itm.id.toLocaleLowerCase()] = itm;
};

Item.process = function ( db, text ) {
  var lines = text.split( "\n" );
  var type = "none";
  while ( lines.length > 0 )
  {
    var line = lines.shift()
        .trim();
    if ( Item.equipTypes.indexOf( line ) > -1
        || Item.consumeTypes.indexOf( line ) > -1 )
      type = line;
    else if ( line.length === 0 )
      type = "none";
    else
      Item.parse( db, type, line );
  }
};

Item.load = function ( db, filename ) {
  Item.process( db, fs.readFileSync( filename, { encoding: "utf8" } ) );
};

Item.loadIntoRoom = function ( db, roomId, text ) {
  var parts = text.split( " " );
  var itemName = parts[0].toLocaleLowerCase();
  var count = parts[1] * 1;
  assert( db[itemName], "Item " + itemName + " doesn't exist." );
  for ( var i = 0; i < count; ++i )
    db.getRoom(roomId).addChild( db.getItem(itemName).copy() );
};
