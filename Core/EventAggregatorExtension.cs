namespace Core;

/// <summary>
/// These are extension methods for the event aggregator to reduce code duplication
/// Due to the architecture of prism disposing of subscribers that don't have a strong reference,
/// combined with Darwing being largely event driven, with the DI container constructing most classes, resulting in
/// them not having strong references, we have to use the keepSubscriberReferenceAlive flag on most subscriptions.
/// </summary>
public static class EventAggregatorExtension
{
    public static void BackGroundSubscribe(this PubSubEvent e, Action action)
    {
        e.Subscribe(action, ThreadOption.BackgroundThread, true);
    }

    public static void BackGroundSubscribe<T>(this PubSubEvent<T> e, Action<T> action)
    {
        e.Subscribe(action, ThreadOption.BackgroundThread, true);
    }
}