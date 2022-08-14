using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreFeed.Services.WorkFlowEngine.Examples
{
    [JsonConverter(typeof(StringEnumConverter))]
    /// <summary>
    /// For each workFlow create class to be inherited from WorkFlowParam
    /// </summary>
    public enum WorkFlow
    {
        /*

        */
        InvoiceRequest,
        InvoiceValidate,
        InvoiceApproval,
        InvoiceClose
    }

    public class InvoiceRequestWorkFlowParam : WorkFlowParam
    {
    }

    public class InvoiceValidateWorkFlowParam : WorkFlowParam
    {

        public string CaseDescrition { get; set; }
        public bool HasAttachment { get; set; }
        public string InvoiceRequestId { get; set; }
    }

    public class InvoiceApprovalWorkFlowParam : WorkFlowParam
    {
      
        public string ApprovelByRrole { get; set; }
    }

    public class InvoiceCloseWorkFlowParam : WorkFlowParam
    {
        
        public string CaseStatus { get; set; }
    }
}