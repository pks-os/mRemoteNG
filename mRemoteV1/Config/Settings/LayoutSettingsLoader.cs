﻿using mRemoteNG.App;
using mRemoteNG.App.Info;
using mRemoteNG.UI.Forms;
using mRemoteNG.UI.Window;
using System;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;

namespace mRemoteNG.Config.Settings
{
    public class LayoutSettingsLoader
    {
        private readonly frmMain _mainForm;

        public LayoutSettingsLoader(frmMain mainForm)
        {
            _mainForm = mainForm;
        }

        public void LoadPanelsFromXml()
        {
            try
            {
                while (_mainForm.pnlDock.Contents.Count > 0)
                {
                    var dc = (DockContent)_mainForm.pnlDock.Contents[0];
                    dc.Close();
                }

                CreatePanels();
#if !PORTABLE
                var oldPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + GeneralAppInfo.ProductName + "\\" + SettingsFileInfo.LayoutFileName;
#endif
                var newPath = SettingsFileInfo.SettingsPath + "\\" + SettingsFileInfo.LayoutFileName;
                if (File.Exists(newPath))
                {
                    _mainForm.pnlDock.LoadFromXml(newPath, GetContentFromPersistString);
#if !PORTABLE
				}
				else if (File.Exists(oldPath))
				{
					_mainForm.pnlDock.LoadFromXml(oldPath, GetContentFromPersistString);
#endif
                }
                else
                {
                    frmMain.Default.SetDefaultLayout();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("LoadPanelsFromXML failed" + Environment.NewLine + ex.Message);
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            // pnlLayout.xml persistence XML fix for refactoring to mRemoteNG
            if (persistString.StartsWith("mRemote."))
                persistString = persistString.Replace("mRemote.", "mRemoteNG.");

            try
            {
                if (persistString == typeof(ConfigWindow).ToString())
                    return Windows.ConfigForm;

                if (persistString == typeof(ConnectionTreeWindow).ToString())
                    return Windows.TreeForm;

                if (persistString == typeof(ErrorAndInfoWindow).ToString())
                    return Windows.ErrorsForm;

                if (persistString == typeof(ScreenshotManagerWindow).ToString())
                    return Windows.ScreenshotForm;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("GetContentFromPersistString failed" + Environment.NewLine + ex.Message);
            }

            return null;
        }

        private void CreatePanels()
        {
            Windows.ConfigForm = new ConfigWindow();
            Windows.TreeForm = new ConnectionTreeWindow();
            Windows.ErrorsForm = new ErrorAndInfoWindow();

            Windows.ScreenshotForm = new ScreenshotManagerWindow(Windows.ScreenshotForm);
            Windows.ScreenshotForm = Windows.ScreenshotForm;

            Windows.UpdateForm = new UpdateWindow(Windows.UpdatePanel);
            Windows.UpdatePanel = Windows.UpdateForm;
        }
    }
}