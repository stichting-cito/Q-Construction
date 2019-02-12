namespace Citolab.QConstruction.Logic.DomainLogic.Events.Stats
{
    /// <summary>
    ///     Interface to update stats
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdateStatsEventProcessor<in T> where T : BaseEvent
    {
        /// <summary>
        /// </summary>
        /// <param name="basevent"></param>
        /// <returns></returns>
        bool Process(T basevent);
    }
}