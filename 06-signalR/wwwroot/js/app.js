"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/monitoringHub")
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .build();

let currentRoom = "";

const messagesList = document.getElementById("messagesList");
const status = document.getElementById("status");

function addMessage(text, className = "") {
    const item = document.createElement("li");
    item.textContent = text;

    if (className) {
        item.className = className;
    }

    messagesList.appendChild(item);
}

connection.on("ConnectionId", function (connectionId) {
    document.getElementById("connectionId").textContent = connectionId;
});

connection.on("OnlineCountUpdated", function (count) {
    document.getElementById("onlineCount").textContent = count;
});

connection.on("ReceiveMessage", function (user, message) {
    addMessage(`${user}: ${message}`);
});

connection.on("ReceiveSystemMessage", function (message) {
    addMessage(`Система: ${message}`);
});

connection.on("ReceiveRoomNotification", function (message) {
    addMessage(`Уведомление комнаты: ${message}`);
});

connection.on("ReceivePrivateMessage", function (message) {
    addMessage(`Приватное: ${message}`, "private");
});

connection.on("PrivateMessageSent", function () {
    addMessage("Приватное сообщение отправлено");
});

connection.on("RoomJoined", function (roomName) {
    addMessage(`Система: вход в комнату ${roomName}`);
});

connection.on("RoomLeft", function (roomName) {
    addMessage(`Система: выход из комнаты ${roomName}`);
});

connection.onreconnecting(function () {
    status.textContent = "Переподключение…";
});

connection.onreconnected(async function () {
    status.textContent = "Подключено";

    if (currentRoom) {
        await connection.invoke("JoinRoom", currentRoom);
    }
});

connection.onclose(function () {
    status.textContent = "Соединение потеряно";
});

document.getElementById("joinButton").addEventListener("click", async function () {
    const roomName = document.getElementById("roomInput").value;

    if (roomName.trim()) {
        currentRoom = roomName;
        await connection.invoke("JoinRoom", currentRoom);
    }
});

document.getElementById("leaveButton").addEventListener("click", async function () {
    if (currentRoom) {
        await connection.invoke("LeaveRoom", currentRoom);
        currentRoom = "";
    }
});

document.getElementById("sendButton").addEventListener("click", async function () {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    if (currentRoom && user && message) {
        await connection.invoke(
            "SendMessage",
            currentRoom,
            user,
            message);
    }
});

document.getElementById("privateButton")
    .addEventListener("click", async function () {
        const targetId = document.getElementById("targetInput").value;
        const user = document.getElementById("userInput").value;
        const message = document.getElementById("privateMessageInput").value;

        if (targetId && user && message) {
            await connection.invoke(
                "SendPrivateMessage",
                targetId,
                user,
                message);
        }
    });

document.getElementById("roomNotificationButton")
    .addEventListener("click", async function () {
        const message = document.getElementById("notificationInput").value;

        if (!currentRoom || !message) {
            return;
        }

        await fetch("/api/room-notification", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                roomName: currentRoom,
                message: message
            })
        });
    });

connection.start()
    .then(() => {
        status.textContent = "Подключено";
    })
    .catch(error => console.error(error));
