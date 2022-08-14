using CoreFeed.Core.Com.Entity.App;

namespace CoreFeed.Services.WorkFlowEngine
{
    /// <summary>
    /// Initialize work flow.
    /// </summary>
    public interface IWorkFlowRequestMapper
    {
        /// <summary>
        /// Extract work flow request from incoming request.
        /// </summary>
        /// <param name="workFlowApiRequest"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        WorkFlowParam Map(WorkFlowApiRequest workFlowApiRequest, Request request);
    }
}