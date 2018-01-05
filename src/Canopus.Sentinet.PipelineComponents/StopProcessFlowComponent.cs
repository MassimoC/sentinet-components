using Nevatech.Vsb.Repository.Processing;


namespace Canopus.Sentinet.PipelineComponents
{
    /// <summary>
    ///     Stop the message processing and directs the cached message into the beginning of the response pipeline.
    /// </summary>
    public sealed class StopProcessFlowComponent : IMessageProcessor
    {
        void IMessageProcessor.ImportConfiguration(string configuration)
        {
        }

        public MessagePipelineResult ProcessMessage(MessagePipelineContext context)
        {
            return MessagePipelineResult.Return;
        }

    }

}