using System;
using System.Collections.Generic;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.models.bamboo {
    public interface BambooBuildListModel {
        ICollection<BambooBuild> Builds { get; }

        BambooBuild getBuild(string key, BambooServer server);

        event EventHandler<EventArgs> ModelChanged;

        void removeAllListeners();

        void clear(bool notify);

        void addBuilds(ICollection<BambooBuild> newBuilds);
    }
}
