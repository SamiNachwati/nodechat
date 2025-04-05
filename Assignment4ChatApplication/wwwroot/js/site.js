// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.

// TOP LEVEL SCOPE VARIABLES

let darkmode = localStorage.getItem("darkmode");
const themeSwitch = document.getElementById("theme-switch");


const enableDarkmode = () => {
    document.body.classList.add("darkmode");
    localStorage.setItem("darkmode", "active");
}

const disableDarkmode = () => {
    document.body.classList.remove("darkmode");
    localStorage.setItem("darkmode", null);
}

if (darkmode === "active") enableDarkmode();

themeSwitch.addEventListener("click", () => {
    darkmode = localStorage.getItem("darkmode")
    darkmode !== "active" ? enableDarkmode() : disableDarkmode()
})






var roomName;
var userName;
var disconnectButton = document.getElementById("btnDisconnect");
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatArea")
    .build();
var msgForm = document.forms.msgForm;


// CONNECTION CODE
connection.on('ReceiveMessage', displayMessage);

connection.start()
    .then(() => {
        console.log("Connected to SignalR.");
    })
    .catch(err => console.error("SignalR connection error:", err));

// USERNAME VALIDATION
document.getElementById('username').addEventListener("input", (e) => {
    e.target.value = e.target.value.replace(/\d/g, '');
});

// MESSAGE FORM SUBMIT
msgForm.addEventListener('submit', function (e) {
    e.preventDefault();
    var message = document.getElementById('userMessage');
    var text = message.value.trim();
    message.value = '';

    userName = document.getElementById('username').value;
    roomName = document.getElementById('roomName').value;

    if (text.length && userName.length && roomName.length) {
        sendMessage(userName, text);
    }
});


// JOIN ROOM BUTTON
document.getElementById("btnJoin").addEventListener('click', function (e) {
    e.preventDefault();
    roomName = document.getElementById('roomName').value;
    userName = document.getElementById('username').value;

    if (roomName && userName) {
        document.getElementById('btnJoin').disabled = true;
        disconnectButton.className = "btn btn-danger";
        document.getElementById("chatHistory").innerHTML = "";
        connection.invoke("JoinRoom", roomName, userName);
    }
});

// DISCONNECT BUTTON
disconnectButton.addEventListener("click", function (e) {
    e.preventDefault();
    roomName = document.getElementById('roomName').value;
    userName = document.getElementById('username').value;

    if (roomName && userName) {
        disconnectButton.className = "btn btn-danger visually-hidden";
        document.getElementById('btnJoin').disabled = false;
        document.getElementById('chatHistory').innerHTML = "";
        connection.invoke("DisconnectRoom", roomName, userName);
    }
});

// AUTO DISCONNECT ON TAB CLOSE
window.addEventListener("beforeunload", function () {
    const room = document.getElementById('roomName').value;
    const user = document.getElementById('username').value;

    if (room && user) {
        connection.invoke("DisconnectRoom", room, user);
    }
});

/**
 * Method used to send a message
 */
function sendMessage(userName, message) {
    if (message && message.length) {
        connection.invoke("SendMessage", roomName, userName, message);
    }
}

/**
 * Method used to show the message
 */
function displayMessage(name, message, time) {
    var friendlyTime = moment(time).format('H:mm:ss');
    var specialClass;

    switch (name) {
        case "Chat Area":
            specialClass = "systemUser";
            break;
        case userName:
            specialClass = "sender";
            break;
        default:
            specialClass = "recipient";
    }

    var userLi = document.createElement('li');
    userLi.className = 'userLi list-group-item ' + specialClass;
    userLi.textContent = `${friendlyTime}, ${name} says:`;
    userLi.style.color = localStorage.getItem("darkmode") !== "active" ? "black" : "white";

    // Listen for theme switch clicks
    themeSwitch.addEventListener("click", () => {
        const darkmodeStatus = localStorage.getItem("darkmode");
        userLi.style.color = darkmodeStatus !== "active" ? "black" : "white";
    });

    var messageLi = document.createElement('li');
    messageLi.className = 'messageLi list-group-item pl-5';
    messageLi.innerHTML = message;
 

    var chatHistoryUl = document.getElementById('chatHistory');
    chatHistoryUl.appendChild(userLi);
    chatHistoryUl.appendChild(messageLi);

    $('#chatHistory').animate({ scrollTop: $('#chatHistory').prop('scrollHeight') }, 50);
}
