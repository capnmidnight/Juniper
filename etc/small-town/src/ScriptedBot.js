/* global require, module, exports */
﻿var AIBody = require( "./aibody.js" );
var Message = require( "./message.js" );
var Aggressor = require( "./aggressor.js" );
var core = require( "./core.js" );
var format = require( "util" ).format;
var fs = require( "fs" );

// ScriptedBot class
//  A bot that can do things in reaction to user actions
function ScriptedBot ( db, fileName ) {
  this.tutorial = fs
      .readFileSync( fileName, { encoding: "utf8" } )
      .split( "\n" )
      .map( function ( l ) {
        return l.trim();
      } )
      .filter( function ( l ) {
        return l.length > 0;
      } );
  var id = fileName.match( /([^\\/]+).script$/ )[1];
  var roomId = this.tutorial.shift().toLocaleLowerCase();
  var initialItems = JSON.parse( this.tutorial.shift() );

  AIBody.call( this, db, roomId, Number.MAX_VALUE, initialItems, {
    "tool": "katana", "torso": "imperial-breastplate" }, id );

  this.users = { };
  this.monster = null;
  this.dt = 2000;
}

ScriptedBot.prototype = Object.create( AIBody.prototype );
module.exports = ScriptedBot;

ScriptedBot.prototype.doForEveryone = function ( thunk ) {
  for ( var userId in this.users ) {
    userId = userId.toLocaleLowerCase();
    var stepNo = this.users[userId];
    if ( stepNo < this.tutorial.length ) {
      var step = this.tutorial[stepNo]
          .replace( /USERID/g, userId );
      var delta = thunk.call( this, userId, step );
      if ( delta > 0 )
        this.users[userId] += delta;
    }
  }
};

ScriptedBot.prototype.idleAction = function () {
  this.doForEveryone( function ( userId, step ) {
    if ( step.substring( 0, 4 ) !== "wait" ) {
      if ( step !== "skip" )
        this.cmd( step );
      return 1;
    }
    return 0;
  } );
};

ScriptedBot.prototype.react = function ( msg ) {
  if ( this.db.getPerson(msg.fromId) && msg.fromId !== this.id ) {
    var fromId = msg.fromId.toLocaleLowerCase();
    if ( this.users[fromId] === undefined )
      this.users[fromId] = 0;

    var cmd = format( "wait %s %s %s", msg.fromId, msg.message,
        msg.payload.join( " " ) );
    cmd = cmd.trim()
        .toLowerCase();
    this.doForEveryone( function ( userId, step ) {
      return ( step.toLowerCase() === cmd ? 1 : 0 );
    } );
  }
};

ScriptedBot.prototype.cmd_done = function ( userId ) {
  if ( this.users[userId] ) {
    userId = userId.toLocaleLowerCase();
    delete this.users[userId];
  }
};

ScriptedBot.prototype.cmd_spawn = function ( monsterName ) {
  var name = this.id + "'s-" + monsterName;
  if ( !this.db.getPerson(name) )
    this.monster = new Aggressor(
        this.db,
        this.roomId.toLocaleLowerCase(),
        10,
        { "gold": 10 },
    null,
        name );
};

ScriptedBot.loadFromDir = function loadFromDir ( db, dirName ) {
  fs.readdirSync( dirName )
      .filter( function ( f ) {
        return f.match( /\.script$/ );
      } )
      .forEach( function ( f ) {
        console.log(f);
        new ScriptedBot( db, dirName + f );
      } );
};