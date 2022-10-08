namespace Cryptomarkets.Apis.Binance
{
  /// <summary>
  /// Each withdraw endpoint has a security type that determines the how you will interact with it
  /// </summary>
  public enum EndpointSecurityType
  {
    /// <summary>Endpoint can be accessed freely.</summary>
    NONE,
    /// <summary>
    /// Endpoint requires sending a valid API-Key and signature.
    /// </summary>
    TRADE,
    /// <summary>
    /// Endpoint requires sending a valid API-Key and signature.
    /// </summary>
    USER_DATA,
    /// <summary>Endpoint requires sending a valid API-Key.</summary>
    USER_STREAM,
    /// <summary>Endpoint requires sending a valid API-Key.</summary>
    MARKET_DATA,
  }
}
