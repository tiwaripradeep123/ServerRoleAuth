using CoreFeed.Core.Com.Entity.App;

namespace CoreFeed.Services.WorkFlowEngine
{
    /// <inheritdoc/>
    public class WorkFlowRequestMapper : IWorkFlowRequestMapper
    {
        /// <inheritdoc/>
        public WorkFlowRequestMapper()
        {
        }

        /// <summary>
        /// Used to map common properties
        /// </summary>
        /// <param name="data"></param>
        /// <param name="workFlowApiRequest"></param>
        public virtual void MapResponseWithDataExtractor(object data, WorkFlowApiRequest workFlowApiRequest)
        {

        }

        /// <summary>
        /// By Default it resolve the dependency with <see cref="WorkFlowApiRequest.WorkFlow"/>
        /// </summary>
        /// <param name="workFlowApiRequest"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string GetResolutionName(WorkFlowApiRequest workFlowApiRequest, Request request)
        {
            return workFlowApiRequest.WorkFlow.ToString();
        }

        /// <inheritdoc/>
        public WorkFlowParam Map(WorkFlowApiRequest workFlowApiRequest, Request request)
        {
            /*
              Consumer of service will register IWorkFlowDataProvider per WorkFlow.
              This function will call data provider for that workflow.
            */

            //IWorkFlowDataProvider = dynamic
            var wfextractor = (dynamic)workFlowApiRequest.DTO.Invoke(GetResolutionName(workFlowApiRequest, request));
            //returns workflowParam
            var response = wfextractor.DataExtractor(request);

            //response.AssociationId = workFlowApiRequest.WfParam.AssociationId;
            MapResponseWithDataExtractor(response, workFlowApiRequest);

            response.ProcessType = workFlowApiRequest.WfParam.ProcessType;
            response.UserRole = workFlowApiRequest.WfParam.UserRole;

            return response;
        }
    }
}