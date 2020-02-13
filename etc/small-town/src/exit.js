/* global require, module, exports */
var Thing = require( "./thing.js" );
var assert = require( "assert" );
var format = require( "util" ).format;
var core = require( "./core.js" );
/*
 * Exit class:
 *  A doorway from one room to the other, with an optional lock.
 *  Exits are uni-directional. An exit from Room A to Room B does
 *  not automatically create a path from Room B to Room A.
 *
 *  - db: a database of all Things.
 *  - direction: the name to use for this Exit.
 *  - fromRoomId: the room from which this Exit starts
 *  - toRoomId: the room to which this Exit links
 *  - options (optional): a hash of the following options
 *      - cloak (optional): an itemId that must be in the user's
 *          inventory for them to be allowed to *see* the door.
 *      - lock (optional): an itemId that must be the user's inventory
 *          for them to be allowed through the door.
 *      - lockMessage (optional): the message to display to the user if
 *          they try to go through the exit but don't have the
 *          key item. Use this to create puzzle hints.
 *      - oneWay (optional): if truish, don't create the reverse link.
 *      - timeCloak (optional): a structure that defines a time period
 *          on which the exit is cloaked or visible:
 *          - period: the total time the visibility cycle takes.
 *          - shift (optional): an offset of time to push the start
 *              of the phase away. Defaults to 0.
 *          - width (optional): the proportion of time of the cycle that
 *              the phase is "on". Defaults to 0.5.
 *      - timeLocked (optional): a structure that defines a time period
 *          on which the exit is locked or unlocked:
 *          - period: the total time the lock cycle takes.
 *          - shift (optional): an offset of time to push the start
 *              of the phase away. Defaults to 0.
 *          - width (optional): the proportion of time of the cycle that
 *              the phase is "on". Defaults to 0.5.
 */
function Exit ( db, direction, fromRoomId, toRoomId, options )
{
  this.fromRoomId = checkRoomId( db, fromRoomId, "from" );
  var fromRoom = db.getRoom(this.fromRoomId);
  this.toRoomId = checkRoomId( db, toRoomId, "to" );

  direction = direction.toLocaleLowerCase();
  var id = direction && format(
      "exit-%s-from-%s-to-%s",
      direction,
      this.fromRoomId,
      this.toRoomId );

  Thing.call( this, db, "exits", id, direction );
  fromRoom.exits[direction] = this;
  options = options || { };

  this.cloak = checkLockSet( this.db, options.cloak );
  this.lock = checkLockSet( this.db, options.lock );
  this.lockMessage = options.lockMessage ||
      "You need a key to get through this exit.";

  this.timeCloak = checkTimerSet( options.timeCloak );
  this.timeLock = checkTimerSet( options.timeLock );

  if ( !options.oneWay )
  {
    options.oneWay = true;
    var reverse = new Exit( this.db, reverseDirection[direction], toRoomId,
        fromRoomId, options );
    this.reverseId = reverse.id;
  }
}

Exit.prototype = Object.create( Thing.prototype );
module.exports = Exit;

function parseLocker ( part, options, name )
{
    name = name.toLowerCase();
  if ( part === "with" )
  {
    options[name] = [ ];
  }
  else if ( part === "when" )
  {
    name = "time" + name;
    options[name] = { };
  }
  else if ( part[0] === '"' )
  {
    name = name + "message";
    options[name] = part.substring( 1 );
  }
  else
    name = part;

  return name;
}

var parsers =
    {
      none: function ( part, options )
      {
        return part;
      },
      to: function ( part, options )
      {
        options.toRoomId = part;
        return "none";
      },
      cloaked: function ( part, options )
      {
        return parseLocker( part, options, "cloak" );
      },
      locked: function ( part, options )
      {
        return parseLocker( part, options, "lock" );
      },
      cloak: function ( itemId, options )
      {
        return collectItemList( itemId, options, "cloak" );
      },
      lock: function ( itemId, options )
      {
        return collectItemList( itemId, options, "lock" );
      },
      timelock: function ( number, options )
      {
        return collectTimeConstraints( number, options, "lock" );
      },
      timecloak: function ( number, options )
      {
        return collectTimeConstraints( number, options, "cloak" );
      },
      lockmessage: function ( part, options )
      {
        var state = "lockmessage";
        if ( part[part.length - 2] !== '\\'
            && part[part.length - 1] === '"' )
        {
          part = part.substring( 0, part.length - 1 );
          state = "locked";
        }
        options.lockMessage += " " + part;
        if ( state === "locked" )
          options.lockMessage = options.lockMessage.replace( /\\"/g, '"' );
        return state;
      }
    };

function collectItemList ( itemId, options, name )
{
  var state = name;
  if ( itemId[itemId.length - 1] === ',' )
    itemId = itemId.substring( 0, itemId.length - 1 );
  else
    state = name + "ed";
  options[name].push( itemId );
  return state;
}

function collectTimeConstraints ( number, options, name )
{
  var prop = "time" + name;
  var state = prop;
  if ( number[number.length - 1] === ',' )
    number = number.substring( 0, number.length - 1 );
  else
    state = ( name + "ed" ).toLowerCase();
  number *= 1;
  if ( !options[prop].period )
    options[prop].period = number;
  else if ( !options[prop].width )
    options[prop].width = number;
  else if ( !options[prop].shift )
    options[prop].shift = number;
  return state;
}

Exit.parse = function ( db, fromRoomId, text )
{
  var options = {
    oneWay: ( text[0] === '-' )
  };

  if ( options.oneWay )
    text = text.substring( 1 );

  var parts = text.split( " " );
  var direction = parts[0];
  var state = "none";
  for ( var i = 1; i < parts.length; ++i ) {
    state = parsers[state]( parts[i], options );
  }

  return new Exit( db, direction, fromRoomId, options.toRoomId, options );
};

function checkLockSet ( db, lock )
{
  if ( !( lock instanceof Array ) )
    lock = lock ? [ lock ] : [ ];
  return lock.map( function ( itemId ) {
    if ( itemId.id )
      itemId = itemId.id;
    assert.ok( db.getItem(itemId), itemId + " is not an item." );
    return itemId;
  } );
}

var reverseDirection =
    {
      "north": "south",
      "east": "west",
      "south": "north",
      "up": "down",
      "down": "up",
      "west": "east",
      "exit": "enter",
      "enter": "exit"
    };

function checkRoomId ( db, roomId, name ) {
  if ( roomId.id ) {
    roomId = roomId.id;
  }

  assert.ok( roomId, name + "RoomId required" );
  roomId = roomId.toLocaleLowerCase();
  assert.ok( db.getRoom(roomId),
      "room \"" + roomId + "\" must exist before exit can be created." );

  return roomId;
}

function checkLockClosed ( db, lock, user )
{
  if ( typeof user === "string" || user instanceof String ){
    user = db.getPerson(user);
  }
  equip = core.values( user.equipment );
  return !lock.reduce( function ( prev, itemId ) {
    return prev && ( user.items[itemId] || equip.indexOf( itemId ) > -1 );
  }, true );
}

Exit.prototype.isCloakedTo = function ( user )
{
  return checkLockClosed( this.db, this.cloak, user );
};

Exit.prototype.isLockedTo = function ( user )
{
  return this.isCloakedTo( user )
      || checkLockClosed( this.db, this.lock, user );
};

function checkTimerSet ( timer )
{
  timer = timer || { };
  timer.period = timer.period || 1;
  assert( timer.period > 0, "timer period must be greater than 0" );
  timer.shift = timer.shift || 0;
  timer.width = timer.width || 0.5;
  assert( timer.width > 0, "timer width must be > 0" );
  assert( timer.width <= 1, "timer width must be <= 1" );
  if ( timer.period === 1 )
  {
    timer.mid = 1;
    timer.shift = 0;
    timer.width = 0;
  }
  timer.mid = Math.floor( timer.period * timer.width );
  return timer;
}

function checkTimerOn ( timer, t )
{
  t = t || core.time();
  t -= timer.shift;
  while ( t < 0 )
    t += timer.period;
  return ( t % timer.period ) < timer.mid;
}

Exit.prototype.isCloakedAt = function ( t )
{
  t = t || core.time();
  return checkTimerOn( this.timeCloak, t );
};

Exit.prototype.isLockedAt = function ( t )
{
  t = t || core.time();
  return this.isCloakedAt( t )
      || checkTimerOn( this.timeLock, t );
};

Exit.prototype.isCloaked = function ( user, t )
{
  t = t || core.time();
  return this.isCloakedTo( user )
      || this.isCloakedAt( t );
};

Exit.prototype.isLocked = function ( user, t )
{
  t = t || core.time();
  return this.isLockedTo( user )
      || this.isLockedAt( t );
};

Exit.prototype.describe = function ( user )
{
  var t = core.time();
  if ( this.isCloaked( user, t ) )
    return "";
  else
    return format( "\t%s to %s%s",
        this.description,
        this.toRoomId,
        this.isLocked( user, t ) ? " (LOCKED)" : "" );
};
