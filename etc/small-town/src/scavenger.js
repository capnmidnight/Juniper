/* global require, module, exports */
var AIBody = require( "./aibody.js" );
var core = require( "./core.js" );
var format = require( "util" ).format;

function Scavenger ( db, roomId, hp, items, equipment, id )
{
  AIBody.call( this, db, roomId, hp, items, equipment, id );
  this.moving = false;
}

Scavenger.prototype = Object.create( AIBody.prototype );
module.exports = Scavenger;

Scavenger.prototype.idleAction = function ()
{
  var rm = this.db.getRoom(this.roomId);
  var items = core.hashMap( rm.items, core.key );
  var item = core.selectRandom( items );
  var exits = core.hashMap( rm.exits, core.key );
  var exit = core.selectRandom( exits );
  if ( !this.moving && item )
    this.cmd( format( "take %s", item ) );
  else if ( exit )
    this.cmd( exit );
  this.moving = !this.moving;
};
