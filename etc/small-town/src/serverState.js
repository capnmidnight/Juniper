/* global require, module, exports */
var fs = require( "fs" );
var loadRooms = require( "./room.js" ).loadFromDir;
var loadItems = require( "./item.js" ).load;
var loadRecipes = require( "./recipe.js" ).load;
var loadBots = require( "./bots.js" );
var assert = require( "assert" );
var core = require( "./core.js" );
var format = require( "util" ).format;

function ServerState () {
  this.users = { };
  this.items = { };
  this.rooms = { };
  this.exits = { };
  this.recipes = { };
  this.lastSpawn = 0;
  this.respawnRate = 5 * 60 * 1000;

  loadItems( this, "./data/items.txt" );
  loadRecipes( this, "./data/recipes.txt" );
  loadRooms( this, "./data/rooms/" );
  loadBots( this );
}
;
module.exports = ServerState;

ServerState.prototype.isNameInUse = function ( name ) {
  return !!this.getPerson(name);
};

ServerState.prototype.getPeopleIn = function ( roomId, excludeUserId ) {
  roomId = roomId.toLocaleLowerCase();
  return core.values( this.users )
      .filter( function ( user ) {
        return user.roomId.toLocaleLowerCase() === roomId
            && user.id !== excludeUserId;
      } );
};

function from(name, id){
  return this[name][id.toLocaleLowerCase()];
}

ServerState.prototype.getPerson = function ( userId, roomId ) {
  roomId = roomId && roomId.toLocaleLowerCase();
  var user = from.call(this, "users", userId);
  if ( !roomId || ( user && user.roomId === roomId ) )
    return user;
};

ServerState.prototype.getItem = function ( itemId ) {
  return from.call(this, "items", itemId);
};

ServerState.prototype.getRoom = function ( roomId ) {
  return from.call(this, "rooms", roomId);
};

ServerState.prototype.getExit = function ( exitId ) {
  return from.call(this, "exits", exitId);
};


ServerState.prototype.getRecipe = function ( recipeId ) {
  return from.call(this, "recipes", recipeId);
};
ServerState.prototype.inform = function ( message, roomId, excludeUserId ) {
  for ( var userId in this.users ) {
    if ( userId !== excludeUserId ) {
      var user = this.getPerson(userId, roomId);
      if ( user ) {
        user.informUser( message );
      }
    }
  }
};

ServerState.prototype.pump = function () {
  this.respawn();
  this.updateUsers();
};

ServerState.prototype.respawn = function () {
  var now = Date.now();
  if ( ( now - this.lastSpawn ) > this.respawnRate ) {
    for ( var userId in this.users ) {
      this.spawnNPC( userId );
    }

    for ( var roomId in this.rooms ) {
      this.getRoom(roomId).spawnItems();
    }

    this.lastSpawn = now;
  }
};

ServerState.prototype.spawnNPC = function ( userId ) {
  var user = this.getPerson(userId);
  user.hp = user.startHP || user.hp;
};

ServerState.prototype.updateUsers = function () {
  for ( var bodyId in this.users ) {
    var body = this.getPerson(bodyId);
    if ( body.quit ) {
      body.socket.disconnect();
    }
    else {
      body.update();
    }
  }
};