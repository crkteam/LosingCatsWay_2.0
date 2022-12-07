﻿// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Editor.EditorUI.Windows.Internal;
using UnityEditor;

namespace Doozy.Editor.EditorUI.Windows
{
    public class EditorLayoutsWindow : EditorUIDatabaseWindow<EditorLayoutsWindow>
    {
        private const string WINDOW_TITLE = "Editor Layouts";

        [MenuItem(EditorUIWindow.k_WindowMenuPath + "/" + WINDOW_TITLE + "/Window", priority = 100)]
        private static void ShowWindow() => InternalOpenWindow(WINDOW_TITLE);

        [MenuItem(EditorUIWindow.k_WindowMenuPath + "/" + WINDOW_TITLE + "/Refresh")]
        private static void RefreshDatabase() => EditorDataLayoutDatabase.instance.RefreshDatabase();
    }
}
