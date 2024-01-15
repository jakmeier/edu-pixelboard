using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PixelBoard.MainServer.Services;
using PixelBoard.Grpc;

namespace PixelBoard.MainServer.Grpc;

public class GrcpService : RealTimeGoGrpc.RealTimeGoGrpcBase
{
    private readonly RealTimGoGameService _game;
    public GrcpService(RealTimGoGameService game)
    {
        _game = game;
    }

    public override async Task<TeamInfoReply> GetTeamInfo(TeamInfoRequest request, ServerCallContext context)
    {
        Dictionary<string, string?> info = await _game.GetTeamInfo(request.Team);

        string? score = info["Score"];
        string? budget = info["PaintBudget"];
        string? locked = info["Locked"];

        if (score is null || budget is null || locked is null)
            throw new DbCorruptException("Team Info has missing fields");

        return new TeamInfoReply
        {
            Score = uint.Parse(score),
            PaintBudget = uint.Parse(budget),
            Locked = Timestamp.FromDateTime(DateTime.Parse(locked))
        };
    }
}