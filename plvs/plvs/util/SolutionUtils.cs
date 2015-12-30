using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using EnvDTE;
using DteConstants = EnvDTE.Constants;

namespace Atlassian.plvs.util {
    public static class SolutionUtils {
        public static bool openSolutionFile(string fileName, string lineAndColumnNumber, Solution solution) {
            List<ProjectItem> files = new List<ProjectItem>();

            matchProjectItems(fileName, files);

            ProjectItem selectedProjectItem = null;
            if (files.Count == 0) {
                MessageBox.Show("No matching files found for " + fileName, Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else if (files.Count > 1) {
                FileListPicker picker = new FileListPicker(files);
                if (DialogResult.OK == picker.ShowDialog()) {
                    selectedProjectItem = picker.SelectedFile;
                }
            } else {
                selectedProjectItem = files[0];
            }
            if (selectedProjectItem == null) return false;

            try {
                int? lineNo = null;
                int? columnNo = null;
                if (lineAndColumnNumber != null) {
                    string lineNoStr = lineAndColumnNumber.Contains(",")
                                           ? lineAndColumnNumber.Substring(0, lineAndColumnNumber.IndexOf(','))
                                           : lineAndColumnNumber;
                    string columnNumberStr = lineAndColumnNumber.Contains(",")
                                              ? lineAndColumnNumber.Substring(lineAndColumnNumber.IndexOf(',') + 1)
                                              : null;
                    lineNo = int.Parse(lineNoStr);
                    if (columnNumberStr != null) {
                        columnNo = int.Parse(columnNumberStr);
                    }
                }

                Window w = selectedProjectItem.Open(DteConstants.vsViewKindCode);
                w.Visible = true;
                w.Document.Activate();
                TextSelection sel = w.DTE.ActiveDocument.Selection as TextSelection;
                if (sel != null) {
                    sel.SelectAll();
                    if (lineNo.HasValue) {
                        // sometimes our current copy of the file is shorter than the line number that 
                        // the compiler reports for errors and bad HRESULT is returned from COM in this case. 
                        // Let's silently catch it here
                        try {
                            sel.MoveToDisplayColumn(lineNo.Value, columnNo.HasValue ? columnNo.Value : 0, false);
                            sel.Cancel();
                            return true;
                        } catch (Exception e) {
                            sel.Cancel();
                            PlvsUtils.showError("Unable to navigate to line " + lineNo.Value + " - no such line number in the file", e);
                        }
                    } else {
                        return true;
                    }
                } else {
                    throw new Exception("Unable to retrieve text selection for the document");
                }
            } catch (Exception ex) {
                PlvsUtils.showError("Unable to open the specified file", ex);
            }
            return false;
        }

        private static readonly List<ProjectItem> allProjectItems = new List<ProjectItem>();

        public static void refillAllSolutionProjectItems(Solution solution) {
            allProjectItems.Clear();
            foreach (Project project in solution.Projects) {
                refillProjectItems(project.ProjectItems);
            }
        }

        private static void refillProjectItems(ProjectItems items) {
            if (items == null) return;

            foreach (ProjectItem item in items) {
                allProjectItems.Add(item);
                refillProjectItems(item.ProjectItems);
            }
        }

        public static bool solutionContainsFile(string file, Solution solution) {
            List<ProjectItem> files = new List<ProjectItem>();
            matchProjectItems(file, files);
            return files.Count > 0;
        }

        private static void matchProjectItems(string file, ICollection<ProjectItem> files) {
            if (allProjectItems.Count == 0) {
                Debug.WriteLine("************ SolutionUtils.matchProjectItems() - empty project item list, have you forgotten to call refillAllSolutionProjectItems()?");
            }
            try {
                foreach (var item in allProjectItems) {
                    if (file.Contains("\\")) {
                        if (file.EndsWith("\\" + item.Name)) {
                            files.Add(item);
                        }
                    } else {
                        if (file.Equals(item.Name)) {
                            files.Add(item);
                        }
                    }
                }
            } catch(Exception e) {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
