using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using TutorService.Application.DTOs.VideoCall;
using TutorService.Application.Interfaces;
using TutorService.Web.Helpers;

namespace TutorService.Web.Hubs;

[Authorize]
public class VideoCallHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<VideoCallHub> _logger;
    
    private static readonly ConcurrentDictionary<Guid, VideoCallRoom> _videoCallRooms = new();
    
    private static readonly ConcurrentDictionary<string, Guid> _connectionUserMap = new();

    public VideoCallHub(IChatService chatService, ILogger<VideoCallHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        _connectionUserMap[Context.ConnectionId] = userId;
        
        _logger.LogInformation("VideoCall: User {UserId} connected with connection {ConnectionId}", 
            userId, Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        _connectionUserMap.TryRemove(Context.ConnectionId, out _);
        
        await HandleUserDisconnection(userId);
        
        _logger.LogInformation("VideoCall: User {UserId} disconnected", userId);
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinVideoCall(VideoCallJoinRequest request)
    {
        try
        {
            var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
            
            var chat = await _chatService.GetChatByIdAsync(request.ChatId, userId);
            if (chat == null)
            {
                await Clients.Caller.SendAsync("VideoCallError", "Chat not found or access denied");
                return;
            }

            var groupName = GetVideoCallGroupName(request.ChatId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            var room = _videoCallRooms.GetOrAdd(request.ChatId, _ => new VideoCallRoom
            {
                ChatId = request.ChatId
            });

            var participant = new VideoCallParticipant
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                JoinedAt = DateTime.UtcNow
            };

            room.Participants.AddOrUpdate(userId, participant, (_, __) => participant);

            var otherParticipants = room.GetOtherParticipantIds(userId).ToList();
            foreach (var otherId in otherParticipants)
            {
                if (room.Participants.TryGetValue(otherId, out var otherParticipant) && 
                    otherParticipant.IsConnected)
                {
                    await Clients.Client(otherParticipant.ConnectionId)
                        .SendAsync("ParticipantJoined", new { UserId = userId });
                }
            }

            var currentParticipants = room.Participants.Values
                .Where(p => p.IsConnected)
                .Select(p => p.UserId)
                .ToList();
            
            await Clients.Caller.SendAsync("VideoCallJoined", new
            {
                ChatId = request.ChatId,
                Participants = currentParticipants
            });

            _logger.LogInformation("User {UserId} joined video call for chat {ChatId}. Participants: {Participants}", userId, request.ChatId, currentParticipants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JoinVideoCall");
            await Clients.Caller.SendAsync("VideoCallError", "Internal server error");
        }
    }

    public async Task LeaveVideoCall(VideoCallJoinRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        await RemoveUserFromRoom(request.ChatId, userId, "left the call");
    }
    
    public async Task SendOffer(VideoCallOfferRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        _logger.LogInformation("SendOffer from {UserId} to {TargetUserId} in chat {ChatId}", userId, request.TargetUserId, request.ChatId);
    
        if (!_videoCallRooms.TryGetValue(request.ChatId, out var room))
        {
            await Clients.Caller.SendAsync("VideoCallError", "Room not found");
            return;
        }

        if (!room.HasParticipant(userId) || !room.HasParticipant(request.TargetUserId))
        {
            await Clients.Caller.SendAsync("VideoCallError", "Participants not in same call");
            return;
        }

        if (room.Participants.TryGetValue(request.TargetUserId, out var target) && target.IsConnected)
        {
            await Clients.Client(target.ConnectionId).SendAsync("OfferReceived", new
            {
                FromUserId = userId,
                Offer = request.Offer,
                ChatId = request.ChatId
            });
        }
    }
    
    public async Task SendAnswer(VideoCallAnswerRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
    
        if (!_videoCallRooms.TryGetValue(request.ChatId, out var room))
        {
            await Clients.Caller.SendAsync("VideoCallError", "Room not found");
            return;
        }

        if (!room.HasParticipant(userId) || !room.HasParticipant(request.TargetUserId))
        {
            await Clients.Caller.SendAsync("VideoCallError", "Participants not in same call");
            return;
        }

        if (room.Participants.TryGetValue(request.TargetUserId, out var targetParticipant) && 
            targetParticipant.IsConnected)
        {
            await Clients.Client(targetParticipant.ConnectionId).SendAsync("AnswerReceived", new
            {
                FromUserId = userId,
                Answer = request.Answer,
                ChatId = request.ChatId
            });
        }
    }

    public async Task SendIceCandidate(VideoCallIceCandidateRequest request)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        
        _logger.LogInformation("Ice candidate was sended for user {UserId}", userId);
    
        if (!_videoCallRooms.TryGetValue(request.ChatId, out var room))
        {
            return;
        }

        if (!room.HasParticipant(userId) || !room.HasParticipant(request.TargetUserId))
            return;

        if (room.Participants.TryGetValue(request.TargetUserId, out var targetParticipant) && 
            targetParticipant.IsConnected)
        {
            await Clients.Client(targetParticipant.ConnectionId).SendAsync("IceCandidateReceived", new
            {
                FromUserId = userId,
                Candidate = request.Candidate,
                ChatId = request.ChatId
            });
        }
    }

    private static string GetVideoCallGroupName(Guid chatId) => $"video_{chatId}";

    private bool TryGetRoomByParticipants(Guid userId1, Guid userId2, out VideoCallRoom room)
    {
        room = null!;
        foreach (var kvp in _videoCallRooms)
        {
            var r = kvp.Value;
            if (r.HasParticipant(userId1) && r.HasParticipant(userId2))
            {
                room = r;
                return true;
            }
        }
        return false;
    }

    private async Task RemoveUserFromRoom(Guid chatId, Guid userId, string reason)
    {
        if (!_videoCallRooms.TryGetValue(chatId, out var room))
            return;

        var groupName = GetVideoCallGroupName(chatId);
        if (_connectionUserMap.TryGetValue(Context.ConnectionId, out _))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        if (room.Participants.TryGetValue(userId, out var participant))
        {
            participant.IsConnected = false;
        }
        room.Participants.TryRemove(userId, out _);

        var otherParticipants = room.Participants.Values.Where(p => p.IsConnected).ToList();
        foreach (var other in otherParticipants)
        {
            await Clients.Client(other.ConnectionId).SendAsync("ParticipantLeft", new
            {
                UserId = userId,
                Reason = reason
            });
        }

        if (!room.IsActive)
        {
            _videoCallRooms.TryRemove(chatId, out _);
            _logger.LogInformation("Video call room {ChatId} removed (no active participants)", chatId);
        }

        _logger.LogInformation("User {UserId} removed from video call {ChatId}, reason: {Reason}", 
            userId, chatId, reason);
    }

    private async Task HandleUserDisconnection(Guid userId)
    {
        var roomsToLeave = _videoCallRooms.Values
            .Where(r => r.HasParticipant(userId))
            .Select(r => r.ChatId)
            .ToList();

        foreach (var chatId in roomsToLeave)
        {
            await RemoveUserFromRoom(chatId, userId, "disconnected");
        }
    }
    
    public async Task SetMicrophoneState(Guid chatId, bool isEnabled)
    {
        var userId = ControllerHelper.GetUserIdFromClaims(Context.User);
        if (!_videoCallRooms.TryGetValue(chatId, out var room))
            return;

        if (room.Participants.TryGetValue(userId, out var participant))
        {
            participant.IsMicrophoneEnabled = isEnabled;
        
            var otherParticipants = room.Participants.Values
                .Where(p => p.UserId != userId && p.IsConnected)
                .ToList();
        
            foreach (var other in otherParticipants)
            {
                await Clients.Client(other.ConnectionId).SendAsync("ParticipantMicrophoneStateChanged", new
                {
                    UserId = userId,
                    IsEnabled = isEnabled
                });
            }
        }
    }

}