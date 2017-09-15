// Write your Javascript code.
(function () {
    var webSocketProtocol = location.protocol === "https:" ? "wss:" : "ws:";
    var webSocketUri = webSocketProtocol + "//" + location.host + "/ws";

    var socket = new WebSocket(webSocketUri);
    var logelement = document.getElementById("log");

    socket.onopen = function () {
        log("Connected.");
    };

    socket.onclose = function (event) {
        if (event.wasClean) {
            log('Disconnected.');
        } else {
            log('Connection lost.'); // for example if server processes is killed
        }
        log('Code: ' + event.code + '. Reason: ' + event.reason);
    };

    socket.onmessage = function (event) {
        log(":> " + event.data);
    };

    socket.onerror = function (error) {
        log("Error: " + error.message);
    };

    var form = document.getElementById('form');
    var message = document.getElementById('message');
    form.onsubmit = function () {
        socket.send(message.value);
        message.value = '';
        return false;
    };
    function log(message) {
        logelement.innerHTML += '<span>' + message + '</span><br/>';
    }

})();