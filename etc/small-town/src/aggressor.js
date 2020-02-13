/* global require, module, exports */
var AIBody = require( "./aibody.js" );
var core = require( "./core.js" );

/*
 * Aggressor class
 *  A violent NPC. Will alternate between moving and attacking
 *
 * players.
 *  - roomId: the name of the room in which the Aggressor starts.
 *  - hp: how much health the Aggressor starts with.
 *  - items (optional): an associative array of item IDs to counts,
 *      representing the stuff in the character's pockets.
 *  - equipment (optional): an associative array of item IDs to
 *      counts, representing the stuff in use by the character.
 */
function Aggressor ( db, roomId, hp, items, equipment, id ) {
  AIBody.call( this, db, roomId, hp, items, equipment, id );
  this.targetId = null;
}

Aggressor.prototype = Object.create( AIBody.prototype );
module.exports = Aggressor;

Aggressor.prototype.findTarget = function findTarget () {
  var target;
  if ( this.targetId ) {
    target = this.db.getPerson(this.targetId);
  }
  else {
    var people = this.db.getPeopleIn( this.roomId, this.id );
    var realUsers = people.filter( function ( p ) {
      return p.isPerson;
    } );
    target = core.selectRandom( realUsers );
    this.targetId = target && target.id;
  }

  if ( !target || target.roomId.toLocaleLowerCase() !== this.roomId.toLocaleLowerCase() || target.hp <= 0 ) {
    this.targetId = null;
    target = null;
  }

  return target;
};

Aggressor.prototype.idleAction = function idleAction () {
  var target = this.findTarget();
  if ( target ) {
    this.cmd( "say RAAAARGH!" );
    this.cmd( "attack " + this.targetId );
  }
  else {
    var rm = this.db.getRoom(this.roomId);
    var exit = core.selectRandom( core.keys( rm.exits ) );
    if ( exit ) {
      this.cmd( exit );
    }
  }
};

Aggressor.prototype.react_attack = function react_attack ( m ) {
  this.targetId = m.payload[0];
};