using System;

namespace Atlassian.plvs.ui.jira.issues {
    public interface ToolWindowStateMonitor {
        event EventHandler<EventArgs> ToolWindowShown;
        event EventHandler<EventArgs> ToolWindowHidden;
    }
}
