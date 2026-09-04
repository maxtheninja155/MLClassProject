namespace BossFight.Core
{
    /// <summary>
    /// Whatever is driving a body: human input, a scripted bot, or an ML agent.
    /// A body reads this once per FixedUpdate and acts on it. Swapping the source never changes the body.
    /// </summary>
    public interface IIntentSource
    {
        Intent GetIntent();
    }
}
