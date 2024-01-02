using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

public interface IRedisConnectionFactory
{
    ConnectionMultiplexer Connection();
}

public class RedisConnectionFactory : IRedisConnectionFactory
{
    private readonly Lazy<ConnectionMultiplexer> _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost"));

    public ConnectionMultiplexer Connection()
    {
        return this._connection.Value;
    }
}