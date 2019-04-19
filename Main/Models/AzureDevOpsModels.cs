using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureDevOps
{
    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Timeline
    {
        public string href { get; set; }
    }

    public class Badge
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Web web { get; set; }
        public Timeline timeline { get; set; }
        public Badge badge { get; set; }
    }

    public class Properties
    {
    }

    public class Plan
    {
        public string planId { get; set; }
    }

    public class TriggerInfo
    {
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Definition
    {
        public List<object> drafts { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string uri { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string queueStatus { get; set; }
        public int revision { get; set; }
        public Project project { get; set; }
    }

    public class Project2
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Pool
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isHosted { get; set; }
    }

    public class Queue
    {
        public int id { get; set; }
        public string name { get; set; }
        public Pool pool { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public Avatar avatar { get; set; }
    }

    public class RequestedFor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links2 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Avatar2
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public Avatar2 avatar { get; set; }
    }

    public class RequestedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links3 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Avatar3
    {
        public string href { get; set; }
    }

    public class Links4
    {
        public Avatar3 avatar { get; set; }
    }

    public class LastChangedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links4 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class OrchestrationPlan
    {
        public string planId { get; set; }
    }

    public class Logs
    {
        public int id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Repository
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public object clean { get; set; }
        public bool checkoutSubmodules { get; set; }
    }

    public class QueueBuildResult
    {
        public Links _links { get; set; }
        public Properties properties { get; set; }
        public List<object> tags { get; set; }
        public List<object> validationResults { get; set; }
        public List<Plan> plans { get; set; }
        public TriggerInfo triggerInfo { get; set; }
        public int id { get; set; }
        public string buildNumber { get; set; }
        public string status { get; set; }
        public DateTime queueTime { get; set; }
        public string url { get; set; }
        public Definition definition { get; set; }
        public int buildNumberRevision { get; set; }
        public Project2 project { get; set; }
        public string uri { get; set; }
        public string sourceBranch { get; set; }
        public Queue queue { get; set; }
        public string priority { get; set; }
        public string reason { get; set; }
        public RequestedFor requestedFor { get; set; }
        public RequestedBy requestedBy { get; set; }
        public DateTime lastChangedDate { get; set; }
        public LastChangedBy lastChangedBy { get; set; }
        public OrchestrationPlan orchestrationPlan { get; set; }
        public Logs logs { get; set; }
        public Repository repository { get; set; }
        public bool keepForever { get; set; }
        public bool retainedByRelease { get; set; }
        public object triggeredByBuild { get; set; }
    }

    #region "Definitions"
    public class AuthoredBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links2 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Value
    {
        public Links _links { get; set; }
        public string quality { get; set; }
        public AuthoredBy authoredBy { get; set; }
        public List<object> drafts { get; set; }
        public Queue queue { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string uri { get; set; }
        public string path { get; set; }
        public string type { get; set; }
        public string queueStatus { get; set; }
        public int revision { get; set; }
        public DateTime createdDate { get; set; }
        public Project project { get; set; }
    }

    public class GetDefinitionsResult
    {
        public int count { get; set; }
        public List<Value> value { get; set; }
    }
    #endregion

}