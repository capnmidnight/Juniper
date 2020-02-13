/* global require, module, exports */
var AIBody = require( "./aibody.js" );
var core = require( "./core.js" );
var format = require( "util" ).format;

function ShopKeep ( db, roomId, hp, items, prices, equipment, id )
{
  AIBody.call( this, db, roomId, hp, items, equipment, id );
  this.prices = { };
  for ( var sellId in prices )
  {
    sellId = sellId.toLocaleLowerCase();
    this.prices[sellId] = { };
    for ( var costId in prices[sellId] ) {
      costId = costId.toLocaleLowerCase();
      this.prices[sellId][costId] = prices[sellId][costId];
    }
  }
}

ShopKeep.prototype = Object.create( AIBody.prototype );
module.exports = ShopKeep;

function vkString ( k, v ) {
  return format( "%d %s", v, k );
}
ShopKeep.prototype.react_tell = function ( m )
{
  var target = this.db.getPerson( m.fromId, this.roomId );
  if ( target )
  {
    var msg = m.payload[0];
    if ( msg === "inv" )
    {
      var output = "";
      for ( var itemId in this.items )
        itemId = itemId.toLocaleLowerCase();
        if ( this.prices[itemId] )
          output += format(
              "\t%s (%d) - %s\n\n",
              itemId,
              this.items[itemId],
              core.hashMap(
                  this.prices[itemId],
                  vkString )
              .join( "," ) );
      if ( output.length === 0 )
        output = " nothing";
      else
        output = "\n\n" + output;
      this.cmd( format( "tell %s I have:%s", m.fromId, output ) );
    }
    else if ( AIBody.prototype.react_tell )
      AIBody.prototype.react_tell.call( this, m );
  }
};

ShopKeep.prototype.react_buy = function ( m )
{
  var target = this.db.getPerson( m.fromId, this.roomId );
  if ( target )
  {
    var itemId = m.payload[0].toLocaleLowerCase();
    var item = this.items[itemId];
    var price = this.prices[itemId];
    core.test( ">>>> BUY", itemId, item, price );
    for ( var id in price ) {
      id = id.toLocaleLowerCase();
      core.test( ">>> USR", id, target.items[id] );
    }
    if ( !item )
      this.cmd( format( "tell %s item not available", m.fromId ) );
    else if ( !core.hashSatisfies( target.items, price ) )
      this.cmd( format( "tell %s price not met", m.fromId ) );
    else
    {
      for ( var k in price ) {
        k = k.toLocaleLowerCase();
        target.cmd_give( this.id, k, price[k] );
      }
      this.cmd_give( target.id, itemId );
      this.cmd( format( "tell %s pleasure doing business",
          m.fromId ) );
    }
  }
};

ShopKeep.prototype.react_sell = function ( m )
{
  var target = this.db.getPerson( m.fromId, this.roomId );
  if ( target )
  {
    var itemId = m.payload[0].toLocaleLowerCase();
    var item = target.items[itemId];
    var price = this.prices[itemId];
    if ( !item ) {
      this.cmd( format( "tell %s you don't have that item", m.fromId ) );
    }
    else if ( !core.hashSatisfies( this.items, price ) ) {
      this.cmd( format( "tell %s I can't afford that", m.fromId ) );
    }
    else
    {
      for ( var k in price ) {
        k = k.toLocaleLowerCase();
        this.cmd_give( target.id, k, price[k] );
      }
      target.cmd_give( this.id, itemId );
      this.cmd( format( "tell %s pleasure doing business",
          m.fromId ) );
    }
  }
};

ShopKeep.prototype.idleAction = function ()
{
  // do nothing, and keep the shopkeep where they are set.
};
