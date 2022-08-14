using CoreFeed.Core.Com.Entity.App;
using System;


namespace CoreFeed.Core.Com.Entity.WorkFlowEngine
{
    /// <summary>
    /// The <see cref="Request"/> class used across, to extract the work flow request param from the incoming <see cref="Request"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class WorkFlowApiRequestExtractor<T> : Request
    {
        /// <summary>
        /// Param defined if any.
        /// </summary>
        public string AdditionalParam { get; set; }

        /// <summary>
        /// Request param.
        /// </summary>
        /// <remarks>Can be any request required for WorkFlow.</remarks>
        public T WfParam { get; set; }

        /// <summary>
        /// A WorkFlowName
        /// </summary>
        public string WorkFlow { get; set; }

        /// <summary>
        /// WorkFlowID unique if assigned
        /// </summary>
        public string WorkFlowId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// A category under which many <see cref="WorkFlow"/> can be combined.
        /// </summary>
        /// <remarks>E.g. <see cref="Scenario"/>= "Invoice" and <see cref="WorkFlow"/> = InvoiceApproval, InvoiceRejection etc. </remarks>
        public string Scenario { get; set; }
    }
}

namespace CoreFeed.Services.WorkFlowEngine
{
    /// <summary>
    /// WorkFlow API Request. That is the request to be received by server e.g. Logical App etc.
    /// See also: WorkFlowApiRequestExtractor, to extract the request from the incoming request to APP.
    /// </summary>
    public class WorkFlowApiRequest : Request
    {
        /*
         * How it works:
         * 
         * Cosumer will use like this this.
         * 
         * Step 1:
         * public class InvoiceApprovalWorkFlowParam : WorkFlowParam {}
         * 
         * Step 2:
         * THis class can be extracted using from the API request
         * request.ExtractRequest<WorkFlowApiRequestExtractor<InvoiceApprovalWorkFlowParam>>();
         * 
         * Step 2.1:
         * Define interface that can provide some data from the API.
         * e.g. IWorkFlowDataProvider { WorkFlowParam DataExtractor(Request request) }
         * 
         * Step 3:
         * Register the workflow in the container
         * RegisterServices<IWorkFlowDataProvider, InvoiceRequestDataExtractor>("InvoiceRequest"); //WorkFlowName  = InvoiceRequest
 
         * Step 4:
         * From the InvoiceRequestDataMapper return the WorkFlowParam
         * 
         * 
         * Step 5:
         * USe the data WorkFlowParam from the InvoiceRequestDataExtractor in the WorkFlowRequetMapper to build the WorkFlow request.
         * insode this amp the base class property that is missing in the WorkFlowParam
         * 
         * Step 6:
         * Use the mapper inside WorkFlowExecutor to call the actual workFlow api that is Logical app and build the response received.
         * 
         * */


        /// <summary>
        /// Param
        /// </summary>
        public string AdditionalParam { get; set; }

        /// <summary>
        /// WorkFLow Param
        /// </summary>
        public WorkFlowParam WfParam { get; set; }

        /// <summary>
        /// WorkFlow Name
        /// </summary>
        public string WorkFlow { get; set; }

        /// <summary>
        /// Unique Id for each request of WorkFlow.
        /// </summary>
        /// <remarks>This can be used to identify the status of the request.</remarks>
        public string WorkFlowId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// A category under which many <see cref="WorkFlow"/> can be combined.
        /// </summary>
        /// <remarks>E.g. <see cref="WorkFlowApiRequest.Scenario"/>= "Invoice" and <see cref="WorkFlowApiRequest.WorkFlow"/> = InvoiceApproval, InvoiceRejection etc. </remarks>
        public string Scenario { get; set; }
    }


    /// <summary>
    /// Response Received from the WorkFlow Api, e.g. Logical App.
    /// </summary>
    public class WorkFlowApiResponse : Response
    {
        /// <summary>
        /// A response received from the workflow engine. e.g. Logical APP.
        /// </summary>
        public WorkFlowResponse WorkFlowResponse { get; set; }
    }

    /// <summary>
    /// The common properties across workflows response.
    /// Derived the class that is required for each work from this class.
    /// You can derive one more common base class from the class and use that across your org.
    /// </summary>
    public class WorkFlowParam
    {
        /*
            {
              "associationId": null,
              "userRole": null,
              "processType": null
             }}
        */
        
        /// <summary>
        /// Process Type
        /// </summary>
        public string ProcessType { get; set; } = "Default";
        
        /// <summary>
        /// Current UserRole.
        /// </summary>
        public string UserRole { get; set; }

        /// <summary>
        /// UserID if any
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// WorkFlowRequest
    /// </summary>
    public class WorkFlowRequest
    {
        /*
           {
              "workFlow": 0,
              "param": null,
              "startTime": "0001-01-01T00:00:00",
              "lastWorkFlowResponse": null
            }
        */

        /// <summary>
        /// Keep track of all steps
        /// </summary>
        public WorkFlowResponse LastWorkFlowResponse { get; set; }

        /// <summary>
        /// Request to be validated by Rules, Before making call to WF fill this data
        /// </summary>
        public WorkFlowParam Param { get; set; } = new WorkFlowParam();

        /// <summary>
        /// Add Request Time = Defaullt is when WF is requested
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Current Workflow Name
        /// </summary>
        public string WorkFlow { get; set; }

        /// <summary>
        /// Unique Id for each request of WorkFlow
        /// </summary>
        public string WorkFlowId { get; set; }
    }

    public class WorkFlowResponse : WorkFlowResponseBase
    {
        /*
           {
              "workFlowPrevious": 0,
              "lastParam": null,
              "endTime": "0001-01-01T00:00:00",
              "lastWorkFlowRequest": null,
              "workFlowNext": 0,
              "workFlowPossibleNext": null,
              "toDoItems": null,
              "errors": null,
              "newParam": null
            }

        */

        /// <summary>
        /// End time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Workflow param used last time
        /// </summary>
        public WorkFlowParam LastParam { get; set; }

        /// <summary>
        /// To Keep track of the wf request
        /// </summary>
        public WorkFlowRequest LastWorkFlowRequest { get; set; }

        /// <summary>
        /// Unique Id for each request of WorkFlow
        /// </summary>
        public string WorkFlowId { get; set; }

        /// <summary>
        /// Which workflow gives the current response
        /// </summary>
        public string WorkFlowPrevious { get; set; }
    }

    public class WorkFlowResponseBase
    {
        /*
            {
              "workFlowNext": 0,
              "workFlowPossibleNext": null,
              "toDoItems": null,
              "errors": null,
              "newParam": null
            }
         */

        /// <summary>
        /// Error if any
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        /// Param to be send again in the next wf
        /// </summary>
        public WorkFlowParam NewParam { get; set; }

        /// <summary>
        /// Mandatory Items to complete, manually
        /// </summary>
        public string ToDoItems { get; set; }

        /// <summary>
        /// Which Workflow should be executed next
        /// </summary>
        public string WorkFlowNext { get; set; }

        /// <summary>
        /// Which Workflow should be executed next, possible
        /// </summary>
        public string WorkFlowPossibleNext { get; set; }
    }
}