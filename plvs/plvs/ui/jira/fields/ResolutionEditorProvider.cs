using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public sealed class ResolutionEditorProvider : NamedEntityComboEditorProvider {
        public ResolutionEditorProvider(JiraServer server, JiraField field, int selectedEntityId, IEnumerable<JiraNamedEntity> entities, FieldValidListener validListener) 
            : base(server, field, selectedEntityId, entities, validListener, false) {

            SortedDictionary<int, JiraNamedEntity> resolutions = JiraServerCache.Instance.getResolutions(server);
            if (resolutions.Count == 0) {
                return;
            }

            JiraNamedEntityComboBox comboBox = Widget as JiraNamedEntityComboBox;
            if (comboBox == null) {
                return;
            }
            if (comboBox.SelectedItem != null) {
                return;
            }
            foreach (var neItem in comboBox.Items
                    .OfType<ComboBoxWithImagesItem<JiraNamedEntity>>() 
                    .Where(neItem => neItem.Value.Id.Equals(resolutions.Keys.First()))) {
                comboBox.SelectedItem = neItem;
                return;
            }
        }
    }
}
