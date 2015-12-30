using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.jira.fields {
    public class NamedEntityComboEditorProvider : FixedHeightFieldEditorProvider {
        private readonly JiraNamedEntityComboBox combo = new JiraNamedEntityComboBox();

        public NamedEntityComboEditorProvider(
            JiraServer server, JiraField field, int selectedEntityId, IEnumerable<JiraNamedEntity> entities, FieldValidListener validListener) 
            : base(field, validListener) {
            init(server, selectedEntityId, entities, true);
        }

        public NamedEntityComboEditorProvider(
            JiraServer server, JiraField field, int selectedEntityId, IEnumerable<JiraNamedEntity> entities, FieldValidListener validListener, bool useImages)
            : base(field, validListener) {
            init(server, selectedEntityId, entities, useImages);
        }

        private void init(JiraServer server, int selectedEntityId, IEnumerable<JiraNamedEntity> entities, bool useImages) {

            ImageList imageList = new ImageList();
            int i = 0;
            int selectedIndex = -1;
            if (entities != null) {
                foreach (var entity in entities) {
                    ComboBoxWithImagesItem<JiraNamedEntity> item = new ComboBoxWithImagesItem<JiraNamedEntity>(entity, i);
                    if (useImages) {
                        imageList.Images.Add(ImageCache.Instance.getImage(server, entity.IconUrl).Img);
                    }
                    combo.Items.Add(item);
                    if (selectedEntityId != JiraIssue.UNKNOWN && selectedEntityId == entity.Id) {
                        selectedIndex = i;
                    }
                    ++i;
                }
            }
            
            if (useImages) {
                combo.ImageList = imageList;
            }

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Height = SINGLE_LINE_EDITOR_HEIGHT + 6;

            combo.ItemHeight = SINGLE_LINE_EDITOR_HEIGHT;

            combo.SelectedIndexChanged += combo_SelectedIndexChanged;
            if (selectedIndex != -1) {
                combo.SelectedIndex = selectedIndex;
            }
            
            FieldValid = combo.SelectedItem != null;
        }

        private void combo_SelectedIndexChanged(object sender, System.EventArgs e) {
            FieldValid = combo.SelectedItem != null;
        }

        public override Control Widget {
            get { return combo; }
        }

        public override int VerticalSkip {
            get { return combo.Height; }
        }

        public override void resizeToWidth(int width) {
            combo.Width = width;
        }

        public override List<string> getValues() {
            return combo.SelectedItem != null 
                       ? new List<string> { ((ComboBoxWithImagesItem<JiraNamedEntity>) combo.SelectedItem).Value.Id.ToString() } 
                       : new List<string>();
        }
    }
}