const net = require('net');
var Address4 = require('ip-address').Address4;
var Address6 = require('ip-address').Address6;
const GameServerPort = 13119;
const ClientServerPort = 13120;

var activeServers = {};
var connectedClients = {};

//###########################################GAME SERVER
var GameServer = net.createServer();
GameServer.on('connection', handleGameServerConnection);

GameServer.listen(GameServerPort, '0.0.0.0', function() { 
	//the address is needed so we use IPv4 by default
	console.log('game server listening to %j', GameServer.address());  
});

function handleGameServerConnection(conn) {
	var remoteAddressString = conn.remoteAddress + ':' + conn.remotePort;
	//figure out if IPv4 or IPv6 here
	var remoteAddress = new Address4(conn.remoteAddress);
	console.log('[GameServer] new client connection from %s', remoteAddressString);

	conn.on('data', onConnData);
	function onConnData(packet) {
		var receivedLength = packet.readUInt32BE();
		var receivedData = packet.subarray(4);
		if(receivedData.length != receivedLength) {
			console.log("[GameServer] error!! Length differs!");
		}

		var IpLengthBuffer = Buffer.alloc(1);
		IpLengthBuffer.writeInt8(4); //Length of IP address
		var IpBuffer = Buffer.from(remoteAddress.toArray());
		var outgoingData = Buffer.concat([IpLengthBuffer, IpBuffer, receivedData]);

		var dataLengthBuffer = Buffer.alloc(4);
		dataLengthBuffer.writeUInt32BE(outgoingData.length);
		var fullBuffer = Buffer.concat([dataLengthBuffer, outgoingData]);

		if(!activeServers[remoteAddressString] || Buffer.compare(activeServers[remoteAddressString], fullBuffer) != 0) {
			//Only update if anything changed
			activeServers[remoteAddressString] = fullBuffer;
			console.log("Updated game server list");
			updateClients();
		}
	}

	conn.once('close', onConnClose);
	function onConnClose() {
		delete activeServers[remoteAddressString];
		updateClients();
		console.log('[GameServer] connection from %s closed', remoteAddressString);  
	}

	conn.on('error', onConnError);
	function onConnError(err) {
		console.log('[GameServer] Connection %s error: %s', remoteAddressString, err.message);  
	}  
}

function updateClients() {
	console.log("Update the client lists");
	for(client in connectedClients) {
		for(var server in activeServers) {
			connectedClients[client].write(activeServers[server]);
		}
	}
}

//###########################################Client SERVER
var ClientServer = net.createServer();
ClientServer.on('connection', handleClientServerConnection);

ClientServer.listen(ClientServerPort, '0.0.0.0', function() { //the address is needed so we use IPv4 by default
	console.log('client server listening to %j', ClientServer.address());
});

function handleClientServerConnection(conn) {
	var remoteAddressString = conn.remoteAddress + ':' + conn.remotePort;
	//figure out if IPv4 or IPv6 here
	console.log('[ClientServer] new client connection from %s', remoteAddressString);
	connectedClients[remoteAddressString] = conn;

	for(var server in activeServers) {
		conn.write(activeServers[server]);
	}

	conn.on('data', onConnData);
	function onConnData(packet) {  
		console.log('[ClientServer] connection data from %s: %j', remoteAddressString, packet);
	}

	conn.once('close', onConnClose);
	function onConnClose() {
		delete connectedClients[remoteAddressString];
		console.log('[ClientServer] connection from %s closed', remoteAddressString);  
	}

	conn.on('error', onConnError);
	function onConnError(err) {
		console.log('[GameServer] Connection %s error: %s', remoteAddressString, err.message);  
	}  
}