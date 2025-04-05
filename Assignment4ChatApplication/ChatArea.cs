using Assignment4ChatApplication.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Win32.SafeHandles;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Assignment4ChatApplication

// I, Sami Nachwati, student number 000879289, certify that this material is my original work. No other person's
// work has been used without due acknowledgment and I have not made my work available to anyone else.
{
    public class ChatArea : Hub
    {
        /* db context */
        private readonly ChatDbContext _db;
        
        /* hashmap for mapping rooms */
        private static readonly ConcurrentDictionary<string, Room> chatRooms = new();

        /* hashmap for mapping users to connection ids */
        private static readonly ConcurrentDictionary<string, string> userNames = new();

        /* hashmap for mapping name of room with user logs */
        private static readonly ConcurrentDictionary<string, List<User>> messageLogs = new(); 

        /// <summary>
        /// Constructor for ChatArea
        /// </summary>
        /// <param name="db">db context</param>
        public ChatArea(ChatDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Method used to admit user into room
        /// </summary>
        /// <param name="roomName">name of room</param>
        /// <param name="username">name of user</param>
        /// <returns>Asynchronous task job</returns>
        public async Task JoinRoom(string roomName, string username)
        {
            try { 
                roomName = roomName.ToLower();
                string currentConnectionId = Context.ConnectionId;

                userNames[currentConnectionId] = username;

                if (!chatRooms.ContainsKey(roomName))
                {
                    var newRoom = new Room
                    {
                        RoomName = roomName,
                        ConnectionIds = new List<string> { currentConnectionId }
                    };
                    chatRooms.TryAdd(roomName, newRoom);
                }
                else if (chatRooms.TryGetValue(roomName, out var existingRoom))
                {
                    if (!existingRoom.ConnectionIds.Contains(currentConnectionId))
                        existingRoom.ConnectionIds.Add(currentConnectionId);
                }

                await Groups.AddToGroupAsync(currentConnectionId, roomName);

                if (messageLogs.ContainsKey(roomName))
                {
                    foreach (var logUser in messageLogs[roomName])
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", logUser.Username, logUser.Message, logUser.MessageAt);
                    }
                }

                DateTimeOffset connectedAt = DateTimeOffset.Now;

                if (!_db.HasUser(username))
                {
                    _db.Users.Add(new User
                    {
                        Username = username,
                        ConnectedAt = connectedAt
                    });
                    _db.SaveChanges();
                }
                else
                {
                    _db.UpdateUserConnectionTime(username);
                    var user = _db.GetUser(username);
                    connectedAt = user?.ConnectedAt ?? connectedAt;
                }

                await Clients.Caller.SendAsync("ReceiveMessage", "Chat Area", $"You joined room: {roomName}!", connectedAt);
                await Clients.Group(roomName).SendAsync("ReceiveMessage", "Chat Area", $"{username} Connected ({connectedAt:T})", connectedAt);
                // Store "connected" system message
                if (!messageLogs.ContainsKey(roomName))
                    messageLogs[roomName] = new List<User>();

                messageLogs[roomName].Add(new User
                {
                    Username = "Chat Area",
                    Message = $"{username} Connected ({connectedAt:T})",
                    MessageAt = connectedAt
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JoinRoom ERROR] {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION] {ex.InnerException.Message}");
                }

                throw;
            }
        }



        /// <summary>
        /// Method used to remove user from user
        /// </summary>
        /// <param name="roomName">name of room</param>
        /// <param name="username">name of user</param>
        /// <returns>Asynchronous task job</returns>
        public async Task DisconnectRoom(string roomName, string username)
        {
            if (chatRooms.TryGetValue(roomName, out var room))
            {
                room.ConnectionIds.Remove(Context.ConnectionId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            userNames.TryRemove(Context.ConnectionId, out _);

            var user = _db.GetUser(username);
            if (user != null)
            {
                user.DisconnectedAt = DateTimeOffset.Now;
                _db.SaveChanges();
            }

            await Clients.Group(roomName).SendAsync("ReceiveMessage", "Chat Area", $"{username} disconnected ({user.DisconnectedAt:T})", user.DisconnectedAt);
            if (!messageLogs.ContainsKey(roomName))
                messageLogs[roomName] = new List<User>();

            messageLogs[roomName].Add(new User
            {
                Username = "Chat Area",
                Message = $"{username} disconnected ({user.DisconnectedAt:T})",
                MessageAt = user.DisconnectedAt ?? DateTimeOffset.Now
            });

        }

        /// <summary>
        /// Method used to detect automatic disconnection when user exits tab
        /// </summary>
        /// <param name="exception">error exception</param>
        /// <returns>Asynchronous task job</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            if (userNames.TryRemove(connectionId, out string? username))
            {
                var room = chatRooms.Values.FirstOrDefault(r => r.ConnectionIds.Contains(connectionId));

                if (room != null)
                {
                    room.ConnectionIds.Remove(connectionId);

                    await Groups.RemoveFromGroupAsync(connectionId, room.RoomName);

                    var user = _db.GetUser(username);
                    if (user != null)
                    {
                        user.DisconnectedAt = DateTimeOffset.Now;
                        _db.SaveChanges();
                    }

                    await Clients.Group(room.RoomName).SendAsync("ReceiveMessage", "Chat Area", $"{username} disconnected ({DateTimeOffset.Now:T})", DateTimeOffset.Now);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Method used to send a message
        /// </summary>
        /// <param name="roomname">name of room</param>
        /// <param name="userName">name of user</param>
        /// <param name="textMessage">content of message</param>
        /// <returns>Asynchronous task job</returns>
        public async Task SendMessage(string roomname, string userName, string textMessage)
        {

            // If the user is not currently tracked as connected, block them
            if (!userNames.ContainsKey(Context.ConnectionId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Chat Area", "You must join a room before sending messages.", DateTimeOffset.Now);
                return;
            }


            userNames[Context.ConnectionId] = userName; // Always update mapping

            if (!_db.HasUser(userName))
            {
                _db.Users.Add(new User()
                {
                    Username = userName,
                    MessageAt = DateTimeOffset.Now,
                    Message = textMessage,
                });
                _db.SaveChanges();
            }
            else
            {
                _db.UpdateUserMessageTime(userName);
                _db.UpdateUserMessage(userName, textMessage);
                _db.SaveChanges();
            }

            var user = _db.GetUser(userName);
            await Clients.Group(roomname.ToLower()).SendAsync("ReceiveMessage", user.Username, user.Message, user.MessageAt);
            roomname = roomname.ToLower();
            if (!messageLogs.ContainsKey(roomname))
            {
                messageLogs[roomname] = new List<User>();
            }

            messageLogs[roomname].Add(new User
            {
                Username = user.Username,
                Message = user.Message,
                MessageAt = user.MessageAt
            });
        }

    }
}
