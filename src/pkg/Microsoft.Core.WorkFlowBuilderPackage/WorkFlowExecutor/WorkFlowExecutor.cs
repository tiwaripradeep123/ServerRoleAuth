using CoreFeed.Core.Com.Entity.App;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CoreFeed.Services.WorkFlowEngine
{
    /// <summary>
    /// Execute workflow
    /// </summary>
    public class WorkFlowExecutor
    {
        /// <summary>
        /// Create <see cref="WorkFlowExecutor"/>
        /// </summary>
        /// <returns></returns>
        public WorkFlowExecutor Create()
        {
            return new WorkFlowExecutor();
        }

        /// <summary>
        /// Create <see cref="WorkFlowExecutor"/>
        /// </summary>
        /// <returns></returns>
        public T Create<T>() where T: WorkFlowExecutor
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Build Request
        /// </summary>
        /// <param name="reqParamFromApi"></param>
        /// <param name="extractBaseRequest"></param>
        /// <returns></returns>
        public virtual WorkFlowRequest BuildWorkFlowRequest(WorkFlowParam reqParamFromApi, WorkFlowApiRequest extractBaseRequest)
        {
            return new WorkFlowRequest()
            {
                Param = reqParamFromApi,
                StartTime = DateTime.UtcNow,
                WorkFlow = extractBaseRequest.WorkFlow,
                WorkFlowId = extractBaseRequest.WorkFlowId
            };
        }

        /// <summary>
        /// BuildWorkFlowApiResponse
        /// </summary>
        /// <param name="req"></param>
        /// <param name="reqParamFromApi"></param>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <param name="extractBaseRequest"></param>
        /// <returns></returns>

        public virtual WorkFlowApiResponse BuildWorkFlowApiResponse(WorkFlowRequest req, WorkFlowParam reqParamFromApi, WorkFlowResponseBase response, Request request, WorkFlowApiRequest extractBaseRequest)
        {
            return new WorkFlowApiResponse()
            {
                WorkFlowResponse = new WorkFlowResponse
                {
                    LastWorkFlowRequest = req,
                    EndTime = DateTime.UtcNow,
                    LastParam = reqParamFromApi,
                    ToDoItems = response.ToDoItems,
                    WorkFlowNext = response.WorkFlowNext,
                    NewParam = response.NewParam,
                    Errors = response.Errors,
                    WorkFlowPossibleNext = response.WorkFlowPossibleNext,
                    WorkFlowPrevious = extractBaseRequest.WorkFlow,
                    WorkFlowId = req.WorkFlowId
                },
                ExecutionContext = request.ExecutionContext
            };
        }

        /// <summary>
        ///  Execute work flow request if configured and supported by consumer.
        /// </summary>
        /// <param name="request">Request class that should have the workflow param.</param>
        /// <param name="mapper">Data to be passed to work flow.</param>
        /// <param name="action">Execure any work flow e.g. Logical APP.</param>
        /// <returns></returns>
        public virtual async Task<WorkFlowApiResponse> ExecuteAsync(Request request, IWorkFlowRequestMapper mapper, Func<string, Task<WorkFlowResponseBase>> action)
        {
            var extractBaseRequest = request.ExtractRequest<WorkFlowApiRequest>();

            // FOr each type of work flow request, data handler will provide additional data.
            var reqParamFromApi = mapper.Map(extractBaseRequest, request);

            var req = BuildWorkFlowRequest(reqParamFromApi, extractBaseRequest);

            var workFlowRequest = JsonConvert.SerializeObject(req);
            var response = await action.Invoke(workFlowRequest);

            return BuildWorkFlowApiResponse(req, reqParamFromApi, response, request, extractBaseRequest);
        }
    }
}