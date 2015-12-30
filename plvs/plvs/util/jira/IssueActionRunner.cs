using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.facade;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.util.jira {
    public sealed class IssueActionRunner {
        public static void runAction(Control owner, JiraNamedEntity action, JiraIssueListModel model, JiraIssue issue, StatusLabel status, Action onFinish) {
            Thread runner = PlvsUtils.createThread(delegate {
                                                       try {
                                                           status.setInfo("Retrieving fields for action \"" + action.Name + "\"...");
                                                           List<JiraField> fields = SmartJiraServerFacade.Instance.getFieldsForAction(issue, action.Id);
                                                           runAction(owner, action, model, issue, fields, status, onFinish);
                                                       } catch (Exception e) {
                                                           status.setError("Failed to run action " + action.Name + " on issue " + issue.Key, e);
                                                       }
                                                   });
            runner.Start();
        }

        private static void runAction(Control owner, JiraNamedEntity action, JiraIssueListModel model,
                                      JiraIssue issue, List<JiraField> fields, StatusLabel status, Action onFinish) {

            JiraIssue issueWithTime = SmartJiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
            issueWithTime.SecurityLevel = SmartJiraServerFacade.Instance.getSecurityLevel(issue);
            object rawIssueObject = SmartJiraServerFacade.Instance.getRawIssueObject(issue);
            List<JiraField> fieldsWithValues = JiraActionFieldType.fillFieldValues(issue, rawIssueObject, fields);
            
            // PLVS-133 - this should never happen but does?
            if (model == null) {
                owner.Invoke(new MethodInvoker(()
                    =>
                    PlvsUtils.showError("Issue List Model was null, please report this as a bug",
                    new Exception("IssueActionRunner.runAction()"))));
                model = JiraIssueListModelImpl.Instance;
            }

            if (fieldsWithValues == null || fieldsWithValues.Count == 0) {
                runActionWithoutFields(owner, action, model, issue, status, onFinish);
            } else {
                owner.Invoke(new MethodInvoker(() => 
                    new IssueWorkflowAction(issue, action, model, fieldsWithValues, status, onFinish).initAndShowDialog()));
            }
        }

        private static void runActionWithoutFields(
            Control owner, JiraNamedEntity action, JiraIssueListModel model, 
            JiraIssue issue, StatusLabel status, Action onFinish) {

            status.setInfo("Running action \"" + action.Name + "\" on issue " + issue.Key + "...");
            SmartJiraServerFacade.Instance.runIssueActionWithoutParams(issue, action);
            status.setInfo("Action \"" + action.Name + "\" successfully run on issue " + issue.Key);
            var newIssue = SmartJiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
            UsageCollector.Instance.bumpJiraIssuesOpen();
            owner.Invoke(new MethodInvoker(() => { model.updateIssue(newIssue); if (onFinish != null) onFinish(); }));
        }
    }
}